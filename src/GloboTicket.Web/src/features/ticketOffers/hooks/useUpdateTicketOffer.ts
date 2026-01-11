import { useMutation, useQueryClient } from '@tanstack/react-query';
import { updateTicketOffer } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { TicketOffer, UpdateTicketOfferDto } from '../../../types/ticketOffer';

interface UpdateTicketOfferRequest {
  ticketOfferGuid: string;
  dto: UpdateTicketOfferDto;
}

/**
 * Hook to update an existing ticket offer
 * Invalidates ticket offers and capacity queries on success to refresh the data
 * @returns Mutation result with mutate function, loading state, and error
 */
export function useUpdateTicketOffer() {
  const queryClient = useQueryClient();

  return useMutation<TicketOffer, Error, UpdateTicketOfferRequest>({
    mutationFn: ({ ticketOfferGuid, dto }) => updateTicketOffer(ticketOfferGuid, dto),
    onSuccess: (updatedOffer) => {
      // Invalidate the specific ticket offer query
      queryClient.invalidateQueries({
        queryKey: queryKeys.ticketOffers.byGuid(updatedOffer.ticketOfferGuid),
      });
      // Invalidate ticket offers list for the show
      queryClient.invalidateQueries({
        queryKey: queryKeys.ticketOffers.byShow(updatedOffer.showGuid),
      });
      // Invalidate capacity query for the show
      queryClient.invalidateQueries({
        queryKey: queryKeys.capacity.byShow(updatedOffer.showGuid),
      });
    },
  });
}
