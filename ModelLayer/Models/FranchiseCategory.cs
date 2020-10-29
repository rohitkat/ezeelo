using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class FranchiseCategory
    {
        public long ID { get; set; }
        
        public long FranchiseLocationID { get; set; }
        public int CategoryID { get; set; }
        public Nullable<int> ParentCategoryId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Category Category { get; set; }
        public virtual Category Category1 { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual FranchiseLocation FranchiseLocation { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
