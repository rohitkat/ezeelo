--USE [Ezeelo]
GO
/****** Object:  StoredProcedure [dbo].[InactivePointsPayout]    Script Date: 05-Jun-19 11:59:21 AM ******/
/****** Yashaswi - For Inactive Point payout*****/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[InactivePointsPayout]
@EzeeMoneyPayoutId bigint,@flag int
as
begin	
	
	create table #Report
	(
		Id bigint,
		UserLoginId bigint,	
		Name nvarchar(500),
		EmailId nvarchar(500),
		phoneno nvarchar(500),
		InactivePoint decimal(18,2),
		EzeeMoney decimal(18,2)
	)
	 --Flag:0  - Display REport
	 --Flag:1  - Payout

	insert into #Report
	select Id,UserLoginId,Name,EmailId,phoneno,erp As InactivePoint,ezeemoney as EzeeMoney from EzeeMoneyPayoutDetails where UserLoginId 
	in(select UserLoginId from EzeeMoneyPayoutDetails where EzeeMoneyPayoutID = @EzeeMoneyPayoutId and Status =1)
	and Ref_EzeeMoneyPayoutID is null and Status = 0 and IsInactivePaid != 1
	and EzeeMoneyPayoutID in (select top 3 id from EzeeMoneyPayout order by id desc)
	and ERP !=0
	
	BEGIN TRANSACTION;
	BEGIN TRY

	if @flag = 1
	begin
		update EzeeMoneyPayout set totalinactivepoints = (select sum(InactivePoint) from #Report), inactivefreezedate = getdate(), inactivePaidDate= getdate(),IsInactivePaid=1 where id = @EzeeMoneyPayoutId
		
		update EzeeMoneyPayoutdetails set inactivepoints=erp, inactiveezeemoney=ezeemoney,Ref_EzeeMoneyPayoutID = @EzeeMoneyPayoutId,IsInactivePaid =1 
		where Id in ( select ID from #Report)

		update MLMWalletDetails set InactivePoint = temp.InactivePoint,InactiveAmount = temp.EzeeMoney from  MLMWalletDetails 
		inner join
		(select UserLoginId,Name,EmailId,phoneno,sum(InactivePoint) As InactivePoint,sum(ezeemoney) as EzeeMoney
		from #Report 
		group by UserLoginId,Name,EmailId,phoneno) as temp
		on temp.userloginid=  MLMWalletDetails.userloginid and  MLMWalletDetails.EzeeMoneyPayoutId=11
		where  MLMWalletDetails.EzeeMoneyPayoutId=11

		update MLMWallet set isactive=1, LastModifyDate = getdate(), amount=amount+temp.EzeeMoney,points=points+temp.InactivePoint
		from mlmwallet
		inner join 
		(select UserLoginId,Name,EmailId,phoneno,sum(InactivePoint) As InactivePoint,sum(ezeemoney) as EzeeMoney
		from #Report 
		group by UserLoginId,Name,EmailId,phoneno) as temp
		on temp.userloginid=mlmwallet.userloginid
		
	end 

	select UserLoginId,Name,EmailId,phoneno,sum(InactivePoint) As InactivePoint,sum(ezeemoney) as EzeeMoney
	from #Report 
	group by UserLoginId,Name,EmailId,phoneno
	
	 COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH

    ROLLBACK TRANSACTION;
END CATCH;

end