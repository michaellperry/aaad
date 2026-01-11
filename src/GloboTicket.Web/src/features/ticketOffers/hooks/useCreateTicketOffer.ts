import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createTicketOffer } from '../../../api/client';
import { queryKeys } from '../../queryKeys';
import type { TicketOffer, CreateTicketOfferDto } from '../../../types/ticketOffer';

interface CreateTicketOfferRequest {
  showGuid: string;
  dto: CreateTicketOfferDto;
}

/**
 * Hook to create a new ticket offer
 * Invalidates ticket offers and capacity queries on success to refresh the data
 * @returns Mutation result with mutate function, loading state, and error
 */
export function useCreateTicketOffer() {
  const queryClient = useQueryClient();

  return useMutation<TicketOffer, Error, CreateTicketOfferRequest>({
    mutationFn: ({ showGuid, dto }) => createTicketOffer(showGuid, dto),
    onSuccess: (_, { showGuid }) => {
      // Invalidate both ticket offers and capacity queries
      queryClient.invalidateQueries({
        queryKey: queryKeys.ticketOffers.byShow(showGuid),
      });
      queryClient.invalidateQueries({
        queryKey: queryKeys.capacity.byShow(showGuid),
      });
    },
  });
}
