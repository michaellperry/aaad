# Component Structure

Use when defining props, polymorphism, and composition.

## Prop Interfaces
```tsx
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  isLoading?: boolean
}

interface HeadingProps extends React.HTMLAttributes<HTMLHeadingElement> {
  as?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6'
  size?: 'sm' | 'md' | 'lg' | 'xl'
}
```

```tsx
export const Heading = ({ as: Component = 'h2', size = 'md', children, className, ...props }: HeadingProps) => (
  <Component className={cn(headingVariants({ size }), className)} {...props}>
    {children}
  </Component>
)
```

## Composition
```tsx
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

Card.Header = CardHeader
Card.Content = CardContent
export { Card }
```

## Error Boundary Integration
```tsx
export const SafeCard = ({ children, fallback, ...props }: SafeCardProps) => (
  <ErrorBoundary fallback={fallback || <CardError />}>
    <Card {...props}>{children}</Card>
  </ErrorBoundary>
)

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
