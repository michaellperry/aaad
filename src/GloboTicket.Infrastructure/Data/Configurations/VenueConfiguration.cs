using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Venue entity.
/// Defines table structure, constraints, indexes, and multi-tenant support.
/// </summary>
public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    /// <summary>
    /// Configures the Venue entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        // Table name
        builder.ToTable("Venues");

        // Primary key
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd();

        // Composite alternate key for multi-tenant uniqueness
        builder.HasAlternateKey(v => new { v.TenantId, v.VenueGuid });

        // Index on VenueGuid for queries
        builder.HasIndex(v => v.VenueGuid);

        // VenueGuid property
        builder.Property(v => v.VenueGuid)
            .IsRequired();

        // Name property
        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Address property (optional)
        builder.Property(v => v.Address)
            .HasMaxLength(300);

        // Location property - geography type for geospatial data
        builder.Property(v => v.Location)
            .HasColumnType("geography");

        // SeatingCapacity property
        builder.Property(v => v.SeatingCapacity)
            .IsRequired();

        // Description property
        builder.Property(v => v.Description)
            .IsRequired();

        // Foreign key relationship to Tenant with cascade delete
        builder.HasOne(v => v.Tenant)
            .WithMany()
            .HasForeignKey(v => v.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(v => v.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(v => v.UpdatedAt)
            .IsRequired(false);
    }
}