import { ArrowLeft, MapPin } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { Card, EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES } from '../../router/routes';

/**
 * Create venue page - placeholder implementation
 */
export const CreateVenuePage = () => {
  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => (window.location.href = ROUTES.VENUES)}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Venues
      </Button>

      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Create New Venue
        </Heading>
        <Text variant="muted">
          Add a new venue to your event management system
        </Text>
      </div>

      {/* Form Placeholder */}
      <Card>
        <EmptyState
          icon={MapPin}
          title="Form Not Implemented"
          description="The venue creation form will be implemented in a future iteration. This will include fields for venue name, address, capacity, amenities, and more."
        />
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Create Venue page.
          Form validation, API integration, and venue creation logic will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};