using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class StockComponent
    {
        public long ID { get; set; }
        public long ShopStockID { get; set; }
        public int ComponentID { get; set; }
        public Nullable<decimal> ComponentWeight { get; set; }
        public Nullable<int> ComponentUnitID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Component Component { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual ShopStock ShopStock { get; set; }
    }
}
