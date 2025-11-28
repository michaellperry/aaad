using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Interfaces;
using NetTopologySuite.Geometries;

namespace GloboTicket.UnitTests.Domain;

public class VenueTests
{
    [Fact]
    public void GivenNewVenue_WhenCreated_ThenCanSetAndRetrieveAllProperties()
    {
        // Arrange
        var venueGuid = Guid.NewGuid();
        var name = "Madison Square Garden";
        var address = "4 Pennsylvania Plaza, New York, NY 10001";
        var location = new Point(-73.9935, 40.7505) { SRID = 4326 };
        var seatingCapacity = 20000;
        var description = "World-famous arena";
        var tenantId = 1;

        // Act
        var venue = new Venue
        {
            VenueGuid = venueGuid,
            Name = name,
            Address = address,
            Location = location,
            SeatingCapacity = seatingCapacity,
            Description = description,
            TenantId = tenantId
        };

        // Assert
        venue.VenueGuid.Should().Be(venueGuid);
        venue.Name.Should().Be(name);
        venue.Address.Should().Be(address);
        venue.Location.Should().Be(location);
        venue.SeatingCapacity.Should().Be(seatingCapacity);
        venue.Description.Should().Be(description);
        venue.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public void GivenVenueGuid_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedGuid = Guid.NewGuid();

        // Act
        venue.VenueGuid = expectedGuid;

        // Assert
        venue.VenueGuid.Should().Be(expectedGuid);
    }

    [Fact]
    public void GivenNewVenue_WhenCreated_ThenNameDefaultsToEmptyString()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void GivenVenueName_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedName = "The Apollo Theater";

        // Act
        venue.Name = expectedName;

        // Assert
        venue.Name.Should().Be(expectedName);
    }

    [Fact]
    public void GivenNewVenue_WhenCreated_ThenAddressDefaultsToNull()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Address.Should().BeNull();
    }

    [Fact]
    public void GivenVenueAddress_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedAddress = "253 W 125th St, New York, NY 10027";

        // Act
        venue.Address = expectedAddress;

        // Assert
        venue.Address.Should().Be(expectedAddress);
    }

    [Fact]
    public void GivenVenueAddress_WhenSetToNull_ThenRemainsNull()
    {
        // Arrange
        var venue = new Venue
        {
            Address = "123 Test Street"
        };

        // Act
        venue.Address = null;

        // Assert
        venue.Address.Should().BeNull();
    }

    [Fact]
    public void GivenNewVenue_WhenCreated_ThenLocationDefaultsToNull()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Location.Should().BeNull();
    }

    [Fact]
    public void GivenVenueLocation_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedLocation = new Point(-73.9857, 40.7580) { SRID = 4326 };

        // Act
        venue.Location = expectedLocation;

        // Assert
        venue.Location.Should().Be(expectedLocation);
        venue.Location.X.Should().Be(-73.9857);
        venue.Location.Y.Should().Be(40.7580);
        venue.Location.SRID.Should().Be(4326);
    }

    [Fact]
    public void GivenVenueLocation_WhenSetToNull_ThenRemainsNull()
    {
        // Arrange
        var venue = new Venue
        {
            Location = new Point(-73.9857, 40.7580) { SRID = 4326 }
        };

        // Act
        venue.Location = null;

        // Assert
        venue.Location.Should().BeNull();
    }

    [Fact]
    public void GivenNewVenue_WhenCreated_ThenSeatingCapacityDefaultsToZero()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.SeatingCapacity.Should().Be(0);
    }

    [Fact]
    public void GivenVenueSeatingCapacity_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedCapacity = 15000;

        // Act
        venue.SeatingCapacity = expectedCapacity;

        // Assert
        venue.SeatingCapacity.Should().Be(expectedCapacity);
    }

    [Fact]
    public void GivenNewVenue_WhenCreated_ThenDescriptionDefaultsToEmptyString()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void GivenVenueDescription_WhenSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedDescription = "Historic venue known for soul and R&B performances";

        // Act
        venue.Description = expectedDescription;

        // Assert
        venue.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void GivenVenue_WhenChecked_ThenInheritsFromMultiTenantEntity()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Should().BeAssignableTo<MultiTenantEntity>();
    }

    [Fact]
    public void GivenVenue_WhenChecked_ThenImplementsITenantEntity()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Should().BeAssignableTo<ITenantEntity>();
    }

    [Fact]
    public void GivenVenue_WhenTenantIdSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedTenantId = 42;

        // Act
        venue.TenantId = expectedTenantId;

        // Assert
        venue.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenVenue_WhenTenantSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedTenant = new Tenant("venue-mgmt", "Venue Management Co", "venue-mgmt");

        // Act
        venue.Tenant = expectedTenant;

        // Assert
        venue.Tenant.Should().Be(expectedTenant);
        venue.Tenant.Name.Should().Be("Venue Management Co");
    }

    [Fact]
    public void GivenVenue_WhenChecked_ThenInheritsFromEntity()
    {
        // Arrange & Act
        var venue = new Venue();

        // Assert
        venue.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void GivenVenue_WhenEntityPropertiesSet_ThenCanBeRetrieved()
    {
        // Arrange
        var venue = new Venue();
        var expectedId = 123;
        var expectedCreatedAt = new DateTime(2025, 11, 28, 12, 0, 0, DateTimeKind.Utc);
        var expectedUpdatedAt = new DateTime(2025, 11, 28, 14, 30, 0, DateTimeKind.Utc);

        // Act
        venue.Id = expectedId;
        venue.CreatedAt = expectedCreatedAt;
        venue.UpdatedAt = expectedUpdatedAt;

        // Assert
        venue.Id.Should().Be(expectedId);
        venue.CreatedAt.Should().Be(expectedCreatedAt);
        venue.UpdatedAt.Should().Be(expectedUpdatedAt);
    }

    [Fact]
    public void GivenVenue_WhenCastToITenantEntity_ThenCanAccessTenantId()
    {
        // Arrange
        var venue = new Venue();
        var expectedTenantId = 99;
        venue.TenantId = expectedTenantId;

        // Act
        ITenantEntity tenantEntity = venue;

        // Assert
        tenantEntity.TenantId.Should().Be(expectedTenantId);
    }

    [Fact]
    public void GivenVenueWithMultipleProperties_WhenAllPropertiesSet_ThenAllRetainValues()
    {
        // Arrange
        var venue = new Venue();
        var guid = Guid.NewGuid();
        var name = "Test Venue";
        var address = "123 Test St";
        var location = new Point(-118.2437, 34.0522) { SRID = 4326 };
        var capacity = 5000;
        var description = "Test Description";
        var tenantId = 5;
        var id = 100;
        var createdAt = DateTime.UtcNow;

        // Act
        venue.VenueGuid = guid;
        venue.Name = name;
        venue.Address = address;
        venue.Location = location;
        venue.SeatingCapacity = capacity;
        venue.Description = description;
        venue.TenantId = tenantId;
        venue.Id = id;
        venue.CreatedAt = createdAt;

        // Assert
        venue.VenueGuid.Should().Be(guid);
        venue.Name.Should().Be(name);
        venue.Address.Should().Be(address);
        venue.Location.Should().Be(location);
        venue.SeatingCapacity.Should().Be(capacity);
        venue.Description.Should().Be(description);
        venue.TenantId.Should().Be(tenantId);
        venue.Id.Should().Be(id);
        venue.CreatedAt.Should().Be(createdAt);
    }
}