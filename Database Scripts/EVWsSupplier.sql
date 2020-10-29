USE [Ezeelo]
GO

/****** Object:  Table [dbo].[EVWsSupplier]    Script Date: 4/10/2019 11:20:31 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EVWsSupplier](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[WarehouseId] [bigint] NULL,
	[SupplierId] [bigint] NULL,
	[IsActive] [bit] NULL,
	[CreateBy] [bigint] NULL,
	[CreateDate] [datetime] NULL,
	[ModifyBy] [bigint] NULL,
	[ModifyDate] [datetime] NULL,
	[NetworkIP] [nvarchar](15) NULL,
 CONSTRAINT [PK_EVWsSupplier] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


