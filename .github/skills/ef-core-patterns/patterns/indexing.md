# Indexing Strategy

Create indexes for frequent query patterns: tenant filters, uniqueness, composite searches.

```csharp
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(v => v.Id);
        
        builder.HasIndex(v => new { v.TenantId, v.Name })
            .IsUnique()
            .HasDatabaseName("IX_Venues_TenantId_Name_Unique");
        
        builder.HasIndex(v => v.TenantId)
            .HasDatabaseName("IX_Venues_TenantId");
        
        builder.HasIndex(v => new { v.TenantId, v.IsActive })
            .HasDatabaseName("IX_Venues_TenantId_IsActive");
        
        builder.HasIndex(v => new { v.TenantId, v.IsActive, v.CreatedAt })
            .HasDatabaseName("IX_Venues_TenantId_IsActive_CreatedAt");
    }
}
```

Always index tenant ID; add composite indexes for common filter combinations.
