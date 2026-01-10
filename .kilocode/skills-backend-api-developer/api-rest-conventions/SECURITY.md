# Security and Authentication

## Authentication Schemes
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

## Rate Limiting
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

