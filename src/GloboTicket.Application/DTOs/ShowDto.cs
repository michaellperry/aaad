namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing a show.
/// Used for read operations and API responses.
/// </summary>
public class ShowDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the show.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this show.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the venue.
    /// </summary>
    public Guid VenueGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the venue.
    /// </summary>
    public string VenueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the act.
    /// </summary>
    public Guid ActGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the act.
    /// </summary>
    public string ActName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the show will occur.
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the show was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the show was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}