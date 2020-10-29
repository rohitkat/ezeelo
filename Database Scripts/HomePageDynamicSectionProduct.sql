SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HomePageDynamicSectionProduct](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[HomePageDynamicSectionId] [bigint]NOT NULL,
	[ShopStockId] [bigint]NOT NULL,
	[SequenceOrder] [int]NOT NULL,
	[IsActive] [bit]NOT NULL,
	[StartDate] [datetime]NOT NULL,
	[EndDate] [datetime]NOT NULL,
	[CreateDate] [datetime]NOT NULL,
	[CreatedBy] [bigint]NOT NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[NetworkIp] [varchar](15) NULL,
 CONSTRAINT [PK_HomePageDynamicSectionProducts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO