using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using System.Web.Mvc;

namespace ModelLayer.Models.ViewModel
{
 
    // Added by Amit on 8/8/2018 for leaders profile
    public class LeadersProfileViewModel
    {
        
        public long UserID { get; set; }
        public int? BankID { get; set; }
        public string FirstName { get; set; }
        public string MiddelName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",  ErrorMessage = "Not a valid phone number")]
        public string Mobile { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

        [Remote("AgeValidate", "LeadersProfile", HttpMethod = "POST", ErrorMessage = "Age should not be small than 18 years!!")]
        public DateTime? DOB { get; set; }

        public string Address { get; set; }
        public string RefferelID { get; set; }
        public string Gender { get; set; }
        [Required(ErrorMessage = "Pincode Required")]
        public string Pincode { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }

        [Required(ErrorMessage="Aadhar Card No. Required")]
       
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Aadhar Number must be numeric")]
        [RegularExpression(@"^(\d{12}|\d{16})$", ErrorMessage = "* Invalid Aadhar Number")]
        public string AdharCardNo { get; set; }
        [Required(ErrorMessage = "PAN Card No. Required")]
        [RegularExpression(@"[A-Z]{5}\d{4}[A-Z]{1}", ErrorMessage = "* Invalid PAN Number")]
        public string PAN { get; set; }
        [Required(ErrorMessage = "Bank Name Required")]
        public string BankName { get; set; }
        [Required(ErrorMessage = "IFSC Required")]
        [RegularExpression(@"^[A-Za-z]{4}[0][A-Za-z0-9]{6}$", ErrorMessage = "* Invalid IFSC Code")]
        public string IFSC { get; set; }
        public string AccountType { get; set; }
        [Required(ErrorMessage = "Account Number Required")]
        [StringLength(16, MinimumLength = 11)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Account Number  must be numeric")]

        //[Range(11,17)]
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public Decimal? ERP { get; set; }
        public long? TotalMember { get; set; }
        public Decimal? AvgIncome { get; set; }



        [Required(ErrorMessage = "Benificiary Name Required")]
        public string BenificiaryName { get; set; } // on 15-1-19
        [Required(ErrorMessage = "Benificiary Email Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string BenificiaryEmail { get; set; }  //on 15-1-19

        public string PANImage { get; set; }
        public string PassbookImage { get; set; }
        public string AdhaarImageUrl { get; set; }

        public string KYCForm { get; set; }  // on 15-2-19

        public DateTime? UpdateDate { get; set; }//Yashaswi
        public string SMSText { get; set; }//Yashaswi
        public bool IsVerified { get; set; }//Yashaswi
        public string Remark { get; set; }//Yashaswi
    }
}
