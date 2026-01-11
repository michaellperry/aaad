# Schema Migrations

Additive changes (backward-compatible) and breaking changes (multi-step migrations).

## Additive Changes
```sql
-- Add new optional column
ALTER TABLE Venues 
ADD Description NVARCHAR(1000) NULL;

-- Add new table with foreign keys
CREATE TABLE VenueAmenities (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VenueId INT NOT NULL,
    AmenityType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT FK_VenueAmenities_VenueId 
        FOREIGN KEY (VenueId) REFERENCES Venues(Id)
);
```

## Breaking Changes (Rename Column Example)
```sql
-- Step 1: Add new column
ALTER TABLE Venues ADD NewColumnName NVARCHAR(200) NULL;

-- Step 2: Migrate data
UPDATE Venues SET NewColumnName = OldColumnName;

-- Step 3: Update application code (deploy)

-- Step 4: Make new column NOT NULL
ALTER TABLE Venues ALTER COLUMN NewColumnName NVARCHAR(200) NOT NULL;

-- Step 5: Drop old column
ALTER TABLE Venues DROP COLUMN OldColumnName;
```

Breaking changes require multi-step migrations with application deployments between steps.
