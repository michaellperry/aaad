using System.Security.Claims;
using GloboTicket.API.Services;

namespace GloboTicket.API.Middleware;

/// <summary>
/// Middleware that applies rate limiting to geocoding endpoints.
/// Tracks requests per authenticated user and returns 429 when limit exceeded.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitService _rateLimitService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IRateLimitService rateLimitService,
        IConfiguration configuration,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _rateLimitService = rateLimitService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply rate limiting to geocoding endpoints
        if (!context.Request.Path.StartsWithSegments("/api/geocoding"))
        {
            await _next(context);
            return;
        }

        // Get user identifier from claims
        var userId = GetUserIdentifier(context);
        if (string.IsNullOrEmpty(userId))
        {
            // If not authenticated, let authorization middleware handle it
            await _next(context);
            return;
        }

        // Get rate limit configuration
        var requestsPerWindow = _configuration.GetValue<int>("RateLimiting:GeocodingSearch:RequestsPerWindow", 60);
        var windowSeconds = _configuration.GetValue<int>("RateLimiting:GeocodingSearch:WindowSeconds", 60);

        // Create rate limit key: endpoint:userId
        var endpoint = context.Request.Path.Value?.Split('/').LastOrDefault() ?? "search";
        var key = $"geocoding:{endpoint}:{userId}";

        // Check rate limit
        var status = _rateLimitService.GetStatus(key, requestsPerWindow, windowSeconds);

        // Add rate limit headers to response
        context.Response.Headers.Append("X-RateLimit-Limit", status.Limit.ToString());
        context.Response.Headers.Append("X-RateLimit-Remaining", status.Remaining.ToString());
        context.Response.Headers.Append("X-RateLimit-Reset", status.Reset.ToString());

        if (!_rateLimitService.IsAllowed(key, requestsPerWindow, windowSeconds))
        {
            // Calculate retry-after seconds
            var resetTime = DateTimeOffset.FromUnixTimeSeconds(status.Reset);
            var retryAfter = (int)Math.Ceiling((resetTime - DateTimeOffset.UtcNow).TotalSeconds);
            retryAfter = Math.Max(1, retryAfter);

            context.Response.Headers.Append("Retry-After", retryAfter.ToString());
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            _logger.LogWarning(
                "Rate limit exceeded for user {UserId} on endpoint {Endpoint}. Retry after {RetryAfter}s",
                userId, endpoint, retryAfter);

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"Too many requests. Please try again in {retryAfter} second(s).",
                retryAfter
            });

            return;
        }

        await _next(context);
    }

    private static string? GetUserIdentifier(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // Prefer NameIdentifier claim, fallback to Name
        var identifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst(ClaimTypes.Name)?.Value
                      ?? user.FindFirst("TenantId")?.Value;

        return identifier;
    }
}

