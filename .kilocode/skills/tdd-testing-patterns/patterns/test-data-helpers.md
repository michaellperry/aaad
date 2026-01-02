# Test Data Helpers

## Given Helper Methods Pattern

**Use helper methods with the `Given` prefix in test classes to create test data. These methods take parameters with default values, allowing tests to override only the important properties.**

### Pattern Structure

Test classes should contain private helper methods that:
- Use the `Given` prefix (e.g., `GivenVenue`, `GivenAct`, `GivenCustomer`)
- Set up test scenarios and create test data
- Accept parameters with default values
- Allow tests to override only the properties that matter for the specific test

### ✅ Good - Given Helper Methods

```csharp
public class VenueServiceTests
{
    private GloboTicketDbContext _context;
    private VenueService _venueService;
    private Guid _tenantId;
    
    public VenueServiceTests()
    {
        _context = new GloboTicketDbContext(/* in-memory options */);
        _venueService = new VenueService(_context);
        _tenantId = Guid.NewGuid();
    }
    
    // Given helper method with default parameters
    private Venue GivenVenue(
        string name = "Test Venue",
        Address address = null,
        Guid? tenantId = null,
        bool isActive = true)
    {
        return new Venue(
            name,
            address ?? CreateTestAddress(),
            tenantId ?? _tenantId)
        {
            IsActive = isActive
        };
    }
    
    // Test overriding only important properties - visually distinct
    [Fact]
    public void VenueService_CreateVenue_ValidData_ReturnsCreatedVenue()
    {
        // Arrange
        var venue = GivenVenue(name: "Madison Square Garden");
        
        // Act & Assert
        // Test implementation
    }
    
    // Test using default values - clearly shows generic test data
    [Fact]
    public async Task VenueService_GetVenue_ExistingVenue_ReturnsVenue()
    {
        // Arrange
        var venue = GivenVenue();
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync();
        
        // Act & Assert
        // Test implementation
    }
    
    // Test overriding multiple properties when needed
    [Fact]
    public void VenueService_CreateVenue_InactiveVenue_ThrowsException()
    {
        // Arrange
        var venue = GivenVenue(
            name: "Closed Venue",
            isActive: false);
        
        // Act & Assert
        // Test implementation
    }
}
```

### Benefits

1. **Visual Distinction**: Tests that override parameters (e.g., `GivenVenue(name: "Madison Square Garden")`) are visually distinct from tests using defaults (`GivenVenue()`)
2. **Simplicity**: No separate builder classes needed; helpers live in the test class
3. **Flexibility**: Default parameters allow tests to override only what matters
4. **Consistency**: Aligns with the Given-When-Then naming convention used in test methods

### ❌ Bad - Separate Builder Classes

```csharp
// Avoid separate builder classes
public class VenueBuilder
{
    private string _name = "Test Venue";
    // ... builder implementation
}

[Fact]
public void VenueService_CreateVenue_ValidData_ReturnsCreatedVenue()
{
    // Arrange
    var venue = VenueBuilder.Create()
        .WithName("Madison Square Garden")
        .WithTenantId(_tenantId)
        .Build();
    
    // Act & Assert
    // Test implementation
}
```

**Why avoid this?** Separate builder classes add unnecessary complexity and don't provide the visual distinction that `Given` helper methods with default parameters offer.

