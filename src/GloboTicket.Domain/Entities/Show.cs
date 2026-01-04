namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a show (performance) where an act performs at a venue.
/// Show is a child entity that inherits tenant context through its Venue relationship.
/// </summary>
public class Show : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Show"/> class.
    /// </summary>
    /// <param name="act">The Act that is performing in this show.</param>
    /// <param name="venue">The Venue where this show is held.</param>
    /// <exception cref="ArgumentNullException">Thrown when act or venue is null.</exception>
    public Show(Act act, Venue venue)
    {
        ArgumentNullException.ThrowIfNull(act);
        ArgumentNullException.ThrowIfNull(venue);

        Act = act;
        ActId = act.Id;
        Venue = venue;
        VenueId = venue.Id;
        TicketOffers = new List<TicketOffer>();
    }

    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Show()
    {
        Act = null!;
        Venue = null!;
        TicketOffers = new List<TicketOffer>();
    }

    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this show.
    /// </summary>
    public Guid ShowGuid { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets the foreign key to the Venue where the show will be held.
    /// </summary>
    public int VenueId { get; private set; }

    /// <summary>
    /// Gets the Venue where this show is held.
    /// </summary>
    public Venue Venue { get; private set; }

    /// <summary>
    /// Gets the foreign key to the Act performing at this show.
    /// </summary>
    public int ActId { get; private set; }

    /// <summary>
    /// Gets the Act that is performing in this show.
    /// </summary>
    public Act Act { get; private set; }

    /// <summary>
    /// Gets or sets the number of tickets available for this show.
    /// </summary>
    public int TicketCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the start time of the show with timezone offset.
    /// </summary>
    public DateTimeOffset StartTime { get; set; } = default;

    /// <summary>
    /// Gets the collection of ticket offers for this show.
    /// </summary>
    public ICollection<TicketOffer> TicketOffers { get; private set; }

    /// <summary>
    /// Calculates the available capacity for new ticket offers.
    /// </summary>
    /// <returns>The number of tickets available for new offers.</returns>
    public int GetAvailableCapacity()
    {
        if (TicketOffers == null || !TicketOffers.Any())
        {
            return TicketCount;
        }

        var allocatedTickets = TicketOffers.Sum(o => o.TicketCount);
        return TicketCount - allocatedTickets;
    }

    /// <summary>
    /// Validates whether a new ticket offer with the specified ticket count can be added.
    /// </summary>
    /// <param name="ticketCount">The number of tickets for the new offer.</param>
    /// <returns>True if the offer can be added; otherwise, false.</returns>
    public bool CanAddTicketOffer(int ticketCount)
    {
        return ticketCount <= GetAvailableCapacity();
    }
}
