# Error Handling

Comprehensive error response patterns and validation for RESTful APIs.

## Standard Error Response Format

### RFC 7807 Problem Details
Standardized error response format for consistent error handling.

```csharp
// Standard error response structure
public record ProblemDetails(
    string Type,
    string Title,
    int Status,
    string Detail,
    string Instance,
    string? TraceId = null,
    Dictionary<string, object>? Extensions = null
);

// Example error response
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "The request could not be understood by the server due to malformed syntax.",
  "instance": "/api/venues",
  "traceId": "80000001-0000-0000-b63f-84710c7967bb"
}
```

### Validation Error Response
Detailed validation errors with field-level information.

```csharp
// Validation error response
public record ValidationProblemDetails(
    string Type,
    string Title,
    int Status,
    string Detail,
    string Instance,
    Dictionary<string, string[]> Errors,
    string? TraceId = null
) : ProblemDetails(Type, Title, Status, Detail, Instance, TraceId);

// Example validation error
POST /api/venues
{
  "name": "",
  "capacity": -5,
  "email": "invalid-email",
  "address": {
    "street": "",
    "postalCode": "invalid"
  }
}

422 Unprocessable Entity
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "detail": "Validation failed for one or more fields.",
  "instance": "/api/venues",
  "errors": {
    "name": [
      "Name is required.",
      "Name must be at least 3 characters long."
    ],
    "capacity": [
      "Capacity must be greater than 0.",
      "Capacity must be less than or equal to 100000."
    ],
    "email": [
      "Email is not in a valid format."
    ],
    "address.street": [
      "Street address is required."
    ],
    "address.postalCode": [
      "Postal code format is invalid."
    ]
  },
  "traceId": "80000001-0000-0000-b63f-84710c7967bb"
}
```

## Business Logic Errors

### Resource Not Found
Standard format for 404 errors.

```csharp
// Single resource not found
GET /api/venues/999999

404 Not Found
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource Not Found",
  "status": 404,
  "detail": "Venue with ID '999999' was not found.",
  "instance": "/api/venues/999999",
  "traceId": "80000002-0000-0000-b63f-84710c7967bb",
  "resourceType": "venue",
  "resourceId": "999999"
}

// Nested resource not found
GET /api/venues/{venueId}/acts/999999

404 Not Found
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Act Not Found",
  "status": 404,
  "detail": "Act with ID '999999' was not found for venue '{venueId}'.",
  "instance": "/api/venues/{venueId}/acts/999999",
  "traceId": "80000003-0000-0000-b63f-84710c7967bb",
  "resourceType": "act",
  "resourceId": "999999",
  "parentResourceType": "venue",
  "parentResourceId": "{venueId}"
}
```

### Conflict Errors
Handling resource conflicts and duplicate data.

```csharp
// Duplicate resource conflict
POST /api/venues
{
  "name": "Grand Theater",  // Already exists
  "address": { ... }
}

409 Conflict
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Resource Conflict",
  "status": 409,
  "detail": "A venue with the name 'Grand Theater' already exists.",
  "instance": "/api/venues",
  "traceId": "80000004-0000-0000-b63f-84710c7967bb",
  "conflictType": "duplicate_resource",
  "conflictingField": "name",
  "conflictingValue": "Grand Theater",
  "existingResourceId": "123e4567-e89b-12d3-a456-426614174001",
  "existingResourceUrl": "/api/venues/123e4567-e89b-12d3-a456-426614174001"
}

// Concurrent modification conflict
PUT /api/venues/{id}
If-Match: "old-etag-value"

409 Conflict
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Concurrent Modification",
  "status": 409,
  "detail": "The resource has been modified by another request since it was last retrieved.",
  "instance": "/api/venues/{id}",
  "traceId": "80000005-0000-0000-b63f-84710c7967bb",
  "conflictType": "concurrent_modification",
  "providedETag": "old-etag-value",
  "currentETag": "current-etag-value",
  "lastModifiedBy": "user@example.com",
  "lastModifiedAt": "2024-12-06T15:30:00Z"
}
```

### Business Rule Violations
Errors for domain-specific business rules.

