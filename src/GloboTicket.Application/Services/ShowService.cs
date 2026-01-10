using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Application.MultiTenancy;
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Application.Services;

/// <summary>
/// Service implementation for managing show operations.
/// Handles show-related business logic and data access with tenant isolation through Venue relationship.
/// </summary>
public class ShowService : IShowService
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for accessing current tenant ID.</param>
    public ShowService(DbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<ShowDto?> GetByGuidAsync(Guid showGuid, CancellationToken cancellationToken = default)
    {
        // Use IgnoreQueryFilters to bypass global filter, then manually filter by Venue.TenantId
        // This ensures proper tenant isolation through the Venue relationship
        var show = await _dbContext.Set<Show>()
            .IgnoreQueryFilters()
            .Include(s => s.Act)
            .Include(s => s.Venue)
            .Where(s => s.ShowGuid == showGuid)
            .Where(s => _tenantContext.CurrentTenantId == null || s.Venue.TenantId == _tenantContext.CurrentTenantId)
            .FirstOrDefaultAsync(cancellationToken);

        return show == null ? null : MapToDto(show);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ShowDto>> GetShowsByActGuidAsync(Guid actGuid, CancellationToken cancellationToken = default)
    {
        // Validate act exists (with tenant filtering if context is set)
        var actExists = await _dbContext.Set<Act>()
            .AnyAsync(a => a.ActGuid == actGuid, cancellationToken);

        if (!actExists)
        {
            throw new KeyNotFoundException($"Act with GUID {actGuid} not found");
        }

        // Query shows directly by Act.ActGuid to support multi-tenant acts
        // Use IgnoreQueryFilters to bypass Act tenant filtering, but manually filter by Venue tenant
        var shows = await _dbContext.Set<Show>()
            .IgnoreQueryFilters()
            .Include(s => s.Act)
            .Include(s => s.Venue)
            .Where(s => s.Act.ActGuid == actGuid)
            .Where(s => _tenantContext.CurrentTenantId == null || s.Venue.TenantId == _tenantContext.CurrentTenantId)
            .ToListAsync(cancellationToken);

        return shows.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<ShowDto> CreateAsync(Guid actGuid, CreateShowDto dto, CancellationToken cancellationToken = default)
    {
        // Validate act exists (tenant-filtered)
        var act = await _dbContext.Set<Act>()
            .FirstOrDefaultAsync(a => a.ActGuid == actGuid, cancellationToken);

        if (act == null)
        {
            throw new KeyNotFoundException($"Act with GUID {actGuid} not found");
        }

        // Validate venue exists (tenant-filtered)
        var venue = await _dbContext.Set<Venue>()
            .FirstOrDefaultAsync(v => v.VenueGuid == dto.VenueGuid, cancellationToken);

        if (venue == null)
        {
            throw new KeyNotFoundException($"Venue with GUID {dto.VenueGuid} not found");
        }

        // Validate ticket count <= venue capacity
        if (dto.TicketCount > venue.SeatingCapacity)
        {
            throw new ArgumentException($"Ticket count cannot exceed venue capacity of {venue.SeatingCapacity}", nameof(dto.TicketCount));
        }

        // Validate start time is in the future
        if (dto.StartTime <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("Start time must be in the future", nameof(dto.StartTime));
        }

        // Create show entity
        var show = new Show(act, venue)
        {
            ShowGuid = dto.ShowGuid,
            TicketCount = dto.TicketCount,
            StartTime = dto.StartTime
        };

        _dbContext.Set<Show>().Add(show);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(show);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid showGuid, CancellationToken cancellationToken = default)
    {
        // Use IgnoreQueryFilters to bypass global filter, then manually filter by Venue.TenantId
        // This ensures proper tenant isolation through the Venue relationship
        var show = await _dbContext.Set<Show>()
            .IgnoreQueryFilters()
            .Include(s => s.Venue)
            .Where(s => s.ShowGuid == showGuid)
            .Where(s => _tenantContext.CurrentTenantId == null || s.Venue.TenantId == _tenantContext.CurrentTenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (show == null)
        {
            return false;
        }

        _dbContext.Set<Show>().Remove(show);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<NearbyShowsResponse> GetNearbyShowsAsync(Guid venueGuid, DateTimeOffset startTime, CancellationToken cancellationToken = default)
    {
        // Validate venue exists (tenant-filtered)
        var venue = await _dbContext.Set<Venue>()
            .FirstOrDefaultAsync(v => v.VenueGuid == venueGuid, cancellationToken);

        if (venue == null)
        {
            throw new KeyNotFoundException($"Venue with GUID {venueGuid} not found");
        }

        // Calculate 48-hour window using UTC for accurate comparison
        var startUtc = startTime.UtcDateTime;
        var windowStart = startUtc.AddHours(-48);
        var windowEnd = startUtc.AddHours(48);

        // Query shows within the 48-hour window
        var nearbyShows = await _dbContext.Set<Show>()
            .Include(s => s.Act)
            .Where(s => s.VenueId == venue.Id)
            .Where(s => s.StartTime.UtcDateTime >= windowStart)
            .Where(s => s.StartTime.UtcDateTime <= windowEnd)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var message = nearbyShows.Count == 0
            ? "No other shows scheduled at this venue within 48 hours"
            : $"{nearbyShows.Count} show(s) found within 48 hours";

        return new NearbyShowsResponse
        {
            VenueGuid = venue.VenueGuid,
            VenueName = venue.Name,
            ReferenceTime = startTime,
            Shows = nearbyShows.Select(s => new NearbyShowDto
            {
                ShowGuid = s.ShowGuid,
                ActName = s.Act.Name,
                StartTime = s.StartTime
            }).ToList(),
            Message = message
        };
    }

    /// <summary>
    /// Maps a Show entity to a ShowDto.
    /// </summary>
    /// <param name="show">The show entity.</param>
    /// <returns>The show DTO.</returns>
    private static ShowDto MapToDto(Show show)
    {
        return new ShowDto
        {
            Id = show.Id,
            ShowGuid = show.ShowGuid,
            ActGuid = show.Act.ActGuid,
            ActName = show.Act.Name,
            VenueGuid = show.Venue.VenueGuid,
            VenueName = show.Venue.Name,
            VenueCapacity = show.Venue.SeatingCapacity,
            TicketCount = show.TicketCount,
            StartTime = show.StartTime,
            CreatedAt = show.CreatedAt,
            UpdatedAt = show.UpdatedAt
        };
    }
}
