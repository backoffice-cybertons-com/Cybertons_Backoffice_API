USE [SoftAccDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure to update user password
CREATE PROCEDURE [dbo].[sp_User_UpdatePassword]
    @Email NVARCHAR(200),
    @NewPasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if user exists
    IF NOT EXISTS (SELECT 1 FROM [dbo].[TabUserMaster] WHERE [Email] = @Email)
    BEGIN
        RAISERROR('User not found', 16, 1);
        RETURN;
    END
    
    -- Update password
    UPDATE [dbo].[TabUserMaster]
    SET [PasswordHash] = @NewPasswordHash
    WHERE [Email] = @Email;
    
    -- Return success
    SELECT 1 AS Success;
END
GO
