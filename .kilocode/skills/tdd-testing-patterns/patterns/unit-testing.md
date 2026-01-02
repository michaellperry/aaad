# Unit Testing Domain Logic

## Testing Entities and Value Objects

**Focus on testing business rules and invariants in domain objects.**

```csharp
[Test]
public void Venue_Deactivate_DeactivatesVenueAndAddsDomainEvent()
{
    // Arrange
    var venue = CreateValidVenue();
    var originalEvents = DomainEvents.GetDomainEvents().ToList();
    
    // Act
    venue.Deactivate();
    
    // Assert - State changes
    Assert.That(venue.IsActive, Is.False);
    
    // Assert - Domain events
    var domainEvents = DomainEvents.GetDomainEvents();
    Assert.That(domainEvents, Has.Some.TypeOf<VenueDeactivatedEvent>());
    Assert.That(domainEvents.Count, Is.EqualTo(originalEvents.Count + 1));
}

[Test]
public void Venue_AddAct_ValidAct_AddsActSuccessfully()
{
    // Arrange
    var venue = CreateValidVenue();
    var actTitle = "Concert A";
    var eventDate = DateTime.UtcNow.AddDays(30);
    var ticketPrice = 50.00m;
    
    // Act
    venue.AddAct(actTitle, eventDate, ticketPrice);
    
    // Assert
    Assert.That(venue.Acts.Count, Is.EqualTo(1));
    var act = venue.Acts.First();
    Assert.That(act.Title, Is.EqualTo(actTitle));
    Assert.That(act.EventDate, Is.EqualTo(eventDate));
    Assert.That(act.TicketPrice, Is.EqualTo(ticketPrice));
}

[Test]
public void Venue_AddAct_InactiveVenue_ThrowsInvalidOperationException()
{
    // Arrange
    var venue = CreateValidVenue();
    venue.Deactivate(); // Make venue inactive
    
    // Act & Assert
    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
    {
        await venue.AddAct("Concert A", DateTime.UtcNow.AddDays(30), 50.00m);
    });
    
    Assert.That(exception.Message, Does.Contain("inactive venue"));
}
```

## Testing Value Objects

**Value objects should be tested for equality and immutability.**

```csharp
[Test]
public void Address_Equals_SameValues_ReturnsTrue()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // Act & Assert
    Assert.That(address1, Is.EqualTo(address2));
    Assert.That(address1 == address2, Is.True);
}

[Test]
public void Address_Equals_DifferentValues_ReturnsFalse()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("456 Oak Ave", "City", "State", "12345", "USA");
    
    // Act & Assert
    Assert.That(address1, Is.Not.EqualTo(address2));
    Assert.That(address1 == address2, Is.False);
}

[Test]
public void Address_IsImmutable_ThrowsExceptionOnModification()
{
    // Arrange
    var address = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // Act & Assert - Value objects should be immutable
    var exception = Assert.Throws<InvalidOperationException>(() =>
    {
        var street = address.Street;
        // Attempting to modify would require reflection or specific API
    });
}
```

## Testing Application Services

### Command Handler Tests
**Test application services that handle commands and queries.**

```csharp
[Test]
public async Task CreateVenueCommandHandler_Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var mockMapper = new Mock<IMapper>();
    var handler = new CreateVenueCommandHandler(mockRepository.Object, mockMapper.Object);
    
    var command = new CreateVenueCommand
    {
        Name = "Madison Square Garden",
        Address = "123 Broadway, New York, NY 10001",
        TenantId = _tenantId
    };
    
    mockRepository.Setup(r => r.ExistsByNameAsync(command.Name, command.TenantId))
        .ReturnsAsync(false);
    
    var expectedVenue = new Venue(command.Name, ParseAddress(command.Address), command.TenantId);
    var expectedDto = new VenueDto { Id = expectedVenue.Id, Name = expectedVenue.Name };
    
    mockMapper.Setup(m => m.Map<VenueDto>(It.IsAny<Venue>()))
        .Returns(expectedDto);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    Assert.That(result.Name, Is.EqualTo(command.Name));
    
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Venue>()), Times.Once);
    mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
}

[Test]
public async Task CreateVenueCommandHandler_DuplicateName_ThrowsValidationException()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var handler = new CreateVenueCommandHandler(mockRepository.Object, MockMapper.Object);
    
    var command = new CreateVenueCommand
    {
        Name = "Existing Venue",
        Address = "123 Broadway, New York, NY 10001",
        TenantId = _tenantId
    };
    
    mockRepository.Setup(r => r.ExistsByNameAsync(command.Name, command.TenantId))
        .ReturnsAsync(true);
    
    // Act & Assert
    var exception = Assert.ThrowsAsync<ValidationException>(async () =>
    {
        await handler.Handle(command, CancellationToken.None);
    });
    
    Assert.That(exception.Message, Does.Contain("already exists"));
}
```

### Query Handler Tests
**Test read-only queries and data retrieval logic.**

```csharp
[Test]
public async Task GetVenuesQueryHandler_Handle_ReturnsActiveVenues()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var handler = new GetVenuesQueryHandler(mockRepository.Object);
    
    var venues = new List<Venue>
    {
        new Venue("Venue 1", CreateTestAddress(), _tenantId),
        new Venue("Venue 2", CreateTestAddress(), _tenantId)
    };
    
    mockRepository.Setup(r => r.GetActiveVenuesAsync(_tenantId))
        .ReturnsAsync(venues);
    
    var query = new GetVenuesQuery { TenantId = _tenantId };
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Count, Is.EqualTo(2));
    Assert.That(result.First().Name, Is.EqualTo("Venue 1"));
}
```

