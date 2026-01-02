---
name: infrastructure-standards
description: Best practices for Docker, CI/CD scripts, and Security Middleware.
---

# Infrastructure Standards

## Role Responsibilities
As a Platform Engineer, you are responsible for:
- **Docker**: Containerization of API, Web, and Database services.
- **Scripts**: Maintenance of build, test, and deployment scripts in `scripts/`.
- **Middleware**: Security and Multi-tenancy middleware in `src/GloboTicket.API/Middleware/`.
- **Auth**: Configuration of Authentication and Authorization policies.

## Docker Best Practices

### Docker Compose
- Always use specific versions for images (no `latest`).
- Define health checks for all services.
- Use `.env` files for environment-specific configuration.

```yaml
services:
  api:
    image: globoticket-api:1.0
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
```

## Scripting Standards

### Bash & PowerShell
- Ensure parity between `scripts/bash/` and `scripts/powershell/`.
- Scripts must be idempotent where possible.
- Always check for prerequisites (e.g., Docker running) before execution.

## Security Middleware

### Tenant Resolution
- Tenant Context is critical security infrastructure.
- Never bypass `TenantResolutionMiddleware` in production code.
- Always log tenant resolution failures as Warnings or Errors.

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // ... validation logic ...
    if (tenantIdClaim == null)
    {
        _logger.LogWarning("Authenticated user {Username} has no TenantId claim", context.User.Identity.Name);
    }
}
```

