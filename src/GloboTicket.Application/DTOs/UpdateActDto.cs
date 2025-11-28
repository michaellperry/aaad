using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing act.
/// Contains validation rules for act updates.
/// </summary>
public class UpdateActDto
{
    /// <summary>
    /// Gets or sets the name of the act.
    /// Maximum length: 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}