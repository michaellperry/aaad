namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a show (performance) where an act performs at a venue.
/// Show is a child entity that inherits tenant context through its Venue relationship.
/// </summary>
public class Show : Entity
{
    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this show.
    /// </summary>
    public Guid ShowGuid { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the Venue where the show will be held.
    /// </summary>
    public int VenueId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the Venue.
    /// </summary>
    public required Venue Venue { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the Act performing at this show.
    /// </summary>
    public int ActId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the Act.
    /// </summary>
    public required Act Act { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets available for this show.
    /// </summary>
    public int TicketCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the start time of the show with timezone offset.
    /// </summary>
    public DateTimeOffset StartTime { get; set; } = default;
}
