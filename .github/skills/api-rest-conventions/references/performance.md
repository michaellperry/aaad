# Performance & Caching

ETags, compression, pagination optimization, and performance best practices for RESTful APIs.

## HTTP Caching

### ETag Implementation
Entity tags for cache validation and optimistic concurrency control.

```csharp
// ETag generation and validation
public class ETagService
{
    public string GenerateETag(object entity)
    {
        var json = JsonSerializer.Serialize(entity);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash)[..16];  // Use first 16 characters
    }
    
    public string GenerateWeakETag(DateTime lastModified)
    {
        return $"W/\"{lastModified.Ticks:X}\"";
    }
}

// Controller with ETag support
[HttpGet("{id}")]
public async Task<IActionResult> GetVenue(Guid id)
{
    var venue = await _venueService.GetVenueAsync(id);
    if (venue == null)
        return NotFound();
        
    var etag = _etagService.GenerateETag(venue);
    
    // Check If-None-Match header
    var clientETag = Request.Headers.IfNoneMatch.FirstOrDefault();
    if (clientETag == etag)
    {
        return StatusCode(304); // Not Modified
    }
    
    Response.Headers.ETag = etag;
    Response.Headers.CacheControl = "public, max-age=300"; // 5 minutes
    
    return Ok(venue);
}

// Update with ETag validation
[HttpPut("{id}")]
public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] UpdateVenueDto dto)
{
    var venue = await _venueService.GetVenueAsync(id);
    if (venue == null)
        return NotFound();
        
    var currentETag = _etagService.GenerateETag(venue);
    var clientETag = Request.Headers.IfMatch.FirstOrDefault();
    
    // Check for concurrent modifications
    if (clientETag != null && clientETag != currentETag)
    {
        return Conflict(new
        {
            error = "Resource has been modified by another request",
            currentETag,
            providedETag = clientETag
        });
    }
    
    var updatedVenue = await _venueService.UpdateVenueAsync(id, dto);
    var newETag = _etagService.GenerateETag(updatedVenue);
    
    Response.Headers.ETag = newETag;
    return Ok(updatedVenue);
}
```

### Cache-Control Headers
Controlling client and proxy caching behavior.

```csharp
// Different caching strategies
[HttpGet]
public async Task<IActionResult> GetVenues()
{
    var venues = await _venueService.GetVenuesAsync();
    
    // Public cache for 10 minutes
    Response.Headers.CacheControl = "public, max-age=600";
    Response.Headers.Expires = DateTimeOffset.UtcNow.AddMinutes(10).ToString("R");
    
    return Ok(venues);
}

[HttpGet("categories")]
public async Task<IActionResult> GetVenueCategories()
{
    var categories = await _venueService.GetCategoriesAsync();
    
    // Long-term public cache for static data
    Response.Headers.CacheControl = "public, max-age=86400, immutable"; // 24 hours
    
    return Ok(categories);
}

[HttpGet("user/favorites")]
[Authorize]
public async Task<IActionResult> GetUserFavorites()
{
    var userId = User.GetUserId();
    var favorites = await _venueService.GetUserFavoritesAsync(userId);
    
    // Private cache only, short duration
    Response.Headers.CacheControl = "private, max-age=300, must-revalidate"; // 5 minutes
    
    return Ok(favorites);
}

[HttpPost]
public async Task<IActionResult> CreateVenue([FromBody] CreateVenueDto dto)
{
    var venue = await _venueService.CreateVenueAsync(dto);
    
    // No cache for mutable operations
    Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
    Response.Headers.Pragma = "no-cache";
    
    return CreatedAtAction(nameof(GetVenue), new { id = venue.Id }, venue);
}
```

### Last-Modified Headers
Time-based cache validation.

```csharp
// Last-Modified implementation
[HttpGet("{id}")]
public async Task<IActionResult> GetVenue(Guid id)
{
    var venue = await _venueService.GetVenueAsync(id);
    if (venue == null)
        return NotFound();
        
    var lastModified = venue.UpdatedAt ?? venue.CreatedAt;
    
    // Check If-Modified-Since header
    if (Request.Headers.IfModifiedSince.Any())
    {
        if (DateTimeOffset.TryParse(Request.Headers.IfModifiedSince.First(), out var ifModifiedSince))
        {
            if (lastModified <= ifModifiedSince)
            {
                return StatusCode(304); // Not Modified
            }
        }
    }
    
    Response.Headers.LastModified = lastModified.ToString("R");
    Response.Headers.CacheControl = "public, max-age=600";
    
    return Ok(venue);
}
```

