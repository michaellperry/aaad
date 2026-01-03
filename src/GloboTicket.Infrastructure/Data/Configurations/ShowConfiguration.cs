using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Show entity.
/// Defines table structure, constraints, indexes, and relationships.
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

        // Index on ShowGuid for queries (unique)
        builder.HasIndex(s => s.ShowGuid)
            .IsUnique();

        // Composite index for nearby shows query optimization
        builder.HasIndex(s => new { s.VenueId, s.StartTime });

        // Index on ActId for listing shows by act
        builder.HasIndex(s => s.ActId);

        // ShowGuid property
        builder.Property(s => s.ShowGuid)
            .IsRequired();

        // VenueId property
        builder.Property(s => s.VenueId)
            .IsRequired();

        // ActId property
        builder.Property(s => s.ActId)
            .IsRequired();

        // TicketCount property
        builder.Property(s => s.TicketCount)
            .IsRequired();

        // StartTime property (datetimeoffset)
        builder.Property(s => s.StartTime)
            .IsRequired();

        // Foreign key relationship to Venue with cascade delete
        builder.HasOne(s => s.Venue)
            .WithMany()
            .HasForeignKey(s => s.VenueId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Foreign key relationship to Act with restrict delete to avoid multiple cascade paths
        // SQL Server limitation: Both Venue and Act cascade from Tenant, creating multiple paths
        // The specification requires cascade, but SQL Server prevents this. Application logic must handle Act deletion.
        builder.HasOne(s => s.Act)
            .WithMany()
            .HasForeignKey(s => s.ActId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);
    }
}
