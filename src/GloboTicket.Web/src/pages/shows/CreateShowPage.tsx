/**
 * CreateShowPage
 * 
 * Page for creating a new show for an act.
 * Follows atomic design principles - composes organisms and molecules.
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { PageHeader } from '../../components/molecules/PageHeader';
import { Card } from '../../components/molecules/Card';
import { ShowForm } from '../../components/organisms/ShowForm';
import { Stack } from '../../components/layout/Stack';
import { Container } from '../../components/layout/Container';
import { Button } from '../../components/atoms/Button';
import { Text } from '../../components/atoms/Text';
import { Spinner } from '../../components/atoms/Spinner';
import { getAct } from '../../api/client';
import type { Act } from '../../types/act';

/**
 * CreateShowPage component for creating new shows for an act.
 * 
 * @example
 * ```tsx
 * <Route path="/acts/:id/shows/new" element={<CreateShowPage />} />
 * ```
 */
export function CreateShowPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [act, setAct] = useState<Act | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch act data to display act name
  useEffect(() => {
    const fetchAct = async () => {
      if (!id) {
        setError('Act ID is required');
        setIsLoading(false);
        return;
      }

      try {
        const data = await getAct(id);
        setAct(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load act');
      } finally {
        setIsLoading(false);
      }
    };

    fetchAct();
  }, [id]);

  const handleSuccess = () => {
    navigate(`/acts/${id}`);
  };

  const handleCancel = () => {
    navigate(`/acts/${id}`);
  };

  const handleBack = () => {
    navigate(`/acts/${id}`);
  };

  // Loading state
  if (isLoading) {
    return (
      <Container>
        <div className="flex justify-center items-center min-h-[400px]">
          <Spinner size="lg" />
        </div>
      </Container>
    );
  }

  // Error state
  if (error || !act) {
    return (
      <Container>
        <Stack gap="xl">
          <Button variant="secondary" onClick={handleBack}>
            ← Back to Act
          </Button>
          <Card>
            <div className="p-8 text-center">
              <Text className="text-error">{error || 'Act not found'}</Text>
            </div>
          </Card>
        </Stack>
      </Container>
    );
  }

  // Success state
  return (
    <Container>
      <Stack gap="xl">
        <Button variant="secondary" onClick={handleBack}>
          ← Back to Act
        </Button>
        <PageHeader
          title={`Add Show for ${act.name}`}
          description="Schedule a new performance for this act"
        />
        <Card>
          <ShowForm
            actGuid={act.actGuid}
            actName={act.name}
            onSuccess={handleSuccess}
            onCancel={handleCancel}
          />
        </Card>
      </Stack>
    </Container>
  );
}
