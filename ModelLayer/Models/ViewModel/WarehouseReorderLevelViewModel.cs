using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
  public class WarehouseReorderLevelViewModel
    {  
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
        public int PendingQty { get; set; } //Added by Rumana on 14-05-2019
    }


    //31-7-2018 For Export
    public class ProductReorderExport
    {
        public string ProductName { get; set; }
        public long AvailableQty { get; set; }
        public long ReorderQyt { get; set; }
       
    }
    //31-7-2018 For Export
    public class ProductReorderExport_ForBatchAllotmentReport
    {
        public int SrNo { get; set; }
        public long SKUID { get; set; }
        public string Item { get; set; }
        public string HSNCode { get; set; }
        public int AvailableQty { get; set; }
        public int InShopQty { get; set; }
        public int PendingQty { get; set; }
        public int PlacedQty { get; set; }
        public int ReOrderLevel { get; set; }
        public string IsLinkedWithShop { get; set; }
      

    }
}
