/**
 * EditActPage
 * 
 * Page for editing an existing act.
 * Follows atomic design principles - composes organisms and molecules.
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { PageHeader } from '../../components/molecules/PageHeader';
import { Card } from '../../components/molecules/Card';
import { ActForm } from '../../components/organisms/ActForm';
import { Spinner } from '../../components/atoms/Spinner';
import { Text } from '../../components/atoms/Text';
import { Stack } from '../../components/layout/Stack';
import { Container } from '../../components/layout/Container';
import { getAct } from '../../api/client';
import type { Act } from '../../types/act';

/**
 * EditActPage component for editing existing acts.
 * 
 * @example
 * ```tsx
 * <Route path="/acts/:id/edit" element={<EditActPage />} />
 * ```
 */
export function EditActPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [act, setAct] = useState<Act | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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

  const handleSuccess = (updatedAct: Act) => {
    navigate('/acts');
  };

  const handleCancel = () => {
    navigate('/acts');
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

  if (error || !act) {
    return (
      <Container>
        <Stack gap="xl">
          <PageHeader title="Edit Act" description="Update act information" />
          <Card>
            <div className="p-8 text-center">
              <Text className="text-error">{error || 'Act not found'}</Text>
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
          title="Edit Act"
          description="Update act information"
        />
        <Card>
          <ActForm act={act} onSuccess={handleSuccess} onCancel={handleCancel} />
        </Card>
      </Stack>
    </Container>
  );
}