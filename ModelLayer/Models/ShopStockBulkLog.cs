using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ShopStockBulkLog
    {
        public int ID { get; set; }
        public long TempProductID { get; set; }
        public long TempShopStockID { get; set; }
        public int ExcelRowID { get; set; }
        public int BulkLogID { get; set; }
        public string Result { get; set; }
        public string Description { get; set; }
        public Nullable<int> ImageCount { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BulkLog BulkLog { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual TempProduct TempProduct { get; set; }
        public virtual TempShopStock TempShopStock { get; set; }
    }
}
