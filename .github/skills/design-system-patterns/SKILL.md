---
name: design-system-patterns
description: Design system patterns for building reusable UI components using atomic design principles, Tailwind CSS, and accessibility standards. Use when creating atoms, molecules, implementing theme systems, or ensuring visual consistency.
---

# Design System Patterns

Best practices for building reusable UI components with atomic design, accessibility standards, and consistent styling patterns.

## Role & Responsibilities

The Design System Engineer acts as the bridge between design and code:
- **Input**: Component Gap Analysis from Frontend Technical Specs (FTS)
- **Output**: Reusable components in `src/components/atoms` and `molecules`
- **Goal**: Ensure visual consistency, accessibility, and maintainability

## Atomic Design Principles

### Component Hierarchy
```
Atoms (Design System Engineer)
├── Button, Input, Icon, Badge
├── Typography (Heading, Text, Label)
└── Base components with no business logic

Molecules (Design System Engineer)
├── SearchInput, FormField, Card
├── Groups of atoms with minimal logic
└── Reusable composite components

Organisms (Product Developer)
├── Forms, Lists, Navigation
├── Complex sections with business logic
└── Feature-specific implementations
```

### Component Classification
```tsx
// ✅ Atom - Basic building block
interface ButtonProps {
  variant: 'primary' | 'secondary' | 'danger'
  size: 'sm' | 'md' | 'lg'
  children: React.ReactNode
}

// ✅ Molecule - Combination of atoms
interface FormFieldProps {
  label: string
  error?: string
  children: React.ReactNode
}

// ❌ Organism - Business logic (not Design System scope)
interface VenueFormProps {
  venue?: Venue
  onSubmit: (venue: CreateVenueDto) => void
}
```

## Styling with Tailwind CSS

### Design Token Integration
```typescript
// src/theme/tokens.ts
export const tokens = {
  colors: {
    primary: {
      50: '#eff6ff',
      100: '#dbeafe',
      500: '#3b82f6',
      600: '#2563eb',
      900: '#1e3a8a'
    },
    neutral: {
      50: '#f9fafb',
      100: '#f3f4f6',
      500: '#6b7280',
      900: '#111827'
    }
  },
  spacing: {
    xs: '0.5rem',   // 8px
    sm: '0.75rem',  // 12px
    md: '1rem',     // 16px
    lg: '1.5rem',   // 24px
    xl: '2rem'      // 32px
  },
  borderRadius: {
    sm: '0.25rem',  // 4px
    md: '0.375rem', // 6px
    lg: '0.5rem'    // 8px
  }
} as const
```

### Utility Class Composition
```tsx
import { cn } from '@/utils/cn'
import { cva } from 'class-variance-authority'

// Button component with variants
const buttonVariants = cva(
  // Base classes
  'inline-flex items-center justify-center rounded-md font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 disabled:pointer-events-none disabled:opacity-50',
  {
    variants: {
      variant: {
        primary: 'bg-primary-600 text-white hover:bg-primary-700 dark:bg-primary-500 dark:hover:bg-primary-600',
        secondary: 'bg-neutral-100 text-neutral-900 hover:bg-neutral-200 dark:bg-neutral-800 dark:text-neutral-100 dark:hover:bg-neutral-700',
        danger: 'bg-red-600 text-white hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600'
      },
      size: {
        sm: 'h-8 px-3 text-sm',
        md: 'h-10 px-4 text-sm',
        lg: 'h-12 px-6 text-base'
      }
    },
    defaultVariants: {
      variant: 'primary',
      size: 'md'
    }
  }
)

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
}

export const Button = ({ 
  className, 
  variant, 
  size, 
  ...props 
}: ButtonProps) => {
  return (
    <button
      className={cn(buttonVariants({ variant, size }), className)}
      {...props}
    />
  )
}
```

