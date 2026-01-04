/**
 * TicketOfferCard Molecule
 * 
 * Displays a single ticket offer in a card format.
 * Shows offer name, price per ticket, and ticket count.
 * 
 * Follows atomic design principles - composes atoms with minimal styling.
 * 
 * @requires Text atom for displaying offer information
 * @requires Stack layout primitive for vertical arrangement
 * @requires Row layout primitive for horizontal arrangement
 */

import { DollarSign, Ticket } from 'lucide-react';
import { Text } from '../atoms/Text';
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
  return (
    <div className="p-4 rounded-lg border border-border-default bg-surface-base hover:shadow-sm transition-shadow duration-200">
      <Stack gap="sm">
        {/* Offer Name - Primary information */}
        <Text size="lg" className="font-semibold">
          {offer.name}
        </Text>
        
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
