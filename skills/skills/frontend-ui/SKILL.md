---
name: frontend-ui
description: Use for questions about frontend React components, Canvas visualization, i18n/LocaleContext, state management in App.tsx, piece editing, and styling conventions.
---

# Frontend UI

## State Management (All in `App.tsx`)
```typescript
const [pieces, setPieces] = useState<PieceInputType[]>([])
const [editingIndex, setEditingIndex] = useState<number | null>(null)
const [sheetWidth, setSheetWidth] = useState(2440)
const [sheetHeight, setSheetHeight] = useState(1220)
const [unit, setUnit] = useState<'mm' | 'cm'>('cm')
const [activeSheet, setActiveSheet] = useState(0)
const [currentProjectId, setCurrentProjectId] = useState<string | null>(null)
const [saveName, setSaveName] = useState('')
```
- `useCuttingPlan()` hook: exposes `{ plan, loading, error, optimize, clear, setPlan }`
- Clearing plan on piece/sheet changes: `clear()` is called in `handleRemovePiece`, `handleUpdatePiece`, `handleOptimize` (via sheet dims change)

### Piece Operations
- **Add**: `handleAddPiece(p)` — appends to pieces[]
- **Edit**: click ✎ button → sets `editingIndex` → PieceInput populates fields → button shows "Update" → calls `handleUpdatePiece` → replaces at index, clears editingIndex
- **Delete**: `handleRemovePiece(i)` — filters out, calls `clear()`

## Components

### `PieceInput.tsx`
Props: `onAdd`, `onUpdate?`, `editingPiece?`, `unit`
- Add mode: submits via `onAdd`, clears fields
- Edit mode: `useEffect` populates fields from `editingPiece` (converts mm→cm for display), submits via `onUpdate`, clears
- Button text: `{t.piece.add}` or `{t.piece.update}`

### `SheetInput.tsx`
Props: `defaultWidth`, `defaultHeight`, `unit`, `onDimensionsChange`
- Converts cm→mm in callback
- Calls `clear()` on dimension change

### `SheetLayoutView.tsx`
  - **SVG-based layout** with DOM elements for better performance
  - Key responsibilities:
    - Draws sheet as white rectangle with thin border
    - Draws each placed piece as filled colored rect
    - Draws leftover outlines in grey (no background fill)
  - **Label rendering**:
    - **Font**: `max(12, min(pw*0.12, ph*0.12, 15))` — bold, clamped 12–15 canvas units
    - **Color**: dark charcoal `#1C1C1C` for high contrast on all backgrounds
    - **Width label**: top edge, `X = px + pw/2`, `Y = py + 6` (fixed 6px from border). `textAlign='middle'`, `dominantBaseline='hanging'`
    - **Height label**: left edge, `translate(px + 6, py + ph/2)`, rotated −90°. `textAnchor='middle'`, `dominantBaseline='central'`
    - **Guardrail for tiny strips**: if `pw < fs+4 || ph < fs+4`, falls back to single combined string `"W×H"` dead-center. Hidden completely if still too small.
    - **Background**: `color + '88'` (53% opacity)
  - Zoom controls: `-`, `+`, `Fit` buttons
  - Canvas scales to fit container while maintaining aspect ratio
  - Uses `useLocale()` for `n()` (Arabic-Indic digit formatting)



### `ExportButton.tsx`
- Uses `jspdf` library (v2.5.2)
- Exports current sheet view as PDF
- No `setAlpha` — this method doesn't exist in jspdf 2.5.2

### `ProjectBar.tsx`
- Dropdown to list/load/delete projects
- Delete button (✕) appears on hover over each project row

## i18n (`LocaleContext.tsx`)
```typescript
const { t, locale, setLocale, dir, n } = useLocale()
```
- `t` — translation object (en | ar)
- `locale` — `'en'` | `'ar'`
- `dir` — `'ltr'` | `'rtl'`
- `n(num)` — formats number; in Arabic uses Arabic-Indic digits (٠١٢٣٤٥٦٧٨٩) via `toLocaleString('ar-SA')`
- Translation files: `en.ts` (source of truth for type) and `ar.ts`
- Keys: `t.app.*`, `t.unit.*`, `t.sheet.*`, `t.piece.*`, `t.canvas.*`, `t.results.*`, `t.export_.*`

## Layout Structure
```
Header (title, ProjectBar, lang/unit toggles, New button)
└── Body (flex row gap-4)
    ├── Sidebar (w-72, scroll)
    │   └── Card: SheetInput + PieceInput + Piece List + Optimize button
    └── Main (flex-1)
        └── Empty state (if !plan) OR
            └── Flex col: Sheet tabs + Canvas + Stats/Save bar
```

## Styling Conventions
- Tailwind CSS utility classes
- Color scheme: gray-100 bg, white cards with gray-200 borders, blue-600 primary buttons
- Rounded: `rounded-xl` on cards, `rounded-lg` on buttons/inputs
- Sidebar: `w-72 shrink-0`
- Canvas section: `min-h-0 flex flex-col` for proper flex shrink

## TypeScript Interfaces (`types/index.ts`)
- `PieceInput { width: number, height: number, quantity: number }`
- `Placement { piece_index, x, y, width, height, rotated, label }`
- `SheetPlan { sheet_index, placements, used_area, waste_area, waste_percentage, leftovers? }`
- `OptimizeResponse { sheets, total_sheets, total_area, used_area, waste_area, waste_percentage, algorithm, leftovers? }`
- `Project { id, name, sheet_width, sheet_height, created_at, updated_at, pieces?, plan? }`

## Important Gotchas
- `jsPDF` 2.5.2 has no `setAlpha` — do not import it
- All read-only numbers use `n()` formatter (Arabic-Indic in ar locale)
- Canvas dimension labels use placed dimensions `pw`/`ph`, not original `w`/`h`
- Piece validation in backend checks BOTH orientations
- Editing a piece calls `clear()` (invalidates the plan)
- Loaded projects auto-save the plan on any plan change (via useEffect with autoSaving ref guard)
