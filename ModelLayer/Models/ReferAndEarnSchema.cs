using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ReferAndEarnSchema
    {
        public ReferAndEarnSchema()
        {
            this.EarnDetails = new List<EarnDetail>();
            this.ReferDetails = new List<ReferDetail>();
            this.SchemeBudgets = new List<SchemeBudget>();
            this.SchemeBudgetTransactions = new List<SchemeBudgetTransaction>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
        public bool OrderwiseEarn { get; set; }
        public bool UserwiseEarn { get; set; }
        public Nullable<decimal> EarnInRS { get; set; }
        public Nullable<decimal> EarnInPercentage { get; set; }
        public Nullable<int> MaxNoOfOrders { get; set; }
        public Nullable<decimal> MaxPurchaseAmount { get; set; }
        public Nullable<int> ExpirationDays { get; set; }
        public long CityID { get; set; }
        public Nullable<int> FranchiseID { get; set; } ////added
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
      
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }////added
        public virtual ICollection<EarnDetail> EarnDetails { get; set; }
        public virtual ICollection<ReferDetail> ReferDetails { get; set; }
        public virtual ICollection<SchemeBudget> SchemeBudgets { get; set; }
        public virtual ICollection<SchemeBudgetTransaction> SchemeBudgetTransactions { get; set; }
    }
}
