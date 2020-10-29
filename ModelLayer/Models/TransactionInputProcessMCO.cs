using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TransactionInputProcessMCO
    {
        public long ID { get; set; }
        public long TransactionInputProcessShopID { get; set; }
        public int Qty { get; set; }
        public decimal TotalMRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public decimal MCOCustomerReceivable { get; set; }
        public decimal MCOShopReceivable { get; set; }
        public decimal MCODeliveryReceivable { get; set; }
        public decimal GBReceivable { get; set; }
        public decimal AmountRemaining { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
    }
}
