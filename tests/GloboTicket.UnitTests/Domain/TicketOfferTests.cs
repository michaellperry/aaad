using FluentAssertions;
using GloboTicket.Domain.Entities;

namespace GloboTicket.UnitTests.Domain;

public class TicketOfferTests
{
    [Fact]
    public void GivenValidShowAndProperties_WhenCreatingTicketOffer_ThenTicketOfferIsCreated()
    {
        // Arrange
        var showGuid = Guid.NewGuid();
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Test Venue" };
        venue.GetType().GetProperty("Id")!.SetValue(venue, 1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        act.GetType().GetProperty("Id")!.SetValue(act, 1);
        var show = new Show(act, venue) { ShowGuid = showGuid };
        show.GetType().GetProperty("Id")!.SetValue(show, 1);

        var ticketOfferGuid = Guid.NewGuid();
        var name = "General Admission";
        var price = 50.00m;
        var ticketCount = 100;

        // Act
        var ticketOffer = new TicketOffer(show, ticketOfferGuid, name, price, ticketCount);

        // Assert
        ticketOffer.Should().NotBeNull();
        ticketOffer.ShowId.Should().Be(show.Id);
        ticketOffer.Show.Should().BeSameAs(show);
        ticketOffer.TicketOfferGuid.Should().Be(ticketOfferGuid);
        ticketOffer.Name.Should().Be(name);
        ticketOffer.Price.Should().Be(price);
        ticketOffer.TicketCount.Should().Be(ticketCount);
    }
}
