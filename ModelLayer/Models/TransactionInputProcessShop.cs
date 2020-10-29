using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TransactionInputProcessShop
    {
        public long ID { get; set; }
        public long TransactionInputID { get; set; }
        public int Qty { get; set; }
        public decimal TotalMRP { get; set; }
        public decimal TotalSaleRate { get; set; }
        public decimal TotalOffer { get; set; }
        public decimal NewSaleRateAfterOffer { get; set; }
        public decimal TotalShopFinalPrice { get; set; }
        public decimal ShopReceivable { get; set; }
        public bool IsShopHandleOtherTAX { get; set; }
        public decimal OtherTAXPayableReceivableFromMerchant { get; set; }
        public decimal SumOfAmountShopReceivableAfterOtherTAX { get; set; }
        public decimal GBReceivableAmount { get; set; }
        public decimal GBTransactionFee { get; set; }
        public decimal GBServiceTAXOnTransactionFee { get; set; }
        public decimal FinalShopReceivableAfterAllDone { get; set; }
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
