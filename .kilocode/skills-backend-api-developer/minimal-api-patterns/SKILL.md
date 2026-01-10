---
name: minimal-api-patterns
description: Best practices for defining Endpoints, DTOs, and Validators using ASP.NET Core Minimal APIs.
---

# Minimal API Patterns

## Role Responsibilities
As an API Contract Guardian, you are responsible for:
- **Contracts**: Defining clear Request/Response DTOs in `GloboTicket.Application/DTOs`.
- **Endpoints**: Implementing `MapGroup` definitions in `GloboTicket.API/Endpoints/`.
- **Services**: Implementing Application Services that orchestrate Domain Entities.
- **Validation**: Ensuring all inputs are validated before processing.

## Endpoint Structure

### Grouping
- Use `MapGroup` to organize related endpoints.
- Apply common middleware (Auth, Versioning) at the group level.

```csharp
public static WebApplication MapVenueEndpoints(this WebApplication app)
{
    var venues = app.MapGroup("/api/venues")
        .RequireAuthorization()
        .WithTags("Venues");

    venues.MapGet("/", GetAllVenues);
    return app;
}
```

## DTO Pattern

### Separation of Concerns
- **NEVER** return Domain Entities directly from an endpoint.
- Always map to a specific DTO.
- Use `record` types for DTOs for immutability.

```csharp
// Good
public record VenueDto(Guid Id, string Name, string Location);

// Bad
public class Venue : Entity { ... } // Do not return this!
```

## Service Layer Integration

### Injection
- Inject Application Services (`IVenueService`) into endpoint delegates.
- Keep endpoint logic thin; delegate business logic to the Service.

```csharp
venues.MapPost("/", async (CreateVenueDto dto, IVenueService service) => 
{
    var result = await service.CreateAsync(dto);
    return Results.Created($"/api/venues/{result.Id}", result);
});
```

