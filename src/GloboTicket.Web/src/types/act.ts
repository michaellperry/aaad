/**
 * Act interface matching the backend ActDto
 */
export interface Act {
  /** Unique identifier for the act */
  id: number;
  
  /** Unique GUID identifier for the act */
  actGuid: string;
  
  /** Name of the act */
  name: string;
  
  /** UTC timestamp when the act was created */
  createdAt: string;
  
  /** UTC timestamp when the act was last updated */
  updatedAt?: string;
}

/**
 * DTO for creating a new act
 */
export interface CreateActDto {
  actGuid: string;
  name: string;
}

/**
 * DTO for updating an existing act
 */
export interface UpdateActDto {
  name: string;
}