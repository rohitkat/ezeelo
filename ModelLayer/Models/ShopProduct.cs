using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ShopProduct
    {
        public ShopProduct()
        {
            this.ShopStocks = new List<ShopStock>();
        }

        public long ID { get; set; }
        public long ShopID { get; set; }
        public long ProductID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime DisplayProductFromDate { get; set; }
        public long? DeliveryTime { get; set; }
        public Nullable<decimal> DeliveryRate { get; set; }
        public Nullable<decimal> TaxRate { get; set; }
        public Nullable<decimal> TaxRatePer { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Product Product { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual List<ShopStock> ShopStocks { get; set; }
    }
}
