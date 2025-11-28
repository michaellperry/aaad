import type { HTMLAttributes } from 'react';
import { cn } from '../../utils';

/**
 * Avatar sizes
 */
export type AvatarSize = 'sm' | 'md' | 'lg' | 'xl';

export interface AvatarProps extends HTMLAttributes<HTMLDivElement> {
  /**
   * Image source URL
   */
  src?: string;
  
  /**
   * Alt text for the image
   */
  alt?: string;
  
  /**
   * Initials to display as fallback
   */
  initials?: string;
  
  /**
   * Size of the avatar
   * @default 'md'
   */
  size?: AvatarSize;
}

/**
 * Avatar component with image and initials fallback.
 * 
 * @example
 * ```tsx
 * <Avatar src="/user.jpg" alt="John Doe" />
 * <Avatar initials="JD" size="lg" />
 * <Avatar initials="AB" size="sm" />
 * ```
 */
export const Avatar = ({
  src,
  alt,
  initials,
  size = 'md',
  className,
  ...props
}: AvatarProps) => {
  const baseStyles = [
    'relative inline-flex items-center justify-center',
    'rounded-full',
    'bg-surface-elevated',
    'text-text-primary font-medium',
    'overflow-hidden',
    'select-none',
  ];

  const sizeStyles = {
    sm: 'w-8 h-8 text-xs',
    md: 'w-10 h-10 text-sm',
    lg: 'w-12 h-12 text-base',
    xl: 'w-16 h-16 text-lg',
  };

  return (
    <div
      className={cn(
        baseStyles,
        sizeStyles[size],
        className
      )}
      {...props}
    >
      {src ? (
        <img
          src={src}
          alt={alt || 'Avatar'}
          className="w-full h-full object-cover"
        />
      ) : (
        <span className="uppercase" aria-label={alt}>
          {initials || '?'}
        </span>
      )}
    </div>
  );
};