# Atomic Design

Use when classifying and scoping components before implementation.

- **Atoms (DS Engineer)**: Button, Input, Icon, Badge, Typography. No business logic.
- **Molecules (DS Engineer)**: SearchInput, FormField, Card. Light logic, reusable composition.
- **Organisms (Product Dev)**: Feature-specific forms, lists, navigation; business logic lives here, not in the design system.

## Classification Examples
```tsx
// Atom
interface ButtonProps { variant: 'primary' | 'secondary' | 'danger'; size: 'sm' | 'md' | 'lg'; children: React.ReactNode }

// Molecule
interface FormFieldProps { label: string; error?: string; children: React.ReactNode }

// Not DS scope (Organism)
interface VenueFormProps { venue?: Venue; onSubmit: (venue: CreateVenueDto) => void }
```
