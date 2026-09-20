---
name: docs-nextjs
description: >
  Next.js 16 conventions and breaking changes for building a documentation
  website — App Router, async params, proxy.ts, cacheComponents, and
  Tailwind CSS v4 integration.
---

## Version

Next.js **16.2.x** with React **19.2.x** and TypeScript **5.x**.

## Key Breaking Changes from v14/v15

### `middleware.ts` → `proxy.ts`

Middleware has been renamed to **Proxy**. The file is now `proxy.ts` (or `proxy.js`) at the project root or `src/` root, and the exported function is named `proxy` instead of `middleware`.

```ts
// proxy.ts
import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

export function proxy(request: NextRequest) {
  return NextResponse.redirect(new URL('/home', request.url))
}

export const config = {
  matcher: '/about/:path*',
}
```

Proxy defaults to the **Node.js runtime** (no longer Edge-only).

### `params` and `searchParams` are Promises

In `page.tsx`, `layout.tsx`, and `route.ts`, `params` and `searchParams` are **Promises**. You must use `async/await` or React's `use()` hook to access them.

```tsx
// page.tsx — correct pattern
export default async function Page({
  params,
  searchParams,
}: {
  params: Promise<{ slug: string }>
  searchParams: Promise<{ [key: string]: string | string[] | undefined }>
}) {
  const { slug } = await params
  const filters = (await searchParams).filters
  return <h1>{slug}</h1>
}
```

For client components, use `use()` instead of `await`:

```tsx
'use client'
import { use } from 'react'

export default function Page({
  params,
}: {
  params: Promise<{ slug: string }>
}) {
  const { slug } = use(params)
  return <h1>{slug}</h1>
}
```

### `cookies()` and `headers()` are async

```ts
import { cookies } from 'next/headers'
import { headers } from 'next/headers'

const cookieStore = await cookies()
const headersList = await headers()
```

### `fetch` Default Caching

The default `fetch` cache behavior is now **`auto no cache`** (not `force-cache` as in v14). Routes are statically prerendered unless Request-time APIs are detected. Use `cache: 'force-cache'` explicitly if you want persistent caching.

```ts
fetch('https://api.example.com/data', { cache: 'force-cache' })
fetch('https://api.example.com/data', { next: { revalidate: 3600 } })
```

## Type Helpers (Globally Available)

No import needed — types are generated during `next dev` / `next build` / `next typegen`.

### `PageProps<Route>`

```tsx
export default async function Page(props: PageProps<'/blog/[slug]'>) {
  const { slug } = await props.params
  return <h1>Blog Post: {slug}</h1>
}
```

### `LayoutProps<Route>`

```tsx
export default function Layout(props: LayoutProps<'/dashboard'>) {
  return <section>{props.children}</section>
}
```

### `RouteContext<Route>`

```tsx
export async function GET(_req: NextRequest, ctx: RouteContext<'/users/[id]'>) {
  const { id } = await ctx.params
  return Response.json({ id })
}
```

## `cacheComponents` Flag

Set `cacheComponents: true` in `next.config.ts` to enable:

- Component and function-level caching via the `use cache` directive
- **Partial Prerendering (PPR)** — static HTML shell served immediately, dynamic content streams in
- **Activity-based navigation** — React's `<Activity>` preserves component state during client-side navigation
- `cacheLife()` and `cacheTag()` functions

```ts
// next.config.ts
const nextConfig: NextConfig = {
  cacheComponents: true,
}
```

```tsx
export default async function Page() {
  'use cache'
  const data = await fetch('https://api.example.com/data')
  return <div>{/* ... */}</div>
}
```

## App Router File Conventions

| File | Purpose |
|---|---|
| `page.tsx` | Route UI (leaf component) |
| `layout.tsx` | Shared layout for segments |
| `loading.tsx` | Suspense fallback |
| `error.tsx` | Error boundary (catches errors in segment) |
| `not-found.tsx` | 404 UI per segment |
| `route.tsx` | API route handler |
| `proxy.ts` | Request proxy (replaces middleware.ts) |

## Recommended Project Structure

```
src/
  app/
    [locale]/           ← dynamic locale segment (if using i18n)
      layout.tsx        ← locale-aware root layout
      page.tsx          ← landing / home page
      guides/
        [slug]/
          page.tsx      ← guide detail page
  components/
    docs/               ← documentation-specific components (CodeBlock, Callout, etc.)
    layout/             ← layout components (Topbar, Sidebar, DocsPageLayout)
  content/              ← guide content data & registry
    index.ts            ← content registry
  i18n/                 ← next-intl routing and request config (if using i18n)
  lib/                  ← shared utilities
messages/               ← translation JSON files (if using i18n)
```

## Tailwind CSS v4

Tailwind v4 uses **CSS-first configuration** instead of `tailwind.config.js`. Configure via `@import` in `globals.css`:

```css
@import "tailwindcss";
```

Custom theme values are defined with `@theme`:

```css
@theme {
  --color-bg: #f8f9fa;
  --color-surface: #ffffff;
  --font-family-sans: 'Inter', 'Noto Sans Arabic', sans-serif;
  --font-family-mono: 'JetBrains Mono', monospace;
}
```

The `dark:` variant works with a class-based strategy.

## Route Group Convention

Use route groups `(name)` for organizing without affecting URL structure:

```
app/
  (docs)/               ← shared docs layout
    guides/
    tutorials/
  (landing)/            ← landing page layout
    page.tsx
```

## ESLint

Use `eslint-config-next` (v16.x). Run with:

```bash
npm run lint
```
