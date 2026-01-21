-- Stored Procedures for Party Related Tables

-- =============================================
-- Party Address Master Stored Procedure
-- =============================================
IF OBJECT_ID('[dbo].[sp_PartyAddress_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_PartyAddress_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyAddress_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID, SELECT_BY_PARTY_ID
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
        ([PartyId], [AddressType], [Attention], [Country], [Street1], [Street2], [City], [State], [PinCode], [Phone], [FaxNumber], [IsDefault], [CreatedOn])
        VALUES 
        (@PartyId, @AddressType, @Attention, @Country, @Street1, @Street2, @City, @State, @PinCode, @Phone, @FaxNumber, @IsDefault, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyAddress]
        SET [PartyId] = @PartyId, [AddressType] = @AddressType, [Attention] = @Attention, [Country] = @Country,
            [Street1] = @Street1, [Street2] = @Street2, [City] = @City, [State] = @State,
            [PinCode] = @PinCode, [Phone] = @Phone, [FaxNumber] = @FaxNumber, [IsDefault] = @IsDefault
        WHERE [AddressId] = @AddressId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabPartyAddress] WHERE [AddressId] = @AddressId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabPartyAddress]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyAddress] WHERE [AddressId] = @AddressId
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyAddress] WHERE [PartyId] = @PartyId
    END
END
GO

-- =============================================
-- Party Bank Details Master Stored Procedure
-- =============================================
IF OBJECT_ID('[dbo].[sp_PartyBankDetails_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_PartyBankDetails_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyBankDetails_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID, SELECT_BY_PARTY_ID
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
        WHERE [BankDetailId] = @BankDetailId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabPartyBankDetails] WHERE [BankDetailId] = @BankDetailId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabPartyBankDetails]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyBankDetails] WHERE [BankDetailId] = @BankDetailId
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyBankDetails] WHERE [PartyId] = @PartyId
    END
END
GO

-- =============================================
-- Party Contact Person Master Stored Procedure
-- =============================================
IF OBJECT_ID('[dbo].[sp_PartyContactPerson_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_PartyContactPerson_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyContactPerson_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID, SELECT_BY_PARTY_ID
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
        ([PartyId], [Salutation], [FirstName], [LastName], [EmailAddress], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn])
        VALUES 
        (@PartyId, @Salutation, @FirstName, @LastName, @EmailAddress, @WorkPhone, @Mobile, @IsPrimary, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyContactPersons]
        SET [PartyId] = @PartyId, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [EmailAddress] = @EmailAddress, [WorkPhone] = @WorkPhone, [Mobile] = @Mobile, [IsPrimary] = @IsPrimary
        WHERE [ContactPersonId] = @ContactPersonId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabPartyContactPersons]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyContactPersons] WHERE [PartyId] = @PartyId
    END
END
GO

-- =============================================
-- Party Reporting Tag Master Stored Procedure
-- =============================================
IF OBJECT_ID('[dbo].[sp_PartyReportingTag_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_PartyReportingTag_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyReportingTag_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID, SELECT_BY_PARTY_ID
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
        ([PartyId], [ReportingTagId], [TagValue], [CreatedOn])
        VALUES 
        (@PartyId, @ReportingTagId, @TagValue, GETDATE())
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyReportingTags]
        SET [PartyId] = @PartyId, [ReportingTagId] = @ReportingTagId, [TagValue] = @TagValue
        WHERE [PartyReportingTagId] = @PartyReportingTagId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabPartyReportingTags] WHERE [PartyReportingTagId] = @PartyReportingTagId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabPartyReportingTags]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyReportingTags] WHERE [PartyReportingTagId] = @PartyReportingTagId
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyReportingTags] WHERE [PartyId] = @PartyId
    END
END
GO

