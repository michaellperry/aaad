using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing venue operations.
/// Defines the contract for venue-related business logic.
/// All operations respect tenant isolation through query filters.
/// </summary>
public interface IVenueService
{
    /// <summary>
    /// Retrieves a venue by its unique identifier.
    /// </summary>
    /// <param name="id">The venue ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The venue DTO if found; otherwise, null.</returns>
    Task<VenueDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a venue by its unique GUID identifier.
    /// </summary>
    /// <param name="venueGuid">The venue GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The venue DTO if found; otherwise, null.</returns>
    Task<VenueDto?> GetByGuidAsync(Guid venueGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all venues for the current tenant.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all venue DTOs for the current tenant.</returns>
    Task<IEnumerable<VenueDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new venue for the current tenant.
    /// </summary>
    /// <param name="dto">The venue creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created venue DTO.</returns>
    Task<VenueDto> CreateAsync(CreateVenueDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing venue.
    /// </summary>
    /// <param name="id">The venue ID to update.</param>
    /// <param name="dto">The venue update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated venue DTO if found; otherwise, null.</returns>
    Task<VenueDto?> UpdateAsync(int id, UpdateVenueDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a venue by its unique identifier.
    /// </summary>
    /// <param name="id">The venue ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the venue was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
