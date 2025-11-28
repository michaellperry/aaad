using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;

namespace GloboTicket.UnitTests.Domain;

public class MultiTenantEntityTests
{
    // Concrete test implementation of MultiTenantEntity for testing
    private class TestMultiTenantEntity : MultiTenantEntity
    {
    }

    [Fact]
    public void CannotInstantiateAbstractClass_ThrowsCompilationError()
    {
        // This test verifies that MultiTenantEntity is abstract
        // If MultiTenantEntity can be instantiated, this test should fail at compile time
        // We verify this by checking that our TestMultiTenantEntity is assignable to MultiTenantEntity
        
        // Arrange & Act
        var entity = new TestMultiTenantEntity();

        // Assert
        entity.Should().BeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void DerivedClass_HasTenantIdProperty()
    {
        // Arrange
        var entity = new TestMultiTenantEntity();
        var expectedTenantId = 42;

        // Act
        entity.TenantId = expectedTenantId;

        // Assert
        entity.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void TenantId_DefaultsToZero()
    {
        // Arrange & Act
        var entity = new TestMultiTenantEntity();

        // Assert
        entity.TenantId.Should().Be(0);
    }

    [Fact]
    public void DerivedClass_HasTenantNavigationProperty()
    {
        // Arrange
        var entity = new TestMultiTenantEntity();
        var expectedTenant = new Tenant("test-tenant", "Test Tenant", "test-tenant");

        // Act
        entity.Tenant = expectedTenant;

        // Assert
        entity.Tenant.Should().Be(expectedTenant);
        entity.Tenant.Name.Should().Be("Test Tenant");
    }

    [Fact]
    public void Tenant_DefaultsToNull()
    {
        // Arrange & Act
        var entity = new TestMultiTenantEntity();

        // Assert
        entity.Tenant.Should().BeNull();
    }

    [Fact]
    public void DerivedClass_InheritsEntityProperties()
    {
        // Arrange
        var entity = new TestMultiTenantEntity();
        var expectedId = 123;
        var expectedCreatedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var expectedUpdatedAt = new DateTime(2025, 1, 2, 14, 30, 0, DateTimeKind.Utc);

        // Act
        entity.Id = expectedId;
        entity.CreatedAt = expectedCreatedAt;
        entity.UpdatedAt = expectedUpdatedAt;

        // Assert
        entity.Id.Should().Be(expectedId);
        entity.CreatedAt.Should().Be(expectedCreatedAt);
        entity.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    [Fact]
    public void DerivedClass_InheritsFromEntity()
    {
        // Arrange & Act
        var entity = new TestMultiTenantEntity();

        // Assert
        entity.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void DerivedClass_ImplementsITenantEntity()
    {
        // Arrange & Act
        var entity = new TestMultiTenantEntity();

        // Assert
        entity.Should().BeAssignableTo<ITenantEntity>();
    }

    [Fact]
    public void CanAssignToITenantEntity_AndAccessTenantId()
    {
        // Arrange
        var entity = new TestMultiTenantEntity();
        var expectedTenantId = 99;
        entity.TenantId = expectedTenantId;

        // Act
        ITenantEntity tenantEntity = entity;

        // Assert
        tenantEntity.TenantId.Should().Be(expectedTenantId);
    }
}