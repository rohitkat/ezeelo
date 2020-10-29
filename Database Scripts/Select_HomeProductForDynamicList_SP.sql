CREATE procedure [dbo].[Select_HomeProductForDynamicList]
(
@HomepageDynamicSectionID as int,
@StartDate as datetime,
@EndDate as datetime
)
as 

begin 

select BI.id ,p.name from HomePageDynamicSectionProduct  BI
inner join Shopstock ss on ss.ID=BI.ShopStockId
inner join shopproduct sp on sp.ID=ss.shopproductID
inner join product p on sp.productid=p.id
inner join HomePageDynamicSection DB on  DB.ID=BI.HomePageDynamicSectionID
where  BI.HomepageDynamicSectionID = @HomepageDynamicSectionID and BI.StartDate >= @StartDate and BI.EndDate <= @EndDate and BI.IsActive = 1
--cast(BI.StartDate as date)>=cast(@StartDate as date)
--and cast(BI.EndDate as date)<=cast(@EndDate as date)

end