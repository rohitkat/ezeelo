alter table MLMWalletDetails
add InactivePoint decimal(18,2)

alter table MLMWalletDetails
add InactiveAmount decimal(18,2)

alter table EzeeMoneyPayout
add InactiveFreezeDate datetime 

alter table EzeeMoneyPayout
add InactivePaidDate datetime 

alter table EzeeMoneyPayout
add IsInactivePaid bit 

alter table EzeeMoneyPayoutDetails
add InactiveEzeeMoney decimal(18,2)

alter table EzeeMoneyPayoutDetails
add IsInactivePaid bit not null default 0

update EzeeMoneyPayout set IsInactivePaid = 1 where ID between 1 and 8

update EzeeMoneyPayoutDetails set IsInactivePaid = 1 where EzeeMoneyPayoutID between 1 and 8