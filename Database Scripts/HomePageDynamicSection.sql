CREATE TABLE [dbo].[HomePageDynamicSection](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SectionId] [bigint] NULL,
	[SectionDisplayName] [nvarchar](500) NULL,
	[SectionStyle] [nvarchar](max) NULL,
	[SequenceOrder] [int] Not NULL,
	[ShowInApp] [bit] Not NULL,
	[IsActive] [bit] Not NULL,
	[FranchiseId] [bigint] Not NULL,
	[IsBanner] [bit]Not NULL,
	[IsProduct] [bit]Not NULL,
	[IsCategory] [bit]Not NULL,
	[CreateBy] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[ModifyDate] [datetime] NULL,
	[NetworkIP] [nvarchar](15) NULL,
 CONSTRAINT [PK_HomePageDynamicSection] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
