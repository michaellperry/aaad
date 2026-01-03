namespace GloboTicket.Domain.Interfaces;

/// <summary>
/// Defines the contract for entities that belong to a specific tenant.
/// Entities implementing this interface will have tenant-level isolation
/// enforced through global query filters in Entity Framework Core.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// Gets the tenant identifier that this entity belongs to.
    /// This property is used for multi-tenant data isolation.
    /// Implementations should manage this through the Tenant navigation property.
    /// </summary>
    int TenantId { get; }
}