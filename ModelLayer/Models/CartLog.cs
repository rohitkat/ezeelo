using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class CartLog
    {
        public long ID { get; set; }
        public long CartID { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Description must be between 3 - 150 characters ")]   
        public string Description { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
