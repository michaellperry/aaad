# Service Layer Integration

Inject application services into endpoints; do not inject repositories directly.

```csharp
venues.MapPost("/", async (
    CreateVenueDto dto,
    IVenueService service,
    IValidator<CreateVenueDto> validator) =>
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var result = await service.CreateAsync(dto);
    return Results.Created($"/api/venues/{result.Id}", result);
});
```

Minimal service implementation (domain orchestration + tenant guard):

```csharp
public class VenueService
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    public VenueService(DbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    public async Task<VenueDto> CreateAsync(CreateVenueDto dto, CancellationToken cancellationToken)
    {
        if (!_tenantContext.CurrentTenantId.HasValue)
        {
            throw new InvalidOperationException("Tenant context is required");
        }

        var venue = new Venue { Name = dto.Name, Address = dto.Address, SeatingCapacity = dto.Capacity };
        _dbContext.Set<Venue>().Add(venue);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new VenueDto(venue.Id, venue.Name, venue.Address, venue.SeatingCapacity, true, DateTime.UtcNow);
    }
}
```
