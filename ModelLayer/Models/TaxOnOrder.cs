using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class TaxOnOrder
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "CustomerOrderDetailID is Required")]
        public long CustomerOrderDetailID { get; set; }
        [Required(ErrorMessage = "ProductTaxID is Required")]
        public long ProductTaxID { get; set; }
        [Required(ErrorMessage = "Amount is Required")]
        public decimal Amount { get; set; }
        public bool IsGSTInclusive { get; set; } // Added by Zubair for GST on 06-07-2017
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrderDetail CustomerOrderDetail { get; set; }
        public virtual ProductTax ProductTax { get; set; }
    }
}
