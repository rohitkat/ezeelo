using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ProductBulkDetail
    {
        public int ID { get; set; }
        public int BulkLogID { get; set; }
        public int ExcelRowID { get; set; }
        public long TempProductID { get; set; }
        public string ExcelProductName { get; set; }
        public string Result { get; set; }
        public bool  IsDescUpload { get; set; }
        public string Description { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual BulkLog BulkLog { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual TempProduct TempProduct { get; set; }
    }
}
