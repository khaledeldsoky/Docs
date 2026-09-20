---
name: frontend-conventions
description: React/Vite frontend code conventions, CSS variable system, component patterns, API call patterns, and toast usage. Load when writing or editing any frontend code.
---

# Frontend Code Conventions

## Project Location
`/home/khaled/app/login-app/`

## CSS Variable System (App.css)

### Theme Variables (lines 3–81)
Always use these instead of hardcoded values:

| Category | Variables | Example |
|---|---|---|
| **Font sizes** | `--font-xs` through `--font-xxl`, `--font-heading` | `font-size: var(--font-base)` |
| **Border radius** | `--radius-sm` (4px) to `--radius-xl` (12px) | `border-radius: var(--radius-lg)` |
| **Gaps** | `--gap-xs` to `--gap-xl` | `gap: var(--gap-lg)` |
| **Paddings** | `--pad-xs` to `--pad-xxl` | `padding: var(--pad-xl)` |
| **Shadows** | `--shadow-card`, `--shadow-sm`, `--shadow-dropdown`, `--shadow-focus` | `box-shadow: var(--shadow-card)` |
| **Backgrounds** | `--bg-body`, `--bg-card`, `--bg-input`, `--bg-hover`, `--bg-light`, `--bg-table-header`, `--bg-table-hover`, `--bg-table-hover-employee`, `--bg-legend`, `--bg-tag-info` | `background: var(--bg-card)` |
| **Text** | `--text-primary`, `--text-secondary`, `--text-muted`, `--text-light` | `color: var(--text-primary)` |
| **Borders** | `--border-color`, `--border-input`, `--cell-border`, `--cell-border-light` | `border: 1px solid var(--border-color)` |
| **Brand** | `--primary`, `--primary-hover`, `--primary-disabled`, `--danger`, `--success`, `--warning` | `background: var(--primary)` |
| **Alerts** | `--alert-{type}-bg`, `--alert-{type}-text` (type: info/success/warning/danger) | `background: var(--alert-warning-bg); color: var(--alert-warning-text)` |
| **Status colors** | `--status-present`, `--status-late`, `--status-absent`, `--status-leave`, `--status-mission`, `--status-holiday` | Used in JSX inline styles in TeamCalendar.jsx |

### Dark Mode
- All variables auto-switch when `[data-theme="dark"]` is set on `<html>`.
- Never hardcode dark mode colors — always use variables.
- To add a dark mode override, add to the `[data-theme="dark"]` block in App.css.

## Imports Pattern

```jsx
import { useState, useEffect } from 'react'
import { toast } from '../Toast'     // toast.success/error/info
import { ToastContainer } from '../Toast'  // place once in app
```

- API constants: `const API = 'http://localhost:5000'` at top of component file.
- Sidebar/Navbar: `import Sidebar from './Sidebar'`, `import Navbar from './Navbar'`
- Skeleton loading: `import { TableSkeleton, ProfileSkeleton } from './Skeleton'`

## Component Patterns

### Props
```jsx
function ComponentName({ user, onLogout, ...otherProps }) {
```

### State
```jsx
const [data, setData] = useState(null)
const [loading, setLoading] = useState(true)
const [error, setError] = useState('')
```

### API Calls
```jsx
useEffect(() => {
  const fetchData = async () => {
    setLoading(true)
    try {
      const res = await fetch(`${API}/api/endpoint?employeeId=${user?.employeeId || ''}`)
      if (res.ok) setData(await res.json())
      else toast.error('Failed to load data')
    } catch { /* ignore network errors */ }
    finally { setLoading(false) }
  }
  fetchData()
}, [user?.employeeId])
```

Key patterns:
- `employeeId` is always passed as a query parameter
- Loading state: show `<div className="loading-spinner">` or skeleton
- Error state: show message in red
- Try/catch with empty catch block for network errors
- `finally { setLoading(false) }`

### Toast Notifications
```jsx
toast.success('Operation completed')   // green
toast.error('Something went wrong')    // red
toast.info('New notification')         // blue
// Optional second param: duration in ms (default 3500)
toast.success('Saved!', 2000)
```

Place `<ToastContainer />` once in the layout (ProfilePage.jsx or App.jsx already has it).

## CSS Placement

| Scope | Location | Notes |
|---|---|---|
| **Global/shared styles** | `App.css` | Use CSS variables |
| **One-off / dynamic** | Inline `style={{ }}` in JSX | For truly unique values only |
| **Component-scoped** | Add class in JSX + rule in App.css | Preferred over inline |

## Routing
- `main.jsx` wraps `<App />` in `<BrowserRouter>`
- Use `useNavigate` for programmatic navigation
- Route params / query params via React Router

## Loading States
- `<TableSkeleton />` — for table/list content
- `<ProfileSkeleton />` — for profile page
- `<div className="loading-spinner" style={{ textAlign: 'center', padding: '3rem' }}>Loading...</div>` — generic fallback
