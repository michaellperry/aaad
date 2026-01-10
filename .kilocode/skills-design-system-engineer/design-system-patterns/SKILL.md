---
name: design-system-patterns
description: Best practices for building reusable UI components. Use when creating atoms, molecules, or updating the theme.
---

# Design System Patterns

## Role & Responsibilities
The Design System Engineer acts as the bridge between design and code.
- **Input**: Component Gap Analysis from the FTS.
- **Output**: Reusable components in `src/components/atoms` and `molecules`.
- **Goal**: Ensure visual consistency and accessibility.

## Atomic Design
- **Atoms**: Basic building blocks (Buttons, Inputs, Icons). No business logic.
- **Molecules**: Groups of atoms (Form Fields, Cards). Minimal logic.
- **Organisms**: Complex sections (Forms, Lists). **Product Developer territory**.

## Styling (Tailwind CSS)
- **Tokens**: Use values from `src/theme/tokens.ts` via Tailwind classes.
- **Utility Classes**: Use `clsx` and `tailwind-merge` for conditional styling.
- **Dark Mode**: Always implement `dark:` variants for every color.

## Accessibility (a11y)
- **Interactive Elements**: Must have focus states (`focus-visible:ring`).
- **Semantic HTML**: Use `<button>`, `<nav>`, `<main>`, etc.
- **ARIA**: Use `aria-label` or `aria-describedby` when text is insufficient.
- **Keyboard Navigation**: Ensure all interactions work without a mouse.

## Component Structure
```tsx
import { cn } from '@/utils/cn';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary';
}

export const Button = ({ className, variant = 'primary', ...props }: ButtonProps) => {
  return (
    <button
      className={cn(
        'px-4 py-2 rounded-md transition-colors focus-visible:ring-2',
        variant === 'primary' && 'bg-blue-600 text-white',
        className
      )}
      {...props}
    />
  );
};
```

