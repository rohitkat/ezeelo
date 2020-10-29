USE [Ezeelo]
GO
/****** Object:  StoredProcedure [dbo].[SP_HomePageDynamicSections]    Script Date: 3/14/2019 1:21:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Created by Yashaswi 
--exec SP_HomePageDynamicSections 1052
Create proc [dbo].[SP_HomePageDynamicSections]
(
@FranchiseID as int 
)
as begin

Declare @CityId bigint
Declare @ImagePath nvarchar(500)

Declare @Temp_Section as table (
SectionId int,
HomePageDynamicSectionsId bigint,
SectionDisplayName nvarchar(Max),
IsBanner bit,
IsCategory bit,
IsProduct bit,
SequenceOrder int,
SectionStyle nvarchar(500)
)

Declare @Temp_SectionBanner as table (
HomePageDynamicSectionsId bigint,
SectionId int,
ImageName nvarchar(Max),
SequenceOrder int,
ToolTip nvarchar(Max),
LinkURL nvarchar(Max),
IsBanner bit,
CreateDate datetime)

Declare @Temp_SectionProduct as table (
HomePageDynamicSectionsId bigint,
SectionId int,
ShopStockId bigint,
ProductID bigint,
SequenceOrder int,
Name nvarchar(500),
RetailPoint decimal(18,2),
MRP decimal(18,2),
ColorName nvarchar(100),
RetailerRate decimal(18,2))

insert into @Temp_Section
select HPDS.SectionId,HPDS.id,HPDS.SectionDisplayName,HPDS.IsBanner,HPDS.IsCategory,HPDS.IsProduct,HPDS.SequenceOrder,HPDS.SectionStyle
from HomePageDynamicSection HPDS
where HPDS.IsActive = 1 and HPDS.ShowInApp = 0 and HPDS.FranchiseId = @FranchiseID


select @CityId = CityID from Pincode where ID in( select PincodeID from Franchise where id= @FranchiseID )


Declare @HomePageDynamicSectionsId bigint
Declare @SectionId int
Declare @IsBanner bit
Declare @IsCategory bit
Declare @IsProduct bit

Declare MY_data CURSOR FOR
Select HomePageDynamicSectionsId,IsBanner,IsCategory,IsProduct,SectionId from  @Temp_Section 
OPEN MY_data
FETCH NEXT FROM MY_data INTO @HomePageDynamicSectionsId ,@IsBanner,@IsCategory,@IsProduct,@SectionId
WHILE @@FETCH_STATUS = 0
     BEGIN

	 --Create Folder path for banner and category Image
	set @ImagePath =''
	set @ImagePath = '/' + CAST(@CityId as nvarchar(15)) + '/' + CAST( @FranchiseID as nvarchar(15)) + '/' + CAST( @SectionId as nvarchar(15)) + '/'

	---For Banner	
	insert into @Temp_SectionBanner	
	select @HomePageDynamicSectionsId,@SectionId,@ImagePath+ImageName,SequenceOrder,ToolTip,LinkURL,IsBanner
	,ISNULL(HPDSB.ModifyDate,HPDSB.CreateDate) as CreateDate
     from HomePageDynamicSectionBanner HPDSB
	where HPDSB.HomePageDynamicSectionId  = @HomePageDynamicSectionsId and HPDSB.IsActive = 1
	and cast(GETDATE()as date) > =cast(HPDSB.StartDate as date) and cast(GETDATE()as date) < =cast(HPDSB.EndDate as date) 
	and HPDSB.IsBanner = 1

	---For Category
	insert into @Temp_SectionBanner
	select @HomePageDynamicSectionsId,@SectionId,@ImagePath+ImageName,SequenceOrder,ToolTip,LinkURL,IsBanner
	,ISNULL(HPDSB.ModifyDate,HPDSB.CreateDate) as CreateDate
	 from HomePageDynamicSectionBanner HPDSB
	where HPDSB.HomePageDynamicSectionId  = @HomePageDynamicSectionsId and HPDSB.IsActive = 1
	and cast(GETDATE()as date) > =cast(HPDSB.StartDate as date) and cast(GETDATE()as date) < =cast(HPDSB.EndDate as date) 
	and HPDSB.IsBanner = 0

	---For Product
	insert into @Temp_SectionProduct
	select @HomePageDynamicSectionsId,@SectionId,ShopStockId,ProductID,SequenceOrder,p.Name as Name,ISNULL( SS.BusinessPoints,0) RetailPoint
	,SS.MRP,'n/a',SS.RetailerRate
	 from HomePageDynamicSectionProduct HPDSP
	left join ShopStock SS on SS.ID = HPDSP.ShopStockId
	left join ShopProduct SP on SP.ID = SS.ShopProductID
	left join Product P on P.ID = SP.ProductID
	where HPDSP.HomePageDynamicSectionId = @HomePageDynamicSectionsId and HPDSP.IsActive = 1
	and cast(GETDATE()as date) > =cast(HPDSP.StartDate as date) and cast(GETDATE()as date) < =cast(HPDSP.EndDate as date) 
	and SS.Qty > 0 and SS.IsActive = 1 and SP.IsActive = 1 and P.IsActive = 1
	 
		
	 FETCH NEXT FROM MY_data INTO @HomePageDynamicSectionsId ,@IsBanner,@IsCategory,@IsProduct,@SectionId
	 END

    CLOSE MY_data
DEALLOCATE MY_data

select * from @Temp_Section order by SequenceOrder 

select * from @Temp_SectionBanner

select * from @Temp_SectionProduct

end
