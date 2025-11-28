# GloboTicket Design System Architecture

## Executive Summary

This document defines the complete architecture for the GloboTicket web application's design system, following atomic design principles with strict separation between theme and layout. The system is built on:

- **Styling**: Tailwind CSS with custom configuration
- **Routing**: React Router v6
- **State Management**: TanStack Query + React Context for theme/auth
- **Component Base**: Headless UI for complex interactive components
- **Design Philosophy**: Light mode default, explicit dark mode, styling at the atom level

---

## 1. Project Structure

```
src/GloboTicket.Web/src/
├── main.tsx                          # Application entry point
├── App.tsx                           # Root component with providers
├── index.css                         # Global styles and Tailwind imports
│
├── config/
│   ├── theme.config.ts              # Theme token definitions
│   ├── tailwind.config.ts           # Tailwind configuration
│   └── routes.config.ts             # Route definitions
│
├── theme/
│   ├── ThemeProvider.tsx            # Theme context provider
│   ├── tokens.ts                    # Design tokens (colors, spacing, etc.)
│   └── types.ts                     # Theme-related TypeScript types
│
├── layouts/
│   ├── primitives/
│   │   ├── Stack.tsx                # Vertical layout primitive
│   │   ├── Row.tsx                  # Horizontal layout primitive
│   │   ├── Column.tsx               # Column within grid
│   │   ├── Grid.tsx                 # Grid layout primitive
│   │   ├── Container.tsx            # Max-width container
│   │   ├── Spacer.tsx               # Flexible spacing component
│   │   └── index.ts                 # Barrel export
│   │
│   ├── AppLayout.tsx                # Main application layout
│   ├── AuthLayout.tsx               # Authentication pages layout
│   └── index.ts                     # Barrel export
│
├── components/
│   ├── atoms/
│   │   ├── Button/
│   │   │   ├── Button.tsx
│   │   │   ├── Button.types.ts
│   │   │   └── index.ts
│   │   ├── Input/
│   │   │   ├── Input.tsx
│   │   │   ├── Input.types.ts
│   │   │   └── index.ts
│   │   ├── Heading/
│   │   │   ├── Heading.tsx
│   │   │   ├── Heading.types.ts
│   │   │   └── index.ts
│   │   ├── Text/
│   │   │   ├── Text.tsx
│   │   │   ├── Text.types.ts
│   │   │   └── index.ts
│   │   ├── Icon/
│   │   │   ├── Icon.tsx
│   │   │   ├── Icon.types.ts
│   │   │   ├── icons/              # SVG icon components
│   │   │   └── index.ts
│   │   ├── Badge/
│   │   │   ├── Badge.tsx
│   │   │   ├── Badge.types.ts
│   │   │   └── index.ts
│   │   ├── Avatar/
│   │   │   ├── Avatar.tsx
│   │   │   ├── Avatar.types.ts
│   │   │   └── index.ts
│   │   ├── Spinner/
│   │   │   ├── Spinner.tsx
│   │   │   └── index.ts
│   │   └── index.ts                # Barrel export
│   │
│   ├── molecules/
│   │   ├── FormField/
│   │   │   ├── FormField.tsx       # Label + Input + Error
│   │   │   ├── FormField.types.ts
│   │   │   └── index.ts
│   │   ├── SearchBar/
│   │   │   ├── SearchBar.tsx       # Input + Icon + Clear button
│   │   │   ├── SearchBar.types.ts
│   │   │   └── index.ts
│   │   ├── Card/
│   │   │   ├── Card.tsx
│   │   │   ├── CardHeader.tsx
│   │   │   ├── CardBody.tsx
│   │   │   ├── CardFooter.tsx
│   │   │   ├── Card.types.ts
│   │   │   └── index.ts
│   │   ├── EmptyState/
│   │   │   ├── EmptyState.tsx      # Icon + Heading + Text + Action
│   │   │   ├── EmptyState.types.ts
│   │   │   └── index.ts
│   │   ├── Alert/
│   │   │   ├── Alert.tsx           # Icon + Message + Close
│   │   │   ├── Alert.types.ts
│   │   │   └── index.ts
│   │   ├── Dropdown/
│   │   │   ├── Dropdown.tsx        # Built on Headless UI Menu
│   │   │   ├── Dropdown.types.ts
│   │   │   └── index.ts
│   │   ├── Modal/
│   │   │   ├── Modal.tsx           # Built on Headless UI Dialog
│   │   │   ├── ModalHeader.tsx
│   │   │   ├── ModalBody.tsx
│   │   │   ├── ModalFooter.tsx
│   │   │   ├── Modal.types.ts
│   │   │   └── index.ts
│   │   ├── Tabs/
│   │   │   ├── Tabs.tsx            # Built on Headless UI Tab
│   │   │   ├── Tabs.types.ts
│   │   │   └── index.ts
│   │   └── index.ts                # Barrel export
│   │
│   ├── organisms/
│   │   ├── Navigation/
│   │   │   ├── AppHeader.tsx       # Top navigation bar
│   │   │   ├── Sidebar.tsx         # Side navigation
│   │   │   ├── NavItem.tsx         # Navigation link component
│   │   │   ├── UserMenu.tsx        # User dropdown menu
│   │   │   ├── Navigation.types.ts
│   │   │   └── index.ts
│   │   ├── VenueCard/
│   │   │   ├── VenueCard.tsx       # Venue display card
│   │   │   ├── VenueCard.types.ts
│   │   │   └── index.ts
│   │   ├── ActCard/
│   │   │   ├── ActCard.tsx         # Act display card
│   │   │   ├── ActCard.types.ts
│   │   │   └── index.ts
│   │   ├── ShowCard/
│   │   │   ├── ShowCard.tsx        # Show display card
│   │   │   ├── ShowCard.types.ts
│   │   │   └── index.ts
│   │   ├── VenueForm/
│   │   │   ├── VenueForm.tsx       # Venue create/edit form
│   │   │   ├── VenueForm.types.ts
│   │   │   └── index.ts
│   │   ├── ActForm/
│   │   │   ├── ActForm.tsx         # Act create/edit form
│   │   │   ├── ActForm.types.ts
│   │   │   └── index.ts
│   │   ├── ShowForm/
│   │   │   ├── ShowForm.tsx        # Show create/edit form
│   │   │   ├── ShowForm.types.ts
│   │   │   └── index.ts
│   │   ├── DataTable/
│   │   │   ├── DataTable.tsx       # Reusable data table
│   │   │   ├── DataTable.types.ts
│   │   │   └── index.ts
│   │   └── index.ts                # Barrel export
│   │
│   └── index.ts                    # Barrel export for all components
│
├── pages/
│   ├── auth/
│   │   ├── LoginPage.tsx
│   │   └── index.ts
│   ├── venues/
│   │   ├── VenuesListPage.tsx
│   │   ├── VenueDetailPage.tsx
│   │   ├── VenueCreatePage.tsx
│   │   ├── VenueEditPage.tsx
│   │   └── index.ts
│   ├── acts/
│   │   ├── ActsListPage.tsx
│   │   ├── ActDetailPage.tsx
│   │   ├── ActCreatePage.tsx
│   │   ├── ActEditPage.tsx
│   │   └── index.ts
│   ├── shows/
│   │   ├── ShowsListPage.tsx
│   │   ├── ShowDetailPage.tsx
│   │   ├── ShowCreatePage.tsx
│   │   ├── ShowEditPage.tsx
│   │   └── index.ts
│   ├── dashboard/
│   │   ├── DashboardPage.tsx
│   │   └── index.ts
│   └── index.ts                    # Barrel export
│
├── features/
│   ├── auth/
│   │   ├── hooks/
│   │   │   ├── useAuth.ts
│   │   │   ├── useLogin.ts
│   │   │   └── useLogout.ts
│   │   ├── context/
│   │   │   └── AuthContext.tsx
│   │   └── index.ts
│   ├── venues/
│   │   ├── hooks/
│   │   │   ├── useVenues.ts
│   │   │   ├── useVenue.ts
│   │   │   ├── useCreateVenue.ts
│   │   │   ├── useUpdateVenue.ts
│   │   │   └── useDeleteVenue.ts
│   │   └── index.ts
│   ├── acts/
│   │   ├── hooks/
│   │   │   ├── useActs.ts
│   │   │   ├── useAct.ts
│   │   │   ├── useCreateAct.ts
│   │   │   ├── useUpdateAct.ts
│   │   │   └── useDeleteAct.ts
│   │   └── index.ts
│   ├── shows/
│   │   ├── hooks/
│   │   │   ├── useShows.ts
│   │   │   ├── useShow.ts
│   │   │   ├── useCreateShow.ts
│   │   │   ├── useUpdateShow.ts
│   │   │   └── useDeleteShow.ts
│   │   └── index.ts
│   └── index.ts
│
├── api/
│   ├── client.ts                   # Base API client (existing)
│   ├── venues.ts                   # Venue API endpoints
│   ├── acts.ts                     # Act API endpoints
│   ├── shows.ts                    # Show API endpoints
│   └── index.ts
│
├── types/
│   ├── api.ts                      # API types (existing)
│   ├── venue.ts                    # Venue domain types
│   ├── act.ts                      # Act domain types
│   ├── show.ts                     # Show domain types
│   └── index.ts
│
├── utils/
│   ├── cn.ts                       # Tailwind class name utility
│   ├── format.ts                   # Date/number formatting
│   ├── validation.ts               # Form validation helpers
│   └── index.ts
│
└── hooks/
    ├── useTheme.ts                 # Theme hook
    ├── useMediaQuery.ts            # Responsive hook
    ├── useDebounce.ts              # Debounce hook
    └── index.ts
```

