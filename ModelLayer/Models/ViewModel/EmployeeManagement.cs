using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models.ViewModel
{
   public class EmployeeManagement
    {
        public long ID { get; set; }
        public long UserLoginID { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<long> OwnerID { get; set; }

        [Required(ErrorMessage = "Salutation is Required")]
        public int SalutationID { get; set; }

       [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

       [Required(ErrorMessage = "Middle Name is Required")]
        public string MiddleName { get; set; }

       [Required(ErrorMessage = "Last Name is Required")]
       public string LastName { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string Gender { get; set; }
        public Nullable<int> PincodeID { get; set; }
        public string Address { get; set; }
        public string AlternateMobile { get; set; }
        public string AlternateEmail { get; set; }

        [Required(ErrorMessage = "Modile Number is Required")]
        [RegularExpression(@"^(?:\d{10}|00\d{10}|\+\d{2}\d{8})$", ErrorMessage = "Please enter proper Mobile No.")]
       public string Mobile { get; set; }
       [EmailAddress(ErrorMessage="Invalid Email Address")]
       [Required(ErrorMessage = "Email Address is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
       [Required(ErrorMessage = "Password is Required")]
       [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
   

    }
}
