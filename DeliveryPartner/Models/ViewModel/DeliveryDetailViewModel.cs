using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryPartner.Models.ViewModel
{
    public class DeliveryDetailViewModel
    {
        public long ID { get; set; }
        public string ShopOrderCode { get; set; }
        public string ShopName { get; set; }
        public string PickUpName { get; set; }
        public string PickUpAddress { get; set; }
        public string PickUpContact { get; set; }
        public string PickUpAlternateContact { get; set; }
        public string DeliverToName { get; set; }
        public string DeliverToEmail { get; set; }
        public string DeliverToAddress { get; set; }
        public string DeliveryToContact { get; set; }
        public string DeliveryToAlternateContact { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal GandhibaghCharge { get; set; }
        public string GandhibaghOrderCode { get; set; }
        public List<TrackOrderViewModel> TrackOrderViewModels { get; set; }

        //------------------- new fields added on 08-sep-2015.
        //-- Changes made by AVI VERMA.
        //-- For Getting Mode of Payment. 
        //-- Reason : - If payment mode is online. then, for display on delivery memo.. COD = 0 Rs. As it is paid online.
        public string PaymentMode { get; set; }

    }
}