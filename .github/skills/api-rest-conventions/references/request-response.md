# Request/Response Formats

DTOs, serialization, and content negotiation patterns for RESTful APIs.

## Data Transfer Objects (DTOs)

### Request/Response DTOs
Separate DTOs for different operations to ensure proper data encapsulation.

```csharp
// Create DTO (input)
public record CreateVenueDto(
    [Required] string Name,
    [Required] AddressDto Address,
    [Range(1, 100000)] int Capacity,
    [Required] VenueTypeDto Type,
    string? Description = null,
    bool IsActive = true
);

// Response DTO (output)
public record VenueDto(
    Guid Id,
    string Name,
    AddressDto Address,
    int Capacity,
    VenueTypeDto Type,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

// Update DTO (input - all optional for PATCH)
public record UpdateVenueDto(
    string? Name = null,
    AddressDto? Address = null,
    [Range(1, 100000)] int? Capacity = null,
    VenueTypeDto? Type = null,
    string? Description = null,
    bool? IsActive = null
);

// List DTO (output - simplified for collections)
public record VenueListDto(
    Guid Id,
    string Name,
    string City,
    int Capacity,
    bool IsActive,
    DateTime CreatedAt
);

// Summary DTO (minimal data for dropdowns/references)
public record VenueSummaryDto(
    Guid Id,
    string Name,
    string City
);
```

### Nested DTOs
Reusable DTOs for complex object composition.

```csharp
// Address DTO
public record AddressDto(
    [Required] string Street,
    [Required] string City,
    [Required] string State,
    [Required] string PostalCode,
    [Required] string Country,
    string? Suite = null,
    decimal? Latitude = null,
    decimal? Longitude = null
);

// Venue Type DTO
public record VenueTypeDto(
    Guid Id,
    string Name,
    string Category,
    string? Description = null
);

// Contact Information DTO
public record ContactInfoDto(
    string? Email = null,
    string? Phone = null,
    string? Website = null,
    SocialMediaDto? SocialMedia = null
);

public record SocialMediaDto(
    string? Facebook = null,
    string? Twitter = null,
    string? Instagram = null
);
```

## Collection Response Formats

### Paginated Response
Standard format for paginated collections.

```csharp
// Paginated response wrapper
public record PagedResponse<T>(
    IEnumerable<T> Data,
    PaginationMeta Pagination,
    LinksMeta? Links = null
) where T : class;

public record PaginationMeta(
    int Page,
    int PageSize,
    int TotalPages,
    int TotalItems,
    bool HasNext,
    bool HasPrevious
);

public record LinksMeta(
    string? First = null,
    string? Previous = null,
    string? Next = null,
    string? Last = null
);

// Example response
{
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174001",
      "name": "Grand Theater",
      "city": "New York",
      "capacity": 1500,
      "isActive": true,
      "createdAt": "2024-12-06T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalItems": 87,
    "hasNext": true,
    "hasPrevious": false
  },
  "links": {
    "first": "/api/venues?page=1&pageSize=20",
    "next": "/api/venues?page=2&pageSize=20",
    "last": "/api/venues?page=5&pageSize=20"
  }
}
```

### Filtered Response
Response format for filtered collections.

```csharp
// Filtered response wrapper
public record FilteredResponse<T>(
    IEnumerable<T> Data,
    FilterMeta Filters,
    PaginationMeta? Pagination = null
) where T : class;

public record FilterMeta(
    Dictionary<string, object> AppliedFilters,
    int FilteredCount,
    int TotalCount
);

// Example response
{
  "data": [...],
  "filters": {
    "appliedFilters": {
      "isActive": true,
      "city": "New York",
      "minCapacity": 100
    },
    "filteredCount": 15,
    "totalCount": 87
  },
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalPages": 1,
    "totalItems": 15,
    "hasNext": false,
    "hasPrevious": false
  }
}
```

## Content Negotiation

### JSON Responses (Default)
Standard JSON formatting with consistent naming.

