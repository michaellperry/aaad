import { Link } from 'react-router-dom';
import { Moon, Sun } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';
import { cn } from '../../utils';
import { Button, Heading, Icon } from '../atoms';
import { Row } from '../layout';
import { UserMenu } from './UserMenu';
import type { UserMenuProps } from './UserMenu';

export interface NavLink {
  /**
   * Unique identifier
   */
  id: string;
  
  /**
   * Label text
   */
  label: string;
  
  /**
   * Navigation href
   */
  href: string;
  
  /**
   * Optional icon
   */
  icon?: LucideIcon;
}

export interface AppHeaderProps {
  /**
   * Application logo (text or image)
   */
  logo?: string | React.ReactNode;
  
  /**
   * Navigation links
   */
  navLinks?: NavLink[];
  
  /**
   * Currently active link ID
   */
  activeLinkId?: string;
  
  /**
   * Current theme
   */
  theme?: 'light' | 'dark';
  
  /**
   * Theme toggle handler
   */
  onThemeToggle?: () => void;
  
  /**
   * User menu props
   */
  userMenu?: UserMenuProps;
  
  /**
   * Whether to show the theme toggle
   * @default true
   */
  showThemeToggle?: boolean;
}

/**
 * Top navigation bar with logo, navigation items, theme toggle, and user menu.
 * 
 * @example
 * ```tsx
 * <AppHeader
 *   logo="GloboTicket"
 *   navLinks={[
 *     { id: 'events', label: 'Events', href: '/events' },
 *     { id: 'venues', label: 'Venues', href: '/venues' },
 *   ]}
 *   activeLinkId="events"
 *   theme="light"
 *   onThemeToggle={() => toggleTheme()}
 *   userMenu={{
 *     userName: 'John Doe',
 *     userEmail: 'john@example.com',
 *     initials: 'JD',
 *     onLogout: () => console.log('Logout'),
 *   }}
 * />
 * ```
 */
export const AppHeader = ({
  logo = 'GloboTicket',
  navLinks = [],
  activeLinkId,
  theme = 'light',
  onThemeToggle,
  userMenu,
  showThemeToggle = true,
}: AppHeaderProps) => {
  return (
    <header
      className={cn(
        'sticky top-0 z-50',
        'bg-surface-base',
        'border-b border-border-default',
        'shadow-sm'
      )}
    >
      <div className="px-4 sm:px-6 lg:px-8">
        <Row
          justify="between"
          align="center"
          className="h-16"
        >
          {/* Logo */}
          <div className="flex items-center gap-8">
            <div className="flex-shrink-0">
              {typeof logo === 'string' ? (
                <Heading level="h1" className="text-xl">
                  {logo}
                </Heading>
              ) : (
                logo
              )}
            </div>

            {/* Navigation Links */}
            {navLinks.length > 0 && (
              <nav className="hidden md:flex items-center gap-1">
                {navLinks.map((link) => (
                  <Link
                    key={link.id}
                    to={link.href}
                    className={cn(
                      'flex items-center gap-2',
                      'px-3 py-2',
                      'rounded-lg',
                      'text-sm font-medium',
                      'transition-colors duration-200',
                      'focus:outline-none focus:ring-2 focus:ring-brand-primary focus:ring-offset-2',
                      link.id === activeLinkId
                        ? 'bg-brand-primary/10 text-brand-primary'
                        : 'text-text-secondary hover:bg-surface-elevated hover:text-text-primary'
                    )}
                    aria-current={link.id === activeLinkId ? 'page' : undefined}
                  >
                    {link.icon && <Icon icon={link.icon} size="sm" />}
                    {link.label}
                  </Link>
                ))}
              </nav>
            )}
          </div>

          {/* Right side actions */}
          <Row gap="sm" align="center">
            {/* Theme Toggle */}
            {showThemeToggle && onThemeToggle && (
              <Button
                variant="ghost"
                size="sm"
                onClick={onThemeToggle}
                aria-label={`Switch to ${theme === 'light' ? 'dark' : 'light'} mode`}
              >
                {theme === 'light' ? (
                  <Moon className="w-5 h-5" />
                ) : (
                  <Sun className="w-5 h-5" />
                )}
              </Button>
            )}

            {/* User Menu */}
            {userMenu && <UserMenu {...userMenu} />}
          </Row>
        </Row>
      </div>
    </header>
  );
};