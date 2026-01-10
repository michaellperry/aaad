using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new tenant.
/// Contains validation rules for tenant creation.
/// </summary>
public class CreateTenantDto
{
    /// <summary>
    /// Gets or sets the natural key identifier for the tenant.
    /// This human-readable identifier is used for lookups and references.
    /// Examples: "globoticket-ny", "venue-chicago", "promoter-la"
    /// Maximum length: 100 characters.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TenantIdentifier { get; set; } = string.Empty;

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
