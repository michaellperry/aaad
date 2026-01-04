using FluentAssertions;
using GloboTicket.Domain.Entities;

namespace GloboTicket.UnitTests.Domain;

public class ShowTests
{
    [Fact]
    public void GivenShowWithExistingOffers_WhenValidatingNewOfferCapacity_ThenReturnsTrueIfCapacityAvailable()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) 
        { 
            ShowGuid = Guid.NewGuid(),
            TicketCount = 1000
        };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        // Create existing ticket offers that allocate 600 tickets
        var offer1 = new TicketOffer(show, Guid.NewGuid(), "General Admission", 50.00m, 400);
        var offer2 = new TicketOffer(show, Guid.NewGuid(), "VIP", 150.00m, 200);
        
        // Available capacity should be: 1000 - (400 + 200) = 400

        // Act
        var canAddOffer = show.CanAddTicketOffer(300);

        // Assert
        canAddOffer.Should().BeTrue("because 300 tickets is less than the available capacity of 400");
    }

    [Fact]
    public void GivenShowWithExistingOffers_WhenValidatingNewOfferExceedingCapacity_ThenReturnsFalse()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) 
        { 
            ShowGuid = Guid.NewGuid(),
            TicketCount = 1000
        };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        // Create existing ticket offers that allocate 800 tickets
        var offer1 = new TicketOffer(show, Guid.NewGuid(), "General Admission", 50.00m, 600);
        var offer2 = new TicketOffer(show, Guid.NewGuid(), "VIP", 150.00m, 200);
        
        // Available capacity should be: 1000 - (600 + 200) = 200

        // Act
        var canAddOffer = show.CanAddTicketOffer(300);

        // Assert
        canAddOffer.Should().BeFalse("because 300 tickets exceeds the available capacity of 200");
    }

    [Fact]
    public void GivenShowWithExistingOffers_WhenValidatingNewOfferWithExactRemainingCapacity_ThenReturnsTrue()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) 
        { 
            ShowGuid = Guid.NewGuid(),
            TicketCount = 1000
        };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        // Create existing ticket offers that allocate 800 tickets
        var offer1 = new TicketOffer(show, Guid.NewGuid(), "General Admission", 50.00m, 600);
        var offer2 = new TicketOffer(show, Guid.NewGuid(), "VIP", 150.00m, 200);
        
        // Available capacity should be: 1000 - (600 + 200) = 200

        // Act
        var canAddOffer = show.CanAddTicketOffer(200);

        // Assert
        canAddOffer.Should().BeTrue("because 200 tickets exactly matches the available capacity of 200");
    }

    [Fact]
    public void GivenShowWithNoOffers_WhenValidatingNewOffer_ThenReturnsTrueIfWithinTotalCapacity()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) 
        { 
            ShowGuid = Guid.NewGuid(),
            TicketCount = 1000
        };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        // No existing offers, so available capacity = 1000

        // Act
        var canAddOffer = show.CanAddTicketOffer(500);

        // Assert
        canAddOffer.Should().BeTrue("because 500 tickets is less than the total capacity of 1000");
    }

    [Fact]
    public void GivenShowWithNoOffers_WhenValidatingNewOfferExceedingTotalCapacity_ThenReturnsFalse()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) 
        { 
            ShowGuid = Guid.NewGuid(),
            TicketCount = 1000
        };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        // No existing offers, so available capacity = 1000

        // Act
        var canAddOffer = show.CanAddTicketOffer(1500);

        // Assert
        canAddOffer.Should().BeFalse("because 1500 tickets exceeds the total capacity of 1000");
    }

    [Fact]
    public void GivenShow_WhenCalculatingAvailableCapacity_ThenReturnsCorrectValue()
    {
        // Arrange
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) 
        { 
            ShowGuid = Guid.NewGuid(),
            TicketCount = 1000
        };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        // Create existing ticket offers
        var offer1 = new TicketOffer(show, Guid.NewGuid(), "General Admission", 50.00m, 600);
        var offer2 = new TicketOffer(show, Guid.NewGuid(), "VIP", 150.00m, 200);

        // Act
        var availableCapacity = show.GetAvailableCapacity();

        // Assert
        availableCapacity.Should().Be(200, "because 1000 - (600 + 200) = 200");
    }
}
