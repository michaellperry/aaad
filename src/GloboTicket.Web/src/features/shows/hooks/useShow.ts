import { useQuery } from '@tanstack/react-query';
import { getShow } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { Show } from '../../../types/show';

/**
 * Hook to fetch a single show by ID
 * @param id - The show GUID
 * @returns Query result with show data, loading state, and error
 */
export function useShow(id: string | undefined) {
  return useQuery<Show, Error>({
    queryKey: queryKeys.shows.detail(id || ''),
    queryFn: () => getShow(id!),
    enabled: !!id,
  });
}
