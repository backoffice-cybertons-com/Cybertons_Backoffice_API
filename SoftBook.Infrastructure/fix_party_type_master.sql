USE [SoftAccDb]
GO

-- 1. Ensure the table TabPartyTypeMaster exists with the correct schema
IF OBJECT_ID('[dbo].[TabPartyTypeMaster]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TabPartyTypeMaster] (
        [PartyTypeId] INT IDENTITY(1,1) PRIMARY KEY,
        [PartyTypeCode] NVARCHAR(50) NOT NULL,
        [PartyTypeName] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [IsCustomer] BIT DEFAULT 0,
        [IsVendor] BIT DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [SortOrder] INT DEFAULT 0,
        [CreatedOn] DATETIME DEFAULT GETUTCDATE(),
        [PartyId] INT NULL,        -- For backward compatibility with some scripts
        [PartyType] VARCHAR(20) NULL -- For backward compatibility with some scripts
    );
END
GO

-- 2. Populate default data if empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[TabPartyTypeMaster])
BEGIN
    INSERT INTO [dbo].[TabPartyTypeMaster] ([PartyTypeCode], [PartyTypeName], [IsCustomer], [IsVendor], [SortOrder])
    VALUES 
    ('CUST', 'Customer', 1, 0, 1),
    ('VEND', 'Vendor', 0, 1, 2);
END
GO

-- 3. Create or Update the sp_PartyTypeMaster procedure to use correct table name
IF OBJECT_ID('[dbo].[sp_PartyTypeMaster]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PartyTypeMaster]
GO

CREATE PROCEDURE dbo.sp_PartyTypeMaster
  @Action NVARCHAR(50),
  @PartyTypeId INT = NULL,
  @PartyTypeCode NVARCHAR(50) = NULL,
  @PartyTypeName NVARCHAR(200) = NULL,
  @Description NVARCHAR(MAX) = NULL,
  @IsCustomer BIT = NULL,
  @IsVendor BIT = NULL,
  @IsActive BIT = NULL,
  @SortOrder INT = NULL,
  @PartyType NVARCHAR(50) = NULL -- Used for SELECT_BY_TYPE
AS
BEGIN
  SET NOCOUNT ON;

  IF @Action = 'SELECT'
  BEGIN
    SELECT
      PartyTypeId,
      PartyTypeCode,
      PartyTypeName,
      Description,
      IsCustomer,
      IsVendor,
      IsActive,
      SortOrder,
      CreatedOn
    FROM dbo.TabPartyTypeMaster
    WHERE IsActive = 1
    ORDER BY SortOrder, PartyTypeName;
  END
  ELSE IF @Action = 'SELECT_BY_ID'
  BEGIN
    SELECT
      PartyTypeId,
      PartyTypeCode,
      PartyTypeName,
      Description,
      IsCustomer,
      IsVendor,
      IsActive,
      SortOrder,
      CreatedOn
    FROM dbo.TabPartyTypeMaster
    WHERE PartyTypeId = @PartyTypeId;
  END
  ELSE IF @Action = 'SELECT_BY_TYPE'
  BEGIN
    -- This action is used to fetch parties filtering by a type name like 'Vendor' or 'Customer'
    -- Based on sp_Party_Master logic, it returns records from TabPartyMaster
    SELECT *, CurrencyId AS [Currency] 
    FROM [dbo].[TabPartyMaster] 
    WHERE [PartyType] = @PartyType OR @PartyType IS NULL;
  END
  ELSE IF @Action = 'INSERT'
  BEGIN
    INSERT INTO dbo.TabPartyTypeMaster
    (
      PartyTypeCode,
      PartyTypeName,
      Description,
      IsCustomer,
      IsVendor,
      IsActive,
      SortOrder,
      CreatedOn
    )
    VALUES
    (
      @PartyTypeCode,
      @PartyTypeName,
      @Description,
      ISNULL(@IsCustomer, 0),
      ISNULL(@IsVendor, 0),
      ISNULL(@IsActive, 1),
      ISNULL(@SortOrder, 0),
      GETUTCDATE()
    );

    SELECT SCOPE_IDENTITY() AS PartyTypeId;
  END
  ELSE IF @Action = 'UPDATE'
  BEGIN
    UPDATE dbo.TabPartyTypeMaster
    SET
      PartyTypeCode = COALESCE(@PartyTypeCode, PartyTypeCode),
      PartyTypeName = COALESCE(@PartyTypeName, PartyTypeName),
      Description = @Description,
      IsCustomer = COALESCE(@IsCustomer, IsCustomer),
      IsVendor = COALESCE(@IsVendor, IsVendor),
      IsActive = COALESCE(@IsActive, IsActive),
      SortOrder = COALESCE(@SortOrder, SortOrder)
    WHERE PartyTypeId = @PartyTypeId;
  END
  ELSE IF @Action = 'DELETE'
  BEGIN
    DELETE FROM dbo.TabPartyTypeMaster
    WHERE PartyTypeId = @PartyTypeId;
  END
  ELSE
  BEGIN
    RAISERROR('Invalid @Action value supplied to sp_PartyTypeMaster: %s', 16, 1, @Action);
  END
END
GO
