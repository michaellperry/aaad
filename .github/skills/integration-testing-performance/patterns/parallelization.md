# Test Parallelization

Enable parallel execution with unique tenant IDs and fresh DbContext instances per test.

```csharp
// In test project file (.csproj)
<PropertyGroup>
    <ParallelizeTestExecution>true</ParallelizeTestExecution>
    <MaxCpuCount>0</MaxCpuCount>
</PropertyGroup>

[Parallelizable(ParallelScope.All)]
public class VenueServiceIntegrationTests
{
    [Test]
    public async Task CreateVenue_ValidData_CreatesVenueSuccessfully()
    {
        var tenantId = _fixture.GenerateRandomTenantId();
        using var scope = _fixture.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<VenueService>();
        // Act & Assert
    }
}
```

```csharp
// ✅ Isolated test data
[Test]
public async Task ParallelSafeTest()
{
    var tenantId = _fixture.GenerateRandomTenantId();
    var venueId = Guid.NewGuid();
    // operate on isolated data
}

// ❌ Shared test data
[Test]
public async Task SharedDataTest()
{
    var venue = await GetSharedTestVenue();
    // shared state can cause interference
}
```