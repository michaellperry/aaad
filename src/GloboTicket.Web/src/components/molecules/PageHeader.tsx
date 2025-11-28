/**
 * PageHeader Molecule
 * 
 * Reusable page header with title, description, and optional action button.
 * Follows atomic design principles - composes atoms with minimal styling.
 */

import type { ReactNode } from 'react';
import { Heading } from '../atoms/Heading';
import { Text } from '../atoms/Text';
import { Stack } from '../layout/Stack';

export interface PageHeaderProps {
  /** Page title */
  title: string;
  
  /** Optional page description */
  description?: string;
  
  /** Optional action button or element */
  action?: ReactNode;
}

/**
 * PageHeader component for consistent page headers across the application.
 * 
 * @example
 * ```tsx
 * <PageHeader 
 *   title="Venues" 
 *   description="Browse all available venues"
 *   action={<Button>Add Venue</Button>}
 * />
 * ```
 */
export function PageHeader({ title, description, action }: PageHeaderProps) {
  return (
    <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-md">
      <Stack gap="xs" className="flex-1">
        <Heading level="h1">{title}</Heading>
        {description && (
          <Text variant="muted" size="lg">
            {description}
          </Text>
        )}
      </Stack>
      {action && (
        <div className="flex-shrink-0">
          {action}
        </div>
      )}
    </div>
  );
}