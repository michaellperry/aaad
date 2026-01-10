# Performance Optimization

LINQ performance patterns and optimization techniques.

## Performance Guidelines

### 1. Use AsNoTracking() for Read Operations
Always use `AsNoTracking()` when projecting to DTOs or when entities won't be modified:

```csharp
// ✅ Good - Read-only queries
var venuesSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    select new VenueDto { ... };

// ❌ Bad - Unnecessary change tracking
var venuesSpec =
    from venue in _context.Venues  // Missing AsNoTracking()
    where venue.TenantId == tenantId
    select new VenueDto { ... };
```

### 2. Project Early and Select Only Required Data
Minimize data transfer by selecting only needed fields:

```csharp
// ✅ Good - Select only required fields
var venueOptionsSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId && venue.IsActive
    select new VenueOptionDto
    {
        Id = venue.Id,
        Name = venue.Name
    };

// ❌ Bad - Selecting unnecessary data
var venueOptionsSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId && venue.IsActive
    select new VenueDto
    {
        Id = venue.Id,
        Name = venue.Name,
        Description = venue.Description,  // Not needed for options
        Address = venue.Address,          // Expensive navigation
        Acts = venue.Acts.ToList()        // Very expensive
    };
```

### 3. Query Splitting for Complex Includes
Use query splitting when including multiple collections:

```csharp
// For complex includes, consider query splitting
var venueWithDataSpec =
    from venue in _context.Venues.AsNoTracking()
        .AsSplitQuery()  // Splits into multiple queries
    where venue.Id == id
    select new VenueDetailsDto
    {
        Id = venue.Id,
        Name = venue.Name,
        Acts = venue.Acts.Select(a => new ActDto { ... }),
        Reviews = venue.Reviews.Select(r => new ReviewDto { ... })
    };
```

## Index-Supporting Queries

### Always Include Tenant Filtering
Ensure all multi-tenant queries filter by TenantId first:

```csharp
// ✅ Good - Tenant filtering first
var venuesSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId  // Index on TenantId
        && venue.IsActive             // Then other filters
        && venue.Capacity >= minCapacity
    select new VenueDto { ... };
```

### Order By Indexed Columns
Use indexed columns for ordering:

```csharp
// ✅ Good - Order by indexed column
var venuesSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    orderby venue.CreatedAt desc  // Index on (TenantId, CreatedAt)
    select new VenueDto { ... };

// Consider this index: CREATE INDEX IX_Venues_TenantId_CreatedAt 
// ON Venues (TenantId, CreatedAt DESC)
```

## Avoiding Common Performance Issues

### 1. Avoid N+1 Queries
Don't execute queries inside loops:

```csharp
// ❌ Bad - N+1 query problem
var venues = await venuesSpec.ToListAsync();
foreach (var venue in venues)
{
    // This executes a query for each venue!
    var actCount = await (from act in _context.Acts
                         where act.VenueId == venue.Id
                         select act).CountAsync();
}

// ✅ Good - Single query with projection
var venuesWithCountsSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    select new VenueDto
    {
        Id = venue.Id,
        Name = venue.Name,
        ActCount = venue.Acts.Count()  // Calculated in single query
    };
```

### 2. Use Exists() Instead of Any() with Count
For existence checks, use efficient patterns:

```csharp
// ✅ Good - Use Any() for existence
var hasActiveActs = venue.Acts.Any(a => a.IsActive);

// ❌ Bad - Inefficient count check
var hasActiveActs = venue.Acts.Count(a => a.IsActive) > 0;
```

### 3. Pagination for Large Result Sets
Always implement pagination for potentially large result sets:

```csharp
public async Task<PaginatedResult<VenueDto>> GetVenuesPagedAsync(
    Guid tenantId, 
    int page, 
    int pageSize, 
    CancellationToken cancellationToken)
{
    var venuesSpec =
        from venue in _context.Venues.AsNoTracking()
        where venue.TenantId == tenantId
        orderby venue.Name
        select new VenueDto { ... };

    var totalCount = await venuesSpec.CountAsync(cancellationToken);
    
    var venues = await venuesSpec
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

    return new PaginatedResult<VenueDto>
    {
        Items = venues,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

## Monitoring Query Performance

### Enable Sensitive Data Logging (Development Only)
```csharp
// In development, enable detailed logging
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (Environment.IsDevelopment())
    {
        optionsBuilder
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
    }
}
```

### Use Query Tags for Tracking
```csharp
var venuesSpec =
    from venue in _context.Venues.AsNoTracking()
        .TagWith("GetActiveVenuesForDashboard")  // Appears in logs
    where venue.TenantId == tenantId && venue.IsActive
    select new VenueDto { ... };
```