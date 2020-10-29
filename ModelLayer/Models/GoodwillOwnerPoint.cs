using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class GoodwillOwnerPoint
    {
        public int ID { get; set; }
        public int BusinessTypeID { get; set; }
        public long OwnerId { get; set; }
        public decimal LastPoint { get; set; }
        public decimal PointAdded { get; set; }
        public decimal TotalPoint { get; set; }
        public string Description { get; set; }
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
