# URL Naming Conventions

Detailed URL structure and naming patterns for RESTful APIs.

## Multi-Word Resource Naming

### Kebab-case Convention
Use kebab-case for multi-word resource names for better readability.

```csharp
// ✅ Good - Kebab-case for readability
GET    /api/ticket-types
GET    /api/customer-accounts
GET    /api/event-categories
GET    /api/venue-configurations

// ✅ Good - Nested multi-word resources
GET    /api/ticket-types/{id}/price-tiers
POST   /api/customer-accounts/{id}/payment-methods
GET    /api/venue-configurations/{id}/seating-charts

// ❌ Bad - CamelCase (harder to read in URLs)
GET    /api/ticketTypes
GET    /api/customerAccounts

// ❌ Bad - snake_case (less conventional for URLs)
GET    /api/ticket_types
GET    /api/customer_accounts
```

## API Versioning in URLs

### URL Versioning (Recommended)
Version your API to maintain backward compatibility.

```csharp
// ✅ Good - Version in URL path (most common)
GET /api/v1/venues/{id}
GET /api/v2/venues/{id}
POST /api/v1/venues
PUT /api/v2/venues/{id}

// ✅ Good - Separate endpoints for different versions
GET /api/v1/venues  // Returns basic venue info
GET /api/v2/venues  // Returns enhanced venue info with additional fields

// ❌ Bad - Query parameters for versions
GET /api/venues/{id}?version=1
GET /api/venues/{id}?v=2
```

### Header Versioning (Alternative)
More RESTful approach using content negotiation.

```csharp
// ✅ Good - Version in Accept headers
GET /api/venues/{id}
Accept: application/vnd.globoticket.v1+json

GET /api/venues/{id}
Accept: application/vnd.globoticket.v2+json

// ✅ Good - Custom versioning headers
GET /api/venues/{id}
API-Version: 1.0

GET /api/venues/{id}
API-Version: 2.0
```

## Query Parameters

### Standard Filtering Patterns
Consistent patterns for filtering, sorting, and pagination.

```csharp
// ✅ Good - Standard query patterns
GET /api/venues?page=2&limit=10              // Pagination
GET /api/venues?sort=name&order=asc          // Sorting
GET /api/venues?category=music&active=true   // Filtering
GET /api/venues?search=broadway              // Text search
GET /api/venues?tenantId={tenantId}          // Tenant filtering

// ✅ Good - Multiple filters
GET /api/venues?active=true&city=New%20York&minCapacity=1000

// ✅ Good - Date range filtering
GET /api/events?startDate=2024-01-01&endDate=2024-12-31
GET /api/venues?createdAfter=2024-01-01T00:00:00Z
```

### Advanced Filtering
Complex filtering for power users and advanced scenarios.

```csharp
// ✅ Good - Range filtering with operators
GET /api/venues?capacity[gte]=100&capacity[lte]=1000
GET /api/events?price[min]=50&price[max]=200
GET /api/venues?created[after]=2024-01-01&created[before]=2024-12-31

// ✅ Good - Array and list filtering
GET /api/venues?category[in]=music,theater,comedy
GET /api/venues?tags[contains]=handicap-accessible
GET /api/events?venueIds[]=123&venueIds[]=456&venueIds[]=789

// ✅ Good - Nested property filtering
GET /api/venues?address.city=New%20York
GET /api/venues?address.state=NY&address.country=US

// ✅ Good - Advanced text search
GET /api/venues?search=broadway&searchFields=name,description,address
GET /api/venues?q=manhattan%20theater&fuzzy=true
```

### Sorting Patterns
Consistent sorting parameter conventions.

```csharp
// ✅ Good - Standard sorting
GET /api/venues?sort=name&order=asc
GET /api/venues?sortBy=createdAt&sortOrder=desc

// ✅ Good - Multiple sort criteria
GET /api/venues?sort=category,name&order=asc,desc
GET /api/venues?sort[]=category:asc&sort[]=name:desc

// ✅ Good - Nested property sorting
GET /api/venues?sort=address.city&order=asc
GET /api/events?sort=venue.name,startDate&order=asc,desc
```

