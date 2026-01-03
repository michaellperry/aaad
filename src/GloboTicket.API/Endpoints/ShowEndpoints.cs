using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping show-related endpoints.
/// </summary>
public static class ShowEndpoints
{
    /// <summary>
    /// Maps show management endpoints including list by act, get by GUID, create, delete, and nearby shows query.
    /// All endpoints require authentication and respect tenant isolation through Venue relationship.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapShowEndpoints(this WebApplication app)
    {
        var shows = app.MapGroup("/api")
            .RequireAuthorization();

        // GET /api/acts/{actGuid}/shows
        shows.MapGet("/acts/{actGuid:guid}/shows", async (Guid actGuid, IShowService showService, CancellationToken cancellationToken) =>
        {
            try
            {
                var showsList = await showService.GetShowsByActGuidAsync(actGuid, cancellationToken);
                return Results.Ok(showsList);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Act with GUID {actGuid} not found" });
            }
        })
        .WithName("GetShowsByAct");

        // POST /api/acts/{actGuid}/shows
        shows.MapPost("/acts/{actGuid:guid}/shows", async (Guid actGuid, CreateShowDto dto, IShowService showService, CancellationToken cancellationToken) =>
        {
            try
            {
                var show = await showService.CreateAsync(actGuid, dto, cancellationToken);
                return Results.Created($"/api/shows/{show.ShowGuid}", show);
            }
            catch (ArgumentException ex)
            {
                // Return validation error in problem details format
                var errors = new Dictionary<string, string[]>();
                
                // Parse the parameter name from the exception to determine which field failed
                if (ex.ParamName == nameof(dto.TicketCount))
                {
                    errors["TicketCount"] = new[] { ex.Message };
                }
                else if (ex.ParamName == nameof(dto.StartTime))
                {
                    errors["StartTime"] = new[] { ex.Message };
                }
                else
                {
                    errors["General"] = new[] { ex.Message };
                }
                
                return Results.BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors
                });
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = "Act or Venue not found" });
            }
        })
        .WithName("CreateShow");

        // GET /api/shows/{showGuid}
        shows.MapGet("/shows/{showGuid:guid}", async (Guid showGuid, IShowService showService, CancellationToken cancellationToken) =>
        {
            var show = await showService.GetByGuidAsync(showGuid, cancellationToken);
            return show == null ? Results.NotFound(new { message = $"Show with GUID {showGuid} not found" }) : Results.Ok(show);
        })
        .WithName("GetShowByGuid");

        // DELETE /api/shows/{showGuid}
        shows.MapDelete("/shows/{showGuid:guid}", async (Guid showGuid, IShowService showService, CancellationToken cancellationToken) =>
        {
            var deleted = await showService.DeleteAsync(showGuid, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound(new { message = $"Show with GUID {showGuid} not found" });
        })
        .WithName("DeleteShow");

        // GET /api/venues/{venueGuid}/shows/nearby?startTime={iso8601}
        shows.MapGet("/venues/{venueGuid:guid}/shows/nearby", async (Guid venueGuid, string startTime, IShowService showService, CancellationToken cancellationToken) =>
        {
            if (!DateTimeOffset.TryParse(startTime, out var parsedStartTime))
            {
                return Results.BadRequest(new { message = "Invalid startTime format. Expected ISO 8601 format with timezone offset." });
            }

            try
            {
                var nearbyShows = await showService.GetNearbyShowsAsync(venueGuid, parsedStartTime, cancellationToken);
                return Results.Ok(nearbyShows);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Venue with GUID {venueGuid} not found" });
            }
        })
        .WithName("GetNearbyShows");

        return app;
    }
}
