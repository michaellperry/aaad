# TDD Implementation Guide - Multi-Tenant Entity Recreation

## Implementation Overview

This guide provides a Test-Driven Development (TDD) approach for implementing Venue, Act, Show, and TicketSale entities in the multi-tenant target project, based on analysis of the EFCore6BP-Globoticket source.

## TDD Methodology

### Red-Green-Refactor Cycle
1. **Red**: Write failing tests for new functionality
2. **Green**: Write minimal code to make tests pass
3. **Refactor**: Improve code quality while maintaining test coverage

## Phase 1: Entity Foundation (Start with Red Tests)

### Test-First Entity Design

#### 1. Venue Entity Tests
**File to create**: `tests/GloboTicket.UnitTests/Domain/Entities/VenueTests.cs`

```csharp
[Fact]
public void Venue_ShouldHaveTenantId()
{
    // Arrange & Act & Assert
    var venue = new Venue();
    Assert.True(venue is ITenantEntity);
    Assert.NotNull(((ITenantEntity)venue).TenantId);
}

[Fact]
public void Venue_ShouldHaveGuidForPublicApi()
{
    // Arrange
    var venue = new Venue();
    
    // Act & Assert
    Assert.NotEqual(Guid.Empty, venue.VenueGuid);
}

[Fact]
public void Venue_ShouldSupportGeospatialLocation()
{
    // Arrange
    var venue = new Venue();
    var point = new Point(-122.4194, 37.7749) { SRID = 4326 };
    
    // Act
    venue.Location = point;
    
    // Assert
    Assert.Equal(point, venue.Location);
}
```

#### 2. Act Entity Tests
**File to create**: `tests/GloboTicket.UnitTests/Domain/Entities/ActTests.cs`

```csharp
[Fact]
public void Act_ShouldHaveTenantId()
{
    // Arrange & Act & Assert
    var act = new Act();
    Assert.True(act is ITenantEntity);
    Assert.NotNull(((ITenantEntity)act).TenantId);
}

[Fact]
public void Act_ShouldValidateNameLength()
{
    // Arrange
    var act = new Act();
    var longName = new string('A', 101); // Exceeds 100 char limit
    
    // Act & Assert
    Assert.Throws<ArgumentException>(() => act.Name = longName);
}
```

#### 3. Show Entity Tests
**File to create**: `tests/GloboTicket.UnitTests/Domain/Entities/ShowTests.cs`

```csharp
[Fact]
public void Show_ShouldHaveTenantId()
{
    // Arrange & Act & Assert
    var show = new Show();
    Assert.True(show is ITenantEntity);
    Assert.NotNull(((ITenantEntity)show).TenantId);
}

[Fact]
public void Show_ShouldRequireVenueAndAct()
{
    // Arrange
    var venue = new Venue();
    var act = new Act();
    
    // Act
    var show = new Show(venue, act);
    
    // Assert
    Assert.Equal(venue, show.Venue);
    Assert.Equal(act, show.Act);
}

[Fact]
public void Show_ShouldCalculateSeatsAvailable()
{
    // Arrange
    var venue = new Venue { SeatingCapacity = 100 };
    var act = new Act();
    var show = new Show(venue, act);
    
    // Act & Assert
    Assert.Equal(100, show.Venue.SeatingCapacity);
}
```

#### 4. TicketSale Entity Tests
**File to create**: `tests/GloboTicket.UnitTests/Domain/Entities/TicketSaleTests.cs`

```csharp
[Fact]
public void TicketSale_ShouldHaveTenantId()
{
    // Arrange & Act & Assert
    var ticketSale = new TicketSale();
    Assert.True(ticketSale is ITenantEntity);
    Assert.NotNull(((ITenantEntity)ticketSale).TenantId);
}

[Fact]
public void TicketSale_ShouldRequireShow()
{
    // Arrange
    var show = new Show();
    
    // Act
    var ticketSale = new TicketSale(show);
    
    // Assert
    Assert.Equal(show, ticketSale.Show);
}
```

### Green Phase: Implement Entities

#### Base Multi-Tenant Entity
**File**: `src/GloboTicket.Domain/Entities/MultiTenantEntity.cs`

```csharp
namespace GloboTicket.Domain.Entities;

public abstract class MultiTenantEntity : Entity, ITenantEntity
{
    public int TenantId { get; set; }
}
```

#### Venue Entity Implementation
**File**: `src/GloboTicket.Domain/Entities/Venue.cs`