```csharp
// Standard JSON response
GET /api/venues/{id}
Accept: application/json

200 OK
Content-Type: application/json; charset=utf-8
{
  "id": "123e4567-e89b-12d3-a456-426614174001",
  "name": "Grand Theater",
  "address": {
    "street": "123 Broadway",
    "city": "New York",
    "state": "NY",
    "postalCode": "10001",
    "country": "USA"
  },
  "capacity": 1500,
  "type": {
    "id": "456e7890-e89b-12d3-a456-426614174002",
    "name": "Theater",
    "category": "Entertainment"
  },
  "description": "Historic theater in the heart of Manhattan",
  "isActive": true,
  "createdAt": "2024-12-06T10:00:00Z",
  "updatedAt": "2024-12-06T15:30:00Z"
}
```

### HAL+JSON Responses
Hypertext Application Language format for HATEOAS.

```csharp
// HAL+JSON response
GET /api/venues/{id}
Accept: application/hal+json

200 OK
Content-Type: application/hal+json
{
  "_links": {
    "self": { "href": "/api/venues/123e4567-e89b-12d3-a456-426614174001" },
    "acts": { "href": "/api/venues/123e4567-e89b-12d3-a456-426614174001/acts" },
    "bookings": { "href": "/api/venues/123e4567-e89b-12d3-a456-426614174001/bookings" },
    "edit": { "href": "/api/venues/123e4567-e89b-12d3-a456-426614174001", "type": "PUT" },
    "activate": { "href": "/api/venues/123e4567-e89b-12d3-a456-426614174001/activate", "type": "POST" }
  },
  "id": "123e4567-e89b-12d3-a456-426614174001",
  "name": "Grand Theater",
  "capacity": 1500,
  "isActive": true,
  "createdAt": "2024-12-06T10:00:00Z"
}
```

### XML Responses
XML format for legacy system integration.

```csharp
// XML response
GET /api/venues/{id}
Accept: application/xml

200 OK
Content-Type: application/xml; charset=utf-8
<?xml version="1.0" encoding="UTF-8"?>
<venue>
  <id>123e4567-e89b-12d3-a456-426614174001</id>
  <name>Grand Theater</name>
  <address>
    <street>123 Broadway</street>
    <city>New York</city>
    <state>NY</state>
    <postalCode>10001</postalCode>
    <country>USA</country>
  </address>
  <capacity>1500</capacity>
  <type>
    <id>456e7890-e89b-12d3-a456-426614174002</id>
    <name>Theater</name>
    <category>Entertainment</category>
  </type>
  <description>Historic theater in the heart of Manhattan</description>
  <isActive>true</isActive>
  <createdAt>2024-12-06T10:00:00Z</createdAt>
  <updatedAt>2024-12-06T15:30:00Z</updatedAt>
</venue>
```

### CSV Responses
CSV format for data export and reporting.

```csharp
// CSV response for collections
GET /api/venues
Accept: text/csv

200 OK
Content-Type: text/csv
Content-Disposition: attachment; filename="venues.csv"

Id,Name,City,State,Capacity,IsActive,CreatedAt
123e4567-e89b-12d3-a456-426614174001,"Grand Theater","New York","NY",1500,true,2024-12-06T10:00:00Z
456e7890-e89b-12d3-a456-426614174002,"Music Hall","Boston","MA",800,true,2024-12-05T14:30:00Z
```

## Date and Time Formatting

### ISO 8601 Standard
Consistent date/time format across all responses.

```csharp
// Standard ISO 8601 format with UTC timezone
{
  "createdAt": "2024-12-06T10:00:00Z",
  "updatedAt": "2024-12-06T15:30:00.123Z",
  "eventDate": "2024-12-25T19:30:00Z",
  "doorOpenTime": "2024-12-25T18:30:00Z"
}

// With timezone offset for local times
{
  "localEventTime": "2024-12-25T19:30:00-05:00",  // EST
  "utcEventTime": "2024-12-26T00:30:00Z"           // UTC equivalent
}

// Date only (for dates without time)
{
  "birthDate": "1985-07-15",
  "contractStartDate": "2024-01-01",
  "contractEndDate": "2024-12-31"
}
```

## Field Selection

### Sparse Fieldsets
Allow clients to request only needed fields for performance.

