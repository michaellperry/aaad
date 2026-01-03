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
import { useVenues } from '../../features/venues/hooks';
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
  
  // Fetch venues using TanStack Query
  const { data: venues = [], isLoading, error } = useVenues();

  const handleVenueClick = (venue: Venue) => {
    // Navigate to venue detail page
    navigate(`/venues/${venue.venueGuid}`);
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
      
      <VenueList 
        venues={venues}
        isLoading={isLoading}
        error={error?.message}
        onVenueClick={handleVenueClick}
      />
    </Stack>
  );
}
