using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class ChangePassword
    {   [Required(ErrorMessage="Customer loginID required.")]
        public long CustLoginID { get; set; }
        [Required(ErrorMessage = "Old password required.")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New password required.")]
        public string NewPassword { get; set; }
        
    }
}