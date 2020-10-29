using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class ForgetPassword
    {
        [Required(ErrorMessage = "Please Enter the Mobile No.")]
        [Display(Name = "Customer Mobile No.")]
        [RegularExpression(@"^([7-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Please Enter SessionCode.")]
        public string SessionCode { get; set; }
        [Required(ErrorMessage = "Please Enter OTP.")]
        public string OTP { get; set; }





    }
}