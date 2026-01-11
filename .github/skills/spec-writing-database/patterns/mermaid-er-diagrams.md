# Mermaid ER Diagrams

Use Mermaid syntax to visualize entities, relationships, and attributes.

## Basic Entity Relationships
```mermaid
erDiagram
    Tenant ||--o{ Venue : has
    Tenant ||--o{ Act : has
    Venue ||--o{ Show : hosts
    Act ||--o{ Show : performs
    Show ||--o{ TicketOffer : contains

    Tenant {
        int Id PK
        uuid TenantGuid UK "Unique identifier for API"
        string Name "Organization name"
        string Domain "Subdomain for tenant"
        bool IsActive "Tenant status"
        datetime CreatedAt "Tenant creation date"
        datetime UpdatedAt "Last modification date"
    }

    Venue {
        int Id PK
        int TenantId FK "Multi-tenant isolation"
        uuid VenueGuid UK "External API identifier"
        string Name "Venue display name"
        string Address "Full venue address"
        string City "Venue city"
        string State "Venue state/province"
        string PostalCode "Postal/ZIP code"
        string Country "Country code"
        int Capacity "Maximum venue capacity"
        int VenueTypeId FK "Reference to VenueTypes"
        bool IsActive "Venue operational status"
        datetime CreatedAt "Record creation timestamp"
        datetime UpdatedAt "Last update timestamp"
    }
```

## Complex Multi-Tenant Relationships
```mermaid
erDiagram
    Tenant ||--o{ User : contains
    Tenant ||--o{ Venue : owns
    Tenant ||--o{ Act : manages
    Tenant ||--o{ Show : schedules
    
    User ||--o{ Show : creates
    User ||--o{ TicketOffer : manages
    
    Venue ||--o{ Show : hosts
    Act ||--o{ Show : performs
    Show ||--o{ TicketOffer : contains
    
    VenueType ||--o{ Venue : categorizes
    TicketType ||--o{ TicketOffer : defines

    Show {
        int Id PK
        int TenantId FK "Tenant isolation"
        uuid ShowGuid UK "External identifier"
        int VenueId FK "Hosting venue"
        int ActId FK "Performing act"
        int CreatedByUserId FK "Show creator"
        datetime EventDate "Show date and time"
        decimal TicketPrice "Base ticket price"
        int AvailableTickets "Remaining capacity"
        bool IsPublished "Visibility status"
        datetime CreatedAt "Creation timestamp"
        datetime UpdatedAt "Modification timestamp"
    }
```

Include field types (PK, FK, UK), data types, and descriptions for clarity.
