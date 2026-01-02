---
name: tdd-testing-patterns
description: Provides TDD patterns, AAA structure, and testing strategies for .NET applications. Use when writing unit tests or implementing test-driven development workflows in GloboTicket. Note: TDD is about unit tests only; integration tests follow implementation and are handled separately.
---

# Test-Driven Development Patterns

This skill provides TDD patterns and testing strategies for .NET applications, supporting the Red-Green-Refactor cycle.

## TDD Philosophy

**TDD is about unit tests only.** Unit tests drive implementation through the red-green-refactor cycle. They use real repositories with EF Core In-Memory Provider for fast, isolated testing of business logic.

**Integration tests follow implementation.** They use real repositories with SQL Server (via Testcontainers) and migrated databases to verify migrations, complex queries, and system integration. Integration tests are valuable but separate from the TDD workflow.

## Test Structure and Organization

### AAA Pattern (Arrange-Act-Assert)
**Structure every test using the AAA pattern for clarity and maintainability.**

```csharp
[Test]
public void Venue_Constructor_SetsPropertiesCorrectly()
{
    // Arrange - Setup test data and dependencies
    var name = "Madison Square Garden";
    var address = new Address("123 Broadway", "New York", "NY", "10001", "USA");
    var tenantId = Guid.NewGuid();
    
    // Act - Execute the method under test
    var venue = new Venue(name, address, tenantId);
    
    // Assert - Verify expected outcomes
    Assert.Multiple(() =>
    {
        Assert.That(venue.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(venue.Name, Is.EqualTo(name));
        Assert.That(venue.Address, Is.EqualTo(address));
        Assert.That(venue.TenantId, Is.EqualTo(tenantId));
        Assert.That(venue.IsActive, Is.True);
        Assert.That(venue.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
    });
}
```

### Test Naming Conventions
**Use descriptive test names that follow the pattern: MethodName_Scenario_ExpectedBehavior**

```csharp
// ✅ Good - Descriptive test names
[Test]
public void AddAct_ValidAct_AddsActToVenue()
{
    // Test implementation
}

[Test]
public void AddAct_InactiveVenue_ThrowsInvalidOperationException()
{
    // Test implementation
}

[Test]
public void VenueRepository_GetActiveVenues_ReturnsOnlyActiveVenues()
{
    // Test implementation
}

// ❌ Bad - Unclear test names
[Test]
public void Test1() { }
[Test]
public void TestVenue() { }
[Test]
public void AddTest() { }
```

### Given-When-Then Format
**Use Given-When-Then comments to make test intent crystal clear.**

```csharp
[Test]
public async Task VenueService_CreateVenue_ValidData_ReturnsCreatedVenue()
{
    // Given we have valid venue data
    var createVenueDto = new CreateVenueDto
    {
        Name = "The O2 Arena",
        Address = "Peninsula Square, London SE10 0DX",
        TenantId = _tenantId
    };
    
    // And our in-memory database is empty (no existing venues)
    // (Setup occurs in [SetUp] method with UseInMemoryDatabase)
    
    // When we create the venue
    var result = await _venueService.CreateVenueAsync(createVenueDto);
    
    // Then we should receive a valid venue DTO
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(createVenueDto.Name));
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    
    // And the venue should be persisted in the database
    var savedVenue = await _context.Venues.SingleOrDefaultAsync(v => v.Id == result.Id);
    Assert.That(savedVenue, Is.Not.Null);
    Assert.That(savedVenue!.Name, Is.EqualTo(createVenueDto.Name));
}
```

## Detailed Patterns

For comprehensive examples and detailed guidance, see:

- **[Unit Testing](patterns/unit-testing.md)**: Testing entities, value objects, and domain logic
- **[Mocking](patterns/mocking.md)**: Prefer in-memory database for repositories; mock only external dependencies
- **[Test Data Helpers](patterns/test-data-helpers.md)**: Given helper methods with default parameters for creating test data
- **[Performance](patterns/performance.md)**: Test parallelization, cleanup strategies, and CI/CD patterns

**Note**: Integration testing patterns are separate from TDD. Integration tests follow implementation and use Testcontainers with real SQL Server. See the `integration-test-writer` mode and `integration-test-patterns` skill for integration testing guidance.
