# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GloboTicket is a multi-tenant event ticketing platform built with .NET 10 and React 18. The project demonstrates Clean Architecture principles with strong emphasis on multi-tenancy, data isolation, security, and test-driven development.

**Key Architectural Concepts:**
- **Environments vs Tenants**: Environments are separate deployments (Dev/Staging/Production). Tenants provide row-level data isolation within an environment's database, enabling scenarios like running smoke tests in production without affecting production data.
- **Clean Architecture**: Clear separation between Domain, Application, Infrastructure, and Presentation layers
- **Multi-Tenant by Design**: Row-level isolation using `TenantId` with automatic query filtering

## Build & Run Commands

### Infrastructure Setup

```bash
# Start SQL Server (includes automatic user initialization)
./scripts/bash/docker-up.sh

# Check Docker status
./scripts/bash/docker-status.sh

# Stop infrastructure
./scripts/bash/docker-down.sh

# View SQL Server logs
./scripts/bash/docker-logs.sh
```

### Database Operations

```bash
# Apply all pending migrations (includes dotnet restore)
./scripts/bash/db-update.sh

# Add a new migration
./scripts/bash/db-migrate-add.sh <MigrationName>

# List all migrations
./scripts/bash/db-migrate-list.sh

# Remove last migration
./scripts/bash/db-migrate-remove.sh

# Rollback to specific migration
./scripts/bash/db-migrate-rollback.sh <MigrationName>

# Reset database (drop and recreate)
./scripts/bash/db-reset.sh

# Reset test database
./scripts/bash/docker-test-reset.sh
```

**Migration Context:**
- **Project**: Migrations are in `src/GloboTicket.Infrastructure`
- **Startup Project**: Always use `src/GloboTicket.API` as startup project
- **Output Directory**: `Data/Migrations` (within Infrastructure project)
- **Migration User**: Uses `migration_user` with elevated privileges
- **Manual Command**:
  ```bash
  dotnet ef migrations add <Name> \
    --project src/GloboTicket.Infrastructure \
    --startup-project src/GloboTicket.API \
    --output-dir Data/Migrations
  ```

### Testing

```bash
# Run all tests
./scripts/bash/test.sh

# Run unit tests only
./scripts/bash/test-unit.sh

# Run integration tests only
./scripts/bash/test-integration.sh

# Run specific test project
dotnet test tests/GloboTicket.UnitTests --verbosity normal
dotnet test tests/GloboTicket.IntegrationTests --verbosity normal

# Run single test method
dotnet test --filter "FullyQualifiedName~MethodName"
```

### Build & Run

```bash
# Build entire solution
./scripts/bash/build.sh

# Clean build artifacts
./scripts/bash/clean.sh

# Run API
./scripts/bash/run-api.sh
# Or manually:
cd src/GloboTicket.API && dotnet run

# Run frontend
./scripts/bash/run-web.sh
# Or manually:
cd src/GloboTicket.Web && npm run dev
```

**Ports:**
- API: `http://localhost:5028`
- API Swagger: `http://localhost:5028/swagger`
- Frontend: `http://localhost:5173`
- SQL Server: `localhost:1433`

## Solution Structure

```
src/
├── GloboTicket.Domain/         # Core entities, interfaces (no dependencies)
├── GloboTicket.Application/    # Use cases, DTOs, service interfaces
├── GloboTicket.Infrastructure/ # EF Core, DbContext, configurations, migrations
├── GloboTicket.API/           # Minimal APIs, middleware, authentication
└── GloboTicket.Web/           # React frontend (Vite)

tests/
├── GloboTicket.UnitTests/        # Fast, isolated domain/logic tests
└── GloboTicket.IntegrationTests/ # Full-stack tests with Testcontainers
```

**Dependency Rules:**
- Domain has NO external dependencies
- Application depends on Domain only
- Infrastructure depends on Domain and Application
- API depends on all three
- Web communicates with API via HTTP

## Multi-Tenancy Architecture

### Tenant Resolution Flow

1. User authenticates with username/password
2. `UserConfiguration.cs` maps user to their tenant
3. `TenantResolutionMiddleware` (runs after authentication) sets `ITenantContext.CurrentTenantId`
4. `GloboTicketDbContext` receives tenant context via constructor injection
5. EF Core global query filters automatically filter all queries by `TenantId`

