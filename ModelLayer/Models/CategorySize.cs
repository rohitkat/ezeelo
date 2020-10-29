using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ModelLayer.Models
{
    public partial class CategorySize
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "Category is Required")]
        [Range(1,int.MaxValue, ErrorMessage="Invalid Category")]        
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Size is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Size")]
        public int SizeID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Category Category { get; set; }
        public virtual Size Size { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
