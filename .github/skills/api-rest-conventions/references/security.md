# Authentication & Security

JWT, API keys, CORS, rate limiting, and security best practices for RESTful APIs.

## JWT Authentication

### Bearer Token Implementation
Standard JWT Bearer token authentication pattern.

```csharp
// Request with JWT token
GET /api/venues
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

// JWT payload structure
{
  "sub": "123e4567-e89b-12d3-a456-426614174001",
  "name": "John Doe",
  "email": "john.doe@example.com",
  "role": "admin",
  "tenant": "tenant-abc123",
  "permissions": ["venues:read", "venues:write", "venues:delete"],
  "iat": 1701864000,
  "exp": 1701950400,
  "aud": "globoticket-api",
  "iss": "globoticket-auth"
}

// ASP.NET Core JWT configuration
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "globoticket-auth",
            ValidAudience = "globoticket-api",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
```

### Token Refresh Pattern
Secure token refresh implementation.

```csharp
// Token refresh request
POST /auth/refresh
Content-Type: application/json
{
  "refreshToken": "refresh_token_here",
  "accessToken": "current_access_token_here"
}

// Successful refresh response
200 OK
{
  "accessToken": "new_access_token_here",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "refreshToken": "new_refresh_token_here",
  "scope": "api:read api:write"
}

// Invalid refresh token
401 Unauthorized
{
  "error": "invalid_grant",
  "error_description": "The provided refresh token is invalid or expired",
  "loginUrl": "/auth/login"
}
```

## API Key Authentication

### Header-Based API Keys
API key authentication patterns.

```csharp
// API key in custom header (preferred)
GET /api/venues
X-API-Key: your-api-key-here
X-API-Secret: your-api-secret-here  // For HMAC signing

// API key in Authorization header
GET /api/venues
Authorization: ApiKey your-api-key-here

// HMAC-signed request
GET /api/venues
X-API-Key: your-api-key-here
X-Timestamp: 1701864000
X-Signature: computed-hmac-signature

// API key validation response
401 Unauthorized
{
  "error": {
    "code": "INVALID_API_KEY",
    "message": "The provided API key is invalid or has been revoked",
    "details": {
      "keyId": "key_abc123",
      "status": "revoked",
      "revokedAt": "2024-12-01T10:00:00Z",
      "contactSupport": "Contact support to reactivate your API key"
    }
  }
}
```

### Query Parameter API Keys
Less secure but sometimes necessary for simple integrations.

```csharp
// API key in query parameter (less secure)
GET /api/venues?api_key=your-api-key-here&api_secret=your-secret-here

// Rate limiting for query-based keys
429 Too Many Requests
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1701867600
{
  "error": {
    "code": "API_KEY_RATE_LIMIT_EXCEEDED",
    "message": "API key rate limit exceeded",
    "details": {
      "keyId": "key_abc123",
      "limit": 100,
      "period": "1 hour",
      "upgradeUrl": "/billing/upgrade"
    }
  }
}
```

## Authorization Patterns

### Role-Based Access Control (RBAC)
Implementing role-based authorization.

```csharp
// Controller with role authorization
[Authorize(Roles = "admin,manager")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteVenue(Guid id)
{
    // Only admins and managers can delete venues
}

[Authorize(Roles = "admin")]
[HttpPost("bulk-import")]
public async Task<IActionResult> BulkImportVenues([FromBody] ImportRequest request)
{
    // Only admins can perform bulk operations
}

// Custom authorization attribute
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePermissionAttribute : Attribute, IAuthorizationRequirement
{
    public string Permission { get; }
    
    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }
}

[RequirePermission("venues:delete")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteVenue(Guid id)
{
    // Requires specific permission
}
```

### Resource-Based Authorization
Authorization based on resource ownership or context.

```csharp
// Tenant-based authorization
[HttpGet]
public async Task<IActionResult> GetVenues()
{
    var tenantId = User.GetTenantId();
    var venues = await _venueService.GetVenuesByTenantAsync(tenantId);
    return Ok(venues);
}

// Resource ownership authorization
[HttpPut("{id}")]
public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] UpdateVenueDto dto)
{
    var venue = await _venueService.GetVenueAsync(id);
    if (venue == null)
        return NotFound();
        
    var currentUserId = User.GetUserId();
    var userTenantId = User.GetTenantId();
    
    // Check tenant access
    if (venue.TenantId != userTenantId)
        return Forbid("You do not have access to this venue");
        
    // Check ownership or admin role
    if (venue.CreatedBy != currentUserId && !User.IsInRole("admin"))
        return Forbid("You can only edit venues you created");
        
    // Update venue
    var updatedVenue = await _venueService.UpdateVenueAsync(id, dto);
    return Ok(updatedVenue);
}
```

## CORS Configuration

