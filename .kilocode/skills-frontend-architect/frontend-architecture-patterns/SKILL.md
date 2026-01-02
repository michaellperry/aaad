---
name: frontend-architecture-patterns
description: Best practices for frontend architecture. Use when planning new features, defining routes, or designing data strategies.
---

# Frontend Architecture Patterns

## Role & Responsibilities
The Frontend Architect is responsible for the **HOW** of the implementation.
- **Output**: Frontend Technical Spec (FTS) in `docs/specs/`.
- **Goal**: Define the technical blueprint before any code is written.

## Frontend Technical Spec (FTS) Template
Every major feature must start with an FTS containing:
1. **Route Definition**: URL structure and React Router configuration.
2. **Data Strategy**: Query keys, mutation strategies, and cache invalidation.
3. **Component Gap Analysis**: List of new Atoms/Molecules vs. Organisms needed.
4. **Security/Auth**: Required roles and permission checks.

## Routing (React Router 7)
- Use file-based routing structure in `src/pages`.
- Protect routes using `ProtectedRoute`.
- Define loaders for data pre-fetching where applicable.

## State Management (TanStack Query)
- **Queries**: Define unique query keys in a factory pattern (if established) or consistent string arrays.
- **Mutations**: Always invalidate relevant queries after successful mutations.
- **Error Handling**: Use global error boundaries or specific `onError` callbacks.

## Component Gap Analysis
- Check `src/components/atoms` and `molecules` first.
- If a UI element doesn't exist, spec it out for the Design System Engineer.
- Do NOT implement low-level components; delegate them.

