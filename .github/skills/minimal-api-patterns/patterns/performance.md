# Performance and Async Patterns

Add caching for read endpoints and prefer async with cancellation.

```csharp
venues.MapGet("/types", GetVenueTypes)
    .CacheOutput(policy => policy.Expire(TimeSpan.FromHours(1)).Tag("venue-types"));

venues.MapGet("/{id:guid}", GetVenueById)
    .CacheOutput(policy => policy.Expire(TimeSpan.FromMinutes(15)).SetVaryByRouteValue("id"));
```

```csharp
static async Task<IResult> GetVenue(Guid id, IVenueService service)
{
    var venue = await service.GetByIdAsync(id);
    return venue != null ? Results.Ok(venue) : Results.NotFound();
}

static async Task<IResult> GetAllVenues(IVenueService service, CancellationToken cancellationToken)
{
    var venues = await service.GetAllAsync(cancellationToken);
    return Results.Ok(venues);
}
```
