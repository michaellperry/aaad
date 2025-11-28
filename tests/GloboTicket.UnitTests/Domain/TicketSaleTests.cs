using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;

namespace GloboTicket.UnitTests.Domain;

public class TicketSaleTests
{
    [Fact]
    public void GivenShow_WhenTicketSaleCreated_ThenTicketSaleIsCreated()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);

        // Act
        var ticketSale = new TicketSale(show);

        // Assert
        ticketSale.Should().NotBeNull();
    }

    [Fact]
    public void GivenShow_WhenTicketSaleCreated_ThenShowIsSet()
    {
        // Arrange
        var venue = new Venue { Name = "Madison Square Garden" };
        var act = new Act { Name = "The Rolling Stones" };
        var show = new Show(venue, act);

        // Act
        var ticketSale = new TicketSale(show);

        // Assert
        ticketSale.Show.Should().Be(show);
        ticketSale.Show.Venue.Name.Should().Be("Madison Square Garden");
        ticketSale.Show.Act.Name.Should().Be("The Rolling Stones");
    }

    [Fact]
    public void GivenTicketSaleGuid_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedGuid = Guid.NewGuid();

        // Act
        ticketSale.TicketSaleGuid = expectedGuid;

        // Assert
        ticketSale.TicketSaleGuid.Should().Be(expectedGuid);
    }

    [Fact]
    public void GivenShowId_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedShowId = 42;

        // Act
        ticketSale.ShowId = expectedShowId;

        // Assert
        ticketSale.ShowId.Should().Be(expectedShowId);
    }

    [Fact]
    public void GivenQuantity_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedQuantity = 5;

        // Act
        ticketSale.Quantity = expectedQuantity;

        // Assert
        ticketSale.Quantity.Should().Be(expectedQuantity);
    }

    [Fact]
    public void GivenNewTicketSale_WhenCreated_ThenQuantityDefaultsToZero()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);

        // Act
        var ticketSale = new TicketSale(show);

        // Assert
        ticketSale.Quantity.Should().Be(0);
    }

    [Fact]
    public void GivenTicketSale_WhenChecked_ThenInheritsFromMultiTenantEntity()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);

        // Act
        var ticketSale = new TicketSale(show);

        // Assert
        ticketSale.Should().BeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void GivenTicketSale_WhenChecked_ThenImplementsITenantEntity()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);

        // Act
        var ticketSale = new TicketSale(show);

        // Assert
        ticketSale.Should().BeAssignableTo<ITenantEntity>();
    }

    [Fact]
    public void GivenTicketSale_WhenTenantIdSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedTenantId = 99;

        // Act
        ticketSale.TenantId = expectedTenantId;

        // Assert
        ticketSale.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenTicketSale_WhenTenantSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedTenant = new Tenant("Ticket Sales Company", "ticket-sales");

        // Act
        ticketSale.Tenant = expectedTenant;

        // Assert
        ticketSale.Tenant.Should().Be(expectedTenant);
        ticketSale.Tenant.Name.Should().Be("Ticket Sales Company");
    }

    [Fact]
    public void GivenTicketSale_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);

        // Act
        var ticketSale = new TicketSale(show);

        // Assert
        ticketSale.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenTicketSale_WhenEntityPropertiesSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedId = 123;
        var expectedCreatedAt = new DateTime(2025, 11, 28, 12, 0, 0, DateTimeKind.Utc);
        var expectedUpdatedAt = new DateTime(2025, 11, 28, 14, 30, 0, DateTimeKind.Utc);

        // Act
        ticketSale.Id = expectedId;
        ticketSale.CreatedAt = expectedCreatedAt;
        ticketSale.UpdatedAt = expectedUpdatedAt;

        // Assert
        ticketSale.Id.Should().Be(expectedId);
        ticketSale.CreatedAt.Should().Be(expectedCreatedAt);
        ticketSale.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    [Fact]
    public void GivenTicketSale_WhenCastToITenantEntity_ThenCanAccessTenantId()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var expectedTenantId = 77;
        ticketSale.TenantId = expectedTenantId;

        // Act
        ITenantEntity tenantEntity = ticketSale;

        // Assert
        tenantEntity.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenTicketSaleWithAllProperties_WhenSet_ThenAllRetainValues()
    {
        // Arrange
        var venue = new Venue { Name = "The O2 Arena" };
        var act = new Act { Name = "Coldplay" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);
        var ticketSaleGuid = Guid.NewGuid();
        var showId = 50;
        var quantity = 10;
        var tenantId = 5;
        var id = 100;
        var createdAt = DateTime.UtcNow;

        // Act
        ticketSale.TicketSaleGuid = ticketSaleGuid;
        ticketSale.ShowId = showId;
        ticketSale.Quantity = quantity;
        ticketSale.TenantId = tenantId;
        ticketSale.Id = id;
        ticketSale.CreatedAt = createdAt;

        // Assert
        ticketSale.TicketSaleGuid.Should().Be(ticketSaleGuid);
        ticketSale.Show.Should().Be(show);
        ticketSale.ShowId.Should().Be(showId);
        ticketSale.Quantity.Should().Be(quantity);
        ticketSale.TenantId.Should().Be(tenantId);
        ticketSale.Id.Should().Be(id);
        ticketSale.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void GivenDifferentShows_WhenTicketSalesCreated_ThenCorrectReferencesAreSet()
    {
        // Arrange
        var venue1 = new Venue { Name = "Venue 1" };
        var act1 = new Act { Name = "Act 1" };
        var show1 = new Show(venue1, act1);

        var venue2 = new Venue { Name = "Venue 2" };
        var act2 = new Act { Name = "Act 2" };
        var show2 = new Show(venue2, act2);

        // Act
        var ticketSale1 = new TicketSale(show1);
        var ticketSale2 = new TicketSale(show2);

        // Assert
        ticketSale1.Show.Should().Be(show1);
        ticketSale1.Show.Venue.Name.Should().Be("Venue 1");
        ticketSale1.Show.Act.Name.Should().Be("Act 1");
        ticketSale2.Show.Should().Be(show2);
        ticketSale2.Show.Venue.Name.Should().Be("Venue 2");
        ticketSale2.Show.Act.Name.Should().Be("Act 2");
    }

    [Fact]
    public void GivenTicketSaleWithAllProperties_WhenCreated_ThenCanSetAndRetrieveAllProperties()
    {
        // Arrange
        var venue = new Venue { Name = "Madison Square Garden" };
        var act = new Act { Name = "The Rolling Stones" };
        var show = new Show(venue, act);
        var ticketSaleGuid = Guid.NewGuid();
        var showId = 25;
        var quantity = 3;
        var tenantId = 10;

        // Act
        var ticketSale = new TicketSale(show)
        {
            TicketSaleGuid = ticketSaleGuid,
            ShowId = showId,
            Quantity = quantity,
            TenantId = tenantId
        };

        // Assert
        ticketSale.TicketSaleGuid.Should().Be(ticketSaleGuid);
        ticketSale.Show.Should().Be(show);
        ticketSale.Show.Venue.Name.Should().Be("Madison Square Garden");
        ticketSale.Show.Act.Name.Should().Be("The Rolling Stones");
        ticketSale.ShowId.Should().Be(showId);
        ticketSale.Quantity.Should().Be(quantity);
        ticketSale.TenantId.Should().Be(tenantId);
    }
}