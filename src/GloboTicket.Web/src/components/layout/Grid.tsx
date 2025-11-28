/**
 * Grid Layout Primitive
 * 
 * CSS Grid layout component with responsive columns.
 * Layout-only component with no theme styling.
 */

import type { ReactNode } from 'react';
import { cn } from '../../utils/cn';

/** Spacing options for gap between children */
type Gap = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl';

/** Column count options */
type Cols = 1 | 2 | 3 | 4 | 6 | 12;

/** Responsive column configuration */
interface ResponsiveConfig {
  /** Columns at small breakpoint (640px+) */
  sm?: Cols;
  /** Columns at medium breakpoint (768px+) */
  md?: Cols;
  /** Columns at large breakpoint (1024px+) */
  lg?: Cols;
}

interface GridProps {
  /** Child elements to arrange in grid */
  children: ReactNode;
  /** Number of columns (default: 1) */
  cols?: Cols;
  /** Gap between grid items (default: 'md') */
  gap?: Gap;
  /** Responsive column configuration */
  responsive?: ResponsiveConfig;
  /** Additional CSS classes */
  className?: string;
  /** HTML element to render (default: 'div') */
  as?: 'div' | 'section' | 'article' | 'aside' | 'main' | 'nav' | 'ul' | 'ol';
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

const colsMap: Record<Cols, string> = {
  1: 'grid-cols-1',
  2: 'grid-cols-2',
  3: 'grid-cols-3',
  4: 'grid-cols-4',
  6: 'grid-cols-6',
  12: 'grid-cols-12',
};

const responsiveColsMap = {
  sm: {
    1: 'sm:grid-cols-1',
    2: 'sm:grid-cols-2',
    3: 'sm:grid-cols-3',
    4: 'sm:grid-cols-4',
    6: 'sm:grid-cols-6',
    12: 'sm:grid-cols-12',
  },
  md: {
    1: 'md:grid-cols-1',
    2: 'md:grid-cols-2',
    3: 'md:grid-cols-3',
    4: 'md:grid-cols-4',
    6: 'md:grid-cols-6',
    12: 'md:grid-cols-12',
  },
  lg: {
    1: 'lg:grid-cols-1',
    2: 'lg:grid-cols-2',
    3: 'lg:grid-cols-3',
    4: 'lg:grid-cols-4',
    6: 'lg:grid-cols-6',
    12: 'lg:grid-cols-12',
  },
};

/**
 * Grid component for grid layouts
 * 
 * @example
 * ```tsx
 * <Grid cols={3} gap="lg" responsive={{ sm: 1, md: 2 }}>
 *   <div>Item 1</div>
 *   <div>Item 2</div>
 *   <div>Item 3</div>
 * </Grid>
 * ```
 */
export function Grid({
  children,
  cols = 1,
  gap = 'md',
  responsive,
  className,
  as: Component = 'div',
}: GridProps) {
  return (
    <Component
      className={cn(
        'grid',
        colsMap[cols],
        gapMap[gap],
        responsive?.sm && responsiveColsMap.sm[responsive.sm],
        responsive?.md && responsiveColsMap.md[responsive.md],
        responsive?.lg && responsiveColsMap.lg[responsive.lg],
        className
      )}
    >
      {children}
    </Component>
  );
}