import { BarChart3, Calendar, MapPin, Users } from 'lucide-react';
import { Heading, Text, Button } from '../components/atoms';
import { Card } from '../components/molecules';
import { Grid, Stack } from '../components/layout';
import { ROUTES } from '../router/routes';

/**
 * Dashboard overview page with key metrics and quick actions
 */
export const DashboardPage = () => {
  // Mock statistics
  const stats = [
    {
      id: 'venues',
      label: 'Total Venues',
      value: '12',
      icon: MapPin,
      href: ROUTES.VENUES,
    },
    {
      id: 'acts',
      label: 'Active Acts',
      value: '48',
      icon: Users,
      href: ROUTES.ACTS,
    },
    {
      id: 'shows',
      label: 'Upcoming Shows',
      value: '24',
      icon: Calendar,
      href: ROUTES.SHOWS,
    },
    {
      id: 'sales',
      label: 'Tickets Sold',
      value: '1,234',
      icon: BarChart3,
      href: '#',
    },
  ];

  return (
    <Stack gap="xl">
      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Dashboard
        </Heading>
        <Text variant="muted">
          Welcome to GloboTicket. Here's an overview of your event management system.
        </Text>
      </div>

      {/* Statistics Grid */}
      <Grid cols={1} gap="lg" responsive={{ sm: 2, lg: 4 }}>
        {stats.map((stat) => {
          const Icon = stat.icon;
          return (
            <Card key={stat.id} interactive>
              <Stack gap="md">
                <div className="flex items-center justify-between">
                  <div className="w-12 h-12 rounded-lg bg-brand-primary/10 flex items-center justify-center">
                    <Icon className="w-6 h-6 text-brand-primary" />
                  </div>
                </div>
                <div>
                  <Text variant="muted" size="sm" className="mb-1">
                    {stat.label}
                  </Text>
                  <Heading level="h2" variant="default">
                    {stat.value}
                  </Heading>
                </div>
                {stat.href !== '#' && (
                  <a
                    href={stat.href}
                    className="text-sm text-brand-primary hover:text-brand-primary-hover"
                  >
                    View all →
                  </a>
                )}
              </Stack>
            </Card>
          );
        })}
      </Grid>

      {/* Quick Actions */}
      <Card>
        <Stack gap="lg">
          <Heading level="h2" variant="default">
            Quick Actions
          </Heading>
          <Grid cols={1} gap="md" responsive={{ sm: 2, lg: 3 }}>
            <Button
              variant="primary"
              size="lg"
              onClick={() => (window.location.href = ROUTES.VENUE_CREATE)}
              className="w-full"
            >
              Create Venue
            </Button>
            <Button
              variant="primary"
              size="lg"
              onClick={() => (window.location.href = ROUTES.ACT_CREATE)}
              className="w-full"
            >
              Add Act
            </Button>
            <Button
              variant="primary"
              size="lg"
              onClick={() => (window.location.href = ROUTES.SHOW_CREATE)}
              className="w-full"
            >
              Schedule Show
            </Button>
          </Grid>
        </Stack>
      </Card>

      {/* Placeholder Notice */}
      <Card>
        <Stack gap="md">
          <Heading level="h3" variant="default">
            🚧 Development Notice
          </Heading>
          <Text variant="muted">
            This is a placeholder dashboard demonstrating the design system and navigation structure.
            Real data integration and features will be implemented in future iterations.
          </Text>
        </Stack>
      </Card>
    </Stack>
  );
};