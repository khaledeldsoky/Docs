---
name: claude-theme
description: >
  Generate dark-themed Arabic HTML documentation pages with Nagdy-inspired
  typography — Noto Sans Arabic for UI, Noto Naskh Arabic for prose, JetBrains
  Mono for code. Uses a formal typographic scale with CSS custom properties.
---

## Reference

Read `linux-admin-guide-new.html` — it is the canonical style reference for
this theme. Match its CSS variables, typographic scale, component classes,
layout, and visual language exactly.

## Design System

### Typography

| Token | Value | Usage |
|---|---|---|
| `--font-sans` | `'Noto Sans Arabic', sans-serif` | Headings, sidebar, nav, UI text, section titles, tables |
| `--font-serif` | `'Noto Naskh Arabic', serif` | Body prose, paragraphs, reading content |
| `--font-mono` | `'JetBrains Mono', monospace` | Code blocks, inline code, technical labels |
| `--text-body` | `1.125rem` (18px) | Base body size |
| `--text-body--line-height` | `1.8` | Body line height (Arabic) |
| `--text-prose` | `1.0625rem` (17px) | Prose paragraph size |
| `--text-prose--line-height` | `1.9` | Prose line height (Arabic, generous) |
| `--text-h1` | `2rem` (32px) | Cover / page title |
| `--text-h1--line-height` | `1.2` | H1 line height |
| `--text-h2` | `1.5rem` (24px) | Section title |
| `--text-h2--line-height` | `1.35` | H2 line height |
| `--text-h3` | `1.25rem` (20px) | Subsection title |
| `--text-h3--line-height` | `1.5` | H3 line height |
| `--text-code` | `0.8125rem` (13px) | Code block font size |
| `--text-code--line-height` | `1.6` | Code line height |
| `--text-body-sm` | `0.875rem` (14px) | Small body / metadata |
| `--text-body-sm--line-height` | `1.7` | Small body line height |
| `--text-small` | `0.8125rem` (13px) | Tiny labels, footnotes |
| `--text-small--line-height` | `1.35` | Tiny line height |

### Arabic-Specific Rules

| Aspect | Rule |
|---|---|
| Heading `letter-spacing` | Always `0` (never use negative tracking for Arabic) |
| Body font | Use `--font-serif` (Noto Naskh Arabic) for readability |
| UI font | Use `--font-sans` (Noto Sans Arabic) for headings, nav, sidebar |
| Paragraph class | `<p class="prose">` — applies serif font, 1.9 line-height |
| Paragraph color | `var(--text-dim)` — slightly muted for reading comfort |
| Code direction | `<code>` / `<pre>` must be `direction: ltr; unicode-bidi: isolate;` |
| Sidebar | DOM order: `<main>` first, `<aside class="sidebar">` after |

### Colors (Dark Palette)

| Variable | Hex | Usage |
|---|---|---|
| `--bg` | `#0a0d12` | Page background |
| `--surface` | `#0f1318` | Sidebar, card bases |
| `--surface2` | `#141a22` | Hover, table headers, card backgrounds |
| `--surface3` | `#192030` | Inline code background |
| `--border` | `#222c40` | Section dividers, card borders (slightly blue-tinted for separation) |
| `--border2` | `#2a3850` | Secondary borders |
| `--accent` | `#4f8ef7` | Links, active indicators, buttons |
| `--accent2` | `#00c2a8` | Teal accent (cover highlight, subsection bars) |
| `--accent3` | `#f0a500` | Amber accent (placeholders, warnings) |
| `--accent-red` | `#e85555` | Danger callouts, errors |
| `--accent-green` | `#22c97a` | Verify blocks, success states |
| `--text` | `#dce6f5` | Primary text |
| `--text-dim` | `#8899b5` | Body prose, muted text |
| `--text-muted` | `#4d607a` | Labels, footnotes, metadata |
| `--code-bg` | `#080b0f` | Code block background (darkest surface) |
| `--code-border` | `#1a2233` | Code block border |
| `--heavy1` / `--heavy1-accent` | `#1a2f4a` / `#4f8ef7` | Node-1 surface/accent |
| `--heavy2` / `--heavy2-accent` | `#1a3a2a` / `#22c97a` | Node-2 surface/accent |
| `--heavy3` / `--heavy3-accent` | `#2a1f3a` / `#b87fff` | Node-3 surface/accent |
| `--tag-h1-bg` / `--tag-h1-fg` | `#1a2f4a` / `#7ab8ff` | H1 tag bg/fg |
| `--tag-h2-bg` / `--tag-h2-fg` | `#0f2a1a` / `#4de89a` | H2 tag bg/fg |
| `--tag-h3-bg` / `--tag-h3-fg` | `#221840` / `#c89fff` | H3 tag bg/fg |
| `--tag-all-bg` / `--tag-all-fg` | `#1a1a2e` / `#a0a8e8` | All-nodes tag bg/fg |

