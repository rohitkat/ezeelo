using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class WarehouseStockViewModel
    {
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public long InvoiceID { get; set; }
        public string BatchCode { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string ItemName { get; set; }
        public string StockThumbPath { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public decimal BuyRatePerUnit { get; set; }
        public Nullable<decimal> SaleRatePerUnit { get; set; }
        public int InitialQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public Nullable<bool> StockStatus { get; set; }
        public int SizeID { get; set; }
        public Nullable<int> PackUnitID { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public long SupplierID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string HSNCode { get; set; }
        public int InStockQty { get; set; }
        public int InShopQty { get; set; }
        public int PlacedQty { get; set; }
        public int PendingQty { get; set; }//Added by Rumana on 14-05-2019
        public long ShopStockID { get; set; }
        public Nullable<long> WarehouseStockID { get; set; }
        public bool isAllottedToShop { get; set; }
        public int ItemsAllotedToLocation { get; set; }
        public List<AutoSearchWarehouseLocation> lAutoSearchWarehouseLocations { get; set; }

        public string PurchaseFor { get; set; }
        public  List<InvoiceViewModel> lInvoiceViewModelList = new List<InvoiceViewModel>();
        public  List<WarehouseReorderLevel> lWarehouseReorderLevelList = new List<WarehouseReorderLevel>();
        public List<WarehouseReorderLevelViewModel> lWarehouseReorderLevelViewModel = new List<WarehouseReorderLevelViewModel>();
    }         
}
