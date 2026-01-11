/**
 * EditTicketOfferPage
 * 
 * Page for editing an existing ticket offer.
 * Follows atomic design principles - composes organisms and molecules.
 */

import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { Heading, Text, Button, Spinner } from '../../components/atoms';
import { Card } from '../../components/molecules';
import { Stack, Container } from '../../components/layout';
import { TicketOfferForm } from '../../components/organisms';
import { useTicketOffer } from '../../features/ticketOffers/hooks';
import { getShowCapacity } from '../../api/client';
import type { ShowCapacity } from '../../types/ticketOffer';

/**
 * EditTicketOfferPage component for editing existing ticket offers.
 * 
 * @example
 * ```tsx
 * <Route path="/ticket-offers/:ticketOfferGuid/edit" element={<EditTicketOfferPage />} />
 * ```
 */
export function EditTicketOfferPage() {
  const { ticketOfferGuid } = useParams<{ ticketOfferGuid: string }>();
  const navigate = useNavigate();
  
  const [capacity, setCapacity] = useState<ShowCapacity | null>(null);
  const [capacityError, setCapacityError] = useState<string | null>(null);

  // Fetch ticket offer
  const { data: ticketOffer, isLoading, error } = useTicketOffer(ticketOfferGuid);

  // Fetch capacity when ticket offer is loaded
  useEffect(() => {
    const fetchCapacity = async () => {
      if (ticketOffer?.showGuid) {
        try {
          const capacityData = await getShowCapacity(ticketOffer.showGuid);
          setCapacity(capacityData);
        } catch (err) {
          setCapacityError(err instanceof Error ? err.message : 'Failed to load capacity');
        }
      }
    };

    fetchCapacity();
  }, [ticketOffer]);

  const handleSuccess = () => {
    if (ticketOffer) {
      navigate(`/shows/${ticketOffer.showGuid}`);
    }
  };

  const handleCancel = () => {
    if (ticketOffer) {
      navigate(`/shows/${ticketOffer.showGuid}`);
    } else {
      navigate('/acts');
    }
  };

  const handleBack = () => {
    if (ticketOffer) {
      navigate(`/shows/${ticketOffer.showGuid}`);
    } else {
      navigate('/acts');
    }
  };

  if (isLoading || !capacity) {
    return (
      <Container>
        <div className="flex justify-center items-center min-h-[400px]">
          <Spinner size="lg" />
        </div>
      </Container>
    );
  }

  if (error || !ticketOffer) {
    return (
      <Container>
        <Stack gap="xl">
          <Button
            variant="ghost"
            size="sm"
            onClick={handleBack}
            className="self-start"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Back
          </Button>
          
          <div className="space-y-4">
            <Heading level="h1" className="text-3xl">
              Edit Ticket Offer
            </Heading>
            <Text variant="muted">
              Update ticket offer information
            </Text>
          </div>

          <Card>
            <div className="p-8 text-center">
              <Text className="text-error">
                {error?.message || 'Ticket offer not found'}
              </Text>
            </div>
          </Card>
        </Stack>
      </Container>
    );
  }

  if (capacityError) {
    return (
      <Container>
        <Stack gap="xl">
          <Button
            variant="ghost"
            size="sm"
            onClick={handleBack}
            className="self-start"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Back
          </Button>

          <Card>
            <div className="p-8 text-center">
              <Text className="text-error">
                {capacityError}
              </Text>
            </div>
          </Card>
        </Stack>
      </Container>
    );
  }

  // Calculate available capacity for this offer (includes current offer's allocation)
  const availableCapacityForOffer = capacity.availableCapacity + ticketOffer.ticketCount;

  return (
    <Container>
      <Stack gap="xl">
        <Button
          variant="ghost"
          size="sm"
          onClick={handleBack}
          className="self-start"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Back to Show
        </Button>

        <div className="space-y-4">
          <Heading level="h1" className="text-3xl">
            Edit Ticket Offer
          </Heading>
          <Text variant="muted">
            Update the ticket offer details below
          </Text>
        </div>

        <Card>
          <div className="p-6">
            <TicketOfferForm
              showGuid={ticketOffer.showGuid}
              availableCapacity={availableCapacityForOffer}
              ticketOffer={ticketOffer}
              onSuccess={handleSuccess}
              onCancel={handleCancel}
            />
          </div>
        </Card>
      </Stack>
    </Container>
  );
}
