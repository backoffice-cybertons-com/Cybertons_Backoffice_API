USE [SoftAccDb]
GO
/****** Object:  StoredProcedure [dbo].[USP_GetUserSidebarAndPermissions]    Script Date: 12-01-2026 3:37:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_GetUserSidebarAndPermissions]
(
    @UserId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    /* 
       1. Get the role for this user
    */
    DECLARE @RoleId INT;

    SELECT TOP 1 @RoleId = UR.RoleId
    FROM TabUserRole UR
    INNER JOIN TabRoleMaster RM ON UR.RoleId = RM.RoleId
    WHERE UR.UserId = @UserId
      AND RM.IsActive = 1
      AND RM.IsDeleted = 0;

    IF @RoleId IS NULL
    BEGIN
        RAISERROR('No active role assigned to this user.', 16, 1);
        RETURN;
    END;

    /*
       2. Fetch sidebar + permissions
    */
    SELECT  
        RM.RoleId,
        RM.RoleName,

        T.TabId,
        T.TabName,
        T.TabUrl,
        T.ParentId,
        T.IconPath,
        T.SortOrder,

        P.CanView,
        P.CanAdd,
        P.CanEdit,
        P.CanDelete
    FROM TabRolePermission P
    INNER JOIN Tabs T ON P.TabId = T.TabId
    INNER JOIN TabRoleMaster RM ON P.RoleId = RM.RoleId
    WHERE 
        P.RoleId = @RoleId
        AND P.CanView = 1
        AND P.IsActive = 1
        AND T.IsActive = 1
        AND RM.IsActive = 1
        AND RM.IsDeleted = 0
    ORDER BY 
        T.ParentId,
        T.SortOrder,
        T.TabName;
END
