import { Fragment } from 'react';
import { Menu, MenuButton, MenuItems, MenuItem, Transition } from '@headlessui/react';
import type { ReactNode } from 'react';
import type { LucideIcon } from 'lucide-react';
import { cn } from '../../utils';
import { Icon } from '../atoms';

export interface DropdownItem {
  /**
   * Unique identifier
   */
  id: string;
  
  /**
   * Label text
   */
  label: string;
  
  /**
   * Optional icon
   */
  icon?: LucideIcon;
  
  /**
   * Click handler
   */
  onClick?: () => void;
  
  /**
   * Whether the item is disabled
   */
  disabled?: boolean;
  
  /**
   * Whether the item is destructive (red)
   */
  destructive?: boolean;
  
  /**
   * Divider after this item
   */
  divider?: boolean;
}

export interface DropdownProps {
  /**
   * Trigger element
   */
  trigger: ReactNode;
  
  /**
   * Menu items
   */
  items: DropdownItem[];
  
  /**
   * Alignment of the dropdown menu
   * @default 'right'
   */
  align?: 'left' | 'right';
}

/**
 * Dropdown menu component using Headless UI.
 * 
 * @example
 * ```tsx
 * import { Settings, LogOut } from 'lucide-react';
 * 
 * <Dropdown
 *   trigger={<Button>Menu</Button>}
 *   items={[
 *     { id: '1', label: 'Settings', icon: Settings, onClick: () => {} },
 *     { id: '2', label: 'Logout', icon: LogOut, onClick: () => {}, destructive: true },
 *   ]}
 * />
 * ```
 */
export const Dropdown = ({
  trigger,
  items,
  align = 'right',
}: DropdownProps) => {
  return (
    <Menu as="div" className="relative inline-block text-left">
      <MenuButton as={Fragment}>
        {trigger}
      </MenuButton>

      <Transition
        as={Fragment}
        enter="transition ease-out duration-100"
        enterFrom="transform opacity-0 scale-95"
        enterTo="transform opacity-100 scale-100"
        leave="transition ease-in duration-75"
        leaveFrom="transform opacity-100 scale-100"
        leaveTo="transform opacity-0 scale-95"
      >
        <MenuItems
          className={cn(
            'absolute z-10 mt-2 w-56',
            'origin-top-right rounded-lg',
            'bg-surface-base border border-border-default',
            'shadow-lg',
            'focus:outline-none',
            'py-1',
            align === 'right' ? 'right-0' : 'left-0'
          )}
        >
          {items.map((item) => (
            <Fragment key={item.id}>
              <MenuItem disabled={item.disabled}>
                {({ active }) => (
                  <button
                    onClick={item.onClick}
                    className={cn(
                      'w-full flex items-center gap-3',
                      'px-4 py-2',
                      'text-sm text-left',
                      'transition-colors',
                      active && 'bg-surface-elevated',
                      item.disabled && 'opacity-50 cursor-not-allowed',
                      item.destructive
                        ? 'text-error'
                        : 'text-text-primary'
                    )}
                    disabled={item.disabled}
                  >
                    {item.icon && (
                      <Icon icon={item.icon} size="sm" />
                    )}
                    <span>{item.label}</span>
                  </button>
                )}
              </MenuItem>
              {item.divider && (
                <div className="my-1 border-t border-border-default" />
              )}
            </Fragment>
          ))}
        </MenuItems>
      </Transition>
    </Menu>
  );
};