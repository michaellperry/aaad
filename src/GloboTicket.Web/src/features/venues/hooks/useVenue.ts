import { useQuery } from '@tanstack/react-query';
import { getVenue } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { Venue } from '../../../types/venue';

/**
 * Hook to fetch a single venue by ID
 * @param id - The venue GUID
 * @returns Query result with venue data, loading state, and error
 */
export function useVenue(id: string | undefined) {
  return useQuery<Venue, Error>({
    queryKey: queryKeys.venues.detail(id || ''),
    queryFn: () => getVenue(id!),
    enabled: !!id,
  });
}
