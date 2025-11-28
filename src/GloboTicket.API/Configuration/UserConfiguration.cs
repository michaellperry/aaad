namespace GloboTicket.API.Configuration;

/// <summary>
/// Configuration model for users defined in appsettings.json.
/// Maps to the Users section and associates each user with a tenant.
/// 
/// Each user is mapped to a specific tenant, which determines their data context
/// within the environment's database. Multiple tenants can exist in the same database,
/// providing complete data isolation.
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
    /// Gets or sets the tenant identifier (natural key) associated with this user.
    /// The tenant identifier is a human-readable string that determines which tenant
    /// (data context) the user accesses within the environment's database.
    /// Examples: "production", "smoke-test", "integration-test"
    /// </summary>
    public required string TenantIdentifier { get; set; }
}