# URL Structure and Naming

## Resource-Based URLs
**Use plural nouns for resources and avoid actions in URLs.**

```csharp
// ✅ Good - Resource-based URLs
GET    /api/venues          // Get all venues
GET    /api/venues/{id}     // Get specific venue
POST   /api/venues          // Create new venue
PUT    /api/venues/{id}     // Update entire venue
PATCH  /api/venues/{id}     // Partial update
DELETE /api/venues/{id}     // Delete venue

// ✅ Good - Related resources
GET    /api/venues/{id}/acts        // Get acts for venue
POST   /api/venues/{id}/acts        // Create act for venue
GET    /api/venues/{id}/acts/{actId} // Get specific act

// ❌ Bad - Action-based URLs
GET    /api/getVenues
POST   /api/createVenue
GET    /api/venue/{id}/getActs
POST   /api/venue/{id}/createAct
```

## Multi-Word Resource Naming
**Use kebab-case for multi-word resource names.**

```csharp
// ✅ Good - Kebab-case for readability
GET    /api/ticket-types
GET    /api/customer-accounts
GET    /api/event-categories

// ✅ Good - Nested multi-word resources
GET    /api/ticket-types/{id}/price-tiers
POST   /api/customer-accounts/{id}/payment-methods

// ❌ Bad - CamelCase (harder to read in URLs)
GET    /api/ticketTypes
GET    /api/customerAccounts
```

## Filtering and Querying
**Use query parameters for filtering, sorting, and pagination.**

```csharp
// ✅ Good - Filtering
GET /api/venues?tenantId={tenantId}
GET /api/venues?isActive=true
GET /api/venues?city=New%20York

// ✅ Good - Multiple filters
GET /api/venues?isActive=true&city=New%20York&minCapacity=1000

// ✅ Good - Sorting
GET /api/venues?sortBy=name&sortOrder=asc
GET /api/venues?sortBy=createdAt&sortOrder=desc

// ✅ Good - Pagination
GET /api/venues?page=1&pageSize=20
GET /api/venues?offset=0&limit=20  // Alternative pagination style

// ✅ Good - Search
GET /api/venues?search=madison
GET /api/venues?q=madison%20square   // Alternative search parameter
```

