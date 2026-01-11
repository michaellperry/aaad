import { useQuery } from '@tanstack/react-query';
import { getTicketOffer } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { TicketOffer } from '../../../types/ticketOffer';

/**
 * Hook to get a specific ticket offer by GUID
 * @param ticketOfferGuid - The GUID of the ticket offer to fetch
 * @returns Query result with ticket offer data, loading state, and error
 */
export function useTicketOffer(ticketOfferGuid: string | undefined) {
  return useQuery<TicketOffer, Error>({
    queryKey: queryKeys.ticketOffers.byGuid(ticketOfferGuid || ''),
    queryFn: () => getTicketOffer(ticketOfferGuid!),
    enabled: !!ticketOfferGuid,
  });
}
