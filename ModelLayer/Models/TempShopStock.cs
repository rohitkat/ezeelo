using ModelLayer.Models.Enum;
using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class TempShopStock
    {
        public TempShopStock()
        {
            this.ShopStockBulkLogs = new List<ShopStockBulkLog>();
            /*For Taxtion */
            this.TempProductTaxes = new List<TempProductTax>();
            this.TempStockComponents = new List<TempStockComponent>();
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
        public bool IsInclusiveOfTax { get; set; }
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
        public virtual List<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
        public virtual ICollection<TempProductTax> TempProductTaxes { get; set; }
        public virtual TempProductVarient TempProductVarient { get; set; }
        public virtual TempShopProduct TempShopProduct { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual List<TempStockComponent> TempStockComponents { get; set; }
        public Offer_Product.OfferType OfferType { get; set; }//Added by Sonali_13-11-2018
    }
}
