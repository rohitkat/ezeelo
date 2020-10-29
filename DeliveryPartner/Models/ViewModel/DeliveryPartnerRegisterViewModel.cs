using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DeliveryPartner.Models.ViewModel
{
    public class DeliveryPartnerRegisterViewModel
    {
        public long ID { get; set; }

        public long LoginID { get; set; }

        public int SalutationID { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Bussiness Name is required")]
        public string BussinessName { get; set; }

        public long BusinessID { get; set; }

        public long ShopID { get; set; }

        //public string Website { get; set; }
        [Required(ErrorMessage = "Email is required (we promise not to spam you!)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mobile No. is required")]
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
        public string Mobile { get; set; }

        public string PinCode { get; set; }

        //public string TIN { get; set; }

        //public string WAT { get; set; }

       // public string PAN { get; set; }
        [Required]
        public int ServiceLevel { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Select security question")]
        public int SecurityQuestionID { get; set; }
        [Required(ErrorMessage = "Security answer is required")]
        public string SecurityAnswer { get; set; }

        public LoginViewModel Login { get; set; }
    }
}

