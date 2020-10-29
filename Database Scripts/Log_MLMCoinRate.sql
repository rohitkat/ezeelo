SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Log_MLMCoinRate](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateBy] [bigint] NOT NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[NetworkIP] [varchar](50) NULL,
	[DeviceType] [varchar](15) NULL,
	[DeviceID] [varchar](15) NULL,
	[Last_Create_Date] [datetime] NULL,
 CONSTRAINT [PK_Log_MLMCoinRate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Log_MLMCoinRate]  WITH CHECK ADD  CONSTRAINT [FK_Log_MLMCoinRate_Log_MLMCoinRate] FOREIGN KEY([ID])
REFERENCES [dbo].[Log_MLMCoinRate] ([ID])
GO
ALTER TABLE [dbo].[Log_MLMCoinRate] CHECK CONSTRAINT [FK_Log_MLMCoinRate_Log_MLMCoinRate]
GO