```csharp
// Business rule violation
DELETE /api/venues/{id}
// Venue has active bookings

422 Unprocessable Entity
{
  "type": "https://example.com/problems/business-rule-violation",
  "title": "Business Rule Violation",
  "status": 422,
  "detail": "Cannot delete venue because it has active bookings.",
  "instance": "/api/venues/{id}",
  "traceId": "80000006-0000-0000-b63f-84710c7967bb",
  "violatedRule": "venue_deletion_with_active_bookings",
  "activeBookingsCount": 15,
  "nextAvailableDeletionDate": "2025-01-15T00:00:00Z",
  "suggestions": [
    "Cancel all active bookings first",
    "Mark venue as inactive instead of deleting",
    "Schedule deletion for after last booking ends"
  ]
}

// Capacity exceeded
POST /api/venues/{venueId}/events/{eventId}/tickets
{
  "quantity": 100
}

422 Unprocessable Entity
{
  "type": "https://example.com/problems/capacity-exceeded",
  "title": "Venue Capacity Exceeded",
  "status": 422,
  "detail": "Cannot sell 100 tickets. Only 25 seats remain available.",
  "instance": "/api/venues/{venueId}/events/{eventId}/tickets",
  "traceId": "80000007-0000-0000-b63f-84710c7967bb",
  "requestedQuantity": 100,
  "availableQuantity": 25,
  "totalCapacity": 1500,
  "soldQuantity": 1475
}
```

## Authentication and Authorization Errors

### Authentication Required
Missing or invalid authentication.

```csharp
// Missing authentication
GET /api/venues

401 Unauthorized
WWW-Authenticate: Bearer realm="GloboTicket API"
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication is required to access this resource.",
  "instance": "/api/venues",
  "traceId": "80000008-0000-0000-b63f-84710c7967bb",
  "authenticationSchemes": ["Bearer"],
  "loginUrl": "/auth/login",
  "documentationUrl": "https://docs.globoticket.com/authentication"
}

// Invalid/expired token
GET /api/venues
Authorization: Bearer expired_token

401 Unauthorized
WWW-Authenticate: Bearer realm="GloboTicket API", error="invalid_token", error_description="The access token expired"
{
  "type": "https://tools.ietf.org/html/rfc6750#section-3.1",
  "title": "Invalid Token",
  "status": 401,
  "detail": "The provided access token is invalid or has expired.",
  "instance": "/api/venues",
  "traceId": "80000009-0000-0000-b63f-84710c7967bb",
  "errorCode": "invalid_token",
  "tokenStatus": "expired",
  "expiredAt": "2024-12-06T14:00:00Z",
  "refreshTokenUrl": "/auth/refresh"
}
```

### Authorization Denied
Valid authentication but insufficient permissions.

```csharp
// Role-based authorization failure
DELETE /api/venues/{id}
Authorization: Bearer valid_user_token

403 Forbidden
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to delete venues.",
  "instance": "/api/venues/{id}",
  "traceId": "80000010-0000-0000-b63f-84710c7967bb",
  "requiredRole": "admin",
  "currentRole": "user",
  "requiredPermissions": ["venues:delete"],
  "currentPermissions": ["venues:read", "venues:create", "venues:update"],
  "contactInfo": "Contact your administrator to request additional permissions"
}

// Tenant isolation violation
GET /api/venues/{id}
Authorization: Bearer tenant_a_token
// Requesting resource from tenant B

403 Forbidden
{
  "type": "https://example.com/problems/tenant-access-denied",
  "title": "Tenant Access Denied",
  "status": 403,
  "detail": "You do not have access to resources from this tenant.",
  "instance": "/api/venues/{id}",
  "traceId": "80000011-0000-0000-b63f-84710c7967bb",
  "requestedTenant": "tenant-b",
  "currentTenant": "tenant-a",
  "resourceId": "{id}"
}
```

## Rate Limiting Errors

### Rate Limit Exceeded
Too many requests from client.

```csharp
// Rate limit exceeded
GET /api/venues

429 Too Many Requests
Retry-After: 3600
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1701878400
{
  "type": "https://tools.ietf.org/html/rfc6585#section-4",
  "title": "Rate Limit Exceeded",
  "status": 429,
  "detail": "Rate limit of 1000 requests per hour exceeded.",
  "instance": "/api/venues",
  "traceId": "80000012-0000-0000-b63f-84710c7967bb",
  "rateLimit": {
    "limit": 1000,
    "period": "1 hour",
    "remaining": 0,
    "resetTime": "2024-12-06T17:00:00Z",
    "retryAfterSeconds": 3600
  },
  "upgradeOptions": [
    {
      "plan": "premium",
      "limit": 5000,
      "url": "/billing/upgrade"
    }
  ]
}
```

## Server Errors

### Internal Server Error
Unexpected server-side errors.

```csharp
// Generic server error
GET /api/venues/{id}

500 Internal Server Error
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred while processing your request.",
  "instance": "/api/venues/{id}",
  "traceId": "80000013-0000-0000-b63f-84710c7967bb",
  "supportReference": "Please contact support with trace ID 80000013-0000-0000-b63f-84710c7967bb",
  "timestamp": "2024-12-06T16:30:00Z"
}
```

### Service Unavailable
Temporary service issues.

