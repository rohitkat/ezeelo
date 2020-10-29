--Added by Sonali 08-02-2019 ,Added DisplayView app filed for APP redirection of Banner.
alter table [dbo].[BlockItemsList]
add DisplayViewApp nvarchar(50)


update  BlockItemslist
set DisplayViewApp = 'productlist'