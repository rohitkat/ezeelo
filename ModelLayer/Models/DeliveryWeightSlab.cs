using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DeliveryWeightSlab
    {
        public int ID { get; set; }
        public int DeliveryPartnerID { get; set; }
        [Range(1, 10000)]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper max weight.")]
        [Required(ErrorMessage = "Max weight is required")]
        public decimal MaxWeight { get; set; }
        [Range(1, 100000)]
        [Required(ErrorMessage = "Normal delivery charge within pincode is required")]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper normal delivery charge within pincode.")]
        public decimal NormalRateWithinPincodeList { get; set; }
        [Range(1, 100000)]
        [Required(ErrorMessage = "Express delivery charge within pincode is required")]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper express delivery charge within pincode.")]
        public decimal ExpressRateWithinPincodeList { get; set; }
        [Range(1, 100000)]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper express delivery charge out of pincode.")]
        public Nullable<decimal> ExpressRateOutOfPincodeList { get; set; }
        [Range(1, 100000)]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper normal delivery charge out of pincode.")]
        public Nullable<decimal> NormalRateOutOfPincodeList { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual DeliveryPartner DeliveryPartner { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
    }
}
