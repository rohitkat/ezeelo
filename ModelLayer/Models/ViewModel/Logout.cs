using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
        public class Logout
        {
            [Required(ErrorMessage = "Please Enter the User Name")]
            [Display(Name = "User Name")]
            public string UserName { get; set; }


        }
    
}
