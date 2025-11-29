/**
 * ActList Organism
 * 
 * Fetches and displays a grid of act cards with loading, error, and empty states.
 * Follows atomic design principles - composes molecules and atoms.
 */

import { useState, useEffect } from 'react';
import { Music, AlertCircle } from 'lucide-react';
import { getActs } from '../../api/client';
import type { Act } from '../../types/act';
import { ActCard } from '../molecules/ActCard';
import { EmptyState } from '../molecules/EmptyState';
import { Spinner } from '../atoms/Spinner';
import { Text } from '../atoms/Text';
import { Grid } from '../layout/Grid';
import { Stack } from '../layout/Stack';

export interface ActListProps {
  /** Optional click handler for act cards */
  onActClick?: (act: Act) => void;
}

/**
 * ActList component that fetches and displays acts in a responsive grid.
 * 
 * @example
 * ```tsx
 * <ActList onActClick={(act) => navigate(`/acts/${act.actGuid}`)} />
 * ```
 */
export function ActList({ onActClick }: ActListProps) {
  const [acts, setActs] = useState<Act[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchActs = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const data = await getActs();
        setActs(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load acts');
      } finally {
        setIsLoading(false);
      }
    };

    fetchActs();
  }, []);

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