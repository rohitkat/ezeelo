using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ShopPriority
    {
        public long ID { get; set; }
        public long CityID { get; set; }
        public long CategoryID { get; set; }
        public long ShopID { get; set; }
        public int PriorityLevel { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<int> FranchiseID { get; set; } ////added
    }
}
