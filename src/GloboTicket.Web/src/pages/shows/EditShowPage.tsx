import { useParams } from 'react-router-dom';
import { ArrowLeft, Calendar } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { Card, EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';

/**
 * Edit show page - placeholder implementation
 */
export const EditShowPage = () => {
  const { id } = useParams<{ id: string }>();

  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => (window.location.href = routeHelpers.showDetail(id!))}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Show
      </Button>

      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Edit Show #{id}
        </Heading>
        <Text variant="muted">
          Update show information and settings
        </Text>
      </div>

      {/* Form Placeholder */}
      <Card>
        <EmptyState
          icon={Calendar}
          title="Form Not Implemented"
          description="The show edit form will be implemented in a future iteration. This will include fields for updating venue, act, date/time, ticket pricing, and more."
        />
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Edit Show page.
          Form validation, API integration, and show update logic will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};