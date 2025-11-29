import { useNavigate } from 'react-router-dom';
import { PageHeader } from '../../components/molecules/PageHeader';
import { Card } from '../../components/molecules/Card';
import { VenueForm } from '../../components/organisms/VenueForm';
import { Stack } from '../../components/layout/Stack';
import { Container } from '../../components/layout/Container';

export function CreateVenuePage() {
  const navigate = useNavigate();

  const handleSuccess = () => {
    // Navigate back to venues list after successful creation
    navigate('/venues');
  };

  const handleCancel = () => {
    navigate('/venues');
  };

  return (
    <Container>
      <Stack gap="xl">
        <PageHeader
          title="Create Venue"
          description="Add a new venue to the system"
        />
        <Card>
          <VenueForm onSuccess={handleSuccess} onCancel={handleCancel} />
        </Card>
      </Stack>
    </Container>
  );
}