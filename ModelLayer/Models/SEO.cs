using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class SEO
    {

        public long ID { get; set; }
        [Required(ErrorMessage = "Please Select Businesstype")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Invalid Selection")]
        public int BusinessTypeID { get; set; }

        [Required(ErrorMessage = "Please Select the Option")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Invalid Option Selection")]
        public long EntityID { get; set; }

        //[Required(ErrorMessage = "Please Enter the Header")]
        public string H1 { get; set; }
        [Required(ErrorMessage = "Please Enter the Author")]
        public string Metatag { get; set; }
        [Required(ErrorMessage = "Please Enter the Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Enter the MetaKeyword")]
        public string MetaKeyword { get; set; }
        public string URL { get; set; }
        public string PageName { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
