using FluentAssertions;
using GloboTicket.Domain.Entities;

namespace GloboTicket.UnitTests.Domain;

public class ShowTests
{
    [Fact]
    public void GivenNewShow_WhenCreated_ThenShowGuidDefaultsToEmptyGuid()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.ShowGuid.Should().Be(Guid.Empty);
    }

    [Fact]
    public void GivenNewShow_WhenCreated_ThenTicketCountDefaultsToZero()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.TicketCount.Should().Be(0);
    }

    [Fact]
    public void GivenNewShow_WhenCreated_ThenStartTimeDefaultsToDefault()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.StartTime.Should().Be(default(DateTimeOffset));
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenDoesNotInheritFromMultiTenantEntity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.Should().NotBeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenHasVenueNavigationProperty()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.Venue.Should().NotBeNull();
        show.Venue.Should().BeSameAs(venue);
    }

    [Fact]
    public void GivenShow_WhenChecked_ThenHasActNavigationProperty()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.Act.Should().NotBeNull();
        show.Act.Should().BeSameAs(act);
    }

    [Fact]
    public void GivenNullAct_WhenCreatingShow_ThenThrowsArgumentNullException()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);

        // Act
        var createShow = () => new Show(null!, venue);

        // Assert
        createShow.Should().Throw<ArgumentNullException>()
            .WithParameterName("act");
    }

    [Fact]
    public void GivenNullVenue_WhenCreatingShow_ThenThrowsArgumentNullException()
    {
        // Arrange
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);

        // Act
        var createShow = () => new Show(act, null!);

        // Assert
        createShow.Should().Throw<ArgumentNullException>()
            .WithParameterName("venue");
    }

    [Fact]
    public void GivenValidActAndVenue_WhenCreatingShow_ThenSetsActIdFromActEntity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 42);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 99);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.ActId.Should().Be(99);
    }

    [Fact]
    public void GivenValidActAndVenue_WhenCreatingShow_ThenSetsVenueIdFromVenueEntity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 42);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 99);

        // Act
        var show = new Show(act, venue);

        // Assert
        show.VenueId.Should().Be(42);
    }
}
