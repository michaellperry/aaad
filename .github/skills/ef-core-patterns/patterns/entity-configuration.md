# Entity Configuration

Use Fluent API over data annotations; configure entities in separate classes.

```csharp
public class Venue : MultiTenantEntity
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<Act> _acts = new();
    public IReadOnlyCollection<Act> Acts => _acts.AsReadOnly();
    
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

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.Property(v => v.IsActive).HasDefaultValue(true);
        builder.Property(v => v.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(v => v.TenantId).IsRequired();
        
        builder.HasIndex(v => v.TenantId);
        builder.HasIndex(v => new { v.TenantId, v.Name });
        
        builder.OwnsOne(v => v.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Address_Street").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("Address_City").HasMaxLength(100);
        });
        
        builder.HasMany<Act>().WithOne(a => a.Venue).HasForeignKey(a => a.VenueId).OnDelete(DeleteBehavior.Restrict);
    }
}
```
