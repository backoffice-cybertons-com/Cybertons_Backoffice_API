USE [SoftAccDb]
GO

-- =============================================
-- REMAINING MASTERS PART 4 - STORED PROCEDURES
-- SAC, Salesperson, TaxGroup, TCS, TDS, Transporter
-- =============================================

-- SAC MASTER

-- Unified SAC Master SP
IF OBJECT_ID('[dbo].[sp_SAC_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_SAC_Master]
GO
CREATE PROCEDURE [dbo].[sp_SAC_Master]
    @Action varchar(20),
    @SACId int = NULL,
    @SACCode varchar(20) = NULL,
    @Description varchar(300) = NULL,
    @TaxRate decimal(18,2) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabSACMaster] ([SACCode], [Description], [TaxRate]) VALUES (@SACCode, @Description, @TaxRate)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabSACMaster] SET [SACCode] = @SACCode, [Description] = @Description, [TaxRate] = @TaxRate WHERE [SACId] = @SACId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabSACMaster] WHERE [SACId] = @SACId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabSACMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabSACMaster] WHERE [SACId] = @SACId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllSACs]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllSACs]
IF OBJECT_ID('[dbo].[sp_GetSACById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetSACById]
IF OBJECT_ID('[dbo].[sp_InsertSAC]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertSAC]
IF OBJECT_ID('[dbo].[sp_UpdateSAC]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateSAC]
IF OBJECT_ID('[dbo].[sp_DeleteSAC]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteSAC]
GO

-- SALESPERSON MASTER

-- Unified Salesperson Master SP
IF OBJECT_ID('[dbo].[sp_Salesperson_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Salesperson_Master]
GO
CREATE PROCEDURE [dbo].[sp_Salesperson_Master]
    @Action varchar(20),
    @SalespersonId int = NULL,
    @SalespersonName varchar(200) = NULL,
    @Email varchar(200) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabSalespersonMaster] ([SalespersonName], [Email]) VALUES (@SalespersonName, @Email)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabSalespersonMaster] SET [SalespersonName] = @SalespersonName, [Email] = @Email WHERE [SalespersonId] = @SalespersonId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabSalespersonMaster] WHERE [SalespersonId] = @SalespersonId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabSalespersonMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabSalespersonMaster] WHERE [SalespersonId] = @SalespersonId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllSalespersons]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllSalespersons]
IF OBJECT_ID('[dbo].[sp_GetSalespersonById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetSalespersonById]
IF OBJECT_ID('[dbo].[sp_InsertSalesperson]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertSalesperson]
IF OBJECT_ID('[dbo].[sp_UpdateSalesperson]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateSalesperson]
IF OBJECT_ID('[dbo].[sp_DeleteSalesperson]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteSalesperson]
GO

-- TAX GROUP MASTER

-- Unified TaxGroup Master SP
IF OBJECT_ID('[dbo].[sp_TaxGroup_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_TaxGroup_Master]
GO
CREATE PROCEDURE [dbo].[sp_TaxGroup_Master]
    @Action varchar(20),
    @TaxGroupId int = NULL,
    @GroupName varchar(100) = NULL,
    @Description varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTaxGroupMaster] ([GroupName], [Description]) VALUES (@GroupName, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTaxGroupMaster] SET [GroupName] = @GroupName, [Description] = @Description WHERE [TaxGroupId] = @TaxGroupId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTaxGroupMaster] WHERE [TaxGroupId] = @TaxGroupId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTaxGroupMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTaxGroupMaster] WHERE [TaxGroupId] = @TaxGroupId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllTaxGroups]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllTaxGroups]
