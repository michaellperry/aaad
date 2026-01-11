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

    [Fact]
    public void GivenShowWithMultipleTicketOffers_WhenCheckingIfCanUpdateTicketOffer_ThenExcludesCurrentOfferFromCapacityCalculation()
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

        // Create two ticket offers: one with 600 tickets (to be updated) and one with 200 tickets
        var offerToUpdate = new TicketOffer(show, Guid.NewGuid(), "General Admission", 50.00m, 600);
        offerToUpdate.GetType().GetProperty("Id")!.SetValue(offerToUpdate, 1);
        var otherOffer = new TicketOffer(show, Guid.NewGuid(), "VIP", 150.00m, 200);
        otherOffer.GetType().GetProperty("Id")!.SetValue(otherOffer, 2);

        // Available capacity for updating offerToUpdate should be: 1000 - 200 = 800
        // (excluding the current offer's 600 tickets from the calculation)

        // Act & Assert
        // Updating to 800 tickets should be allowed (exactly at capacity)
        var canUpdate800 = show.CanUpdateTicketOffer(offerToUpdate, 800);
        canUpdate800.Should().BeTrue("because 800 + 200 (other offer) = 1000, which equals show capacity");

        // Updating to 801 tickets should NOT be allowed (exceeds capacity)
        var canUpdate801 = show.CanUpdateTicketOffer(offerToUpdate, 801);
        canUpdate801.Should().BeFalse("because 801 + 200 (other offer) = 1001, which exceeds show capacity of 1000");
    }
}
