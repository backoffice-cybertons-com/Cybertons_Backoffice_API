-- ==========================================================================================
-- CONSOLIDATED PARTY MASTER STORED PROCEDURES (CORRECTED IDENTITY HANDLING)
-- RUN THIS SCRIPT TO ENSURE ALL PARTY-RELATED STORED PROCEDURES ARE UP TO DATE
-- ==========================================================================================

USE [SoftAccDb]
GO

-- 1. sp_Party_Master
IF OBJECT_ID('[dbo].[sp_Party_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Party_Master]
GO

CREATE PROCEDURE [dbo].[sp_Party_Master]
    @Action varchar(20),
    @PartyId int = NULL,
    @PartyType varchar(20) = NULL,
    @Salutation varchar(20) = NULL,
    @FirstName varchar(100) = NULL,
    @LastName varchar(100) = NULL,
    @CompanyName varchar(200) = NULL,
    @DisplayName varchar(200) = NULL,
    @Email varchar(200) = NULL,
    @PhoneWork varchar(20) = NULL,
    @PhoneMobile varchar(20) = NULL,
    @PreferredLanguage varchar(50) = NULL,
    @GSTTreatment varchar(100) = NULL,
    @PlaceOfSupply varchar(100) = NULL,
    @PAN varchar(20) = NULL,
    @GSTNO varchar(40) = NULL,
    @MSMERegistered bit = NULL,
    @TaxPreference varchar(50) = NULL,
    @TDS varchar(100) = NULL,
    @Currency int = NULL,
    @AccountsReceivableAccount int = NULL,
    @AccountsPayableAccount int = NULL,
    @OpeningBalance decimal(18,2) = NULL,
    @OpeningBalanceType varchar(50) = NULL,
    @PaymentTerms varchar(100) = NULL,
    @EnablePortal bit = NULL,
    @WebsiteURL varchar(500) = NULL,
    @Department varchar(100) = NULL,
    @Remarks varchar(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyMaster] 
        ([PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], 
         [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [GSTNO], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId],
         [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms],
         [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn])
        VALUES 
        (@PartyType, @Salutation, @FirstName, @LastName, @CompanyName, @DisplayName, @Email, @PhoneWork, @PhoneMobile,
         @PreferredLanguage, @GSTTreatment, @PlaceOfSupply, @PAN, @GSTNO, @MSMERegistered, @TaxPreference, @TDS, @Currency,
         @AccountsReceivableAccount, @AccountsPayableAccount, @OpeningBalance, @OpeningBalanceType, @PaymentTerms,
         @EnablePortal, @WebsiteURL, @Department, @Remarks, GETDATE())
        
        SELECT SCOPE_IDENTITY() AS PartyId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyMaster]
        SET [PartyType] = @PartyType, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [CompanyName] = @CompanyName, [DisplayName] = @DisplayName, [Email] = @Email, [PhoneWork] = @PhoneWork,
            [PhoneMobile] = @PhoneMobile, [PreferredLanguage] = @PreferredLanguage, [GSTTreatment] = @GSTTreatment,
            [PlaceOfSupply] = @PlaceOfSupply, [PAN] = @PAN, [GSTNO] = @GSTNO, [MSMERegistered] = @MSMERegistered, 
            [TaxPreference] = @TaxPreference, [TDS] = @TDS, [CurrencyId] = @Currency, 
            [AccountsReceivableAccount] = @AccountsReceivableAccount, [AccountsPayableAccount] = @AccountsPayableAccount, 
            [OpeningBalance] = @OpeningBalance, [OpeningBalanceType] = @OpeningBalanceType, [PaymentTerms] = @PaymentTerms, 
            [EnablePortal] = @EnablePortal, [WebsiteURL] = @WebsiteURL, [Department] = @Department, [Remarks] = @Remarks
        WHERE [PartyId] = @PartyId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, CurrencyId AS [Currency] FROM [dbo].[TabPartyMaster] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, CurrencyId AS [Currency] FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId END
    ELSE IF @Action = 'SELECT_BY_TYPE' 
    BEGIN 
        SELECT *, CurrencyId AS [Currency] 
        FROM [dbo].[TabPartyMaster] 
        WHERE [PartyType] = @PartyType OR @PartyType IS NULL
    END
END
GO

