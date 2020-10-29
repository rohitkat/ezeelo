using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CustomerOrderOfferDetail
    {
        public int ID { get; set; }
        public long CustomerOrderDetailId { get; set; }
        public long FreeShopStockId { get; set; }
        public int FreeQty { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrderDetail CustomerOrderDetail { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual ShopStock ShopStock { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
    }
}
