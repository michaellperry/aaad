import { useParams } from 'react-router-dom';
import { ArrowLeft, Edit, Calendar, Trash2 } from 'lucide-react';
import { Heading, Text, Button, Badge } from '../../components/atoms';
import { Card } from '../../components/molecules';
import { Stack, Row } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';

/**
 * Show detail page - placeholder implementation
 */
export const ShowDetailPage = () => {
  const { id } = useParams<{ id: string }>();

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
      <div className="flex items-start justify-between">
        <div className="flex items-start gap-4">
          <div className="w-16 h-16 rounded-lg bg-brand-primary/10 flex items-center justify-center">
            <Calendar className="w-8 h-8 text-brand-primary" />
          </div>
          <div>
            <Heading level="h1" variant="default" className="mb-2">
              Show #{id}
            </Heading>
            <Text variant="muted">
              Placeholder show details
            </Text>
          </div>
        </div>
        <Row gap="sm">
          <Button
            variant="secondary"
            onClick={() => (window.location.href = routeHelpers.showEdit(id!))}
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

      {/* Show Information */}
      <Card header={<Heading level="h2">Show Information</Heading>}>
        <Stack gap="md">
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Title
            </Text>
            <Text>Sample Show Title</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Date & Time
            </Text>
            <Text>December 15, 2024 at 7:00 PM</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Venue
            </Text>
            <Text>Sample Venue Name</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Act
            </Text>
            <Text>Sample Act Name</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Status
            </Text>
            <Badge variant="success">Scheduled</Badge>
          </div>
        </Stack>
      </Card>

      {/* Ticket Sales */}
      <Card header={<Heading level="h2">Ticket Sales</Heading>}>
        <Text variant="muted">
          No ticket sales data available yet.
        </Text>
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Show detail page.
          Real show data, ticket sales, and management features will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};