-- 2. sp_PartyAddress_Master
IF OBJECT_ID('[dbo].[sp_PartyAddress_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PartyAddress_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyAddress_Master]
    @Action varchar(20),
    @AddressId int = NULL,
    @PartyId int = NULL,
    @AddressType varchar(50) = NULL,
    @Attention varchar(200) = NULL,
    @Country varchar(100) = NULL,
    @Street1 varchar(500) = NULL,
    @Street2 varchar(500) = NULL,
    @City varchar(100) = NULL,
    @State varchar(100) = NULL,
    @PinCode varchar(20) = NULL,
    @Phone varchar(20) = NULL,
    @FaxNumber varchar(20) = NULL,
    @IsDefault bit = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Handle AddressType mapping for INSERT/UPDATE
    DECLARE @ActualAddressTypeId INT = 1; -- Default to Billing
    IF @AddressType = 'Shipping' OR @AddressType = '2' SET @ActualAddressTypeId = 2;
    ELSE IF @AddressType = 'Billing' OR @AddressType = '1' SET @ActualAddressTypeId = 1;

    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyAddress] 
        ([PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [CreatedOn], [IsActive])
        VALUES 
        (@PartyId, @ActualAddressTypeId, @Attention, @Country, @Street1, @Street2, @City, @State, @PinCode, @Phone, @FaxNumber, ISNULL(@IsDefault, 0), GETDATE(), 1)
        
        SELECT SCOPE_IDENTITY() AS AddressId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyAddress]
        SET [PartyId] = @PartyId, [AddressTypeId] = @ActualAddressTypeId, [Attention] = @Attention, [Country] = @Country,
            [AddressLine1] = @Street1, [AddressLine2] = @Street2, [City] = @City, [State] = @State,
            [PinCode] = @PinCode, [Phone] = @Phone, [Fax] = @FaxNumber, [IsDefault] = ISNULL(@IsDefault, 0)
        WHERE [AddressId] = @AddressId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyAddress] WHERE [AddressId] = @AddressId END
    ELSE IF @Action = 'SELECT' 
    BEGIN 
        SELECT *, 
               AddressLine1 AS Street1, 
               AddressLine2 AS Street2, 
               CASE WHEN AddressTypeId = 1 THEN 'Billing' ELSE 'Shipping' END AS AddressType, 
               Fax AS FaxNumber 
        FROM [dbo].[TabPartyAddress] 
    END
    ELSE IF @Action = 'SELECT_BY_ID' 
    BEGIN 
        SELECT *, 
               AddressLine1 AS Street1, 
               AddressLine2 AS Street2, 
               CASE WHEN AddressTypeId = 1 THEN 'Billing' ELSE 'Shipping' END AS AddressType, 
               Fax AS FaxNumber 
        FROM [dbo].[TabPartyAddress] 
        WHERE [AddressId] = @AddressId 
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' 
    BEGIN 
        SELECT *, 
               AddressLine1 AS Street1, 
               AddressLine2 AS Street2, 
               CASE WHEN AddressTypeId = 1 THEN 'Billing' ELSE 'Shipping' END AS AddressType, 
               Fax AS FaxNumber 
        FROM [dbo].[TabPartyAddress] 
        WHERE [PartyId] = @PartyId 
    END
END
GO

-- 3. sp_PartyBankDetails_Master
IF OBJECT_ID('[dbo].[sp_PartyBankDetails_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PartyBankDetails_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyBankDetails_Master]
    @Action varchar(20),
    @BankDetailId int = NULL,
    @PartyId int = NULL,
    @AccountHolderName varchar(200) = NULL,
    @BankName varchar(200) = NULL,
    @AccountNumber varchar(50) = NULL,
    @IFSC varchar(20) = NULL,
    @Branch varchar(200) = NULL,
    @MICR varchar(20) = NULL,
    @IsDefault bit = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyBankDetails] ([PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC])
        VALUES (@PartyId, @AccountHolderName, @BankName, @AccountNumber, @IFSC)
        
        SELECT SCOPE_IDENTITY() AS BankDetailId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyBankDetails]
        SET [PartyId] = @PartyId, [AccountHolderName] = @AccountHolderName, [BankName] = @BankName,
            [AccountNumber] = @AccountNumber, [IFSC] = @IFSC
        WHERE [BankId] = @BankDetailId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyBankDetails] WHERE [BankId] = @BankDetailId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, BankId AS BankDetailId, NULL AS Branch, NULL AS MICR, CAST(0 AS BIT) AS IsDefault, GETDATE() AS CreatedOn FROM [dbo].[TabPartyBankDetails] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, BankId AS BankDetailId, NULL AS Branch, NULL AS MICR, CAST(0 AS BIT) AS IsDefault, GETDATE() AS CreatedOn FROM [dbo].[TabPartyBankDetails] WHERE [BankId] = @BankDetailId END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, BankId AS BankDetailId, NULL AS Branch, NULL AS MICR, CAST(0 AS BIT) AS IsDefault, GETDATE() AS CreatedOn FROM [dbo].[TabPartyBankDetails] WHERE [PartyId] = @PartyId END