## Response Compression

### Automatic Compression
Compressing responses to reduce bandwidth usage.

```csharp
// Response compression configuration
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "application/xml", "text/csv" }
    );
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

app.UseResponseCompression();

// Request/response headers
GET /api/venues
Accept-Encoding: gzip, deflate, br

200 OK
Content-Encoding: br
Content-Type: application/json; charset=utf-8
Original-Content-Length: 15420
Content-Length: 3892
Compression-Ratio: 25.2%
```

### Conditional Compression
Selective compression based on response size and type.

```csharp
// Custom compression middleware
public class ConditionalCompressionMiddleware
{
    private const int MinimumSizeForCompression = 1024; // 1KB
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        await next(context);
        
        // Check if compression is beneficial
        if (responseBody.Length >= MinimumSizeForCompression &&
            IsCompressibleContentType(context.Response.ContentType) &&
            AcceptsCompression(context.Request.Headers.AcceptEncoding))
        {
            // Compress and write
            await CompressAndWriteResponseAsync(context, responseBody, originalBodyStream);
        }
        else
        {
            // Write uncompressed
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
```

## Pagination Optimization

### Cursor-Based Pagination
Efficient pagination for large datasets.

```csharp
// Cursor-based pagination implementation
public record PagedResult<T>(
    IEnumerable<T> Items,
    string? NextCursor,
    string? PreviousCursor,
    bool HasMore
);

public class VenueService
{
    public async Task<PagedResult<VenueDto>> GetVenuesAsync(
        string? cursor = null,
        int limit = 20,
        string? sortField = "createdAt",
        string sortDirection = "asc")
    {
        var query = _context.Venues.AsQueryable();
        
        // Apply cursor filtering
        if (!string.IsNullOrEmpty(cursor))
        {
            var cursorData = DecodeCursor(cursor);
            if (sortField == "createdAt")
            {
                query = sortDirection == "asc" 
                    ? query.Where(v => v.CreatedAt > cursorData.CreatedAt)
                    : query.Where(v => v.CreatedAt < cursorData.CreatedAt);
            }
            else if (sortField == "name")
            {
                query = sortDirection == "asc"
                    ? query.Where(v => string.Compare(v.Name, cursorData.Name) > 0)
                    : query.Where(v => string.Compare(v.Name, cursorData.Name) < 0);
            }
        }
        
        // Apply sorting
        query = sortField switch
        {
            "name" => sortDirection == "asc" ? query.OrderBy(v => v.Name) : query.OrderByDescending(v => v.Name),
            "capacity" => sortDirection == "asc" ? query.OrderBy(v => v.Capacity) : query.OrderByDescending(v => v.Capacity),
            _ => sortDirection == "asc" ? query.OrderBy(v => v.CreatedAt) : query.OrderByDescending(v => v.CreatedAt)
        };
        
        // Fetch one extra item to check if there are more
        var venues = await query.Take(limit + 1).ToListAsync();
        
        var hasMore = venues.Count > limit;
        if (hasMore)
        {
            venues = venues.Take(limit).ToList();
        }
        
        var items = venues.Select(v => v.ToDto()).ToList();
        
        string? nextCursor = null;
        if (hasMore && items.Any())
        {
            var lastItem = venues.Last();
            nextCursor = EncodeCursor(new CursorData
            {
                Id = lastItem.Id,
                CreatedAt = lastItem.CreatedAt,
                Name = lastItem.Name
            });
        }
        
        return new PagedResult<VenueDto>(items, nextCursor, cursor, hasMore);
    }
    
    private string EncodeCursor(CursorData cursorData)
    {
        var json = JsonSerializer.Serialize(cursorData);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }
    
    private CursorData DecodeCursor(string cursor)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        return JsonSerializer.Deserialize<CursorData>(json)!;
    }
}

public record CursorData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Name { get; init; } = string.Empty;
}
```

### Optimized Offset Pagination
Improving offset-based pagination performance.

