/**
 * Row Layout Primitive
 * 
 * Horizontal layout component with consistent spacing.
 * Layout-only component with no theme styling.
 */

import type { ReactNode } from 'react';
import { cn } from '../../utils/cn';

/** Spacing options for gap between children */
type Gap = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl';

/** Vertical alignment options for children */
type Align = 'start' | 'center' | 'end' | 'baseline' | 'stretch';

/** Horizontal justification options for children */
type Justify = 'start' | 'center' | 'end' | 'between' | 'around' | 'evenly';

interface RowProps {
  /** Child elements to arrange horizontally */
  children: ReactNode;
  /** Gap between children (default: 'md') */
  gap?: Gap;
  /** Vertical alignment of children (default: 'center') */
  align?: Align;
  /** Horizontal justification of children (default: 'start') */
  justify?: Justify;
  /** Allow wrapping to multiple lines (default: false) */
  wrap?: boolean;
  /** Additional CSS classes */
  className?: string;
  /** HTML element to render (default: 'div') */
  as?: 'div' | 'section' | 'article' | 'aside' | 'main' | 'nav' | 'header' | 'footer';
}

const gapMap: Record<Gap, string> = {
  xs: 'gap-xs',
  sm: 'gap-sm',
  md: 'gap-md',
  lg: 'gap-lg',
  xl: 'gap-xl',
  '2xl': 'gap-2xl',
  '3xl': 'gap-3xl',
};

const alignMap: Record<Align, string> = {
  start: 'items-start',
  center: 'items-center',
  end: 'items-end',
  baseline: 'items-baseline',
  stretch: 'items-stretch',
};

const justifyMap: Record<Justify, string> = {
  start: 'justify-start',
  center: 'justify-center',
  end: 'justify-end',
  between: 'justify-between',
  around: 'justify-around',
  evenly: 'justify-evenly',
};

/**
 * Row component for horizontal layouts
 * 
 * @example
 * ```tsx
 * <Row gap="sm" justify="between" align="center">
 *   <div>Left</div>
 *   <div>Right</div>
 * </Row>
 * ```
 */
export function Row({
  children,
  gap = 'md',
  align = 'center',
  justify = 'start',
  wrap = false,
  className,
  as: Component = 'div',
}: RowProps) {
  return (
    <Component
      className={cn(
        'flex flex-row',
        gapMap[gap],
        alignMap[align],
        justifyMap[justify],
        wrap && 'flex-wrap',
        className
      )}
    >
      {children}
    </Component>
  );
}