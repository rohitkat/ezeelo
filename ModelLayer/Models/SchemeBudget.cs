using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SchemeBudget
    {
        public long ID { get; set; }
        public long ReferAndEarnSchemaID { get; set; }
        public decimal BudgetAmount { get; set; }
        public Nullable<decimal> PreRemainingAmt { get; set; }
        public Nullable<decimal> PreUsedAmt { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public bool ActionStatus { get; set; }
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
