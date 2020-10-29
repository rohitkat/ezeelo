using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class ProductByShopStockViewModel
    {
        public long ShopStockID { get; set; }
        public long ShopProductID { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
    }
}