//-----------------------------------------------------------------------
// <copyright file="ProductStockDetailViewModel" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.ViewModel
{
    //This View Model is used for showing productList on mobile

    public class ProductStockDetailViewModel
    {
        public String StockThumbPath { get; set; }
        public String StockSmallImagePath { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string ShopName { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int DimensionID { get; set; }
        public string DimensionName { get; set; }
        public int MaterialID { get; set; }
        public string MaterialName { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public long CityID { get; set; }
        public long ShopID { get; set; }
        public long ShopStockID { get; set; }
        public decimal PackSize { get; set; }
        public string PackUnit { get; set; }
        public int StockQty { get; set; }
        public bool StockStatus { get; set; }
        public bool IsAddedInWishlist { get; set; }
        public int FranchiseID { get; set; }//Added by sonali for display pincode in wishlist on 18-04-2019
        public string CityName { get; set; }//Added by sonali for display pincode in wishlist on 18-04-2019
        public string Pincode { get; set; }//Added by sonali for display pincode in wishlist on 18-04-2019
        public long PincodeId { get; set; }//Added by sonali for display pincode in wishlist on 18-04-2019
        public List<ProductDescriptionViewModel> ProductDescription { get; set; }
    }

    /*Added By Pradnyakar Badge
         * 13-06-2016
         * For New Product List in Mobile APP
         */
    public class Mobile_ProductStockDetailViewModel
    {
        public long ProductID { get; set; }
        public string ShopName { get; set; }
        public string ColorName { get; set; }
        public String StockThumbPath { get; set; }
        public String StockSmallImagePath { get; set; }
        public string ProductName { get; set; }
        public decimal MRP { get; set; }
        public decimal SaleRate { get; set; }
        public long ShopStockID { get; set; }
        public bool StockStatus { get; set; }
        public List<ProductListVarientForMobileViewModel> VarientList { get; set; }
    }

}
