# Core Concepts

Use when defining frontend architecture for a new feature before implementation.

- **File-Based Routing**: Organize pages under `src/pages/<feature>/` with nested folders for dynamic segments and a shared `_layout.tsx` where appropriate.
- **Separation of Concerns**: Keep routing, data fetching, and UI components decoupled. Use loaders for prefetch, Query hooks for runtime data, and components for presentation.
- **Component Taxonomy**: Classify components as Atoms, Molecules, Organisms. Atoms are reusable UI primitives; Molecules combine atoms; Organisms introduce business logic.
- **Design System Boundary**: Frontend Architect specifies requirements; Design System Engineer owns low-level atoms/molecules. Do not implement DS components while drafting features.
- **Security-First**: Route-level and component-level permission checks are explicit and centralized.
