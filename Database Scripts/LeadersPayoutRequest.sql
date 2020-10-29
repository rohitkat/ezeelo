--Alter table LeadersPayoutRequest
--Add IsActive bit null

Alter table LeadersPayoutRequest
Add TransactionID nvarchar(max) null


--Alter table LeadersSubstractDaysForPayoutMonth
--Add NextPayoutDate datetime null

--Alter table LeadersSubstractDaysForPayoutMonth
--Add LastPayoutDate datetime null

--Alter table LeadersPayoutRequest
--Add PaymentStatus bit null

--Alter table LeadersPayoutRequest
--Add Remark nvarchar(max) null




