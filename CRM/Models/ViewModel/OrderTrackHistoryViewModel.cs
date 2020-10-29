using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class OrderTrackHistoryViewModel
    {
        public long CustomerOrderID { get; set; }
        public string ShopOrderCode { get; set; }
        public int OrderStatus { get; set; }
        //public long ShopID { get; set; }
        //public string ShopName { get; set; }
        //public long DeliveryPartnerID { get; set; }
        //public string DeliveryPartnerName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifyBy { get; set; }
        public string Duration { get; set; }
    }
}