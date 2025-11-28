/**
 * Container Layout Primitive
 * 
 * Max-width container with responsive padding.
 * Layout-only component with no theme styling.
 */

import type { ReactNode } from 'react';
import { cn } from '../../utils/cn';

/** Container size options */
type Size = 'sm' | 'md' | 'lg' | 'xl' | 'full';

interface ContainerProps {
  /** Child elements */
  children: ReactNode;
  /** Maximum width of container (default: 'lg') */
  size?: Size;
  /** Add horizontal padding (default: true) */
  padding?: boolean;
  /** Additional CSS classes */
  className?: string;
  /** HTML element to render (default: 'div') */
  as?: 'div' | 'section' | 'article' | 'aside' | 'main' | 'nav' | 'header' | 'footer';
}

const sizeMap: Record<Size, string> = {
  sm: 'max-w-screen-sm',   // 640px
  md: 'max-w-screen-md',   // 768px
  lg: 'max-w-screen-lg',   // 1024px
  xl: 'max-w-screen-xl',   // 1280px
  full: 'max-w-full',
};

/**
 * Container component for max-width layouts
 * 
 * @example
 * ```tsx
 * <Container size="lg" padding>
 *   <h1>Page Content</h1>
 * </Container>
 * ```
 */
export function Container({
  children,
  size = 'lg',
  padding = true,
  className,
  as: Component = 'div',
}: ContainerProps) {
  return (
    <Component
      className={cn(
        'mx-auto w-full',
        sizeMap[size],
        padding && 'px-4 sm:px-6 lg:px-8',
        className
      )}
    >
      {children}
    </Component>
  );
}