# Tenant Resolution Middleware

Resolve tenant from JWT claim first, then header fallback; exclude health/auth paths.

```csharp
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var tenantId = ExtractTenantId(context);
        if (tenantId == null)
        {
            _logger.LogWarning("Authenticated user {Username} has no TenantId claim at {Path}", context.User.Identity?.Name, context.Request.Path);
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Tenant context required");
            return;
        }

        context.Items["TenantId"] = tenantId;
        _logger.LogDebug("Tenant {TenantId} resolved for user {Username}", tenantId, context.User.Identity?.Name);

        await _next(context);
    }

    private Guid? ExtractTenantId(HttpContext context)
    {
        var tenantClaim = context.User.FindFirst("tenant_id");
        if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
        {
            return tenantId;
        }

        var tenantHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (tenantHeader != null && Guid.TryParse(tenantHeader, out var headerTenantId))
        {
            return headerTenantId;
        }

        return null;
    }

    private static bool IsExcludedPath(PathString path)
    {
        var excludedPaths = new[] { "/health", "/auth", "/swagger", "/.well-known" };
        return excludedPaths.Any(excludedPath => path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase));
    }
}
```
