# Mocking and Dependency Management

## Mock External Dependencies

**Use interfaces for all dependencies and mock them in tests.**

```csharp
// ✅ Good - Interface-based design
public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id);
    Task AddAsync(Venue venue);
    Task<bool> ExistsByNameAsync(string name, Guid tenantId);
}

public class VenueService
{
    private readonly IVenueRepository _repository;
    
    public VenueService(IVenueRepository repository)
    {
        _repository = repository;
    }
}

// ✅ Good - Mocked dependencies in tests
[Test]
public async Task VenueService_CreateVenue_ValidData_CallsRepository()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var service = new VenueService(mockRepository.Object);
    
    var createDto = new CreateVenueDto
    {
        Name = "Test Venue",
        Address = "Test Address",
        TenantId = _tenantId
    };
    
    mockRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()))
        .ReturnsAsync(false);
    
    // Act
    var result = await service.CreateVenueAsync(createDto);
    
    // Assert
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Venue>()), Times.Once);
}
```

## In-Memory Database for Repository Tests

**Use in-memory database for repository integration tests.**

```csharp
public class VenueRepositoryTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private Guid _tenantId;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
        _repository = new VenueRepository(_context);
        _tenantId = Guid.NewGuid();
    }
    
    [Test]
    public async Task AddAsync_ValidVenue_AddsVenueSuccessfully()
    {
        // Arrange
        var venue = new Venue("Test Venue", CreateTestAddress(), _tenantId);
        
        // Act
        await _repository.AddAsync(venue);
        await _context.SaveChangesAsync();
        
        // Assert
        var savedVenue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venue.Id);
        Assert.That(savedVenue, Is.Not.Null);
        Assert.That(savedVenue!.Name, Is.EqualTo("Test Venue"));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingVenue_ReturnsVenue()
    {
        // Arrange
        var venue = new Venue("Test Venue", CreateTestAddress(), _tenantId);
        await _repository.AddAsync(venue);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetByIdAsync(venue.Id);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(venue.Id));
        Assert.That(result.Name, Is.EqualTo("Test Venue"));
    }
}
```

**Note**: For more details on in-memory database testing, see the `testing-ef-core-in-memory` skill.

