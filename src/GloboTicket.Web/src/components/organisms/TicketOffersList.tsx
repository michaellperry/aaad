import { Ticket } from 'lucide-react';
import { Spinner, Text } from '../atoms';
import { EmptyState, TicketOfferCard } from '../molecules';
import { Stack } from '../layout';
import { useTicketOffers } from '../../features/ticketOffers/hooks';

export interface TicketOffersListProps {
  showGuid: string;
}

/**
 * TicketOffersList organism - displays list of ticket offers for a show
 * 
 * Handles loading, error, and empty states automatically.
 * Maps ticket offers to TicketOfferCard components.
 * 
 * @param showGuid - GUID of the show to display offers for
 */
export const TicketOffersList = ({ showGuid }: TicketOffersListProps) => {
  const { data: offers = [], isLoading, error } = useTicketOffers(showGuid);

  if (isLoading) {
    return (
      <div className="flex justify-center py-8">
        <Spinner size="md" />
      </div>
    );
  }

  if (error) {
    return (
      <Text className="text-error" role="alert">
        {error.message}
      </Text>
    );
  }

  if (offers.length === 0) {
    return (
      <EmptyState
        icon={Ticket}
        title="No ticket offers yet"
        description="Create your first ticket offer to start selling tickets for this show."
      />
    );
  }

  return (
    <Stack gap="md">
      {offers.map((offer) => (
        <TicketOfferCard key={offer.ticketOfferGuid} offer={offer} />
      ))}
    </Stack>
  );
};
