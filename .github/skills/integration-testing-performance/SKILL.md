---
name: integration-testing-performance
description: Integration testing performance optimization, test parallelization, cleanup strategies, and CI/CD categorization patterns. Use when optimizing test execution speed, managing test data, or structuring tests for automated pipelines.
---

# Integration Testing Performance and Maintainability

Performance optimization and maintainability patterns for integration testing with Entity Framework, Testcontainers, and CI/CD pipelines.

## Test Parallelization

**Run tests in parallel for faster feedback. Integration tests can safely run in parallel when using isolated tenant IDs and fresh database contexts.**

```csharp
// In test project file (.csproj)
<PropertyGroup>
    <ParallelizeTestExecution>true</ParallelizeTestExecution>
    <MaxCpuCount>0</MaxCpuCount>
</PropertyGroup>

// In test class
[Parallelizable(ParallelScope.All)]
public class VenueServiceIntegrationTests
{
    // Tests will run in parallel
    // Each test uses unique tenant ID via _fixture.GenerateRandomTenantId()
    
    [Test]
    public async Task CreateVenue_ValidData_CreatesVenueSuccessfully()
    {
        // Arrange
        var tenantId = _fixture.GenerateRandomTenantId(); // Unique per test
        using var scope = _fixture.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<VenueService>();
        
        // Act & Assert - Safe to run in parallel
    }
}
```

### Parallel Testing Best Practices

```csharp
// ✅ Good - Isolated test data
[Test]
public async Task ParallelSafeTest()
{
    var tenantId = _fixture.GenerateRandomTenantId(); // Always unique
    var venueId = Guid.NewGuid(); // Unique identifiers
    
    // Each test operates on completely separate data
}

// ❌ Bad - Shared test data
[Test]
public async Task SharedDataTest()
{
    var venue = await GetSharedTestVenue(); // Causes test interference
    // Multiple tests modifying same data causes failures
}
```

## Test Cleanup Strategies

**Ensure proper cleanup between tests to prevent data pollution in shared database containers.**

### Automatic Cleanup with Disposable Pattern

```csharp
public class TestDataScope : IDisposable
{
    private readonly List<(Type EntityType, Guid Id, Guid TenantId)> _createdEntities = new();
    private readonly GloboTicketDbContext _context;
    
    public TestDataScope(GloboTicketDbContext context)
    {
        _context = context;
    }
    
    public T Track<T>(T entity) where T : class, ITenantEntity
    {
        _createdEntities.Add((typeof(T), entity.Id, entity.TenantId));
        return entity;
    }
    
    public void Dispose()
    {
        // Cleanup in reverse order of creation
        foreach (var (entityType, id, tenantId) in _createdEntities.AsEnumerable().Reverse())
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

### Manual Cleanup Pattern

```csharp
[Test]
public async Task TestWithManualCleanup()
{
    // Arrange
    var tenantId = _fixture.GenerateRandomTenantId();
    var testVenue = await CreateTestDataAsync(tenantId);
    
    try
    {
        // Act
        var result = await _service.ProcessVenueAsync(testVenue.Id);
        
        // Assert
        Assert.That(result.Success, Is.True);
    }
    finally
    {
        // Cleanup - ensure test data is removed from shared container
        await CleanupTestDataAsync(testVenue.Id, tenantId);
    }
}

private async Task CleanupTestDataAsync(Guid venueId, Guid tenantId)
{
    using var scope = _fixture.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GloboTicketDbContext>();
    
    // Remove all related entities for this tenant
    var shows = await context.Shows
        .Where(s => s.Venue.Id == venueId && s.TenantId == tenantId)
        .ToListAsync();
    context.Shows.RemoveRange(shows);
    
    var venue = await context.Venues.FindAsync(venueId);
    if (venue != null)
    {
        context.Venues.Remove(venue);
    }
    
    await context.SaveChangesAsync();
}
```

## Test Categorization for CI/CD

**Structure tests for CI/CD pipelines using traits to control which tests run in different environments.**

### Test Category Attributes

```csharp
// Fast unit tests - always run
[Trait("Category", "Unit")]
[Trait("Speed", "Fast")]
public class VenueDomainTests 
{ 
    // Fast unit tests using in-memory database
    // < 100ms per test
}

