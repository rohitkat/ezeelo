using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SubscriptionPlanUsedBy
    {
        public long ID { get; set; }
        public long SubscriptionPlanPurchasedByID { get; set; }
        public long SubscriptionPlanFacilityID { get; set; }
        public int SubsriptionValue { get; set; }
        public long CustomerOrderID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual SubscriptionPlanFacility SubscriptionPlanFacility { get; set; }
        public virtual SubscriptionPlanPurchasedBy SubscriptionPlanPurchasedBy { get; set; }
    }
}
