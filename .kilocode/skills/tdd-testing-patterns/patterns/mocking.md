# Mocking and Dependency Management

## Prefer In-Memory Database for Repository Tests

**Use actual repositories backed by in-memory database providers for all unit tests. This provides better coverage of database behavior and catches real EF Core query issues while maintaining fast unit test execution.**

```csharp
// ✅ Good - In-memory database with actual repository
public class VenueServiceTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private VenueService _service;
    private Guid _tenantId;
    
    public VenueServiceTests()
    {
        _tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
        _repository = new VenueRepository(_context);
        _service = new VenueService(_repository);
    }
    
    [Fact]
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
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Venue");
        
        var savedVenue = await _context.Venues.SingleOrDefaultAsync(v => v.Id == result.Id);
        savedVenue.Should().NotBeNull();
        savedVenue!.Name.Should().Be("Test Venue");
    }
    
    [Fact]
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
        var act = async () => await _service.CreateVenueAsync(createDto);
        
        await act.Should().ThrowAsync<InvalidOperationException>();
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

[Fact]
public async Task CreateVenue_ValidData_SendsConfirmationEmail()
{
    // Arrange
    var tenantId = Guid.NewGuid();
    var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    var context = new GloboTicketDbContext(options, MockTenantContext(tenantId));
    var repository = new VenueRepository(context);
    
    var mockEmailService = Substitute.For<IEmailService>();
    var service = new VenueService(repository, mockEmailService);
    
    var createDto = new CreateVenueDto
    {
        Name = "Test Venue",
        Address = "Test Address",
        TenantId = tenantId
    };
    
    // Act
    await service.CreateVenueAsync(createDto);
    
    // Assert
    await mockEmailService.Received(1)
        .SendConfirmationEmailAsync(Arg.Any<string>(), Arg.Any<string>());
}
```

```csharp
// ❌ Bad - Mocking repository when in-memory database is available
[Fact]
public async Task CreateVenue_ValidData_CreatesVenue()
{
    var mockRepository = Substitute.For<IVenueRepository>();
    var service = new VenueService(mockRepository);
    // ... test implementation
}
```

## When to Use Each Approach

- **✅ In-Memory Database**: Use for all repository and service tests that interact with data persistence. Provides real database behavior validation.
- **✅ Mocking**: Use only for external dependencies (email services, payment gateways, third-party APIs) or when testing error scenarios that are difficult to reproduce with in-memory database.

## Related Skill

**For EF Core in-memory database setup**: See the `testing-ef-core-in-memory` skill for:
- Technical setup patterns (`CreateInMemoryDbContext()`)
- `TestTenantContext` implementation
- Multi-tenant testing strategies
- Package requirements

This skill (`tdd-testing-patterns`) focuses on test structure, naming, and patterns. The `testing-ef-core-in-memory` skill provides the infrastructure setup. **Use both skills together when writing service and repository tests.**

