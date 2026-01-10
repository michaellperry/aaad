using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Act entity.
/// Defines table structure, constraints, indexes, and multi-tenant support.
/// </summary>
public class ActConfiguration : IEntityTypeConfiguration<Act>
{
    /// <summary>
    /// Configures the Act entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Act> builder)
    {
        // Table name
        builder.ToTable("Acts");

        // Primary key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        // Composite alternate key for multi-tenant uniqueness
        builder.HasAlternateKey(a => new { a.TenantId, a.ActGuid });

        // Index on ActGuid for queries
        builder.HasIndex(a => a.ActGuid);

        // ActGuid property
        builder.Property(a => a.ActGuid)
            .IsRequired();

        // Name property
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Foreign key relationship to Tenant with cascade delete
        builder.HasOne(a => a.Tenant)
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);
    }
}