Do not invent new color variables — reuse the existing ones. All non-variable
hardcoded colors (syntax spans, callouts, chips, `.cb-head`, `.benefit-card.danger`)
have `.light` overrides if light mode is added (see `html-docs` skill for the
toggle pattern).

### Google Fonts Import

```css
@import url('https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600;700&family=Noto+Naskh+Arabic:wght@400;500;600;700&family=Noto+Sans+Arabic:wght@300;400;500;600;700&display=swap');
```

## Required Structure

Same as `html-docs` skill — see that skill for the full component reference.
Key structure:

```
<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>...</title>
  <style>  /* all CSS inline */  </style>
</head>
<body>
  <main class="main">
    <div class="cover">  ...  </div>
    <section class="section"> ... </section>
    ...
  </main>
  <aside class="sidebar">  ...  </aside>
</body>
</html>
```

## Components

See `html-docs` skill for complete component reference. The `claude-theme`
skill only overrides **typography**. Component HTML structure, classes, and
layout remain identical to `html-docs`.

### Key Differences from `html-docs`

1. **Body font**: `--font-serif` instead of Cairo
2. **Heading font**: `--font-sans` (Noto Sans Arabic) instead of Cairo
3. **Base size**: 16px instead of 14px
4. **Prose paragraphs**: Use `<p class="prose">` instead of inline `style="..."`
5. **Code font size**: `var(--text-code)` = 0.8125rem (13px) instead of 15px
6. **`letter-spacing: 0`** on all Arabic headings
7. **No `tracking-tight`** or negative letter-spacing — Arabic doesn't need it
8. **Inline style ban**: Never use inline `font-size`/`line-height`/`color` styles on `<p>` — always use `class="prose"`
9. **Step-list bullet**: CSS `content: "• "` instead of `"- "` for `.step-list li .desc::before`
10. **Q&A format**: Split question and answer with `<br>`, prefix answer with `<span class="punct">• </span>`
11. **Colon-list break**: Insert `<br>` after colon when introducing a multi-item list
12. **Info table spacing**: `padding: 12px 16px`, `font-size: var(--text-body-sm)`, `line-height: 1.8`
13. **Rem-only sizing**: Every `font-size` uses `var(--text-*)` — no hardcoded `px` in CSS (except icons). All sizes scale with browser font-size setting.
14. **Benefit-card code LTR**: Multi-word command `<code>` inside `.bc-body` gets `dir="ltr"` for robust bidi rendering.

### Prose Class

```css
.prose {
  font-family: var(--font-serif);
  font-size: var(--text-prose);
  line-height: var(--text-prose--line-height);
  color: var(--text-dim);
  margin-bottom: 1.15em;
}
```

Use `class="prose"` on all paragraph text, descriptions, and body copy that
is part of the reading content. Do NOT use inline `font-size` / `line-height`
styles on paragraphs.

### Active Nav Tracking

Include before `</body>`. Adds `.active` class to sidebar links based on
scroll position via `IntersectionObserver`:

```html
<script>
const sections = document.querySelectorAll('.section[id]');
const navLinks = document.querySelectorAll('.nav-item');
if (sections.length && navLinks.length) {
  const navMap = {};
  navLinks.forEach(a => {
    const id = a.getAttribute('href')?.replace('#', '');
    if (id) navMap[id] = a;
  });
  const obs = new IntersectionObserver(entries => {
    let best = null, bestRatio = 0;
    entries.forEach(e => {
      if (e.intersectionRatio > bestRatio) { best = e.target.id; bestRatio = e.intersectionRatio; }
    });
    navLinks.forEach(a => a.classList.remove('active'));
    if (best && navMap[best]) navMap[best].classList.add('active');
  }, { threshold: [0, 0.25, 0.5, 0.75, 1] });
  sections.forEach(s => obs.observe(s));
}
</script>
```

CSS: `.nav-item.active { color: var(--text); border-right-color: var(--accent); background: var(--surface2); }`

### Syntax Highlighting Colors

Token colors used in `<pre>` code blocks:

| Token | Color | Usage |
|---|---|---|
| `.cm` | `#7096b8` italic | Comments |
| `.kw` | `#ff7b72` | Keywords |
| `.val` | `#d2a8ff` | Values |
| `.str` | `#a5d6ff` | Strings |
| `.ph` | `var(--accent3)` | Placeholders |
| `code` | `#7dd3fc` | Inline code |

Code block header background: `#0e141e` (subtly lighter than `--code-bg`).

## Process

1. Read `linux-admin-guide-new.html` to absorb the exact style
2. Ask user for: document title, sections/content, and any per-node scoping
3. Generate a single self-contained `.html` file matching the conventions above
4. Read the file back and verify all tags are closed and classes match
