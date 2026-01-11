---
name: minimal-api-patterns
description: ASP.NET Core Minimal APIs patterns for endpoints, DTOs, validation, and service integration. Use when implementing API endpoints, defining request/response contracts, or structuring API projects with clean separation of concerns.
---

# ASP.NET Core Minimal API Patterns

Use when adding or adjusting Minimal API endpoints, DTOs, validation, or OpenAPI metadata in GloboTicket.

## When to use
- Defining or updating endpoint groups in src/GloboTicket.API/Endpoints
- Creating/maintaining request/response DTOs and validators in GloboTicket.Application
- Wiring services into handlers (no repositories in endpoints)
- Enforcing auth/authorization per group or route and documenting responses

## Core principles
- Group related routes with `MapGroup`, shared auth, tags, and OpenAPI
- Return DTOs only; validate inputs with FluentValidation before executing handlers
- Inject application services into handlers; keep domain/data access out of endpoints
- Centralize error handling via filters; standardize validation/problem responses
- Apply policies/roles per route; name/describe endpoints for Swagger
- Cache read endpoints where safe; prefer async + cancellation tokens

## Resources
- Endpoint structure: [patterns/endpoint-grouping.md](patterns/endpoint-grouping.md), [patterns/endpoint-delegates.md](patterns/endpoint-delegates.md)
- DTOs and validation: [patterns/dto-and-validation.md](patterns/dto-and-validation.md)
- Service integration: [patterns/service-layer.md](patterns/service-layer.md)
- Errors and filters: [patterns/error-handling.md](patterns/error-handling.md)
- AuthZ: [patterns/authentication-authorization.md](patterns/authentication-authorization.md)
- OpenAPI docs: [patterns/openapi.md](patterns/openapi.md)
- Performance/async: [patterns/performance.md](patterns/performance.md)

## Default locations
- Endpoint groups: src/GloboTicket.API/Endpoints
- DTOs/validators: src/GloboTicket.Application/DTOs and Validators
- Services: src/GloboTicket.Application/Services
- Filters/middleware: src/GloboTicket.API (filters or middleware folder)

## Validation checklist
- Endpoint groups require auth and are tagged/named
- DTOs returned instead of domain entities; validators applied in handlers
- Handlers inject services (no repositories); tenant/context checks live in services
- Error/validation filters registered on groups or globally
- Swagger metadata set (names, summaries, Produces/Accepts)
- Read endpoints use caching when appropriate; async handlers accept CancellationToken