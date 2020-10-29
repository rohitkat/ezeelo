using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TransactionInput
    {
        public TransactionInput()
        {
            this.TransactionInputProcessAccounts = new List<TransactionInputProcessAccount>();
        }

        public long ID { get; set; }
        public long CustomerOrderDetailID { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public Nullable<long> ChannelPartnerID { get; set; }
        public Nullable<long> MCOCustomerID { get; set; }
        public Nullable<long> MCOShopID { get; set; }
        public Nullable<long> MCODeliveryID { get; set; }
        public long GandhibaghID { get; set; }
        public int Qty { get; set; }
        public decimal MRPPerUnit { get; set; }
        public decimal SaleRatePerUnit { get; set; }
        public Nullable<decimal> OfferInPercentByShopPerUnit { get; set; }
        public decimal OfferInRsByShopPerUnit { get; set; }
        public bool IsInclusiveOfTAX { get; set; }
        public long TAXID { get; set; }
        public decimal ServiceTAX { get; set; }
        public Nullable<bool> IsShopHandleOtherTAX { get; set; }
        public Nullable<decimal> SumOfOtherTAX { get; set; }
        public Nullable<decimal> LandingPriceByShopPerUnit { get; set; }
        public Nullable<decimal> ChargeInRsByGBPerUnit { get; set; }
        public Nullable<decimal> ChargeINPercentByGBPerUnit { get; set; }
        public Nullable<decimal> CommisionInRsMCOCustomer { get; set; }
        public Nullable<decimal> CommisionInPercentMCOCustomer { get; set; }
        public Nullable<decimal> CommisionInRsMCOShop { get; set; }
        public Nullable<decimal> CommisionInPercentMCOShop { get; set; }
        public Nullable<decimal> CommisionInRsMCODelivery { get; set; }
        public Nullable<decimal> CommisionInPercentMCODelivery { get; set; }
        public Nullable<decimal> CommisionInRsGB { get; set; }
        public Nullable<decimal> CommisionInPercentGB { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual ICollection<TransactionInputProcessAccount> TransactionInputProcessAccounts { get; set; }
    }
}