### Entity Classification

**Top-Level Multi-Tenant Entities** (inherit from `MultiTenantEntity`):
- Store `TenantId` directly
- Examples: `Venue`, `Act`, `Customer`
- Entry points for tenant context in their hierarchies
- Automatically filtered via query filters on `TenantId` property

**Child Entities** (inherit from `Entity`):
- Do NOT store `TenantId`
- Inherit tenant context through navigation properties
- Examples: `Show` (via `Venue`), `TicketSale` (via `Show` → `Venue`)
- Filtered via query filters on navigation property chains

**Non-Tenant Entities** (inherit from `Entity`):
- Entities that exist outside tenant isolation
- Example: `Tenant` itself (manages tenant configuration)

### Query Filters

Configured in `GloboTicketDbContext.OnModelCreating()`:

```csharp
// Top-level entity (direct TenantId)
modelBuilder.Entity<Venue>()
    .HasQueryFilter(v => _tenantContext.CurrentTenantId == null ||
                        v.TenantId == _tenantContext.CurrentTenantId);

// Child entity (relationship-based)
modelBuilder.Entity<Show>()
    .HasQueryFilter(s => _tenantContext.CurrentTenantId == null ||
                        s.Venue.TenantId == _tenantContext.CurrentTenantId);
```

**Pattern:**
- Always check for `null` tenant context (allows admin/system queries)
- Top-level entities filter on `TenantId` property
- Child entities filter through navigation property chain

### SaveChanges Behavior

`DbContext` automatically:
1. Sets `TenantId` for new `MultiTenantEntity` instances from `ITenantContext`
2. Sets `CreatedAt` timestamp for new `Entity` instances
3. Sets `UpdatedAt` timestamp for modified `Entity` instances

## Entity Framework Patterns

### Base Class Hierarchy

```
Entity (abstract)
├── Properties: Id, CreatedAt, UpdatedAt
├── Used for: Non-tenant entities, child entities
│
└── MultiTenantEntity (abstract)
    ├── Inherits from Entity, implements ITenantEntity
    ├── Additional Properties: TenantId, Tenant (navigation)
    └── Used for: Top-level tenant-scoped entities
```

### Entity Configuration Requirements

**All Entities:**
```csharp
// 1. Table name
builder.ToTable("EntityNames");

// 2. Primary key
builder.HasKey(e => e.Id);
builder.Property(e => e.Id).ValueGeneratedOnAdd();

// 3. Property configurations (required/optional, max length)
builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

// 4. Inherited timestamp properties
builder.Property(e => e.CreatedAt).IsRequired();
builder.Property(e => e.UpdatedAt).IsRequired(false);
```

**Multi-Tenant Entities (additional):**
```csharp
// 3. Composite alternate key (before indexes)
builder.HasAlternateKey(e => new { e.TenantId, e.EntityGuid });

// 4. Index on GUID
builder.HasIndex(e => e.EntityGuid);

// 5. Tenant foreign key relationship
builder.HasOne(e => e.Tenant)
    .WithMany()
    .HasForeignKey(e => e.TenantId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired();
```

**Configuration Discovery:**
- Automatically applied via `ApplyConfigurationsFromAssembly()` in `DbContext.OnModelCreating()`
- Do NOT manually register configurations

### Complex Types (Value Objects)

For value objects like `Address` that don't have identity:

```csharp
// Entity property (required or optional)
public required Address BillingAddress { get; set; }
public Address? ShippingAddress { get; set; }

// Configuration
builder.ComplexProperty(c => c.BillingAddress, address =>
{
    address.Property(a => a.StreetLine1)
        .IsRequired()
        .HasMaxLength(200)
        .HasColumnName("BillingStreetLine1");  // Prefix to avoid conflicts
    // ... configure other properties
});
```

**Rules:**
- Complex types do NOT inherit from `Entity`
- No `Id` property
- Always use `HasColumnName()` with clear prefixes
- Use `required` keyword for required properties in class definition

### Geospatial Properties

For properties like `Location` (Point from NetTopologySuite):

```csharp
// Entity property
public Point? Location { get; set; }

// Configuration
builder.Property(v => v.Location)
    .HasColumnType("geography");

// DbContext registration (in Program.cs)
builder.Services.AddDbContext<GloboTicketDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.UseNetTopologySuite()));
```

