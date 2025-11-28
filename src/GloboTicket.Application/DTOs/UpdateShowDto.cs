using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing show.
/// Contains validation rules for show updates.
/// Note: Only the date can be updated; venue and act cannot be changed after creation.
/// </summary>
public class UpdateShowDto
{
    /// <summary>
    /// Gets or sets the date and time when the show will occur.
    /// </summary>
    [Required]
    public DateTimeOffset Date { get; set; }
}