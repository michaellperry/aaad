---
name: spec-writing-database
description: Database schema specification using Mermaid ER diagrams, table structures, constraints, and indexes for multi-tenant applications.
---

# Database Schema Specification Patterns

Use when designing database schemas with Mermaid ER diagrams, tables, constraints, and indexes for multi-tenant applications. **IMPORTANT**: Specify WHAT needs to be built (data structure, relationships, business rules), NOT HOW to build it (no Entity Framework entity classes or fluent configuration code).

## When to use
- Creating ER diagrams for new features or schema documentation
- Defining table structures with columns, data types, defaults, and constraints
- Specifying CHECK, UNIQUE, and FOREIGN KEY constraints for data integrity
- Designing indexes for tenant-filtered queries, uniqueness, and performance
- Planning additive or breaking schema migrations

## Core principles
- Use Mermaid erDiagram syntax with PK/FK/UK annotations and field descriptions
- All domain entities include TenantId FK; unique constraints scoped as (TenantId, BusinessKey)
- Indexes lead with TenantId; use INCLUDE for covering indexes; filter WHERE for active records
- Integer PKs for performance; GUID alternate keys for external APIs; audit timestamps (CreatedAt, UpdatedAt)
- CHECK constraints enforce business rules; FK cascade rules respect relationships (RESTRICT, CASCADE)
- Breaking changes require multi-step migrations with application deployments between steps

## Resources
- Mermaid ER diagrams: [patterns/mermaid-er-diagrams.md](patterns/mermaid-er-diagrams.md)
- Table structures: [patterns/table-structures.md](patterns/table-structures.md)
- Constraints: [patterns/constraints.md](patterns/constraints.md)
- Indexes: [patterns/indexes.md](patterns/indexes.md)
- Design principles: [patterns/design-principles.md](patterns/design-principles.md)
- Schema migrations: [patterns/schema-migrations.md](patterns/schema-migrations.md)

## Default locations
- Specifications: docs/specs/{feature-name}.md (Database Schema section)
- Mermaid diagrams embedded in markdown specs
- Migration scripts referenced but not specified in detail

## Validation checklist
- ER diagram shows all entities, relationships (||--o{, etc.), and field annotations (PK, FK, UK)
- Tables include TenantId FK, integer PK, GUID UK, audit timestamps (CreatedAt, UpdatedAt), and defaults
- Composite unique constraints: (TenantId, Name) or (TenantId, BusinessGuid)
- CHECK constraints validate business rules (capacity ranges, dates, prices, non-empty strings)
- FK constraints specify ON DELETE behavior (RESTRICT for parents, CASCADE for children)
- Indexes: TenantId first column, INCLUDE for projections, filtered WHERE for active records
- Design principles documented: tenant isolation, integer PKs, GUID UKs, audit fields, soft deletes
- Migration strategy specified: additive (backward-compatible) vs breaking (multi-step)