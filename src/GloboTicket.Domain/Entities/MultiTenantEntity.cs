using GloboTicket.Domain.Interfaces;

namespace GloboTicket.Domain.Entities;

/// <summary>
/// Abstract base class for entities that belong to a specific tenant.
/// Provides multi-tenant isolation capabilities by implementing ITenantEntity.
/// </summary>
public abstract class MultiTenantEntity : Entity, ITenantEntity
{
    /// <summary>
    /// Gets the tenant identifier that this entity belongs to.
    /// This property is managed by Entity Framework Core through the Tenant navigation property.
    /// Set via the <see cref="Tenant"/> navigation property instead of directly.
    /// </summary>
    public int TenantId { get; private set; } = 0;

    /// <summary>
    /// Gets or sets the navigation property to the tenant that owns this entity.
    /// Setting this property automatically updates the TenantId foreign key.
    /// </summary>
    public Tenant? Tenant { get; set; } = null;
}