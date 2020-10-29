using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class CorporateCustomerShippingDeliveryDetail
    {
        public int ID { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public long FromUserLoginID { get; set; }
        public string ToName { get; set; }
        public decimal DeliveryCharges { get; set; }
        public System.DateTime ExpectedDeliveryDate { get; set; }
        public int Quantity { get; set; }
        public string PrimaryMobile { get; set; }
        public string SecondaryMobile { get; set; }
        public string ShippingAddress { get; set; }
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrderDetail CustomerOrderDetail { get; set; }
    }
}
