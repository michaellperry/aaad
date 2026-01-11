# Constraints

Specify CHECK, UNIQUE, and FOREIGN KEY constraints for data integrity and business rules.

## Data Validation Constraints
```sql
-- Business rules
CONSTRAINT CHK_Venue_Capacity CHECK (Capacity BETWEEN 1 AND 100000)
CONSTRAINT CHK_Venue_Name CHECK (LEN(RTRIM(Name)) >= 3)
CONSTRAINT CHK_Venue_PostalCode CHECK (PostalCode NOT LIKE '%[^0-9A-Z -]%')

-- Date validation
CONSTRAINT CHK_Show_EventDate CHECK (EventDate > GETUTCDATE())
CONSTRAINT CHK_TicketOffer_SaleDates CHECK (SaleEndDate > SaleStartDate)
CONSTRAINT CHK_TicketOffer_SaleStartFuture CHECK (SaleStartDate >= CAST(GETUTCDATE() AS DATE))

-- Price and quantity validation
CONSTRAINT CHK_TicketOffer_Price CHECK (Price >= 0 AND Price <= 9999.99)
CONSTRAINT CHK_TicketOffer_Quantity CHECK (Quantity >= 0)
```

## Multi-Tenant Uniqueness
```sql
-- Composite unique constraints for multi-tenancy
CONSTRAINT UQ_Venues_TenantGuid UNIQUE (TenantId, VenueGuid)
CONSTRAINT UQ_Venues_TenantName UNIQUE (TenantId, Name)
CONSTRAINT UQ_Acts_TenantGuid UNIQUE (TenantId, ActGuid)
CONSTRAINT UQ_Acts_TenantName UNIQUE (TenantId, Name)

-- Global uniqueness where required
CONSTRAINT UQ_Tenants_Domain UNIQUE (Domain)
CONSTRAINT UQ_Users_Email UNIQUE (Email)
```

## Referential Integrity
```sql
-- ON DELETE RESTRICT prevents parent deletion
CONSTRAINT FK_Shows_VenueId 
    FOREIGN KEY (VenueId) REFERENCES Venues(Id)
    ON DELETE RESTRICT

-- ON DELETE CASCADE removes children with parent
CONSTRAINT FK_TicketOffers_ShowId 
    FOREIGN KEY (ShowId) REFERENCES Shows(Id)
    ON DELETE CASCADE
```
