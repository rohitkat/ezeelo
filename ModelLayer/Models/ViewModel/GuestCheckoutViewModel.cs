using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class GuestCheckoutViewModel
    {
        //[Required]
        //public int SalutationID { get; set; }

        //[Required(ErrorMessage = "Please Enter the First Name")]
        //[Display(Name = "Customer First Name")]
        //public string FirstName { get; set; }
       
        //public string MiddleName { get; set; }

        //[Required(ErrorMessage = "Please Enter the Last Name")]
        //[Display(Name = "Customer Last Name")]
        //public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter the Email Id")]
        [Display(Name = "Customer Email Id")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                        @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                        @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                        ErrorMessage = "Email is not valid")]
        public string EmailID { get; set; }

        [Required(ErrorMessage = "Please Enter the Mobile No.")]
        [Display(Name = "Customer Mobile No.")]
        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string MobileNo { get; set; }
    }
}
