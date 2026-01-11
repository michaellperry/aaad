# Authentication and Authorization

Apply auth at group level; add policies/roles per endpoint as needed.

```csharp
var venues = app.MapGroup("/api/venues")
    .RequireAuthorization()
    .WithTags("Venues");

venues.MapDelete("/{id:guid}", DeleteVenue)
    .RequireAuthorization("AdminOnly");

venues.MapPost("/", CreateVenue)
    .RequireAuthorization(policy => policy.RequireRole("Manager", "Admin"));

venues.MapPut("/{id:guid}/approve", ApproveVenue)
    .RequireAuthorization("CanApproveVenues");
```
