using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ReferAndEarnTransaction
    {
        public long ID { get; set; }
        public long CustomerOrderID { get; set; }
        public long ReferDetailID { get; set; }
        public Nullable<long> TransactionID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual ReferDetail ReferDetail { get; set; }
    }
}
