---
name: ef-core-patterns
description: Use this skill when implementing data access, repositories, or query logic with Entity Framework Core.
---

# Entity Framework Core Best Practices

This skill provides comprehensive patterns and best practices for Entity Framework Core development in .NET applications.

## Entity Configuration

### Fluent API over Data Annotations
**Use Fluent API for complex mappings and keep entities clean.**

```csharp
// ✅ Good - Clean entity, complex configuration in Fluent API
public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid TenantId { get; private set; }
    
    // Private parameterless constructor for EF Core
    private Venue() { }
    
    public Venue(string name, string address, Guid tenantId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}

// Configuration class
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(v => v.Address)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(v => v.IsActive)
            .HasDefaultValue(true);
        builder.Property(v => v.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Tenant isolation
        builder.Property(v => v.TenantId)
            .IsRequired();
            
        // Indexes
        builder.HasIndex(v => v.TenantId)
            .HasDatabaseName("IX_Venues_TenantId");
        builder.HasIndex(v => new { v.TenantId, v.Name })
            .HasDatabaseName("IX_Venues_TenantId_Name");
            
        // Relationships
        builder.HasMany<Act>()
            .WithOne(a => a.Venue)
            .HasForeignKey(a => a.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### Value Objects for Immutable Concepts
**Use value objects for concepts that don't have identity.**

```csharp
// ✅ Good - Address as value object
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }
    
    private Address() { } // EF Core
    
    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
}

// Entity using value object
public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Address Address { get; private set; }
    
    private Venue() { }
    
    public Venue(string name, Address address)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
    }
}

// Configuration for value objects
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.OwnsOne(v => v.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200);
            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100);
            // ... other address properties
        });
    }
}
```

### Aggregate Root Pattern
**Use aggregate roots to maintain consistency boundaries.**

```csharp
// ✅ Good - Venue as aggregate root
public class Venue : AggregateRoot<Venue>
{
    private readonly List<Act> _acts = new();
    public IReadOnlyCollection<Act> Acts => _acts.AsReadOnly();
    
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public bool IsActive { get; private set; }
    
    private Venue() { }
    
    public Venue(string name, Address address)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    // Domain methods that maintain invariants
    public void AddAct(string title, DateTime eventDate, decimal ticketPrice)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot add acts to inactive venue");
            
        var act = new Act(title, eventDate, ticketPrice, Id, TenantId);
        _acts.Add(act);
    }
    
    public void Deactivate()
    {
        IsActive = false;
        DomainEvents.Raise(new VenueDeactivated(Id));
    }
}
```

## Query Performance

### AsNoTracking for Read-Only Queries
**Use AsNoTracking to improve performance when entities won't be modified.**

```csharp
// ✅ Good - Read-only query
public async Task<IEnumerable<VenueDto>> GetActiveVenuesAsync(Guid tenantId)
{
    return await _context.Venues
        .Where(v => v.TenantId == tenantId && v.IsActive)
        .AsNoTracking() // Performance optimization
        .OrderBy(v => v.Name)
        .Select(v => new VenueDto 
        { 
            Id = v.Id, 
            Name = v.Name,
            Address = $"{v.Address.Street}, {v.Address.City}"
        })
        .ToListAsync();
}

// ✅ Good - Tracking for modifications
public async Task<Venue> GetVenueForEditAsync(Guid venueId, Guid tenantId)
{
    return await _context.Venues
        .FirstOrDefaultAsync(v => v.Id == venueId && v.TenantId == tenantId);
        // No AsNoTracking - we want to modify this entity
}
```

### Proper Indexing Strategy
**Create indexes for frequent query patterns.**

```csharp
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        // Primary key (automatically created)
        builder.HasKey(v => v.Id);
        
        // Unique index for tenant + name combination
        builder.HasIndex(v => new { v.TenantId, v.Name })
            .IsUnique()
            .HasDatabaseName("IX_Venues_TenantId_Name_Unique");
            
        // Index for filtering by tenant
        builder.HasIndex(v => v.TenantId)
            .HasDatabaseName("IX_Venues_TenantId");
            
        // Index for active venues
        builder.HasIndex(v => new { v.TenantId, v.IsActive })
            .HasDatabaseName("IX_Venues_TenantId_IsActive");
            
        // Composite index for common queries
        builder.HasIndex(v => new { v.TenantId, v.IsActive, v.CreatedAt })
            .HasDatabaseName("IX_Venues_TenantId_IsActive_CreatedAt");
    }
}
```

### Avoiding N+1 Queries
**Use Include() and ThenInclude() judiciously, or consider split queries.**

```csharp
// ❌ Bad - N+1 query problem
var venues = await _context.Venues
    .Where(v => v.TenantId == tenantId)
    .ToListAsync();

foreach (var venue in venues)
{
    venue.Acts = await _context.Acts
        .Where(a => a.VenueId == venue.Id)
        .ToListAsync();
}

// ✅ Good - Single query with Include
var venues = await _context.Venues
    .Include(v => v.Acts.Where(a => a.EventDate >= DateTime.UtcNow))
    .Where(v => v.TenantId == tenantId && v.IsActive)
    .ToListAsync();

// ✅ Good - Split query for better performance
var venues = await _context.Venues
    .AsSplitQuery()
    .Include(v => v.Acts)
    .Where(v => v.TenantId == tenantId && v.IsActive)
    .ToListAsync();
```

## Change Tracking and Concurrency

### Optimistic Concurrency
**Use row version (timestamp) for optimistic concurrency control.**

```csharp
public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public byte[] RowVersion { get; private set; } // Concurrency token
    
    private Venue() { }
    
    public void UpdateName(string newName)
    {
        Name = newName;
    }
}

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.Property(v => v.RowVersion)
            .IsRowVersion()
            .HasColumnName("RowVersion");
    }
}

