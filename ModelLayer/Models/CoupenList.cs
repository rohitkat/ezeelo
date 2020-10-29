using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CoupenList
    {
        public int ID { get; set; }
        public int SchemeTypeID { get; set; }
        public string CoupenCode { get; set; }
        public int CoupenQty { get; set; }
        public int UsedQty { get; set; }
        public long CityID { get; set; }
        public Nullable<int> FranchiseID { get; set; }//added
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual City City { get; set; }
        public virtual Franchise Franchise { get; set; }////added
        public virtual SchemeType SchemeType { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
