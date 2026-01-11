---
name: design-system-patterns
description: Design system patterns for reusable UI via atomic design, Tailwind tokens/variants, and accessibility. Use when creating atoms/molecules, theming, or hardening a11y.
---

# Design System Patterns

Concise guidance for building consistent, accessible atoms and molecules. Keep SKILL lean; open resources only when needed.

## When To Use
- Creating/updating atoms or molecules from a component gap analysis
- Defining tokens, variants, and dark mode support
- Enforcing accessibility (focus, keyboard, semantics, SR)
- Structuring components (polymorphic props, compound patterns) and adding tests/visual coverage

## Core Patterns
- **Atomic Scope**: DS owns atoms/molecules; organisms stay with product/feature teams.
- **Tokens/Variants**: Central tokens + CVA variants; always include dark mode.
- **Accessibility**: Focus-visible, keyboard flows, semantic labels, SR text.
- **Composition**: Polymorphic headings, compound components, safe error boundaries.
- **Testing**: RTL for behavior/accessibility; Storybook for visual regressions.

## Resources
- [Atomic Design](patterns/atomic-design.md) — load when classifying scope (atom vs molecule vs organism).
- [Styling, Tokens, and Variants](patterns/styling-tokens-and-variants.md) — load when defining tokens, CVA variants, dark mode.
- [Accessibility](patterns/accessibility.md) — load when enforcing focus, keyboard, semantics, SR support.
- [Component Structure](patterns/component-structure.md) — load when defining props, polymorphism, composition, error boundaries.
- [Testing](patterns/testing.md) — load when writing component tests or visual regression coverage.