---

## 2. Theme Token System

### 2.1 Design Tokens Structure

**File: `src/theme/tokens.ts`**

```typescript
export const tokens = {
  colors: {
    // Brand colors
    brand: {
      primary: {
        light: '#3B82F6',      // Blue-500
        dark: '#60A5FA',       // Blue-400
      },
      secondary: {
        light: '#8B5CF6',      // Violet-500
        dark: '#A78BFA',       // Violet-400
      },
    },
    
    // Semantic colors
    success: {
      light: '#10B981',        // Green-500
      dark: '#34D399',         // Green-400
    },
    warning: {
      light: '#F59E0B',        // Amber-500
      dark: '#FCD34D',         // Amber-300
    },
    error: {
      light: '#EF4444',        // Red-500
      dark: '#F87171',         // Red-400
    },
    info: {
      light: '#3B82F6',        // Blue-500
      dark: '#60A5FA',         // Blue-400
    },
    
    // Surface colors
    surface: {
      base: {
        light: '#FFFFFF',
        dark: '#1F2937',       // Gray-800
      },
      elevated: {
        light: '#F9FAFB',      // Gray-50
        dark: '#374151',       // Gray-700
      },
      overlay: {
        light: '#F3F4F6',      // Gray-100
        dark: '#4B5563',       // Gray-600
      },
    },
    
    // Text colors
    text: {
      primary: {
        light: '#111827',      // Gray-900
        dark: '#F9FAFB',       // Gray-50
      },
      secondary: {
        light: '#6B7280',      // Gray-500
        dark: '#D1D5DB',       // Gray-300
      },
      tertiary: {
        light: '#9CA3AF',      // Gray-400
        dark: '#9CA3AF',       // Gray-400
      },
      inverse: {
        light: '#FFFFFF',
        dark: '#111827',       // Gray-900
      },
      disabled: {
        light: '#D1D5DB',      // Gray-300
        dark: '#6B7280',       // Gray-500
      },
    },
    
    // Border colors
    border: {
      default: {
        light: '#E5E7EB',      // Gray-200
        dark: '#4B5563',       // Gray-600
      },
      subtle: {
        light: '#F3F4F6',      // Gray-100
        dark: '#374151',       // Gray-700
      },
      strong: {
        light: '#D1D5DB',      // Gray-300
        dark: '#6B7280',       // Gray-500
      },
    },
  },
  
  spacing: {
    xs: '0.25rem',    // 4px
    sm: '0.5rem',     // 8px
    md: '1rem',       // 16px
    lg: '1.5rem',     // 24px
    xl: '2rem',       // 32px
    '2xl': '3rem',    // 48px
    '3xl': '4rem',    // 64px
  },
  
  typography: {
    fontFamily: {
      sans: 'system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
      mono: 'ui-monospace, SFMono-Regular, "SF Mono", Menlo, Consolas, monospace',
    },
    fontSize: {
      xs: '0.75rem',      // 12px
      sm: '0.875rem',     // 14px
      base: '1rem',       // 16px
      lg: '1.125rem',     // 18px
      xl: '1.25rem',      // 20px
      '2xl': '1.5rem',    // 24px
      '3xl': '1.875rem',  // 30px
      '4xl': '2.25rem',   // 36px
    },
    fontWeight: {
      normal: '400',
      medium: '500',
      semibold: '600',
      bold: '700',
    },
    lineHeight: {
      tight: '1.25',
      normal: '1.5',
      relaxed: '1.75',
    },
  },
  
  borderRadius: {
    none: '0',
    sm: '0.25rem',    // 4px
    md: '0.375rem',   // 6px
    lg: '0.5rem',     // 8px
    xl: '0.75rem',    // 12px
    full: '9999px',
  },
  
  shadows: {
    sm: {
      light: '0 1px 2px 0 rgb(0 0 0 / 0.05)',
      dark: '0 1px 2px 0 rgb(0 0 0 / 0.3)',
    },
    md: {
      light: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
      dark: '0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.3)',
    },
    lg: {
      light: '0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1)',
      dark: '0 10px 15px -3px rgb(0 0 0 / 0.4), 0 4px 6px -4px rgb(0 0 0 / 0.3)',
    },
    xl: {
      light: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)',
      dark: '0 20px 25px -5px rgb(0 0 0 / 0.4), 0 8px 10px -6px rgb(0 0 0 / 0.3)',
    },
  },
  
  transitions: {
    fast: '150ms cubic-bezier(0.4, 0, 0.2, 1)',
    base: '200ms cubic-bezier(0.4, 0, 0.2, 1)',
    slow: '300ms cubic-bezier(0.4, 0, 0.2, 1)',
  },
  
  breakpoints: {
    sm: '640px',
    md: '768px',
    lg: '1024px',
    xl: '1280px',
    '2xl': '1536px',
  },
} as const;
```

