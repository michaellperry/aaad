using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new show.
/// Contains validation rules for show creation.
/// </summary>
public class CreateShowDto
{
    /// <summary>
    /// Gets or sets the unique identifier for this show.
    /// </summary>
    [Required]
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the venue where the show will be held.
    /// </summary>
    [Required]
    public Guid VenueGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the act performing at this show.
    /// </summary>
    [Required]
    public Guid ActGuid { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the show will occur.
    /// </summary>
    [Required]
    public DateTimeOffset Date { get; set; }
}