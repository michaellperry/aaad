using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;
using GloboTicket.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.Infrastructure.Data;

/// <summary>
/// The main Entity Framework Core DbContext for GloboTicket with multi-tenant support.
/// Implements row-level isolation using global query filters based on TenantId.
/// 
/// Tenants provide data isolation within an environment's database. Multiple tenants
/// can coexist in the same database (e.g., Production tenant and Smoke Test tenant
/// in a production environment), with complete data isolation between them.
/// </summary>
public class GloboTicketDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Gets or sets the Tenants DbSet for managing tenant entities.
    /// </summary>
    public DbSet<Tenant> Tenants { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Venues DbSet for managing venue entities.
    /// </summary>
    public DbSet<Venue> Venues { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Acts DbSet for managing act entities.
    /// </summary>
    public DbSet<Act> Acts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Shows DbSet for managing show entities.
    /// </summary>
    public DbSet<Show> Shows { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="GloboTicketDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    /// <param name="tenantContext">The tenant context providing the current tenant ID.</param>
    public GloboTicketDbContext(
        DbContextOptions<GloboTicketDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Configures the model using Fluent API and applies entity configurations.
    /// Sets up global query filters for multi-tenant isolation.
    /// 
    /// Note: Tenants exist within an environment. An environment is a deployment
    /// (dev, staging, production) with its own database. Tenants provide isolation
    /// within that environment's database.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GloboTicketDbContext).Assembly);

        // Configure global query filters for multi-tenant entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if the entity implements ITenantEntity
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Create the filter expression: entity => _tenantContext.CurrentTenantId == null || entity.TenantId == _tenantContext.CurrentTenantId
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "entity");
                
                // _tenantContext.CurrentTenantId == null
                var tenantIdIsNull = System.Linq.Expressions.Expression.Equal(
                    System.Linq.Expressions.Expression.Property(
                        System.Linq.Expressions.Expression.Constant(_tenantContext),
                        nameof(ITenantContext.CurrentTenantId)),
                    System.Linq.Expressions.Expression.Constant(null, typeof(int?)));

                // entity.TenantId
                var entityTenantId = System.Linq.Expressions.Expression.Property(
                    parameter,
                    nameof(ITenantEntity.TenantId));

                // _tenantContext.CurrentTenantId
                var currentTenantId = System.Linq.Expressions.Expression.Property(
                    System.Linq.Expressions.Expression.Constant(_tenantContext),
                    nameof(ITenantContext.CurrentTenantId));

                // entity.TenantId == _tenantContext.CurrentTenantId
                var tenantIdMatches = System.Linq.Expressions.Expression.Equal(
                    entityTenantId,
                    System.Linq.Expressions.Expression.Convert(currentTenantId, typeof(int)));

                // _tenantContext.CurrentTenantId == null || entity.TenantId == _tenantContext.CurrentTenantId
                var filterExpression = System.Linq.Expressions.Expression.OrElse(
                    tenantIdIsNull,
                    tenantIdMatches);

                var lambda = System.Linq.Expressions.Expression.Lambda(filterExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically sets TenantId (environment context) for new tenant entities and timestamps for all entities.
    /// 
    /// The TenantId represents the data context within the environment's database,
    /// ensuring all new entities are associated with the correct tenant.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Process all tracked entities
        foreach (var entry in ChangeTracker.Entries())
        {
            // Handle Entity base class timestamps
            if (entry.Entity is Entity entity)
            {
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Handle ITenantEntity TenantId assignment
            if (entry.Entity is ITenantEntity tenantEntity && entry.State == EntityState.Added)
            {
                // Only set TenantId if current tenant context is available
                if (_tenantContext.CurrentTenantId.HasValue)
                {
                    tenantEntity.TenantId = _tenantContext.CurrentTenantId.Value;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}