### 2.2 Tailwind Configuration

**File: `tailwind.config.ts`**

```typescript
import type { Config } from 'tailwindcss';

export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  darkMode: 'class', // Explicit dark mode via class
  theme: {
    extend: {
      colors: {
        brand: {
          primary: 'var(--color-brand-primary)',
          secondary: 'var(--color-brand-secondary)',
        },
        success: 'var(--color-success)',
        warning: 'var(--color-warning)',
        error: 'var(--color-error)',
        info: 'var(--color-info)',
        surface: {
          base: 'var(--color-surface-base)',
          elevated: 'var(--color-surface-elevated)',
          overlay: 'var(--color-surface-overlay)',
        },
        text: {
          primary: 'var(--color-text-primary)',
          secondary: 'var(--color-text-secondary)',
          tertiary: 'var(--color-text-tertiary)',
          inverse: 'var(--color-text-inverse)',
          disabled: 'var(--color-text-disabled)',
        },
        border: {
          DEFAULT: 'var(--color-border-default)',
          subtle: 'var(--color-border-subtle)',
          strong: 'var(--color-border-strong)',
        },
      },
      spacing: {
        xs: 'var(--spacing-xs)',
        sm: 'var(--spacing-sm)',
        md: 'var(--spacing-md)',
        lg: 'var(--spacing-lg)',
        xl: 'var(--spacing-xl)',
        '2xl': 'var(--spacing-2xl)',
        '3xl': 'var(--spacing-3xl)',
      },
      borderRadius: {
        sm: 'var(--radius-sm)',
        md: 'var(--radius-md)',
        lg: 'var(--radius-lg)',
        xl: 'var(--radius-xl)',
      },
      boxShadow: {
        sm: 'var(--shadow-sm)',
        md: 'var(--shadow-md)',
        lg: 'var(--shadow-lg)',
        xl: 'var(--shadow-xl)',
      },
      transitionDuration: {
        fast: 'var(--transition-fast)',
        base: 'var(--transition-base)',
        slow: 'var(--transition-slow)',
      },
    },
  },
  plugins: [],
} satisfies Config;
```