```csharp
// Maintenance mode
GET /api/venues

503 Service Unavailable
Retry-After: 1800
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.4",
  "title": "Service Unavailable",
  "status": 503,
  "detail": "Service is temporarily unavailable for scheduled maintenance.",
  "instance": "/api/venues",
  "traceId": "80000014-0000-0000-b63f-84710c7967bb",
  "maintenance": {
    "reason": "scheduled_maintenance",
    "estimatedDuration": "30 minutes",
    "startTime": "2024-12-06T16:00:00Z",
    "endTime": "2024-12-06T16:30:00Z",
    "statusPageUrl": "https://status.globoticket.com"
  },
  "retryAfterSeconds": 1800
}

// Database connectivity issues
GET /api/venues

503 Service Unavailable
{
  "type": "https://example.com/problems/database-unavailable",
  "title": "Database Unavailable",
  "status": 503,
  "detail": "Unable to connect to the database. Please try again later.",
  "instance": "/api/venues",
  "traceId": "80000015-0000-0000-b63f-84710c7967bb",
  "service": "database",
  "estimatedRecoveryTime": "2024-12-06T16:45:00Z",
  "statusPageUrl": "https://status.globoticket.com"
}
```

## Input Validation Patterns

### Field-Level Validation
Detailed validation for individual fields.

```csharp
// Comprehensive field validation
{
  "type": "https://tools.ietf.org/html/rfc4918#section-11.2",
  "title": "Validation Failed",
  "status": 422,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/venues",
  "errors": {
    "name": [
      "Name is required.",
      "Name must be between 3 and 100 characters.",
      "Name cannot contain special characters except spaces, hyphens, and apostrophes."
    ],
    "email": [
      "Email is required.",
      "Email must be in a valid format.",
      "Email must not exceed 320 characters."
    ],
    "capacity": [
      "Capacity is required.",
      "Capacity must be a positive integer.",
      "Capacity must be between 1 and 100,000."
    ],
    "address.street": [
      "Street address is required.",
      "Street address must not exceed 200 characters."
    ],
    "address.postalCode": [
      "Postal code is required.",
      "Postal code format is invalid for the specified country."
    ],
    "phoneNumber": [
      "Phone number format is invalid.",
      "Phone number must include country code."
    ]
  },
  "traceId": "80000016-0000-0000-b63f-84710c7967bb"
}
```

### Custom Validation Rules
Domain-specific validation patterns.

```csharp
// Business rule validation
{
  "type": "https://example.com/problems/business-validation",
  "title": "Business Validation Failed",
  "status": 422,
  "detail": "The request violates one or more business rules.",
  "instance": "/api/events",
  "errors": {
    "startDate": [
      "Event start date must be at least 24 hours in the future.",
      "Event start date cannot be more than 2 years in the future."
    ],
    "endDate": [
      "Event end date must be after start date.",
      "Event duration cannot exceed 8 hours."
    ],
    "ticketPrice": [
      "Ticket price must be greater than $0.",
      "Ticket price cannot exceed $10,000.",
      "Ticket price must be divisible by $0.01."
    ],
    "venueCapacity": [
      "Number of tickets cannot exceed venue capacity of 1,500."
    ]
  },
  "traceId": "80000017-0000-0000-b63f-84710c7967bb"
}
```

## Error Logging and Monitoring

### Error Context
Additional context for debugging and monitoring.

```csharp
// Error with debugging context (internal logging)
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred while processing your request.",
  "instance": "/api/venues/{id}",
  "traceId": "80000018-0000-0000-b63f-84710c7967bb",
  "timestamp": "2024-12-06T16:30:00Z",
  "correlationId": "req_123456789",
  "userId": "user_abc123",
  "tenantId": "tenant_xyz789",
  "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
  "ipAddress": "192.168.1.100",
  "requestMethod": "GET",
  "requestPath": "/api/venues/123e4567-e89b-12d3-a456-426614174001",
  "responseTime": "2.5s",
  "errorCode": "DB_CONNECTION_TIMEOUT",
  "internalMessage": "Connection to database pool timed out after 30 seconds"
}
```

### Error Recovery Suggestions
Help clients recover from errors.

```csharp
// Error with recovery suggestions
{
  "type": "https://example.com/problems/network-timeout",
  "title": "Network Timeout",
  "status": 504,
  "detail": "The request timed out while communicating with an external service.",
  "instance": "/api/venues/{id}/weather",
  "traceId": "80000019-0000-0000-b63f-84710c7967bb",
  "service": "weather_api",
  "timeout": "30 seconds",
  "suggestions": [
    "Retry the request after a few minutes",
    "Check the service status page",
    "Use cached weather data if available",
    "Contact support if the issue persists"
  ],
  "retryPolicy": {
    "recommended": true,
    "maxRetries": 3,
    "backoffSeconds": [10, 30, 60]
  },
  "serviceStatusUrl": "https://status.weatherapi.com",
  "supportContact": "support@globoticket.com"
}
```

This comprehensive error handling ensures clients can understand and appropriately respond to different failure scenarios.