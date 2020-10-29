using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class TaxationMaster
    {
        public TaxationMaster()
        {
            this.FranchiseTaxDetails = new List<FranchiseTaxDetail>();
            this.ProductTaxes = new List<ProductTax>();
            this.TaxationBases = new List<TaxationBase>();
            /*Taxes New Changes for product Upload*/
            this.TempProductTaxes = new List<TempProductTax>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Prefix is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Prefix must be between 3 - 50 characters ")]
        public string Prefix { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Name must be between 3 - 150 characters ")]
        public string Name { get; set; }
        [StringLength(300, MinimumLength = 0, ErrorMessage = "Description must be between 0 - 300 characters ")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<FranchiseTaxDetail> FranchiseTaxDetails { get; set; }
        public virtual ICollection<ProductTax> ProductTaxes { get; set; }
        public virtual ICollection<TaxationBase> TaxationBases { get; set; }

        /*Taxes New Changes for product Upload*/
        public virtual ICollection<TempProductTax> TempProductTaxes { get; set; }
    }
}