IF OBJECT_ID('[dbo].[sp_GetTaxGroupById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetTaxGroupById]
IF OBJECT_ID('[dbo].[sp_InsertTaxGroup]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertTaxGroup]
IF OBJECT_ID('[dbo].[sp_UpdateTaxGroup]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateTaxGroup]
IF OBJECT_ID('[dbo].[sp_DeleteTaxGroup]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteTaxGroup]
GO

-- TCS MASTER

-- Unified TCS Master SP
IF OBJECT_ID('[dbo].[sp_TCS_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_TCS_Master]
GO
CREATE PROCEDURE [dbo].[sp_TCS_Master]
    @Action varchar(20),
    @TCSId int = NULL,
    @Rate decimal(18,2) = NULL,
    @NatureOfCollection varchar(100) = NULL,
    @IsHigherRate bit = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTCSMaster] ([Rate], [NatureOfCollection], [IsHigherRate], [StartDate], [EndDate], [CreatedAt])
        VALUES (@Rate, @NatureOfCollection, @IsHigherRate, @StartDate, @EndDate, GETDATE())
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTCSMaster]
        SET [Rate] = @Rate, [NatureOfCollection] = @NatureOfCollection, [IsHigherRate] = @IsHigherRate,
            [StartDate] = @StartDate, [EndDate] = @EndDate
        WHERE [TCSId] = @TCSId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTCSMaster] WHERE [TCSId] = @TCSId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTCSMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTCSMaster] WHERE [TCSId] = @TCSId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllTCS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllTCS]
IF OBJECT_ID('[dbo].[sp_GetTCSById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetTCSById]
IF OBJECT_ID('[dbo].[sp_InsertTCS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertTCS]
IF OBJECT_ID('[dbo].[sp_UpdateTCS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateTCS]
IF OBJECT_ID('[dbo].[sp_DeleteTCS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteTCS]
GO

-- TDS MASTER

-- Unified TDS Master SP
IF OBJECT_ID('[dbo].[sp_TDS_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_TDS_Master]
GO
CREATE PROCEDURE [dbo].[sp_TDS_Master]
    @Action varchar(20),
    @TDSId int = NULL,
    @TaxName varchar(100) = NULL,
    @Rate decimal(18,2) = NULL,
    @Section varchar(50) = NULL,
    @IsHigherRate bit = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTDSMaster] ([TaxName], [Rate], [Section], [IsHigherRate], [StartDate], [EndDate], [CreatedAt])
        VALUES (@TaxName, @Rate, @Section, @IsHigherRate, @StartDate, @EndDate, GETDATE())
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTDSMaster]
        SET [TaxName] = @TaxName, [Rate] = @Rate, [Section] = @Section, [IsHigherRate] = @IsHigherRate,
            [StartDate] = @StartDate, [EndDate] = @EndDate
        WHERE [TDSId] = @TDSId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTDSMaster] WHERE [TDSId] = @TDSId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTDSMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTDSMaster] WHERE [TDSId] = @TDSId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllTDS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllTDS]
IF OBJECT_ID('[dbo].[sp_GetTDSById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetTDSById]
IF OBJECT_ID('[dbo].[sp_InsertTDS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertTDS]
IF OBJECT_ID('[dbo].[sp_UpdateTDS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateTDS]
IF OBJECT_ID('[dbo].[sp_DeleteTDS]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteTDS]
GO

-- TRANSPORTER MASTER

-- Unified Transporter Master SP
IF OBJECT_ID('[dbo].[sp_Transporter_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Transporter_Master]
GO
CREATE PROCEDURE [dbo].[sp_Transporter_Master]
    @Action varchar(20),
    @TransporterId int = NULL,
    @TransporterName varchar(200) = NULL,
    @GSTNumber varchar(20) = NULL,
    @ContactPerson varchar(100) = NULL,
    @Phone varchar(20) = NULL,
    @Address varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTransporterMaster] ([TransporterName], [GSTNumber], [ContactPerson], [Phone], [Address])
        VALUES (@TransporterName, @GSTNumber, @ContactPerson, @Phone, @Address)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTransporterMaster]
        SET [TransporterName] = @TransporterName, [GSTNumber] = @GSTNumber, [ContactPerson] = @ContactPerson,
            [Phone] = @Phone, [Address] = @Address
        WHERE [TransporterId] = @TransporterId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTransporterMaster] WHERE [TransporterId] = @TransporterId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTransporterMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTransporterMaster] WHERE [TransporterId] = @TransporterId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllTransporters]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllTransporters]
IF OBJECT_ID('[dbo].[sp_GetTransporterById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetTransporterById]
IF OBJECT_ID('[dbo].[sp_InsertTransporter]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertTransporter]
IF OBJECT_ID('[dbo].[sp_UpdateTransporter]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateTransporter]
IF OBJECT_ID('[dbo].[sp_DeleteTransporter]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteTransporter]
GO
