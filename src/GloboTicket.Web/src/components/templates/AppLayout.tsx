import { Outlet, useLocation } from 'react-router-dom';
import { Home, MapPin, Users } from 'lucide-react';
import { AppHeader } from '../organisms/AppHeader';
import { Sidebar } from '../organisms/Sidebar';
import type { NavSection } from '../organisms/Sidebar';
import { Container } from '../layout';
import { ROUTES } from '../../router/routes';
import { useTheme } from '../../theme';
import { useAuth } from '../../hooks/useAuth';

/**
 * Main application layout with header, sidebar, and content area.
 * Uses React Router's Outlet for nested route rendering.
 */
export const AppLayout = () => {
  const location = useLocation();
  const { theme, toggleTheme } = useTheme();
  const { user, logout } = useAuth();

  // Determine active navigation item based on current path
  const getActiveItemId = () => {
    const path = location.pathname;
    if (path === ROUTES.DASHBOARD) return 'dashboard';
    if (path.startsWith('/venues')) return 'venues';
    if (path.startsWith('/acts')) return 'acts';
    return undefined;
  };

  // Sidebar navigation sections
  const sidebarSections: NavSection[] = [
    {
      title: 'Main',
      items: [
        {
          id: 'dashboard',
          icon: Home,
          label: 'Dashboard',
          href: ROUTES.DASHBOARD,
        },
      ],
    },
    {
      title: 'Management',
      items: [
        {
          id: 'venues',
          icon: MapPin,
          label: 'Venues',
          href: ROUTES.VENUES,
        },
        {
          id: 'acts',
          icon: Users,
          label: 'Acts',
          href: ROUTES.ACTS,
        },
      ],
    },
  ];

  // Get user initials from username
  const getUserInitials = (username: string): string => {
    const parts = username.split(/[\s@._-]+/);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return username.substring(0, 2).toUpperCase();
  };

  // Handle logout
  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  return (
    <div className="min-h-screen bg-surface-base">
      {/* Header */}
      <AppHeader
        logo="GloboTicket"
        theme={theme}
        onThemeToggle={toggleTheme}
        userMenu={
          user
            ? {
                userName: user.username,
                userEmail: `Tenant ID: ${user.tenantId}`,
                initials: getUserInitials(user.username),
                onLogout: handleLogout,
              }
            : undefined
        }
      />

      {/* Main layout with sidebar and content */}
      <div className="flex h-[calc(100vh-4rem)]">
        {/* Sidebar - hidden on mobile, visible on desktop */}
        <div className="hidden md:block">
          <Sidebar
            sections={sidebarSections}
            activeItemId={getActiveItemId()}
            collapsible
          />
        </div>

        {/* Main content area */}
        <main className="flex-1 overflow-y-auto">
          <Container size="xl" className="py-8">
            <Outlet />
          </Container>
        </main>
      </div>
    </div>
  );
};