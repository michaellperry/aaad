# Performance and Maintainability

## Test Parallelization

**Run tests in parallel for faster feedback.**

```csharp
// In test project file (.csproj)
<PropertyGroup>
    <ParallelizeTestExecution>true</ParallelizeTestExecution>
</PropertyGroup>

// In test class
[Parallelizable(ParallelScope.All)]
public class VenueServiceTests
{
    // Tests will run in parallel
}
```

## Test Cleanup

**Ensure proper cleanup between tests.**

```csharp
[Test]
public async Task TestCleanupExample()
{
    // Arrange
    var testVenue = await CreateTestDataAsync();
    
    try
    {
        // Act
        var result = await _service.ProcessVenueAsync(testVenue.Id);
        
        // Assert
        Assert.That(result.Success, Is.True);
    }
    finally
    {
        // Cleanup - ensure test data is removed
        await CleanupTestDataAsync(testVenue.Id);
    }
}
```

## Test Categorization for CI/CD

**Structure tests for CI/CD pipelines using traits. Note: This is about organizing existing tests, not TDD patterns.**

```csharp
// Different test categories for CI/CD filtering
[Trait("Category", "Unit")]
public class VenueDomainTests { /* Fast unit tests using in-memory database */ }

[Trait("Category", "Integration")]
public class VenueDatabaseTests { /* Integration tests using Testcontainers - created separately from TDD */ }

[Trait("Category", "Slow")]
public class VenuePerformanceTests { /* Performance tests */ }

// Run appropriate test categories in CI
dotnet test --filter "Category!=Slow"  // Fast CI builds
dotnet test --filter "Category=Slow"   // Nightly performance tests
```