END
GO

-- 4. sp_PartyContactPerson_Master
IF OBJECT_ID('[dbo].[sp_PartyContactPerson_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PartyContactPerson_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyContactPerson_Master]
    @Action varchar(20),
    @ContactPersonId int = NULL,
    @PartyId int = NULL,
    @Salutation varchar(20) = NULL,
    @FirstName varchar(100) = NULL,
    @LastName varchar(100) = NULL,
    @EmailAddress varchar(200) = NULL,
    @WorkPhone varchar(20) = NULL,
    @Mobile varchar(20) = NULL,
    @IsPrimary bit = NULL 
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyContactPersons] ([PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile])
        VALUES (@PartyId, @Salutation, @FirstName, @LastName, @EmailAddress, @WorkPhone, @Mobile)
        
        SELECT SCOPE_IDENTITY() AS ContactPersonId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyContactPersons]
        SET [PartyId] = @PartyId, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [Email] = @EmailAddress, [WorkPhone] = @WorkPhone, [Mobile] = @Mobile
        WHERE [ContactPersonId] = @ContactPersonId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, Email AS EmailAddress, CAST(0 AS BIT) AS IsPrimary, GETDATE() AS CreatedOn FROM [dbo].[TabPartyContactPersons] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, Email AS EmailAddress, CAST(0 AS BIT) AS IsPrimary, GETDATE() AS CreatedOn FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, Email AS EmailAddress, CAST(0 AS BIT) AS IsPrimary, GETDATE() AS CreatedOn FROM [dbo].[TabPartyContactPersons] WHERE [PartyId] = @PartyId END
END
GO

-- 5. sp_PartyReportingTag_Master
IF OBJECT_ID('[dbo].[sp_PartyReportingTag_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PartyReportingTag_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyReportingTag_Master]
    @Action varchar(20),
    @PartyReportingTagId int = NULL,
    @PartyId int = NULL,
    @ReportingTagId int = NULL,
    @TagValue varchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyReportingTags] ([PartyId], [TagName])
        VALUES (@PartyId, @TagValue)
        
        SELECT SCOPE_IDENTITY() AS PartyReportingTagId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyReportingTags]
        SET [PartyId] = @PartyId, [TagName] = @TagValue
        WHERE [TagId] = @PartyReportingTagId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyReportingTags] WHERE [TagId] = @PartyReportingTagId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, TagId AS PartyReportingTagId, TagId AS ReportingTagId, TagName AS TagValue, GETDATE() AS CreatedOn FROM [dbo].[TabPartyReportingTags] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, TagId AS PartyReportingTagId, TagId AS ReportingTagId, TagName AS TagValue, GETDATE() AS CreatedOn FROM [dbo].[TabPartyReportingTags] WHERE [TagId] = @PartyReportingTagId END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, TagId AS PartyReportingTagId, TagId AS ReportingTagId, TagName AS TagValue, GETDATE() AS CreatedOn FROM [dbo].[TabPartyReportingTags] WHERE [PartyId] = @PartyId END
END
GO

-- 6. sp_PartyTypeMaster & TabPartyTypeMaster Setup
-- ==========================================================================================

-- Ensure TabPartyTypeMaster table exists
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
        [PartyId] INT NULL,        -- For backward compatibility
        [PartyType] VARCHAR(20) NULL -- For backward compatibility
    );
END
GO

-- Populate default data if empty
IF NOT EXISTS (SELECT 1 FROM [dbo].[TabPartyTypeMaster])
BEGIN
    INSERT INTO [dbo].[TabPartyTypeMaster] ([PartyTypeCode], [PartyTypeName], [IsCustomer], [IsVendor], [SortOrder])
    VALUES 
    ('CUST', 'Customer', 1, 0, 1),
    ('VEND', 'Vendor', 0, 1, 2);
END
GO

-- Create or Update the sp_PartyTypeMaster procedure
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
    SELECT *, CurrencyId AS [Currency] 
    FROM [dbo].[TabPartyMaster] 
    WHERE [PartyType] = @PartyType OR @PartyType IS NULL;
  END
  ELSE IF @Action = 'INSERT'
  BEGIN
    INSERT INTO dbo.TabPartyTypeMaster
    (
      PartyTypeCode, PartyTypeName, Description, IsCustomer, IsVendor, IsActive, SortOrder, CreatedOn
    )
    VALUES
    (
      @PartyTypeCode, @PartyTypeName, @Description, ISNULL(@IsCustomer, 0), ISNULL(@IsVendor, 0), ISNULL(@IsActive, 1), ISNULL(@SortOrder, 0), GETUTCDATE()
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
