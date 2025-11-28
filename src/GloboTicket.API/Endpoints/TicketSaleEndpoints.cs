using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;

namespace GloboTicket.API.Endpoints;

/// <summary>
/// Extension methods for mapping ticket sale-related endpoints.
/// </summary>
public static class TicketSaleEndpoints
{
    /// <summary>
    /// Maps ticket sale management endpoints including list, get by GUID, get by show, create, update, and delete.
    /// All endpoints require authentication and respect tenant isolation.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapTicketSaleEndpoints(this WebApplication app)
    {
        var ticketSales = app.MapGroup("/api/ticket-sales")
            .RequireAuthorization();

        // GET /api/ticket-sales
        ticketSales.MapGet("/", async (ITicketSaleService ticketSaleService, CancellationToken cancellationToken) =>
        {
            var allTicketSales = await ticketSaleService.GetAllAsync(cancellationToken);
            return Results.Ok(allTicketSales);
        })
        .WithName("GetAllTicketSales");

        // GET /api/ticket-sales/{guid}
        ticketSales.MapGet("/{guid:guid}", async (Guid guid, ITicketSaleService ticketSaleService, CancellationToken cancellationToken) =>
        {
            var ticketSale = await ticketSaleService.GetByGuidAsync(guid, cancellationToken);
            
            if (ticketSale == null)
            {
                return Results.NotFound(new { message = $"Ticket sale with GUID {guid} not found" });
            }

            return Results.Ok(ticketSale);
        })
        .WithName("GetTicketSaleByGuid");

        // GET /api/ticket-sales/by-show/{showGuid}
        ticketSales.MapGet("/by-show/{showGuid:guid}", async (Guid showGuid, ITicketSaleService ticketSaleService, CancellationToken cancellationToken) =>
        {
            var showTicketSales = await ticketSaleService.GetByShowGuidAsync(showGuid, cancellationToken);
            return Results.Ok(showTicketSales);
        })
        .WithName("GetTicketSalesByShow");

        // POST /api/ticket-sales
        ticketSales.MapPost("/", async (CreateTicketSaleDto dto, ITicketSaleService ticketSaleService, CancellationToken cancellationToken) =>
        {
            try
            {
                var ticketSale = await ticketSaleService.CreateAsync(dto, cancellationToken);
                return Results.Created($"/api/ticket-sales/{ticketSale.TicketSaleGuid}", ticketSale);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("CreateTicketSale");

        // PUT /api/ticket-sales/{guid}
        ticketSales.MapPut("/{guid:guid}", async (Guid guid, UpdateTicketSaleDto dto, ITicketSaleService ticketSaleService, CancellationToken cancellationToken) =>
        {
            // First, find the ticket sale by GUID to get its ID
            var existingTicketSale = await ticketSaleService.GetByGuidAsync(guid, cancellationToken);
            
            if (existingTicketSale == null)
            {
                return Results.NotFound(new { message = $"Ticket sale with GUID {guid} not found" });
            }

            var updatedTicketSale = await ticketSaleService.UpdateAsync(existingTicketSale.Id, dto, cancellationToken);
            
            if (updatedTicketSale == null)
            {
                return Results.NotFound(new { message = $"Ticket sale with GUID {guid} not found" });
            }

            return Results.Ok(updatedTicketSale);
        })
        .WithName("UpdateTicketSale");

        // DELETE /api/ticket-sales/{guid}
        ticketSales.MapDelete("/{guid:guid}", async (Guid guid, ITicketSaleService ticketSaleService, CancellationToken cancellationToken) =>
        {
            // First, find the ticket sale by GUID to get its ID
            var existingTicketSale = await ticketSaleService.GetByGuidAsync(guid, cancellationToken);
            
            if (existingTicketSale == null)
            {
                return Results.NotFound(new { message = $"Ticket sale with GUID {guid} not found" });
            }

            var deleted = await ticketSaleService.DeleteAsync(existingTicketSale.Id, cancellationToken);
            
            if (!deleted)
            {
                return Results.NotFound(new { message = $"Ticket sale with GUID {guid} not found" });
            }

            return Results.NoContent();
        })
        .WithName("DeleteTicketSale");

        return app;
    }
}