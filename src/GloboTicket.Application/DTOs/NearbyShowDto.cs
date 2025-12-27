namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing a show near a specific time at a venue.
/// Used for the nearby shows query.
/// </summary>
public class NearbyShowDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the show.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the act performing.
    /// </summary>
    public string ActName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the show start time with timezone offset.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }
}
