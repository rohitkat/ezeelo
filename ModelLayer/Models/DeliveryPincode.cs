using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class DeliveryPincode
    {
        public long ID { get; set; }
        public int DeliveryPartnerID { get; set; }
        public int PincodeID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual DeliveryPartner DeliveryPartner { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
