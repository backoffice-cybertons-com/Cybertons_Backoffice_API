USE [SoftAccDb]
GO

-- =============================================
-- UNIT MASTER STORED PROCEDURES
-- =============================================

-- Unified Unit Master SP
IF OBJECT_ID('[dbo].[sp_Unit_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Unit_Master]
GO
CREATE PROCEDURE [dbo].[sp_Unit_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID
    @UnitId int = NULL,
    @UnitName varchar(20) = NULL,
    @UQC varchar(20) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabUnitMaster] ([UnitName], [UQC]) VALUES (@UnitName, @UQC)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabUnitMaster] SET [UnitName] = @UnitName, [UQC] = @UQC WHERE [UnitId] = @UnitId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabUnitMaster] WHERE [UnitId] = @UnitId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabUnitMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabUnitMaster] WHERE [UnitId] = @UnitId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllUnits]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllUnits]
IF OBJECT_ID('[dbo].[sp_GetUnitById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetUnitById]
IF OBJECT_ID('[dbo].[sp_InsertUnit]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertUnit]
IF OBJECT_ID('[dbo].[sp_UpdateUnit]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateUnit]
IF OBJECT_ID('[dbo].[sp_DeleteUnit]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteUnit]
GO

-- =============================================
-- TAX MASTER STORED PROCEDURES
-- =============================================

-- Unified Tax Master SP
IF OBJECT_ID('[dbo].[sp_Tax_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Tax_Master]
GO
CREATE PROCEDURE [dbo].[sp_Tax_Master]
    @Action varchar(20),
    @TaxId int = NULL,
    @TaxName varchar(200) = NULL,
    @RatePercent decimal(18,2) = NULL,
    @TaxType varchar(50) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTaxMaster] ([TaxName], [RatePercent], [TaxType]) VALUES (@TaxName, @RatePercent, @TaxType)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTaxMaster] SET [TaxName] = @TaxName, [RatePercent] = @RatePercent, [TaxType] = @TaxType WHERE [TaxId] = @TaxId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTaxMaster] WHERE [TaxId] = @TaxId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTaxMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTaxMaster] WHERE [TaxId] = @TaxId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllTaxes]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllTaxes]
IF OBJECT_ID('[dbo].[sp_GetTaxById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetTaxById]
IF OBJECT_ID('[dbo].[sp_InsertTax]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertTax]
IF OBJECT_ID('[dbo].[sp_UpdateTax]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateTax]
IF OBJECT_ID('[dbo].[sp_DeleteTax]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteTax]
GO

-- =============================================
-- HSN MASTER STORED PROCEDURES
-- =============================================

-- Unified HSN Master SP
IF OBJECT_ID('[dbo].[sp_HSN_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_HSN_Master]
GO
CREATE PROCEDURE [dbo].[sp_HSN_Master]
    @Action varchar(20),
    @HSNId int = NULL,
    @HSNCode varchar(20) = NULL,
    @Description varchar(300) = NULL,
    @TaxRate decimal(18,2) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabHSNMaster] ([HSNCode], [Description], [TaxRate]) VALUES (@HSNCode, @Description, @TaxRate)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabHSNMaster] SET [HSNCode] = @HSNCode, [Description] = @Description, [TaxRate] = @TaxRate WHERE [HSNId] = @HSNId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabHSNMaster] WHERE [HSNId] = @HSNId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabHSNMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabHSNMaster] WHERE [HSNId] = @HSNId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllHSNs]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllHSNs]
IF OBJECT_ID('[dbo].[sp_GetHSNById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetHSNById]
IF OBJECT_ID('[dbo].[sp_InsertHSN]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertHSN]
IF OBJECT_ID('[dbo].[sp_UpdateHSN]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateHSN]
IF OBJECT_ID('[dbo].[sp_DeleteHSN]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteHSN]
GO

-- =============================================
-- CURRENCY MASTER STORED PROCEDURES
-- =============================================

-- Unified Currency Master SP
IF OBJECT_ID('[dbo].[sp_Currency_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Currency_Master]
GO
CREATE PROCEDURE [dbo].[sp_Currency_Master]
    @Action varchar(20),
    @CurrencyId int = NULL,
    @CurrencyCode varchar(10) = NULL,
    @CurrencySymbol varchar(10) = NULL,
    @CurrencyName varchar(100) = NULL,
    @DecimalPlaces tinyint = NULL,
    @Format varchar(50) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabCurrencyMaster] ([CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) 
        VALUES (@CurrencyCode, @CurrencySymbol, @CurrencyName, @DecimalPlaces, @Format)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabCurrencyMaster] 
        SET [CurrencyCode] = @CurrencyCode, [CurrencySymbol] = @CurrencySymbol, [CurrencyName] = @CurrencyName, [DecimalPlaces] = @DecimalPlaces, [Format] = @Format
        WHERE [CurrencyId] = @CurrencyId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabCurrencyMaster] WHERE [CurrencyId] = @CurrencyId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabCurrencyMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabCurrencyMaster] WHERE [CurrencyId] = @CurrencyId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllCurrencies]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllCurrencies]
IF OBJECT_ID('[dbo].[sp_GetCurrencyById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetCurrencyById]
IF OBJECT_ID('[dbo].[sp_InsertCurrency]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertCurrency]
IF OBJECT_ID('[dbo].[sp_UpdateCurrency]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateCurrency]
IF OBJECT_ID('[dbo].[sp_DeleteCurrency]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteCurrency]
GO

-- =============================================
-- PAYMENT MODE MASTER STORED PROCEDURES
-- =============================================

-- Unified Payment Mode Master SP
IF OBJECT_ID('[dbo].[sp_PaymentMode_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PaymentMode_Master]
GO
CREATE PROCEDURE [dbo].[sp_PaymentMode_Master]
    @Action varchar(20),
    @PaymentModeId int = NULL,
    @ModeName varchar(100) = NULL,
    @IsDefault bit = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabPaymentModeMaster] ([ModeName], [IsDefault]) VALUES (@ModeName, @IsDefault)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabPaymentModeMaster] SET [ModeName] = @ModeName, [IsDefault] = @IsDefault WHERE [PaymentModeId] = @PaymentModeId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabPaymentModeMaster] WHERE [PaymentModeId] = @PaymentModeId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabPaymentModeMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabPaymentModeMaster] WHERE [PaymentModeId] = @PaymentModeId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllPaymentModes]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllPaymentModes]
IF OBJECT_ID('[dbo].[sp_GetPaymentModeById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetPaymentModeById]
IF OBJECT_ID('[dbo].[sp_InsertPaymentMode]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertPaymentMode]
IF OBJECT_ID('[dbo].[sp_UpdatePaymentMode]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdatePaymentMode]
IF OBJECT_ID('[dbo].[sp_DeletePaymentMode]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeletePaymentMode]
GO
