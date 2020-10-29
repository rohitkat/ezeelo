using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SubscriptionEmployeeKey
    {
        public long ID { get; set; }
        public long EmployeeID { get; set; }
        public string EmployeeKey { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
    }
}
