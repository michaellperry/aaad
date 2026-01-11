# Database Configuration

DbContext with automatic tenant filtering and SaveChanges validation.

```csharp
public class GloboTicketDbContext : DbContext
{
    private readonly Guid _currentTenantId;
    
    public GloboTicketDbContext(DbContextOptions<GloboTicketDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _currentTenantId = tenantContext.TenantId;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
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
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set tenant ID for new entities
        var tenantAwareEntries = ChangeTracker.Entries<ITenantEntity>().Where(e => e.State == EntityState.Added);
        foreach (var entry in tenantAwareEntries)
        {
            if (entry.Entity.TenantId == Guid.Empty)
                entry.Entity.TenantId = _currentTenantId;
        }
        
        // Verify tenant ownership for updates/deletes
        var modifiedEntries = ChangeTracker.Entries<ITenantEntity>().Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);
        foreach (var entry in modifiedEntries)
        {
            if (!entry.Entity.BelongsToTenant(_currentTenantId))
                throw new UnauthorizedAccessException($"Entity {entry.Entity.Id} does not belong to tenant {_currentTenantId}");
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

Global query filters automatically applied to all ITenantEntity types.