### 2.3 CSS Variables Implementation

**File: `src/index.css`**

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  :root {
    /* Light mode (default) */
    --color-brand-primary: #3B82F6;
    --color-brand-secondary: #8B5CF6;
    
    --color-success: #10B981;
    --color-warning: #F59E0B;
    --color-error: #EF4444;
    --color-info: #3B82F6;
    
    --color-surface-base: #FFFFFF;
    --color-surface-elevated: #F9FAFB;
    --color-surface-overlay: #F3F4F6;
    
    --color-text-primary: #111827;
    --color-text-secondary: #6B7280;
    --color-text-tertiary: #9CA3AF;
    --color-text-inverse: #FFFFFF;
    --color-text-disabled: #D1D5DB;
    
    --color-border-default: #E5E7EB;
    --color-border-subtle: #F3F4F6;
    --color-border-strong: #D1D5DB;
    
    --spacing-xs: 0.25rem;
    --spacing-sm: 0.5rem;
    --spacing-md: 1rem;
    --spacing-lg: 1.5rem;
    --spacing-xl: 2rem;
    --spacing-2xl: 3rem;
    --spacing-3xl: 4rem;
    
    --radius-sm: 0.25rem;
    --radius-md: 0.375rem;
    --radius-lg: 0.5rem;
    --radius-xl: 0.75rem;
    
    --shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
    --shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
    --shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);
    --shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1);
    
    --transition-fast: 150ms cubic-bezier(0.4, 0, 0.2, 1);
    --transition-base: 200ms cubic-bezier(0.4, 0, 0.2, 1);
    --transition-slow: 300ms cubic-bezier(0.4, 0, 0.2, 1);
  }
  
  .dark {
    /* Dark mode - explicit overrides */
    --color-brand-primary: #60A5FA;
    --color-brand-secondary: #A78BFA;
    
    --color-success: #34D399;
    --color-warning: #FCD34D;
    --color-error: #F87171;
    --color-info: #60A5FA;
    
    --color-surface-base: #1F2937;
    --color-surface-elevated: #374151;
    --color-surface-overlay: #4B5563;
    
    --color-text-primary: #F9FAFB;
    --color-text-secondary: #D1D5DB;
    --color-text-tertiary: #9CA3AF;
    --color-text-inverse: #111827;
    --color-text-disabled: #6B7280;
    
    --color-border-default: #4B5563;
    --color-border-subtle: #374151;
    --color-border-strong: #6B7280;
    
    --shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.3);
    --shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.4), 0 2px 4px -2px rgb(0 0 0 / 0.3);
    --shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.4), 0 4px 6px -4px rgb(0 0 0 / 0.3);
    --shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.4), 0 8px 10px -6px rgb(0 0 0 / 0.3);
  }
  
  * {
    @apply border-border;
  }
  
  body {
    @apply bg-surface-base text-text-primary;
    font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  }
}
```

---

## 3. Layout Primitives

### 3.1 Stack Component

**Purpose**: Vertical layout with consistent spacing

```typescript
// src/layouts/primitives/Stack.tsx
interface StackProps {
  children: React.ReactNode;
  gap?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl';
  align?: 'start' | 'center' | 'end' | 'stretch';
  className?: string;
}

