/**
 * CreateActPage
 * 
 * Page for creating a new act.
 * Follows atomic design principles - composes organisms and molecules.
 */

import { useNavigate } from 'react-router-dom';
import { PageHeader } from '../../components/molecules/PageHeader';
import { Card } from '../../components/molecules/Card';
import { ActForm } from '../../components/organisms/ActForm';
import { Stack } from '../../components/layout/Stack';
import { Container } from '../../components/layout/Container';

/**
 * CreateActPage component for creating new acts.
 * 
 * @example
 * ```tsx
 * <Route path="/acts/new" element={<CreateActPage />} />
 * ```
 */
export function CreateActPage() {
  const navigate = useNavigate();

  const handleSuccess = () => {
    navigate('/acts');
  };

  const handleCancel = () => {
    navigate('/acts');
  };

  return (
    <Container>
      <Stack gap="xl">
        <PageHeader
          title="Create Act"
          description="Add a new performing act to the system"
        />
        <Card>
          <ActForm onSuccess={handleSuccess} onCancel={handleCancel} />
        </Card>
      </Stack>
    </Container>
  );
}