## API Patterns

### Minimal API Endpoints

Endpoints organized by feature in `src/GloboTicket.API/Endpoints/`:
- `AuthEndpoints.cs` - Login/logout
- `TenantEndpoints.cs` - Tenant listing (admin)
- `VenueEndpoints.cs` - Venue CRUD
- `ActEndpoints.cs` - Act CRUD
- `GeocodingEndpoints.cs` - Location search

**Endpoint Registration:**
```csharp
// In Program.cs
app.MapAuthEndpoints();
app.MapTenantEndpoints();
app.MapVenueEndpoints();
```

**Pattern:**
```csharp
public static class FeatureEndpoints
{
    public static void MapFeatureEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/feature")
            .RequireAuthorization();  // Most endpoints require auth

        group.MapGet("/", async (IService service) => {
            // Endpoint implementation
        });
    }
}
```

### Middleware Order

Critical order in `Program.cs`:
1. `UseHttpsRedirection()`
2. `UseCors()`
3. `UseAuthentication()` (sets user identity)
4. `UseAuthorization()` (checks permissions)
5. `UseMiddleware<TenantResolutionMiddleware>()` (sets tenant context)
6. `UseMiddleware<RateLimitingMiddleware>()` (rate limiting)
7. Endpoint mapping

**Important:** Tenant resolution MUST come after authentication to access user claims.

### Authentication

Cookie-based authentication using `CookieAuthenticationDefaults.AuthenticationScheme`:
- Cookie name: `.GloboTicket.Auth`
- Secure, HTTP-only, SameSite=Strict
- Login path: `/auth/login`
- Returns 401 (not redirect) on unauthorized requests

**Test Users** (in `UserConfiguration.cs`):
- `prod` / `prod123` → Production tenant
- `smoke` / `smoke123` → Smoke Test tenant
- `playwright` / `playwright123` → Playwright Test tenant

## Testing Patterns

### Unit Test Structure

**Naming Convention:** `Given{State}_When{Action}_Then{Result}`

```csharp
[Fact]
public void GivenNewAct_WhenCreated_ThenNameDefaultsToEmptyString()
{
    // Arrange
    var act = new Act();

    // Act
    var name = act.Name;

    // Assert
    name.Should().Be(string.Empty);
}
```

**AAA Pattern:**
- Always include `// Arrange`, `// Act`, `// Assert` comments
- Use `// Arrange & Act` when action is object creation
- Use FluentAssertions for all assertions (`.Should()`)

**What to Test for Domain Entities:**
1. **Business Rules & Invariants**: Constructor validation, domain constraints, business logic
2. **Domain Behavior**: Methods that implement business logic, calculations, state transitions
3. **Business-Driven Defaults**: Only test defaults that represent business decisions, not language defaults

**What NOT to Test (Low Quality - Tests Language/Framework):**
- ❌ Property getters/setters (tests the compiler)
- ❌ Inheritance relationships (tests the type system)
- ❌ Interface implementations (tests the type system)
- ❌ Navigation properties (tests EF Core framework)
- ❌ Language-level nullable handling (tests the compiler)
- ❌ Simple property assignment (if `x.Name = "test"` doesn't work, .NET doesn't work)

**Test Organization:**
- One test class per entity: `{EntityName}Tests`
- All test classes are `public`
- Namespace: `GloboTicket.UnitTests.{Category}` (e.g., `Domain`, `API`)

### Integration Test Structure

**Key Characteristics:**
- Uses Testcontainers for real SQL Server
- Each test class gets a unique random tenant ID
- Automatic migration execution before tests
- Safe for parallel execution

**Base Pattern:**
```csharp
public class ServiceIntegrationTests : IAsyncLifetime
{
    private readonly SqlServerContainer _dbContainer;
    private int _testTenantId;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        // Run migrations
        // Generate random tenant ID
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
```

**Tenant Isolation:** Each test uses a unique tenant to prevent cross-test data pollution and validate filtering logic.

## Database Security Model

**Two-User Architecture:**

1. **migration_user** (privileged):
   - DDL operations (CREATE, ALTER, DROP)
   - Full access to `__EFMigrationsHistory`
   - Used for: Migrations only
   - Connection: In migration scripts

