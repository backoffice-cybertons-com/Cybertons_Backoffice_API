USE [SoftAccDb]
GO

IF OBJECT_ID('[dbo].[sp_Master_Universal]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[sp_Master_Universal]
GO

CREATE PROCEDURE [dbo].[sp_Master_Universal]
    @MasterName varchar(100),
    @Action varchar(20),
    @Id int = NULL,
    @JsonData nvarchar(max) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. TabAccountMaster
    IF @MasterName = 'Account' OR @MasterName = 'TabAccountMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabAccountMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabAccountMaster] WHERE [AccountId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabAccountMaster] ([AccountType], [AccountName], [IsSubAccount], [ParentAccountId], [AccountCode], [Description])
            SELECT [AccountType], [AccountName], [IsSubAccount], [ParentAccountId], [AccountCode], [Description] FROM OPENJSON(@JsonData) WITH ([AccountType] varchar(100), [AccountName] varchar(200), [IsSubAccount] bit, [ParentAccountId] int, [AccountCode] varchar(50), [Description] varchar(500))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[AccountType] = J.[AccountType], T.[AccountName] = J.[AccountName], T.[IsSubAccount] = J.[IsSubAccount], T.[ParentAccountId] = J.[ParentAccountId], T.[AccountCode] = J.[AccountCode], T.[Description] = J.[Description]
            FROM [dbo].[TabAccountMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([AccountType] varchar(100), [AccountName] varchar(200), [IsSubAccount] bit, [ParentAccountId] int, [AccountCode] varchar(50), [Description] varchar(500)) J WHERE T.[AccountId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabAccountMaster] WHERE [AccountId] = @Id
    END

    -- 2. TabAddressTypeMaster
    ELSE IF @MasterName = 'AddressType' OR @MasterName = 'TabAddressTypeMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabAddressTypeMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabAddressTypeMaster] WHERE [AddressTypeId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabAddressTypeMaster] ([TypeName], [Description]) SELECT [TypeName], [Description] FROM OPENJSON(@JsonData) WITH ([TypeName] varchar(100), [Description] varchar(200))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[TypeName] = J.[TypeName], T.[Description] = J.[Description] FROM [dbo].[TabAddressTypeMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([TypeName] varchar(100), [Description] varchar(200)) J WHERE T.[AddressTypeId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabAddressTypeMaster] WHERE [AddressTypeId] = @Id
    END

    -- 3. TabBankMaster
    ELSE IF @MasterName = 'Bank' OR @MasterName = 'TabBankMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabBankMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabBankMaster] WHERE [BankMasterId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabBankMaster] ([BankName], [Branch], [AccountNumber], [IFSC], [MICR]) SELECT [BankName], [Branch], [AccountNumber], [IFSC], [MICR] FROM OPENJSON(@JsonData) WITH ([BankName] varchar(200), [Branch] varchar(150), [AccountNumber] varchar(50), [IFSC] varchar(20), [MICR] varchar(20))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[BankName] = J.[BankName], T.[Branch] = J.[Branch], T.[AccountNumber] = J.[AccountNumber], T.[IFSC] = J.[IFSC], T.[MICR] = J.[MICR] FROM [dbo].[TabBankMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([BankName] varchar(200), [Branch] varchar(150), [AccountNumber] varchar(50), [IFSC] varchar(20), [MICR] varchar(20)) J WHERE T.[BankMasterId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabBankMaster] WHERE [BankMasterId] = @Id
    END

    -- 4. TabBranchMaster
    ELSE IF @MasterName = 'Branch' OR @MasterName = 'TabBranchMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabBranchMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabBranchMaster] WHERE [BranchId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabBranchMaster] ([BranchName], [Address], [City], [State], [Country], [Phone], [IsActive]) SELECT [BranchName], [Address], [City], [State], [Country], [Phone], [IsActive] FROM OPENJSON(@JsonData) WITH ([BranchName] varchar(150), [Address] varchar(300), [City] varchar(100), [State] varchar(100), [Country] varchar(100), [Phone] varchar(20), [IsActive] bit)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[BranchName] = J.[BranchName], T.[Address] = J.[Address], T.[City] = J.[City], T.[State] = J.[State], T.[Country] = J.[Country], T.[Phone] = J.[Phone], T.[IsActive] = J.[IsActive] FROM [dbo].[TabBranchMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([BranchName] varchar(150), [Address] varchar(300), [City] varchar(100), [State] varchar(100), [Country] varchar(100), [Phone] varchar(20), [IsActive] bit) J WHERE T.[BranchId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabBranchMaster] WHERE [BranchId] = @Id
    END

    -- 5. TabCostCenterMaster
    ELSE IF @MasterName = 'CostCenter' OR @MasterName = 'TabCostCenterMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabCostCenterMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabCostCenterMaster] WHERE [CostCenterId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabCostCenterMaster] ([CostCenterName], [Description]) SELECT [CostCenterName], [Description] FROM OPENJSON(@JsonData) WITH ([CostCenterName] varchar(150), [Description] varchar(300))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[CostCenterName] = J.[CostCenterName], T.[Description] = J.[Description] FROM [dbo].[TabCostCenterMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([CostCenterName] varchar(150), [Description] varchar(300)) J WHERE T.[CostCenterId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabCostCenterMaster] WHERE [CostCenterId] = @Id
    END

    -- 6. TabCurrencyMaster
    ELSE IF @MasterName = 'Currency' OR @MasterName = 'TabCurrencyMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabCurrencyMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabCurrencyMaster] WHERE [CurrencyId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabCurrencyMaster] ([CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) SELECT [CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format] FROM OPENJSON(@JsonData) WITH ([CurrencyCode] varchar(10), [CurrencySymbol] varchar(10), [CurrencyName] varchar(100), [DecimalPlaces] tinyint, [Format] varchar(50))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[CurrencyCode] = J.[CurrencyCode], T.[CurrencySymbol] = J.[CurrencySymbol], T.[CurrencyName] = J.[CurrencyName], T.[DecimalPlaces] = J.[DecimalPlaces], T.[Format] = J.[Format] FROM [dbo].[TabCurrencyMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([CurrencyCode] varchar(10), [CurrencySymbol] varchar(10), [CurrencyName] varchar(100), [DecimalPlaces] tinyint, [Format] varchar(50)) J WHERE T.[CurrencyId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabCurrencyMaster] WHERE [CurrencyId] = @Id
    END

    -- 7. TabFiscalYearMaster
    ELSE IF @MasterName = 'FiscalYear' OR @MasterName = 'TabFiscalYearMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabFiscalYearMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabFiscalYearMaster] WHERE [FiscalYearId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabFiscalYearMaster] ([StartDate], [EndDate], [IsCurrent]) SELECT [StartDate], [EndDate], [IsCurrent] FROM OPENJSON(@JsonData) WITH ([StartDate] date, [EndDate] date, [IsCurrent] bit)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[StartDate] = J.[StartDate], T.[EndDate] = J.[EndDate], T.[IsCurrent] = J.[IsCurrent] FROM [dbo].[TabFiscalYearMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([StartDate] date, [EndDate] date, [IsCurrent] bit) J WHERE T.[FiscalYearId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabFiscalYearMaster] WHERE [FiscalYearId] = @Id
    END

    -- 8. TabGodownMaster
    ELSE IF @MasterName = 'Godown' OR @MasterName = 'TabGodownMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabGodownMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabGodownMaster] WHERE [GodownId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabGodownMaster] ([WarehouseId], [GodownName]) SELECT [WarehouseId], [GodownName] FROM OPENJSON(@JsonData) WITH ([WarehouseId] int, [GodownName] varchar(150))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[WarehouseId] = J.[WarehouseId], T.[GodownName] = J.[GodownName] FROM [dbo].[TabGodownMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([WarehouseId] int, [GodownName] varchar(150)) J WHERE T.[GodownId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabGodownMaster] WHERE [GodownId] = @Id
    END

    -- 9. TabHSNMaster
    ELSE IF @MasterName = 'HSN' OR @MasterName = 'TabHSNMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabHSNMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabHSNMaster] WHERE [HSNId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabHSNMaster] ([HSNCode], [Description], [TaxRate]) SELECT [HSNCode], [Description], [TaxRate] FROM OPENJSON(@JsonData) WITH ([HSNCode] varchar(20), [Description] varchar(300), [TaxRate] decimal(18,2))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[HSNCode] = J.[HSNCode], T.[Description] = J.[Description], T.[TaxRate] = J.[TaxRate] FROM [dbo].[TabHSNMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([HSNCode] varchar(20), [Description] varchar(300), [TaxRate] decimal(18,2)) J WHERE T.[HSNId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabHSNMaster] WHERE [HSNId] = @Id
    END

    -- 10. TabPartyTypeMaster
    ELSE IF @MasterName = 'PartyType' OR @MasterName = 'TabPartyTypeMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabPartyTypeMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabPartyTypeMaster] WHERE [PartyTypeId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabPartyTypeMaster] ([PartyTypeCode], [PartyTypeName], [Description], [IsCustomer], [IsVendor], [IsActive], [SortOrder], [CreatedOn])
            SELECT [PartyTypeCode], [PartyTypeName], [Description], [IsCustomer], [IsVendor], [IsActive], [SortOrder], GETDATE() FROM OPENJSON(@JsonData) WITH ([PartyTypeCode] nvarchar(50), [PartyTypeName] nvarchar(200), [Description] nvarchar(max), [IsCustomer] bit, [IsVendor] bit, [IsActive] bit, [SortOrder] int)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[PartyTypeCode] = J.[PartyTypeCode], T.[PartyTypeName] = J.[PartyTypeName], T.[Description] = J.[Description], T.[IsCustomer] = J.[IsCustomer], T.[IsVendor] = J.[IsVendor], T.[IsActive] = J.[IsActive], T.[SortOrder] = J.[SortOrder]
            FROM [dbo].[TabPartyTypeMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([PartyTypeCode] nvarchar(50), [PartyTypeName] nvarchar(200), [Description] nvarchar(max), [IsCustomer] bit, [IsVendor] bit, [IsActive] bit, [SortOrder] int) J WHERE T.[PartyTypeId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabPartyTypeMaster] WHERE [PartyTypeId] = @Id
    END

    -- 11. TabPaymentModeMaster
    ELSE IF @MasterName = 'PaymentMode' OR @MasterName = 'TabPaymentModeMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabPaymentModeMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabPaymentModeMaster] WHERE [PaymentModeId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabPaymentModeMaster] ([ModeName], [IsDefault]) SELECT [ModeName], [IsDefault] FROM OPENJSON(@JsonData) WITH ([ModeName] varchar(100), [IsDefault] bit)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[ModeName] = J.[ModeName], T.[IsDefault] = J.[IsDefault] FROM [dbo].[TabPaymentModeMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([ModeName] varchar(100), [IsDefault] bit) J WHERE T.[PaymentModeId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabPaymentModeMaster] WHERE [PaymentModeId] = @Id
    END

    -- 12. TabPaymentTermMaster
    ELSE IF @MasterName = 'PaymentTerm' OR @MasterName = 'TabPaymentTermMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabPaymentTermMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabPaymentTermMaster] WHERE [PaymentTermId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabPaymentTermMaster] ([TermName], [Days]) SELECT [TermName], [Days] FROM OPENJSON(@JsonData) WITH ([TermName] varchar(100), [Days] int)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[TermName] = J.[TermName], T.[Days] = J.[Days] FROM [dbo].[TabPaymentTermMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([TermName] varchar(100), [Days] int) J WHERE T.[PaymentTermId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabPaymentTermMaster] WHERE [PaymentTermId] = @Id
    END

    -- 13. TabProductGroupMaster
    ELSE IF @MasterName = 'ProductGroup' OR @MasterName = 'TabProductGroupMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabProductGroupMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabProductGroupMaster] WHERE [ProductGroupId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabProductGroupMaster] ([GroupName], [Description]) SELECT [GroupName], [Description] FROM OPENJSON(@JsonData) WITH ([GroupName] varchar(100), [Description] varchar(200))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[GroupName] = J.[GroupName], T.[Description] = J.[Description] FROM [dbo].[TabProductGroupMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([GroupName] varchar(100), [Description] varchar(200)) J WHERE T.[ProductGroupId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabProductGroupMaster] WHERE [ProductGroupId] = @Id
    END

    -- 14. TabProjectMaster
    ELSE IF @MasterName = 'Project' OR @MasterName = 'TabProjectMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabProjectMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabProjectMaster] WHERE [ProjectId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabProjectMaster] ([ProjectName], [StartDate], [EndDate], [Status], [Description]) SELECT [ProjectName], [StartDate], [EndDate], [Status], [Description] FROM OPENJSON(@JsonData) WITH ([ProjectName] varchar(200), [StartDate] date, [EndDate] date, [Status] varchar(50), [Description] varchar(500))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[ProjectName] = J.[ProjectName], T.[StartDate] = J.[StartDate], T.[EndDate] = J.[EndDate], T.[Status] = J.[Status], T.[Description] = J.[Description] FROM [dbo].[TabProjectMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([ProjectName] varchar(200), [StartDate] date, [EndDate] date, [Status] varchar(50), [Description] varchar(500)) J WHERE T.[ProjectId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabProjectMaster] WHERE [ProjectId] = @Id
    END

    -- 15. TabRoleMaster
    ELSE IF @MasterName = 'Role' OR @MasterName = 'TabRoleMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabRoleMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabRoleMaster] WHERE [RoleId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabRoleMaster] ([RoleName], [Description]) SELECT [RoleName], [Description] FROM OPENJSON(@JsonData) WITH ([RoleName] varchar(100), [Description] varchar(250))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[RoleName] = J.[RoleName], T.[Description] = J.[Description] FROM [dbo].[TabRoleMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([RoleName] varchar(100), [Description] varchar(250)) J WHERE T.[RoleId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabRoleMaster] WHERE [RoleId] = @Id
    END

    -- 16. TabSACMaster
    ELSE IF @MasterName = 'SAC' OR @MasterName = 'TabSACMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabSACMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabSACMaster] WHERE [SACId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabSACMaster] ([SACCode], [Description], [TaxRate]) SELECT [SACCode], [Description], [TaxRate] FROM OPENJSON(@JsonData) WITH ([SACCode] varchar(20), [Description] varchar(300), [TaxRate] decimal(18,2))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[SACCode] = J.[SACCode], T.[Description] = J.[Description], T.[TaxRate] = J.[TaxRate] FROM [dbo].[TabSACMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([SACCode] varchar(20), [Description] varchar(300), [TaxRate] decimal(18,2)) J WHERE T.[SACId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabSACMaster] WHERE [SACId] = @Id
    END

    -- 17. TabSalespersonMaster
    ELSE IF @MasterName = 'Salesperson' OR @MasterName = 'TabSalespersonMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabSalespersonMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabSalespersonMaster] WHERE [SalespersonId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabSalespersonMaster] ([SalespersonName], [Email]) SELECT [SalespersonName], [Email] FROM OPENJSON(@JsonData) WITH ([SalespersonName] varchar(200), [Email] varchar(200))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[SalespersonName] = J.[SalespersonName], T.[Email] = J.[Email] FROM [dbo].[TabSalespersonMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([SalespersonName] varchar(200), [Email] varchar(200)) J WHERE T.[SalespersonId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabSalespersonMaster] WHERE [SalespersonId] = @Id
    END

    -- 18. TabTaxExemptionMaster
    ELSE IF @MasterName = 'TaxExemption' OR @MasterName = 'TabTaxExemptionMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTaxExemptionMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTaxExemptionMaster] WHERE [TaxExemptionId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTaxExemptionMaster] ([ExemptionName], [Description]) SELECT [ExemptionName], [Description] FROM OPENJSON(@JsonData) WITH ([ExemptionName] varchar(100), [Description] varchar(200))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[ExemptionName] = J.[ExemptionName], T.[Description] = J.[Description] FROM [dbo].[TabTaxExemptionMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([ExemptionName] varchar(100), [Description] varchar(200)) J WHERE T.[TaxExemptionId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTaxExemptionMaster] WHERE [TaxExemptionId] = @Id
    END

    -- 19. TabTaxGroupMaster
    ELSE IF @MasterName = 'TaxGroup' OR @MasterName = 'TabTaxGroupMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTaxGroupMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTaxGroupMaster] WHERE [TaxGroupId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTaxGroupMaster] ([GroupName], [Description]) SELECT [GroupName], [Description] FROM OPENJSON(@JsonData) WITH ([GroupName] varchar(100), [Description] varchar(300))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[GroupName] = J.[GroupName], T.[Description] = J.[Description] FROM [dbo].[TabTaxGroupMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([GroupName] varchar(100), [Description] varchar(300)) J WHERE T.[TaxGroupId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTaxGroupMaster] WHERE [TaxGroupId] = @Id
    END

    -- 20. TabTaxMaster
    ELSE IF @MasterName = 'Tax' OR @MasterName = 'TabTaxMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTaxMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTaxMaster] WHERE [TaxId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTaxMaster] ([TaxName], [RatePercent], [TaxType]) SELECT [TaxName], [RatePercent], [TaxType] FROM OPENJSON(@JsonData) WITH ([TaxName] varchar(200), [RatePercent] decimal(18,2), [TaxType] varchar(50))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[TaxName] = J.[TaxName], T.[RatePercent] = J.[RatePercent], T.[TaxType] = J.[TaxType] FROM [dbo].[TabTaxMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([TaxName] varchar(200), [RatePercent] decimal(18,2), [TaxType] varchar(50)) J WHERE T.[TaxId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTaxMaster] WHERE [TaxId] = @Id
    END

    -- 21. TabTaxPreferenceMaster
    ELSE IF @MasterName = 'TaxPreference' OR @MasterName = 'TabTaxPreferenceMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTaxPreferenceMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTaxPreferenceMaster] WHERE [TaxPreferenceId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTaxPreferenceMaster] ([PreferenceName], [Description]) SELECT [PreferenceName], [Description] FROM OPENJSON(@JsonData) WITH ([PreferenceName] varchar(100), [Description] varchar(200))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[PreferenceName] = J.[PreferenceName], T.[Description] = J.[Description] FROM [dbo].[TabTaxPreferenceMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([PreferenceName] varchar(100), [Description] varchar(200)) J WHERE T.[TaxPreferenceId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTaxPreferenceMaster] WHERE [TaxPreferenceId] = @Id
    END

    -- 22. TabTCSMaster
    ELSE IF @MasterName = 'TCS' OR @MasterName = 'TabTCSMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTCSMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTCSMaster] WHERE [TCSId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTCSMaster] ([Rate], [NatureOfCollection], [IsHigherRate], [StartDate], [EndDate], [CreatedAt])
            SELECT [Rate], [NatureOfCollection], [IsHigherRate], [StartDate], [EndDate], GETDATE() FROM OPENJSON(@JsonData) WITH ([Rate] decimal(18,2), [NatureOfCollection] varchar(100), [IsHigherRate] bit, [StartDate] date, [EndDate] date)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[Rate] = J.[Rate], T.[NatureOfCollection] = J.[NatureOfCollection], T.[IsHigherRate] = J.[IsHigherRate], T.[StartDate] = J.[StartDate], T.[EndDate] = J.[EndDate]
            FROM [dbo].[TabTCSMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([Rate] decimal(18,2), [NatureOfCollection] varchar(100), [IsHigherRate] bit, [StartDate] date, [EndDate] date) J WHERE T.[TCSId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTCSMaster] WHERE [TCSId] = @Id
    END

    -- 23. TabTDSMaster
    ELSE IF @MasterName = 'TDS' OR @MasterName = 'TabTDSMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTDSMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTDSMaster] WHERE [TDSId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTDSMaster] ([TaxName], [Rate], [Section], [IsHigherRate], [StartDate], [EndDate], [CreatedAt])
            SELECT [TaxName], [Rate], [Section], [IsHigherRate], [StartDate], [EndDate], GETDATE() FROM OPENJSON(@JsonData) WITH ([TaxName] varchar(100), [Rate] decimal(18,2), [Section] varchar(50), [IsHigherRate] bit, [StartDate] date, [EndDate] date)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[TaxName] = J.[TaxName], T.[Rate] = J.[Rate], T.[Section] = J.[Section], T.[IsHigherRate] = J.[IsHigherRate], T.[StartDate] = J.[StartDate], T.[EndDate] = J.[EndDate]
            FROM [dbo].[TabTDSMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([TaxName] varchar(100), [Rate] decimal(18,2), [Section] varchar(50), [IsHigherRate] bit, [StartDate] date, [EndDate] date) J WHERE T.[TDSId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTDSMaster] WHERE [TDSId] = @Id
    END

    -- 24. TabTransporterMaster
    ELSE IF @MasterName = 'Transporter' OR @MasterName = 'TabTransporterMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTransporterMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTransporterMaster] WHERE [TransporterId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTransporterMaster] ([TransporterName], [GSTNumber], [ContactPerson], [Phone], [Address]) SELECT [TransporterName], [GSTNumber], [ContactPerson], [Phone], [Address] FROM OPENJSON(@JsonData) WITH ([TransporterName] varchar(200), [GSTNumber] varchar(20), [ContactPerson] varchar(100), [Phone] varchar(20), [Address] varchar(300))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[TransporterName] = J.[TransporterName], T.[GSTNumber] = J.[GSTNumber], T.[ContactPerson] = J.[ContactPerson], T.[Phone] = J.[Phone], T.[Address] = J.[Address] FROM [dbo].[TabTransporterMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([TransporterName] varchar(200), [GSTNumber] varchar(20), [ContactPerson] varchar(100), [Phone] varchar(20), [Address] varchar(300)) J WHERE T.[TransporterId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTransporterMaster] WHERE [TransporterId] = @Id
    END

    -- 25. TabUnitMaster
    ELSE IF @MasterName = 'Unit' OR @MasterName = 'TabUnitMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabUnitMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabUnitMaster] WHERE [UnitId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabUnitMaster] ([UnitName], [UQC]) SELECT [UnitName], [UQC] FROM OPENJSON(@JsonData) WITH ([UnitName] varchar(20), [UQC] varchar(20))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[UnitName] = J.[UnitName], T.[UQC] = J.[UQC] FROM [dbo].[TabUnitMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([UnitName] varchar(20), [UQC] varchar(20)) J WHERE T.[UnitId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabUnitMaster] WHERE [UnitId] = @Id
    END

    -- 26. TabUserMaster
    ELSE IF @MasterName = 'User' OR @MasterName = 'TabUserMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT [UserId], [Username], [Email], [FullName], [Phone], [IsActive] FROM [dbo].[TabUserMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT [UserId], [Username], [Email], [FullName], [Phone], [IsActive] FROM [dbo].[TabUserMaster] WHERE [UserId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabUserMaster] ([Username], [Email], [FullName], [Phone], [IsActive], [PasswordHash])
            SELECT [Username], [Email], [FullName], [Phone], ISNULL([IsActive], 1), [PasswordHash] FROM OPENJSON(@JsonData) WITH ([Username] nvarchar(100), [Email] nvarchar(200), [FullName] nvarchar(200), [Phone] nvarchar(20), [IsActive] bit, [PasswordHash] nvarchar(256))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[Email] = J.[Email], T.[FullName] = J.[FullName], T.[Phone] = J.[Phone], T.[IsActive] = ISNULL(J.[IsActive], T.[IsActive])
            FROM [dbo].[TabUserMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([Email] nvarchar(200), [FullName] nvarchar(200), [Phone] nvarchar(20), [IsActive] bit) J WHERE T.[UserId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabUserMaster] WHERE [UserId] = @Id
    END

    -- 27. TabWarehouseMaster
    ELSE IF @MasterName = 'Warehouse' OR @MasterName = 'TabWarehouseMaster'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabWarehouseMaster]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabWarehouseMaster] WHERE [WarehouseId] = @Id
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabWarehouseMaster] ([WarehouseName], [BranchId], [Address]) SELECT [WarehouseName], [BranchId], [Address] FROM OPENJSON(@JsonData) WITH ([WarehouseName] varchar(150), [BranchId] int, [Address] varchar(300))
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[WarehouseName] = J.[WarehouseName], T.[BranchId] = J.[BranchId], T.[Address] = J.[Address] FROM [dbo].[TabWarehouseMaster] T CROSS APPLY OPENJSON(@JsonData) WITH ([WarehouseName] varchar(150), [BranchId] int, [Address] varchar(300)) J WHERE T.[WarehouseId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabWarehouseMaster] WHERE [WarehouseId] = @Id
    END

    -- 28. TabTaxGroupDetail (Special Handling for filtering)
    ELSE IF @MasterName = 'TaxGroupDetail' OR @MasterName = 'TabTaxGroupDetail'
    BEGIN
        IF @Action = 'SELECT' SELECT * FROM [dbo].[TabTaxGroupDetail]
        ELSE IF @Action = 'SELECT_BY_ID' SELECT * FROM [dbo].[TabTaxGroupDetail] WHERE [TaxGroupDetailId] = @Id
        ELSE IF @Action = 'SELECT_BY_FILTER' SELECT * FROM [dbo].[TabTaxGroupDetail] WHERE [TaxGroupId] = @Id -- Here Id works as filter
        ELSE IF @Action = 'INSERT' INSERT INTO [dbo].[TabTaxGroupDetail] ([TaxGroupId], [TaxId]) SELECT [TaxGroupId], [TaxId] FROM OPENJSON(@JsonData) WITH ([TaxGroupId] int, [TaxId] int)
        ELSE IF @Action = 'UPDATE' UPDATE T SET T.[TaxGroupId] = J.[TaxGroupId], T.[TaxId] = J.[TaxId] FROM [dbo].[TabTaxGroupDetail] T CROSS APPLY OPENJSON(@JsonData) WITH ([TaxGroupId] int, [TaxId] int) J WHERE T.[TaxGroupDetailId] = @Id
        ELSE IF @Action = 'DELETE' DELETE FROM [dbo].[TabTaxGroupDetail] WHERE [TaxGroupDetailId] = @Id
    END

    -- 29. Default / Error
    ELSE
    BEGIN
        RAISERROR('Master Name not supported or invalid action', 16, 1)
    END

END
GO
