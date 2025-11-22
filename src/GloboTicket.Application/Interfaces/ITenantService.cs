using GloboTicket.Application.DTOs;

namespace GloboTicket.Application.Interfaces;

/// <summary>
/// Service interface for managing tenant operations.
/// Defines the contract for tenant-related business logic.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Retrieves a tenant by its unique identifier.
    /// </summary>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant DTO if found; otherwise, null.</returns>
    Task<TenantDto?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a tenant by its unique slug identifier.
    /// </summary>
    /// <param name="slug">The tenant slug.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant DTO if found; otherwise, null.</returns>
    Task<TenantDto?> GetTenantBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all tenants in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all tenant DTOs.</returns>
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new tenant in the system.
    /// </summary>
    /// <param name="dto">The tenant creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tenant DTO.</returns>
    Task<TenantDto> CreateTenantAsync(CreateTenantDto dto, CancellationToken cancellationToken = default);
}