USE [SoftAccDb]
GO

-- 1. Fix TabPartyAddress
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyAddress]') AND name = 'IsActive')
    ALTER TABLE [dbo].[TabPartyAddress] ADD [IsActive] BIT NULL DEFAULT 1;
GO

-- 2. Fix TabPartyBankDetails
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyBankDetails]') AND name = 'Branch')
    ALTER TABLE [dbo].[TabPartyBankDetails] ADD [Branch] VARCHAR(200) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyBankDetails]') AND name = 'MICR')
    ALTER TABLE [dbo].[TabPartyBankDetails] ADD [MICR] VARCHAR(20) NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyBankDetails]') AND name = 'IsDefault')
    ALTER TABLE [dbo].[TabPartyBankDetails] ADD [IsDefault] BIT NULL DEFAULT 0;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyBankDetails]') AND name = 'CreatedOn')
    ALTER TABLE [dbo].[TabPartyBankDetails] ADD [CreatedOn] DATETIME NULL DEFAULT GETDATE();
GO

-- 3. Fix TabPartyContactPersons
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyContactPersons]') AND name = 'IsPrimary')
    ALTER TABLE [dbo].[TabPartyContactPersons] ADD [IsPrimary] BIT NULL DEFAULT 0;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyContactPersons]') AND name = 'CreatedOn')
    ALTER TABLE [dbo].[TabPartyContactPersons] ADD [CreatedOn] DATETIME NULL DEFAULT GETDATE();
GO

-- 4. Fix TabPartyReportingTags
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyReportingTags]') AND name = 'ReportingTagId')
    ALTER TABLE [dbo].[TabPartyReportingTags] ADD [ReportingTagId] INT NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyReportingTags]') AND name = 'CreatedOn')
    ALTER TABLE [dbo].[TabPartyReportingTags] ADD [CreatedOn] DATETIME NULL DEFAULT GETDATE();
GO

-- 5. Fix TabPartyTypeMaster
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyTypeMaster]') AND name = 'PartyId')
    ALTER TABLE [dbo].[TabPartyTypeMaster] ADD [PartyId] INT NULL;
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyTypeMaster]') AND name = 'PartyType')
    ALTER TABLE [dbo].[TabPartyTypeMaster] ADD [PartyType] VARCHAR(20) NULL;
GO

-- 6. Update Stored Procedures

-- sp_PartyAddress_Master
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
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyAddress] 
        ([PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [CreatedOn])
        VALUES 
        (@PartyId, 1, @Attention, @Country, @Street1, @Street2, @City, @State, @PinCode, @Phone, @FaxNumber, @IsDefault, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyAddress]
        SET [PartyId] = @PartyId, [Attention] = @Attention, [Country] = @Country,
            [AddressLine1] = @Street1, [AddressLine2] = @Street2, [City] = @City, [State] = @State,
            [PinCode] = @PinCode, [Phone] = @Phone, [Fax] = @FaxNumber, [IsDefault] = @IsDefault
        WHERE [AddressId] = @AddressId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyAddress] WHERE [AddressId] = @AddressId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, AddressLine1 AS [Street1], AddressLine2 AS [Street2], Fax AS [FaxNumber] FROM [dbo].[TabPartyAddress] END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, AddressLine1 AS [Street1], AddressLine2 AS [Street2], Fax AS [FaxNumber] FROM [dbo].[TabPartyAddress] WHERE [PartyId] = @PartyId END
END
GO

-- sp_PartyBankDetails_Master
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
        INSERT INTO [dbo].[TabPartyBankDetails] 
        ([PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn])
        VALUES 
        (@PartyId, @AccountHolderName, @BankName, @AccountNumber, @IFSC, @Branch, @MICR, @IsDefault, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyBankDetails]
        SET [PartyId] = @PartyId, [AccountHolderName] = @AccountHolderName, [BankName] = @BankName,
            [AccountNumber] = @AccountNumber, [IFSC] = @IFSC, [Branch] = @Branch, [MICR] = @MICR, [IsDefault] = @IsDefault
        WHERE [BankId] = @BankDetailId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyBankDetails] WHERE [BankId] = @BankDetailId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, BankId AS [BankDetailId] FROM [dbo].[TabPartyBankDetails] END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, BankId AS [BankDetailId] FROM [dbo].[TabPartyBankDetails] WHERE [PartyId] = @PartyId END
END
GO

-- sp_PartyContactPerson_Master
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
        INSERT INTO [dbo].[TabPartyContactPersons] 
        ([PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn])
        VALUES 
        (@PartyId, @Salutation, @FirstName, @LastName, @EmailAddress, @WorkPhone, @Mobile, @IsPrimary, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyContactPersons]
        SET [PartyId] = @PartyId, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [Email] = @EmailAddress, [WorkPhone] = @WorkPhone, [Mobile] = @Mobile, [IsPrimary] = @IsPrimary
        WHERE [ContactPersonId] = @ContactPersonId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, Email AS [EmailAddress] FROM [dbo].[TabPartyContactPersons] END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, Email AS [EmailAddress] FROM [dbo].[TabPartyContactPersons] WHERE [PartyId] = @PartyId END
END
GO

-- sp_PartyReportingTag_Master
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
        INSERT INTO [dbo].[TabPartyReportingTags] 
        ([PartyId], [ReportingTagId], [TagName], [CreatedOn])
        VALUES 
        (@PartyId, @ReportingTagId, @TagValue, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyReportingTags]
        SET [PartyId] = @PartyId, [ReportingTagId] = @ReportingTagId, [TagName] = @TagValue
        WHERE [TagId] = @PartyReportingTagId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyReportingTags] WHERE [TagId] = @PartyReportingTagId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, TagId AS [PartyReportingTagId], TagName AS [TagValue] FROM [dbo].[TabPartyReportingTags] END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, TagId AS [PartyReportingTagId], TagName AS [TagValue] FROM [dbo].[TabPartyReportingTags] WHERE [PartyId] = @PartyId END
END
GO

-- sp_PartyTypeMaster_Master
IF OBJECT_ID('[dbo].[sp_PartyTypeMaster_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PartyTypeMaster_Master]
GO
CREATE PROCEDURE [dbo].[sp_PartyTypeMaster_Master]
    @Action varchar(20),
    @PartyTypeId int = NULL,
    @PartyId int = NULL,
    @PartyType varchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyTypeMaster] 
        ([PartyId], [PartyType], [CreatedOn])
        VALUES 
        (@PartyId, @PartyType, GETDATE())
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyTypeMaster] WHERE [PartyId] = @PartyId
    END
END
GO
