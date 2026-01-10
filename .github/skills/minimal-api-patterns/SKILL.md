---
name: minimal-api-patterns
description: ASP.NET Core Minimal APIs patterns for endpoints, DTOs, validation, and service integration. Use when implementing API endpoints, defining request/response contracts, or structuring API projects with clean separation of concerns.
---

# ASP.NET Core Minimal API Patterns

Best practices for defining Endpoints, DTOs, and Validators using ASP.NET Core Minimal APIs with clean separation of concerns.

## Role Responsibilities

As an API Contract Guardian, you are responsible for:
- **Contracts**: Defining clear Request/Response DTOs in `GloboTicket.Application/DTOs`
- **Endpoints**: Implementing `MapGroup` definitions in `GloboTicket.API/Endpoints/`
- **Services**: Implementing Application Services in `GloboTicket.Application/Services/` that orchestrate Domain Entities
- **Validation**: Ensuring all inputs are validated before processing

## Endpoint Structure

### Grouping Pattern
**Use `MapGroup` to organize related endpoints and apply common middleware.**

```csharp
// Venue endpoints grouping
public static WebApplication MapVenueEndpoints(this WebApplication app)
{
    var venues = app.MapGroup("/api/venues")
        .RequireAuthorization()
        .WithTags("Venues")
        .WithOpenApi();

    venues.MapGet("/", GetAllVenues)
        .WithName("GetVenues")
        .WithSummary("Get all venues for the authenticated tenant");
        
    venues.MapGet("/{id:guid}", GetVenueById)
        .WithName("GetVenueById")
        .WithSummary("Get venue by ID");
        
    venues.MapPost("/", CreateVenue)
        .WithName("CreateVenue")
        .WithSummary("Create a new venue");
        
    venues.MapPut("/{id:guid}", UpdateVenue)
        .WithName("UpdateVenue")
        .WithSummary("Update an existing venue");
        
    venues.MapDelete("/{id:guid}", DeleteVenue)
        .WithName("DeleteVenue")
        .WithSummary("Delete a venue");

    return app;
}
```

### Endpoint Delegate Patterns

```csharp
// GET endpoint with query parameters
static async Task<IResult> GetAllVenues(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? search = null,
    IVenueService venueService = null!)
{
    var result = await venueService.GetVenuesAsync(page, pageSize, search);
    return Results.Ok(result);
}

// POST endpoint with DTO validation
static async Task<IResult> CreateVenue(
    CreateVenueDto dto,
    IVenueService venueService,
    IValidator<CreateVenueDto> validator)
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var result = await venueService.CreateAsync(dto);
    return Results.Created($"/api/venues/{result.Id}", result);
}

// PUT endpoint with route parameter
static async Task<IResult> UpdateVenue(
    Guid id,
    UpdateVenueDto dto,
    IVenueService venueService,
    IValidator<UpdateVenueDto> validator)
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var result = await venueService.UpdateAsync(id, dto);
    return result != null ? Results.Ok(result) : Results.NotFound();
}
```

## DTO Pattern

### Separation of Concerns
**NEVER return Domain Entities directly from an endpoint. Always map to specific DTOs.**

```csharp
// ✅ Good - Dedicated DTOs for different operations
public record VenueDto(
    Guid Id,
    string Name,
    string Address,
    int Capacity,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateVenueDto(
    string Name,
    string Address,
    int Capacity,
    VenueTypeDto VenueType
);

public record UpdateVenueDto(
    string Name,
    string Address,
    int Capacity,
    bool IsActive
);

// ✅ Good - Nested DTOs for complex objects
public record VenueTypeDto(
    Guid Id,
    string Name,
    string Category
);

// ❌ Bad - Returning domain entities directly
public class Venue : Entity { ... } // Do not return this from endpoints!
```

### DTO Validation

```csharp
// FluentValidation validators for DTOs
public class CreateVenueDtoValidator : AbstractValidator<CreateVenueDto>
{
    public CreateVenueDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
            
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
            
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0")
            .LessThanOrEqualTo(100000).WithMessage("Capacity cannot exceed 100,000");
            
        RuleFor(x => x.VenueType)
            .NotNull().WithMessage("Venue type is required");
    }
}
```

## Service Layer Integration

### Dependency Injection
**Inject Application Services into endpoint delegates, not repositories directly.**

```csharp
// ✅ Good - Service injection
venues.MapPost("/", async (
    CreateVenueDto dto, 
    IVenueService service,
    IValidator<CreateVenueDto> validator) => 
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
    
    var result = await service.CreateAsync(dto);
    return Results.Created($"/api/venues/{result.Id}", result);
});

// ❌ Bad - Repository injection (bypass service layer)
venues.MapPost("/", async (CreateVenueDto dto, IVenueRepository repo) => { ... });
```

