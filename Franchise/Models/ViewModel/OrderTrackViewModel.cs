using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace Franchise.Models.ViewModel
{
    public class OrderTrackViewModel
    {
        public long CustomerOrderID { get; set; }
        public string ShopOrderCode { get; set; }
        public int OrderStatus { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public long DeliveryPartnerID { get; set; }
        public string DeliveryPartnerName { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}