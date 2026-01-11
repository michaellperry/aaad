# Security Considerations

Always filter by tenant; validate ownership; sanitize data.

## Always Filter by Tenant
```csharp
// ✅ Correct
var venues = await _context.Venues.Where(v => v.TenantId == currentTenantId).ToListAsync();

// ❌ Wrong - no tenant filter
var venues = await _context.Venues.ToListAsync();
```

## Tenant Ownership Validation
```csharp
public async Task<Venue> GetVenueAsync(Guid venueId, Guid tenantId)
{
    var venue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venueId && v.TenantId == tenantId);
    if (venue == null) throw new VenueNotFoundException(venueId);
    return venue;
}
```

## Data Sanitization
- Never expose tenant_id in API responses unless explicitly needed
- Log tenant_id with all operations for audit trails
- Implement tenant-level rate limiting
- Monitor for cross-tenant data access attempts

Global query filters provide automatic protection; explicit validation adds defense in depth.
