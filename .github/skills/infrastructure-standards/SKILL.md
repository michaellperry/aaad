---
name: infrastructure-standards
description: Infrastructure standards for Docker containerization, CI/CD scripting, security middleware, and authentication policies. Use when setting up deployments, configuring containers, writing build scripts, or implementing security middleware.
---

# Infrastructure Standards

Best practices for Docker, CI/CD scripts, security middleware, and authentication configuration for multi-tenant applications.

## Role Responsibilities

As a Platform Engineer, you are responsible for:
- **Docker**: Containerization of API, Web, and Database services
- **Scripts**: Maintenance of build, test, and deployment scripts in `scripts/`
- **Middleware**: Security and Multi-tenancy middleware in `src/GloboTicket.API/Middleware/`
- **Auth**: Configuration of Authentication and Authorization policies

## Docker Best Practices

### Docker Compose Configuration
**Always use specific versions for images and define comprehensive health checks.**

```yaml
version: '3.8'
services:
  api:
    image: globoticket-api:1.2.3  # Specific version, never 'latest'
    container_name: globoticket-api
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=GloboTicket;Uid=globoticket;Pwd=password;
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    depends_on:
      db:
        condition: service_healthy
    restart: unless-stopped
    
  web:
    image: globoticket-web:1.2.3
    container_name: globoticket-web
    ports:
      - "3000:3000"
    environment:
      - REACT_APP_API_URL=http://localhost:8080/api
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
    depends_on:
      - api
    restart: unless-stopped
    
  db:
    image: postgres:15.2-alpine
    container_name: globoticket-db
    environment:
      - POSTGRES_DB=GloboTicket
      - POSTGRES_USER=globoticket
      - POSTGRES_PASSWORD=password
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./docker/init-db:/docker-entrypoint-initdb.d
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U globoticket -d GloboTicket"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped

volumes:
  postgres_data:
```

### Environment Configuration
```yaml
# .env file structure
COMPOSE_PROJECT_NAME=globoticket

# Database
POSTGRES_VERSION=15.2-alpine
POSTGRES_DB=GloboTicket
POSTGRES_USER=globoticket
POSTGRES_PASSWORD=password

# API
API_VERSION=1.2.3
API_PORT=8080
ASPNETCORE_ENVIRONMENT=Development

# Web
WEB_VERSION=1.2.3
WEB_PORT=3000
REACT_APP_API_URL=http://localhost:8080/api
```

### Dockerfile Best Practices
```dockerfile
# Multi-stage build for .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/GloboTicket.API/GloboTicket.API.csproj", "GloboTicket.API/"]
COPY ["src/GloboTicket.Application/GloboTicket.Application.csproj", "GloboTicket.Application/"]
COPY ["src/GloboTicket.Domain/GloboTicket.Domain.csproj", "GloboTicket.Domain/"]
COPY ["src/GloboTicket.Infrastructure/GloboTicket.Infrastructure.csproj", "GloboTicket.Infrastructure/"]

RUN dotnet restore "GloboTicket.API/GloboTicket.API.csproj"

# Copy source code and build
COPY src/ .
RUN dotnet build "GloboTicket.API/GloboTicket.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "GloboTicket.API/GloboTicket.API.csproj" -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r globoticket && useradd -r -g globoticket globoticket
USER globoticket

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
ENTRYPOINT ["dotnet", "GloboTicket.API.dll"]
```

## Scripting Standards

### Cross-Platform Script Parity
**Ensure identical functionality between `scripts/bash/` and `scripts/powershell/`.**

#### Bash Script Example
```bash
#!/bin/bash
# scripts/bash/docker-up.sh

set -e  # Exit on error

# Check prerequisites
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed or not in PATH"
    exit 1
fi

if ! docker info &> /dev/null; then
    echo "❌ Docker daemon is not running"
    exit 1
fi

echo "🚀 Starting GloboTicket services..."

# Build and start services
docker-compose up -d --build

# Wait for health checks
echo "⏳ Waiting for services to be healthy..."
timeout=120
counter=0

while [ $counter -lt $timeout ]; do
    if docker-compose ps | grep -q "Up (healthy)"; then
        echo "✅ All services are healthy"
        docker-compose ps
        exit 0
    fi
    sleep 5
    counter=$((counter + 5))
    echo "   Waiting... (${counter}s/${timeout}s)"
done

echo "❌ Services failed to start within ${timeout}s"
docker-compose logs
exit 1
```

#### PowerShell Script Example
```powershell
# scripts/powershell/docker-up.ps1

param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# Check prerequisites
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

try {
    docker info | Out-Null
} catch {
    Write-Host "❌ Docker daemon is not running" -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Starting GloboTicket services..." -ForegroundColor Green

# Build and start services
$composeArgs = @("up", "-d")
if ($Force) {
    $composeArgs += "--build"
}

docker-compose @composeArgs

# Wait for health checks
Write-Host "⏳ Waiting for services to be healthy..." -ForegroundColor Yellow
$timeout = 120
$counter = 0

while ($counter -lt $timeout) {
    $status = docker-compose ps --format json | ConvertFrom-Json
    $healthyServices = $status | Where-Object { $_.State -eq "Up (healthy)" }
    
    if ($healthyServices.Count -eq $status.Count) {
        Write-Host "✅ All services are healthy" -ForegroundColor Green
        docker-compose ps
        exit 0
    }
    
    Start-Sleep 5
    $counter += 5
    Write-Host "   Waiting... (${counter}s/${timeout}s)" -ForegroundColor Yellow
}

Write-Host "❌ Services failed to start within ${timeout}s" -ForegroundColor Red
docker-compose logs
exit 1
```

