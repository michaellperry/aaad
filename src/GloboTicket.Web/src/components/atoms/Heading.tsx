import { createElement, forwardRef } from 'react';
import type { HTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Heading levels (h1-h6)
 */
export type HeadingLevel = 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6';

/**
 * Heading visual variants
 */
export type HeadingVariant = 'default' | 'muted' | 'gradient';

export interface HeadingProps extends HTMLAttributes<HTMLHeadingElement> {
  /**
   * Semantic heading level
   * @default 'h2'
   */
  level?: HeadingLevel;
  
  /**
   * Visual variant
   * @default 'default'
   */
  variant?: HeadingVariant;
  
  /**
   * Custom element to render (overrides level)
   */
  as?: HeadingLevel;
}

/**
 * Heading component with semantic levels and visual variants.
 * 
 * @example
 * ```tsx
 * <Heading level="h1">Page Title</Heading>
 * <Heading level="h2" variant="muted">Section Title</Heading>
 * <Heading level="h3" variant="gradient">Featured</Heading>
 * ```
 */
export const Heading = forwardRef<HTMLHeadingElement, HeadingProps>(
  (
    {
      level = 'h2',
      variant = 'default',
      as,
      className,
      children,
      ...props
    },
    ref
  ) => {
    const element = as || level;

    const baseStyles = [
      'font-semibold',
      'tracking-tight',
    ];

    const levelStyles = {
      h1: 'text-4xl leading-tight',
      h2: 'text-3xl leading-tight',
      h3: 'text-2xl leading-normal',
      h4: 'text-xl leading-normal',
      h5: 'text-lg leading-normal',
      h6: 'text-base leading-normal',
    };

    const variantStyles = {
      default: 'text-text-primary',
      muted: 'text-text-secondary',
      gradient: 'bg-gradient-to-r from-brand-primary to-brand-secondary bg-clip-text text-transparent',
    };

    return createElement(
      element,
      {
        ref,
        className: cn(
          baseStyles,
          levelStyles[level],
          variantStyles[variant],
          className
        ),
        ...props,
      },
      children
    );
  }
);

Heading.displayName = 'Heading';