--USE [EzeeloLive]
GO
/****** Object:  StoredProcedure [dbo].[Leaders_Report6Select]    Script Date: 01/16/2019 4:11:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
-- =============================================
-- Author: Amit PAntawane
-- Create date: 1/08/2018
-- Modified date:
-- Description:    
-- =============================================
ALTER PROCEDURE [dbo].[Leaders_Report6Select]
AS
BEGIN
 
 
SELECT  Userlogin.ID ,  CustomerOrder.OrderCode , PersonalDetail.FirstName AS Customer, UserLogin.Mobile,
--CustomerOrder.ShippingAddress,
 
Pincode.Name as Pincode , City.name AS City, CustomerOrder.CreateDate 'OrderDate' , CustomerOrder.ModifyDate 'DeliveryDate' , 'DeliveryTime' , CustomerOrder.PayableAmount , CustomerOrder.PaymentMode ,
 
CustomerOrder.DeviceType , MLMWallet.IsMLMUser , MLMWallet.IsActive ,
 
CASE CustomerOrderDetail.OrderStatus
 
        when 1 then 'PLACED'
 
              when 2 then 'CONFIRM'
 
              when 3 then 'PACKED'
 
              when 4 then 'DISPATCH FROM SHOP'
 
              when 5 then 'IN GODOWN'
 
              when 6 then 'DISPATCH FROM GODOWN'
 
              when 7 then 'DELIVERED'
 
              when 8 then 'RETURNED'
 
              when 9 then 'CANCELLED'
 
              end AS [Status] ,
 
              (Select a.Ref_Id  from Mlm_User a where a.UserID  = MLMWalletTransaction.UserLoginID ) 'ReferralID' ,
 
              (Select a.Refered_Id_ref   from Mlm_User a where a.UserID  = MLMWalletTransaction.UserLoginID ) 'ReferBy' ,
 
              ( Select b.Email  from UserLogin b where b.ID in ( Select a.UserID  from Mlm_User a where a.Ref_Id in (Select a.Refered_Id_ref   from Mlm_User a where a.UserID  =MLMWalletTransaction.UserLoginID ))) 'Parent' ,
 
               --'0-Level' as Level , 
               CustomerOrder.BusinessPointsTotal 'RPEarned_Order',
               (select p.firstName + ISNULL(p.lastname,'') from PersonalDetail p where UserLoginID = MLMWallet_DirectIncome.CurrentLevel_UserLoginId) 'Current_Level_User',
               --(Select PersonalDetail.FirstName  where PersonalDetail.UserLoginID  = MLMWallet_DirectIncome.CurrentLevel_UserLoginId) 'Current_Level_User',
                (case 
                     when MLMWallet_DirectIncome.CurrentLevel_IsPaid = 1
                        then MLMWallet_DirectIncome.CurrentLevel
                        else 0
                ENd)
               'RP_Distribution_Level0',
               
               --MLMWallet_DirectIncome.CurrentLevel_UserLoginId 'Current Level UserID',
               (case 
                     when MLMWallet_DirectIncome.UpLine1_IsPaid = 1
                        then MLMWallet_DirectIncome.UpLine1
                        else 0
                ENd)  'RP_Distribution_Level1',
               
               (select p.firstName + ISNULL(p.lastname,'') from PersonalDetail p where UserLoginID = MLMWallet_DirectIncome.UpLine1_UserLoginId) 'Level1_User',
              
               --MLMWallet_DirectIncome.UpLine2 'RP_Distribution_Level2',
               --(MLMWallet_DirectIncome.UpLine2_UserLoginId ) 'Level2_User',
               --MLMWallet_DirectIncome.UpLine2 'RP_Distribution_Level2',
               --MLMWallet_DirectIncome.UpLine2_UserLoginId 'Level2 UserID',
			    (case 
                     when MLMWallet_DirectIncome.UpLine2_IsPaid = 1
                        then MLMWallet_DirectIncome.UpLine2
                        else 0
                ENd)  'RP_Distribution_Level2',
               (select p.firstName + ISNULL(p.lastname,'') from PersonalDetail p where UserLoginID = MLMWallet_DirectIncome.UpLine2_UserLoginId) 'Level2_User',
               --MLMWallet_DirectIncome.UpLine3 'RP_Distribution_Level3',
               
			    (case 
                     when MLMWallet_DirectIncome.UpLine3_IsPaid = 1
                        then MLMWallet_DirectIncome.UpLine3
                        else 0
                ENd)  'RP_Distribution_Level3',
			   (select p.firstName + ISNULL(p.lastname,'') from PersonalDetail p where UserLoginID = MLMWallet_DirectIncome.UpLine3_UserLoginId) 'Level3_User',
              -- MLMWallet_DirectIncome.UpLine4 'RP_Distribution_Level4',
              
			  (case 
                     when MLMWallet_DirectIncome.UpLine4_IsPaid = 1
                        then MLMWallet_DirectIncome.UpLine4
                        else 0
                ENd)  'RP_Distribution_Level4',
			  (select p.firstName + ISNULL(p.lastname,'') from PersonalDetail p where UserLoginID = MLMWallet_DirectIncome.UpLine4_UserLoginId) 'Level4_User',
 
              CustomerOrder.BusinessPointsTotal*0.1 'EzeeMoney'
 
FROM MLMWalletTransaction INNER JOIN 
 MLMWallet_DirectIncome on MLMWalletTransaction.ID=MLMWallet_DirectIncome.MLMWalletTransactionId  inner Join
 CustomerOrder on MLMWalletTransaction.CustomerOrderID=CustomerOrder.ID and MLMWalletTransaction.TransactionTypeID=7  inner Join
 
                     Pincode ON CustomerOrder.PincodeID = Pincode.ID INNER JOIN
 
                     City ON Pincode.CityID = City.ID INNER JOIN
 
                     UserLogin ON MLMWalletTransaction.UserLoginID = UserLogin.ID INNER JOIN
 
                     PersonalDetail ON PersonalDetail.UserLoginID = MLMWalletTransaction.UserLoginID INNER JOIN
 
                     Mlm_User ON MLMWalletTransaction.UserLoginID = Mlm_User.UserID INNER JOIN
 
                     MLMWallet on MLMWalletTransaction.UserLoginID = MLMWallet.Userloginid inner JOIN
                     --MLMWalletTransaction on UserLogin.ID=MLMWalletTransaction.UserLoginID and MLMWalletTransaction.TransactionTypeID=7  inner JOIN
                     --MLMWallet_DirectIncome on MLMWalletTransaction.ID=MLMWallet_DirectIncome.MLMWalletTransactionId   inner Join
 
                     CustomerOrderDetail ON CustomerOrderDetail.CustomerOrderID = CustomerOrder.ID
--					 where(

--					  MLMWalletTransaction.TransactionTypeID = 1 and MLMWalletTransaction.TransactionTypeID = 7  )
--and
-- (( MLMWalletTransaction.OrderAmount > 0 or MLMWalletTransaction.TransactionTypeID = 7)
--or (MLMWalletTransaction.OrderAmount > 0))
--group by CustomerOrder.UserLoginID 

 
                     where MLMWalletTransaction.OrderAmount >=0 and CustomerOrderDetail.OrderStatus=07 and MLMWalletTransaction.TransactionTypeID = 7
                     --where month(CustomerOrder.CreateDate) = 07 and year(CustomerOrder.CreateDate) = 2018 
 
                     group by Userlogin.ID ,  CustomerOrder.OrderCode , UserLogin.Mobile,
 
                 City.name , CustomerOrder.CreateDate,  CustomerOrder.ModifyDate , CustomerOrder.PayableAmount , CustomerOrder.PaymentMode ,
 
            CustomerOrder.DeviceType , MLMWallet.IsMLMUser , MLMWallet.IsActive , CustomerOrderDetail.OrderStatus , UserLogin.Email ,  CustomerOrder.BusinessPointsTotal, MLMWallet_DirectIncome.CurrentLevel,MLMWallet_DirectIncome.CurrentLevel,
            MLMWallet_DirectIncome.UpLine1, MLMWallet_DirectIncome.UpLine1_UserLoginId, MLMWallet_DirectIncome.UpLine2,MLMWallet_DirectIncome.UpLine2_UserLoginId,MLMWallet_DirectIncome.UpLine3, MLMWallet_DirectIncome.UpLine3_UserLoginId,
            MLMWallet_DirectIncome.UpLine4, MLMWallet_DirectIncome.UpLine4_UserLoginId, MLMWallet_DirectIncome.CurrentLevel_UserLoginId,MLMWalletTransaction.UserLoginID ,PersonalDetail.FirstName,PersonalDetail.UserLoginID
            ,CurrentLevel_IsPaid,UpLine1_IsPaid, MLMWallet_DirectIncome.UpLine2_IsPaid,MLMWallet_DirectIncome.UpLine3_IsPaid,MLMWallet_DirectIncome.UpLine4_IsPaid, Pincode.Name
             order by CustomerOrder.OrderCode asc
             end
 

             
--exec Leaders_Report6Select
 

--exec Leaders_Report6Select