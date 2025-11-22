namespace GloboTicket.Domain.Entities;

/// <summary>
/// Abstract base class for all domain entities.
/// Provides common properties for entity identification and auditing.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets or sets the primary key identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was last updated.
    /// Null if the entity has never been updated after creation.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}