### Idempotent Script Design
```bash
# Example: Database migration script
#!/bin/bash

DB_CONTAINER="globoticket-db"
DB_NAME="GloboTicket"

# Check if container exists and is running
if ! docker ps | grep -q $DB_CONTAINER; then
    echo "❌ Database container $DB_CONTAINER is not running"
    exit 1
fi

# Check current migration status
CURRENT_MIGRATION=$(docker exec $DB_CONTAINER psql -U globoticket -d $DB_NAME -t -c "SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId DESC LIMIT 1;" 2>/dev/null | xargs)

if [ -z "$CURRENT_MIGRATION" ]; then
    echo "📦 No migrations found, running initial migration..."
    dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
else
    echo "📦 Current migration: $CURRENT_MIGRATION"
    echo "📦 Checking for pending migrations..."
    
    # Run migrations (idempotent)
    dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
fi

echo "✅ Database migration completed"
```

## Security Middleware

### Tenant Resolution Middleware
**Critical security infrastructure for multi-tenant applications.**

```csharp
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(
        RequestDelegate next,
        ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip tenant resolution for health checks and authentication endpoints
        if (IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var tenantId = ExtractTenantId(context);
        
        if (tenantId == null)
        {
            _logger.LogWarning(
                "Authenticated user {Username} has no TenantId claim at {Path}",
                context.User.Identity?.Name,
                context.Request.Path);
                
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Tenant context required");
            return;
        }

        // Set tenant context
        context.Items["TenantId"] = tenantId;
        _logger.LogDebug("Tenant {TenantId} resolved for user {Username}", 
            tenantId, context.User.Identity?.Name);

        await _next(context);
    }

    private Guid? ExtractTenantId(HttpContext context)
    {
        // Try JWT claim first
        var tenantClaim = context.User.FindFirst("tenant_id");
        if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
        {
            return tenantId;
        }

        // Fallback to header (for system-to-system calls)
        var tenantHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (tenantHeader != null && Guid.TryParse(tenantHeader, out var headerTenantId))
        {
            return headerTenantId;
        }

        return null;
    }

    private static bool IsExcludedPath(PathString path)
    {
        var excludedPaths = new[]
        {
            "/health",
            "/auth",
            "/swagger",
            "/.well-known"
        };

        return excludedPaths.Any(excludedPath => 
            path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase));
    }
}
```

### Security Headers Middleware
```csharp
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Remove server information
        context.Response.Headers.Remove("Server");
        
        // Security headers
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Strict-Transport-Security", 
            "max-age=31536000; includeSubDomains");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Permissions-Policy", 
            "geolocation=(), microphone=(), camera=()");

        // Content Security Policy (adjust based on needs)
        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            context.Response.Headers.Add("Content-Security-Policy",
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' https:; connect-src 'self' https:");
        }

        await _next(context);
    }
}
```

## Authentication Configuration

### JWT Authentication Setup
```csharp
// Program.cs configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.NoResult();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = JsonSerializer.Serialize(new
                {
                    error = "Authentication failed",
                    message = context.Exception.Message
                });
                
                return context.Response.WriteAsync(result);
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = JsonSerializer.Serialize(new
                {
                    error = "Authentication required",
                    message = "Valid JWT token required"
                });
                
                return context.Response.WriteAsync(result);
            }
        };
    });
```

### Authorization Policies
```csharp
// Authorization policies configuration
builder.Services.AddAuthorization(options =>
{
    // Tenant member policy
    options.AddPolicy("TenantMember", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("tenant_id"));
              
    // Manager role policy
    options.AddPolicy("Manager", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("tenant_id")
              .RequireClaim("role", "manager", "admin"));
              
    // Admin only policy
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "admin"));
              
    // Custom requirement
    options.AddPolicy("CanManageVenues", policy =>
        policy.RequireAuthenticatedUser()
              .AddRequirements(new VenueManagementRequirement()));
});

// Custom authorization requirement
public class VenueManagementRequirement : IAuthorizationRequirement { }

public class VenueManagementHandler : AuthorizationHandler<VenueManagementRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        VenueManagementRequirement requirement)
    {
        var userRole = context.User.FindFirst("role")?.Value;
        var permissions = context.User.FindAll("permissions").Select(c => c.Value);
        
        if (userRole == "admin" || permissions.Contains("manage:venues"))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

### Middleware Registration Order
```csharp
// Critical: Middleware order in Program.cs
var app = builder.Build();

// Security headers first
app.UseMiddleware<SecurityHeadersMiddleware>();

// Exception handling
app.UseExceptionHandler("/error");

// HTTPS redirection
app.UseHttpsRedirection();

// Static files (before authentication)
app.UseStaticFiles();

// Routing
app.UseRouting();

// CORS (before auth)
app.UseCors();

// Authentication
app.UseAuthentication();

// Tenant resolution (after authentication)
app.UseMiddleware<TenantResolutionMiddleware>();

// Authorization (after tenant resolution)
app.UseAuthorization();

// Endpoints
app.MapControllers();
```

These infrastructure standards ensure secure, maintainable, and scalable deployment patterns for multi-tenant applications with proper security controls and operational practices.