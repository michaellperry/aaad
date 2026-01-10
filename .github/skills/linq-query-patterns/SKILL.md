---
name: linq-query-patterns
description: Essential LINQ query syntax patterns and separation of concerns for data access. For detailed examples and advanced patterns, see references section.
---

# LINQ Query Patterns

Essential patterns for writing consistent, testable LINQ queries following project conventions.

## Core Principles

### 1. Separation of Concerns
The definition of a query (specification) must be separate from its execution.

```csharp
// ✅ Good - Separate specification
var actSpec =
    from act in context.Acts.AsNoTracking()
    where act.Id == id
    select new ActDto
    {
        Id = act.Id,
        Name = act.Name,
        EventDate = act.EventDate,
        VenueName = act.Venue.Name
    };

return await actSpec.FirstOrDefaultAsync(cancellationToken);

// ❌ Bad - Inline query
return await context.Acts
    .Where(a => a.Id == id)
    .Select(a => new ActDto { ... })
    .FirstOrDefaultAsync();
```

### 2. Query Syntax Required
All queries must use LINQ Query Syntax, not Method Syntax.

```csharp
// ✅ Good - Query syntax
var venuesSpec =
    from venue in context.Venues.AsNoTracking()
    where venue.TenantId == tenantId && venue.IsActive
    select new VenueDto { Id = venue.Id, Name = venue.Name };

// ❌ Bad - Method syntax
var venues = context.Venues.AsNoTracking()
    .Where(v => v.TenantId == tenantId && v.IsActive)
    .Select(v => new VenueDto { Id = v.Id, Name = v.Name });
```

### 3. Async Execution
All database materialization must be asynchronous.

```csharp
// ✅ Good - Async execution
var venues = await venuesSpec.ToListAsync(cancellationToken);
var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);
var count = await venuesSpec.CountAsync(cancellationToken);

// ❌ Bad - Synchronous execution
var venues = venuesSpec.ToList();
var venue = venueSpec.FirstOrDefault();
```

### 4. Projection for Reads
Read operations must project directly into DTOs. Entities only for modifications.

```csharp
// ✅ Good - Direct projection for reads
var actSpec =
    from act in context.Acts.AsNoTracking()
    where act.Id == id
    select new ActDto
    {
        Id = act.Id,
        Name = act.Name,
        EventDate = act.EventDate
    };

// ✅ Good - Entity selection for modifications  
var actEntitySpec =
    from a in context.Acts
    where a.Id == id
    select a;

var act = await actEntitySpec.FirstOrDefaultAsync(cancellationToken);
if (act != null)
{
    act.UpdateName(dto.Name);
}
```

## Common Patterns

### Basic Read Operation
```csharp
public async Task<VenueDto?> GetVenueAsync(Guid id, CancellationToken cancellationToken)
{
    var venueSpec =
        from venue in _context.Venues.AsNoTracking()
        where venue.Id == id
        select new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Capacity = venue.Capacity,
            IsActive = venue.IsActive
        };

    return await venueSpec.FirstOrDefaultAsync(cancellationToken);
}
```

### Entity Modification
```csharp
public async Task UpdateVenueAsync(Guid id, UpdateVenueDto dto, CancellationToken cancellationToken)
{
    var venueSpec =
        from v in _context.Venues
        where v.Id == id
        select v;

    var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);
    if (venue != null)
    {
        venue.UpdateDetails(dto.Name, dto.Capacity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### Using Let Clauses
```csharp
var venueStatsSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    let activeActs = venue.Acts.Where(a => a.IsActive)
    let totalCapacity = activeActs.Sum(a => a.Capacity)
    select new VenueStatsDto
    {
        Id = venue.Id,
        Name = venue.Name,
        TotalCapacity = totalCapacity,
        ActiveActsCount = activeActs.Count()
    };
```

## References

For detailed implementations and advanced patterns:

- **[Read Operations](references/read-operations.md)** - Projection patterns and DTO mapping examples
- **[Modification Operations](references/modification-operations.md)** - Entity selection and update patterns
- **[Complex Queries](references/complex-queries.md)** - Joins, grouping, and advanced query patterns
- **[Performance Optimization](references/performance.md)** - AsNoTracking, query splitting, and optimization techniques
- **[Testing Patterns](references/testing.md)** - Unit testing LINQ specifications and mock strategies