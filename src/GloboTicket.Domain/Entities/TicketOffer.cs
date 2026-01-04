namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a ticket offer for a show with specific pricing and inventory allocation.
/// TicketOffer is a child entity that inherits tenant context through its Show relationship.
/// </summary>
public class TicketOffer : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TicketOffer"/> class.
    /// </summary>
    /// <param name="show">The Show this ticket offer belongs to.</param>
    /// <param name="ticketOfferGuid">The unique identifier (GUID) for this ticket offer.</param>
    /// <param name="name">The name of the ticket offer (e.g., "General Admission", "VIP").</param>
    /// <param name="price">The price per ticket.</param>
    /// <param name="ticketCount">The number of tickets allocated to this offer.</param>
    /// <exception cref="ArgumentNullException">Thrown when show is null.</exception>
    public TicketOffer(Show show, Guid ticketOfferGuid, string name, decimal price, int ticketCount)
    {
        ArgumentNullException.ThrowIfNull(show);

        Show = show;
        ShowId = show.Id;
        TicketOfferGuid = ticketOfferGuid;
        Name = name;
        Price = price;
        TicketCount = ticketCount;
        
        // Add this ticket offer to the show's collection
        show.TicketOffers.Add(this);
    }

    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private TicketOffer()
    {
        Show = null!;
        Name = string.Empty;
    }

    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this ticket offer.
    /// </summary>
    public Guid TicketOfferGuid { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets the foreign key to the Show this ticket offer belongs to.
    /// </summary>
    public int ShowId { get; private set; }

    /// <summary>
    /// Gets the Show this ticket offer belongs to.
    /// </summary>
    public Show Show { get; private set; }

    /// <summary>
    /// Gets or sets the name of the ticket offer (e.g., "General Admission", "VIP").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price per ticket.
    /// </summary>
    public decimal Price { get; set; } = 0m;

    /// <summary>
    /// Gets or sets the number of tickets allocated to this offer.
    /// </summary>
    public int TicketCount { get; set; } = 0;
}
