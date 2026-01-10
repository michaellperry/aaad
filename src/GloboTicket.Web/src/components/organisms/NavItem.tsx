import { forwardRef } from 'react';
import type { AnchorHTMLAttributes } from 'react';
import { Link } from 'react-router-dom';
import type { LucideIcon } from 'lucide-react';
import { cn } from '../../utils';
import { Icon, Text, Badge } from '../atoms';

export interface NavItemProps extends Omit<AnchorHTMLAttributes<HTMLAnchorElement>, 'href'> {
  /**
   * Icon to display
   */
  icon: LucideIcon;
  
  /**
   * Label text
   */
  label: string;
  
  /**
   * Navigation path (required)
   */
  href: string;
  
  /**
   * Whether the item is active
   * @default false
   */
  active?: boolean;
  
  /**
   * Optional badge count
   */
  badge?: number;
  
  /**
   * Whether the item is collapsed (icon only)
   * @default false
   */
  collapsed?: boolean;
}

/**
 * Navigation item component with active state and icon.
 * 
 * @example
 * ```tsx
 * import { Home } from 'lucide-react';
 * 
 * <NavItem
 *   icon={Home}
 *   label="Dashboard"
 *   href="/dashboard"
 *   active
 * />
 * 
 * <NavItem
 *   icon={Inbox}
 *   label="Messages"
 *   href="/messages"
 *   badge={5}
 * />
 * ```
 */
export const NavItem = forwardRef<HTMLAnchorElement, NavItemProps>(
  (
    {
      icon,
      label,
      href,
      active = false,
      badge,
      collapsed = false,
      className,
      ...props
    },
    ref
  ) => {
    const baseStyles = [
      'flex items-center gap-3',
      'px-3 py-2',
      'rounded-lg',
      'transition-colors duration-200',
      'text-sm font-medium',
      'focus:outline-none focus:ring-2 focus:ring-brand-primary focus:ring-offset-2',
    ];

    const stateStyles = active
      ? [
          'bg-brand-primary/10 text-brand-primary',
        ]
      : [
          'text-text-secondary',
          'hover:bg-surface-elevated hover:text-text-primary',
        ];

    return (
      <Link
        ref={ref}
        to={href}
        className={cn(
          baseStyles,
          stateStyles,
          collapsed && 'justify-center',
          className
        )}
        aria-current={active ? 'page' : undefined}
        {...props}
      >
        <Icon icon={icon} size="md" />
        
        {!collapsed && (
          <>
            <Text
              as="span"
              size="sm"
              className="flex-1 overflow-hidden text-ellipsis whitespace-nowrap"
            >
              {label}
            </Text>
            
            {badge !== undefined && badge > 0 && (
              <Badge variant="info">
                {badge > 99 ? '99+' : badge}
              </Badge>
            )}
          </>
        )}
      </Link>
    );
  }
);

NavItem.displayName = 'NavItem';