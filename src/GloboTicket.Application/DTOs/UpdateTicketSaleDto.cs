using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing ticket sale.
/// Contains validation rules for ticket sale updates.
/// </summary>
public class UpdateTicketSaleDto
{
    /// <summary>
    /// Gets or sets the quantity of tickets sold.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}