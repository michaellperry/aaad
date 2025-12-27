using FluentAssertions;
using GloboTicket.Domain.Entities;

namespace GloboTicket.UnitTests.Domain;

public class ShowTests
{
    [Fact]
    public void GivenNewShow_WhenCreated_ThenShowGuidDefaultsToEmptyGuid()
    {
        // Arrange & Act
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.ShowGuid.Should().Be(Guid.Empty);
    }

    [Fact]
    public void GivenNewShow_WhenCreated_ThenTicketCountDefaultsToZero()
    {
        // Arrange & Act
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.TicketCount.Should().Be(0);
    }

    [Fact]
    public void GivenNewShow_WhenCreated_ThenStartTimeDefaultsToDefault()
    {
        // Arrange & Act
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.StartTime.Should().Be(default(DateTimeOffset));
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        // Act
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenDoesNotInheritFromMultiTenantEntity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        // Act
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.Should().NotBeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenHasVenueNavigationProperty()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        // Act
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.Venue.Should().NotBeNull();
        show.Venue.Should().BeSameAs(venue);
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenHasActNavigationProperty()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        // Act
        var show = new Show { Venue = venue, Act = act };

        // Assert
        show.Act.Should().NotBeNull();
        show.Act.Should().BeSameAs(act);
    }
}
