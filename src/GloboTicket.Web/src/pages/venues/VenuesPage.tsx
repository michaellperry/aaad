import { MapPin, Plus, Search } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES } from '../../router/routes';

/**
 * Venues list page - placeholder implementation
 */
export const VenuesPage = () => {
  return (
    <Stack gap="xl">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <Heading level="h1" variant="default" className="mb-2">
            Venues
          </Heading>
          <Text variant="muted">
            Manage your event venues and locations
          </Text>
        </div>
        <Button
          variant="primary"
          onClick={() => (window.location.href = ROUTES.VENUE_CREATE)}
        >
          <Plus className="w-4 h-4 mr-2" />
          Create Venue
        </Button>
      </div>

      {/* Search Bar Placeholder */}
      <div className="flex items-center gap-2 p-3 border border-border-default rounded-lg bg-surface-base">
        <Search className="w-5 h-5 text-text-secondary" />
        <input
          type="text"
          placeholder="Search venues..."
          disabled
          className="flex-1 bg-transparent border-none outline-none text-text-primary placeholder:text-text-muted"
        />
      </div>

      {/* Empty State */}
      <EmptyState
        icon={MapPin}
        title="No venues yet"
        description="Get started by creating your first venue. Venues are locations where shows and events take place."
        actionLabel="Create Your First Venue"
        onAction={() => (window.location.href = ROUTES.VENUE_CREATE)}
      />

      {/* Development Notice */}
      <div className="mt-8 p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Venues list page.
          Real venue data, filtering, sorting, and pagination will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};