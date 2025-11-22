using GloboTicket.Infrastructure.MultiTenancy;
using System.Security.Claims;

namespace GloboTicket.API.Middleware;

/// <summary>
/// Implementation of ITenantContext that extracts tenant information from HTTP context claims.
/// Uses the authenticated user's claims to determine the current tenant.
/// 
/// Tenants provide data isolation within an environment's database. Multiple tenants
/// can coexist in the same database, allowing scenarios like running smoke tests in
/// production without affecting production tenant data.
/// </summary>
public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current tenant ID from the authenticated user's claims.
    /// Returns null if the user is not authenticated or if the TenantId claim is not present.
    /// 
    /// The tenant ID determines which data context the user accesses within the
    /// current environment's database.
    /// </summary>
    public int? CurrentTenantId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var tenantIdClaim = user.FindFirst("TenantId");
            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out var tenantId))
            {
                return tenantId;
            }

            return null;
        }
    }
}