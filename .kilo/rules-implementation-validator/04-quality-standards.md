# Quality Standards

## Precision

Every ❌ must include:
- Exact file path where implementation should exist or is incomplete
- Line number if file exists but implementation is wrong
- Specific remediation: "Add X to file Y" or "Implement method Z in class W"
- Code example when helpful

## Completeness

Validate ALL layers:
- Domain entities and business rules
- Application DTOs and interfaces
- Infrastructure implementations and configurations
- API endpoints and registrations
- Database migrations and DbContext setup
- Dependency injection wiring

## Actionability

Never report "endpoint missing" - report "Add `app.MapShowEndpoints();` to Program.cs line 87 (after app.MapActEndpoints())"

## Context Awareness

Reference project conventions:
- "Entity should inherit from MultiTenantEntity per project patterns"
- "Configuration must follow standard order: ToTable, HasKey, HasAlternateKey..."
- "Query filter missing in GloboTicketDbContext.OnModelCreating() per multi-tenancy rules"

## Edge Cases & Special Handling

1. **Partial Implementations**: If a class exists but key methods throw NotImplementedException, mark the class ✅ but methods ❌ separately

2. **Convention-Based Discovery**: If spec says "Create ShowService" but doesn't specify exact location, use project conventions (Infrastructure/Services/)

3. **Implicit Requirements**: Validate implied requirements:
   - If spec shows API endpoint, validate service is registered in DI
   - If spec shows multi-tenant entity, validate query filter exists
   - If spec shows DTO with validation, check for validation attributes

4. **Configuration Validation**: Don't just check if configuration exists - verify:
   - Correct configuration order per project rules
   - Required vs. optional matches spec
   - MaxLength values match spec
   - Foreign key cascade behavior is appropriate

5. **Multi-File Features**: Track requirements that span files:
   - Entity + Configuration + Query Filter + Migration
   - DTO + Validation + Service Interface + Service Implementation + Endpoint + Registration

## Self-Validation Checklist

Before finalizing your report, verify:
- [ ] Parsed 100% of specification requirements
- [ ] Checked all architectural layers (Domain, Application, Infrastructure, API)
- [ ] Validated dependency injection registrations
- [ ] Confirmed endpoint mappings in correct order
- [ ] Verified query filters for multi-tenant entities
- [ ] Checked for NotImplementedException in all implementations
- [ ] Provided file paths and line numbers for every item
- [ ] Included specific remediation steps for all ❌ items
- [ ] Calculated accurate completion percentage
- [ ] Prioritized critical missing items by dependency order
