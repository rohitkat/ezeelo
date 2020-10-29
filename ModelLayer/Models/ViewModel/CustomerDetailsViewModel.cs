//-----------------------------------------------------------------------
// <copyright file="CustomerDetailsViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    public class CustomerDetailsViewModel
    {

        public long UserLoginID { get; set; }
        //[Required]
        public int SalutationID { get; set; }
        //[Required]
        public string SalutationName { get; set; }
        [Required(ErrorMessage = "Please Enter the First Name")]
        [Display(Name = "Customer First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        // [Required]
        public string LastName { get; set; }
        // [Required]
        // [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailId { get; set; }
        // [Required]
        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string MobileNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DOB { get; set; }
        public string Gender { get; set; }
        public string Pincode { get; set; }
        public Nullable<int> PincodeID { get; set; }
        public string Address { get; set; }
        [RegularExpression(@"^([6-9]{1}[0-9]{9})$", ErrorMessage = "Mobile is not valid")]
        public string AlternateMobile { get; set; }
        // [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string AlternateEmail { get; set; }
        public string Password { get; set; }

        // start Sonali 27/8/2018
        public string AdhaarNo { get; set; }
        public string PanNo { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string BankIFSC { get; set; }
        public string BranchName { get; set; }
        public string AccountType { get; set; }
        public string AdhaarImageUrl { get; set; }
        public string PanImageUrl { get; set; }
        public string PassbookImageUrl { get; set; }
        public decimal AvgErp { get; set; }
        public decimal AvgIncome { get; set; }

        public string ReferralId { get; set; }

        public List<SecurityQuestion> QuetionsList { get; set; }

        public List<LoginSecurityAnswer> AnswerList { get; set; }
        //End Sonali 27/8/2018
    }
}
