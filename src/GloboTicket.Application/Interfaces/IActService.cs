using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing act operations.
/// Defines the contract for act-related business logic.
/// All operations respect tenant isolation through query filters.
/// </summary>
public interface IActService
{
    /// <summary>
    /// Retrieves an act by its unique identifier.
    /// </summary>
    /// <param name="id">The act ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The act DTO if found; otherwise, null.</returns>
    Task<ActDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an act by its unique GUID identifier.
    /// </summary>
    /// <param name="actGuid">The act GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The act DTO if found; otherwise, null.</returns>
    Task<ActDto?> GetByGuidAsync(Guid actGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all acts for the current tenant.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all act DTOs for the current tenant.</returns>
    Task<IEnumerable<ActDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new act for the current tenant.
    /// </summary>
    /// <param name="dto">The act creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created act DTO.</returns>
    Task<ActDto> CreateAsync(CreateActDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing act.
    /// </summary>
    /// <param name="id">The act ID to update.</param>
    /// <param name="dto">The act update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated act DTO if found; otherwise, null.</returns>
    Task<ActDto?> UpdateAsync(int id, UpdateActDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an act by its unique identifier.
    /// </summary>
    /// <param name="id">The act ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the act was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of acts for the current tenant.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The total number of acts for the current tenant.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}