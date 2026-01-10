# API Testing

Integration tests, contract testing, and mock strategies for RESTful APIs.

## Basic Integration Testing

### Test Setup
Setting up integration tests for API endpoints.

```csharp
// Test class setup
[TestClass]
public class VenuesControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    // Replace database with in-memory version
                    services.RemoveDbContext<GloboTicketContext>();
                    services.AddDbContext<GloboTicketContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
```

### GET Endpoint Testing
Testing successful and error scenarios for GET requests.

```csharp
[TestMethod]
public async Task GetVenues_ReturnsSuccessAndCorrectContentType()
{
    // Act
    var response = await _client.GetAsync("/api/venues");

    // Assert
    response.EnsureSuccessStatusCode();
    Assert.AreEqual("application/json; charset=utf-8", 
        response.Content.Headers.ContentType?.ToString());
}

[TestMethod]
public async Task GetVenue_WithValidId_ReturnsVenue()
{
    // Arrange
    var venueId = await CreateTestVenueAsync();

    // Act
    var response = await _client.GetAsync($"/api/venues/{venueId}");

    // Assert
    response.EnsureSuccessStatusCode();
    var venue = await response.Content.ReadFromJsonAsync<VenueDto>();
    Assert.IsNotNull(venue);
    Assert.AreEqual(venueId, venue.Id);
}

[TestMethod]
public async Task GetVenue_WithInvalidId_ReturnsNotFound()
{
    // Arrange
    var invalidId = Guid.NewGuid();

    // Act
    var response = await _client.GetAsync($"/api/venues/{invalidId}");

    // Assert
    Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
}
```

### POST Endpoint Testing
Testing creation endpoints with valid and invalid data.

```csharp
[TestMethod]
public async Task CreateVenue_WithValidData_ReturnsCreatedVenue()
{
    // Arrange
    var createDto = new CreateVenueDto
    {
        Name = "Test Theater",
        Capacity = 500,
        Address = new AddressDto
        {
            Street = "123 Main St",
            City = "Test City",
            State = "TS",
            PostalCode = "12345",
            Country = "USA"
        }
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/venues", createDto);

    // Assert
    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    var venue = await response.Content.ReadFromJsonAsync<VenueDto>();
    Assert.IsNotNull(venue);
    Assert.AreEqual(createDto.Name, venue.Name);
    
    // Check Location header
    Assert.IsTrue(response.Headers.Location?.ToString().Contains($"/api/venues/{venue.Id}"));
}

[TestMethod]
public async Task CreateVenue_WithInvalidData_ReturnsBadRequest()
{
    // Arrange
    var invalidDto = new CreateVenueDto
    {
        Name = "", // Invalid: empty name
        Capacity = -1, // Invalid: negative capacity
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/venues", invalidDto);

    // Assert
    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
}

// Helper method for test setup
private async Task<Guid> CreateTestVenueAsync()
{
    var createDto = new CreateVenueDto
    {
        Name = "Test Venue",
        Capacity = 1000,
        Address = new AddressDto
        {
            Street = "456 Test Ave",
            City = "Test City",
            State = "TS", 
            PostalCode = "54321",
            Country = "USA"
        }
    };
    
    var response = await _client.PostAsJsonAsync("/api/venues", createDto);
    var venue = await response.Content.ReadFromJsonAsync<VenueDto>();
    return venue!.Id;
}
```