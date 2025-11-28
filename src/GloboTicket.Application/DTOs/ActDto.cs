namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object representing an act.
/// Used for read operations and API responses.
/// </summary>
public class ActDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the act.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this act.
    /// </summary>
    public Guid ActGuid { get; set; }

    /// <summary>
    /// Gets or sets the name of the act.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the act was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the act was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}