namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a performing act (artist, band, performer) in the ticketing system.
/// </summary>
public class Act : MultiTenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this act.
    /// </summary>
    public Guid ActGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the act.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}