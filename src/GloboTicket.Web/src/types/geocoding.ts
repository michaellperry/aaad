/**
 * Geocoding result from the backend API
 */
export interface GeocodeResult {
  /** Unique identifier from Mapbox */
  id: string;
  
  /** Display name for the location */
  displayName: string;
  
  /** Full address string */
  address: string;
  
  /** Latitude coordinate */
  latitude: number;
  
  /** Longitude coordinate */
  longitude: number;
  
  /** Additional context (city, state, etc.) */
  context: string;
}

/**
 * Rate limit error thrown when API returns 429
 */
export class RateLimitError extends Error {
  public readonly retryAfter: number;
  
  constructor(
    message: string,
    retryAfter: number
  ) {
    super(message);
    this.name = 'RateLimitError';
    this.retryAfter = retryAfter;
  }
}

