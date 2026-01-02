# Test Data Helper Patterns

Comprehensive guide to creating `Given` helper methods for test data.

## Navigation Property Initialization

**CRITICAL**: Always initialize parent navigation properties, never foreign keys. This ensures EF Core properly tracks relationships and applies query filters correctly.

### ❌ WRONG - Setting Foreign Keys

```csharp
private Show GivenShow(int actId, int venueId)
{
    return new Show
    {
        ActId = actId,      // Don't set foreign keys
        VenueId = venueId,  // Don't set foreign keys
        TicketCount = 500
    };
}
```

**Problems:**
- EF Core doesn't track relationships properly
- Query filters may not work correctly
- Navigation properties remain null
- Harder to debug relationship issues

### ✅ CORRECT - Setting Navigation Properties

```csharp
private Show GivenShow(Act act, Venue venue, int ticketCount = 500)
{
    return new Show
    {
        Act = act,          // Set navigation properties
        Venue = venue,      // Set navigation properties
        TicketCount = ticketCount
    };
}
```

**Benefits:**
- EF Core properly tracks relationships
- Query filters work correctly
- Navigation properties are available
- Clear entity dependencies

## Required vs. Optional Parameters

### Rule: Parent Entities Required, Scalars Optional

- **Required parameters**: Parent navigation properties that are essential for the entity (no default value, non-nullable)
- **Optional parameters**: Scalar values that can have sensible defaults (with default values)

### Example Pattern

```csharp
private Show GivenShow(
    Act act,                              // Required - no default
    Venue venue,                          // Required - no default
    Guid? showGuid = null,                // Optional - has default
    int ticketCount = 500,                // Optional - has default
    DateTimeOffset? startTime = null)     // Optional - has default
{
    return new Show
    {
        ShowGuid = showGuid ?? Guid.NewGuid(),
        Act = act,
        Venue = venue,
        TicketCount = ticketCount,
        StartTime = startTime ?? DateTimeOffset.UtcNow.AddDays(7)
    };
}
```

### Why This Pattern?

**Compile-Time Safety:**
```csharp
// This won't compile - forces you to provide required dependencies
var show = GivenShow(); // ERROR: Missing required parameters

// This compiles - explicit dependencies
var venue = GivenVenue();
var act = GivenAct();
var show = GivenShow(act: act, venue: venue); // CORRECT
```

**Visual Clarity:**
```csharp
// Immediately clear what entities are needed
var show = GivenShow(act: act, venue: venue);

// vs. unclear where dependencies come from
var show = GivenShow(); // Where do act and venue come from?
```

## Complete Example

```csharp
public class ShowServiceTests
{
    // Root entities can have all defaults
    private Venue GivenVenue(
        Guid? venueGuid = null,
        string name = "Test Venue",
        int seatingCapacity = 1000,
        int tenantId = 1)
    {
        return new Venue
        {
            VenueGuid = venueGuid ?? Guid.NewGuid(),
            Name = name,
            SeatingCapacity = seatingCapacity,
            TenantId = tenantId,
            Description = "Test venue description"
        };
    }

    private Act GivenAct(
        Guid? actGuid = null,
        string name = "Test Act",
        int tenantId = 1)
    {
        return new Act
        {
            ActGuid = actGuid ?? Guid.NewGuid(),
            Name = name,
            TenantId = tenantId
        };
    }

    // Child entities require parent navigation properties
    private Show GivenShow(
        Act act,                          // Required parent
        Venue venue,                      // Required parent
        Guid? showGuid = null,            // Optional scalar
        int ticketCount = 500,            // Optional scalar
        DateTimeOffset? startTime = null) // Optional scalar
    {
        return new Show
        {
            ShowGuid = showGuid ?? Guid.NewGuid(),
            Act = act,                    // Navigation property, not ActId
            Venue = venue,                // Navigation property, not VenueId
            TicketCount = ticketCount,
            StartTime = startTime ?? DateTimeOffset.UtcNow.AddDays(7)
        };
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesShow()
    {
        // Given: Create parent entities first
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();
        
        // Then create child with navigation properties
        var show = GivenShow(act: act, venue: venue);
        
        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();
        
        // This ensures EF Core properly tracks relationships
        show.ActId.Should().Be(act.Id);
        show.VenueId.Should().Be(venue.Id);
    }
}
```

## Benefits Summary

1. **Visual Distinction**: Overridden parameters vs. defaults are visually distinct
2. **Simplicity**: No separate builder classes needed; helpers live in test class
3. **Flexibility**: Override only what matters for each test
4. **Explicit Dependencies**: Clear entity relationships in test code
5. **Compile-Time Safety**: Missing required parents cause compilation errors
6. **Better EF Core Tracking**: Navigation properties ensure proper relationship tracking
7. **Clearer Intent**: Reading test code immediately shows what entities are needed
8. **Prevents Bugs**: Can't accidentally create orphaned entities

## Anti-Patterns to Avoid

### ❌ Separate Builder Classes

```csharp
// Avoid separate builder classes
public class VenueBuilder
{
    private string _name = "Test Venue";
    // ... builder implementation
}

[Fact]
public void Test()
{
    var venue = VenueBuilder.Create()
        .WithName("Madison Square Garden")
        .Build();
}
```

**Why avoid?** Adds unnecessary complexity and doesn't provide the visual distinction that `Given` helper methods with default parameters offer.

### ❌ Setting Foreign Keys

```csharp
// Don't do this
private Show GivenShow(int actId, int venueId)
{
    return new Show { ActId = actId, VenueId = venueId };
}
```

**Why avoid?** EF Core doesn't track relationships properly, query filters may fail, navigation properties remain null.

### ❌ All Optional Parameters for Child Entities

```csharp
// Don't do this
private Show GivenShow(
    Act? act = null,      // Should be required
    Venue? venue = null)  // Should be required
{
    return new Show
    {
        Act = act ?? GivenAct(),
        Venue = venue ?? GivenVenue()
    };
}
```

**Why avoid?** Hides dependencies, makes test code unclear, loses compile-time safety.
