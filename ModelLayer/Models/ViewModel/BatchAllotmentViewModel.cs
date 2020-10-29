using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class BatchAllotmentViewModel
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

        public List<WarehouseStockViewModel> lWarehouseStockViewModels = new List<WarehouseStockViewModel>();
    }
}
