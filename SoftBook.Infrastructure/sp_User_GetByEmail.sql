USE [SoftAccDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure to get user by email
CREATE PROCEDURE [dbo].[sp_User_GetByEmail]
    @Email NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT [UserId], [Username], [Email], [FullName], [Phone], [IsActive], [PasswordHash]
    FROM [dbo].[TabUserMaster]
    WHERE [Email] = @Email AND [IsActive] = 1;
END
GO
