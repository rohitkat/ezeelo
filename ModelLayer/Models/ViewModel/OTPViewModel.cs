using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class OTPViewModel
    {
        public string OTP { get; set; }
    }
    public class RegisterOTPViewModel
    {
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string EmailOTP { get; set; }
        public string MobileOTP { get; set; }
    }
}