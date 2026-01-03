/**
 * ShowCard Molecule
 * 
 * Displays show information in a clickable card format.
 * Follows atomic design principles - composes atoms with minimal styling.
 * 
 * @requires Show type from '../../types/show'
 * @requires formatDateTime utility for date/time formatting
 */

import { Calendar, MapPin, Ticket } from 'lucide-react';
import { Card } from './Card';
import { Text } from '../atoms/Text';
import { Icon } from '../atoms/Icon';
import { Stack } from '../layout/Stack';
import { Row } from '../layout/Row';
import type { Show } from '../../types/show';

export interface ShowCardProps {
  /** Show data to display */
  show: Show;
  
  /** Click handler for navigation - receives showGuid */
  onClick: (showGuid: string) => void;
}

/**
 * Format date and time together (e.g., "March 15, 2026 at 7:30 PM")
 * NOTE: This should be imported from '../../utils/format' once created
 */
function formatDateTime(dateString: string): string {
  const date = new Date(dateString);
  
  const formattedDate = date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
  
  const formattedTime = date.toLocaleTimeString('en-US', {
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  });
  
  return `${formattedDate} at ${formattedTime}`;
}

/**
 * ShowCard component for displaying show information in a clickable card.
 * 
 * Displays:
 * - Venue name (prominent)
 * - Start date and time (formatted)
 * - Ticket count
 * 
 * Accessibility:
 * - Keyboard navigable (Enter/Space to activate)
 * - ARIA labels for screen readers
 * - Focus visible styles
 * - Semantic button role
 * 
 * @example
 * ```tsx
 * <ShowCard 
 *   show={showData}
 *   onClick={(showGuid) => navigate(`/shows/${showGuid}`)}
 * />
 * ```
 */
export function ShowCard({ show, onClick }: ShowCardProps) {
  const handleClick = () => {
    onClick(show.showGuid);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      onClick(show.showGuid);
    }
  };

  return (
    <Card
      interactive
      onClick={handleClick}
      onKeyDown={handleKeyDown}
      tabIndex={0}
      role="button"
      aria-label={`View show details at ${show.venueName}`}
    >
      <Stack gap="md">
        {/* Venue Name - Primary information */}
        <Row align="start" gap="sm">
          <Icon 
            icon={MapPin} 
            size="sm" 
            className="text-text-secondary flex-shrink-0 mt-0.5" 
          />
          <Text 
            size="lg" 
            className="font-semibold flex-1 overflow-hidden text-ellipsis whitespace-nowrap"
          >
            {show.venueName}
          </Text>
        </Row>

        {/* Start Date and Time */}
        <Row align="start" gap="sm">
          <Icon 
            icon={Calendar} 
            size="sm" 
            className="text-text-secondary flex-shrink-0 mt-0.5" 
          />
          <time dateTime={show.startTime}>
            <Text variant="muted" size="sm">
              {formatDateTime(show.startTime)}
            </Text>
          </time>
        </Row>

        {/* Ticket Count */}
        <Row align="start" gap="sm">
          <Icon 
            icon={Ticket} 
            size="sm" 
            className="text-text-secondary flex-shrink-0 mt-0.5" 
          />
          <Text variant="muted" size="sm">
            {show.ticketCount.toLocaleString()} tickets available
          </Text>
        </Row>
      </Stack>
    </Card>
  );
}
