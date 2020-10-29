using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class SubscriptionFacility
    {
        public SubscriptionFacility()
        {
            this.SubscriptionPlanFacilities = new List<SubscriptionPlanFacility>();
        }

        public long ID { get; set; }
        [Display(Name = "Facilitie")]
        public string Name { get; set; }
        [Display(Name = "Behavior Type")]
        public int BehaviorType { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<SubscriptionPlanFacility> SubscriptionPlanFacilities { get; set; }
    }
}
