using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing show operations.
/// Defines the contract for show-related business logic.
/// All operations respect tenant isolation through query filters on the Venue relationship.
/// </summary>
public interface IShowService
{
    /// <summary>
    /// Retrieves a show by its unique GUID identifier.
    /// </summary>
    /// <param name="showGuid">The show GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The show DTO if found; otherwise, null.</returns>
    Task<ShowDto?> GetByGuidAsync(Guid showGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all shows for a specific act.
    /// </summary>
    /// <param name="actGuid">The act GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of show DTOs for the specified act.</returns>
    Task<IEnumerable<ShowDto>> GetShowsByActGuidAsync(Guid actGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new show for the specified act.
    /// Validates that ticket count does not exceed venue capacity and start time is in the future.
    /// </summary>
    /// <param name="actGuid">The act GUID.</param>
    /// <param name="dto">The show creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created show DTO.</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails (ticket count > capacity, start time in past).</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the act or venue is not found.</exception>
    Task<ShowDto> CreateAsync(Guid actGuid, CreateShowDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a show by its unique GUID identifier.
    /// </summary>
    /// <param name="showGuid">The show GUID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the show was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(Guid showGuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets shows at a venue within 48 hours before or after the specified time.
    /// Used for informational display during show creation.
    /// </summary>
    /// <param name="venueGuid">The venue GUID.</param>
    /// <param name="startTime">The reference time for the nearby shows query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response object containing nearby shows and venue information.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the venue is not found.</exception>
    Task<NearbyShowsResponse> GetNearbyShowsAsync(Guid venueGuid, DateTimeOffset startTime, CancellationToken cancellationToken = default);
}
