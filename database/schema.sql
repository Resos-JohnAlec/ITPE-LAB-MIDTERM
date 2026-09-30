-- SQL Server / LocalDB. Run within a dedicated database. Non-destructive on repeat runs.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY CLUSTERED,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(254) COLLATE Latin1_General_100_CI_AS NOT NULL,
        CONSTRAINT UQ_Users_Email UNIQUE NONCLUSTERED (Email),
        CONSTRAINT CK_Users_FullName CHECK (LEN(LTRIM(RTRIM(FullName))) BETWEEN 2 AND 100),
        -- SQL checks basic shape; the application enforces the configured exact domain.
        CONSTRAINT CK_Users_Email CHECK (Email LIKE N'%_@_%._%' AND Email NOT LIKE N'% %'
            AND Email = LOWER(Email) COLLATE Latin1_General_100_BIN2)
    );
END;

IF OBJECT_ID(N'dbo.Events', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Events (
        EventId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Events PRIMARY KEY CLUSTERED,
        Title NVARCHAR(150) NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        Location NVARCHAR(150) NOT NULL,
        StartsAt DATETIMEOFFSET(0) NOT NULL,
        Capacity INT NOT NULL,
        CONSTRAINT CK_Events_Title CHECK (LEN(LTRIM(RTRIM(Title))) > 0),
        CONSTRAINT CK_Events_Description CHECK (LEN(LTRIM(RTRIM(Description))) > 0),
        CONSTRAINT CK_Events_Location CHECK (LEN(LTRIM(RTRIM(Location))) > 0),
        CONSTRAINT CK_Events_Capacity CHECK (Capacity BETWEEN 1 AND 10000)
    );
    CREATE NONCLUSTERED INDEX IX_Events_StartsAt ON dbo.Events (StartsAt);
END;

IF OBJECT_ID(N'dbo.Registrations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Registrations (
        RegistrationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Registrations PRIMARY KEY CLUSTERED,
        UserId INT NOT NULL,
        EventId INT NOT NULL,
        RegisteredAt DATETIMEOFFSET(0) NOT NULL
            CONSTRAINT DF_Registrations_RegisteredAt DEFAULT SYSDATETIMEOFFSET(),
        CONSTRAINT FK_Registrations_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId)
            ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT FK_Registrations_Events FOREIGN KEY (EventId) REFERENCES dbo.Events (EventId)
            ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT UQ_Registrations_User_Event UNIQUE NONCLUSTERED (UserId, EventId)
    );
    -- UserId is already the leading key of the unique non-clustered index above.
    CREATE NONCLUSTERED INDEX IX_Registrations_EventId ON dbo.Registrations (EventId)
        INCLUDE (UserId, RegisteredAt);
END;

-- Available seats are derived, not stored. Cross-row capacity is enforced by
-- RegistrationService's locked transaction; CHECK cannot count related rows.
-- NO ACTION preserves attendance history and blocks accidental parent deletion.
COMMIT TRANSACTION;
