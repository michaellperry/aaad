# Performance and Maintainability

## Test Parallelization

**Run tests in parallel for faster feedback. Integration tests can safely run in parallel when using isolated tenant IDs and fresh database contexts.**

```csharp
// In test project file (.csproj)
<PropertyGroup>
    <ParallelizeTestExecution>true</ParallelizeTestExecution>
</PropertyGroup>

// In test class
[Parallelizable(ParallelScope.All)]
public class VenueServiceIntegrationTests
{
    // Tests will run in parallel
    // Each test uses unique tenant ID via _fixture.GenerateRandomTenantId()
}
```

## Test Cleanup

**Ensure proper cleanup between tests to prevent data pollution in the shared database container.**

```csharp
[Test]
public async Task TestCleanupExample()
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
```

## Test Categorization for CI/CD

**Structure tests for CI/CD pipelines using traits to control which tests run in different environments.**

```csharp
// Different test categories for CI/CD filtering
[Trait("Category", "Unit")]
public class VenueDomainTests { /* Fast unit tests using in-memory database */ }

[Trait("Category", "Integration")]
public class VenueDatabaseTests { /* Integration tests using Testcontainers */ }

[Trait("Category", "Slow")]
public class VenuePerformanceTests { /* Performance tests */ }

// Run appropriate test categories in CI
dotnet test --filter "Category!=Slow"  // Fast CI builds
dotnet test --filter "Category=Slow"   // Nightly performance tests
dotnet test --filter "Category=Integration"  // Integration tests only
```

