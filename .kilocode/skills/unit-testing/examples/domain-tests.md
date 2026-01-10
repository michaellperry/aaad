# Domain Logic Testing Examples

Examples of testing entities and value objects.

## Testing Entities

Focus on testing business rules and invariants in domain objects.

```csharp
public class VenueTests
{
    [Fact]
    public void Venue_Deactivate_DeactivatesVenueAndAddsDomainEvent()
    {
        // Given: An active venue
        var venue = GivenVenue();
        
        // When: Deactivating the venue
        venue.Deactivate();
        
        // Then: Venue is inactive
        venue.IsActive.Should().BeFalse();
    }
    
    [Fact]
    public void Venue_AddAct_InactiveVenue_ThrowsInvalidOperationException()
    {
        // Given: An inactive venue
        var venue = GivenVenue();
        venue.Deactivate();
        
        // When: Attempting to add an act
        var act = () => venue.AddAct("Concert A", DateTime.UtcNow.AddDays(30), 50.00m);
        
        // Then: Exception is thrown
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*inactive venue*");
    }
    
    private Venue GivenVenue(string name = "Test Venue", bool isActive = true)
    {
        return new Venue(name, CreateTestAddress(), Guid.NewGuid())
        {
            IsActive = isActive
        };
    }
}
```

## Testing Value Objects

Value objects should be tested for equality and immutability.

```csharp
[Fact]
public void Address_Equals_SameValues_ReturnsTrue()
{
    // Given: Two addresses with same values
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // When/Then: They should be equal
    address1.Should().Be(address2);
    (address1 == address2).Should().BeTrue();
}

[Fact]
public void Address_Equals_DifferentValues_ReturnsFalse()
{
    // Given: Two addresses with different values
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("456 Oak Ave", "City", "State", "12345", "USA");
    
    // When/Then: They should not be equal
    address1.Should().NotBe(address2);
    (address1 == address2).Should().BeFalse();
}
```
