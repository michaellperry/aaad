using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping tenant-related endpoints.
/// </summary>
public static class TenantEndpoints
{
    /// <summary>
    /// Maps tenant management endpoints including list, get by ID, and create.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapTenantEndpoints(this WebApplication app)
    {
        var tenants = app.MapGroup("/api/tenants")
            .RequireAuthorization();

        // GET /api/tenants
        tenants.MapGet("/", async (ITenantService tenantService, CancellationToken cancellationToken) =>
        {
            var allTenants = await tenantService.GetAllTenantsAsync(cancellationToken);
            return Results.Ok(allTenants);
        })
        .WithName("GetAllTenants")
        .WithOpenApi();

        // GET /api/tenants/{id}
        tenants.MapGet("/{id:int}", async (int id, ITenantService tenantService, CancellationToken cancellationToken) =>
        {
            var tenant = await tenantService.GetTenantByIdAsync(id, cancellationToken);
            
            if (tenant == null)
            {
                return Results.NotFound(new { message = $"Tenant with ID {id} not found" });
            }

            return Results.Ok(tenant);
        })
        .WithName("GetTenantById")
        .WithOpenApi();

        // POST /api/tenants
        tenants.MapPost("/", async (CreateTenantDto dto, ITenantService tenantService, CancellationToken cancellationToken) =>
        {
            var tenant = await tenantService.CreateTenantAsync(dto, cancellationToken);
            return Results.Created($"/api/tenants/{tenant.Id}", tenant);
        })
        .WithName("CreateTenant")
        .WithOpenApi();

        return app;
    }
}