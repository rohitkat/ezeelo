using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Plan
    {
        public Plan()
        {
            this.OwnerPlans = new List<OwnerPlan>();
            this.PlanBinds = new List<PlanBind>();
            this.PlanCategoryCharges = new List<PlanCategoryCharge>();
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "Code is Required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Code Name must be between 3 to 15 Chatacter")]
        public string PlanCode { get; set; }

        [Required(ErrorMessage = "Short Name is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Short Name must be between 3 to 50 Chatacter")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Entities Allowed is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Entities Allowed")]
        public int NoOfEntitiesAllowed { get; set; }

        [Required(ErrorMessage = "No of Year is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid No of Year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "No of Month is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid No of MMonth")]
        public int Month { get; set; }

        [Required(ErrorMessage = "No of Day is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid No of Day ")]
        public int Day { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Entities Allowed is Required")]
        [Range(double.MinValue,double.MaxValue, ErrorMessage = "Invalid Entities Allowed")]
        public decimal Fees { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<OwnerPlan> OwnerPlans { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<PlanBind> PlanBinds { get; set; }
        public virtual List<PlanCategoryCharge> PlanCategoryCharges { get; set; }
    }
}
