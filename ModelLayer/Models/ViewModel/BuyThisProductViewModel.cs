using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class BuyThisProductViewModel
    {
        public string StockThumbPath { get; set; }
        public string ProductName { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public long ShopStockID { get; set; }
        public long ProductID { get; set; }

        public int StockQty { get; set; }
        public bool StockStatus { get; set; }

    }
}
