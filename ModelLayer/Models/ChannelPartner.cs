using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ChannelPartner
    {
        public int ID { get; set; }
        public long BusinessDetailID { get; set; }
        public string ContactPerson { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Landline { get; set; }
        public string FAX { get; set; }
        public string Address { get; set; }
        public Nullable<int> PincodeID { get; set; }
        public bool IsActive { get; set; }
        public bool IsCODAllowed { get; set; }
        public Nullable<decimal> MinimumCODRange { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public int FranchiseID { get; set; }
        
        public virtual BusinessDetail BusinessDetail { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
    }
}
