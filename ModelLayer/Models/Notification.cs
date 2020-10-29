using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Notification
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public int FromBusinessTypeID { get; set; }
        public long FromPersonalDetailID { get; set; }
        public int ToBusinessTypeID { get; set; }
        public long ToPersonalDetailID { get; set; }
        public long TargetOwnerId { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual BusinessType BusinessType1 { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual PersonalDetail PersonalDetail2 { get; set; }
        public virtual PersonalDetail PersonalDetail3 { get; set; }
    }
}
