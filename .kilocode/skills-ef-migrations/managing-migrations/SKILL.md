---
name: managing-migrations
description: Manages Entity Framework Core migrations using project scripts. Use when creating, applying, listing, or removing database migrations.
---

# Managing EF Core Migrations

## Overview

This project provides standardized scripts for all EF Core migration operations. **Always use these scripts instead of running `dotnet ef` commands directly.** The scripts ensure consistent project paths, output directories, and connection strings across all environments.

## Available Scripts

### Bash Scripts (Linux/macOS)
Located in `scripts/bash/`:
- `db-migrate-add.sh` - Create a new migration
- `db-migrate-list.sh` - List all migrations and their status
- `db-migrate-remove.sh` - Remove the last migration (if not applied)
- `db-update.sh` - Apply all pending migrations to the database
- `db-reset.sh` - Drop and recreate the database with all migrations

### PowerShell Scripts (Windows/Cross-platform)
Located in `scripts/powershell/`:
- `db-migrate-add.ps1` - Create a new migration
- `db-migrate-list.ps1` - List all migrations and their status
- `db-migrate-remove.ps1` - Remove the last migration (if not applied)
- `db-update.ps1` - Apply all pending migrations to the database
- `db-reset.ps1` - Drop and recreate the database with all migrations

## Common Workflows

### 1. Creating a New Migration

After modifying entity configurations or adding new entities:

**Bash:**
```bash
./scripts/bash/db-migrate-add.sh AddShowEntity
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-add.ps1 -MigrationName AddShowEntity
```

**Migration Naming Conventions:**
- Use PascalCase
- Start with a verb: `Add`, `Update`, `Remove`, `Rename`
- Be descriptive: `AddShowEntity`, `UpdateVenueAddressFields`, `RemoveObsoleteColumns`

### 2. Applying Migrations

After creating a migration or pulling new migrations from source control:

**Bash:**
```bash
./scripts/bash/db-update.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-update.ps1
```

### 3. Checking Migration Status

To see which migrations exist and which have been applied:

**Bash:**
```bash
./scripts/bash/db-migrate-list.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-list.ps1
```

### 4. Removing a Migration

If you created a migration by mistake and **haven't applied it yet**:

**Bash:**
```bash
./scripts/bash/db-migrate-remove.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-remove.ps1
```

**Important:** This only works if the migration hasn't been applied to any database. If it has been applied, you need to create a new migration to revert the changes.

### 5. Resetting the Database

To drop and recreate the database with all migrations (useful for development):

**Bash:**
```bash
./scripts/bash/db-reset.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-reset.ps1
```

**Warning:** This destroys all data in the database. Only use in development environments.

## Standard Workflow

When implementing persistence layer changes:

1. **Modify entity configurations** in `src/GloboTicket.Infrastructure/Data/Configurations/`
2. **Create migration** using `db-migrate-add.sh` or `db-migrate-add.ps1`
3. **Review generated migration** in `src/GloboTicket.Infrastructure/Data/Migrations/`
4. **Apply migration** using `db-update.sh` or `db-update.ps1`
5. **Verify schema** by checking the database or running integration tests

## Script Configuration

The scripts are pre-configured with:
- **Project:** `src/GloboTicket.Infrastructure`
- **Startup Project:** `src/GloboTicket.API`
- **Output Directory:** `Data/Migrations`
- **Connection String:** `Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True`

You should **never need to specify these manually** when using the scripts.

## Why Use Scripts?

1. **Consistency:** Everyone uses the same project paths and settings
2. **Safety:** Connection strings and output directories are standardized
3. **Simplicity:** No need to remember complex `dotnet ef` command syntax
4. **Cross-platform:** Bash and PowerShell versions provide identical functionality
5. **Error Handling:** Scripts include proper error checking and user feedback

## Troubleshooting

### "No migrations configuration type was found"
- Ensure you're running the script from the project root directory
- The scripts expect to be run from the workspace root (not from subdirectories)

### "The migration has already been applied to the database"
- You cannot remove a migration that has been applied
- Create a new migration to revert the changes instead

### "Build failed"
- Fix compilation errors before creating migrations
- EF Core needs a successful build to generate migrations

### "Unable to connect to the database"
- Ensure Docker containers are running: `./scripts/bash/docker-up.sh`
- Check database status: `./scripts/bash/docker-status.sh`