```csharp
// Optimized offset pagination
public async Task<PagedResponse<VenueDto>> GetVenuesOptimizedAsync(
    int page = 1,
    int pageSize = 20,
    string? filter = null)
{
    var offset = (page - 1) * pageSize;
    
    // Use separate count query with same filters
    var baseQuery = _context.Venues.AsQueryable();
    if (!string.IsNullOrEmpty(filter))
    {
        baseQuery = baseQuery.Where(v => v.Name.Contains(filter) || v.City.Contains(filter));
    }
    
    // Execute count and data queries in parallel
    var countTask = baseQuery.CountAsync();
    var dataTask = baseQuery
        .OrderBy(v => v.CreatedAt)
        .Skip(offset)
        .Take(pageSize)
        .Select(v => new VenueDto
        {
            Id = v.Id,
            Name = v.Name,
            City = v.City,
            Capacity = v.Capacity,
            IsActive = v.IsActive
        })
        .ToListAsync();
    
    await Task.WhenAll(countTask, dataTask);
    
    var totalCount = await countTask;
    var venues = await dataTask;
    
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    
    return new PagedResponse<VenueDto>(
        venues,
        new PaginationMeta
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalCount,
            HasNext = page < totalPages,
            HasPrevious = page > 1
        }
    );
}
```

## Database Query Optimization

### Projection and Field Selection
Reducing data transfer by selecting only needed fields.

```csharp
// Field selection implementation
[HttpGet]
public async Task<IActionResult> GetVenues([FromQuery] string? fields)
{
    var query = _context.Venues.AsQueryable();
    
    if (!string.IsNullOrEmpty(fields))
    {
        var fieldList = fields.Split(',').Select(f => f.Trim().ToLower()).ToList();
        
        // Use projection to select only requested fields
        if (fieldList.Contains("id") && fieldList.Contains("name") && fieldList.Count == 2)
        {
            var result = await query
                .Select(v => new { v.Id, v.Name })
                .ToListAsync();
            return Ok(result);
        }
        
        // More complex projection logic
        var projection = BuildProjectionExpression(fieldList);
        var venues = await query.Select(projection).ToListAsync();
        return Ok(venues);
    }
    
    // Return full objects if no field selection
    var allVenues = await query.ToListAsync();
    return Ok(allVenues);
}

// Include optimization
[HttpGet("{id}")]
public async Task<IActionResult> GetVenue(Guid id, [FromQuery] string? include)
{
    var query = _context.Venues.AsQueryable();
    
    if (!string.IsNullOrEmpty(include))
    {
        var includes = include.Split(',').Select(i => i.Trim()).ToList();
        
        foreach (var includeItem in includes)
        {
            query = includeItem.ToLower() switch
            {
                "acts" => query.Include(v => v.Acts),
                "address" => query.Include(v => v.Address),
                "type" => query.Include(v => v.Type),
                "acts.shows" => query.Include(v => v.Acts).ThenInclude(a => a.Shows),
                _ => query
            };
        }
    }
    
    var venue = await query.FirstOrDefaultAsync(v => v.Id == id);
    if (venue == null)
        return NotFound();
        
    return Ok(venue);
}
```

### Query Result Caching
Caching expensive database queries.

```csharp
// In-memory caching for frequently accessed data
public class CachedVenueService
{
    private readonly IMemoryCache _cache;
    private readonly VenueService _venueService;
    private readonly ILogger<CachedVenueService> _logger;
    
    public async Task<IEnumerable<VenueCategoryDto>> GetVenueCategoriesAsync()
    {
        const string cacheKey = "venue_categories";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<VenueCategoryDto>? categories))
        {
            _logger.LogDebug("Venue categories retrieved from cache");
            return categories!;
        }
        
        categories = await _venueService.GetVenueCategoriesAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4),
            SlidingExpiration = TimeSpan.FromMinutes(30),
            Priority = CacheItemPriority.High
        };
        
        _cache.Set(cacheKey, categories, cacheOptions);
        _logger.LogDebug("Venue categories cached for 4 hours");
        
        return categories;
    }
    
    public async Task<VenueDto?> GetVenueAsync(Guid id)
    {
        var cacheKey = $"venue_{id}";
        
        if (_cache.TryGetValue(cacheKey, out VenueDto? venue))
        {
            return venue;
        }
        
        venue = await _venueService.GetVenueAsync(id);
        if (venue != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                Priority = CacheItemPriority.Normal
            };
            
            _cache.Set(cacheKey, venue, cacheOptions);
        }
        
        return venue;
    }
    
    public async Task InvalidateVenueCacheAsync(Guid id)
    {
        _cache.Remove($"venue_{id}");
        _cache.Remove("venue_categories");
        _logger.LogDebug("Invalidated cache for venue {VenueId}", id);
    }
}
```

