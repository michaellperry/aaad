/**
 * VenueCard Molecule
 * 
 * Displays venue information in a card format.
 * Follows atomic design principles - composes atoms with minimal styling.
 */

import { MapPin, Users } from 'lucide-react';
import type { Venue } from '../../types/venue';
import { Card } from './Card';
import { Heading } from '../atoms/Heading';
import { Text } from '../atoms/Text';
import { Badge } from '../atoms/Badge';
import { Icon } from '../atoms/Icon';
import { Stack } from '../layout/Stack';

export interface VenueCardProps {
  /** Venue data to display */
  venue: Venue;
  
  /** Optional click handler */
  onClick?: (venue: Venue) => void;
}

/**
 * VenueCard component for displaying venue information.
 * 
 * @example
 * ```tsx
 * <VenueCard 
 *   venue={venueData}
 *   onClick={(venue) => navigate(`/venues/${venue.id}`)}
 * />
 * ```
 */
export function VenueCard({ venue, onClick }: VenueCardProps) {
  const handleClick = () => {
    if (onClick) {
      onClick(venue);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (onClick && (e.key === 'Enter' || e.key === ' ')) {
      e.preventDefault();
      onClick(venue);
    }
  };

  return (
    <Card
      interactive={!!onClick}
      onClick={onClick ? handleClick : undefined}
      onKeyDown={onClick ? handleKeyDown : undefined}
      tabIndex={onClick ? 0 : undefined}
      role={onClick ? 'button' : undefined}
      aria-label={onClick ? `View details for ${venue.name}` : undefined}
    >
      <Stack gap="md">
        <div className="flex items-start justify-between gap-sm">
          <Heading level="h3" className="flex-1 overflow-hidden text-ellipsis whitespace-nowrap mr-2">
            {venue.name}
          </Heading>
          <Badge variant="info">
            <Icon icon={Users} size="xs" className="mr-1" />
            {venue.seatingCapacity.toLocaleString()}
          </Badge>
        </div>

        {venue.address && (
          <div className="flex items-start gap-sm">
            <Icon 
              icon={MapPin} 
              size="sm" 
              className="text-text-secondary flex-shrink-0 mt-0.5" 
            />
            <Text variant="muted" size="sm" className="flex-1 overflow-hidden text-ellipsis whitespace-nowrap">
              {venue.address}
            </Text>
          </div>
        )}

        {venue.description && (
          <Text variant="muted" size="sm" className="line-clamp-2">
            {venue.description}
          </Text>
        )}
      </Stack>
    </Card>
  );
}