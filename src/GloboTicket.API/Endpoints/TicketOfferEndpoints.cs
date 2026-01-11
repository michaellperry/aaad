using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping ticket offer-related endpoints.
/// </summary>
public static class TicketOfferEndpoints
{
    /// <summary>
    /// Maps ticket offer management endpoints including create, list, and capacity query.
    /// All endpoints require authentication and respect tenant isolation through Show → Venue relationship.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapTicketOfferEndpoints(this WebApplication app)
    {
        var ticketOffers = app.MapGroup("/api/shows")
            .RequireAuthorization();

        // POST /api/shows/{showGuid}/ticket-offers
        ticketOffers.MapPost("/{showGuid:guid}/ticket-offers", async (
            Guid showGuid,
            CreateTicketOfferDto dto,
            ITicketOfferService ticketOfferService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var ticketOffer = await ticketOfferService.CreateTicketOfferAsync(showGuid, dto, cancellationToken);
                return Results.Created($"/api/ticket-offers/{ticketOffer.TicketOfferGuid}", ticketOffer);
            }
            catch (ArgumentException ex)
            {
                // Return validation error in problem details format
                var errors = new Dictionary<string, string[]>();

                // Parse the parameter name from the exception to determine which field failed
                if (ex.ParamName == "TicketCount" || ex.Message.Contains("capacity"))
                {
                    errors["TicketCount"] = new[] { ex.Message };
                }
                else if (ex.ParamName == "Name")
                {
                    errors["Name"] = new[] { ex.Message };
                }
                else if (ex.ParamName == "Price")
                {
                    errors["Price"] = new[] { ex.Message };
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
                return Results.NotFound(new { message = $"Show with GUID {showGuid} not found" });
            }
        })
        .WithName("CreateTicketOffer");

        // GET /api/shows/{showGuid}/ticket-offers
        ticketOffers.MapGet("/{showGuid:guid}/ticket-offers", async (
            Guid showGuid,
            ITicketOfferService ticketOfferService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var offers = await ticketOfferService.GetTicketOffersByShowAsync(showGuid, cancellationToken);
                return Results.Ok(offers);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Show with GUID {showGuid} not found" });
            }
        })
        .WithName("GetTicketOffersByShow");

        // GET /api/shows/{showGuid}/capacity
        ticketOffers.MapGet("/{showGuid:guid}/capacity", async (
            Guid showGuid,
            ITicketOfferService ticketOfferService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var capacity = await ticketOfferService.GetShowCapacityAsync(showGuid, cancellationToken);
                return Results.Ok(capacity);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { message = $"Show with GUID {showGuid} not found" });
            }
        })
        .WithName("GetShowCapacity");

        return app;
    }
}
