using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Color
    {
        public Color()
        {
            this.ProductVarients = new List<ProductVarient>();
            this.TempProductVarients = new List<TempProductVarient>();
        }

        public int ID { get; set; }
        [Required(ErrorMessage = "Color Code is Required")]
        [StringLength(7, MinimumLength = 3, ErrorMessage = "Color Code must be between 3 to 7 Chatacter")]
        public string HtmlCode { get; set; }

        [Required(ErrorMessage = "Color Name is Required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Color Name must be between 3 to 50 Chatacter")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ProductVarient> ProductVarients { get; set; }
        public virtual List<TempProductVarient> TempProductVarients { get; set; }
    }
}
