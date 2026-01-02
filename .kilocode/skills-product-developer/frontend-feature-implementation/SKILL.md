---
name: frontend-feature-implementation
description: Best practices for implementing features. Use when building pages, wiring API integrations, or assembling organisms.
---

# Frontend Feature Implementation Patterns

## Role & Responsibilities
The Product Developer assembles the application using the blueprint (FTS) and parts (Design System).
- **Input**: FTS and Component Library.
- **Output**: Working features in `src/pages` and `src/components/organisms`.
- **Goal**: Deliver functional user value.

## Page Assembly
- **Layout**: Use `src/templates/AppLayout.tsx` for consistent page structure.
- **Loading States**: Handle loading states from TanStack Query gracefully (skeletons or spinners).
- **Error States**: Display user-friendly error messages when API calls fail.

## Forms (React Hook Form)
- **Validation**: Use Zod schemas to define validation rules.
- **Integration**: Connect forms to `useMutation` hooks.
- **UX**: Disable submit buttons while `isPending` is true.

## API Integration
- **Client**: Use functions from `src/api/client.ts`.
- **Hooks**: Create custom hooks for reusable data logic (e.g., `useVenue(id)`).
- **Mapbox**: When using maps, handle the `onLoad` event and ensure markers are cleaned up.

## Testability
- **Data Attributes**: Add `data-testid="feature-name"` to key interactive elements.
- **Accessibility**: Ensure form fields have associated labels.

