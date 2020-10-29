using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Dimension
    {
        public Dimension()
        {
            this.CategoryDimensions = new List<CategoryDimension>();
            this.ProductVarients = new List<ProductVarient>();
            this.TempProductVarients = new List<TempProductVarient>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Dimension Name is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Dimension Name must be between 3 to 50 Chatacter")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<CategoryDimension> CategoryDimensions { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ProductVarient> ProductVarients { get; set; }
        public virtual List<TempProductVarient> TempProductVarients { get; set; }
    }
}
