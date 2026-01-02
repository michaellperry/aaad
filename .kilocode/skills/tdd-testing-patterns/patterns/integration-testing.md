# Integration Testing

## API Endpoint Tests

**Test API controllers and HTTP endpoints.**

```csharp
[Test]
public async Task VenueEndpoints_CreateVenue_ValidRequest_ReturnsCreated()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new CreateVenueRequest
    {
        Name = "Madison Square Garden",
        Address = "123 Broadway, New York, NY 10001"
    };
    
    client.DefaultRequestHeaders.Add("X-Tenant-Id", _tenantId.ToString());
    
    // Act
    var response = await client.PostAsJsonAsync("/api/venues", request);
    
    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<CreatedVenueResponse>(content);
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
}

[Test]
public async Task VenueEndpoints_GetVenues_NoTenantHeader_ReturnsUnauthorized()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/venues");
    
    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
}
```

## Database Integration Tests

**Test with real database for integration scenarios.**

```csharp
[Collection("Database")]
public class VenueDatabaseTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    
    public VenueDatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task VenueRepository_ComplexQuery_PerformsCorrectly()
    {
        // Arrange
        var tenantId = await _fixture.CreateTestTenantAsync();
        var venue = await _fixture.CreateTestVenueAsync(tenantId);
        var act = await _fixture.CreateTestActAsync(venue.Id, tenantId);
        
        // Act
        var result = await _fixture.VenueService
            .GetVenuesWithUpcomingActsAsync(tenantId, DateTime.UtcNow.AddDays(7));
        
        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Acts, Has.Some.TypeOf<ActDto>());
    }
}
```

