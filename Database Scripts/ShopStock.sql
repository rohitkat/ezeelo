--Added OfferType by sonali 08-02-2019
alter table [dbo].[ShopStock]
add OfferType int not null default 2
select * from [ShopStock]
