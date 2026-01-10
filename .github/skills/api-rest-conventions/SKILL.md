---
name: api-rest-conventions
description: Essential RESTful API design conventions for URL patterns, HTTP methods, and response formats. For detailed implementations and advanced patterns, see references section.
---

# RESTful API Design Conventions

Essential patterns for designing consistent, maintainable RESTful APIs.

## Core Principles

### 1. Resource-Based URLs
Use plural nouns for resources, avoid actions in URLs.

```csharp
// ✅ Good
GET    /api/venues          // Get all venues
GET    /api/venues/{id}     // Get specific venue
POST   /api/venues          // Create venue
PUT    /api/venues/{id}     // Update venue
DELETE /api/venues/{id}     // Delete venue

// ❌ Bad
GET    /api/getVenues
POST   /api/createVenue
```

### 2. HTTP Method Usage
Map operations to appropriate HTTP methods.

```csharp
GET    /api/venues        // Read (safe, cacheable)
POST   /api/venues        // Create (not idempotent)
PUT    /api/venues/{id}   // Replace (idempotent)
PATCH  /api/venues/{id}   // Partial update (idempotent)
DELETE /api/venues/{id}   // Remove (idempotent)
```

### 3. Nested Resources
Structure related resources hierarchically.

```csharp
// ✅ Good - Clear hierarchy
GET    /api/venues/{id}/acts         // Acts for venue
POST   /api/venues/{id}/acts         // Create act for venue
GET    /api/acts/{id}/shows          // Shows for act

// ❌ Bad - Flat structure loses context
GET    /api/acts?venueId={id}
```

## Response Formats

### Success Responses
Standard patterns for successful operations.

```csharp
// GET - Return resource(s)
200 OK
{
  "id": 123,
  "name": "Music Hall",
  "capacity": 500
}

// POST - Return created resource with location
201 Created
Location: /api/venues/123
{
  "id": 123,
  "name": "Music Hall"
}

// PUT/PATCH - Return updated resource
200 OK
{
  "id": 123,
  "name": "Updated Music Hall"
}

// DELETE - Confirm deletion
204 No Content
```

### Error Responses
Consistent error structure.

```csharp
400 Bad Request
{
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "Invalid venue data",
    "details": [
      {
        "field": "name",
        "message": "Name is required"
      }
    ]
  }
}
```

## Common Status Codes

Essential status codes for RESTful APIs:

- **200 OK** - Successful GET, PUT, PATCH
- **201 Created** - Successful POST
- **204 No Content** - Successful DELETE
- **400 Bad Request** - Invalid client request
- **401 Unauthorized** - Missing/invalid authentication
- **403 Forbidden** - Access denied
- **404 Not Found** - Resource doesn't exist
- **409 Conflict** - Resource conflict (duplicate)
- **422 Unprocessable Entity** - Validation errors
- **500 Internal Server Error** - Server error

## References

For detailed implementations and advanced patterns:

- **[URL Naming Conventions](references/url-naming.md)** - Detailed URL structure, naming patterns, and query parameters
- **[HTTP Status Codes](references/status-codes.md)** - Complete status code reference with usage scenarios
- **[Request/Response Formats](references/request-response.md)** - DTOs, serialization, content negotiation
- **[Error Handling](references/error-handling.md)** - Comprehensive error response patterns and validation
- **[Authentication & Security](references/security.md)** - JWT, API keys, CORS, rate limiting
- **[Performance & Caching](references/performance.md)** - ETags, compression, pagination optimization
- **[API Testing](references/testing.md)** - Integration tests, contract testing, mock strategies
- **[Documentation](references/documentation.md)** - OpenAPI/Swagger specifications and examples