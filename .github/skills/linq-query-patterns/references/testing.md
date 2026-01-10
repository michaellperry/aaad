# Testing Patterns

Unit testing strategies for LINQ specifications and repository methods.

## Testing LINQ Specifications

### Unit Testing Query Logic
Test query specifications separately from database execution:

```csharp
[Test]
public void ActiveVenuesSpec_FiltersCorrectly()
{
    // Arrange - Create test data
    var venues = new List<Venue>
    {
        new Venue { Id = Guid.NewGuid(), TenantId = _tenantId, IsActive = true, Name = "Active Venue" },
        new Venue { Id = Guid.NewGuid(), TenantId = _tenantId, IsActive = false, Name = "Inactive Venue" },
        new Venue { Id = Guid.NewGuid(), TenantId = Guid.NewGuid(), IsActive = true, Name = "Other Tenant" }
    };

    // Act - Apply specification to in-memory collection
    var activeVenuesSpec =
        from venue in venues.AsQueryable()
        where venue.TenantId == _tenantId && venue.IsActive
        select new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            IsActive = venue.IsActive
        };

    var result = activeVenuesSpec.ToList();

    // Assert
    Assert.That(result.Count, Is.EqualTo(1));
    Assert.That(result[0].Name, Is.EqualTo("Active Venue"));
}
```

### Testing with Mock DbContext
Use a test database or in-memory provider:

```csharp
[Test]
public async Task GetVenueAsync_ReturnsCorrectVenue()
{
    // Arrange
    await using var context = CreateTestContext();
    
    var venue = new Venue 
    { 
        Id = Guid.NewGuid(), 
        TenantId = _tenantId, 
        Name = "Test Venue",
        IsActive = true 
    };
    
    context.Venues.Add(venue);
    await context.SaveChangesAsync();

    var repository = new VenueRepository(context);

    // Act
    var result = await repository.GetVenueAsync(venue.Id, CancellationToken.None);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Id, Is.EqualTo(venue.Id));
    Assert.That(result.Name, Is.EqualTo("Test Venue"));
}

private TestDbContext CreateTestContext()
{
    var options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    
    return new TestDbContext(options);
}
```

## Testing Repository Methods

### Testing Read Operations
```csharp
public class VenueRepositoryTests
{
    private TestDbContext _context;
    private VenueRepository _repository;
    private readonly Guid _tenantId = Guid.NewGuid();

    [SetUp]
    public void Setup()
    {
        _context = CreateTestContext();
        _repository = new VenueRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task SearchVenuesAsync_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        var venues = new[]
        {
            CreateVenue("Venue A", "City1", isActive: true),
            CreateVenue("Venue B", "City2", isActive: true),
            CreateVenue("Venue C", "City1", isActive: false)
        };

        _context.Venues.AddRange(venues);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.SearchVenuesAsync(
            _tenantId, 
            searchTerm: "Venue", 
            cityFilter: "City1", 
            activeOnly: true);

        // Assert
        Assert.That(results.Count(), Is.EqualTo(1));
        Assert.That(results.First().Name, Is.EqualTo("Venue A"));
    }

    private Venue CreateVenue(string name, string city, bool isActive)
    {
        return new Venue
        {
            Id = Guid.NewGuid(),
            TenantId = _tenantId,
            Name = name,
            Address = new Address { City = city },
            IsActive = isActive,
            Capacity = 100
        };
    }
}
```

### Testing Modification Operations
```csharp
[Test]
public async Task UpdateVenueAsync_ValidData_UpdatesSuccessfully()
{
    // Arrange
    var venue = CreateVenue("Original Name", "Original City", true);
    _context.Venues.Add(venue);
    await _context.SaveChangesAsync();

    var updateDto = new UpdateVenueDto
    {
        Name = "Updated Name",
        Capacity = 200
    };

    // Act
    await _repository.UpdateVenueAsync(venue.Id, updateDto, CancellationToken.None);

    // Assert
    var updatedVenue = await _context.Venues.FindAsync(venue.Id);
    Assert.That(updatedVenue.Name, Is.EqualTo("Updated Name"));
    Assert.That(updatedVenue.Capacity, Is.EqualTo(200));
}

[Test]
public async Task DeleteVenueAsync_VenueExists_ReturnsTrue()
{
    // Arrange
    var venue = CreateVenue("Test Venue", "Test City", true);
    _context.Venues.Add(venue);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.DeleteVenueAsync(venue.Id, CancellationToken.None);

    // Assert
    Assert.That(result, Is.True);
    
    var deletedVenue = await _context.Venues.FindAsync(venue.Id);
    Assert.That(deletedVenue.IsActive, Is.False); // Soft delete
}
```

## Testing Complex Scenarios

### Testing Business Rules
```csharp
[Test]
public async Task DeactivateVenueAsync_WithFutureEvents_ReturnsFalse()
{
    // Arrange
    var venue = CreateVenue("Test Venue", "Test City", true);
    var futureAct = new Act 
    { 
        Id = Guid.NewGuid(),
        VenueId = venue.Id,
        EventDate = DateTime.UtcNow.AddDays(30),
        IsActive = true
    };
    
    venue.Acts.Add(futureAct);
    _context.Venues.Add(venue);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.DeactivateVenueAsync(venue.Id, CancellationToken.None);

    // Assert
    Assert.That(result, Is.False);
    
    var unchangedVenue = await _context.Venues.FindAsync(venue.Id);
    Assert.That(unchangedVenue.IsActive, Is.True);
}
```

### Integration Test Example
```csharp
[Test]
public async Task GetVenueStatsAsync_ComplexCalculation_ReturnsCorrectStats()
{
    // Arrange - Create complex test data
    var venue = CreateVenue("Test Venue", "Test City", true);
    
    var acts = new[]
    {
        CreateAct(venue.Id, DateTime.UtcNow.AddDays(-10), isActive: true),
        CreateAct(venue.Id, DateTime.UtcNow.AddDays(10), isActive: true),
        CreateAct(venue.Id, DateTime.UtcNow.AddDays(20), isActive: false)
    };

    _context.Venues.Add(venue);
    _context.Acts.AddRange(acts);
    await _context.SaveChangesAsync();

    // Act
    var stats = await _repository.GetVenueStatsAsync(venue.Id, CancellationToken.None);

    // Assert
    Assert.That(stats, Is.Not.Null);
    Assert.That(stats.TotalActiveActs, Is.EqualTo(2));
    Assert.That(stats.UpcomingActsCount, Is.EqualTo(1));
    Assert.That(stats.PastActsCount, Is.EqualTo(1));
}
```

## Performance Testing

### Testing Query Performance
```csharp
[Test]
[Explicit("Performance test")]
public async Task SearchVenues_LargeDataSet_PerformsWithinAcceptableTime()
{
    // Arrange - Create large dataset
    var venues = Enumerable.Range(1, 10000)
        .Select(i => CreateVenue($"Venue {i}", $"City {i % 100}", i % 2 == 0))
        .ToArray();
    
    _context.Venues.AddRange(venues);
    await _context.SaveChangesAsync();

    var stopwatch = Stopwatch.StartNew();

    // Act
    var results = await _repository.SearchVenuesAsync(_tenantId, activeOnly: true);

    // Assert
    stopwatch.Stop();
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), 
        "Query should complete within 1 second");
    Assert.That(results.Count(), Is.EqualTo(5000)); // Half should be active
}
```