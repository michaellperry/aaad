using GloboTicket.API.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        auth.MapPost("/login", async (LoginRequest request, IConfiguration configuration, HttpContext httpContext) =>
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

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("TenantId", user.TenantId.ToString())
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
                tenantId = user.TenantId,
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
            
            return Results.Ok(new
            {
                username,
                tenantId = tenantIdClaim != null && int.TryParse(tenantIdClaim, out var tid) ? tid : (int?)null,
                isAuthenticated = true
            });
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser");

        return app;
    }
}

/// <summary>
/// Request model for user login.
/// </summary>
public record LoginRequest(string Username, string Password);