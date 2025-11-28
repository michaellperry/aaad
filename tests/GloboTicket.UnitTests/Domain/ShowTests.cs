using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;

namespace GloboTicket.UnitTests.Domain;

public class ShowTests
{
    [Fact]
    public void GivenVenueAndAct_WhenShowCreated_ThenShowIsCreated()
    {
        // Arrange
        var venue = new Venue { Name = "Madison Square Garden" };
        var act = new Act { Name = "The Rolling Stones" };

        // Act
        var show = new Show(venue, act);

        // Assert
        show.Should().NotBeNull();
    }

    [Fact]
    public void GivenVenueAndAct_WhenShowCreated_ThenVenueIsSet()
    {
        // Arrange
        var venue = new Venue { Name = "Madison Square Garden" };
        var act = new Act { Name = "The Rolling Stones" };

        // Act
        var show = new Show(venue, act);

        // Assert
        show.Venue.Should().Be(venue);
        show.Venue.Name.Should().Be("Madison Square Garden");
    }

    [Fact]
    public void GivenVenueAndAct_WhenShowCreated_ThenActIsSet()
    {
        // Arrange
        var venue = new Venue { Name = "Madison Square Garden" };
        var act = new Act { Name = "The Rolling Stones" };

        // Act
        var show = new Show(venue, act);

        // Assert
        show.Act.Should().Be(act);
        show.Act.Name.Should().Be("The Rolling Stones");
    }

    [Fact]
    public void GivenShowGuid_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var expectedGuid = Guid.NewGuid();

        // Act
        show.ShowGuid = expectedGuid;

        // Assert
        show.ShowGuid.Should().Be(expectedGuid);
    }

    [Fact]
    public void GivenShowDate_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var expectedDate = new DateTimeOffset(2025, 12, 31, 20, 0, 0, TimeSpan.FromHours(-6));

        // Act
        show.Date = expectedDate;

        // Assert
        show.Date.Should().Be(expectedDate);
    }

    [Fact]
    public void GivenNewShow_WhenCreated_ThenTicketSalesCollectionIsInitialized()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };

        // Act
        var show = new Show(venue, act);

        // Assert
        show.TicketSales.Should().NotBeNull();
        show.TicketSales.Should().BeEmpty();
    }

    [Fact]
    public void GivenNewShow_WhenCreated_ThenTicketSalesCollectionIsEmptyList()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };

        // Act
        var show = new Show(venue, act);

        // Assert
        show.TicketSales.Should().BeAssignableTo<ICollection<TicketSale>>();
        show.TicketSales.Count.Should().Be(0);
    }

    [Fact]
    public void GivenTicketSalesCollection_WhenItemAdded_ThenCountIncreases()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var ticketSale = new TicketSale(show);

        // Act
        show.TicketSales.Add(ticketSale);

        // Assert
        show.TicketSales.Count.Should().Be(1);
        show.TicketSales.Should().Contain(ticketSale);
    }


    [Fact]
    public void GivenShow_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };

        // Act
        var show = new Show(venue, act);

        // Assert
        show.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenShow_WhenEntityPropertiesSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue { Name = "Test Venue" };
        var act = new Act { Name = "Test Act" };
        var show = new Show(venue, act);
        var expectedId = 123;
        var expectedCreatedAt = new DateTime(2025, 11, 28, 12, 0, 0, DateTimeKind.Utc);
        var expectedUpdatedAt = new DateTime(2025, 11, 28, 14, 30, 0, DateTimeKind.Utc);

        // Act
        show.Id = expectedId;
        show.CreatedAt = expectedCreatedAt;
        show.UpdatedAt = expectedUpdatedAt;

        // Assert
        show.Id.Should().Be(expectedId);
        show.CreatedAt.Should().Be(expectedCreatedAt);
        show.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    [Fact]
    public void GivenShowWithAllProperties_WhenSet_ThenAllRetainValues()
    {
        // Arrange
        var venue = new Venue { Name = "Madison Square Garden" };
        var act = new Act { Name = "The Rolling Stones" };
        var show = new Show(venue, act);
        var showGuid = Guid.NewGuid();
        var date = new DateTimeOffset(2025, 12, 31, 20, 0, 0, TimeSpan.FromHours(-6));
        var id = 100;
        var createdAt = DateTime.UtcNow;

        // Act
        show.ShowGuid = showGuid;
        show.Date = date;
        show.Id = id;
        show.CreatedAt = createdAt;

        // Assert
        show.ShowGuid.Should().Be(showGuid);
        show.Venue.Should().Be(venue);
        show.Act.Should().Be(act);
        show.Date.Should().Be(date);
        show.Id.Should().Be(id);
        show.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void GivenDifferentVenueAndAct_WhenShowCreated_ThenCorrectReferencesAreSet()
    {
        // Arrange
        var venue1 = new Venue { Name = "Venue 1" };
        var venue2 = new Venue { Name = "Venue 2" };
        var act1 = new Act { Name = "Act 1" };
        var act2 = new Act { Name = "Act 2" };

        // Act
        var show1 = new Show(venue1, act1);
        var show2 = new Show(venue2, act2);

        // Assert
        show1.Venue.Should().Be(venue1);
        show1.Act.Should().Be(act1);
        show2.Venue.Should().Be(venue2);
        show2.Act.Should().Be(act2);
    }

    [Fact]
    public void GivenShowWithAllProperties_WhenCreated_ThenCanSetAndRetrieveAllProperties()
    {
        // Arrange
        var venue = new Venue { Name = "The O2 Arena" };
        var act = new Act { Name = "Coldplay" };
        var showGuid = Guid.NewGuid();
        var date = new DateTimeOffset(2026, 6, 15, 19, 30, 0, TimeSpan.Zero);

        // Act
        var show = new Show(venue, act)
        {
            ShowGuid = showGuid,
            Date = date
        };

        // Assert
        show.ShowGuid.Should().Be(showGuid);
        show.Venue.Should().Be(venue);
        show.Venue.Name.Should().Be("The O2 Arena");
        show.Act.Should().Be(act);
        show.Act.Name.Should().Be("Coldplay");
        show.Date.Should().Be(date);
        show.TicketSales.Should().NotBeNull();
        show.TicketSales.Should().BeEmpty();
    }
}