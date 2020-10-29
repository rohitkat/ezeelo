using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class FranchiseTaxDetail
    {
        public FranchiseTaxDetail()
        {
            this.TaxationBases = new List<TaxationBase>();
        }

        public int ID { get; set; }
        public long CityID { get; set; }
        [Required(ErrorMessage = "Franchise is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Franchise is not valid")]
        public int FranchiseID { get; set; }
        [Required(ErrorMessage = "Taxation is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Taxation is not valid")]
        public int TaxationID { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "InPercentage is not valid")]
        public decimal InPercentage { get; set; }
         [Range(0, double.MaxValue, ErrorMessage = "InRupees is not valid")]
        public decimal InRupees { get; set; }
        public bool IsDirect { get; set; }
        public bool IsCustomerSide { get; set; }
        public bool IsOnTaxSum { get; set; }
        public bool IsMinusTaxs { get; set; }
        public bool IsIncludeSaleRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPercentage { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "LowerLimit is not valid")]
        public decimal LowerLimit { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "UpperLimit is not valid")]
        public decimal UpperLimit { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual TaxationMaster TaxationMaster { get; set; }
        public virtual ICollection<TaxationBase> TaxationBases { get; set; }
    }
}
