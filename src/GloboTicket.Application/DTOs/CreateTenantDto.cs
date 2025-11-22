using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new tenant.
/// Contains validation rules for tenant creation.
/// </summary>
public class CreateTenantDto
{
    /// <summary>
    /// Gets or sets the display name of the tenant.
    /// Maximum length: 200 characters.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique slug identifier for the tenant.
    /// Maximum length: 50 characters.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Slug { get; set; } = string.Empty;
}