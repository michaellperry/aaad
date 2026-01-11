# Global Query Filters

Automatically filter by tenant ID for all queries on multi-tenant entities.

```csharp
public class GloboTicketDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

Use `IgnoreQueryFilters()` to bypass when needed (admin scenarios).
