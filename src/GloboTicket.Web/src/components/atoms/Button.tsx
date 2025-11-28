import { forwardRef } from 'react';
import type { ButtonHTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Button variants define the visual style
 */
export type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'danger';

/**
 * Button sizes
 */
export type ButtonSize = 'sm' | 'md' | 'lg';

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  /**
   * Visual variant of the button
   * @default 'primary'
   */
  variant?: ButtonVariant;
  
  /**
   * Size of the button
   * @default 'md'
   */
  size?: ButtonSize;
  
  /**
   * Whether the button is in a loading state
   * @default false
   */
  isLoading?: boolean;
  
  /**
   * Whether the button should take full width
   * @default false
   */
  fullWidth?: boolean;
}

/**
 * Button component with multiple variants, sizes, and states.
 * 
 * @example
 * ```tsx
 * <Button variant="primary" size="md">Click me</Button>
 * <Button variant="secondary" size="sm" disabled>Disabled</Button>
 * <Button variant="danger" isLoading>Loading...</Button>
 * ```
 */
export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      variant = 'primary',
      size = 'md',
      isLoading = false,
      fullWidth = false,
      className,
      children,
      disabled,
      ...props
    },
    ref
  ) => {
    const baseStyles = [
      // Base styles
      'inline-flex items-center justify-center',
      'font-medium',
      'rounded-lg',
      'transition-colors duration-200',
      'focus:outline-none focus:ring-2 focus:ring-offset-2',
      'disabled:opacity-50 disabled:cursor-not-allowed',
      // Full width
      fullWidth && 'w-full',
    ];

    const variantStyles = {
      primary: [
        'bg-brand-primary text-text-inverse',
        'hover:bg-brand-primary/90',
        'focus:ring-brand-primary',
        'active:bg-brand-primary/80',
      ],
      secondary: [
        'bg-surface-elevated text-text-primary',
        'border border-border-default',
        'hover:bg-surface-overlay',
        'focus:ring-brand-primary',
        'active:bg-border-default',
      ],
      ghost: [
        'bg-transparent text-text-primary',
        'hover:bg-surface-elevated',
        'focus:ring-brand-primary',
        'active:bg-surface-overlay',
      ],
      danger: [
        'bg-error text-text-inverse',
        'hover:bg-error/90',
        'focus:ring-error',
        'active:bg-error/80',
      ],
    };

    const sizeStyles = {
      sm: 'px-3 py-1.5 text-sm gap-1.5',
      md: 'px-4 py-2 text-base gap-2',
      lg: 'px-6 py-3 text-lg gap-2.5',
    };

    return (
      <button
        ref={ref}
        className={cn(
          baseStyles,
          variantStyles[variant],
          sizeStyles[size],
          className
        )}
        disabled={disabled || isLoading}
        {...props}
      >
        {isLoading && (
          <svg
            className="animate-spin h-4 w-4"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <circle
              className="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              strokeWidth="4"
            />
            <path
              className="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            />
          </svg>
        )}
        {children}
      </button>
    );
  }
);

Button.displayName = 'Button';