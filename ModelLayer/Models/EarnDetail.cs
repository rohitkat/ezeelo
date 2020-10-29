using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class EarnDetail
    {
        public long ID { get; set; }
        public Nullable<long> ReferAndEarnSchemaID { get; set; }
        public long EarnUID { get; set; }
        public Nullable<long> ReferUID { get; set; }
        public Nullable<decimal> EarnAmount { get; set; }
        public Nullable<decimal> UsedAmount { get; set; }
        public Nullable<decimal> RemainingAmount { get; set; }
        public Nullable<long> CustomerOrderID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual ReferAndEarnSchema ReferAndEarnSchema { get; set; }

        public bool IsActive { get; set; }
    }
}
