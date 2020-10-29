using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class OfferZoneProduct
    {
        public int ID { get; set; }
        public long OfferID { get; set; }
        public long ShopStockID { get; set; }
        public Nullable<long> FreeStockID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Offer Offer { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual ShopStock ShopStock { get; set; }
        public virtual ShopStock ShopStock1 { get; set; }
    }
}