```csharp
// Request specific fields
GET /api/venues?fields=id,name,capacity

200 OK
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174001",
    "name": "Grand Theater",
    "capacity": 1500
  },
  {
    "id": "456e7890-e89b-12d3-a456-426614174002",
    "name": "Music Hall",
    "capacity": 800
  }
]

// Request nested fields
GET /api/venues?fields=id,name,address.city,address.state

200 OK
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174001",
    "name": "Grand Theater",
    "address": {
      "city": "New York",
      "state": "NY"
    }
  }
]
```

### Include Related Resources
Include related resources in a single request.

```csharp
// Include related resources
GET /api/venues?include=type,acts

200 OK
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174001",
    "name": "Grand Theater",
    "capacity": 1500,
    "type": {
      "id": "456e7890-e89b-12d3-a456-426614174002",
      "name": "Theater",
      "category": "Entertainment"
    },
    "acts": [
      {
        "id": "789e0123-e89b-12d3-a456-426614174003",
        "name": "Jazz Night",
        "genre": "Jazz"
      }
    ]
  }
]

// Include with field selection
GET /api/venues?include=acts&fields=id,name,acts.name

200 OK
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174001",
    "name": "Grand Theater",
    "acts": [
      { "name": "Jazz Night" },
      { "name": "Rock Concert" }
    ]
  }
]
```

## Request Body Formats

### Standard JSON Request
Most common request format for create and update operations.

```csharp
// Create venue request
POST /api/venues
Content-Type: application/json

{
  "name": "New Theater",
  "address": {
    "street": "456 Main St",
    "city": "Boston",
    "state": "MA",
    "postalCode": "02101",
    "country": "USA"
  },
  "capacity": 800,
  "type": {
    "id": "456e7890-e89b-12d3-a456-426614174002"
  },
  "description": "Modern theater with state-of-the-art sound system",
  "isActive": true
}
```

### JSON Patch Format
Structured partial updates using RFC 6902 JSON Patch.

```csharp
// JSON Patch request
PATCH /api/venues/{id}
Content-Type: application/json-patch+json

[
  {
    "op": "replace",
    "path": "/capacity",
    "value": 900
  },
  {
    "op": "replace",
    "path": "/description",
    "value": "Updated description with new capacity"
  },
  {
    "op": "add",
    "path": "/tags",
    "value": ["renovated", "expanded"]
  }
]
```

### Form Data Request
For file uploads and simple form submissions.

```csharp
// Multipart form data
POST /api/venues/{id}/images
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary

------WebKitFormBoundary
Content-Disposition: form-data; name="file"; filename="theater.jpg"
Content-Type: image/jpeg

[binary image data]
------WebKitFormBoundary
Content-Disposition: form-data; name="caption"

Main theater entrance
------WebKitFormBoundary
Content-Disposition: form-data; name="isPublic"

true
------WebKitFormBoundary--

// URL-encoded form data
POST /api/venues/search
Content-Type: application/x-www-form-urlencoded

name=theater&city=New+York&minCapacity=100&isActive=true
```

## Response Headers

### Standard Response Headers
Consistent headers for all API responses.

```csharp
// Successful response headers
200 OK
Content-Type: application/json; charset=utf-8
Content-Length: 1234
Cache-Control: public, max-age=300
ETag: "123456789abcdef"
Last-Modified: Thu, 06 Dec 2024 15:30:00 GMT
X-Request-ID: abc123def456
X-Response-Time: 125ms

// CORS headers
Access-Control-Allow-Origin: https://app.globoticket.com
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE
Access-Control-Allow-Headers: Authorization, Content-Type, X-Requested-With
Access-Control-Expose-Headers: X-Total-Count, X-Request-ID
Access-Control-Max-Age: 3600

// Security headers
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000; includeSubDomains
```

### Pagination Headers
Headers for collection responses.

```csharp
// Pagination information in headers
200 OK
X-Total-Count: 150
X-Page-Number: 2
X-Page-Size: 20
X-Total-Pages: 8
Link: </api/venues?page=1&pageSize=20>; rel="first",
      </api/venues?page=1&pageSize=20>; rel="prev",
      </api/venues?page=3&pageSize=20>; rel="next",
      </api/venues?page=8&pageSize=20>; rel="last"

// Rate limiting headers
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1701878400
```

These formats ensure consistent data structure and efficient client-server communication.