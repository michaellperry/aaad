import { Calendar, Plus, Search } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES } from '../../router/routes';

/**
 * Shows list page - placeholder implementation
 */
export const ShowsPage = () => {
  return (
    <Stack gap="xl">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <Heading level="h1" variant="default" className="mb-2">
            Shows
          </Heading>
          <Text variant="muted">
            Manage scheduled shows and events
          </Text>
        </div>
        <Button
          variant="primary"
          onClick={() => (window.location.href = ROUTES.SHOW_CREATE)}
        >
          <Plus className="w-4 h-4 mr-2" />
          Schedule Show
        </Button>
      </div>

      {/* Search Bar Placeholder */}
      <div className="flex items-center gap-2 p-3 border border-border-default rounded-lg bg-surface-base">
        <Search className="w-5 h-5 text-text-secondary" />
        <input
          type="text"
          placeholder="Search shows..."
          disabled
          className="flex-1 bg-transparent border-none outline-none text-text-primary placeholder:text-text-muted"
        />
      </div>

      {/* Empty State */}
      <EmptyState
        icon={Calendar}
        title="No shows yet"
        description="Get started by scheduling your first show. Shows are events where acts perform at venues for your audience."
        actionLabel="Schedule Your First Show"
        onAction={() => (window.location.href = ROUTES.SHOW_CREATE)}
      />

      {/* Development Notice */}
      <div className="mt-8 p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Shows list page.
          Real show data, filtering, sorting, calendar view, and pagination will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};