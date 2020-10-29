using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.ViewModel
{
   public class FranchiseRegisterNowViewModel
    {
        public long ID { get; set; }

        public long LoginID { get; set; }

        public int SalutationID { get; set; }

        public long BusinessID { get; set; }

 public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
        
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string PinCode { get; set; }

        public string UserName { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string ConfirmPassword { get; set; }

        public int SecurityQuestionID { get; set; }

        public string SecurityAnswer { get; set; }
    }
}
