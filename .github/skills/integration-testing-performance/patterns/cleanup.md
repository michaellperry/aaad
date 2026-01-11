# Cleanup Strategies

Ensure test data is removed when using shared containers. Prefer automatic disposal; fall back to explicit cleanup.

```csharp
public class TestDataScope : IDisposable
{
    private readonly List<(Type EntityType, Guid Id, Guid TenantId)> _created = new();
    private readonly GloboTicketDbContext _context;

    public TestDataScope(GloboTicketDbContext context)
    {
        _context = context;
    }

    public T Track<T>(T entity) where T : class, ITenantEntity
    {
        _created.Add((typeof(T), entity.Id, entity.TenantId));
        return entity;
    }

    public void Dispose()
    {
        foreach (var (entityType, id, _) in _created.AsEnumerable().Reverse())
        {
            var entity = _context.Find(entityType, id);
            if (entity != null)
            {
                _context.Remove(entity);
            }
        }
        _context.SaveChanges();
    }
}
```

```csharp
[Test]
public async Task TestWithManualCleanup()
{
    var tenantId = _fixture.GenerateRandomTenantId();
    var testVenue = await CreateTestDataAsync(tenantId);

    try
    {
        var result = await _service.ProcessVenueAsync(testVenue.Id);
        Assert.That(result.Success, Is.True);
    }
    finally
    {
        await CleanupTestDataAsync(testVenue.Id, tenantId);
    }
}

private async Task CleanupTestDataAsync(Guid venueId, Guid tenantId)
{
    using var scope = _fixture.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GloboTicketDbContext>();

    var shows = await context.Shows.Where(s => s.Venue.Id == venueId && s.TenantId == tenantId).ToListAsync();
    context.Shows.RemoveRange(shows);

    var venue = await context.Venues.FindAsync(venueId);
    if (venue != null)
    {
        context.Venues.Remove(venue);
    }

    await context.SaveChangesAsync();
}
```