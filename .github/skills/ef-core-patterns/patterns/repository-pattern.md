# Repository Pattern

Encapsulate data access behind repositories; keep SaveChanges in repositories or unit of work.

```csharp
public class VenueRepository : IVenueRepository
{
    private readonly GloboTicketDbContext _context;
    
    public VenueRepository(GloboTicketDbContext context)
    {
        _context = context;
    }
    
    public async Task<Venue> AddAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        _context.Venues.Add(venue);
        await _context.SaveChangesAsync(cancellationToken);
        return venue;
    }
    
    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Venues.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }
    
    public async Task UpdateAsync(Venue venue, CancellationToken cancellationToken = default)
    {
        _context.Venues.Update(venue);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var venue = await GetByIdAsync(id, cancellationToken);
        if (venue != null)
        {
            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
```

Alternative: inject DbContext into services directly (no repository layer).
