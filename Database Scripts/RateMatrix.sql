USE [Ezeelo]
GO

/****** Object:  Table [dbo].[RateMatrix]    Script Date: 4/10/2019 11:10:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RateMatrix](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductId] [bigint] NULL,
	[ProductVarientId] [bigint] NULL,
	[MRP] [float] NULL,
	[GSTInPer] [int] NULL,
	[GrossMarginFlat] [float] NULL,
	[DecidedSalePrice] [float] NULL,
	[ValuePostGST] [float] NULL,
	[Dividend] [float] NULL,
	[BaseInwardPriceEzeelo] [float] NULL,
	[InwardMarginValue] [float] NULL,
	[RateExpiry] [datetime] NULL,
	[GSTOnPR] [float] NULL,
	[MaxInwardMargin] [float] NULL,
	[MarginPassedToCustomer] [float] NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
	[NetworkIP] [nvarchar](15) NULL,
	[DeviceType] [nvarchar](50) NULL,
	[DeviceID] [nvarchar](50) NULL,
	[ActualFlatMargin] [float] NULL
) ON [PRIMARY]

GO


