using GloboTicket.Application.DTOs;
using GloboTicket.Application.Interfaces;
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using GloboTicket.Application.MultiTenancy;

namespace GloboTicket.Infrastructure.Services;

/// <summary>
/// Service implementation for managing ticket offer operations.
/// Handles ticket offer-related business logic and data access with tenant isolation through Show → Venue relationship.
/// </summary>
public class TicketOfferService : ITicketOfferService
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicketOfferService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for data access.</param>
    /// <param name="tenantContext">The tenant context for accessing current tenant ID.</param>
    public TicketOfferService(DbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public async Task<TicketOfferDto> CreateTicketOfferAsync(Guid showGuid, CreateTicketOfferDto dto, CancellationToken cancellationToken = default)
    {
        // Use a transaction to ensure thread-safe capacity validation
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Find show with tenant filtering through Venue relationship
            // Include TicketOffers for capacity validation
            var show = await _dbContext.Set<Show>()
                .IgnoreQueryFilters()
                .Include(s => s.Venue)
                .Include(s => s.TicketOffers)
                .Where(s => s.ShowGuid == showGuid)
                .Where(s => _tenantContext.CurrentTenantId == null || s.Venue.TenantId == _tenantContext.CurrentTenantId)
                .FirstOrDefaultAsync(cancellationToken);

            if (show == null)
            {
                throw new KeyNotFoundException($"Show with GUID {showGuid} not found");
            }

            // Validate capacity using domain logic
            if (!show.CanAddTicketOffer(dto.TicketCount))
            {
                var availableCapacity = show.GetAvailableCapacity();
                throw new ArgumentException(
                    $"Cannot create ticket offer. Requested {dto.TicketCount} tickets, but only {availableCapacity} tickets available. " +
                    $"Show capacity: {show.TicketCount}, Already allocated: {show.TicketCount - availableCapacity}",
                    nameof(dto.TicketCount));
            }

            // Create ticket offer entity
            var ticketOffer = new TicketOffer(
                show,
                dto.TicketOfferGuid,
                dto.Name,
                dto.Price,
                dto.TicketCount);

            _dbContext.Set<TicketOffer>().Add(ticketOffer);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return MapToDto(ticketOffer);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TicketOfferDto>> GetTicketOffersByShowAsync(Guid showGuid, CancellationToken cancellationToken = default)
    {
        // Validate show exists with tenant filtering through Venue relationship
        var showExists = await _dbContext.Set<Show>()
            .IgnoreQueryFilters()
            .Include(s => s.Venue)
            .Where(s => s.ShowGuid == showGuid)
            .Where(s => _tenantContext.CurrentTenantId == null || s.Venue.TenantId == _tenantContext.CurrentTenantId)
            .AnyAsync(cancellationToken);

        if (!showExists)
        {
            throw new KeyNotFoundException($"Show with GUID {showGuid} not found");
        }

        // Query ticket offers for the show
        // Tenant isolation is inherited through Show → Venue relationship
        var ticketOffers = await _dbContext.Set<TicketOffer>()
            .Include(to => to.Show)
                .ThenInclude(s => s.Venue)
            .Where(to => to.Show.ShowGuid == showGuid)
            .Where(to => _tenantContext.CurrentTenantId == null || to.Show.Venue.TenantId == _tenantContext.CurrentTenantId)
            .OrderBy(to => to.CreatedAt)
            .ToListAsync(cancellationToken);

        return ticketOffers.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<ShowCapacityDto> GetShowCapacityAsync(Guid showGuid, CancellationToken cancellationToken = default)
    {
        // Find show with tenant filtering through Venue relationship
        // Include TicketOffers for capacity calculation
        var show = await _dbContext.Set<Show>()
            .IgnoreQueryFilters()
            .Include(s => s.Venue)
            .Include(s => s.TicketOffers)
            .Where(s => s.ShowGuid == showGuid)
            .Where(s => _tenantContext.CurrentTenantId == null || s.Venue.TenantId == _tenantContext.CurrentTenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (show == null)
        {
            throw new KeyNotFoundException($"Show with GUID {showGuid} not found");
        }

        // Calculate capacity using domain logic
        var availableCapacity = show.GetAvailableCapacity();
        var allocatedTickets = show.TicketOffers.Sum(o => o.TicketCount);

        return new ShowCapacityDto
        {
            ShowGuid = show.ShowGuid,
            TotalTickets = show.TicketCount,
            AllocatedTickets = allocatedTickets,
            AvailableCapacity = availableCapacity
        };
    }

    /// <summary>
    /// Maps a TicketOffer entity to a TicketOfferDto.
    /// </summary>
    /// <param name="ticketOffer">The ticket offer entity.</param>
    /// <returns>The ticket offer DTO.</returns>
    private static TicketOfferDto MapToDto(TicketOffer ticketOffer)
    {
        return new TicketOfferDto
        {
            Id = ticketOffer.Id,
            TicketOfferGuid = ticketOffer.TicketOfferGuid,
            ShowGuid = ticketOffer.Show.ShowGuid,
            Name = ticketOffer.Name,
            Price = ticketOffer.Price,
            TicketCount = ticketOffer.TicketCount,
            CreatedAt = ticketOffer.CreatedAt
        };
    }
}
