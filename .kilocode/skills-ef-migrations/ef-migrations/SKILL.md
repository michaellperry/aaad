---
name: ef-migrations
description: Instructions for managing Entity Framework Core migrations using project scripts. Use when adding, removing, applying, or listing database migrations.
---

# Entity Framework Core Migrations

This project uses shell scripts to manage EF Core migrations. These scripts handle project paths and configuration automatically.

## Scripts Location

- Bash (macOS/Linux): `./scripts/bash/`
- PowerShell (Windows): `./scripts/powershell/`

## Workflow

### 1. Create a New Migration

Use this when you have modified domain entities or configurations and need to update the database schema.

**Bash:**
```bash
./scripts/bash/db-migrate-add.sh <MigrationName>
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-add.ps1 <MigrationName>
```

### 2. Apply Migrations

Use this to update the database to match the current migrations.

**Bash:**
```bash
./scripts/bash/db-update.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-update.ps1
```

### 3. Remove Last Migration

Use this if you created a migration by mistake and haven't applied it yet (or want to revert the code changes and the migration artifact).

**Bash:**
```bash
./scripts/bash/db-migrate-remove.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-remove.ps1
```

### 4. List Migrations

**Bash:**
```bash
./scripts/bash/db-migrate-list.sh
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-list.ps1
```

### 5. Rollback Database

Use this to revert the *database* to a specific previous migration.

**Bash:**
```bash
./scripts/bash/db-migrate-rollback.sh <TargetMigrationName>
```

**PowerShell:**
```powershell
./scripts/powershell/db-migrate-rollback.ps1 <TargetMigrationName>
```

