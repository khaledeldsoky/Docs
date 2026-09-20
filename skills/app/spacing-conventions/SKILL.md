# Skill: spacing-conventions

## When to Use
Load this skill when working on CSS, adding new components, or fixing layout/spacing issues. Follow these rules to maintain consistent spacing across all components.

## Spacing Variables (`App.css`)

All spacing uses a single scale defined in `:root`. Use `var(--space-*)` everywhere — never hardcode `px` or `rem` values for margins, paddings, or gaps.

```
--space-xxs: 0.25rem  (4px)  — tiny gaps (icon-text, badge padding)
--space-xs:  0.5rem   (8px)  — button padding, small gaps
--space-sm:  0.75rem  (12px) — form field padding, small card padding
--space-md:  1rem     (16px) — default spacing, form-group margin
--space-lg:  1.5rem   (24px) — card padding, section spacing
--space-xl:  2rem     (32px) — large section padding, modal padding
--space-xxl: 3rem     (48px) — page-level margins, hero spacing
```

## Application Rules

### 1. Container Padding (Internal Spacing)
| Component | Padding |
|---|---|
| Cards (`.card`, `.table-container`, `.login-form`) | `var(--space-xl)` |
| Form cards (`.add-employee-form`, `.add-department-form`) | `var(--space-lg)` |
| Compact cards (`.team-calendar-controls`) | `var(--space-md) var(--space-lg)` |
| Modals | `var(--space-xl)` |
| Table cells (`.employee-table td, .employee-table th`) | `var(--space-xs) var(--space-sm)` |
| `.btn` | `var(--space-xs) var(--space-sm)` |
| `.btn-sm` | `var(--space-xxs) var(--space-xs)` |
| Form inputs (`.form-input, .form-select, .search-input`) | `var(--space-xs) var(--space-sm)` |
| `.form-group` → `margin-bottom` | `var(--space-md)` |
| `.form-group label` → `margin-bottom` | `var(--space-xxs)` |

### 2. External Margins (Spacing Between Elements)
- **Between major sections** → use `margin-bottom: var(--space-xl)` on the lower element
- **Between form rows** → `margin-bottom: var(--space-md)`
- **Between buttons** in a flex row → use `gap: var(--space-sm)`
- **Between filter/action toolbar items** → `gap: var(--space-sm)`

### 3. Prefer `gap` Over `margin` in Flex/Grid Layouts
```css
/* ✅ Correct: consistent gap, no double-margin issues */
display: flex;
gap: var(--space-sm);

/* ❌ Avoid: children need margin + first/last child hacks */
display: flex;
.child { margin-right: var(--space-sm); }
```

### 4. Typography
| Element | Margin |
|---|---|
| `h1` → `margin-bottom` | `var(--space-lg)` |
| `h2` → `margin-bottom` | `var(--space-md)` |
| `.form-section-title` → `margin-bottom` | `var(--space-sm)` |
| `.form-help, .form-hint` → `margin-top` | `var(--space-xxs)` |

### 5. Header + Content Pattern
- `.calendar-header` / section headers → `margin-bottom: var(--space-sm)`
- `.admin-toolbar` → `margin-bottom: var(--space-lg)`; `gap: var(--space-sm)`
- `.employee-filters` → `margin-bottom: var(--space-md)`; `gap: var(--space-xs)`

### 6. What NOT to Do
- ❌ Never hardcode `px` or raw `rem` values. Always use `var(--space-*)`.
- ❌ Never use `margin` on flex/grid children when `gap` can be used on the parent.
- ❌ Never define spacing on both parent (`gap`) AND children (`margin`) — pick one.
- ❌ Never use different spacing variables for the same role (e.g., don't use `--space-sm` on one card and `--space-md` on another for the same purpose).
- ❌ Don't nest margins — prefer `gap` on the parent container.

### 7. Mapping Old → New (Refactored)
| Old variable | New variable |
|---|---|
| `--gap-xs` / `--pad-xs` | `--space-xxs` |
| `--gap-sm` / `--pad-sm` | `--space-xs` |
| `--gap-md` / `--pad-md` | `--space-sm` |
| `--gap-lg` / `--pad-lg` | `--space-md` |
| `--gap-xl` / `--pad-xl` | `--space-lg` |
| — | `--space-xl` (was `--pad-xxl`) |
| — | `--space-xxl` (new) |

## How to Verify
- Load CSS in browser, inspect elements, confirm `var(--space-*)` is used
- Check no raw `margin:` or `padding:` values exist in component styles (except `0`)
- Run `npm run build` to confirm no CSS errors
