import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { PageHeader } from '../../components/molecules/PageHeader';
import { Card } from '../../components/molecules/Card';
import { VenueForm } from '../../components/organisms/VenueForm';
import { Stack } from '../../components/layout/Stack';
import { Container } from '../../components/layout/Container';
import { Spinner } from '../../components/atoms/Spinner';
import { Text } from '../../components/atoms/Text';
import { getVenue } from '../../api/client';
import type { Venue } from '../../types/venue';

export function EditVenuePage() {
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

  const handleSuccess = () => {
    navigate('/venues');
  };

  const handleCancel = () => {
    navigate('/venues');
  };

  if (isLoading) {
    return (
      <Container>
        <div className="flex justify-center items-center min-h-[400px]">
          <Spinner size="lg" />
        </div>
      </Container>
    );
  }

  if (error || !venue) {
    return (
      <Container>
        <Stack gap="xl">
          <PageHeader title="Edit Venue" description="Update venue information" />
          <Card>
            <div className="p-8 text-center">
              <Text className="text-error">{error || 'Venue not found'}</Text>
            </div>
          </Card>
        </Stack>
      </Container>
    );
  }

  return (
    <Container>
      <Stack gap="xl">
        <PageHeader
          title="Edit Venue"
          description="Update venue information"
        />
        <Card>
          <VenueForm venue={venue} onSuccess={handleSuccess} onCancel={handleCancel} />
        </Card>
      </Stack>
    </Container>
  );
}