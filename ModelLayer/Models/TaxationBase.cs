using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class TaxationBase
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Taxation is Required")]
        public int TaxationID { get; set; }
        public int FranchiseTaxDetailID { get; set; }
        [StringLength(150, MinimumLength = 0, ErrorMessage = "Description must be between 3 - 150 characters ")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual FranchiseTaxDetail FranchiseTaxDetail { get; set; }
        public virtual TaxationMaster TaxationMaster { get; set; }
    }
}
