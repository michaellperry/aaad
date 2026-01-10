using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping act-related endpoints.
/// </summary>
public static class ActEndpoints
{
    /// <summary>
    /// Maps act management endpoints including list, get by GUID, create, update, and delete.
    /// All endpoints require authentication and respect tenant isolation.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapActEndpoints(this WebApplication app)
    {
        var acts = app.MapGroup("/api/acts")
            .RequireAuthorization();

        // GET /api/acts
        acts.MapGet("/", async (IActService actService, CancellationToken cancellationToken) =>
        {
            var allActs = await actService.GetAllAsync(cancellationToken);
            return Results.Ok(allActs);
        })
        .WithName("GetAllActs");

        // GET /api/acts/count
        acts.MapGet("/count", async (IActService actService, CancellationToken cancellationToken) =>
        {
            var count = await actService.GetCountAsync(cancellationToken);
            return Results.Ok(new { count });
        })
        .WithName("GetActsCount");

        // GET /api/acts/{guid}
        acts.MapGet("/{guid:guid}", async (Guid guid, IActService actService, CancellationToken cancellationToken) =>
        {
            var act = await actService.GetByGuidAsync(guid, cancellationToken);

            if (act == null)
            {
                return Results.NotFound(new { message = $"Act with GUID {guid} not found" });
            }

            return Results.Ok(act);
        })
        .WithName("GetActByGuid");

        // POST /api/acts
        acts.MapPost("/", async (CreateActDto dto, IActService actService, CancellationToken cancellationToken) =>
        {
            var act = await actService.CreateAsync(dto, cancellationToken);
            return Results.Created($"/api/acts/{act.ActGuid}", act);
        })
        .WithName("CreateAct");

        // PUT /api/acts/{guid}
        acts.MapPut("/{guid:guid}", async (Guid guid, UpdateActDto dto, IActService actService, CancellationToken cancellationToken) =>
        {
            // First, find the act by GUID to get its ID
            var existingAct = await actService.GetByGuidAsync(guid, cancellationToken);

            if (existingAct == null)
            {
                return Results.NotFound(new { message = $"Act with GUID {guid} not found" });
            }

            var updatedAct = await actService.UpdateAsync(existingAct.Id, dto, cancellationToken);

            if (updatedAct == null)
            {
                return Results.NotFound(new { message = $"Act with GUID {guid} not found" });
            }

            return Results.Ok(updatedAct);
        })
        .WithName("UpdateAct");

        // DELETE /api/acts/{guid}
        acts.MapDelete("/{guid:guid}", async (Guid guid, IActService actService, CancellationToken cancellationToken) =>
        {
            // First, find the act by GUID to get its ID
            var existingAct = await actService.GetByGuidAsync(guid, cancellationToken);

            if (existingAct == null)
            {
                return Results.NotFound(new { message = $"Act with GUID {guid} not found" });
            }

            var deleted = await actService.DeleteAsync(existingAct.Id, cancellationToken);

            if (!deleted)
            {
                return Results.NotFound(new { message = $"Act with GUID {guid} not found" });
            }

            return Results.NoContent();
        })
        .WithName("DeleteAct");

        return app;
    }
}
