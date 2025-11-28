using GloboTicket.API.Configuration;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping authentication-related endpoints.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps authentication endpoints including login, logout, and current user info.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth");

        // POST /auth/login
        auth.MapPost("/login", async (LoginRequest request, IConfiguration configuration, HttpContext httpContext, GloboTicketDbContext dbContext) =>
        {
            // Get users from configuration
            var users = configuration.GetSection("Users").Get<List<UserConfiguration>>();
            
            if (users == null || users.Count == 0)
            {
                return Results.Problem("No users configured in the system");
            }

            // Validate credentials
            var user = users.FirstOrDefault(u =>
                u.Username == request.Username && u.Password == request.Password);

            if (user == null)
            {
                return Results.Unauthorized();
            }

            // Look up or create tenant by identifier
            var tenant = await dbContext.Tenants
                .FirstOrDefaultAsync(t => t.TenantIdentifier == user.TenantIdentifier);

            if (tenant == null)
            {
                // Auto-create tenant if it doesn't exist
                tenant = new Tenant
                {
                    TenantIdentifier = user.TenantIdentifier,
                    Name = GetTenantNameFromIdentifier(user.TenantIdentifier),
                    Slug = GetTenantSlugFromIdentifier(user.TenantIdentifier),
                    IsActive = true
                };
                dbContext.Tenants.Add(tenant);
                await dbContext.SaveChangesAsync();
            }

            var tenantDbId = tenant.Id;

            // Create claims with both database ID and natural key
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("TenantId", tenantDbId.ToString()),
                new Claim("TenantIdentifier", user.TenantIdentifier)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Results.Ok(new
            {
                username = user.Username,
                tenantId = tenantDbId,
                tenantIdentifier = user.TenantIdentifier,
                message = "Login successful"
            });
        })
        .WithName("Login");

        // POST /auth/logout
        auth.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok(new { message = "Logout successful" });
        })
        .WithName("Logout");

        // GET /auth/me
        auth.MapGet("/me", (HttpContext httpContext) =>
        {
            var user = httpContext.User;
            
            if (user?.Identity?.IsAuthenticated != true)
            {
                return Results.Unauthorized();
            }

            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            var tenantIdClaim = user.FindFirst("TenantId")?.Value;
            var tenantIdentifier = user.FindFirst("TenantIdentifier")?.Value;
            
            return Results.Ok(new
            {
                username,
                tenantId = tenantIdClaim != null && int.TryParse(tenantIdClaim, out var tid) ? tid : (int?)null,
                tenantIdentifier,
                isAuthenticated = true
            });
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser");

        return app;
    }

    /// <summary>
    /// Helper method to get tenant name from identifier.
    /// </summary>
    private static string GetTenantNameFromIdentifier(string identifier)
    {
        return identifier switch
        {
            "production" => "Production",
            "smoke-test" => "Smoke Test",
            _ => $"Tenant {identifier}"
        };
    }

    /// <summary>
    /// Helper method to get tenant slug from identifier.
    /// </summary>
    private static string GetTenantSlugFromIdentifier(string identifier)
    {
        return identifier switch
        {
            "production" => "production",
            "smoke-test" => "smoke-test",
            _ => identifier.ToLowerInvariant().Replace("_", "-")
        };
    }
}

/// <summary>
/// Request model for user login.
/// </summary>
public record LoginRequest(string Username, string Password);