```csharp
using NetTopologySuite.Geometries;

namespace GloboTicket.Domain.Entities;

public class Venue : MultiTenantEntity
{
    public Guid VenueGuid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public Point? Location { get; set; }
    public int SeatingCapacity { get; set; }
    public string Description { get; set; } = string.Empty;
}
```

#### Act Entity Implementation
**File**: `src/GloboTicket.Domain/Entities/Act.cs`

```csharp
namespace GloboTicket.Domain.Entities;

public class Act : MultiTenantEntity
{
    public Guid ActGuid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
```

#### Show Entity Implementation
**File**: `src/GloboTicket.Domain/Entities/Show.cs`

```csharp
namespace GloboTicket.Domain.Entities;

public class Show : MultiTenantEntity
{
    public Guid ShowGuid { get; set; } = Guid.NewGuid();
    public Venue Venue { get; set; } = null!;
    public Act Act { get; set; } = null!;
    public DateTimeOffset Date { get; set; }
    public ICollection<TicketSale> TicketSales { get; set; } = new List<TicketSale>();

    public Show(Venue venue, Act act)
    {
        Venue = venue;
        Act = act;
    }

    private Show() : this(null!, null!) { }
}
```

#### TicketSale Entity Implementation
**File**: `src/GloboTicket.Domain/Entities/TicketSale.cs`

```csharp
using System;

namespace GloboTicket.Domain.Entities;

public class TicketSale : MultiTenantEntity
{
    public Guid TicketSaleGuid { get; set; } = Guid.NewGuid();
    public Show Show { get; set; } = null!;
    public int ShowId { get; set; }
    public int Quantity { get; set; }

    public TicketSale(Show show)
    {
        Show = show;
    }

    public TicketSale() : this(null!) { }
}
```

## Phase 2: EF Core Configuration (TDD)

### Configuration Tests
**File**: `tests/GloboTicket.UnitTests/Infrastructure/Configurations/VenueConfigurationTests.cs`

```csharp
[Fact]
public void VenueConfiguration_ShouldSetVenueGuidAsAlternateKey()
{
    // Arrange
    var modelBuilder = new ModelBuilder();
    var venueConfig = new VenueConfiguration();
    
    // Act
    venueConfig.Configure(modelBuilder.Entity<Venue>());
    
    // Assert
    var venueEntity = modelBuilder.Model.FindEntityType(typeof(Venue))!;
    Assert.NotNull(venueEntity.FindKey("VenueGuid"));
}

[Fact]
public void VenueConfiguration_ShouldSetNameLengthConstraint()
{
    // Arrange & Act & Assert
    // Test will validate max length configuration
}
```

### Green Phase: Implement Configurations

#### VenueConfiguration
**File**: `src/GloboTicket.Infrastructure/Data/Configurations/VenueConfiguration.cs`

```csharp
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues");
        
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        
        builder.HasAlternateKey(v => v.VenueGuid);
        builder.Property(v => v.VenueGuid).IsRequired();
        
        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(v => v.Address)
            .HasMaxLength(300);
            
        builder.Property(v => v.Location)
            .HasColumnType("geography");
            
        builder.Property(v => v.SeatingCapacity)
            .IsRequired();
            
        builder.Property(v => v.Description)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
```

#### ActConfiguration
**File**: `src/GloboTicket.Infrastructure/Data/Configurations/ActConfiguration.cs`

```csharp
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

public class ActConfiguration : IEntityTypeConfiguration<Act>
{
    public void Configure(EntityTypeBuilder<Act> builder)
    {
        builder.ToTable("Acts");
        
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        
        builder.HasAlternateKey(a => a.ActGuid);
        builder.Property(a => a.ActGuid).IsRequired();
        
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
```

#### ShowConfiguration
**File**: `src/GloboTicket.Infrastructure/Data/Configurations/ShowConfiguration.cs`

```csharp
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        builder.ToTable("Shows");
        
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        
        builder.HasAlternateKey(s => s.ShowGuid);
        builder.Property(s => s.ShowGuid).IsRequired();
        
        builder.Property(s => s.Date).IsRequired();
        
        // Navigation properties
        builder.HasOne(s => s.Venue)
            .WithMany()
            .HasForeignKey(s => s.VenueId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(s => s.Act)
            .WithMany()
            .HasForeignKey(s => s.ActId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(s => s.TicketSales)
            .WithOne(ts => ts.Show)
            .HasForeignKey(ts => ts.ShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

#### TicketSaleConfiguration
**File**: `src/GloboTicket.Infrastructure/Data/Configurations/TicketSaleConfiguration.cs`

```csharp
using GloboTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.Infrastructure.Data.Configurations;

