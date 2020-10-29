using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ProductVarient
    {
        public ProductVarient()
        {
            this.ShopStocks = new List<ShopStock>();
        }

        public long ID { get; set; }
        public long ProductID { get; set; }
        public int ColorID { get; set; }
        public int SizeID { get; set; }
        public int DimensionID { get; set; }
        public int MaterialID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Color Color { get; set; }
        public virtual Dimension Dimension { get; set; }
        public virtual Material Material { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual Size Size { get; set; }
        public virtual List<ShopStock> ShopStocks { get; set; }
    }
}
