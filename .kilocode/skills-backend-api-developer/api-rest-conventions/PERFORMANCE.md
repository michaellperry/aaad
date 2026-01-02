# Performance Optimization

## Response Compression
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

## Caching Headers
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

## Pagination for Large Datasets
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

