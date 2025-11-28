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
        var act = await _dbContext.Acts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return act == null ? null : MapToDto(act);
    }

    /// <inheritdoc />
    public async Task<ActDto?> GetByGuidAsync(Guid actGuid, CancellationToken cancellationToken = default)
    {
        var act = await _dbContext.Acts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ActGuid == actGuid, cancellationToken);

        return act == null ? null : MapToDto(act);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ActDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var acts = await _dbContext.Acts
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return acts.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<ActDto> CreateAsync(CreateActDto dto, CancellationToken cancellationToken = default)
    {
        var act = new Act
        {
            ActGuid = dto.ActGuid,
            Name = dto.Name,
            TenantId = _tenantContext.CurrentTenantId ?? throw new InvalidOperationException("Tenant context is required for act creation.")
        };

        _dbContext.Acts.Add(act);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(act);
    }

    /// <inheritdoc />
    public async Task<ActDto?> UpdateAsync(int id, UpdateActDto dto, CancellationToken cancellationToken = default)
    {
        var act = await _dbContext.Acts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (act == null)
        {
            return null;
        }

        act.Name = dto.Name;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(act);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var act = await _dbContext.Acts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (act == null)
        {
            return false;
        }

        _dbContext.Acts.Remove(act);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Maps an Act entity to an ActDto.
    /// </summary>
    /// <param name="act">The act entity to map.</param>
    /// <returns>The mapped act DTO.</returns>
    private static ActDto MapToDto(Act act)
    {
        return new ActDto
        {
            Id = act.Id,
            ActGuid = act.ActGuid,
            Name = act.Name,
            CreatedAt = act.CreatedAt,
            UpdatedAt = act.UpdatedAt
        };
    }
}