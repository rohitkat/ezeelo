//-----------------------------------------------------------------------
// <copyright file="ProductList.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using ModelLayer.Models;
/*
 Handed over to Mohit, Tejaswee
 */
namespace BusinessLogicLayer
{
    public class ProductList : ProductDisplay
    {
        public ProductList(System.Web.HttpServerUtility server) : base(server) { }

        public int gSortType { get; set; }

        private EzeeloDBContext db = new EzeeloDBContext();

        /// <summary>
        /// Get list of products for the searching criteria given by user
        /// This method is used web Customer module only
        ///  Sort type 0 :- Sale Rate Asending
        ///// 1 :- Sale Rate desending
        ///// 2 :- Alphabate Asending
        ///// 3 :- Alphabate desending
        ///// 4 :- Datewise  ascending
        ///// 5 :- Datewise  descending
        /// </summary>
        /// <param name="productSearch">Product Search object</param>
        /// <returns>List of provided page size(say 12) no of product list</returns>
        public ProductWithRefinementViewModel GetProductList(ProductSearchViewModel productSearch, bool IsMobile) //Last Param Added by Sonali
        {

            ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();

            //Get Dataset containing page size(say 12) no of product list in first table,
            //Datable all bulky data required for refinements & 
            //Total Page Size, and product count

            DataSet ds = GetAllProducts(productSearch, false);
            List<SearchProductDetailsViewModel> lproductList = new List<SearchProductDetailsViewModel>();

            //collect product list from page size(say 12) no of product list in first table 
            lproductList = (from DataRow dr in ds.Tables[0].Rows
                            select new SearchProductDetailsViewModel()
                            {
                                ProductThumbPath = string.Empty,
                                ProductID = Convert.ToInt32(dr["ProductID"]),
                                Name = CommonFunctions.ConvertStringToCamelCase(dr["ProductName"].ToString()),
                                CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                CategoryName = CommonFunctions.ConvertStringToCamelCase(dr["CategoryName"].ToString()),//Sonali_04-12-2018
                                MRP = Convert.ToDecimal(dr["MRP"]),
                                SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                StockStatus = Convert.ToInt32(dr["StockStatus"]),
                                ShopStockID = Convert.ToInt32(dr["ShopStockID"]),
                                Color = dr["ColorName"].ToString(),
                                PackSize = Convert.ToDecimal(dr["Packsize"]),
                                PackUnit = dr["PackUnit"].ToString(),
                                Dimension = dr["DimensionName"].ToString(),
                                Size = dr["SizeName"].ToString(),
                                Material = dr["MaterialName"].ToString(),
                                StockQty = Convert.ToInt32(dr["StockQty"]),
                                HtmlColorCode = dr["ColorHtmlCode"].ToString(),
                                RetailPoint = Convert.ToDecimal(dr["RetailPoint"]), //Yashaswi 10-7-2018
                                CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"])
                            }).ToList();

            //- Start Changes made by Avi Verma. Date : 07-june-2016
            //- Reason : Product Varients included in listing page.
            List<PrdVarientViewModel> lPrdVarientViewModels = (from DataRow dr in ds.Tables[1].Rows
                                                               select new PrdVarientViewModel()
                                                               {
                                                                   ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                                                   ProductID = Convert.ToInt64(dr["ProductID"]),
                                                                   CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                                                   ShopID = Convert.ToInt32(dr["ShopID"]),
                                                                   SizeID = Convert.ToInt32(dr["SizeID"]),
                                                                   Size = Convert.ToString(dr["Size"]),
                                                                   MRP = Convert.ToDecimal(dr["MRP"]),
                                                                   SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                                                   RetailPoint = Convert.ToDecimal(dr["RetailPoint"]), //Yashaswi 10-7-2018
                                                                   StockStatus = Convert.ToInt32(dr["StockStatus"]),  //Yashaswi 25-7-2018
                                                                   StockQty = Convert.ToInt32(dr["StockQty"]),  //Yashaswi 25-7-2018
                                                                   BrandId = Convert.ToInt64(dr["BrandID"]),//Added for api sort by Sonali_04-01-2018
                                                                   BrandName = CommonFunctions.ConvertStringToCamelCase(dr["BrandName"].ToString()),//Added for api sort by Sonali_04-01-2018
                                                                   CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                                                   IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"])
                                                               }).ToList();
            lPrdVarientViewModels = (from pvvm in lPrdVarientViewModels
                                     join shpPriority in db.ShopPriorities on pvvm.CategoryID equals shpPriority.CategoryID
                                     where pvvm.SizeID != 1 // "N/A" 
                                     group pvvm by pvvm.ShopStockID
                                         into grps
                                     select new PrdVarientViewModel
                                     {
                                         ShopStockID = grps.Key,
                                         ProductID = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).ProductID,
                                         ShopID = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).ShopID,
                                         Size = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).Size,
                                         MRP = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).MRP,
                                         SaleRate = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).SaleRate,
                                         RetailPoint = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).RetailPoint,  //Yashaswi 10-7-2018 
                                         CashbackPoint = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).CashbackPoint,
                                         IsDisplayCB = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).IsDisplayCB,
                                         StockStatus = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).StockStatus,  //Yashaswi 25-7-2018
                                         StockQty = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).StockQty,  //Yashaswi 25-7-2018
                                         BrandId = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).BrandId,//Added for api sort by Sonali_04-01-2018
                                         BrandName = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).BrandName,//Added for api sort by Sonali_04-01-2018
                                     }).OrderBy(x => x.Size).ThenByDescending(x => x.StockQty).ToList();//Added orderbydescending via stockQty by sonali on 18-04-2019

            foreach (var item in lproductList)
            {
                PrdVarientViewModel lPrdVarientViewModel = lPrdVarientViewModels.FirstOrDefault(x => x.ShopStockID == item.ShopStockID);
                if (lPrdVarientViewModel != null)
                {
                    item.ShopID = lPrdVarientViewModel.ShopID;
                    item.BrandId = lPrdVarientViewModel.BrandId;//Added for api sort by Sonali_04-01-2018
                    item.BrandName = lPrdVarientViewModel.BrandName;//Added for api sort by Sonali_04-01-2018
                }
            }
            //- End Changes made by Avi Verma. Date : 07-june-2016

            //get product thumbnail
            foreach (var item in lproductList)
            {
                //if (item.Color != null && item.Color != "N/A")
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.Color, string.Empty, ProductUpload.THUMB_TYPE.SD);
                //else
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                //==============================================================//
                //Tejaswee (5/11/2015)
                //call different function for product thumbnail
                //==============================================================//
                if (item.Color != null && item.Color != "N/A")
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.Color, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                else
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);


                //productuploadtempviewmodel.CategoryID = TP.CategoryID;
                var obj = (from n in db.Categories
                           join m in db.Categories on n.ID equals m.ParentCategoryID
                           join p in db.Categories on m.ID equals p.ParentCategoryID
                           where p.ID == item.CategoryID
                           select new
                           {
                               LevelOne = n.ID
                           }).FirstOrDefault();

                item.FirstLevelCatId = Convert.ToInt32(obj.LevelOne);

                //-- Added by Avi Verma. For Dropdownlist
                if (lPrdVarientViewModels.Where(x => x.ProductID == item.ProductID && x.ShopID == item.ShopID).ToList().Count > 0)
                {
                    item.ProductVarientViewModels = lPrdVarientViewModels.Where(x => x.ProductID == item.ProductID && x.ShopID == item.ShopID).OrderByDescending(x => x.StockQty).ToList();
                    item.MRP = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).OrderByDescending(x => x.StockQty).FirstOrDefault().MRP;
                    item.SaleRate = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).OrderByDescending(x => x.StockQty).FirstOrDefault().SaleRate;
                    item.ShopStockID = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).OrderByDescending(x => x.StockQty).FirstOrDefault().ShopStockID;
                    item.Size = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).OrderByDescending(x => x.StockQty).FirstOrDefault().Size;
                }

                //Added for SEO ULR Structure RULE by AShish
                item.URLStructureName = GetURLStructureName(item.Name);
                item.Name = item.Name.Replace("+", " ");

            }
            //add the list to the object to be returned
            productWithRefinementViewModel.productList = lproductList;

            //Collect refinements if page index is one, as we don't need it on scrolling 
            //refinement data will remain same
            if (productSearch.PageIndex == 1)
            {

                if (ds.Tables[1] != null)
                {
                    List<ProductRefinementsViewModel> lproductRefinements = new List<ProductRefinementsViewModel>();

                    lproductRefinements = (from DataRow dr in ds.Tables[1].Rows
                                           select new ProductRefinementsViewModel()
                                           {

                                               BrandID = Convert.ToInt32(dr["BrandID"]),
                                               BrandName = dr["BrandName"].ToString(),
                                               CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                               CategoryName = dr["CategoryName"].ToString(),
                                               // CityID = Convert.ToInt32(dr["CityID"]),////hide
                                               ColorID = Convert.ToInt32(dr["ColorID"]),
                                               Color = dr["Color"].ToString(),
                                               DimensionID = Convert.ToInt32(dr["DimensionID"]),
                                               Dimension = dr["Dimension"].ToString(),
                                               MaterialID = Convert.ToInt32(dr["MaterialID"]),
                                               Material = dr["Material"].ToString(),
                                               MRP = Convert.ToDecimal(dr["MRP"]),
                                               ProductID = Convert.ToInt64(dr["ProductID"]),
                                               ProductName = dr["ProductName"].ToString(),
                                               SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                               ShopID = Convert.ToInt64(dr["ShopID"]),
                                               ShopName = dr["ShopName"].ToString(),
                                               SizeID = Convert.ToInt32(dr["SizeID"]),
                                               Size = dr["Size"].ToString(),
                                               SpecificationID = Convert.ToInt32(dr["SpecificationID"]),
                                               SpecificationValue = dr["SpecificationValue"].ToString(),
                                               ProductSpecificationID = Convert.ToInt32(dr["ProductSpecificationID"]),
                                               SpecificationName = dr["SpecificationName"].ToString(),
                                               PackSize = Convert.ToDecimal(dr["Packsize"]),
                                               PackUnit = dr["PackUnit"].ToString(),
                                               CategoryOrderSequence = Convert.ToInt32(dr["ShopMenuOrder"]),
                                               FranchiseID = Convert.ToInt32(dr["FranchiseID"]),////added
                                               RetailPoint = Convert.ToDecimal(dr["RetailPoint"]), //Yashaswi 10-7-2018
                                               CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                               IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"])
                                           }).ToList();
                    productWithRefinementViewModel.productRefinements = lproductRefinements;
                }
                //collect page size and product count
                if (ds.Tables[2] != null)
                {
                    SearchCountViewModel searchCount = new SearchCountViewModel();
                    searchCount.PageCount = Convert.ToInt32(ds.Tables[2].Rows[0]["PageCount"]);
                    searchCount.ProductCount = Convert.ToInt32(ds.Tables[2].Rows[0]["Productcount"]);

                    productWithRefinementViewModel.searchCount = searchCount;

                }

            }
            //collect page size and product count
            else if (ds.Tables[2] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(ds.Tables[2].Rows[0]["PageCount"]);
                searchCount.ProductCount = Convert.ToInt32(ds.Tables[2].Rows[0]["Productcount"]);

                productWithRefinementViewModel.searchCount = searchCount;

            }

            return productWithRefinementViewModel;

        }
        /// <summary>
        /// Added for ULR Structure RULE by AShish
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetURLStructureName(string Name)
        {
            string str = Name;
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\\\#$~%.':*?<>{} ]", " ").Replace("&", "and");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+/g", " ");
            // string[] parts2 = Regex.Split(str, @"\+\/\-\,\(\)");
            ///////////////////
            string concat = "";
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\/\+\-\,()]", "|");
            string[] strSplit = str.Split('|');
            for (int i = 0; i < strSplit.Length; i++)
            {
                if (concat.Length <= 30)
                {
                    concat = concat.Length == 0 ? strSplit[i].Trim() : concat + ' ' + strSplit[i].Trim();
                }
            }

            //////////////////
            /* string remaining = "";
             string concat = "";
             string[] strSplit = str.Split('+');
             if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
             {
                 if (concat.Length + strSplit[0].Length + 1 <= 30)
                 { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }
             }


             if (strSplit.Length >= 2)
             { remaining = strSplit[1]; }
             else { remaining = strSplit[0].Trim(); }
             strSplit = null;
             strSplit = remaining.Split('/');
             if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
             {
                 if (concat.Length + strSplit[0].Length + 1 <= 30)
                 { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }
             }
             if (concat.Length <= 30)
             {
                 remaining = "";
                 if (strSplit.Length >= 2)
                 { remaining = strSplit[1]; }
                 else { remaining = strSplit[0].Trim(); }

                 strSplit = null;
                 strSplit = remaining.Split('-');
                 if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                 {
                     if (concat.Length + strSplit[0].Length + 1 <= 30)
                     { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }

                 }
                 if (concat.Length <= 30)
                 {
                     remaining = "";
                     if (strSplit.Length >= 2)
                     { remaining = strSplit[1]; }
                     else { remaining = strSplit[0].Trim(); }

                     strSplit = null;
                     strSplit = remaining.Split(',');
                     if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                     {
                         if (concat.Length + strSplit[0].Length + 1 <= 30)
                         { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }

                     }
                     if (concat.Length <= 30)
                     {
                         remaining = "";
                         if (strSplit.Length >= 2)
                         { remaining = strSplit[1]; }
                         else { remaining = strSplit[0].Trim(); }

                         strSplit = null;
                         strSplit = remaining.Split('(');
                         if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                         {
                             if (concat.Length + strSplit[0].Length + 1 <= 30)
                             { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }

                         }
                         if (concat.Length <= 30)
                         {
                             remaining = "";
                             if (strSplit.Length >= 2)
                             { remaining = strSplit[1]; }
                             else { remaining = strSplit[0].Trim(); }

                             strSplit = null;
                             strSplit = remaining.Split(')');
                             if (!String.IsNullOrEmpty(strSplit[0]) && strSplit.Length >= 2)
                             {
                                 if (concat.Length + strSplit[0].Length + 1 <= 30)
                                 { concat = concat.Length == 0 ? strSplit[0].Trim() : concat + ' ' + strSplit[0].Trim(); }
                             }
                             if (concat.Length <= 30)
                             {
                                 ///////////
                                 if (strSplit.Length >= 2)
                                 { remaining = strSplit[1]; }
                                 else { remaining = strSplit[0].Trim(); }
                                 ///////////////
                                 if (concat == "" || remaining != "")
                                 {
                                     if (concat == "")
                                     { concat = remaining.Trim(); }
                                     else
                                     {
                                         remaining = System.Text.RegularExpressions.Regex.Replace(remaining, @"\s+/g", " ").Trim();
                                         concat = concat.Trim() + ' ' + System.Text.RegularExpressions.Regex.Replace(remaining, @"[\/\\#,+()$~%.':*?<>{} ]", " ").Replace("&", "and").Trim(); //remaining.Trim(); 
                                     }
                                 }
                             }

                         }
                     }
                 }

             }*/


            // concat = concat.Replace(" ", "-");//working
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"\s+", " ");
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and");
            concat = concat.Trim(new[] { '-' });
            ////////////
            //string test = concat.Substring(0, 1);

            //if (test == "-")
            //{ concat = concat.Substring(1, concat.Length); }
            //var test2 = concat[concat.Length - 1];
            //if (test2 == '-')
            //{ concat = concat.Substring(0, concat.Length - 1); }
            ////////////
            return concat;
        }
        /// <summary>
        /// Get list of products for the searching criteria given by user
        /// This method is used web API only
        /// </summary>
        /// <param name="productSearch">Product Search object</param>
        /// <returns>List of provided page size(say 12) no of product list</returns>
        public ProductStockVarientViewModel GetProductStockList(ProductSearchViewModel productSearch)
        {
            ProductStockVarientViewModel lProducts = new ProductStockVarientViewModel();

            //Get Dataset containing page size(say 12) no of product list in first table,
            //Datable all bulky data required for refinements & 
            //Total Page Size, and product count
            DataSet ds = GetAllProducts(productSearch, true);

            //collect product list from page size(say 12) no of product list in first table 
            if (ds.Tables[0] != null)
            {


                lProducts.ProductInfo = (from DataRow dr in ds.Tables[0].Rows
                                             // orderby Convert.ToDecimal(dr["SaleRate"]) ascending //added by mohit ----as requirment was to show lowest sail rate first(requirment by sumit)
                                         select new ProductStockDetailViewModel()
                                         {

                                             ProductID = Convert.ToInt64(dr["ProductID"]),
                                             ProductName = dr["ProductName"].ToString().Replace("+", " "), //Added for SEO URL Structure RULE by AShish,
                                             BrandID = Convert.ToInt32(dr["BrandID"]),
                                             BrandName = dr["BrandName"].ToString(),
                                             CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                             CategoryName = dr["CategoryName"].ToString(),
                                             //CityID = Convert.ToInt32(dr["CityID"]),////hide
                                             ColorID = Convert.ToInt32(dr["ColorID"]),
                                             ColorName = dr["Color"].ToString(),
                                             ColorCode = dr["HtmlCode"].ToString(),
                                             DimensionID = Convert.ToInt32(dr["DimensionID"]),
                                             DimensionName = dr["Dimension"].ToString(),
                                             MaterialID = Convert.ToInt32(dr["MaterialID"]),
                                             MaterialName = dr["Material"].ToString(),
                                             ShopID = Convert.ToInt64(dr["ShopID"]),
                                             ShopName = dr["ShopName"].ToString(),
                                             SizeID = Convert.ToInt32(dr["SizeID"]),
                                             SizeName = dr["Size"].ToString(),
                                             ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                             ShortDescription = dr["ProductDescription"].ToString(),
                                             MRP = Convert.ToDecimal(dr["MRP"]),
                                             SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                             IsAddedInWishlist = false,
                                             PackSize = Convert.ToDecimal(dr["Packsize"]),
                                             PackUnit = dr["PackUnit"].ToString(),
                                             StockQty = Convert.ToInt32(dr["StockQty"].ToString()),
                                             StockStatus = Convert.ToBoolean(dr["StockStatus"]),
                                             FranchiseID = Convert.ToInt32(dr["FranchiseID"])////added

                                         }).ToList();


                //Load product Specification details
                foreach (var item in lProducts.ProductInfo)
                {
                    item.ProductDescription = (
                        from ps in db.ProductSpecifications
                        join s in db.Specifications on ps.SpecificationID equals s.ID
                        where ps.ProductID == item.ProductID && ps.IsActive == true && s.IsActive == true
                        select new ProductDescriptionViewModel()
                        {
                            ProductID = ps.ProductID,
                            ProductSpecificationID = ps.ID,
                            SpecificationID = s.ID,
                            SpecificationName = s.Name,
                            SpecificationValue = ps.Value
                        }).ToList();

                    //get MM size product image; it displays in product listing
                    //item.StockThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                    //    string.Empty, ProductUpload.THUMB_TYPE.MM);

                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                        string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    //get ll size product image

                    if (productSearch.ImageType == "SD")
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                       string.Empty, ProductUpload.THUMB_TYPE.SD);
                    else if (productSearch.ImageType == "SS")
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                       string.Empty, ProductUpload.THUMB_TYPE.SS);
                    else if (productSearch.ImageType == "MM")
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                       string.Empty, ProductUpload.THUMB_TYPE.MM);
                    else
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                           string.Empty, ProductUpload.THUMB_TYPE.LL);

                    //Check whether product is added in provided in customer's wishlist
                    if (productSearch.CustLoginID != null && productSearch.CustLoginID > 0)
                    {
                        var lContainsWishlist = db.WishLists.Where(x => x.UserLoginID == productSearch.CustLoginID && x.ShopStockID == item.ShopStockID).ToList();
                        if (lContainsWishlist.Count > 0)
                            item.IsAddedInWishlist = true;
                    }
                }
            }
            //Collect page size and product count information
            if (ds.Tables[1] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(ds.Tables[1].Rows[0]["PageCount"]);
                searchCount.ProductCount = Convert.ToInt32(ds.Tables[1].Rows[0]["Productcount"]);

                lProducts.searchCount = searchCount;

            }

            return lProducts;

        }


        // Get Dataset containing page size(say 12) no of product list in first table,
        // Datable all bulky data required for refinements & 
        // Total Page Size, and product count
        //This method is used in API as well as Customer Module
        /// </summary>
        /// <param name="productSearch">product search object like category, Brand, SearchKeyword etc.</param>
        /// <param name="forMobile">is for Android APP</param>
        /// <returns></returns>
        private DataSet GetAllProducts(ProductSearchViewModel productSearch, bool forMobile)
        {
            DataSet ds = new DataSet();
            try
            {
                //Yashaswi 27-9-2018
                AutoSearch objAutoSearch = new AutoSearch();
                string Syllable = objAutoSearch.GetSyllables(productSearch.Keyword);
                string result;
                string result1;
                string result2;
                string resultSound;
                objAutoSearch.GetSerachChar(Syllable, out result, out result1, out result2, out resultSound);
                //Yashaswi 27-9-2018

                string query = string.Empty;
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);

                if (productSearch.PageIndex == 1 && !productSearch.IsScroll && !forMobile)
                    query = "[SEARCH_PRODUCTS_WITH_REFINEMENTS1]";
                // query = "[Search_Products_With_Refinement]";  
                else if ((productSearch.PageIndex > 1 && productSearch.IsScroll && !forMobile) || (productSearch.PageIndex == 1 && productSearch.IsScroll && !forMobile))
                    query = "[SEARCH_PRODUCTS_WITH_REFINEMENTS_FOR_PAGEINDEX1]";
                //query = "[Search_Products_With_Refinements_For_PageIndex]";  
                else if (forMobile)
                    query = "[Search_Products]";

                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;

                //Yashaswi 27-9-2018
                cmd.Parameters.AddWithValue("@SearchPar1", result);
                cmd.Parameters.AddWithValue("@SearchPar2", result1);
                cmd.Parameters.AddWithValue("@SearchPar3", result2);
                cmd.Parameters.AddWithValue("@SearchPar4", resultSound);
                //Yashaswi 27-9-2018

                cmd.Parameters.AddWithValue("@CityID", productSearch.CityID);/////hide
                cmd.Parameters.AddWithValue("@FranchiseID", productSearch.FranchiseID);////added
                cmd.Parameters.AddWithValue("@ShopID", productSearch.ShopID);
                cmd.Parameters.AddWithValue("@Keyword", productSearch.Keyword);
                cmd.Parameters.AddWithValue("@CategoryID", productSearch.CategoryID);
                if (forMobile)
                {
                    cmd.Parameters.AddWithValue("@ProductID", productSearch.ProductID);
                    cmd.Parameters.AddWithValue("@BrandIDS", productSearch.BrandIDs);
                    cmd.Parameters.AddWithValue("@ColorIDs", productSearch.ColorIDs);
                    cmd.Parameters.AddWithValue("@SizeIDs", productSearch.SizeIDs);
                    cmd.Parameters.AddWithValue("@DimensionIDs", productSearch.DimensionIDs);
                    cmd.Parameters.AddWithValue("@SpecificationIDs", productSearch.SpecificationIDs);
                    cmd.Parameters.AddWithValue("@SpecificationValues", productSearch.SpecificationValues);

                    cmd.Parameters.AddWithValue("@MinPrice", productSearch.MinPrice);
                    cmd.Parameters.AddWithValue("@MaxPrice", productSearch.MaxPrice);
                }
                if ((productSearch.PageIndex > 1 && !forMobile) || (productSearch.PageIndex == 1 && productSearch.IsScroll && !forMobile))
                {
                    cmd.Parameters.AddWithValue("@BrandIDS", productSearch.BrandIDs);
                    cmd.Parameters.AddWithValue("@ColorIDs", productSearch.ColorIDs);
                    cmd.Parameters.AddWithValue("@SizeIDs", productSearch.SizeIDs);
                    cmd.Parameters.AddWithValue("@DimensionIDs", productSearch.DimensionIDs);
                    cmd.Parameters.AddWithValue("@SpecificationIDs", productSearch.SpecificationIDs);
                    cmd.Parameters.AddWithValue("@SpecificationValues", productSearch.SpecificationValues);

                    cmd.Parameters.AddWithValue("@MinPrice", productSearch.MinPrice);
                    cmd.Parameters.AddWithValue("@MaxPrice", productSearch.MaxPrice);
                }
                cmd.Parameters.AddWithValue("@PageIndex", productSearch.PageIndex);
                if (forMobile)
                {
                    //this parameter is added for android app only
                    cmd.Parameters.AddWithValue("@SearchInCategoryOnly", productSearch.SearchInCategoryOnly);
                }
                cmd.Parameters.AddWithValue("@PageSize", productSearch.PageSize);
                cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Productcount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                if (forMobile)
                {
                    // for sorting accordingly 
                    /* 28-01-2016
                     * Pradnyakar Badge
                     */
                    if (productSearch.SortType != null)
                    {
                        cmd.Parameters.AddWithValue("@SortType", productSearch.SortType);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SortType", 0);
                    }

                    cmd.Parameters.AddWithValue("@IsVarientRestricted", productSearch.IsVarientRestricted);
                }
                else
                {
                    //this parameter is added for WEB only
                    cmd.Parameters.AddWithValue("@ShopStockList", productSearch.ShopStockIDList);
                    cmd.Parameters.AddWithValue("@SortType", productSearch.SortVal);
                }

                using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                    }
                }
                //Collect page size and product count information from out put variables of Stored Procedure.
                DataTable dt = new DataTable("SeachCount");
                dt.Columns.Add("PageCount", typeof(int));
                dt.Columns.Add("ProductCount", typeof(int));
                dt.Rows.Add(cmd.Parameters["@PageCount"].Value, cmd.Parameters["@Productcount"].Value);
                ds.Tables.Add(dt);

            }
            catch (Exception ex)
            {

            }
            //return three tables in dataset
            return ds;
        }

        /// <summary>
        /// Product Varients and other detail of given Product id in city
        /// for Product Preview page in mobile application
        /// Consume in API
        /// </summary>
        /// <param name="ProductID">Product ID</param>
        /// <param name="CityID">City ID</param>
        /// <returns>List<PreviewProductForMobileViewModel>object</PreviewProductForMobileViewModel></returns>
        public List<PreviewProductForMobileViewModel> GetPreviewProductForMobile(long ShopStockID)
        {
            List<PreviewProductForMobileViewModel> ls = new List<PreviewProductForMobileViewModel>();
            List<object> paramValues = new List<object>();
            paramValues.Add(ShopStockID);
            //paramValues.Add(CityID);
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
            DataAccessLayer.DbOperations Dbset = new DataAccessLayer.GetData(readCon.DB_CONNECTION.ToString());
            Int64 shopID = (from n in db.Shops
                            join sp in db.ShopProducts on n.ID equals sp.ShopID
                            join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                            where ss.ID == ShopStockID
                            select new { n.ID }).FirstOrDefault().ID;


            DataTable dt = new DataTable();
            dt = Dbset.GetRecords("PreviewProductForMobileViewModel", paramValues);

            ls = (from n in dt.AsEnumerable()
                  select new PreviewProductForMobileViewModel
                  {
                      ShopStockID = n.Field<Int64>("ShopStockID"),
                      ShopID = n.Field<Int64>("ShopID"),
                      ShopName = n.Field<string>("ShopName"),
                      ProductID = n.Field<Int64>("ProductID"),
                      ProductName = n.Field<string>("ProductName"),
                      CategoryID = n.Field<int>("CategoryID"),
                      CategoryName = n.Field<string>("CategoryName"),
                      SizeID = n.Field<int>("SizeID"),
                      SizeName = n.Field<string>("SizeName"),
                      ColorID = n.Field<int>("ColorID"),
                      ColorName = n.Field<string>("ColorName"),
                      MaterialID = n.Field<int>("MaterialID"),
                      MaterialName = n.Field<string>("MaterialName"),
                      ReorderLevel = n.Field<int>("ReorderLevel"),
                      StockStatus = n.Field<bool>("StockStatus"),
                      PackUnitID = n.Field<int>("PackUnitID"),
                      UnitName = n.Field<string>("UnitName"),
                      PackSize = n.Field<decimal>("PackSize"),
                      MRP = n.Field<decimal>("MRP"),
                      RetailerRate = n.Field<decimal>("RetailerRate"),
                      StockQty = n.Field<int>("QTY"),
                      IsInclusiveOfTax = n.Field<bool>("IsInclusiveOfTax"),
                      StockSmallPath = ImageDisplay.LoadProductThumbnails(n.Field<Int64>("ProductID"), n.Field<string>("ColorName").Trim().Equals("N/A") ? "Default" : n.Field<string>("ColorName").Trim(), string.Empty, ProductUpload.THUMB_TYPE.LL),
                      BusinessPoint = n.Field<decimal>("BusinessPoints"),
                      CashbackPoint = n.Field<decimal>("CashbackPoints"),
                      IsDisplayCB = n.Field<int>("IsDisplayCB"),
                      TaxesOnProduct = db.ProductTaxes.Where(x => x.ShopStockID == ShopStockID && x.IsActive == true).Count() > 0 ?
                                          (from pt in db.ProductTaxes
                                           join tm in db.TaxationMasters on pt.TaxID equals tm.ID
                                           join ft in db.FranchiseTaxDetails on tm.ID equals ft.TaxationID
                                           join s in db.Shops on ft.FranchiseID equals s.FranchiseID
                                           where pt.IsActive == true && tm.IsActive == true && ft.IsActive == true
                                           && pt.ShopStockID == ShopStockID && s.IsActive == true && s.ID == shopID
                                           select new CalulatedTaxesRecord
                                           {
                                               ProductTaxID = pt.TaxID,
                                               ShopStockID = pt.ShopStockID,
                                               TaxName = tm.Name,
                                               TaxPrefix = tm.Prefix,
                                               TaxPercentage = ft.InPercentage
                                           }).ToList()
                                             : null


                  }).ToList();

            //get product thumbnail
            //for (int i = 0; i < ls.Count(); i++)
            //{
            //    if (ls[i].CategoryName != null && ls[i].ColorName != "N/A")
            //        ls[i].ProductThumbPath = ImageDisplay.SetProductThumbPath(ls[i].ProductID, ls[i].ColorName, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            //    else
            //        ls[i].ProductThumbPath = ImageDisplay.SetProductThumbPath(ls[i].ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            //}


            return ls;
        }

        /// <summary>
        /// Tejaswee 
        /// Get shop wise category list in two level(i.e. in second level and third level)
        /// </summary>
        /// <param name="shopID"></param>
        public List<RefineCategoryViewModel> GetShopCategoryList(int shopID)
        {
            ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);

            string query = "[Select_ShopCategoryList]";
            SqlCommand cmd = new SqlCommand(query);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ShopID", shopID);

            DataTable dt = new DataTable();
            List<RefineCategoryViewModel> list = new List<RefineCategoryViewModel>();
            using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    sda.Fill(dt);

                    foreach (DataRow r in dt.Rows)
                    {
                        RefineCategoryViewModel st = new RefineCategoryViewModel();
                        st.ID = Convert.ToInt32(r["ThirdLevelCatID"]);
                        st.Name = r["ThirdLevelCatName"].ToString();
                        st.SecondLevelCatID = Convert.ToInt32(r["SecondLevelCatID"]);
                        st.SecondLevelCatName = r["SecondLevelCatName"].ToString();
                        list.Add(st);

                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Pradnyakar Badge
        /// 13-06-2016
        /// For Mobile Product List
        /// Get list of products for the searching criteria given by user
        /// This method is used web API only
        /// </summary>
        /// <param name="productSearch">Product Search object</param>
        /// <returns>List of provided page size(say 12) no of product list</returns>
        public Mobile_ProductStockVarientViewModel Mobile_GetProductStockList(ProductSearchViewModel productSearch)
        {
            Mobile_ProductStockVarientViewModel lProducts = new Mobile_ProductStockVarientViewModel();

            //Get Dataset containing page size(say 12) no of product list in first table,
            //Datable all bulky data required for refinements & 
            //Total Page Size, and product count
            DataSet ds = GetAllProducts(productSearch, true);

            //collect product list from page size(say 12) no of product list in first table 
            if (ds.Tables[0] != null)
            {
                lProducts.ProductInfo = (from DataRow dr in ds.Tables[0].Rows
                                             // orderby Convert.ToDecimal(dr["SaleRate"]) ascending //added by mohit ----as requirment was to show lowest sail rate first(requirment by sumit)
                                         select new Mobile_ProductStockDetailViewModel
                                         {
                                             ProductID = Convert.ToInt64(dr["ProductID"]),
                                             ProductName = dr["ProductName"].ToString().Replace("+", " "), //Added for SEO URL Structure RULE by AShish
                                             ShopName = dr["ShopName"].ToString(),
                                             ColorName = dr["Color"].ToString(),
                                             ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                             MRP = Convert.ToDecimal(dr["MRP"]),
                                             SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                             StockStatus = Convert.ToBoolean(dr["StockStatus"])
                                         }).ToList();

                /*Collect All Shop Stock List*/
                DataTable dt = new DataTable();
                dt.Columns.Add("ShopStockID");

                foreach (Mobile_ProductStockDetailViewModel pa in lProducts.ProductInfo)
                {
                    DataRow dr = dt.NewRow();
                    dr["ShopStockID"] = pa.ShopStockID;
                    dt.Rows.Add(dr);
                }

                /*Get All Varient List By ShopStock List*/
                List<ProductListVarientForMobileViewModel> lsVarient = new List<ProductListVarientForMobileViewModel>();
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);

                DataTable VarientDtResult = new DataTable();
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(readCon.DB_CONNECTION);
                SqlCommand cmd = new SqlCommand("ProductListVarientForMobileViewModel", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ShopStockID", SqlDbType.Structured).Value = dt;
                SqlDataAdapter sqlDt = new SqlDataAdapter(cmd);
                sqlDt.Fill(VarientDtResult);


                //Load product Specification details
                foreach (var item in lProducts.ProductInfo)
                {

                    item.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                        string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    //get ll size product image
                    if (productSearch.ImageType == "SD")
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                       string.Empty, ProductUpload.THUMB_TYPE.SD);
                    else if (productSearch.ImageType == "SS")
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                       string.Empty, ProductUpload.THUMB_TYPE.SS);
                    else if (productSearch.ImageType == "MM")
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                       string.Empty, ProductUpload.THUMB_TYPE.MM);
                    else
                        item.StockSmallImagePath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.ColorName.Trim().Equals("N/A") ? "Default" : item.ColorName.Trim(),
                           string.Empty, ProductUpload.THUMB_TYPE.LL);

                    item.VarientList = (from n in VarientDtResult.AsEnumerable()
                                        where n.Field<long>("ParameterShopStockID") == item.ShopStockID
                                        select new ProductListVarientForMobileViewModel
                                        {
                                            ShopStockID = n.Field<Int64>("ShopStockID"),
                                            SizeID = n.Field<Int32>("SizeID"),
                                            SizeName = n.Field<string>("SizeName"),
                                            ColorID = n.Field<Int32>("ColorID"),
                                            ColorName = n.Field<string>("ColorName"),
                                            MRP = n.Field<decimal>("MRP"),
                                            RetailerRate = n.Field<decimal>("RetailerRate"),
                                            StockStatus = n.Field<bool>("StockStatus"),
                                            IsInclusiveOfTax = n.Field<bool>("IsInclusiveOfTax"),
                                            StockQty = n.Field<Int32>("QTY"),
                                        }).OrderBy(x => x.RetailerRate).ToList(); //changes done by Ashwini Meshram to show lowest to highest price listing



                }
                //added by Ashwini Meshram 21-Dec-2016 to show item is exist in wish list
                foreach (var item in lProducts.ProductInfo)
                {
                    foreach (var items in item.VarientList)
                    {
                        if (productSearch.CustLoginID == 0)
                        {
                            items.isWishList = false;
                        }
                        else if (productSearch.CustLoginID > 0)
                        {
                            if (db.WishLists.Where(x => x.UserLoginID == productSearch.CustLoginID && x.ShopStockID == items.ShopStockID).ToList().Count == 0)
                            {
                                items.isWishList = false;
                            }
                            else
                            {
                                items.isWishList = true;
                            }
                        }

                    }
                }


            }
            //Collect page size and product count information
            if (ds.Tables[1] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(ds.Tables[1].Rows[0]["PageCount"]);
                searchCount.ProductCount = Convert.ToInt32(ds.Tables[1].Rows[0]["Productcount"]);

                lProducts.searchCount = searchCount;

            }

            return lProducts;

        }


        /// <summary>
        /// Sonali Warhade
        /// 28-03-2019
        /// For Get Product List by 3rd level CategoryIds list
        /// Get list of products for the 3rd level category
        /// This method is used web API only
        /// </summary>
        /// <param name="productSearch">Product Search object</param>
        /// <returns>List of provided page size(say 12) no of product list</returns>
        /// 
        public ProductWithRefinementViewModel GetProductListBy3rdLevelCategory(ProductListByCategoryViewModel productSearch)
        {
            ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();

            //Get Dataset containing page size(say 12) no of product list in first table,
            //Datable all bulky data required for refinements & 
            //Total Page Size, and product count

            DataSet ds = GetAllProductsByCategoryIds(productSearch);
            List<SearchProductDetailsViewModel> lproductList = new List<SearchProductDetailsViewModel>();

            //collect product list from page size(say 12) no of product list in first table 
            lproductList = (from DataRow dr in ds.Tables[0].Rows
                            select new SearchProductDetailsViewModel()
                            {
                                ProductThumbPath = string.Empty,
                                ProductID = Convert.ToInt32(dr["ProductID"]),
                                Name = CommonFunctions.ConvertStringToCamelCase(dr["ProductName"].ToString()),
                                CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                CategoryName = CommonFunctions.ConvertStringToCamelCase(dr["CategoryName"].ToString()),//Sonali_04-12-2018
                                MRP = Convert.ToDecimal(dr["MRP"]),
                                SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                StockStatus = Convert.ToInt32(dr["StockStatus"]),
                                ShopStockID = Convert.ToInt32(dr["ShopStockID"]),
                                Color = dr["ColorName"].ToString(),
                                PackSize = Convert.ToDecimal(dr["Packsize"]),
                                PackUnit = dr["PackUnit"].ToString(),
                                Dimension = dr["DimensionName"].ToString(),
                                Size = dr["SizeName"].ToString(),
                                Material = dr["MaterialName"].ToString(),
                                StockQty = Convert.ToInt32(dr["StockQty"]),
                                HtmlColorCode = dr["ColorHtmlCode"].ToString(),
                                RetailPoint = Convert.ToDecimal(dr["RetailPoint"]), //Yashaswi 10-7-2018
                                CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"]),
                            }).ToList();

            //- Start Changes made by Avi Verma. Date : 07-june-2016
            //- Reason : Product Varients included in listing page.
            List<PrdVarientViewModel> lPrdVarientViewModels = (from DataRow dr in ds.Tables[1].Rows
                                                               select new PrdVarientViewModel()
                                                               {
                                                                   ShopStockID = Convert.ToInt64(dr["ShopStockID"]),
                                                                   ProductID = Convert.ToInt64(dr["ProductID"]),
                                                                   CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                                                   ShopID = Convert.ToInt32(dr["ShopID"]),
                                                                   SizeID = Convert.ToInt32(dr["SizeID"]),
                                                                   Size = Convert.ToString(dr["Size"]),
                                                                   MRP = Convert.ToDecimal(dr["MRP"]),
                                                                   SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                                                   RetailPoint = Convert.ToDecimal(dr["RetailPoint"]), //Yashaswi 10-7-2018
                                                                   CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                                                   IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"]),
                                                                   StockStatus = Convert.ToInt32(dr["StockStatus"]),  //Yashaswi 25-7-2018
                                                                   StockQty = Convert.ToInt32(dr["StockQty"]),  //Yashaswi 25-7-2018
                                                                   BrandId = Convert.ToInt64(dr["BrandID"]),//Added for api sort by Sonali_04-01-2018
                                                                   BrandName = CommonFunctions.ConvertStringToCamelCase(dr["BrandName"].ToString()),//Added for api sort by Sonali_04-01-2018
                                                               }).ToList();
            lPrdVarientViewModels = (from pvvm in lPrdVarientViewModels
                                     join shpPriority in db.ShopPriorities on pvvm.CategoryID equals shpPriority.CategoryID
                                     where pvvm.SizeID != 1 // "N/A" 
                                     group pvvm by pvvm.ShopStockID
                                         into grps
                                     select new PrdVarientViewModel
                                     {
                                         ShopStockID = grps.Key,
                                         ProductID = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).ProductID,
                                         ShopID = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).ShopID,
                                         Size = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).Size,
                                         MRP = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).MRP,
                                         SaleRate = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).SaleRate,
                                         RetailPoint = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).RetailPoint,  //Yashaswi 10-7-2018 
                                         CashbackPoint = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).CashbackPoint,
                                         IsDisplayCB = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).IsDisplayCB,
                                         StockStatus = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).StockStatus,  //Yashaswi 25-7-2018
                                         StockQty = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).StockQty,  //Yashaswi 25-7-2018
                                         BrandId = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).BrandId,//Added for api sort by Sonali_04-01-2018
                                         BrandName = grps.FirstOrDefault(x => x.ShopStockID == grps.Key).BrandName,//Added for api sort by Sonali_04-01-2018
                                     }).ToList();

            foreach (var item in lproductList)
            {
                PrdVarientViewModel lPrdVarientViewModel = lPrdVarientViewModels.FirstOrDefault(x => x.ShopStockID == item.ShopStockID);
                if (lPrdVarientViewModel != null)
                {
                    item.ShopID = lPrdVarientViewModel.ShopID;
                    item.BrandId = lPrdVarientViewModel.BrandId;//Added for api sort by Sonali_04-01-2018
                    item.BrandName = lPrdVarientViewModel.BrandName;//Added for api sort by Sonali_04-01-2018
                }
            }
            //- End Changes made by Avi Verma. Date : 07-june-2016

            //get product thumbnail
            foreach (var item in lproductList)
            {
                //if (item.Color != null && item.Color != "N/A")
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, item.Color, string.Empty, ProductUpload.THUMB_TYPE.SD);
                //else
                //    item.ProductThumbPath = ImageDisplay.LoadProductThumbnails(item.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                //==============================================================//
                //Tejaswee (5/11/2015)
                //call different function for product thumbnail
                //==============================================================//
                if (item.Color != null && item.Color != "N/A")
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, item.Color, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                else
                    item.ProductThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);


                //productuploadtempviewmodel.CategoryID = TP.CategoryID;
                var obj = (from n in db.Categories
                           join m in db.Categories on n.ID equals m.ParentCategoryID
                           join p in db.Categories on m.ID equals p.ParentCategoryID
                           where p.ID == item.CategoryID
                           select new
                           {
                               LevelOne = n.ID
                           }).FirstOrDefault();

                item.FirstLevelCatId = Convert.ToInt32(obj.LevelOne);

                //-- Added by Avi Verma. For Dropdownlist
                if (lPrdVarientViewModels.Where(x => x.ProductID == item.ProductID && x.ShopID == item.ShopID).ToList().Count > 0)
                {
                    item.ProductVarientViewModels = lPrdVarientViewModels.Where(x => x.ProductID == item.ProductID && x.ShopID == item.ShopID).ToList();
                    item.MRP = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).FirstOrDefault().MRP;
                    item.SaleRate = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).FirstOrDefault().SaleRate;
                    item.ShopStockID = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).FirstOrDefault().ShopStockID;
                    item.Size = item.ProductVarientViewModels.OrderBy(x => x.SaleRate).FirstOrDefault().Size;
                }

                //Added for SEO ULR Structure RULE by AShish
                item.URLStructureName = GetURLStructureName(item.Name);
                item.Name = item.Name.Replace("+", " ");

            }
            //add the list to the object to be returned
            productWithRefinementViewModel.productList = lproductList;

            //Collect refinements if page index is one, as we don't need it on scrolling 
            //refinement data will remain same
            if (productSearch.PageIndex == 1)
            {

                if (ds.Tables[1] != null)
                {
                    List<ProductRefinementsViewModel> lproductRefinements = new List<ProductRefinementsViewModel>();

                    lproductRefinements = (from DataRow dr in ds.Tables[1].Rows
                                           select new ProductRefinementsViewModel()
                                           {

                                               BrandID = Convert.ToInt32(dr["BrandID"]),
                                               BrandName = dr["BrandName"].ToString(),
                                               CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                               CategoryName = dr["CategoryName"].ToString(),
                                               // CityID = Convert.ToInt32(dr["CityID"]),////hide
                                               ColorID = Convert.ToInt32(dr["ColorID"]),
                                               Color = dr["Color"].ToString(),
                                               DimensionID = Convert.ToInt32(dr["DimensionID"]),
                                               Dimension = dr["Dimension"].ToString(),
                                               MaterialID = Convert.ToInt32(dr["MaterialID"]),
                                               Material = dr["Material"].ToString(),
                                               MRP = Convert.ToDecimal(dr["MRP"]),
                                               ProductID = Convert.ToInt64(dr["ProductID"]),
                                               ProductName = dr["ProductName"].ToString(),
                                               SaleRate = Convert.ToDecimal(dr["SaleRate"]),
                                               ShopID = Convert.ToInt64(dr["ShopID"]),
                                               ShopName = dr["ShopName"].ToString(),
                                               SizeID = Convert.ToInt32(dr["SizeID"]),
                                               Size = dr["Size"].ToString(),
                                               SpecificationID = Convert.ToInt32(dr["SpecificationID"]),
                                               SpecificationValue = dr["SpecificationValue"].ToString(),
                                               ProductSpecificationID = Convert.ToInt32(dr["ProductSpecificationID"]),
                                               SpecificationName = dr["SpecificationName"].ToString(),
                                               PackSize = Convert.ToDecimal(dr["Packsize"]),
                                               PackUnit = dr["PackUnit"].ToString(),
                                               CategoryOrderSequence = Convert.ToInt32(dr["ShopMenuOrder"]),
                                               FranchiseID = Convert.ToInt32(dr["FranchiseID"]),////added
                                               RetailPoint = Convert.ToDecimal(dr["RetailPoint"]), //Yashaswi 10-7-2018
                                               CashbackPoint = Convert.ToDecimal(dr["CashbackPoint"]),
                                               IsDisplayCB = Convert.ToInt16(dr["IsDisplayCB"])
                                           }).ToList();
                    productWithRefinementViewModel.productRefinements = lproductRefinements;
                }
                //collect page size and product count
                if (ds.Tables[2] != null)
                {
                    SearchCountViewModel searchCount = new SearchCountViewModel();
                    searchCount.PageCount = Convert.ToInt32(ds.Tables[2].Rows[0]["PageCount"]);
                    searchCount.ProductCount = Convert.ToInt32(ds.Tables[2].Rows[0]["Productcount"]);

                    productWithRefinementViewModel.searchCount = searchCount;

                }

            }
            //collect page size and product count
            else if (ds.Tables[2] != null)
            {
                SearchCountViewModel searchCount = new SearchCountViewModel();
                searchCount.PageCount = Convert.ToInt32(ds.Tables[2].Rows[0]["PageCount"]);
                searchCount.ProductCount = Convert.ToInt32(ds.Tables[2].Rows[0]["Productcount"]);

                productWithRefinementViewModel.searchCount = searchCount;

            }

            return productWithRefinementViewModel;
        }

        // Get Dataset containing page size(say 12) no of product list in first table,
        // Datable all bulky data required for refinements & 
        // Total Page Size, and product count
        //This method is used in API 
        /// </summary>
        /// <param name="productSearch">product search object like category</param>
        /// <returns></returns>
        private DataSet GetAllProductsByCategoryIds(ProductListByCategoryViewModel productSearch)
        {
            DataSet ds = new DataSet();
            try
            {
                //Yashaswi 27-9-2018
                AutoSearch objAutoSearch = new AutoSearch();
                string Syllable = objAutoSearch.GetSyllables(productSearch.Keyword);
                string result;
                string result1;
                string result2;
                string resultSound;
                objAutoSearch.GetSerachChar(Syllable, out result, out result1, out result2, out resultSound);
                //Yashaswi 27-9-2018

                string query = string.Empty;
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                query = "[ProductListByCategoryIds]";
                SqlCommand cmd = new SqlCommand(query);
                cmd.CommandType = CommandType.StoredProcedure;

                //Yashaswi 27-9-2018
                cmd.Parameters.AddWithValue("@SearchPar1", result);
                cmd.Parameters.AddWithValue("@SearchPar2", result1);
                cmd.Parameters.AddWithValue("@SearchPar3", result2);
                cmd.Parameters.AddWithValue("@SearchPar4", resultSound);
                //Yashaswi 27-9-2018

                cmd.Parameters.AddWithValue("@CityID", productSearch.CityID);/////hide
                cmd.Parameters.AddWithValue("@FranchiseID", productSearch.FranchiseID);////added
                cmd.Parameters.AddWithValue("@ShopID", productSearch.ShopID);
                cmd.Parameters.AddWithValue("@Keyword", productSearch.Keyword);
                cmd.Parameters.AddWithValue("@CategoryIDList", productSearch.CategoryIDList);
                cmd.Parameters.AddWithValue("@PageIndex", productSearch.PageIndex);
                cmd.Parameters.AddWithValue("@PageSize", productSearch.PageSize);
                cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Productcount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                //this parameter is added for WEB only
                cmd.Parameters.AddWithValue("@ShopStockList", productSearch.ShopStockIDList);
                cmd.Parameters.AddWithValue("@SortType", productSearch.SortVal);


                using (SqlConnection con = new SqlConnection(readCon.DB_CONNECTION))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(ds);
                    }
                }
                //Collect page size and product count information from out put variables of Stored Procedure.
                DataTable dt = new DataTable("SeachCount");
                dt.Columns.Add("PageCount", typeof(int));
                dt.Columns.Add("ProductCount", typeof(int));
                dt.Rows.Add(cmd.Parameters["@PageCount"].Value, cmd.Parameters["@Productcount"].Value);
                ds.Tables.Add(dt);

            }
            catch (Exception ex)
            {

            }
            //return three tables in dataset
            return ds;
        }


    }
}
