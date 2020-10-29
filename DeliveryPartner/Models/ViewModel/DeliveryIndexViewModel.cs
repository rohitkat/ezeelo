using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryPartner.Models.ViewModel
{
    public class DeliveryIndexViewModel
    {
        public long ID { get; set; }
        public int DeliveryPartnerID { get; set; }
        public long GandhibaghOrderID { get; set; }//Added by Mohit 19-10-15
        public string GandhibaghOrderCode { get; set; }
        public string ShopOrderCode { get; set; }
        public decimal Weight { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal GandhibaghCharge { get; set; }
        public string DeliveryType { get; set; }
        public bool IsMyPincode { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }


        //----------------------------- Extra added -//
        public int OrderStatus { get; set; }

        public Nullable<DateTime> DeliveryDate { get; set; }

        //public string DeliveryDate { get; set; }

        public string DeliveryScheduleName { get; set; }

        //-- Add By Ashish --//
        public string Assignment { get; set; }
        public string EmployeeCode { get; set; }
        public string DeliveredType { get; set; }
        public string PaymentMode { get; set; }
        //-- End Add --//

    }
}