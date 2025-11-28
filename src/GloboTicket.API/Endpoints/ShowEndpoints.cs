using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping show-related endpoints.
/// </summary>
public static class ShowEndpoints
{
    /// <summary>
    /// Maps show management endpoints including list, get by GUID, create, update, and delete.
    /// All endpoints require authentication and respect tenant isolation.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapShowEndpoints(this WebApplication app)
    {
        var shows = app.MapGroup("/api/shows")
            .RequireAuthorization();

        // GET /api/shows
        shows.MapGet("/", async (IShowService showService, CancellationToken cancellationToken) =>
        {
            var allShows = await showService.GetAllAsync(cancellationToken);
            return Results.Ok(allShows);
        })
        .WithName("GetAllShows");

        // GET /api/shows/{guid}
        shows.MapGet("/{guid:guid}", async (Guid guid, IShowService showService, CancellationToken cancellationToken) =>
        {
            var show = await showService.GetByGuidAsync(guid, cancellationToken);
            
            if (show == null)
            {
                return Results.NotFound(new { message = $"Show with GUID {guid} not found" });
            }

            return Results.Ok(show);
        })
        .WithName("GetShowByGuid");

        // POST /api/shows
        shows.MapPost("/", async (CreateShowDto dto, IShowService showService, CancellationToken cancellationToken) =>
        {
            try
            {
                var show = await showService.CreateAsync(dto, cancellationToken);
                return Results.Created($"/api/shows/{show.ShowGuid}", show);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("CreateShow");

        // PUT /api/shows/{guid}
        shows.MapPut("/{guid:guid}", async (Guid guid, UpdateShowDto dto, IShowService showService, CancellationToken cancellationToken) =>
        {
            // First, find the show by GUID to get its ID
            var existingShow = await showService.GetByGuidAsync(guid, cancellationToken);
            
            if (existingShow == null)
            {
                return Results.NotFound(new { message = $"Show with GUID {guid} not found" });
            }

            var updatedShow = await showService.UpdateAsync(existingShow.Id, dto, cancellationToken);
            
            if (updatedShow == null)
            {
                return Results.NotFound(new { message = $"Show with GUID {guid} not found" });
            }

            return Results.Ok(updatedShow);
        })
        .WithName("UpdateShow");

        // DELETE /api/shows/{guid}
        shows.MapDelete("/{guid:guid}", async (Guid guid, IShowService showService, CancellationToken cancellationToken) =>
        {
            // First, find the show by GUID to get its ID
            var existingShow = await showService.GetByGuidAsync(guid, cancellationToken);
            
            if (existingShow == null)
            {
                return Results.NotFound(new { message = $"Show with GUID {guid} not found" });
            }

            var deleted = await showService.DeleteAsync(existingShow.Id, cancellationToken);
            
            if (!deleted)
            {
                return Results.NotFound(new { message = $"Show with GUID {guid} not found" });
            }

            return Results.NoContent();
        })
        .WithName("DeleteShow");

        return app;
    }
}