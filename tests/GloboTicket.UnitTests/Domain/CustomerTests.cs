using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;

namespace GloboTicket.UnitTests.Domain;

public class CustomerTests
{
    [Fact]
    public void GivenNewCustomer_WhenCreated_ThenCanSetAndRetrieveAllProperties()
    {
        // Arrange
        var customerGuid = Guid.NewGuid();
        var name = "Acme Corp";
        var tenantId = 1;

        // Act
        var customer = new Customer
        {
            CustomerGuid = customerGuid,
            Name = name,
            TenantId = tenantId
        };

        // Assert
        customer.CustomerGuid.Should().Be(customerGuid);
        customer.Name.Should().Be(name);
        customer.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public void GivenCustomerGuid_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var customer = new Customer();
        var expectedGuid = Guid.NewGuid();

        // Act
        customer.CustomerGuid = expectedGuid;

        // Assert
        customer.CustomerGuid.Should().Be(expectedGuid);
    }

    [Fact]
    public void GivenNewCustomer_WhenCreated_ThenNameDefaultsToEmptyString()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void GivenCustomerName_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var customer = new Customer();
        var expectedName = "Globomantics";

        // Act
        customer.Name = expectedName;

        // Assert
        customer.Name.Should().Be(expectedName);
    }

    [Fact]
    public void GivenCustomer_WhenChecked_ThenInheritsFromMultiTenantEntity()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.Should().BeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void GivenCustomer_WhenChecked_ThenImplementsITenantEntity()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.Should().BeAssignableTo<ITenantEntity>();
    }

    [Fact]
    public void GivenCustomer_WhenTenantIdSet_ThenCanBeRetrieved()
    {
        // Arrange
        var customer = new Customer();
        var expectedTenantId = 42;

        // Act
        customer.TenantId = expectedTenantId;

        // Assert
        customer.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenCustomer_WhenTenantSet_ThenCanBeRetrieved()
    {
        // Arrange
        var customer = new Customer();
        var expectedTenant = new Tenant("customer-mgmt", "Customer Management Co", "customer-mgmt");

        // Act
        customer.Tenant = expectedTenant;

        // Assert
        customer.Tenant.Should().Be(expectedTenant);
        customer.Tenant!.Name.Should().Be("Customer Management Co");
    }

    [Fact]
    public void GivenCustomer_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange & Act
        var customer = new Customer();

        // Assert
        customer.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenCustomer_WhenEntityPropertiesSet_ThenCanBeRetrieved()
    {
        // Arrange
        var customer = new Customer();
        var expectedId = 123;
        var expectedCreatedAt = new DateTime(2025, 11, 28, 12, 0, 0, DateTimeKind.Utc);
        var expectedUpdatedAt = new DateTime(2025, 11, 28, 14, 30, 0, DateTimeKind.Utc);

        // Act
        customer.Id = expectedId;
        customer.CreatedAt = expectedCreatedAt;
        customer.UpdatedAt = expectedUpdatedAt;

        // Assert
        customer.Id.Should().Be(expectedId);
        customer.CreatedAt.Should().Be(expectedCreatedAt);
        customer.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    [Fact]
    public void GivenCustomer_WhenCastToITenantEntity_ThenCanAccessTenantId()
    {
        // Arrange
        var customer = new Customer();
        var expectedTenantId = 99;
        customer.TenantId = expectedTenantId;

        // Act
        ITenantEntity tenantEntity = customer;

        // Assert
        tenantEntity.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenCustomerWithMultipleProperties_WhenAllPropertiesSet_ThenAllRetainValues()
    {
        // Arrange
        var customer = new Customer();
        var guid = Guid.NewGuid();
        var name = "Test Customer";
        var tenantId = 5;
        var id = 100;
        var createdAt = DateTime.UtcNow;

        // Act
        customer.CustomerGuid = guid;
        customer.Name = name;
        customer.TenantId = tenantId;
        customer.Id = id;
        customer.CreatedAt = createdAt;

        // Assert
        customer.CustomerGuid.Should().Be(guid);
        customer.Name.Should().Be(name);
        customer.TenantId.Should().Be(tenantId);
        customer.Id.Should().Be(id);
        customer.CreatedAt.Should().Be(createdAt);
    }
}
