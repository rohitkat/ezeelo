/* select product list in gallery in admin */
-- written by : Roshan Gomase
-- Date :2/15/2019
-- Procedure for select gallery product in home page in admin module in web

CREATE procedure [dbo].[Select_HomeProductGallerysetsequence]
(
@FranchiseID as int,
@HomePageDynamicSectionId as int
)
as 

begin 
select  BI.ID,DB.FranchiseID,BI.EndDate as EndDate,BI.StartDate as StartDate,BI.Imagename ,BI.Tooltip,BI.SequenceOrder,BI.IsActive,BI.Linkurl 
from HomePageDynamicSectionBanner BI

inner join HomePageDynamicSection DB on  DB.ID=BI.HomePageDynamicSectionID
where DB.franchiseid=@FranchiseID and BI.homepagedynamicsectionid=@HomePageDynamicSectionId order by BI.SequenceOrder
end
