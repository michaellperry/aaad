# HTTP Status Codes Reference

Complete HTTP status code reference with usage scenarios for RESTful APIs.

## 2xx Success Codes

### 200 OK
Successful GET, PUT, PATCH operations.

```csharp
// GET - Resource retrieval
GET /api/venues/{id}

200 OK
Content-Type: application/json
{
  "id": 123,
  "name": "Grand Theater",
  "capacity": 1500,
  "isActive": true
}

// PUT - Complete resource replacement
PUT /api/venues/{id}
Content-Type: application/json
{
  "name": "Updated Grand Theater",
  "capacity": 1600,
  "address": "456 Broadway"
}

200 OK
Content-Type: application/json
{
  "id": 123,
  "name": "Updated Grand Theater",
  "capacity": 1600,
  "address": "456 Broadway",
  "updatedAt": "2024-12-06T15:30:00Z"
}

// PATCH - Partial resource update
PATCH /api/venues/{id}
Content-Type: application/json
{
  "capacity": 1700
}

200 OK
Content-Type: application/json
{
  "id": 123,
  "name": "Grand Theater",
  "capacity": 1700,
  "updatedAt": "2024-12-06T15:45:00Z"
}
```

### 201 Created
Successful POST operations for resource creation.

```csharp
// POST - Create new resource
POST /api/venues
Content-Type: application/json
{
  "name": "New Theater",
  "capacity": 800,
  "address": "789 Main St"
}

201 Created
Location: /api/venues/456
Content-Type: application/json
{
  "id": 456,
  "name": "New Theater",
  "capacity": 800,
  "address": "789 Main St",
  "isActive": true,
  "createdAt": "2024-12-06T16:00:00Z"
}

// POST - Create nested resource
POST /api/venues/{venueId}/acts
Content-Type: application/json
{
  "name": "Jazz Night",
  "genre": "Jazz",
  "duration": 120
}

201 Created
Location: /api/venues/123/acts/789
Content-Type: application/json
{
  "id": 789,
  "name": "Jazz Night",
  "genre": "Jazz",
  "duration": 120,
  "venueId": 123,
  "createdAt": "2024-12-06T16:15:00Z"
}
```

### 202 Accepted
Request accepted for asynchronous processing.

```csharp
// POST - Long-running operation
POST /api/venues/import
Content-Type: application/json
{
  "sourceFile": "venues.csv",
  "validateOnly": false,
  "notifyOnComplete": true
}

202 Accepted
Location: /api/operations/import-123
Content-Type: application/json
{
  "operationId": "import-123",
  "status": "accepted",
  "estimatedCompletionTime": "2024-12-06T17:00:00Z",
  "statusUrl": "/api/operations/import-123",
  "message": "Import operation started. Processing 1000 venues."
}

// Check operation status
GET /api/operations/import-123

200 OK
{
  "operationId": "import-123",
  "status": "running",
  "progress": {
    "processed": 450,
    "total": 1000,
    "percentage": 45,
    "errors": 5
  },
  "startedAt": "2024-12-06T16:30:00Z",
  "estimatedCompletionTime": "2024-12-06T17:00:00Z"
}
```

### 204 No Content
Successful operation with no response body.

```csharp
// DELETE - Resource deletion
DELETE /api/venues/{id}

204 No Content

// PUT - Update without returning updated resource
PUT /api/venues/{id}/settings
Content-Type: application/json
{
  "emailNotifications": true,
  "autoPublish": false
}

204 No Content

// POST - Action with no meaningful response
POST /api/venues/{id}/activate
Content-Type: application/json
{
  "reason": "Maintenance completed",
  "activatedBy": "admin@example.com"
}

204 No Content
```

### 206 Partial Content
Partial response for range requests or pagination.

```csharp
// GET - Range request for large file
GET /api/venues/{id}/images/large.jpg
Range: bytes=0-1023

206 Partial Content
Content-Range: bytes 0-1023/2048576
Content-Length: 1024
Content-Type: image/jpeg
[binary data]

// GET - Paginated response (alternative to 200)
GET /api/venues?page=2&limit=10

206 Partial Content
Content-Range: venues 10-19/150
Link: </api/venues?page=1&limit=10>; rel="prev", 
      </api/venues?page=3&limit=10>; rel="next"
{
  "data": [...],
  "pagination": {
    "page": 2,
    "limit": 10,
    "total": 150,
    "pages": 15
  }
}
```

## 3xx Redirection Codes

### 301 Moved Permanently
Permanent redirection for API versioning or resource relocation.

```csharp
// Deprecated API endpoint
GET /api/venues/getAll

301 Moved Permanently
Location: /api/v1/venues
Content-Type: application/json
{
  "error": {
    "code": "ENDPOINT_MOVED",
    "message": "This endpoint has been moved permanently",
    "newLocation": "/api/v1/venues",
    "deprecated": true
  }
}
```

