/**
 * CapacityDisplay Molecule
 * 
 * Displays show capacity allocation with visual progress bar.
 * Shows total tickets, allocated tickets, and available capacity with color-coded indicators.
 * 
 * Follows atomic design principles - composes atoms with minimal styling.
 * 
 * @requires Text atom for displaying metrics
 * @requires Stack layout primitive for vertical arrangement
 * @requires Row layout primitive for horizontal arrangement
 */

import { AlertTriangle, CheckCircle } from 'lucide-react';
import { Text } from '../atoms/Text';
import { Icon } from '../atoms/Icon';
import { Stack } from '../layout/Stack';
import { Row } from '../layout/Row';
import { cn } from '../../utils';

export interface CapacityDisplayProps {
  /** Total ticket count for the show */
  totalTickets: number;
  
  /** Sum of tickets across all offers */
  allocatedTickets: number;
  
  /** Remaining capacity (totalTickets - allocatedTickets) */
  availableCapacity: number;
}

/**
 * CapacityDisplay component for showing ticket allocation status.
 * 
 * Displays:
 * - Total tickets
 * - Allocated tickets
 * - Available capacity (with color-coded indicator)
 * - Visual progress bar showing allocation percentage
 * 
 * Color Coding:
 * - Green: > 20% capacity available (healthy)
 * - Yellow: 10-20% capacity available (warning)
 * - Red: < 10% capacity available (critical)
 * - Success indicator: Fully allocated (100%)
 * 
 * Accessibility:
 * - Progress bar uses role="progressbar" with ARIA attributes
 * - Capacity changes announced via aria-live="polite"
 * - Color is not the only indicator (uses icons and text)
 * 
 * @example
 * ```tsx
 * <CapacityDisplay
 *   totalTickets={1000}
 *   allocatedTickets={800}
 *   availableCapacity={200}
 * />
 * ```
 */
export function CapacityDisplay({
  totalTickets,
  allocatedTickets,
  availableCapacity,
}: CapacityDisplayProps) {
  // Calculate allocation percentage
  const allocationPercentage = totalTickets > 0 
    ? (allocatedTickets / totalTickets) * 100 
    : 0;
  
  // Determine capacity status
  const capacityPercentage = totalTickets > 0
    ? (availableCapacity / totalTickets) * 100
    : 0;
  
  const isFullyAllocated = availableCapacity === 0;
  const isLowCapacity = capacityPercentage < 10 && !isFullyAllocated;
  const isMediumCapacity = capacityPercentage >= 10 && capacityPercentage <= 20;
  
  // Determine color scheme
  const getCapacityColor = () => {
    if (isFullyAllocated) return 'text-green-600';
    if (isLowCapacity) return 'text-red-600';
    if (isMediumCapacity) return 'text-yellow-600';
    return 'text-green-600';
  };
  
  const getProgressBarColor = () => {
    if (isFullyAllocated) return 'bg-green-500';
    if (isLowCapacity) return 'bg-red-500';
    if (isMediumCapacity) return 'bg-yellow-500';
    return 'bg-blue-500';
  };

  return (
    <Stack gap="md" aria-live="polite" aria-atomic="true">
      {/* Capacity Metrics */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {/* Total Tickets */}
        <div>
          <Text size="sm" variant="muted" className="mb-1">
            Total Tickets
          </Text>
          <Text size="lg" className="font-semibold">
            {totalTickets.toLocaleString()}
          </Text>
        </div>
        
        {/* Allocated Tickets */}
        <div>
          <Text size="sm" variant="muted" className="mb-1">
            Allocated
          </Text>
          <Text size="lg" className="font-semibold">
            {allocatedTickets.toLocaleString()}
          </Text>
        </div>
        
        {/* Available Capacity */}
        <div>
          <Text size="sm" variant="muted" className="mb-1">
            Available
          </Text>
          <Row align="center" gap="sm">
            <Text size="lg" className={cn('font-semibold', getCapacityColor())}>
              {availableCapacity.toLocaleString()}
            </Text>
            {isFullyAllocated && (
              <Icon 
                icon={CheckCircle} 
                size="sm" 
                className="text-green-600"
                aria-label="Fully allocated"
              />
            )}
            {isLowCapacity && (
              <Icon 
                icon={AlertTriangle} 
                size="sm" 
                className="text-red-600"
                aria-label="Low capacity warning"
              />
            )}
            {isMediumCapacity && (
              <Icon 
                icon={AlertTriangle} 
                size="sm" 
                className="text-yellow-600"
                aria-label="Medium capacity warning"
              />
            )}
          </Row>
        </div>
      </div>
      
      {/* Progress Bar */}
      <div>
        <div className="flex justify-between items-center mb-2">
          <Text size="sm" variant="muted">
            Allocation Progress
          </Text>
          <Text size="sm" variant="muted">
            {allocationPercentage.toFixed(1)}%
          </Text>
        </div>
        <div 
          className="w-full h-3 bg-gray-200 rounded-full overflow-hidden"
          role="progressbar"
          aria-valuenow={allocationPercentage}
          aria-valuemin={0}
          aria-valuemax={100}
          aria-label={`${allocationPercentage.toFixed(1)}% of tickets allocated`}
        >
          <div
            className={cn(
              'h-full transition-all duration-300 ease-in-out',
              getProgressBarColor()
            )}
            style={{ width: `${allocationPercentage}%` }}
          />
        </div>
      </div>
      
      {/* Status Messages */}
      {isFullyAllocated && (
        <div className="p-3 rounded-lg bg-green-50 border border-green-200">
          <Text size="sm" className="text-green-800">
            All tickets have been allocated to offers.
          </Text>
        </div>
      )}
      
      {isLowCapacity && (
        <div className="p-3 rounded-lg bg-red-50 border border-red-200">
          <Text size="sm" className="text-red-800">
            Critical: Less than 10% capacity remaining.
          </Text>
        </div>
      )}
      
      {isMediumCapacity && (
        <div className="p-3 rounded-lg bg-yellow-50 border border-yellow-200">
          <Text size="sm" className="text-yellow-800">
            Warning: Less than 20% capacity remaining.
          </Text>
        </div>
      )}
    </Stack>
  );
}
