using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ReorderLevelReportViewModelOnPlaced
    {
        public DateTime? ReorderLevelHitDate { get; set; }
        public int SoldQty { get; set; }
        public string Cat1 { get; set; }
        public string Cat2 { get; set; }
        public string Cat3 { get; set; }
        public int InitialQty { get; set; }
        public long ID { get; set; }
        public long WarehouseID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string ItemName { get; set; }
        public string StockThumbPath { get; set; }
        public int AvailableQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public string HSNCode { get; set; }
        public int InStockQty { get; set; }
        public int InShopQty { get; set; }
        public int PlacedQty { get; set; }
        public long ShopStockID { get; set; }
        public Nullable<long> WarehouseStockID { get; set; }
        public bool isAllottedToShop { get; set; }
        public string SKUUnit { get; set; }
        public long SKUID { get; set; }
        public string BrandName { get; set; }
    }
    public class ReorderLevelReportViewModelOnPlaced_Detail
    {
        public string Cat1 { get; set; }
        public string Cat2 { get; set; }
        public string Cat3 { get; set; }
        public long SKUID { get; set; }
        public long PurchaseOrderDetailsID { get; set; }
        public long SupplierID { get; set; }
        public long PurchaseOrderID { get; set; }
        public long ProductID { get; set; }
        public long ProductVarientID { get; set; }
        public string SKUUnit { get; set; }
        public string SKUName { get; set; }
        public string HSNCode { get; set; }
        public int Quantity { get; set; }
        public String BrandName { get; set; }


        public int ReorderLevel { get; set; }
        public decimal Amount { get; set; }
        public decimal OrderAmount { get; set; }
        public Nullable<int> GSTInPer { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string Remark { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public bool IsItemExistsInShop { get; set; }
        public long ShopStockID { get; set; }
        public int ItemsAllotedToLocation { get; set; }
        public decimal TaxableValue { get; set; }///Added by Priti  
        public List<AutoSearchWarehouseLocation> lAutoSearchWarehouseLocations { get; set; }

    }
    //31-7-2018 For Export
    public class ProductReorderLevelExport1
    {
        public string ProductName { get; set; }
        public long AvailableQty { get; set; }
        public long ReorderQyt { get; set; }

    }
}