### 302 Found / 303 See Other
Temporary redirection or redirect after POST.

```csharp
// POST - Redirect after creation (PRG pattern)
POST /api/venues

303 See Other
Location: /api/venues/123
Content-Type: application/json
{
  "message": "Venue created successfully",
  "resourceLocation": "/api/venues/123"
}
```

### 304 Not Modified
Conditional requests - resource unchanged.

```csharp
// Conditional GET with ETag
GET /api/venues/{id}
If-None-Match: "686897696a7c876b7e"

304 Not Modified
ETag: "686897696a7c876b7e"
Cache-Control: max-age=3600

// Conditional GET with Last-Modified
GET /api/venues/{id}
If-Modified-Since: Thu, 06 Dec 2024 10:00:00 GMT

304 Not Modified
Last-Modified: Thu, 06 Dec 2024 10:00:00 GMT
Cache-Control: max-age=3600
```

## 4xx Client Error Codes

### 400 Bad Request
Invalid request syntax or malformed request.

```csharp
// Invalid JSON syntax
POST /api/venues
Content-Type: application/json
{
  "name": "Theater",
  "capacity": 500,  // trailing comma error
}

400 Bad Request
Content-Type: application/json
{
  "error": {
    "code": "INVALID_JSON",
    "message": "Request body contains invalid JSON",
    "details": {
      "line": 4,
      "column": 1,
      "description": "Unexpected token } in JSON"
    }
  }
}

// Invalid query parameters
GET /api/venues?page=invalid&limit=abc

400 Bad Request
{
  "error": {
    "code": "INVALID_PARAMETERS",
    "message": "One or more query parameters are invalid",
    "details": [
      {
        "parameter": "page",
        "value": "invalid",
        "message": "Page must be a positive integer"
      },
      {
        "parameter": "limit",
        "value": "abc",
        "message": "Limit must be an integer between 1 and 100"
      }
    ]
  }
}

// Missing required headers
POST /api/venues
// Missing Content-Type header

400 Bad Request
{
  "error": {
    "code": "MISSING_CONTENT_TYPE",
    "message": "Content-Type header is required for POST requests",
    "requiredHeaders": ["Content-Type"]
  }
}
```

### 401 Unauthorized
Missing or invalid authentication.

```csharp
// Missing Authorization header
GET /api/venues

401 Unauthorized
WWW-Authenticate: Bearer realm="GloboTicket API"
Content-Type: application/json
{
  "error": {
    "code": "AUTHENTICATION_REQUIRED",
    "message": "Authentication is required to access this resource",
    "details": {
      "requiredAuth": "Bearer token",
      "loginUrl": "/auth/login"
    }
  }
}

// Invalid/expired token
GET /api/venues
Authorization: Bearer invalid_token_here

401 Unauthorized
WWW-Authenticate: Bearer realm="GloboTicket API", error="invalid_token"
{
  "error": {
    "code": "INVALID_TOKEN",
    "message": "The provided authentication token is invalid or expired",
    "details": {
      "tokenStatus": "expired",
      "expiredAt": "2024-12-06T14:00:00Z",
      "refreshUrl": "/auth/refresh"
    }
  }
}
```

### 403 Forbidden
Valid authentication but insufficient permissions.

```csharp
// Insufficient role permissions
DELETE /api/venues/{id}
Authorization: Bearer valid_user_token

403 Forbidden
Content-Type: application/json
{
  "error": {
    "code": "INSUFFICIENT_PERMISSIONS",
    "message": "You do not have permission to delete venues",
    "details": {
      "requiredRole": "admin",
      "currentRole": "user",
      "requiredPermissions": ["venues:delete"],
      "currentPermissions": ["venues:read", "venues:create"]
    }
  }
}

// Tenant access restriction
GET /api/venues/{id}
Authorization: Bearer tenant_a_token
// Requesting resource from tenant B

403 Forbidden
{
  "error": {
    "code": "TENANT_ACCESS_DENIED",
    "message": "You do not have access to resources from this tenant",
    "details": {
      "requestedTenant": "tenant-b",
      "currentTenant": "tenant-a"
    }
  }
}
```

### 404 Not Found
Resource doesn't exist or user doesn't have access.

