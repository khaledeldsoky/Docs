---
name: html-docs
description: >
  Generate dark-themed HTML documentation pages with sidebar navigation, code
  blocks (macOS-style dots), callouts, info tables, verification blocks,
  benefit cards, and step lists. For infrastructure, storage, platform, or
  ops-style documentation.
---

## Reference File

Read `linux-admin-guide-new.html` first — it is the canonical
style reference for Arabic RTL docs. Match its CSS variables, component
classes, layout, typography tokens, and visual language exactly.

## Required Structure

```
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>...</title>
  <!-- all CSS inline in <style> — no external files -->
</head>
<body>
  <aside class="sidebar">  ...  </aside>
  <main class="main">
    <div class="cover">    ...  </div>   <!-- always required -->
    <section class="section"> ... </section>  <!-- one per phase/topic -->
    ...
  </main>
</body>
</html>
```

## Components

| Component | HTML Pattern | Notes |
|---|---|---|
| **Cover** | `.cover > .breadcrumb + h1 (+ .hl span) + .sub + .chip-row` | Top of every page |
| **Section** | `section.section#id > .section-header > .section-num + h2.section-title` | `.section-num` can be number or icon |
| **Subsection** | `.subsection > .sub-title` | Green accent bar via `::before` |
| **Node tag** | `span.node-tag.tag-h1` / `.tag-h2` / `.tag-h3` / `.tag-all` | Colored per-node scoping |
| **Code block** | `.code-block` (opt `.h1/.h2/.h3`) > `.cb-head` (`.cb-label` + `.cb-dots`) + `pre` | Syntax spans: `.cm` (comment), `.kw` (keyword), `.val` (value), `.str` (string), `.ph` (placeholder) |
| **Callout** | `.callout.info/.warn/.danger/.success` > `.callout-icon` + `div` | Icon + rich text |
| **Verify** | `.verify` > `.v-label` + `p` | Green-bordered verification box (always include expected output) |
| **Benefit card** | `.benefit-grid` > `.benefit-card` (opt `.danger`) > `.bc-icon` + `.bc-title` + `.bc-body` | 2-column grid |
| **Step list** | `ol.step-list` > `li > div > strong + span.desc` | Auto-numbered via CSS counter; title in `<strong>` (accent blue, own line, 13.5px), description in `<span class="desc">` (13px, indented with `• ` prefix on next line) |
| **Step list CSS** | `.step-list li > div { flex:1 }` + `.step-list li strong { display:block; color:var(--accent); font-size:13.5px }` + `.step-list li .desc { display:block; font-size:13px; line-height:1.7 }` + `.step-list li .desc::before { content:"• "; color:var(--text-muted) }` | Forces title on its own line above description |
| **Info table** | `table.info-table` > `thead > tr > th` + `tbody > tr > td` | Hover highlight; cells: padding 12px 16px, font-size 14px, line-height 1.8 |
| **Verify block** | `.verify` > `.v-label` + `p` — also `.mono-text` for inline code blocks, `.mono-table` for inner tables | Green-bordered verification box
| **Delta table** | `table.delta-table` > td `.added` / `.removed` | For before/after comparisons |
| **Arch diagram** | `div.arch-diagram` with ASCII art + inline spans for color | `white-space: pre` |
| **Chip** | `span.chip.chip-green/.chip-blue/.chip-amber/.chip-red` | In `.chip-row` |
| **Divider** | `hr.divider` | |
| **Inline code** | `<code>` | JetBrains Mono, blue tint |

## RTL / Arabic Docs

When generating **Arabic (RTL)** documentation:

