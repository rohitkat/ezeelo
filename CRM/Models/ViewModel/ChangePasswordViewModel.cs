using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models.ViewModel
{
    public class ChangePasswordViewModel
    {
        public long ID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReTypeNewPassword { get; set; }
    }
}