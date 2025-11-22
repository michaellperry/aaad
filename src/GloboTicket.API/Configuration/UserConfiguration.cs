namespace GloboTicket.API.Configuration;

/// <summary>
/// Configuration model for users defined in appsettings.json.
/// Maps to the Users section and associates each user with a tenant.
/// </summary>
public class UserConfiguration
{
    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID associated with this user.
    /// </summary>
    public int TenantId { get; set; }
}