// Usage: <Stack gap="md" align="start">...</Stack>
```

### 3.2 Row Component

**Purpose**: Horizontal layout with consistent spacing

```typescript
// src/layouts/primitives/Row.tsx
interface RowProps {
  children: React.ReactNode;
  gap?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl';
  align?: 'start' | 'center' | 'end' | 'baseline' | 'stretch';
  justify?: 'start' | 'center' | 'end' | 'between' | 'around' | 'evenly';
  wrap?: boolean;
  className?: string;
}

// Usage: <Row gap="sm" justify="between" align="center">...</Row>
```

### 3.3 Grid Component

**Purpose**: CSS Grid layout with responsive columns

```typescript
// src/layouts/primitives/Grid.tsx
interface GridProps {
  children: React.ReactNode;
  cols?: 1 | 2 | 3 | 4 | 6 | 12;
  gap?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl';
  responsive?: {
    sm?: 1 | 2 | 3 | 4 | 6 | 12;
    md?: 1 | 2 | 3 | 4 | 6 | 12;
    lg?: 1 | 2 | 3 | 4 | 6 | 12;
  };
  className?: string;
}

// Usage: <Grid cols={3} gap="lg" responsive={{ sm: 1, md: 2 }}>...</Grid>
```

### 3.4 Container Component

**Purpose**: Max-width container with responsive padding

```typescript
// src/layouts/primitives/Container.tsx
interface ContainerProps {
  children: React.ReactNode;
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full';
  padding?: boolean;
  className?: string;
}

