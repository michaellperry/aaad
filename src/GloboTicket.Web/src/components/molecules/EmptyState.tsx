import type { ReactNode } from 'react';
import type { LucideIcon } from 'lucide-react';
import { Stack } from '../layout';
import { Heading, Text, Button, Icon } from '../atoms';
import type { ButtonProps } from '../atoms';

export interface EmptyStateProps {
  /**
   * Icon to display
   */
  icon: LucideIcon;
  
  /**
   * Title text
   */
  title: string;
  
  /**
   * Description text
   */
  description?: string;
  
  /**
   * Action button text
   */
  actionLabel?: string;
  
  /**
   * Action button click handler
   */
  onAction?: () => void;
  
  /**
   * Action button variant
   */
  actionVariant?: ButtonProps['variant'];
  
  /**
   * Custom action element (overrides actionLabel/onAction)
   */
  action?: ReactNode;
}

/**
 * Empty state component with icon, title, description, and optional action.
 * 
 * @example
 * ```tsx
 * import { Inbox } from 'lucide-react';
 * 
 * <EmptyState
 *   icon={Inbox}
 *   title="No items found"
 *   description="Get started by creating your first item"
 *   actionLabel="Create Item"
 *   onAction={() => console.log('Create')}
 * />
 * ```
 */
export const EmptyState = ({
  icon,
  title,
  description,
  actionLabel,
  onAction,
  actionVariant = 'primary',
  action,
}: EmptyStateProps) => {
  return (
    <Stack gap="lg" align="center" className="py-12 px-4 text-center">
      <div className="w-16 h-16 rounded-full bg-surface-elevated flex items-center justify-center">
        <Icon icon={icon} size="xl" className="text-text-secondary" />
      </div>
      
      <Stack gap="sm" align="center">
        <Heading level="h3" variant="default">
          {title}
        </Heading>
        
        {description && (
          <Text variant="muted" className="max-w-md">
            {description}
          </Text>
        )}
      </Stack>
      
      {action || (actionLabel && onAction && (
        <Button
          variant={actionVariant}
          onClick={onAction}
        >
          {actionLabel}
        </Button>
      ))}
    </Stack>
  );
};