import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Edit, MapPin, Trash2 } from 'lucide-react';
import { Heading, Text, Button, Badge, Spinner } from '../../components/atoms';
import { Card } from '../../components/molecules';
import { Stack, Row } from '../../components/layout';
import { ROUTES, routeHelpers } from '../../router/routes';
import { getVenue, deleteVenue } from '../../api/client';
import type { Venue } from '../../types/venue';

/**
 * Venue detail page - displays venue information and management options
 */
export const VenueDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [venue, setVenue] = useState<Venue | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchVenue = async () => {
      if (!id) {
        setError('Venue ID is required');
        setIsLoading(false);
        return;
      }

      try {
        const data = await getVenue(id);
        setVenue(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load venue');
      } finally {
        setIsLoading(false);
      }
    };

    fetchVenue();
  }, [id]);

  const handleDelete = async () => {
    if (!venue) return;
    
    if (window.confirm(`Are you sure you want to delete "${venue.name}"? This action cannot be undone.`)) {
      try {
        await deleteVenue(venue.venueGuid);
        navigate('/venues');
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to delete venue');
      }
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center min-h-[400px]">
        <Spinner size="lg" />
      </div>
    );
  }

  if (error || !venue) {
    return (
      <Stack gap="xl">
        <Card>
          <div className="p-8 text-center">
            <Text className="text-error">{error || 'Venue not found'}</Text>
          </div>
        </Card>
      </Stack>
    );
  }

  return (
    <Stack gap="xl">
      {/* Back Button */}
      <Button
        variant="ghost"
        onClick={() => navigate('/venues')}
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
              {venue.name}
            </Heading>
            <Text variant="muted">
              {venue.address}
            </Text>
          </div>
        </div>
        <Row gap="sm">
          <Button
            variant="secondary"
            onClick={() => navigate(`/venues/${id}/edit`)}
          >
            <Edit className="w-4 h-4 mr-2" />
            Edit
          </Button>
          <Button
            variant="danger"
            onClick={handleDelete}
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
            <Text>{venue.name}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Address
            </Text>
            <Text>{venue.address}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Capacity
            </Text>
            <Text>{venue.seatingCapacity?.toLocaleString() || 'N/A'}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Description
            </Text>
            <Text>{venue.description || 'No description available'}</Text>
          </div>
          <div>
            <Text variant="muted" size="sm" className="mb-1">
              Coordinates
            </Text>
            <Text>Latitude: {venue.latitude}, Longitude: {venue.longitude}</Text>
          </div>
        </Stack>
      </Card>

      {/* Upcoming Shows */}
      <Card header={<Heading level="h2">Upcoming Shows</Heading>}>
        <Text variant="muted">
          No upcoming shows scheduled at this venue.
        </Text>
      </Card>

    </Stack>
  );
};