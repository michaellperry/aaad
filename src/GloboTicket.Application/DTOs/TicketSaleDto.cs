namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing a ticket sale.
/// Used for read operations and API responses.
/// </summary>
public class TicketSaleDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the ticket sale.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this ticket sale.
    /// </summary>
    public Guid TicketSaleGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the show.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the show will occur.
    /// </summary>
    public DateTimeOffset ShowDate { get; set; }

    /// <summary>
    /// Gets or sets the name of the venue where the show will be held.
    /// </summary>
    public string VenueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the act performing at the show.
    /// </summary>
    public string ActName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of tickets sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the ticket sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the ticket sale was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}