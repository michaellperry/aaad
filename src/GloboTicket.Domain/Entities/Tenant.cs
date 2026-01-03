namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a tenant in the multi-tenant system.
/// Each tenant has isolated data through the TenantId discriminator.
/// </summary>
public class Tenant : Entity
{
    /// <summary>
    /// Gets or sets the natural key identifier for the tenant.
    /// This human-readable identifier is used for lookups and references.
    /// Maximum length: 100 characters.
    /// </summary>
    public string TenantIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the tenant.
    /// Maximum length: 200 characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique slug identifier for the tenant.
    /// Used for URL routing and identification.
    /// Maximum length: 50 characters.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the tenant is active.
    /// Inactive tenants cannot access the system.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tenant"/> class.
    /// </summary>
    public Tenant()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tenant"/> class with specified values.
    /// </summary>
    /// <param name="tenantIdentifier">The natural key identifier for the tenant.</param>
    /// <param name="name">The tenant name.</param>
    /// <param name="slug">The unique slug identifier.</param>
    /// <param name="isActive">Whether the tenant is active. Defaults to true.</param>
    public Tenant(string tenantIdentifier, string name, string slug, bool isActive = true)
    {
        TenantIdentifier = tenantIdentifier;
        Name = name;
        Slug = slug;
        IsActive = isActive;
    }

    /// <summary>
    /// Returns a string representation of the tenant.
    /// </summary>
    /// <returns>The tenant name.</returns>
    public override string ToString()
    {
        return Name;
    }
}
