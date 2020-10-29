using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class SubscriptionPlanDealWithCategory
    {
        public SubscriptionPlanDealWithCategory()
        {
            this.SubscriptionPlanDealWithCategoryLogs = new List<SubscriptionPlanDealWithCategoryLog>();
        }

        public long ID { get; set; }
        [Display(Name = "Subscription Plan")]
        public long SubscriptionPlanID { get; set; }
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        [Display(Name = "Minimum Amount")]
        public decimal MinimumAmount { get; set; }
        [Display(Name = "Discount In Rs")]
        public decimal DiscountInRs { get; set; }
        [Display(Name = "Discount In Per")]
        public decimal DiscountInPer { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
        public virtual ICollection<SubscriptionPlanDealWithCategoryLog> SubscriptionPlanDealWithCategoryLogs { get; set; }
    }
}
