/**
 * TicketOfferCard Molecule
 * 
 * Displays a single ticket offer in a card format.
 * Shows offer name, price per ticket, and ticket count.
 * Includes edit button to navigate to edit page.
 * 
 * Follows atomic design principles - composes atoms with minimal styling.
 * 
 * @requires Text atom for displaying offer information
 * @requires Button atom for edit action
 * @requires Stack layout primitive for vertical arrangement
 * @requires Row layout primitive for horizontal arrangement
 */

import { DollarSign, Ticket, Edit } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { Text } from '../atoms/Text';
import { Button } from '../atoms/Button';
import { Icon } from '../atoms/Icon';
import { Stack } from '../layout/Stack';
import { Row } from '../layout/Row';
import type { TicketOffer } from '../../types/ticketOffer';

export interface TicketOfferCardProps {
  /** Ticket offer data to display */
  offer: TicketOffer;
}

/**
 * Format price as USD currency
 */
function formatPrice(price: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(price);
}

/**
 * TicketOfferCard component for displaying a single ticket offer.
 * 
 * Displays:
 * - Offer name (prominent)
 * - Price per ticket (formatted as currency)
 * - Ticket count (formatted with locale)
 * 
 * Accessibility:
 * - Semantic HTML structure
 * - Icons have descriptive aria-labels
 * - Proper text hierarchy
 * 
 * Responsive:
 * - Adapts to container width
 * - Stacks on mobile, inline on larger screens
 * 
 * @example
 * ```tsx
 * <TicketOfferCard
 *   offer={{
 *     ticketOfferGuid: '123',
 *     name: 'General Admission',
 *     price: 50.00,
 *     ticketCount: 600
 *   }}
 * />
 * ```
 */
export function TicketOfferCard({ offer }: TicketOfferCardProps) {
  const navigate = useNavigate();

  const handleEdit = () => {
    navigate(`/ticket-offers/${offer.ticketOfferGuid}/edit`);
  };

  return (
    <div className="p-4 rounded-lg border border-border-default bg-surface-base hover:shadow-sm transition-shadow duration-200">
      <Stack gap="sm">
        {/* Header with Name and Edit Button */}
        <div className="flex items-start justify-between gap-4">
          <Text size="lg" className="font-semibold flex-1">
            {offer.name}
          </Text>
          <Button
            variant="ghost"
            size="sm"
            onClick={handleEdit}
            aria-label={`Edit ${offer.name}`}
            className="flex-shrink-0"
          >
            <Icon icon={Edit} size="sm" />
          </Button>
        </div>
        
        {/* Price and Ticket Count */}
        <div className="flex flex-col sm:flex-row sm:items-center gap-3 sm:gap-6">
          {/* Price */}
          <Row align="center" gap="xs">
            <Icon 
              icon={DollarSign} 
              size="sm" 
              className="text-text-secondary flex-shrink-0"
              aria-label="Price"
            />
            <Text variant="muted" size="sm">
              {formatPrice(offer.price)} per ticket
            </Text>
          </Row>
          
          {/* Ticket Count */}
          <Row align="center" gap="xs">
            <Icon 
              icon={Ticket} 
              size="sm" 
              className="text-text-secondary flex-shrink-0"
              aria-label="Tickets"
            />
            <Text variant="muted" size="sm">
              {offer.ticketCount.toLocaleString()} tickets
            </Text>
          </Row>
        </div>
      </Stack>
    </div>
  );
}
