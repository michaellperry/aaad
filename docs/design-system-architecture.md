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

The application follows a layered architecture organized by responsibility:

```
src/GloboTicket.Web/src/
├── main.tsx & App.tsx              # Application entry
├── index.css                       # Global styles + Tailwind
│
├── config/                         # Configuration files
├── theme/                          # Design tokens + ThemeProvider
├── layouts/                        # Layout primitives + page layouts
├── components/                     # Atomic design components
│   ├── atoms/                      # Primitive UI elements
│   ├── molecules/                  # Composite UI patterns
│   └── organisms/                  # Complex components
│
├── pages/                          # Route-level page components
│   ├── auth/, venues/, acts/, shows/, dashboard/
│
├── features/                       # Feature-specific logic
│   ├── auth/, venues/, acts/, shows/
│   │   ├── hooks/                  # TanStack Query hooks
│   │   └── context/                # Feature-specific context
│
├── api/                            # API client + endpoints
├── types/                          # TypeScript type definitions
├── utils/                          # Utility functions
└── hooks/                          # Shared custom hooks
```

**Organizational Principles:**

1. **Atomic Design**: Components organized by complexity (atoms → molecules → organisms)
2. **Feature Colocation**: Domain logic grouped by feature (venues, acts, shows)
3. **Separation of Concerns**: Theme, layout, components, and data each in dedicated directories
4. **Barrel Exports**: Each directory exports via `index.ts` for clean imports

**Component File Pattern:**
- Each component in its own folder
- `ComponentName.tsx` - Implementation
- `ComponentName.types.ts` - TypeScript interfaces
- `index.ts` - Barrel export

---

## 2. Theme Token System

### 2.1 Design Token Strategy

**Location**: `src/theme/tokens.ts`

Design tokens are the single source of truth for all visual properties. They are organized into semantic categories:

**Color Categories:**
- **Brand**: Primary and secondary brand colors
- **Semantic**: Success, warning, error, info states
- **Surface**: Base, elevated, and overlay backgrounds
- **Text**: Primary through disabled text hierarchies
- **Border**: Default, subtle, and strong borders

**Other Token Categories:**
- **Spacing**: Consistent spacing scale (xs through 3xl)
- **Typography**: Font families, sizes, weights, and line heights
- **Border Radius**: Consistent rounding (sm through full)
- **Shadows**: Elevation system (sm through xl)
- **Transitions**: Animation timing (fast, base, slow)
- **Breakpoints**: Responsive design breakpoints

**Key Principle**: Every color token has explicit light and dark mode values. No runtime color calculations.

### 2.2 Theme Implementation Approach

**CSS Variables Bridge**: Tokens are exposed to Tailwind via CSS variables in `src/index.css`:
- Variables defined in `:root` for light mode (default)
- Variables redefined in `.dark` class for dark mode
- Tailwind references these variables via `var(--token-name)`

**Benefits of this approach:**
- Single theme switch (add/remove `.dark` class on `<html>`)
- No duplicate class names (`dark:bg-x`)
- Theme changes update globally
- Runtime theme switching without style recalculation

### 2.3 Tailwind Configuration

**Location**: `tailwind.config.ts`

Tailwind is configured to:
- Use `class` strategy for dark mode (explicit opt-in)
- Extend theme with semantic tokens (not override defaults)
- Reference CSS variables for all custom values
- Maintain access to standard Tailwind utilities

---

## 3. Layout Primitives

Layout primitives are presentational components that handle spacing and structure without styling. They eliminate the need for custom margins and provide consistent layouts throughout the application.

### 3.1 Core Principles

1. **Layout components handle spacing, atoms do not** - Atoms should never have external margins
2. **Composable hierarchy** - Layout primitives can nest to create complex layouts
3. **Responsive by default** - All primitives support responsive props
4. **Type-safe gaps** - Only token-based spacing values allowed

### 3.2 Available Primitives

| Primitive | Purpose | Key Props |
|-----------|---------|-----------|
| **Stack** | Vertical layout with flex-col | `gap`, `align` |
| **Row** | Horizontal layout with flex-row | `gap`, `align`, `justify`, `wrap` |
| **Grid** | CSS Grid with columns | `cols`, `gap`, `responsive` |
| **Container** | Max-width content wrapper | `size`, `padding` |
| **Column** | Grid column with span control | `span`, `start` |
| **Spacer** | Flexible spacing element | `size` |

### 3.3 Usage Philosophy

**Bad Practice** (atoms with margins):
```tsx
<Button className="mb-4">Submit</Button>
<Text className="mt-2">Description</Text>
```

