using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class SocialRegistrationViewModel
    {
        public int ID { get; set; }

        //[Required(ErrorMessage = "Please Enter First Name")]
        [Display(Name = "Customer First Name")]
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

        [Required(ErrorMessage = "Please Provide SocialId")]
        public string SocialId { get; set; }
        [Required(ErrorMessage = "Please Provide SocialAccType")]
        public string SocialAccType { get; set; }
        public string ReferralId { get; set; }
    }
}
