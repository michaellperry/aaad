# Styling, Tokens, and Variants

Use when defining tokens and component variants with Tailwind and CVA.

## Tokens
```typescript
export const tokens = {
  colors: {
    primary: { 50: '#eff6ff', 100: '#dbeafe', 500: '#3b82f6', 600: '#2563eb', 900: '#1e3a8a' },
    neutral: { 50: '#f9fafb', 100: '#f3f4f6', 500: '#6b7280', 900: '#111827' }
  },
  spacing: { xs: '0.5rem', sm: '0.75rem', md: '1rem', lg: '1.5rem', xl: '2rem' },
  borderRadius: { sm: '0.25rem', md: '0.375rem', lg: '0.5rem' }
} as const
```

## CVA Variants Example
```tsx
import { cva } from 'class-variance-authority'
import { cn } from '@/utils/cn'

const buttonVariants = cva(
  'inline-flex items-center justify-center rounded-md font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 disabled:pointer-events-none disabled:opacity-50',
  {
    variants: {
      variant: {
        primary: 'bg-primary-600 text-white hover:bg-primary-700 dark:bg-primary-500 dark:hover:bg-primary-600',
        secondary: 'bg-neutral-100 text-neutral-900 hover:bg-neutral-200 dark:bg-neutral-800 dark:text-neutral-100 dark:hover:bg-neutral-700',
        danger: 'bg-red-600 text-white hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600'
      },
      size: { sm: 'h-8 px-3 text-sm', md: 'h-10 px-4 text-sm', lg: 'h-12 px-6 text-base' }
    },
    defaultVariants: { variant: 'primary', size: 'md' }
  }
)

export const Button = ({ className, variant, size, ...props }: ButtonProps) => (
  <button className={cn(buttonVariants({ variant, size }), className)} {...props} />
)
```

## Dark Mode
```tsx
const cardVariants = cva('rounded-lg border shadow-sm', {
  variants: {
    variant: {
      default: 'bg-white border-neutral-200 dark:bg-neutral-900 dark:border-neutral-800',
      secondary: 'bg-neutral-50 border-neutral-200 dark:bg-neutral-800 dark:border-neutral-700'
    }
  }
})

export function ThemeToggle() {
  const [theme, setTheme] = useTheme()
  return (
    <Button variant="secondary" size="sm" onClick={() => setTheme(theme === 'dark' ? 'light' : 'dark')} aria-label={`Switch to ${theme === 'dark' ? 'light' : 'dark'} mode`}>
      {theme === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
    </Button>
  )
}
```