2. **app_user** (restricted):
   - SELECT, INSERT, UPDATE, DELETE on data tables
   - No schema modification rights
   - Used for: API runtime, integration tests
   - Connection: In `appsettings.json`

**Why:** Compromised application cannot modify schema (defense in depth, least privilege principle).

## Common Development Tasks

### Adding a New Entity

1. Create entity class in `src/GloboTicket.Domain/Entities/`
   - Inherit from `Entity` or `MultiTenantEntity`
   - Add XML documentation
   - Use `required` for required properties

2. Create configuration in `src/GloboTicket.Infrastructure/Data/Configurations/`
   - Follow configuration order (see Entity Configuration Requirements)
   - Use `IEntityTypeConfiguration<EntityName>`

3. Add query filter in `GloboTicketDbContext.OnModelCreating()`
   - For `MultiTenantEntity`: filter on `TenantId`
   - For child entities: filter via navigation properties

4. Create and apply migration:
   ```bash
   ./scripts/bash/db-migrate-add.sh AddEntityName
   ./scripts/bash/db-update.sh
   ```

5. Write unit tests in `tests/GloboTicket.UnitTests/Domain/`
   - Follow Given-When-Then naming
   - Test all properties, inheritance, interfaces

### Adding a New Endpoint

1. Create service interface in `src/GloboTicket.Application/Interfaces/`
2. Implement service in `src/GloboTicket.Infrastructure/Services/`
3. Create endpoint file in `src/GloboTicket.API/Endpoints/`
   - Use `MapGroup()` for related endpoints
   - Add `.RequireAuthorization()` if needed
4. Register in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IService, ServiceImplementation>();
   app.MapServiceEndpoints();
   ```

### Running Tests During Development

```bash
# Fast feedback loop - unit tests only
./scripts/bash/test-unit.sh

# Before commit - all tests
./scripts/bash/test.sh

# Debug single test
dotnet test --filter "GivenNewAct_WhenCreated_ThenNameDefaultsToEmptyString"
```

## Important Cursor Rules

This project includes Cursor rules in `.cursor/rules/` that define critical patterns:

1. **entity-base-classes.mdc**: Entity inheritance hierarchy (`Entity` vs `MultiTenantEntity`)
2. **multi-tenancy.mdc**: Tenant filtering, query filters, SaveChanges behavior
3. **entity-configuration.mdc**: EF Core Fluent API patterns and ordering
4. **complex-types.mdc**: Value object patterns for `Address` and similar types
5. **relationships.mdc**: Foreign key relationships and navigation properties
6. **entity-properties.mdc**: Property patterns and constraints
7. **unit-test-patterns.mdc**: Test naming, structure (AAA), and coverage requirements

**When working on entity classes, configurations, or tests, refer to these rules for consistent patterns.**

## Known Technical Debt

### Rate Limiting Service

**Issue:** `RateLimitService` uses in-memory `ConcurrentDictionary`, which doesn't work in multi-server deployments.

**Impact:**
- Single-server: Works correctly
- Multi-server: Rate limits apply per server (effective limit = configured × server count)
- Server restarts reset counters

**Solution:** Migrate to distributed cache (Redis, NCache) for shared state across servers.

**Priority:** Low for single-server, high for production multi-server environments.

## Frontend Development

**Stack:**
- React 18, TypeScript, Vite
- Tanstack Query 5 for server state
- Located in `src/GloboTicket.Web/`

**Development:**
```bash
cd src/GloboTicket.Web
npm install
npm run dev  # Runs on http://localhost:5173
```

**Configuration:**
- Vite config: `vite.config.ts`
- API proxy configured for `/api` routes
- CORS enabled for `http://localhost:5173` in API

## Environment Variables & Secrets

**Local Development:**
- SQL Server: Connection string in `appsettings.json` and scripts
- Mapbox: Use User Secrets for API token (see README.md)
  ```bash
  dotnet user-secrets set "Mapbox:AccessToken" "your_token"
  ```

**Production:** Use Azure Key Vault or environment variables (never commit secrets).

## Additional Resources

- **README.md**: Quick start, prerequisites, full setup guide
- **GETTING_STARTED.md**: Step-by-step setup instructions
- **docs/architecture.md**: Detailed system architecture
- **docs/prd.md**: Product requirements
- **database/README.md**: Database schema and migration guide
- **docker/README.md**: Docker configuration details
