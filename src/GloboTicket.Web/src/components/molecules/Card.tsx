import type { ReactNode, HTMLAttributes } from 'react';
import { cn } from '../../utils';

export interface CardProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Optional header content
   */
  header?: ReactNode;
  
  /**
   * Main body content
   */
  children: ReactNode;
  
  /**
   * Optional footer content
   */
  footer?: ReactNode;
  
  /**
   * Whether the card should have padding
   * @default true
   */
  padded?: boolean;
  
  /**
   * Whether the card should be interactive (hover effect)
   * @default false
   */
  interactive?: boolean;
}

/**
 * Card container component with optional header, body, and footer sections.
 * 
 * @example
 * ```tsx
 * <Card header={<Heading level="h3">Title</Heading>}>
 *   <Text>Card content goes here</Text>
 * </Card>
 * 
 * <Card 
 *   header="Quick Header"
 *   footer={<Button>Action</Button>}
 *   interactive
 * >
 *   Content
 * </Card>
 * ```
 */
export const Card = ({
  header,
  children,
  footer,
  padded = true,
  interactive = false,
  className,
  ...props
}: CardProps) => {
  const baseStyles = [
    'bg-surface-base',
    'border border-border-default',
    'rounded-lg',
    'shadow-sm',
    'transition-shadow duration-200',
  ];

  const interactiveStyles = interactive && [
    'hover:shadow-md',
    'cursor-pointer',
  ];

  return (
    <div
      className={cn(
        baseStyles,
        interactiveStyles,
        className
      )}
      {...props}
    >
      {header && (
        <div
          className={cn(
            'border-b border-border-default',
            padded && 'px-6 py-4'
          )}
        >
          {header}
        </div>
      )}
      
      <div className={cn(padded && 'px-6 py-4')}>
        {children}
      </div>
      
      {footer && (
        <div
          className={cn(
            'border-t border-border-default',
            padded && 'px-6 py-4'
          )}
        >
          {footer}
        </div>
      )}
    </div>
  );
};