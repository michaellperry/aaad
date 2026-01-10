/**
 * VenueList Organism
 * 
 * Displays a grid of venue cards with loading, error, and empty states.
 * Follows atomic design principles - composes molecules and atoms.
 * This is a pure presentational component that receives data via props.
 */

import { Building2, AlertCircle } from 'lucide-react';
import type { Venue } from '../../types/venue';
import { VenueCard } from '../molecules/VenueCard';
import { EmptyState } from '../molecules/EmptyState';
import { Spinner } from '../atoms/Spinner';
import { Text } from '../atoms/Text';
import { Grid } from '../layout/Grid';
import { Stack } from '../layout/Stack';

export interface VenueListProps {
  /** Array of venues to display */
  venues: Venue[];
  /** Loading state */
  isLoading: boolean;
  /** Error message if any */
  error?: string | null;
  /** Optional click handler for venue cards */
  onVenueClick?: (venue: Venue) => void;
}

/**
 * VenueList component that displays venues in a responsive grid.
 * 
 * @example
 * ```tsx
 * const { data: venues = [], isLoading, error } = useVenues();
 * <VenueList 
 *   venues={venues} 
 *   isLoading={isLoading}
 *   error={error?.message}
 *   onVenueClick={(venue) => navigate(`/venues/${venue.id}`)} 
 * />
 * ```
 */
export function VenueList({ venues, isLoading, error, onVenueClick }: VenueListProps) {
  // Loading state
  if (isLoading) {
    return (
      <Stack gap="lg" align="center" className="py-12">
        <Spinner size="lg" label="Loading venues..." />
        <Text variant="muted">Loading venues...</Text>
      </Stack>
    );
  }

  // Error state
  if (error) {
    return (
      <EmptyState
        icon={AlertCircle}
        title="Failed to load venues"
        description={error}
        actionLabel="Try Again"
        onAction={() => window.location.reload()}
        actionVariant="secondary"
      />
    );
  }

  // Empty state
  if (venues.length === 0) {
    return (
      <EmptyState
        icon={Building2}
        title="No venues found"
        description="There are no venues available at the moment. Check back later or contact support."
      />
    );
  }

  // Success state - display venues in responsive grid
  return (
    <Grid
      cols={1}
      gap="lg"
      responsive={{ sm: 1, md: 2, lg: 3 }}
    >
      {venues.map((venue) => (
        <VenueCard
          key={venue.id}
          venue={venue}
          onClick={onVenueClick}
        />
      ))}
    </Grid>
  );
}
