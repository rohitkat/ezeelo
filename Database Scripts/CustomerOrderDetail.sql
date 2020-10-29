--Added by yashaswi 05-02-2019 ,To track current quantity in shop and warehouse at the time of order
ALTER TABLE CustomerOrderDetail
  ADD  CurrentShopStockQty int  NULL,
	   CurrentWarehouseStockQty int  NULL