### CORS Headers
Proper CORS configuration for cross-origin requests.

```csharp
// CORS policy configuration
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("GloboTicketPolicy", policy =>
    {
        policy.WithOrigins(
                "https://app.globoticket.com",
                "https://admin.globoticket.com",
                "https://staging.globoticket.com"
            )
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .WithHeaders(
                "Authorization",
                "Content-Type",
                "X-Requested-With",
                "X-API-Key",
                "X-Tenant-Id"
            )
            .WithExposedHeaders(
                "X-Total-Count",
                "X-Request-ID",
                "X-RateLimit-Remaining"
            )
            .SetPreflightMaxAge(TimeSpan.FromMinutes(60));
    });
});

// Response headers
Access-Control-Allow-Origin: https://app.globoticket.com
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE, OPTIONS
Access-Control-Allow-Headers: Authorization, Content-Type, X-Requested-With, X-API-Key
Access-Control-Expose-Headers: X-Total-Count, X-Request-ID
Access-Control-Max-Age: 3600
Access-Control-Allow-Credentials: true
```

### Preflight Request Handling
Handling OPTIONS requests for complex CORS scenarios.

```csharp
// Preflight request
OPTIONS /api/venues
Origin: https://app.globoticket.com
Access-Control-Request-Method: POST
Access-Control-Request-Headers: Authorization, Content-Type

// Preflight response
200 OK
Access-Control-Allow-Origin: https://app.globoticket.com
Access-Control-Allow-Methods: GET, POST, PUT, PATCH, DELETE
Access-Control-Allow-Headers: Authorization, Content-Type, X-Requested-With
Access-Control-Max-Age: 3600
Content-Length: 0
```

## Rate Limiting

### Request Rate Limiting
Protecting API from abuse and ensuring fair usage.

```csharp
// Rate limiting configuration
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 1000,
                Window = TimeSpan.FromHours(1)
            }
        )
    );
    
    options.AddFixedWindowLimiter("api-strict", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    });
});

// Rate limit headers
200 OK
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1701867600
X-RateLimit-Used: 1

// Rate limit exceeded
429 Too Many Requests
Retry-After: 3600
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 0
X-RateLimit-Reset: 1701867600
```

### Tiered Rate Limiting
Different limits based on authentication and subscription tiers.

```csharp
// Tiered rate limiting
[EnableRateLimiting("premium-tier")]  // 5000 req/hour
[HttpGet]
public async Task<IActionResult> GetVenuesPremium()
{
    // Premium tier endpoint
}

[EnableRateLimiting("standard-tier")]  // 1000 req/hour
[HttpGet]
public async Task<IActionResult> GetVenuesStandard()
{
    // Standard tier endpoint
}

[EnableRateLimiting("free-tier")]  // 100 req/hour
[HttpGet]
public async Task<IActionResult> GetVenuesFree()
{
    // Free tier endpoint
}
```

## Security Headers

### Essential Security Headers
Protection against common web vulnerabilities.

```csharp
// Security headers middleware
public class SecurityHeadersMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Prevent content type sniffing
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        
        // Prevent clickjacking
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        
        // XSS protection (legacy browsers)
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        
        // Force HTTPS
        context.Response.Headers.Add("Strict-Transport-Security", 
            "max-age=31536000; includeSubDomains; preload");
        
        // Content Security Policy
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'");
        
        // Referrer policy
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        
        // Feature policy
        context.Response.Headers.Add("Permissions-Policy", 
            "camera=(), microphone=(), geolocation=()");
        
        await next(context);
    }
}
```

## Input Validation & Sanitization

### SQL Injection Prevention
Parameterized queries and input validation.

```csharp
// ✅ Good - Parameterized query
public async Task<IEnumerable<Venue>> GetVenuesByNameAsync(string name)
{
    const string sql = "SELECT * FROM Venues WHERE Name = @Name";
    return await _connection.QueryAsync<Venue>(sql, new { Name = name });
}

// ✅ Good - Entity Framework with LINQ
public async Task<IEnumerable<Venue>> GetVenuesByCityAsync(string city)
{
    return await _context.Venues
        .Where(v => v.Address.City == city)
        .ToListAsync();
}

// ❌ Bad - String concatenation (vulnerable)
public async Task<IEnumerable<Venue>> GetVenuesByNameUnsafe(string name)
{
    string sql = $"SELECT * FROM Venues WHERE Name = '{name}'";
    return await _connection.QueryAsync<Venue>(sql);
}
```

### Input Sanitization
Sanitizing user input to prevent XSS and other attacks.

