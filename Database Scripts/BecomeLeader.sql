CREATE TABLE [dbo].[BecomeLeader](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NULL,
	[Email] [varchar](500) NULL,
	[Phone] [varchar](500) NULL,
	[PinCode] [varchar](500) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [bigint] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[NetworkIP] [varchar](500) NULL,
	[DeviceType] [varchar](500) NULL,
	[DeviceID] [varchar](500) NULL,
 CONSTRAINT [PK_BecomeLeader] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

SET IDENTITY_INSERT [dbo].[BecomeLeader] ON 

INSERT [dbo].[BecomeLeader] ([ID], [Name], [Email], [Phone], [PinCode], [CreateDate], [CreateBy], [ModifyDate], [ModifyBy], [NetworkIP], [DeviceType], [DeviceID]) VALUES (1, N'Ruma', N'ruma@gmail.com', N'9372764668', N'440018', CAST(N'2019-03-07 18:07:57.433' AS DateTime), 1, NULL, NULL, N'::1', N'xyz', N'xyz123')
INSERT [dbo].[BecomeLeader] ([ID], [Name], [Email], [Phone], [PinCode], [CreateDate], [CreateBy], [ModifyDate], [ModifyBy], [NetworkIP], [DeviceType], [DeviceID]) VALUES (2, N'Sonali', N'sonalip.warhade@gmail.com', N'7057601102', N'440024', CAST(N'2019-03-07 18:45:55.267' AS DateTime), 3, CAST(N'2019-03-07 19:12:37.117' AS DateTime), NULL, N'::1', N'Mobile', N'123456789')
SET IDENTITY_INSERT [dbo].[BecomeLeader] OFF