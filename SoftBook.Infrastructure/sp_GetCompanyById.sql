USE [CompanyMaster]
GO

CREATE PROCEDURE [dbo].[sp_GetCompanyById]
    @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CompanyId, CompanyName, ConnectionString
    FROM TabCompanyMaster
    WHERE CompanyId = @CompanyId
END
