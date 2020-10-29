using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class StockManagementReportViewModel
    {
        public long SKUID { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long VariantId { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string DimensionName { get; set; }
        public string MaterialName { get; set; }
        public long ShopStockId { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public decimal SaleRate { get; set; }
        public decimal? NewSaleRate { get; set; }
        public Nullable<decimal> WholeSaleRate { get; set; }
        public Nullable<decimal> NewWholeSaleRate { get; set; }
        public decimal Mrp { get; set; }
        public decimal? NewsMrp { get; set; }
        public string StockStatus { get; set; }
        public DateTime ProductUploadDate { get; set; }
        public DateTime? ProductModifiedDate { get; set; }
    }
}
