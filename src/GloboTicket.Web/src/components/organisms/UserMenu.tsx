import { User, Settings, LogOut } from 'lucide-react';
import { Avatar, Button } from '../atoms';
import { Dropdown } from '../molecules';
import type { DropdownItem } from '../molecules';

export interface UserMenuProps {
  /**
   * User's name
   */
  userName: string;
  
  /**
   * User's email
   */
  userEmail?: string;
  
  /**
   * User's avatar URL
   */
  avatarUrl?: string;
  
  /**
   * User's initials for avatar fallback
   */
  initials?: string;
  
  /**
   * Profile click handler
   */
  onProfile?: () => void;
  
  /**
   * Settings click handler
   */
  onSettings?: () => void;
  
  /**
   * Logout click handler
   */
  onLogout?: () => void;
}

/**
 * User menu dropdown with profile and settings options.
 * 
 * @example
 * ```tsx
 * <UserMenu
 *   userName="John Doe"
 *   userEmail="john@example.com"
 *   initials="JD"
 *   onProfile={() => console.log('Profile')}
 *   onSettings={() => console.log('Settings')}
 *   onLogout={() => console.log('Logout')}
 * />
 * ```
 */
export const UserMenu = ({
  userName,
  userEmail,
  avatarUrl,
  initials,
  onProfile,
  onSettings,
  onLogout,
}: UserMenuProps) => {
  const menuItems: DropdownItem[] = [
    ...(onProfile ? [{
      id: 'profile',
      label: 'Profile',
      icon: User,
      onClick: onProfile,
    }] : []),
    ...(onSettings ? [{
      id: 'settings',
      label: 'Settings',
      icon: Settings,
      onClick: onSettings,
      divider: true,
    }] : []),
    ...(onLogout ? [{
      id: 'logout',
      label: 'Sign out',
      icon: LogOut,
      onClick: onLogout,
      destructive: true,
    }] : []),
  ];

  return (
    <Dropdown
      align="right"
      trigger={
        <Button
          variant="ghost"
          className="gap-2"
          aria-label="User menu"
        >
          <Avatar
            src={avatarUrl}
            initials={initials || userName.substring(0, 2).toUpperCase()}
            alt={userName}
            size="sm"
          />
          <div className="hidden md:flex flex-col items-start">
            <span className="text-sm font-medium text-text-primary">
              {userName}
            </span>
            {userEmail && (
              <span className="text-xs text-text-secondary">
                {userEmail}
              </span>
            )}
          </div>
        </Button>
      }
      items={menuItems}
    />
  );
};