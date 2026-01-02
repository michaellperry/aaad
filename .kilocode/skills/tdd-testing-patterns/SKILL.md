---
name: tdd-testing-patterns
description: Provides TDD patterns, AAA structure, and testing strategies for .NET applications. Use when writing unit tests, integration tests, or implementing test-driven development workflows in GloboTicket.
---

# Test-Driven Development Patterns

This skill provides comprehensive TDD patterns and testing strategies for .NET applications, supporting the Red-Green-Refactor cycle.

## Quick Reference

- **Test Structure**: Use AAA pattern (Arrange-Act-Assert) - see [Test Structure](#test-structure-and-organization)
- **Unit Testing**: Domain logic, entities, value objects - see [patterns/unit-testing.md](patterns/unit-testing.md)
- **Mocking**: Dependency management and mocking strategies - see [patterns/mocking.md](patterns/mocking.md)
- **Integration Testing**: API endpoints and database tests - see [patterns/integration-testing.md](patterns/integration-testing.md)
- **Test Builders**: Object Mother pattern and test data builders - see [patterns/test-builders.md](patterns/test-builders.md)
- **Performance**: Test parallelization, cleanup, and CI/CD - see [patterns/performance.md](patterns/performance.md)

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
    
    // And our repository is empty (no existing venues)
    _venueRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()))
        .ReturnsAsync(false);
    
    // When we create the venue
    var result = await _venueService.CreateVenueAsync(createVenueDto);
    
    // Then we should receive a valid venue DTO
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(createVenueDto.Name));
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    
    // And the repository should have been called to save the venue
    _venueRepository.Verify(r => r.AddAsync(It.IsAny<Venue>()), Times.Once);
}
```

## Detailed Patterns

For comprehensive examples and detailed guidance, see the pattern files:

- **[Unit Testing](patterns/unit-testing.md)**: Testing entities, value objects, and domain logic
- **[Mocking](patterns/mocking.md)**: Dependency management, interface-based design, and mocking strategies
- **[Integration Testing](patterns/integration-testing.md)**: API endpoint tests and database integration tests
- **[Test Builders](patterns/test-builders.md)**: Object Mother pattern and reusable test data builders
- **[Performance](patterns/performance.md)**: Test parallelization, cleanup strategies, and CI/CD patterns

Following these TDD patterns ensures comprehensive test coverage, maintainable test suites, and reliable feedback during development.
