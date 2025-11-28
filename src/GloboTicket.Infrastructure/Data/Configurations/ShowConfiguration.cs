using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Show entity.
/// Defines table structure, constraints, indexes, and multi-tenant support with compound foreign keys.
/// </summary>
public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    /// <summary>
    /// Configures the Show entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        // Table name
        builder.ToTable("Shows");

        // Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        // Composite alternate key for multi-tenant uniqueness
        builder.HasAlternateKey(s => new { s.TenantId, s.ShowGuid });

        // Index on ShowGuid for queries
        builder.HasIndex(s => s.ShowGuid);

        // ShowGuid property
        builder.Property(s => s.ShowGuid)
            .IsRequired();

        // Date property
        builder.Property(s => s.Date)
            .IsRequired();

        // Foreign key relationship to Tenant with cascade delete
        builder.HasOne(s => s.Tenant)
            .WithMany()
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Foreign key relationship to Venue with compound key for tenant isolation
        // This ensures a show can only reference venues within the same tenant
        builder.HasOne(s => s.Venue)
            .WithMany()
            .HasForeignKey("TenantId", "VenueId")
            .HasPrincipalKey(v => new { v.TenantId, v.Id })
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Foreign key relationship to Act with compound key for tenant isolation
        // This ensures a show can only reference acts within the same tenant
        builder.HasOne(s => s.Act)
            .WithMany()
            .HasForeignKey("TenantId", "ActId")
            .HasPrincipalKey(a => new { a.TenantId, a.Id })
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Ignore TicketSales collection for now (will be configured when TicketSale entity exists)
        builder.Ignore(s => s.TicketSales);

        // CreatedAt property (inherited from Entity)
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);
    }
}