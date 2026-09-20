---
name: docs-i18n
description: >
  Internationalization setup for a bilingual (English / Arabic)
  documentation website using next-intl v4 — locale routing, RTL
  layout, Arabic typography, number formatting, and translation
  file conventions.
---

## Tech Stack

- **Library:** `next-intl` v4
- **Locales:** `en` (English, LTR), `ar` (Arabic, RTL)
- **Default locale:** `ar` (Arabic) or `en` (English) — configurable

## Directory Structure

```
src/
  i18n/
    request.ts       ← next-intl request configuration
    routing.ts       ← locale routing (pathnames, localePrefix)
  app/
    [locale]/        ← dynamic locale segment
      layout.tsx     ← locale-aware root layout
      page.tsx       ← landing page
      guides/
        [slug]/
          page.tsx
messages/
  en.json            ← English translations
  ar.json            ← Arabic translations
next.config.ts       ← withNextIntl plugin
```

## next-intl v4 Setup

### `next.config.ts`

```ts
import type { NextConfig } from 'next'
import createNextIntlPlugin from 'next-intl/plugin'

const withNextIntl = createNextIntlPlugin('./src/i18n/request.ts')

const nextConfig: NextConfig = {}

export default withNextIntl(nextConfig)
```

### `src/i18n/request.ts`

```ts
import { getRequestConfig } from 'next-intl/server'
import { routing } from './routing'

export default getRequestConfig(async ({ requestLocale }) => {
  let locale = await requestLocale
  if (!locale || !routing.locales.includes(locale)) {
    locale = routing.defaultLocale
  }
  return {
    locale,
    messages: (await import(`../../messages/${locale}.json`)).default,
  }
})
```

### `src/i18n/routing.ts`

```ts
import { defineRouting } from 'next-intl/routing'

export const routing = defineRouting({
  locales: ['en', 'ar'],
  defaultLocale: 'ar',
  localePrefix: 'as-needed',
})
```

Use `localePrefix: 'as-needed'` to omit the default locale prefix from URLs if desired.

## RTL Layout

### Root Layout (in `src/app/[locale]/layout.tsx`)

```tsx
import { NextIntlClientProvider } from 'next-intl'
import { getMessages, getTranslations } from 'next-intl/server'
import { ThemeProvider } from 'next-themes'

export default async function LocaleLayout({
  children,
  params,
}: {
  children: React.ReactNode
  params: Promise<{ locale: string }>
}) {
  const { locale } = await params
  const messages = await getMessages()

  return (
    <html lang={locale} dir={locale === 'ar' ? 'rtl' : 'ltr'} suppressHydrationWarning>
      <body>
        <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
          <NextIntlClientProvider messages={messages}>
            {children}
          </NextIntlClientProvider>
        </ThemeProvider>
      </body>
    </html>
  )
}
```

The `dir` attribute is set on `<html>` based on locale. All layout logic uses this attribute for mirrored positioning.

### CSS for RTL

Use CSS logical properties whenever possible:

```css
.element {
  margin-inline-start: 1rem;
  margin-inline-end: 0;
}

.element {
  inset-inline-start: 0;
}
```

When logical properties are not practical, use `[dir="rtl"]` overrides:

```css
.sidebar {
  [dir="ltr"] & { left: 0; }
  [dir="rtl"] & { right: 0; }
}
```

## Translation File Structure

### `messages/en.json`

```json
{
  "nav": {
    "home": "Home",
    "guides": "Guides",
    "docs": "Documentation"
  },
  "guides": {
    "getting-started": {
      "title": "Getting Started Guide",
      "eyebrow": "Basics / Setup"
    }
  },
  "common": {
    "search": "Search documentation...",
    "theme": {
      "light": "Light",
      "dark": "Dark",
      "system": "System"
    },
    "language": "Language"
  }
}
```

### `messages/ar.json`

```json
{
  "nav": {
    "home": "الرئيسية",
    "guides": "الأدلة",
    "docs": "التوثيق"
  },
  "guides": {
    "getting-started": {
      "title": "دليل البدء",
      "eyebrow": "أساسيات / إعداد"
    }
  },
  "common": {
    "search": "ابحث في التوثيق...",
    "theme": {
      "light": "فاتح",
      "dark": "داكن",
      "system": "النظام"
    },
    "language": "اللغة"
  }
}
```

English file is the source of truth for key structure. Arabic translations match the same keys.

## Using Translations in Components

```tsx
import { useTranslations } from 'next-intl'

function Navbar() {
  const t = useTranslations('nav')
  return <nav>
    <Link href="/">{t('home')}</Link>
    <Link href="/guides">{t('guides')}</Link>
  </nav>
}
```

For server components:

```tsx
import { getTranslations } from 'next-intl/server'

export default async function Page() {
  const t = await getTranslations('nav')
  return <h1>{t('home')}</h1>
}
```

## Number Formatting

```tsx
import { useFormatter } from 'next-intl'

function Stats({ count }: { count: number }) {
  const format = useFormatter()
  return <span>{format.number(count)}</span>
  // English: 1,234 — Arabic: ١٬٢٣٤
}
```

`format.number()` automatically uses Arabic-Indic digits (٠١٢٣٤٥٦٧٨٩) for `ar` locale.

## Date Formatting

```tsx
format.dateTime(new Date(), {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
})
// English: "July 6, 2026"
// Arabic: "٦ يوليو ٢٠٢٦"
```

## Language Switcher

```tsx
import { useLocale } from 'next-intl'
import { useRouter, usePathname } from 'next/navigation'

function LanguageToggle() {
  const locale = useLocale()
  const router = useRouter()
  const pathname = usePathname()

  const toggleLanguage = () => {
    const newLocale = locale === 'en' ? 'ar' : 'en'
    router.replace(`/${newLocale}${pathname}`)
  }

  return (
    <button onClick={toggleLanguage}>
      {locale === 'en' ? 'العربية' : 'English'}
    </button>
  )
}
```

Place the language switcher in the top bar for easy access.

## Arabic Typography Specifics

| Font | Role | Weight |
|---|---|---|
| `Noto Sans Arabic` | UI, headings, nav, sidebar | 300–700 |
| `Noto Naskh Arabic` | Body prose, paragraphs | 400–700 |
| `JetBrains Mono` | Code blocks, inline code | 400, 600, 700 |

Rules:
- Arabic headings: `letter-spacing: 0` (never use letter-spacing for Arabic text)
- Body line-height: `1.9` for Arabic (taller than English 1.7)
- Code blocks: `direction: ltr; unicode-bidi: isolate` to prevent bidi reordering
- Inline `<code>`: `direction: ltr; unicode-bidi: isolate;`
- UI text (sidebar, nav): `font-family: 'Noto Sans Arabic'`, weight 400–500

## Links Between Locales

Always link to the current locale when navigating within the site:

```tsx
<Link href={`/${locale}/guides/${slug}`}>
```

Use `pathnames` in routing config if you want different URL paths per locale.

## SEO

Set `hrefLang` for alternate language pages:

```tsx
export async function generateMetadata({ params }: { params: Promise<{ locale: string }> }) {
  const { locale } = await params
  return {
    alternates: {
      languages: {
        en: '/en/guides/getting-started',
        ar: '/ar/guides/getting-started',
      },
    },
  }
}
```
