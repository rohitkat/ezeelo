using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Offer
    {
        public Offer()
        {
            this.OfferDurations = new List<OfferDuration>();
            this.OfferZoneProducts = new List<OfferZoneProduct>();
        }

        public long ID { get; set; }
        public int BusinessTypeID { get; set; }
        public long OwnerID { get; set; }
        [Required(ErrorMessage = "Offer Name is Required")]
        public string ShortName { get; set; }
         [Required(ErrorMessage = "Offer Description is Required")]
        public string Description { get; set; }
        public bool IsFree { get; set; }

        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper Min Purchase Quantity.")]
        public int MinPurchaseQty { get; set; }

        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper  Free Quantity.")]
        public int FreeOty { get; set; }
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper Value.")]
        public decimal DiscountInRs { get; set; }
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper Value")]
        public decimal DiscountInPercent { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<OfferDuration> OfferDurations { get; set; }
        public virtual List<OfferZoneProduct> OfferZoneProducts { get; set; }
    }
}
