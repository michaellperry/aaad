import { useQuery } from '@tanstack/react-query';
import { getVenuesCount, getActsCount } from '../../../api/client';
import { queryKeys } from '../../queryKeys';

export interface DashboardStats {
  totalVenues: number;
  activeActs: number;
  isLoading: boolean;
  error: Error | null;
}

/**
 * Hook to fetch dashboard statistics
 * Uses optimized count endpoints to avoid fetching all entity data
 * @returns Dashboard statistics with loading state and error
 */
export function useDashboardStats(): DashboardStats {
  const {
    data: venuesCount,
    isLoading: venuesLoading,
    error: venuesError,
  } = useQuery({
    queryKey: queryKeys.venues.count,
    queryFn: getVenuesCount,
  });

  const {
    data: actsCount,
    isLoading: actsLoading,
    error: actsError,
  } = useQuery({
    queryKey: queryKeys.acts.count,
    queryFn: getActsCount,
  });

  return {
    totalVenues: venuesCount ?? 0,
    activeActs: actsCount ?? 0,
    isLoading: venuesLoading || actsLoading,
    error: venuesError || actsError || null,
  };
}
