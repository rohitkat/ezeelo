using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class CorporateOrderShippingFacilityDetail
    {
        public long ID { get; set; }


        public long CustomerOrderDetailID { get; set; }
        public decimal ShippingFacilityCharges { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public long CorporateshippingFacilityID { get; set; }
        public virtual CorporateShippingFacility CorporateShippingFacility { get; set; }
        public virtual CustomerOrderDetail CustomerOrderDetail { get; set; }
    }
}
