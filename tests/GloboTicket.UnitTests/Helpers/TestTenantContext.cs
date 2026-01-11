using GloboTicket.Application.MultiTenancy;

namespace GloboTicket.UnitTests.Helpers;

/// <summary>
/// Test implementation of ITenantContext for use in unit tests.
/// Allows setting a fixed tenant ID for testing multi-tenant filtering.
/// </summary>
public class TestTenantContext : ITenantContext
{
    /// <summary>
    /// Gets or sets the current tenant identifier.
    /// </summary>
    public int? CurrentTenantId { get; set; }
}
