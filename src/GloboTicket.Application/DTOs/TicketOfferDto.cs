namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for ticket offer information.
/// </summary>
public class TicketOfferDto
{
    /// <summary>
    /// Gets or sets the database-generated unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the client-generated unique identifier.
    /// </summary>
    public Guid TicketOfferGuid { get; set; }

    /// <summary>
    /// Gets or sets the GUID of the associated show.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the ticket offer.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price per ticket.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets allocated to this offer.
    /// </summary>
    public int TicketCount { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the offer was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the offer was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
