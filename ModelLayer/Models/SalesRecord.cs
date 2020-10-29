using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SalesRecord
    {
        public int ID { get; set; }
        public string FormSerialNumber { get; set; }
        public string SystemSerialNumber { get; set; }
        public System.DateTime FormSignDate { get; set; }
        public long EmployeeID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
