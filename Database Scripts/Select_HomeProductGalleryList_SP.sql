CREATE procedure [dbo].[Select_HomeProductGalleryList]
(
@FranchiseID as int,
@StartDate as datetime2,
@EndDate as datetime2
)
as 

begin 

select BI.ProductID,P.Name from BlockItemsList BI
inner join Product P on BI.ProductID = P.ID 
inner join DesignBlockType DB on BI.DesignBlockTypeID = DB.ID and DB.Name='Product Gallery'
where BI.FranchiseID=@FranchiseID and 
cast(BI.StartDate as date)>=cast(@StartDate as date)
and cast(BI.EndDate as date)<=cast(@EndDate as date)

end

GO
/****** Object:  StoredProcedure [dbo].[Select_HomeProductGallerysetsequence]    Script Date: 2/25/2019 5:47:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO