
//-----------------------------------------------------------------------
// <copyright file="PreviewItemController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using System.Globalization;
using System.Data;
using Gandhibagh.Models;
using System.Web.Configuration;
using System.Collections;

namespace Gandhibagh.Controllers
{

    // [RoutePrefix("eZeelo")]
    [DynamicMetaTag]
    public class PreviewItemController : Controller
    {
        public class WebMethodParamSimilar
        {
            public long cityID { get; set; }
            public int categoryID { get; set; }
            public long productID { get; set; }
            public long shopID { get; set; }
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
            public int franchiseID { get; set; }// added 
        }


        //Added by Zubair on 16-09-2017 for filling CityCookie for SEO Schema
        //if link is directly hitted from google search
        public class GetCityIdFranchiseIDContact2 ////added GetCityIdFranchiseIDContact
        {
            public long cityId { get; set; }
            public int franchiseId { get; set; }
            public string contact { get; set; }
        }
        //End by Zubair

        EzeeloDBContext db = new EzeeloDBContext();
        //private string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();

        //[Route("products/{itemID}")]
        // GET: /PreviewItem/
        [SessionExpire]
        [DynamicMetaTag]
        public ActionResult Index(long itemID, long? shopID, long? shopStockID, string itemName)
        {
            TempData["ReturnFromUrlpurchaseComplete"] = null; ////added change by Tejaswee on 28/9/2016

            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();
            /*
               Indents: 
             * Description: This method is used to display item details
             * Parameters: itemID: Contains ItemId, shopID: Contains ShopId, shopStockID: ShopStockId
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Storing ItemId, ProductId and WishList Status in ViewBag
             *        2) Call GetItemDetails() that internally calls these methods to assign property of object to PreviewItemViewModel:
             *          GetBasicDetails(), GetStockVarients(), GetSellersDealsInProduct(), 
             *          GetStockComponentDetails(), GetStockOffers(), SearchSimilarProducts(), SearchFrequentlyBuyedProducts(),
             *          GetTechnicalSpecifications(), GetGeneralDescription(), GetRecentlyViewedListItemInformation() 
             *          
             */
            PreviewItemViewModel previewItemViewModel = new PreviewItemViewModel();
            try
            {
              

                ViewBag.PID = itemID;
                ViewBag.SID = shopID;
                ViewBag.WishListStatus = Session["UID"];  // Tejaswee
                previewItemViewModel = this.GetItemDetails(itemID, shopID, shopStockID,null,null,null,null,null);

                
                //============== Manoj ======================
                ViewBag.SessionValue = Convert.ToInt64(Session["UID"]);

                if(TempData["CurrentPageIndex"]!=null)
                {
                    TempData.Keep("CurrentPageIndex");
                }
                long cityID = 0;
                int franchiseID = 0;////added
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    cityID = Convert.ToInt64(arr[0]);
                    franchiseID = Convert.ToInt32(arr[2]);////added
                }
                //Get contact no. for SEO purpose
                string helpLineNo = db.HelpDeskDetails.Where(x => x.CityID == cityID && x.FranchiseID == franchiseID).Select(x => x.HelpLineNumber).FirstOrDefault();////added && x.FranchiseID == franchiseID
                TempData["helpLineNo"] = helpLineNo;//--used in _StockVarient.cshtml

                List<WeeklySeasonalFestivalPageMessage> WSFMsg1 = new List<WeeklySeasonalFestivalPageMessage>();
                WSFMsg1 = CallGetPageMessageAPI();
                ViewBag.MinOrderBuyNow = WSFMsg1.Where(x => x.MessageType == "MinimumOrder").Select(i => i.MinimumOrderInRupee).FirstOrDefault();//--used in _StockVarient.cshtml
                //---- Added for new Landing Page by Ashish-----//
                //---- For side Menu ----//
                //int ProductCategoryID = previewItemViewModel.BasicDetails.CategoryID;
                int? Parent = db.Categories.Where(x => x.ID == previewItemViewModel.BasicDetails.CategoryID).Select(x => x.ParentCategoryID).FirstOrDefault();
                List<RefineCategoryViewModel> categoryList = new List<RefineCategoryViewModel>();

                //categoryList = (from c in db.Categories
                //                join cp in db.Categories on c.ID equals cp.ParentCategoryID
                //                where c.IsActive == true && cp.IsActive == true && cp.ParentCategoryID == Parent
                //                select new RefineCategoryViewModel
                //                {
                //                    ID = cp.ID,
                //                    Name = cp.Name.ToLower(),
                //                    SecondLevelCatID = c.ID,
                //                    SecondLevelCatName = c.Name.ToLower()

                //                }
                // ).ToList();



                //categoryList = (from fm in db.FranchiseMenus
                //                join c1 in db.Categories on fm.CategoryID equals c1.ID
                //                join c2 in db.Categories on c1.ParentCategoryID equals c2.ID
                //                join c3 in db.Categories on c2.ParentCategoryID equals c3.ID
                //                join p in db.Products on c1.ID equals p.CategoryID
                //                join sp in db.ShopProducts on p.ID equals sp.ProductID
                //                join s in db.Shops on sp.ShopID equals s.ID
                //                where c1.IsActive == true && c2.IsActive == true && c3.IsActive == true
                //                && p.IsActive == true && sp.IsActive == true && s.IsActive == true
                //                && c2.ID == Parent
                //                && fm.ID == franchiseID
                //                && s.FranchiseID == franchiseID
                //                group p by new { fm.CategoryID, fm.CategoryName, fm.SequenceOrder, fm.Level, c2.ID, c2.Name } into temp
                //                where temp.Count() > 1
                //                select new RefineCategoryViewModel
                //                {
                //                    ID = temp.Key.CategoryID,
                //                    Name = temp.Key.CategoryName.ToLower(),
                //                    SecondLevelCatID = temp.Key.ID,
                //                    SecondLevelCatName = temp.Key.Name.ToLower()
                //                }
                // ).Distinct().ToList();

                //previewItemViewModel.lCatList = categoryList;

                //----- For DynamicBlocks ---//
                // DataSet ds = BusinessLogicLayer.HomePageBlockItemsList.GetHomeIndexItemList(cityVal, System.Web.HttpContext.Current.Server);////hide
                DataSet ds = BusinessLogicLayer.HomePageBlockItemsList.GetHomeIndexItemList(franchiseID, System.Web.HttpContext.Current.Server);////added

                /*Select All Menu By Franchise */
                DataTable dt = new DataTable();
                dt = ds.Tables[0];

                List<BlockViewModel> lBlockTypes = new List<BlockViewModel>();
                lBlockTypes = (from n in dt.AsEnumerable()
                               select new BlockViewModel
                               {
                                   ID = n.Field<Int64>("ID"),
                                   Name = n.Field<string>("Name"),
                                   ImageWidth = n.Field<decimal>("ImageWidth"),
                                   ImageHeight = n.Field<decimal>("ImageHeight"),
                                   MaxLimit = n.Field<int>("MaxLimit"),
                                   IsActive = n.Field<bool>("IsActive")
                               }).OrderBy(x => x.Name).ToList();

                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                DataTable dt1 = new DataTable();
                dt1 = ds.Tables[1];

                DataView dv = new DataView(dt1);
                foreach (BlockViewModel B in lBlockTypes)
                {
                    //int max = 0;
                    //int.TryParse(Convert.ToString(B.MaxLimit), out max);

                    dv.RowFilter = "DesignBlockTypeID=" + B.ID;
                    B.blockItemsList = (from n in dv.ToTable().AsEnumerable()
                                        select new HomePageBlockItemsViewModel
                                        {
                                            ID = n.Field<Int64>("ID"),
                                            ImageName = rcKey.HOME_IMAGE_HTTP + n.Field<string>("ImageName"),
                                            LinkUrl = n.Field<string>("LinkUrl"),
                                            Tooltip = n.Field<string>("Tooltip"),
                                            SequenceOrder = n.Field<int>("SequenceOrder"),
                                            IsActive = n.Field<bool>("IsActive")
                                        }).OrderBy(x => x.SequenceOrder).ToList();
                }

                TempData.Remove("DynamicBlocks");
                TempData.Add("DynamicBlocks", lBlockTypes.Where(x => x.Name.ToLower().Trim() == "logo" || x.Name.ToLower().Trim() == "navigation bar").ToList());

                //----End Added for new Landing Page by Ashish-----//
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("SomethingWrong");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("SomethingWrong");
            }
            return View(previewItemViewModel);
        }

