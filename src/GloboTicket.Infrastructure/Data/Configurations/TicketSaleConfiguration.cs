using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the TicketSale entity.
/// Defines table structure, constraints, indexes, and multi-tenant support with compound foreign keys.
/// </summary>
public class TicketSaleConfiguration : IEntityTypeConfiguration<TicketSale>
{
    /// <summary>
    /// Configures the TicketSale entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<TicketSale> builder)
    {
        // Table name
        builder.ToTable("TicketSales");

        // Primary key
        builder.HasKey(ts => ts.Id);
        builder.Property(ts => ts.Id)
            .ValueGeneratedOnAdd();

        // Composite alternate key for multi-tenant uniqueness
        builder.HasAlternateKey(ts => new { ts.TenantId, ts.TicketSaleGuid });

        // Index on TicketSaleGuid for queries
        builder.HasIndex(ts => ts.TicketSaleGuid);

        // TicketSaleGuid property
        builder.Property(ts => ts.TicketSaleGuid)
            .IsRequired();

        // Quantity property
        builder.Property(ts => ts.Quantity)
            .IsRequired();

        // Foreign key relationship to Tenant with cascade delete
        builder.HasOne(ts => ts.Tenant)
            .WithMany()
            .HasForeignKey(ts => ts.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Foreign key relationship to Show with compound key for tenant isolation
        // This ensures a ticket sale can only reference shows within the same tenant
        builder.HasOne(ts => ts.Show)
            .WithMany(s => s.TicketSales)
            .HasForeignKey("TenantId", "ShowId")
            .HasPrincipalKey(s => new { s.TenantId, s.Id })
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(ts => ts.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(ts => ts.UpdatedAt)
            .IsRequired(false);
    }
}