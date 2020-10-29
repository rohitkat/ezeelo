using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantAppOrderListViewModel
    {
        public long ID { get; set; }
        public string CustomerName { get; set; }
        public string ShopOrderCode { get; set; }
        public System.DateTime CreateDate { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable<System.DateTime> ActualTimeFrom { get; set; }
        public Nullable<System.DateTime> ActualTimeTo { get; set; }
        //public System.TimeSpan ActualTimeFrom { get; set; }
        //public System.TimeSpan ActualTimeTo { get; set; }
        public int TotalItem { get; set; }
    }
}
