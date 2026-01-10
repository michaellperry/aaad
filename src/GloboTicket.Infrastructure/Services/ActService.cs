using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Infrastructure.Services;

/// <summary>
/// Service implementation for managing act operations.
/// Handles act-related business logic and data access with tenant isolation.
/// </summary>
public class ActService : IActService
{
    private readonly GloboTicketDbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for tenant isolation.</param>
    public ActService(GloboTicketDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<ActDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var actSpec =
            from act in _dbContext.Acts.AsNoTracking()
            where act.Id == id
            select new ActDto
            {
                Id = act.Id,
                ActGuid = act.ActGuid,
                Name = act.Name,
                CreatedAt = act.CreatedAt,
                UpdatedAt = act.UpdatedAt
            };

        return await actSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ActDto?> GetByGuidAsync(Guid actGuid, CancellationToken cancellationToken = default)
    {
        var actSpec =
            from act in _dbContext.Acts.AsNoTracking()
            where act.ActGuid == actGuid
            select new ActDto
            {
                Id = act.Id,
                ActGuid = act.ActGuid,
                Name = act.Name,
                CreatedAt = act.CreatedAt,
                UpdatedAt = act.UpdatedAt
            };

        return await actSpec.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ActDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var actsSpec =
            from act in _dbContext.Acts.AsNoTracking()
            select new ActDto
            {
                Id = act.Id,
                ActGuid = act.ActGuid,
                Name = act.Name,
                CreatedAt = act.CreatedAt,
                UpdatedAt = act.UpdatedAt
            };

        return await actsSpec.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ActDto> CreateAsync(CreateActDto dto, CancellationToken cancellationToken = default)
    {
        // Validate tenant context is available
        if (!_tenantContext.CurrentTenantId.HasValue)
        {
            throw new InvalidOperationException("Tenant context is required for act creation.");
        }

        var act = new Act
        {
            ActGuid = dto.ActGuid,
            Name = dto.Name
            // TenantId will be automatically set by DbContext.SaveChangesAsync
        };

        _dbContext.Acts.Add(act);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ActDto
        {
            Id = act.Id,
            ActGuid = act.ActGuid,
            Name = act.Name,
            CreatedAt = act.CreatedAt,
            UpdatedAt = act.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<ActDto?> UpdateAsync(int id, UpdateActDto dto, CancellationToken cancellationToken = default)
    {
        var actSpec =
            from a in _dbContext.Acts
            where a.Id == id
            select a;

        var act = await actSpec.FirstOrDefaultAsync(cancellationToken);

        if (act == null)
        {
            return null;
        }

        act.Name = dto.Name;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ActDto
        {
            Id = act.Id,
            ActGuid = act.ActGuid,
            Name = act.Name,
            CreatedAt = act.CreatedAt,
            UpdatedAt = act.UpdatedAt
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var actSpec =
            from a in _dbContext.Acts
            where a.Id == id
            select a;

        var act = await actSpec.FirstOrDefaultAsync(cancellationToken);

        if (act == null)
        {
            return false;
        }

        _dbContext.Acts.Remove(act);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Acts.CountAsync(cancellationToken);
    }
}
