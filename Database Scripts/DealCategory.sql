CREATE TABLE [dbo].[DealCategoryList](
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
 CONSTRAINT [PK_DealCategoryList] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET IDENTITY_INSERT [dbo].[DealCategoryList] ON 

INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (1, 8, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/1.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (2, 8, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/2.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (3, 9, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/3.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (4, 9, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/13.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (5, 10, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/5.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (6, 10, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/6.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (7, 10, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/7.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (8, 10, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/8.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (9, 12, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/9.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
INSERT [dbo].[DealCategoryList] ([ID], [DealId], [StartDate], [EndDate], [FranchiseId], [SequenceOrder], [ImageName], [Tooltip], [ProductID], [IsActive], [CreateDate], [CreatedBy], [ModifyDate], [ModifyBy], [DeviceType], [DeviceID], [NetworkIP], [Remarks], [CategoryID], [BrandId], [ShopID], [Keyword], [OfferID], [DisplayViewApp]) VALUES (10, 12, CAST(N'2018-12-31 00:00:00.000' AS DateTime), CAST(N'2019-12-02 00:00:00.000' AS DateTime), 1052, 1, N'/10909/1052/3/10.png', NULL, NULL, 1, CAST(N'2018-12-31 19:01:09.257' AS DateTime), 1, NULL, NULL, N'Net Browser', N'x', N'103.252.171.105', NULL, 1072, NULL, NULL, NULL, NULL, N'productlist')
SET IDENTITY_INSERT [dbo].[DealCategoryList] OFF