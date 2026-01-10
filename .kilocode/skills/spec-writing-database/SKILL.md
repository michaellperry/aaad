---
name: spec-writing-database
description: Writes database schema specifications using Mermaid ER diagrams and defines table structures for multi-tenant entities. Use when designing database schemas, creating entity-relationship diagrams, or specifying database constraints for GloboTicket features.
---

# Database Schema

**IMPORTANT**: This section specifies the database schema design only. Do NOT include Entity Framework entity classes or fluent configuration code. The specification should describe WHAT needs to be built, not HOW to build it.

Provide Entity-Relationship diagram using Mermaid:

```mermaid
erDiagram
    Tenant ||--o{ NewEntity : has
    NewEntity ||--o{ ChildEntity : contains
    ExistingEntity ||--o{ NewEntity : references

    NewEntity {
        int Id PK
        int TenantId FK
        uuid EntityGuid UK
        string Name
        datetime CreatedAt
        datetime UpdatedAt
    }

    ChildEntity {
        int Id PK
        int NewEntityId FK
        string Property
        datetime CreatedAt
        datetime UpdatedAt
    }
```

List indexes for new or modified tables.

## Database Design Principles

- All domain entities must be directly or indirectly related to Tenant for multi-tenancy
- Define ALL properties with data types, constraints, and nullability
- Use composite alternate keys for multi-tenant entities
- Specify relationships and foreign keys
- Add indexes for frequently queried columns
- Use meaningful table names (plural, PascalCase)
- Show modifications to existing entities if needed

