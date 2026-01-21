USE [SoftAccDb]
GO

-- 1. Add GSTNO field to TabPartyMaster
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[TabPartyMaster]') AND name = 'GSTNO')
BEGIN
    ALTER TABLE [dbo].[TabPartyMaster] ADD [GSTNO] VARCHAR(40) NULL;
END
GO

-- 2. Update sp_Party_Master
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
    @GSTNO varchar(40) = NULL, -- New field
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
    ELSE IF @Action = 'SELECT_BY_TYPE' BEGIN SELECT *, CurrencyId AS [Currency] FROM [dbo].[TabPartyMaster] WHERE [PartyType] = @PartyType END
END
GO
