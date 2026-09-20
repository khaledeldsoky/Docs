---
name: docs-theme
description: >
  Design system and eye-comfort theme for a documentation website with
  dark/light mode, English/Arabic RTL support, accessible typography,
  and harmonious color palettes.
---

## Design Principles

- **Eye comfort** — muted text for body copy, generous line-height (1.7+), sufficient contrast (WCAG AA minimum 4.5:1 for body, 3:1 for large text)
- **Reduced visual noise** — subtle borders, no harsh shadows, soft color transitions
- **RTL-first** — design for Arabic layout from the start, then adjust for English
- **Responsive scale** — everything in `rem`, respect user font-size preferences

## Color Palettes

Define colors as CSS custom properties on `:root` and toggle via a `[data-theme="dark"]` or `.dark` class on `<html>`.

### Light Mode

| Token | Hex | Usage |
|---|---|---|
| `--bg` | `#f8f9fa` | Page background |
| `--surface` | `#ffffff` | Card, sidebar, component surfaces |
| `--surface2` | `#f0f1f3` | Hover states, secondary surfaces |
| `--surface3` | `#e8eaed` | Inline code background |
| `--border` | `#dde0e4` | Borders, dividers |
| `--border2` | `#c8ccd2` | Stronger borders, focus rings |
| `--text` | `#1a1d23` | Primary text (headings, nav) |
| `--text-dim` | `#4a5060` | Body prose, secondary text |
| `--text-muted` | `#808898` | Labels, footnotes, metadata |
| `--accent` | `#3b82f6` | Links, active indicators, buttons |
| `--accent2` | `#059669` | Success states, secondary accent |
| `--accent-red` | `#dc2626` | Danger, errors |
| `--accent-amber` | `#d97706` | Warnings, placeholders |

### Dark Mode

| Token | Hex | Usage |
|---|---|---|
| `--bg` | `#0a0d12` | Page background |
| `--surface` | `#0f1318` | Card, sidebar, component surfaces |
| `--surface2` | `#141a22` | Hover states, secondary surfaces |
| `--surface3` | `#192030` | Inline code background |
| `--border` | `#222c40` | Borders, dividers |
| `--border2` | `#2a3850` | Stronger borders, focus rings |
| `--text` | `#dce6f5` | Primary text (headings, nav) |
| `--text-dim` | `#8899b5` | Body prose, secondary text |
| `--text-muted` | `#4d607a` | Labels, footnotes, metadata |
| `--accent` | `#4f8ef7` | Links, active indicators, buttons |
| `--accent2` | `#22c97a` | Success states, secondary accent |
| `--accent-red` | `#e85555` | Danger, errors |
| `--accent-amber` | `#f0a500` | Warnings, placeholders |

### Code Syntax Colors (Dark)

| Token | Color | Element |
|---|---|---|
| `--code-bg` | `#080b0f` | Code block background |
| `--code-border` | `#1a2233` | Code block border |
| `--syn-comment` | `#7096b8` italic | Comments |
| `--syn-keyword` | `#ff7b72` | Keywords |
| `--syn-value` | `#d2a8ff` | Values |
| `--syn-string` | `#a5d6ff` | Strings |
| `--syn-placeholder` | `var(--accent-amber)` | Placeholders |
| `--code-inline` | `#7dd3fc` | Inline `<code>` |

For light mode, adjust syntax colors to be slightly darker (e.g. `--syn-comment: #5a7a99`).

## Typography Scale

All sizes in `rem`, no hardcoded `px` in CSS (except icons).

| Token | Value (rem) | ~px | Usage |
|---|---|---|---|
| `--text-h1` | `2rem` | 32px | Page title / cover |
| `--text-h2` | `1.5rem` | 24px | Section titles |
| `--text-h3` | `1.25rem` | 20px | Subsection titles |
| `--text-body` | `1.0625rem` | 17px | Body prose |
| `--text-body-sm` | `0.875rem` | 14px | Sidebar nav, small body |
| `--text-code` | `0.8125rem` | 13px | Code blocks |
| `--text-small` | `0.75rem` | 12px | Labels, footnotes, metadata |

### Font Stacks

| Role | Font | Fallback |
|---|---|---|
| UI / Headings | `'Inter', 'Noto Sans Arabic'` | `sans-serif` |
| Body prose | `'Inter', 'Noto Naskh Arabic'` | `serif` |
| Code | `'JetBrains Mono', 'IBM Plex Mono'` | `monospace` |

### Typography Rules

- Body line-height: `1.7` – `1.9` (Arabic needs more generous leading)
- Code line-height: `1.5` – `1.6`
- Heading letter-spacing: `-0.02em` for Latin, `0` for Arabic
- Maximum text measure: `65ch` – `75ch` for body paragraphs
- Body color: `var(--text-dim)` (not pure black/white) to reduce eye strain
- Headings: `var(--text)` for maximum readability

## Dark/Light Mode Implementation

```css
:root {
  /* light mode variables */
  --bg: #f8f9fa;
  --text: #1a1d23;
  /* ... */
}

html.dark {
  --bg: #0a0d12;
  --text: #dce6f5;
  /* ... */
}
```

Toggle pattern in React:

```tsx
import { useTheme } from 'next-themes'

function ThemeToggle() {
  const { theme, setTheme } = useTheme()
  return (
    <button onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}>
      {theme === 'dark' ? '☀️' : '🌙'}
    </button>
  )
}
```

Wrap the app in `<ThemeProvider attribute="class" defaultTheme="system" enableSystem>` and use `class` mode so Tailwind's `dark:` variant works.

## Spacing

Use a consistent 4px or 8px grid:

| Token | Value | Usage |
|---|---|---|
| `--space-xs` | `0.25rem` (4px) | Tight gaps |
| `--space-sm` | `0.5rem` (8px) | Related items |
| `--space-md` | `1rem` (16px) | Default spacing |
| `--space-lg` | `1.5rem` (24px) | Section gaps |
| `--space-xl` | `2rem` (32px) | Major sections |
| `--space-2xl` | `3rem` (48px) | Page sections |

## RTL Layout Rules

- Use CSS logical properties (`margin-inline-start`, `padding-inline-end`, `inset-inline-start`) instead of `margin-left`/`margin-right`
- Set `dir="rtl"` on `<html>` for Arabic pages
- Sidebar appears on the right in RTL mode — place `<aside>` after `<main>` in DOM order and use `order` or flexbox `row-reverse`
- Code blocks and inline `<code>` must be `direction: ltr; unicode-bidi: isolate;` even in RTL mode
- All heading `letter-spacing` must be `0` for Arabic (Arabic doesn't use letter-spacing)
- Google Fonts import for Arabic:
  ```css
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=JetBrains+Mono:wght@400;600;700&family=Noto+Naskh+Arabic:wght@400;500;600;700&family=Noto+Sans+Arabic:wght@300;400;500;600;700&display=swap');
  ```

## Accessibility

- Focus rings: `outline: 2px solid var(--accent); outline-offset: 2px`
- Skip-to-content link as first focusable element
- Color combinations must pass WCAG AA (4.5:1 for text, 3:1 for large text)
- Never convey information through color alone
- Interactive elements minimum target size: 44×44px
- Support `prefers-reduced-motion`