```csharp
// Resource not found
GET /api/venues/999999

404 Not Found
Content-Type: application/json
{
  "error": {
    "code": "RESOURCE_NOT_FOUND",
    "message": "Venue with ID 999999 was not found",
    "details": {
      "resourceType": "venue",
      "resourceId": "999999"
    }
  }
}

// Nested resource not found
GET /api/venues/{venueId}/acts/999999

404 Not Found
{
  "error": {
    "code": "ACT_NOT_FOUND",
    "message": "Act with ID 999999 was not found for venue 123",
    "details": {
      "resourceType": "act",
      "resourceId": "999999",
      "parentResource": "venue",
      "parentId": "123"
    }
  }
}

// Endpoint not found
GET /api/invalid-endpoint

404 Not Found
{
  "error": {
    "code": "ENDPOINT_NOT_FOUND",
    "message": "The requested endpoint was not found",
    "details": {
      "path": "/api/invalid-endpoint",
      "method": "GET",
      "suggestions": ["/api/venues", "/api/events", "/api/customers"]
    }
  }
}
```

### 405 Method Not Allowed
HTTP method not supported for the endpoint.

```csharp
// PATCH not supported
PATCH /api/venues/{id}/activate

405 Method Not Allowed
Allow: POST
Content-Type: application/json
{
  "error": {
    "code": "METHOD_NOT_ALLOWED",
    "message": "PATCH method is not allowed for this endpoint",
    "details": {
      "allowedMethods": ["POST"],
      "requestedMethod": "PATCH",
      "endpoint": "/api/venues/{id}/activate"
    }
  }
}

// PUT on collection endpoint
PUT /api/venues

405 Method Not Allowed
Allow: GET, POST
{
  "error": {
    "code": "METHOD_NOT_ALLOWED",
    "message": "PUT method is not allowed on collection endpoints",
    "details": {
      "allowedMethods": ["GET", "POST"],
      "requestedMethod": "PUT",
      "endpoint": "/api/venues"
    }
  }
}
```

### 406 Not Acceptable
Requested format not available.

```csharp
// Unsupported Accept header
GET /api/venues
Accept: application/xml

406 Not Acceptable
Content-Type: application/json
{
  "error": {
    "code": "UNSUPPORTED_MEDIA_TYPE",
    "message": "The requested media type is not supported",
    "details": {
      "requestedType": "application/xml",
      "supportedTypes": ["application/json", "application/hal+json"]
    }
  }
}
```

### 409 Conflict
Resource conflict, typically for duplicates or concurrent modifications.

```csharp
// Duplicate resource
POST /api/venues
Content-Type: application/json
{
  "name": "Grand Theater",  // Already exists
  "address": "123 Broadway"
}

409 Conflict
Content-Type: application/json
{
  "error": {
    "code": "RESOURCE_CONFLICT",
    "message": "A venue with this name already exists",
    "details": {
      "conflictingField": "name",
      "conflictingValue": "Grand Theater",
      "existingResourceId": 123,
      "existingResourceUrl": "/api/venues/123"
    }
  }
}

// Concurrent modification conflict
PUT /api/venues/{id}
If-Match: "old-etag-value"
Content-Type: application/json
{
  "name": "Updated Theater Name"
}

409 Conflict
ETag: "current-etag-value"
{
  "error": {
    "code": "CONCURRENT_MODIFICATION",
    "message": "Resource has been modified by another request",
    "details": {
      "providedETag": "old-etag-value",
      "currentETag": "current-etag-value",
      "lastModifiedBy": "user@example.com",
      "lastModifiedAt": "2024-12-06T15:30:00Z"
    }
  }
}
```

### 410 Gone
Resource previously existed but is no longer available.

```csharp
// Deleted resource
GET /api/venues/{id}
// Resource was deleted

410 Gone
Content-Type: application/json
{
  "error": {
    "code": "RESOURCE_DELETED",
    "message": "Venue was permanently deleted",
    "details": {
      "deletedAt": "2024-12-01T10:00:00Z",
      "deletedBy": "admin@example.com",
      "reason": "Venue closed permanently"
    }
  }
}
```

### 412 Precondition Failed
Conditional request failed.

```csharp
// If-Match failed
PUT /api/venues/{id}
If-Match: "wrong-etag"

412 Precondition Failed
ETag: "correct-etag"
{
  "error": {
    "code": "PRECONDITION_FAILED",
    "message": "The precondition specified in the request failed",
    "details": {
      "failedPrecondition": "If-Match",
      "providedValue": "wrong-etag",
      "currentValue": "correct-etag"
    }
  }
}

// If-Unmodified-Since failed
DELETE /api/venues/{id}
If-Unmodified-Since: Thu, 05 Dec 2024 10:00:00 GMT

412 Precondition Failed
Last-Modified: Thu, 06 Dec 2024 15:30:00 GMT
{
  "error": {
    "code": "RESOURCE_MODIFIED",
    "message": "Resource has been modified since the specified date",
    "details": {
      "specifiedDate": "2024-12-05T10:00:00Z",
      "lastModified": "2024-12-06T15:30:00Z"
    }
  }
}
```

### 413 Payload Too Large
Request entity too large.

