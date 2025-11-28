import { useState } from 'react';
import type { LucideIcon } from 'lucide-react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { cn } from '../../utils';
import { Button, Text } from '../atoms';
import { Stack } from '../layout';
import { NavItem } from './NavItem';

export interface NavSection {
  /**
   * Section title
   */
  title?: string;
  
  /**
   * Navigation items in this section
   */
  items: NavItemConfig[];
}

export interface NavItemConfig {
  /**
   * Unique identifier
   */
  id: string;
  
  /**
   * Icon to display
   */
  icon: LucideIcon;
  
  /**
   * Label text
   */
  label: string;
  
  /**
   * Navigation href
   */
  href: string;
  
  /**
   * Optional badge count
   */
  badge?: number;
}

export interface SidebarProps {
  /**
   * Navigation sections
   */
  sections: NavSection[];
  
  /**
   * Currently active item ID
   */
  activeItemId?: string;
  
  /**
   * Whether the sidebar is collapsible
   * @default true
   */
  collapsible?: boolean;
  
  /**
   * Initial collapsed state
   * @default false
   */
  defaultCollapsed?: boolean;
  
  /**
   * Callback when collapsed state changes
   */
  onCollapsedChange?: (collapsed: boolean) => void;
}

/**
 * Sidebar navigation with collapsible sections.
 * 
 * @example
 * ```tsx
 * import { Home, Calendar, Users } from 'lucide-react';
 * 
 * <Sidebar
 *   sections={[
 *     {
 *       title: 'Main',
 *       items: [
 *         { id: 'home', icon: Home, label: 'Dashboard', href: '/' },
 *         { id: 'events', icon: Calendar, label: 'Events', href: '/events' },
 *       ]
 *     },
 *     {
 *       title: 'Management',
 *       items: [
 *         { id: 'users', icon: Users, label: 'Users', href: '/users' },
 *       ]
 *     }
 *   ]}
 *   activeItemId="home"
 * />
 * ```
 */
export const Sidebar = ({
  sections,
  activeItemId,
  collapsible = true,
  defaultCollapsed = false,
  onCollapsedChange,
}: SidebarProps) => {
  const [collapsed, setCollapsed] = useState(defaultCollapsed);

  const handleToggleCollapse = () => {
    const newCollapsed = !collapsed;
    setCollapsed(newCollapsed);
    onCollapsedChange?.(newCollapsed);
  };

  return (
    <aside
      className={cn(
        'flex flex-col',
        'h-full',
        'bg-surface-base',
        'border-r border-border-default',
        'transition-[width] duration-300',
        collapsed ? 'w-16' : 'w-64'
      )}
    >
      <Stack gap="lg" className="flex-1 p-4 overflow-y-auto">
        {sections.map((section, sectionIndex) => (
          <Stack key={sectionIndex} gap="sm">
            {section.title && !collapsed && (
              <Text
                as="div"
                size="xs"
                variant="muted"
                className="px-3 uppercase tracking-wider font-semibold"
              >
                {section.title}
              </Text>
            )}
            
            <nav>
              <Stack gap="xs">
                {section.items.map((item) => (
                  <NavItem
                    key={item.id}
                    icon={item.icon}
                    label={item.label}
                    href={item.href}
                    badge={item.badge}
                    active={item.id === activeItemId}
                    collapsed={collapsed}
                  />
                ))}
              </Stack>
            </nav>
          </Stack>
        ))}
      </Stack>

      {collapsible && (
        <div className="p-4 border-t border-border-default">
          <Button
            variant="ghost"
            size="sm"
            onClick={handleToggleCollapse}
            className={cn(
              'w-full',
              collapsed && 'justify-center'
            )}
            aria-label={collapsed ? 'Expand sidebar' : 'Collapse sidebar'}
          >
            {collapsed ? (
              <ChevronRight className="w-4 h-4" />
            ) : (
              <>
                <ChevronLeft className="w-4 h-4" />
                <span className="flex-1 text-left">Collapse</span>
              </>
            )}
          </Button>
        </div>
      )}
    </aside>
  );
};