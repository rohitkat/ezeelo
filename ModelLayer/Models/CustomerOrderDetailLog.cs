using System;
using System.Collections.Generic;

namespace ModelLayer.Models
{
    public partial class CustomerOrderDetailLog
    {
        public long ID { get; set; }
        public string ShopOrderCode { get; set; }
        public string ReferenceShopOrderCode { get; set; }
        public Nullable<long> CustomerOrderID { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public int Qty { get; set; }
        public int OrderStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal OfferRs { get; set; }
        public bool IsInclusivOfTax { get; set; }
        public decimal TotalAmount { get; set; }
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
