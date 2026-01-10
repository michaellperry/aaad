# Modification Operations - Entity Selection

Complete examples for entity selection and modification patterns.

## Entity Update Patterns

### Basic Entity Update
```csharp
public async Task UpdateActAsync(Guid id, UpdateActDto dto, CancellationToken cancellationToken)
{
    var actSpec =
        from a in _context.Acts
        where a.Id == id
        select a;

    var act = await actSpec.FirstOrDefaultAsync(cancellationToken);
    if (act != null)
    {
        act.UpdateName(dto.Name);
        act.UpdateEventDate(dto.EventDate);
        act.UpdateDescription(dto.Description);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### Conditional Update with Business Logic
```csharp
public async Task<bool> DeactivateVenueAsync(Guid id, CancellationToken cancellationToken)
{
    var venueSpec =
        from v in _context.Venues
        where v.Id == id
        select v;

    var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);
    if (venue != null)
    {
        // Business rule: Can't deactivate if there are future events
        var hasFutureEvents = venue.Acts.Any(a => a.EventDate > DateTime.UtcNow);
        if (!hasFutureEvents)
        {
            venue.Deactivate();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
    
    return false;
}
```

## Entity Creation Patterns

### Creating with Related Data
```csharp
public async Task<Guid> CreateActAsync(CreateActDto dto, Guid tenantId, CancellationToken cancellationToken)
{
    // First verify venue exists and belongs to tenant
    var venueSpec =
        from v in _context.Venues
        where v.Id == dto.VenueId && v.TenantId == tenantId
        select v;

    var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);
    if (venue == null)
    {
        throw new InvalidOperationException("Venue not found or access denied");
    }

    var act = Act.Create(dto.Name, dto.EventDate, venue);
    _context.Acts.Add(act);
    
    await _context.SaveChangesAsync(cancellationToken);
    return act.Id;
}
```

## Entity Deletion Patterns

### Soft Delete
```csharp
public async Task<bool> DeleteActAsync(Guid id, CancellationToken cancellationToken)
{
    var actSpec =
        from a in _context.Acts
        where a.Id == id && a.IsActive
        select a;

    var act = await actSpec.FirstOrDefaultAsync(cancellationToken);
    if (act != null)
    {
        act.Delete(); // Marks as inactive
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    return false;
}
```

### Hard Delete with Cascade Check
```csharp
public async Task<bool> HardDeleteVenueAsync(Guid id, CancellationToken cancellationToken)
{
    var venueSpec =
        from v in _context.Venues
        where v.Id == id
        select v;

    var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);
    if (venue != null)
    {
        // Check for dependent entities
        if (venue.Acts.Any())
        {
            throw new InvalidOperationException("Cannot delete venue with existing acts");
        }
        
        _context.Venues.Remove(venue);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    return false;
}
```

## Bulk Operations

### Bulk Update
```csharp
public async Task DeactivateExpiredActsAsync(CancellationToken cancellationToken)
{
    var expiredActsSpec =
        from act in _context.Acts
        where act.EventDate < DateTime.UtcNow && act.IsActive
        select act;

    var expiredActs = await expiredActsSpec.ToListAsync(cancellationToken);
    
    foreach (var act in expiredActs)
    {
        act.Deactivate();
    }
    
    await _context.SaveChangesAsync(cancellationToken);
}
```