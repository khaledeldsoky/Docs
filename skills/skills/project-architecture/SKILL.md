---
name: project-architecture
description: Use for questions about overall structure, tech stack, Docker, backend API, database schema, deployment, and routing of the Sheet Cutting Optimizer.
---

# Sheet Cutting Optimizer — Project Architecture

## Tech Stack
- **Frontend**: Vite + React 19 + TypeScript + Tailwind CSS
- **Backend**: FastAPI (Python 3.12+), async SQLAlchemy + asyncpg
- **Database**: PostgreSQL
- **Visualization**: HTML5 Canvas (no libraries)
- **Export**: jsPDF (no `setAlpha` — removed entirely)
- **Containerization**: Docker Compose (db + backend + frontend)

## Project Structure
```
cutting/
├── docker-compose.yml          # db (postgres:15) + backend + frontend
├── backend/
│   ├── Dockerfile
│   ├── requirements.txt
│   └── app/
│       ├── main.py             # FastAPI app entry
│       ├── config.py           # DATABASE_URL from env (asyncpg)
│       ├── database.py         # create_async_engine + async_sessionmaker + async def get_db
│       ├── models/
│       │   └── project.py      # Project, Piece, CuttingPlan ORM models
│       ├── schemas/
│       │   └── plan.py         # Pydantic: PieceInput, OptimizeRequest/Response, ProjectCreate/Update/Detail
│       ├── api/
│       │   └── plans.py        # All REST endpoints (router prefix /api)
│       └── core/
│           ├── geometry.py     # Rect dataclass (fits, contains, overlaps, area, right, top)
│           ├── maxrects.py     # MaxRectsBin + GuillotineBin + LeftoverAnalysis
│           └── optimizer.py    # Orchestrator: routes algorithm, builds leftover schema
├── frontend/
│   ├── Dockerfile
│   ├── nginx.conf
│   ├── package.json
│   ├── index.html
│   └── src/
│       ├── App.tsx             # Root: state, layout, auto-save
│       ├── types/index.ts      # TypeScript interfaces
│       ├── services/api.ts     # API client (fetch wrapper)
│       ├── hooks/
│       │   └── useCuttingPlan.ts
│       ├── i18n/
│       │   ├── LocaleContext.tsx # n() formatter, Arabic-Indic digits
│       │   ├── en.ts
│       │   └── ar.ts
│       └── components/
│           ├── Input/
│           │   ├── SheetInput.tsx
│           │   └── PieceInput.tsx
│           ├── Visualization/
│           │   └── CanvasView.tsx
│           ├── Export/
│           │   └── ExportButton.tsx
│           └── ProjectBar.tsx
```

## API Endpoints (prefix `/api`)
| Method | Path | Purpose |
|--------|------|---------|
| POST | `/optimize` | Run cutting optimization |
| POST | `/projects` | Create project + pieces |
| GET | `/projects` | List projects |
| GET | `/projects/{id}` | Get project with pieces + latest plan |
| PUT | `/projects/{id}` | Update project (name, sheet, pieces) |
| DELETE | `/projects/{id}` | Delete project (cascades) |
| POST | `/projects/{id}/save-plan` | Save/update a cutting plan |

## Key Conventions
- Default unit: **cm** (all backend math in mm)
- cm→mm conversion: multiply by 10, round to int
- mm→cm display: divide by 10, `.toFixed(1)`
- Fmt helper in App.tsx: `const fmt = (v) => n(unit === 'cm' ? +(v/10).toFixed(1) : v)`
- Piece validation at `plans.py:39`: piece fits if `(w≤sw ∧ h≤sh) ∨ (h≤sw ∧ w≤sh)` (either orientation)
- All read-only numbers in Arabic mode use Arabic-Indic digits via `n()` formatter
- Auto-save plan when project loaded and plan changes (uses `autoSaving` ref to prevent loops)
- Backend uses **async SQLAlchemy** (`create_async_engine` + `async_sessionmaker` + `async def get_db`)
- No algorithm selector/display in UI (removed per user request)
- `selectinload` needed for Project.pieces relationship when loading full project
- Timezone-naive DateTime columns in ORM (no timezone in DB)
- **Drag & Drop**: Added manual piece repositioning on canvas after auto-pack optimization
  - `CanvasView.tsx` supports drag & drop with grid snapping
  - `App.tsx` manages drag state and updates plan on drag end
  - Pieces can be dragged within sheet boundaries with visual feedback

## Component Architecture
- **SheetLayoutView**: Interactive SVG component
  - DOM-based rendering with drag & drop support
  - Detects pieces under mouse cursor
  - Provides visual feedback during drag operations
  - Constrains pieces to sheet boundaries
  - Emits drag events to parent component

- **App**: Root component with state management
  - Manages drag state and plan updates
  - Handles user interactions
  - Integrates drag & drop with existing features

- **useCuttingPlan Hook**: Custom hook for cutting plan state
  - Manages optimization state
  - Provides drag & drop integration
  - Handles plan updates

## Docker
- `docker compose up -d` to start
- `docker compose build backend` required for backend code changes (no volume mount for backend)
- Frontend auto-reloads in dev via Vite; Docker only needed for production build
- `docker compose down -v` removes volumes (deletes DB data)
