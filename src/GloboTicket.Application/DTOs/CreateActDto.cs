using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new act.
/// Contains validation rules for act creation.
/// </summary>
public class CreateActDto
{
    /// <summary>
    /// Gets or sets the unique identifier for this act.
    /// </summary>
    [Required]
    public Guid ActGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the act.
    /// Maximum length: 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}