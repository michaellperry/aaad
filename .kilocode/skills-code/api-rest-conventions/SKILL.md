---
name: api-rest-conventions
description: RESTful API design conventions and OpenAPI specifications for .NET applications
---

# RESTful API Design Conventions

This skill provides comprehensive guidance for designing RESTful APIs following industry best practices and conventions.

## URL Structure and Naming

### Resource-Based URLs
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

### Multi-Word Resource Naming
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

### Filtering and Querying
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

## HTTP Methods and Semantics

### Standard HTTP Methods
**Use HTTP methods according to their intended semantics.**

```csharp
// GET - Retrieve resources (safe, idempotent)
[HttpGet]
Task<ActionResult<IEnumerable<VenueDto>>> GetVenues([FromQuery] Guid tenantId);

[HttpGet("{id}")]
Task<ActionResult<VenueDto>> GetVenue(Guid id, Guid tenantId);

// POST - Create new resources (not idempotent)
[HttpPost]
Task<ActionResult<VenueDto>> CreateVenue([FromBody] CreateVenueRequest request);

// PUT - Replace entire resource (idempotent)
[HttpPut("{id}")]
Task<ActionResult<VenueDto>> UpdateVenue(Guid id, [FromBody] UpdateVenueRequest request);

// PATCH - Partial update (idempotent)
[HttpPatch("{id}")]
Task<ActionResult<VenueDto>> PartialUpdateVenue(Guid id, [FromBody] JsonPatchDocument<UpdateVenueRequest> request);

// DELETE - Remove resource (idempotent)
[HttpDelete("{id}")]
Task<IActionResult> DeleteVenue(Guid id);
```

### Idempotency Considerations
**Ensure PUT, DELETE, and GET are idempotent; use POST for non-idempotent operations.**

```csharp
// ✅ Good - Idempotent DELETE
DELETE /api/venues/{id}
// First call: 200 OK (deleted)
// Subsequent calls: 200 OK (already deleted) or 404 Not Found

// ✅ Good - Idempotent PUT
PUT /api/venues/{id}
// Updates the entire resource to the specified state
// Multiple calls with same data result in same state

// ✅ Good - Non-idempotent POST
POST /api/venues
// Each call creates a new resource with unique ID
// Multiple calls create multiple resources
```

## Status Codes and Error Handling

### Standard Status Codes
**Use appropriate HTTP status codes for different scenarios.**

```csharp
// Success codes
200 OK                  // Successful GET, PUT, PATCH
201 Created            // Successful POST (resource created)
204 No Content         // Successful DELETE, PUT (no response body)

// Client error codes
400 Bad Request        // Invalid request data
401 Unauthorized       // Authentication required
403 Forbidden          // No permission for resource
404 Not Found          // Resource doesn't exist
409 Conflict           // Resource conflict (e.g., duplicate name)
422 Unprocessable Entity // Validation errors

// Server error codes
500 Internal Server Error // Unhandled server errors
503 Service Unavailable   // Service temporarily unavailable
```

### Error Response Format
**Consistent error response structure for all endpoints.**

```csharp
// Standard error response
{
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "The request data is invalid",
        "details": [
            {
                "field": "Name",
                "message": "Venue name is required"
            },
            {
                "field": "Address",
                "message": "Address cannot exceed 500 characters"
            }
        ],
        "correlationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "timestamp": "2025-12-31T23:59:59.999Z"
    }
}

// Implementation in ASP.NET Core
public class ErrorResponse
{
    public ErrorInfo Error { get; set; } = new();
}

public class ErrorInfo
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail> Details { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
```

