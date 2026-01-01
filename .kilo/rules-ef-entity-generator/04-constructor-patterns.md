# Constructor Patterns

## For Entities with Required Navigation Properties

Entities that have required navigation properties (non-nullable relationships) must have:

1. **Public Constructor**: Accepts navigation entities as parameters and initializes both the navigation property and its foreign key
2. **Private Parameterless Constructor**: For Entity Framework Core to use during materialization

```csharp
public class Show : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Show"/> class.
    /// </summary>
    /// <param name="act">The Act that is performing in this show.</param>
    /// <param name="venue">The Venue where this show is held.</param>
    /// <exception cref="ArgumentNullException">Thrown when act or venue is null.</exception>
    public Show(Act act, Venue venue)
    {
        ArgumentNullException.ThrowIfNull(act);
        ArgumentNullException.ThrowIfNull(venue);

        Act = act;
        ActId = act.Id;
        Venue = venue;
        VenueId = venue.Id;
    }

    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Show()
    {
        Act = null!;
        Venue = null!;
    }

    /// <summary>
    /// Gets or sets the start time of the show in UTC.
    /// </summary>
    public required DateTime StartTime { get; set; }

    /// <summary>
    /// Gets the foreign key for the Act performing in this show.
    /// </summary>
    public int ActId { get; private set; }

    /// <summary>
    /// Gets the Act that is performing in this show.
    /// </summary>
    public Act Act { get; private set; }

    /// <summary>
    /// Gets the foreign key for the Venue where this show is held.
    /// </summary>
    public int VenueId { get; private set; }

    /// <summary>
    /// Gets the Venue where this show is held.
    /// </summary>
    public Venue Venue { get; private set; }
}
```

## Key Constructor Rules

- Constructor parameters should ONLY be used for navigation properties, not value properties
- Value properties should use the `required` keyword instead
- Public constructor validates navigation properties are not null using `ArgumentNullException.ThrowIfNull()`
- Public constructor sets both the navigation property and its foreign key from the passed entity's Id
- Private parameterless constructor initializes navigation properties with `null!` to suppress compiler warnings
- Foreign keys and navigation properties have `private set` to enforce initialization through constructor
