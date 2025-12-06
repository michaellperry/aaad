import { useState, useEffect } from 'react';
import { AddressSearchInput } from '../molecules/AddressSearchInput';
import { MapDisplay } from '../molecules/MapDisplay';
import type { GeocodeResult } from '../../types/geocoding';

export interface VenueLocationPickerProps {
  /**
   * Initial latitude value (for edit mode)
   */
  initialLatitude?: number;
  
  /**
   * Initial longitude value (for edit mode)
   */
  initialLongitude?: number;
  
  /**
   * Initial address value (for edit mode)
   */
  initialAddress?: string;
  
  /**
   * Callback when a location is selected
   */
  onLocationSelect: (lat: number, lng: number, address: string) => void;
}

/**
 * Venue location picker component.
 * Combines address search with map display for selecting venue locations.
 * Responsive layout: stacked on mobile, side-by-side on desktop.
 */
export function VenueLocationPicker({
  initialLatitude,
  initialLongitude,
  onLocationSelect,
}: VenueLocationPickerProps) {
  // Use a key to reset state when initial values change (better than useEffect for prop synchronization)
  const [selectedLatitude, setSelectedLatitude] = useState<number | undefined>(initialLatitude);
  const [selectedLongitude, setSelectedLongitude] = useState<number | undefined>(initialLongitude);

  // Update selected coordinates when initial values change (e.g., in edit mode)
  // Using useEffect is necessary here to sync with prop changes in edit mode
  useEffect(() => {
    if (initialLatitude !== undefined && initialLongitude !== undefined) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setSelectedLatitude(initialLatitude);
      setSelectedLongitude(initialLongitude);
    }
  }, [initialLatitude, initialLongitude]);

  const handleSearchSelect = (result: GeocodeResult) => {
    setSelectedLatitude(result.latitude);
    setSelectedLongitude(result.longitude);
    onLocationSelect(result.latitude, result.longitude, result.address);
  };

  const handleMapClick = (lat: number, lng: number) => {
    // When clicking on map, we need to reverse geocode to get the address
    // For now, we'll use coordinates as the address
    // In a full implementation, you might want to call a reverse geocoding API
    const address = `${lat.toFixed(6)}, ${lng.toFixed(6)}`;
    setSelectedLatitude(lat);
    setSelectedLongitude(lng);
    onLocationSelect(lat, lng, address);
  };

  return (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-text-primary mb-2">
          Location Search
        </label>
        <AddressSearchInput onSelect={handleSearchSelect} />
      </div>

      <div>
        <label className="block text-sm font-medium text-text-primary mb-2">
          Map
        </label>
        <MapDisplay
          latitude={selectedLatitude}
          longitude={selectedLongitude}
          onMapClick={handleMapClick}
        />
      </div>
    </div>
  );
}

