using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    

    public class ProductTaxViewModel
    {
        public long ID { get; set; }
        public long ShopStockID { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Dimension { get; set; }
        public string Material { get; set; }
        public int TaxID { get; set; }
        public bool IsActive { get; set; }
       

    }
    public class ProductTaxIndexView
    {
        public List<ProductVarientByCategoryAndShop> ProductVarientByCategoryAndShop { get; set; }
        public int TaxMasterID { get; set; }
    }

    public class TempShopStockAndTax
    {
        public Int64 ShopStockID { get; set; }
        public int TaxID { get; set; }
    }

    public class ProductVarientByCategoryAndShop
    {
        public Int64 ProductTaxID { get; set; }
        public Int64 ShopStockID { get; set; }
        public Int64 ShopID { get; set; }
        public string ShopName { get; set; }
        public Int64 ProductID { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public int ReorderLevel { get; set; }
        public bool StockStatus { get; set; }
        public int PackUnitID { get; set; }
        public string UnitName { get; set; }
        public decimal PackSize { get; set; }
        public decimal MRP { get; set; }
        public decimal RetailerRate { get; set; }
        public bool IsInclusiveOfTax { get; set; }
        public int QTY { get; set; }
        public bool isSelected { get; set; }
    }

    public class CalulatedTaxesRecord
    {
        public Int64 ShopStockID { get; set; }
        public Int64 ProductTaxID { get; set; }
        public string TaxName { get; set; }
        public string TaxPrefix { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxableAmount { get; set; }
        public string Remarks { get; set; }

        public bool IsGSTInclusive { get; set; }   //Added by Zubair on 05-07-2017 for GST
         
    }

    public class DropdownList
    {
        public long ID { get; set; }
        public string Name { get; set; }
        
    }

    public class ProductTaxDetail
    {
        public Int64 ID { get; set; }
        public Int64 ShopstockID { get; set; }
        public int? FranchiseID { get; set; }
        public string FranmchiseName { get; set; }
        public Int64 shopID { get; set; }
        public string ShopName { get; set; }
        public int TaxID { get; set; }
        public string TaxName { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
    }

    public class IndirectTaxSetting
    {

        public int ID { get; set; }
        public int FranchiseID { get; set; }
        public string FranmchiseName { get; set; }
        public string TaxName { get; set; }
        public string DependTaxName { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxableAmount { get; set; }
        public bool IsOnTaxSum { get; set; }
        public bool IsIncludeSaleRate { get; set; }
    }


    public class CalculatedTaxList
    {
        public string TaxName { get; set; }
        public decimal Amount { get; set; }

        public string TaxPrefix { get; set; }
        public long shopStockID { get; set; }

        public bool IsGSTInclusive { get; set; }  // Added by Zubair for GST on 06-07-2017
    }


}
