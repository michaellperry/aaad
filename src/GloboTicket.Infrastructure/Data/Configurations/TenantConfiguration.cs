using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Tenant entity.
/// Defines table structure, constraints, and indexes.
/// </summary>
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    /// <summary>
    /// Configures the Tenant entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        // Table name
        builder.ToTable("Tenants");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        // Name property
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Slug property
        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(50);

        // Unique index on Slug
        builder.HasIndex(t => t.Slug)
            .IsUnique();

        // IsActive property
        builder.Property(t => t.IsActive)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);
    }
}