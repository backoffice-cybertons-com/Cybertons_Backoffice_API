USE [SoftAccDb]
GO

-- =============================================
-- WAREHOUSE MASTER STORED PROCEDURES
-- =============================================

-- Unified Warehouse Master SP
IF OBJECT_ID('[dbo].[sp_Warehouse_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Warehouse_Master]
GO
CREATE PROCEDURE [dbo].[sp_Warehouse_Master]
    @Action varchar(20),
    @WarehouseId int = NULL,
    @WarehouseName varchar(150) = NULL,
    @BranchId int = NULL,
    @Address varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabWarehouseMaster] ([WarehouseName], [BranchId], [Address]) VALUES (@WarehouseName, @BranchId, @Address)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabWarehouseMaster] SET [WarehouseName] = @WarehouseName, [BranchId] = @BranchId, [Address] = @Address WHERE [WarehouseId] = @WarehouseId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabWarehouseMaster] WHERE [WarehouseId] = @WarehouseId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabWarehouseMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabWarehouseMaster] WHERE [WarehouseId] = @WarehouseId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllWarehouses]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllWarehouses]
IF OBJECT_ID('[dbo].[sp_GetWarehouseById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetWarehouseById]
IF OBJECT_ID('[dbo].[sp_InsertWarehouse]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertWarehouse]
IF OBJECT_ID('[dbo].[sp_UpdateWarehouse]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateWarehouse]
IF OBJECT_ID('[dbo].[sp_DeleteWarehouse]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteWarehouse]
GO

-- =============================================
-- BRANCH MASTER STORED PROCEDURES
-- =============================================

-- Unified Branch Master SP
IF OBJECT_ID('[dbo].[sp_Branch_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Branch_Master]
GO
CREATE PROCEDURE [dbo].[sp_Branch_Master]
    @Action varchar(20),
    @BranchId int = NULL,
    @BranchName varchar(150) = NULL,
    @Address varchar(300) = NULL,
    @City varchar(100) = NULL,
    @State varchar(100) = NULL,
    @Country varchar(100) = NULL,
    @Phone varchar(20) = NULL,
    @IsActive bit = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabBranchMaster] ([BranchName], [Address], [City], [State], [Country], [Phone], [IsActive])
        VALUES (@BranchName, @Address, @City, @State, @Country, @Phone, @IsActive)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabBranchMaster]
        SET [BranchName] = @BranchName, [Address] = @Address, [City] = @City, 
            [State] = @State, [Country] = @Country, [Phone] = @Phone, [IsActive] = @IsActive
        WHERE [BranchId] = @BranchId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabBranchMaster] WHERE [BranchId] = @BranchId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabBranchMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabBranchMaster] WHERE [BranchId] = @BranchId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllBranches]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllBranches]
IF OBJECT_ID('[dbo].[sp_GetBranchById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetBranchById]
IF OBJECT_ID('[dbo].[sp_InsertBranch]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertBranch]
IF OBJECT_ID('[dbo].[sp_UpdateBranch]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateBranch]
IF OBJECT_ID('[dbo].[sp_DeleteBranch]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteBranch]
GO

-- =============================================
-- BANK MASTER STORED PROCEDURES
-- =============================================

-- Unified Bank Master SP
IF OBJECT_ID('[dbo].[sp_Bank_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Bank_Master]
GO
CREATE PROCEDURE [dbo].[sp_Bank_Master]
    @Action varchar(20),
    @BankMasterId int = NULL,
    @BankName varchar(200) = NULL,
    @Branch varchar(150) = NULL,
    @AccountNumber varchar(50) = NULL,
    @IFSC varchar(20) = NULL,
    @MICR varchar(20) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabBankMaster] ([BankName], [Branch], [AccountNumber], [IFSC], [MICR])
        VALUES (@BankName, @Branch, @AccountNumber, @IFSC, @MICR)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabBankMaster]
        SET [BankName] = @BankName, [Branch] = @Branch, [AccountNumber] = @AccountNumber,
            [IFSC] = @IFSC, [MICR] = @MICR
        WHERE [BankMasterId] = @BankMasterId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabBankMaster] WHERE [BankMasterId] = @BankMasterId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabBankMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabBankMaster] WHERE [BankMasterId] = @BankMasterId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllBanks]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllBanks]
