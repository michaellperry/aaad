namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing a venue.
/// Used for read operations and API responses.
/// </summary>
public class VenueDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the venue.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this venue.
    /// </summary>
    public Guid VenueGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the venue.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical address of the venue.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the latitude coordinate of the venue location.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the venue location.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the maximum seating capacity of the venue.
    /// </summary>
    public int SeatingCapacity { get; set; }

    /// <summary>
    /// Gets or sets a description of the venue.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the venue was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the venue was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
