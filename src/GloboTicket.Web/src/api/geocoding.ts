import type { GeocodeResult } from '../types/geocoding';
import { RateLimitError } from '../types/geocoding';

const API_BASE_URL = '';

/**
 * Searches for addresses using the geocoding API.
 * @param query - Search query (e.g., "Madison Square Garden" or "New York, NY")
 * @returns Promise resolving to array of geocoding results
 * @throws RateLimitError if rate limit is exceeded
 * @throws Error for other API failures
 */
export async function searchAddresses(query: string): Promise<GeocodeResult[]> {
  if (!query.trim()) {
    return [];
  }

  const response = await fetch(
    `${API_BASE_URL}/api/geocoding/search?query=${encodeURIComponent(query)}`,
    {
      method: 'GET',
      credentials: 'include',
    }
  );

  // Handle rate limiting
  if (response.status === 429) {
    const retryAfterHeader = response.headers.get('Retry-After');
    const retryAfter = retryAfterHeader ? parseInt(retryAfterHeader, 10) : 60;
    
    let errorMessage = 'Too many requests. Please try again in a moment.';
    try {
      const errorData = await response.json();
      if (errorData.message) {
        errorMessage = errorData.message;
      }
    } catch {
      // Use default message if JSON parsing fails
    }

    throw new RateLimitError(errorMessage, retryAfter);
  }

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error: ${response.status} - ${errorText}`);
  }

  return response.json();
}

