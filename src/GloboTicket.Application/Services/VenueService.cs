using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Application.MultiTenancy;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Application.Services;

/// <summary>
/// Service implementation for managing venue operations.
/// Handles venue-related business logic and data access with tenant isolation.
/// </summary>
public class VenueService : IVenueService
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="VenueService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for tenant isolation.</param>
    public VenueService(DbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<VenueDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var venueSpec =
            from v in _dbContext.Set<Venue>().AsNoTracking()
            where v.Id == id
            select new VenueDto
            {
                Id = v.Id,
                VenueGuid = v.VenueGuid,
                Name = v.Name,
                Address = v.Address,
                Latitude = v.Location != null ? v.Location.Y : null,
                Longitude = v.Location != null ? v.Location.X : null,
                SeatingCapacity = v.SeatingCapacity,
                Description = v.Description,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            };

        return await venueSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<VenueDto?> GetByGuidAsync(Guid venueGuid, CancellationToken cancellationToken = default)
    {
        var venueSpec =
            from v in _dbContext.Set<Venue>().AsNoTracking()
            where v.VenueGuid == venueGuid
            select new VenueDto
            {
                Id = v.Id,
                VenueGuid = v.VenueGuid,
                Name = v.Name,
                Address = v.Address,
                Latitude = v.Location != null ? v.Location.Y : null,
                Longitude = v.Location != null ? v.Location.X : null,
                SeatingCapacity = v.SeatingCapacity,
                Description = v.Description,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            };

        return await venueSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VenueDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var venuesSpec =
            from v in _dbContext.Set<Venue>().AsNoTracking()
            select new VenueDto
            {
                Id = v.Id,
                VenueGuid = v.VenueGuid,
                Name = v.Name,
                Address = v.Address,
                Latitude = v.Location != null ? v.Location.Y : null,
                Longitude = v.Location != null ? v.Location.X : null,
                SeatingCapacity = v.SeatingCapacity,
                Description = v.Description,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            };

        return await venuesSpec.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<VenueDto> CreateAsync(CreateVenueDto dto, CancellationToken cancellationToken = default)
    {
        // Validate tenant context is available
        if (!_tenantContext.CurrentTenantId.HasValue)
        {
            throw new InvalidOperationException("Tenant context is required for venue creation.");
        }

        var venue = new Venue
        {
            VenueGuid = dto.VenueGuid,
            Name = dto.Name,
            Address = dto.Address,
            Location = GeographyService.CreatePoint(dto.Latitude, dto.Longitude),
            SeatingCapacity = dto.SeatingCapacity,
            Description = dto.Description
            // TenantId will be automatically set by DbContext.SaveChangesAsync
        };

        _dbContext.Set<Venue>().Add(venue);
        await _dbContext.SaveChangesAsync(cancellationToken);

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

    /// <inheritdoc />
    public async Task<VenueDto?> UpdateAsync(int id, UpdateVenueDto dto, CancellationToken cancellationToken = default)
    {
        var venueSpec =
            from v in _dbContext.Set<Venue>()
            where v.Id == id
            select v;

        var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);

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

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var venueSpec =
            from v in _dbContext.Set<Venue>()
            where v.Id == id
            select v;

        var venue = await venueSpec.FirstOrDefaultAsync(cancellationToken);

        if (venue == null)
        {
            return false;
        }

        _dbContext.Set<Venue>().Remove(venue);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Venue>().CountAsync(cancellationToken);
    }
}