### Global Exception Handling
**Centralized exception handling for consistent error responses.**

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var errorResponse = new ErrorResponse
        {
            Error = new ErrorInfo
            {
                CorrelationId = httpContext.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            }
        };
        
        switch (exception)
        {
            case ValidationException validationEx:
                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                errorResponse.Error.Code = "VALIDATION_ERROR";
                errorResponse.Error.Message = "The request data is invalid";
                errorResponse.Error.Details = validationEx.Errors
                    .Select(e => new ErrorDetail { Field = e.PropertyName, Message = e.ErrorMessage })
                    .ToList();
                break;
                
            case UnauthorizedAccessException:
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                errorResponse.Error.Code = "UNAUTHORIZED";
                errorResponse.Error.Message = "Authentication is required";
                break;
                
            case NotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Error.Code = "NOT_FOUND";
                errorResponse.Error.Message = exception.Message;
                break;
                
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse.Error.Code = "INTERNAL_SERVER_ERROR";
                errorResponse.Error.Message = "An unexpected error occurred";
                
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }
        
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
        return true;
    }
}
```

## Data Transfer Objects (DTOs)

### Request DTOs
**Clear, focused request objects for creating and updating resources.**

```csharp
// Create request DTO
public class CreateVenueRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}

// Update request DTO
public class UpdateVenueRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    public bool? IsActive { get; set; }
}

// Controller with DTOs
[HttpPost]
public async Task<ActionResult<VenueDto>> CreateVenue(
    [FromBody] CreateVenueRequest request,
    [FromHeader("X-Tenant-Id")] Guid tenantId)
{
    var command = new CreateVenueCommand
    {
        Name = request.Name,
        Address = request.Address,
        IsActive = request.IsActive,
        TenantId = tenantId
    };
    
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetVenue), new { id = result.Id }, result);
}
```

### Response DTOs
**Consistent response DTOs with appropriate data and metadata.**

```csharp
// Standard response DTO
public class VenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Computed properties
    public string DisplayName => $"{Name} ({Address})";
}

// Response with metadata
public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