        private RelatedProductsViewModel SearchSimilarProducts(int franchiseId, int categoryID, long productID, long shopID, int pageIndex, int pageSize)////added long cityID->int franchiseId
        {
            RelatedProductsViewModel relatedProductsViewModel = new RelatedProductsViewModel();
            try
            {
                //Check Similar products show on preview product page or not
                PreviewSpecification lPreviewSpecification = new PreviewSpecification();
                if (lPreviewSpecification.GetSpecificationShowStatus(categoryID, PreviewSpecificationEnum.SimilarProducts))
                {

                    SearchSimilarProductViewModel searchSimilarProductViewModel = new SearchSimilarProductViewModel();

                   // searchSimilarProductViewModel.CityID = cityID;////hide
                    searchSimilarProductViewModel.FranchiseID = franchiseId; ////added
                    searchSimilarProductViewModel.CategoryID = categoryID;
                    searchSimilarProductViewModel.ProductID = productID;
                    searchSimilarProductViewModel.ShopID = shopID;
                    searchSimilarProductViewModel.PageIndex = pageIndex;
                    searchSimilarProductViewModel.PageSize = pageSize;
                  

                    ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

                    relatedProductsViewModel = productDetails.GetSimillarProducts(searchSimilarProductViewModel);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][M:SearchSimilarProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][M:SearchSimilarProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return relatedProductsViewModel;
        }

        private RelatedProductsViewModel SearchFrequentlyBuyedProducts(int franchiseId, long productID, long shopID, int categoryID, int pageIndex, int pageSize)////added long cityID->int franchiseId
        {
            RelatedProductsViewModel relatedProductsViewModel = new RelatedProductsViewModel();
            try
            {
                //Check Frequently Buyed Product show on preview product page or not
                PreviewSpecification lPreviewSpecification = new PreviewSpecification();
                if (lPreviewSpecification.GetSpecificationShowStatus(categoryID, PreviewSpecificationEnum.FrequentlyBuyedProducts))
                {
                    SearchFrequentlyBuyedProductViewModel searchFrequentlyBuyedProductViewModel = new SearchFrequentlyBuyedProductViewModel();

                   // searchFrequentlyBuyedProductViewModel.CityID = cityID;////hide
                    searchFrequentlyBuyedProductViewModel.FranchiseID = franchiseId;////added
                    searchFrequentlyBuyedProductViewModel.ProductID = productID;
                    searchFrequentlyBuyedProductViewModel.ShopID = shopID;
                    searchFrequentlyBuyedProductViewModel.PageIndex = pageIndex;
                    searchFrequentlyBuyedProductViewModel.PageSize = pageSize;

                    ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

                    relatedProductsViewModel = productDetails.GetFrequentlyBuyedProducts(searchFrequentlyBuyedProductViewModel);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][M:SearchFrequentlyBuyedProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][M:SearchFrequentlyBuyedProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return relatedProductsViewModel;
        }

        /// <summary>
        /// Function to set recently viewed item cookie
        /// </summary>
        /// <param name="itemID"></param>
        public void SetRecentlyViewedItemsCookie(long itemID, long ShopStockID, string ProductName, string productThumbPath)
        {
            try
            {
                if (ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"] != null && ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"].ToString() != string.Empty) //
                {
                    string cookRecView = ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"].Value;
                    string checkItmId = itemID + "$" + ShopStockID + "$" + ProductName;
                    if (!cookRecView.Contains(checkItmId))
                    {
                        ControllerContext.HttpContext.Response.Cookies["RecentlyViewedItems"].Value = ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"].Value + "|" + itemID + "$" + ShopStockID + "$" + ProductName + "$" + productThumbPath;
                    }
                    else
                    {
                        //Initialize cookie again because , cookie value overwrite by null if we are not initialize cookie
                        ControllerContext.HttpContext.Response.Cookies["RecentlyViewedItems"].Value = ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"].Value;
                    }
                }
                else
                {
                    ControllerContext.HttpContext.Response.Cookies["RecentlyViewedItems"].Value = itemID + "$" + ShopStockID + "$" + ProductName + "$" + productThumbPath;
                }
               // ControllerContext.HttpContext.Response.Cookies["RecentlyViewedItems"].Value = "$1$$2$$3$$4$";
                ControllerContext.HttpContext.Response.Cookies["RecentlyViewedItems"].Expires = DateTime.Now.AddDays(1);
                ControllerContext.HttpContext.Response.AppendCookie(ControllerContext.HttpContext.Response.Cookies["RecentlyViewedItems"]);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][M:SetRecentlyViewedItemsCookie]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][M:SetRecentlyViewedItemsCookie]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
        }

        public RecentlyViewedCollectionViewModel GetRecentlyViewedListItemInformation(long itemID, int categoryID)
        {
            RecentlyViewedCollectionViewModel rvm = new RecentlyViewedCollectionViewModel();
            //Check Recently viewed Product show on preview product page or not
            PreviewSpecification lPreviewSpecification = new PreviewSpecification();
            if (lPreviewSpecification.GetSpecificationShowStatus(categoryID, PreviewSpecificationEnum.RecentlyViewedProducts))
            {
                // Set cookie for recently view items
                //SetRecentlyViewedItemsCookie(itemID);

                //string[] productDetails;
                List<object> lstProductDetails = new List<object>();

                List<RecentlyViewedViewModel> lListRecentlyViewed = new List<RecentlyViewedViewModel>();

                if (ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"] != null)
                {
                    string cookRecView = ControllerContext.HttpContext.Request.Cookies["RecentlyViewedItems"].Value;
                    string[] ProductId = cookRecView.Split('|');
                    foreach (string item in ProductId)
                    {
                        if (item != string.Empty)
                        {
                            string[] productDetail = item.Split('$');
                            if (productDetail.Length < 4)
                                continue;

                            RecentlyViewedViewModel obj = new RecentlyViewedViewModel();

                            obj.ItemID = Convert.ToInt64(productDetail[0]);
                            obj.ShopStockID = Convert.ToInt64(productDetail[1]);
                            //obj.Name = productDetail[2]; //hide
                            //Added for SEO ULR Structure RULE by AShish
                            obj.Name = productDetail[2].Replace("+"," ");
                            obj.ImageUrl = productDetail[3];
                            obj.URLStructureName = GetURLStructureName(productDetail[2]);

                            //var lPitemID = int.Parse(item);
                            ////var queryResult = (from p in db.Products where p.ID == itemID select new { p.Name }).ToList();
                            //////lstProductDetails.Add(queryResult);


                            ////if (queryResult.Count() > 0)
                            ////{
                            //for (int i = 0; i < 4; i++)
                            //{
                            //    //obj.Name = queryResult[i].Name;
                            //    obj.Name = "Tejaswee";
                            //}
                            ////}
                            lListRecentlyViewed.Add(obj);
                        }
                    }
                }
                
                rvm.lRecentlyViewedCollectionViewModel = lListRecentlyViewed;
                //productDetails queryResult
                //return Json(lListRecentlyViewed, JsonRequestBehavior.AllowGet);
                //return Json(rvm, JsonRequestBehavior.AllowGet);
               
                //return PartialView("RecentlyViewedItemsPartial", rvm);
            }
            return rvm;
        }

        /// <summary>
        /// Added for SEO URL Structure RULE by AShish
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string GetURLStructureName(string Name)
        {
            string str = Name;
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[\\\#$~%.':*?<>{} ]", " ").Replace("&", "and");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+/g", " ");
            // string[] parts2 = Regex.Split(str, @"\+\/\-\,\(\)");
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

            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"\s+", " ");
            concat = System.Text.RegularExpressions.Regex.Replace(concat, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and");
            concat = concat.Trim(new[] { '-' });

            //string test = concat.Substring(0, 1);

            //if (test == "-")
            //{ concat = concat.Substring(1, concat.Length); }
            //var test2 = concat[concat.Length - 1];
            //if (test2 == '-')
            //{ concat = concat.Substring(0, concat.Length - 1); }

            return concat;
        } 
        //private PreviewItemViewModel GetItemDetails(long itemID, long? shopID, long? shopStockID)
        //{
            
        //    long lShopID = 0, cityID = 0, areaID = 0;
        //    long.TryParse(Convert.ToString(shopID), out lShopID);

        //    if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
        //    {
        //        string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
        //        string[] arr = cookieValue.Split('$');
        //        cityID = Convert.ToInt64(arr[0]);
        //    }

        //    ViewBag.CityID = cityID;

        //    PreviewItemViewModel previewItemViewModel = new PreviewItemViewModel();

        //    try
        //    {
        //        ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

        //        //ProductViewBasicDetailsViewModels productBasicDetails = productDetails.GetBasicDetails(itemID);
        //        ProductViewBasicDetailsViewModels productBasicDetails = productDetails.GetBasicDetails(itemID, cityID);
        //        previewItemViewModel.DeliveryTime = SetDeliveryTime(productBasicDetails.ProductID);



        //        long lCustLoginID = 0;
        //        long.TryParse(Convert.ToString(Session["UID"]), out lCustLoginID);

        //        //IEnumerable<ShopProductVarientViewModel> listStockVarient = productDetails.GetStockVarients(itemID, lShopID, lCustLoginID, cityID);
        //        IEnumerable<ShopProductVarientViewModel> listStockVarient = productDetails.GetStockVarients(itemID, lShopID, lCustLoginID, cityID);
        //        List<ProductSellersViewModel> listProductSellers = productDetails.GetSellersDealsInProduct(itemID, cityID, areaID);

        //        long ssID = 0;

        //        var itemMinSaleRate = listStockVarient.Min(y => y.SaleRate);
        //        var itemsMin = listStockVarient.Where(x => x.SaleRate == itemMinSaleRate).First();

        //        if (shopStockID > 0)
        //        {
        //            ViewBag.ShopStockID = shopStockID;
        //            itemsMin = listStockVarient.Where(x => x.ShopStockID == shopStockID).First();
        //        }

        //        if (itemsMin != null)
        //        {
        //            ViewBag.SID = itemsMin.ShopID;
        //            ssID = itemsMin.ShopStockID;
        //        }

        //        SetRecentlyViewedItemsCookie(itemID, ssID, itemsMin.ProductName, itemsMin.StockThumbPath);  // Tejaswee
                

        //        //send parameter shopstockID; when clicked on varients
        //        StockComponentsViewModel stockComponents = new StockComponentsViewModel();
        //        if (shopStockID != null && shopStockID > 0)
        //        {
        //             stockComponents = productDetails.GetStockComponentDetails( Convert.ToInt64(shopStockID), itemsMin.ShopID);
        //        }
        //        else
        //        {
        //             stockComponents = productDetails.GetStockComponentDetails(ssID, itemsMin.ShopID); 
        //        }
        //        PriceAndOffers priceAndOffers = new PriceAndOffers(System.Web.HttpContext.Current.Server);

        //        ProductOffersViewModel ProductOffers = priceAndOffers.GetStockOffers(ssID);

        //        PayablePricesViewModel payablePrice = new PayablePricesViewModel();

        //        if (ProductOffers != null)
        //        {
        //            payablePrice = this.GetPayablePrice(ProductOffers, itemsMin.SaleRate, itemsMin.MRP, ssID, itemsMin.ShopID);
        //        }

        //        int lCategoryID = 0;
        //        int.TryParse(Convert.ToString(productBasicDetails.CategoryID), out lCategoryID);

        //        ViewBag.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(productBasicDetails.ProductName.ToLower()) +" | eZeelo";
        //        RelatedProductsViewModel similarProducts = this.SearchSimilarProducts(cityID, lCategoryID, itemID, lShopID, 1, 3);
        //        RelatedProductsViewModel frequentlyBuyedProducts = this.SearchFrequentlyBuyedProducts(cityID, itemID, lShopID, 1, 5);
        //        IEnumerable<ProductTechnicalSpecificationViewModel> technicalSpecification = productDetails.GetTechnicalSpecifications(itemID);
        //        string generalFilePath = productDetails.GetGeneralDescription(itemID,true);

        //        RecentlyViewedCollectionViewModel rvm = this.GetRecentlyViewedListItemInformation(itemID);

        //        //Display Reviews
        //        BusinessLogicLayer.Review review = new Review();
        //        DisplayReviewsViewModel productReviews = new DisplayReviewsViewModel();
        //        productReviews = review.GetReviews(itemID, itemsMin.ProductID, BusinessLogicLayer.Review.REVIEWS.PRODUCT);
        //        productReviews.AvgPoints.OwnerID = itemID;
        //        DisplayReviewsViewModel shopReviews = new DisplayReviewsViewModel();
        //        shopReviews = review.GetReviews(itemsMin.ShopID,itemsMin.ProductID, BusinessLogicLayer.Review.REVIEWS.SHOP);
        //        shopReviews.AvgPoints.OwnerID = itemsMin.ShopID;
        //        //productReviews = review.GetReviews(4, BusinessLogicLayer.Review.REVIEWS.PRODUCT);
        //        //productReviews.AvgPoints.OwnerID = 4;
        //        //DisplayReviewsViewModel shopReviews = new DisplayReviewsViewModel();
        //        //shopReviews = review.GetReviews(1, BusinessLogicLayer.Review.REVIEWS.SHOP);
        //        //shopReviews.AvgPoints.OwnerID = 1;
                

        //        previewItemViewModel.BasicDetails = productBasicDetails;
        //        previewItemViewModel.StockVarient = listStockVarient;
        //        previewItemViewModel.ProductOffers = ProductOffers;
        //        previewItemViewModel.PayablePrice = payablePrice;
        //        previewItemViewModel.ProductSellers = listProductSellers;
        //        previewItemViewModel.StockComponents = stockComponents;
        //        previewItemViewModel.SimilarProducts = similarProducts;
        //        previewItemViewModel.FrequentlyBuyedProducts = frequentlyBuyedProducts; 
        //        previewItemViewModel.TechnicalSpecification = technicalSpecification;
        //        previewItemViewModel.GeneralSpecifications = generalFilePath;
        //        previewItemViewModel.RecentlyViewedItems = rvm;
        //        previewItemViewModel.BuyProduct = new BuyThisProductViewModel();

        //        //Attach review models
        //        previewItemViewModel.ProductReviews = productReviews;
        //        previewItemViewModel.ShopReviews = shopReviews; 

        //        Bedcrumbs b = new Bedcrumbs();
        //        string bedcrumbString = itemsMin.ProductName + "$" + b.GetCategoryHierarchy(lCategoryID);
        //        ViewBag.Bedcrumb = bedcrumbString;


        //        /*Tejaswee
        //        * For SEO H1 Tag on the Preview Page Title
        //        * 08-Jan-2016                 
        //        */
        //        SEO ldata = new SEO();

        //        ldata = db.SEOs.Where(x => x.EntityID == itemID && x.BusinessType.Prefix == "MTDT").FirstOrDefault();
        //        if (ldata != null)
        //        {
        //            TempData["MetaData_Category"] = ldata.H1;

        //        }

        //        //var itemMinSaleRate = listStockVarient.Min(y => y.SaleRate);
        //        //var itemsMax = listStockVarient.Where(x => x.SaleRate == itemMinSaleRate).First();

               
        //    }
        //    catch (MyException myEx)
        //    {
        //        throw new BusinessLogicLayer.MyException("[PreviewItemController][M:GetItemDetails]", "Can't get item details !" + Environment.NewLine + myEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[PreviewItemController][M:GetItemDetails]", "Can't get item details !" + Environment.NewLine + ex.Message);
        //    }
        //    return previewItemViewModel;
        //}

       
        private PreviewItemViewModel GetItemDetails(long itemID, long? shopID, long? shopStockID, string selectedType, string ColorID, string SizeID, string DimensionID, string MaterialID)
        { //Change by Snehal for offer percent not getting according to packsize issue on date:17/02/2016

            long lShopID = 0, cityID = 0, areaID = 0;
            int franchiseID = 0;////added
            long.TryParse(Convert.ToString(shopID), out lShopID);

            ViewBag.shopID = lShopID;

            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                string[] arr = cookieValue.Split('$');
                cityID = Convert.ToInt64(arr[0]);
                franchiseID = Convert.ToInt32(arr[2]);////added
            }
            else
            {
                //Added by Zubair on 16-09-2017 for filling CityCookie for SEO Schema
                //if link is directly hitted from google search
                //string cname = Request.Url.ToString().Replace("http://localhost:5555/", "");
                //string cname = Request.Url.AbsoluteUri.ToString().Replace("http://customer.identical/", "");
                string cname = Request.Url.AbsoluteUri.ToString().Replace("" + (new URLsFromConfig()).GetURL("CUSTOMER") + "", "");
                string cityName = cname.Substring(0, cname.IndexOf('/'));
                ArrayList al = new ArrayList(cname.Split('/'));
                franchiseID = Convert.ToInt32(al[1]);

                var CityIDFranchiseIdContact = (from f in db.Franchises
                                                join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                join pc in db.Pincodes on bd.PincodeID equals pc.ID
                                                join c in db.Cities on pc.CityID equals c.ID
                                                join hd in db.HelpDeskDetails on f.ID equals hd.FranchiseID into LOJ
                                                from hd in LOJ.DefaultIfEmpty()//-- For Left Outer Join --//
                                                where f.ID != 1 && f.IsActive == true && c.Name.ToLower().Trim() == cityName.ToLower().Trim()
                                                      && f.ID == franchiseID && ul.IsLocked == false && c.IsActive == true
                                                select new GetCityIdFranchiseIDContact2
                                                {
                                                    cityId = c.ID,
                                                    franchiseId = f.ID,
                                                    contact = hd.HelpLineNumber
                                                }).FirstOrDefault();

                if (CityIDFranchiseIdContact != null)
                {
                    cityID = Convert.ToInt32(CityIDFranchiseIdContact.cityId);
                    string contact = CityIDFranchiseIdContact.contact;
                    string City = cityID + "$" + cityName + "$" + franchiseID + "$" + contact;
                    //HttpCookie cookie = Request.Cookies["CityCookie"];//Get the existing cookie by cookie name.
                    HttpCookie cookie = new HttpCookie("CityCookie");
                    cookie.Value = Convert.ToString(City);
                    cookie.Expires = System.DateTime.Now.AddDays(7);
                    Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
                }
            }
            //End by Zubair

            ViewBag.CityID = cityID;
            ViewBag.FranchiseID = franchiseID;////added
           
            PreviewItemViewModel previewItemViewModel = new PreviewItemViewModel();

            try
            {
                ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);

                //ProductViewBasicDetailsViewModels productBasicDetails = productDetails.GetBasicDetails(itemID);
                ProductViewBasicDetailsViewModels productBasicDetails = productDetails.GetBasicDetails(itemID,  franchiseID);////added cityID->franchiseID
                previewItemViewModel.DeliveryTime = SetDeliveryTime(productBasicDetails.ProductID);



                long lCustLoginID = 0;
                long.TryParse(Convert.ToString(Session["UID"]), out lCustLoginID);

                //IEnumerable<ShopProductVarientViewModel> listStockVarient = productDetails.GetStockVarients(itemID, lShopID, lCustLoginID, cityID);
                IEnumerable<ShopProductVarientViewModel> listStockVarient = productDetails.GetStockVarients(itemID, lShopID, lCustLoginID,  franchiseID);////added cityID->franchiseID
                List<ProductSellersViewModel> listProductSellers = productDetails.GetSellersDealsInProduct(itemID, cityID, areaID, franchiseID);////added cityID->franchiseID old

                long ssID = 0;

                var itemMinSaleRate = listStockVarient.Min(y => y.SaleRate);
                var itemsMin = listStockVarient.Where(x => x.SaleRate == itemMinSaleRate).First();

                if (shopStockID > 0 && string.IsNullOrEmpty(selectedType))
                {
                    ViewBag.ShopStockID = shopStockID;
                    itemsMin = listStockVarient.Where(x => x.ShopStockID == shopStockID).First();
                }
                else if(!string.IsNullOrEmpty(selectedType))
                {
                    int color = 0, size = 0, dimension = 0, material = 0;
                    int.TryParse(ColorID, out color);
                    int.TryParse(SizeID, out size);
                    int.TryParse(DimensionID, out dimension);
                    int.TryParse(MaterialID, out material);

                    if (selectedType == "c")
                    {
                        itemsMin = listStockVarient.Where(x => x.ColorID == color).First();

                    }
                    else if (selectedType == "s")
                    {
                        itemsMin = listStockVarient.Where(x => x.ColorID == color && x.SizeID == size).First();
                    }
                    else if (selectedType == "d")
                    {
                        itemsMin = listStockVarient.Where(x => x.ColorID == color && x.SizeID == size && x.DimensionID == dimension).First();
                    }
                    else if (selectedType == "m")
                    {
                        itemsMin = listStockVarient.Where(x => x.ColorID == color && x.SizeID == size && x.DimensionID == dimension && x.MaterialID == material).First();
                    }
                    ViewBag.ShopStockID = itemsMin.ShopStockID;
                }

                if (itemsMin != null)
                {
                    ViewBag.SID = itemsMin.ShopID;
                    ssID = itemsMin.ShopStockID;
                }

                SetRecentlyViewedItemsCookie(itemID, ssID, itemsMin.ProductName, itemsMin.StockThumbPath);  // Tejaswee


                //send parameter shopstockID; when clicked on varients
                StockComponentsViewModel stockComponents = new StockComponentsViewModel();
                if (shopStockID != null && shopStockID > 0)
                {
                    stockComponents = productDetails.GetStockComponentDetails(Convert.ToInt64(shopStockID), itemsMin.ShopID);
                }
                else
                {
                    stockComponents = productDetails.GetStockComponentDetails(ssID, itemsMin.ShopID);
                }
                PriceAndOffers priceAndOffers = new PriceAndOffers(System.Web.HttpContext.Current.Server);

                ProductOffersViewModel ProductOffers = priceAndOffers.GetStockOffers(ssID);

                PayablePricesViewModel payablePrice = new PayablePricesViewModel();

                if (ProductOffers != null)
                {
                    payablePrice = this.GetPayablePrice(ProductOffers, itemsMin.SaleRate, itemsMin.MRP, ssID, itemsMin.ShopID);
                }

                int lCategoryID = 0;
                int.TryParse(Convert.ToString(productBasicDetails.CategoryID), out lCategoryID);
             
              //  ViewBag.IsManaged=(db.ShopPriorities.Where(x => x.CategoryID == lCategoryID && x.CityID == cityID && x.IsActive == true).Count() > 0) ?  1 : 0;////hide
                ViewBag.IsManaged = (from sp in db.ShopPriorities
                                     join sh in db.Shops on sp.ShopID equals sh.ID
                                     where sp.CategoryID==lCategoryID && sp.CityID==cityID && sh.FranchiseID==franchiseID && sp.IsActive==true select sp.ID).Count()>0?1:0; ////added





                //   ViewBag.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(productBasicDetails.ProductName.ToLower()) + " | eZeelo";
                //Yashaswi 21-02-2019 For TASK 1296 To display extra product in You Might Also Like Section 
                //change SearchSimilarProducts last parameter from 3 to 5
                RelatedProductsViewModel similarProducts = this.SearchSimilarProducts(franchiseID, lCategoryID, itemID, lShopID, 1, 5);////added cityID->franchiseID
               
                RelatedProductsViewModel frequentlyBuyedProducts = this.SearchFrequentlyBuyedProducts(franchiseID, itemID, lShopID, lCategoryID, 1, 5 );////added cityID->franchiseID

                PreviewSpecification lPreviewSpecification = new PreviewSpecification();
                IEnumerable<ProductTechnicalSpecificationViewModel> technicalSpecification = new List<ProductTechnicalSpecificationViewModel>();
                if (lPreviewSpecification.GetSpecificationShowStatus(lCategoryID, PreviewSpecificationEnum.TechnicalDescription))
                {
                    technicalSpecification = productDetails.GetTechnicalSpecifications(itemID);
                }


                ////added by Tejaswee for showing general specification or not
                 
                 string generalFilePath = string.Empty;
                 if (lPreviewSpecification.GetSpecificationShowStatus(lCategoryID, PreviewSpecificationEnum.GeneralDescription))
                 {
                     generalFilePath = productDetails.GetGeneralDescription(itemID, true);
                 }

                RecentlyViewedCollectionViewModel rvm = this.GetRecentlyViewedListItemInformation(itemID, lCategoryID);

                //Display Reviews
                BusinessLogicLayer.Review review = new Review();
                DisplayReviewsViewModel productReviews = new DisplayReviewsViewModel();
                DisplayReviewsViewModel shopReviews = new DisplayReviewsViewModel();
                if (lPreviewSpecification.GetSpecificationShowStatus(lCategoryID, PreviewSpecificationEnum.ProductReviews))
                {
                    productReviews = new DisplayReviewsViewModel();
                    productReviews = review.GetReviews(itemID, itemsMin.ProductID, BusinessLogicLayer.Review.REVIEWS.PRODUCT);
                    productReviews.AvgPoints.OwnerID = itemID;
                    productReviews.IsDisplay = true;
                }
                //else
                //{
                //    productReviews.IsDisplay=false;
                //}
                if (lPreviewSpecification.GetSpecificationShowStatus(lCategoryID, PreviewSpecificationEnum.ShopReviews))
                {
                    shopReviews = new DisplayReviewsViewModel();
                    shopReviews = review.GetReviews(itemsMin.ShopID, itemsMin.ProductID, BusinessLogicLayer.Review.REVIEWS.SHOP);
                    shopReviews.AvgPoints.OwnerID = itemsMin.ShopID;
                    shopReviews.IsDisplay = true;
                }
                //productReviews = review.GetReviews(4, BusinessLogicLayer.Review.REVIEWS.PRODUCT);
                //productReviews.AvgPoints.OwnerID = 4;
                //DisplayReviewsViewModel shopReviews = new DisplayReviewsViewModel();
                //shopReviews = review.GetReviews(1, BusinessLogicLayer.Review.REVIEWS.SHOP);
                //shopReviews.AvgPoints.OwnerID = 1;

                previewItemViewModel.BasicDetails = productBasicDetails;
                previewItemViewModel.StockVarient = listStockVarient;
                previewItemViewModel.ProductOffers = ProductOffers;
                previewItemViewModel.PayablePrice = payablePrice;
                previewItemViewModel.ProductSellers = listProductSellers;
                previewItemViewModel.StockComponents = stockComponents;
                previewItemViewModel.SimilarProducts = similarProducts;
                previewItemViewModel.FrequentlyBuyedProducts = frequentlyBuyedProducts;
                previewItemViewModel.TechnicalSpecification = technicalSpecification;
                previewItemViewModel.GeneralSpecifications = generalFilePath;
                previewItemViewModel.RecentlyViewedItems = rvm;
                previewItemViewModel.BuyProduct = new BuyThisProductViewModel();

                //Attach review models
                previewItemViewModel.ProductReviews = productReviews;
                previewItemViewModel.ShopReviews = shopReviews;

                Bedcrumbs b = new Bedcrumbs();
                string bedcrumbString = itemsMin.ProductName + "$" + b.GetCategoryHierarchy(lCategoryID);

                ViewBag.Bedcrumb = bedcrumbString;


                /*Tejaswee
                * For SEO H1 Tag on the Preview Page Title
                * 08-Jan-2016                 
                */
                SEO ldata = new SEO();

                ldata = db.SEOs.Where(x => x.EntityID == itemID && x.BusinessType.Prefix == "MTDT").FirstOrDefault();
                if (ldata != null)
                {
                    TempData["MetaData_Category"] = ldata.H1;

                }

                //var itemMinSaleRate = listStockVarient.Min(y => y.SaleRate);
                //var itemsMax = listStockVarient.Where(x => x.SaleRate == itemMinSaleRate).First();
                //================ Manoj Yadav ===================================================================
                long UserID = Convert.ToInt64(Session["UID"]);
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                   // TrackSearchBusiness.InsertSearchDetails(UserID, 0, Convert.ToInt64(shopID), itemsMin.ProductName, "", "", "", "", cookieValue.Split('$')[1],"");////hide
                    TrackSearchBusiness.InsertSearchDetails(UserID, 0, Convert.ToInt64(shopID), itemsMin.ProductName, "", "", "", "", cookieValue.Split('$')[1], "", Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//
                }

                //=====Added by Ashwini Meshram 12-Dec-2016 to show category list ===========================================================
                //int? Parent = db.Categories.Where(x => x.ID == previewItemViewModel.BasicDetails.CategoryID).Select(x => x.ParentCategoryID).FirstOrDefault();
                //List<RefineCategoryViewModel> categoryList = new List<RefineCategoryViewModel>();
                //categoryList = (from c in db.Categories
                //                join cp in db.Categories on c.ID equals cp.ParentCategoryID
                //                where c.IsActive == true && cp.IsActive == true && cp.ParentCategoryID == Parent
                //                select new RefineCategoryViewModel
                //                {
                //                    ID = cp.ID,
                //                    Name = cp.Name.ToLower(),
                //                    SecondLevelCatID = c.ID,
                //                    SecondLevelCatName = c.Name.ToLower()
                //                    //URLStructureName=GetURLStructureName(cp.Name)
                //                }
                // ).ToList();

                //previewItemViewModel.lCatList = categoryList;

                //================ END ===========================================================================
                //===================Get category Hierarchy ==============
                previewItemViewModel.lCatList = this.GetCategoryList(previewItemViewModel.BasicDetails.CategoryID, franchiseID);


            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:GetItemDetails]", "Can't get item details !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:GetItemDetails]", "Can't get item details !" + Environment.NewLine + ex.Message);
            }
            return previewItemViewModel;
        }

        public ActionResult RebindStockVarient(long itemID, long? shopID, long? shopStockID, string selectedType, string selectedID, string ColorID, string SizeID, string DimensionID, string MaterialID, string MinOrder) //-- Added MinOrder for Dynamic Message --//
        {
            PreviewItemViewModel previewItemViewModel = new PreviewItemViewModel();
            try
            {
                ViewBag.PID = itemID;
                ViewBag.SID = shopID;

                ViewBag.ColorID = ColorID;
                ViewBag.SizeID = SizeID;
                ViewBag.DimensionID = DimensionID;
                ViewBag.MaterialID = MaterialID;

                ViewBag.selectedID = selectedID;
                ViewBag.selectedType = selectedType;

                ViewBag.WishListStatus = Session["UID"];  // Tejaswee
                //Change by Snehal for offer percent not getting according to packsize issue on date:17/02/2016
                 previewItemViewModel = this.GetItemDetails(itemID, shopID, Convert.ToInt64(shopStockID), selectedType, ColorID, SizeID, DimensionID, MaterialID);
                
                //previewItemViewModel = this.GetItemDetails(itemID, shopID, Convert.ToInt64(shopStockID));

                 //-- Added for Dynamic Message --//
                 ViewBag.MinOrderBuyNow = MinOrder;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][GET:RebindStockVarient]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][GET:RebindStockVarient]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return PartialView("_StockVarient", previewItemViewModel);
        }

