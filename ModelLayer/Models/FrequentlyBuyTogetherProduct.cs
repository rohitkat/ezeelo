using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class FrequentlyBuyTogetherProduct
    {
        public long ID { get; set; }
        public long WithProductID { get; set; }
        public long ThisProductID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
      //  public int CategoryID { get; set; }
      //  public int SpecificationID { get; set; }
      //  public int ProductID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Product Product { get; set; }
        public virtual Product Product1 { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
