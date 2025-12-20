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

        // Configure BillingAddress as a required complex type
        builder.ComplexProperty(c => c.BillingAddress, address =>
        {
            address.Property(a => a.StreetLine1)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("BillingStreetLine1");

            address.Property(a => a.StreetLine2)
                .HasMaxLength(200)
                .HasColumnName("BillingStreetLine2");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("BillingCity");

            address.Property(a => a.StateOrProvince)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("BillingStateOrProvince");

            address.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("BillingPostalCode");

            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("BillingCountry");
        });

        // Configure ShippingAddress as an optional complex type
        // When present, all properties except StreetLine2 are required
        builder.ComplexProperty(c => c.ShippingAddress, address =>
        {
            address.Property(a => a.StreetLine1)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ShippingStreetLine1");

            address.Property(a => a.StreetLine2)
                .HasMaxLength(200)
                .HasColumnName("ShippingStreetLine2");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingCity");

            address.Property(a => a.StateOrProvince)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingStateOrProvince");

            address.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("ShippingPostalCode");

            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("ShippingCountry");
        });
    }
}

