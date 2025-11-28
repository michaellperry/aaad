using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing show operations.
/// Defines the contract for show-related business logic.
/// All operations respect tenant isolation through query filters.
/// </summary>
public interface IShowService
{
    /// <summary>
    /// Retrieves a show by its unique identifier.
    /// </summary>
    /// <param name="id">The show ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The show DTO if found; otherwise, null.</returns>
    Task<ShowDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a show by its unique GUID identifier.
    /// </summary>
    /// <param name="showGuid">The show GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The show DTO if found; otherwise, null.</returns>
    Task<ShowDto?> GetByGuidAsync(Guid showGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all shows for the current tenant.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all show DTOs for the current tenant.</returns>
    Task<IEnumerable<ShowDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new show for the current tenant.
    /// </summary>
    /// <param name="dto">The show creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created show DTO.</returns>
    Task<ShowDto> CreateAsync(CreateShowDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing show.
    /// </summary>
    /// <param name="id">The show ID to update.</param>
    /// <param name="dto">The show update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated show DTO if found; otherwise, null.</returns>
    Task<ShowDto?> UpdateAsync(int id, UpdateShowDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a show by its unique identifier.
    /// </summary>
    /// <param name="id">The show ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the show was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}