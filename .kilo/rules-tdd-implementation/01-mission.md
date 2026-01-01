# TDD Implementation - Mission

Implement the minimum code necessary to make all failing tests pass, following specifications exactly as documented in the `docs/specs` folder. You work in the "Green" phase of TDD - turning red (failing) tests green (passing).

## Critical Workflow

Follow this exact sequence - do NOT skip steps:

### Phase 1: Pre-Implementation Validation

1. **Compile the Solution**
   - Run: `./scripts/bash/build.sh` or `dotnet build`
   - If compilation FAILS: ABORT immediately and report the compilation errors to the user
   - If compilation SUCCEEDS: Proceed to Phase 2
   - Rationale: Cannot fix tests if code doesn't compile

2. **Identify Failing Tests**
   - Run: `./scripts/bash/test.sh` or appropriate test command
   - Capture which tests are failing and why
   - Document the failure messages and stack traces
   - If NO tests fail: Report that all tests already pass and exit
   - If tests fail: Proceed to Phase 3

### Phase 2: Specification Analysis

1. **Locate Relevant Specifications**
   - Examine `docs/specs/` folder for specifications related to failing tests
   - Identify the exact requirements that need to be implemented
   - Note any constraints, validation rules, or business logic
   - If specifications are unclear or missing: Request clarification from user

2. **Plan Implementation**
   - Determine which files need to be created or modified
   - Identify the minimal code changes required
   - Consider Clean Architecture layer boundaries (Domain, Application, Infrastructure)
   - Respect the existing codebase patterns from CLAUDE.md

### Phase 3: Implementation

1. **Write Minimal Code**
   - Implement ONLY what is needed to make tests pass
   - Follow project-specific patterns:
     - Entity configurations in Infrastructure/Data/Configurations/
     - Service implementations in Infrastructure/Services/
     - Endpoint definitions in API/Endpoints/
     - Follow entity base class rules (Entity vs MultiTenantEntity)
   - Apply proper error handling and validation
   - Include XML documentation comments
   - Use `required` keyword for required properties

2. **Adhere to Project Standards**
   - Follow Clean Architecture dependency rules
   - Implement multi-tenancy patterns correctly (query filters, TenantId)
   - Use proper EF Core configuration patterns
   - Follow naming conventions and code organization
   - Respect the security model (app_user vs migration_user)

3. **Verify After Each Change**
   - Compile: `./scripts/bash/build.sh`
   - Run tests: `./scripts/bash/test.sh`
   - Check if previously failing tests now pass
   - Ensure no existing tests broke (regression check)

### Phase 4: Completion

1. **Final Validation**
   - Run full test suite one final time
   - Verify ALL tests pass (unit and integration)
   - Confirm compilation succeeds

2. **Report Results**
   - SUCCESS: "Implementation complete. All tests now pass. Summary: [list what was implemented]"
   - PARTIAL: "Implementation incomplete. Passing: X tests. Still failing: Y tests. Reason: [explanation]"
   - FAILURE: "Unable to complete implementation. Reason: [detailed explanation]. User action needed: [specific guidance]"
