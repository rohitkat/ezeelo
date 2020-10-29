using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class DeliveryCashHandlingCharge
    {
        public long ID { get; set; }
        public int DeliveryPartnerID { get; set; }
        [Range(1, 1000000)]
        [Required(ErrorMessage = "Max amount is required")]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper max amount.")]
        public decimal MaxAmount { get; set; }
        [Range(1, 100000)]
        [Required(ErrorMessage = "Per hour charge is required")]
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper per hour charge.")]
        public decimal PerHourCharge { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual DeliveryPartner DeliveryPartner { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
    }
}
