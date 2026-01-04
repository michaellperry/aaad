using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new ticket offer.
/// </summary>
public class CreateTicketOfferDto
{
    /// <summary>
    /// Gets or sets the client-generated unique identifier for the ticket offer.
    /// </summary>
    [Required(ErrorMessage = "Ticket offer GUID is required")]
    public Guid TicketOfferGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the ticket offer (e.g., "General Admission", "VIP").
    /// </summary>
    [Required(ErrorMessage = "Offer name is required")]
    [StringLength(100, ErrorMessage = "Offer name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price per ticket.
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets allocated to this offer.
    /// </summary>
    [Required(ErrorMessage = "Ticket count is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Ticket count must be greater than zero")]
    public int TicketCount { get; set; }
}
