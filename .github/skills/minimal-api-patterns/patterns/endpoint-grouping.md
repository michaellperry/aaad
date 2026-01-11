# Endpoint Grouping

Use `MapGroup` to organize related endpoints, apply shared middleware, and tag for OpenAPI.

```csharp
public static WebApplication MapVenueEndpoints(this WebApplication app)
{
    var venues = app.MapGroup("/api/venues")
        .RequireAuthorization()
        .WithTags("Venues")
        .WithOpenApi();

    venues.MapGet("/", GetAllVenues).WithName("GetVenues");
    venues.MapGet("/{id:guid}", GetVenueById).WithName("GetVenueById");
    venues.MapPost("/", CreateVenue).WithName("CreateVenue");
    venues.MapPut("/{id:guid}", UpdateVenue).WithName("UpdateVenue");
    venues.MapDelete("/{id:guid}", DeleteVenue).WithName("DeleteVenue");

    return app;
}
```
