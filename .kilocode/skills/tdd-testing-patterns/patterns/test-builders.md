# Test Data Builders

## Object Mother Pattern

**Create reusable test data builders for complex objects.**

```csharp
public class VenueBuilder
{
    private string _name = "Test Venue";
    private Address _address = CreateTestAddress();
    private Guid _tenantId = Guid.NewGuid();
    private bool _isActive = true;
    
    public static VenueBuilder Create()
    {
        return new VenueBuilder();
    }
    
    public VenueBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public VenueBuilder WithAddress(Address address)
    {
        _address = address;
        return this;
    }
    
    public VenueBuilder WithTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
        return this;
    }
    
    public VenueBuilder Inactive()
    {
        _isActive = false;
        return this;
    }
    
    public Venue Build()
    {
        return new Venue(_name, _address, _tenantId) { IsActive = _isActive };
    }
}

// Usage in tests
[Test]
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

