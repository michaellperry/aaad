using GloboTicket.Domain.Interfaces;

namespace GloboTicket.Domain.Entities;

/// <summary>
/// Abstract base class for entities that belong to a specific tenant.
/// Provides multi-tenant isolation capabilities by implementing ITenantEntity.
/// </summary>
public abstract class MultiTenantEntity : Entity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the tenant identifier that this entity belongs to.
    /// This property is used for multi-tenant data isolation.
    /// </summary>
    public int TenantId { get; set; } = 0;

    /// <summary>
    /// Gets or sets the navigation property to the tenant that owns this entity.
    /// </summary>
    public Tenant? Tenant { get; set; } = null;
}
