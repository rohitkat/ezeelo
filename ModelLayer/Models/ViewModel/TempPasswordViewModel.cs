using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
   public class TempPasswordViewModel
    {
       public string Customer { get; set; }
       public string Email { get; set; }
       public string Mobile { get; set; }
       public DateTime PasswordGenerateDate { get; set; }
       public string PasswordGeneratedBy { get; set; }
       public DateTime LoginTime { get; set; }
    }
}