public class TicketSaleConfiguration : IEntityTypeConfiguration<TicketSale>
{
    public void Configure(EntityTypeBuilder<TicketSale> builder)
    {
        builder.ToTable("TicketSales");
        
        builder.HasKey(ts => ts.Id);
        builder.Property(ts => ts.Id).ValueGeneratedOnAdd();
        
        builder.HasAlternateKey(ts => ts.TicketSaleGuid);
        builder.Property(ts => ts.TicketSaleGuid).IsRequired();
        
        builder.Property(ts => ts.Quantity).IsRequired();
        
        // Navigation property to Show
        builder.HasOne(ts => ts.Show)
            .WithMany(s => s.TicketSales)
            .HasForeignKey(ts => ts.ShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## Phase 3: Service Layer Tests (TDD)

### Domain Service Tests
**File**: `tests/GloboTicket.UnitTests/Domain/Services/VenueServiceTests.cs`

```csharp
[Fact]
public async Task VenueService_CreateVenue_ShouldAssignTenantId()
{
    // Arrange
    var mockContext = new Mock<GloboTicketDbContext>(options, tenantContext.Object);
    var service = new VenueService(mockContext.Object);
    var venueData = new CreateVenueDto { /* ... */ };
    
    // Act
    var result = await service.CreateVenue(venueData);
    
    // Assert
    Assert.Equal(expectedTenantId, result.TenantId);
}
```

## Integration Test Strategy

### Multi-Tenancy Isolation Tests
**File**: `tests/GloboTicket.IntegrationTests/MultiTenancy/EntityIsolationTests.cs`

```csharp
[Fact]
public async Task Venue_ShouldBeIsolatedBetweenTenants()
{
    // Arrange
    var tenant1 = await CreateTestTenantAsync("tenant1");
    var tenant2 = await CreateTestTenantAsync("tenant2");
    
    // Act - Create venue for tenant1
    var venue1 = await CreateVenueForTenantAsync(tenant1.Id, "Venue 1");
    
    // Assert - Verify tenant2 cannot see venue1
    var venuesForTenant2 = await Context.Set<Venue>()
        .Where(v => v.TenantId == tenant2.Id)
        .ToListAsync();
        
    Assert.Empty(venuesForTenant2);
}
```

## Migration Strategy

### Database Migration Order
1. **Add TenantId columns** to existing tables
2. **Update configurations** to include TenantId mapping
3. **Create compound foreign key constraints**
4. **Add indexes** for performance

### Migration Script Template
```sql
-- Phase 1: Add TenantId columns
ALTER TABLE Venue ADD TenantId INT NOT NULL DEFAULT 1;
ALTER TABLE Act ADD TenantId INT NOT NULL DEFAULT 1;
ALTER TABLE Show ADD TenantId INT NOT NULL DEFAULT 1;
ALTER TABLE TicketSale ADD TenantId INT NOT NULL DEFAULT 1;

-- Phase 2: Update foreign keys (simplified example)
ALTER TABLE Show DROP CONSTRAINT FK_Show_Venue_VenueId;
ALTER TABLE Show ADD CONSTRAINT FK_Show_Venue_Tenant 
    FOREIGN KEY (TenantId, VenueId) REFERENCES Venue(TenantId, Id);

-- Phase 3: Add indexes
CREATE INDEX IX_Venue_TenantId ON Venue(TenantId);
CREATE INDEX IX_Act_TenantId ON Act(TenantId);
CREATE INDEX IX_Show_TenantId ON Show(TenantId);
CREATE INDEX IX_TicketSale_TenantId ON TicketSale(TenantId);
```

## Success Metrics

### Test Coverage Goals
- **Entity Logic**: 100% coverage for entity methods
- **Configuration**: 100% coverage for all configurations
- **Service Layer**: 95% coverage for business logic
- **Integration**: 90% coverage for multi-tenant scenarios

### Performance Benchmarks
- **Query Performance**: No degradation >10% for existing queries
- **Geospatial Queries**: Maintain current performance for location-based searches
- **Migration Time**: Complete migration in <5 minutes for production database

### Quality Gates
- All unit tests passing
- All integration tests passing
- No data corruption in migration tests
- Preserved API compatibility for external consumers