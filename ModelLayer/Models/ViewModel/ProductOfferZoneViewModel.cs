using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ProductOfferZoneViewModel
    {
        public long ShopID { get; set; }
        public long ShopStockID { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long ProductVarientID { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public string DimensionName { get; set; }
        public string MaterialName { get; set; }
        public decimal Total { get; set; }
        public decimal MRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public string ShopImage { get; set; }
        public decimal PackSize { get; set; }
        public bool IsActive { get; set; }
        public decimal RetailerRate { get; set; }
        public int MinPurchaseQty { get; set; }
        public int FreeOty { get; set; }
        public int Qty { get; set; }
    }
}
