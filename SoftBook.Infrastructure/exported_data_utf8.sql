USE [SoftAccDb]
GO
SET IDENTITY_INSERT [dbo].[TabBranchMaster] ON 

INSERT [dbo].[TabBranchMaster] ([BranchId], [BranchName], [Address], [City], [State], [Country], [Phone], [IsActive]) VALUES (1, N'Main', N'909,SoftSense Technologies', N'Surat', N'Gujrati', N'India', N'9999988888', 1)
INSERT [dbo].[TabBranchMaster] ([BranchId], [BranchName], [Address], [City], [State], [Country], [Phone], [IsActive]) VALUES (2, N'Surat Rural', N'Olpad', N'Surat', N'Gujrat', N'India', N'02612554444', NULL)
INSERT [dbo].[TabBranchMaster] ([BranchId], [BranchName], [Address], [City], [State], [Country], [Phone], [IsActive]) VALUES (1002, N'Mumbai', N'Goregaon', N'Mumbai', N'Maharashtra', N'India', N'02612554444', NULL)
SET IDENTITY_INSERT [dbo].[TabBranchMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabWarehouseMaster] ON 

INSERT [dbo].[TabWarehouseMaster] ([WarehouseId], [WarehouseName], [BranchId], [Address]) VALUES (1, N'Palanpur Patiya WareHouse', 1, N'Palanpur Patiya Nr Lal Mandir ')
SET IDENTITY_INSERT [dbo].[TabWarehouseMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabAccountMaster] ON 

INSERT [dbo].[TabAccountMaster] ([AccountId], [AccountType], [AccountName], [IsSubAccount], [ParentAccountId], [AccountCode], [Description]) VALUES (2, N'Cost Of Goods Sold', N'Cash Account ', 0, NULL, N'8785989596', NULL)
SET IDENTITY_INSERT [dbo].[TabAccountMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPartyTypeMaster] ON 

