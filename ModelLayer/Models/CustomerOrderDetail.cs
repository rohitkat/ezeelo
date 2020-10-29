using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ModelLayer.Models
{
    public partial class CustomerOrderDetail
    {
        public CustomerOrderDetail()
        {
            this.CorporateCustomerShippingDeliveryDetails = new List<CorporateCustomerShippingDeliveryDetail>();
            this.CorporateOrderShippingFacilityDetails = new List<CorporateOrderShippingFacilityDetail>();
            this.CustomerOrderOfferDetails = new List<CustomerOrderOfferDetail>();
            this.GandhibaghTransactions = new List<GandhibaghTransaction>();
            /*14-03-2016
             * Prdnyakar Badge 
             * For Taxation Work
            */
            this.TaxOnOrders = new List<TaxOnOrder>();
        }
        public long ID { get; set; }
        public string ShopOrderCode { get; set; }
        public string ReferenceShopOrderCode { get; set; }
        public long CustomerOrderID { get; set; }
        public long ShopStockID { get; set; }
        public Nullable<long> WarehouseStockID { get; set; } //Added by Zubair for Inventory on 28-03-2018
        public long ShopID { get; set; }

        [Range(1, int.MaxValue)]
        public int Qty { get; set; }
        public int OrderStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal OfferRs { get; set; }
        public bool IsInclusivOfTax { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable<decimal> BusinessPointPerUnit { get; set; } //Added by Zubair for MLM on 06-04-2018
        public Nullable<decimal> BusinessPoints { get; set; } //Added by Zubair for MLM on 06-04-2018
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual ShopStock ShopStock { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual List<CustomerOrderOfferDetail> CustomerOrderOfferDetails { get; set; }
        public virtual List<GandhibaghTransaction> GandhibaghTransactions { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        //Added by yashaswi
        public int? CurrentShopStockQty { get; set; }
        public int? CurrentWarehouseStockQty { get; set; }
        public decimal CashbackPointPerUnit { get; set; }
        public decimal CashbackPoints { get; set; }

        //--------------- Added by mohit on 07-10-2015-------------------------//
        public virtual ICollection<CorporateCustomerShippingDeliveryDetail> CorporateCustomerShippingDeliveryDetails { get; set; }
        public virtual ICollection<CorporateOrderShippingFacilityDetail> CorporateOrderShippingFacilityDetails { get; set; }

        /*14-03-2016
         * Prdnyakar Badge 
         * For Taxation Work
        */
        public virtual ICollection<TaxOnOrder> TaxOnOrders { get; set; }
    }
}
