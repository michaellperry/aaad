# Source Entity Analysis - EFCore6BP-Globoticket

## Overview

This document provides a comprehensive analysis of the source entities from the EFCore6BP-Globoticket project for recreation in the multi-tenant target project.

## Entity Structure Analysis

### 1. Venue Entity

**File Location**: `GloboTicket.Domain/Entities/Venue.cs`

**Properties**:
- `VenueId` (int, Primary Key, Identity)
- `VenueGuid` (Guid, Alternate Key/Unique)
- `Name` (string, max 100 chars)
- `Address` (string?, max 300 chars)
- `Location` (Point?, NetTopologySuite.Geometries)
- `SeatingCapacity` (int)
- `Description` (string)

**Key Characteristics**:
- Uses NetTopologySuite for geospatial data (geography type)
- Supports geographic search with distance calculations
- Includes seating capacity for ticket sales tracking
- Has a GUID for public API identification

### 2. Act Entity

**File Location**: `GloboTicket.Domain/Entities/Act.cs`

**Properties**:
- `ActId` (int, Primary Key, Identity)
- `ActGuid` (Guid, Alternate Key/Unique)
- `Name` (string, max 100 chars)

**Key Characteristics**:
- Simple entity representing performers/acts
- GUID for public API identification
- Minimal configuration, just name validation

### 3. Show Entity

**File Location**: `GloboTicket.Domain/Entities/Show.cs`

**Properties**:
- `ShowId` (int, Primary Key, Identity)
- `ShowGuid` (Guid, Alternate Key/Unique)
- `Venue` (Navigation Property)
- `Act` (Navigation Property)
- `Date` (DateTimeOffset)
- `TicketSales` (ICollection<TicketSale>)

**Key Characteristics**:
- Uses constructor injection for required navigation properties
- Supports many-to-many relationship through TicketSale
- Includes event scheduling information
- Complex object with multiple dependencies

**Constructor Pattern**:
```csharp
public Show(Venue venue, Act act)
{
    Venue = venue;
    Act = act;
}

private Show() : this(null!, null!) { }
```

### 4. TicketSale Entity

**File Location**: `GloboTicket.Domain/Entities/TicketSale.cs`

**Properties**:
- `TicketSaleId` (int, Primary Key, Identity)
- `TicketSaleGuid` (Guid, Unique)
- `Show` (Navigation Property)
- `ShowId` (int, Foreign Key)
- `Quantity` (int)

**Key Characteristics**:
- Simple transaction entity
- Tracks quantity of tickets sold per show
- Supports revenue reporting and capacity tracking

## EF Core Configuration Analysis

### Configuration Patterns

**VenueConfiguration**:
- Sets VenueGuid as alternate key (unique constraint)
- Validates Name max length (100)
- Validates Address max length (300)
- No explicit relationship configuration (using conventions)

**ActConfiguration**:
- Sets ActGuid as alternate key
- Validates Name max length (100)

**ShowConfiguration**:
- Sets ShowGuid as alternate key
- No explicit relationship configuration visible

### Database Schema (from Migrations)

**Table: Venue**
- VenueId (PK, Identity)
- VenueGuid (Unique)
- Name (nvarchar(100), Required)
- Address (nvarchar(300), Originally required, later made optional)
- Location (geography, Nullable)
- Description (nvarchar(max), Required)
- SeatingCapacity (int, Required)

**Table: Act**
- ActId (PK, Identity)
- ActGuid (Unique)
- Name (nvarchar(100), Required)

**Table: Show**
- ShowId (PK, Identity)
- ShowGuid (Unique)
- VenueId (FK to Venue, Cascade delete)
- ActId (FK to Act, Cascade delete)
- Date (datetimeoffset, Required)

**Table: TicketSale**
- TicketSaleId (PK, Identity)
- TicketSaleGuid (Unique)
- ShowId (FK to Show, Cascade delete)
- Quantity (int, Required)

## API Controller Analysis

### ShowsController (Most Complete Implementation)

**Endpoints**:
- `GET /api/shows` - Search shows by location and date range
- `GET /api/shows/{showGuid}` - Get single show (not implemented)
- `PATCH /api/shows/{showGuid}` - Reschedule show

**Business Logic Patterns**:
- Geographic search using Haversine formula via NetTopologySuite
- Complex query with distance calculations and date filtering
- Query validation with multiple parameters
- Service-based architecture using PromotionService

### VenuesController & ActsController
- Basic CRUD endpoints (not implemented)
- Use GUID-based routing
- Simple model patterns

## Domain Services Analysis

### 1. PromotionService (Primary Service)

**Responsibilities**:
- Booking shows (creating Show entities)
- Creating venues and acts
- Geographic search and filtering
- Show rescheduling

**Key Methods**:
- `BookShow()` - Creates shows with validation
- `CreateVenue()` - Creates venues with geospatial data
- `CreateAct()` - Creates acts
- `FindShowsByDistanceAndDateRange()` - Complex geospatial query
- `RescheduleShow()` - Updates show dates

### 2. SalesService

**Responsibilities**:
- Processing ticket sales
- Creating TicketSale entities
- Transaction management

**Key Methods**:
- `SellTickets()` - Creates ticket sales with validation

### 3. FeedService

**Responsibilities**:
- Providing show listings
- Data transformation for feeds
- Async streaming support

**Key Methods**:
- `ListShows()` - Async enumerable pattern for streaming

### 4. GeographyService

**Responsibilities**:
- Geospatial utility functions
- Point creation with SRID 4326 (WGS84)

**Key Methods**:
- `GeographicLocation()` - Creates Point from lat/lon

## Key Architectural Patterns

### 1. GUID-Based Public APIs
- All entities have GUID properties for public API exposure
- Internal integer IDs for database operations
- Clear separation of concerns

### 2. Constructor Injection Pattern
- Required navigation properties set via constructor
- Parameterless constructor for EF Core
- Ensures referential integrity

### 3. Geospatial Integration
- NetTopologySuite integration for geographic operations
- SQL Server geography type support
- Distance-based search capabilities

### 4. Service Layer Pattern
- Business logic isolated in domain services
- DbContext injection for data access
- Clear separation from controllers

### 5. Async/Await Patterns
- Async methods throughout
- Async enumerable for streaming data
- Cancellation token support

## Multi-Tenancy Considerations

### Current Implementation
- No existing multi-tenant infrastructure in source
- Single-tenant data model
- No tenant isolation mechanisms

### Target Project Requirements
- ITenantEntity interface for isolation
- Global query filters in DbContext
- TenantId automatic assignment on save
- Row-level security within environment database

### Recommendations for Migration
1. **Add TenantId to all entities** (except Tenant itself)
2. **Implement ITenantEntity interface** on Venue, Act, Show, TicketSale
3. **Update constructors** to require tenant context
4. **Modify relationships** to respect tenant boundaries
5. **Update services** to work with tenant context
6. **Preserve GUID patterns** for public APIs
7. **Consider tenant-aware query filters** for performance