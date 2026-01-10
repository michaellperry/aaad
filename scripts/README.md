# Developer Scripts

This directory contains PowerShell and Bash scripts to simplify common development tasks for the GloboTicket project. Both script sets provide identical functionality - choose the one that works best for your environment.

## Choosing Your Script Environment

### PowerShell Scripts (`powershell/*.ps1`)

**Best for:**
- Windows developers (native PowerShell)
- Cross-platform teams (PowerShell Core works on Windows, Mac, and Linux)

**Requirements:**
- PowerShell Core (pwsh) - Install from [PowerShell GitHub](https://github.com/PowerShell/PowerShell/releases)
- Verify installation: `pwsh --version`

**Usage:**
```powershell
pwsh scripts/powershell/db-update.ps1
```

### Bash Scripts (`bash/*.sh`)

**Best for:**
- Mac and Linux developers (native bash)
- Windows developers using WSL or Git Bash

**Requirements:**
- Bash shell (usually pre-installed on Mac/Linux)
- Windows: Use Git Bash (included with Git) or WSL

**Usage:**
```bash
# First time: make scripts executable
chmod +x scripts/bash/*.sh

# Then run scripts
./scripts/bash/db-update.sh
```

## Script Reference

### Database Migration Scripts

#### db-update.ps1 / db-update.sh
Apply all pending migrations to the database.

**Usage:**
```powershell
pwsh scripts/powershell/db-update.ps1
```
```bash
./scripts/bash/db-update.sh
```

**What it does:**
- Uses `migration_user` credentials for elevated permissions
- Applies all pending migrations to the database
- Shows success/failure status

---

#### db-migrate-add.ps1 / db-migrate-add.sh
Create a new migration.

**Usage:**
```powershell
pwsh scripts/powershell/db-migrate-add.ps1 -MigrationName "AddEventTable"
```
```bash
./scripts/bash/db-migrate-add.sh AddEventTable
```

**Parameters:**
- `MigrationName` (required): Name of the migration in PascalCase (e.g., `AddEventTable`)

**What it does:**
- Creates migration files in `src/GloboTicket.Infrastructure/Data/Migrations/`
- Generates migration class, designer file, and updates snapshot

---

#### db-migrate-remove.ps1 / db-migrate-remove.sh
Remove the last migration (unapplies if applied, deletes if not).

**Usage:**
```powershell
pwsh scripts/powershell/db-migrate-remove.ps1
```
```bash
./scripts/bash/db-migrate-remove.sh
```

**What it does:**
- Removes the most recent migration
- Only works if the migration hasn't been applied to any database
- If migration is applied, you must rollback first using `db-migrate-rollback`

---

#### db-migrate-list.ps1 / db-migrate-list.sh
List all migrations and their applied status.

**Usage:**
```powershell
pwsh scripts/powershell/db-migrate-list.ps1
```
```bash
./scripts/bash/db-migrate-list.sh
```

**What it does:**
- Shows all migrations in the project
- Marks applied migrations with `*`
- Helps identify which migrations need to be applied

---

#### db-migrate-delete.ps1 / db-migrate-delete.sh
Delete a migration file after confirming it's not applied to the database.

**Usage:**
```powershell
pwsh scripts/powershell/db-migrate-delete.ps1 -MigrationName "AddEventTable"
```
```bash
./scripts/bash/db-migrate-delete.sh AddEventTable
```

**Parameters:**
- `MigrationName` (required): Name of the migration to delete

**What it does:**
1. Checks if the migration is applied to the database
2. If applied, shows an error and exits
3. If not applied, removes the migration files
4. Safely prevents deletion of applied migrations

**Note:** If the migration has been applied, use `db-migrate-rollback` first.

---

#### db-migrate-rollback.ps1 / db-migrate-rollback.sh
Rollback to a specific migration.

**Usage:**
```powershell
# Rollback to a specific migration
pwsh scripts/powershell/db-migrate-rollback.ps1 -TargetMigration "InitialCreate"

# Rollback all migrations
pwsh scripts/powershell/db-migrate-rollback.ps1 -TargetMigration "0"
```
```bash
# Rollback to a specific migration
./scripts/bash/db-migrate-rollback.sh InitialCreate

# Rollback all migrations
./scripts/bash/db-migrate-rollback.sh 0
```

**Parameters:**
- `TargetMigration` (required): Name of the migration to rollback to, or `"0"` to rollback all migrations

**What it does:**
- Rolls back the database to the specified migration
- Uses `migration_user` credentials for elevated permissions
- Useful for undoing applied migrations

---

#### db-reset.ps1 / db-reset.sh
Drop and recreate the database with all migrations.

**Usage:**
```powershell
pwsh scripts/powershell/db-reset.ps1
```
```bash
./scripts/bash/db-reset.sh
```

**What it does:**
1. Drops the existing database (ignores errors if database doesn't exist)
2. Recreates the database
3. Applies all migrations from scratch
4. Useful for starting fresh during development

**Warning:** This will delete all data in the database!

---

### Docker Scripts

#### docker-up.ps1 / docker-up.sh
Start Docker infrastructure.

**Usage:**
```powershell
pwsh scripts/powershell/docker-up.ps1
```
```bash
./scripts/bash/docker-up.sh
```

**What it does:**
- Starts SQL Server container in detached mode
- Waits 5 seconds for containers to initialize
- Shows container status

---

#### docker-down.ps1 / docker-down.sh
Stop Docker infrastructure.

**Usage:**
```powershell
pwsh scripts/powershell/docker-down.ps1
```
```bash
./scripts/bash/docker-down.sh
```

**What it does:**
- Stops and removes Docker containers
- Cleans up network resources

---

#### docker-status.ps1 / docker-status.sh
Check Docker container status.

**Usage:**
```powershell
pwsh scripts/powershell/docker-status.ps1
```
```bash
./scripts/bash/docker-status.sh
```

**What it does:**
- Shows status of all containers
- Displays health status and uptime

---

#### docker-logs.ps1 / docker-logs.sh
View Docker logs.

**Usage:**
```powershell
# View all logs
pwsh scripts/powershell/docker-logs.ps1

# View logs for specific service
pwsh scripts/powershell/docker-logs.ps1 -ServiceName "sqlserver"
```
```bash
# View all logs
./scripts/bash/docker-logs.sh

# View logs for specific service
./scripts/bash/docker-logs.sh sqlserver
```

**Parameters:**
- `ServiceName` (optional): Name of the service to view logs for

**What it does:**
- Shows container logs
- Useful for debugging container issues

---

### Testing Scripts

#### test.ps1 / test.sh
Run tests with optional filter.

**Usage:**
```powershell
# Run all tests
pwsh scripts/powershell/test.ps1

# Run unit tests only
pwsh scripts/powershell/test.ps1 -Filter "unit"

# Run integration tests only
pwsh scripts/powershell/test.ps1 -Filter "integration"
```
```bash
# Run all tests
./scripts/bash/test.sh

# Run unit tests only
./scripts/bash/test.sh unit

# Run integration tests only
./scripts/bash/test.sh integration
```

**Parameters:**
- `Filter` (optional): `"unit"`, `"integration"`, or `"all"` (default)

**What it does:**
- Runs tests with normal verbosity
- Shows test results and summary

---

#### test-unit.ps1 / test-unit.sh
Run unit tests only.

**Usage:**
```powershell
pwsh scripts/powershell/test-unit.ps1
```
```bash
./scripts/bash/test-unit.sh
```

**What it does:**
- Runs only unit tests from `tests/GloboTicket.UnitTests`

---

#### test-integration.ps1 / test-integration.sh
Run integration tests only.

**Usage:**
```powershell
pwsh scripts/powershell/test-integration.ps1
```
```bash
./scripts/bash/test-integration.sh
```

**What it does:**
- Runs only integration tests from `tests/GloboTicket.IntegrationTests`
- Requires Docker to be running

---

### Build Scripts

#### build.ps1 / build.sh
Build the solution.

**Usage:**
```powershell
pwsh scripts/powershell/build.ps1
```
```bash
./scripts/bash/build.sh
```

**What it does:**
- Builds all projects in the solution
- Shows build errors and warnings

---

#### clean.ps1 / clean.sh
Clean build artifacts.

**Usage:**
```powershell
pwsh scripts/powershell/clean.ps1
```
```bash
./scripts/bash/clean.sh
```

**What it does:**
- Removes all build outputs (bin/, obj/ directories)
- Useful before rebuilding from scratch

---

#### format.ps1 / format.sh
Format code and remove unnecessary imports.

**Usage:**
```powershell
pwsh scripts/powershell/format.ps1
```
```bash
./scripts/bash/format.sh
```

**What it does:**
- Runs `dotnet format` on the entire solution
- Removes unnecessary using statements (IDE0005)
- Applies code style rules from `.editorconfig`
- Formats code according to configured style preferences

**Note:** The `.editorconfig` file in the repository root configures the formatting rules, including the IDE0005 rule set to warning severity.

---

### Run Scripts

#### run-api.ps1 / run-api.sh
Run the API server.

**Usage:**
```powershell
pwsh scripts/powershell/run-api.ps1
```
```bash
./scripts/bash/run-api.sh
```

**What it does:**
- Starts the API server on `http://localhost:5028`
- Runs until Ctrl+C is pressed
- Shows API logs in the console

---

#### run-web.ps1 / run-web.sh
Run the web frontend.

**Usage:**
```powershell
pwsh scripts/powershell/run-web.ps1
```
```bash
./scripts/bash/run-web.sh
```

**What it does:**
- Checks for `node_modules` and installs dependencies if needed
- Starts the Vite development server on `http://localhost:5173`
- Runs until Ctrl+C is pressed

---

## Common Workflows

### Initial Setup
```bash
# Start Docker
./scripts/bash/docker-up.sh

# Apply migrations
./scripts/bash/db-update.sh

# Build solution
./scripts/bash/build.sh

# Run tests
./scripts/bash/test.sh
```

### Formatting Code
```bash
# Format entire solution
./scripts/bash/format.sh

# Verify no formatting changes are needed (useful for CI/CD)
dotnet format GloboTicket.sln --verify-no-changes
```

### Creating a New Migration
```bash
# Create migration
./scripts/bash/db-migrate-add.sh AddEventTable

# Review migration files
# Edit if needed

# Apply migration
./scripts/bash/db-update.sh
```

### Undoing a Migration
```bash
# Check migration status
./scripts/bash/db-migrate-list.sh

# If not applied, remove it
./scripts/bash/db-migrate-remove.sh

# If applied, rollback first
./scripts/bash/db-migrate-rollback.sh InitialCreate
./scripts/bash/db-migrate-remove.sh
```

### Resetting Database
```bash
# Drop and recreate database
./scripts/bash/db-reset.sh
```

### Development Workflow
```bash
# Start Docker
./scripts/bash/docker-up.sh

# Run API in one terminal
./scripts/bash/run-api.sh

# Run web frontend in another terminal
./scripts/bash/run-web.sh
```

## Troubleshooting

### PowerShell Scripts Not Running

**Issue:** `pwsh: command not found`

**Solution:** Install PowerShell Core:
- Windows: Download from [PowerShell GitHub](https://github.com/PowerShell/PowerShell/releases)
- Mac: `brew install --cask powershell`
- Linux: Follow [installation guide](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-linux)

### Bash Scripts Not Executable

**Issue:** `Permission denied`

**Solution:** Make scripts executable:
```bash
chmod +x scripts/bash/*.sh
```

### Migration Scripts Failing

**Issue:** `CREATE TABLE permission denied`

**Solution:** The scripts use `migration_user` credentials automatically. If you're still getting permission errors:
1. Verify Docker is running: `./scripts/bash/docker-status.sh`
2. Check that database users are initialized (see `docker/init-db/01-create-users.sql`)
3. Wait for SQL Server to be healthy (may take 30-60 seconds after startup)

### Docker Scripts Failing

**Issue:** `docker compose: command not found`

**Solution:** 
- Ensure Docker Desktop is installed and running
- Use `docker compose` (with space) not `docker-compose` (with hyphen)
- Verify Docker is in your PATH

### Scripts Not Found

**Issue:** Scripts can't be found when running

**Solution:** 
- Always run scripts from the project root directory
- Use relative paths: `./scripts/bash/db-update.sh` or `pwsh scripts/powershell/db-update.ps1`
- Don't use absolute paths unless necessary

## Platform-Specific Notes

### Windows
- PowerShell scripts work natively
- Bash scripts require Git Bash or WSL
- Use forward slashes in paths (scripts handle this)

### Mac
- Bash scripts work natively
- PowerShell requires `brew install --cask powershell`
- Both script sets work equally well

### Linux
- Bash scripts work natively
- PowerShell requires installation (see PowerShell GitHub)
- Both script sets work equally well

## Script Naming Conventions

- **db-***: Database and migration operations
- **docker-***: Docker container management
- **test-***: Test execution
- **build/clean**: Build operations
- **run-***: Application execution

All scripts use kebab-case naming for consistency.

## Contributing

When adding new scripts:
1. Create both PowerShell and Bash versions
2. Keep functionality identical between platforms
3. Include error handling and parameter validation
4. Update this README with documentation
5. Test on your target platform before committing

