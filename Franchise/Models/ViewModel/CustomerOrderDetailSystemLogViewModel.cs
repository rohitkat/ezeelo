using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class CustomerOrderDetailSystemLogViewModel
    {
        public long ID { get; set; }
        public string ReferenceShopOrderCode { get; set; }
        public long ShopStockID { get; set; }
        public string ProductName { get; set; }
        public int OrderStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public int Qty { get; set; }
        public decimal TotalAmount { get; set; }
        public long CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ChangeByPersonName { get; set; }
    }
}