### Pagination Patterns
Standard pagination approaches for different use cases.

```csharp
// ✅ Good - Offset-based pagination (simple)
GET /api/venues?page=1&pageSize=20
GET /api/venues?offset=0&limit=20
GET /api/venues?skip=40&take=20

// ✅ Good - Cursor-based pagination (recommended for large datasets)
GET /api/venues?limit=10&cursor=eyJpZCI6MTIzfQ==
GET /api/venues?after=2024-01-01T10:00:00Z&limit=25

// ✅ Good - Combined pagination with filtering
GET /api/venues?active=true&page=2&pageSize=10
GET /api/events?venueId=123&after=event_456&limit=20
```

## Resource Hierarchy Patterns

### Nested Resource URLs
Structure related resources hierarchically for clear relationships.

```csharp
// ✅ Good - Clear parent-child relationships
GET    /api/venues/{venueId}/acts                    // Acts for specific venue
POST   /api/venues/{venueId}/acts                    // Create act for venue
GET    /api/venues/{venueId}/acts/{actId}            // Specific act at venue
PUT    /api/venues/{venueId}/acts/{actId}            // Update specific act
DELETE /api/venues/{venueId}/acts/{actId}            // Remove act from venue

// ✅ Good - Three-level hierarchy
GET    /api/venues/{venueId}/acts/{actId}/shows                // Shows for act
POST   /api/venues/{venueId}/acts/{actId}/shows                // Create show
GET    /api/venues/{venueId}/acts/{actId}/shows/{showId}       // Specific show

// ✅ Good - Related resource operations
POST   /api/venues/{venueId}/acts/{actId}/shows/{showId}/tickets // Create tickets
GET    /api/venues/{venueId}/bookings                           // Venue bookings
GET    /api/customers/{customerId}/bookings                     // Customer bookings
```

### Resource Collection Operations
Batch and bulk operations on collections.

```csharp
// ✅ Good - Collection-level operations
GET    /api/venues/_count                    // Count venues without data transfer
GET    /api/venues/_export?format=csv        // Export venues
POST   /api/venues/_import                   // Bulk import venues
POST   /api/venues/_batch                    // Batch operations

// ✅ Good - Bulk operations with specific actions
POST   /api/venues/_activate                 // Activate multiple venues
POST   /api/venues/_deactivate              // Deactivate multiple venues
DELETE /api/venues/_batch                   // Bulk delete venues

// Request body for batch operations
{
  "venueIds": [123, 456, 789],
  "action": "activate",
  "reason": "End of maintenance period"
}
```

## Special URL Patterns

### Custom Actions (Non-CRUD)
When REST doesn't fit, use POST with action in URL.

```csharp
// ✅ Good - Resource-specific actions
POST /api/venues/{id}/activate              // Activate venue
POST /api/venues/{id}/deactivate            // Deactivate venue
POST /api/venues/{id}/publish               // Publish venue
POST /api/venues/{id}/archive               // Archive venue

// ✅ Good - Show/event actions
POST /api/shows/{id}/cancel                 // Cancel show
POST /api/shows/{id}/postpone               // Postpone show
POST /api/shows/{id}/sell-tickets           // Start ticket sales
POST /api/shows/{id}/stop-sales            // Stop ticket sales

// ✅ Good - Customer actions
POST /api/customers/{id}/suspend            // Suspend customer
POST /api/customers/{id}/reactivate         // Reactivate customer
POST /api/customers/{id}/send-notification  // Send notification

// Include context in request body
{
  "reason": "Maintenance required",
  "scheduledFor": "2024-01-15T10:00:00Z",
  "notifyCustomers": true
}
```

### Search and Discovery
Specialized endpoints for search and discovery operations.