### Service Implementation Pattern

```csharp
// Application service implementation in GloboTicket.Application/Services/
public class VenueService
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    
    public VenueService(
        DbContext dbContext,
        ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }
    
    public async Task<VenueDto> CreateAsync(CreateVenueDto dto, CancellationToken cancellationToken)
    {
        if (!_tenantContext.CurrentTenantId.HasValue)
            throw new InvalidOperationException("Tenant context is required");
        
        // Business logic and domain entity creation
        var venue = new Venue
        {
            VenueGuid = dto.VenueGuid,
            Name = dto.Name,
            Address = dto.Address,
            SeatingCapacity = dto.SeatingCapacity,
            Description = dto.Description,
            Location = GeographyService.CreatePoint(dto.Latitude, dto.Longitude)
        };
        
        _dbContext.Set<Venue>().Add(venue);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return MapToDto(venue);
    }
    
    private static VenueDto MapToDto(Venue venue) => new VenueDto
    {
        Id = venue.Id,
        VenueGuid = venue.VenueGuid,
        Name = venue.Name,
        Address = venue.Address,
        SeatingCapacity = venue.SeatingCapacity,
        Description = venue.Description
    };
}
```

## Error Handling

### Consistent Error Responses

```csharp
// Global exception handler
public static IResult HandleException(Exception ex)
{
    return ex switch
    {
        ValidationException validationEx => Results.ValidationProblem(
            validationEx.Errors.ToDictionary(
                error => error.PropertyName,
                error => new[] { error.ErrorMessage }
            )),
        NotFoundException => Results.NotFound(),
        UnauthorizedAccessException => Results.Forbid(),
        _ => Results.Problem(
            title: "An error occurred",
            detail: ex.Message,
            statusCode: 500
        )
    };
}

// Apply error handling to endpoint groups
venues.MapPost("/", CreateVenue)
    .AddEndpointFilter<ValidationFilter>()
    .AddEndpointFilter<ExceptionFilter>();
```

## Authentication and Authorization

### Endpoint Security

```csharp
// Group-level authorization
var venues = app.MapGroup("/api/venues")
    .RequireAuthorization() // All endpoints require authentication
    .WithTags("Venues");

// Specific authorization policies
venues.MapDelete("/{id:guid}", DeleteVenue)
    .RequireAuthorization("AdminOnly"); // Specific policy

// Role-based authorization
venues.MapPost("/", CreateVenue)
    .RequireAuthorization(policy => policy.RequireRole("Manager", "Admin"));

// Custom authorization requirements
venues.MapPut("/{id:guid}/approve", ApproveVenue)
    .RequireAuthorization("CanApproveVenues");
```

## OpenAPI Integration

### Documentation Attributes

```csharp
venues.MapGet("/", GetAllVenues)
    .WithName("GetVenues")
    .WithSummary("Get all venues")
    .WithDescription("Retrieves a paginated list of venues for the authenticated tenant")
    .Produces<PagedResult<VenueListDto>>(200)
    .Produces<ProblemDetails>(400)
    .Produces(401)
    .WithTags("Venues");

venues.MapPost("/", CreateVenue)
    .WithName("CreateVenue")
    .WithSummary("Create a new venue")
    .WithDescription("Creates a new venue with the provided details")
    .Accepts<CreateVenueDto>("application/json")
    .Produces<VenueDto>(201)
    .ProducesValidationProblem()
    .Produces<ProblemDetails>(409)
    .WithTags("Venues");
```

## Performance Considerations

### Response Caching

```csharp
// Cache GET endpoints
venues.MapGet("/types", GetVenueTypes)
    .CacheOutput(policy => policy
        .Expire(TimeSpan.FromHours(1))
        .Tag("venue-types"));

// Cache with conditional logic
venues.MapGet("/{id:guid}", GetVenueById)
    .CacheOutput(policy => policy
        .Expire(TimeSpan.FromMinutes(15))
        .SetVaryByRouteValue("id"));
```

### Async Patterns

```csharp
// Always use async/await for I/O operations
static async Task<IResult> GetVenue(Guid id, IVenueService service)
{
    var venue = await service.GetByIdAsync(id);
    return venue != null ? Results.Ok(venue) : Results.NotFound();
}

// Use CancellationToken for long-running operations
static async Task<IResult> GetAllVenues(
    IVenueService service,
    CancellationToken cancellationToken)
{
    var venues = await service.GetAllAsync(cancellationToken);
    return Results.Ok(venues);
}
```

These patterns ensure clean, maintainable, and performant ASP.NET Core Minimal APIs with proper separation of concerns and consistent error handling.