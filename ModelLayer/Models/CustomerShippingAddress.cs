using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class CustomerShippingAddress
    {
        public int ID { get; set; }
         [Required(ErrorMessage = "Please provide UserLoginID.")]
        public long UserLoginID { get; set; }

        [Required(ErrorMessage = "Please Enter the Mobile No.")]
        [Display(Name = "Shipping Mobile No.")]
        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string PrimaryMobile { get; set; }
         [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string SecondaryMobile { get; set; }

        [Required(ErrorMessage = "Please Enter the Shipping Address.")]
        [Display(Name = "Shipping Address")]
        [MaxLength]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Please Enter the Pincode")]
        [Display(Name = "Shipping Pincode")]
        public int PincodeID { get; set; }
        public Nullable<int> AreaID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Area Area { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Pincode Pincode { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public string FirstName { get; set; }//Sonali for Api_25/10/2018
        public string LastName { get; set; }//Sonali for Api_25/10/2018
        public bool IsDeliveryAddress { get; set; }//Sonali for Api_25/10/2018
    }
}
