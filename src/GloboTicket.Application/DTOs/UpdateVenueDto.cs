using System.ComponentModel.DataAnnotations;

namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing venue.
/// Contains validation rules for venue updates.
/// </summary>
public class UpdateVenueDto
{
    /// <summary>
    /// Gets or sets the name of the venue.
    /// Maximum length: 200 characters.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical address of the venue.
    /// Maximum length: 500 characters.
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the latitude coordinate of the venue location.
    /// Valid range: -90 to 90.
    /// </summary>
    [Range(-90, 90)]
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the venue location.
    /// Valid range: -180 to 180.
    /// </summary>
    [Range(-180, 180)]
    public double? Longitude { get; set; }

    /// <summary>
    /// Gets or sets the maximum seating capacity of the venue.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int SeatingCapacity { get; set; }

    /// <summary>
    /// Gets or sets a description of the venue.
    /// Maximum length: 2000 characters.
    /// </summary>
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}
