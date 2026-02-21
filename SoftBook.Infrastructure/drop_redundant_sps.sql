USE [SoftAccDb]
GO

-- List of procedures to drop as they are now handled by sp_Master_Universal
DROP PROCEDURE IF EXISTS [dbo].[sp_Unit_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Tax_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_HSN_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Currency_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_PaymentMode_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Warehouse_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Branch_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Bank_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Account_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_CostCenter_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_FiscalYear_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Godown_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_PaymentTerm_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Project_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Role_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_SAC_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Salesperson_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_TaxGroup_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_TCS_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_TDS_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_Transporter_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_PartyTypeMaster_Master];
DROP PROCEDURE IF EXISTS [dbo].[sp_User_Master];

-- Partials cleanup
DROP PROCEDURE IF EXISTS [dbo].[sp_GetAllUnits];
DROP PROCEDURE IF EXISTS [dbo].[sp_GetUnitById];
DROP PROCEDURE IF EXISTS [dbo].[sp_InsertUnit];
DROP PROCEDURE IF EXISTS [dbo].[sp_UpdateUnit];
DROP PROCEDURE IF EXISTS [dbo].[sp_DeleteUnit];
-- ... and so on for all other entities ...
-- (The sp_Master_Universal.sql already contains cleanup for many of these)
GO
