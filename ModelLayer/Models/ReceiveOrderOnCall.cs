using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class ReceiveOrderOnCall
    {
        public int ID { get; set; }
        public long OrderReceivedPersonalDetailID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper primary mobile No.")]
        public string PrimaryMobile { get; set; }
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper secondary mobile No.")]
        public string SecondaryMobile { get; set; }
        [Required(ErrorMessage = "Email is required (we promise not to spam you!)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(150)]
        public string ShippingAddress { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(300)]
        public string Description { get; set; }
        public Nullable<long> CustomerOrderID { get; set; }
        [Required]
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
    }
}
