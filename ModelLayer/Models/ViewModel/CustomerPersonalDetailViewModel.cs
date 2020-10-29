using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ModelLayer.CustomAnnotation;

namespace ModelLayer.Models.ViewModel
{
    public class CustomerPersonalDetailViewModel:IValidatableObject
    {
        [Required]
        public Int64 ID { get; set; }
        [Required]
        public Int64 UserLoginID { get; set; }
        [Required(ErrorMessage = "Please Select Salutation")]
        [Display(Name = "Salutation")]
        public int SalutationID { get; set; }

        [Required(ErrorMessage = "Please Select Security Question")]
        [Display(Name = "Security Question")]
        public int SecurityQuestionID { get; set; }

        [Required(ErrorMessage = "Please Enter Security Answer")]
        [Display(Name = "Security Answer")]
        public string SecurityAnswer { get; set; }

        [Required(ErrorMessage = "Please Enter the First Name")]
        [Display(Name = "Customer First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please Enter the Last Name")]
        [Display(Name = "Customer Last Name")]
        public string LastName { get; set; }

       // [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }

        [Required]
        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string Mobile { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DOB { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "Please Enter the Pincode")]
        [Display(Name = "Pincode")]
        public string PincodeID { get; set; }
        public string Address { get; set; }

        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string AlternateMobileNo { get; set; }

        [DataType(DataType.EmailAddress)]
        public string AlternateEmailID { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    throw new NotImplementedException();
        //}
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB > DateTime.Now)
            {
                yield return new ValidationResult("Date can't be in future.");
            }
            if (DOB < DateTime.Now.AddYears(-100))
            {
                yield return new ValidationResult("Date of birth can't be too past.");
            }
        }
    }
}