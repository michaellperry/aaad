---
name: infrastructure-standards
description: Infrastructure standards for Docker, scripts, middleware, and authentication in multi-tenant deployments.
---

# Infrastructure Standards

Use when containerizing API/Web/DB services, keeping scripts aligned across platforms, or configuring security/auth for GloboTicket deployments.

## When to use
- Bootstrapping or adjusting Docker/compose environments (local or CI)
- Adding or updating scripts under scripts/bash or scripts/powershell
- Implementing security/tenant middleware or JWT/authorization policies
- Auditing middleware order before releases

## Core principles
- Pin images, add health checks, and prefer multi-stage builds with non-root runtime
- Keep bash and PowerShell feature parity; make scripts idempotent, especially migrations
- Resolve tenant after authentication; enforce security headers and correct pipeline order
- Validate JWTs with explicit tenant/role policies and custom permission requirements

## Resources
- Docker and env: [patterns/docker-compose.md](patterns/docker-compose.md), [patterns/env-configuration.md](patterns/env-configuration.md), [patterns/dockerfile-api.md](patterns/dockerfile-api.md)
- Scripts: [patterns/scripts-cross-platform.md](patterns/scripts-cross-platform.md), [patterns/scripts-idempotent-migrations.md](patterns/scripts-idempotent-migrations.md)
- Middleware and auth: [patterns/middleware-security-headers.md](patterns/middleware-security-headers.md), [patterns/middleware-tenant-resolution.md](patterns/middleware-tenant-resolution.md), [patterns/authentication-jwt.md](patterns/authentication-jwt.md), [patterns/authorization-policies.md](patterns/authorization-policies.md), [patterns/middleware-order.md](patterns/middleware-order.md)

## Default locations
- Compose and env files under docker/, .env in repository root
- Scripts in scripts/bash and scripts/powershell
- Middleware in src/GloboTicket.API/Middleware, auth configuration in Program.cs
- Database initialization scripts in docker/init-db

## Validation checklist
- Services build and pass health checks with pinned images
- Scripts succeed in bash and PowerShell with identical behaviors
- Middleware order matches pattern; tenant resolution enforced on protected endpoints
- JWT validation returns JSON errors; policies enforce tenant and role requirements