using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Customer entity.
/// Defines table structure, constraints, indexes, and multi-tenant support.
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <summary>
    /// Configures the Customer entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Table name
        builder.ToTable("Customers");

        // Primary key
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        // Composite alternate key for multi-tenant uniqueness
        builder.HasAlternateKey(c => new { c.TenantId, c.CustomerGuid });

        // Index on CustomerGuid for queries
        builder.HasIndex(c => c.CustomerGuid);

        // CustomerGuid property
        builder.Property(c => c.CustomerGuid)
            .IsRequired();

        // Name property
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(250);

        // Foreign key relationship to Tenant with cascade delete
        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // CreatedAt property (inherited from Entity)
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // UpdatedAt property (inherited from Entity)
        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);
    }
}

