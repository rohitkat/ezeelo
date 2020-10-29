using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Advertiser
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ContactPersone { get; set; }
        public long BusinessDetailID { get; set; }
        public int PincodeID { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> IsLive { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessDetail BusinessDetail { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Pincode Pincode { get; set; }
    }
}
