# GloboTicket Docker Infrastructure

This directory contains Docker Compose configuration for running GloboTicket's local development environment.

## Overview

The Docker infrastructure provides:
- **SQL Server 2022**: Database server with persistent storage
- **Two-User Security Model**: Separate users for migrations and runtime operations
- **Health Checks**: Automatic verification of database readiness
- **Data Persistence**: Named volumes preserve data between container restarts

## Two-User Security Model

GloboTicket implements a defense-in-depth security approach using two separate database users:

### 1. Migration User (`migration_user`)

**Purpose**: Execute database migrations and schema changes

**Credentials**:
- Username: `migration_user`
- Password: `Migration@Pass123`

**Permissions**:
- Full DDL rights (CREATE, ALTER, DROP tables/indexes/etc.)
- Full DML rights on all tables
- Can modify `__EFMigrationsHistory` table
- Member of `db_owner` role

**Usage**: 
- ONLY for running EF Core migration bundles
- Should be used during deployment/migration windows only
- Never used by the running application

**Security Rationale**:
Schema modification is a privileged operation that should be isolated from normal application operations. By restricting this capability to a separate user, we ensure that even if the application is compromised, attackers cannot modify the database schema.

### 2. Application User (`app_user`)

**Purpose**: Normal runtime database operations for the API

**Credentials**:
- Username: `app_user`
- Password: `YourStrong@Passw0rd`