// Usage: <Container size="lg" padding>...</Container>
```

---

## 4. Component Hierarchy

### 4.1 Atoms

**Button Component**
- Variants: `primary`, `secondary`, `outline`, `ghost`, `danger`
- Sizes: `sm`, `md`, `lg`
- States: `default`, `hover`, `active`, `disabled`, `loading`
- Both light and dark mode styles defined

**Input Component**
- Types: `text`, `email`, `password`, `number`, `search`, `tel`, `url`
- States: `default`, `focus`, `error`, `disabled`
- Sizes: `sm`, `md`, `lg`

**Heading Component**
- Levels: `h1`, `h2`, `h3`, `h4`, `h5`, `h6`
- Variants: `default`, `display`, `section`
- Semantic HTML with proper hierarchy

**Text Component**
- Variants: `body`, `caption`, `label`
- Sizes: `xs`, `sm`, `base`, `lg`, `xl`
- Colors: `primary`, `secondary`, `tertiary`, `inverse`, `disabled`

**Icon Component**
- SVG-based icon system
- Sizes: `xs`, `sm`, `md`, `lg`, `xl`
- Colors inherit from parent or explicit prop

**Badge Component**
- Variants: `default`, `success`, `warning`, `error`, `info`
- Sizes: `sm`, `md`, `lg`

**Avatar Component**
- Sizes: `xs`, `sm`, `md`, `lg`, `xl`
- Fallback to initials
- Image loading states

**Spinner Component**
- Sizes: `sm`, `md`, `lg`
- Colors: `primary`, `secondary`, `white`

### 4.2 Molecules

**FormField Component**
- Composition: Label + Input + Error message
- Handles validation state
- Accessible with proper ARIA attributes

**SearchBar Component**
- Composition: Icon + Input + Clear button
- Debounced search functionality
- Keyboard navigation support

**Card Component**
- Subcomponents: CardHeader, CardBody, CardFooter
- Variants: `default`, `elevated`, `outlined`
- Hover states for interactive cards

**EmptyState Component**
- Composition: Icon + Heading + Text + Optional action button
- Used for empty lists, no results, etc.

**Alert Component**
- Variants: `info`, `success`, `warning`, `error`
- Dismissible option
- Icon + Message + Close button

**Dropdown Component**
- Built on Headless UI Menu
- Keyboard navigation
- Positioning with Floating UI

**Modal Component**
- Built on Headless UI Dialog
- Subcomponents: ModalHeader, ModalBody, ModalFooter
- Focus trap and scroll lock
- Backdrop click to close

**Tabs Component**
- Built on Headless UI Tab
- Keyboard navigation
- Variants: `line`, `enclosed`, `pills`

### 4.3 Organisms

**Navigation Components**

**AppHeader**
- Logo/brand
- Main navigation links
- User menu dropdown
- Theme toggle
- Responsive mobile menu

**Sidebar**
- Collapsible navigation
- Active state indication
- Icon + label navigation items
- Nested navigation support

**NavItem**
- Active/inactive states
- Icon support
- Badge/count support
- Keyboard accessible

**UserMenu**
- User avatar
- User info display
- Dropdown with actions (Profile, Settings, Logout)

**Domain-Specific Cards**

**VenueCard**
- Venue name and address
- Seating capacity
- Location map preview
- Actions: View, Edit, Delete

**ActCard**
- Act name
- Performance type/genre
- Upcoming shows count
- Actions: View, Edit, Delete

**ShowCard**
- Show date and time
- Venue and act information
- Ticket availability
- Distance from user (if location search)
- Actions: View details, Book tickets

**Domain-Specific Forms**

**VenueForm**
- Name, address fields
- Location picker (map integration)
- Seating capacity
- Description textarea
- Form validation

**ActForm**
- Name field
- Genre/type selection
- Description textarea
- Form validation

**ShowForm**
- Date/time picker
- Venue selection dropdown
- Act selection dropdown
- Form validation

**DataTable**
- Generic reusable table
- Sorting, filtering, pagination
- Row selection
- Responsive (cards on mobile)
- Empty state handling

---

## 5. Navigation Structure

### 5.1 Route Configuration

```typescript
// src/config/routes.config.ts
export const routes = {
  auth: {
    login: '/login',
  },
  dashboard: '/',
  venues: {
    list: '/venues',
    detail: '/venues/:id',
    create: '/venues/new',
    edit: '/venues/:id/edit',
  },
  acts: {
    list: '/acts',
    detail: '/acts/:id',
    create: '/acts/new',
    edit: '/acts/:id/edit',
  },
  shows: {
    list: '/shows',
    detail: '/shows/:id',
    create: '/shows/new',
    edit: '/shows/:id/edit',
  },
} as const;
```

### 5.2 Navigation Menu Structure

```typescript
// Primary navigation items
const navigationItems = [
  {
    label: 'Dashboard',
    path: '/',
    icon: 'HomeIcon',
  },
  {
    label: 'Venues',
    path: '/venues',
    icon: 'BuildingIcon',
    badge: venueCount, // Optional count badge
  },
  {
    label: 'Acts',
    path: '/acts',
    icon: 'UsersIcon',
    badge: actCount,
  },
  {
    label: 'Shows',
    path: '/shows',
    icon: 'CalendarIcon',
    badge: upcomingShowsCount,
  },
];
```

### 5.3 Application Layout Structure

```
┌─────────────────────────────────────────────────────┐
│ AppHeader (fixed top)                               │
│ ┌─────────┐  Navigation Links  ┌──────────────┐   │
│ │  Logo   │  [Venues] [Acts]   │ Theme | User │   │
│ └─────────┘  [Shows]            └──────────────┘   │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌──────────┐  ┌──────────────────────────────┐   │
│  │          │  │                              │   │
│  │ Sidebar  │  │  Main Content Area           │   │
│  │          │  │  (Page-specific content)     │   │
│  │ [Home]   │  │                              │   │
│  │ [Venues] │  │                              │   │
│  │ [Acts]   │  │                              │   │
│  │ [Shows]  │  │                              │   │
│  │          │  │                              │   │
│  └──────────┘  └──────────────────────────────┘   │
│                                                     │
└─────────────────────────────────────────────────────┘
```

**Responsive Behavior:**
- Desktop (≥1024px): Sidebar visible, full navigation
- Tablet (768px-1023px): Collapsible sidebar
- Mobile (<768px): Hamburger menu, full-screen overlay navigation

---

## 6. Implementation Guidelines

### 6.1 Component Development Rules

1. **Atoms must define both light and dark mode styles**
   - Use Tailwind's `dark:` prefix for dark mode variants
   - Never use inline conditional color logic
   - All visual states must be explicitly defined

2. **Layout primitives handle spacing, not atoms**
   - Atoms should not have external margins
   - Use layout primitives (Stack, Row, Grid) for spacing
   - Atoms can have internal padding

3. **Molecules compose atoms, don't restyle them**
   - Use atom variants instead of overriding styles
   - Minimal layout logic (flex/grid for internal arrangement)
   - No new color or typography definitions

4. **Organisms arrange molecules and atoms**
   - Use layout primitives for structure
   - Choose variants, don't create new styles
   - Responsive behavior through layout primitives

5. **Pages are composition-only**
   - Wire organisms to data
   - Handle routing and loading states
   - No custom CSS or styling

### 6.2 Styling Conventions

**Tailwind Class Organization:**
```typescript
// Use cn() utility for conditional classes
import { cn } from '@/utils/cn';

