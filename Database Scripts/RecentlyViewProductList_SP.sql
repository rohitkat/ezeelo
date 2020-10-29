-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE RecentlyViewProductList 
	 @FranchiseID int 
	 ,@UserLoginId bigInt
AS
BEGIN
	Declare @Result table(RowNumber bigint, ProductID bigint,CategoryID bigint,ProductName varchar(150), StockStatus int,MRP decimal(18, 2),SaleRate decimal(18, 2),RetailPoint decimal(18,2)) -- Added RetailPoint by Sonali_29-11-2018
	Insert into @Result		
		SELECT	
		ROW_NUMBER() OVER
		(
			 
			  ORDER BY  p.ID desc
		)AS RowNumber		
	,p.ID, p.CategoryID, p.Name, ss.StockStatus, Min(ss.MRP), Min(ss.RetailerRate), ss.BusinessPoints --Added BusinessPoints by Sonali 29-11-2018 
	from RecentlyViewProduct rp
	inner join Product p on rp.ProductId = p.ID
	inner join ShopProduct sp on p.ID =sp.ProductID 
	inner join ShopStock ss on sp.ID = ss.ShopProductID 	
	inner join shop sh on sp.ShopID = sh.ID 
	inner join Pincode pin on sh.PincodeID = pin.ID 
	inner join City ct on pin.CityID = ct.ID -- Changes on Mohit on 18-12-15
	Where rp.UserLoginId =@UserLoginId And rp.FranchiseId = @FranchiseID And p.IsActive = 1 And sh.IsLive = 1 And sh.IsActive = 1 
	And sp.IsActive = 1 And ss.IsActive = 1 And pin.IsActive = 1 And ct.IsActive = 1 And ss.StockStatus = 1
	And sh.FranchiseID=@FranchiseID --Added by Ashish on 19-9-2016 for Multiple Franchise in same city
	And ss.WarehouseStockID is not null --Yashaswi 22-11-2018
	group by p.Id, p.CategoryID,p.Name, ss.StockStatus , ss.BusinessPoints -- Added BusinessPoints by Sonali 29-11-2018 
	
	select * from @Result
END
GO
