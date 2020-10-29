USE [Ezeelo]
GO

DROP TYPE [dbo].[TblTypePurchaseOrderDetail]
/****** Object:  UserDefinedTableType [dbo].[TblTypePurchaseOrderDetail]    Script Date: 4/8/2019 2:49:06 PM ******/
CREATE TYPE [dbo].[TblTypePurchaseOrderDetail] AS TABLE(
	[PurchaseOrderDetailID] [bigint] NULL,
	[PurchaseOrderID] [bigint] NULL,
	[ProductID] [bigint] NULL,
	[ProductNickname] [varchar](500) NULL,
	[ProductVarientID] [bigint] NULL,
	[Quantity] [int] NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[RateCalculationID] [bigint] NULL,
	[RateMatrixId] [bigint] NULL,
	[RateMatrixExtensionId] [bigint] NULL,
	[Remark] [varchar](500) NULL,
	[IsActive] [bit] NULL
)
GO