const buttonClasses = cn(
  // Base styles
  'inline-flex items-center justify-center',
  'rounded-md font-medium transition-colors',
  'focus-visible:outline-none focus-visible:ring-2',
  
  // Variant styles
  variant === 'primary' && 'bg-brand-primary text-text-inverse',
  variant === 'secondary' && 'bg-surface-elevated text-text-primary',
  
  // Size styles
  size === 'sm' && 'h-9 px-3 text-sm',
  size === 'md' && 'h-10 px-4 text-base',
  
  // State styles
  disabled && 'opacity-50 cursor-not-allowed',
  
  // Custom classes
  className
);
```

**Component File Structure:**
```typescript
// Button.tsx
import { cn } from '@/utils/cn';
import type { ButtonProps } from './Button.types';

export function Button({
  variant = 'primary',
  size = 'md',
  disabled = false,
  loading = false,
  children,
  className,
  ...props
}: ButtonProps) {
  return (
    <button
      className={cn(/* classes */)}
      disabled={disabled || loading}
      {...props}
    >
      {loading && <Spinner size="sm" />}
      {children}
    </button>
  );
}
```

### 6.3 Theme Context Usage

```typescript
// src/theme/ThemeProvider.tsx
import { createContext, useContext, useState, useEffect } from 'react';

type Theme = 'light' | 'dark';

interface ThemeContextValue {
  theme: Theme;
  setTheme: (theme: Theme) => void;
  toggleTheme: () => void;
}

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined);