-- =============================================
-- Party Type Master Stored Procedure
-- =============================================
IF OBJECT_ID('[dbo].[sp_PartyTypeMaster_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_PartyTypeMaster_Master]
GO

CREATE PROCEDURE [dbo].[sp_PartyTypeMaster_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID, SELECT_BY_PARTY_ID
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
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyTypeMaster]
        SET [PartyId] = @PartyId, [PartyType] = @PartyType
        WHERE [PartyTypeId] = @PartyTypeId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabPartyTypeMaster] WHERE [PartyTypeId] = @PartyTypeId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabPartyTypeMaster]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyTypeMaster] WHERE [PartyTypeId] = @PartyTypeId
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyTypeMaster] WHERE [PartyId] = @PartyId
    END
END
GO

-- =============================================
-- Update Party Master Stored Procedure to support SELECT_BY_TYPE
-- =============================================
-- Note: Run this ALTER statement to add SELECT_BY_TYPE support to existing sp_Party_Master

IF OBJECT_ID('[dbo].[sp_Party_Master]', 'P') IS NOT NULL
BEGIN
    -- Add SELECT_BY_TYPE action to existing procedure
    -- Note: This is a simplified version - you may need to adjust based on your actual procedure
    EXEC('
    ALTER PROCEDURE [dbo].[sp_Party_Master]
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
        @GSTTreatment varchar(100) = NULL,
        @PAN varchar(20) = NULL,
        @PreferredLanguage varchar(50) = NULL,
        @PlaceOfSupply varchar(100) = NULL,
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

        IF @Action = ''INSERT''
        BEGIN
            INSERT INTO [dbo].[TabPartyMaster] 
            ([PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], 
             [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [Currency],
             [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms],
             [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn])
            VALUES 
            (@PartyType, @Salutation, @FirstName, @LastName, @CompanyName, @DisplayName, @Email, @PhoneWork, @PhoneMobile,
             @PreferredLanguage, @GSTTreatment, @PlaceOfSupply, @PAN, @MSMERegistered, @TaxPreference, @TDS, @Currency,
             @AccountsReceivableAccount, @AccountsPayableAccount, @OpeningBalance, @OpeningBalanceType, @PaymentTerms,
             @EnablePortal, @WebsiteURL, @Department, @Remarks, GETDATE())
            
            SELECT SCOPE_IDENTITY() AS PartyId
        END
        ELSE IF @Action = ''UPDATE''
        BEGIN
            UPDATE [dbo].[TabPartyMaster]
            SET [PartyType] = @PartyType, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
                [CompanyName] = @CompanyName, [DisplayName] = @DisplayName, [Email] = @Email, [PhoneWork] = @PhoneWork,
                [PhoneMobile] = @PhoneMobile, [PreferredLanguage] = @PreferredLanguage, [GSTTreatment] = @GSTTreatment,
                [PlaceOfSupply] = @PlaceOfSupply, [PAN] = @PAN, [MSMERegistered] = @MSMERegistered, [TaxPreference] = @TaxPreference,
                [TDS] = @TDS, [Currency] = @Currency, [AccountsReceivableAccount] = @AccountsReceivableAccount,
                [AccountsPayableAccount] = @AccountsPayableAccount, [OpeningBalance] = @OpeningBalance,
                [OpeningBalanceType] = @OpeningBalanceType, [PaymentTerms] = @PaymentTerms, [EnablePortal] = @EnablePortal,
                [WebsiteURL] = @WebsiteURL, [Department] = @Department, [Remarks] = @Remarks
            WHERE [PartyId] = @PartyId
        END
        ELSE IF @Action = ''DELETE''
        BEGIN
            DELETE FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId
        END
        ELSE IF @Action = ''SELECT''
        BEGIN
            SELECT * FROM [dbo].[TabPartyMaster]
        END
        ELSE IF @Action = ''SELECT_BY_ID''
        BEGIN
            SELECT * FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId
        END
        ELSE IF @Action = ''SELECT_BY_TYPE''
        BEGIN
            SELECT DISTINCT p.* 
            FROM [dbo].[TabPartyMaster] p
            INNER JOIN [dbo].[TabPartyTypeMaster] ptm ON p.PartyId = ptm.PartyId
            WHERE ptm.PartyType = @PartyType
        END
    END
    ')
END
GO