**Good Practice** (layout primitives):
```tsx
<Stack gap="md">
  <Button>Submit</Button>
  <Text>Description</Text>
</Stack>
```

This approach keeps atoms reusable and layout concerns separated.

---

## 4. Component Hierarchy

The component system follows strict atomic design principles with clear responsibilities at each level.

### 4.1 Atoms (Primitive UI Elements)

Atoms are the smallest, indivisible UI components. They:
- Define ALL visual styling (light + dark modes explicitly)
- Accept variant and size props for consistent variations
- Have NO external margins or layout responsibilities
- Are fully accessible and keyboard navigable
- Handle their own internal states (hover, focus, active, disabled)

**Complete Atom Inventory:**

| Atom | Variants | Sizes | Key Features |
|------|----------|-------|--------------|
| **Button** | primary, secondary, outline, ghost, danger | sm, md, lg | Loading state, icon support |
| **Input** | (type variants) | sm, md, lg | Error state, prefix/suffix |
| **Heading** | default, display, section | h1-h6 | Semantic HTML |
| **Text** | body, caption, label | xs-xl | Color semantic tokens |
| **Icon** | - | xs-xl | SVG-based, inherits color |
| **Badge** | default, success, warning, error, info | sm, md, lg | Pill/square variants |
| **Avatar** | - | xs-xl | Initials fallback |
| **Spinner** | - | sm, md, lg | Smooth animations |

**Critical Atom Requirements:**
- Must work standalone in Storybook/isolation
- Must define styles for both `.dark` and default modes
- Must use only design tokens (no arbitrary values)
- Must handle all interactive states explicitly

### 4.2 Molecules (Composite UI Patterns)

Molecules combine multiple atoms into functional UI patterns. They:
- Compose atoms without restyling them (use atom variants instead)
- Add minimal internal layout logic (flex/grid for arrangement)
- Handle interaction patterns (forms, search, modals)
- Delegate complex behavior to Headless UI libraries
- Are still domain-agnostic and reusable

**Molecule Inventory:**

| Molecule | Composition | Purpose | Technology |
|----------|-------------|---------|------------|
| **FormField** | Label + Input + Error | Form input with validation | Native HTML + ARIA |
| **SearchBar** | Icon + Input + Clear button | Search with debounce | Custom hook |
| **Card** | Header + Body + Footer | Content containers | Layout primitives |
| **EmptyState** | Icon + Heading + Text + Button | No data scenarios | Atoms only |
| **Alert** | Icon + Message + Close | User notifications | Atoms + dismiss logic |
| **Dropdown** | Trigger + Menu + Items | Selection menus | Headless UI Menu |
| **Modal** | Header + Body + Footer | Dialogs/overlays | Headless UI Dialog |
| **Tabs** | Tab list + Panels | Content switching | Headless UI Tab |

**Molecule Responsibilities:**
- **Pattern composition**: Arrange atoms into common patterns
- **Accessibility**: Ensure ARIA attributes and keyboard navigation
- **Internal state**: Simple local state (expanded, selected, dismissed)
- **NO business logic**: No API calls, no domain knowledge

### 4.3 Organisms (Complex Components)

Organisms are substantial UI sections that combine molecules and atoms. They CAN be domain-specific and may connect to data/business logic.

**Two Types of Organisms:**

1. **Structural Organisms** (Navigation, Layout)
   - Reusable across the application
   - Handle app-wide concerns (navigation, user session)
   - Example: AppHeader, Sidebar, UserMenu

2. **Domain Organisms** (Cards, Forms, Tables)
   - Specific to business domain (Venues, Acts, Shows)
   - Display and manipulate domain data
   - Example: VenueCard, ShowForm, DataTable

**Organism Architecture:**

| Category | Components | Domain Knowledge | Data Connection |
|----------|------------|------------------|-----------------|
| **Navigation** | AppHeader, Sidebar, NavItem, UserMenu | None (structural) | Auth context only |
| **Domain Cards** | VenueCard, ActCard, ShowCard | High (entity-specific) | Props only (presentational) |
| **Domain Forms** | VenueForm, ActForm, ShowForm | High (entity-specific) | Form state + callbacks |
| **Data Display** | DataTable | Low (generic) | Props-based data |