INSERT [dbo].[TabPartyTypeMaster] ([PartyTypeId], [PartyTypeCode], [PartyTypeName], [Description], [IsCustomer], [IsVendor], [IsActive], [SortOrder], [CreatedOn], [PartyId], [PartyType]) VALUES (1, N'CUST', N'CUSTOMER', N'CUSTOMER', 1, 0, 1, 1, CAST(N'2026-01-08T17:03:02.203' AS DateTime), NULL, NULL)
INSERT [dbo].[TabPartyTypeMaster] ([PartyTypeId], [PartyTypeCode], [PartyTypeName], [Description], [IsCustomer], [IsVendor], [IsActive], [SortOrder], [CreatedOn], [PartyId], [PartyType]) VALUES (2, N'VEND', N'VENDOR', N'VENDOR', 0, 1, 1, 2, CAST(N'2026-01-08T17:03:46.577' AS DateTime), NULL, NULL)
INSERT [dbo].[TabPartyTypeMaster] ([PartyTypeId], [PartyTypeCode], [PartyTypeName], [Description], [IsCustomer], [IsVendor], [IsActive], [SortOrder], [CreatedOn], [PartyId], [PartyType]) VALUES (3, N'TRAD', N'TRADER', N'TRADER', 1, 1, 1, 3, CAST(N'2026-01-08T17:04:27.420' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[TabPartyTypeMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPartyMaster] ON 

INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (1, N'Jewellery', N'Mister', N'John', N'Doe', N'ABC ', N'ABC Jewellers', N'John@gmail.com', N'9865956569', N'9565656956', NULL, N'18', NULL, N'454545454545', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2025-12-10T15:52:39.433' AS DateTime), NULL, NULL)
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (4, N'Customer', NULL, N'Hetak', N'Modi', N'Softech', N'Hetak', N'hetakm@gmail.com', N'9574686754', N'9656965696', N'English', N'Consumer', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 0, N'www.softech.com', N'PURCHASE', N'GSSDGSG', CAST(N'2026-01-10T12:20:03.903' AS DateTime), NULL, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (5, N'Vendor', N'Mr.', N'Hetak', N'Modi', N'Softech', N'Hetak', N'hetakm@gmail.com', N'9574686754', N'9574686754', N'English', N'Consumer', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 0, N'www.softech.com', N'PURCHASE', N'SGA', CAST(N'2026-01-10T12:31:24.000' AS DateTime), 2, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (7, N'CUSTOMER', N'Mr.', N'Sanjay', N'Gogiya', N'Star Impex llp', N'Sanjay Gogiya', N'D@gmail.com', N'9574686754', N'9656965696', N'English', N'Regular', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, NULL, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 1, N'www.softech.com', N'PURCHASE', N'stringDSA', CAST(N'2026-01-10T16:08:31.273' AS DateTime), 1, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (8, N'Customer', N'Mr.', N'Pradeep', N'Patel', N'Softech', N'Hetak', N'hetakm@gmail.com', N'9574686754', N'9574686754', N'English', N'Consumer', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 1, N'book.softoncloud.com', N'PURCHASE', N'daf', CAST(N'2026-01-10T16:37:54.360' AS DateTime), 1, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (9, N'Customer', N'Mr.', N'Neha', N'Singh', N'StarMark', N'Hetak', N'hetakm@gmail.com', N'9574686754', N'9574686754', N'English', N'Regular', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 1, N'www.softech.com', N'PURCHASE', N'DFAFD', CAST(N'2026-01-10T17:03:14.650' AS DateTime), 1, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (10, N'VENDOR', N'Mr.', N'Sudeep', N'Singh', N'Star Impex', N'Pradeep', N'hetakm@gmail.com', N'9574686759', N'9656965698', N'English', N'Composition', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1, 2, 2, CAST(25000.00 AS Decimal(18, 2)), N'Head Office', N'1', 1, N'www.softech.com', N'PURCHASE', N'DFADF', CAST(N'2026-01-10T17:24:08.503' AS DateTime), 2, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (11, N'VENDOR', N'Mr.', N'Neha', N'Singh', N'Star Impex', N'Sudeep Gogiya', N'hetakm@gmail.com', N'9574686754', N'9574686754', N'English', N'Regular', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 0, N'www.softech.com', N'PURCHASE', N'46+454646khkh', CAST(N'2026-01-10T18:03:24.320' AS DateTime), 2, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (1004, N'Customer', N'Mr.', N'Deep', N'Shikha', N'Dipanjali Jewel', N'Dipanjali', N'dipanjali@gmail.com', N'9574686754', N'9656965696', N'English', N'Consumer', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 1003, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 1, N'www.softech.com', N'PURCHASE', N'456123', CAST(N'2026-01-12T17:52:07.560' AS DateTime), 1, N'123456789456')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (1005, N'CUSTOMER', N'Mr.', N'Neha', N'Singh', N'Dipanjali Jewel', N'Neha Singh', N'hetakm@gmail.com', N'9574686754', N'9574686754', N'English', N'Regular', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 4, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 1, N'www.softech.com', N'PURCHASE', N'faadsfasdasf
123456789', CAST(N'2026-01-13T11:39:30.900' AS DateTime), NULL, N'124545784545')
INSERT [dbo].[TabPartyMaster] ([PartyId], [PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId], [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms], [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn], [PartyTypeId], [GSTNO]) VALUES (1006, N'VENDOR', N'Mr.', N'Deep', N'Shikha', N'Dipanjali Jewel', N'Dipanjali', N'Deep@gmail.com', N'9574686754', N'9656965696', N'English', N'Composition', NULL, N'AMOPM9812X', NULL, N'Taxable', NULL, 3, 2, 2, CAST(15000.00 AS Decimal(18, 2)), N'Head Office', N'1', 0, N'www.softech.com', N'PURCHASE', N'dfafasfafasfasdf', CAST(N'2026-01-13T11:42:31.940' AS DateTime), NULL, N'124545784545')
SET IDENTITY_INSERT [dbo].[TabPartyMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTaxMaster] ON 

INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (3, N'CGST 0%', CAST(0.00 AS Decimal(5, 2)), N'CGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (4, N'CGST 2.5%', CAST(2.50 AS Decimal(5, 2)), N'CGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (5, N'CGST 6%', CAST(6.00 AS Decimal(5, 2)), N'CGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (6, N'CGST 9%', CAST(9.00 AS Decimal(5, 2)), N'CGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (7, N'CGST 14%', CAST(14.00 AS Decimal(5, 2)), N'CGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (8, N'SGST 0%', CAST(0.00 AS Decimal(5, 2)), N'SGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (9, N'SGST 2.5%', CAST(2.50 AS Decimal(5, 2)), N'SGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (10, N'SGST 6%', CAST(6.00 AS Decimal(5, 2)), N'SGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (11, N'SGST 9%', CAST(9.00 AS Decimal(5, 2)), N'SGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (12, N'SGST 14%', CAST(14.00 AS Decimal(5, 2)), N'SGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (13, N'IGST 0%', CAST(0.00 AS Decimal(5, 2)), N'IGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (14, N'IGST 5%', CAST(5.00 AS Decimal(5, 2)), N'IGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (15, N'IGST 12%', CAST(12.00 AS Decimal(5, 2)), N'IGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (16, N'IGST 18%', CAST(18.00 AS Decimal(5, 2)), N'IGST')
INSERT [dbo].[TabTaxMaster] ([TaxId], [TaxName], [RatePercent], [TaxType]) VALUES (17, N'IGST 28%', CAST(28.00 AS Decimal(5, 2)), N'IGST')
SET IDENTITY_INSERT [dbo].[TabTaxMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTransporterMaster] ON 

INSERT [dbo].[TabTransporterMaster] ([TransporterId], [TransporterName], [GSTNumber], [ContactPerson], [Phone], [Address]) VALUES (1, N'John Doe', N'89898989989', N'Johnny', N'9898989899', N'Pal')
INSERT [dbo].[TabTransporterMaster] ([TransporterId], [TransporterName], [GSTNumber], [ContactPerson], [Phone], [Address]) VALUES (2, N'XYZ', N'12121212121212', N'Hetak Modi', N'02612554444', N'909,SoftSense Technologies')
SET IDENTITY_INSERT [dbo].[TabTransporterMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabHSNMaster] ON 

INSERT [dbo].[TabHSNMaster] ([HSNId], [HSNCode], [Description], [TaxRate]) VALUES (1, N'565686', N'this is HSN demo', CAST(18.00 AS Decimal(5, 2)))
INSERT [dbo].[TabHSNMaster] ([HSNId], [HSNCode], [Description], [TaxRate]) VALUES (2, N'56568633', N'this is HSN demos', CAST(18.00 AS Decimal(5, 2)))
SET IDENTITY_INSERT [dbo].[TabHSNMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTaxGroupMaster] ON 

INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (1, N'Local', N'Local tax / Non-GST', N'NA', NULL, NULL, NULL)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (2, N'GST 0%', N'GST Exempt Supplies', N'INTRA', 0, 0, NULL)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (3, N'GST 5%', N'GST at 5%', N'INTRA', 2.5, 2.5, NULL)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (4, N'GST 12%', N'GST at 12%', N'INTRA', 6, 6, NULL)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (5, N'GST 18%', N'GST at 18%', N'INTRA', 9, 9, NULL)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (6, N'GST 28%', N'GST at 28%', N'INTRA', 14, 14, NULL)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (7, N'IGST Exempt', N'IGST Exempt Supplies', N'INTER', NULL, NULL, 0)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (8, N'IGST 5%  ', N'IGST at 5%', N'INTER', NULL, NULL, 5)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (9, N'IGST 12% ', N'IGST at 12%', N'INTER', NULL, NULL, 12)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (10, N'IGST 18% ', N'IGST at 18%', N'INTER', NULL, NULL, 18)
INSERT [dbo].[TabTaxGroupMaster] ([TaxGroupId], [GroupName], [Description], [SupplyType], [CGSTPer], [SGSTPer], [IGSTPer]) VALUES (11, N'IGST 28% ', N'IGST at 28%', N'INTER', NULL, NULL, 28)
SET IDENTITY_INSERT [dbo].[TabTaxGroupMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTaxGroupDetail] ON 

INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (1, 2, 3, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (2, 2, 8, 2)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (3, 3, 4, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (4, 3, 9, 2)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (5, 4, 5, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (6, 4, 10, 2)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (7, 5, 6, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (8, 5, 11, 2)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (9, 6, 7, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (10, 6, 12, 2)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (11, 7, 13, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (12, 8, 14, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (13, 9, 15, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (14, 10, 16, 1)
INSERT [dbo].[TabTaxGroupDetail] ([TaxGroupDetailId], [TaxGroupId], [TaxId], [ApplyOrder]) VALUES (15, 11, 17, 1)
SET IDENTITY_INSERT [dbo].[TabTaxGroupDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[TabUnitMaster] ON 

INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (1, N'Kilogram', N'KGS', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (2, N'Liter', N'LTR', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (3, N'Meter', N'MTR', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (5, N'Box', N'BOX', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (6, N'Centimeter', N'CMS', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (7, N'Dozen', N'DOZ', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (8, N'Feet', N'FT', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (9, N'Grams', N'G', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (10, N'Inches', N'IN', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (13, N'Pieces', N'PCS', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (14, N'Square Feet', N'SQF', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (1005, N'KARAT', N'KARAT', NULL, NULL)
INSERT [dbo].[TabUnitMaster] ([UnitId], [UnitName], [UQC], [CRT], [PackingUnits]) VALUES (2005, N'Dozone', N'12Pcs', NULL, NULL)
SET IDENTITY_INSERT [dbo].[TabUnitMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabSACMaster] ON 

INSERT [dbo].[TabSACMaster] ([SACId], [SACCode], [Description], [TaxRate]) VALUES (1, N'10000017', N'Newer SAC CODEss', CAST(182.00 AS Decimal(5, 2)))
INSERT [dbo].[TabSACMaster] ([SACId], [SACCode], [Description], [TaxRate]) VALUES (2, N'10111', N'demosac', CAST(100.00 AS Decimal(5, 2)))
SET IDENTITY_INSERT [dbo].[TabSACMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTaxPreferenceMaster] ON 

INSERT [dbo].[TabTaxPreferenceMaster] ([TaxPreferenceId], [PreferenceCode], [PreferenceName], [IsGSTApplicable], [IsActive]) VALUES (1, N'TAXABLE', N'Taxable', 1, 1)
INSERT [dbo].[TabTaxPreferenceMaster] ([TaxPreferenceId], [PreferenceCode], [PreferenceName], [IsGSTApplicable], [IsActive]) VALUES (2, N'NON_TAXABLE', N'Non-Taxable', 0, 1)
INSERT [dbo].[TabTaxPreferenceMaster] ([TaxPreferenceId], [PreferenceCode], [PreferenceName], [IsGSTApplicable], [IsActive]) VALUES (3, N'OUT_OF_SCOPE', N'Out of Scope', 0, 1)
INSERT [dbo].[TabTaxPreferenceMaster] ([TaxPreferenceId], [PreferenceCode], [PreferenceName], [IsGSTApplicable], [IsActive]) VALUES (4, N'NON_GST', N'Non-GST Supply', 0, 1)
SET IDENTITY_INSERT [dbo].[TabTaxPreferenceMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabProductGroupMaster] ON 

INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (1, N'General Trading', N'GEN', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (2, N'Milk & Dairy Products', N'MILK', N'DAIRY', NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (3, N'Bakery Items', N'BAK', N'BAKERY', NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (4, N'Groceries', N'GROC', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (5, N'Fruits & Vegetables', N'FNV', N'VEG', NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (6, N'Beverages', N'BEV', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (7, N'Confectionery', N'CONF', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (8, N'Stationery', N'STAT', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (9, N'Hardware & Tools', N'HARD', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (10, N'Electrical Items', N'ELEC', NULL, NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (11, N'Pharmaceuticals', N'PHAR', N'MEDICINE', NULL)
INSERT [dbo].[TabProductGroupMaster] ([ProductGroupId], [ProductGroupName], [ShortName], [AliasName], [TradingAccountId]) VALUES (12, N'Services', N'SERV', NULL, NULL)
SET IDENTITY_INSERT [dbo].[TabProductGroupMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabProductMaster] ON 

INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (1, N'Stationary', N'Pencil Box', CAST(120.00 AS Decimal(18, 2)), NULL, NULL, CAST(80.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2, N'Stationary', N'Eraser Bunch (10 Pcs)', CAST(100.00 AS Decimal(18, 2)), NULL, NULL, CAST(60.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (5, N'Stationary', N'Bond Paper A4 Size', CAST(500.00 AS Decimal(18, 2)), N'1111', N'Stationary Sale bond paper', CAST(410.00 AS Decimal(18, 2)), N'1112', N'New Stationary Purchase', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (6, N'Premium', N'Bond Paper A3 Size', CAST(500.00 AS Decimal(18, 2)), N'Mohit ', N'Nil', CAST(400.00 AS Decimal(18, 2)), N'Hetak', N'Nil', NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (7, N'Goods', N'Oats ', CAST(5000.00 AS Decimal(18, 2)), NULL, N'dwdwdw', CAST(4000.00 AS Decimal(18, 2)), NULL, N'lkjj', NULL, 1, 1, 1, 1, NULL, 1, 3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (1007, N'Service', N'LCD', CAST(8000.00 AS Decimal(18, 2)), NULL, N'dd', CAST(7600.00 AS Decimal(18, 2)), NULL, N'dd', NULL, 1, 1, 5, NULL, 2, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2007, N'Goods', N'Amul Gold ', CAST(50.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 2, 1, NULL, 1, 3, 2, 2, 5, 20, 100, NULL, NULL, 2, 4, 6, 8, 20, 50, 0, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2008, N'Goods', N'SUMUL MILK', CAST(50.00 AS Decimal(18, 2)), N'Cash Account ', NULL, NULL, NULL, NULL, NULL, 1, 0, 2, 1, NULL, 1, 5, 6, 2, 5, 60, 300, NULL, 2, 2, 4, 6, 8, 2, 12, 2, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2010, N'Service', N'SUMUL MILK', CAST(222.00 AS Decimal(18, 2)), N'Cash Account ', NULL, NULL, NULL, NULL, NULL, 1, 0, 13, NULL, 1, 1, NULL, 2, 10, 0, 0, 0, NULL, 2, 2, 3, 4, 5, 0, 0, 0, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2011, N'Goods', N'Oats sss', CAST(222.00 AS Decimal(18, 2)), N'Cash Account ', NULL, NULL, NULL, NULL, NULL, 1, 1, 10, 1, NULL, 1, 3, 9, 10, 10, 10, 100, NULL, 2, 22, 22, 22, 22, 10, 10, 0, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2012, N'Goods', N'Punjabi DAHI', CAST(100.00 AS Decimal(18, 2)), N'Cash Account ', NULL, NULL, NULL, NULL, NULL, 1, 0, 2, 1, NULL, 1, 5, 2, 2, 10, 20, 200, NULL, 2, 2, 3, 4, 5, 20, 20, 0, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2013, N'Goods', N'SUMUL MILKKK', CAST(11111.00 AS Decimal(18, 2)), N'Cash Account ', NULL, NULL, NULL, NULL, NULL, 1, 1, 3, 1, NULL, 1, 5, 11, 14, 10, 10, 100, NULL, 2, 1, 2, 2, 3, 10, 10, 0, NULL)
INSERT [dbo].[TabProductMaster] ([ProductId], [ProductType], [ProductName], [SellingPrice], [SalesAccount], [SalesDescription], [CostPrice], [PurchaseAccount], [PurchaseDescription], [PreferredVendorId], [Sellable], [Purchasable], [UnitId], [HSNId], [SACId], [TaxPreferenceId], [TaxGroupId], [ProductGroupId], [ProductPackingUnit], [OpeningQuantity], [OpeningPrice], [OpeningAmount], [PurchaseEffectedAccount], [SaleEffectedAccount], [SellingPrice2], [SellingPrice3], [SellingPrice4], [SellingPrice5], [MinQuantity], [MaxQuantity], [ReOrderQuantity], [TaxExemptionId]) VALUES (2014, N'Goods', N'SUMUL MILK', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 1005, 1, NULL, 1, 3, 2, 2, 1000, 5, 5000, NULL, NULL, NULL, NULL, NULL, NULL, 100, 400, 50, NULL)
SET IDENTITY_INSERT [dbo].[TabProductMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPartyContactPersons] ON 

INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (2, 9, N'Mr.', N'Rajan', N'Patel', N'rajanpatel@gmail.com', N'02612554445', N'9574686754', 0, CAST(N'2026-01-10T17:03:14.693' AS DateTime))
INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (1002, 1004, N'Mr.', N'Rajan', N'Patel', N'rajanpatel@gmail.com', N'02612554445', N'9574686754', 0, CAST(N'2026-01-12T17:52:07.573' AS DateTime))
INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (1009, 10, NULL, N'Mitesh', N'Patel', N'miteshpatel@gmail.com', N'02612554445', N'9574686754', 0, CAST(N'2026-01-13T11:24:37.947' AS DateTime))
INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (1013, 11, NULL, N'Rajan', N'Patel', N'rajanpatel@gmail.com', N'02612554445', N'9574686754', 0, CAST(N'2026-01-13T11:26:54.347' AS DateTime))
INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (1014, 7, NULL, N'Hetak', N'Modi', N'hetakm@yahoo.co.in', N'02612554445', N'02612553333', 0, CAST(N'2026-01-13T11:28:37.587' AS DateTime))
INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (1016, 1005, NULL, N'Mitesh', N'Patel', N'miteshpatel@gmail.com', N'02612554445', N'9574686754', 0, CAST(N'2026-01-13T11:40:00.557' AS DateTime))
INSERT [dbo].[TabPartyContactPersons] ([ContactPersonId], [PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile], [IsPrimary], [CreatedOn]) VALUES (1018, 1006, NULL, N'Mitesh', N'Modi', N'miteshpatel@gmail.com', N'02612554445', N'02612553333', 0, CAST(N'2026-01-13T11:43:01.770' AS DateTime))
SET IDENTITY_INSERT [dbo].[TabPartyContactPersons] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPartyBankDetails] ON 

INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (2, 9, N'HETAK MODI', N'BANK OF INDIA', N'270016310000016', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-10T17:03:14.690' AS DateTime))
INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (1002, 1004, N'HETAK MODI', N'SOUTH BANK OF INDIA', N'27000000000000', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-12T17:52:07.570' AS DateTime))
INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (1009, 10, N'HETAK MODI', N'SOUTH BANK OF INDIA', N'270016310000016', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-13T11:24:37.940' AS DateTime))
INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (1013, 11, N'HETAK MODI', N'BANK OF INDIA', N'270016310000016', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-13T11:26:54.347' AS DateTime))
INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (1014, 7, N'HETAK MODI', N'BANK OF INDIA', N'27001631000000', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-13T11:28:37.580' AS DateTime))
INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (1016, 1005, N'HETAK MODI', N'BANK OF INDIA', N'270016310000016', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-13T11:40:00.557' AS DateTime))
INSERT [dbo].[TabPartyBankDetails] ([BankId], [PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC], [Branch], [MICR], [IsDefault], [CreatedOn]) VALUES (1018, 1006, N'HETAK MODI', N'BANK OF INDIA', N'27001631000000', N'BKID0002700', NULL, NULL, 0, CAST(N'2026-01-13T11:43:01.767' AS DateTime))
SET IDENTITY_INSERT [dbo].[TabPartyBankDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[TabCurrencyMaster] ON 

INSERT [dbo].[TabCurrencyMaster] ([CurrencyId], [CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) VALUES (1, N'RS000', N'Rs', N'Indian Ruppee', 2, NULL)
INSERT [dbo].[TabCurrencyMaster] ([CurrencyId], [CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) VALUES (2, N'US000', N'US$', N'US DOLLAR', 2, N'0.00')
INSERT [dbo].[TabCurrencyMaster] ([CurrencyId], [CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) VALUES (3, N'DIN00', N'UED', N'UED', 0, N'0.00')
INSERT [dbo].[TabCurrencyMaster] ([CurrencyId], [CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) VALUES (4, N'AUD00', N'AUD', N'AUSTRAILIAN DOLLAR', NULL, N'0.00')
INSERT [dbo].[TabCurrencyMaster] ([CurrencyId], [CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) VALUES (1003, N'NZD', N'NZ$', N'New Zealand Dollar', 2, N'0.00')
SET IDENTITY_INSERT [dbo].[TabCurrencyMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabAddressTypeMaster] ON 

INSERT [dbo].[TabAddressTypeMaster] ([AddressTypeId], [AddressTypeCode], [AddressTypeName], [UsageCategory], [IsActive], [SortOrder], [CreatedOn]) VALUES (1, N'BILL', N'BILLING', N'BILLING', 1, 1, CAST(N'2025-12-30T11:33:08.577' AS DateTime))
INSERT [dbo].[TabAddressTypeMaster] ([AddressTypeId], [AddressTypeCode], [AddressTypeName], [UsageCategory], [IsActive], [SortOrder], [CreatedOn]) VALUES (2, N'SHIP', N'SHIPPING', N'SHIPPING', 1, 1, CAST(N'2025-12-30T11:33:22.673' AS DateTime))
SET IDENTITY_INSERT [dbo].[TabAddressTypeMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPartyAddress] ON 

INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (7, 9, 1, N'HETAK MODI', N'India', N'152 GRUHAM LUXURIA', N'OLPAD', N'SURAT', NULL, N'394540', N'026125544', N'02612554455', 0, 1, CAST(N'2026-01-10T17:03:14.683' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (8, 9, 2, N'HETAK MODI', N'India', N'152 GRUHAM LUXURIA', N'OLPAD', N'SURAT', NULL, N'394540', N'026125544', N'02612554455', 0, 1, CAST(N'2026-01-10T17:03:14.687' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1002, 1004, 1, N'HETAK MODI', N'India', N'152 GRUHAM LUXURIA', N'123', N'SURAT', NULL, N'394540', N'02612554445', N'02612554455', 0, 1, CAST(N'2026-01-12T17:52:07.567' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1003, 1004, 2, N'HETAK MODI', N'India', N'152 GRUHAM LUXURIA', N'123', N'SURAT', NULL, N'394540', N'02612554445', N'02612554455', 0, 1, CAST(N'2026-01-12T17:52:07.567' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1016, 10, 1, N'HETAK MODI', N'India', N'152 GRUHAM LUXURIA', N'OLPAD', N'SURAT', NULL, N'395010', N'02612554445', N'02612554455', 0, 1, CAST(N'2026-01-13T11:24:37.930' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1017, 10, 2, N'HETAK MODI', N'India', N'909 Softsense', N'Vesu', N'SURAT', NULL, N'395010', N'02612554445', N'02612554455', 0, 1, CAST(N'2026-01-13T11:24:37.933' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1024, 11, 1, N'GRUHAM LUXURIA LLP', N'India', N'152 GRUHAM LUX', N'ORMA MASMA', N'SURAT', NULL, N'395010', N'02612554445', N'02612554445', 0, 1, CAST(N'2026-01-13T11:26:54.343' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1025, 11, 2, N'GRUHAM LUXURIA Impex', N'India', N'SoftSense Tech. Pvt Ltd.', N'Vesu', N'SURAT', NULL, N'395010', N'02612554445', N'02612554445', 0, 1, CAST(N'2026-01-13T11:26:54.343' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1026, 7, 1, N'HETAK MODI', N'India', N'152 GRUHAM LUXURIA llp', N'ORMA MASMA', N'SURAT', NULL, N'394540', N'02612554445', N'02612554455', 0, 1, CAST(N'2026-01-13T11:28:37.577' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1027, 7, 2, N'HETAK MODI', N'India', N'909 SoftSense Technology Pvt Ltd.', N'Vesu', N'SURAT', NULL, N'395010', N'02612554445', N'02612554455', 0, 1, CAST(N'2026-01-13T11:28:37.577' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1030, 1005, 1, N'NehaSinghLLP', N'India', N'Near bahumali Athwagate', N'Athwalines', N'Surat', NULL, N'394540', N'02612554445', N'02612554445', 0, 1, CAST(N'2026-01-13T11:40:00.553' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1031, 1005, 2, N'NehaSinghLLP', N'India', N'Athwagate', N'Athwalines', N'Surat', NULL, N'394540', N'02612554445', N'02612554445', 0, 1, CAST(N'2026-01-13T11:40:00.553' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1034, 1006, 1, N'DeepShikhaLLP', N'India', N'Palanpur Patia lal mandir', N'Adajan', N'Surat', NULL, N'394540', N'026125544', N'02612554455', 0, 1, CAST(N'2026-01-13T11:43:01.760' AS DateTime))
INSERT [dbo].[TabPartyAddress] ([AddressId], [PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [IsActive], [CreatedOn]) VALUES (1035, 1006, 2, N'DeepShikhaLLP', N'India', N'Palanpur Patia', N'Adajan', N'Surat', NULL, N'394540', N'026125544', N'02612554455', 0, 1, CAST(N'2026-01-13T11:43:01.763' AS DateTime))
SET IDENTITY_INSERT [dbo].[TabPartyAddress] OFF
GO
SET IDENTITY_INSERT [dbo].[TabRoleMaster] ON 

INSERT [dbo].[TabRoleMaster] ([RoleId], [RoleName], [IsActive], [IsDeleted]) VALUES (1, N'Admin', 1, 0)
INSERT [dbo].[TabRoleMaster] ([RoleId], [RoleName], [IsActive], [IsDeleted]) VALUES (2, N'User', 1, 0)
INSERT [dbo].[TabRoleMaster] ([RoleId], [RoleName], [IsActive], [IsDeleted]) VALUES (3, N'Manager', 1, 0)
INSERT [dbo].[TabRoleMaster] ([RoleId], [RoleName], [IsActive], [IsDeleted]) VALUES (4, N'Demo', 1, 0)
SET IDENTITY_INSERT [dbo].[TabRoleMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[Tabs] ON 

INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (1, N'Home', N'/Dashboard/Home', NULL, N'fa-solid fa-gauge', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (2, N'Products', N'/Item/ItemList', NULL, N'fa-solid fa-boxes-stacked', 3, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (3, N'Sales', N'/Sale/SaleList', NULL, N'fa-solid fa-file-invoice-dollar', 4, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (4, N'Purchases', N'/Purchase/PurchaseList', NULL, N'fa-solid fa-cart-shopping', 5, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (5, N'Time Tracking', N'/TimeTracking/TrackList', NULL, N'fa-solid fa-clock', 6, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (6, N'Banking', N'/Banking/Baking', NULL, N'fa-solid fa-building-columns', 7, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (7, N'Filling & Compliance', N'/Compliance/Compliance', NULL, N'fa-solid fa-scale-balanced', 8, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (8, N'Accountant', N'/Accountant/AccountantList', NULL, N'fa-solid fa-user-tie', 9, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (9, N'Reports', N'/Reports/ReportList', NULL, N'fa-solid fa-chart-line', 10, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (10, N'Documents', N'/Documents/Documentation', NULL, N'fa-solid fa-folder-open', 11, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (11, N'Products', N'/Product', 2, N'fa-solid fa-box', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (12, N'Customers', N'/Sale/CustomersList', 3, N'fa-solid fa-user-group', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (13, N'Quotes', N'/Sale/QuotesList', 3, N'fa-solid fa-file-signature', 2, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (14, N'Retainer Invoices', N'/Sale/RetainerInvoiceList', 3, N'fa-solid fa-file-invoice', 3, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (15, N'Sales Orders', N'/Sale/SaleOrdersList', 3, N'fa-solid fa-bag-shopping', 4, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (16, N'Invoices', N'/Sale/InvoicesList', 3, N'fa-solid fa-file-invoice-dollar', 5, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (17, N'Recurring Invoices', N'/Sale/RecurringInvoicesList', 3, N'fa-solid fa-rotate', 6, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (18, N'Delivery Challans', N'/Sale/DeliveryChallansList', 3, N'fa-solid fa-truck', 7, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (19, N'Payments Received', N'/Sale/PaymentsReceivedList', 3, N'fa-solid fa-arrow-down-wide-short', 8, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (20, N'Credit Notes', N'/Sale/CreditNotesList', 3, N'fa-solid fa-receipt', 9, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (21, N'e-Way Bills', N'/Sale/eWayBillsList', 3, N'fa-solid fa-road', 10, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (22, N'Vendors', N'/Purchase/VendorsList', 4, N'fa-solid fa-store', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (23, N'Expenses', N'/Purchase/ExpensesList', 4, N'fa-solid fa-money-bill-wave', 2, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (24, N'Recurring Expenses', N'/Purchase/RecurringExpList', 4, N'fa-solid fa-arrows-rotate', 3, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (25, N'Purchase Orders', N'/Purchase/PurchaseOrdersList', 4, N'fa-solid fa-file-contract', 4, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (26, N'Bills', N'/Purchase/BillsList', 4, N'fa-solid fa-file-invoice', 5, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (27, N'Recurring Bills', N'/Purchase/RecurringBillList', 4, N'fa-solid fa-rotate', 6, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (28, N'Payments Made', N'/Purchase/PaymentsMadeList', 4, N'fa-solid fa-arrow-up-right-dots', 7, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (29, N'Vendor Credits', N'/Purchase/VendorCreditsList', 4, N'fa-solid fa-file-circle-check', 8, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (30, N'Projects', N'/TimeTracking/ProjectsList', 5, N'fa-solid fa-diagram-project', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (31, N'Timesheet', N'/TimeTracking/TimesheetList', 5, N'fa-solid fa-table-cells', 2, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (32, N'GST Filling', N'/Compliance/EnableGST', 7, N'fa-solid fa-receipt', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (33, N'Manual Journals', N'/Accountant/JournalList', 8, N'fa-solid fa-book', 1, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (34, N'Bulk Update', N'/Accountant/BulkUpdate', 8, N'fa-solid fa-layer-group', 2, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (35, N'Currency Adjustments', N'/Accountant/CurrencyAdj', 8, N'fa-solid fa-money-bill-transfer', 3, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (36, N'Chart of Accounts', N'/Accountant/AccountInfo', 8, N'fa-solid fa-list', 4, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (37, N'Budgets', N'/Accountant/BudgetList', 8, N'fa-solid fa-piggy-bank', 5, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (38, N'Transaction Locking', N'/Accountant/LockingList', 8, N'fa-solid fa-lock', 6, 1)
INSERT [dbo].[Tabs] ([TabId], [TabName], [TabUrl], [ParentId], [IconPath], [SortOrder], [IsActive]) VALUES (1011, N'Masters', N'/Pages/AccountSettings/Parameter', NULL, N'fa-solid fa-cogs
', 2, 1)
SET IDENTITY_INSERT [dbo].[Tabs] OFF
GO
SET IDENTITY_INSERT [dbo].[TabRolePermission] ON 

INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (1, 1, 1, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (2, 1, 2, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (3, 1, 3, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (4, 1, 4, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (5, 1, 5, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (6, 1, 6, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (7, 1, 7, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (8, 1, 8, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (9, 1, 9, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (10, 1, 10, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (11, 1, 11, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (12, 1, 12, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (13, 1, 13, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (14, 1, 14, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (15, 1, 15, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (16, 1, 16, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (17, 1, 17, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (18, 1, 18, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (19, 1, 19, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (20, 1, 20, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (21, 1, 21, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (22, 1, 22, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (23, 1, 23, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (24, 1, 24, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (25, 1, 25, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (26, 1, 26, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (27, 1, 27, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (28, 1, 28, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (29, 1, 29, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (30, 1, 30, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (31, 1, 31, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (32, 1, 32, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (33, 1, 33, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (34, 1, 34, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (35, 1, 35, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (36, 1, 36, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (37, 1, 37, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (38, 1, 38, 1, 1, 1, 1, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (39, 2, 1, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (40, 2, 2, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (41, 2, 11, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (42, 2, 3, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (43, 2, 12, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (44, 2, 13, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (45, 2, 15, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (46, 2, 16, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (47, 2, 18, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (48, 2, 19, 1, 1, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (49, 2, 4, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (50, 2, 22, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (51, 2, 26, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (52, 2, 5, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (53, 2, 30, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (54, 2, 31, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (55, 2, 10, 1, 1, 1, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (56, 3, 1, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (57, 3, 9, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (58, 4, 1, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (59, 4, 2, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (60, 4, 3, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (61, 4, 4, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (62, 4, 5, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (63, 4, 6, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (64, 4, 7, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (65, 4, 8, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (66, 4, 9, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (67, 4, 10, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (68, 4, 11, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (69, 4, 12, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (70, 4, 13, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (71, 4, 14, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (72, 4, 15, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (73, 4, 16, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (74, 4, 17, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (75, 4, 18, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (76, 4, 19, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (77, 4, 20, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (78, 4, 21, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (79, 4, 22, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (80, 4, 23, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (81, 4, 24, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (82, 4, 25, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (83, 4, 26, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (84, 4, 27, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (85, 4, 28, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (86, 4, 29, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (87, 4, 30, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (88, 4, 31, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (89, 4, 32, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (90, 4, 33, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (91, 4, 34, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (92, 4, 35, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (93, 4, 36, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (94, 4, 37, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (95, 4, 38, 1, 0, 0, 0, 1)
INSERT [dbo].[TabRolePermission] ([PermissionId], [RoleId], [TabId], [CanView], [CanAdd], [CanEdit], [CanDelete], [IsActive]) VALUES (1002, 1, 1011, 1, 1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[TabRolePermission] OFF
GO
SET IDENTITY_INSERT [dbo].[TabUserMaster] ON 

INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (1, N'Admin', N'O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=', N'Hetak Modi', N'hetakmodi@gmail.com', N'9574686754', 1, N'890039')
INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (2, N'User', N'O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=', N'Sanjay', N'sanjay@gmail.com', N'9574686754', 1, N'890039')
INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (3, N'Common', N'O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=', N'Raj', N'Raj@gmail.com', N'9574686754', 1, N'890039')
INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (4, N'Demo', N'062TFbe+XdU7MaJzs7Orpd7+cAgIMFqhajBit2ZYp5E=', N'Mohit', N'mohit@gmail.com', N'9574686754', 1, N'890039')
INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (1002, N'GAUTAM1', N'FeKw08M4keuw8e9gnsQZQgwg4yDOlMZfvIwzEkSOsiU=', NULL, N'gautam@gmail.com', NULL, 1, N'890039')
INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (1003, N'HETAK1', N'O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I=', NULL, N'hdmodi1987@gmail.com', NULL, 1, N'102')
INSERT [dbo].[TabUserMaster] ([UserId], [Username], [PasswordHash], [FullName], [Email], [Phone], [IsActive], [CompanyId]) VALUES (2006, N'Adminxx', N'3b612c75a7b5048a435fb6ec81e52ff92d6d795a8b5a9c17070f6a63c97a53b2', N'Admin kumar', N'admin@xxx.com', N'9898717856', 1, NULL)
SET IDENTITY_INSERT [dbo].[TabUserMaster] OFF
GO
INSERT [dbo].[TabUserRole] ([UserId], [RoleId]) VALUES (1, 1)
INSERT [dbo].[TabUserRole] ([UserId], [RoleId]) VALUES (2, 2)
INSERT [dbo].[TabUserRole] ([UserId], [RoleId]) VALUES (3, 3)
INSERT [dbo].[TabUserRole] ([UserId], [RoleId]) VALUES (4, 4)
GO
SET IDENTITY_INSERT [dbo].[TabProjectMaster] ON 

INSERT [dbo].[TabProjectMaster] ([ProjectId], [ProjectName], [StartDate], [EndDate], [Status], [Description]) VALUES (1, N'NewAcc', CAST(N'2025-11-01' AS Date), CAST(N'2026-06-01' AS Date), N'Running', N'Project Under Construction')
SET IDENTITY_INSERT [dbo].[TabProjectMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabGodownMaster] ON 

INSERT [dbo].[TabGodownMaster] ([GodownId], [WarehouseId], [GodownName]) VALUES (3, 1, N'Palanpur goddown')
INSERT [dbo].[TabGodownMaster] ([GodownId], [WarehouseId], [GodownName]) VALUES (6, NULL, N'VIP Road Godown')
SET IDENTITY_INSERT [dbo].[TabGodownMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabBankMaster] ON 

INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (1, N'Bank Of Indi', N'BKID0001', N'27001631000016', N'BKID0007', N'BKID225588')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (2, N'Bank Of Baroda', N'VIP Vesu Branch', N'2700163100001645', N'BRD0058', N'BRD989855')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (3, N'Axis Bank ', N'Bhagal Branch', N'11001100110011', N'46465465', N'4654654654')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (4, N'Hdfc Bank of Indi', N'VIP Vesu Branch', N'11001100110011', N'HDFC00785', N'0785')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (5, N'Bank Of Vadnagar', N'Vadnagar', N'27001631000000', N'BKID0000', N'9898')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (1005, N'South Indian Bank Ltd', N'Surat', N'2700163100001645', N'456456465', N'4654654654')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (2005, N'State Bank Of India', N'Lalgate', N'21005000300200', N'SBI000071', N'1221')
INSERT [dbo].[TabBankMaster] ([BankMasterId], [BankName], [Branch], [AccountNumber], [IFSC], [MICR]) VALUES (3005, N'State Bank Of Saurashtra', N'Rajkot', N'123456789', N'4656565', N'4564654')
SET IDENTITY_INSERT [dbo].[TabBankMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabCostCenterMaster] ON 

INSERT [dbo].[TabCostCenterMaster] ([CostCenterId], [CostCenterName], [Description]) VALUES (1, N'Demo1', N'etcetc')
INSERT [dbo].[TabCostCenterMaster] ([CostCenterId], [CostCenterName], [Description]) VALUES (2, N'demo2', N'dm')
INSERT [dbo].[TabCostCenterMaster] ([CostCenterId], [CostCenterName], [Description]) VALUES (1002, N'Demo4', N'123456')
SET IDENTITY_INSERT [dbo].[TabCostCenterMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabFiscalYearMaster] ON 

INSERT [dbo].[TabFiscalYearMaster] ([FiscalYearId], [StartDate], [EndDate], [IsCurrent]) VALUES (1, CAST(N'2025-01-04' AS Date), CAST(N'2026-01-03' AS Date), 1)
INSERT [dbo].[TabFiscalYearMaster] ([FiscalYearId], [StartDate], [EndDate], [IsCurrent]) VALUES (2, CAST(N'2024-04-01' AS Date), CAST(N'2025-03-01' AS Date), 0)
INSERT [dbo].[TabFiscalYearMaster] ([FiscalYearId], [StartDate], [EndDate], [IsCurrent]) VALUES (3, CAST(N'2023-04-01' AS Date), CAST(N'2024-03-30' AS Date), 1)
SET IDENTITY_INSERT [dbo].[TabFiscalYearMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPaymentModeMaster] ON 

INSERT [dbo].[TabPaymentModeMaster] ([PaymentModeId], [ModeName], [IsDefault]) VALUES (1, N'CASH', 1)
SET IDENTITY_INSERT [dbo].[TabPaymentModeMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabPaymentTermMaster] ON 

INSERT [dbo].[TabPaymentTermMaster] ([TermId], [TermName], [NumberOfDays]) VALUES (1, N'20 Days', 20)
INSERT [dbo].[TabPaymentTermMaster] ([TermId], [TermName], [NumberOfDays]) VALUES (2, N'1 Month', 30)
SET IDENTITY_INSERT [dbo].[TabPaymentTermMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabSalespersonMaster] ON 

INSERT [dbo].[TabSalespersonMaster] ([SalespersonId], [SalespersonName], [Email]) VALUES (1, N'KENIL KAKADIYA', N'kenil@gmail.com')
INSERT [dbo].[TabSalespersonMaster] ([SalespersonId], [SalespersonName], [Email]) VALUES (2, N'sanjay gogiya', N'sanjay123@gmail.com')
SET IDENTITY_INSERT [dbo].[TabSalespersonMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTCSMaster] ON 

INSERT [dbo].[TabTCSMaster] ([TCSId], [TaxName], [Rate], [NatureOfCollection], [IsHigherRate], [StartDate], [EndDate], [CreatedAt]) VALUES (5, N'GST', CAST(100.00 AS Decimal(5, 2)), N'CASH', 1, CAST(N'2025-12-10' AS Date), CAST(N'2025-12-11' AS Date), CAST(N'2025-12-10T16:47:37.840' AS DateTime))
SET IDENTITY_INSERT [dbo].[TabTCSMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[TabTDSMaster] ON 

INSERT [dbo].[TabTDSMaster] ([TDSId], [TaxName], [Rate], [Section], [IsHigherRate], [StartDate], [EndDate], [CreatedAt]) VALUES (1, N'GST', CAST(100.00 AS Decimal(5, 2)), N'demo', 0, CAST(N'2025-12-10' AS Date), CAST(N'2025-12-11' AS Date), CAST(N'2025-12-10T17:17:21.737' AS DateTime))
SET IDENTITY_INSERT [dbo].[TabTDSMaster] OFF
GO
