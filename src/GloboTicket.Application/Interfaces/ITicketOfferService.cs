using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing ticket offer operations.
/// Defines the contract for ticket offer-related business logic.
/// All operations respect tenant isolation through query filters on the Show → Venue relationship.
/// </summary>
public interface ITicketOfferService
{
    /// <summary>
    /// Creates a new ticket offer for the specified show.
    /// Validates that ticket count does not exceed available capacity using a database transaction.
    /// </summary>
    /// <param name="showGuid">The show GUID.</param>
    /// <param name="dto">The ticket offer creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ticket offer DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the show is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when ticket count exceeds available capacity.</exception>
    Task<TicketOfferDto> CreateTicketOfferAsync(Guid showGuid, CreateTicketOfferDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all ticket offers for a specific show.
    /// </summary>
    /// <param name="showGuid">The show GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of ticket offer DTOs for the specified show.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the show is not found.</exception>
    Task<IEnumerable<TicketOfferDto>> GetTicketOffersByShowAsync(Guid showGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets capacity information for a show including total tickets, allocated tickets, and available capacity.
    /// </summary>
    /// <param name="showGuid">The show GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A capacity information DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the show is not found.</exception>
    Task<ShowCapacityDto> GetShowCapacityAsync(Guid showGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing ticket offer for the specified show.
    /// Validates that the updated ticket count does not exceed available capacity using a database transaction.
    /// </summary>
    /// <param name="ticketOfferGuid">The ticket offer GUID.</param>
    /// <param name="dto">The ticket offer update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ticket offer DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the ticket offer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when ticket count exceeds available capacity.</exception>
    Task<TicketOfferDto> UpdateTicketOfferAsync(Guid ticketOfferGuid, UpdateTicketOfferDto dto, CancellationToken cancellationToken = default);
}
