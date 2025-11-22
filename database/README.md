# GloboTicket Database Management

This directory contains documentation and resources for managing the GloboTicket database schema and migrations.

## Table of Contents

- [Overview](#overview)
- [Two-User Security Model](#two-user-security-model)
- [Connection Strings](#connection-strings)
- [Creating Migrations](#creating-migrations)
- [Applying Migrations](#applying-migrations)
- [Migration Bundles](#migration-bundles)
- [Common Tasks](#common-tasks)
- [Troubleshooting](#troubleshooting)

## Overview

GloboTicket uses Entity Framework Core for database schema management and migrations. The project implements a two-user security model to provide defense-in-depth security where the application runtime has restricted permissions.

**Migration Files Location**: `src/GloboTicket.Infrastructure/Data/Migrations/`

## Two-User Security Model

The database uses two separate users with different privilege levels:

### 1. Migration User (`migration_user`)
- **Purpose**: Execute database migrations and schema changes
- **Permissions**: Full DDL (CREATE, ALTER, DROP) and DML operations
- **Password**: `Migration@Pass123`
- **Usage**: EF Core migration execution only
- **Security Note**: Should only be used during deployment/migration windows

### 2. Application User (`app_user`)
- **Purpose**: Runtime data operations for the API
- **Permissions**: DML only (SELECT, INSERT, UPDATE, DELETE) on application tables
- **Password**: `YourStrong@Passw0rd`
- **Usage**: Normal API operations via connection string
- **Security Note**: Cannot modify schema, follows principle of least privilege

This separation provides defense-in-depth security: even if the application is compromised, attackers cannot modify the database schema or access system tables.

## Connection Strings

### Migration User (for migrations)
```
Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True
```

### Application User (for API runtime)
```
Server=localhost,1433;Database=GloboTicket;User Id=app_user;Password=YourStrong@Passw0rd;TrustServerCertificate=True;Encrypt=True
```

## Creating Migrations

### Prerequisites
1. Ensure Docker is running: `docker compose -f docker/docker-compose.yml ps`
2. Ensure SQL Server is healthy (may take ~50 seconds after start)
3. Database users must be initialized (see docker/init-db/01-create-users.sql)

### Create a New Migration

From the project root directory:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output-dir Data/Migrations
```

**Example:**
```bash
dotnet ef migrations add AddEventTable \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output-dir Data/Migrations
```

This creates three files in `src/GloboTicket.Infrastructure/Data/Migrations/`:
- `<timestamp>_<MigrationName>.cs` - The migration class
- `<timestamp>_<MigrationName>.Designer.cs` - Migration metadata
- `GloboTicketDbContextModelSnapshot.cs` - Updated snapshot

### Best Practices for Migration Names
- Use PascalCase: `AddEventTable`, `UpdateTenantIndexes`
- Be descriptive: `AddEventAndCategoryTables` is better than `Changes`
- Indicate the type of change: `Add`, `Update`, `Remove`, `Alter`

## Applying Migrations

### Development Environment

Apply migrations using the `migration_user` connection string with elevated privileges:

```bash
dotnet ef database update \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"
```

### Verify Migration

Check which migrations have been applied:

```bash
dotnet ef migrations list \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

### Rollback Migration

To roll back to a previous migration:

```bash
dotnet ef database update <PreviousMigrationName> \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"
```

To roll back all migrations:

```bash
dotnet ef database update 0 \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"
```

## Migration Bundles

Migration bundles are self-contained executables that can apply migrations without requiring the .NET SDK. They are useful for production deployments.

### Creating a Migration Bundle

```bash
dotnet ef migrations bundle \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output database/efbundle
```

### Using a Migration Bundle

On the target server (production):

```bash
./efbundle \
  --connection "Server=<server>;Database=GloboTicket;User Id=migration_user;Password=<password>;TrustServerCertificate=True;Encrypt=True"
```

### Bundle Options

- `--self-contained`: Include .NET runtime in bundle
- `--runtime <RID>`: Specify runtime identifier (e.g., linux-x64, win-x64)
- `--target-runtime <RID>`: Alternative to --runtime
- `--force`: Overwrite existing bundle

**Example for Linux production:**
```bash
dotnet ef migrations bundle \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output database/efbundle \
  --self-contained \
  --runtime linux-x64
```

## Common Tasks

### View Current Schema

```bash
docker compose -f docker/docker-compose.yml exec sqlserver \
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U app_user \
  -P 'YourStrong@Passw0rd' -d GloboTicket -C \
  -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"
```

### Generate Migration Script

Generate SQL script without applying:

```bash
dotnet ef migrations script \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output database/migration.sql
```

Generate script for specific migration range:

```bash
dotnet ef migrations script FromMigration ToMigration \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output database/migration.sql
```

### Remove Last Migration (if not applied)

```bash
dotnet ef migrations remove \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

**Warning:** Only works if the migration hasn't been applied to any database.

## Troubleshooting

### Migration Fails with "CREATE TABLE permission denied"

**Problem**: You're using `app_user` instead of `migration_user`.

**Solution**: Always use the `--connection` parameter with `migration_user` credentials when applying migrations.

### Docker SQL Server Not Ready

**Problem**: Database connection fails.

**Solution**: 
1. Check if container is running: `docker compose -f docker/docker-compose.yml ps`
2. Wait for SQL Server to be healthy (may take ~50 seconds)
3. Check logs: `docker compose -f docker/docker-compose.yml logs sqlserver`

### Database Users Not Found

**Problem**: Login failed for `migration_user` or `app_user`.

**Solution**: Initialize database users:
```bash
docker compose -f docker/docker-compose.yml exec sqlserver \
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa \
  -P 'YourStrong!Passw0rd' -C \
  -Q "CREATE DATABASE GloboTicket; CREATE LOGIN migration_user WITH PASSWORD = 'Migration@Pass123'; CREATE LOGIN app_user WITH PASSWORD = 'YourStrong@Passw0rd';"
```

### Migration Already Applied

**Problem**: Trying to apply a migration that's already in the database.

**Solution**: Check applied migrations:
```bash
dotnet ef migrations list \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

### Connection String in Wrong File

**Problem**: EF tools can't find connection string.

**Solution**: 
- For migrations: Use `--connection` parameter
- For API runtime: Check `src/GloboTicket.API/appsettings.json`
- For design-time: Check `src/GloboTicket.Infrastructure/appsettings.json` (optional)

## Security Best Practices

1. **Never commit passwords**: Use environment variables or Azure Key Vault in production
2. **Rotate credentials regularly**: Especially `migration_user` password
3. **Limit migration_user access**: Only available during deployment windows
4. **Monitor migration_user usage**: Audit all schema changes
5. **Use migration bundles in production**: Don't deploy source code
6. **Test migrations in staging**: Always test before production
7. **Backup before migrations**: Always have a rollback plan

## Additional Resources

- [EF Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [EF Core CLI Tools Documentation](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [GloboTicket Architecture Documentation](../docs/architecture.md)
- [Docker Setup](../docker/README.md)

## Quick Reference

```bash
# Create migration
dotnet ef migrations add <Name> --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API --output-dir Data/Migrations

# Apply migration (development)
dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API --connection "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

# List migrations
dotnet ef migrations list --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API

# Generate SQL script
dotnet ef migrations script --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API --output database/migration.sql

# Create migration bundle
dotnet ef migrations bundle --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API --output database/efbundle