| Aspect | Rule |
|---|---|
| HTML | `<html lang="ar" dir="rtl">` |
| Sidebar | Place `<aside class="sidebar">` **after** `<main>` in DOM; it renders on the right |
| Text align | All body text is `text-align: right;` |
| Fonts load | `@import url('https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600;700&family=Noto+Naskh+Arabic:wght@400;500;600;700&family=Noto+Sans+Arabic:wght@300;400;500;600;700&display=swap');` |
| UI / nav font | `Noto Sans Arabic` (`--font-sans`) — for sidebar, headings, labels |
| Prose body font | `Noto Naskh Arabic` (`--font-serif`) — for paragraph body text (more readable at 16px) |
| Code font | `JetBrains Mono` (`--font-mono`) for `<code>` / `<pre>` |
| Inline code | `code { direction: ltr; unicode-bidi: isolate; }` — keeps code LTR in RTL text |
| Code blocks | `.code-block pre { direction: ltr; }` — entire block LTR |
| Comments | `.cm { unicode-bidi: plaintext; }` — each comment line auto-detects direction |
| `.cm` with Arabic | Lines whose **first strong character** is Arabic (e.g. `# زود مسار`) → leave without `dir` — `plaintext` renders them RTL, parentheses appear correctly |
| `.cm` with English | Lines whose **first strong character** is Latin / digit (e.g. `# Copy`, `# Symlink`, `# 2592000`) → add `dir="ltr"` to prevent flipped brackets from RTL bidi |
| CSS for LTR override | `.code-block pre .cm[dir="ltr"] { unicode-bidi: embed; direction: ltr; }` |
| `letter-spacing: 0` | All heading, nav, and sidebar text must have `letter-spacing: 0` (Arabic doesn't use letter-spacing) |
| Verify p[dir="ltr"] | Pure-English `<p>` inside `.verify` blocks gets `dir="ltr"` + CSS `.verify p[dir="ltr"] { text-align: left; }` |
| Q&A line break | Question + answer in callout/verify → split with `<br>` after question, prefix answer with `<span class="punct">• </span>` |
| Colon-list break | When a colon introduces a multi-item list, insert `<br>` after the colon to start list on new line |

**Determining first strong character:** Look at the first non-`# ` character of a comment line:
- Arabic letter (ا, ب, ت, ز, ف, ك, م, إ, etc.) → Arabic → no `dir`
- Latin letter (a, b, c, C, S, X, E, etc.) or digit (2, 5, 8) → LTR → add `dir="ltr"`

## Punctuation Readability (Arabic)

Wrap punctuation marks in **bold spans** with spaces for visual clarity in Arabic text:
- CSS: `.punct { font-weight: 700; }`
- Wrap each punctuation char as `<span class="punct"> , </span>` (space + char + space)
- **Skip:** content inside `<pre>`, `<code>`, `<style>` tags (use the script approach below)
- **Avoid:** wrapping `&` or `;` inside HTML entities (`&amp;`, `&gt;`, `&lt;`)

**Script** (run after writing the HTML):
```python
# /tmp/punctify_v2.py — run with: python3 /tmp/punctify_v2.py file.html
import re, sys
html = open(sys.argv[1], encoding='utf-8').read()
saved = {}
def save(m, p):
    i = len([k for k in saved if p in k])
    ph = f'@@PUNCT_SAVED_{p}_{i}@@'; saved[ph] = m.group(0); return ph
html = re.sub(r'<pre[^>]*>.*?</pre>', lambda m: save(m,'PRE'), html, flags=re.DOTALL)
html = re.sub(r'<style[^>]*>.*?</style>', lambda m: save(m,'STYLE'), html, flags=re.DOTALL)
html = re.sub(r'<code>.*?</code>', lambda m: save(m,'CODE'), html, flags=re.DOTALL)
punct = set('،.,!؟?;:؛()[]""\'\u201c\u201d\u2018\u2019\u060c\u061b\u061f\u2013\u2014')
def wrap(t): return ''.join(f'<span class="punct"> {c} </span>' if c in punct else c for c in t)
html = re.sub(r'(>)([^<]*?)(<)', lambda m: m[1]+wrap(m[2])+m[3], html)
html = re.sub(r'^([^<]*?)(<)', lambda m: wrap(m[1])+m[2], html)
html = re.sub(r'(>)([^>]*?)$', lambda m: m[1]+wrap(m[2]), html)
for ph, orig in reversed(list(saved.items())): html = html.replace(ph, orig)
open(sys.argv[1], 'w', encoding='utf-8').write(html)
```

## Style Rules

- **Fonts:** `Noto Sans Arabic` (UI), `Noto Naskh Arabic` (prose body), `JetBrains Mono` (code) via Google Fonts `@import`
- **Colors:** Use the CSS variables from the reference (`--bg`, `--surface`, `--accent`, etc.) — do not invent new ones
- **JavaScript** — minimal JS only:
  - `IntersectionObserver` tracks visible sections → adds `.active` class to sidebar nav items
  - Theme toggle (if light mode added): class toggle + localStorage; button in `.sidebar-logo` next to the title
- **Active nav:** `.nav-item.active { color: var(--text); border-right-color: var(--accent); background: var(--surface2); }` — matches `.nav-item:hover`
- **Responsive:** Hide `.sidebar` at 900px, `.benefit-grid` collapses to 1 column
- **Print:** Hide sidebar, avoid breaking code blocks across pages
- **Per-node coloring:** Use `.code-block.h1` (blue border/head) + `.tag-h1` for heavy-1, `.h2` + `.tag-h2` for heavy-2, `.h3` + `.tag-h3` for heavy-3, `.tag-all` for commands on all nodes
- **Numbering:** Sections numbered sequentially (1, 2, 3…). Subsections can be unnumbered or use the sub-title pattern.
- **IDs:** Each section needs an `id` matching its anchor link in the sidebar nav
- **Sidebar nav:** `.nav-item.l1` `var(--text-body-sm)` (500 weight, line-height 1.55), `.l2` `var(--text-code)`; `.nav-group` `var(--text-small)`, `--font-sans`, no letter-spacing, no uppercase
- **Sidebar group labels:** Arabic group headers include English translation: `النظام — System`, `الشبكات — Network`, `التخزين — Storage`, `الخدمات — Services`, `الأداء — Performance`
- **Typography tokens:** All sizes MUST use CSS custom properties (never hardcoded `px`):
  - `--text-body: 1.125rem` (18px) — base body
  - `--text-prose: 1.0625rem` (17px) — prose paragraphs
  - `--text-h1: 2rem` (32px) — page title
  - `--text-h2: 1.5rem` (24px) — section title
  - `--text-h3: 1.25rem` (20px) — subsection title
  - `--text-code: 0.8125rem` (13px) — code blocks & small technical text
  - `--text-body-sm: 0.875rem` (14px) — sidebar nav, callout body, benefit-card title, step-list title
  - `--text-small: 0.8125rem` (13px) — verify block, benefit-card body, tags, footnotes
- **Benefit card:** `.bc-title` `var(--text-body-sm)`, `.bc-body` `var(--text-small)`, line-height 1.7
- **Step list:** `.step-list li strong` `var(--text-body-sm)`, `.step-list li .desc` `var(--text-code)`, line-height 1.7
- **Info table:** `padding: 12px 16px`, font `var(--text-body-sm)`, `line-height: 1.8` on td/th
- **Inline styles:** Never use inline `style="..."` on paragraphs — use `<p class="prose">` instead
- **`.punct` bullets:** Use `<span class="punct">• </span>` for Q&A answer prefixes; step-list `.desc::before` uses CSS `content: "• "`
- **Rem-only rule:** Every `font-size` in CSS must use `var(--text-*)` — no hardcoded `px` values except on icon elements (`.callout-icon`, `.bc-icon`, `.cb-dots`)
- **Code syntax highlighting:**
  - `.cm` (comments): `#7096b8` italic — 6:1 contrast on `--code-bg`
  - `.kw` (keywords): `#ff7b72` (soft red)
  - `.val` (values): `#d2a8ff` (purple)
  - `.str` (strings): `#a5d6ff` (light blue)
  - `.ph` (placeholders): `var(--accent3)` (`#f0a500`, amber)
  - Inline `<code>`: `#7dd3fc` on `--surface3`
- **Code block header:** `.cb-head` background `#0e141e` — subtly lighter than `.code-block` (`--code-bg: #080b0f`) for visual separation
- **Border colors:** `--border: #222c40`, `--border2: #2a3850` — slight blue tint for separation without harshness
- **`.benefit-card.danger`:** `background: #1a0a0a`, `border-color: #5a1515` — richer red than the base danger palette

## JavaScript — Active Nav Tracking

Include before `</body>`. Watches sections via `IntersectionObserver` and highlights the matching sidebar link:

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

## Process

1. Read the reference HTML file to absorb the exact style
2. Ask user for: document title, sections/content, and any per-node scoping
3. Generate a single self-contained `.html` file matching every component convention above
4. Read the file back and verify all tags are closed and classes match the reference
