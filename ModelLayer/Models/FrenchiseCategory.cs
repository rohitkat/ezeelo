using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class FrenchiseCategory
    {
        public long ID { get; set; }
        public long FrenchiseLocationID { get; set; }
        public long CategoryID { get; set; }
        public long ParentCategoryID { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
