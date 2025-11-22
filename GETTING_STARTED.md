# Getting Started with GloboTicket

This guide will help you set up and run the GloboTicket multi-tenant ticketing platform on your local machine.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

### Required
- **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (version 10.0 or later)
  ```bash
  # Verify installation
  dotnet --version
  ```

- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (for SQL Server)
  ```bash
  # Verify installation
  docker --version
  docker compose version
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

## 🚀 Step-by-Step Setup

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd aaad
```

### Step 2: Start Docker Infrastructure

The project uses SQL Server 2022 running in Docker.

```bash
# Navigate to docker directory
cd docker

# Start SQL Server container
docker compose up -d

# Verify container is running and healthy
docker compose ps
```

**Expected output:**
```
NAME                    STATUS
globoticket-sqlserver   Up X minutes (healthy)
```

**Wait for the container to become healthy** (indicated by the "healthy" status). This may take 30-60 seconds.

### Step 3: Return to Project Root

```bash
cd ..
```

### Step 4: Restore Dependencies

```bash
# Restore NuGet packages for entire solution
dotnet restore
```

### Step 5: Apply Database Migrations

```bash
# Run EF Core migrations to create database schema
dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
```

This will:
- Create the `GloboTicket` database
- Create the `Tenants` table
- Seed initial tenant data (ACME Corp and TechStart Inc)

**Verify database setup:**
```bash
docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U app_user -P 'YourStrong@Passw0rd' -d GloboTicket -C -Q "SELECT Id, Name, Slug FROM Tenants"
```

### Step 6: Build the Solution

```bash
# Build all projects
dotnet build GloboTicket.sln
```

**Expected output:** `Build succeeded in X.Xs`

### Step 7: Run Tests (Optional but Recommended)

```bash
# Run all tests
dotnet test --verbosity normal
```

**Expected results:**
- Unit Tests: 22 passed
- Integration Tests: 10 passed
- **Total: 32 passed, 0 failed**

## 🎯 Running the Application

### Starting the API

```bash
cd src/GloboTicket.API
dotnet run
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

#### 1. Health Check
```bash
curl http://localhost:5028/health
```

**Expected response:**
```json
{"status":"healthy","timestamp":"2025-11-22T..."}
```

#### 2. Login (Get Authentication Cookie)
```bash
curl -X POST http://localhost:5028/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' \
  -c cookies.txt -v
```

**Expected response:**
```json
{"username":"admin","tenantId":1,"message":"Login successful"}
```

#### 3. Get Tenants (Authenticated Request)
```bash
curl http://localhost:5028/api/tenants -b cookies.txt
```

**Expected response:**
```json
[
  {
    "id": 1,
    "name": "ACME Corp",
    "slug": "acme",
    "isActive": true,
    "createdAt": "2025-11-22T..."
  },
  {
    "id": 2,
    "name": "TechStart Inc",
    "slug": "techstart",
    "isActive": true,
    "createdAt": "2025-11-22T..."
  }
]
```

#### 4. Logout
```bash
curl -X POST http://localhost:5028/auth/logout -b cookies.txt
```

**Expected response:**
```json
{"message":"Logout successful"}
```

#### 5. Verify Logout (Should Fail)
```bash
curl http://localhost:5028/api/tenants -b cookies.txt
```

**Expected response:** HTTP 401 Unauthorized

### Starting the Frontend (Optional)

The React frontend is configured but requires implementation of components.

```bash
# Navigate to frontend directory
cd src/GloboTicket.Web

# Install dependencies (first time only)
npm install

# Start development server
npm run dev
```

The frontend will be available at: `http://localhost:5173`

## 🧪 Running Tests

### Run All Tests
```bash
dotnet test --verbosity normal
```

### Run Unit Tests Only
```bash
dotnet test tests/GloboTicket.UnitTests --verbosity normal
```

### Run Integration Tests Only
```bash
dotnet test tests/GloboTicket.IntegrationTests --verbosity normal
```

### Run Tests with Coverage (Optional)
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Common Development Tasks

### Create a New Migration

```bash
dotnet ef migrations add MigrationName \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

### Apply Migrations

```bash
dotnet ef database update \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

### Remove Last Migration

```bash
dotnet ef migrations remove \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

### Reset Database

```bash
# Drop database
dotnet ef database drop --force \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API

# Recreate with migrations
dotnet ef database update \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API
```

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

3. Test connection manually:
   ```bash
   docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U app_user -P 'YourStrong@Passw0rd' -C -Q "SELECT @@VERSION"
   ```

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
     -d '{"username":"admin","password":"admin123"}' \
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
| `admin`  | `admin123`| ACME Corp      | 1         |
| `user`   | `user123` | TechStart Inc  | 2         |

These are configured in [`src/GloboTicket.API/Configuration/UserConfiguration.cs`](src/GloboTicket.API/Configuration/UserConfiguration.cs).

**⚠️ Important:** These test credentials are for development only. Do not use in production.

## 📚 Next Steps

Now that you have GloboTicket running, here are some suggested next steps:

### 1. Explore the Codebase
- Review [`docs/architecture.md`](docs/architecture.md) for system design details
- Examine the Clean Architecture layers
- Study the multi-tenancy implementation

### 2. Understand Multi-Tenancy
- Test with different users (admin vs user)
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