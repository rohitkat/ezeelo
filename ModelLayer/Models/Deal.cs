using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models
{
    [Table("Deal")]
    public class Deal
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "Offer Name is Required")]
        public string ShortName { get; set; }
        [Required(ErrorMessage = "Offer Description is Required")]
        public string Description { get; set; }
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper Min Purchase Quantity.")]
        public int MinPurchaseQty { get; set; }
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper Value.")]
        public decimal DiscountInRs { get; set; }
        [RegularExpression(@"^(?:0|[1-9]\d*)(?:\.(?!.*000)\d+)?$", ErrorMessage = "Please enter proper Value")]
        public decimal DiscountInPercent { get; set; }
        public bool IsActive { get; set; }
        public bool IdSectionDisplay { get; set; }//added by Sonali_17-01-2019
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime EndDateTime { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<DealProduct> DealProducts { get; set; }
    }
}
