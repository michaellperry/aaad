import { useQuery } from '@tanstack/react-query';
import { getShowCapacity } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { ShowCapacity } from '../../../types/ticketOffer';

/**
 * Hook to fetch capacity information for a show
 * @param showGuid - GUID of the show to fetch capacity for
 * @returns Query result with capacity information, loading state, and error
 */
export function useShowCapacity(showGuid: string | undefined) {
  return useQuery<ShowCapacity, Error>({
    queryKey: queryKeys.capacity.byShow(showGuid || ''),
    queryFn: () => getShowCapacity(showGuid!),
    enabled: !!showGuid,
  });
}
