---
name: hr-system
description: Use when working on the HR management system (ASP.NET Core backend + React frontend). Covers attendance, leave, mission, document management, employee management, and approval workflows.
---

# HR Management System

## Project Structure

```
login-app/          React frontend (Vite)
  src/components/
    Dashboard/      Main dashboard components
      Admin/        HR admin components (EmployeeList, etc.)
    Toast.jsx       Toast notification system
  FEATURE_TRACKER.md
  updated_future.md

login-api/          .NET 8 C# backend (ASP.NET Core Web API)
  Controllers/      API controllers
  Models/           Data models & DTOs
  Data/             EF Core DbContext
  Services/         Business logic (RoleHelper, AuditService)
  database/         SQL migration & seed scripts
```

## Common Commands

**Frontend (login-app/):**
- `npm run dev` — start dev server (port 5173)
- `npm run build` — production build

**Backend (login-api/):**
- Restart: `kill $(pgrep -f 'dotnet run') ; tmux kill-session -t backend ; sleep 2 ; tmux new-session -d -s backend 'export PATH=$HOME/.dotnet:$PATH && cd /home/khaled/app/login-api && dotnet run'`

## Conventions

- **API base URL:** `http://localhost:5000`
- **DB:** PostgreSQL via EF Core
- **Employee ID** is passed as query param `employeeId`
- **Auth:** Simple employeeCode + password login
- **CSS:** Inline styles in JSX for one-offs, `App.css` for all global/shared styles
- **Dark mode:** Uses `[data-theme="dark"]` selector with CSS variables
- **Toast notifications:** Import `{ toast }` from `'../Toast'` — `.success()`, `.error()`, `.info()`
- **Leave approval chain:** Supervisor → HR (multi-step)
- **Mission approval:** Single-step (supervisor or HR)
- **Attendance:** 8 AM – 6 PM window, 8h standard, check-in/check-out with status (Present/Late/Absent/Leave/Mission)

## Key Files

- `opencode.json` — project opencode config
- `AGENTS.md` — project instructions
- `FEATURE_TRACKER.md` — feature history
- `updated_future.md` — future roadmap

> For feature suggestions and task documentation see the `hr-project-tracker` skill.
