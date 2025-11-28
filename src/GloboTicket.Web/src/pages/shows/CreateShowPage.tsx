import { ArrowLeft, Calendar } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { Card, EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES } from '../../router/routes';

/**
 * Create show page - placeholder implementation
 */
export const CreateShowPage = () => {
  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => (window.location.href = ROUTES.SHOWS)}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Shows
      </Button>

      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Schedule New Show
        </Heading>
        <Text variant="muted">
          Create a new show event with venue, act, and scheduling details
        </Text>
      </div>

      {/* Form Placeholder */}
      <Card>
        <EmptyState
          icon={Calendar}
          title="Form Not Implemented"
          description="The show creation form will be implemented in a future iteration. This will include fields for selecting venue, act, date/time, ticket pricing, and more."
        />
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Create Show page.
          Form validation, API integration, and show creation logic will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};