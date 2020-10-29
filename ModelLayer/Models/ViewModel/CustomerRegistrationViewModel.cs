using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public partial class CustomerRegistrationViewModel
    {
        public int ID { get; set; }

        [Required (ErrorMessage="Please Enter First Name")]
        [Display(Name="Customer First Name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        //[Required(ErrorMessage = "Please Enter the Last Name")]
        [Display(Name = "Customer Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter the Email Id")]
        [Display(Name = "Customer Email Id")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                        ErrorMessage = "Email is not valid")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile No.")]
        [Display(Name = "Customer Mobile No.")]
        [RegularExpression(@"^([5-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Use 6 characters or more for your password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [StringLength(int.MaxValue, MinimumLength = 6, ErrorMessage = "Use 6 characters or more for your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        //Yashaswi for mlm sighUp
        [Required(ErrorMessage = "Please Enter Referral Id")]
        [System.Web.Mvc.Remote("ValidateReferralId", "Login", ErrorMessage = "Not a valid Referral ID!")]
        public string ReferralId { get; set; }
    }
}