```csharp
// Input validation attributes
public record CreateVenueDto
(
    [Required]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9\s\-']+$", ErrorMessage = "Invalid characters in venue name")]
    string Name,
    
    [Required]
    [EmailAddress]
    string ContactEmail,
    
    [Phone]
    string? Phone,
    
    [Range(1, 100000)]
    int Capacity,
    
    [StringLength(2000)]
    [AllowedHtml]  // Custom attribute for safe HTML
    string? Description
);

// Custom HTML sanitization
public class AllowedHtmlAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string html || string.IsNullOrEmpty(html))
            return true;
            
        // Use HTML sanitization library like HtmlSanitizer
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("p");
        sanitizer.AllowedTags.Add("br");
        sanitizer.AllowedTags.Add("strong");
        sanitizer.AllowedTags.Add("em");
        
        var sanitized = sanitizer.Sanitize(html);
        return sanitized == html;
    }
}
```

## API Security Monitoring

### Security Event Logging
Logging security events for monitoring and alerting.

```csharp
// Security event logging
public class SecurityEventLogger
{
    public async Task LogAuthenticationFailureAsync(string ipAddress, string userAgent, string reason)
    {
        var securityEvent = new SecurityEvent
        {
            EventType = "AUTHENTICATION_FAILURE",
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Reason = reason,
            Timestamp = DateTimeOffset.UtcNow,
            Severity = "WARNING"
        };
        
        await _eventStore.SaveAsync(securityEvent);
        
        // Alert on multiple failures from same IP
        var recentFailures = await GetRecentFailuresByIpAsync(ipAddress);
        if (recentFailures.Count >= 5)
        {
            await _alertService.SendSecurityAlertAsync(
                $"Multiple authentication failures from IP {ipAddress}");
        }
    }
    
    public async Task LogSuspiciousActivityAsync(string userId, string activity, object details)
    {
        var securityEvent = new SecurityEvent
        {
            EventType = "SUSPICIOUS_ACTIVITY",
            UserId = userId,
            Activity = activity,
            Details = JsonSerializer.Serialize(details),
            Timestamp = DateTimeOffset.UtcNow,
            Severity = "HIGH"
        };
        
        await _eventStore.SaveAsync(securityEvent);
        await _alertService.SendSecurityAlertAsync(
            $"Suspicious activity detected for user {userId}: {activity}");
    }
}
```

### Anomaly Detection
Detecting unusual patterns that might indicate attacks.

```csharp
// Request pattern analysis
public class AnomalyDetectionService
{
    public async Task<bool> IsAnomalousRequestAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var endpoint = context.Request.Path;
        
        // Check request frequency
        var requestCount = await GetRequestCountAsync(ipAddress, TimeSpan.FromMinutes(1));
        if (requestCount > 100)
        {
            await LogAnomalyAsync("HIGH_REQUEST_FREQUENCY", ipAddress, new { RequestCount = requestCount });
            return true;
        }
        
        // Check for suspicious user agents
        if (IsSuspiciousUserAgent(userAgent))
        {
            await LogAnomalyAsync("SUSPICIOUS_USER_AGENT", ipAddress, new { UserAgent = userAgent });
            return true;
        }
        
        // Check for path traversal attempts
        if (endpoint.Value?.Contains("..") == true)
        {
            await LogAnomalyAsync("PATH_TRAVERSAL_ATTEMPT", ipAddress, new { Endpoint = endpoint });
            return true;
        }
        
        return false;
    }
}
```

## Secure Communication

### HTTPS Configuration
Forcing HTTPS and secure communication.

```csharp
// HTTPS redirection
app.UseHttpsRedirection();
app.UseHsts();  // HTTP Strict Transport Security

// HSTS configuration
app.UseHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubdomains = true;
    options.Preload = true;
});

// Certificate configuration
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });
});
```

### Certificate Pinning
Validating specific certificates to prevent man-in-the-middle attacks.

```csharp
// Certificate pinning for external API calls
public class PinnedCertificateHandler : HttpClientHandler
{
    private readonly string[] _expectedThumbprints;
    
    public PinnedCertificateHandler(params string[] expectedThumbprints)
    {
        _expectedThumbprints = expectedThumbprints;
    }
    
    protected override bool CheckCertificateRevocationList => true;
    
    public override bool ServerCertificateCustomValidationCallback(
        HttpRequestMessage message,
        X509Certificate2? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors != SslPolicyErrors.None)
            return false;
            
        if (certificate == null)
            return false;
            
        var thumbprint = certificate.Thumbprint;
        return _expectedThumbprints.Contains(thumbprint, StringComparer.OrdinalIgnoreCase);
    }
}

// Usage
services.AddHttpClient<WeatherApiClient>()
    .ConfigurePrimaryHttpMessageHandler(() => new PinnedCertificateHandler(
        "AA:BB:CC:DD:EE:FF:11:22:33:44:55:66:77:88:99:AA:BB:CC:DD:EE"
    ));
```

This comprehensive security implementation protects against common vulnerabilities and ensures secure API operation.