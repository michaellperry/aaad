using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Infrastructure.Services;

/// <summary>
/// Service implementation for managing ticket sale operations.
/// Handles ticket sale-related business logic and data access with tenant isolation.
/// </summary>
public class TicketSaleService : ITicketSaleService
{
    private readonly GloboTicketDbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicketSaleService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for tenant isolation.</param>
    public TicketSaleService(GloboTicketDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<TicketSaleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticketSale = await _dbContext.TicketSales
            .AsNoTracking()
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Venue)
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Act)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);

        return ticketSale == null ? null : MapToDto(ticketSale);
    }

    /// <inheritdoc />
    public async Task<TicketSaleDto?> GetByGuidAsync(Guid ticketSaleGuid, CancellationToken cancellationToken = default)
    {
        var ticketSale = await _dbContext.TicketSales
            .AsNoTracking()
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Venue)
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Act)
            .FirstOrDefaultAsync(ts => ts.TicketSaleGuid == ticketSaleGuid, cancellationToken);

        return ticketSale == null ? null : MapToDto(ticketSale);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TicketSaleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var ticketSales = await _dbContext.TicketSales
            .AsNoTracking()
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Venue)
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Act)
            .ToListAsync(cancellationToken);

        return ticketSales.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TicketSaleDto>> GetByShowGuidAsync(Guid showGuid, CancellationToken cancellationToken = default)
    {
        var ticketSales = await _dbContext.TicketSales
            .AsNoTracking()
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Venue)
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Act)
            .Where(ts => ts.Show.ShowGuid == showGuid)
            .ToListAsync(cancellationToken);

        return ticketSales.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<TicketSaleDto> CreateAsync(CreateTicketSaleDto dto, CancellationToken cancellationToken = default)
    {
        // Fetch Show by Guid within the current tenant (includes Venue and Act for DTO mapping)
        var show = await _dbContext.Shows
            .Include(s => s.Venue)
            .Include(s => s.Act)
            .FirstOrDefaultAsync(s => s.ShowGuid == dto.ShowGuid, cancellationToken);

        if (show == null)
        {
            throw new InvalidOperationException($"Show with GUID {dto.ShowGuid} not found.");
        }

        // Create TicketSale using constructor that requires Show instance
        var ticketSale = new TicketSale(show)
        {
            TicketSaleGuid = dto.TicketSaleGuid,
            Quantity = dto.Quantity,
            TenantId = _tenantContext.CurrentTenantId ?? throw new InvalidOperationException("Tenant context is required for ticket sale creation.")
        };

        _dbContext.TicketSales.Add(ticketSale);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(ticketSale);
    }

    /// <inheritdoc />
    public async Task<TicketSaleDto?> UpdateAsync(int id, UpdateTicketSaleDto dto, CancellationToken cancellationToken = default)
    {
        var ticketSale = await _dbContext.TicketSales
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Venue)
            .Include(ts => ts.Show)
                .ThenInclude(s => s.Act)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);

        if (ticketSale == null)
        {
            return null;
        }

        // Update the quantity
        ticketSale.Quantity = dto.Quantity;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(ticketSale);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticketSale = await _dbContext.TicketSales
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);

        if (ticketSale == null)
        {
            return false;
        }

        _dbContext.TicketSales.Remove(ticketSale);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Maps a TicketSale entity to a TicketSaleDto.
    /// </summary>
    /// <param name="ticketSale">The ticket sale entity to map.</param>
    /// <returns>The mapped ticket sale DTO.</returns>
    private static TicketSaleDto MapToDto(TicketSale ticketSale)
    {
        return new TicketSaleDto
        {
            Id = ticketSale.Id,
            TicketSaleGuid = ticketSale.TicketSaleGuid,
            ShowGuid = ticketSale.Show.ShowGuid,
            ShowDate = ticketSale.Show.Date,
            VenueName = ticketSale.Show.Venue.Name,
            ActName = ticketSale.Show.Act.Name,
            Quantity = ticketSale.Quantity,
            CreatedAt = ticketSale.CreatedAt,
            UpdatedAt = ticketSale.UpdatedAt
        };
    }
}