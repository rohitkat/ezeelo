using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class SchemeType
    {
        public SchemeType()
        {
            this.CoupenLists = new List<CoupenList>();
            this.TodaySchemes = new List<TodayScheme>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public decimal ValueInRs { get; set; }
        public Nullable<decimal> ApplicableOnPurchaseOfRs { get; set; }
        public string SchemeCode { get; set; }
        public Nullable<int> BussinessTypeID { get; set; }
        public int OwnerId { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BusinessType BusinessType { get; set; }
        public virtual ICollection<CoupenList> CoupenLists { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual ICollection<TodayScheme> TodaySchemes { get; set; }
    }
}
