namespace GloboTicket.Application.DTOs;

/// <summary>
/// Response object for the nearby shows query.
/// Contains venue information and list of shows within 48 hours.
/// </summary>
public class NearbyShowsResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the venue.
    /// </summary>
    public Guid VenueGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the venue.
    /// </summary>
    public string VenueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reference time around which nearby shows were searched.
    /// </summary>
    public DateTimeOffset ReferenceTime { get; set; }

    /// <summary>
    /// Gets or sets the list of shows within 48 hours of the reference time.
    /// </summary>
    public List<NearbyShowDto> Shows { get; set; } = new();

    /// <summary>
    /// Gets or sets an informational message about the nearby shows.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
