using ModelLayer.Models.Enum;
using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class ShopStock
    {
        public ShopStock()
        {
            this.CustomerOrderDetails = new List<CustomerOrderDetail>();
            this.CustomerOrderHistories = new List<CustomerOrderHistory>();
            this.CustomerOrderOfferDetails = new List<CustomerOrderOfferDetail>();
            this.OfferZoneProducts = new List<OfferZoneProduct>();
            this.OfferZoneProducts1 = new List<OfferZoneProduct>();
            this.StockComponents = new List<StockComponent>();
            this.StockComponentOffers = new List<StockComponentOffer>();
            this.StockComponentPriceLogs = new List<StockComponentPriceLog>();
            this.WishLists = new List<WishList>();
            /*14-03-2016
            * Prdnyakar Badge 
            * For Taxation Work
           */
            this.ProductTaxes = new List<ProductTax>();
        }

        public long ID { get; set; }
        public long ShopProductID { get; set; }
        public long ProductVarientID { get; set; }
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public decimal PackSize { get; set; }
        public int PackUnitID { get; set; }
        public decimal MRP { get; set; }
        public Nullable<decimal> WholeSaleRate { get; set; }
        public decimal RetailerRate { get; set; }
        public Nullable<long> WarehouseStockID { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        public decimal BusinessPoints { get; set; } //Added by Zubair on 21-12-2017 for MLM
        public decimal CashbackPoints { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> IsPriority { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual List<CustomerOrderDetail> CustomerOrderDetails { get; set; }
        public virtual List<CustomerOrderHistory> CustomerOrderHistories { get; set; }
        public virtual List<CustomerOrderOfferDetail> CustomerOrderOfferDetails { get; set; }
        public virtual List<OfferZoneProduct> OfferZoneProducts { get; set; }
        public virtual List<OfferZoneProduct> OfferZoneProducts1 { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        /*14-03-2016
            * Prdnyakar Badge 
            * For Taxation Work
           */
        public virtual ICollection<ProductTax> ProductTaxes { get; set; }
        public virtual ProductVarient ProductVarient { get; set; }
        public virtual ShopProduct ShopProduct { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual List<StockComponent> StockComponents { get; set; }
        public virtual List<StockComponentOffer> StockComponentOffers { get; set; }
        public virtual List<StockComponentPriceLog> StockComponentPriceLogs { get; set; }
        public virtual List<WishList> WishLists { get; set; }
        public Offer_Product.OfferType OfferType { get; set; }//Added by Sonali_14-11-2018
    }
}
