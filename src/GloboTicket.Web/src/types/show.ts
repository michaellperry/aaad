/**
 * Show interface matching the backend ShowDto
 */
export interface Show {
  /** Database-generated unique identifier */
  id: number;
  
  /** Unique GUID identifier for the show */
  showGuid: string;
  
  /** GUID of the associated act */
  actGuid: string;
  
  /** Name of the act performing at this show */
  actName: string;
  
  /** GUID of the venue where the show takes place */
  venueGuid: string;
  
  /** Name of the venue where the show takes place */
  venueName: string;
  
  /** Maximum seating capacity of the venue */
  venueCapacity: number;
  
  /** Number of tickets available for this show */
  ticketCount: number;
  
  /** Show start time with timezone offset (ISO 8601 format) */
  startTime: string;
  
  /** UTC timestamp when the show was created */
  createdAt: string;
  
  /** UTC timestamp when the show was last updated */
  updatedAt?: string;
}

/**
 * CreateShowDto interface matching the backend CreateShowDto
 */
export interface CreateShowDto {
  /** Client-generated unique identifier for the show */
  showGuid: string;
  
  /** GUID of the venue where the show will be held */
  venueGuid: string;
  
  /** Number of tickets available (must be <= venue capacity) */
  ticketCount: number;
  
  /** Show start time in ISO 8601 format with timezone offset */
  startTime: string;
}

/**
 * NearbyShow interface for shows within 48 hours at the same venue
 */
export interface NearbyShow {
  /** Unique GUID identifier for the show */
  showGuid: string;
  
  /** Name of the act performing */
  actName: string;
  
  /** Show start time with timezone offset (ISO 8601 format) */
  startTime: string;
}

/**
 * NearbyShowsResponse interface for the nearby shows API response
 */
export interface NearbyShowsResponse {
  /** GUID of the venue */
  venueGuid: string;
  
  /** Name of the venue */
  venueName: string;
  
  /** The time around which nearby shows were searched */
  referenceTime: string;
  
  /** List of nearby shows */
  shows: NearbyShow[];
  
  /** Informational message (e.g., "No other shows scheduled at this venue within 48 hours") */
  message: string;
}
