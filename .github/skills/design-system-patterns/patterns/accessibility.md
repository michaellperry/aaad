# Accessibility

Use when enforcing focus, keyboard support, semantics, and screen reader affordances.

## Interactive Elements
```tsx
export const Button = ({ children, ...props }: ButtonProps) => (
  <button
    className={cn('focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2', 'transition-colors duration-200')}
    {...props}
  >
    {children}
  </button>
)
```

## Keyboard Navigation
```tsx
export const DropdownMenu = ({ children, trigger }: DropdownMenuProps) => {
  const [isOpen, setIsOpen] = useState(false)
  const handleKeyDown = (e: KeyboardEvent) => {
    if (e.key === 'Escape') setIsOpen(false)
    if (e.key === 'ArrowDown' && !isOpen) setIsOpen(true)
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

## Semantic Fields
```tsx
export const FormField = ({ label, children, error, required = false }: FormFieldProps) => {
  const id = useId()
  const errorId = `${id}-error`
  return (
    <div className="space-y-2">
      <label htmlFor={id} className="block text-sm font-medium text-neutral-700 dark:text-neutral-300">
        {label}
        {required && <span className="text-red-500 ml-1" aria-label="required">*</span>}
      </label>
      {React.cloneElement(children, { id, 'aria-invalid': !!error, 'aria-describedby': error ? errorId : undefined })}
      {error && (
        <p id={errorId} className="text-sm text-red-600 dark:text-red-400" role="alert">
          {error}
        </p>
      )}
    </div>
  )
}
```

## Screen Reader Support
```tsx
export const LoadingButton = ({ children, isLoading = false, ...props }: LoadingButtonProps) => (
  <Button {...props} disabled={isLoading || props.disabled}>
    {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" aria-hidden="true" />}
    {children}
    {isLoading && <span className="sr-only">Loading...</span>}
  </Button>
)

export const IconButton = ({ icon: Icon, label, ...props }: IconButtonProps) => (
  <Button {...props} aria-label={label} className={cn('p-2', props.className)}>
    <Icon className="h-4 w-4" aria-hidden="true" />
    <span className="sr-only">{label}</span>
  </Button>
)
```
