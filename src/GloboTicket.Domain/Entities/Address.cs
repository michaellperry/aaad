namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a postal address as a complex type.
/// Complex types in EF Core are value objects that don't have their own identity
/// and are stored as part of the owning entity.
/// </summary>
public class Address
{
    /// <summary>
    /// Gets or sets the first line of the street address.
    /// </summary>
    public required string StreetLine1 { get; set; }

    /// <summary>
    /// Gets or sets the second line of the street address (optional).
    /// </summary>
    public string? StreetLine2 { get; set; }

    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public required string StateOrProvince { get; set; }

    /// <summary>
    /// Gets or sets the postal or zip code.
    /// </summary>
    public required string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public required string Country { get; set; }
}

