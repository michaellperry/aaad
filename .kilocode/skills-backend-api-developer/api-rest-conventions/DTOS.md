# Data Transfer Objects (DTOs)

## Request DTOs
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

## Response DTOs
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

