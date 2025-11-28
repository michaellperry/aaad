/**
 * Stack Layout Primitive
 * 
 * Vertical layout component with consistent spacing.
 * Layout-only component with no theme styling.
 */

import type { ReactNode } from 'react';
import { cn } from '../../utils/cn';

/** Spacing options for gap between children */
type Gap = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl';

/** Alignment options for children */
type Align = 'start' | 'center' | 'end' | 'stretch';

interface StackProps {
  /** Child elements to stack vertically */
  children: ReactNode;
  /** Gap between children (default: 'md') */
  gap?: Gap;
  /** Horizontal alignment of children (default: 'stretch') */
  align?: Align;
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
  stretch: 'items-stretch',
};

/**
 * Stack component for vertical layouts
 * 
 * @example
 * ```tsx
 * <Stack gap="md" align="start">
 *   <div>Item 1</div>
 *   <div>Item 2</div>
 *   <div>Item 3</div>
 * </Stack>
 * ```
 */
export function Stack({
  children,
  gap = 'md',
  align = 'stretch',
  className,
  as: Component = 'div',
}: StackProps) {
  return (
    <Component
      className={cn(
        'flex flex-col',
        gapMap[gap],
        alignMap[align],
        className
      )}
    >
      {children}
    </Component>
  );
}