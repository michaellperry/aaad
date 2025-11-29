/**
 * ActCard Molecule
 * 
 * Displays act information in a card format.
 * Follows atomic design principles - composes atoms with minimal styling.
 */

import { Music } from 'lucide-react';
import type { Act } from '../../types/act';
import { Card } from './Card';
import { Heading } from '../atoms/Heading';
import { Text } from '../atoms/Text';
import { Badge } from '../atoms/Badge';
import { Icon } from '../atoms/Icon';
import { Stack } from '../layout/Stack';
import { Row } from '../layout/Row';

export interface ActCardProps {
  /** Act data to display */
  act: Act;
  
  /** Optional click handler */
  onClick?: (act: Act) => void;
}

/**
 * ActCard component for displaying act information.
 * 
 * @example
 * ```tsx
 * <ActCard 
 *   act={actData}
 *   onClick={(act) => navigate(`/acts/${act.actGuid}`)}
 * />
 * ```
 */
export function ActCard({ act, onClick }: ActCardProps) {
  const handleClick = () => {
    if (onClick) {
      onClick(act);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (onClick && (e.key === 'Enter' || e.key === ' ')) {
      e.preventDefault();
      onClick(act);
    }
  };

  return (
    <Card
      interactive={!!onClick}
      onClick={onClick ? handleClick : undefined}
      onKeyDown={onClick ? handleKeyDown : undefined}
      tabIndex={onClick ? 0 : undefined}
      role={onClick ? 'button' : undefined}
      aria-label={onClick ? `View details for ${act.name}` : undefined}
    >
      <Stack gap="md">
        <Row justify="between" align="start">
          <Heading level="h3" className="flex-1 overflow-hidden text-ellipsis whitespace-nowrap mr-2">
            {act.name}
          </Heading>
          <Badge variant="info">
            <Icon icon={Music} size="xs" className="mr-1" />
          </Badge>
        </Row>

        <Text variant="muted" size="sm">
          Created {new Date(act.createdAt).toLocaleDateString()}
        </Text>
      </Stack>
    </Card>
  );
}