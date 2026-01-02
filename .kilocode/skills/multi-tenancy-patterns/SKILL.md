---
name: multi-tenancy-patterns
description: Multi-tenancy implementation patterns and tenant isolation strategies
---

# Multi-Tenancy Implementation Patterns

Multi-tenancy allows a single application to serve multiple customers (tenants) while keeping their data isolated. This is critical for GloboTicket's SaaS architecture.

## Tenant Isolation Strategies

### 1. Database per Tenant
**Pros:** Maximum data isolation, simple backup/restore per tenant
**Cons:** High operational overhead, resource intensive, complex migrations
**Use Case:** Large enterprise tenants with strict compliance requirements

```sql
-- Separate database per tenant
tenant1_globoticket_db
tenant2_globoticket_db
```

### 2. Schema per Tenant  
**Pros:** Good isolation, manageable operational overhead
**Cons:** Limited scalability, complex connection management
**Use Case:** Medium-sized tenants, moderate tenant count

```sql
-- Separate schema per tenant
tenant1.Venues, tenant1.Acts
tenant2.Venues, tenant2.Acts
```

### 3. Row Level Security (RLS)
**Pros:** Shared infrastructure, cost-effective, scalable
**Cons:** Requires careful security implementation, potential for data leaks
**Use Case:** GloboTicket's approach - many small-medium tenants

```sql
-- Tenant filter on all tables
CREATE POLICY tenant_isolation ON Venues
    USING (tenant_id = current_setting('app.current_tenant')::uuid);
```

### 4. Discriminator Column
**Pros:** Simple implementation, shared queries
**Cons:** Poor isolation, complex queries, scalability issues
**Use Case:** Not recommended for production use

## GloboTicket's Row Level Security Implementation

### Entity Structure
```csharp
// All tenant-aware entities implement ITenantEntity
public interface ITenantEntity
{
    Guid TenantId { get; }
    bool BelongsToTenant(Guid tenantId);
}

// Multi-tenant base entity
public abstract class MultiTenantEntity : Entity<Guid>, ITenantEntity
{
    public Guid TenantId { get; protected set; }
    
    public bool BelongsToTenant(Guid tenantId) => 
        TenantId == tenantId;
}

// Example entity
public class Venue : MultiTenantEntity
{
    public string Name { get; private set; }
    public string Address { get; private set; }
    
    private Venue() { } // EF Core constructor
    
    public Venue(string name, string address, Guid tenantId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        TenantId = tenantId;
    }
}
```

### Tenant Context Resolution
```csharp
// Tenant context middleware
public class TenantResolutionMiddleware
{
    public async Task InvokeAsync(HttpContext context, 
        RequestDelegate next, ITenantContext tenantContext)
    {
        // Resolve tenant from various sources
        var tenantId = await ResolveTenantId(context);
        tenantContext.TenantId = tenantId;
        
        // Set tenant for database context
        context.Items["TenantId"] = tenantId;
        
        await next(context);
    }
    
    private async Task<Guid> ResolveTenantId(HttpContext context)
    {
        // 1. Subdomain: tenant1.globoticket.com
        var subdomain = GetSubdomain(context.Request.Host);
        if (!string.IsNullOrEmpty(subdomain))
        {
            return await _tenantService.GetTenantIdBySubdomainAsync(subdomain);
        }
        
        // 2. Custom domain: tickets.company.com
        var domain = context.Request.Host.Value;
        if (await _tenantService.IsCustomDomainAsync(domain))
        {
            return await _tenantService.GetTenantIdByDomainAsync(domain);
        }
        
        // 3. Header: X-Tenant-Id (for API testing)
        var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(headerTenantId, out var parsedTenantId))
        {
            return parsedTenantId;
        }
        
        // 4. JWT token claim
        var user = context.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = user.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var jwtTenantId))
            {
                return jwtTenantId;
            }
        }
        
        throw new TenantResolutionException("Unable to resolve tenant");
    }
}
```

