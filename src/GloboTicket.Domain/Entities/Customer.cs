namespace GloboTicket.Domain.Entities;

/// <summary>
/// Represents a customer within a tenant.
/// </summary>
public class Customer : MultiTenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier (GUID) for this customer.
    /// </summary>
    public Guid CustomerGuid { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