        public JsonResult BindSimilarProducts(WebMethodParamSimilar myParam)
        {
            try
            {
                RelatedProductsViewModel similarProducts = this.SearchSimilarProducts(myParam.franchiseID, myParam.categoryID, myParam.productID, myParam.shopID, myParam.pageIndex, myParam.pageSize);////added  myParam.cityID->myParam.franchiseID
                return Json(similarProducts.ProductList, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][JSON:BindSimilarProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][JSON:BindSimilarProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BindFrequentlyBuyedProducts(WebMethodParamSimilar myParam)
        {
            try
            {
                //RelatedProductsViewModel frequentlyBuyedProducts = this.SearchFrequentlyBuyedProducts(myParam.cityID, myParam.productID, myParam.shopID, myParam.pageIndex, myParam.pageSize);////hide
                RelatedProductsViewModel frequentlyBuyedProducts = this.SearchFrequentlyBuyedProducts(myParam.franchiseID, myParam.productID, 0, myParam.categoryID, myParam.pageIndex, myParam.pageSize);////added myParam.cityID->myParam.franchiseID
               // RelatedProductsViewModel similarProducts = this.SearchSimilarProducts(myParam.cityID, myParam.categoryID, myParam.productID, myParam.shopID, myParam.pageIndex, myParam.pageSize);
                return Json(frequentlyBuyedProducts.ProductList, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][JSON:BindFrequentlyBuyedProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][JSON:BindFrequentlyBuyedProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckDeliveryPincode(string pincode)
        {
            /*
               Indents: 
             * Description: This method is used to check deliverable pincode and 
             * if delivery is possible then set cookie "DeliverablePincode" by calling method from SetVerifiedPincode()
             *
             
             * Parameters: pincode: Pincode to check delivery is posible or not
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Check pincode is deliverable or not 
             *        2) If yes, Set Cookie for deliverable pincode
             * Return : True if exist else false     
             */

            try
            {
                PincodeVerification pincodeVerification = new PincodeVerification();
                ShoppingCartInitialization sci = new ShoppingCartInitialization();
                bool isDeliverable = pincodeVerification.IsDeliverablePincode(pincode); // 1
                if (isDeliverable == true) // 2
                {
                    sci.SetVerifiedPincode(pincode);
                }//Yashaswi 4/6/2018
                else
                {
                    sci.RemoveVerifiedPincode();
                }
                return Json(isDeliverable, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][JSON:CheckDeliveryPincode]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][JSON:CheckDeliveryPincode]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        private PayablePricesViewModel GetPayablePrice(ProductOffersViewModel productOffers, decimal saleRate, decimal mrp, long shopStockID, long shopID)
        {
            PayablePricesViewModel payablePricesViewModel = new PayablePricesViewModel();

            try
            {
                PriceAndOffers priceAndOffers = new PriceAndOffers(System.Web.HttpContext.Current.Server);

                if (productOffers.ComponentOffer != null)
                {
                    Decimal lSavedRs = 0;
                    Decimal lSavedPer = 0;

                    Decimal lFinalAmount = 0;
                    lFinalAmount = priceAndOffers.GetComponentOfferAmount(shopStockID, shopID, saleRate, ref lSavedRs, ref lSavedPer);

                    payablePricesViewModel.FinalAmount = lFinalAmount;
                    payablePricesViewModel.SavedRs = lSavedRs;
                    payablePricesViewModel.SavedPercent = lSavedPer;
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:GetPayablePrice]", "Can't get payable price !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:GetPayablePrice]", "Can't get payable price !" + Environment.NewLine + ex.Message);
            }

            return payablePricesViewModel;
        }


        //-----------------------------------------------------------------------
        // <copyright file="CustomerDetails.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
        //     Copyright  Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
        // </copyright>
        // <author>Tejaswee Taktewale</author>
        //-----------------------------------------------------------------------

        #region WishList And ShortList

        public class WebmethodParams
        {
            public int shopStockID { get; set; }
            public string productName { get; set; }
            public int productID { get; set; }
           
        }

        public JsonResult AddToWishList(string shopStockID)
        {
            //System.Threading.Thread.Sleep(1000);
            long lCustID = 0, lShopStockID = 0;
            //Session["UID"] = 1;
            long.TryParse(Session["UID"].ToString(), out lCustID);
            long.TryParse(shopStockID, out lShopStockID);
            CustomerWishlist cw = new CustomerWishlist(System.Web.HttpContext.Current.Server);
            int oprStatus = cw.SetWishlist(lCustID, lShopStockID);
            return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost] 
        //public JsonResult AddToShortList(string shopStockID, string productName, string productID)

        //Not in use
        //public JsonResult AddToShortList(WebmethodParams myParam)
        //{
        //    string imgUrl = "../Content/no_image.png";
            
        //    try
        //    {
        //        if (ControllerContext.HttpContext.Request.Cookies["ShortListCookie"] != null && ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].ToString() != string.Empty) //
        //        {

        //            string cookShortListItems = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value;
        //            string checkItmId = myParam.productID + "$" + myParam.shopStockID + "$" + myParam.productName + "$" + imgUrl;
        //            if (!cookShortListItems.Contains(checkItmId))
        //            {
        //                ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value +
        //                    "," + myParam.productID + "$" + myParam.shopStockID + "$" + myParam.productName + "$" + imgUrl;
        //            }
        //            else
        //            {
        //                //Initialize cookie again because , cookie value overwrite by null if we are not initialize cookie
        //                ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value;
        //            }
        //        }
        //        else
        //        {
        //            ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = myParam.productID + "$" + myParam.shopStockID + "$" + myParam.productName + "$" + imgUrl;
        //        }
        //        ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Expires = DateTime.Now.AddDays(1);
        //        ControllerContext.HttpContext.Response.AppendCookie(ControllerContext.HttpContext.Response.Cookies["ShortListCookie"]);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return Json("true", JsonRequestBehavior.AllowGet);
        //    ////System.Threading.Thread.Sleep(1000);
        //    //long lCustID = 0, lShopStockID = 0;
        //    //Session["UID"] = 1;
        //    //long.TryParse(Session["UID"].ToString(), out lCustID);
        //    //long.TryParse(shopStockID, out lShopStockID);
        //    //CustomerWishlist cw = new CustomerWishlist(System.Web.HttpContext.Current.Server);
        //    //int oprStatus = cw.SetWishlist(lCustID, lShopStockID);
        //    //return Json(oprStatus.ToString(), JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// This method add product in shortlist from "Product" and "Preview" page
        /// </summary>
        /// <param name="shopStockID"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        public JsonResult AddProductToShortList(WebmethodParams myParam)
        {
            var ColorName = from SS in db.ShopStocks
                            join PV in db.ProductVarients on SS.ProductVarientID equals PV.ID
                            join C in db.Colors on PV.ColorID equals C.ID
                            where SS.ID == myParam.shopStockID
                            select new { C.Name };

            string imgUrl = "";
            if (ColorName.FirstOrDefault().Name == "N/A")
            {
                //imgUrl = ImageDisplay.LoadProductThumbnails(myParam.productID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                imgUrl = ImageDisplay.SetProductThumbPath(myParam.productID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            }
            else
            {
                //change by harshada to image display
                //imgUrl = ImageDisplay.LoadProductThumbnails(myParam.productID, ColorName.FirstOrDefault().Name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                imgUrl = ImageDisplay.SetProductThumbPath(myParam.productID, ColorName.FirstOrDefault().Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            }
            try
            {
                if (ControllerContext.HttpContext.Request.Cookies["ShortListCookie"] != null && ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].ToString() != string.Empty) //
                {

                    string cookShortListItems = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value;
                    string checkItmId = myParam.productID + "$" + myParam.shopStockID + "$" + myParam.productName + "$" + imgUrl;
                    if (!cookShortListItems.Contains(checkItmId))
                    {
                        ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value +
                            "," + myParam.productID + "$" + myParam.shopStockID + "$" + myParam.productName + "$" + imgUrl;
                    }
                    else
                    {
                        //Initialize cookie again because , cookie value overwrite by null if we are not initialize cookie
                        ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value;
                    }
                }
                else
                {
                    ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = myParam.productID + "$" + myParam.shopStockID + "$" + myParam.productName + "$" + imgUrl;
                }
                ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Expires = DateTime.Now.AddDays(1);
                ControllerContext.HttpContext.Response.AppendCookie(ControllerContext.HttpContext.Response.Cookies["ShortListCookie"]);
            }
            catch (Exception)
            {

                throw;
            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }
       

        public JsonResult RemoveShortListItem(string shopStockID)
        {
            try
            {
                if (ControllerContext.HttpContext.Request.Cookies["ShortListCookie"] != null && ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].ToString() != string.Empty) //
                {
                    string P = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value;
                    string[] individualItemCookie = P.Split(',');
                    HttpCookie ShortListCookie = new HttpCookie("ShortListCookie");

                    //Delete whole cookie
                    if (ControllerContext.HttpContext.Request.Cookies["ShortListCookie"] != null)
                    {
                        ShortListCookie.Expires = DateTime.Now.AddDays(-1);
                        ControllerContext.HttpContext.Response.Cookies.Add(ShortListCookie);
                    }
                    if (ShortListCookie.Expires < DateTime.Now)
                    {
                        ControllerContext.HttpContext.Request.Cookies.Remove("ShortListCookie");
                    }
                    foreach (string item in individualItemCookie)
                    {
                        string[] individualItemDetailsCookie = item.Split('$');
                        if (ControllerContext.HttpContext.Request.Cookies["ShortListCookie"] == null)
                        {
                            if (individualItemDetailsCookie.Length > 3)
                            {
                                if (individualItemDetailsCookie[1] != shopStockID)
                                {
                                    ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = individualItemDetailsCookie[0] + "$" + individualItemDetailsCookie[1] + "$" + individualItemDetailsCookie[2]
                                   + "$" + individualItemDetailsCookie[3];
                                    ControllerContext.HttpContext.Response.Cookies.Add(ShortListCookie);
                                    ShortListCookie.Expires = System.DateTime.Now.AddDays(30);
                                }
                            }
                           
                        }
                        else if (individualItemDetailsCookie[1] != shopStockID)
                        {
                            if (individualItemDetailsCookie.Length > 3)
                            {
                                ControllerContext.HttpContext.Response.Cookies["ShortListCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShortListCookie"].Value + ","
                                    + individualItemDetailsCookie[0] + "$" + individualItemDetailsCookie[1] + "$" + individualItemDetailsCookie[2] + "$" + individualItemDetailsCookie[3];
                                ControllerContext.HttpContext.Response.Cookies.Add(ShortListCookie);
                                ShortListCookie.Expires = System.DateTime.Now.AddDays(30);
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json("true", JsonRequestBehavior.AllowGet);
         
        }

        #endregion

        //-----------------------------------------------------------------------
        // <copyright file="CustomerDetails.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
        //     Copyright  Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
        // </copyright>
        // <author>Tejaswee Taktewale</author>
        //-----------------------------------------------------------------------

        #region Shopping Cart

        public ActionResult SetCookie(long ShopStockID, long itemID)
        {
            // HttpCookie ShoppingCartCookie = new HttpCookie("ShoppingCartCookie");
            if (ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"] == null)
            {
                //Add Item Details cookie
                ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ShopStockID + "$" + itemID + "$" + 1;
                //ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
                ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"]);
                ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
            }
            else
            {
                string cook = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
                string[] ItemID = cook.Split(',');
                // if (ItemID.Contains(ShopStockID + "$" + itemID + "$" + 1))
                if (cook.Contains(ShopStockID + "$" + itemID + "$"))
                {
                    ////prevent to add duplicate item in shopping cart
                    /*************************** If we not reInitialize cookie then Cookie gets cleared****************************/
                    ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
                    //ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
                    ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"]);
                    ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
                }
                else
                {
                    ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value + "," +
                        ShopStockID.ToString() + "$" + itemID.ToString() + "$" + "1";
                    ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"]);
                    ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
                }


                //ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value + "";
            }
            //ShoppingCartCookie.Value = "4$1,5$1";
            // ControllerContext.HttpContext.Response.Cookies.Add(ShoppingCartCookie);
            //ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"]);
            //ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
            return RedirectToAction("Index", "ShoppingCart");
        }

        //[HttpPost]
        public ActionResult ExpressBuy(long ShopStockID, long itemID, string couponCode, double? couponAmount, double? couponPercent, string productQuantity)
        
        {
            try
            {
               // string e = frm["productQuantity"];
                if (couponCode == string.Empty)
                {
                    couponAmount = 0;
                    couponPercent = 0;
                }
                string lproductQuantity = productQuantity.ToString();
                this.ExpressBuyProduct(ShopStockID, itemID, couponCode, Convert.ToDouble(couponAmount), Convert.ToDouble(couponPercent), lproductQuantity);
                this.GetExpressBuyDetail();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PreviewItemController][GET:ExpressBuy]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PreviewItemController][GET:ExpressBuy]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("CustomerPaymentProcess", "PaymentProcess", new { IsExpressBuy=true});
        }

        private void ExpressBuyProduct(long ShopStockID, long itemID, string couponCode, double couponAmount, double couponPercent, string productQuantity)
        { 
            try
            {
                string a=Request.Form["productQuantity"];
                if (ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"] != null)
                {
                    this.ClearExpressBuyCookie();
                }

                ControllerContext.HttpContext.Response.Cookies["ExpressBuyCookie"].Value = ShopStockID + "$" + itemID + "$" + productQuantity;
                ControllerContext.HttpContext.Response.Cookies["ExpressBuyCookie"].Expires = DateTime.Now.AddDays(30);
                ControllerContext.HttpContext.Response.AppendCookie(ControllerContext.HttpContext.Response.Cookies["ExpressBuyCookie"]);

                if (couponCode != string.Empty)
                {
                    BusinessLogicLayer.CouponManagement coupon = new BusinessLogicLayer.CouponManagement(); 
                    coupon.SetCouponCookie(couponCode, ShopStockID, couponAmount, couponPercent);
            }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:ExpressBuyProduct]", "Can't get payable price !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:ExpressBuyProduct]", "Can't get payable price !" + Environment.NewLine + ex.Message);
            }
        }

        private void ClearExpressBuyCookie()
        {
            try
            {
                /***************************************For Clearing Cookies*****************************************/
                HttpCookie ExpressBuyCookie = new HttpCookie("ExpressBuyCookie");
                if (ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"] != null)
                {
                    ExpressBuyCookie.Expires = DateTime.Now.AddDays(-1);
                    ControllerContext.HttpContext.Response.Cookies.Add(ExpressBuyCookie);
                }
                if (ExpressBuyCookie.Expires < DateTime.Now)
                {
                    ControllerContext.HttpContext.Request.Cookies.Remove("ExpressBuyCookie");
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:ClearExpressBuyCookie]", "Can't clear express buy cookie !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PreviewItemController][M:ClearExpressBuyCookie]", "Can't clear express buy cookie !" + Environment.NewLine + ex.Message);
            }
        }

        private void GetExpressBuyDetail()
        {
            try
            {
                if (ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"] != null)
                {
                    string[] ItmPrice = ControllerContext.HttpContext.Request.Cookies["ExpressBuyCookie"].Value.Split(',');
                    ShoppingCartInitialization sci = new ShoppingCartInitialization();
                    string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
                    ShopProductVarientViewModelCollection lShoppingCartCollection = sci.GetCookie(ItmPrice, fConnectionString);
                    DeliveryCharges dc = new DeliveryCharges();
                    ShoppingCartOrderDetails scartOrder = new ShoppingCartOrderDetails();
                    //scartOrder.CoupenAmount = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenAmount;
                    //scartOrder.CoupenCode = lShoppingCartCollection.lShoppingCartOrderDetails.CoupenCode;
                    scartOrder.NoOfPointUsed = 0;
                    scartOrder.ValuePerPoint = 0;
                    //Grand Total
                    scartOrder.TotalOrderAmount = Convert.ToInt32(Math.Round(lShoppingCartCollection.lShopProductVarientViewModel[0].SaleRate * lShoppingCartCollection.lShopProductVarientViewModel[0].PurchaseQuantity));
                    scartOrder.BusinessPointsTotal = lShoppingCartCollection.lShopProductVarientViewModel[0].BusinessPointPerUnit * lShoppingCartCollection.lShopProductVarientViewModel[0].PurchaseQuantity; //Added by Zubair for MLM on 05-01-2018
                    //Offer Less, Delivery Charges Add in Grand Total
                    decimal deliveryCharge = dc.GetDeliveryCharges(ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value, lShoppingCartCollection.lShopProductVarientViewModel[0].ActualWeight, true, scartOrder.TotalOrderAmount);


                    decimal totalCouponAmount = 0;
                    foreach(var item in lShoppingCartCollection.lShopProductVarientViewModel)
                    {
                        if (item.CouponValueRs > 0)
                        {
                            totalCouponAmount += Convert.ToDecimal(item.CouponValueRs);
                        }
                        else if (item.CouponValuePercent > 0)
                        {
                            totalCouponAmount += (Convert.ToDecimal(item.SaleRate) * Convert.ToDecimal(item.CouponValuePercent)) / 100;
                        }
                    }
                    decimal prodTaxAmt = 0;
                    if (lShoppingCartCollection.lCalculatedTaxList != null)
                    {
                        foreach (var item in lShoppingCartCollection.lCalculatedTaxList)
                        {

                            prodTaxAmt = prodTaxAmt + item.Amount;
                        }
                    }

                    scartOrder.PayableAmount = Convert.ToInt32(Math.Round(lShoppingCartCollection.lShopProductVarientViewModel[0].SaleRate 
                        * lShoppingCartCollection.lShopProductVarientViewModel[0].PurchaseQuantity))
                        + deliveryCharge 
                        - totalCouponAmount
                        + prodTaxAmt;
                    //scartOrder.PayableAmount = Convert.ToInt32(50);
                    lShoppingCartCollection.lShoppingCartOrderDetails = scartOrder;

                    ShopWiseDeliveryCharges lShopWiseDeliveryCharges = new ShopWiseDeliveryCharges();
                    //===================== Initialize Property ======================================
                    List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();
                    lShopWiseDeliveryCharges.ShopID = lShoppingCartCollection.lShopProductVarientViewModel[0].ShopID;
                    //lShopWiseDeliveryCharges.DeliveryCharge = Convert.ToInt32(Math.Round(lShoppingCartCollection.lShopWiseDeliveryCharges[0].DeliveryCharge));
                    lShopWiseDeliveryCharges.DeliveryCharge = deliveryCharge;
                    lShopWiseDeliveryCharges.OrderAmount = Convert.ToInt32(Math.Round(lShoppingCartCollection.lShopProductVarientViewModel[0].SaleRate * lShoppingCartCollection.lShopProductVarientViewModel[0].PurchaseQuantity));
                    lShopWiseDeliveryCharges.Weight = lShoppingCartCollection.lShopProductVarientViewModel[0].ActualWeight;
                    lShopWiseDeliveryCharges.DeliveryType = "Express";
                    listShopWiseDeliveryCharges.Add(lShopWiseDeliveryCharges);

                    //==================== Add Delivery schedule Id (add first slot for express buy) ===============//
                    //DeliveryScheduleBLL lDeliveryScheduleBLL = new DeliveryScheduleBLL(System.Web.HttpContext.Current.Server);
                    ////List<DeliverySchedule1> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule();
                    //long cityId = Convert.ToInt64(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                    //List<DeliveryScheduleViewModel> lDeliverySchedule = lDeliveryScheduleBLL.SetDeliverySchedule(cityId, ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"].Value);
                    //lShoppingCartCollection.DeliveryScheduleID = lDeliverySchedule.FirstOrDefault().delScheduleId;

                    lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;
                    TempData["ExpressBuyCollection"] = lShoppingCartCollection;

                    //return RedirectToAction("CustomerPaymentProcess", "PaymentProcess");
                }
                
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
        }

        #endregion

        public JsonResult CheckCouponCode(string couponCode, long organisationId, long itemId)
        {
            BusinessLogicLayer.CouponManagement couponManagement = new BusinessLogicLayer.CouponManagement(); 
            
            long userLoginId = 0, customerId = 0;
            long.TryParse(Convert.ToString(Session["UID"]), out userLoginId);

            DataTable dtCouponDetails = new DataTable();

            if (userLoginId > 0)
            {
                customerId = CommonFunctions.GetPersonalDetailsID(userLoginId);
                long cityId =Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[0]);
                int franchiseId = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                dtCouponDetails = couponManagement.CheckCouponCode(couponCode, organisationId, itemId, customerId, cityId, franchiseId);////added cityId-> franchiseId old
            }
            else
                dtCouponDetails = couponManagement.CheckCouponCode(couponCode, organisationId, itemId);


            List<CouponDetailsViewModel> listCouponDetails = new List<CouponDetailsViewModel>();

            if (dtCouponDetails.Rows.Count > 0)
            {
                listCouponDetails = (from DataRow dr in dtCouponDetails.Rows
                                     select new CouponDetailsViewModel()
                                     {
                                         VoucherAmount = Convert.ToDouble(dr["Voucher_Amount"]),
                                         VoucherPercent = Convert.ToDouble(dr["Voucher_Percent"]),
                                         MinimumPurchaseAmount = Convert.ToDouble(dr["Min_Purchase_Amt"]),
                                         MinimumPurchaseQuantity = Convert.ToDouble(dr["Min_Purchase_Qty"]),
                                         IsFreeDelivery = Convert.ToBoolean(dr["Is_Free_Delivery"]),
                                         Result = Convert.ToInt32(dr["VALIDITY CODE"])
                                     }).ToList();

                if (Convert.ToInt32(dtCouponDetails.Rows[0]["VALIDITY CODE"]) == 1)
                {
                    bool isCouponUsed = couponManagement.GetCouponCookie(couponCode);
                    if (isCouponUsed)
                    {
                        return Json("true", JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(listCouponDetails, JsonRequestBehavior.AllowGet);
        }


        private long SetDeliveryTime(long ProductID)
        { 
            long DelT = 0;
            try
            {
                var DeliveryTime = db.ShopProducts.Where(x => x.ProductID == ProductID).Select(x => x.DeliveryTime).FirstOrDefault();
                if (DeliveryTime != null)
                {
                    DelT = Convert.ToInt64(DeliveryTime);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return DelT;
        }

        private List<RefineCategoryViewModel> GetCategoryList(int CategoryID, long franchiseID)
        {
            try
            {
                int? Parent = db.Categories.Where(x => x.ID == CategoryID).Select(x => x.ParentCategoryID).FirstOrDefault();
                List<RefineCategoryViewModel> categoryList = new List<RefineCategoryViewModel>();

                categoryList = (from fm in db.FranchiseMenus
                                join c1 in db.Categories on fm.CategoryID equals c1.ID
                                join c2 in db.Categories on c1.ParentCategoryID equals c2.ID
                                join c3 in db.Categories on c2.ParentCategoryID equals c3.ID
                                join p in db.Products on c1.ID equals p.CategoryID
                                join sp in db.ShopProducts on p.ID equals sp.ProductID
                                join s in db.Shops on sp.ShopID equals s.ID
                                //join f in db.Franchises on fm.FranchiseID equals f.ID
                                //join pc in db.Pincodes on f.PincodeID equals pc.ID
                                where c1.IsActive == true && c2.IsActive == true && c3.IsActive == true
                                && (fm.IsExpire == true && fm.ExpiryDate > DateTime.Now) || fm.IsExpire == false
                                && p.IsActive == true && sp.IsActive == true && s.IsActive == true && fm.IsActive == true
                                && c2.ID == Parent && s.IsLive == true
                                    //&& pc.CityID==cityID
                               // && fm.Franchise.Pincode.CityID == cityID
                               //&& s.Pincode.CityID == cityID
                                && fm.FranchiseID == franchiseID
                               && s.FranchiseID == franchiseID
                                group p by new { fm.CategoryID, fm.CategoryName, fm.SequenceOrder, fm.Level, c2.ID, c2.Name } into temp
                                //where temp.Count()>1
                                //group by FM.CategoryID, FM.CategoryName, FM.SequenceOrder, FM.[Level],C2.ParentCategoryID ,C2.Name
                                select new RefineCategoryViewModel
                                {
                                    //ID=c2.ID,
                                    //Name=c2.Name.ToLower(),
                                    //SecondLevelCatID=c1.ID,
                                    //SecondLevelCatName=c1.Name.ToLower()
                                    ID = temp.Key.CategoryID,
                                    Name = temp.Key.CategoryName.ToLower(),
                                    SecondLevelCatID = temp.Key.ID,
                                    SecondLevelCatName = temp.Key.Name.ToLower()


                                }
                 ).Distinct().ToList();

                return categoryList;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Geting value from API BussinessLayer for Dynamic Message
        /// By Ashish
        /// </summary>
        /// <returns></returns>
        private List<WeeklySeasonalFestivalPageMessage> CallGetPageMessageAPI()
        {
            int FranchiseID = Convert.ToInt32(ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2].Trim());

            List<WeeklySeasonalFestivalPageMessage> WSFMsg = new List<WeeklySeasonalFestivalPageMessage>();
            WSFMsg = FranchisePageMessages.GetFranchisePageMessage(FranchiseID);

            return WSFMsg;
        }
    }
}