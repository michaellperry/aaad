# Optimistic Concurrency

Use row version (timestamp) tokens to detect concurrent updates.

```csharp
public class Venue
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public byte[] RowVersion { get; private set; }
    
    private Venue() { }
    
    public void UpdateName(string newName)
    {
        Name = newName;
    }
}

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.Property(v => v.RowVersion).IsRowVersion();
    }
}

public async Task<bool> UpdateVenueAsync(Guid venueId, string newName, byte[] rowVersion)
{
    try 
    {
        var venue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venueId);
        if (venue == null) return false;
        
        venue.UpdateName(newName);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateConcurrencyException)
    {
        return false;
    }
}
```

Catch `DbUpdateConcurrencyException` to handle conflicts.
