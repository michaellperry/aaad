/**
 * Venue interface matching the backend VenueDto
 */
export interface Venue {
  /** Unique identifier for the venue */
  id: number;
  
  /** Unique GUID identifier for the venue */
  venueGuid: string;
  
  /** Name of the venue */
  name: string;
  
  /** Physical address of the venue */
  address?: string;
  
  /** Latitude coordinate of the venue location */
  latitude?: number;
  
  /** Longitude coordinate of the venue location */
  longitude?: number;
  
  /** Maximum seating capacity of the venue */
  seatingCapacity: number;
  
  /** Description of the venue */
  description: string;
  
  /** UTC timestamp when the venue was created */
  createdAt: string;
  
  /** UTC timestamp when the venue was last updated */
  updatedAt?: string;
}

/**
 * DTO for creating a new venue
 */
export interface CreateVenueDto {
  venueGuid: string;
  name: string;
  address?: string;
  latitude?: number;
  longitude?: number;
  seatingCapacity: number;
  description: string;
}

/**
 * DTO for updating an existing venue
 */
export interface UpdateVenueDto {
  name: string;
  address?: string;
  latitude?: number;
  longitude?: number;
  seatingCapacity: number;
  description: string;
}