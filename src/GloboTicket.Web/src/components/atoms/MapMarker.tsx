import { MapPin } from 'lucide-react';
import { cn } from '../../utils';

export interface MapMarkerProps {
  /**
   * Latitude coordinate
   */
  latitude: number;
  
  /**
   * Longitude coordinate
   */
  longitude: number;
  
  /**
   * Optional className for styling
   */
  className?: string;
}

/**
 * Map marker component for displaying a location on a map.
 * Uses a pin icon styled to match the theme.
 */
export function MapMarker({ className }: MapMarkerProps) {
  return (
    <div
      className={cn(
        'flex items-center justify-center',
        'w-8 h-8',
        'text-brand-primary',
        'drop-shadow-lg',
        className
      )}
      aria-label="Location marker"
    >
      <MapPin className="w-full h-full fill-current" />
    </div>
  );
}