### Database Configuration
```csharp
// DbContext with automatic tenant filtering
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
        // Configure all entities to be tenant-aware
        ConfigureTenantAwareEntities(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfiguration(new VenueConfiguration());
        modelBuilder.ApplyConfiguration(new ActConfiguration());
    }
    
    private void ConfigureTenantAwareEntities(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Add tenant filter
                entityType.AddQueryFilter("TenantFilter", 
                    $"tenant_id = '{_currentTenantId}'");
                
                // Ensure tenant isolation in saves
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var tenantProperty = Expression.Property(parameter, "TenantId");
                var tenantValue = Expression.Constant(_currentTenantId);
                var equals = Expression.Equal(tenantProperty, tenantValue);
                var lambda = Expression.Lambda(equals, parameter);
                
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Ensure all new entities have current tenant ID
        var tenantAwareEntries = ChangeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Added);
            
        foreach (var entry in tenantAwareEntries)
        {
            if (entry.Entity.TenantId == Guid.Empty)
            {
                entry.Entity.TenantId = _currentTenantId;
            }
        }
        
        // Verify tenant ownership for updates/deletes
        var modifiedEntries = ChangeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);
            
        foreach (var entry in modifiedEntries)
        {
            if (!entry.Entity.BelongsToTenant(_currentTenantId))
            {
                throw new UnauthorizedAccessException(
                    $"Entity {entry.Entity.Id} does not belong to tenant {_currentTenantId}");
            }
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

## Security Considerations

### Always Filter by Tenant
```csharp
// ✅ Correct - Filtered query
var venues = await _context.Venues
    .Where(v => v.TenantId == currentTenantId)
    .ToListAsync();

// ❌ Wrong - No tenant filter
var venues = await _context.Venues.ToListAsync();
```

### Tenant Ownership Validation
```csharp
public async Task<Venue> GetVenueAsync(Guid venueId, Guid tenantId)
{
    var venue = await _context.Venues
        .FirstOrDefaultAsync(v => v.Id == venueId && v.TenantId == tenantId);
        
    if (venue == null)
        throw new VenueNotFoundException(venueId);
        
    return venue;
}
```

### Data Sanitization
- Never expose tenant_id in API responses unless explicitly needed
- Log tenant_id with all operations for audit trails
- Implement tenant-level rate limiting
- Monitor for cross-tenant data access attempts

## Migration Considerations

### Zero-Downtime Migrations
```sql
-- 1. Add new column nullable
ALTER TABLE Venues ADD COLUMN new_field nvarchar(100) NULL;

-- 2. Backfill data with tenant context
UPDATE Venues SET new_field = 'default_value' 
WHERE tenant_id IN (SELECT DISTINCT tenant_id FROM Venues);

-- 3. Make column required
ALTER TABLE Venues ALTER COLUMN new_field nvarchar(100) NOT NULL;
```

### Cross-Tenant Schema Changes
- Test migrations across all tenant scenarios
- Use feature flags for gradual rollout
- Monitor migration performance per tenant
- Plan rollback strategies

## Performance Optimization

### Indexing Strategy
```sql
-- Composite indexes for tenant + entity
CREATE INDEX IX_Venues_TenantId_Name ON Venues (tenant_id, name);
CREATE INDEX IX_Acts_TenantId_VenueId ON Acts (tenant_id, venue_id);
```

### Query Optimization
- Always include tenant_id in WHERE clauses
- Use covering indexes for tenant-specific queries
- Consider partitioning by tenant_id for large datasets
- Implement query result caching per tenant

## Testing Multi-Tenancy

### Unit Tests
```csharp
[Test]
public async Task GetVenue_WithDifferentTenant_ReturnsNull()
{
    // Arrange
    var tenant1Id = Guid.NewGuid();
    var tenant2Id = Guid.NewGuid();
    var venue = CreateVenueForTenant(tenant1Id);
    
    // Act
    var result = await _repository.GetVenueAsync(venue.Id, tenant2Id);
    
    // Assert
    Assert.That(result, Is.Null);
}
```

### Integration Tests
```csharp
[Test]
public async Task ApiEndpoints_RequireTenantContext()
{
    // Test without tenant - should return 401
    var response = await _client.GetAsync("/api/venues");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    
    // Test with tenant - should work
    _client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());
    response = await _client.GetAsync("/api/venues");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}
```

This multi-tenancy implementation ensures data isolation, security, and scalability while maintaining the simplicity of a shared database architecture.