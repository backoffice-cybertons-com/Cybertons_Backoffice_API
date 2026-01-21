USE [SoftAccDb]
GO

-- =============================================
-- REMAINING MASTERS PART 3 - STORED PROCEDURES
-- CostCenter, FiscalYear, Godown, PaymentTerm, Project, Role
-- =============================================

-- COST CENTER MASTER

-- Unified CostCenter Master SP
IF OBJECT_ID('[dbo].[sp_CostCenter_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_CostCenter_Master]
GO
CREATE PROCEDURE [dbo].[sp_CostCenter_Master]
    @Action varchar(20),
    @CostCenterId int = NULL,
    @CostCenterName varchar(150) = NULL,
    @Description varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabCostCenterMaster] ([CostCenterName], [Description]) VALUES (@CostCenterName, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabCostCenterMaster] SET [CostCenterName] = @CostCenterName, [Description] = @Description WHERE [CostCenterId] = @CostCenterId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabCostCenterMaster] WHERE [CostCenterId] = @CostCenterId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabCostCenterMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabCostCenterMaster] WHERE [CostCenterId] = @CostCenterId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllCostCenters]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllCostCenters]
IF OBJECT_ID('[dbo].[sp_GetCostCenterById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetCostCenterById]
IF OBJECT_ID('[dbo].[sp_InsertCostCenter]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertCostCenter]
IF OBJECT_ID('[dbo].[sp_UpdateCostCenter]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateCostCenter]
IF OBJECT_ID('[dbo].[sp_DeleteCostCenter]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteCostCenter]
GO

-- FISCAL YEAR MASTER

-- Unified FiscalYear Master SP
IF OBJECT_ID('[dbo].[sp_FiscalYear_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_FiscalYear_Master]
GO
CREATE PROCEDURE [dbo].[sp_FiscalYear_Master]
    @Action varchar(20),
    @FiscalYearId int = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL,
    @IsCurrent bit = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabFiscalYearMaster] ([StartDate], [EndDate], [IsCurrent]) VALUES (@StartDate, @EndDate, @IsCurrent)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabFiscalYearMaster] SET [StartDate] = @StartDate, [EndDate] = @EndDate, [IsCurrent] = @IsCurrent WHERE [FiscalYearId] = @FiscalYearId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabFiscalYearMaster] WHERE [FiscalYearId] = @FiscalYearId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabFiscalYearMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabFiscalYearMaster] WHERE [FiscalYearId] = @FiscalYearId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllFiscalYears]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllFiscalYears]
IF OBJECT_ID('[dbo].[sp_GetFiscalYearById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetFiscalYearById]
IF OBJECT_ID('[dbo].[sp_InsertFiscalYear]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertFiscalYear]
IF OBJECT_ID('[dbo].[sp_UpdateFiscalYear]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateFiscalYear]
IF OBJECT_ID('[dbo].[sp_DeleteFiscalYear]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteFiscalYear]
GO

-- GODOWN MASTER

-- Unified Godown Master SP
IF OBJECT_ID('[dbo].[sp_Godown_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Godown_Master]
GO
CREATE PROCEDURE [dbo].[sp_Godown_Master]
    @Action varchar(20),
    @GodownId int = NULL,
    @WarehouseId int = NULL,
    @GodownName varchar(150) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabGodownMaster] ([WarehouseId], [GodownName]) VALUES (@WarehouseId, @GodownName)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabGodownMaster] SET [WarehouseId] = @WarehouseId, [GodownName] = @GodownName WHERE [GodownId] = @GodownId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabGodownMaster] WHERE [GodownId] = @GodownId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabGodownMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabGodownMaster] WHERE [GodownId] = @GodownId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllGodowns]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllGodowns]
