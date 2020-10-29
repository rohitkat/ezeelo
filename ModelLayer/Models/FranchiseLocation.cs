using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class FranchiseLocation
    {
        public FranchiseLocation()
        {
            this.FranchiseCategories = new List<FranchiseCategory>();
        }

        public long ID { get; set; }
        public Nullable<int> FranchiseID { get; set; }
        public Nullable<int> AreaID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Area Area { get; set; }
        public virtual Franchise Franchise { get; set; }
        public virtual List<FranchiseCategory> FranchiseCategories { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
