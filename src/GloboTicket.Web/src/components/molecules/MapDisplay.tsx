import { useEffect, useRef, useState } from 'react';
import Map, { Marker } from 'react-map-gl';
import type { MapRef } from 'react-map-gl';
import 'mapbox-gl/dist/mapbox-gl.css';
import { MapMarker } from '../atoms/MapMarker';
import { useTheme } from '../../theme/ThemeProvider';
import { cn } from '../../utils';

export interface MapDisplayProps {
  /**
   * Selected location coordinates
   */
  latitude?: number;
  longitude?: number;
  
  /**
   * Callback when user clicks on the map
   */
  onMapClick?: (lat: number, lng: number) => void;
  
  /**
   * Optional className for styling
   */
  className?: string;
}

/**
 * Map display component using Mapbox GL JS.
 * Shows a marker at the selected location and supports click-to-select.
 * Auto-switches map style based on theme (light/dark).
 */
export function MapDisplay({
  latitude,
  longitude,
  onMapClick,
  className,
}: MapDisplayProps) {
  const { theme } = useTheme();
  const mapRef = useRef<MapRef>(null);
  const [mapLoaded, setMapLoaded] = useState(false);

  // Determine map style based on theme
  const mapStyle = theme === 'dark' 
    ? 'mapbox://styles/mapbox/dark-v11'
    : 'mapbox://styles/mapbox/light-v11';

  // Default viewport (New York City)
  const defaultViewport = {
    latitude: 40.7128,
    longitude: -74.0060,
    zoom: 10,
  };

  // Update map center when location changes
  useEffect(() => {
    if (mapRef.current && latitude !== undefined && longitude !== undefined) {
      mapRef.current.flyTo({
        center: [longitude, latitude],
        zoom: 14,
        duration: 1000,
      });
    }
  }, [latitude, longitude]);

  const handleMapClick = (event: { lngLat?: { lat: number; lng: number } }) => {
    if (onMapClick && event.lngLat) {
      onMapClick(event.lngLat.lat, event.lngLat.lng);
    }
  };

  // Get Mapbox access token from environment
  // Note: For map rendering, we need a public token (URL-restricted)
  // The geocoding API uses a secret token via backend
  const mapboxToken = import.meta.env.VITE_MAPBOX_ACCESS_TOKEN;

  if (!mapboxToken) {
    return (
      <div
        className={cn(
          'flex items-center justify-center',
          'h-64 md:h-96',
          'rounded-lg border border-border-default',
          'bg-surface-elevated text-text-secondary',
          className
        )}
      >
        <p className="text-sm">Mapbox access token not configured</p>
      </div>
    );
  }

  return (
    <div
      className={cn(
        'w-full h-64 md:h-96',
        'rounded-lg border border-border-default',
        'overflow-hidden',
        className
      )}
    >
      <Map
        ref={mapRef}
        mapboxAccessToken={mapboxToken}
        initialViewState={{
          ...defaultViewport,
          ...(latitude !== undefined && longitude !== undefined
            ? { latitude, longitude, zoom: 14 }
            : {}),
        }}
        style={{ width: '100%', height: '100%' }}
        mapStyle={mapStyle}
        onClick={handleMapClick}
        onLoad={() => setMapLoaded(true)}
        cursor={onMapClick ? 'pointer' : 'default'}
      >
        {mapLoaded && latitude !== undefined && longitude !== undefined && (
          <Marker longitude={longitude} latitude={latitude} anchor="bottom">
            <MapMarker latitude={latitude} longitude={longitude} />
          </Marker>
        )}
      </Map>
    </div>
  );
}

