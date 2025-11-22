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
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js 18+](https://nodejs.org/) (for frontend)

### 1. Start Infrastructure

```bash
# Start SQL Server in Docker
cd docker
docker compose up -d
cd ..
```

### 2. Run Migrations

```bash
# Apply database migrations
dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
```

### 3. Start the API

```bash
cd src/GloboTicket.API
dotnet run
```

The API will be available at `http://localhost:5028`

### 4. Start the Frontend (Optional)

```bash
cd src/GloboTicket.Web
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

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

| Username | Password  | Tenant         | Tenant ID |
|----------|-----------|----------------|-----------|
| prod     | prod123   | Production     | 1         |
| smoke    | smoke123  | Smoke Test     | 2         |

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