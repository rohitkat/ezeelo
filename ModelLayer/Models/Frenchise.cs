using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Frenchise
    {
        public int ID { get; set; }
        public long BusinessDetailID { get; set; }
        public string ServiceNumber { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string LandlineNumber { get; set; }
        public string FAX { get; set; }
        public string Address { get; set; }
        public Nullable<long> PincodeID { get; set; }
        public bool IsLive { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
