using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public partial class Product
    {
        public Product()
        {
            this.FrequentlyBuyTogetherProducts = new List<FrequentlyBuyTogetherProduct>();
            this.FrequentlyBuyTogetherProducts1 = new List<FrequentlyBuyTogetherProduct>();
            this.ProductSpecifications = new List<ProductSpecification>();
            this.ShopProducts = new List<ShopProduct>();
            this.ShopProductCharges = new List<ShopProductCharge>();
            this.ProprietoryProducts = new List<ProprietoryProduct>();
            this.DynamicCategoryProducts = new List<DynamicCategoryProduct>();
            this.BlockItemsLists = new List<BlockItemsList>();
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
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "HSNCode must be between 1 and 15 char")]
        public string HSNCode { get; set; } //Added by Zubair for GST on 10-07-2017
        [StringLength(15, MinimumLength = 1, ErrorMessage = "EANCode must be between 1 and 15 char")]
        public string EANCode { get; set; } // Added by Priti for Products on 4-07-2018
        public virtual List<FrequentlyBuyTogetherProduct> FrequentlyBuyTogetherProducts { get; set; }
        public virtual List<FrequentlyBuyTogetherProduct> FrequentlyBuyTogetherProducts1 { get; set; }
        public virtual PersonalDetail PersonalDetail { get; set; }
        public virtual PersonalDetail PersonalDetail1 { get; set; }
        public virtual List<ProductSpecification> ProductSpecifications { get; set; }
        public virtual List<ShopProduct> ShopProducts { get; set; }
        public virtual List<ShopProductCharge> ShopProductCharges { get; set; }
        public virtual List<ProprietoryProduct> ProprietoryProducts { get; set; }
        public virtual List<DynamicCategoryProduct> DynamicCategoryProducts { get; set; }
        public virtual List<BlockItemsList> BlockItemsLists { get; set; }

    }
}
