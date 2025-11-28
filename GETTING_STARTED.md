# Getting Started with GloboTicket

This guide will help you set up and run the GloboTicket multi-tenant ticketing platform on your local machine.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

### Required

**IMPORTANT:** This project requires .NET 10 SDK. Using an older version will cause installation and runtime errors.

- **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (version 10.0 or later) - **REQUIRED**
  ```bash
  # Verify you have .NET 10
  dotnet --version
  # Must show 10.x.x (e.g., 10.0.100)
  ```
  
  **⚠️ If you see version 9.x or earlier:**
  - Download and install .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
  - After installation, restart your terminal and verify: `dotnet --version`

- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (for SQL Server)
  ```bash
  # Verify installation
  docker --version
  docker compose version
  ```

- **Entity Framework Core Tools** (required for database migrations)
  
  **Note:** EF Core tools must be installed AFTER you have .NET 10 SDK installed.
  
  ```bash
  # Install EF Core tools globally
  dotnet tool install --global dotnet-ef
  
  # Verify installation
  dotnet ef --version
  ```
  
  If you already have an older version installed, update it:
  ```bash
  dotnet tool update --global dotnet-ef
  ```

### Optional (for Frontend Development)
- **[Node.js](https://nodejs.org/)** (version 18.0 or later)
  ```bash
  # Verify installation
  node --version
  npm --version
  ```

### Development Tools (Recommended)
- **Visual Studio Code** with C# Dev Kit extension
- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **JetBrains Rider**

## Choosing Your Development Scripts

This project includes developer scripts to simplify common tasks. Both PowerShell and Bash versions are available with identical functionality.

### Script Options Overview

- **PowerShell scripts** (`scripts/powershell/*.ps1`): Work on Windows, Mac, and Linux (requires PowerShell Core)
- **Bash scripts** (`scripts/bash/*.sh`): Work on Mac, Linux, and Windows (via WSL or Git Bash)

Both sets provide identical functionality - choose the one that works best for your environment.

### Installation Requirements

**PowerShell:**
- Verify installation: `pwsh --version`
- If not installed:
  - Windows: Download from [PowerShell GitHub](https://github.com/PowerShell/PowerShell/releases)
  - Mac: `brew install --cask powershell`
  - Linux: Follow [installation guide](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-linux)

**Bash:**
- Usually pre-installed on Mac/Linux
- Windows: Use Git Bash (included with Git) or WSL

### Usage Examples

**PowerShell:**
```powershell
# From project root
pwsh scripts/powershell/db-update.ps1
pwsh scripts/powershell/db-migrate-add.ps1 -MigrationName "AddEventTable"
```

**Bash:**
```bash
# From project root
./scripts/bash/db-update.sh
./scripts/bash/db-migrate-add.sh AddEventTable
```

For complete script documentation, see [`scripts/README.md`](scripts/README.md).

## 🚀 Step-by-Step Setup

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd aaad
```

### Step 2: Start Docker Infrastructure

The project uses SQL Server 2022 running in Docker with automatic database initialization.

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/docker-up.ps1

# Bash
./scripts/bash/docker-up.sh
```

The script will:
- Start SQL Server container
- Wait for SQL Server to become healthy (30-60 seconds)
- Automatically run database initialization to create users (`migration_user` and `app_user`)
- Verify initialization completed successfully

**Expected output:**
```
✓ SQL Server is healthy!
✓ Database initialization completed successfully!

NAME                    STATUS
globoticket-sqlserver   Up X minutes (healthy)
globoticket-db-init     Exited (0)
```

**Note:** The `db-init` container runs automatically after SQL Server is healthy and exits when initialization is complete. This ensures database users are created automatically for new developers.

### Step 3: Restore Dependencies

```bash
# Restore NuGet packages for entire solution
dotnet restore
```

### Step 4: Apply Database Migrations

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-update.ps1

# Bash
./scripts/bash/db-update.sh
```

This will:
- Create the `GloboTicket` database (if it doesn't exist - it was created by the init container)
- Apply all pending migrations
- Create all database tables

**Note:** Database users (`migration_user` and `app_user`) were already created automatically by the init container in Step 2. The migration scripts now automatically run `dotnet restore` before applying migrations, but you can also run it manually if needed.

**Verify database setup:**
```bash
docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U app_user -P 'YourStrong@Passw0rd' -d GloboTicket -C -Q "SELECT Id, Name, Slug FROM Tenants"
```

### Step 5: Build the Solution

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/build.ps1

# Bash
./scripts/bash/build.sh
```

**Expected output:** `Build succeeded in X.Xs`

### Step 6: Run Tests (Optional but Recommended)

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/test.ps1

# Bash
./scripts/bash/test.sh
```

**Expected results:**
- Unit Tests: 22 passed
- Integration Tests: 10 passed
- **Total: 32 passed, 0 failed**

## 🎯 Running the Application

### Starting the API

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/run-api.ps1

# Bash
./scripts/bash/run-api.sh
```

The API will start on: `http://localhost:5028`

**You should see:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5028
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

Leave this terminal running and open a new terminal for testing.

### Testing the API

The easiest way to test the API is using Swagger UI, which provides an interactive interface for exploring and testing all endpoints. Swagger UI is available in Development environment only.

**Access Swagger UI:**
Open your browser and navigate to `http://localhost:5028/swagger`. You'll see a visual interface listing all available API endpoints. Each endpoint can be expanded to view its details, parameters, and response schemas.

#### 1. Health Check

1. Find the `GET /health` endpoint in Swagger UI
2. Click on it to expand the details
3. Click the **"Try it out"** button
4. Click **"Execute"**
5. Review the response in the **"Responses"** section

**Expected response:**
```json
{
  "status": "healthy",
  "timestamp": "2025-11-22T..."
}
```

#### 2. Login (Authenticate)

1. Find the `POST /auth/login` endpoint
2. Click to expand and then click **"Try it out"**
3. In the **Request body** field, enter:
   ```json
   {
     "username": "prod",
     "password": "prod123"
   }
   ```
4. Click **"Execute"**
5. The browser will automatically store the authentication cookie for subsequent requests

**Expected response:**
```json
{
  "username": "prod",
  "tenantId": 1,
  "message": "Login successful"
}
```

**Note:** After logging in, the authentication cookie is automatically included in all subsequent requests made through Swagger UI. This allows you to test protected endpoints without additional authentication steps.

#### 3. Get All Tenants (Authenticated Request)

1. Find the `GET /api/tenants` endpoint
2. Click to expand and then click **"Try it out"**
3. Click **"Execute"**
4. The request will automatically include the authentication cookie from the previous login

**Expected response:**
```json
[
  {
    "id": 1,
    "name": "Production",
    "slug": "production",
    "isActive": true,
    "createdAt": "2025-11-22T..."
  },
  {
    "id": 2,
    "name": "Smoke Test",
    "slug": "smoke-test",
    "isActive": true,
    "createdAt": "2025-11-22T..."
  }
]
```

#### 4. Get Tenant by ID

1. Find the `GET /api/tenants/{id}` endpoint
2. Click to expand and then click **"Try it out"**
3. Enter `1` in the **id** parameter field
4. Click **"Execute"**

**Expected response:**
```json
{
  "id": 1,
  "name": "Production",
  "slug": "production",
  "isActive": true,
  "createdAt": "2025-11-22T..."
}
```

#### 5. Get Current User

1. Find the `GET /auth/me` endpoint
2. Click to expand and then click **"Try it out"**
3. Click **"Execute"**

**Expected response:**
```json
{
  "username": "prod",
  "tenantId": 1,
  "isAuthenticated": true
}
```

#### 6. Logout

1. Find the `POST /auth/logout` endpoint
2. Click to expand and then click **"Try it out"**
3. Click **"Execute"**

**Expected response:**
```json
{
  "message": "Logout successful"
}
```

#### 7. Verify Logout (Should Fail)

After logging out, try accessing a protected endpoint:

1. Find the `GET /api/tenants` endpoint again
2. Click **"Try it out"** and then **"Execute"**
3. You should receive a **401 Unauthorized** response

**Expected response:** HTTP 401 Unauthorized

**Tip:** You can test with different users by logging in again with different credentials:
- `{"username":"smoke","password":"smoke123"}` for the Smoke Test tenant (ID: 2)

**Note:** Swagger UI is only available when running in Development environment. In Production, the Swagger UI endpoints are disabled for security.

**Alternative: Command Line Testing**

If you prefer using `curl` for testing, you can still use the command-line approach. The API endpoints work the same way regardless of how you call them.

### Starting the Frontend (Optional)

The React frontend is configured but requires implementation of components.

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/run-web.ps1

# Bash
./scripts/bash/run-web.sh
```

The frontend will be available at: `http://localhost:5173`

## 🧪 Running Tests

### Run All Tests

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/test.ps1

# Bash
./scripts/bash/test.sh
```

### Run Unit Tests Only

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/test-unit.ps1

# Bash
./scripts/bash/test-unit.sh
```

### Run Integration Tests Only

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/test-integration.ps1

# Bash
./scripts/bash/test-integration.sh
```

### Run Tests with Coverage (Optional)
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Common Development Tasks

### Create a New Migration

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-migrate-add.ps1 -MigrationName "MigrationName"

# Bash
./scripts/bash/db-migrate-add.sh MigrationName
```

### Apply Migrations

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-update.ps1

# Bash
./scripts/bash/db-update.sh
```

### List Migrations

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-migrate-list.ps1

# Bash
./scripts/bash/db-migrate-list.sh
```

### Remove Last Migration

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-migrate-remove.ps1

# Bash
./scripts/bash/db-migrate-remove.sh
```

**Note:** This only works if the migration hasn't been applied to any database. If it has been applied, use `db-migrate-rollback` first.

### Rollback to a Previous Migration

**Using scripts (recommended):**
```bash
# PowerShell - rollback to specific migration
pwsh scripts/powershell/db-migrate-rollback.ps1 -TargetMigration "PreviousMigrationName"

# PowerShell - rollback all migrations
pwsh scripts/powershell/db-migrate-rollback.ps1 -TargetMigration "0"

# Bash - rollback to specific migration
./scripts/bash/db-migrate-rollback.sh PreviousMigrationName

# Bash - rollback all migrations
./scripts/bash/db-migrate-rollback.sh 0
```

### Delete a Migration (if not applied)

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-migrate-delete.ps1 -MigrationName "MigrationName"

# Bash
./scripts/bash/db-migrate-delete.sh MigrationName
```

**Note:** The script automatically checks if the migration is applied and prevents deletion if it is.

### Reset Database

**Using scripts (recommended):**
```bash
# PowerShell
pwsh scripts/powershell/db-reset.ps1

# Bash
./scripts/bash/db-reset.sh
```

**Warning:** This will delete all data in the database!

### View Connection String

The connection string is in [`src/GloboTicket.API/appsettings.json`](src/GloboTicket.API/appsettings.json):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=GloboTicket;User Id=app_user;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
}
```

## 🔍 Troubleshooting

### Issue: Docker Container Won't Start

**Symptoms:**
```
ERROR: Cannot start service sqlserver: driver failed programming external connectivity
```

**Solutions:**
1. Check if port 1433 is already in use:
   ```bash
   # macOS/Linux
   lsof -i :1433
   
   # Windows
   netstat -ano | findstr :1433
   ```

2. Stop any existing SQL Server instances or change the port in `docker/docker-compose.yml`

3. Restart Docker Desktop

### Issue: Database Connection Failed

**Symptoms:**
```
A network-related or instance-specific error occurred while establishing a connection to SQL Server
```

**Solutions:**
1. Verify Docker container is healthy:
   ```bash
   docker compose -f docker/docker-compose.yml ps
   ```

2. Wait for the container to become healthy (30-60 seconds after startup)

3. Verify database initialization completed:
   ```bash
   docker compose -f docker/docker-compose.yml ps db-init
   ```
   The `db-init` container should show status `Exited (0)`. If it shows a non-zero exit code, check the logs:
   ```bash
   docker compose -f docker/docker-compose.yml logs db-init
   ```

4. Test connection manually:
   ```bash
   docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U app_user -P 'YourStrong@Passw0rd' -C -Q "SELECT @@VERSION"
   ```

### Issue: Database Initialization Failed

**Symptoms:**
- `db-init` container shows non-zero exit code
- Cannot login with `migration_user` or `app_user`

**Solutions:**
1. Check initialization logs:
   ```bash
   docker compose -f docker/docker-compose.yml logs db-init
   ```

2. Verify SQL Server is healthy before init runs:
   ```bash
   docker compose -f docker/docker-compose.yml ps sqlserver
   ```

3. Manually run initialization (if needed):
   ```bash
   docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
     -S localhost -U sa -P 'YourStrong!Passw0rd' -C \
     -i /init-db/01-create-users.sql
   ```

4. Test fresh setup with volume recreation:
   ```bash
   # Bash
   ./scripts/bash/docker-test-reset.sh
   
   # PowerShell
   pwsh scripts/powershell/docker-test-reset.ps1
   ```

### Issue: Build Failed - .NET SDK Does Not Support .NET 10.0

**Symptoms:**
```
error NETSDK1045: The current .NET SDK does not support targeting .NET 10.0.
Either target .NET 9.0 or lower, or use a version of the .NET SDK that supports .NET 10.0.
```

**Root Cause:** You have an older .NET SDK installed (e.g., .NET 9 or .NET 8), but this project requires .NET 10.

**Solution:**
1. Download and install .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
2. Restart your terminal
3. Verify installation: `dotnet --version` should show 10.x.x
4. Try building again: `dotnet build`

### Issue: EF Core Tools Not Found or Installation Failed

**Symptoms:**
```
Could not execute because the specified command or file was not found.
```
OR
```
Tool 'dotnet-ef' failed to update due to the following:
The settings file in the tool's NuGet package is invalid
```

**Root Cause:** This usually means you don't have .NET 10 SDK installed, or the EF Core tools version doesn't match your SDK version.

**Solutions:**

1. **First, verify your .NET SDK version:**
   ```bash
   dotnet --version
   ```
   
   **If the version is NOT 10.x.x (e.g., if it shows 9.x.x or 8.x.x):**
   - This project requires .NET 10 SDK
   - Download and install .NET 10 SDK from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
   - After installation, restart your terminal
   - Verify again: `dotnet --version` should show 10.x.x

2. **After confirming you have .NET 10 SDK, install EF Core tools:**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. **If you already have EF Core tools installed but for an older .NET version:**
   ```bash
   dotnet tool update --global dotnet-ef
   ```

4. **Verify the installation:**
   ```bash
   dotnet ef --version
   ```

5. **If the tool is installed but not found in PATH:**
   - Ensure your PATH includes the .NET tools directory:
     - **Linux/macOS**: `~/.dotnet/tools`
     - **Windows**: `%USERPROFILE%\.dotnet\tools`
   - Restart your terminal after installation

### Issue: Migration Failed

**Symptoms:**
```
Build failed. Use dotnet build to see the errors.
```

**Solutions:**
1. Build the solution first:
   ```bash
   dotnet build
   ```

2. Check for compilation errors

3. Ensure you're in the project root directory

### Issue: Tests Failing

**Symptoms:**
```
Test run failed. Multiple tests failed.
```

**Solutions:**
1. Ensure Docker is running (integration tests need it)

2. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   dotnet test
   ```

3. Check if ports are available (1433 for SQL Server)

### Issue: API Returns 401 Unauthorized

**Symptoms:**
API endpoints return 401 even after login.

**Solutions:**
1. Ensure you're using cookies:
   ```bash
   # Login first
   curl -X POST http://localhost:5028/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"prod","password":"prod123"}' \
     -c cookies.txt
   
   # Then use cookie in subsequent requests
   curl http://localhost:5028/api/tenants -b cookies.txt
   ```

2. Check if the cookie file was created: `ls -la cookies.txt`

3. Verify credentials match those in [`UserConfiguration.cs`](src/GloboTicket.API/Configuration/UserConfiguration.cs)

### Issue: Port Already in Use

**Symptoms:**
```
Unable to bind to http://localhost:5028 on the IPv4 loopback interface: 'Address already in use'.
```

**Solutions:**
1. Stop the other process using the port

2. Change the port in [`launchSettings.json`](src/GloboTicket.API/Properties/launchSettings.json)

3. Use a different profile:
   ```bash
   dotnet run --launch-profile https
   ```

## 🎓 Understanding the Test Credentials

The system includes pre-configured test users for development:

| Username | Password  | Tenant         | Tenant ID |
|----------|-----------|----------------|-----------|
| `prod`   | `prod123` | Production     | 1         |
| `smoke`  | `smoke123`| Smoke Test     | 2         |

These are configured in [`src/GloboTicket.API/Configuration/UserConfiguration.cs`](src/GloboTicket.API/Configuration/UserConfiguration.cs).

**Note:** These tenants exist within the same environment's database. In a production environment, you can use the "Smoke Test" tenant to run post-deployment validation tests without affecting the "Production" tenant's data.

**⚠️ Important:** These test credentials are for development only. Do not use in production.

## 📚 Next Steps

Now that you have GloboTicket running, here are some suggested next steps:

### 1. Explore the Codebase
- Review [`docs/architecture.md`](docs/architecture.md) for system design details
- Examine the Clean Architecture layers
- Study the multi-tenancy implementation

### 2. Understand Multi-Tenancy
- Test with different users (prod vs smoke)
- Observe tenant isolation in action
- Review [`TenantResolutionMiddleware.cs`](src/GloboTicket.API/Middleware/TenantResolutionMiddleware.cs)

### 3. Review Tests
- Run and examine unit tests in `tests/GloboTicket.UnitTests`
- Study integration tests in `tests/GloboTicket.IntegrationTests`
- Add your own tests for new features

### 4. Implement Domain Features
Start implementing domain-specific features:
- Event management (Event entity, EventService)
- Ticket management (Ticket entity, inventory)
- Order processing (Order entity, payment flow)
- Customer management (Customer entity)

### 5. Extend the API
- Add new endpoints in `src/GloboTicket.API/Endpoints`
- Implement additional services
- Add validation and error handling

### 6. Build the Frontend
- Implement React components in `src/GloboTicket.Web/src`
- Connect to API endpoints
- Add authentication flow
- Build UI for events and tickets

## 🔗 Additional Resources

- **[README.md](README.md)** - Project overview and architecture
- **[Architecture Documentation](docs/architecture.md)** - Detailed system design
- **[Product Requirements](docs/prd.md)** - Business requirements
- **[Docker Setup](docker/README.md)** - Container configuration
- **[Database Guide](database/README.md)** - Migration and schema information
- **[Verification Checklist](VERIFICATION.md)** - System verification results

## 💡 Tips for Success

1. **Start with tests** - Run tests frequently to catch issues early
2. **Follow Clean Architecture** - Keep dependencies flowing inward
3. **Respect tenant boundaries** - Never bypass tenant isolation
4. **Use the debugger** - Set breakpoints and step through code
5. **Read the logs** - API logs show tenant resolution and SQL queries
6. **Commit often** - Small, focused commits are easier to review

## 🆘 Getting Help

If you encounter issues not covered in this guide:

1. Check the troubleshooting section above
2. Review the logs in the terminal where the API is running
3. Examine Docker logs: `docker compose -f docker/docker-compose.yml logs`
4. Verify all prerequisites are correctly installed
5. Ensure you're in the correct directory for each command

---

**Happy coding!** 🚀 You're now ready to develop with GloboTicket.