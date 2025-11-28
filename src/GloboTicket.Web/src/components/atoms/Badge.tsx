import type { HTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Badge variants
 */
export type BadgeVariant = 'default' | 'success' | 'warning' | 'error' | 'info';

export interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  /**
   * Visual variant
   * @default 'default'
   */
  variant?: BadgeVariant;
}

/**
 * Badge component with semantic color variants.
 * 
 * @example
 * ```tsx
 * <Badge>Default</Badge>
 * <Badge variant="success">Active</Badge>
 * <Badge variant="warning">Pending</Badge>
 * <Badge variant="error">Error</Badge>
 * ```
 */
export const Badge = ({
  variant = 'default',
  className,
  children,
  ...props
}: BadgeProps) => {
  const baseStyles = [
    'inline-flex items-center',
    'px-2.5 py-0.5',
    'rounded-full',
    'text-xs font-medium',
    'transition-colors',
  ];

  const variantStyles = {
    default: [
      'bg-surface-elevated text-text-primary',
      'border border-border-default',
    ],
    success: [
      'bg-success/10 text-success',
      'border border-success/20',
    ],
    warning: [
      'bg-warning/10 text-warning',
      'border border-warning/20',
    ],
    error: [
      'bg-error/10 text-error',
      'border border-error/20',
    ],
    info: [
      'bg-info/10 text-info',
      'border border-info/20',
    ],
  };

  return (
    <span
      className={cn(
        baseStyles,
        variantStyles[variant],
        className
      )}
      {...props}
    >
      {children}
    </span>
  );
};