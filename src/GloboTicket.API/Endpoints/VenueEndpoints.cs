using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping venue-related endpoints.
/// </summary>
public static class VenueEndpoints
{
    /// <summary>
    /// Maps venue management endpoints including list, get by GUID, create, update, and delete.
    /// All endpoints require authentication and respect tenant isolation.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapVenueEndpoints(this WebApplication app)
    {
        var venues = app.MapGroup("/api/venues")
            .RequireAuthorization();

        // GET /api/venues
        venues.MapGet("/", async (IVenueService venueService, CancellationToken cancellationToken) =>
        {
            var allVenues = await venueService.GetAllAsync(cancellationToken);
            return Results.Ok(allVenues);
        })
        .WithName("GetAllVenues");

        // GET /api/venues/{guid}
        venues.MapGet("/{guid:guid}", async (Guid guid, IVenueService venueService, CancellationToken cancellationToken) =>
        {
            var venue = await venueService.GetByGuidAsync(guid, cancellationToken);
            
            if (venue == null)
            {
                return Results.NotFound(new { message = $"Venue with GUID {guid} not found" });
            }

            return Results.Ok(venue);
        })
        .WithName("GetVenueByGuid");

        // POST /api/venues
        venues.MapPost("/", async (CreateVenueDto dto, IVenueService venueService, CancellationToken cancellationToken) =>
        {
            var venue = await venueService.CreateAsync(dto, cancellationToken);
            return Results.Created($"/api/venues/{venue.VenueGuid}", venue);
        })
        .WithName("CreateVenue");

        // PUT /api/venues/{guid}
        venues.MapPut("/{guid:guid}", async (Guid guid, UpdateVenueDto dto, IVenueService venueService, CancellationToken cancellationToken) =>
        {
            // First, find the venue by GUID to get its ID
            var existingVenue = await venueService.GetByGuidAsync(guid, cancellationToken);
            
            if (existingVenue == null)
            {
                return Results.NotFound(new { message = $"Venue with GUID {guid} not found" });
            }

            var updatedVenue = await venueService.UpdateAsync(existingVenue.Id, dto, cancellationToken);
            
            if (updatedVenue == null)
            {
                return Results.NotFound(new { message = $"Venue with GUID {guid} not found" });
            }

            return Results.Ok(updatedVenue);
        })
        .WithName("UpdateVenue");

        // DELETE /api/venues/{guid}
        venues.MapDelete("/{guid:guid}", async (Guid guid, IVenueService venueService, CancellationToken cancellationToken) =>
        {
            // First, find the venue by GUID to get its ID
            var existingVenue = await venueService.GetByGuidAsync(guid, cancellationToken);
            
            if (existingVenue == null)
            {
                return Results.NotFound(new { message = $"Venue with GUID {guid} not found" });
            }

            var deleted = await venueService.DeleteAsync(existingVenue.Id, cancellationToken);
            
            if (!deleted)
            {
                return Results.NotFound(new { message = $"Venue with GUID {guid} not found" });
            }

            return Results.NoContent();
        })
        .WithName("DeleteVenue");

        return app;
    }
}