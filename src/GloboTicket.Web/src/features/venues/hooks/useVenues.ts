import { useQuery } from '@tanstack/react-query';
import { getVenues } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { Venue } from '../../../types/venue';

/**
 * Hook to fetch all venues
 * @returns Query result with venues data, loading state, and error
 */
export function useVenues() {
  return useQuery<Venue[], Error>({
    queryKey: queryKeys.venues.all,
    queryFn: getVenues,
  });
}
