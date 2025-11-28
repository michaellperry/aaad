import { ArrowLeft, Users } from 'lucide-react';
import { Heading, Text, Button } from '../../components/atoms';
import { Card, EmptyState } from '../../components/molecules';
import { Stack } from '../../components/layout';
import { ROUTES } from '../../router/routes';

/**
 * Create act page - placeholder implementation
 */
export const CreateActPage = () => {
  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => (window.location.href = ROUTES.ACTS)}
      >
        <ArrowLeft className="w-4 h-4 mr-2" />
        Back to Acts
      </Button>

      {/* Page Header */}
      <div>
        <Heading level="h1" variant="default" className="mb-2">
          Add New Act
        </Heading>
        <Text variant="muted">
          Add a new performer or entertainment act to your system
        </Text>
      </div>

      {/* Form Placeholder */}
      <Card>
        <EmptyState
          icon={Users}
          title="Form Not Implemented"
          description="The act creation form will be implemented in a future iteration. This will include fields for act name, genre, description, contact information, and more."
        />
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Create Act page.
          Form validation, API integration, and act creation logic will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};