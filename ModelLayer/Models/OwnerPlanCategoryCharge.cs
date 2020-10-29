using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class OwnerPlanCategoryCharge
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Plan Name is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Plan Name")]
        public int OwnerPlanID { get; set; }

        [Required(ErrorMessage = "Category is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Category")]
        public int CategoryID { get; set; }

        [Range(0, 100, ErrorMessage = "Invalid Charge")]
        public decimal ChargeInPercent { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Invalid Charge")]
        public decimal ChargeInRupee { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public virtual Category Category { get; set; }
        public virtual OwnerPlan OwnerPlan { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
