---
name: frontend-architecture-patterns
description: Frontend architecture patterns for React apps (React Router 7, TanStack Query). Use when planning features, defining routes, data strategies, and component gaps.
---

# Frontend Architecture Patterns

Concise guidance for planning frontend features.

## When To Use
- Defining routes and loaders for a new feature
- Planning query keys, mutations, and error handling
- Auditing UI components and specifying gaps
- Designing permission checks at route and component levels

## Core Patterns
- **File-Based Routing**: Pages under `src/pages/<feature>/` with nested folders; shared `_layout.tsx` for feature.
- **Data Strategy**: Query key factory; prefetch via loaders; mutations with optimism + invalidation.
- **Component Taxonomy**: Atoms → Molecules → Organisms; DS owns atoms/molecules; feature specs define requirements.
- **Security-First**: Centralized permissions; protect routes; conditionally render privileged actions.

## Resources
- [Core Concepts](patterns/core-concepts.md) — load when drafting the FTS and overall architecture.
- [State Management](patterns/state-management.md) — load when defining query keys, queries, mutations, and error handling.
- [Security & Permissions](patterns/security-permissions.md) — load when specifying guards and permission-based rendering.
- [Routing Setup](examples/routing-setup.md) — load when wiring routes, loaders, and protection.
- [Component Spec Templates](examples/component-spec-templates.md) — load when specifying new Molecules/Organisms.
- [Permission Guards](examples/permission-guards.md) — load when rendering privileged UI.

## Frontend Technical Spec (FTS)
- **Output**: FTS document in docs/specs/ for each major feature
- **Goal**: Decide architecture before implementation
- **Scope**: Routing, data, permissions, component gaps — not low-level component code