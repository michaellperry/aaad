namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing a tenant.
/// Used for read operations and API responses.
/// </summary>
public class TenantDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the tenant.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the tenant.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique slug identifier for the tenant.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the tenant is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the tenant was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}