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
