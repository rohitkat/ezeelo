SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HomePageDynamicSectionsMaster](
	[ID] [bigint]NOT NULL,
	[SectionID] [int]NOT NULL,
	[Section] [nvarchar](500) NULL,
	[SectionHeader] [nvarchar](500) NULL,
	[Description] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[MobileImgWidth] [decimal](18, 0) NULL,
	[MobileImgHeight] [decimal](18, 0) NULL,
	[MobileImgSize] [varchar](50) NULL,
	[PortalImgWidth] [decimal](18, 0) NULL,
	[PortalImgHeight] [decimal](18, 0) NULL,
	[PortalImgSize] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreatedBy] [bigint] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[NetworkIp] [varchar](15) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

SET IDENTITY_INSERT [dbo].[HomePageDynamicSectionProduct] OFF
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (1, 1, N'Main Section', N'Main Banner',  0, 1, CAST(0 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), N'0', CAST(0 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), N'0', CAST(N'2019-07-02 00:00:00.000' AS DateTime), 2, NULL, NULL, N':::1')
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (2, 2, N'Leaders Section', N'Leaders Section New User',  0, 1, CAST(0 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), N'0', CAST(0 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), N'0', CAST(N'2019-07-02 00:00:00.000' AS DateTime), 2, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (3, 3, N'Leaders Section', N'Leaders Section Registered User', 0, 1, CAST(0 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), N'0', CAST(0 AS Decimal(18, 0)), CAST(0 AS Decimal(18, 0)), N'0', CAST(N'2019-07-02 00:00:00.000' AS DateTime), 2, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (4, 4, N'Category Section ', N'Shop By Category ',  0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (5, 5, N'Offer Section 1', N'Season''s Special',  0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (6, 6, N'Offer Section 2', N'Hot Deals and offers',  1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (7, 7, N'Offer Section 3', N'Deals of the day', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (8, 8, N'Offer Section 4', N'Major Retail points',  1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (9, 9, N'Offer Section 5', N'48 Hours Deal', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (10, 10, N'Offer Section 6', N'Newly Launched',  1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (11, 11, N'Offer Section 7', N'Trending Deals',  1, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (12, 12, N'Category Section 1', N'Grocery And Staples', 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (13, 13, N'Category Section 2', N'Personal Care', 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (14, 14, N'Category Section 3', N'House Hold', 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (15, 15, N'Category Section 4', N'Branded Food', 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (16, 16, N'Category Section 5', N'Baby Care', 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[HomePageDynamicSectionsMaster] ([ID], [SectionID], [Section], [SectionHeader], [Description],  [IsActive], [MobileImgWidth], [MobileImgHeight], [MobileImgSize], [PortalImgWidth], [PortalImgHeight], [PortalImgSize], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [NetworkIp]) VALUES (17, 17, N'Brand Section', N'Shop by Brands', 0, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO