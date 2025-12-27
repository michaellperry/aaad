namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing a show.
/// Used for read operations and API responses.
/// </summary>
public class ShowDto
{
    /// <summary>
    /// Gets or sets the database-generated unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the client-generated unique identifier.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the associated act.
    /// </summary>
    public Guid ActGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the associated act.
    /// </summary>
    public string ActName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GUID of the venue.
    /// </summary>
    public Guid VenueGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the venue.
    /// </summary>
    public string VenueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum capacity of the venue.
    /// </summary>
    public int VenueCapacity { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets available.
    /// </summary>
    public int TicketCount { get; set; }

    /// <summary>
    /// Gets or sets the show start time with timezone offset.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the show was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the show was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
