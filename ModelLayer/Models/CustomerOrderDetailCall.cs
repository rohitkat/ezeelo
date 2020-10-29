using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModelLayer.CustomAnnotation;

namespace ModelLayer.Models
{
    public partial class CustomerOrderDetailCall
    {
        public long ID { get; set; }
        public int BusinessTypeID { get; set; }
        public long OwnerID { get; set; }
        public string ShopOrderCode { get; set; }
        public int OrderStatus { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(300)]
        public string Description { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }

        //// [DeliveredOTPValidation("IsOTPAvailable", "CustomerOrderDetailCall", ErrorMessage = "OTP is not matching")] // Hide from Ashish for Live
        ////public string OTP { get; set; } // Hide from Ashish for Live
    }
}