### Dark Mode Implementation
```tsx
// Always implement dark variants
const cardVariants = cva(
  'rounded-lg border shadow-sm',
  {
    variants: {
      variant: {
        default: 'bg-white border-neutral-200 dark:bg-neutral-900 dark:border-neutral-800',
        secondary: 'bg-neutral-50 border-neutral-200 dark:bg-neutral-800 dark:border-neutral-700'
      }
    }
  }
)

// Theme toggle component
export function ThemeToggle() {
  const [theme, setTheme] = useTheme()
  
  return (
    <Button
      variant="secondary"
      size="sm"
      onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')}
      aria-label={`Switch to ${theme === 'dark' ? 'light' : 'dark'} mode`}
    >
      {theme === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
    </Button>
  )
}
```

## Accessibility Standards

### Interactive Element Requirements
```tsx
// ✅ Proper focus management
export const Button = ({ children, ...props }: ButtonProps) => {
  return (
    <button
      className={cn(
        'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2',
        'transition-colors duration-200'
      )}
      {...props}
    >
      {children}
    </button>
  )
}

// ✅ Keyboard navigation support
export const DropdownMenu = ({ children, trigger }: DropdownMenuProps) => {
  const [isOpen, setIsOpen] = useState(false)
  
  const handleKeyDown = (e: KeyboardEvent) => {
    if (e.key === 'Escape') {
      setIsOpen(false)
    }
    if (e.key === 'ArrowDown' && !isOpen) {
      setIsOpen(true)
    }
  }
  
  return (
    <div className="relative" onKeyDown={handleKeyDown}>
      <button
        className="focus-visible:ring-2 focus-visible:ring-primary-500"
        onClick={() => setIsOpen(!isOpen)}
        aria-expanded={isOpen}
        aria-haspopup="menu"
      >
        {trigger}
      </button>
      {isOpen && (
        <div role="menu" className="absolute z-50 mt-1 bg-white border rounded-md shadow-lg">
          {children}
        </div>
      )}
    </div>
  )
}
```

### Semantic HTML and ARIA
```tsx
// ✅ Semantic form field
export const FormField = ({ 
  label, 
  children, 
  error, 
  required = false 
}: FormFieldProps) => {
  const id = useId()
  const errorId = `${id}-error`
  
  return (
    <div className="space-y-2">
      <label 
        htmlFor={id}
        className="block text-sm font-medium text-neutral-700 dark:text-neutral-300"
      >
        {label}
        {required && <span className="text-red-500 ml-1" aria-label="required">*</span>}
      </label>
      
      {React.cloneElement(children, {
        id,
        'aria-invalid': !!error,
        'aria-describedby': error ? errorId : undefined
      })}
      
      {error && (
        <p id={errorId} className="text-sm text-red-600 dark:text-red-400" role="alert">
          {error}
        </p>
      )}
    </div>
  )
}

// ✅ Accessible input component
export const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ className, type = 'text', ...props }, ref) => {
    return (
      <input
        type={type}
        className={cn(
          'flex h-10 w-full rounded-md border border-neutral-300 bg-white px-3 py-2 text-sm',
          'placeholder:text-neutral-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500',
          'disabled:cursor-not-allowed disabled:opacity-50',
          'dark:border-neutral-700 dark:bg-neutral-900 dark:text-neutral-100 dark:placeholder:text-neutral-400',
          className
        )}
        ref={ref}
        {...props}
      />
    )
  }
)
```

### Screen Reader Support
```tsx
// ✅ Accessible loading button
export const LoadingButton = ({ 
  children, 
  isLoading = false, 
  ...props 
}: LoadingButtonProps) => {
  return (
    <Button {...props} disabled={isLoading || props.disabled}>
      {isLoading && (
        <Loader2 
          className="mr-2 h-4 w-4 animate-spin" 
          aria-hidden="true"
        />
      )}
      {children}
      {isLoading && <span className="sr-only">Loading...</span>}
    </Button>
  )
}

// ✅ Accessible icon button
export const IconButton = ({ 
  icon: Icon, 
  label, 
  ...props 
}: IconButtonProps) => {
  return (
    <Button 
      {...props}
      aria-label={label}
      className={cn('p-2', props.className)}
    >
      <Icon className="h-4 w-4" aria-hidden="true" />
      <span className="sr-only">{label}</span>
    </Button>
  )
}
```

## Component Structure Patterns

