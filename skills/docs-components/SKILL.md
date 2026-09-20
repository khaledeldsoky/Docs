---
name: docs-components
description: >
  Reusable documentation page components — cover, sections, code blocks,
  callouts, tables, sidebar navigation, and responsive layout patterns
  for a bilingual technical documentation website.
---

## Page Layout Structure

```
<PageLayout>                    ← full-page wrapper
  <Sidebar />                   ← sticky side navigation
  <main class="main-content">
    <Cover />                  ← page header area
    <Section>                  ← one per topic
      <SectionHeader />
      <SubSection />
      <CodeBlock />
      <Callout />
      <InfoTable />
      <VerifyBlock />
      <StepList />
      <BenefitCardGrid />
    </Section>
  </main>
</PageLayout>
```

## Cover

The top hero area of every doc page.

```tsx
<Cover
  breadcrumb={[{ label: 'Guides', href: '/guides' }, { label: 'Installation' }]}
  title="Installation Guide"
  subtitle="Step-by-step instructions for setting up the environment"
  chips={['Beginner', 'Linux', 'CLI']}
/>
```

| Prop | Type | Description |
|---|---|---|
| `breadcrumb` | `{label: string, href?: string}[]` | Breadcrumb trail |
| `title` | `string` | Page title (`--text-h1`) |
| `subtitle` | `string` | Short description |
| `chips` | `string[]` | Tags / metadata badges |

Breadcrumb separator: `/` in LTR, `\` in RTL (flip via CSS `[dir="rtl"] .breadcrumb-sep { transform: scaleX(-1) }`).

## Section

```tsx
<Section id="prerequisites">
  <SectionHeader number={1} title="Prerequisites" />
  <p class="prose">Before you begin...</p>
</Section>
```

| Component | Description |
|---|---|
| `Section` | Wrapper with `id` anchor, scroll-margin-top for fixed header |
| `SectionHeader` | Numbered or unnumbered (`.section-num` + `.section-title`) |
| `SubSection` | H3 heading with accent `::before` bar, nested content |

Sections use `id` attributes for sidebar anchor links and IntersectionObserver scroll tracking.

## CodeBlock

```tsx
<CodeBlock language="bash" label="/etc/hosts" node="all">
  {`192.168.1.10 api.example.com
192.168.1.11 *.apps.example.com`}
</CodeBlock>
```

| Prop | Type | Description |
|---|---|---|
| `language` | `string` | Language for syntax highlighting |
| `label` | `string` | File path / context label in header |
| `node` | `'h1' \| 'h2' \| 'h3' \| 'all'` | Per-node scoping color |
| `children` | `string` | Code content |

Layout:
- Header: background slightly lighter than code area, contains label + copy button
- Body: `<pre>` with syntax-colored spans
- Per-node coloring: blue border/tag for `h1`, green for `h2`, purple for `h3`, grey for `all`
- In RTL mode: code block is always `direction: ltr`

Use `shiki` for syntax highlighting (compile to colored spans at build time, zero JS runtime).

## Callout

```tsx
<Callout type="info">
  This is informational text.
</Callout>
```

| Prop | Type | Description |
|---|---|---|
| `type` | `'info' \| 'warn' \| 'danger' \| 'success'` | Variant |
| `children` | `ReactNode` | Content |

Styling per variant:
- `info`: blue left border + icon
- `warn`: amber left border + icon
- `danger`: red left border + icon
- `success`: green left border + icon

## InfoTable

```tsx
<InfoTable
  headers={['Parameter', 'Value', 'Description']}
  rows={[
    ['cluster_id', 'my-cluster', 'Unique cluster identifier'],
  ]}
/>
```

| Prop | Type | Description |
|---|---|---|
| `headers` | `string[]` | Column headers (`<th>`) |
| `rows` | `string[][]` | Data rows (`<td>`) |

Row hover highlight, alternating row backgrounds for readability.

## VerifyBlock

```tsx
<VerifyBlock label="Expected Output">
  Verify the service status shows all nodes as Ready.
</VerifyBlock>
```

| Prop | Type | Description |
|---|---|---|
| `label` | `string` | Green-bordered label |
| `children` | `ReactNode` | Content (often includes `<code>` or `<pre>`) |

Green left border, subtle green background tint.

## StepList

```tsx
<StepList>
  <Step title="Download the installer">
    Navigate to the official download page.
  </Step>
  <Step title="Extract the binary">
    Run <code>tar xvf installer-linux.tar.gz</code>
  </Step>
</StepList>
```

| Component | Description |
|---|---|
| `StepList` | `<ol>` with CSS counter for auto-numbering |
| `Step` | `<li>` with `title` prop displayed as bold accent-colored heading, description on next line with `•` prefix |

## BenefitCardGrid

```tsx
<BenefitCardGrid>
  <BenefitCard icon="🚀" title="Faster Deployments">
    Automated provisioning reduces deployment time by 60%.
  </BenefitCard>
  <BenefitCard icon="🛡️" title="Security First" danger>
    Built-in security controls and compliance checks.
  </BenefitCard>
</BenefitCardGrid>
```

2-column grid, collapses to 1 column on mobile. Cards have icon, title, and body.

## Sidebar

```tsx
<Sidebar groups={sidebarGroups} />
```

| Prop | Type | Description |
|---|---|---|
| `groups` | `NavGroup[]` | Grouped navigation items |

```typescript
type NavGroup = {
  label: string
  items: NavItem[]
}

type NavItem = {
  label: string
  href: string
  level?: 1 | 2  // nesting level
}
```

Features:
- Sticky positioning, scrollable within viewport
- Active item tracking via IntersectionObserver (highlights current section on scroll)
- Collapsible groups
- Hidden on small screens (mobile drawer or hamburger menu)
- In RTL: mirrored position, group labels without letter-spacing

## Responsive Breakpoints

| Breakpoint | Width | Behavior |
|---|---|---|
| Desktop | `≥1024px` | Full sidebar + content |
| Tablet | `768px – 1023px` | Collapsible sidebar |
| Mobile | `<768px` | Hidden sidebar, drawer overlay |

## Loading & Empty States

- Page loading: skeleton placeholders matching content layout
- Empty state: centered message with icon for missing content
- Error state: Callout `danger` with retry button
