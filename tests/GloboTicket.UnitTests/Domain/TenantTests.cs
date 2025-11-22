using FluentAssertions;
using GloboTicket.Domain.Entities;

namespace GloboTicket.UnitTests.Domain;

public class TenantTests
{
    [Fact]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // Arrange
        var name = "Test Tenant";
        var slug = "test-tenant";

        // Act
        var tenant = new Tenant(name, slug);

        // Assert
        tenant.Name.Should().Be(name);
        tenant.Slug.Should().Be(slug);
        tenant.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithIsActiveFalse_SetsIsActiveToFalse()
    {
        // Arrange
        var name = "Inactive Tenant";
        var slug = "inactive-tenant";

        // Act
        var tenant = new Tenant(name, slug, isActive: false);

        // Assert
        tenant.Name.Should().Be(name);
        tenant.Slug.Should().Be(slug);
        tenant.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithIsActiveTrue_SetsIsActiveToTrue()
    {
        // Arrange
        var name = "Active Tenant";
        var slug = "active-tenant";

        // Act
        var tenant = new Tenant(name, slug, isActive: true);

        // Assert
        tenant.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DefaultConstructor_InitializesProperties_WithDefaultValues()
    {
        // Act
        var tenant = new Tenant();

        // Assert
        tenant.Name.Should().Be(string.Empty);
        tenant.Slug.Should().Be(string.Empty);
        tenant.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        // Arrange
        var name = "Test Tenant";
        var slug = "test-tenant";
        var tenant = new Tenant(name, slug);

        // Act
        var result = tenant.ToString();

        // Assert
        result.Should().Be(name);
    }

    [Fact]
    public void IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var tenant = new Tenant("Test", "test");

        // Assert
        tenant.IsActive.Should().BeTrue();
    }

    [Fact]
    public void InheritsFromEntity()
    {
        // Arrange & Act
        var tenant = new Tenant("Test", "test");

        // Assert
        tenant.Should().BeAssignableTo<Entity>();
    }
}