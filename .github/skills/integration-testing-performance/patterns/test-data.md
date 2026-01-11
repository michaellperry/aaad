# Test Data Management

Use bulk creation for large datasets and shared fixtures only when necessary.

```csharp
public async Task<List<Venue>> CreateVenuesInBulkAsync(int count, Guid tenantId)
{
    var venues = new List<Venue>();
    for (int i = 0; i < count; i++)
    {
        venues.Add(new Venue($"Test Venue {i}", address, 1000 + i, venueType, tenantId));
    }

    _context.Venues.AddRange(venues);
    await _context.SaveChangesAsync();
    return venues;
}
```

```csharp
public class SharedTestFixture : IDisposable
{
    public GloboTicketDbContext Context { get; }
    public Guid SharedTenantId { get; }

    public SharedTestFixture()
    {
        SharedTenantId = Guid.NewGuid();
        Context = CreateDatabaseContext();
        SeedReferenceDataAsync().Wait();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
```