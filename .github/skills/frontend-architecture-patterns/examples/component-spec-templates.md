# Component Spec Templates

Use when specifying new Molecules/Organisms for a feature.

```markdown
## Component: VenueCapacityInput (Molecule)

**Purpose**: Specialized input for venue capacity with validation and formatting

**Props**:
- `value: number` - Current capacity value
- `onChange: (value: number) => void` - Change handler
- `min?: number` - Minimum capacity (default: 1)
- `max?: number` - Maximum capacity (default: 100000)
- `error?: string` - Validation error message

**Behavior**:
- Format numbers with thousands separators
- Validate range on blur
- Show error state with message
- Disable input when loading

**Dependencies**: 
- Input (Atom)
- ErrorText (Atom)
```
