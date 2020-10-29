--Added DisplayViewApp in Select_HomePageBlockItemList storeprocedure by Sonali on 08-02-2019
USE [Ezeelo]
GO
/****** Object:  StoredProcedure [dbo].[Select_HomePageBlockItemList]    Script Date: 2/8/2019 12:57:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
------------------------------------------------
/* Select Block Item List for home page in customer 
written by : Snehal Shende
 Procedure for select Block Item List for home page in customer module in web*/
 --exec Select_HomePageBlockItemList 2
 --***** Changes are done for Multiple MCO in same city- By Ashish on 16/9/2016 ********
ALTER procedure [dbo].[Select_HomePageBlockItemList]
(
 --@CityID as int --hide for Multiple MCO in same city
 @FranchiseID as int --added for Multiple MCO in same city
)
as 
begin 
--hide for Multiple MCO in same city--
--Declare @FranchiseID int
--select top(1) @FranchiseID=F.ID from Franchise F
--inner join FranchiseLocation FL on FL.FranchiseID=F.ID
--inner join Area A on A.ID = FL.AreaID
--inner join Pincode P on P.ID =A.PincodeID and P.CityID =@CityID
--where F.ID != 1
--group by F.ID
--end hide--
/*to get design block types list*/
Select Distinct DBT.ID,DBT.[Name],DBT.[ImageWidth],DBT.[ImageHeight],DBT.[MaxLimit],DBT.[IsActive] from DesignBlockType DBT
inner join BlockItemsList BI on DBT.ID = BI.DesignBlockTypeID and BI.FranchiseID =@FranchiseID
where BI.IsActive = 1 and DBT.IsActive=1
/*end*/
/*to get block items list*/
Select BI.ID,BI.DesignBlockTypeID,BI.SequenceOrder,BI.ImageName,BI.LinkUrl,BI.Tooltip,BI.IsActive,BI.DisplayViewApp,BI.CategoryID from BlockItemsList BI
inner join DesignBlockType DBT on DBT.ID = BI.DesignBlockTypeID and DBT.IsActive=1 and DBT.Name!='Product Gallery'
where BI.FranchiseID=@FranchiseID and cast(GETDATE()as date) > =cast(BI.StartDate as date) and
cast(GETDATE()as date) < =cast(BI.EndDate as date) and BI.IsActive=1
group by BI.DesignBlockTypeID, BI.ID,BI.SequenceOrder,BI.ImageName,BI.LinkUrl,BI.Tooltip,BI.IsActive,BI.DisplayViewApp,BI.CategoryID order by BI.SequenceOrder
/*end*/
 /* to get products list in block items list*/
 --Yashaswi 6-7-2018 New Column RetailPoint
Declare @Temp_SS as table (RowNumber int,PID bigint,ShopProductID bigint,SSID bigint,RetailerRate decimal,MRP decimal,ColorName varchar(50),RetailPoint decimal)
;WITH tblTemp as
(
	SELECT ROW_NUMBER() Over(PARTITION BY P.ID ORDER BY SS.RetailerRate)
	As RowNumber,P.ID AS PID,SS.ShopProductID, SS.ID As SSID, SS.RetailerRate, SS.MRP,C.Name as ColorName,SS.BusinessPoints as BP FROM ShopStock SS
	inner join ShopProduct SP on SS.ShopProductID =SP.ID
	inner join Product P on SP.ProductID=P.ID
	inner join Shop S on SP.ShopID=S.ID and S.IsLive=1 and S.IsActive=1 and S.FranchiseID=@FranchiseID
	inner join ProductVarient PV on SS.ProductVarientID=PV.ID and PV.IsActive=1
	inner join Color C on PV.ColorID=C.ID
	where SS.Qty>0 and SS.IsActive=1 and SP.IsActive=1 and SS.StockStatus=1
)
insert into @Temp_SS
select * from tblTemp
where  RowNumber =1
select BI.ID,BI.DesignBlockTypeID,BI.SequenceOrder,BI.ImageName,BI.LinkUrl,BI.Tooltip,BI.IsActive,
       BI.ProductID,P.Name,SS.SSID as ShopStockID,SS.RetailerRate,SS.MRP,SS.ColorName,ISNULL( SS.RetailPoint,0) RetailPoint from BlockItemsList BI
inner join DesignBlockType DBT on BI.DesignBlockTypeID=DBT.ID and DBT.IsActive=1 and DBT.Name='Product Gallery'
inner join Product P on BI.ProductID = P.ID and P.IsActive =1
inner join FranchiseMenu FM on P.CategoryID = FM.CategoryID and FM.IsActive =1 
inner join @Temp_SS SS on P.ID = SS.PID 
inner join Category C on C.ID = P.CategoryID  and C.IsActive =1 
where BI.IsActive=1 and cast(GETDATE()as date) > =cast(BI.StartDate as date) and
cast(GETDATE()as date) < =cast(BI.EndDate as date) and BI.FranchiseID=@FranchiseID
group by BI.ProductID,SS.SSID,P.Name,BI.SequenceOrder,SS.RetailerRate,SS.MRP,SS.ColorName,
			BI.ID,BI.DesignBlockTypeID,BI.SequenceOrder,BI.ImageName,BI.LinkUrl,BI.Tooltip,BI.IsActive,SS.RetailPoint,BI.DisplayViewApp
/* product listing ends here */
end