IF OBJECT_ID('[dbo].[sp_GetBankById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetBankById]
IF OBJECT_ID('[dbo].[sp_InsertBank]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertBank]
IF OBJECT_ID('[dbo].[sp_UpdateBank]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateBank]
IF OBJECT_ID('[dbo].[sp_DeleteBank]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteBank]
GO

-- =============================================
-- ACCOUNT MASTER STORED PROCEDURES
-- =============================================

-- Unified Account Master SP
IF OBJECT_ID('[dbo].[sp_Account_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Account_Master]
GO
CREATE PROCEDURE [dbo].[sp_Account_Master]
    @Action varchar(20),
    @AccountId int = NULL,
    @AccountType varchar(100) = NULL,
    @AccountName varchar(200) = NULL,
    @IsSubAccount bit = NULL,
    @ParentAccountId int = NULL,
    @AccountCode varchar(50) = NULL,
    @Description varchar(500) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabAccountMaster] ([AccountType], [AccountName], [IsSubAccount], [ParentAccountId], [AccountCode], [Description])
        VALUES (@AccountType, @AccountName, @IsSubAccount, @ParentAccountId, @AccountCode, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabAccountMaster]
        SET [AccountType] = @AccountType, [AccountName] = @AccountName, [IsSubAccount] = @IsSubAccount,
            [ParentAccountId] = @ParentAccountId, [AccountCode] = @AccountCode, [Description] = @Description
        WHERE [AccountId] = @AccountId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabAccountMaster] WHERE [AccountId] = @AccountId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabAccountMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabAccountMaster] WHERE [AccountId] = @AccountId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllAccounts]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllAccounts]
IF OBJECT_ID('[dbo].[sp_GetAccountById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAccountById]
IF OBJECT_ID('[dbo].[sp_InsertAccount]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertAccount]
IF OBJECT_ID('[dbo].[sp_UpdateAccount]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateAccount]
IF OBJECT_ID('[dbo].[sp_DeleteAccount]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteAccount]
GO

-- =============================================
-- PARTY MASTER STORED PROCEDURES
-- =============================================

-- Unified Party Master SP
IF OBJECT_ID('[dbo].[sp_Party_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Party_Master]
GO

CREATE PROCEDURE [dbo].[sp_Party_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID
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
    @GSTTreatment varchar(100) = NULL,
    @PAN varchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyMaster] 
        ([PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [GSTTreatment], [PAN], [CreatedOn])
        VALUES 
        (@PartyType, @Salutation, @FirstName, @LastName, @CompanyName, @DisplayName, @Email, @PhoneWork, @PhoneMobile, @GSTTreatment, @PAN, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyMaster]
        SET [PartyType] = @PartyType, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [CompanyName] = @CompanyName, [DisplayName] = @DisplayName, [Email] = @Email, [PhoneWork] = @PhoneWork,
            [PhoneMobile] = @PhoneMobile, [GSTTreatment] = @GSTTreatment, [PAN] = @PAN
        WHERE [PartyId] = @PartyId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabPartyMaster]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId
    END
END
GO

-- Drop old procedures if they exist (cleanup)
IF OBJECT_ID('[dbo].[sp_GetAllParties]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllParties]
IF OBJECT_ID('[dbo].[sp_GetPartyById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetPartyById]
IF OBJECT_ID('[dbo].[sp_InsertParty]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertParty]
IF OBJECT_ID('[dbo].[sp_UpdateParty]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateParty]
IF OBJECT_ID('[dbo].[sp_DeleteParty]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteParty]
GO
