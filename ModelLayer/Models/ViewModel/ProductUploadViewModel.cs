using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelLayer.Models.ViewModel
{
    public class ProductUploadViewModelList
    {
        public List<TempProduct> TempProduct { get; set; }
        public List<TempProductVarient> TempProductVarient { get; set; }
        public List<TempShopStock> TempShopStock { get; set; }
        public List<TempShopProduct> TempShopProduct { get; set; }
        public List<Category> Category { get; set; }
        public List<Brand> Brand { get; set; }
    }
    public class ProductUploadViewModel
    {
        public string ShopName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        //--------------------------------------Product table field-------
        //public long ID { get; set; }
        public long ProductID { get; set; }
        //public string Name { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public int WeightInGram { get; set; }
        public int LengthInCm { get; set; }
        public int BreadthInCm { get; set; }
        public int HeightInCm { get; set; }
        public string Description { get; set; }
        public int BrandID { get; set; }
        public string SearchKeyword { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> ApprovalStatus { get; set; }
        public string ApprovalRemark { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductBulkDetail> ProductBulkDetails { get; set; }
        public virtual ICollection<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
        public virtual ICollection<TempProductSpecification> TempProductSpecifications { get; set; }
        public virtual ICollection<TempProductVarient> TempProductVarients { get; set; }
        public virtual ICollection<TempShopProduct> TempShopProducts { get; set; }

        //--------------------------------------ProductVarient table field-------
        public long ProductVarientID { get; set; }
        public long ProductVarientCount { get; set; }
       // public long ProductID { get; set; }
        public int ColorID { get; set; }
        public int SizeID { get; set; }
        public int DimensionID { get; set; }
        public int MaterialID { get; set; }
        //public bool IsActive { get; set; }
        //public System.DateTime CreateDate { get; set; }
        //public long CreateBy { get; set; }
        //public Nullable<System.DateTime> ModifyDate { get; set; }
        //public Nullable<long> ModifyBy { get; set; }
        //public string NetworkIP { get; set; }
        //public string DeviceType { get; set; }
        //public string DeviceID { get; set; }
        public virtual Color Color { get; set; }
        public virtual Dimension Dimension { get; set; }
        public virtual Material Material { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual Size Size { get; set; }
        public virtual TempProduct TempProduct { get; set; }
        public virtual ICollection<TempShopStock> TempShopStocks { get; set; }
        //--------------------------------------ShopStock table field-------
        //public long ID { get; set; }
        public long ShopStockID { get; set; }
        public long ShopProductID { get; set; }
        //public long ProductVarientID { get; set; }
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public decimal PackSize { get; set; }
        public int PackUnitID { get; set; }
        public decimal MRP { get; set; }
        public Nullable<decimal> WholeSaleRate { get; set; }
        public decimal RetailerRate { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "HSNCode must be between 1 and 15 char")]
        
        public string HSNCode { get; set; } //Added by Zubair for GST on 10-07-2017
        [StringLength(15, MinimumLength = 1, ErrorMessage = "EANCode must be between 1 and 15 char")]
        
        public string EANCode { get; set; } //Added by Priti for GST on 11-07-2018
        //public bool IsActive { get; set; }
        //public System.DateTime CreateDate { get; set; }
        //public long CreateBy { get; set; }
        //public Nullable<System.DateTime> ModifyDate { get; set; }
        //public Nullable<long> ModifyBy { get; set; }
        //public string NetworkIP { get; set; }
        //public string DeviceType { get; set; }
        //public string DeviceID { get; set; }
        //public virtual ICollection<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
        public virtual TempProductVarient TempProductVarient { get; set; }
        public virtual TempShopProduct TempShopProduct { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual ICollection<TempStockComponent> TempStockComponents { get; set; }
       // -----------------------------------------------------------------------------TempShopProduct-----------------
        //public long ShopProductID { get; set; }
        public long ShopID { get; set; }
        //public long ProductID { get; set; }
        //public bool IsActive { get; set; }
        public System.DateTime DisplayProductFromDate { get; set; }
        //public System.DateTime CreateDate { get; set; }
        //public long CreateBy { get; set; }
        //public Nullable<System.DateTime> ModifyDate { get; set; }
        //public Nullable<long> ModifyBy { get; set; }
        //public string NetworkIP { get; set; }
        //public string DeviceType { get; set; }
        //public string DeviceID { get; set; }
        public virtual Shop Shop { get; set; }
        //public virtual TempProduct TempProduct { get; set; }
        //public virtual ICollection<TempShopStock> TempShopStocks { get; set; }

        //Priti
       // public string EANCode { get; set; }
    }
}