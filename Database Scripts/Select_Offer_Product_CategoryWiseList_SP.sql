--Addded OfferType in Select_Offer_Product_CategoryWiseList StoreProcedure by Sonali on 08-02-2019
USE [Ezeelo]
GO
/****** Object:  StoredProcedure [dbo].[Select_Offer_Product_CategoryWiseList]    Script Date: 2/8/2019 1:10:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author: Ajit Jain>
-- Create date: <Create Date: 04 Feb 2016>
-- Description:	<Description: Offer Product Category wise>
--exec Select_Offer_Product_CategoryWiseList 4968,1,0,null,null --old
--exec Select_Offer_Product_CategoryWiseList 4968,2,3,0,1313,null --new
--exec Select_Offer_Product_CategoryWiseList 4968,2,1,1313,null,null --new
-- =============================================
--**************  change done for multiple franchise in same city by Ashish *****************
ALTER PROCEDURE [dbo].[Select_Offer_Product_CategoryWiseList] 
	@CityId Bigint,
	@FranchiseId int=null,--added multiple MCO in same city
	@offerStatus int, ---All Avaiable--1 ,Missed Deal--0, Upcoming--2
	@CategoryID int,----- All-0,CategoryID=@CategoryID
	@PageIndex int,
	@PageSize int
AS
BEGIN
--select VWPW.categoryIDFirstLevel,VWPW.NameFirstLevel,*
--from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
--inner join Offer O on OZP.OfferID=O.ID
--inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
--inner join ShopStock SS on OZP.ShopStockID=SS.ID
--inner join ShopProduct SP on SS.ShopProductID= SP.ID
--inner join Shop S on SP.ShopID=S.ID
--where S.PincodeID in (select ID from Pincode where CityID=4968) 
--and S.IsActive=1 and S.IsLive=1 
--and OZP.FreeStockID Is Null
--select VWPW.ProductID,VWPW.categoryIDFirstLevel,VWPW.NameFirstLevel,*
--from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
--inner join Offer O on OZP.OfferID=O.ID
--inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
--inner join ShopStock SS on OZP.ShopStockID=SS.ID
--inner join ShopProduct SP on SS.ShopProductID= SP.ID
--inner join Shop S on SP.ShopID=S.ID
--where S.PincodeID in (select ID from Pincode where CityID=4968) 
--and S.IsActive=1 and S.IsLive=1 
--and OZP.FreeStockID Is Null
if(@offerStatus=1)
begin
if(@FranchiseId is null OR @FranchiseId=0)
begin
--- Old Code for Old APK  ---
--print 'C'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OD.StartDateTime As OfferStartTime,OD.EndDateTime As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.ShortName As OfferName,
O.DiscountInRs As RsOffer,O.DiscountInPercent As [%Offer]
,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
 ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
 ,SS.OfferType -- Added by Sonali 21-01-2019
from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
inner join Offer O on OZP.OfferID=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.PincodeID in (select ID from Pincode where CityID=@CityId) 
and S.IsActive=1 and S.IsLive=1 
and OZP.FreeStockID Is Null
and OD.IsActive=1
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OD.StartDateTime <= GETDATE() and OD.EndDateTime > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
else
begin
--- New Code for Multiple MCO  ---
--print 'F'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OD.StartDateTime As OfferStartTime,OD.EndDateTime As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.ShortName As OfferName,
O.DiscountInRs As RsOffer,O.DiscountInPercent As [%Offer]
,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
  ,SS.OfferType -- Added by Sonali 21-01-2019
from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
inner join Offer O on OZP.OfferID=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.FranchiseID=@FranchiseId--added multiple MCO in same city
and S.IsActive=1 and S.IsLive=1 
and OZP.FreeStockID Is Null
and OD.IsActive=1
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OD.StartDateTime <= GETDATE() and OD.EndDateTime > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
end
else if(@offerStatus=2)
begin
if(@FranchiseId is null OR @FranchiseId=0)
begin
--- Old Code for Old APK  ---
--print 'C'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OD.StartDateTime As OfferStartTime,OD.EndDateTime As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.ShortName As OfferName,
O.DiscountInRs As RsOffer,O.DiscountInPercent As [%Offer]
,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
  ,SS.OfferType -- Added by Sonali 21-01-2019
from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
inner join Offer O on OZP.OfferID=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.PincodeID in (select ID from Pincode where CityID=@CityId) 
and S.IsActive=1 and S.IsLive=1 
and OZP.FreeStockID Is Null
and OD.IsActive=1
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OD.StartDateTime <= GETDATE() and OD.EndDateTime > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
else
begin
--- New Code for Multiple MCO  ---
--print 'F'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OD.StartDateTime As OfferStartTime,OD.EndDateTime As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.ShortName As OfferName,
O.DiscountInRs As RsOffer,O.DiscountInPercent As [%Offer]
,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
  ,SS.OfferType -- Added by Sonali 21-01-2019
from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
inner join Offer O on OZP.OfferID=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.FranchiseID=@FranchiseId--added multiple MCO in same city
and S.IsActive=1 and S.IsLive=1 
and OZP.FreeStockID Is Null
and OD.IsActive=1
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OD.StartDateTime <= GETDATE() and OD.EndDateTime > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
end
else if(@offerStatus=3)
begin
if(@FranchiseId is null OR @FranchiseId=0)
begin
--- Old Code for Old APK  ---
--print 'C'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OD.StartDateTime As OfferStartTime,OD.EndDateTime As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.ShortName As OfferName,
O.DiscountInRs As RsOffer,O.DiscountInPercent As [%Offer]
,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
  ,SS.OfferType -- Added by Sonali 21-01-2019
from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
inner join Offer O on OZP.OfferID=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.PincodeID in (select ID from Pincode where CityID=@CityId)
and S.IsActive=1 and S.IsLive=1 
and OZP.FreeStockID Is Null
and OD.IsActive=1
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1  and O.IsActive=1--Added by Tejaswee
and OD.StartDateTime <= GETDATE() and OD.EndDateTime > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
else
begin
--- New Code for Multiple MCO  ---
--print 'F'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OD.StartDateTime As OfferStartTime,OD.EndDateTime As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.ShortName As OfferName,
O.DiscountInRs As RsOffer,O.DiscountInPercent As [%Offer]
,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
  ,SS.OfferType -- Added by Sonali 21-01-2019
from OfferZoneProduct OZP inner join OfferDuration  OD on OZP.OfferID=OD.OfferID 
inner join Offer O on OZP.OfferID=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.FranchiseID=@FranchiseId--added multiple MCO in same city
and S.IsActive=1 and S.IsLive=1 
and OZP.FreeStockID Is Null
and OD.IsActive=1
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1  and O.IsActive=1--Added by Tejaswee
and OD.StartDateTime <= GETDATE() and OD.EndDateTime > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
end
--SELECT @RecordCount = COUNT(*) FROM @TempResult
--		Set @Productcount = @RecordCount 
--		Print  @Productcount
--		SET @PageCount = CEILING(CAST(@RecordCount AS DECIMAL(10, 2)) / CAST(@PageSize AS DECIMAL(10, 2)))
--		PRINT   @PageCount
		
--		SELECT * FROM @TempResult
--		WHERE   RowNumber BETWEEN(@PageIndex -1) * @PageSize + 1 AND(((@PageIndex -1) * @PageSize + 1) + @PageSize) - 1	
				
END