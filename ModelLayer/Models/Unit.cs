using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class Unit
    {
        public Unit()
        {
            this.ShopStocks = new List<ShopStock>();
            this.TempShopStocks = new List<TempShopStock>();
            this.TempStockComponents = new List<TempStockComponent>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
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
        public virtual List<ShopStock> ShopStocks { get; set; }
        public virtual List<TempShopStock> TempShopStocks { get; set; }
        public virtual List<TempStockComponent> TempStockComponents { get; set; }
    }
}
