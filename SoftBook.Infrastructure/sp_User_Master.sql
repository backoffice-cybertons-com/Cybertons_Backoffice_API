USE [SoftAccDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_User_Master]
    @Action VARCHAR(20),
    @UserId INT = NULL,
    @Username NVARCHAR(100) = NULL,
    @PasswordHash NVARCHAR(256) = NULL,
    @Email NVARCHAR(200) = NULL,
    @FullName NVARCHAR(200) = NULL,
    @Phone NVARCHAR(20) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'SELECT'
    BEGIN
        SELECT [UserId] AS Id, [Username], [Email], [FullName], [Phone], [IsActive]
        FROM [dbo].[TabUserMaster]
        ORDER BY [Username];
    END

    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT [UserId] AS Id, [Username], [Email], [FullName], [Phone], [IsActive]
        FROM [dbo].[TabUserMaster]
        WHERE [UserId] = @UserId;
    END

    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabUserMaster]
        SET 
            [Email] = @Email,
            [FullName] = @FullName,
            [Phone] = @Phone,
            [IsActive] = ISNULL(@IsActive, [IsActive])
        WHERE [UserId] = @UserId;
    END

    ELSE IF @Action = 'DELETE'
    BEGIN
        -- Option 1: Hard Delete
        DELETE FROM [dbo].[TabUserMaster] WHERE [UserId] = @UserId;
        
        -- Option 2: Soft Delete (Uncomment if preferred)
        -- UPDATE [dbo].[TabUserMaster] SET [IsActive] = 0 WHERE [UserId] = @UserId;
    END
END
GO
