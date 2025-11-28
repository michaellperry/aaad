import { useParams } from 'react-router-dom';
import { ArrowLeft, Edit, Users, Trash2 } from 'lucide-react';
import { Heading, Text, Button, Badge } from '../../components/atoms';
import { Card } from '../../components/molecules';
import { Stack, Row } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';

/**
 * Act detail page - placeholder implementation
 */
export const ActDetailPage = () => {
  const { id } = useParams<{ id: string }>();

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
      <div className="flex items-start justify-between">
        <div className="flex items-start gap-4">
          <div className="w-16 h-16 rounded-lg bg-brand-primary/10 flex items-center justify-center">
            <Users className="w-8 h-8 text-brand-primary" />
          </div>
          <div>
            <Heading level="h1" variant="default" className="mb-2">
              Act #{id}
            </Heading>
            <Text variant="muted">
              Placeholder act details
            </Text>
          </div>
        </div>
        <Row gap="sm">
          <Button
            variant="secondary"
            onClick={() => (window.location.href = routeHelpers.actEdit(id!))}
          >
            <Edit className="w-4 h-4 mr-2" />
            Edit
          </Button>
          <Button
            variant="danger"
            onClick={() => alert('Delete functionality not implemented')}
          >
            <Trash2 className="w-4 h-4 mr-2" />
            Delete
          </Button>
        </Row>
      </div>

      {/* Act Information */}
      <Card header={<Heading level="h2">Act Information</Heading>}>
        <Stack gap="md">
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Name
            </Text>
            <Text>Sample Act Name</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Genre
            </Text>
            <Text>Rock / Pop</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Description
            </Text>
            <Text>A talented performer with years of experience in live entertainment.</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Status
            </Text>
            <Badge variant="success">Active</Badge>
          </div>
        </Stack>
      </Card>

      {/* Upcoming Shows */}
      <Card header={<Heading level="h2">Upcoming Shows</Heading>}>
        <Text variant="muted">
          No upcoming shows scheduled for this act.
        </Text>
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Act detail page.
          Real act data, show listings, and management features will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};