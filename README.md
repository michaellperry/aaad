# GloboTicket

A modern, multi-tenant event ticketing platform built with .NET 10 and React, demonstrating clean architecture principles and enterprise-grade patterns.

## 🎯 Project Overview

GloboTicket is a multi-tenant event ticketing platform built with .NET 10 and React. The platform supports **data isolation through tenants** within each deployment environment. This enables scenarios such as running smoke tests in a production environment without affecting production data, by using separate tenants within the same database.

**Environments vs Tenants:**
- **Environments** are separate deployments (Development, Staging, Production) with their own servers and databases
- **Tenants** provide data isolation within an environment's database, allowing multiple data contexts to coexist safely

This project serves as a reference implementation for:
- **Clean Architecture** with clear separation of concerns
- **Multi-tenancy** at the database level with row-level isolation
- **Domain-Driven Design** principles
- **Test-Driven Development** with comprehensive unit and integration tests
- **Modern API design** using Minimal APIs
- **Container-first deployment** with Docker

## 🏗️ Architecture

The application follows Clean Architecture principles with clear boundaries between layers:

```
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                     │
│  ┌──────────────┐              ┌──────────────┐         │
│  │  Web (React) │              │  API (.NET)  │         │
│  └──────────────┘              └──────────────┘         │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│               Application Layer (Use Cases)             │
│  • DTOs          • Services        • Interfaces         │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│              Domain Layer (Business Logic)              │
│  • Entities      • Value Objects   • Domain Services    │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│           Infrastructure Layer (External)               │
│  • EF Core       • SQL Server      • Repositories       │
└─────────────────────────────────────────────────────────┘
```

### Key Architectural Components

- **Domain Layer**: Core business logic and entities with no external dependencies
- **Application Layer**: Use cases and business workflows
- **Infrastructure Layer**: Database access, external services, and technical implementations
- **API Layer**: RESTful endpoints, authentication, and tenant resolution
- **Web Layer**: React-based frontend (configured, ready for implementation)

For detailed architecture documentation, see [`docs/architecture.md`](docs/architecture.md).

## 🚀 Technology Stack

### Backend
- **.NET 10** - Latest framework features and performance improvements
- **ASP.NET Core Minimal APIs** - Modern, lightweight API design
- **Entity Framework Core 10** - ORM with migrations and LINQ support
- **SQL Server 2022** - Enterprise-grade relational database
- **xUnit** - Unit and integration testing framework
- **FluentAssertions** - Expressive test assertions
- **Testcontainers** - Docker-based integration testing

### Frontend
- **React 18** - Modern UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server

### Infrastructure
- **Docker & Docker Compose** - Containerized development and deployment
- **SQL Server in Docker** - Isolated database environment

## 🔐 Multi-Tenancy

GloboTicket implements **database-level multi-tenancy** with row-level isolation:

### Tenant Resolution Strategy
1. User authenticates with credentials
2. System maps user to their tenant via [`UserConfiguration`](src/GloboTicket.API/Configuration/UserConfiguration.cs)
3. [`TenantResolutionMiddleware`](src/GloboTicket.API/Middleware/TenantResolutionMiddleware.cs) sets tenant context for the request
4. All database queries automatically filter by tenant ID

### Data Isolation
- Each entity implementing [`ITenantEntity`](src/GloboTicket.Domain/Interfaces/ITenantEntity.cs) includes a `TenantId`
- EF Core query filters ensure automatic tenant filtering
- No cross-tenant data access is possible
- Multiple tenants can coexist within the same environment's database

### Use Cases
- **Multiple Organizations**: Each organization gets its own tenant for complete data isolation
- **Environment Testing**: Within a production environment, use separate tenants for production data and smoke test data
- **Data Segregation**: Isolate different data contexts (e.g., production vs. validation) within the same database

### Benefits
- **Security**: Complete data isolation between tenants
- **Efficiency**: Shared infrastructure and codebase within an environment
- **Scalability**: Easy to add new tenants without code changes
- **Cost Effective**: Single database can serve multiple tenants
- **Testing**: Run validation tests in production environment without affecting production tenant data

## 📦 Project Structure

```
GloboTicket/
├── src/
│   ├── GloboTicket.Domain/           # Core business entities and interfaces
│   ├── GloboTicket.Application/      # Use cases, DTOs, and services
│   ├── GloboTicket.Infrastructure/   # Data access, EF Core, migrations
│   ├── GloboTicket.API/              # REST API, endpoints, middleware
│   └── GloboTicket.Web/              # React frontend application
├── tests/
│   ├── GloboTicket.UnitTests/        # Unit tests (22 tests)
│   └── GloboTicket.IntegrationTests/ # Integration tests (10 tests)
├── docker/                           # Docker Compose and initialization
├── database/                         # Database documentation
└── docs/                             # Architecture and requirements
```

## ⚡ Quick Start

### Prerequisites

**IMPORTANT:** This project requires .NET 10 SDK. Verify your installation before proceeding.

