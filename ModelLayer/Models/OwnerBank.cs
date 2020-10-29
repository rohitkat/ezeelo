using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class OwnerBank
    {
        public long ID { get; set; }
        public int BusinessTypeID { get; set; }
        public long OwnerID { get; set; }

        [Display(Name = "Bank")]
        public int BankID { get; set; }
        [Required]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required]
        [Display(Name = "IFSC Code")]
        [StringLength(11, ErrorMessage = "IFSC Code cannot be longer than 11 characters.")]
        public string IFSCCode { get; set; }

        [Display(Name = "MICR Code")]
        [StringLength(50, ErrorMessage = "MICR Code cannot be longer than 50 characters.")]
        public string MICRCode { get; set; }

        //[StringLength(20, ErrorMessage = "Account Number cannot be longer than 50 characters.")]
        [Required]        
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Bank Account Type")]
        public int BankAccountTypeID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        public virtual Bank Bank { get; set; }
        public virtual BankAccountType BankAccountType { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
