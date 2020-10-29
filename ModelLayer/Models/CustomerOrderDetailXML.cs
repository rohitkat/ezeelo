using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CustomerOrderDetailXML
    {
        public int ID { get; set; }
        public long CustomerOrderID { get; set; }
        public string XML { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
