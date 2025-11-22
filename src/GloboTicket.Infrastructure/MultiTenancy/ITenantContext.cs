namespace GloboTicket.Infrastructure.MultiTenancy;

/// <summary>
/// Provides the current tenant context for multi-tenant data access.
/// This interface is injected into the DbContext to enable tenant-specific query filtering.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant identifier.
    /// Returns null when operating in a tenant-agnostic context (e.g., during migrations or tenant management).
    /// </summary>
    int? CurrentTenantId { get; }
}