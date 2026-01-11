# Table Structures

Define CREATE TABLE syntax with columns, data types, defaults, constraints, and foreign keys.

## Multi-Tenant Entity Pattern
```sql
CREATE TABLE Venues (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    VenueGuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(2) NOT NULL DEFAULT 'US',
    Capacity INT NOT NULL CHECK (Capacity > 0 AND Capacity <= 100000),
    VenueTypeId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Venues_TenantId FOREIGN KEY (TenantId) REFERENCES Tenants(Id),
    CONSTRAINT FK_Venues_VenueTypeId FOREIGN KEY (VenueTypeId) REFERENCES VenueTypes(Id),
    CONSTRAINT UQ_Venues_TenantGuid UNIQUE (TenantId, VenueGuid),
    CONSTRAINT UQ_Venues_TenantName UNIQUE (TenantId, Name)
);
```

## Child Entity Pattern
```sql
CREATE TABLE Shows (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    ShowGuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    VenueId INT NOT NULL,
    ActId INT NOT NULL,
    CreatedByUserId INT NOT NULL,
    EventDate DATETIME2 NOT NULL,
    TicketPrice DECIMAL(10,2) NOT NULL CHECK (TicketPrice >= 0),
    AvailableTickets INT NOT NULL CHECK (AvailableTickets >= 0),
    IsPublished BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_Shows_TenantId FOREIGN KEY (TenantId) REFERENCES Tenants(Id),
    CONSTRAINT FK_Shows_VenueId FOREIGN KEY (VenueId) REFERENCES Venues(Id),
    CONSTRAINT FK_Shows_ActId FOREIGN KEY (ActId) REFERENCES Acts(Id),
    CONSTRAINT FK_Shows_CreatedByUserId FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
    CONSTRAINT UQ_Shows_TenantGuid UNIQUE (TenantId, ShowGuid),
    CONSTRAINT CHK_Shows_EventDateFuture CHECK (EventDate > GETUTCDATE())
);
```

Include TenantId, integer PK, GUID alternate key, timestamps, and business checks.
