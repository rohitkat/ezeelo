using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class TestModel
    {
        // add Captcha code as a field in the protected action Model
        [Required(ErrorMessage = "Retyping the characters from the picture is required.")]
        [Display(Name = "Please retype the characters from the picture")]
        public string CaptchaCode { get; set; }
    }
}
