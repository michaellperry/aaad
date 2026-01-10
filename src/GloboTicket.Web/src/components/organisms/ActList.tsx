/**
 * ActList Organism
 *
 * Displays a grid of act cards with loading, error, and empty states.
 * Pure presentational component - data fetching is handled by parent.
 * Follows atomic design principles - composes molecules and atoms.
 */

import { Music, AlertCircle } from 'lucide-react';
import type { Act } from '../../types/act';
import { ActCard } from '../molecules/ActCard';
import { EmptyState } from '../molecules/EmptyState';
import { Spinner } from '../atoms/Spinner';
import { Text } from '../atoms/Text';
import { Grid } from '../layout/Grid';
import { Stack } from '../layout/Stack';

export interface ActListProps {
  /** Acts data to display */
  acts: Act[];
  /** Loading state */
  isLoading: boolean;
  /** Optional error message */
  error?: string | null;
  /** Optional click handler for act cards */
  onActClick?: (act: Act) => void;
}

/**
 * ActList component that displays acts in a responsive grid.
 *
 * @example
 * ```tsx
 * const { data: acts, isLoading, error } = useActs();
 * <ActList
 *   acts={acts || []}
 *   isLoading={isLoading}
 *   error={error?.message}
 *   onActClick={(act) => navigate(`/acts/${act.actGuid}`)}
 * />
 * ```
 */
export function ActList({ acts, isLoading, error, onActClick }: ActListProps) {
  // Loading state
  if (isLoading) {
    return (
      <Stack gap="lg" align="center" className="py-12">
        <Spinner size="lg" label="Loading acts..." />
        <Text variant="muted">Loading acts...</Text>
      </Stack>
    );
  }

  // Error state
  if (error) {
    return (
      <EmptyState
        icon={AlertCircle}
        title="Failed to load acts"
        description={error}
        actionLabel="Try Again"
        onAction={() => window.location.reload()}
        actionVariant="secondary"
      />
    );
  }

  // Empty state
  if (acts.length === 0) {
    return (
      <EmptyState
        icon={Music}
        title="No acts found"
        description="There are no acts available at the moment. Check back later or contact support."
      />
    );
  }

  // Success state - display acts in responsive grid
  return (
    <Grid
      cols={1}
      gap="lg"
      responsive={{ sm: 1, md: 2, lg: 3 }}
    >
      {acts.map((act) => (
        <ActCard
          key={act.id}
          act={act}
          onClick={onActClick}
        />
      ))}
    </Grid>
  );
}