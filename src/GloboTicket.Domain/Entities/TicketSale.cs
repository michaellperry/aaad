namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a ticket sale for a show.
/// </summary>
public class TicketSale : Entity
{
    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this ticket sale.
    /// </summary>
    public Guid TicketSaleGuid { get; set; }

    /// <summary>
    /// Gets or sets the show for which tickets are sold.
    /// </summary>
    public Show Show { get; set; }

    /// <summary>
    /// Gets or sets the foreign key for the show.
    /// </summary>
    public int ShowId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of tickets sold.
    /// </summary>
    public int Quantity { get; set; } = 0;

    /// <summary>
    /// Initializes a new instance of the TicketSale class with the specified show.
    /// </summary>
    /// <param name="show">The show for which tickets are sold.</param>
    public TicketSale(Show show)
    {
        Show = show;
    }

    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private TicketSale() : this(null!)
    {
    }
}