### Prop Interface Design
```tsx
// ✅ Extend native HTML props
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  isLoading?: boolean
}

// ✅ Polymorphic component pattern
interface HeadingProps extends React.HTMLAttributes<HTMLHeadingElement> {
  as?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6'
  size?: 'sm' | 'md' | 'lg' | 'xl'
}

export const Heading = ({ 
  as: Component = 'h2', 
  size = 'md', 
  children, 
  className,
  ...props 
}: HeadingProps) => {
  return (
    <Component
      className={cn(headingVariants({ size }), className)}
      {...props}
    >
      {children}
    </Component>
  )
}
```

### Composition Patterns
```tsx
// ✅ Compound component pattern
const Card = ({ children, className, ...props }: CardProps) => (
  <div className={cn(cardVariants(), className)} {...props}>
    {children}
  </div>
)

const CardHeader = ({ children, className, ...props }: CardHeaderProps) => (
  <div className={cn('p-6 pb-0', className)} {...props}>
    {children}
  </div>
)

const CardContent = ({ children, className, ...props }: CardContentProps) => (
  <div className={cn('p-6', className)} {...props}>
    {children}
  </div>
)

// Export as compound component
Card.Header = CardHeader
Card.Content = CardContent

export { Card }

// Usage
<Card>
  <Card.Header>
    <Heading as="h3">Venue Details</Heading>
  </Card.Header>
  <Card.Content>
    <p>Madison Square Garden</p>
  </Card.Content>
</Card>
```

### Error Boundary Integration
```tsx
// Component-specific error boundary
export const SafeCard = ({ children, fallback, ...props }: SafeCardProps) => {
  return (
    <ErrorBoundary fallback={fallback || <CardError />}>
      <Card {...props}>{children}</Card>
    </ErrorBoundary>
  )
}

const CardError = () => (
  <Card className="border-red-200 bg-red-50 dark:border-red-800 dark:bg-red-900/20">
    <Card.Content>
      <div className="flex items-center space-x-2 text-red-600 dark:text-red-400">
        <AlertCircle className="h-4 w-4" />
        <span>Failed to render card content</span>
      </div>
    </Card.Content>
  </Card>
)
```

## Testing Patterns

### Component Testing
```tsx
// Component test example
import { render, screen, userEvent } from '@testing-library/react'
import { Button } from './Button'

describe('Button', () => {
  it('renders with correct variant styles', () => {
    render(<Button variant="primary">Click me</Button>)
    
    const button = screen.getByRole('button', { name: /click me/i })
    expect(button).toHaveClass('bg-primary-600')
  })
  
  it('handles click events', async () => {
    const handleClick = vi.fn()
    render(<Button onClick={handleClick}>Click me</Button>)
    
    const button = screen.getByRole('button', { name: /click me/i })
    await userEvent.click(button)
    
    expect(handleClick).toHaveBeenCalledOnce()
  })
  
  it('is accessible via keyboard', async () => {
    const handleClick = vi.fn()
    render(<Button onClick={handleClick}>Click me</Button>)
    
    const button = screen.getByRole('button', { name: /click me/i })
    button.focus()
    
    await userEvent.keyboard('{Enter}')
    expect(handleClick).toHaveBeenCalledOnce()
    
    await userEvent.keyboard(' ')
    expect(handleClick).toHaveBeenCalledTimes(2)
  })
})
```

### Visual Regression Testing
```tsx
// Storybook story for visual testing
import type { Meta, StoryObj } from '@storybook/react'
import { Button } from './Button'

const meta: Meta<typeof Button> = {
  title: 'Atoms/Button',
  component: Button,
  parameters: {
    layout: 'centered',
  },
  argTypes: {
    variant: {
      control: { type: 'select' },
      options: ['primary', 'secondary', 'danger']
    }
  }
}

export default meta
type Story = StoryObj<typeof meta>

export const Primary: Story = {
  args: {
    children: 'Button',
    variant: 'primary'
  }
}

export const AllVariants: Story = {
  render: () => (
    <div className="space-x-2">
      <Button variant="primary">Primary</Button>
      <Button variant="secondary">Secondary</Button>
      <Button variant="danger">Danger</Button>
    </div>
  )
}
```

These design system patterns ensure consistent, accessible, and maintainable UI components that scale with your application while providing excellent developer and user experiences.