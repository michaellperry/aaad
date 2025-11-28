import { useParams } from 'react-router-dom';
import { ArrowLeft, Users } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { Card, EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';

/**
 * Edit act page - placeholder implementation
 */
export const EditActPage = () => {
  const { id } = useParams<{ id: string }>();

  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => (window.location.href = routeHelpers.actDetail(id!))}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Act
      </Button>

      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Edit Act #{id}
        </Heading>
        <Text variant="muted">
          Update act information and settings
        </Text>
      </div>

      {/* Form Placeholder */}
      <Card>
        <EmptyState
          icon={Users}
          title="Form Not Implemented"
          description="The act edit form will be implemented in a future iteration. This will include fields for updating act name, genre, description, contact information, and more."
        />
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Edit Act page.
          Form validation, API integration, and act update logic will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};