## Connection Pooling & Database Optimization

### Connection Pool Configuration
Optimizing database connection management.

```csharp
// Connection string with pooling settings
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GloboTicket;Trusted_Connection=true;MultipleActiveResultSets=true;Pooling=true;Min Pool Size=5;Max Pool Size=100;Connection Lifetime=300;Connection Timeout=30;Command Timeout=30"
  }
}

// DbContext configuration
builder.Services.AddDbContext<GloboTicketContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
        
        sqlOptions.CommandTimeout(30);
    });
    
    // Enable detailed errors only in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    
    // Configure query splitting for better performance
    options.UseSqlServer(o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});
```

### Async Best Practices
Proper async/await usage for better throughput.

```csharp
// ✅ Good - Proper async implementation
[HttpGet]
public async Task<IActionResult> GetVenues()
{
    var venues = await _venueService.GetVenuesAsync();
    return Ok(venues);
}

[HttpPost]
public async Task<IActionResult> CreateVenue([FromBody] CreateVenueDto dto)
{
    var venue = await _venueService.CreateVenueAsync(dto);
    return CreatedAtAction(nameof(GetVenue), new { id = venue.Id }, venue);
}

// ✅ Good - Parallel execution for independent operations
[HttpGet("{id}/summary")]
public async Task<IActionResult> GetVenueSummary(Guid id)
{
    var venueTask = _venueService.GetVenueAsync(id);
    var actsTask = _actService.GetActsByVenueAsync(id);
    var bookingsTask = _bookingService.GetUpcomingBookingsAsync(id);
    
    await Task.WhenAll(venueTask, actsTask, bookingsTask);
    
    var venue = await venueTask;
    if (venue == null)
        return NotFound();
        
    var acts = await actsTask;
    var bookings = await bookingsTask;
    
    var summary = new VenueSummaryDto
    {
        Venue = venue,
        TotalActs = acts.Count(),
        UpcomingBookings = bookings.Count()
    };
    
    return Ok(summary);
}

// ❌ Bad - Blocking async calls
[HttpGet]
public IActionResult GetVenuesBad()
{
    var venues = _venueService.GetVenuesAsync().Result; // Don't do this!
    return Ok(venues);
}

// ❌ Bad - Sequential execution of independent operations
[HttpGet("{id}/summary-bad")]
public async Task<IActionResult> GetVenueSummaryBad(Guid id)
{
    var venue = await _venueService.GetVenueAsync(id);
    var acts = await _actService.GetActsByVenueAsync(id);        // Sequential
    var bookings = await _bookingService.GetUpcomingBookingsAsync(id); // Sequential
    
    // ... rest of implementation
}
```

## API Response Optimization

### Response Size Reduction
Minimizing payload size for better performance.

```csharp
// Lightweight DTOs for list endpoints
public record VenueListItemDto(
    Guid Id,
    string Name,
    string City,
    int Capacity,
    bool IsActive
);

public record VenueDetailDto(
    Guid Id,
    string Name,
    AddressDto Address,
    int Capacity,
    VenueTypeDto Type,
    string? Description,
    ContactInfoDto? ContactInfo,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<ActSummaryDto> RecentActs
);

// Use appropriate DTO for endpoint
[HttpGet]
public async Task<IActionResult> GetVenues()
{
    // Return lightweight DTOs for list
    var venues = await _venueService.GetVenueListAsync();
    return Ok(venues); // VenueListItemDto[]
}

[HttpGet("{id}")]
public async Task<IActionResult> GetVenue(Guid id)
{
    // Return detailed DTO for single item
    var venue = await _venueService.GetVenueDetailAsync(id);
    return Ok(venue); // VenueDetailDto
}
```

### JSON Serialization Optimization
Optimizing JSON serialization for performance.

```csharp
// System.Text.Json configuration
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.WriteIndented = false; // Minimize size in production
});

// Custom JSON context for AOT compilation
[JsonSerializable(typeof(VenueDto))]
[JsonSerializable(typeof(VenueDto[]))]
[JsonSerializable(typeof(PagedResponse<VenueDto>))]
[JsonSerializable(typeof(ErrorResponse))]
public partial class ApiJsonContext : JsonSerializerContext
{
}

// Use custom context
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApiJsonContext.Default);
});
```

These performance optimizations ensure efficient resource utilization and improved user experience.