using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class BulkLog
    {
        public BulkLog()
        {
            this.ProductBulkDetails = new List<ProductBulkDetail>();
            this.ShopStockBulkLogs = new List<ShopStockBulkLog>();
        }

        public int ID { get; set; }
        public long ShopID { get; set; }
        public string ExcelSheetName { get; set; }
        public int BulkType { get; set; }
        public Nullable<int> TotalSuccess { get; set; }
        public Nullable<int> TotalFail { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual ICollection<ProductBulkDetail> ProductBulkDetails { get; set; }
        public virtual ICollection<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
    }
}
