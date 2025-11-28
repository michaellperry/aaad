import type { LucideIcon } from 'lucide-react';
import type { HTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Icon sizes
 */
export type IconSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';

export interface IconProps extends Omit<HTMLAttributes<SVGElement>, 'children'> {
  /**
   * Lucide icon component
   */
  icon: LucideIcon;
  
  /**
   * Size of the icon
   * @default 'md'
   */
  size?: IconSize;
  
  /**
   * Accessible label for the icon
   */
  label?: string;
}

/**
 * Icon wrapper for Lucide icons with size variants.
 * 
 * @example
 * ```tsx
 * import { Home } from 'lucide-react';
 * 
 * <Icon icon={Home} size="md" label="Home" />
 * <Icon icon={Settings} size="sm" />
 * ```
 */
export const Icon = ({
  icon: IconComponent,
  size = 'md',
  label,
  className,
  ...props
}: IconProps) => {
  const sizeStyles = {
    xs: 'w-3 h-3',
    sm: 'w-4 h-4',
    md: 'w-5 h-5',
    lg: 'w-6 h-6',
    xl: 'w-8 h-8',
  };

  return (
    <IconComponent
      className={cn(sizeStyles[size], className)}
      aria-label={label}
      aria-hidden={!label}
      {...props}
    />
  );
};