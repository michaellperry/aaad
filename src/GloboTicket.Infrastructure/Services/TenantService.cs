using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Infrastructure.Services;

/// <summary>
/// Service implementation for managing tenant operations.
/// Handles tenant-related business logic and data access.
/// </summary>
public class TenantService : ITenantService
{
    private readonly GloboTicketDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    public TenantService(GloboTicketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<TenantDto?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        return tenant == null ? null : MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task<TenantDto?> GetTenantBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);

        return tenant == null ? null : MapToDto(tenant);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _dbContext.Tenants
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return tenants.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Name = dto.Name,
            Slug = dto.Slug,
            IsActive = true
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(tenant);
    }

    /// <summary>
    /// Maps a Tenant entity to a TenantDto.
    /// </summary>
    /// <param name="tenant">The tenant entity to map.</param>
    /// <returns>The mapped tenant DTO.</returns>
    private static TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt
        };
    }
}