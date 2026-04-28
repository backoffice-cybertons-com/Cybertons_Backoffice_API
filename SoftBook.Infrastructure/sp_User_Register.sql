USE [SoftAccDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Stored procedure to register a new user
CREATE PROCEDURE [dbo].[sp_User_Register]
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(256),
    @Email NVARCHAR(200),
    @FullName NVARCHAR(200) = NULL,
    @Phone NVARCHAR(20) = NULL,
    @p_role_id INT = NULL,
    @p_company_id INT = NULL,
    @UserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if username already exists
    IF EXISTS (SELECT 1 FROM [dbo].[TabUserMaster] WHERE [Username] = @Username)
    BEGIN
        RAISERROR('Username already exists', 16, 1);
        RETURN;
    END
    
    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM [dbo].[TabUserMaster] WHERE [Email] = @Email)
    BEGIN
        RAISERROR('Email already registered', 16, 1);
        RETURN;
    END
    
    -- Insert new user
    INSERT INTO [dbo].[TabUserMaster] 
        ([Username], [PasswordHash], [Email], [FullName], [Phone], [IsActive])
    VALUES 
        (@Username, @PasswordHash, @Email, @FullName, @Phone, 1);
    
    -- Return the new user ID
    SET @UserId = SCOPE_IDENTITY();
    
    -- If role and company exist, map user role
    IF @p_role_id IS NOT NULL AND @p_company_id IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[TabUserRole] (UserId, RoleId, CompanyId)
        VALUES (@UserId, @p_role_id, @p_company_id);
    END
    
    -- Return the newly created user
    SELECT [UserId], [Username], [Email], [FullName], [Phone], [IsActive]
    FROM [dbo].[TabUserMaster]
    WHERE [UserId] = @UserId;
END
GO
