# Entity Class Construction Rules

## Base Structure

```csharp
namespace GloboTicket.Domain.Entities;

/// <summary>
/// [Clear, concise description of the entity's business purpose]
/// </summary>
public class EntityName : [Entity | MultiTenantEntity]
{
    // Properties defined here
}
```

## Property Patterns

### Required Properties
- Use `required` keyword: `public required string Name { get; set; }`
- Include XML doc comment explaining the property's purpose
- Provide default value in initializer if sensible: `= string.Empty;`

### Optional Properties
- Make nullable: `public string? Description { get; set; }`
- Include XML doc comment
- Default to `null` (implicit)

### Navigation Properties (Collections)
```csharp
/// <summary>
/// The collection of related entities.
/// </summary>
public ICollection<RelatedEntity> RelatedEntities { get; set; } = new List<RelatedEntity>();
```

**IMPORTANT: When creating a new relationship, ALWAYS add the collection navigation property to the parent (one) side:**
- If you're creating a child entity (many side), you MUST also add the collection property to the parent entity
- Example: When creating `Show` with a relationship to `Venue`, add `public ICollection<Show> Shows { get; set; } = new List<Show>();` to the `Venue` entity
- Use the `WithMany(parent => parent.Collection)` pattern in the configuration on the many side

### Navigation Properties (Single - Optional)
```csharp
/// <summary>
/// The parent entity.
/// </summary>
public ParentEntity? Parent { get; set; }

/// <summary>
/// Foreign key for the parent entity.
/// </summary>
public int? ParentId { get; set; }
```

### Navigation Properties (Single - Required)
```csharp
/// <summary>
/// The parent entity.
/// </summary>
public ParentEntity Parent { get; private set; }

/// <summary>
/// Foreign key for the parent entity.
/// </summary>
public int ParentId { get; private set; }
```

**Important Notes on Required Navigation Properties:**
- Required navigation properties should be non-nullable
- Both foreign key and navigation properties should have `private set`
- Use null assertion assignment (`= null!;`) in constructors to avoid compiler warnings
- Required navigation properties are initialized via constructor parameters
- A private parameterless constructor must be provided for Entity Framework Core

### Complex Types (Value Objects)
```csharp
/// <summary>
/// The billing address for this entity.
/// </summary>
public required Address BillingAddress { get; set; }

/// <summary>
/// Optional shipping address.
/// </summary>
public Address? ShippingAddress { get; set; }
```

### Geospatial Properties
```csharp
using NetTopologySuite.Geometries;

/// <summary>
/// Geographic location (latitude/longitude).
/// </summary>
public Point? Location { get; set; }
```

## Multi-Tenant Entity Additional Requirements

When inheriting from `MultiTenantEntity`, DO NOT manually add:
- `TenantId` property (inherited from base)
- `Tenant` navigation property (inherited from base)
- `Id`, `CreatedAt`, `UpdatedAt` (inherited through base chain)

These are automatically provided by the inheritance hierarchy.

## Common Property Types

- **Strings**: Default to `string.Empty` for required, `null` for optional
- **Decimals**: Use `decimal` for monetary values, prices
- **DateTimes**: Use `DateTime` (UTC assumed), nullable for optional dates
- **Booleans**: Default to `false` with clear semantic meaning
- **Enums**: Create strongly-typed enums in `Domain.Enums` namespace
- **GUIDs**: Use `Guid` type, typically optional except for unique identifiers
