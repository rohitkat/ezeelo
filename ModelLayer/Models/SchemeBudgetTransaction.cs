using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SchemeBudgetTransaction
    {
        public long ID { get; set; }
        public long ReferAndEarnSchemaID { get; set; }
        public decimal TotalBudgetAmt { get; set; }
        public decimal RemainingAmt { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ReferAndEarnSchema ReferAndEarnSchema { get; set; }
    }
}
