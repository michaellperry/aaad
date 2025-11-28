import type { HTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Spinner sizes
 */
export type SpinnerSize = 'sm' | 'md' | 'lg' | 'xl';

export interface SpinnerProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Size of the spinner
   * @default 'md'
   */
  size?: SpinnerSize;
  
  /**
   * Accessible label for the spinner
   * @default 'Loading...'
   */
  label?: string;
}

/**
 * Loading spinner component with size variants.
 * 
 * @example
 * ```tsx
 * <Spinner />
 * <Spinner size="lg" label="Loading data..." />
 * <Spinner size="sm" />
 * ```
 */
export const Spinner = ({
  size = 'md',
  label = 'Loading...',
  className,
  ...props
}: SpinnerProps) => {
  const sizeStyles = {
    sm: 'w-4 h-4 border-2',
    md: 'w-6 h-6 border-2',
    lg: 'w-8 h-8 border-3',
    xl: 'w-12 h-12 border-4',
  };

  return (
    <div
      role="status"
      aria-label={label}
      className={cn('inline-block', className)}
      {...props}
    >
      <div
        className={cn(
          'animate-spin rounded-full',
          'border-border-default border-t-brand-primary',
          sizeStyles[size]
        )}
      />
      <span className="sr-only">{label}</span>
    </div>
  );
};