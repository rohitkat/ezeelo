using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class SubscriptionPlan
    {
        public SubscriptionPlan()
        {
            this.SubscriptionPlanDealWithCategories = new List<SubscriptionPlanDealWithCategory>();
            this.SubscriptionPlanFacilities = new List<SubscriptionPlanFacility>();
            this.SubscriptionPlanPurchasedBies = new List<SubscriptionPlanPurchasedBy>();
        }

        public long ID { get; set; }
        [Display(Name = "Plan Name")]
        public string Name { get; set; }
        [Display(Name = "Fees")]
        public decimal Fees { get; set; }
        [Display(Name = "No Of Days")]
        public int NoOfDays { get; set; }
        [Display(Name = "No Of Coupens")]
        public int NoOfCoupens { get; set; }
        [Display(Name = "Service Tax")]
        public decimal ServiceTax { get; set; }
        [Display(Name = "Extra Tax")]
        public decimal ExtraTax { get; set; }
        [Display(Name = "Start Date")]
        public System.DateTime StartDate { get; set; }
        [Display(Name = "IsActive")]
        public Nullable<bool> IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<SubscriptionPlanDealWithCategory> SubscriptionPlanDealWithCategories { get; set; }
        public virtual ICollection<SubscriptionPlanFacility> SubscriptionPlanFacilities { get; set; }
        public virtual ICollection<SubscriptionPlanPurchasedBy> SubscriptionPlanPurchasedBies { get; set; }
    }
}
