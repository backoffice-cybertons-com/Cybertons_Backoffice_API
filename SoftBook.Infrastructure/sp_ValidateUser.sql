IF OBJECT_ID('[dbo].[sp_ValidateUser]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_ValidateUser];
GO

CREATE PROCEDURE [dbo].[sp_ValidateUser]
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [UserId]
          ,[Username]
          ,[PasswordHash]
          ,[Email]
    FROM [dbo].[TabUserMaster]
    WHERE ([Username] = @Username OR [Email] = @Username)
      AND LTRIM(RTRIM([PasswordHash])) = LTRIM(RTRIM(@PasswordHash))
END
GO
