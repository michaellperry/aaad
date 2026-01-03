using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Services;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Infrastructure.Services;

/// <summary>
/// Service implementation for managing venue operations.
/// Handles venue-related business logic and data access with tenant isolation.
/// </summary>
public class VenueService : IVenueService
{
    private readonly GloboTicketDbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="VenueService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for tenant isolation.</param>
    public VenueService(GloboTicketDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<VenueDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return venue == null ? null : MapToDto(venue);
    }

    /// <inheritdoc />
    public async Task<VenueDto?> GetByGuidAsync(Guid venueGuid, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VenueGuid == venueGuid, cancellationToken);

        return venue == null ? null : MapToDto(venue);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VenueDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var venues = await _dbContext.Venues
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return venues.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<VenueDto> CreateAsync(CreateVenueDto dto, CancellationToken cancellationToken = default)
    {
        var venue = new Venue
        {
            VenueGuid = dto.VenueGuid,
            Name = dto.Name,
            Address = dto.Address,
            Location = GeographyService.CreatePoint(dto.Latitude, dto.Longitude),
            SeatingCapacity = dto.SeatingCapacity,
            Description = dto.Description,
            TenantId = _tenantContext.CurrentTenantId ?? throw new InvalidOperationException("Tenant context is required for venue creation.")
        };

        _dbContext.Venues.Add(venue);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(venue);
    }

    /// <inheritdoc />
    public async Task<VenueDto?> UpdateAsync(int id, UpdateVenueDto dto, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venue == null)
        {
            return null;
        }

        venue.Name = dto.Name;
        venue.Address = dto.Address;
        venue.Location = GeographyService.CreatePoint(dto.Latitude, dto.Longitude);
        venue.SeatingCapacity = dto.SeatingCapacity;
        venue.Description = dto.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(venue);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (venue == null)
        {
            return false;
        }

        _dbContext.Venues.Remove(venue);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Maps a Venue entity to a VenueDto.
    /// </summary>
    /// <param name="venue">The venue entity to map.</param>
    /// <returns>The mapped venue DTO.</returns>
    private static VenueDto MapToDto(Venue venue)
    {
        return new VenueDto
        {
            Id = venue.Id,
            VenueGuid = venue.VenueGuid,
            Name = venue.Name,
            Address = venue.Address,
            Latitude = venue.Location?.Y,
            Longitude = venue.Location?.X,
            SeatingCapacity = venue.SeatingCapacity,
            Description = venue.Description,
            CreatedAt = venue.CreatedAt,
            UpdatedAt = venue.UpdatedAt
        };
    }
}
