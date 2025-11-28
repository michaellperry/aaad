import { useState } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import { Home, MapPin, Users, Calendar } from 'lucide-react';
import { AppHeader } from '../organisms/AppHeader';
import { Sidebar } from '../organisms/Sidebar';
import type { NavSection } from '../organisms/Sidebar';
import { Container } from '../layout';
import { ROUTES } from '../../router/routes';

/**
 * Main application layout with header, sidebar, and content area.
 * Uses React Router's Outlet for nested route rendering.
 */
export const AppLayout = () => {
  const location = useLocation();
  const [theme, setTheme] = useState<'light' | 'dark'>('light');

  // Determine active navigation item based on current path
  const getActiveItemId = () => {
    const path = location.pathname;
    if (path === ROUTES.DASHBOARD) return 'dashboard';
    if (path.startsWith('/venues')) return 'venues';
    if (path.startsWith('/acts')) return 'acts';
    if (path.startsWith('/shows')) return 'shows';
    return undefined;
  };

  const handleThemeToggle = () => {
    setTheme((prev) => (prev === 'light' ? 'dark' : 'light'));
    // TODO: Implement actual theme switching with ThemeProvider
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
        {
          id: 'shows',
          icon: Calendar,
          label: 'Shows',
          href: ROUTES.SHOWS,
        },
      ],
    },
  ];

  return (
    <div className="min-h-screen bg-surface-base">
      {/* Header */}
      <AppHeader
        logo="GloboTicket"
        theme={theme}
        onThemeToggle={handleThemeToggle}
        userMenu={{
          userName: 'Admin User',
          userEmail: 'admin@globoticket.com',
          initials: 'AU',
          onLogout: () => console.log('Logout clicked'),
        }}
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