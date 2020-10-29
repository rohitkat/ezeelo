using ModelLayer.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModelLayer.Models.ViewModel
{
    public class ProductUploadTempViewModelList
    {
        public List<ProductUploadTempViewModel> ProductUploadTempViewModelLIst { get; set; }
        public List<TempProductVarient> TempProductVarientList { get; set; }
        public List<TempShopStock> TempShopStockList { get; set; }
    }
    public class ProductUploadTempViewModel
    {
        public string ImageLocation { get; set; }
        //Temp Product
        public long ID { get; set; }
        [Required]
        public string Name { get; set; }
        //[Required]
        public string ProductName { get; set; }
        public long ProductID { get; set; }
        public int CategoryID { get; set; }
        public int WeightInGram { get; set; }
        public int LengthInCm { get; set; }
        public int BreadthInCm { get; set; }
        public int HeightInCm { get; set; }
        public string Description { get; set; }
        public int BrandID { get; set; }
        [Required]
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

        //Temp Product Variant
        public long ProductVarientID { get; set; }
        public long ProductVarientCount { get; set; }

        public int ColorID { get; set; }
        public int SizeID { get; set; }
        public int DimensionID { get; set; }
        public int MaterialID { get; set; }

        //Temp Shop Product
        public long TempShopProductID { get; set; }
        public long ShopID { get; set; }
        public string ShopName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DisplayProductFromDate { get; set; }
        public long? DeliveryTime { get; set; }
        public Nullable<decimal> DeliveryRate { get; set; }
        public Nullable<decimal> TaxRate { get; set; }
        public Nullable<decimal> TaxRatePer { get; set; }

        //Temp Shop Stock
        public long ShopStockID { get; set; }
        public long ShopProductID { get; set; }
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public decimal PackSize { get; set; }
        public int PackUnitID { get; set; }
        public decimal MRP { get; set; }
        public decimal WholeSaleRate { get; set; }
        public decimal RetailerRate { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        //Category
        public string CategoryName { get; set; }
        public int CategoryL_0 { get; set; }
        public int CategoryL_1 { get; set; }
        public int CategoryL_2 { get; set; }
        public int ddlCategorySecondID { get; set; }
        public int ddlCategoryFirstID { get; set; }
        //Brand
        public string BrandName { get; set; }
        //TempStockComponent
        public long TempStockComponentID { get; set; }
        public int ComponentID { get; set; }
        public decimal ComponentWeight { get; set; }
        public int ComponentUnitID { get; set; }
        //Color
        public string ColorName { get; set; }
        //Size
        public string SizeName { get; set; }
        //Diamention
        public string DiamentionName { get; set; }
        //Material
        public string MaterialName { get; set; }

        //Units
        public string UnitName { get; set; }
        //States
        public int stateID { get; set; }
        public string StateName { get; set; }
        public List<NewProductVarient> NewProductVarientS { get; set; }
        public List<NewProductVarient> NewProductVarientPOP { get; set; }
        // public List<Size> SizeDropDown { get; set; }
        public string[] Path { get; set; }
        public int pathValue { get; set; }
        //Product tax
        public Nullable<int> TaxationID { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "HSNCode must be between 1 and 15 char")]
        public string HSNCode { get; set; } //Added by Zubair for GST on 10-07-2017
        [StringLength(15, MinimumLength = 1, ErrorMessage = "EANCode must be between 1 and 15 char")]
        public string EANCode { get; set; } // Added by Priti for Products on 4-07-2018
    }

    public class NewProductVarient
    {
        public long ID { get; set; }
        public long ProductVarientID { get; set; }
        public int ColorID { get; set; }
        public int SizeID { get; set; }
        public int DimensionID { get; set; }
        public int MaterialID { get; set; }
        public long ShopStockID { get; set; }
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
        public decimal PackSize { get; set; }
        public int PackUnitID { get; set; }
        public decimal MRP { get; set; }
        public Nullable<decimal> WholeSaleRate { get; set; }

        public decimal RetailerRate { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        public string ColorName { get; set; }
        public bool IsSelect { get; set; }
        public bool IsActive { get; set; }
        public bool SS_IsActive { get; set; }
        public Nullable<int> IsPriority { get; set; }
        public string[] Path { get; set; }
        public string ThumbPath { get; set; }
        public int pathValue { get; set; }
      //  public Nullable<int> TaxationID { get; set; }
        public List<int> TaxationID { get; set; }

        public bool IsInclusive { get; set; } //Added By Zubair on 04-07-2017
        public decimal BusinessPoints { get; set; }
        public decimal CashbackPoints { get; set; }
        public Offer_Product.OfferType OfferType { get; set; }//Added by Sonali_13-11-2018

    }

    // added by prashant for capturing the product specification in temporray table
    public class CategorySpecificationList
    {
        public int SpecificationID { get; set; }
        public int? ParentID { get; set; }
        public string SpecificationName { get; set; }
        public string SpecificationValue { get; set; }
        public int level { get; set; }
    }


}
