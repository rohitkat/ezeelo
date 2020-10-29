CREATE PROCEDURE [dbo].[Select_HomePage_ProductList]
	-- Add the parameters for the stored procedure here
	@CityId Bigint,
	@FranchiseId int=null,--added multiple MCO in same city
	@offerStatus int, ---All Avaiable--1 ,Missed Deal--0, Upcoming--2
	@CategoryID int,----- All-0,CategoryID=@CategoryID
	@HomePageSectionId int,
	@PageIndex int,
	@PageSize int
AS
BEGIN
	if(@offerStatus=1)
begin
if(@FranchiseId is null OR @FranchiseId=0)
begin
--- Old Code for Old APK  ---
--print 'C'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OZP.StartDate As OfferStartTime,OZP.EndDate As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.SectionDisplayName As OfferName,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
 ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
from HomePageDynamicSectionProduct OZP 
inner join HomePageDynamicSection O on OZP.HomePageDynamicSectionId=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.PincodeID in (select ID from Pincode where CityID=@CityId)
and OZP.HomePageDynamicSectionId = 1 
and S.IsActive=1 and S.IsLive=1 
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OZP.StartDate <= GETDATE() and OZP.EndDate > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
else
begin
--- New Code for Multiple MCO  ---
--print 'F'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OZP.StartDate As OfferStartTime,OZP.EndDate As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.SectionDisplayName As OfferName,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
from HomePageDynamicSectionProduct OZP 
inner join HomePageDynamicSection O on OZP.HomePageDynamicSectionId=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.FranchiseID=@FranchiseId--added multiple MCO in same city
and OZP.HomePageDynamicSectionId = @HomePageSectionId 
and S.IsActive=1 and S.IsLive=1 
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OZP.StartDate <= GETDATE() and OZP.EndDate > getDate() -- Added by Tejaswee
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
SS.Qty As StockQty,SS.StockStatus,OZP.StartDate As OfferStartTime,OZP.EndDate As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.SectionDisplayName As OfferName,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
from HomePageDynamicSectionProduct OZP 
inner join HomePageDynamicSection O on OZP.HomePageDynamicSectionId=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.PincodeID in (select ID from Pincode where CityID=@CityId) 
and OZP.HomePageDynamicSectionId = @HomePageSectionId 
and S.IsActive=1 and S.IsLive=1 
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OZP.StartDate <= GETDATE() and OZP.EndDate > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
else
begin
--- New Code for Multiple MCO  ---
--print 'F'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OZP.StartDate As OfferStartTime,OZP.EndDate As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.SectionDisplayName As OfferName,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
from HomePageDynamicSectionProduct OZP
inner join HomePageDynamicSection O on OZP.HomePageDynamicSectionId=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.FranchiseID=@FranchiseId--added multiple MCO in same city
and OZP.HomePageDynamicSectionId = @HomePageSectionId 
and S.IsActive=1 and S.IsLive=1 
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1 and O.IsActive=1 --Added by Tejaswee
and OZP.StartDate <= GETDATE() and OZP.EndDate > getDate() -- Added by Tejaswee
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
SS.Qty As StockQty,SS.StockStatus,OZP.StartDate As OfferStartTime,OZP.EndDate As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.SectionDisplayName As OfferName,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
from HomePageDynamicSectionProduct OZP 
inner join HomePageDynamicSection O on OZP.HomePageDynamicSectionId=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.PincodeID in (select ID from Pincode where CityID=@CityId)
and OZP.HomePageDynamicSectionId = @HomePageSectionId 
and S.IsActive=1 and S.IsLive=1 
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1  and O.IsActive=1--Added by Tejaswee
and OZP.StartDate <= GETDATE() and OZP.EndDate > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
else
begin
--- New Code for Multiple MCO  ---
--print 'F'
select VWPW.ProductID,VWPW.ShopStockID,VWPW.Name As ProductName,
VWPW.categoryIDFirstLevel As CategoryID,VWPW.NameFirstLevel As CategoryName,
SS.Qty As StockQty,SS.StockStatus,OZP.StartDate As OfferStartTime,OZP.EndDate As OfferEndTime,
SS.MRP,SS.RetailerRate As SaleRate,O.ID As OfferID,O.SectionDisplayName As OfferName,
VWPW.shortDescription,VWPW.Color,VWPW.Size,VWPW.Material,VWPW.Dimension
 ,S.ID As ShopID,S.Name As ShopName  --Added by Tejaswee for API
  ,SS.BusinessPoints   ---Added by Yashaswi 9-7-2018
from HomePageDynamicSectionProduct OZP 
inner join HomePageDynamicSection O on OZP.HomePageDynamicSectionId=O.ID
inner join VW_ProductWiseCategory VWPW on OZP.ShopStockID=VWPW.shopstockID
inner join ShopStock SS on OZP.ShopStockID=SS.ID
inner join ShopProduct SP on SS.ShopProductID= SP.ID
inner join Shop S on SP.ShopID=S.ID
where S.FranchiseID=@FranchiseId--added multiple MCO in same city
and OZP.HomePageDynamicSectionId = @HomePageSectionId 
and S.IsActive=1 and S.IsLive=1 
and VWPW.categoryIDFirstLevel=(case when @CategoryID=0 then VWPW.categoryIDFirstLevel else  @CategoryID end)
and OZP.IsActive=1  and O.IsActive=1--Added by Tejaswee
and OZP.StartDate <= GETDATE() and OZP.EndDate > getDate() -- Added by Tejaswee
and SP.IsActive=1  and SS.IsActive=1   -- Added by Tejaswee
and SS.Qty>0 ---Added By Ashwini To Avoid showing Out Of Stock Products/Varient 
end
end
END

GO