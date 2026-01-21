USE [SoftAccDb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Unified Product Master SP
IF OBJECT_ID('[dbo].[sp_Product_Master]', 'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_Product_Master]
GO

CREATE PROCEDURE [dbo].[sp_Product_Master]
    @Action varchar(20), -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID
    @ProductId int = NULL,
    @Type varchar(20) = NULL,
    @Name varchar(200) = NULL,
    @SellingPrice decimal(18,2) = NULL,
    @SalesAccount varchar(100) = NULL,
    @SalesDescription varchar(500) = NULL,
    @CostPrice decimal(18,2) = NULL,
    @PurchaseAccount varchar(100) = NULL,
    @PurchaseDescription varchar(500) = NULL,
    @PreferredVendorId int = NULL,
    @Sellable bit = NULL,
    @Purchasable bit = NULL,
    @IntraStateTax varchar(50) = NULL,
    @InterStateTax varchar(50) = NULL,
    @UnitId int = NULL,
    @HSNId int = NULL,
    @SACId int = NULL,
    @TaxPreference varchar(50) = NULL,
    @TaxId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabProductMaster]
               ([Type], [Name], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [IntraStateTax], [InterStateTax], [UnitId], [HSNId], [SACId], [TaxPreference], [TaxId])
        VALUES
               (@Type, @Name, @SellingPrice, @SalesAccount, @SalesDescription, @CostPrice, @PurchaseAccount, @PurchaseDescription, @PreferredVendorId, @Sellable, @Purchasable, @IntraStateTax, @InterStateTax, @UnitId, @HSNId, @SACId, @TaxPreference, @TaxId)
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabProductMaster]
        SET [Type] = @Type,
            [Name] = @Name,
            [SellingPrice] = @SellingPrice,
            [SalesAccount] = @SalesAccount,
            [SalesDescription] = @SalesDescription,
            [CostPrice] = @CostPrice,
            [PurchaseAccount] = @PurchaseAccount,
            [PurchaseDescription] = @PurchaseDescription,
            [PreferredVendorId] = @PreferredVendorId,
            [Sellable] = @Sellable,
            [Purchasable] = @Purchasable,
            [IntraStateTax] = @IntraStateTax,
            [InterStateTax] = @InterStateTax,
            [UnitId] = @UnitId,
            [HSNId] = @HSNId,
            [SACId] = @SACId,
            [TaxPreference] = @TaxPreference,
            [TaxId] = @TaxId
        WHERE [ProductId] = @ProductId
    END
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabProductMaster]
        WHERE [ProductId] = @ProductId
    END
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabProductMaster]
    END
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabProductMaster]
        WHERE [ProductId] = @ProductId
    END
END
GO

-- Drop old procedures if they exist (cleanup)
IF OBJECT_ID('[dbo].[sp_InsertProduct]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_InsertProduct]
IF OBJECT_ID('[dbo].[sp_UpdateProduct]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_UpdateProduct]
IF OBJECT_ID('[dbo].[sp_DeleteProduct]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_DeleteProduct]
IF OBJECT_ID('[dbo].[sp_GetAllProducts]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetAllProducts]
IF OBJECT_ID('[dbo].[sp_GetProductById]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_GetProductById]
GO