```csharp
// ✅ Good - Global search
GET /api/search?q=broadway&type=venue,event  // Cross-entity search
GET /api/search/venues?q=theater             // Venue-specific search
GET /api/search/events?location=nyc          // Event search by location

// ✅ Good - Advanced search endpoints
POST /api/venues/search                      // Complex search with JSON body
POST /api/events/search                      // Advanced event search
GET  /api/venues/suggest?q=broad             // Auto-suggest/typeahead

// Complex search request body
{
  "query": {
    "bool": {
      "must": [
        { "match": { "name": "theater" } },
        { "range": { "capacity": { "gte": 100 } } }
      ],
      "filter": [
        { "term": { "active": true } },
        { "terms": { "category": ["music", "theater"] } }
      ]
    }
  },
  "sort": [{ "name": "asc" }],
  "size": 20
}
```

### File and Media URLs
Handling file uploads and media resources.

```csharp
// ✅ Good - File operations
POST   /api/venues/{id}/images               // Upload venue image
GET    /api/venues/{id}/images               // List venue images
GET    /api/venues/{id}/images/{imageId}     // Get specific image
DELETE /api/venues/{id}/images/{imageId}     // Delete image

// ✅ Good - Document handling
POST   /api/venues/{id}/documents            // Upload documents
GET    /api/venues/{id}/documents/contract   // Get contract document
PUT    /api/venues/{id}/documents/insurance  // Update insurance document

// ✅ Good - Media processing
POST   /api/venues/{id}/images/{imageId}/resize        // Resize image
POST   /api/venues/{id}/images/{imageId}/optimize      // Optimize image
GET    /api/venues/{id}/images/{imageId}/thumbnails    // Get thumbnails
```

### Administrative and Utility URLs
System administration and utility endpoints.

```csharp
// ✅ Good - Health and monitoring
GET /health                                 // Basic health check
GET /health/detailed                        // Detailed health status
GET /metrics                               // Performance metrics
GET /version                               // API version info

// ✅ Good - Administrative operations
GET /admin/stats                           // System statistics
GET /admin/audit-logs                      // Audit trail
POST /admin/cache/clear                    // Clear cache
POST /admin/maintenance/start              // Start maintenance mode

// ✅ Good - Development utilities
GET /api/schema                            // API schema/documentation
GET /api/openapi.json                      // OpenAPI specification
GET /api/docs                              // Interactive documentation
POST /api/test/reset                       // Reset test data (dev only)
```

## URL Design Best Practices

### Length and Readability
Keep URLs manageable and human-readable.

```csharp
// ✅ Good - Reasonable URL length
GET /api/venues/123/acts/456/shows/789
GET /api/customers/abc123/bookings/def456

// ❌ Bad - Overly long URL hierarchy
GET /api/venues/123/acts/456/shows/789/performances/012/tickets/345/seat-assignments/678

// ✅ Better - Break down complex hierarchies
GET /api/performances/012/tickets/345/seat-assignments/678
GET /api/seat-assignments?ticketId=345&performanceId=012
```

### Special Characters and Encoding
Handle special characters properly.

```csharp
// ✅ Good - Proper URL encoding
GET /api/venues?name=O'Reilly%20Theater
GET /api/venues?search=caf%C3%A9%20venue
GET /api/events?title=Rock%20%26%20Roll%20Concert

// ✅ Good - Using safe identifiers
GET /api/venues/music-hall-123
GET /api/events/rock-concert-2024

// ❌ Bad - Unencoded special characters
GET /api/venues?name=O'Reilly Theater
GET /api/events?title=Rock & Roll Concert
```

### Case Sensitivity Considerations
Consistent casing throughout the API.

```csharp
// ✅ Good - Consistent lowercase
GET /api/venues
GET /api/customer-accounts
GET /api/event-categories

// ❌ Bad - Mixed case inconsistency
GET /api/Venues
GET /api/CustomerAccounts
GET /api/eventCategories

// ✅ Good - Consistent query parameter naming
GET /api/venues?sortBy=name&sortOrder=asc
GET /api/venues?pageSize=20&pageNumber=1

// ❌ Bad - Inconsistent parameter casing
GET /api/venues?SortBy=name&sort_order=asc
GET /api/venues?PageSize=20&page_number=1
```