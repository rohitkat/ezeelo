using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Franchise.Models.ViewModel
{
    public class NewCustomerOrderDetailViewModel
    {
        public long CODID { get; set; }
        public string ShopOrderCode { get; set; }
        public string ReferenceShopOrderCode { get; set; }
        public long CustomerOrderID { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }
        public string ShopContactPerson { get; set; }
        public string ShopContactNo { get; set; }

        public long DeliveryPartnerID { get; set; }
        public string DeliveryPartner { get; set; }
        public string DeliveryContactPerson { get; set; }
        public string DeliveryContactNo { get; set; }
        public string DeliveryType { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal GandhibaghCharge { get; set; }
        public int Qty { get; set; }
        public string OrderStatus { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public decimal OfferPercent { get; set; }
        public decimal OfferRs { get; set; }
        public Boolean IsInclusivOfTax { get; set; }
        public decimal TotalAmount { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<DateTime> ModifyDate { get; set; }
        public long ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public long ProductCategoryID { get; set; }
        public string ProductCategoryName { get; set; }
        public int ProductWeight { get; set; }
        public int ProductLength { get; set; }
        public int ProductBreadth { get; set; }
        public int ProductHeight { get; set; }
        public string ProductDescription { get; set; }
        public long ProductBrandID { get; set; }
        public string ProductBrand { get; set; }
        public Boolean ProductIsActive { get; set; }
        public long ProductColorID { get; set; }
        public string ProductColor { get; set; }
        public long ProductSizeID { get; set; }
        public string ProductSize { get; set; }
        public long ProductDimensionID { get; set; }
        public string ProductDimension { get; set; }
        public long ProductMaterialID { get; set; }
        public string ProductMaterial { get; set; }
        public decimal RetailPoints { get; set; }//Yashaswi 30-7-2018
        public decimal CashbackPoints { get; set; }
    }
}