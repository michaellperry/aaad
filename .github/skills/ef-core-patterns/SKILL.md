---
name: ef-core-patterns
description: Entity Framework Core best practices, patterns, and performance optimization for .NET applications. Use when implementing data access, repositories, query logic, or working with EF Core configuration and migrations.
---

# Entity Framework Core Best Practices

This skill provides comprehensive patterns and best practices for Entity Framework Core development in .NET applications.

## Entity Configuration

### Fluent API over Data Annotations
**Use Fluent API for complex mappings and keep entities clean.**

```csharp
// ✅ Good - Clean entity, complex configuration in Fluent API
public class Venue : MultiTenantEntity
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<Act> _acts = new();
    public IReadOnlyCollection<Act> Acts => _acts.AsReadOnly();
    
    // Private parameterless constructor for EF Core
    private Venue() { }
    
    public Venue(string name, Address address, Guid tenantId)
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
            
        // Value object configuration
        builder.OwnsOne(v => v.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200);
            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100);
        });
            
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

// Configuration
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.Property(v => v.RowVersion)
            .IsRowVersion();
    }
}

// Usage
public async Task<bool> UpdateVenueAsync(Guid venueId, string newName, byte[] rowVersion)
{
    try 
    {
        var venue = await _context.Venues
            .FirstOrDefaultAsync(v => v.Id == venueId);
            
        if (venue == null) return false;
        
        venue.UpdateName(newName);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateConcurrencyException)
    {
        // Handle concurrency conflict
        return false;
    }
}
```

## Multi-Tenant Query Filters

### Global Query Filters
**Automatically filter by tenant ID for all queries.**

```csharp
public class GloboTicketDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply tenant filters to all ITenantEntity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var tenantProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                var tenantValue = Expression.Constant(_tenantContext.TenantId);
                var equals = Expression.Equal(tenantProperty, tenantValue);
                var lambda = Expression.Lambda(equals, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
```

## Repository Pattern Implementation

```csharp
public class VenueRepository : IVenueRepository
{
    private readonly GloboTicketDbContext _context;
    
    public VenueRepository(GloboTicketDbContext context)
    {
        _context = context;
    }
    
    public async Task<Venue> AddAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);
        return venue;
    }
    
    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Venues
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }
    
    public async Task UpdateAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        _context.Venues.Update(venue);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var venue = await GetByIdAsync(id, cancellationToken);
        if (venue != null)
        {
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
```

These patterns ensure optimal performance, maintainability, and proper multi-tenant data isolation in Entity Framework Core implementations.