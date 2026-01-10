using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Application.Services;

/// <summary>
/// Service implementation for managing tenant operations.
/// Handles tenant-related business logic and data access.
/// </summary>
public class TenantService : ITenantService
{
    private readonly DbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    public TenantService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<TenantDto?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var tenantSpec =
            from t in _dbContext.Set<Tenant>().AsNoTracking()
            where t.Id == tenantId
            select new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                TenantIdentifier = t.TenantIdentifier,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };

        return await tenantSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TenantDto?> GetTenantBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tenantSpec =
            from t in _dbContext.Set<Tenant>().AsNoTracking()
            where t.Slug == slug
            select new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                TenantIdentifier = t.TenantIdentifier,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };

        return await tenantSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TenantDto?> GetTenantByIdentifierAsync(string tenantIdentifier, CancellationToken cancellationToken = default)
    {
        var tenantSpec =
            from t in _dbContext.Set<Tenant>().AsNoTracking()
            where t.TenantIdentifier == tenantIdentifier
            select new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                TenantIdentifier = t.TenantIdentifier,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };

        return await tenantSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        var tenantsSpec =
            from t in _dbContext.Set<Tenant>().AsNoTracking()
            select new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                TenantIdentifier = t.TenantIdentifier,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };

        return await tenantsSpec.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Name = dto.Name,
            Slug = dto.Slug,
            TenantIdentifier = dto.TenantIdentifier,
            IsActive = true
        };

        _dbContext.Set<Tenant>().Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            TenantIdentifier = tenant.TenantIdentifier,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt
        };
    }
}