```csharp
// Large file upload
POST /api/venues/{id}/images
Content-Type: multipart/form-data
Content-Length: 52428800  // 50MB file

413 Payload Too Large
Content-Type: application/json
{
  "error": {
    "code": "PAYLOAD_TOO_LARGE",
    "message": "Request payload exceeds maximum allowed size",
    "details": {
      "maxSize": "10MB",
      "receivedSize": "50MB",
      "suggestions": [
        "Compress the image before uploading",
        "Use chunked upload for large files",
        "Consider using a CDN for large media files"
      ]
    }
  }
}
```

### 415 Unsupported Media Type
Unsupported Content-Type.

```csharp
// Wrong Content-Type
POST /api/venues
Content-Type: text/plain
New venue data

415 Unsupported Media Type
Content-Type: application/json
{
  "error": {
    "code": "UNSUPPORTED_MEDIA_TYPE",
    "message": "The media type of the request is not supported",
    "details": {
      "providedType": "text/plain",
      "supportedTypes": ["application/json", "application/x-www-form-urlencoded"],
      "endpoint": "/api/venues",
      "method": "POST"
    }
  }
}
```

### 422 Unprocessable Entity
Validation errors in request data.

```csharp
// Validation failures
POST /api/venues
Content-Type: application/json
{
  "name": "",
  "capacity": -5,
  "email": "invalid-email"
}

422 Unprocessable Entity
Content-Type: application/json
{
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "One or more validation errors occurred",
    "details": [
      {
        "field": "name",
        "message": "Name is required and cannot be empty",
        "code": "REQUIRED",
        "value": ""
      },
      {
        "field": "capacity",
        "message": "Capacity must be a positive integer",
        "code": "INVALID_RANGE",
        "value": -5,
        "constraints": { "min": 1, "max": 100000 }
      },
      {
        "field": "email",
        "message": "Email must be a valid email address",
        "code": "INVALID_FORMAT",
        "value": "invalid-email",
        "pattern": "^[\\w\\.-]+@[\\w\\.-]+\\.[a-zA-Z]{2,}$"
      }
    ],
    "traceId": "abc123def456"
  }
}
```

### 429 Too Many Requests
Rate limiting exceeded.

```csharp
// Rate limit exceeded
GET /api/venues
Authorization: Bearer user_token

429 Too Many Requests
Retry-After: 3600
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1701878400
Content-Type: application/json
{
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded. Please try again later.",
    "details": {
      "limit": 1000,
      "period": "1 hour",
      "remaining": 0,
      "resetTime": "2024-12-06T17:00:00Z",
      "retryAfter": 3600
    }
  }
}
```

## 5xx Server Error Codes

### 500 Internal Server Error
Generic server error.

```csharp
// Unexpected server error
GET /api/venues/{id}

500 Internal Server Error
Content-Type: application/json
{
  "error": {
    "code": "INTERNAL_SERVER_ERROR",
    "message": "An unexpected error occurred while processing your request",
    "details": {
      "traceId": "abc123def456",
      "timestamp": "2024-12-06T16:30:00Z",
      "supportReference": "Please contact support with trace ID abc123def456"
    }
  }
}
```

### 502 Bad Gateway
Upstream service error.

```csharp
// Database connection failed
GET /api/venues

502 Bad Gateway
Content-Type: application/json
{
  "error": {
    "code": "UPSTREAM_SERVICE_ERROR",
    "message": "Unable to connect to database service",
    "details": {
      "service": "database",
      "status": "connection_failed",
      "retryAfter": 30,
      "healthCheckUrl": "/health"
    }
  }
}
```

### 503 Service Unavailable
Temporary service unavailability.

```csharp
// Maintenance mode
GET /api/venues

503 Service Unavailable
Retry-After: 1800
Content-Type: application/json
{
  "error": {
    "code": "SERVICE_UNAVAILABLE",
    "message": "Service is temporarily unavailable for maintenance",
    "details": {
      "reason": "scheduled_maintenance",
      "estimatedDowntime": "30 minutes",
      "retryAfter": 1800,
      "maintenanceWindow": {
        "start": "2024-12-06T16:00:00Z",
        "end": "2024-12-06T16:30:00Z"
      }
    }
  }
}
```

### 504 Gateway Timeout
Upstream service timeout.

```csharp
// External service timeout
GET /api/venues/{id}/weather

504 Gateway Timeout
Content-Type: application/json
{
  "error": {
    "code": "GATEWAY_TIMEOUT",
    "message": "The external weather service did not respond in time",
    "details": {
      "service": "weather_api",
      "timeout": "30 seconds",
      "suggestion": "Please try again later or check service status",
      "serviceStatusUrl": "https://status.weatherapi.com"
    }
  }
}
```

These status codes provide clear communication about request outcomes and help clients handle different scenarios appropriately.