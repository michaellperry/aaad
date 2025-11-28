import { useParams } from 'react-router-dom';
import { ArrowLeft, Edit, MapPin, Trash2 } from 'lucide-react';
import { Heading, Text, Button, Badge } from '../../components/atoms';
import { Card } from '../../components/molecules';
import { Stack, Row } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';

/**
 * Venue detail page - placeholder implementation
 */
export const VenueDetailPage = () => {
  const { id } = useParams<{ id: string }>();

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
      <div className="flex items-start justify-between">
        <div className="flex items-start gap-4">
          <div className="w-16 h-16 rounded-lg bg-brand-primary/10 flex items-center justify-center">
            <MapPin className="w-8 h-8 text-brand-primary" />
          </div>
          <div>
            <Heading level="h1" variant="default" className="mb-2">
              Venue #{id}
            </Heading>
            <Text variant="muted">
              Placeholder venue details
            </Text>
          </div>
        </div>
        <Row gap="sm">
          <Button
            variant="secondary"
            onClick={() => (window.location.href = routeHelpers.venueEdit(id!))}
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

      {/* Venue Information */}
      <Card header={<Heading level="h2">Venue Information</Heading>}>
        <Stack gap="md">
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Name
            </Text>
            <Text>Sample Venue Name</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Address
            </Text>
            <Text>123 Main Street, City, State 12345</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Capacity
            </Text>
            <Text>5,000 people</Text>
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
          No upcoming shows scheduled at this venue.
        </Text>
      </Card>

      {/* Development Notice */}
      <div className="p-4 bg-surface-elevated rounded-lg border border-border-default">
        <Text variant="muted" size="sm">
          <strong>🚧 Placeholder Page:</strong> This is a placeholder for the Venue detail page.
          Real venue data, show listings, and management features will be implemented in future iterations.
        </Text>
      </div>
    </Stack>
  );
};