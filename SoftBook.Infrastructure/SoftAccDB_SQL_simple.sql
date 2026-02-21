/****** Object:  Table [dbo].[TabAccountMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabAccountMaster](
	[AccountId] [int] IDENTITY(1,1) NOT NULL,
	[AccountType] [varchar](100) NULL,
	[AccountName] [varchar](200) NULL,
	[IsSubAccount] [bit] NOT NULL,
	[ParentAccountId] [int] NULL,
	[AccountCode] [varchar](50) NULL,
	[Description] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabAddressTypeMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabAddressTypeMaster](
	[AddressTypeId] [int] IDENTITY(1,1) NOT NULL,
	[AddressTypeCode] [varchar](30) NOT NULL,
	[AddressTypeName] [varchar](100) NOT NULL,
	[UsageCategory] [varchar](30) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AddressTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabBankMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabBankMaster](
	[BankMasterId] [int] IDENTITY(1,1) NOT NULL,
	[BankName] [varchar](200) NOT NULL,
	[Branch] [varchar](150) NULL,
	[AccountNumber] [varchar](50) NULL,
	[IFSC] [varchar](20) NULL,
	[MICR] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[BankMasterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabBill]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabBill](
	[BillId] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NOT NULL,
	[LocationId] [int] NOT NULL,
	[BillNumber] [varchar](20) NOT NULL,
	[OrderNumber] [varchar](50) NULL,
	[BillDate] [date] NOT NULL,
	[DueDate] [date] NOT NULL,
	[PaymentTerms] [varchar](50) NOT NULL,
	[AccountsPayableId] [int] NOT NULL,
	[ReverseCharge] [bit] NULL,
	[Subject] [varchar](300) NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[BillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabBillItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabBillItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[BillId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[AccountId] [int] NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[TaxId] [int] NULL,
	[CustomerId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[TDS_TCS] [varchar](5) NULL,
	[Adjustment] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabBillPayment]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabBillPayment](
	[BillPaymentID] [int] NULL,
	[VendorName] [nvarchar](200) NULL,
	[Location] [nvarchar](100) NULL,
	[PaymentNumber] [nvarchar](50) NULL,
	[PaymentMade] [decimal](18, 2) NULL,
	[PaymentDate] [date] NULL,
	[PaymentMode] [nvarchar](100) NULL,
	[PaidThrough] [nvarchar](200) NULL,
	[ReferenceNumber] [nvarchar](200) NULL,
	[Notes] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabBranchMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabBranchMaster](
	[BranchId] [int] IDENTITY(1,1) NOT NULL,
	[BranchName] [varchar](150) NOT NULL,
	[Address] [varchar](300) NULL,
	[City] [varchar](100) NULL,
	[State] [varchar](100) NULL,
	[Country] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[BranchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabBudget]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabBudget](
	[BudgetId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[FiscalYear] [varchar](20) NOT NULL,
	[BudgetPeriod] [varchar](20) NOT NULL,
	[LocationId] [int] NULL,
	[IncludeAssetLiabilityEquity] [bit] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BudgetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabCostCenterMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabCostCenterMaster](
	[CostCenterId] [int] IDENTITY(1,1) NOT NULL,
	[CostCenterName] [varchar](150) NOT NULL,
	[Description] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[CostCenterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabCreditNotes]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabCreditNotes](
	[CreditNoteId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[LocationId] [int] NULL,
	[Reason] [varchar](200) NULL,
	[CreditNoteNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[CreditNoteDate] [date] NOT NULL,
	[SalespersonId] [int] NULL,
	[Notes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
	[Subject ] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[CreditNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabCreditNotesItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabCreditNotesItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[CreditNoteId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[AccountId] [int] NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[TaxId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[TDS_TCS] [varchar](5) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabCurrencyAdjustment]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabCurrencyAdjustment](
	[AdjustmentId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[AdjustmentDate] [date] NOT NULL,
	[ExchangeRate] [decimal](18, 6) NOT NULL,
	[ToCurrency] [varchar](10) NOT NULL,
	[Notes] [varchar](500) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[AdjustmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabCurrencyMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabCurrencyMaster](
	[CurrencyId] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCode] [varchar](10) NULL,
	[CurrencySymbol] [varchar](10) NULL,
	[CurrencyName] [varchar](100) NULL,
	[DecimalPlaces] [tinyint] NULL,
	[Format] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[CurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabCustomerAdvance]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabCustomerAdvance](
	[CustomerAdvanceID] [int] NULL,
	[CustomerName] [nvarchar](200) NULL,
	[PlaceOfSupply] [nvarchar](100) NULL,
	[Location] [nvarchar](100) NULL,
	[DescriptionOfSupply] [nvarchar](max) NULL,
	[AmountReceived] [decimal](18, 2) NULL,
	[BankCharges] [decimal](18, 2) NULL,
	[Tax] [nvarchar](100) NULL,
	[PaymentDate] [date] NULL,
	[PaymentNumber] [nvarchar](50) NULL,
	[PaymentMode] [nvarchar](100) NULL,
	[DepositTo] [nvarchar](200) NULL,
	[ReferenceNumber] [nvarchar](200) NULL,
	[Notes] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabDeliveryChallan]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabDeliveryChallan](
	[DeliveryChallanId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[LocationId] [int] NULL,
	[ChallanNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[ChallanDate] [date] NOT NULL,
	[ChallanType] [varchar](50) NOT NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
	[Warehouse location ] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[DeliveryChallanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabDeliveryChallanItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabDeliveryChallanItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[DeliveryChallanId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[TaxId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[Adjustment] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabEmployee]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabEmployee](
	[EmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeName] [varchar](200) NULL,
	[Email] [varchar](200) NULL,
	[Phone] [varchar](20) NULL,
	[Department] [varchar](100) NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabEWayBills]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabEWayBills](
	[EWayBillId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [varchar](50) NOT NULL,
	[TransactionType] [varchar](50) NOT NULL,
	[TransactionSubType] [varchar](50) NOT NULL,
	[LocationId] [int] NULL,
	[CustomerId] [int] NULL,
	[InvoiceNo] [varchar](20) NOT NULL,
	[Date] [date] NOT NULL,
	[DispatchFrom] [varchar](200) NULL,
	[PlaceOfDelivery] [varchar](200) NULL,
	[DistanceKm] [int] NOT NULL,
	[TransporterId] [int] NULL,
	[Vehicle Type] [varchar](50) NOT NULL,
	[VehicleNo] [varchar](50) NULL,
	[TransporterDocNo] [varchar](50) NULL,
	[TransporterDocDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[EWayBillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabEWayBillsItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabEWayBillsItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[EWayBillId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[HSN Code] [int] NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Taxable Amount] [decimal](18, 2) NULL,
	[IGST] [int] NULL,
	[Cess] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabFiscalYearMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabFiscalYearMaster](
	[FiscalYearId] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[IsCurrent] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[FiscalYearId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabGodownMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabGodownMaster](
	[GodownId] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseId] [int] NULL,
	[GodownName] [varchar](150) NULL,
PRIMARY KEY CLUSTERED 
(
	[GodownId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabHSNMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabHSNMaster](
	[HSNId] [int] IDENTITY(1,1) NOT NULL,
	[HSNCode] [varchar](20) NULL,
	[Description] [varchar](300) NULL,
	[TaxRate] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[HSNId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabInvoice]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabInvoice](
	[InvoiceId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[LocationId] [int] NULL,
	[InvoiceNumber] [varchar](20) NOT NULL,
	[OrderNumber] [varchar](50) NULL,
	[InvoiceDate] [date] NOT NULL,
	[DueDate] [date] NULL,
	[PaymentTerms] [varchar](50) NOT NULL,
	[AccountsReceivable] [int] NULL,
	[SalespersonId] [int] NULL,
	[Subject] [varchar](300) NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
	[Warehouse location ] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabInvoiceItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabInvoiceItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[TaxId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[TDS_TCS] [varchar](5) NULL,
	[Adjustment] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabInvoicePayment]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabInvoicePayment](
	[InvoicePaymentID] [int] NULL,
	[CustomerName] [nvarchar](200) NULL,
	[Location] [nvarchar](100) NULL,
	[AmountReceived] [decimal](18, 2) NULL,
	[BankCharges] [decimal](18, 2) NULL,
	[PaymentDate] [date] NULL,
	[PaymentNumber] [nvarchar](50) NULL,
	[PaymentMode] [nvarchar](100) NULL,
	[DepositTo] [nvarchar](200) NULL,
	[ReferenceNumber] [nvarchar](200) NULL,
	[TaxDeducted] [bit] NULL,
	[TDSTaxAmountId] [int] NULL,
	[Notes] [nvarchar](max) NULL,
	[ReferenceNo] [varchar](50) NULL,
	[NotesText] [varchar](500) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabJournal]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabJournal](
	[JournalId] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NULL,
	[JournalDate] [date] NOT NULL,
	[JournalNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[Notes] [varchar](500) NULL,
	[ReportingMethod] [varchar](20) NOT NULL,
	[Currency] [varchar](10) NOT NULL,
	[Attachments ] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[JournalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabJournalLines]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabJournalLines](
	[LineId] [int] IDENTITY(1,1) NOT NULL,
	[JournalId] [int] NULL,
	[AccountId] [int] NULL,
	[Description] [varchar](200) NULL,
	[ContactId] [int] NULL,
	[Debit] [decimal](18, 2) NULL,
	[Credit] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[LineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabLedgerSettings]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabLedgerSettings](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[AutoNumbering] [bit] NULL,
	[DefaultCurrency] [varchar](10) NULL,
	[EnableTCS] [bit] NULL,
	[EnableTDS] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabLocation]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabLocation](
	[LocationId] [int] IDENTITY(1,1) NOT NULL,
	[LocationName] [varchar](200) NOT NULL,
	[BranchId] [int] NULL,
	[WarehouseId] [int] NULL,
	[Address] [varchar](300) NULL,
	[City] [varchar](100) NULL,
	[State] [varchar](100) NULL,
	[Country] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPartyAddress]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPartyAddress](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[PartyId] [int] NOT NULL,
	[AddressTypeId] [int] NOT NULL,
	[Attention] [varchar](200) NULL,
	[Country] [varchar](100) NULL,
	[AddressLine1] [varchar](200) NULL,
	[AddressLine2] [varchar](200) NULL,
	[City] [varchar](100) NULL,
	[State] [varchar](100) NULL,
	[PinCode] [varchar](20) NULL,
	[Phone] [varchar](20) NULL,
	[Fax] [varchar](20) NULL,
	[IsDefault] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_TabPartyAddress] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPartyBankDetails]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPartyBankDetails](
	[BankId] [int] IDENTITY(1,1) NOT NULL,
	[PartyId] [int] NOT NULL,
	[AccountHolderName] [varchar](200) NULL,
	[BankName] [varchar](200) NULL,
	[AccountNumber] [varchar](50) NULL,
	[IFSC] [varchar](20) NULL,
	[Branch] [varchar](200) NULL,
	[MICR] [varchar](20) NULL,
	[IsDefault] [bit] NULL,
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPartyContactPersons]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPartyContactPersons](
	[ContactPersonId] [int] IDENTITY(1,1) NOT NULL,
	[PartyId] [int] NOT NULL,
	[Salutation] [varchar](20) NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[Email] [varchar](200) NULL,
	[WorkPhone] [varchar](20) NULL,
	[Mobile] [varchar](20) NULL,
	[IsPrimary] [bit] NULL,
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ContactPersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPartyMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPartyMaster](
	[PartyId] [int] IDENTITY(1,1) NOT NULL,
	[PartyType] [varchar](20) NOT NULL,
	[Salutation] [varchar](20) NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[CompanyName] [varchar](200) NULL,
	[DisplayName] [varchar](200) NOT NULL,
	[Email] [varchar](200) NULL,
	[PhoneWork] [varchar](20) NULL,
	[PhoneMobile] [varchar](20) NULL,
	[PreferredLanguage] [varchar](50) NULL,
	[GSTTreatment] [varchar](100) NULL,
	[PlaceOfSupply] [varchar](100) NULL,
	[PAN] [varchar](20) NULL,
	[MSMERegistered] [bit] NULL,
	[TaxPreference] [varchar](20) NULL,
	[TDS] [varchar](50) NULL,
	[CurrencyId] [int] NULL,
	[AccountsReceivableAccount] [int] NULL,
	[AccountsPayableAccount] [int] NULL,
	[OpeningBalance] [decimal](18, 2) NULL,
	[OpeningBalanceType] [varchar](20) NULL,
	[PaymentTerms] [varchar](50) NULL,
	[EnablePortal] [bit] NULL,
	[WebsiteURL] [varchar](200) NULL,
	[Department] [varchar](100) NULL,
	[Remarks] [varchar](max) NULL,
	[CreatedOn] [datetime] NULL,
	[PartyTypeId] [int] NULL,
	[GSTNO] [varchar](40) NULL,
 CONSTRAINT [PK__TabParty__1640CD33B91B5682] PRIMARY KEY CLUSTERED 
(
	[PartyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPartyReportingTags]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPartyReportingTags](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[PartyId] [int] NOT NULL,
	[TagName] [varchar](100) NULL,
	[ReportingTagId] [int] NULL,
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPartyTypeMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPartyTypeMaster](
	[PartyTypeId] [int] IDENTITY(1,1) NOT NULL,
	[PartyTypeCode] [varchar](20) NOT NULL,
	[PartyTypeName] [varchar](100) NOT NULL,
	[Description] [varchar](250) NULL,
	[IsCustomer] [bit] NOT NULL,
	[IsVendor] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[PartyId] [int] NULL,
	[PartyType] [varchar](20) NULL,
 CONSTRAINT [PK_TabPartyTypeMaster] PRIMARY KEY CLUSTERED 
(
	[PartyTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPaymentModeMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPaymentModeMaster](
	[PaymentModeId] [int] IDENTITY(1,1) NOT NULL,
	[ModeName] [varchar](100) NULL,
	[IsDefault] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentModeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPaymentTermMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPaymentTermMaster](
	[TermId] [int] IDENTITY(1,1) NOT NULL,
	[TermName] [varchar](100) NULL,
	[NumberOfDays] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TermId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabProductGroupMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabProductGroupMaster](
	[ProductGroupId] [int] IDENTITY(1,1) NOT NULL,
	[ProductGroupName] [varchar](200) NOT NULL,
	[ShortName] [varchar](50) NULL,
	[AliasName] [varchar](100) NULL,
	[TradingAccountId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabProductMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabProductMaster](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[ProductType] [varchar](20) NULL,
	[ProductName] [varchar](200) NULL,
	[SellingPrice] [decimal](18, 2) NULL,
	[SalesAccount] [varchar](100) NULL,
	[SalesDescription] [varchar](500) NULL,
	[CostPrice] [decimal](18, 2) NULL,
	[PurchaseAccount] [varchar](100) NULL,
	[PurchaseDescription] [varchar](500) NULL,
	[PreferredVendorId] [int] NULL,
	[Sellable] [bit] NULL,
	[Purchasable] [bit] NULL,
	[UnitId] [int] NULL,
	[HSNId] [int] NULL,
	[SACId] [int] NULL,
	[TaxPreferenceId] [int] NULL,
	[TaxGroupId] [int] NULL,
	[ProductGroupId] [int] NULL,
	[ProductPackingUnit] [int] NULL,
	[OpeningQuantity] [float] NULL,
	[OpeningPrice] [float] NULL,
	[OpeningAmount] [float] NULL,
	[PurchaseEffectedAccount] [int] NULL,
	[SaleEffectedAccount] [int] NULL,
	[SellingPrice2] [float] NULL,
	[SellingPrice3] [float] NULL,
	[SellingPrice4] [float] NULL,
	[SellingPrice5] [float] NULL,
	[MinQuantity] [float] NULL,
	[MaxQuantity] [float] NULL,
	[ReOrderQuantity] [float] NULL,
	[TaxExemptionId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabProjectMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabProjectMaster](
	[ProjectId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [varchar](200) NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Status] [varchar](50) NULL,
	[Description] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPurchaseOrder]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPurchaseOrder](
	[PurchaseOrderId] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NOT NULL,
	[LocationId] [int] NOT NULL,
	[CustomerId] [int] NULL,
	[PurchaseOrderNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[Date] [date] NOT NULL,
	[DeliveryDate] [date] NULL,
	[ShipmentPreference] [varchar](100) NULL,
	[PaymentTerms] [varchar](50) NOT NULL,
	[ReverseCharge] [bit] NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabPurchaseOrderItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabPurchaseOrderItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[AccountId] [int] NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[TaxId] [int] NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[TDS_TCS] [varchar](5) NULL,
	[Adjustment] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabQuote]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabQuote](
	[QuoteId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[LocationId] [int] NULL,
	[QuoteNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[QuoteDate] [date] NOT NULL,
	[ExpiryDate] [date] NULL,
	[SalespersonId] [int] NULL,
	[Subject] [varchar](300) NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
	[AutoCreateRetainer] [bit] NULL,
	[PercentageToBeCollected] [decimal](8, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[QuoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabQuoteItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabQuoteItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[QuoteId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[TaxId] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[TDS_TCS] [varchar](5) NULL,
	[Adjustment] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecordExpense]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecordExpense](
	[ExpenseId] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NOT NULL,
	[ExpenseDate] [date] NOT NULL,
	[ExpenseAccountId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaidThroughAccountId] [int] NOT NULL,
	[ExpenseType] [varchar](20) NOT NULL,
	[SAC] [varchar](20) NULL,
	[VendorId] [int] NULL,
	[GSTTreatment] [varchar](50) NOT NULL,
	[SourceOfSupply] [varchar](50) NOT NULL,
	[DestinationOfSupply] [varchar](50) NOT NULL,
	[ReverseCharge] [bit] NULL,
	[TaxId] [int] NULL,
	[AmountIs] [varchar](20) NOT NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[Notes] [varchar](500) NULL,
	[CustomerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ExpenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecordMileage]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecordMileage](
	[MileageId] [int] IDENTITY(1,1) NOT NULL,
	[LocationId] [int] NOT NULL,
	[MileageDate] [date] NOT NULL,
	[EmployeeId] [int] NULL,
	[CalculationMethod] [varchar](20) NOT NULL,
	[DistanceKm] [decimal](10, 2) NULL,
	[OdometerStart] [decimal](10, 2) NULL,
	[OdometerEnd] [decimal](10, 2) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaidThroughAccountId] [int] NOT NULL,
	[VendorId] [int] NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[Notes] [varchar](500) NULL,
	[CustomerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[MileageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecurringBill]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecurringBill](
	[RecurringBillID] [int] NULL,
	[VendorID] [int] NULL,
	[Location] [nvarchar](100) NULL,
	[ProfileName] [nvarchar](200) NULL,
	[RepeatEvery] [nvarchar](50) NULL,
	[StartOn] [date] NULL,
	[EndsOn] [date] NULL,
	[NeverExpires] [bit] NULL,
	[AccountsPayable] [nvarchar](200) NULL,
	[PaymentTerms] [nvarchar](200) NULL,
	[ReverseChargeApplicable] [bit] NULL,
	[WarehouseLocation] [nvarchar](200) NULL,
	[TransactionLevel] [nvarchar](100) NULL,
	[SubTotal] [decimal](18, 2) NULL,
	[Discount] [decimal](18, 2) NULL,
	[TDS] [nvarchar](100) NULL,
	[Adjustment] [decimal](18, 2) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[Notes] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecurringBillItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecurringBillItems](
	[ItemID] [int] NULL,
	[RecurringBillID] [int] NULL,
	[ItemDetails] [nvarchar](300) NULL,
	[Account] [nvarchar](200) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[Rate] [decimal](18, 2) NULL,
	[Tax] [nvarchar](100) NULL,
	[CustomerDetails] [nvarchar](200) NULL,
	[Amount] [decimal](18, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecurringExpense]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecurringExpense](
	[RecurringExpenseId] [int] IDENTITY(1,1) NOT NULL,
	[ProfileName] [varchar](150) NOT NULL,
	[LocationId] [int] NOT NULL,
	[RepeatEvery] [varchar](20) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndsOn] [date] NULL,
	[NeverExpires] [bit] NULL,
	[ExpenseAccountId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaidThroughAccountId] [int] NOT NULL,
	[ExpenseType] [varchar](20) NOT NULL,
	[SAC] [varchar](20) NULL,
	[VendorId] [int] NULL,
	[GSTTreatment] [varchar](50) NOT NULL,
	[SourceOfSupply] [varchar](50) NOT NULL,
	[DestinationOfSupply] [varchar](50) NOT NULL,
	[ReverseCharge] [bit] NULL,
	[TaxId] [int] NULL,
	[AmountIs] [varchar](20) NOT NULL,
	[Notes] [varchar](500) NULL,
	[CustomerId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RecurringExpenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecurringInvoice]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecurringInvoice](
	[RecurringInvoiceID] [int] NULL,
	[CustomerID] [int] NULL,
	[Location] [varchar](100) NULL,
	[ProfileName] [varchar](150) NULL,
	[OrderNumber] [varchar](50) NULL,
	[RepeatEvery] [varchar](20) NULL,
	[StartOn] [date] NULL,
	[EndsOn] [date] NULL,
	[PaymentTerms] [varchar](50) NULL,
	[AccountsReceivable] [int] NULL,
	[SalespersonID] [int] NULL,
	[Subject] [varchar](250) NULL,
	[Warehouse location ] [varchar](250) NULL,
	[CustomerNotes] [varchar](max) NULL,
	[TermsConditions] [varchar](max) NULL,
	[RoundOff] [decimal](10, 2) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRecurringInvoiceItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRecurringInvoiceItems](
	[ItemID] [int] NULL,
	[RecurringInvoiceID] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[Quantity] [decimal](10, 2) NULL,
	[Rate] [decimal](10, 2) NULL,
	[TaxID] [int] NULL,
	[CustomerDetailsID] [int] NULL,
	[Amount] [decimal](10, 2) NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[TDS_TCS] [varchar](5) NULL,
	[Adjustment] [decimal](18, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRetainerInvoice]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRetainerInvoice](
	[RetainerInvoiceId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[LocationId] [int] NULL,
	[RetainerInvoiceNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[InvoiceDate] [date] NOT NULL,
	[ProjectId] [int] NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[RetainerInvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRetainerInvoiceItems]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRetainerInvoiceItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[RetainerInvoiceId] [int] NULL,
	[Description] [varchar](200) NULL,
	[Amount] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRoleMaster]    Script Date: 27-01-2026 6:15:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRoleMaster](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabRolePermission]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabRolePermission](
	[PermissionId] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NULL,
	[TabId] [int] NOT NULL,
	[CanView] [bit] NULL,
	[CanAdd] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanDelete] [bit] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK__TabRoleP__EFA6FB2FC2E381E9] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tabs]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tabs](
	[TabId] [int] IDENTITY(1,1) NOT NULL,
	[TabName] [nvarchar](50) NOT NULL,
	[TabUrl] [nvarchar](255) NULL,
	[ParentId] [int] NULL,
	[IconPath] [nvarchar](255) NULL,
	[SortOrder] [int] NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TabId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabSACMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabSACMaster](
	[SACId] [int] IDENTITY(1,1) NOT NULL,
	[SACCode] [varchar](20) NULL,
	[Description] [varchar](300) NULL,
	[TaxRate] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[SACId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabSalesOrder]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabSalesOrder](
	[SalesOrderId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[LocationId] [int] NULL,
	[SalesOrderNumber] [varchar](20) NOT NULL,
	[ReferenceNo] [varchar](50) NULL,
	[SalesOrderDate] [date] NOT NULL,
	[ExpectedShipmentDate] [date] NULL,
	[PaymentTerms] [varchar](50) NOT NULL,
	[SalespersonId] [int] NULL,
	[DeliveryMethod] [varchar](100) NULL,
	[CustomerNotes] [varchar](500) NULL,
	[Terms] [varchar](max) NULL,
	[Warehouse location ] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[SalesOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabSalesOrderItems]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabSalesOrderItems](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[SalesOrderId] [int] NULL,
	[ItemDetails] [varchar](200) NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[TaxId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[DiscountPct] [decimal](5, 2) NULL,
	[TDS_TCS] [varchar](5) NULL,
	[Adjustment] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabSalespersonMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabSalespersonMaster](
	[SalespersonId] [int] IDENTITY(1,1) NOT NULL,
	[SalespersonName] [varchar](200) NULL,
	[Email] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[SalespersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTaxExemptionMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTaxExemptionMaster](
	[TaxExemptionId] [int] IDENTITY(1,1) NOT NULL,
	[ExemptionReason] [varchar](200) NOT NULL,
	[Description] [varchar](500) NULL,
	[ExemptionType] [varchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_TabTaxExemptionMaster] PRIMARY KEY CLUSTERED 
(
	[TaxExemptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTaxGroupDetail]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTaxGroupDetail](
	[TaxGroupDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TaxGroupId] [int] NOT NULL,
	[TaxId] [int] NOT NULL,
	[ApplyOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TaxGroupDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTaxGroupMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTaxGroupMaster](
	[TaxGroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [varchar](200) NOT NULL,
	[Description] [varchar](300) NULL,
	[SupplyType] [varchar](20) NULL,
	[CGSTPer] [float] NULL,
	[SGSTPer] [float] NULL,
	[IGSTPer] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[TaxGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTaxMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTaxMaster](
	[TaxId] [int] IDENTITY(1,1) NOT NULL,
	[TaxName] [varchar](200) NULL,
	[RatePercent] [decimal](5, 2) NULL,
	[TaxType] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[TaxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTaxPreferenceMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTaxPreferenceMaster](
	[TaxPreferenceId] [int] IDENTITY(1,1) NOT NULL,
	[PreferenceCode] [varchar](30) NOT NULL,
	[PreferenceName] [varchar](100) NOT NULL,
	[IsGSTApplicable] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TaxPreferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PreferenceCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTCSMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTCSMaster](
	[TCSId] [int] IDENTITY(1,1) NOT NULL,
	[TaxName] [varchar](100) NOT NULL,
	[Rate] [decimal](5, 2) NOT NULL,
	[NatureOfCollection] [varchar](100) NOT NULL,
	[IsHigherRate] [bit] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TCSId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTDSMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTDSMaster](
	[TDSId] [int] IDENTITY(1,1) NOT NULL,
	[TaxName] [varchar](100) NOT NULL,
	[Rate] [decimal](5, 2) NOT NULL,
	[Section] [varchar](50) NOT NULL,
	[IsHigherRate] [bit] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TDSId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabTransporterMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabTransporterMaster](
	[TransporterId] [int] IDENTITY(1,1) NOT NULL,
	[TransporterName] [varchar](200) NULL,
	[GSTNumber] [varchar](20) NULL,
	[ContactPerson] [varchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Address] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[TransporterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabUnitMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabUnitMaster](
	[UnitId] [int] IDENTITY(1,1) NOT NULL,
	[UnitName] [varchar](20) NULL,
	[UQC] [varchar](20) NULL,
	[CRT] [bigint] NULL,
	[PackingUnits] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[UnitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabUserMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabUserMaster](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](100) NULL,
	[PasswordHash] [varchar](256) NULL,
	[FullName] [varchar](200) NULL,
	[Email] [varchar](200) NULL,
	[Phone] [varchar](20) NULL,
	[IsActive] [bit] NULL,
	[CompanyId] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabUserRole]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabUserRole](
	[UserId] [int] NULL,
	[RoleId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabVendorAdvanced]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabVendorAdvanced](
	[VendorAdvanceID] [int] NULL,
	[VendorName] [nvarchar](200) NULL,
	[GSTTreatment] [nvarchar](100) NULL,
	[SourceOfSupply] [nvarchar](50) NULL,
	[DestinationOfSupply] [nvarchar](50) NULL,
	[Location] [nvarchar](100) NULL,
	[PaymentNumber] [nvarchar](50) NULL,
	[DescriptionOfSupply] [nvarchar](max) NULL,
	[PaymentMade] [decimal](18, 2) NULL,
	[ReverseCharge] [bit] NULL,
	[TDS] [nvarchar](50) NULL,
	[PaymentDate] [date] NULL,
	[PaymentMode] [nvarchar](100) NULL,
	[PaidThrough] [nvarchar](200) NULL,
	[DepositTo] [nvarchar](200) NULL,
	[ReferenceNumber] [nvarchar](200) NULL,
	[Notes] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabVendorCredit]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabVendorCredit](
	[VendorCreditID] [int] NULL,
	[VendorName] [nvarchar](200) NULL,
	[Location] [nvarchar](100) NULL,
	[CreditNoteNumber] [nvarchar](50) NULL,
	[OrderNumber] [nvarchar](100) NULL,
	[VendorCreditDate] [date] NULL,
	[Subject] [nvarchar](300) NULL,
	[AccountsPayable] [nvarchar](100) NULL,
	[ReverseCharge] [bit] NULL,
	[Notes] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TabWarehouseMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TabWarehouseMaster](
	[WarehouseId] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseName] [varchar](150) NOT NULL,
	[BranchId] [int] NULL,
	[Address] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[Account]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Account] AS
SELECT AccountId, AccountName, AccountType FROM TabAccountMaster;
GO
/****** Object:  View [dbo].[AccountsPayable]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AccountsPayable] AS
SELECT AccountId AS AccountsPayableId, AccountName 
FROM TabAccountMaster WHERE AccountType='Liability';
GO
/****** Object:  View [dbo].[AccountsReceivableRef]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AccountsReceivableRef] AS
SELECT AccountId AS AccountsReceivable, AccountName
FROM TabAccountMaster WHERE AccountType='Asset';
GO
/****** Object:  View [dbo].[Customer]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Customer View
CREATE VIEW [dbo].[Customer] AS
SELECT PartyId AS CustomerId, * FROM TabPartyMaster WHERE PartyType='Customer';
GO
/****** Object:  View [dbo].[ExpenseAccount]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ExpenseAccount View
CREATE VIEW [dbo].[ExpenseAccount] AS
SELECT AccountId AS ExpenseAccountId, AccountName
FROM TabAccountMaster WHERE AccountType='Expense';
GO
/****** Object:  View [dbo].[HSNCodeRef]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- HSN Reference
CREATE VIEW [dbo].[HSNCodeRef] AS
SELECT HSNCode FROM TabHSNMaster;
GO
/****** Object:  View [dbo].[IGSTRef]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- IGST Reference View
CREATE VIEW [dbo].[IGSTRef] AS
SELECT TaxId AS IGST FROM TabTaxMaster WHERE TaxType='IGST';
GO
/****** Object:  View [dbo].[PaidThroughAccount]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- PaidThroughAccount View
CREATE VIEW [dbo].[PaidThroughAccount] AS
SELECT AccountId AS PaidThroughAccountId, AccountName
FROM TabAccountMaster WHERE AccountType IN ('Cash','Bank');
GO
/****** Object:  View [dbo].[SACRef]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- SAC Reference
CREATE VIEW [dbo].[SACRef] AS
SELECT SACCode AS SAC FROM TabSACMaster;
GO
/****** Object:  View [dbo].[Vendor]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Vendor] AS
SELECT PartyId AS VendorId, * FROM TabPartyMaster WHERE PartyType='Vendor';
GO
/****** Object:  Index [UX_TaxGroup_Tax]    Script Date: 27-01-2026 6:15:12 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_TaxGroup_Tax] ON [dbo].[TabTaxGroupDetail]
(
	[TaxGroupId] ASC,
	[TaxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TabAccountMaster] ADD  CONSTRAINT [DF_TabAccount_IsSubAccount]  DEFAULT ((0)) FOR [IsSubAccount]
GO
ALTER TABLE [dbo].[TabAddressTypeMaster] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabAddressTypeMaster] ADD  DEFAULT ((1)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[TabAddressTypeMaster] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabBranchMaster] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabCurrencyAdjustment] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[TabEmployee] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabFiscalYearMaster] ADD  DEFAULT ((0)) FOR [IsCurrent]
GO
ALTER TABLE [dbo].[TabPartyAddress] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[TabPartyAddress] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabPartyAddress] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabPartyBankDetails] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[TabPartyBankDetails] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabPartyContactPersons] ADD  DEFAULT ((0)) FOR [IsPrimary]
GO
ALTER TABLE [dbo].[TabPartyContactPersons] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabPartyMaster] ADD  CONSTRAINT [DF__TabPartyM__Creat__20C1E124]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabPartyReportingTags] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabPartyTypeMaster] ADD  CONSTRAINT [DF_TabPartyTypeMaster_IsCustomer]  DEFAULT ((0)) FOR [IsCustomer]
GO
ALTER TABLE [dbo].[TabPartyTypeMaster] ADD  CONSTRAINT [DF_TabPartyTypeMaster_IsVendor]  DEFAULT ((0)) FOR [IsVendor]
GO
ALTER TABLE [dbo].[TabPartyTypeMaster] ADD  CONSTRAINT [DF_TabPartyTypeMaster_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabPartyTypeMaster] ADD  CONSTRAINT [DF_TabPartyTypeMaster_SortOrder]  DEFAULT ((1)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[TabPartyTypeMaster] ADD  CONSTRAINT [DF_TabPartyTypeMaster_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabRoleMaster] ADD  CONSTRAINT [DF_TabRoleMaster_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabRoleMaster] ADD  CONSTRAINT [DF_TabRoleMaster_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TabRolePermission] ADD  CONSTRAINT [DF_TabRolePermission_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Tabs] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabTaxExemptionMaster] ADD  CONSTRAINT [DF_TaxExemption_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabTaxExemptionMaster] ADD  CONSTRAINT [DF_TaxExemption_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[TabTaxPreferenceMaster] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TabTCSMaster] ADD  CONSTRAINT [DF__TabTCSMas__IsHig__32E0915F]  DEFAULT ((0)) FOR [IsHigherRate]
GO
ALTER TABLE [dbo].[TabTCSMaster] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[TabTDSMaster] ADD  CONSTRAINT [DF__TabTDSMas__IsHig__36B12243]  DEFAULT ((0)) FOR [IsHigherRate]
GO
ALTER TABLE [dbo].[TabTDSMaster] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[TabAccountMaster]  WITH CHECK ADD FOREIGN KEY([ParentAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabBill]  WITH CHECK ADD FOREIGN KEY([AccountsPayableId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabBill]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabBill]  WITH CHECK ADD  CONSTRAINT [FK__TabBill__VendorI__2FCF1A8A] FOREIGN KEY([VendorId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabBill] CHECK CONSTRAINT [FK__TabBill__VendorI__2FCF1A8A]
GO
ALTER TABLE [dbo].[TabBillItems]  WITH CHECK ADD FOREIGN KEY([AccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabBillItems]  WITH CHECK ADD FOREIGN KEY([BillId])
REFERENCES [dbo].[TabBill] ([BillId])
GO
ALTER TABLE [dbo].[TabBillItems]  WITH CHECK ADD  CONSTRAINT [FK__TabBillIt__Custo__37703C52] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabBillItems] CHECK CONSTRAINT [FK__TabBillIt__Custo__37703C52]
GO
ALTER TABLE [dbo].[TabBillItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabCreditNotes]  WITH CHECK ADD  CONSTRAINT [FK__TabCredit__Custo__05D8E0BE] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabCreditNotes] CHECK CONSTRAINT [FK__TabCredit__Custo__05D8E0BE]
GO
ALTER TABLE [dbo].[TabCreditNotes]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabCreditNotesItems]  WITH CHECK ADD FOREIGN KEY([AccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabCreditNotesItems]  WITH CHECK ADD FOREIGN KEY([CreditNoteId])
REFERENCES [dbo].[TabCreditNotes] ([CreditNoteId])
GO
ALTER TABLE [dbo].[TabCreditNotesItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabCurrencyAdjustment]  WITH CHECK ADD FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[TabCurrencyMaster] ([CurrencyId])
GO
ALTER TABLE [dbo].[TabDeliveryChallan]  WITH CHECK ADD  CONSTRAINT [FK__TabDelive__Custo__55F4C372] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabDeliveryChallan] CHECK CONSTRAINT [FK__TabDelive__Custo__55F4C372]
GO
ALTER TABLE [dbo].[TabDeliveryChallan]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabDeliveryChallanItems]  WITH CHECK ADD FOREIGN KEY([DeliveryChallanId])
REFERENCES [dbo].[TabDeliveryChallan] ([DeliveryChallanId])
GO
ALTER TABLE [dbo].[TabDeliveryChallanItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabEWayBills]  WITH CHECK ADD  CONSTRAINT [FK__TabEWayBi__Custo__0F624AF8] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabEWayBills] CHECK CONSTRAINT [FK__TabEWayBi__Custo__0F624AF8]
GO
ALTER TABLE [dbo].[TabEWayBills]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabEWayBills]  WITH CHECK ADD FOREIGN KEY([TransporterId])
REFERENCES [dbo].[TabTransporterMaster] ([TransporterId])
GO
ALTER TABLE [dbo].[TabEWayBillsItems]  WITH CHECK ADD FOREIGN KEY([EWayBillId])
REFERENCES [dbo].[TabEWayBills] ([EWayBillId])
GO
ALTER TABLE [dbo].[TabEWayBillsItems]  WITH CHECK ADD FOREIGN KEY([HSN Code])
REFERENCES [dbo].[TabHSNMaster] ([HSNId])
GO
ALTER TABLE [dbo].[TabEWayBillsItems]  WITH CHECK ADD FOREIGN KEY([IGST])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabGodownMaster]  WITH CHECK ADD FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[TabWarehouseMaster] ([WarehouseId])
GO
ALTER TABLE [dbo].[TabInvoice]  WITH CHECK ADD FOREIGN KEY([AccountsReceivable])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabInvoice]  WITH CHECK ADD  CONSTRAINT [FK__TabInvoic__Custo__5D95E53A] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabInvoice] CHECK CONSTRAINT [FK__TabInvoic__Custo__5D95E53A]
GO
ALTER TABLE [dbo].[TabInvoice]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabInvoiceItems]  WITH CHECK ADD FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[TabInvoice] ([InvoiceId])
GO
ALTER TABLE [dbo].[TabInvoiceItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabJournal]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabJournalLines]  WITH CHECK ADD FOREIGN KEY([AccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabJournalLines]  WITH CHECK ADD FOREIGN KEY([JournalId])
REFERENCES [dbo].[TabJournal] ([JournalId])
GO
ALTER TABLE [dbo].[TabLocation]  WITH CHECK ADD FOREIGN KEY([BranchId])
REFERENCES [dbo].[TabBranchMaster] ([BranchId])
GO
ALTER TABLE [dbo].[TabLocation]  WITH CHECK ADD FOREIGN KEY([WarehouseId])
REFERENCES [dbo].[TabWarehouseMaster] ([WarehouseId])
GO
ALTER TABLE [dbo].[TabPartyAddress]  WITH CHECK ADD  CONSTRAINT [FK_TabPartyAddress_AddressType] FOREIGN KEY([AddressTypeId])
REFERENCES [dbo].[TabAddressTypeMaster] ([AddressTypeId])
GO
ALTER TABLE [dbo].[TabPartyAddress] CHECK CONSTRAINT [FK_TabPartyAddress_AddressType]
GO
ALTER TABLE [dbo].[TabPartyAddress]  WITH CHECK ADD  CONSTRAINT [FK_TabPartyAddress_Party] FOREIGN KEY([PartyId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabPartyAddress] CHECK CONSTRAINT [FK_TabPartyAddress_Party]
GO
ALTER TABLE [dbo].[TabPartyBankDetails]  WITH CHECK ADD  CONSTRAINT [FK__TabPartyB__Party__2C3393D0] FOREIGN KEY([PartyId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabPartyBankDetails] CHECK CONSTRAINT [FK__TabPartyB__Party__2C3393D0]
GO
ALTER TABLE [dbo].[TabPartyContactPersons]  WITH CHECK ADD  CONSTRAINT [FK__TabPartyC__Party__267ABA7A] FOREIGN KEY([PartyId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabPartyContactPersons] CHECK CONSTRAINT [FK__TabPartyC__Party__267ABA7A]
GO
ALTER TABLE [dbo].[TabPartyMaster]  WITH CHECK ADD  CONSTRAINT [FK_TabPartyMaster_PartyType] FOREIGN KEY([PartyTypeId])
REFERENCES [dbo].[TabPartyTypeMaster] ([PartyTypeId])
GO
ALTER TABLE [dbo].[TabPartyMaster] CHECK CONSTRAINT [FK_TabPartyMaster_PartyType]
GO
ALTER TABLE [dbo].[TabPartyReportingTags]  WITH CHECK ADD  CONSTRAINT [FK__TabPartyR__Party__29572725] FOREIGN KEY([PartyId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabPartyReportingTags] CHECK CONSTRAINT [FK__TabPartyR__Party__29572725]
GO
ALTER TABLE [dbo].[TabProductGroupMaster]  WITH CHECK ADD  CONSTRAINT [FK_ProductGroup_TradingAccount] FOREIGN KEY([TradingAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabProductGroupMaster] CHECK CONSTRAINT [FK_ProductGroup_TradingAccount]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD FOREIGN KEY([HSNId])
REFERENCES [dbo].[TabHSNMaster] ([HSNId])
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD FOREIGN KEY([UnitId])
REFERENCES [dbo].[TabUnitMaster] ([UnitId])
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_Product_PackingUnit] FOREIGN KEY([ProductPackingUnit])
REFERENCES [dbo].[TabUnitMaster] ([UnitId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_Product_PackingUnit]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductGroup] FOREIGN KEY([ProductGroupId])
REFERENCES [dbo].[TabProductGroupMaster] ([ProductGroupId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_Product_ProductGroup]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_Product_PurchaseEffectedAccount] FOREIGN KEY([PurchaseEffectedAccount])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_Product_PurchaseEffectedAccount]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_Product_SaleEffectedAccount] FOREIGN KEY([SaleEffectedAccount])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_Product_SaleEffectedAccount]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_Product_TaxExemption] FOREIGN KEY([TaxExemptionId])
REFERENCES [dbo].[TabTaxExemptionMaster] ([TaxExemptionId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_Product_TaxExemption]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_TabProductMaster_TabSACMaster] FOREIGN KEY([SACId])
REFERENCES [dbo].[TabSACMaster] ([SACId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_TabProductMaster_TabSACMaster]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_TabProductMaster_TaxGroup] FOREIGN KEY([TaxGroupId])
REFERENCES [dbo].[TabTaxGroupMaster] ([TaxGroupId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_TabProductMaster_TaxGroup]
GO
ALTER TABLE [dbo].[TabProductMaster]  WITH CHECK ADD  CONSTRAINT [FK_TabProductMaster_TaxPreference] FOREIGN KEY([TaxPreferenceId])
REFERENCES [dbo].[TabTaxPreferenceMaster] ([TaxPreferenceId])
GO
ALTER TABLE [dbo].[TabProductMaster] CHECK CONSTRAINT [FK_TabProductMaster_TaxPreference]
GO
ALTER TABLE [dbo].[TabPurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK__TabPurcha__Custo__3C34F16F] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabPurchaseOrder] CHECK CONSTRAINT [FK__TabPurcha__Custo__3C34F16F]
GO
ALTER TABLE [dbo].[TabPurchaseOrder]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabPurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK__TabPurcha__Vendo__3A4CA8FD] FOREIGN KEY([VendorId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabPurchaseOrder] CHECK CONSTRAINT [FK__TabPurcha__Vendo__3A4CA8FD]
GO
ALTER TABLE [dbo].[TabPurchaseOrderItems]  WITH CHECK ADD FOREIGN KEY([AccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabPurchaseOrderItems]  WITH CHECK ADD FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[TabPurchaseOrder] ([PurchaseOrderId])
GO
ALTER TABLE [dbo].[TabPurchaseOrderItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabQuote]  WITH CHECK ADD  CONSTRAINT [FK__TabQuote__Custom__09746778] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabQuote] CHECK CONSTRAINT [FK__TabQuote__Custom__09746778]
GO
ALTER TABLE [dbo].[TabQuote]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabQuoteItems]  WITH CHECK ADD FOREIGN KEY([QuoteId])
REFERENCES [dbo].[TabQuote] ([QuoteId])
GO
ALTER TABLE [dbo].[TabQuoteItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabRecordExpense]  WITH CHECK ADD  CONSTRAINT [FK__TabRecord__Custo__1EA48E88] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRecordExpense] CHECK CONSTRAINT [FK__TabRecord__Custo__1EA48E88]
GO
ALTER TABLE [dbo].[TabRecordExpense]  WITH CHECK ADD FOREIGN KEY([ExpenseAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabRecordExpense]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabRecordExpense]  WITH CHECK ADD FOREIGN KEY([PaidThroughAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabRecordExpense]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabRecordExpense]  WITH CHECK ADD  CONSTRAINT [FK__TabRecord__Vendo__1CBC4616] FOREIGN KEY([VendorId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRecordExpense] CHECK CONSTRAINT [FK__TabRecord__Vendo__1CBC4616]
GO
ALTER TABLE [dbo].[TabRecordMileage]  WITH CHECK ADD  CONSTRAINT [FK__TabRecord__Custo__25518C17] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRecordMileage] CHECK CONSTRAINT [FK__TabRecord__Custo__25518C17]
GO
ALTER TABLE [dbo].[TabRecordMileage]  WITH CHECK ADD FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[TabEmployee] ([EmployeeId])
GO
ALTER TABLE [dbo].[TabRecordMileage]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabRecordMileage]  WITH CHECK ADD FOREIGN KEY([PaidThroughAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabRecordMileage]  WITH CHECK ADD  CONSTRAINT [FK__TabRecord__Vendo__245D67DE] FOREIGN KEY([VendorId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRecordMileage] CHECK CONSTRAINT [FK__TabRecord__Vendo__245D67DE]
GO
ALTER TABLE [dbo].[TabRecurringExpense]  WITH CHECK ADD  CONSTRAINT [FK__TabRecurr__Custo__2CF2ADDF] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRecurringExpense] CHECK CONSTRAINT [FK__TabRecurr__Custo__2CF2ADDF]
GO
ALTER TABLE [dbo].[TabRecurringExpense]  WITH CHECK ADD FOREIGN KEY([ExpenseAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabRecurringExpense]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabRecurringExpense]  WITH CHECK ADD FOREIGN KEY([PaidThroughAccountId])
REFERENCES [dbo].[TabAccountMaster] ([AccountId])
GO
ALTER TABLE [dbo].[TabRecurringExpense]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabRecurringExpense]  WITH CHECK ADD  CONSTRAINT [FK__TabRecurr__Vendo__2B0A656D] FOREIGN KEY([VendorId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRecurringExpense] CHECK CONSTRAINT [FK__TabRecurr__Vendo__2B0A656D]
GO
ALTER TABLE [dbo].[TabRetainerInvoice]  WITH CHECK ADD  CONSTRAINT [FK__TabRetain__Custo__681373AD] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabRetainerInvoice] CHECK CONSTRAINT [FK__TabRetain__Custo__681373AD]
GO
ALTER TABLE [dbo].[TabRetainerInvoice]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabRetainerInvoice]  WITH CHECK ADD FOREIGN KEY([ProjectId])
REFERENCES [dbo].[TabProjectMaster] ([ProjectId])
GO
ALTER TABLE [dbo].[TabRetainerInvoiceItems]  WITH CHECK ADD FOREIGN KEY([RetainerInvoiceId])
REFERENCES [dbo].[TabRetainerInvoice] ([RetainerInvoiceId])
GO
ALTER TABLE [dbo].[TabRolePermission]  WITH CHECK ADD  CONSTRAINT [FK__TabRolePermission_TabRoleMaster] FOREIGN KEY([RoleId])
REFERENCES [dbo].[TabRoleMaster] ([RoleId])
GO
ALTER TABLE [dbo].[TabRolePermission] CHECK CONSTRAINT [FK__TabRolePermission_TabRoleMaster]
GO
ALTER TABLE [dbo].[TabRolePermission]  WITH CHECK ADD  CONSTRAINT [FK_TabRolePermission_Tabs] FOREIGN KEY([TabId])
REFERENCES [dbo].[Tabs] ([TabId])
GO
ALTER TABLE [dbo].[TabRolePermission] CHECK CONSTRAINT [FK_TabRolePermission_Tabs]
GO
ALTER TABLE [dbo].[Tabs]  WITH CHECK ADD  CONSTRAINT [FK_Tabs_Parent] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Tabs] ([TabId])
GO
ALTER TABLE [dbo].[Tabs] CHECK CONSTRAINT [FK_Tabs_Parent]
GO
ALTER TABLE [dbo].[TabSalesOrder]  WITH CHECK ADD  CONSTRAINT [FK__TabSalesO__Custo__4E53A1AA] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[TabPartyMaster] ([PartyId])
GO
ALTER TABLE [dbo].[TabSalesOrder] CHECK CONSTRAINT [FK__TabSalesO__Custo__4E53A1AA]
GO
ALTER TABLE [dbo].[TabSalesOrder]  WITH CHECK ADD FOREIGN KEY([LocationId])
REFERENCES [dbo].[TabLocation] ([LocationId])
GO
ALTER TABLE [dbo].[TabSalesOrderItems]  WITH CHECK ADD FOREIGN KEY([SalesOrderId])
REFERENCES [dbo].[TabSalesOrder] ([SalesOrderId])
GO
ALTER TABLE [dbo].[TabSalesOrderItems]  WITH CHECK ADD FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabTaxGroupDetail]  WITH CHECK ADD  CONSTRAINT [FK_TaxGroupDetail_Tax] FOREIGN KEY([TaxId])
REFERENCES [dbo].[TabTaxMaster] ([TaxId])
GO
ALTER TABLE [dbo].[TabTaxGroupDetail] CHECK CONSTRAINT [FK_TaxGroupDetail_Tax]
GO
ALTER TABLE [dbo].[TabTaxGroupDetail]  WITH CHECK ADD  CONSTRAINT [FK_TaxGroupDetail_TaxGroup] FOREIGN KEY([TaxGroupId])
REFERENCES [dbo].[TabTaxGroupMaster] ([TaxGroupId])
GO
ALTER TABLE [dbo].[TabTaxGroupDetail] CHECK CONSTRAINT [FK_TaxGroupDetail_TaxGroup]
GO
ALTER TABLE [dbo].[TabUserRole]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[TabRoleMaster] ([RoleId])
GO
ALTER TABLE [dbo].[TabUserRole]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[TabUserMaster] ([UserId])
GO
ALTER TABLE [dbo].[TabWarehouseMaster]  WITH CHECK ADD FOREIGN KEY([BranchId])
REFERENCES [dbo].[TabBranchMaster] ([BranchId])
GO
/****** Object:  StoredProcedure [dbo].[sp_Account_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Account_Master]
    @Action varchar(20),
    @AccountId int = NULL,
    @AccountType varchar(100) = NULL,
    @AccountName varchar(200) = NULL,
    @IsSubAccount bit = NULL,
    @ParentAccountId int = NULL,
    @AccountCode varchar(50) = NULL,
    @Description varchar(500) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabAccountMaster] ([AccountType], [AccountName], [IsSubAccount], [ParentAccountId], [AccountCode], [Description])
        VALUES (@AccountType, @AccountName, @IsSubAccount, @ParentAccountId, @AccountCode, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabAccountMaster]
        SET [AccountType] = @AccountType, [AccountName] = @AccountName, [IsSubAccount] = @IsSubAccount,
            [ParentAccountId] = @ParentAccountId, [AccountCode] = @AccountCode, [Description] = @Description
        WHERE [AccountId] = @AccountId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabAccountMaster] WHERE [AccountId] = @AccountId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabAccountMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabAccountMaster] WHERE [AccountId] = @AccountId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Bank_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Bank_Master]
    @Action varchar(20),
    @BankMasterId int = NULL,
    @BankName varchar(200) = NULL,
    @Branch varchar(150) = NULL,
    @AccountNumber varchar(50) = NULL,
    @IFSC varchar(20) = NULL,
    @MICR varchar(20) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabBankMaster] ([BankName], [Branch], [AccountNumber], [IFSC], [MICR])
        VALUES (@BankName, @Branch, @AccountNumber, @IFSC, @MICR)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabBankMaster]
        SET [BankName] = @BankName, [Branch] = @Branch, [AccountNumber] = @AccountNumber,
            [IFSC] = @IFSC, [MICR] = @MICR
        WHERE [BankMasterId] = @BankMasterId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabBankMaster] WHERE [BankMasterId] = @BankMasterId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabBankMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabBankMaster] WHERE [BankMasterId] = @BankMasterId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Branch_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Branch_Master]
    @Action varchar(20),
    @BranchId int = NULL,
    @BranchName varchar(150) = NULL,
    @Address varchar(300) = NULL,
    @City varchar(100) = NULL,
    @State varchar(100) = NULL,
    @Country varchar(100) = NULL,
    @Phone varchar(20) = NULL,
    @IsActive bit = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabBranchMaster] ([BranchName], [Address], [City], [State], [Country], [Phone], [IsActive])
        VALUES (@BranchName, @Address, @City, @State, @Country, @Phone, @IsActive)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabBranchMaster]
        SET [BranchName] = @BranchName, [Address] = @Address, [City] = @City, 
            [State] = @State, [Country] = @Country, [Phone] = @Phone, [IsActive] = @IsActive
        WHERE [BranchId] = @BranchId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabBranchMaster] WHERE [BranchId] = @BranchId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabBranchMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabBranchMaster] WHERE [BranchId] = @BranchId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CostCenter_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_CostCenter_Master]
    @Action varchar(20),
    @CostCenterId int = NULL,
    @CostCenterName varchar(150) = NULL,
    @Description varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabCostCenterMaster] ([CostCenterName], [Description]) VALUES (@CostCenterName, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabCostCenterMaster] SET [CostCenterName] = @CostCenterName, [Description] = @Description WHERE [CostCenterId] = @CostCenterId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabCostCenterMaster] WHERE [CostCenterId] = @CostCenterId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabCostCenterMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabCostCenterMaster] WHERE [CostCenterId] = @CostCenterId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Currency_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Currency_Master]
    @Action varchar(20),
    @CurrencyId int = NULL,
    @CurrencyCode varchar(10) = NULL,
    @CurrencySymbol varchar(10) = NULL,
    @CurrencyName varchar(100) = NULL,
    @DecimalPlaces tinyint = NULL,
    @Format varchar(50) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabCurrencyMaster] ([CurrencyCode], [CurrencySymbol], [CurrencyName], [DecimalPlaces], [Format]) 
        VALUES (@CurrencyCode, @CurrencySymbol, @CurrencyName, @DecimalPlaces, @Format)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabCurrencyMaster] 
        SET [CurrencyCode] = @CurrencyCode, [CurrencySymbol] = @CurrencySymbol, [CurrencyName] = @CurrencyName, [DecimalPlaces] = @DecimalPlaces, [Format] = @Format
        WHERE [CurrencyId] = @CurrencyId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabCurrencyMaster] WHERE [CurrencyId] = @CurrencyId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabCurrencyMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabCurrencyMaster] WHERE [CurrencyId] = @CurrencyId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_FiscalYear_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_FiscalYear_Master]
    @Action varchar(20),
    @FiscalYearId int = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL,
    @IsCurrent bit = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabFiscalYearMaster] ([StartDate], [EndDate], [IsCurrent]) VALUES (@StartDate, @EndDate, @IsCurrent)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabFiscalYearMaster] SET [StartDate] = @StartDate, [EndDate] = @EndDate, [IsCurrent] = @IsCurrent WHERE [FiscalYearId] = @FiscalYearId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabFiscalYearMaster] WHERE [FiscalYearId] = @FiscalYearId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabFiscalYearMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabFiscalYearMaster] WHERE [FiscalYearId] = @FiscalYearId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetCompanyById]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
GO
/****** Object:  StoredProcedure [dbo].[sp_Godown_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Godown_Master]
    @Action varchar(20),
    @GodownId int = NULL,
    @WarehouseId int = NULL,
    @GodownName varchar(150) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabGodownMaster] ([WarehouseId], [GodownName]) VALUES (@WarehouseId, @GodownName)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabGodownMaster] SET [WarehouseId] = @WarehouseId, [GodownName] = @GodownName WHERE [GodownId] = @GodownId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabGodownMaster] WHERE [GodownId] = @GodownId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabGodownMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabGodownMaster] WHERE [GodownId] = @GodownId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_HSN_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_HSN_Master]
    @Action varchar(20),
    @HSNId int = NULL,
    @HSNCode varchar(20) = NULL,
    @Description varchar(300) = NULL,
    @TaxRate decimal(18,2) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabHSNMaster] ([HSNCode], [Description], [TaxRate]) VALUES (@HSNCode, @Description, @TaxRate)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabHSNMaster] SET [HSNCode] = @HSNCode, [Description] = @Description, [TaxRate] = @TaxRate WHERE [HSNId] = @HSNId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabHSNMaster] WHERE [HSNId] = @HSNId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabHSNMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabHSNMaster] WHERE [HSNId] = @HSNId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Party_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_Party_Master]
    @Action varchar(20),
    @PartyId int = NULL,
    @PartyType varchar(20) = NULL,
    @Salutation varchar(20) = NULL,
    @FirstName varchar(100) = NULL,
    @LastName varchar(100) = NULL,
    @CompanyName varchar(200) = NULL,
    @DisplayName varchar(200) = NULL,
    @Email varchar(200) = NULL,
    @PhoneWork varchar(20) = NULL,
    @PhoneMobile varchar(20) = NULL,
    @PreferredLanguage varchar(50) = NULL,
    @GSTTreatment varchar(100) = NULL,
    @PlaceOfSupply varchar(100) = NULL,
    @PAN varchar(20) = NULL,
    @GSTNO varchar(40) = NULL,
    @MSMERegistered bit = NULL,
    @TaxPreference varchar(50) = NULL,
    @TDS varchar(100) = NULL,
    @Currency int = NULL,
    @AccountsReceivableAccount int = NULL,
    @AccountsPayableAccount int = NULL,
    @OpeningBalance decimal(18,2) = NULL,
    @OpeningBalanceType varchar(50) = NULL,
    @PaymentTerms varchar(100) = NULL,
    @EnablePortal bit = NULL,
    @WebsiteURL varchar(500) = NULL,
    @Department varchar(100) = NULL,
    @Remarks varchar(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyMaster] 
        ([PartyType], [Salutation], [FirstName], [LastName], [CompanyName], [DisplayName], [Email], [PhoneWork], [PhoneMobile], 
         [PreferredLanguage], [GSTTreatment], [PlaceOfSupply], [PAN], [GSTNO], [MSMERegistered], [TaxPreference], [TDS], [CurrencyId],
         [AccountsReceivableAccount], [AccountsPayableAccount], [OpeningBalance], [OpeningBalanceType], [PaymentTerms],
         [EnablePortal], [WebsiteURL], [Department], [Remarks], [CreatedOn])
        VALUES 
        (@PartyType, @Salutation, @FirstName, @LastName, @CompanyName, @DisplayName, @Email, @PhoneWork, @PhoneMobile,
         @PreferredLanguage, @GSTTreatment, @PlaceOfSupply, @PAN, @GSTNO, @MSMERegistered, @TaxPreference, @TDS, @Currency,
         @AccountsReceivableAccount, @AccountsPayableAccount, @OpeningBalance, @OpeningBalanceType, @PaymentTerms,
         @EnablePortal, @WebsiteURL, @Department, @Remarks, GETDATE())
        
        SELECT SCOPE_IDENTITY() AS PartyId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyMaster]
        SET [PartyType] = @PartyType, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [CompanyName] = @CompanyName, [DisplayName] = @DisplayName, [Email] = @Email, [PhoneWork] = @PhoneWork,
            [PhoneMobile] = @PhoneMobile, [PreferredLanguage] = @PreferredLanguage, [GSTTreatment] = @GSTTreatment,
            [PlaceOfSupply] = @PlaceOfSupply, [PAN] = @PAN, [GSTNO] = @GSTNO, [MSMERegistered] = @MSMERegistered, 
            [TaxPreference] = @TaxPreference, [TDS] = @TDS, [CurrencyId] = @Currency, 
            [AccountsReceivableAccount] = @AccountsReceivableAccount, [AccountsPayableAccount] = @AccountsPayableAccount, 
            [OpeningBalance] = @OpeningBalance, [OpeningBalanceType] = @OpeningBalanceType, [PaymentTerms] = @PaymentTerms, 
            [EnablePortal] = @EnablePortal, [WebsiteURL] = @WebsiteURL, [Department] = @Department, [Remarks] = @Remarks
        WHERE [PartyId] = @PartyId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, CurrencyId AS [Currency] FROM [dbo].[TabPartyMaster] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, CurrencyId AS [Currency] FROM [dbo].[TabPartyMaster] WHERE [PartyId] = @PartyId END
    ELSE IF @Action = 'SELECT_BY_TYPE' 
    BEGIN 
        SELECT *, CurrencyId AS [Currency] 
        FROM [dbo].[TabPartyMaster] 
        WHERE [PartyType] = @PartyType OR @PartyType IS NULL
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PartyAddress_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_PartyAddress_Master]
    @Action varchar(20),
    @AddressId int = NULL,
    @PartyId int = NULL,
    @AddressType varchar(50) = NULL,
    @Attention varchar(200) = NULL,
    @Country varchar(100) = NULL,
    @Street1 varchar(500) = NULL,
    @Street2 varchar(500) = NULL,
    @City varchar(100) = NULL,
    @State varchar(100) = NULL,
    @PinCode varchar(20) = NULL,
    @Phone varchar(20) = NULL,
    @FaxNumber varchar(20) = NULL,
    @IsDefault bit = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Handle AddressType mapping for INSERT/UPDATE
    DECLARE @ActualAddressTypeId INT = 1; -- Default to Billing
    IF @AddressType = 'Shipping' OR @AddressType = '2' SET @ActualAddressTypeId = 2;
    ELSE IF @AddressType = 'Billing' OR @AddressType = '1' SET @ActualAddressTypeId = 1;

    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyAddress] 
        ([PartyId], [AddressTypeId], [Attention], [Country], [AddressLine1], [AddressLine2], [City], [State], [PinCode], [Phone], [Fax], [IsDefault], [CreatedOn], [IsActive])
        VALUES 
        (@PartyId, @ActualAddressTypeId, @Attention, @Country, @Street1, @Street2, @City, @State, @PinCode, @Phone, @FaxNumber, ISNULL(@IsDefault, 0), GETDATE(), 1)
        
        SELECT SCOPE_IDENTITY() AS AddressId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyAddress]
        SET [PartyId] = @PartyId, [AddressTypeId] = @ActualAddressTypeId, [Attention] = @Attention, [Country] = @Country,
            [AddressLine1] = @Street1, [AddressLine2] = @Street2, [City] = @City, [State] = @State,
            [PinCode] = @PinCode, [Phone] = @Phone, [Fax] = @FaxNumber, [IsDefault] = ISNULL(@IsDefault, 0)
        WHERE [AddressId] = @AddressId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyAddress] WHERE [AddressId] = @AddressId END
    ELSE IF @Action = 'SELECT' 
    BEGIN 
        SELECT *, 
               AddressLine1 AS Street1, 
               AddressLine2 AS Street2, 
               CASE WHEN AddressTypeId = 1 THEN 'Billing' ELSE 'Shipping' END AS AddressType, 
               Fax AS FaxNumber 
        FROM [dbo].[TabPartyAddress] 
    END
    ELSE IF @Action = 'SELECT_BY_ID' 
    BEGIN 
        SELECT *, 
               AddressLine1 AS Street1, 
               AddressLine2 AS Street2, 
               CASE WHEN AddressTypeId = 1 THEN 'Billing' ELSE 'Shipping' END AS AddressType, 
               Fax AS FaxNumber 
        FROM [dbo].[TabPartyAddress] 
        WHERE [AddressId] = @AddressId 
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' 
    BEGIN 
        SELECT *, 
               AddressLine1 AS Street1, 
               AddressLine2 AS Street2, 
               CASE WHEN AddressTypeId = 1 THEN 'Billing' ELSE 'Shipping' END AS AddressType, 
               Fax AS FaxNumber 
        FROM [dbo].[TabPartyAddress] 
        WHERE [PartyId] = @PartyId 
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PartyBankDetails_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_PartyBankDetails_Master]
    @Action varchar(20),
    @BankDetailId int = NULL,
    @PartyId int = NULL,
    @AccountHolderName varchar(200) = NULL,
    @BankName varchar(200) = NULL,
    @AccountNumber varchar(50) = NULL,
    @IFSC varchar(20) = NULL,
    @Branch varchar(200) = NULL,
    @MICR varchar(20) = NULL,
    @IsDefault bit = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyBankDetails] ([PartyId], [AccountHolderName], [BankName], [AccountNumber], [IFSC])
        VALUES (@PartyId, @AccountHolderName, @BankName, @AccountNumber, @IFSC)
        
        SELECT SCOPE_IDENTITY() AS BankDetailId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyBankDetails]
        SET [PartyId] = @PartyId, [AccountHolderName] = @AccountHolderName, [BankName] = @BankName,
            [AccountNumber] = @AccountNumber, [IFSC] = @IFSC
        WHERE [BankId] = @BankDetailId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyBankDetails] WHERE [BankId] = @BankDetailId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, BankId AS BankDetailId, NULL AS Branch, NULL AS MICR, CAST(0 AS BIT) AS IsDefault, GETDATE() AS CreatedOn FROM [dbo].[TabPartyBankDetails] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, BankId AS BankDetailId, NULL AS Branch, NULL AS MICR, CAST(0 AS BIT) AS IsDefault, GETDATE() AS CreatedOn FROM [dbo].[TabPartyBankDetails] WHERE [BankId] = @BankDetailId END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, BankId AS BankDetailId, NULL AS Branch, NULL AS MICR, CAST(0 AS BIT) AS IsDefault, GETDATE() AS CreatedOn FROM [dbo].[TabPartyBankDetails] WHERE [PartyId] = @PartyId END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PartyContactPerson_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_PartyContactPerson_Master]
    @Action varchar(20),
    @ContactPersonId int = NULL,
    @PartyId int = NULL,
    @Salutation varchar(20) = NULL,
    @FirstName varchar(100) = NULL,
    @LastName varchar(100) = NULL,
    @EmailAddress varchar(200) = NULL,
    @WorkPhone varchar(20) = NULL,
    @Mobile varchar(20) = NULL,
    @IsPrimary bit = NULL 
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyContactPersons] ([PartyId], [Salutation], [FirstName], [LastName], [Email], [WorkPhone], [Mobile])
        VALUES (@PartyId, @Salutation, @FirstName, @LastName, @EmailAddress, @WorkPhone, @Mobile)
        
        SELECT SCOPE_IDENTITY() AS ContactPersonId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyContactPersons]
        SET [PartyId] = @PartyId, [Salutation] = @Salutation, [FirstName] = @FirstName, [LastName] = @LastName,
            [Email] = @EmailAddress, [WorkPhone] = @WorkPhone, [Mobile] = @Mobile
        WHERE [ContactPersonId] = @ContactPersonId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, Email AS EmailAddress, CAST(0 AS BIT) AS IsPrimary, GETDATE() AS CreatedOn FROM [dbo].[TabPartyContactPersons] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, Email AS EmailAddress, CAST(0 AS BIT) AS IsPrimary, GETDATE() AS CreatedOn FROM [dbo].[TabPartyContactPersons] WHERE [ContactPersonId] = @ContactPersonId END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, Email AS EmailAddress, CAST(0 AS BIT) AS IsPrimary, GETDATE() AS CreatedOn FROM [dbo].[TabPartyContactPersons] WHERE [PartyId] = @PartyId END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PartyReportingTag_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_PartyReportingTag_Master]
    @Action varchar(20),
    @PartyReportingTagId int = NULL,
    @PartyId int = NULL,
    @ReportingTagId int = NULL,
    @TagValue varchar(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyReportingTags] ([PartyId], [TagName])
        VALUES (@PartyId, @TagValue)
        
        SELECT SCOPE_IDENTITY() AS PartyReportingTagId
    END
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabPartyReportingTags]
        SET [PartyId] = @PartyId, [TagName] = @TagValue
        WHERE [TagId] = @PartyReportingTagId
    END
    ELSE IF @Action = 'DELETE' BEGIN DELETE FROM [dbo].[TabPartyReportingTags] WHERE [TagId] = @PartyReportingTagId END
    ELSE IF @Action = 'SELECT' BEGIN SELECT *, TagId AS PartyReportingTagId, TagId AS ReportingTagId, TagName AS TagValue, GETDATE() AS CreatedOn FROM [dbo].[TabPartyReportingTags] END
    ELSE IF @Action = 'SELECT_BY_ID' BEGIN SELECT *, TagId AS PartyReportingTagId, TagId AS ReportingTagId, TagName AS TagValue, GETDATE() AS CreatedOn FROM [dbo].[TabPartyReportingTags] WHERE [TagId] = @PartyReportingTagId END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID' BEGIN SELECT *, TagId AS PartyReportingTagId, TagId AS ReportingTagId, TagName AS TagValue, GETDATE() AS CreatedOn FROM [dbo].[TabPartyReportingTags] WHERE [PartyId] = @PartyId END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PartyTypeMaster]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[sp_PartyTypeMaster]
  @Action NVARCHAR(50),
  @PartyTypeId INT = NULL,
  @PartyTypeCode NVARCHAR(50) = NULL,
  @PartyTypeName NVARCHAR(200) = NULL,
  @Description NVARCHAR(MAX) = NULL,
  @IsCustomer BIT = NULL,
  @IsVendor BIT = NULL,
  @IsActive BIT = NULL,
  @SortOrder INT = NULL,
  @PartyType NVARCHAR(50) = NULL -- Used for SELECT_BY_TYPE
AS
BEGIN
  SET NOCOUNT ON;

  IF @Action = 'SELECT'
  BEGIN
    SELECT
      PartyTypeId,
      PartyTypeCode,
      PartyTypeName,
      Description,
      IsCustomer,
      IsVendor,
      IsActive,
      SortOrder,
      CreatedOn
    FROM dbo.TabPartyTypeMaster
    WHERE IsActive = 1
    ORDER BY SortOrder, PartyTypeName;
  END
  ELSE IF @Action = 'SELECT_BY_ID'
  BEGIN
    SELECT
      PartyTypeId,
      PartyTypeCode,
      PartyTypeName,
      Description,
      IsCustomer,
      IsVendor,
      IsActive,
      SortOrder,
      CreatedOn
    FROM dbo.TabPartyTypeMaster
    WHERE PartyTypeId = @PartyTypeId;
  END
  ELSE IF @Action = 'SELECT_BY_TYPE'
  BEGIN
    SELECT *, CurrencyId AS [Currency] 
    FROM [dbo].[TabPartyMaster] 
    WHERE [PartyType] = @PartyType OR @PartyType IS NULL;
  END
  ELSE IF @Action = 'INSERT'
  BEGIN
    INSERT INTO dbo.TabPartyTypeMaster
    (
      PartyTypeCode, PartyTypeName, Description, IsCustomer, IsVendor, IsActive, SortOrder, CreatedOn
    )
    VALUES
    (
      @PartyTypeCode, @PartyTypeName, @Description, ISNULL(@IsCustomer, 0), ISNULL(@IsVendor, 0), ISNULL(@IsActive, 1), ISNULL(@SortOrder, 0), GETUTCDATE()
    );
    SELECT SCOPE_IDENTITY() AS PartyTypeId;
  END
  ELSE IF @Action = 'UPDATE'
  BEGIN
    UPDATE dbo.TabPartyTypeMaster
    SET
      PartyTypeCode = COALESCE(@PartyTypeCode, PartyTypeCode),
      PartyTypeName = COALESCE(@PartyTypeName, PartyTypeName),
      Description = @Description,
      IsCustomer = COALESCE(@IsCustomer, IsCustomer),
      IsVendor = COALESCE(@IsVendor, IsVendor),
      IsActive = COALESCE(@IsActive, IsActive),
      SortOrder = COALESCE(@SortOrder, SortOrder)
    WHERE PartyTypeId = @PartyTypeId;
  END
  ELSE IF @Action = 'DELETE'
  BEGIN
    DELETE FROM dbo.TabPartyTypeMaster
    WHERE PartyTypeId = @PartyTypeId;
  END
  ELSE
  BEGIN
    RAISERROR('Invalid @Action value supplied to sp_PartyTypeMaster: %s', 16, 1, @Action);
  END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PartyTypeMaster_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PartyTypeMaster_Master]
    @Action varchar(20),
    @PartyTypeId int = NULL,
    @PartyId int = NULL,
    @PartyType varchar(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabPartyTypeMaster] 
        ([PartyId], [PartyType], [CreatedOn])
        VALUES 
        (@PartyId, @PartyType, GETDATE())
    END
    ELSE IF @Action = 'SELECT_BY_PARTY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabPartyTypeMaster] WHERE [PartyId] = @PartyId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PaymentMode_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PaymentMode_Master]
    @Action varchar(20),
    @PaymentModeId int = NULL,
    @ModeName varchar(100) = NULL,
    @IsDefault bit = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabPaymentModeMaster] ([ModeName], [IsDefault]) VALUES (@ModeName, @IsDefault)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabPaymentModeMaster] SET [ModeName] = @ModeName, [IsDefault] = @IsDefault WHERE [PaymentModeId] = @PaymentModeId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabPaymentModeMaster] WHERE [PaymentModeId] = @PaymentModeId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabPaymentModeMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabPaymentModeMaster] WHERE [PaymentModeId] = @PaymentModeId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PaymentTerm_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_PaymentTerm_Master]
    @Action varchar(20),
    @TermId int = NULL,
    @TermName varchar(100) = NULL,
    @NumberOfDays int = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabPaymentTermMaster] ([TermName], [NumberOfDays]) VALUES (@TermName, @NumberOfDays)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabPaymentTermMaster] SET [TermName] = @TermName, [NumberOfDays] = @NumberOfDays WHERE [TermId] = @TermId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabPaymentTermMaster] WHERE [TermId] = @TermId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabPaymentTermMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabPaymentTermMaster] WHERE [TermId] = @TermId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Product_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Product_Master]
    @Action VARCHAR(30),

    @ProductId INT = NULL,
    @ProductType VARCHAR(20) = NULL,
    @ProductName VARCHAR(200) = NULL,

    -- Pricing
    @SellingPrice DECIMAL(18,2) = NULL,
    @SellingPrice2 FLOAT = NULL,
    @SellingPrice3 FLOAT = NULL,
    @SellingPrice4 FLOAT = NULL,
    @SellingPrice5 FLOAT = NULL,

    @CostPrice DECIMAL(18,2) = NULL,

    -- Descriptions
    @SalesDescription VARCHAR(500) = NULL,
    @PurchaseDescription VARCHAR(500) = NULL,

    -- Accounts
    @SalesAccount VARCHAR(100) = NULL,
    @PurchaseAccount VARCHAR(100) = NULL,
    @PurchaseEffectedAccount INT = NULL,
    @SaleEffectedAccount INT = NULL,

    -- Flags
    @Sellable BIT = NULL,
    @Purchasable BIT = NULL,

    -- Masters
    @UnitId INT = NULL,
    @ProductPackingUnit INT = NULL,
    @ProductGroupId INT = NULL,
    @PreferredVendorId INT = NULL,

    -- Tax
    @HSNId INT = NULL,
    @SACId INT = NULL,
    @TaxPreferenceId INT = NULL,
    @TaxGroupId INT = NULL,

    -- Inventory
    @OpeningQuantity FLOAT = NULL,
    @OpeningPrice FLOAT = NULL,
    @OpeningAmount FLOAT = NULL,
    @MinQuantity FLOAT = NULL,
    @MaxQuantity FLOAT = NULL,
    @ReOrderQuantity FLOAT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================
       INSERT
    ========================= */
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO dbo.TabProductMaster
        (
            ProductType,
            ProductName,

            SellingPrice,
            SellingPrice2,
            SellingPrice3,
            SellingPrice4,
            SellingPrice5,

            CostPrice,

            SalesAccount,
            PurchaseAccount,
            SalesDescription,
            PurchaseDescription,

            PurchaseEffectedAccount,
            SaleEffectedAccount,

            Sellable,
            Purchasable,

            UnitId,
            ProductPackingUnit,
            ProductGroupId,
            PreferredVendorId,

            HSNId,
            SACId,
            TaxPreferenceId,
            TaxGroupId,

            OpeningQuantity,
            OpeningPrice,
            OpeningAmount,

            MinQuantity,
            MaxQuantity,
            ReOrderQuantity
        )
        VALUES
        (
            @ProductType,
            @ProductName,

            @SellingPrice,
            @SellingPrice2,
            @SellingPrice3,
            @SellingPrice4,
            @SellingPrice5,

            @CostPrice,

            @SalesAccount,
            @PurchaseAccount,
            @SalesDescription,
            @PurchaseDescription,

            @PurchaseEffectedAccount,
            @SaleEffectedAccount,

            @Sellable,
            @Purchasable,

            @UnitId,
            @ProductPackingUnit,
            @ProductGroupId,
            @PreferredVendorId,

            @HSNId,
            @SACId,
            @TaxPreferenceId,
            @TaxGroupId,

            @OpeningQuantity,
            @OpeningPrice,
            @OpeningAmount,

            @MinQuantity,
            @MaxQuantity,
            @ReOrderQuantity
        );
    END

    /* =========================
       UPDATE
    ========================= */
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.TabProductMaster
        SET
            ProductType = @ProductType,
            ProductName = @ProductName,

            SellingPrice = @SellingPrice,
            SellingPrice2 = @SellingPrice2,
            SellingPrice3 = @SellingPrice3,
            SellingPrice4 = @SellingPrice4,
            SellingPrice5 = @SellingPrice5,

            CostPrice = @CostPrice,

            SalesAccount = @SalesAccount,
            PurchaseAccount = @PurchaseAccount,
            SalesDescription = @SalesDescription,
            PurchaseDescription = @PurchaseDescription,

            PurchaseEffectedAccount = @PurchaseEffectedAccount,
            SaleEffectedAccount = @SaleEffectedAccount,

            Sellable = @Sellable,
            Purchasable = @Purchasable,

            UnitId = @UnitId,
            ProductPackingUnit = @ProductPackingUnit,
            ProductGroupId = @ProductGroupId,
            PreferredVendorId = @PreferredVendorId,

            HSNId = @HSNId,
            SACId = @SACId,
            TaxPreferenceId = @TaxPreferenceId,
            TaxGroupId = @TaxGroupId,

            OpeningQuantity = @OpeningQuantity,
            OpeningPrice = @OpeningPrice,
            OpeningAmount = @OpeningAmount,

            MinQuantity = @MinQuantity,
            MaxQuantity = @MaxQuantity,
            ReOrderQuantity = @ReOrderQuantity
        WHERE ProductId = @ProductId;
    END

    /* =========================
       DELETE
    ========================= */
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM dbo.TabProductMaster
        WHERE ProductId = @ProductId;
    END

    /* =========================
       SELECT
    ========================= */
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT *
        FROM dbo.TabProductMaster;
    END

    /* =========================
       SELECT WITH DETAILS (INDEX / LIST VIEW)
    ========================= */
    ELSE IF @Action = 'SELECT_WITH_DETAILS'
    BEGIN
        SELECT
            P.ProductId,
            P.ProductName,
            P.ProductType,

            P.SellingPrice,
			p.SalesDescription,
			p.PurchaseDescription,
            P.CostPrice,

			P.TaxGroupId,
			P.TaxPreferenceId,

            U.UnitName AS BaseUnit,
            PU.UnitName AS PackingUnit,

            PG.ProductGroupName,

            H.HSNCode,
            S.SACCode,

            TG.GroupName AS TaxGroup,

            P.MinQuantity,
            P.MaxQuantity,
            P.ReOrderQuantity

        FROM TabProductMaster P
        LEFT JOIN TabUnitMaster U  ON P.UnitId = U.UnitId
        LEFT JOIN TabUnitMaster PU ON P.ProductPackingUnit = PU.UnitId
        LEFT JOIN TabProductGroupMaster PG ON P.ProductGroupId = PG.ProductGroupId
        LEFT JOIN TabHSNMaster H ON P.HSNId = H.HSNId
        LEFT JOIN TabSACMaster S ON P.SACId = S.SACId
        LEFT JOIN TabTaxGroupMaster TG ON P.TaxGroupId = TG.TaxGroupId;
    END

    /* =========================
       SELECT BY ID
    ========================= */
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM dbo.TabProductMaster
        WHERE ProductId = @ProductId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ProductGroup_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ProductGroup_Master]
    @Action            VARCHAR(20),   -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID
    @ProductGroupId    INT = NULL,
    @ProductGroupName  VARCHAR(200) = NULL,
    @ShortName         VARCHAR(50) = NULL,
    @AliasName         VARCHAR(100) = NULL,
    @TradingAccountId  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================
       INSERT
    ========================= */
    IF @Action = 'INSERT'
    BEGIN
        -- Duplicate check
        IF EXISTS (
            SELECT 1
            FROM TabProductGroupMaster
            WHERE ProductGroupName = @ProductGroupName
        )
        BEGIN
            RAISERROR('Duplicate Product Group Name', 16, 1);
            RETURN;
        END

        INSERT INTO dbo.TabProductGroupMaster
        (
            ProductGroupName,
            ShortName,
            AliasName,
            TradingAccountId
        )
        VALUES
        (
            @ProductGroupName,
            @ShortName,
            @AliasName,
            @TradingAccountId
        );
    END

    /* =========================
       UPDATE
    ========================= */
    ELSE IF @Action = 'UPDATE'
    BEGIN
        -- Duplicate check excluding current record
        IF EXISTS (
            SELECT 1
            FROM TabProductGroupMaster
            WHERE ProductGroupName = @ProductGroupName
              AND ProductGroupId <> @ProductGroupId
        )
        BEGIN
            RAISERROR('Duplicate Product Group Name', 16, 1);
            RETURN;
        END

        UPDATE dbo.TabProductGroupMaster
        SET
            ProductGroupName = @ProductGroupName,
            ShortName        = @ShortName,
            AliasName        = @AliasName,
            TradingAccountId = @TradingAccountId
        WHERE ProductGroupId = @ProductGroupId;
    END

    /* =========================
       DELETE
    ========================= */
    ELSE IF @Action = 'DELETE'
    BEGIN
        -- Prevent delete if product group is used
        IF EXISTS (
            SELECT 1
            FROM TabProductMaster
            WHERE ProductGroupId = @ProductGroupId
        )
        BEGIN
            RAISERROR('Product Group is in use and cannot be deleted', 16, 1);
            RETURN;
        END

        DELETE
        FROM dbo.TabProductGroupMaster
        WHERE ProductGroupId = @ProductGroupId;
    END

    /* =========================
       SELECT ALL
    ========================= */
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT *
        FROM dbo.TabProductGroupMaster
        ORDER BY ProductGroupName;
    END

    /* =========================
       SELECT BY ID
    ========================= */
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM dbo.TabProductGroupMaster
        WHERE ProductGroupId = @ProductGroupId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Project_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Project_Master]
    @Action varchar(20),
    @ProjectId int = NULL,
    @ProjectName varchar(200) = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL,
    @Status varchar(50) = NULL,
    @Description varchar(500) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabProjectMaster] ([ProjectName], [StartDate], [EndDate], [Status], [Description])
        VALUES (@ProjectName, @StartDate, @EndDate, @Status, @Description)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabProjectMaster]
        SET [ProjectName] = @ProjectName, [StartDate] = @StartDate, [EndDate] = @EndDate,
            [Status] = @Status, [Description] = @Description
        WHERE [ProjectId] = @ProjectId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabProjectMaster] WHERE [ProjectId] = @ProjectId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabProjectMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabProjectMaster] WHERE [ProjectId] = @ProjectId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Role_Master]
    @Action varchar(20),
    @RoleId int = NULL,
    @RoleName varchar(100) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabRoleMaster] ([RoleName]) VALUES (@RoleName)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabRoleMaster] SET [RoleName] = @RoleName WHERE [RoleId] = @RoleId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabRoleMaster] WHERE [RoleId] = @RoleId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabRoleMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabRoleMaster] WHERE [RoleId] = @RoleId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SAC_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_SAC_Master]
    @Action varchar(20),
    @SACId int = NULL,
    @SACCode varchar(20) = NULL,
    @Description varchar(300) = NULL,
    @TaxRate decimal(18,2) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabSACMaster] ([SACCode], [Description], [TaxRate]) VALUES (@SACCode, @Description, @TaxRate)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabSACMaster] SET [SACCode] = @SACCode, [Description] = @Description, [TaxRate] = @TaxRate WHERE [SACId] = @SACId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabSACMaster] WHERE [SACId] = @SACId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabSACMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabSACMaster] WHERE [SACId] = @SACId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Salesperson_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Salesperson_Master]
    @Action varchar(20),
    @SalespersonId int = NULL,
    @SalespersonName varchar(200) = NULL,
    @Email varchar(200) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabSalespersonMaster] ([SalespersonName], [Email]) VALUES (@SalespersonName, @Email)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabSalespersonMaster] SET [SalespersonName] = @SalespersonName, [Email] = @Email WHERE [SalespersonId] = @SalespersonId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabSalespersonMaster] WHERE [SalespersonId] = @SalespersonId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabSalespersonMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabSalespersonMaster] WHERE [SalespersonId] = @SalespersonId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Tax_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Tax_Master]
    @Action varchar(20),
    @TaxId int = NULL,
    @TaxName varchar(200) = NULL,
    @RatePercent decimal(5,2) = NULL,
    @TaxType varchar(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'INSERT'
    BEGIN
		
		IF @RatePercent < 0
        BEGIN
            RAISERROR('RatePercent cannot be negative',16,1);
            RETURN;
        END

        IF EXISTS (
            SELECT 1 FROM TabTaxMaster
            WHERE TaxType = @TaxType AND RatePercent = @RatePercent
        )
        BEGIN
            RAISERROR('Duplicate tax rate for same tax type',16,1);
            RETURN;
        END

        INSERT INTO dbo.TabTaxMaster (TaxName, RatePercent, TaxType)
        VALUES (@TaxName, @RatePercent, @TaxType);
    END

    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.TabTaxMaster
        SET TaxName = @TaxName,
            RatePercent = @RatePercent,
            TaxType = @TaxType
        WHERE TaxId = @TaxId;
    END

    ELSE IF @Action = 'DELETE'
    BEGIN
        IF EXISTS (SELECT 1 FROM TabTaxGroupDetail WHERE TaxId = @TaxId)
        BEGIN
            RAISERROR('Tax is used in Tax Groups and cannot be deleted',16,1);
            RETURN;
        END

        DELETE FROM dbo.TabTaxMaster WHERE TaxId = @TaxId;
    END

    ELSE IF @Action = 'SELECT'
        SELECT * FROM dbo.TabTaxMaster;

    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM dbo.TabTaxMaster WHERE TaxId = @TaxId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TaxExemption_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TaxExemption_Master]
    @Action           VARCHAR(20),   -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID
    @TaxExemptionId   INT = NULL,
    @ExemptionReason  VARCHAR(200) = NULL,
    @Description      VARCHAR(500) = NULL,
    @ExemptionType    VARCHAR(20) = NULL, -- CUSTOMER / ITEM
    @IsActive         BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================
       VALIDATION : TYPE
    ========================= */
    IF @Action IN ('INSERT','UPDATE')
    BEGIN
        IF @ExemptionType NOT IN ('CUSTOMER','ITEM')
        BEGIN
            RAISERROR('Invalid Exemption Type. Allowed values: CUSTOMER, ITEM',16,1);
            RETURN;
        END
    END

    /* =========================
       INSERT
    ========================= */
    IF @Action = 'INSERT'
    BEGIN
        -- Duplicate check
        IF EXISTS (
            SELECT 1
            FROM TabTaxExemptionMaster
            WHERE ExemptionReason = @ExemptionReason
              AND ExemptionType = @ExemptionType
        )
        BEGIN
            RAISERROR('Duplicate Tax Exemption Reason for this Type',16,1);
            RETURN;
        END

        INSERT INTO dbo.TabTaxExemptionMaster
        (
            ExemptionReason,
            Description,
            ExemptionType,
            IsActive
        )
        VALUES
        (
            @ExemptionReason,
            @Description,
            @ExemptionType,
            ISNULL(@IsActive,1)
        );
    END

    /* =========================
       UPDATE
    ========================= */
    ELSE IF @Action = 'UPDATE'
    BEGIN
        -- Duplicate check excluding self
        IF EXISTS (
            SELECT 1
            FROM TabTaxExemptionMaster
            WHERE ExemptionReason = @ExemptionReason
              AND ExemptionType = @ExemptionType
              AND TaxExemptionId <> @TaxExemptionId
        )
        BEGIN
            RAISERROR('Duplicate Tax Exemption Reason for this Type',16,1);
            RETURN;
        END

        UPDATE dbo.TabTaxExemptionMaster
        SET
            ExemptionReason = @ExemptionReason,
            Description     = @Description,
            ExemptionType   = @ExemptionType,
            IsActive        = ISNULL(@IsActive, IsActive)
        WHERE TaxExemptionId = @TaxExemptionId;
    END

    /* =========================
       DELETE (SAFE)
    ========================= */
    ELSE IF @Action = 'DELETE'
    BEGIN
        -- Prevent delete if used in Product Master
        IF EXISTS (
            SELECT 1
            FROM TabProductMaster
            WHERE TaxExemptionId = @TaxExemptionId
        )
        BEGIN
            RAISERROR('Tax Exemption is in use and cannot be deleted',16,1);
            RETURN;
        END

        DELETE
        FROM dbo.TabTaxExemptionMaster
        WHERE TaxExemptionId = @TaxExemptionId;
    END

    /* =========================
       SELECT ALL
    ========================= */
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT *
        FROM dbo.TabTaxExemptionMaster
        ORDER BY ExemptionReason;
    END

    /* =========================
       SELECT BY ID
    ========================= */
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM dbo.TabTaxExemptionMaster
        WHERE TaxExemptionId = @TaxExemptionId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TaxGroup_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TaxGroup_Master]
    @Action        VARCHAR(20),
    @TaxGroupId    INT = NULL,
    @GroupName     VARCHAR(100) = NULL,
    @Description   VARCHAR(300) = NULL,
    @SupplyType    VARCHAR(20) = NULL,
    @CGSTPer       FLOAT = NULL,
    @SGSTPer       FLOAT = NULL,
    @IGSTPer       FLOAT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================
       INSERT
    ========================= */
    IF @Action = 'INSERT'
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM TabTaxGroupMaster
            WHERE GroupName = @GroupName
              AND SupplyType = @SupplyType
        )
        BEGIN
            RAISERROR('Duplicate Tax Group',16,1);
            RETURN;
        END

        INSERT INTO dbo.TabTaxGroupMaster
        (
            GroupName,
            Description,
            SupplyType,
            CGSTPer,
            SGSTPer,
            IGSTPer
        )
        VALUES
        (
            @GroupName,
            @Description,
            @SupplyType,
            @CGSTPer,
            @SGSTPer,
            @IGSTPer
        );
    END

    /* =========================
       UPDATE
    ========================= */
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.TabTaxGroupMaster
        SET GroupName   = @GroupName,
            Description = @Description,
            SupplyType  = @SupplyType,
            CGSTPer     = @CGSTPer,
            SGSTPer     = @SGSTPer,
            IGSTPer     = @IGSTPer
        WHERE TaxGroupId = @TaxGroupId;
    END

    /* =========================
       DELETE
    ========================= */
    ELSE IF @Action = 'DELETE'
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM TabTaxGroupDetail
            WHERE TaxGroupId = @TaxGroupId
        )
        BEGIN
            RAISERROR('Tax Group is configured and cannot be deleted',16,1);
            RETURN;
        END

        DELETE
        FROM dbo.TabTaxGroupMaster
        WHERE TaxGroupId = @TaxGroupId;
    END

    /* =========================
       SELECT ALL
    ========================= */
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT *
        FROM dbo.TabTaxGroupMaster;
    END

    /* =========================
       SELECT BY ID
    ========================= */
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM dbo.TabTaxGroupMaster
        WHERE TaxGroupId = @TaxGroupId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TaxGroupDetail_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TaxGroupDetail_Master]
    @Action varchar(30),
    @TaxGroupDetailId int = NULL,
    @TaxGroupId int = NULL,
    @TaxId int = NULL,
    @ApplyOrder int = NULL
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @SupplyType varchar(20), @TaxType varchar(50), @TaxCount int;

    SELECT @SupplyType = SupplyType
    FROM TabTaxGroupMaster
    WHERE TaxGroupId = @TaxGroupId;

    SELECT @TaxType = TaxType
    FROM TabTaxMaster
    WHERE TaxId = @TaxId;

	SELECT @TaxCount = COUNT(*)
    FROM TabTaxGroupDetail
    WHERE TaxGroupId = @TaxGroupId;
    ------------------------------------------------
    -- INSERT
    ------------------------------------------------
    IF @Action = 'INSERT'
    BEGIN
        -- Prevent duplicate tax
        IF EXISTS (
            SELECT 1 FROM TabTaxGroupDetail
            WHERE TaxGroupId = @TaxGroupId AND TaxId = @TaxId
        )
        BEGIN
            RAISERROR('Tax already exists in this group',16,1);
            RETURN;
        END

            IF @SupplyType = 'INTRA'
        BEGIN
            IF @TaxType = 'IGST'
                RAISERROR('IGST not allowed for INTRA supply',16,1);

            IF @TaxCount >= 2
                RAISERROR('INTRA tax group can have only CGST + SGST',16,1);

            IF @ApplyOrder NOT IN (1,2)
                RAISERROR('ApplyOrder must be 1 or 2 for INTRA',16,1);
        END

        IF @SupplyType = 'INTER'
        BEGIN
            IF @TaxType <> 'IGST'
                RAISERROR('Only IGST allowed for INTER supply',16,1);

            IF @TaxCount >= 1
                RAISERROR('INTER tax group can have only one IGST',16,1);

            IF @ApplyOrder <> 1
                RAISERROR('ApplyOrder must be 1 for INTER',16,1);
        END

        INSERT INTO dbo.TabTaxGroupDetail (TaxGroupId, TaxId, ApplyOrder)
        VALUES (@TaxGroupId, @TaxId, @ApplyOrder);
    END

    ------------------------------------------------
    -- UPDATE
    ------------------------------------------------
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.TabTaxGroupDetail
        SET TaxGroupId = @TaxGroupId,
            TaxId = @TaxId,
            ApplyOrder = @ApplyOrder
        WHERE TaxGroupDetailId = @TaxGroupDetailId;
    END

    ------------------------------------------------
    -- DELETE
    ------------------------------------------------
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM dbo.TabTaxGroupDetail
        WHERE TaxGroupDetailId = @TaxGroupDetailId;
    END

    ------------------------------------------------
    -- SELECT ALL (WITH DETAILS FOR UI)
    ------------------------------------------------
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT
            d.TaxGroupDetailId,
            d.TaxGroupId,
            g.GroupName,
            d.TaxId,
            t.TaxName,
            t.RatePercent,
            t.TaxType,
            d.ApplyOrder
        FROM dbo.TabTaxGroupDetail d
        INNER JOIN dbo.TabTaxGroupMaster g ON g.TaxGroupId = d.TaxGroupId
        INNER JOIN dbo.TabTaxMaster t ON t.TaxId = d.TaxId
        ORDER BY g.GroupName, d.ApplyOrder;
    END

    ------------------------------------------------
    -- SELECT BY ID
    ------------------------------------------------
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM dbo.TabTaxGroupDetail
        WHERE TaxGroupDetailId = @TaxGroupDetailId;
    END

    ------------------------------------------------
    -- SELECT BY TAX GROUP (VERY IMPORTANT)
    ------------------------------------------------
    ELSE IF @Action = 'SELECT_BY_GROUP'
    BEGIN
        SELECT
            d.TaxGroupDetailId,
            d.TaxId,
            t.TaxName,
            t.RatePercent,
            t.TaxType,
            d.ApplyOrder
        FROM dbo.TabTaxGroupDetail d
        INNER JOIN dbo.TabTaxMaster t ON t.TaxId = d.TaxId
        WHERE d.TaxGroupId = @TaxGroupId
        ORDER BY d.ApplyOrder;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TaxPreference_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TaxPreference_Master]
    @Action VARCHAR(30),
    @TaxPreferenceId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================
       SELECT ACTIVE (UI USE)
       (INTEGRATES HARDCODED QUERY #1)
    ========================= */
    IF @Action = 'SELECT_ACTIVE'
    BEGIN
        SELECT
            TaxPreferenceId,
            PreferenceCode,
            PreferenceName,
            IsGSTApplicable
        FROM TabTaxPreferenceMaster
        WHERE IsActive = 1
        ORDER BY TaxPreferenceId;
    END

    /* =========================
       SELECT BY ID
    ========================= */
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM TabTaxPreferenceMaster
        WHERE TaxPreferenceId = @TaxPreferenceId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TCS_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TCS_Master]
    @Action VARCHAR(20),
    @TCSId INT = NULL,
    @TaxName VARCHAR(50) = NULL,           -- ? added default
    @Rate DECIMAL(18,2) = NULL,
    @NatureOfCollection VARCHAR(100) = NULL,
    @IsHigherRate BIT = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- INSERT
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO [dbo].[TabTCSMaster]
        (
            [TaxName],                  -- ? added
            [Rate],
            [NatureOfCollection],
            [IsHigherRate],
            [StartDate],
            [EndDate],
            [CreatedAt]
        )
        VALUES 
        (
            @TaxName,                   -- ? added
            @Rate,
            @NatureOfCollection,
            @IsHigherRate,
            @StartDate,
            @EndDate,
            GETDATE()
        );
    END

    -- UPDATE
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE [dbo].[TabTCSMaster]
        SET 
            [TaxName] = @TaxName,       -- ? added
            [Rate] = @Rate,
            [NatureOfCollection] = @NatureOfCollection,
            [IsHigherRate] = @IsHigherRate,
            [StartDate] = @StartDate,
            [EndDate] = @EndDate
        WHERE [TCSId] = @TCSId;
    END

    -- DELETE
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE FROM [dbo].[TabTCSMaster]
        WHERE [TCSId] = @TCSId;
    END

    -- SELECT ALL
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT * FROM [dbo].[TabTCSMaster];
    END

    -- SELECT BY ID
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT * FROM [dbo].[TabTCSMaster]
        WHERE [TCSId] = @TCSId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_TDS_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_TDS_Master]
    @Action varchar(20),
    @TDSId int = NULL,
    @TaxName varchar(100) = NULL,
    @Rate decimal(18,2) = NULL,
    @Section varchar(50) = NULL,
    @IsHigherRate bit = NULL,
    @StartDate date = NULL,
    @EndDate date = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTDSMaster] ([TaxName], [Rate], [Section], [IsHigherRate], [StartDate], [EndDate], [CreatedAt])
        VALUES (@TaxName, @Rate, @Section, @IsHigherRate, @StartDate, @EndDate, GETDATE())
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTDSMaster]
        SET [TaxName] = @TaxName, [Rate] = @Rate, [Section] = @Section, [IsHigherRate] = @IsHigherRate,
            [StartDate] = @StartDate, [EndDate] = @EndDate
        WHERE [TDSId] = @TDSId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTDSMaster] WHERE [TDSId] = @TDSId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTDSMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTDSMaster] WHERE [TDSId] = @TDSId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Transporter_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Transporter_Master]
    @Action varchar(20),
    @TransporterId int = NULL,
    @TransporterName varchar(200) = NULL,
    @GSTNumber varchar(20) = NULL,
    @ContactPerson varchar(100) = NULL,
    @Phone varchar(20) = NULL,
    @Address varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabTransporterMaster] ([TransporterName], [GSTNumber], [ContactPerson], [Phone], [Address])
        VALUES (@TransporterName, @GSTNumber, @ContactPerson, @Phone, @Address)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabTransporterMaster]
        SET [TransporterName] = @TransporterName, [GSTNumber] = @GSTNumber, [ContactPerson] = @ContactPerson,
            [Phone] = @Phone, [Address] = @Address
        WHERE [TransporterId] = @TransporterId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabTransporterMaster] WHERE [TransporterId] = @TransporterId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabTransporterMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabTransporterMaster] WHERE [TransporterId] = @TransporterId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Unit_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Unit_Master]
    @Action        VARCHAR(20),   -- INSERT, UPDATE, DELETE, SELECT, SELECT_BY_ID
    @UnitId        INT = NULL,
    @UnitName      VARCHAR(20) = NULL,
    @UQC           VARCHAR(20) = NULL,
    @CRT           BIGINT = NULL,
    @PackingUnits  BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    /* =========================
       INSERT
    ========================= */
    IF @Action = 'INSERT'
    BEGIN
        INSERT INTO dbo.TabUnitMaster
        (
            UnitName,
            UQC,
            CRT,
            PackingUnits
        )
        VALUES
        (
            @UnitName,
            @UQC,
            @CRT,
            @PackingUnits
        );
    END

    /* =========================
       UPDATE
    ========================= */
    ELSE IF @Action = 'UPDATE'
    BEGIN
        UPDATE dbo.TabUnitMaster
        SET
            UnitName     = @UnitName,
            UQC          = @UQC,
            CRT          = @CRT,
            PackingUnits = @PackingUnits
        WHERE UnitId = @UnitId;
    END

    /* =========================
       DELETE
    ========================= */
    ELSE IF @Action = 'DELETE'
    BEGIN
        DELETE
        FROM dbo.TabUnitMaster
        WHERE UnitId = @UnitId;
    END

    /* =========================
       SELECT ALL
    ========================= */
    ELSE IF @Action = 'SELECT'
    BEGIN
        SELECT *
        FROM dbo.TabUnitMaster;
    END

    /* =========================
       SELECT BY ID
    ========================= */
    ELSE IF @Action = 'SELECT_BY_ID'
    BEGIN
        SELECT *
        FROM dbo.TabUnitMaster
        WHERE UnitId = @UnitId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_User_GetByEmail]    Script Date: 27-01-2026 6:15:12 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_User_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_User_Register]    Script Date: 27-01-2026 6:15:12 PM ******/
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
    
    -- Return the newly created user
    SELECT [UserId], [Username], [Email], [FullName], [Phone], [IsActive]
    FROM [dbo].[TabUserMaster]
    WHERE [UserId] = @UserId;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_User_UpdatePassword]    Script Date: 27-01-2026 6:15:12 PM ******/
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
/****** Object:  StoredProcedure [dbo].[sp_ValidateUser]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
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
/****** Object:  StoredProcedure [dbo].[sp_Warehouse_Master]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[sp_Warehouse_Master]
    @Action varchar(20),
    @WarehouseId int = NULL,
    @WarehouseName varchar(150) = NULL,
    @BranchId int = NULL,
    @Address varchar(300) = NULL
AS BEGIN
    SET NOCOUNT ON;
    IF @Action = 'INSERT'
        INSERT INTO [dbo].[TabWarehouseMaster] ([WarehouseName], [BranchId], [Address]) VALUES (@WarehouseName, @BranchId, @Address)
    ELSE IF @Action = 'UPDATE'
        UPDATE [dbo].[TabWarehouseMaster] SET [WarehouseName] = @WarehouseName, [BranchId] = @BranchId, [Address] = @Address WHERE [WarehouseId] = @WarehouseId
    ELSE IF @Action = 'DELETE'
        DELETE FROM [dbo].[TabWarehouseMaster] WHERE [WarehouseId] = @WarehouseId
    ELSE IF @Action = 'SELECT'
        SELECT * FROM [dbo].[TabWarehouseMaster]
    ELSE IF @Action = 'SELECT_BY_ID'
        SELECT * FROM [dbo].[TabWarehouseMaster] WHERE [WarehouseId] = @WarehouseId
END
GO
/****** Object:  StoredProcedure [dbo].[USP_GetUserSidebarAndPermissions]    Script Date: 27-01-2026 6:15:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USP_GetUserSidebarAndPermissions]
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
GO
