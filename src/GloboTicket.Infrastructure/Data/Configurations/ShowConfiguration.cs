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

        // Alternate key for uniqueness
        builder.HasAlternateKey(s => s.ShowGuid);

        // Index on ShowGuid for queries
        builder.HasIndex(s => s.ShowGuid);

        // ShowGuid property
        builder.Property(s => s.ShowGuid)
            .IsRequired();

        // Date property
        builder.Property(s => s.Date)
            .IsRequired();

        // Foreign key relationship to Venue
        builder.HasOne(s => s.Venue)
            .WithMany()
            .HasForeignKey("VenueId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Foreign key relationship to Act
        builder.HasOne(s => s.Act)
            .WithMany()
            .HasForeignKey("ActId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // TicketSales collection is configured via the TicketSale entity's HasOne relationship

        // CreatedAt property (inherited from Entity)
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);
    }
}