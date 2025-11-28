namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a show, which is a performance by an act at a venue on a specific date.
/// </summary>
public class Show : MultiTenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this show.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the venue where the show will be held.
    /// </summary>
    public Venue Venue { get; set; }

    /// <summary>
    /// Gets or sets the act performing at this show.
    /// </summary>
    public Act Act { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the show will occur.
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Gets or sets the collection of ticket sales for this show.
    /// </summary>
    public ICollection<TicketSale> TicketSales { get; set; }

    /// <summary>
    /// Initializes a new instance of the Show class with the specified venue and act.
    /// </summary>
    /// <param name="venue">The venue where the show will be held.</param>
    /// <param name="act">The act performing at this show.</param>
    public Show(Venue venue, Act act)
    {
        Venue = venue;
        Act = act;
        TicketSales = new List<TicketSale>();
    }

    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private Show() : this(null!, null!)
    {
    }
}