export function ThemeProvider({ children }: { children: React.ReactNode }) {
  const [theme, setTheme] = useState<Theme>('light');
  
  useEffect(() => {
    const root = window.document.documentElement;
    root.classList.remove('light', 'dark');
    root.classList.add(theme);
  }, [theme]);
  
  const toggleTheme = () => {
    setTheme(prev => prev === 'light' ? 'dark' : 'light');
  };
  
  return (
    <ThemeContext.Provider value={{ theme, setTheme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

export function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within ThemeProvider');
  }
  return context;
}
```

### 6.4 Accessibility Requirements

1. **Keyboard Navigation**
   - All interactive elements must be keyboard accessible
   - Proper focus indicators (using Tailwind's `focus-visible:` utilities)
   - Logical tab order

2. **ARIA Attributes**
   - Proper roles for custom components
   - aria-label for icon-only buttons
   - aria-describedby for form errors
   - aria-live regions for dynamic content

3. **Color Contrast**
   - WCAG AA compliance minimum (4.5:1 for normal text)
   - Test both light and dark modes
   - Use tokens that meet contrast requirements

4. **Screen Reader Support**
   - Semantic HTML elements
   - Descriptive labels
   - Hidden text for context where needed

### 6.5 Responsive Design Strategy

**Mobile-First Approach:**
```typescript
// Base styles for mobile, then override for larger screens
<div className="
  flex flex-col gap-4
  md:flex-row md:gap-6
  lg:gap-8
">
```

**Breakpoint Usage:**
- `sm` (640px): Small tablets, large phones in landscape
- `md` (768px): Tablets
- `lg` (1024px): Desktops, laptops
- `xl` (1280px): Large desktops
- `2xl` (1536px): Extra large screens

**Component Responsiveness:**
- Navigation: Hamburger menu on mobile, full nav on desktop
- Cards: Stack on mobile, grid on tablet/desktop
- Forms: Full width on mobile, constrained on desktop
- Tables: Card view on mobile, table on desktop

### 6.6 Performance Considerations

1. **Code Splitting**
   - Lazy load pages with React.lazy()
   - Lazy load heavy organisms (DataTable, Maps)
   - Route-based code splitting

2. **Image Optimization**
   - Use appropriate image formats (WebP with fallbacks)
   - Lazy load images below the fold
   - Responsive images with srcset

3. **Bundle Size**
   - Tree-shake unused Tailwind classes (automatic with Tailwind)
   - Import only needed Headless UI components
   - Monitor bundle size with Vite's build analysis

---

## 7. Development Workflow

### 7.1 Implementation Order

1. **Phase 1: Foundation**
   - Set up Tailwind configuration
   - Implement theme tokens and CSS variables
   - Create ThemeProvider and context
   - Build layout primitives (Stack, Row, Grid, Container)

2. **Phase 2: Atoms**
   - Button (all variants and states)
   - Input (all types and states)
   - Heading, Text
   - Icon system
   - Badge, Avatar, Spinner

3. **Phase 3: Molecules**
   - FormField
   - SearchBar
   - Card components
   - Alert
   - Dropdown (with Headless UI)
   - Modal (with Headless UI)
   - Tabs (with Headless UI)

4. **Phase 4: Organisms**
   - Navigation components (AppHeader, Sidebar, NavItem, UserMenu)
   - Domain cards (VenueCard, ActCard, ShowCard)
   - Domain forms (VenueForm, ActForm, ShowForm)
   - DataTable

5. **Phase 5: Layouts & Pages**
   - AppLayout
   - AuthLayout
   - Page components (list, detail, create, edit for each domain)

6. **Phase 6: Integration**
   - API client extensions
   - TanStack Query hooks
   - Routing setup
   - Authentication flow

### 7.2 Testing Strategy

**Component Testing:**
- Test atoms in isolation with all variants
- Test light and dark mode rendering
- Test keyboard navigation and accessibility
- Test responsive behavior

**Integration Testing:**
- Test page flows (create, edit, delete)
- Test navigation between pages
- Test form validation and submission
- Test error states and loading states

### 7.3 Documentation Requirements

Each component should include:
- TypeScript interface documentation
- Usage examples
- Variant showcase
- Accessibility notes
- Responsive behavior notes

---

## 8. Dependencies to Install

```json
{
  "dependencies": {
    "@headlessui/react": "^2.2.0",
    "@tanstack/react-query": "^5.90.10",
    "@tanstack/react-query-devtools": "^5.91.0",
    "react": "^19.2.0",
    "react-dom": "^19.2.0",
    "react-router-dom": "^7.1.1",
    "clsx": "^2.1.1",
    "tailwind-merge": "^2.7.0"
  },
  "devDependencies": {
    "@types/react": "^19.2.5",
    "@types/react-dom": "^19.2.3",
    "@vitejs/plugin-react": "^5.1.1",
    "autoprefixer": "^10.4.20",
    "postcss": "^8.4.49",
    "tailwindcss": "^3.4.17",
    "typescript": "~5.9.3",
    "vite": "^7.2.4"
  }
}
```

---

## 9. File Creation Checklist

### Configuration Files
- [ ] `tailwind.config.ts` - Tailwind configuration with custom tokens
- [ ] `postcss.config.js` - PostCSS configuration for Tailwind
- [ ] `src/config/theme.config.ts` - Theme configuration
- [ ] `src/config/routes.config.ts` - Route definitions

### Theme Layer
- [ ] `src/theme/tokens.ts` - Design token definitions
- [ ] `src/theme/ThemeProvider.tsx` - Theme context provider
- [ ] `src/theme/types.ts` - Theme TypeScript types
- [ ] `src/index.css` - Updated with CSS variables and Tailwind imports

### Layout Primitives
- [ ] `src/layouts/primitives/Stack.tsx`
- [ ] `src/layouts/primitives/Row.tsx`
- [ ] `src/layouts/primitives/Column.tsx`
- [ ] `src/layouts/primitives/Grid.tsx`
- [ ] `src/layouts/primitives/Container.tsx`
- [ ] `src/layouts/primitives/Spacer.tsx`
- [ ] `src/layouts/primitives/index.ts`

### Layouts
- [ ] `src/layouts/AppLayout.tsx`
- [ ] `src/layouts/AuthLayout.tsx`
- [ ] `src/layouts/index.ts`

### Utilities
- [ ] `src/utils/cn.ts` - Class name utility (clsx + tailwind-merge)
- [ ] `src/utils/format.ts` - Formatting utilities
- [ ] `src/utils/validation.ts` - Validation helpers
- [ ] `src/utils/index.ts`

### Hooks
- [ ] `src/hooks/useTheme.ts` - Theme hook
- [ ] `src/hooks/useMediaQuery.ts` - Responsive hook
- [ ] `src/hooks/useDebounce.ts` - Debounce hook
- [ ] `src/hooks/index.ts`

---

## 10. Success Criteria

The design system architecture is complete when:

1. ✅ All design tokens are defined with explicit light and dark mode values
2. ✅ Layout primitives provide consistent spacing and structure
3. ✅ Component hierarchy follows atomic design principles
4. ✅ Navigation structure supports all PRD requirements
5. ✅ Styling approach is consistent (Tailwind + CSS variables)
6. ✅ Accessibility requirements are documented
7. ✅ Responsive design strategy is defined
8. ✅ Implementation order is clear and logical
9. ✅ All dependencies are identified
10. ✅ File structure supports scalability and maintainability

---

## 11. Next Steps

After this architecture is approved:

1. **Switch to Code mode** to begin implementation
2. **Start with Phase 1** (Foundation: Tailwind config, theme tokens, layout primitives)
3. **Build incrementally** following the implementation order
4. **Test each layer** before moving to the next
5. **Document components** as they are built

This architecture provides a solid foundation for building a maintainable, accessible, and scalable design system that follows atomic design principles and supports both light and dark modes explicitly.