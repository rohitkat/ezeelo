using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class ProductListViewModel
    {
        public bool IsSelected { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public long ShopStockID { get; set; }
        public int Qty { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferRs { get; set; }
        public decimal TotalAmount { get; set; }
    }
}