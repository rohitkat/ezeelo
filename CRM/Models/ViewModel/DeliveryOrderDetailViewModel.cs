using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class DeliveryOrderDetailViewModel
    {
        public long ID { get; set; }
        public string ShopOrderCode { get; set; }
        public int DeliveryPartnerID { get; set; }
        public string DeliveryPartnerName { get; set; }
        public ModelLayer.Models.DeliveryPartner DeliveryPartnerVM { get; set; }

    }
}