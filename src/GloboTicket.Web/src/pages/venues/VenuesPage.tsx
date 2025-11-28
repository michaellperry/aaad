/**
 * VenuesPage
 * 
 * Main venues listing page that composes PageHeader and VenueList organisms.
 * Follows atomic design principles - minimal CSS, mostly composition.
 */

import { useNavigate } from 'react-router-dom';
import { Plus } from 'lucide-react';
import { PageHeader } from '../../components/molecules/PageHeader';
import { VenueList } from '../../components/organisms/VenueList';
import { Stack } from '../../components/layout/Stack';
import { Button } from '../../components/atoms/Button';
import { Icon } from '../../components/atoms/Icon';
import { ROUTES } from '../../router/routes';
import type { Venue } from '../../types/venue';

/**
 * VenuesPage component displaying all venues.
 * 
 * @example
 * ```tsx
 * <Route path="/venues" element={<VenuesPage />} />
 * ```
 */
export function VenuesPage() {
  const navigate = useNavigate();

  const handleVenueClick = (venue: Venue) => {
    // Navigate to venue detail page when implemented
    navigate(`/venues/${venue.id}`);
  };

  return (
    <Stack gap="xl">
      <PageHeader
        title="Venues"
        description="Browse and manage all available venues for your events"
        action={
          <Button
            variant="primary"
            onClick={() => navigate(ROUTES.VENUE_CREATE)}
            aria-label="Add new venue"
          >
            <Icon icon={Plus} size="sm" />
            Add Venue
          </Button>
        }
      />
      
      <VenueList onVenueClick={handleVenueClick} />
    </Stack>
  );
}