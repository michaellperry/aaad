# TDD Test First - Responsibilities

1. **Analyze Specifications Thoroughly**: When given a specification, extract all testable requirements, edge cases, and acceptance criteria. Identify the components needed: DTOs, models, entities, service interfaces, service implementations, and API endpoints.

2. **Design Test-First Architecture**: Before writing any tests, mentally architect the solution:
   - Identify all required types (DTOs, models, entities)
   - Define service interface contracts
   - Plan Minimal API endpoints and their signatures
   - Map specifications to specific test scenarios

3. **Write Comprehensive Failing Unit Tests**: Create unit tests following these principles:
   - **Naming Convention**: Use `Given{Context}_When{Action}_Then{ExpectedOutcome}` format
   - **AAA Pattern**: Structure every test with clear `// Arrange`, `// Act`, `// Assert` sections
   - **FluentAssertions**: Use `.Should()` syntax for all assertions
   - **Coverage**: Write tests for:
     - Happy path scenarios
     - Edge cases and boundary conditions
     - Error handling and validation
     - Null/empty input handling
     - Business rule enforcement
   - **Isolation**: Each test should be independent and test one specific behavior
   - **Clarity**: Test names and failure messages should clearly communicate intent

4. **Create Skeleton Implementations**: Generate minimal code to make tests compile:
   - **DTOs**: Define all properties with appropriate types and attributes (Required, MaxLength, etc.)
   - **Models**: Create domain models with proper encapsulation
   - **Entities**: Follow project patterns (Entity/MultiTenantEntity base classes if applicable)
   - **Service Interfaces**: Define method signatures with clear contracts
   - **Service Classes**: Implement interfaces with methods throwing `NotImplementedException` with descriptive messages
   - **DI Registration**: Note that services must be registered in `Program.cs`:
     ```csharp
     builder.Services.AddScoped<IFeatureService, FeatureService>();
     ```
   - **Minimal API Endpoints**: Create endpoint methods in `{Feature}Endpoints.cs` files with proper routing and authorization, throwing `NotImplementedException` in handlers

5. **Ensure Tests Fail Correctly**: Verify that:
   - All code compiles without errors
   - All tests fail due to `NotImplementedException` (not compilation errors)
   - Failure messages clearly indicate what functionality is missing
   - Test output provides actionable information for implementation
