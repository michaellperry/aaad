namespace GloboTicket.API.Middleware;

/// <summary>
/// Middleware that resolves the current tenant from the authenticated user's claims.
/// Logs tenant resolution for debugging purposes.
/// </summary>
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
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("TenantId");
            if (tenantIdClaim != null)
            {
                _logger.LogInformation("Tenant resolved: {TenantId} for user {Username}", 
                    tenantIdClaim.Value, 
                    context.User.Identity.Name);
            }
            else
            {
                _logger.LogWarning("Authenticated user {Username} has no TenantId claim", 
                    context.User.Identity.Name);
            }
        }
        else
        {
            _logger.LogDebug("Request is not authenticated - no tenant context");
        }

        await _next(context);
    }
}