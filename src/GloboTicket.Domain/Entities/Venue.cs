using NetTopologySuite.Geometries;

namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a venue where events can be held.
/// Includes location data and seating capacity information.
/// </summary>
public class Venue : MultiTenantEntity
{
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
    public string? Address { get; set; } = null;

    /// <summary>
    /// Gets or sets the geographic location of the venue as a point (longitude, latitude).
    /// Uses the WGS84 coordinate system (SRID 4326).
    /// </summary>
    public Point? Location { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum seating capacity of the venue.
    /// </summary>
    public int SeatingCapacity { get; set; } = 0;

    /// <summary>
    /// Gets or sets a description of the venue.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}