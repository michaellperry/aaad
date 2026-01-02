# Mocking and Dependency Management

## Prefer In-Memory Database for Repository Tests

**Use actual repositories backed by in-memory database providers for all unit tests. This provides better integration coverage and catches real database behavior issues.**

```csharp
// ✅ Good - In-memory database with actual repository
public class VenueServiceTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private VenueService _service;
    private Guid _tenantId;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
        _repository = new VenueRepository(_context);
        _service = new VenueService(_repository);
        _tenantId = Guid.NewGuid();
    }
    
    [Test]
    public async Task CreateVenue_ValidData_CreatesVenueSuccessfully()
    {
        // Arrange
        var createDto = new CreateVenueDto
        {
            Name = "Test Venue",
            Address = "Test Address",
            TenantId = _tenantId
        };
        
        // Act
        var result = await _service.CreateVenueAsync(createDto);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Venue"));
        
        var savedVenue = await _context.Venues.SingleOrDefaultAsync(v => v.Id == result.Id);
        Assert.That(savedVenue, Is.Not.Null);
        Assert.That(savedVenue!.Name, Is.EqualTo("Test Venue"));
    }
    
    [Test]
    public async Task CreateVenue_DuplicateName_ThrowsException()
    {
        // Arrange
        var existingVenue = new Venue("Existing Venue", CreateTestAddress(), _tenantId);
        await _repository.AddAsync(existingVenue);
        await _context.SaveChangesAsync();
        
        var createDto = new CreateVenueDto
        {
            Name = "Existing Venue",
            Address = "Test Address",
            TenantId = _tenantId
        };
        
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateVenueAsync(createDto));
    }
}
```

## Mock External Dependencies Only

**Mock only external services, APIs, or dependencies that cannot be easily replaced with in-memory alternatives.**

```csharp
// ✅ Good - Mock external service dependency
public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string subject);
}

public class VenueService
{
    private readonly IVenueRepository _repository;
    private readonly IEmailService _emailService;
    
    public VenueService(IVenueRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }
}

[Test]
public async Task CreateVenue_ValidData_SendsConfirmationEmail()
{
    // Arrange
    var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    var context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
    var repository = new VenueRepository(context);
    
    var mockEmailService = new Mock<IEmailService>();
    var service = new VenueService(repository, mockEmailService.Object);
    
    var createDto = new CreateVenueDto
    {
        Name = "Test Venue",
        Address = "Test Address",
        TenantId = _tenantId
    };
    
    // Act
    await service.CreateVenueAsync(createDto);
    
    // Assert
    mockEmailService.Verify(
        e => e.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), 
        Times.Once);
}
```

```csharp
// ❌ Bad - Mocking repository when in-memory database is available
[Test]
public async Task CreateVenue_ValidData_CreatesVenue()
{
    var mockRepository = new Mock<IVenueRepository>();
    var service = new VenueService(mockRepository.Object);
    // ... test implementation
}
```

## When to Use Each Approach

- **✅ In-Memory Database**: Use for all repository and service tests that interact with data persistence. Provides real database behavior validation.
- **✅ Mocking**: Use only for external dependencies (email services, payment gateways, third-party APIs) or when testing error scenarios that are difficult to reproduce with in-memory database.

**Note**: For more details on in-memory database testing, see the `testing-ef-core-in-memory` skill.