// Integration tests - run on feature branches and main
[Trait("Category", "Integration")]
[Trait("Speed", "Medium")]
public class VenueDatabaseTests 
{ 
    // Integration tests using Testcontainers
    // 100ms - 2s per test
}

// Performance/load tests - run nightly only
[Trait("Category", "Performance")]
[Trait("Speed", "Slow")]
public class VenuePerformanceTests 
{ 
    // Performance and load tests
    // > 2s per test
}

// End-to-end tests - run on deployment
[Trait("Category", "E2E")]
[Trait("Speed", "Slow")]
public class VenueEndToEndTests 
{ 
    // Full system tests
    // > 5s per test
}
```

### CI/CD Pipeline Configuration

```csharp
// Different test execution strategies for different environments

// Pull Request CI - Fast feedback
dotnet test --filter "Category=Unit" --logger trx --results-directory TestResults/

// Feature Branch CI - Include integration tests
dotnet test --filter "Category=Unit|Category=Integration" --logger trx

// Main Branch CI - Comprehensive testing
dotnet test --filter "Category!=Performance&Category!=E2E" --logger trx

// Nightly Build - All tests including performance
dotnet test --logger trx

// Deployment Pipeline - E2E validation
dotnet test --filter "Category=E2E" --logger trx
```

### Custom Test Categories

```csharp
// Database-specific categories
[Trait("Database", "PostgreSQL")]
[Trait("Database", "SqlServer")]

// Feature-specific categories
[Trait("Feature", "MultiTenancy")]
[Trait("Feature", "Venues")]
[Trait("Feature", "TicketOffers")]

// Environment-specific categories
[Trait("Environment", "RequiresDocker")]
[Trait("Environment", "RequiresNetwork")]
[Trait("Environment", "RequiresFileSystem")]

// Run specific feature tests
dotnet test --filter "Feature=Venues&Category=Integration"

// Run tests that don't require external dependencies
dotnet test --filter "Environment!=RequiresDocker&Environment!=RequiresNetwork"
```

## Performance Monitoring

### Test Execution Time Tracking

```csharp
[Test]
[Timeout(5000)] // 5 second timeout
public async Task PerformanceTest_ShouldCompleteWithinTimeout()
{
    var stopwatch = Stopwatch.StartNew();
    
    // Act
    var result = await _service.GetVenuesAsync(_tenantId);
    
    // Assert
    stopwatch.Stop();
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000), 
        "Query should complete within 2 seconds");
    Assert.That(result.Count(), Is.GreaterThan(0));
}
```

### Memory Usage Monitoring

```csharp
[Test]
public async Task MemoryUsage_BulkOperations_ShouldNotExceedThreshold()
{
    var initialMemory = GC.GetTotalMemory(forceFullCollection: true);
    
    // Act - Bulk operation
    await CreateLargeDataSetAsync(1000);
    
    var finalMemory = GC.GetTotalMemory(forceFullCollection: true);
    var memoryIncrease = finalMemory - initialMemory;
    
    // Assert memory usage is reasonable (< 50MB for 1000 records)
    Assert.That(memoryIncrease, Is.LessThan(50 * 1024 * 1024));
}
```

## Test Data Management

### Efficient Test Data Creation

```csharp
// Use bulk operations for large test datasets
public async Task<List<Venue>> CreateVenuesInBulkAsync(int count, Guid tenantId)
{
    var venues = new List<Venue>();
    
    for (int i = 0; i < count; i++)
    {
        venues.Add(new Venue($"Test Venue {i}", address, 1000 + i, venueType, tenantId));
    }
    
    // Single database round trip
    _context.Venues.AddRange(venues);
    await _context.SaveChangesAsync();
    
    return venues;
}

// Use shared setup for expensive operations
public class SharedTestFixture : IDisposable
{
    public GloboTicketDbContext Context { get; private set; }
    public Guid SharedTenantId { get; private set; }
    
    public SharedTestFixture()
    {
        SharedTenantId = Guid.NewGuid();
        Context = CreateDatabaseContext();
        
        // Create expensive shared data once
        SeedReferenceDataAsync().Wait();
    }
}
```

These patterns ensure optimal test performance, reliable execution in CI/CD pipelines, and maintainable test suites that scale with your application.