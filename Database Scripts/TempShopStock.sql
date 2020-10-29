
--Added OfferType by Sonali on 08-02-2019
alter table [dbo].[TempShopStock]
add OfferType int not null default 2
select * from [TempShopStock]