using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing ticket offer.
/// </summary>
public class UpdateTicketOfferDto
{
    /// <summary>
    /// Gets or sets the name of the ticket offer (e.g., "General Admission", "VIP").
    /// </summary>
    [Required(ErrorMessage = "Offer name is required")]
    [StringLength(200, ErrorMessage = "Offer name cannot exceed 200 characters")]
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
