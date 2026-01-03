import { useQuery } from '@tanstack/react-query';
import { getShowsByAct } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { Show } from '../../../types/show';

/**
 * Hook to fetch shows for a specific act
 * @param actGuid - The act GUID
 * @returns Query result with shows data, loading state, and error
 */
export function useShowsByAct(actGuid: string | undefined) {
  return useQuery<Show[], Error>({
    queryKey: queryKeys.shows.byAct(actGuid || ''),
    queryFn: () => getShowsByAct(actGuid!),
    enabled: !!actGuid,
  });
}
