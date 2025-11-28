using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Infrastructure.Services;

/// <summary>
/// Service implementation for managing show operations.
/// Handles show-related business logic and data access with tenant isolation.
/// </summary>
public class ShowService : IShowService
{
    private readonly GloboTicketDbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for tenant isolation.</param>
    public ShowService(GloboTicketDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<ShowDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var show = await _dbContext.Shows
            .AsNoTracking()
            .Include(s => s.Venue)
            .Include(s => s.Act)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return show == null ? null : MapToDto(show);
    }

    /// <inheritdoc />
    public async Task<ShowDto?> GetByGuidAsync(Guid showGuid, CancellationToken cancellationToken = default)
    {
        var show = await _dbContext.Shows
            .AsNoTracking()
            .Include(s => s.Venue)
            .Include(s => s.Act)
            .FirstOrDefaultAsync(s => s.ShowGuid == showGuid, cancellationToken);

        return show == null ? null : MapToDto(show);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ShowDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var shows = await _dbContext.Shows
            .AsNoTracking()
            .Include(s => s.Venue)
            .Include(s => s.Act)
            .ToListAsync(cancellationToken);

        return shows.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<ShowDto> CreateAsync(CreateShowDto dto, CancellationToken cancellationToken = default)
    {
        // Fetch Venue by Guid within the current tenant
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(v => v.VenueGuid == dto.VenueGuid, cancellationToken);

        if (venue == null)
        {
            throw new InvalidOperationException($"Venue with GUID {dto.VenueGuid} not found.");
        }

        // Fetch Act by Guid within the current tenant
        var act = await _dbContext.Acts
            .FirstOrDefaultAsync(a => a.ActGuid == dto.ActGuid, cancellationToken);

        if (act == null)
        {
            throw new InvalidOperationException($"Act with GUID {dto.ActGuid} not found.");
        }

        // Create Show using constructor that requires Venue and Act instances
        var show = new Show(venue, act)
        {
            ShowGuid = dto.ShowGuid,
            Date = dto.Date
        };

        _dbContext.Shows.Add(show);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(show);
    }

    /// <inheritdoc />
    public async Task<ShowDto?> UpdateAsync(int id, UpdateShowDto dto, CancellationToken cancellationToken = default)
    {
        var show = await _dbContext.Shows
            .Include(s => s.Venue)
            .Include(s => s.Act)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (show == null)
        {
            return null;
        }

        // Only update the date (venue and act cannot be changed)
        show.Date = dto.Date;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(show);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var show = await _dbContext.Shows
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (show == null)
        {
            return false;
        }

        _dbContext.Shows.Remove(show);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Maps a Show entity to a ShowDto.
    /// </summary>
    /// <param name="show">The show entity to map.</param>
    /// <returns>The mapped show DTO.</returns>
    private static ShowDto MapToDto(Show show)
    {
        return new ShowDto
        {
            Id = show.Id,
            ShowGuid = show.ShowGuid,
            VenueGuid = show.Venue.VenueGuid,
            VenueName = show.Venue.Name,
            ActGuid = show.Act.ActGuid,
            ActName = show.Act.Name,
            Date = show.Date,
            CreatedAt = show.CreatedAt,
            UpdatedAt = show.UpdatedAt
        };
    }
}