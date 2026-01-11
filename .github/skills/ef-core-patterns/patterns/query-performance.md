# Query Performance

Use `AsNoTracking` for read-only queries; avoid N+1 with `Include` or split queries.

```csharp
// Read-only with AsNoTracking
public async Task<IEnumerable<VenueDto>> GetActiveVenuesAsync(Guid tenantId)
{
    return await _context.Venues
        .Where(v => v.TenantId == tenantId && v.IsActive)
        .AsNoTracking()
        .OrderBy(v => v.Name)
        .Select(v => new VenueDto { Id = v.Id, Name = v.Name, Address = $"{v.Address.Street}, {v.Address.City}" })
        .ToListAsync();
}

// Tracking for updates
public async Task<Venue> GetVenueForEditAsync(Guid venueId, Guid tenantId)
{
    return await _context.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.TenantId == tenantId);
}

// Avoid N+1: use Include
var venues = await _context.Venues
    .Include(v => v.Acts.Where(a => a.EventDate >= DateTime.UtcNow))
    .Where(v => v.TenantId == tenantId && v.IsActive)
    .ToListAsync();

// Split query for large collections
var venues = await _context.Venues
    .AsSplitQuery()
    .Include(v => v.Acts)
    .Where(v => v.TenantId == tenantId && v.IsActive)
    .ToListAsync();
```
