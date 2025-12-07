#!/bin/bash
# Database initialization script for Docker init container
# Waits for SQL Server to be ready and runs user initialization

set -e

SA_PASSWORD="${SA_PASSWORD:-YourStrong!Passw0rd}"
SQL_SERVER="${SQL_SERVER:-sqlserver}"
MAX_RETRIES=30
RETRY_INTERVAL=2

echo "Waiting for SQL Server to be ready..."

# Wait for SQL Server to accept connections
for i in $(seq 1 $MAX_RETRIES); do
    if /opt/mssql-tools18/bin/sqlcmd \
        -S "$SQL_SERVER" \
        -U sa \
        -P "$SA_PASSWORD" \
        -C \
        -Q "SELECT 1" \
        > /dev/null 2>&1; then
        echo "✓ SQL Server is ready!"
        break
    fi
    
    if [ $i -eq $MAX_RETRIES ]; then
        echo "✗ SQL Server failed to become ready after $((MAX_RETRIES * RETRY_INTERVAL)) seconds"
        exit 1
    fi
    
    echo "  Waiting for SQL Server... (attempt $i/$MAX_RETRIES)"
    sleep $RETRY_INTERVAL
done

echo "Running database initialization script..."

# Step 1: Create database if it doesn't exist
echo "Creating GloboTicket database (if needed)..."
/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GloboTicket') CREATE DATABASE GloboTicket;" \
    > /dev/null 2>&1

# Step 2: Create migration_user login
echo "Creating migration_user login..."
/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'migration_user') CREATE LOGIN migration_user WITH PASSWORD = 'Migration@Pass123';" \
    > /dev/null 2>&1

# Step 3: Grant dbcreator server role to migration_user (required for CREATE DATABASE)
echo "Granting dbcreator server role to migration_user..."
/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF IS_SRVROLEMEMBER('dbcreator', 'migration_user') = 0 ALTER SERVER ROLE dbcreator ADD MEMBER migration_user;" \
    > /dev/null 2>&1

# Step 4: Create migration_user database user
echo "Creating migration_user database user..."
/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -d GloboTicket \
    -Q "IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'migration_user') CREATE USER migration_user FOR LOGIN migration_user; ALTER ROLE db_owner ADD MEMBER migration_user;" \
    > /dev/null 2>&1

# Step 5: Create app_user login
echo "Creating app_user login..."
/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'app_user') CREATE LOGIN app_user WITH PASSWORD = 'YourStrong@Passw0rd';" \
    > /dev/null 2>&1

# Step 6: Create app_user database user
echo "Creating app_user database user..."
/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -d GloboTicket \
    -Q "IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'app_user') CREATE USER app_user FOR LOGIN app_user; ALTER ROLE db_datareader ADD MEMBER app_user; ALTER ROLE db_datawriter ADD MEMBER app_user;" \
    > /dev/null 2>&1

# Verify users were created
echo "Verifying users were created..."

# Check if migration_user login exists
MIGRATION_USER_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'migration_user') SELECT 'EXISTS' ELSE SELECT 'NOT_EXISTS'" \
    -h -1 \
    -W 2>/dev/null | grep -i "EXISTS" | head -1 | tr -d '[:space:]' | cut -d'(' -f1)

# Check if app_user login exists
APP_USER_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'app_user') SELECT 'EXISTS' ELSE SELECT 'NOT_EXISTS'" \
    -h -1 \
    -W 2>/dev/null | grep -i "EXISTS" | head -1 | tr -d '[:space:]' | cut -d'(' -f1)

# Check if database exists
DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd \
    -S "$SQL_SERVER" \
    -U sa \
    -P "$SA_PASSWORD" \
    -C \
    -Q "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'GloboTicket') SELECT 'EXISTS' ELSE SELECT 'NOT_EXISTS'" \
    -h -1 \
    -W 2>/dev/null | grep -i "EXISTS" | head -1 | tr -d '[:space:]' | cut -d'(' -f1)

if [ "$MIGRATION_USER_EXISTS" = "EXISTS" ] && [ "$APP_USER_EXISTS" = "EXISTS" ] && [ "$DB_EXISTS" = "EXISTS" ]; then
    echo "✓ Database initialization completed successfully!"
    echo "✓ GloboTicket database created"
    echo "✓ migration_user created"
    echo "✓ app_user created"
    exit 0
else
    echo "✗ Database initialization failed!"
    echo "  Database: $DB_EXISTS"
    echo "  migration_user: $MIGRATION_USER_EXISTS"
    echo "  app_user: $APP_USER_EXISTS"
    exit 1
fi