- **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (version 10.0 or later) - **REQUIRED**
  ```bash
  # Verify you have .NET 10
  dotnet --version
  # Should show 10.x.x
  ```
  
  **If you see version 9.x or earlier, you must upgrade to .NET 10 SDK before continuing.**

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js 18+](https://nodejs.org/) (for frontend)
- **Entity Framework Core Tools** (required for database migrations)
  ```bash
  # Install EF Core tools globally (requires .NET 10 SDK)
  dotnet tool install --global dotnet-ef
  
  # Verify installation
  dotnet ef --version
  ```
  
  **Note:** The EF Core tools version must match your .NET SDK version. If you have .NET 10 SDK installed and the tool installation fails, you may need to specify the version explicitly or update an existing installation:
  ```bash
  dotnet tool update --global dotnet-ef
  ```

### 1. Start Infrastructure

```bash
# Start SQL Server in Docker (automatically initializes database users)
./scripts/bash/docker-up.sh
# OR
pwsh scripts/powershell/docker-up.ps1
```

The script automatically:
- Starts SQL Server container
- Waits for SQL Server to become healthy
- Runs database initialization (creates `migration_user` and `app_user`)
- Verifies initialization completed successfully

### 2. Run Migrations

```bash
# Apply database migrations (automatically restores NuGet packages)
./scripts/bash/db-update.sh
# OR
pwsh scripts/powershell/db-update.ps1
```

**Note:** The migration scripts now automatically run `dotnet restore` before applying migrations. If you prefer to restore packages manually first, you can run:
```bash
dotnet restore
```

### 3. Start the API

```bash
cd src/GloboTicket.API
dotnet run
```

The API documentation will be available at `http://localhost:5028/swagger`

### 4. Configure Mapbox (Required for Venue Location Features)

The venue location picker feature requires Mapbox access tokens for both geocoding (backend) and map rendering (frontend).

#### Get a Mapbox Access Token

1. **Create a Mapbox account** (if you don't have one):
   - Go to [https://account.mapbox.com/](https://account.mapbox.com/)
   - Sign up for a free account (includes 50,000 free map loads per month)

2. **Create an access token**:
   - Navigate to [Access Tokens](https://account.mapbox.com/access-tokens/)
   - Click "Create a token"
   - Name it (e.g., "GloboTicket Development")
   - For development, you can use the default public token
   - **For production**: Create separate secret tokens and restrict by URL

#### Configure Backend (API) - Using User Secrets

**User Secrets** is a .NET feature that stores sensitive configuration outside of your project files, preventing secrets from being committed to source control.

1. **Initialize User Secrets** (one-time setup):
   ```bash
   cd src/GloboTicket.API
   dotnet user-secrets init
   ```

2. **Set the Mapbox access token**:
   ```bash
   dotnet user-secrets set "Mapbox:AccessToken" "your_mapbox_secret_token_here"
   ```

3. **Verify the secret was set** (optional):
   ```bash
   dotnet user-secrets list
   ```

**Note:** 
- Use a **secret token** for the backend (geocoding API calls). Secret tokens should never be exposed to the client.
- User Secrets are automatically loaded in Development environment
- The token is stored in your user profile (not in the project), so it won't be committed to source control
- For other developers: Each developer must set their own User Secrets locally

#### Configure Frontend (Web)

Create a `.env` file in `src/GloboTicket.Web/` (or add to existing `.env`):

```bash
VITE_MAPBOX_ACCESS_TOKEN=your_mapbox_public_token_here
```

**Note:** 
- For the frontend, you can use a **public token** with URL restrictions. In Mapbox dashboard:
  - Go to your token settings
  - Under "URL restrictions", add: `http://localhost:5173/*` (for development)
  - For production, add your production domain
- The `.env` file is already included in `.gitignore` and will not be committed to source control

**Security Best Practices:**
- **Backend token**: Use User Secrets for local development (automatically excluded from source control)
- **Frontend token**: Use a public token with strict URL restrictions in `.env` file (add `.env` to `.gitignore`)
- **Production**: Store tokens in environment variables, Azure Key Vault, or other secure configuration management
- **Never commit secrets**: User Secrets and `.env` files should never be committed to version control

### 5. Start the Frontend (Optional)

```bash
cd src/GloboTicket.Web
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

**Note:** If Mapbox tokens are not configured, the venue location picker will display an error message. The rest of the application will function normally.

For detailed setup instructions, see [`GETTING_STARTED.md`](GETTING_STARTED.md).

## 🧪 Running Tests

### Unit Tests
```bash
dotnet test tests/GloboTicket.UnitTests --verbosity normal
```

Tests cover:
- Domain entity behavior
- Tenant entity validation
- API middleware functionality

### Integration Tests
```bash
dotnet test tests/GloboTicket.IntegrationTests --verbosity normal
```

Tests cover:
- Database operations with Testcontainers
- Multi-tenancy isolation
- Service layer functionality

### All Tests
```bash
dotnet test --verbosity normal
```

## 🔒 Security Model

### Authentication
- **Cookie-based authentication** for stateful sessions
- Session management with automatic expiration
- Secure login/logout endpoints

### Authorization
- **Tenant-based access control** - Users can only access their tenant's data
- **Middleware-enforced isolation** - Automatic tenant filtering on all queries
- **Row-level security** via EF Core query filters

### Test Credentials

For development purposes, test users are configured in [`UserConfiguration.cs`](src/GloboTicket.API/Configuration/UserConfiguration.cs):

| Username   | Password      | Tenant          |
|------------|---------------|-----------------|
| prod       | prod123       | Production      |
| smoke      | smoke123      | Smoke Test      |
| playwright | playwright123 | Playwright Test |

**Note:** These tenants exist within the same environment's database. In a production environment, you can use the "Smoke Test" tenant to run post-deployment validation tests without affecting the "Production" tenant's data.

**⚠️ Note:** These are test credentials only. Production systems must implement proper user management.

## 🔌 API Endpoints

### Health Check
```http
GET /health
```
Returns system health status.

### Authentication
```http
POST /auth/login
Content-Type: application/json

{
  "username": "prod",
  "password": "prod123"
}
```

```http
POST /auth/logout
```

### Tenants
```http
GET /api/tenants
Authorization: Required (Cookie-based)
```
Returns all tenants (for admin demonstration purposes).

## 🛠️ Development Workflow

1. **Make Changes**: Edit code in appropriate layer
2. **Write Tests**: Add/update tests for new functionality
3. **Run Tests**: Ensure all tests pass
4. **Build**: Verify solution compiles
5. **Test Locally**: Run API and verify behavior
6. **Commit**: Follow conventional commit messages

### Adding New Features

1. **Domain**: Add entities and interfaces in `GloboTicket.Domain`
2. **Application**: Create DTOs and services in `GloboTicket.Application`
3. **Infrastructure**: Implement repository patterns in `GloboTicket.Infrastructure`
4. **API**: Add endpoints in `GloboTicket.API/Endpoints`
5. **Tests**: Add unit and integration tests

### Database Changes

```bash
# Create migration
dotnet ef migrations add MigrationName --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API

# Apply migration
dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
```

## 📚 Documentation

- **[Getting Started Guide](GETTING_STARTED.md)** - Step-by-step setup instructions
- **[Architecture Documentation](docs/architecture.md)** - Detailed system design
- **[Product Requirements](docs/prd.md)** - Business requirements and specifications
- **[Docker Setup](docker/README.md)** - Container configuration details
- **[Database Guide](database/README.md)** - Migration and schema information
- **[Verification Checklist](VERIFICATION.md)** - System verification results

## 🎯 Current Status

### ✅ Implemented
- Clean Architecture foundation with all layers
- Multi-tenant data isolation with EF Core
- Database migrations with test data
- Cookie-based authentication and session management
- Tenant resolution middleware
- RESTful API with health checks
- Comprehensive unit tests (22 passing)
- Integration tests with Testcontainers (10 passing)
- Docker-based SQL Server infrastructure
- React frontend project (configured)

### 🚧 Pending Implementation
Domain-specific features such as:
- Event management (create, update, delete events)
- Ticket inventory and pricing
- Order processing and payment
- Customer management
- Reporting and analytics

The architectural foundation is complete and ready for domain feature implementation.

## ⚠️ Technical Debt

### Rate Limiting: Distributed Cache Required

**Current Implementation:**
The rate limiting service (`RateLimitService`) uses in-memory storage (`ConcurrentDictionary`) to track request counts per user. This implementation works for single-server deployments but has significant limitations for production multi-server environments.

**Issues:**
1. **Horizontal Scaling**: Rate limit state is not shared across application servers. In a load-balanced environment, users can exceed limits by making requests to different servers (effective limit = configured limit × number of servers).
2. **Server Restarts**: Rate limit counters are lost on server restart, allowing users to immediately make a full quota of requests again.
3. **Deployment Windows**: During rolling deployments, some servers reset while others maintain state, making rate limiting unreliable.

**Recommended Solution:**
Migrate to a distributed cache (Redis, NCache, or similar) to:
- Share rate limit state across all application servers
- Maintain accuracy in multi-server deployments
- Provide better resilience (cache can survive app restarts if configured)
- Support horizontal scaling without compromising rate limit enforcement

**Implementation Approach:**
- Replace `ConcurrentDictionary` with distributed cache operations
- Store rate limit entries with TTL matching the rate limit window
- Use atomic operations for thread-safe increment/decrement
- Consider using `IDistributedCache` interface for abstraction

**Priority:** Medium (acceptable for single-server deployments, critical for production multi-server environments)

## 🤝 Contributing

This is a reference implementation for educational purposes. Key principles:

1. Follow Clean Architecture patterns
2. Maintain test coverage
3. Respect tenant isolation boundaries
4. Write clear, self-documenting code
5. Update documentation as you go

## 📝 License

This project is for educational and demonstration purposes.

## 🙏 Acknowledgments

Built using industry best practices and patterns from:
- Clean Architecture (Robert C. Martin)
- Domain-Driven Design (Eric Evans)
- Microsoft .NET Documentation
- Entity Framework Core Patterns

---

**Ready to start developing?** Check out the [`GETTING_STARTED.md`](GETTING_STARTED.md) guide for detailed setup instructions.