**Permissions**:
- DML operations only (SELECT, INSERT, UPDATE, DELETE)
- Member of `db_datareader` and `db_datawriter` roles
- NO schema modification rights
- NO access to system tables (beyond what's needed for queries)

**Usage**:
- Used by the API during normal operation
- Connection string in `appsettings.json`
- All entity operations go through this user

**Security Rationale**:
Following the principle of least privilege, the application only needs to read and write data, not modify schema. This significantly reduces the attack surface if the application is compromised.

## Quick Start

### Starting the Infrastructure

```bash
# From the project root directory
cd docker

# Start SQL Server in detached mode
docker compose up -d

# View logs (optional)
docker compose logs -f sqlserver

# Wait for health check to pass (watch for "healthy" status)
docker compose ps
```

The SQL Server container is ready when the health check shows as **healthy** (typically 30-60 seconds after startup).

### Verifying Database Readiness

```bash
# Check container status - look for "healthy" in the STATUS column
docker compose ps

# Expected output:
# NAME                    STATUS
# globoticket-sqlserver   Up X minutes (healthy)
```

### Automatic Database Initialization

**Database users are automatically created** when you start Docker Compose. An init container (`db-init`) runs after SQL Server becomes healthy and executes the initialization script.

**How it works:**
1. SQL Server container starts and becomes healthy
2. `db-init` container automatically starts (depends on SQL Server health)
3. Init script waits for SQL Server to be ready
4. Initialization script creates `migration_user`, `app_user`, and `GloboTicket` database
5. Init container exits with success code

**Verifying initialization:**
```bash
# Check init container status (should show "Exited (0)")
docker compose ps db-init

# View initialization logs
docker compose logs db-init
```

**Manual initialization (if needed):**
If automatic initialization fails, you can manually run the script:
```bash
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' -C \
  -i /init-db/01-create-users.sql
```

### Running Migrations

After initializing users, run EF Core migrations using the migration user:

```bash
# From the project root
cd src/GloboTicket.Infrastructure

# Run migrations (ensure you have a connection string for migration_user)
dotnet ef database update --connection "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=true"
```

### Stopping the Infrastructure

```bash
# Stop containers (preserves data)
docker compose down

# Stop and remove volumes (DELETES ALL DATA)
docker compose down -v
```

## Connection Strings

### For Migrations (migration_user)

```
Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=true;Encrypt=true
```

Use this connection string when:
- Running `dotnet ef database update`
- Creating new migrations
- Executing migration bundles

### For Application Runtime (app_user)

```
Server=localhost,1433;Database=GloboTicket;User Id=app_user;Password=YourStrong@Passw0rd;TrustServerCertificate=true;Encrypt=true
```

Use this connection string in:
- `appsettings.Development.json`
- Integration test configurations
- Any normal API operations

### For Administrative Tasks (sa)

```
Server=localhost,1433;Database=GloboTicket;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;Encrypt=true
```

Use this connection string only for:
- Initial database setup
- Running the user initialization script
- Emergency administrative access
- Troubleshooting

**WARNING**: Never use the `sa` account in application code or configuration files.

## Health Check Details

The Docker Compose configuration includes a health check that:
- Runs every 10 seconds
- Executes `SELECT 1` to verify database connectivity
- Times out after 5 seconds
- Requires 5 consecutive failures before marking unhealthy
- Waits 30 seconds after startup before first check

The container status will show:
- `starting` - Container is starting up
- `healthy` - Database is ready to accept connections
- `unhealthy` - Database failed health checks

## Docker Compose Services

### sqlserver

- **Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Port**: 1433 (mapped to host 1433)
- **Volumes**:
  - `sqlserver-data`: Named volume for persistent data storage
- **Network**: `globoticket-network` (bridge)
- **Restart Policy**: `unless-stopped`
- **Health Check**: Verifies SQL Server is ready to accept connections

### db-init

- **Image**: `mcr.microsoft.com/mssql/server:2022-latest` (uses sqlcmd tools)
- **Purpose**: Automatically initialize database users and GloboTicket database
- **Dependencies**: Waits for `sqlserver` to be healthy before starting
- **Volumes**:
  - `./init-db`: Initialization SQL scripts (read-only)
  - `./scripts`: Initialization bash script (read-only)
- **Network**: `globoticket-network` (bridge)
- **Restart Policy**: `no` (runs once and exits)
- **Behavior**: Runs automatically on first startup, exits after successful initialization

## Troubleshooting

### Container Won't Start

**Symptom**: Container exits immediately after starting

**Possible Causes**:
1. **Password complexity**: SA_PASSWORD must meet SQL Server requirements (8+ chars, upper, lower, digit, special)
2. **EULA not accepted**: Ensure `ACCEPT_EULA=Y` in docker-compose.yml
3. **Port conflict**: Another service is using port 1433

**Solutions**:
```bash
# Check logs for error details
docker compose logs sqlserver

# Check if port 1433 is already in use
lsof -i :1433  # macOS/Linux
netstat -ano | findstr :1433  # Windows

# Verify environment variables
docker compose config
```

### Cannot Connect to Database

**Symptom**: Connection refused or timeout errors

**Solutions**:
```bash
# Verify container is running and healthy
docker compose ps

# Check if SQL Server process is running inside container
docker compose exec sqlserver ps aux | grep sqlservr

# Test connection from inside container
docker compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT @@VERSION"

# Check firewall settings (ensure port 1433 is open)
```

### Users Not Created

**Symptom**: Cannot login with migration_user or app_user

**Possible Causes**:
1. Init container failed to run
2. Init container exited with error
3. SQL Server wasn't healthy when init container started

**Solutions**:
1. Check init container status:
   ```bash
   docker compose ps db-init
   ```

2. View init container logs:
   ```bash
   docker compose logs db-init
   ```

3. Verify SQL Server was healthy:
   ```bash
   docker compose ps sqlserver
   ```

4. Manually run initialization (if needed):
   ```bash
   docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
     -S localhost -U sa -P 'YourStrong!Passw0rd' -C \
     -i /init-db/01-create-users.sql
   ```

5. Test fresh setup:
   ```bash
   # Bash
   ./scripts/bash/docker-test-reset.sh
   
   # PowerShell
   pwsh scripts/powershell/docker-test-reset.ps1
   ```

### Permission Denied Errors

**Symptom**: `app_user` gets permission denied when running migrations

**Cause**: Using wrong user for wrong operation

**Solution**: 
- Use `migration_user` for migrations/schema changes
- Use `app_user` only for SELECT/INSERT/UPDATE/DELETE operations

### Lost Data After Container Restart

**Symptom**: Database is empty after stopping and starting containers

**Cause**: Likely ran `docker compose down -v` which removes volumes

**Solution**:
- Use `docker compose down` (without `-v`) to preserve data
- For permanent deletion, explicitly use `-v` flag
- Backup important data before removing volumes

### Health Check Failing

**Symptom**: Container status shows `unhealthy`

**Solutions**:
```bash
# Check health check logs
docker compose logs sqlserver | grep health

# Manually test the health check command
docker compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT 1"

# Restart the container
docker compose restart sqlserver
```

## Data Management

### Backing Up the Database

```bash
# Create a backup inside the container
docker compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' \
  -Q "BACKUP DATABASE GloboTicket TO DISK='/var/opt/mssql/data/GloboTicket.bak'"

# Copy backup to host
docker compose cp sqlserver:/var/opt/mssql/data/GloboTicket.bak ./GloboTicket.bak
```

### Restoring a Database

```bash
# Copy backup to container
docker compose cp ./GloboTicket.bak sqlserver:/var/opt/mssql/data/GloboTicket.bak

# Restore the database
docker compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' \
  -Q "RESTORE DATABASE GloboTicket FROM DISK='/var/opt/mssql/data/GloboTicket.bak' WITH REPLACE"
```

### Resetting Everything

```bash
# Stop containers and remove volumes (deletes all data)
docker compose down -v

# Start fresh
docker compose up -d

# Wait for healthy status
docker compose ps

# Re-run initialization script
docker compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'YourStrong!Passw0rd' \
  -i /docker-entrypoint-initdb.d/01-create-users.sql

# Re-run migrations
cd ../../src/GloboTicket.Infrastructure
dotnet ef database update --connection "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=true"
```

## Security Best Practices

1. **Never commit passwords to version control**: Use `.env` files (see `.env.example`)
2. **Rotate passwords regularly**: Change database passwords periodically
3. **Use migration_user sparingly**: Only during deployment windows
4. **Monitor app_user activity**: Log all database operations
5. **Keep SQL Server updated**: Pull latest image regularly
6. **Use TLS in production**: Enable encrypted connections
7. **Restrict network access**: Use firewall rules in production

## Environment Variables

For production or when sharing the project, use environment variables instead of hardcoded passwords:

```bash
# Create .env file from template
cp .env.example .env

# Edit .env with your actual passwords (never commit this file!)
```

Then update `docker-compose.yml` to use environment variables:

```yaml
environment:
  SA_PASSWORD: ${SA_PASSWORD}
```

## Development Workflow

Typical daily workflow for developers:

```bash
# 1. Start infrastructure (once per day)
# Using scripts (recommended):
./scripts/bash/docker-up.sh
# OR
pwsh scripts/powershell/docker-up.ps1

# The script automatically:
# - Starts SQL Server
# - Waits for SQL Server to be healthy
# - Runs database initialization (creates users automatically)
# - Verifies initialization completed

# 2. Run migrations (when schema changes)
./scripts/bash/db-update.sh
# OR
pwsh scripts/powershell/db-update.ps1

# 3. Start API (in another terminal)
./scripts/bash/run-api.sh
# OR
pwsh scripts/powershell/run-api.ps1

# 4. Start frontend (in another terminal, optional)
./scripts/bash/run-web.sh
# OR
pwsh scripts/powershell/run-web.ps1

# 5. Develop and test...

# 6. Stop infrastructure (end of day)
docker compose -f docker/docker-compose.yml down  # Preserves data
# OR to remove volumes and start fresh:
docker compose -f docker/docker-compose.yml down -v
```

**Note:** Database users are automatically created on first startup. No manual initialization is required!

## Additional Resources

- [SQL Server Docker Documentation](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [GloboTicket Architecture Documentation](../docs/architecture.md)

## Support

For issues or questions:
1. Check this README's troubleshooting section
2. Review Docker Compose logs: `docker compose logs`
3. Verify health check status: `docker compose ps`
4. Consult the GloboTicket architecture documentation