// Usage in service
public async Task UpdateVenueAsync(Guid venueId, string newName)
{
    var venue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venueId);
    venue.UpdateName(newName);
    
    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        throw new ConcurrencyException("Venue was modified by another user");
    }
}
```

### Short-Lived Contexts
**Use short-lived DbContext instances for web requests.**

```csharp
// ✅ Good - Per-request lifetime
services.AddScoped<GloboTicketDbContext>(provider =>
{
    var optionsBuilder = new DbContextOptionsBuilder<GloboTicketDbContext>();
    optionsBuilder.UseSqlServer(connectionString);
    
    var tenantContext = provider.GetRequiredService<ITenantContext>();
    return new GloboTicketDbContext(optionsBuilder.Options, tenantContext);
});

// ✅ Good - Unit of Work pattern
public class UnitOfWork : IUnitOfWork
{
    private readonly GloboTicketDbContext _context;
    private bool _disposed;
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
```

## Multi-Tenancy Considerations

### Automatic Tenant Filtering
**Implement global query filters for tenant isolation.**

```csharp
public class GloboTicketDbContext : DbContext
{
    private readonly Guid _currentTenantId;
    
    public GloboTicketDbContext(DbContextOptions<GloboTicketDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        _currentTenantId = tenantContext.TenantId;
    }
    
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Act> Acts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply tenant filter to all tenant-aware entities
        ApplyTenantFilters(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var tenantProperty = Expression.Property(parameter, "TenantId");
            var tenantValue = Expression.Constant(_currentTenantId);
            var equals = Expression.Equal(tenantProperty, tenantValue);
            var lambda = Expression.Lambda(equals, parameter);
            
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
```

## Repository Patterns

### Generic Repository Interface
**Keep repositories focused and testable.**

```csharp
public interface IRepository<T> where T : Entity<Guid>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

public interface IVenueRepository : IRepository<Venue>
{
    Task<IEnumerable<Venue>> GetActiveVenuesAsync(Guid tenantId);
    Task<Venue?> GetByNameAsync(string name, Guid tenantId);
    Task<bool> ExistsByNameAsync(string name, Guid tenantId);
}
```

### Specification Pattern
**Use specifications for complex queries.**

```csharp
public abstract class Specification<T> where T : Entity<Guid>
{
    protected Specification(Expression<Func<T, bool>>? criteria)
    {
        Criteria = criteria;
    }
    
    public Expression<Func<T, bool>>? Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
    
    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
}

// Concrete specification
public class ActiveVenuesSpecification : Specification<Venue>
{
    public ActiveVenuesSpecification(Guid tenantId) : base(v => 
        v.TenantId == tenantId && v.IsActive)
    {
        AddInclude(v => v.Acts);
    }
}

// Usage
public async Task<IEnumerable<Venue>> GetActiveVenuesAsync(Guid tenantId)
{
    var specification = new ActiveVenuesSpecification(tenantId);
    return await _repository.GetBySpecificationAsync(specification);
}
```

## Testing with EF Core

### In-Memory Database for Unit Tests
**Use in-memory provider for fast unit tests.**

```csharp
public class VenueServiceTests
{
    private GloboTicketDbContext _context;
    private VenueService _service;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext());
        _service = new VenueService(new VenueRepository(_context));
    }
    
    [Test]
    public async Task AddVenue_ValidVenue_AddsSuccessfully()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var venue = new Venue("Test Venue", new Address("123 Main St", "City", "State", "12345", "USA"));
        
        // Act
        await _service.AddVenueAsync(venue);
        
        // Assert
        var savedVenue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venue.Id);
        Assert.That(savedVenue, Is.Not.Null);
        Assert.That(savedVenue!.Name, Is.EqualTo("Test Venue"));
    }
}
```

### SQL Server for Integration Tests
**Use real SQL Server for integration testing.**

```csharp
public class VenueIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    
    public VenueIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ComplexVenueQuery_PerformsEfficiently()
    {
        // Test actual database performance
        var venues = await _fixture.VenueService
            .GetVenuesWithRecentActsAsync(_fixture.TenantId);
            
        Assert.That(venues, Is.Not.Empty);
    }
}
```

## Migration Best Practices

### Meaningful Migration Names
**Use descriptive names for migrations.**

```bash
# ✅ Good
dotnet ef migrations add AddVenueAndActEntities
dotnet ef migrations add AddTenantSupport
dotnet ef migrations add AddVenueAddressValueObject

# ❌ Bad
dotnet ef migrations add Migration1
dotnet ef migrations add Update
```

### Migration Scripts for Production
**Always generate scripts for production deployments.**

```bash
# Generate migration script
dotnet ef migrations script --output Migration_Script.sql

# Generate script with specific migrations
dotnet ef migrations script --from InitialCreate --to AddVenueSupport
```

### Data Migration Strategy
**Handle data migrations carefully in production.**

```csharp
// ✅ Good - Safe data migration
protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Add new column nullable
    migrationBuilder.AddColumn<string>(
        name: "NewField",
        table: "Venues",
        type: "nvarchar(100)",
        nullable: true);
    
    // 2. Backfill data
    migrationBuilder.Sql(@"
        UPDATE Venues 
        SET NewField = 'Default Value'
        WHERE NewField IS NULL");
    
    // 3. Make column required
    migrationBuilder.AlterColumn<string>(
        name: "NewField",
        table: "Venues",
        type: "nvarchar(100)",
        nullable: false);
}
```

Following these patterns ensures maintainable, performant, and testable data access layers that work well with Clean Architecture and multi-tenancy requirements.