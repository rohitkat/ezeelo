using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Franchise.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter the User Name")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter the Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
