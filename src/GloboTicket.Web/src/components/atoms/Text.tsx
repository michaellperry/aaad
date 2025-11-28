import type { HTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Text sizes
 */
export type TextSize = 'xs' | 'sm' | 'base' | 'lg' | 'xl';

/**
 * Text variants
 */
export type TextVariant = 'body' | 'muted' | 'small';

export interface TextProps extends HTMLAttributes<HTMLElement> {
  /**
   * Text size
   * @default 'base'
   */
  size?: TextSize;
  
  /**
   * Visual variant
   * @default 'body'
   */
  variant?: TextVariant;
  
  /**
   * HTML element to render
   * @default 'p'
   */
  as?: 'p' | 'span' | 'div' | 'label';
}

/**
 * Text component with sizes and variants.
 *
 * @example
 * ```tsx
 * <Text>Default body text</Text>
 * <Text variant="muted" size="sm">Muted small text</Text>
 * <Text as="span" size="xs">Inline text</Text>
 * ```
 */
export const Text = ({
  size = 'base',
  variant = 'body',
  as: Component = 'p',
  className,
  children,
  ...props
}: TextProps) => {
    const baseStyles = [
      'leading-normal',
    ];

    const sizeStyles = {
      xs: 'text-xs',
      sm: 'text-sm',
      base: 'text-base',
      lg: 'text-lg',
      xl: 'text-xl',
    };

    const variantStyles = {
      body: 'text-text-primary',
      muted: 'text-text-secondary',
      small: 'text-text-tertiary text-sm',
    };

  return (
    <Component
      className={cn(
        baseStyles,
        sizeStyles[size],
        variantStyles[variant],
        className
      )}
      {...props}
    >
      {children}
    </Component>
  );
};