IF OBJECT_ID('[dbo].[sp_GetGodownById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetGodownById]
IF OBJECT_ID('[dbo].[sp_InsertGodown]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertGodown]
IF OBJECT_ID('[dbo].[sp_UpdateGodown]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateGodown]
IF OBJECT_ID('[dbo].[sp_DeleteGodown]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteGodown]
GO

-- PAYMENT TERM MASTER

-- Unified PaymentTerm Master SP
IF OBJECT_ID('[dbo].[sp_PaymentTerm_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_PaymentTerm_Master]
GO
CREATE PROCEDURE [dbo].[sp_PaymentTerm_Master]
    @Action varchar(20),
    @TermId int = NULL,
    @TermName varchar(100) = NULL,
    @NumberOfDays int = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabPaymentTermMaster] ([TermName], [NumberOfDays]) VALUES (@TermName, @NumberOfDays)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabPaymentTermMaster] SET [TermName] = @TermName, [NumberOfDays] = @NumberOfDays WHERE [TermId] = @TermId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabPaymentTermMaster] WHERE [TermId] = @TermId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabPaymentTermMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabPaymentTermMaster] WHERE [TermId] = @TermId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllPaymentTerms]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllPaymentTerms]
IF OBJECT_ID('[dbo].[sp_GetPaymentTermById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetPaymentTermById]
IF OBJECT_ID('[dbo].[sp_InsertPaymentTerm]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertPaymentTerm]
IF OBJECT_ID('[dbo].[sp_UpdatePaymentTerm]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdatePaymentTerm]
IF OBJECT_ID('[dbo].[sp_DeletePaymentTerm]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeletePaymentTerm]
GO

-- PROJECT MASTER

-- Unified Project Master SP
IF OBJECT_ID('[dbo].[sp_Project_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Project_Master]
GO
CREATE PROCEDURE [dbo].[sp_Project_Master]
    @Action varchar(20),
    @ProjectId int = NULL,
    @ProjectName varchar(200) = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL,
    @Status varchar(50) = NULL,
    @Description varchar(500) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabProjectMaster] ([ProjectName], [StartDate], [EndDate], [Status], [Description])
        VALUES (@ProjectName, @StartDate, @EndDate, @Status, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabProjectMaster]
        SET [ProjectName] = @ProjectName, [StartDate] = @StartDate, [EndDate] = @EndDate,
            [Status] = @Status, [Description] = @Description
        WHERE [ProjectId] = @ProjectId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabProjectMaster] WHERE [ProjectId] = @ProjectId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabProjectMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabProjectMaster] WHERE [ProjectId] = @ProjectId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllProjects]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllProjects]
IF OBJECT_ID('[dbo].[sp_GetProjectById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetProjectById]
IF OBJECT_ID('[dbo].[sp_InsertProject]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertProject]
IF OBJECT_ID('[dbo].[sp_UpdateProject]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateProject]
IF OBJECT_ID('[dbo].[sp_DeleteProject]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteProject]
GO

-- ROLE MASTER

-- Unified Role Master SP
IF OBJECT_ID('[dbo].[sp_Role_Master]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Role_Master]
GO
CREATE PROCEDURE [dbo].[sp_Role_Master]
    @Action varchar(20),
    @RoleId int = NULL,
    @RoleName varchar(100) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabRoleMaster] ([RoleName]) VALUES (@RoleName)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabRoleMaster] SET [RoleName] = @RoleName WHERE [RoleId] = @RoleId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabRoleMaster] WHERE [RoleId] = @RoleId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabRoleMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabRoleMaster] WHERE [RoleId] = @RoleId
END
GO

-- Cleanup Old SPs
IF OBJECT_ID('[dbo].[sp_GetAllRoles]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllRoles]
IF OBJECT_ID('[dbo].[sp_GetRoleById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetRoleById]
IF OBJECT_ID('[dbo].[sp_InsertRole]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertRole]
IF OBJECT_ID('[dbo].[sp_UpdateRole]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateRole]
IF OBJECT_ID('[dbo].[sp_DeleteRole]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteRole]
GO
