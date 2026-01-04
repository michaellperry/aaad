using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Services;
using GloboTicket.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests for TicketOffer API endpoints verifying behavior patterns.
/// Tests simulate endpoint behavior including authentication requirements, capacity validation,
/// and tenant isolation through the service layer.
/// Note: These tests verify service-level behavior that corresponds to API endpoint requirements.
/// </summary>
public class TicketOfferEndpointsIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly int _testTenantSeed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicketOfferEndpointsIntegrationTests"/> class.
    /// </summary>
    /// <param name="fixture">The database fixture providing the SQL Server container.</param>
    public TicketOfferEndpointsIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _testTenantSeed = _fixture.GenerateRandomTenantId();
    }

    #region Create Ticket Offer Tests

    [Fact]
    public async Task CreateTicketOffer_WithValidData_Returns201Created()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 600
        };

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.CreateTicketOfferAsync(showGuid, dto);

            // Assert - Simulates 201 Created response
            result.Should().NotBeNull("endpoint should return 201 Created with ticket offer data");
            result.TicketOfferGuid.Should().Be(dto.TicketOfferGuid);
            result.Name.Should().Be(dto.Name);
            result.Price.Should().Be(dto.Price);
            result.TicketCount.Should().Be(dto.TicketCount);
        }
    }

    [Fact]
    public async Task CreateTicketOffer_WithValidData_ReturnsOfferInBody()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "VIP",
            Price = 150.00m,
            TicketCount = 200
        };

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.CreateTicketOfferAsync(showGuid, dto);

            // Assert - Verify response body contains complete offer data
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.ShowGuid.Should().Be(showGuid);
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task CreateTicketOffer_WithTicketCountExceedingCapacity_Returns400BadRequest()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        
        // Create first offer using 800 tickets
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 800);

        // Try to create second offer with 300 tickets (exceeds remaining 200)
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "VIP",
            Price = 150.00m,
            TicketCount = 300
        };

        // Act & Assert - Service throws ArgumentException which maps to 400
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.CreateTicketOfferAsync(showGuid, dto);

            await act.Should().ThrowAsync<ArgumentException>(
                "endpoint should return 400 Bad Request when capacity exceeded");
        }
    }

    [Fact]
    public async Task CreateTicketOffer_WithTicketCountExceedingCapacity_ReturnsSpecificErrorMessage()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        
        // Create first offer using 850 tickets
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 850);

        // Try to create second offer with 200 tickets (exceeds remaining 150)
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "VIP",
            Price = 150.00m,
            TicketCount = 200
        };

        // Act & Assert
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.CreateTicketOfferAsync(showGuid, dto);

            var exception = await act.Should().ThrowAsync<ArgumentException>();
            exception.Which.Message.Should().Contain("150",
                "error message should show remaining capacity");
        }
    }

    [Fact]
    public async Task CreateTicketOffer_WithExactRemainingCapacity_CreatesSuccessfully()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        
        // Create first offer using 600 tickets
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 600);

        // Create second offer with exact remaining capacity (400)
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "VIP",
            Price = 150.00m,
            TicketCount = 400
        };

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.CreateTicketOfferAsync(showGuid, dto);

            // Assert - Should succeed with exact capacity
            result.Should().NotBeNull();
            result.TicketCount.Should().Be(400);
        }
    }

    [Fact]
    public async Task CreateTicketOffer_WithNonExistentShow_Returns404NotFound()
    {
        // Arrange
        var nonExistentShowGuid = Guid.NewGuid();
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 100
        };

        // Act & Assert - Service throws KeyNotFoundException which maps to 404
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, _testTenantSeed);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.CreateTicketOfferAsync(nonExistentShowGuid, dto);

            await act.Should().ThrowAsync<KeyNotFoundException>(
                "endpoint should return 404 Not Found for non-existent show");
        }
    }

    [Fact]
    public async Task CreateTicketOffer_ShowFromDifferentTenant_Returns404NotFound()
    {
        // Arrange - Create show in different tenant
        var (showGuid, otherTenantId) = await CreateTestShowAsync(capacity: 1000);
        var (_, myTenantId) = await CreateTestShowAsync(capacity: 1000);

        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 100
        };

        // Act & Assert - Try to create offer from different tenant context
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, myTenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.CreateTicketOfferAsync(showGuid, dto);

            await act.Should().ThrowAsync<KeyNotFoundException>(
                "endpoint should return 404 Not Found for cross-tenant show access");
        }
    }

    [Fact]
    public async Task CreateTicketOffer_StoresPriceWithTwoDecimalPlaces()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "General Admission",
            Price = 50.99m,
            TicketCount = 100
        };

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            await service.CreateTicketOfferAsync(showGuid, dto);
        }

        // Assert - Verify price stored correctly
        using (var verifyContext = _fixture.CreateDbContext(_fixture.ConnectionString, tenantId))
        {
            var offer = await verifyContext.TicketOffers
                .FirstOrDefaultAsync(o => o.TicketOfferGuid == dto.TicketOfferGuid);
            
            offer.Should().NotBeNull();
            offer!.Price.Should().Be(50.99m);
        }
    }

    [Fact]
    public async Task CreateTicketOffer_SetsCreatedAtTimestamp()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 100
        };

        var beforeCreate = DateTime.UtcNow;

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            await service.CreateTicketOfferAsync(showGuid, dto);
        }

        var afterCreate = DateTime.UtcNow;

        // Assert - Verify CreatedAt timestamp
        using (var verifyContext = _fixture.CreateDbContext(_fixture.ConnectionString, tenantId))
        {
            var offer = await verifyContext.TicketOffers
                .FirstOrDefaultAsync(o => o.TicketOfferGuid == dto.TicketOfferGuid);
            
            offer.Should().NotBeNull();
            offer!.CreatedAt.Should().BeOnOrAfter(beforeCreate);
            offer.CreatedAt.Should().BeOnOrBefore(afterCreate);
        }
    }

    #endregion

    #region Get Ticket Offers Tests

    [Fact]
    public async Task GetTicketOffers_WithValidShowGuid_Returns200Ok()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 600);

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetTicketOffersByShowAsync(showGuid);

            // Assert - Simulates 200 OK response
            result.Should().NotBeNull("endpoint should return 200 OK with offers list");
        }
    }

    [Fact]
    public async Task GetTicketOffers_WithValidShowGuid_ReturnsOffersList()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        var offer1Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 600, name: "General Admission");
        var offer2Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 200, name: "VIP");

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetTicketOffersByShowAsync(showGuid);

            // Assert - Verify response contains all offers
            var offersList = result.ToList();
            offersList.Should().HaveCount(2);
            offersList.Should().Contain(o => o.TicketOfferGuid == offer1Guid);
            offersList.Should().Contain(o => o.TicketOfferGuid == offer2Guid);
        }
    }

    [Fact]
    public async Task GetTicketOffers_WithNoOffers_ReturnsEmptyList()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetTicketOffersByShowAsync(showGuid);

            // Assert - Should return empty list, not null
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetTicketOffers_WithNonExistentShow_Returns404NotFound()
    {
        // Arrange
        var nonExistentShowGuid = Guid.NewGuid();

        // Act & Assert - Service throws KeyNotFoundException which maps to 404
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, _testTenantSeed);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.GetTicketOffersByShowAsync(nonExistentShowGuid);

            await act.Should().ThrowAsync<KeyNotFoundException>(
                "endpoint should return 404 Not Found for non-existent show");
        }
    }

    [Fact]
    public async Task GetTicketOffers_ShowFromDifferentTenant_Returns404NotFound()
    {
        // Arrange - Create shows in two different tenants
        var (show1Guid, tenant1Id) = await CreateTestShowAsync(capacity: 1000);
        var (show2Guid, tenant2Id) = await CreateTestShowAsync(capacity: 1000);

        // Act & Assert - Try to get tenant1's show from tenant2 context
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenant2Id);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.GetTicketOffersByShowAsync(show1Guid);

            await act.Should().ThrowAsync<KeyNotFoundException>(
                "endpoint should return 404 Not Found for cross-tenant show access");
        }
    }

    [Fact]
    public async Task GetTicketOffers_SortsByCreatedDate()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        
        // Create offers with slight delay to ensure different timestamps
        var offer1Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 200, name: "First Offer");
        await Task.Delay(100); // Small delay to ensure different CreatedAt
        var offer2Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 300, name: "Second Offer");
        await Task.Delay(100);
        var offer3Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 400, name: "Third Offer");

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetTicketOffersByShowAsync(showGuid);

            // Assert - Verify chronological order
            var offersList = result.ToList();
            offersList.Should().HaveCount(3);
            offersList[0].TicketOfferGuid.Should().Be(offer1Guid, "first offer should be first");
            offersList[1].TicketOfferGuid.Should().Be(offer2Guid, "second offer should be second");
            offersList[2].TicketOfferGuid.Should().Be(offer3Guid, "third offer should be third");
        }
    }

    #endregion

    #region Get Show Capacity Tests

    [Fact]
    public async Task GetShowCapacity_WithValidShowGuid_Returns200Ok()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetShowCapacityAsync(showGuid);

            // Assert - Simulates 200 OK response
            result.Should().NotBeNull("endpoint should return 200 OK with capacity info");
        }
    }

    [Fact]
    public async Task GetShowCapacity_WithValidShowGuid_ReturnsCapacityInfo()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 600);
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 200);

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetShowCapacityAsync(showGuid);

            // Assert - Verify capacity calculations
            result.Should().NotBeNull();
            result.ShowGuid.Should().Be(showGuid);
            result.TotalTickets.Should().Be(1000);
            result.AllocatedTickets.Should().Be(800);
            result.AvailableCapacity.Should().Be(200);
        }
    }

    [Fact]
    public async Task GetShowCapacity_WithNoOffers_ReturnsFullCapacity()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetShowCapacityAsync(showGuid);

            // Assert - All capacity should be available
            result.Should().NotBeNull();
            result.TotalTickets.Should().Be(1000);
            result.AllocatedTickets.Should().Be(0);
            result.AvailableCapacity.Should().Be(1000);
        }
    }

    [Fact]
    public async Task GetShowCapacity_WithFullyAllocated_ReturnsZeroAvailable()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 600);
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 400);

        // Act
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetShowCapacityAsync(showGuid);

            // Assert - No capacity should remain
            result.Should().NotBeNull();
            result.TotalTickets.Should().Be(1000);
            result.AllocatedTickets.Should().Be(1000);
            result.AvailableCapacity.Should().Be(0);
        }
    }

    [Fact]
    public async Task GetShowCapacity_WithNonExistentShow_Returns404NotFound()
    {
        // Arrange
        var nonExistentShowGuid = Guid.NewGuid();

        // Act & Assert - Service throws KeyNotFoundException which maps to 404
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, _testTenantSeed);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.GetShowCapacityAsync(nonExistentShowGuid);

            await act.Should().ThrowAsync<KeyNotFoundException>(
                "endpoint should return 404 Not Found for non-existent show");
        }
    }

    #endregion

    #region Multi-Tenancy Tests

    [Fact]
    public async Task GetTicketOffers_ReturnsOnlyCurrentTenantOffers()
    {
        // Arrange - Create shows in two different tenants
        var (show1Guid, tenant1Id) = await CreateTestShowAsync(capacity: 1000);
        var (show2Guid, tenant2Id) = await CreateTestShowAsync(capacity: 1000);
        
        var offer1Guid = await CreateTicketOfferForShowAsync(show1Guid, tenant1Id, ticketCount: 600);
        var offer2Guid = await CreateTicketOfferForShowAsync(show2Guid, tenant2Id, ticketCount: 600);

        // Act - Query from tenant1 context
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenant1Id);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var result = await service.GetTicketOffersByShowAsync(show1Guid);

            // Assert - Should only see tenant1's offers
            var offersList = result.ToList();
            offersList.Should().HaveCount(1);
            offersList[0].TicketOfferGuid.Should().Be(offer1Guid);
        }
    }

    #endregion

    #region Transaction and Edge Case Tests

    [Fact]
    public async Task CreateTicketOffer_WithConcurrentCreation_EnforcesCapacityCorrectly()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        
        // Create first offer using 700 tickets
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 700);

        // Try to create two more offers that would individually fit but together exceed capacity
        var dto1 = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "VIP",
            Price = 150.00m,
            TicketCount = 200
        };

        var dto2 = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "Premium",
            Price = 100.00m,
            TicketCount = 200
        };

        // Act - Create first offer (should succeed)
        var (context1, tenantContext1) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context1)
        {
            var service1 = new TicketOfferService(context1, tenantContext1);
            var result1 = await service1.CreateTicketOfferAsync(showGuid, dto1);
            result1.Should().NotBeNull("first offer should succeed");
        }

        // Act & Assert - Try to create second offer (should fail)
        var (context2, tenantContext2) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context2)
        {
            var service2 = new TicketOfferService(context2, tenantContext2);
            var act = async () => await service2.CreateTicketOfferAsync(showGuid, dto2);

            await act.Should().ThrowAsync<ArgumentException>(
                "second offer should fail due to insufficient capacity");
        }
    }

    [Fact]
    public async Task CreateTicketOffer_WithZeroCapacityRemaining_ReturnsAppropriateError()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);
        
        // Use all capacity
        await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 1000);

        // Try to create another offer
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = Guid.NewGuid(),
            Name = "Late Bird",
            Price = 40.00m,
            TicketCount = 1
        };

        // Act & Assert
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var act = async () => await service.CreateTicketOfferAsync(showGuid, dto);

            var exception = await act.Should().ThrowAsync<ArgumentException>();
            exception.Which.Message.Should().Contain("0", 
                "error message should indicate zero capacity remaining");
        }
    }

    [Fact]
    public async Task CreateTicketOffer_MultipleOffersWithinCapacity_AllSucceed()
    {
        // Arrange
        var (showGuid, tenantId) = await CreateTestShowAsync(capacity: 1000);

        // Act - Create multiple offers within capacity
        var offer1Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 300, name: "Early Bird");
        var offer2Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 400, name: "General Admission");
        var offer3Guid = await CreateTicketOfferForShowAsync(showGuid, tenantId, ticketCount: 200, name: "VIP");

        // Assert - Verify all offers were created
        using (var verifyContext = _fixture.CreateDbContext(_fixture.ConnectionString, tenantId))
        {
            var offers = await verifyContext.TicketOffers
                .Where(o => o.Show.ShowGuid == showGuid)
                .ToListAsync();

            offers.Should().HaveCount(3);
            offers.Sum(o => o.TicketCount).Should().Be(900);
        }

        // Verify capacity
        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            var capacity = await service.GetShowCapacityAsync(showGuid);

            capacity.AllocatedTickets.Should().Be(900);
            capacity.AvailableCapacity.Should().Be(100);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a test show with associated tenant, venue, and act.
    /// Returns the actual database tenant ID (not the test isolation seed).
    /// </summary>
    /// <param name="capacity">The ticket capacity for the show.</param>
    /// <returns>A tuple containing the show GUID and the actual database tenant ID.</returns>
    private async Task<(Guid showGuid, int tenantId)> CreateTestShowAsync(int capacity)
    {
        // Use null context for setup to avoid global query filter issues
        using var setupContext = _fixture.CreateDbContext(_fixture.ConnectionString, null);

        // Create tenant - let EF Core generate the ID
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var tenant = new Tenant
        {
            TenantIdentifier = $"test-tenant-{_testTenantSeed}-{uniqueId}",
            Name = $"Test Tenant {_testTenantSeed} {uniqueId}",
            Slug = $"test-tenant-{_testTenantSeed}-{uniqueId}",
            IsActive = true
        };
        setupContext.Tenants.Add(tenant);
        await setupContext.SaveChangesAsync();

        // Now tenant.Id has been set by EF Core - this is the actual database ID we need to use

        // Create venue with proper tenant reference
        var venueGuid = Guid.NewGuid();
        var venue = new Venue
        {
            Tenant = tenant, // Use navigation property which will set TenantId
            VenueGuid = venueGuid,
            Name = $"Test Venue {tenant.Id}-{uniqueId}",
            Address = "123 Test St",
            SeatingCapacity = capacity,
            Description = "Test venue description"
        };
        setupContext.Venues.Add(venue);
        await setupContext.SaveChangesAsync();

        // Create act with proper tenant reference
        var actGuid = Guid.NewGuid();
        var act = new Act
        {
            Tenant = tenant, // Use navigation property which will set TenantId
            ActGuid = actGuid,
            Name = $"Test Act {tenant.Id}-{uniqueId}"
        };
        setupContext.Acts.Add(act);
        await setupContext.SaveChangesAsync();

        // Create show with proper references
        var showGuid = Guid.NewGuid();
        var show = new Show(act, venue)
        {
            ShowGuid = showGuid,
            TicketCount = capacity,
            StartTime = DateTimeOffset.UtcNow.AddDays(30)
        };
        setupContext.Shows.Add(show);
        await setupContext.SaveChangesAsync();

        // Return both the show GUID and the actual database tenant ID
        return (showGuid, tenant.Id);
    }

    /// <summary>
    /// Creates a ticket offer for the specified show in a specific tenant context.
    /// </summary>
    /// <param name="showGuid">The GUID of the show.</param>
    /// <param name="tenantId">The actual database tenant ID.</param>
    /// <param name="ticketCount">The number of tickets for the offer.</param>
    /// <param name="name">The name of the offer (optional).</param>
    /// <returns>The GUID of the created ticket offer.</returns>
    private async Task<Guid> CreateTicketOfferForShowAsync(Guid showGuid, int tenantId, int ticketCount, string? name = null)
    {
        var offerGuid = Guid.NewGuid();
        var dto = new CreateTicketOfferDto
        {
            TicketOfferGuid = offerGuid,
            Name = name ?? $"Test Offer {Guid.NewGuid().ToString()[..8]}",
            Price = 50.00m,
            TicketCount = ticketCount
        };

        var (context, tenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantId);
        using (context)
        {
            var service = new TicketOfferService(context, tenantContext);
            await service.CreateTicketOfferAsync(showGuid, dto);
        }

        return offerGuid;
    }

    #endregion
}
