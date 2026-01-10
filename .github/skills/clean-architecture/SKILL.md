---
name: clean-architecture
description: Clean Architecture principles and implementation patterns for .NET applications. Use when designing application structure, clarifying layer responsibilities, or implementing Clean Architecture in .NET projects.
---

# Clean Architecture Implementation Guide

When implementing Clean Architecture in .NET applications, follow these core principles:

## Layer Separation

### Domain Layer (Core Business Logic)
- Contains business entities, value objects, and domain services
- No dependencies on external frameworks or libraries
- Pure business logic without technical concerns
- Example: `Entity<T>`, `IAggregateRoot`, domain events

### Application Layer (Use Cases)
- Implements use cases and application services
- Contains DTOs, interfaces, and command/query handlers
- Orchestrates domain logic but doesn't contain business rules
- Example: `ICommandHandler<T>`, `IQueryHandler<T, TResult>`

### Infrastructure Layer (External Concerns)
- Database access, file system, external APIs
- Implementation of application layer interfaces
- Technical details like Entity Framework configuration
- Example: `DbContext`, `IRepository<T>`, `IEmailService`

### Presentation Layer (API/UI)
- API endpoints, controllers, or web interfaces
- HTTP request/response handling
- Input validation and output formatting
- Example: Controllers, middleware, DTO mapping

## Key Principles

### Dependency Rule
- Dependencies flow inward toward the Domain
- Inner layers don't know about outer layers
- Use dependency inversion for all cross-layer communication

### Business Logic Independence
- Business rules should be framework-agnostic
- No dependency on Entity Framework, ASP.NET Core, etc.
- Entities should be POCOs (Plain Old CLR Objects)

### Interface Segregation
- Define interfaces in the layer that needs them
- Implement interfaces in the appropriate outer layer
- Use dependency injection for all cross-layer dependencies

## Implementation Patterns

### Entity Structure
```csharp
// Domain Entity
public abstract class Entity<T>
{
    public T Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    
    // Domain logic methods
    public void Validate() { /* business rules */ }
}

// Multi-tenant entity
public abstract class MultiTenantEntity : Entity<Guid>, ITenantEntity
{
    public Guid TenantId { get; protected set; }
    
    public bool BelongsToTenant(Guid tenantId) => 
        TenantId == tenantId;
}
```

### Repository Pattern
```csharp
// Application layer interface
public interface IRepository<T> where T : Entity<Guid>
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

// Infrastructure implementation
public class Repository<T> : IRepository<T> where T : Entity<Guid>
{
    private readonly GloboTicketDbContext _context;
    
    // Implementation with EF Core
}
```

### Use Case Pattern
```csharp
public class CreateVenueCommand : IRequest<VenueDto>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public Guid TenantId { get; set; }
}

public class CreateVenueCommandHandler :
    IRequestHandler<CreateVenueCommand, VenueDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;
    
    public async Task<VenueDto> Handle(CreateVenueCommand request)
    {
        var venue = new Venue(request.Name, request.Address, request.TenantId);
        await _venueRepository.AddAsync(venue);
        return _mapper.Map<VenueDto>(venue);
    }
}
```

## GloboTicket Implementation

### Project Structure
- `GloboTicket.Domain/` - Domain entities and business logic
- `GloboTicket.Application/` - Application services, use cases, DTOs, interfaces
- `GloboTicket.Infrastructure/` - EF Core DbContext, configurations, external services
- `GloboTicket.API/` - Minimal API endpoints, middleware, HTTP concerns

### Layer Responsibilities in GloboTicket
**Application Layer (`GloboTicket.Application/`):**
- `Services/` - Application services that orchestrate domain logic (ActService, VenueService, ShowService, etc.)
- `DTOs/` - Data transfer objects for API contracts
- `MultiTenancy/` - ITenantContext interface for tenant resolution
- Services inject EF Core's base `DbContext` class and use `Set<TEntity>()` for data access

**Infrastructure Layer (`GloboTicket.Infrastructure/`):**
- `Data/GloboTicketDbContext.cs` - Concrete EF Core DbContext implementation
- `Data/Configurations/` - Entity Framework Fluent API configurations
- `Data/Migrations/` - EF Core database migrations
- Implements Application layer interfaces, never contains business logic

### Tenant Context Integration
- Use ITenantContext for tenant resolution
- Enforce tenant isolation in all queries
- Include tenant validation in business logic

### Configuration
- Fluent API for entity configuration
- Separate configuration classes per entity
- Proper relationship and constraint definitions

## Testing Strategy
- Unit tests for domain logic (Domain layer)
- Application tests for use cases (Application layer)
- Integration tests for infrastructure (Infrastructure layer)
- API tests for endpoints (Presentation layer)

This architecture ensures maintainability, testability, and adherence to SOLID principles while providing clear separation of concerns.