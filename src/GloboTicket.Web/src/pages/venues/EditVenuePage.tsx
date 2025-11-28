import { useParams } from 'react-router-dom';
import { ArrowLeft, MapPin } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { Card, EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';

/**
 * Edit venue page - placeholder implementation
 */
export const EditVenuePage = () => {
  const { id } = useParams<{ id: string }>();

  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => (window.location.href = routeHelpers.venueDetail(id!))}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Venue
      </Button>

      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Edit Venue #{id}
        </Heading>
        <Text variant="muted">
          Update venue information and settings
        </Text>
      </div>

      {/* Form Placeholder */}
      <Card>
        <EmptyState
          icon={MapPin}
          title="Form Not Implemented"
          description="The venue edit form will be implemented in a future iteration. This will include fields for updating venue name, address, capacity, amenities, and more."
        />
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Edit Venue page.
          Form validation, API integration, and venue update logic will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};