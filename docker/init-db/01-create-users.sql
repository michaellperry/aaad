-- =====================================================================================
-- GloboTicket Database Initialization Script
-- =====================================================================================
-- This script implements the Two-User Security Model for GloboTicket:
--
-- 1. MIGRATION USER (migration_user)
--    - Purpose: Execute database migrations and schema changes
--    - Permissions: Full DDL (CREATE, ALTER, DROP) and DML on migration history
--    - Server Role: dbcreator (allows creating databases)
--    - Database Role: db_owner (full access to GloboTicket database)
--    - Usage: EF Core migration bundle execution only
--    - Security: Should only be used during deployment/migration windows
--
-- 2. APPLICATION USER (app_user)  
--    - Purpose: Runtime data operations for the API
--    - Permissions: DML only (SELECT, INSERT, UPDATE, DELETE) on application tables
--    - Usage: Normal API operations via connection string
--    - Security: Cannot modify schema, follows principle of least privilege
--
-- This separation provides defense-in-depth: even if the application is compromised,
-- attackers cannot modify the database schema or access system tables.
-- =====================================================================================

USE master;
GO

-- =====================================================================================
-- MIGRATION USER SETUP
-- =====================================================================================
-- This user has elevated privileges needed for EF Core migrations

PRINT 'Creating migration_user login and user...';

-- Create server login for migration user
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'migration_user')
BEGIN
    CREATE LOGIN migration_user WITH PASSWORD = 'Migration@Pass123';
    PRINT '✓ migration_user login created';
END
ELSE
BEGIN
    PRINT '⚠ migration_user login already exists';
END

-- Grant dbcreator server role to migration_user
-- This allows EF Core to create databases when they don't exist
IF IS_SRVROLEMEMBER('dbcreator', 'migration_user') = 0
BEGIN
    ALTER SERVER ROLE dbcreator ADD MEMBER migration_user;
    PRINT '✓ migration_user granted dbcreator server role (required for database creation)';
END
ELSE
BEGIN
    PRINT '⚠ migration_user already has dbcreator server role';
END

-- Switch to the GloboTicket database (will be created by first migration)
-- Note: This script runs after database creation, so we need to handle both scenarios

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GloboTicket')
BEGIN
    PRINT 'Creating GloboTicket database...';
    CREATE DATABASE GloboTicket;
    PRINT '✓ GloboTicket database created';
END
ELSE
BEGIN
    PRINT '⚠ GloboTicket database already exists';
END

USE GloboTicket;
GO

-- Create database user for migration login
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'migration_user')
BEGIN
    CREATE USER migration_user FOR LOGIN migration_user;
    PRINT '✓ migration_user database user created';
END
ELSE
BEGIN
    PRINT '⚠ migration_user database user already exists';
END

-- Grant db_owner role to migration user for full DDL/DML access
-- This allows EF Core migrations to create/modify schema and maintain migration history
ALTER ROLE db_owner ADD MEMBER migration_user;
PRINT '✓ migration_user granted db_owner role (required for EF Core migrations)';

GO

-- =====================================================================================
-- APPLICATION USER SETUP
-- =====================================================================================
-- This user has restricted permissions for normal runtime operations

PRINT 'Creating app_user login and user...';

-- Create server login for application user
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'app_user')
BEGIN
    CREATE LOGIN app_user WITH PASSWORD = 'YourStrong@Passw0rd';
    PRINT '✓ app_user login created';
END
ELSE
BEGIN
    PRINT '⚠ app_user login already exists';
END

-- Create database user for application login
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'app_user')
BEGIN
    CREATE USER app_user FOR LOGIN app_user;
    PRINT '✓ app_user database user created';
END
ELSE
BEGIN
    PRINT '⚠ app_user database user already exists';
END

-- Grant data reader and writer roles for basic DML operations
ALTER ROLE db_datareader ADD MEMBER app_user;
ALTER ROLE db_datawriter ADD MEMBER app_user;
PRINT '✓ app_user granted db_datareader and db_datawriter roles';

-- NOTE: After migrations create the schema, app_user will automatically have
-- SELECT, INSERT, UPDATE, DELETE permissions on all tables via db_datareader/db_datawriter
-- 
-- For explicit schema-level permissions (more restrictive approach), you could instead use:
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO app_user;
--
-- app_user explicitly CANNOT:
-- - Execute DDL operations (CREATE, ALTER, DROP)
-- - Access system tables or views (except what's needed for basic queries)
-- - Modify security settings
-- - Execute stored procedures unless explicitly granted

GO

PRINT '=====================================================================================';
PRINT 'Database initialization complete!';
PRINT '';
PRINT 'Users created:';
PRINT '  • migration_user - Use for EF Core migrations (dbcreator server role, db_owner database role)';
PRINT '  • app_user       - Use for API runtime operations (Restricted privileges)';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Run EF Core migrations using migration_user connection string';
PRINT '  2. Configure API to use app_user connection string';
PRINT '  3. Verify app_user cannot modify schema (security validation)';
PRINT '=====================================================================================';
GO