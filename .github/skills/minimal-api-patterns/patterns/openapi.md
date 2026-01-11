# OpenAPI Integration

Annotate endpoints with names, summaries, and response contracts for Swagger.

```csharp
venues.MapGet("/", GetAllVenues)
    .WithName("GetVenues")
    .WithSummary("Get all venues")
    .WithDescription("Retrieves a paginated list of venues for the authenticated tenant")
    .Produces<PagedResult<VenueListDto>>(200)
    .Produces<ProblemDetails>(400)
    .Produces(401)
    .WithTags("Venues");

venues.MapPost("/", CreateVenue)
    .WithName("CreateVenue")
    .WithSummary("Create a new venue")
    .WithDescription("Creates a new venue with the provided details")
    .Accepts<CreateVenueDto>("application/json")
    .Produces<VenueDto>(201)
    .ProducesValidationProblem()
    .Produces<ProblemDetails>(409)
    .WithTags("Venues");
```
