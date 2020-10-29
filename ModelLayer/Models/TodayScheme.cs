using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TodayScheme
    {
        public int ID { get; set; }
        public Nullable<int> SchemeTypeID { get; set; }
        public Nullable<decimal> TodaysValueInRs { get; set; }
        public Nullable<decimal> ApplicableOnPurchaseOfRs { get; set; }
        public Nullable<System.DateTime> StartDatetime { get; set; }
        public Nullable<System.DateTime> EndDatetime { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual SchemeType SchemeType { get; set; }
    }
}
