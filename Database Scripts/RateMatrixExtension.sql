USE [Ezeelo]
GO

/****** Object:  Table [dbo].[RateMatrixExtension]    Script Date: 4/10/2019 11:11:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RateMatrixExtension](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EVWID] [bigint] NULL,
	[ProductVarientId] [bigint] NULL,
	[RateMatrixId] [bigint] NULL,
	[FVID] [bigint] NULL,
	[FVMargin] [float] NULL,
	[DVPurchasePrice] [float] NULL,
	[DVSalePrice] [float] NULL,
	[FVMarginValueWithGST] [float] NULL,
	[FVGST] [float] NULL,
	[DVID] [bigint] NULL,
	[DVMargin] [float] NULL,
	[FVPurchasePrice] [float] NULL,
	[FVSalePrice] [float] NULL,
	[DVMarginValueWithGST] [float] NULL,
	[DVGST] [float] NULL,
	[MarginLeftForEzeeloBeforeLeadershipPayout] [float] NULL,
	[GSTForEzeeloMargin] [float] NULL,
	[PostGSTMargin] [float] NULL,
	[ForLeadershipPercent] [float] NULL,
	[ForLeadershipValue] [float] NULL,
	[ForEzeeloPercent] [float] NULL,
	[ForEzeeloValue] [float] NULL,
	[ForLeadersRoyaltyPercent] [float] NULL,
	[ForLeadersRoyaltyValue] [float] NULL,
	[ForLifestyleFundPercent] [float] NULL,
	[ForLifestyleFundValue] [float] NULL,
	[ForLeadershipDevelopmentFundPercent] [float] NULL,
	[ForLeadershipDevelopmentFundValue] [float] NULL,
	[RetailPoint] [float] NULL,
	[TotalGSTInSupplyChain] [float] NULL,
	[TotalMargin] [float] NULL,
	[OneBPInPaise] [float] NULL,
	[IsActive] [bit] NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [bigint] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[NetworkIP] [nvarchar](15) NULL,
 CONSTRAINT [PK_RateMatrixExtension] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


