using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new ticket sale.
/// Contains validation rules for ticket sale creation.
/// </summary>
public class CreateTicketSaleDto
{
    /// <summary>
    /// Gets or sets the unique identifier for this ticket sale.
    /// </summary>
    [Required]
    public Guid TicketSaleGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the show for which tickets are sold.
    /// </summary>
    [Required]
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the quantity of tickets sold.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}