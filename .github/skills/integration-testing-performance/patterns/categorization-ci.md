# Test Categorization and CI/CD

Tag tests with traits to control execution by pipeline stage.

```csharp
[Trait("Category", "Unit")]
[Trait("Speed", "Fast")]
public class VenueDomainTests { }

[Trait("Category", "Integration")]
[Trait("Speed", "Medium")]
public class VenueDatabaseTests { }

[Trait("Category", "Performance")]
[Trait("Speed", "Slow")]
public class VenuePerformanceTests { }

[Trait("Category", "E2E")]
[Trait("Speed", "Slow")]
public class VenueEndToEndTests { }
```

```csharp
[Trait("Database", "PostgreSQL")]
[Trait("Feature", "MultiTenancy")]
[Trait("Environment", "RequiresDocker")]
```

```bash
# PR fast path
dotnet test --filter "Category=Unit" --logger trx

# Feature branches include integration
dotnet test --filter "Category=Unit|Category=Integration" --logger trx

# Main excludes slowest
dotnet test --filter "Category!=Performance&Category!=E2E" --logger trx

# Nightly full suite
dotnet test --logger trx

# Deployment E2E only
dotnet test --filter "Category=E2E" --logger trx
```