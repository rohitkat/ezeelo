using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TempProductTax
    {

        public long ID { get; set; }
        public long ShopStockID { get; set; }
        public int TaxID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual TaxationMaster TaxationMaster { get; set; }
        public virtual TempShopStock TempShopStock { get; set; }
    }
}
