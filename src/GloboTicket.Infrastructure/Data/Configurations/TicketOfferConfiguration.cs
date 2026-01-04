using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the TicketOffer entity.
/// Defines table structure, constraints, indexes, and relationships.
/// TicketOffer is a child entity that inherits tenant context through Show → Venue relationship.
/// </summary>
public class TicketOfferConfiguration : IEntityTypeConfiguration<TicketOffer>
{
    /// <summary>
    /// Configures the TicketOffer entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<TicketOffer> builder)
    {
        // Table name
        builder.ToTable("TicketOffers");

        // Primary key
        builder.HasKey(to => to.Id);
        builder.Property(to => to.Id)
            .ValueGeneratedOnAdd();

        // Unique index on TicketOfferGuid for fast GUID lookups
        builder.HasIndex(to => to.TicketOfferGuid)
            .IsUnique();

        // Index on ShowId for efficient ticket offer listing by show
        builder.HasIndex(to => to.ShowId);

        // Composite index on (ShowId, CreatedAt) for chronological ordering of offers per show
        builder.HasIndex(to => new { to.ShowId, to.CreatedAt });

        // TicketOfferGuid property
        builder.Property(to => to.TicketOfferGuid)
            .IsRequired();

        // ShowId property
        builder.Property(to => to.ShowId)
            .IsRequired();

        // Name property with max length and required constraint
        builder.Property(to => to.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Price property with decimal precision and check constraint
        builder.Property(to => to.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        // Add check constraint for Price > 0
        builder.ToTable(t => t.HasCheckConstraint("CK_TicketOffers_Price", "[Price] > 0"));

        // TicketCount property with check constraint
        builder.Property(to => to.TicketCount)
            .IsRequired();

        // Add check constraint for TicketCount > 0
        builder.ToTable(t => t.HasCheckConstraint("CK_TicketOffers_TicketCount", "[TicketCount] > 0"));

        // Foreign key relationship to Show with cascade delete
        builder.HasOne(to => to.Show)
            .WithMany(s => s.TicketOffers)
            .HasForeignKey(to => to.ShowId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(to => to.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(to => to.UpdatedAt)
            .IsRequired(false);
    }
}
