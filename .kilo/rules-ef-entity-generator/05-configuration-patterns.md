# EF Core Configuration Class Patterns

For each entity, create a corresponding configuration class following these exact patterns.

## Configuration Class Structure

```csharp
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the {EntityName} entity.
/// Defines table structure, constraints, indexes, and relationships.
/// </summary>
public class {EntityName}Configuration : IEntityTypeConfiguration<{EntityName}>
{
    /// <summary>
    /// Configures the {EntityName} entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<{EntityName}> builder)
    {
        // Configuration in exact order...
    }
}
```

## Configuration Order (Critical!)

Follow this EXACT order:

### 1. Table Name
```csharp
builder.ToTable("Shows");  // Plural, PascalCase
```

### 2. Primary Key
```csharp
builder.HasKey(s => s.Id);
builder.Property(s => s.Id).ValueGeneratedOnAdd();
```

### 3. Composite Alternate Key (MultiTenantEntity ONLY)
```csharp
builder.HasAlternateKey(e => new { e.TenantId, e.EntityGuid });
```

### 4. Indexes
```csharp
builder.HasIndex(e => e.EntityGuid);  // For MultiTenantEntity
```

### 5. Property Configurations
```csharp
builder.Property(s => s.StartTime).IsRequired();
builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
```

### 6. Foreign Key Relationships
```csharp
builder.HasOne(s => s.Venue)
    .WithMany(v => v.Shows)  // Always specify the collection on the parent
    .HasForeignKey(s => s.VenueId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired();
```

### 7. Inherited Property Configurations
```csharp
builder.Property(s => s.CreatedAt).IsRequired();
builder.Property(s => s.UpdatedAt).IsRequired(false);
```

## Child Entity Configuration Example

For entities inheriting from `Entity` (like Show):

```csharp
public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        // 1. Table name
        builder.ToTable("Shows");

        // 2. Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        // 3. Property configurations
        builder.Property(s => s.StartTime).IsRequired();

        // 4. Foreign key relationships
        builder.HasOne(s => s.Venue)
            .WithMany(v => v.Shows)  // Collection on Venue
            .HasForeignKey(s => s.VenueId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(s => s.Act)
            .WithMany(a => a.Shows)  // Collection on Act
            .HasForeignKey(s => s.ActId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // 5. Inherited property configurations
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired(false);
    }
}
```

## MultiTenantEntity Configuration Example

For entities inheriting from `MultiTenantEntity`:

```csharp
public class ActConfiguration : IEntityTypeConfiguration<Act>
{
    public void Configure(EntityTypeBuilder<Act> builder)
    {
        // 1. Table name
        builder.ToTable("Acts");

        // 2. Primary key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        // 3. Composite alternate key for multi-tenant uniqueness
        builder.HasAlternateKey(a => new { a.TenantId, a.ActGuid });

        // 4. Index on ActGuid for queries
        builder.HasIndex(a => a.ActGuid);

        // 5. Property configurations
        builder.Property(a => a.ActGuid).IsRequired();
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);

        // 6. Foreign key relationship to Tenant with cascade delete
        builder.HasOne(a => a.Tenant)
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // 7. Inherited property configurations
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired(false);
    }
}
```

## Relationship Configuration Pattern

**CRITICAL: When creating a child entity with a relationship to a parent entity, you MUST:**

1. **Add the collection navigation property to the parent entity**
   - Example: When creating `Show` that belongs to `Venue`, add `public ICollection<Show> Shows { get; set; } = new List<Show>();` to `Venue`
   - Use XML documentation: `/// <summary>The collection of Shows held at this venue.</summary>`

2. **Configure the relationship on the many (child) side using HasOne().WithMany(parent => parent.Collection).HasForeignKey()**
   - Example in `ShowConfiguration`:
   ```csharp
   builder.HasOne(s => s.Venue)
       .WithMany(v => v.Shows)  // Reference the collection property you added
       .HasForeignKey(s => s.VenueId)
       .OnDelete(DeleteBehavior.Cascade)
       .IsRequired();
   ```

3. **Never use empty `WithMany()`** - This creates a "shadow" relationship without proper navigation, making the model harder to use and query.

**Workflow:**
1. Read the parent entity file (e.g., `Venue.cs`)
2. Add the collection navigation property with proper XML documentation
3. Write the parent entity file back
4. Create the child entity with its navigation property to the parent
5. Create the child entity configuration using `WithMany(parent => parent.Collection)`

## Key Configuration Rules

- **Always** configure inherited properties (CreatedAt, UpdatedAt)
- **Always** specify `HasMaxLength()` for string properties
- **Always** use `DeleteBehavior.Cascade` for parent-child relationships
- **Always** use descriptive comments for each section
- **Always** follow the exact order specified above
- **Always** add collection navigation properties to parent entities when creating new child entities
- **Always** use `WithMany(parent => parent.Collection)` pattern in relationship configuration on the many side
- **Never** skip property configuration - be explicit about all constraints
- **Never** use data annotations - only Fluent API
- **Never** use empty `WithMany()` - always reference the collection property on the parent
