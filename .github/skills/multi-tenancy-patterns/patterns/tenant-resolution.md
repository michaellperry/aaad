# Tenant Resolution

Middleware to resolve tenant from subdomain, custom domain, header, or JWT claim.

```csharp
public class TenantResolutionMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next, ITenantContext tenantContext)
    {
        var tenantId = await ResolveTenantId(context);
        tenantContext.TenantId = tenantId;
        context.Items["TenantId"] = tenantId;
        await next(context);
    }
    
    private async Task<Guid> ResolveTenantId(HttpContext context)
    {
        // 1. Subdomain: tenant1.globoticket.com
        var subdomain = GetSubdomain(context.Request.Host);
        if (!string.IsNullOrEmpty(subdomain))
            return await _tenantService.GetTenantIdBySubdomainAsync(subdomain);
        
        // 2. Custom domain: tickets.company.com
        var domain = context.Request.Host.Value;
        if (await _tenantService.IsCustomDomainAsync(domain))
            return await _tenantService.GetTenantIdByDomainAsync(domain);
        
        // 3. Header: X-Tenant-Id (for API testing)
        var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(headerTenantId, out var parsedTenantId))
            return parsedTenantId;
        
        // 4. JWT token claim
        var user = context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = user.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var jwtTenantId))
                return jwtTenantId;
        }
        
        throw new TenantResolutionException("Unable to resolve tenant");
    }
}
```

Priority: subdomain → custom domain → header → JWT claim.