**Key Principles:**
- Organisms use **Layout Primitives** for structure (Stack, Row, Grid)
- Organisms select **atom/molecule variants** (don't create new styles)
- Domain organisms are **presentational** (receive data via props)
- Data fetching happens in **pages** or **feature hooks**, not organisms

**Responsiveness Strategy:**
- Navigation: Hamburger menu (mobile) → Full nav (desktop)
- Cards: Stack (mobile) → Grid (tablet+)
- Forms: Full-width (mobile) → Constrained (desktop)
- Tables: Card view (mobile) → Table (desktop)

---

## 5. Navigation Structure

### 5.1 Route Architecture

Routes are centrally configured in `src/config/routes.config.ts` for consistency and type safety.

**Route Pattern:**
- List views: `/resource` (e.g., `/venues`)
- Detail views: `/resource/:id` (e.g., `/venues/123`)
- Create: `/resource/new` (e.g., `/venues/new`)
- Edit: `/resource/:id/edit` (e.g., `/venues/123/edit`)

**Primary Resources:**
1. Dashboard: `/` (home page)
2. Venues: `/venues/*`
3. Acts: `/acts/*`
4. Shows: `/shows/*`
5. Auth: `/login`

### 5.2 Navigation Menu Hierarchy

**Primary Navigation Items:**
- Dashboard (Home icon)
- Venues (Building icon, count badge)
- Acts (Users icon, count badge)
- Shows (Calendar icon, upcoming count badge)

**User Menu Items:**
- Profile
- Settings  
- Theme Toggle
- Logout

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

| Level | Styling Rules | Layout Rules | Data Rules |
|-------|---------------|--------------|------------|
| **Atoms** | Define all styles (light + dark) | No external margins | No data fetching |
| **Molecules** | Use atom variants only | Minimal internal layout | Simple local state only |
| **Organisms** | Select variants, no new styles | Use layout primitives | Presentational (props-based) |
| **Pages** | No styling at all | Composition only | Data fetching + routing |

### 6.2 Styling Conventions

**Use the `cn()` utility** for conditional Tailwind classes:
- Location: `src/utils/cn.ts`
- Combines `clsx` and `tailwind-merge`
- Prevents Tailwind class conflicts

**Class Organization Order:**
1. Base styles (display, positioning)
2. Variant styles (colors, backgrounds)
3. Size styles (dimensions, padding)
4. State styles (hover, focus, disabled)
5. Custom className prop (override)

**Component File Organization:**
- `Component.tsx` - Main component implementation
- `Component.types.ts` - TypeScript interfaces and types
- `index.ts` - Barrel export

### 6.3 Theme Management

**ThemeProvider** (`src/theme/ThemeProvider.tsx`):
- Manages global theme state ('light' | 'dark')
- Applies/removes `.dark` class on `<html>`
- Persists preference to localStorage
- Provides `useTheme()` hook

**Theme Switching Mechanism:**
- No per-component theme logic
- Single class toggle on root element
- CSS variables automatically update
- Components reference semantic tokens only

### 6.4 Accessibility Requirements

| Requirement | Implementation | Tools |
|-------------|----------------|-------|
| **Keyboard Navigation** | Tab order, focus indicators | `focus-visible:` utilities |
| **ARIA Attributes** | Roles, labels, descriptions | Headless UI (built-in) |
| **Color Contrast** | WCAG AA (4.5:1 minimum) | Design tokens tested |
| **Screen Readers** | Semantic HTML, descriptive text | Native HTML elements |

**Critical Checks:**
- All interactive elements keyboard accessible
- Icon-only buttons have `aria-label`
- Form errors use `aria-describedby`
- Dynamic updates use `aria-live`
- Test with both light and dark modes

### 6.5 Responsive Design Strategy

**Mobile-First Approach**: Base styles target mobile, use breakpoint prefixes for larger screens.

**Breakpoints:**
- `sm` (640px): Large phones, small tablets
- `md` (768px): Tablets
- `lg` (1024px): Laptops, desktops
- `xl` (1280px): Large desktops
- `2xl` (1536px): Extra large screens

**Responsive Patterns:**
- Navigation: Hamburger → Full nav bar
- Card grids: 1 column → 2 columns → 3+ columns
- Forms: Full width → Centered constrained width
- Tables: Card list → Data table
- Sidebar: Hidden → Overlay → Persistent

### 6.6 Performance Considerations

**Optimization Strategies:**
- Route-based code splitting (React Router + Vite)
- Lazy load pages and heavy components (React.lazy)
- Image optimization (WebP, lazy loading, responsive images)
- Tree-shaking (Tailwind automatic, Headless UI component-level imports)
- Bundle analysis (Vite rollup visualizer)

---

## 7. Development Workflow

### 7.1 Implementation Phases

Build the design system from the foundation up, ensuring each layer is solid before building the next:

| Phase | Components | Dependencies | Validation |
|-------|------------|--------------|------------|
| **1. Foundation** | Tailwind config, Theme tokens, ThemeProvider, Layout primitives | None | Theme switching works |
| **2. Atoms** | Button, Input, Heading, Text, Icon, Badge, Avatar, Spinner | Phase 1 | Light + dark modes render |
| **3. Molecules** | FormField, SearchBar, Card, Alert, Dropdown, Modal, Tabs | Phases 1-2, Headless UI | Accessible, composable |
| **4. Organisms** | Navigation, Domain cards, Domain forms, DataTable | Phases 1-3 | Responsive, domain-aware |
| **5. Layouts & Pages** | AppLayout, AuthLayout, Page components | Phases 1-4 | Full page rendering |
| **6. Integration** | API hooks, Routing, Auth flow | Phases 1-5, TanStack Query | End-to-end flows work |

**Development Principles:**
- Build bottom-up (foundation → atoms → molecules → organisms → pages)
- Test each component in isolation before integration
- Validate accessibility at every level
- Document patterns as you build

### 7.2 Testing Strategy

**Per-Component Testing:**
- All variants render correctly
- Light and dark modes work
- Keyboard navigation functional
- Responsive breakpoints behave correctly

**Integration Testing:**
- Complete CRUD flows (create, read, update, delete)
- Navigation between pages
- Form validation and error handling
- Loading and error states

---

## 8. Key Dependencies

**Production Dependencies:**
- `@headlessui/react` - Accessible UI component primitives (Dropdown, Modal, Tabs)
- `@tanstack/react-query` - Server state management
- `react-router-dom` - Client-side routing
- `clsx` + `tailwind-merge` - Conditional class names utility

**Development Dependencies:**
- `tailwindcss` + `autoprefixer` + `postcss` - Styling toolchain
- `vite` - Build tool and dev server
- `typescript` - Type safety
- `@vitejs/plugin-react` - Vite React support

**Note:** React 19.2.0 is assumed as the base. All dependencies should use compatible versions.

---

## 9. Implementation Checklist

This checklist tracks foundational files that must be created before component development. All files live in `src/GloboTicket.Web/src/` unless noted.

**Configuration & Theme:**
- [ ] `tailwind.config.ts` - Custom token configuration
- [ ] `postcss.config.js` - Tailwind processing
- [ ] `index.css` - CSS variables (light/dark)
- [ ] `theme/tokens.ts` - Design token definitions
- [ ] `theme/ThemeProvider.tsx` - Theme context
- [ ] `config/routes.config.ts` - Centralized routes

**Layout Primitives:**
- [ ] `layouts/primitives/` - Stack, Row, Grid, Container, Column, Spacer + index

**Core Layouts:**
- [ ] `layouts/AppLayout.tsx` - Main authenticated layout
- [ ] `layouts/AuthLayout.tsx` - Login/auth pages layout

**Utilities:**
- [ ] `utils/cn.ts` - Class name merger (clsx + tailwind-merge)
- [ ] `utils/format.ts` - Date/number formatting
- [ ] `utils/validation.ts` - Form validation helpers

**Hooks:**
- [ ] `hooks/useTheme.ts` - Theme management
- [ ] `hooks/useMediaQuery.ts` - Responsive queries
- [ ] `hooks/useDebounce.ts` - Debounced values

**Note:** Component files (atoms, molecules, organisms) are tracked separately per implementation phase.

---

## 10. Architecture Validation

This architecture is complete when these principles are clearly defined:

✅ **Token System**: Explicit light/dark values, no runtime color calculations  
✅ **Component Hierarchy**: Clear atomic design levels with distinct responsibilities  
✅ **Styling Strategy**: Tailwind + CSS variables, consistent conventions  
✅ **Layout Philosophy**: Separation of spacing (primitives) from styling (atoms)  
✅ **Accessibility**: WCAG AA standards, keyboard navigation, ARIA support  
✅ **Responsiveness**: Mobile-first approach with defined breakpoint strategy  
✅ **Performance**: Code splitting, lazy loading, bundle optimization  
✅ **Implementation Path**: Clear phase ordering from foundation to integration  

---

## 11. Next Steps

**From Architecture to Implementation:**

1. Begin with Phase 1 (Foundation) - establish the base layer
2. Build bottom-up through phases 2-6
3. Validate each component in isolation before integration
4. Test accessibility and responsiveness at every level
5. Document component patterns as they emerge

**Key Success Factors:**
- Never skip a phase or build out of order
- Test light and dark modes for every component
- Use layout primitives consistently
- Reference existing code in this document, don't duplicate it
- Focus on reusability and composability

This architecture provides the strategic foundation. The code implements the details.