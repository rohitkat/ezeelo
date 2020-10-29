using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class MerchantRegisterViewModel
    {
        public long ID { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string ConfirmPassword { get; set; }
        public Salutation salutation{ get; set; }
        public UserLogin userLogin { get; set; }
        public SecurityQuestion securityQuestion{ get; set; }
        public LoginSecurityAnswer loginSecurityAnswer { get; set; }
        public PersonalDetail personalDetail { get; set; }
        public BusinessDetail businessDetail { get; set; }
        public Shop shop { get; set; }
        public Bank bank { get; set; }
        public string MyPincode { get; set; }
        public Pincode pincode { get; set; }
        public LoginViewModel loginViewModel { get; set; }
    }
}
