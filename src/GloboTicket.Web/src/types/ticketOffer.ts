/**
 * Type definitions for Ticket Offer domain
 * 
 * These types align with backend DTOs:
 * - TicketOfferDto
 * - CreateTicketOfferDto
 * - ShowCapacityDto
 */

export interface TicketOffer {
  id: number;
  ticketOfferGuid: string;
  showGuid: string;
  name: string;
  price: number;
  ticketCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTicketOfferDto {
  ticketOfferGuid: string;
  name: string;
  price: number;
  ticketCount: number;
}

export interface UpdateTicketOfferDto {
  name: string;
  price: number;
  ticketCount: number;
}

export interface ShowCapacity {
  showGuid: string;
  totalTickets: number;
  allocatedTickets: number;
  availableCapacity: number;
}
