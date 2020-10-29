USE [Ezeelo]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePurchaseOrderDetail]    Script Date: 4/8/2019 2:58:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zubair Ahmad
-- Create date: 05-10-2017
-- Description:	Inser/Update/Delete in PurchaseOrderDetail Table
-- =============================================
DROP PROCEDURE [dbo].[UpdatePurchaseOrderDetail]
Create PROCEDURE [dbo].[UpdatePurchaseOrderDetail]
	(
	@PurchaseOrderID bigint,
	@TblTypePurchaseOrderDetail TblTypePurchaseOrderDetail ReadOnly
	)
AS
BEGIN

      -- Delete
	  DELETE P FROM PurchaseOrderDetail P
	  LEFT JOIN @TblTypePurchaseOrderDetail T ON(P.PurchaseOrderID = T.PurchaseOrderID AND P.ProductID = T.ProductID AND P.ProductVarientID = T.ProductVarientID)
	  WHERE T.PurchaseOrderDetailID IS NULL AND P.PurchaseOrderID = @PurchaseOrderID

	  --Insert
	  INSERT INTO PurchaseOrderDetail (
	  P.PurchaseOrderID,
	  P.ProductID,
	  P.ProductNickname,
	  P.ProductVarientID,
	  P.Quantity,
	  p.UnitPrice,
	  p.RateCalculationId,
	  p.RateMatrixID,
	  p.RateMatrixExtensionID,
	  P.Remark,
	  P.IsActive
	  )	  
	  SELECT T.PurchaseOrderID,T.ProductID,T.ProductNickname,T.ProductVarientID,T.Quantity,T.UnitPrice,T.RateCalculationID,T.RateMatrixID,T.RateMatrixExtensionID,T.Remark,T.IsActive
	  FROM @TblTypePurchaseOrderDetail T 
	  LEFT JOIN PurchaseOrderDetail P ON(P.PurchaseOrderID = T.PurchaseOrderID AND P.ProductID = T.ProductID AND P.ProductVarientID = T.ProductVarientID)
	  WHERE P.PurchaseOrderID IS NULL


	  -- Update
	  UPDATE P SET P.ProductNickname = T.ProductNickname,
	              P.Quantity = T.Quantity,
				  P.Remark = T.Remark,
				  P.UnitPrice = T.UnitPrice,
				  p.RateCalculationId = T.RateCalculationID,
				  p.RateMatrixId =T.RateMatrixID,
				  p.RateMatrixExtensionId = T.RateMatrixExtensionID
	 FROM PurchaseOrderDetail P
	 INNER JOIN @TblTypePurchaseOrderDetail T ON(P.PurchaseOrderID = T.PurchaseOrderID AND P.ProductID = T.ProductID AND P.ProductVarientID = T.ProductVarientID)
	 WHERE P.ProductNickname<>T.ProductNickname OR P.Quantity<>T.Quantity OR P.Remark<> T.Remark OR P.UnitPrice<>T.UnitPrice
	 AND P.PurchaseOrderID = @PurchaseOrderID

END

