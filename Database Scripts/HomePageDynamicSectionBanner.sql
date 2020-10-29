CREATE TABLE [dbo].[HomePageDynamicSectionBanner](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[HomePageDynamicSectionId] [bigint] NOt NULL,
	[ImageName] [nvarchar](max) NULL,
	[SequenceOrder] [int]Not NULL,
	[ToolTip] [nvarchar](500) NULL,
	[StartDate] [datetime] Not NULL,
	[EndDate] [datetime]Not NULL,
	[LinkURL] [nvarchar](1000) NULL,
	[CategoryID] [bigint] NULL,
	[BrandID] [bigint] NULL,
	[ShopId] [bigint] NULL,
	[Keyword] [nvarchar](50) NULL,
	[OfferId] [bigint] NULL,
	[DisplayViewApp] [nvarchar](50) NULL,
	[IsActive] [bit]Not NULL,
	[IsBanner] [bit]Not NULL,
	[CreateDate] [datetime] NULL,
	[CreatedBy] [bigint] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[NetworkIp] [varchar](15) NULL,
 CONSTRAINT [PK_HomePageDynamicSectionBanners] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
