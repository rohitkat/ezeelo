using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please! enter Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Please! Enter OTP")]
        public string OTP { get; set; }

        [Required(ErrorMessage = "Please! enter password")]
        public string NewPassword { get; set; }
       
    }
}
