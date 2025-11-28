import { Users, Plus, Search } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES } from '../../router/routes';

/**
 * Acts list page - placeholder implementation
 */
export const ActsPage = () => {
  return (
    <Stack gap="xl">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <Heading level="h1" variant="default" className="mb-2">
            Acts
          </Heading>
          <Text variant="muted">
            Manage performers, artists, and entertainment acts
          </Text>
        </div>
        <Button
          variant="primary"
          onClick={() => (window.location.href = ROUTES.ACT_CREATE)}
        >
          <Plus className="w-4 h-4 mr-2" />
          Add Act
        </Button>
      </div>

      {/* Search Bar Placeholder */}
      <div className="flex items-center gap-2 p-3 border border-border-default rounded-lg bg-surface-base">
        <Search className="w-5 h-5 text-text-secondary" />
        <input
          type="text"
          placeholder="Search acts..."
          disabled
          className="flex-1 bg-transparent border-none outline-none text-text-primary placeholder:text-text-muted"
        />
      </div>

      {/* Empty State */}
      <EmptyState
        icon={Users}
        title="No acts yet"
        description="Get started by adding your first act. Acts are performers, artists, or entertainment groups that perform at your shows."
        actionLabel="Add Your First Act"
        onAction={() => (window.location.href = ROUTES.ACT_CREATE)}
      />

      {/* Development Notice */}
      <div className="mt-8 p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Acts list page.
          Real act data, filtering, sorting, and pagination will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};