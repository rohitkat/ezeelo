using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class SubscriptionPlanFacility
    {
        public SubscriptionPlanFacility()
        {
            this.SubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
        }

        public long ID { get; set; }
        [Display(Name = "Subscription Plan")]
        public long SubscriptionPlanID { get; set; }
        [Display(Name = "Subscription Facility")]
        public long SubscriptionFacilityID { get; set; }
        [Display(Name = "Facility Value")]
        public int FacilityValue { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual SubscriptionFacility SubscriptionFacility { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
        public virtual ICollection<SubscriptionPlanUsedBy> SubscriptionPlanUsedBies { get; set; }
    }
}
