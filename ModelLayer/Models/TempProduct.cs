using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class TempProduct
    {
        public TempProduct()
        {
            this.ProductBulkDetails = new List<ProductBulkDetail>();
            this.ShopStockBulkLogs = new List<ShopStockBulkLog>();
            this.TempProductSpecifications = new List<TempProductSpecification>();
            this.TempProductVarients = new List<TempProductVarient>();
            this.TempShopProducts = new List<TempShopProduct>();
        }

        public long ID { get; set; }
        public string Name { get; set; }
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
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ProductBulkDetail> ProductBulkDetails { get; set; }
        public virtual List<ShopStockBulkLog> ShopStockBulkLogs { get; set; }
        public virtual List<TempProductSpecification> TempProductSpecifications { get; set; }
        public virtual List<TempProductVarient> TempProductVarients { get; set; }
        public virtual List<TempShopProduct> TempShopProducts { get; set; }
        //priti
        [StringLength(15, MinimumLength = 1, ErrorMessage = "HSNCode must be between 1 and 15 char")]
        public string HSNCode { get; set; } //Added by Zubair for GST on 10-07-2017
        [StringLength(15, MinimumLength = 1, ErrorMessage = "EANCode must be between 1 and 15 char")]
        public string EANCode { get; set; } // Added by Priti for Products on 4-07-2018
    }
}
