import { useQuery } from '@tanstack/react-query';
import { getTicketOffers } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { TicketOffer } from '../../../types/ticketOffer';

/**
 * Hook to fetch ticket offers for a show
 * @param showGuid - GUID of the show to fetch offers for
 * @returns Query result with ticket offers array, loading state, and error
 */
export function useTicketOffers(showGuid: string | undefined) {
  return useQuery<TicketOffer[], Error>({
    queryKey: queryKeys.ticketOffers.byShow(showGuid || ''),
    queryFn: () => getTicketOffers(showGuid!),
    enabled: !!showGuid,
  });
}
