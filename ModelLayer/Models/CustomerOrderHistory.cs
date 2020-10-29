using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CustomerOrderHistory
    {
        public long ID { get; set; }
        public long CustomerOrderID { get; set; }
        public long ShopStockID { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual ShopStock ShopStock { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
