using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ShopOrderStatistics
    {
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public int OrderCount { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int CancelledOrdeCount { get; set; }
        public int DeliveredOrder { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeliveredAmount { get; set; }
        public decimal CancelledAmount { get; set; }
    }
}
