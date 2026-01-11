# Entity Structure

ITenantEntity interface and MultiTenantEntity base class for tenant-aware entities.

```csharp
public interface ITenantEntity
{
    Guid TenantId { get; }
    bool BelongsToTenant(Guid tenantId);
}

public abstract class MultiTenantEntity : Entity<Guid>, ITenantEntity
{
    public Guid TenantId { get; protected set; }
    
    public bool BelongsToTenant(Guid tenantId) => TenantId == tenantId;
}

public class Venue : MultiTenantEntity
{
    public string Name { get; private set; }
    public string Address { get; private set; }
    
    private Venue() { }
    
    public Venue(string name, string address, Guid tenantId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        TenantId = tenantId;
    }
}
```

All domain entities inherit from MultiTenantEntity to ensure tenant isolation.
