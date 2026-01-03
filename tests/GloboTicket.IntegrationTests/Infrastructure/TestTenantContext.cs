using GloboTicket.Infrastructure.MultiTenancy;

namespace GloboTicket.IntegrationTests.Infrastructure;

/// <summary>
/// Test implementation of ITenantContext for integration tests.
/// Allows tests to set a specific tenant context for operations.
/// </summary>
public class TestTenantContext : ITenantContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestTenantContext"/> class.
    /// </summary>
    /// <param name="tenantId">The tenant ID to use for this context. Null for admin/system context.</param>
    public TestTenantContext(int? tenantId)
    {
        CurrentTenantId = tenantId;
    }

    /// <inheritdoc />
    public int? CurrentTenantId { get; }
}
