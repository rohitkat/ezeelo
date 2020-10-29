using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Charge
    {
        public Charge()
        {
            this.GandhibaghTransactions = new List<GandhibaghTransaction>();
            this.PlanCategoryCharges = new List<PlanCategoryCharge>();
            this.ShopProductCharges = new List<ShopProductCharge>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Charge Stage is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Charge Stage")]
        public int ChargeStageID { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Name must be between 5 to 150 Chatacter")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ChargeStage ChargeStage { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual List<GandhibaghTransaction> GandhibaghTransactions { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<PlanCategoryCharge> PlanCategoryCharges { get; set; }
        public virtual List<ShopProductCharge> ShopProductCharges { get; set; }
    }
}
