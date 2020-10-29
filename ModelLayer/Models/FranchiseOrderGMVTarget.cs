using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class FranchiseOrderGMVTarget
    {
        public long ID { get; set; }
        [Required]
        public int FranchiseID { get; set; }
         [Required]
        public long CityID { get; set; }
         [Required]
         public Nullable<int> MonthlyOrderTarget { get; set; }
         [Required]
         public Nullable<decimal> MonthlyGMVTarget { get; set; }
         [Required]
         public Nullable<int> ForYear { get; set; }
         [Required]
         public Nullable<int> FromMonth { get; set; }
         [Required]
         public Nullable<int> ToMonth { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }
    }
}
