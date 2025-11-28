using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;

namespace GloboTicket.UnitTests.Domain;

public class ActTests
{
    [Fact]
    public void GivenNewAct_WhenCreated_ThenCanSetAndRetrieveAllProperties()
    {
        // Arrange
        var actGuid = Guid.NewGuid();
        var name = "The Rolling Stones";
        var tenantId = 1;

        // Act
        var act = new Act
        {
            ActGuid = actGuid,
            Name = name,
            TenantId = tenantId
        };

        // Assert
        act.ActGuid.Should().Be(actGuid);
        act.Name.Should().Be(name);
        act.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public void GivenActGuid_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var act = new Act();
        var expectedGuid = Guid.NewGuid();

        // Act
        act.ActGuid = expectedGuid;

        // Assert
        act.ActGuid.Should().Be(expectedGuid);
    }

    [Fact]
    public void GivenNewAct_WhenCreated_ThenNameDefaultsToEmptyString()
    {
        // Arrange & Act
        var act = new Act();

        // Assert
        act.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void GivenActName_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var act = new Act();
        var expectedName = "Queen";

        // Act
        act.Name = expectedName;

        // Assert
        act.Name.Should().Be(expectedName);
    }

    [Fact]
    public void GivenAct_WhenChecked_ThenInheritsFromMultiTenantEntity()
    {
        // Arrange & Act
        var act = new Act();

        // Assert
        act.Should().BeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void GivenAct_WhenChecked_ThenImplementsITenantEntity()
    {
        // Arrange & Act
        var act = new Act();

        // Assert
        act.Should().BeAssignableTo<ITenantEntity>();
    }

    [Fact]
    public void GivenAct_WhenTenantIdSet_ThenCanBeRetrieved()
    {
        // Arrange
        var act = new Act();
        var expectedTenantId = 42;

        // Act
        act.TenantId = expectedTenantId;

        // Assert
        act.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenAct_WhenTenantSet_ThenCanBeRetrieved()
    {
        // Arrange
        var act = new Act();
        var expectedTenant = new Tenant("Entertainment Company", "entertainment-co");

        // Act
        act.Tenant = expectedTenant;

        // Assert
        act.Tenant.Should().Be(expectedTenant);
        act.Tenant.Name.Should().Be("Entertainment Company");
    }

    [Fact]
    public void GivenAct_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange & Act
        var act = new Act();

        // Assert
        act.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenAct_WhenEntityPropertiesSet_ThenCanBeRetrieved()
    {
        // Arrange
        var act = new Act();
        var expectedId = 123;
        var expectedCreatedAt = new DateTime(2025, 11, 28, 12, 0, 0, DateTimeKind.Utc);
        var expectedUpdatedAt = new DateTime(2025, 11, 28, 14, 30, 0, DateTimeKind.Utc);

        // Act
        act.Id = expectedId;
        act.CreatedAt = expectedCreatedAt;
        act.UpdatedAt = expectedUpdatedAt;

        // Assert
        act.Id.Should().Be(expectedId);
        act.CreatedAt.Should().Be(expectedCreatedAt);
        act.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    [Fact]
    public void GivenAct_WhenCastToITenantEntity_ThenCanAccessTenantId()
    {
        // Arrange
        var act = new Act();
        var expectedTenantId = 99;
        act.TenantId = expectedTenantId;

        // Act
        ITenantEntity tenantEntity = act;

        // Assert
        tenantEntity.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenActWithMultipleProperties_WhenAllPropertiesSet_ThenAllRetainValues()
    {
        // Arrange
        var act = new Act();
        var guid = Guid.NewGuid();
        var name = "The Beatles";
        var tenantId = 5;
        var id = 100;
        var createdAt = DateTime.UtcNow;

        // Act
        act.ActGuid = guid;
        act.Name = name;
        act.TenantId = tenantId;
        act.Id = id;
        act.CreatedAt = createdAt;

        // Assert
        act.ActGuid.Should().Be(guid);
        act.Name.Should().Be(name);
        act.TenantId.Should().Be(tenantId);
        act.Id.Should().Be(id);
        act.CreatedAt.Should().Be(createdAt);
    }
}