// Controller returning paged results
[HttpGet]
public async Task<ActionResult<PagedResponse<VenueDto>>> GetVenues(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromHeader("X-Tenant-Id")] Guid tenantId = default)
{
    var query = new GetVenuesQuery
    {
        Page = page,
        PageSize = pageSize,
        TenantId = tenantId
    };
    
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

## API Versioning

### URL Path Versioning
**Include version in URL path for clarity.**

```csharp
// ✅ Good - Version in URL path
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/venues")]

// ✅ Good - Explicit version routing
[HttpGet("api/v1.0/venues")]
[HttpGet("api/v2.0/venues")]

// ✅ Good - Default version
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});
```

### Backward Compatibility
**Design APIs to be backward compatible across versions.**

```csharp
// V1.0 - Simple venue data
public class VenueDtoV1
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

// V2.0 - Extended with additional fields
public class VenueDtoV2
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public List<string> Amenities { get; set; } = new();
}

// Controllers for different versions
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/venues")]
public class VenuesV1Controller : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueDtoV1>>> GetVenuesV1();
}

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/venues")]
public class VenuesV2Controller : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueDtoV2>>> GetVenivesV2();
}
```

## OpenAPI/Swagger Documentation

### Comprehensive API Documentation
**Use OpenAPI/Swagger for interactive API documentation.**

```csharp
// Configure OpenAPI
services.AddOpenApiDocument(options =>
{
    options.Title = "GloboTicket API";
    options.Version = "v1";
    options.Description = "Event management API for GloboTicket platform";
    
    // Include tenant ID in all operations
    options.OperationProcessors.Add(new TenantIdOperationProcessor());
});

// Operation documentation
[HttpPost]
[SwaggerOperation(
    Summary = "Create a new venue",
    Description = "Creates a new venue for event management",
    OperationId = "CreateVenue"
)]
[SwaggerResponse(201, "Venue created successfully", typeof(VenueDto))]
[SwaggerResponse(400, "Invalid request data")]
[SwaggerResponse(401, "Authentication required")]
public async Task<ActionResult<VenueDto>> CreateVenue(
    [FromBody] [SwaggerParameter("Venue creation data")] CreateVenueRequest request,
    [FromHeader] [SwaggerParameter("Tenant identifier")] Guid tenantId);

// Custom processors for multi-tenancy
public class TenantIdOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Tenant-Id",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" },
            Description = "Tenant identifier for multi-tenant isolation"
        });
        
        return true;
    }
}
```

## Security and Authentication

### Authentication Schemes
**Implement proper authentication and authorization.**

```csharp
// JWT Authentication
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

// Authorization policies
services.AddAuthorization(options =>
{
    options.AddPolicy("VenueOwner", policy =>
        policy.RequireClaim("tenant_id"));
        
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim("role", "admin"));
});

// Secure endpoints
[Authorize(Policy = "VenueOwner")]
[HttpGet("{id}")]
public async Task<ActionResult<VenueDto>> GetVenue(Guid id, Guid tenantId);

[Authorize(Policy = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteVenue(Guid id);
```

### Rate Limiting
**Implement rate limiting to protect API resources.**

```csharp
// Configure rate limiting
services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

// Apply rate limiting to controllers
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("default")]
public class VenuesController : ControllerBase
{
    [HttpGet]
    [DisableRateLimiting] // Allow higher rate for reads
    public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues();
    
    [HttpPost]
    [EnableRateLimiting("strict")] // Stricter rate limiting for writes
    public async Task<ActionResult<VenueDto>> CreateVenue();
}
```

## Performance Optimization

### Response Compression
**Enable compression for large responses.**

```csharp
// Enable response compression
services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

// Configure compression levels
services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});
```

### Caching Headers
**Implement appropriate caching strategies.**

```csharp
[HttpGet("{id}")]
[ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
public async Task<ActionResult<VenueDto>> GetVenue(Guid id, Guid tenantId);

// For frequently accessed, rarely changed data
[HttpGet]
[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
public async Task<ActionResult<IEnumerable<VenueDto>>> GetVenues([FromQuery] Guid tenantId);

// For user-specific data
[HttpGet("my-venues")]
[ResponseCache(Duration = 0, NoStore = true)] // No caching for user-specific data
public async Task<ActionResult<IEnumerable<VenueDto>>> GetMyVenues();
```

### Pagination for Large Datasets
**Always paginate large collections.**

```csharp
// Default pagination
[HttpGet]
public async Task<ActionResult<PagedResponse<VenueDto>>> GetVenues(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromHeader("X-Tenant-Id")] Guid tenantId = default)
{
    // Limit maximum page size to prevent abuse
    pageSize = Math.Min(pageSize, 100);
    
    var query = new GetVenuesQuery
    {
        Page = page,
        PageSize = pageSize,
        TenantId = tenantId
    };
    
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

## Testing and Validation

### Request Validation
**Validate all input data rigorously.**

```csharp
public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
{
    public CreateVenueRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueVenueName)
            .WithMessage("A venue with this name already exists");
            
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);
            
        RuleFor(x => x.IsActive)
            .NotNull();
    }
    
    private async Task<bool> BeUniqueVenueName(
        CreateVenueRequest request, 
        string name, 
        CancellationToken cancellationToken)
    {
        return !await _venueService.ExistsByNameAsync(name, request.TenantId);
    }
}

// Controller with validation
[HttpPost]
public async Task<ActionResult<VenueDto>> CreateVenue(
    [FromBody] CreateVenueRequest request,
    [FromHeader("X-Tenant-Id")] Guid tenantId,
    [FromServices] IValidator<CreateVenueRequest> validator)
{
    request.TenantId = tenantId;
    
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return BadRequest(validationResult.Errors.ToValidationResponse());
    }
    
    var command = _mapper.Map<CreateVenueCommand>(request);
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetVenue), new { id = result.Id }, result);
}
```

Following these RESTful API conventions ensures consistent, maintainable, and developer-friendly APIs that integrate well with modern web and mobile applications.