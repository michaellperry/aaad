# Read Operations - Projection Patterns

Complete examples for read-only LINQ queries with projection patterns.

## Basic Read Examples

### Single Entity Lookup
```csharp
public async Task<ActDto?> GetActByIdAsync(Guid id, CancellationToken cancellationToken)
{
    var actSpec =
        from act in _context.Acts.AsNoTracking()
        where act.Id == id
        select new ActDto
        {
            Id = act.Id,
            Name = act.Name,
            EventDate = act.EventDate,
            VenueName = act.Venue.Name,
            Description = act.Description,
            IsActive = act.IsActive
        };

    return await actSpec.FirstOrDefaultAsync(cancellationToken);
}
```

### Multiple Entity List
```csharp
public async Task<IEnumerable<VenueDto>> GetActiveVenuesAsync(Guid tenantId, CancellationToken cancellationToken)
{
    var venuesSpec =
        from venue in _context.Venues.AsNoTracking()
        where venue.TenantId == tenantId && venue.IsActive
        orderby venue.Name
        select new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Capacity = venue.Capacity,
            City = venue.Address.City,
            IsActive = venue.IsActive
        };

    return await venuesSpec.ToListAsync(cancellationToken);
}
```

## Dynamic Filtering

### Multiple Optional Filters
```csharp
public async Task<IEnumerable<VenueDto>> SearchVenuesAsync(
    Guid tenantId,
    string? searchTerm = null,
    string? cityFilter = null,
    bool activeOnly = false,
    CancellationToken cancellationToken = default)
{
    var searchSpec =
        from venue in _context.Venues.AsNoTracking()
        where venue.TenantId == tenantId
        let matchesName = string.IsNullOrEmpty(searchTerm) || venue.Name.Contains(searchTerm)
        let matchesCity = string.IsNullOrEmpty(cityFilter) || venue.Address.City == cityFilter
        let matchesActive = !activeOnly || venue.IsActive
        where matchesName && matchesCity && matchesActive
        orderby venue.Name
        select new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            City = venue.Address.City,
            Capacity = venue.Capacity,
            IsActive = venue.IsActive,
            CreatedAt = venue.CreatedAt
        };

    return await searchSpec.ToListAsync(cancellationToken);
}
```

### Date Range Filtering
```csharp
var eventsSpec =
    from act in _context.Acts.AsNoTracking()
    where act.TenantId == tenantId
    let withinDateRange = startDate == null || endDate == null || 
                         (act.EventDate >= startDate && act.EventDate <= endDate)
    where withinDateRange
    select new ActDto
    {
        Id = act.Id,
        Name = act.Name,
        EventDate = act.EventDate,
        VenueName = act.Venue.Name
    };
```

## Projection Optimizations

### Selecting Only Required Fields
```csharp
// For dropdown/listing scenarios
var venueOptionsSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId && venue.IsActive
    select new VenueOptionDto
    {
        Id = venue.Id,
        Name = venue.Name
    };
```

### Including Related Data
```csharp
var actWithVenueSpec =
    from act in _context.Acts.AsNoTracking()
    where act.Id == id
    select new ActDetailsDto
    {
        Id = act.Id,
        Name = act.Name,
        EventDate = act.EventDate,
        Venue = new VenueDto
        {
            Id = act.Venue.Id,
            Name = act.Venue.Name,
            Capacity = act.Venue.Capacity
        },
        Shows = act.Shows.Select(s => new ShowDto
        {
            Id = s.Id,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList()
    };
```