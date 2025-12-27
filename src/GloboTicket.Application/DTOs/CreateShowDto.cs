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
    [NotEmptyGuid(ErrorMessage = "ShowGuid cannot be empty")]
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the venue where the show will be held.
    /// </summary>
    [Required]
    [NotEmptyGuid(ErrorMessage = "VenueGuid cannot be empty")]
    public Guid VenueGuid { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets available for this show.
    /// Must be positive and not exceed venue capacity.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Ticket count must be at least 1")]
    public int TicketCount { get; set; }

    /// <summary>
    /// Gets or sets the start time of the show with timezone offset.
    /// Must be in the future.
    /// </summary>
    [Required]
    [NotDefault(ErrorMessage = "StartTime cannot be default")]
    public DateTimeOffset StartTime { get; set; }
}

/// <summary>
/// Validation attribute that ensures a Guid is not empty.
/// </summary>
public class NotEmptyGuidAttribute : ValidationAttribute
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>True if the value is valid; otherwise, false.</returns>
    public override bool IsValid(object? value)
    {
        if (value is Guid guid)
        {
            return guid != Guid.Empty;
        }
        return false;
    }
}

/// <summary>
/// Validation attribute that ensures a DateTimeOffset is not default.
/// </summary>
public class NotDefaultAttribute : ValidationAttribute
{
    /// <summary>
    /// Determines whether the specified value is valid.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns>True if the value is valid; otherwise, false.</returns>
    public override bool IsValid(object? value)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset != default;
        }
        return false;
    }
}
