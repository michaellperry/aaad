using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing ticket sale operations.
/// Defines the contract for ticket sale-related business logic.
/// All operations respect tenant isolation through query filters.
/// </summary>
public interface ITicketSaleService
{
    /// <summary>
    /// Retrieves a ticket sale by its unique identifier.
    /// </summary>
    /// <param name="id">The ticket sale ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket sale DTO if found; otherwise, null.</returns>
    Task<TicketSaleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a ticket sale by its unique GUID identifier.
    /// </summary>
    /// <param name="ticketSaleGuid">The ticket sale GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket sale DTO if found; otherwise, null.</returns>
    Task<TicketSaleDto?> GetByGuidAsync(Guid ticketSaleGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all ticket sales for the current tenant.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all ticket sale DTOs for the current tenant.</returns>
    Task<IEnumerable<TicketSaleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all ticket sales for a specific show.
    /// </summary>
    /// <param name="showGuid">The show GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of ticket sale DTOs for the specified show.</returns>
    Task<IEnumerable<TicketSaleDto>> GetByShowGuidAsync(Guid showGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new ticket sale for the current tenant.
    /// </summary>
    /// <param name="dto">The ticket sale creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ticket sale DTO.</returns>
    Task<TicketSaleDto> CreateAsync(CreateTicketSaleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing ticket sale.
    /// </summary>
    /// <param name="id">The ticket sale ID to update.</param>
    /// <param name="dto">The ticket sale update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ticket sale DTO if found; otherwise, null.</returns>
    Task<TicketSaleDto?> UpdateAsync(int id, UpdateTicketSaleDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a ticket sale by its unique identifier.
    /// </summary>
    /// <param name="id">The ticket sale ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the ticket sale was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}