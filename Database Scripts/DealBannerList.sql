﻿CREATE TABLE [dbo].[DealBannerList](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DealId] [bigint] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[FranchiseId] [int] NOT NULL,
	[SequenceOrder] [int] NOT NULL,
	[ImageName] [nvarchar](300) NOT NULL,
	[Tooltip] [nvarchar](50) NULL,
	[ProductID] [bigint] NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[DeviceType] [nvarchar](50) NULL,
	[DeviceID] [nvarchar](50) NULL,
	[NetworkIP] [nvarchar](50) NULL,
	[Remarks] [nvarchar](50) NULL,
	[CategoryID] [bigint] NULL,
	[BrandId] [bigint] NULL,
	[ShopID] [bigint] NULL,
	[Keyword] [nvarchar](50) NULL,
	[OfferID] [bigint] NULL,
	[DisplayViewApp] [varchar](50) NULL,
	[LinkUrl] [varchar](200) NULL,
	[Offer_Category] [varchar](50) NULL,
 CONSTRAINT [PK_DealBannerList] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET IDENTITY_INSERT [dbo].[DealBannerList] ON 

INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (5, 1, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/2/5.jpg', N'Kissan Jam', NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, CAST(N'2019-02-01 19:18:20.180' AS DateTime), 1, N'Net Browser', N'x', N'::1', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', N'#kissan Jam', NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (7, 3, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 2, N'/10909/1052/2/7.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (8, 4, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/2/4.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (11, 5, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 2, N'/10909/1052/2/9.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (12, 6, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 2, N'/10909/1052/2/3.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (13, 7, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/2/10.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (14, 8, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/2/8.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (15, 9, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/2/12.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (16, 11, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 2, N'/10909/1052/2/13.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (21, 1, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/5.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (22, 2, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/6.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (23, 3, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/7.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (24, 4, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/4.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (25, 5, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/9.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (26, 6, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/3.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (27, 7, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/10.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (28, 8, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/8.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (29, 9, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/12.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (30, 11, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1054, 1, N'/21191/1054/2/13.jpg', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1170, NULL, NULL, NULL, NULL, N'OfferProductList', NULL, NULL)
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (38, 2, CAST(N'2019-01-01 00:00:00.000' AS DateTime), CAST(N'2019-02-04 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/2/3.jpg', N'2019', NULL, 1, CAST(N'2019-01-31 18:44:13.150' AS DateTime), 1, CAST(N'2019-02-01 19:17:24.547' AS DateTime), 1, N'Net Browser', N'x', N'::1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'#2019', N'2')
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (48, 1, CAST(N'2019-02-01 00:00:00.000' AS DateTime), CAST(N'2019-02-20 00:00:00.000' AS DateTime), 1037, 1, N'/683/1037/1/3.jpg', N'q', NULL, 1, CAST(N'2019-02-02 16:45:13.563' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'::1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'#', N'1')
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (50, 1, CAST(N'2019-02-05 00:00:00.000' AS DateTime), CAST(N'2019-02-28 00:00:00.000' AS DateTime), 1052, 2, N'/10909/1052/1/4.jpg', N'#product', NULL, 1, CAST(N'2019-02-04 16:29:03.320' AS DateTime), 1, CAST(N'2019-02-04 17:29:53.123' AS DateTime), 1, N'Net Browser', N'x', N'::1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'#a', N'1')
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (51, 1, CAST(N'2019-02-01 00:00:00.000' AS DateTime), CAST(N'2019-02-23 00:00:00.000' AS DateTime), 1052, 5, N'/10909/1052/1/5.jpg', N'Kissan Jam', NULL, 1, CAST(N'2019-02-04 16:58:13.817' AS DateTime), 1, CAST(N'2019-02-04 18:09:43.677' AS DateTime), 1, N'Net Browser', N'x', N'::1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'#kissan Ja', N'1')
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (52, 2, CAST(N'2019-02-09 00:00:00.000' AS DateTime), CAST(N'2019-02-23 00:00:00.000' AS DateTime), 1052, 2, N'/10909/1052/2/4.jpg', N'abcvs', NULL, 1, CAST(N'2019-02-06 16:43:38.127' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'::1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'#', N'1')
INSERT [dbo].[DealBannerList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp], [LinkUrl], [Offer_Category]) VALUES (53, 2, CAST(N'2019-02-02 00:00:00.000' AS DateTime), CAST(N'2019-02-16 00:00:00.000' AS DateTime), 1052, 3, N'/10909/1052/2/4.jpg', N'p', NULL, 1, CAST(N'2019-02-07 12:55:42.243' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'::1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'#p', N'1')
SET IDENTITY_INSERT [dbo].[DealBannerList] OFF