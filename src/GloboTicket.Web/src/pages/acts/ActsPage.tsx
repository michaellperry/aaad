/**
 * ActsPage
 *
 * Main acts listing page that composes PageHeader and ActList organisms.
 * Follows atomic design principles - minimal CSS, mostly composition.
 */

import { useNavigate } from 'react-router-dom';
import { Plus } from 'lucide-react';
import { PageHeader } from '../../components/molecules/PageHeader';
import { ActList } from '../../components/organisms/ActList';
import { Stack } from '../../components/layout/Stack';
import { Button } from '../../components/atoms/Button';
import { Icon } from '../../components/atoms/Icon';
import { ROUTES } from '../../router/routes';
import { useActs } from '../../features/acts/hooks';
import type { Act } from '../../types/act';

/**
 * ActsPage component displaying all acts.
 *
 * @example
 * ```tsx
 * <Route path="/acts" element={<ActsPage />} />
 * ```
 */
export function ActsPage() {
  const navigate = useNavigate();
  const { data: acts = [], isLoading, error } = useActs();

  const handleActClick = (act: Act) => {
    navigate(`/acts/${act.actGuid}`);
  };

  return (
    <Stack gap="xl">
      <PageHeader
        title="Acts"
        description="Browse and manage all performing acts"
        action={
          <Button
            variant="primary"
            onClick={() => navigate(ROUTES.ACT_CREATE)}
            aria-label="Add new act"
          >
            <Icon icon={Plus} size="sm" />
            Add Act
          </Button>
        }
      />
      
      <ActList
        acts={acts}
        isLoading={isLoading}
        error={error?.message}
        onActClick={handleActClick}
      />
    </Stack>
  );
}