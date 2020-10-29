using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class TotalDiscountViewModel
    {
        public long CustomerOrderID { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public string CustomerOrderDetailShopOrderCode { get; set; }
        public long CustomerOrderDetailLogID { get; set; }
        public string CustomerOrderDetailLogShopOrderCode { get; set; }
        public decimal SaleRate { get; set; }
        public decimal SaleRateLog { get; set; }
        public int CurrentQty { get; set; }
        public decimal Discount { get; set; }
    }
}