// Temporarily commented out - Venue class not yet implemented
/*
using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;
using NetTopologySuite.Geometries;

namespace GloboTicket.UnitTests.Domain;

public class VenueTests
{
    [Fact]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test Venue";
        var address = "123 Test Street";
        var location = new Point(40.7128, -74.0060) { SRID = 4326 };
        var seatingCapacity = 1000;
        var tenantId = 1;

        // Act
        var venue = new Venue(id, name, address, location, seatingCapacity, tenantId);

        // Assert
        venue.Id.Should().Be(id);
        venue.Name.Should().Be(name);
        venue.Address.Should().Be(address);
        venue.Location.Should().Be(location);
        venue.SeatingCapacity.Should().Be(seatingCapacity);
        venue.TenantId.Should().Be(tenantId);
        venue.Should().BeAssignableTo<ITenantEntity>();
    }
}
*/