using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SubscriptionPlanPurchasedBy
    {
        public SubscriptionPlanPurchasedBy()
        {
            this.SubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
        }

        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public Nullable<long> SailedByEmployeeID { get; set; }
        public long SubscriptionPlanID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }


        //------------aded by mohit on 08-10-15-----------------------//
        public Nullable<long> ReferredByLoginID { get; set; }
        public Nullable<bool> ReferredPointUsed { get; set; }
        //------------End of code by mohit --------------------------//


        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
        public virtual ICollection<SubscriptionPlanUsedBy> SubscriptionPlanUsedBies { get; set; }
    }
}
