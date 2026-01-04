namespace GloboTicket.Application.DTOs;

/// <summary>
/// Data transfer object for show capacity information.
/// </summary>
public class ShowCapacityDto
{
    /// <summary>
    /// Gets or sets the GUID of the show.
    /// </summary>
    public Guid ShowGuid { get; set; }

    /// <summary>
    /// Gets or sets the total ticket count for the show.
    /// </summary>
    public int TotalTickets { get; set; }

    /// <summary>
    /// Gets or sets the sum of ticket counts across all offers.
    /// </summary>
    public int AllocatedTickets { get; set; }

    /// <summary>
    /// Gets or sets the remaining tickets available for new offers.
    /// </summary>
    public int AvailableCapacity { get; set; }
}
