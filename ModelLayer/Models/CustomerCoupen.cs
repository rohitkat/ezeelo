using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CustomerCoupen
    {
        public int ID { get; set; }
        public long UserLoginId { get; set; }
        public string CoupenCode { get; set; }
        public Nullable<long> CustomerOrderID { get; set; }
        public Nullable<decimal> CoupenValueInRs { get; set; }
        public bool IsRedeem { get; set; }
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
    }
}
