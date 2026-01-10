# Documentation

OpenAPI/Swagger specifications and examples for RESTful APIs.

## OpenAPI Specification

### Basic Setup
Configuring Swagger/OpenAPI documentation.

```csharp
// Program.cs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GloboTicket API",
        Version = "v1",
        Description = "RESTful API for venue and event management",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "api-support@globoticket.com",
            Url = new Uri("https://support.globoticket.com")
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GloboTicket API v1");
        options.RoutePrefix = "api-docs";
    });
}
```

### Controller Documentation
Documenting API endpoints with XML comments and attributes.

```csharp
/// <summary>
/// Manages venue operations including creation, retrieval, and updates
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VenuesController : ControllerBase
{
    /// <summary>
    /// Get all venues with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="city">Filter by city name</param>
    /// <returns>List of venues</returns>
    /// <response code="200">Returns the list of venues</response>
    /// <response code="400">Invalid query parameters</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<VenueDto>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    public async Task<IActionResult> GetVenues(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? city = null)
    {
        // Implementation
    }

    /// <summary>
    /// Get a specific venue by ID
    /// </summary>
    /// <param name="id">Venue unique identifier</param>
    /// <returns>Venue details</returns>
    /// <response code="200">Returns the venue</response>
    /// <response code="404">Venue not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VenueDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetVenue(Guid id)
    {
        // Implementation
    }
}
```