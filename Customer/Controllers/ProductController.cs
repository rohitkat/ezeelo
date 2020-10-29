
//-----------------------------------------------------------------------
// <copyright file="ProductController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Gaurav Dixit</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using ModelLayer.Models;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Threading;
using Gandhibagh.Models;
using System.Text.RegularExpressions;
using System.Device.Location;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Web.Configuration;
using System.Collections;


namespace Gandhibagh.Controllers
{
    public class ProductController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        private string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        /// <summary>
        /// This class is used as parameter when loading products on scroll.
        /// </summary>
        public class WebmethodParams
        {
            public int cityId { get; set; }
            public int franchiseId { get; set; }////added 
            public string keyword { get; set; }
            public int shopId { get; set; }
            public int categoryId { get; set; }
            public string brands { get; set; }
            public string colors { get; set; }
            public string sizes { get; set; }
            public string dimensions { get; set; }
            public string catDescID { get; set; }
            public string catDescValue { get; set; }
            public double minPrice { get; set; }
            public double maxPrice { get; set; }
            public int pageIndex { get; set; }
            public int pageSize { get; set; }
            public bool FromPaging { get; set; }

            public int SortVal { get; set; }

            public bool FromSorting { get; set; }
        }

        public class Area
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int PincodeID { get; set; }
        }


        //Added by Zubair on 16-09-2017 for filling CityCookie for SEO Schema
        //if link is directly hitted from google search
        public class GetCityIdFranchiseIDContact3 ////added GetCityIdFranchiseIDContact
        {
            public long cityId { get; set; }
            public int franchiseId { get; set; }
            public string contact { get; set; }
        }
        //End by Zubair

        public ActionResult ShopSearch(string shopName)
        {
            EzeeloDBContext db = new EzeeloDBContext();
            string cityName = "";
            int franchiseId = 0;////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null && ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value != "")
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                cityName = cookieValue.Split('$')[1].ToLower();
                franchiseId = Convert.ToInt32(cookieValue.Split('$')[2]);////added
            }

            var shopList = (from sh in db.Shops
                            where sh.Name.Contains(shopName) && sh.IsLive == true && sh.IsActive == true
                            select new
                            {
                                ShopID = sh.ID,
                                ShopName = sh.Name
                            }).ToList();


            if (shopList != null && shopList.Count() > 0)
            {
                shopName = Regex.Replace(shopList.First().ShopName, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and").ToLower();
                //Response.Redirect("/Product/Products?shopID=" + shopList.First().ShopID + "&shopName=" + shopList.First().ShopName);
                // return RedirectToRoute("ShopSearch2", new { shopID = shopList.First().ShopID, shopName = shopName, city = cityName });//hide
                return RedirectToRoute("ShopSearch2", new { shopID = shopList.First().ShopID, shopName = shopName, city = cityName, FranchiseID = franchiseId });//--added by Ashish for multiple franchise in same city--//
            }
            else
            {

                ViewBag.NoshopFoundMessage = shopName;
                return View("eShop");
                //Response.Redirect("/Product/Products?shopID=0&shopName=NoShopFound");

            }


            return RedirectToAction("Products", new { parentCategoryId = 0, selectedCategoryId = 0, item = string.Empty, shopID = 1 });
        }

        [SessionExpire]
        [DynamicMetaTag]
        public ActionResult Products(int? parentCategoryId, int? selectedCategoryId, string item, int? shopID)
        {
            //Yashaswi 21-02-2019 For BUG 716 To display Heading at Category Page
            ViewBag.CategoryHeaderName = item.Replace("-", " ");
            long cityID = 0;
            int franchiseID = 0;////added

            if (Request.QueryString["item"] != null)
            {
                TempData["SearchKeyword"] = Request.QueryString["item"].ToString();
                item = TempData["SearchKeyword"].ToString();
                TempData.Keep("SearchKeyword");

            }
            else if (TempData["SearchKeyword"] != null && !string.IsNullOrEmpty(item) && item.Trim().Equals(TempData["SearchKeyword"].ToString()))
            {
                item = TempData["SearchKeyword"].ToString();
            }
            else
            {
                item = string.Empty;
            }
            /*
               Indents:
             * Description: This controller action method is used for both global product listing/shop wise product listing.
             *              In this action method we can find items either of particular city or whole eZeelo.
             
             * Parameters: parentCategoryId: When user want to search any item, then user enter some character in search box then we provide 
             *             some suggestion related to enter character, And if user select item from suggestion then this variable contains 
             *             categoryId of selected suggestion.
             *             
             *             selectedCategoryId: If user select another category from category section then this variable contains 
             *             current selected Category Id. User can select only one category at a time.
             *             
             *              item: Contains name of item to be searched(it is used as a search keyword)
             *              
             *              shopID: This variable contains shopId(If user is comming from particular shop).
             *             
             * 
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Get Shop Id
             *        2) Get CityId from cookie
             *        3) Find BreadCrum done by tejaswee
             *        4) Call GetProducts() to get products according to selected criteria and display to user. This method returns
             *        productViewModel type object which contains ProductList, Related Brand, Category,  Color, Size, Dimension, Price related
             *        refinement.
             *        5) If shopId>0 
             *        5.1) If TempData["CatList"] == null then Store selected category in TempData["CatList"] and that will be used when user 
             *              search product according to refinement and by using this TempData["CatList"], we set category list property of
             *              productViewModel. so user can see all category related to his/her search.
             *          Else Set TempData["CatList"] == null (Reason: Suppose first time, we search laptop then TempData["CatList"] will store 
                                             *                   laptop category, next time we search desktop 
                                             *                   so TempData["CatList"]  should store desktop related category. so for this we have to set TempData["CatList"]
                                             *                   to null. because we are using TempData.Keep())
             *                   
             *         6) if lShopID == 0 that means we have to display products page otherwise we will display eShop page.
             *             
             */

            /*------Code by Harshada to save url in cookie---*/
            //Add Item Details cookie
            //string url = Request.Url.ToString();
            //Response.Cookies["UrlCookie"].Value = url;
            ////ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
            //Response.Cookies.Add(Response.Cookies["UrlCookie"]);
            //Response.Cookies["UrlCookie"].Expires = System.DateTime.Now.AddDays(30);

            //Set Cookie for Url saving & Use in Continue shopping
            ViewBag.SearchKeyWord = item;
            //================ Manoj Yadav ===================================================================
            long UserID = Convert.ToInt64(Session["UID"]);
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                // TrackSearchBusiness.InsertSearchDetails(UserID, Convert.ToInt64(parentCategoryId), Convert.ToInt64(shopID), item, "", "", "", "", cookieValue.Split('$')[1], "");//hide
                TrackSearchBusiness.InsertSearchDetails(UserID, Convert.ToInt64(parentCategoryId), Convert.ToInt64(shopID), item, "", "", "", "", cookieValue.Split('$')[1], "", Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//
            }
            else
            {
                //Added by Zubair on 16-09-2017 for filling CityCookie for SEO Schema
                //if link is directly hitted from google search
                //string cname = Request.Url.ToString().Replace("http://beta.customer.ezeelo.in/", "");
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
                                                select new GetCityIdFranchiseIDContact3
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

                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    // TrackSearchBusiness.InsertSearchDetails(UserID, Convert.ToInt64(parentCategoryId), Convert.ToInt64(shopID), item, "", "", "", "", cookieValue.Split('$')[1], "");//hide
                    TrackSearchBusiness.InsertSearchDetails(UserID, Convert.ToInt64(parentCategoryId), Convert.ToInt64(shopID), item, "", "", "", "", cookieValue.Split('$')[1], "", Convert.ToInt32(cookieValue.Split('$')[2]));//--added by Ashish for multiple franchise in same city--//
                }
            }
            //End by Zubair


            //================ END ===========================================================================

            URLCookie.SetCookies();
            ProductViewModel productViewModel = new ProductViewModel();
            PartialProductListParam partialProductListParam = new PartialProductListParam();
            // 1
            int lShopID = 0;
            int.TryParse(Convert.ToString(shopID), out lShopID);

            try
            {
                int cityId = 0;
                int franchiseId = 0;////added
                // 2 
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    cityId = Convert.ToInt32(arr[0]);
                    franchiseId = Convert.ToInt32(arr[2]);////added

                    ViewBag.CityID = cityId;
                    ViewBag.FranchiseID = franchiseId;////added
                }
                //============= (Tejaswee) load page with same page index if user view product or login and return back to same page==============
                if ((TempData["ParentCatID"] != null && Convert.ToInt32(TempData["ParentCatID"]) == parentCategoryId)
                    || (TempData["Item"] != null && TempData["Item"] != string.Empty && TempData["Item"].ToString() == item)
                    || (TempData["ShopID"] != null && Convert.ToInt32(TempData["ShopID"]) == shopID) && selectedCategoryId == null)
                {
                    TempData.Keep("CurrentPageIndex");
                }
                else
                {
                    TempData["CurrentPageIndex"] = 1;
                }

                //============= Tejaswee ==============
                // 3 BreadCrum done by tejaswee
                if (parentCategoryId != null && parentCategoryId > 0)// Third level category id
                {
                    // Tejaswee
                    Bedcrumbs bc = new Bedcrumbs();
                    string bedcrumbString = bc.GetCategoryHierarchy(Convert.ToInt32(parentCategoryId));
                    ViewBag.Bedcrumb = bedcrumbString;
                    //1.Paging back
                    TempData["ParentCatID"] = parentCategoryId;
                }
                //2. Paging back
                TempData["Item"] = item;
                // 4
                //productViewModel = this.GetProducts(cityId, parentCategoryId, selectedCategoryId, item, lShopID);//hide
                productViewModel = this.GetProducts(cityId, franchiseId, parentCategoryId, selectedCategoryId, item, lShopID);//--added by Ashish for multiple franchise in same city--//


                if (selectedCategoryId != null)
                {
                    TempData["selectedCategoryId"] = selectedCategoryId;
                    TempData.Keep("selectedCategoryId");
                }
                else
                {
                    TempData.Remove("selectedCategoryId");
                }


                if (productViewModel.productList.Count() == 0 && !item.Trim().Equals(string.Empty))
                {
                    ViewBag.NoProductsFoundMessage = item;
                    return View("Products");

                }

                Int64 categoryID = productViewModel.categoryList[0].ID;
                EzeeloDBContext db = new EzeeloDBContext();
                if (db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Level == 3)
                {
                    categoryID = Convert.ToInt64(db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().ParentCategoryID);
                    string CategoryName = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name.Trim();

                    ViewBag.desc_msg = this.GetCategoryDescription(CategoryName);
                    //ViewBag.desc_msg = this.GetCategoryDescription("Security System");

                    /*Added by Pradnyakar Badge 
                     * for Compare Product Utility
                     * On Dated :- 02-12-2015
                     */
                    Int64 CompareCategoryLevelTwo = 0, CompareThirdLevelCategory;
                    CompareThirdLevelCategory = productViewModel.categoryList[0].ID;
                    CompareCategoryLevelTwo = Convert.ToInt64(db.Categories.Where(x => x.ID == CompareThirdLevelCategory && x.Level == 3).FirstOrDefault().ParentCategoryID);
                    ViewBag.CompareCategoryID = Convert.ToInt64(db.Categories.Where(x => x.ID == CompareCategoryLevelTwo && x.Level == 2).FirstOrDefault().ParentCategoryID);


                }

                /*Tejaswee
                 * For SEO H1 Tag on the Category Page
                 * 08-Jan-2016                 
                 */
                SEO ldata = new SEO();

                ldata = db.SEOs.Where(x => x.EntityID == parentCategoryId && x.BusinessType.Prefix == "CTHD").FirstOrDefault();
                if (ldata != null)
                {
                    TempData["MetaData_Category"] = ldata.H1;

                }

                // 5
                if (shopID > 0)
                {
                    if (productViewModel.shopDetails.ShopBasicDetails != null)
                    {
                        TempData["CatList"] = productViewModel.categoryList;
                        //if (TempData["CatList"] == null) // 5.1
                        //    TempData["CatList"] = productViewModel.categoryList;
                        //else
                        //    productViewModel.categoryList = (List<RefineCategoryViewModel>)TempData["CatList"];
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    //3. Paging back
                    TempData["ShopID"] = lShopID;
                    TempData.Keep();

                    //use for showing sorting dropdown... 
                    //If category Id > 0 then sorting dropdown will show
                    TempData["CatID"] = selectedCategoryId;
                }
                else
                {
                    TempData["CatList"] = null;

                    //use for showing sorting dropdown... 
                    //If category Id > 0 then sorting dropdown will show
                    TempData["CatID"] = parentCategoryId;
                }

                partialProductListParam.productList = productViewModel.productList;
                ViewBag.ProductList = partialProductListParam;

                ViewBag.currentPageIndex = 1;
                //============== Manoj ======================
                ViewBag.SessionValue = Convert.ToInt64(Session["UID"]);

                CommonFunctions obj = new CommonFunctions();
                string telNo = obj.GetCustCareNo();
                TempData["CustCareNo"] = telNo;

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductController][GET:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("SomethingWrong");
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductController][GET:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return View("SomethingWrong");
            }

            // 6
            if (lShopID == 0)
            {
                return View("Products", productViewModel);
            }
            else
            {
                return View("eShop", productViewModel);
            }
        }


        [HttpPost]
        public ActionResult Products(ProductViewModel model, FormCollection form)
        {
            /*
               Indents:
             * Description: This action method respond to POST request.  Show products according to selected refinement and rebind refinements
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Create object of productViewModel
             *        2) Get shopId from form collection
             *        3) Get cityId from cookie
             *        4) Call PostProducts() which will return products according to selected refinement.
             *        5) Same as Get Request
             */

            ProductViewModel productViewModel = new ProductViewModel(); // 1

            // 2
            int lShopID = 0;
            int.TryParse(form["ShopID"], out lShopID);

            try
            {
                // 3
                int cityId = 0;
                int franchiseId = 0;////added
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    cityId = Convert.ToInt32(arr[0]);
                    franchiseId = Convert.ToInt32(arr[2]);////added

                    ViewBag.CityID = cityId;
                    ViewBag.FranchiseID = franchiseId;////added
                }

                ViewBag.ShopID = lShopID;
                // 4
                productViewModel = this.PostProducts(model, form);

                //Tejaswee 4-5-2016
                TempData["CurrentPageIndex"] = 1;
                // 5
                if (lShopID > 0)
                {
                    TempData["CatList"] = productViewModel.categoryList;
                    //if (TempData["CatList"] == null)
                    //    TempData["CatList"] = productViewModel.categoryList;
                    //else
                    //    productViewModel.categoryList = (List<RefineCategoryViewModel>)TempData["CatList"];

                    TempData.Keep();
                }
                else
                {
                    TempData["CatList"] = null;
                }


                /*Added by Pradnyakar Badge 
                 * for Compare Product Utility
                 * On Dated :- 02-12-2015
                 */
                Int64 CompareCategoryLevelTwo = 0;


                int lParentCategory = 0, lSelectedCategory = 0;

                int.TryParse(Convert.ToString(form["ParentCategory"]), out lParentCategory);
                int.TryParse(Convert.ToString(TempData["selectedCategoryId"]), out lSelectedCategory);


                int lCategory = 0;

                if (lSelectedCategory > 0)
                    lCategory = lSelectedCategory;
                else
                    lCategory = lParentCategory;

                CompareCategoryLevelTwo = Convert.ToInt64(db.Categories.Where(x => x.ID == lCategory && x.Level == 3).FirstOrDefault().ParentCategoryID);
                ViewBag.CompareCategoryID = Convert.ToInt64(db.Categories.Where(x => x.ID == CompareCategoryLevelTwo && x.Level == 2).FirstOrDefault().ParentCategoryID);

                /*End of Compare PRoduct Requirement*/

                CommonFunctions obj = new CommonFunctions();
                string telNo = obj.GetCustCareNo();
                TempData["CustCareNo"] = telNo;

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductController][POST:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductController][POST:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }

            if (lShopID == 0)
                return View("Products", productViewModel);
            else
                return View("eShop", productViewModel);
        }

        public ActionResult GetProductOnScroll(WebmethodParams myParam)
        {
            /*
               Indents:
             * Description: This method is used to load products on scroll. This method is called from jquery which is written in Product.js file
             
             * Parameters: WebmethodParams myParam: This class contains the value for all selected filter like brand Id's, category Id's, cityId, 
             *                                      size Id's etc.
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Get all parameter value from object passed using jquery.
             *        2) Call GetSearchData() to get data from database 
             *        3) Return json response to caller jquery function and display using jqeury written in Product.js
             */

            ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();
            PartialProductListParam objProductList = new PartialProductListParam();
            try
            {
                //1
                int lCatId = 0, lPageIndex = 0, lPageSize = 0, lShopID = 0, lCityId = 0;
                int lFranchiseID = 0;////added
                decimal lMinPrice = 0, lMaxPrice = 0;

                //note
                //Add one more parameter i.e fromSorting



                if (TempData["selectedCategoryId"] != null)
                {
                    int.TryParse(Convert.ToString(TempData["selectedCategoryId"]), out lCatId);
                    TempData.Keep("selectedCategoryId");
                }
                else
                {
                    int.TryParse(Convert.ToString(myParam.categoryId), out lCatId);
                }
                int.TryParse(Convert.ToString(myParam.cityId), out lCityId);
                int.TryParse(Convert.ToString(myParam.franchiseId), out lFranchiseID);////added
                int.TryParse(Convert.ToString(myParam.pageSize), out lPageSize);
                int.TryParse(Convert.ToString(myParam.pageIndex), out lPageIndex);
                decimal.TryParse(Convert.ToString(myParam.minPrice), out lMinPrice);
                decimal.TryParse(Convert.ToString(myParam.maxPrice), out lMaxPrice);
                int.TryParse(Convert.ToString(myParam.shopId), out lShopID);



                //if (TempData["CurrentPageIndex"] != null && Convert.ToInt32(TempData["CurrentPageIndex"]) > lPageIndex && !myParam.FromPaging)
                //{
                //    lPageIndex = Convert.ToInt32(TempData["CurrentPageIndex"]);
                //}
                if (myParam.FromSorting == true)
                {
                    lPageIndex = 1;
                }
                else if (TempData["CurrentPageIndex"] != null && Convert.ToInt32(TempData["CurrentPageIndex"]) > lPageIndex && !myParam.FromPaging)
                {
                    lPageIndex = Convert.ToInt32(TempData["CurrentPageIndex"]);
                }

                // 2
                //productWithRefinementViewModel = this.GetSearchData(lCityId, lShopID,(myParam.keyword ?? "").ToString(), lCatId, Convert.ToString(myParam.brands), Convert.ToString(myParam.colors), Convert.ToString(myParam.sizes), Convert.ToString(myParam.dimensions), Convert.ToString(myParam.catDescID), Convert.ToString(myParam.catDescValue), lMinPrice, lMaxPrice, lPageIndex, lPageSize, 0, 0, true,myParam.SortVal);//hide
                productWithRefinementViewModel = this.GetSearchData(lCityId, lFranchiseID, lShopID, (myParam.keyword ?? "").ToString(), lCatId, Convert.ToString(myParam.brands), Convert.ToString(myParam.colors), Convert.ToString(myParam.sizes), Convert.ToString(myParam.dimensions), Convert.ToString(myParam.catDescID), Convert.ToString(myParam.catDescValue), lMinPrice, lMaxPrice, lPageIndex, lPageSize, 0, 0, true, myParam.SortVal);////added lFranchiseID
                ViewBag.currentPageIndex = lPageIndex;
                ViewBag.PageCntForPaging = productWithRefinementViewModel.searchCount.PageCount;
                objProductList.productList = productWithRefinementViewModel.productList;

                ViewBag.ShopID = lShopID;

                TempData["CurrentPageIndex"] = lPageIndex;

                //Yashaswi 21-02-2019 For BUG 715 To display Total product count and current product display count
                int StartPage = (lPageSize * (lPageIndex - 1)) + 1;
                int EndPage = StartPage + (lPageSize - 1);
                EndPage = EndPage >= productWithRefinementViewModel.searchCount.ProductCount ? productWithRefinementViewModel.searchCount.ProductCount : EndPage;
                ViewBag.CurrentPageProductSize = StartPage + " - " + EndPage;
                ViewBag.TotalProductSize = productWithRefinementViewModel.searchCount.ProductCount;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductController][POST:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductController][POST:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            // 3
            return PartialView("_Product", objProductList);
            // return Json(productWithRefinementViewModel.productList, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetProductOnScroll(WebmethodParams myParam)
        //{
        //    /*
        //       Indents:
        //     * Description: This method is used to load products on scroll. This method is called from jquery which is written in Product.js file

        //     * Parameters: WebmethodParams myParam: This class contains the value for all selected filter like brand Id's, category Id's, cityId, 
        //     *                                      size Id's etc.

        //     * Precondition: 
        //     * Postcondition:
        //     * Logic: 1) Get all parameter value from object passed using jquery.
        //     *        2) Call GetSearchData() to get data from database 
        //     *        3) Return json response to caller jquery function and display using jqeury written in Product.js
        //     */

        //    ProductWithRefinementViewModel productWithRefinementViewModel = new ProductWithRefinementViewModel();

        //    try
        //    {
        //        //1
        //        int lCatId = 0, lPageIndex = 0, lPageSize = 0, lShopID = 0, lCityId = 0;
        //        decimal lMinPrice = 0, lMaxPrice = 0;

        //        int.TryParse(Convert.ToString(myParam.cityId), out lCityId);
        //        int.TryParse(Convert.ToString(myParam.categoryId), out lCatId);
        //        int.TryParse(Convert.ToString(myParam.pageSize), out lPageSize);
        //        int.TryParse(Convert.ToString(myParam.pageIndex), out lPageIndex);
        //        decimal.TryParse(Convert.ToString(myParam.minPrice), out lMinPrice);
        //        decimal.TryParse(Convert.ToString(myParam.maxPrice), out lMaxPrice);
        //        int.TryParse(Convert.ToString(myParam.shopId), out lShopID);

        //        // 2
        //        productWithRefinementViewModel = this.GetSearchData(lCityId, lShopID, myParam.keyword, lCatId, Convert.ToString(myParam.brands), Convert.ToString(myParam.colors), Convert.ToString(myParam.sizes), Convert.ToString(myParam.dimensions), Convert.ToString(myParam.catDescID), Convert.ToString(myParam.catDescValue), lMinPrice, lMaxPrice, lPageIndex, lPageSize, 0, 0, true);
        //    }
        //    catch (BusinessLogicLayer.MyException myEx)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
        //            + "[ProductController][POST:Products]" + myEx.EXCEPTION_PATH,
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //    }
        //    catch (Exception ex)
        //    {
        //        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
        //            + Environment.NewLine + ex.Message + Environment.NewLine
        //            + "[ProductController][POST:Products]",
        //            BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
        //    }
        //    // 3
        //    return Json(productWithRefinementViewModel.productList, JsonRequestBehavior.AllowGet);
        //}

        public ProductViewModel ClearRefinement(ProductViewModel model, FormCollection formCollection, string pClearCookieValue, int lCatId)
        {
            /*
               Indents:
             * Description: This method is used to clear refinement. This method inter
             
              * Parameters: ProductViewModel model: This model contains list of products, and all refinement list.
             *             FormCollection form: FormCollection Contains ParentCategoryId, SelectedCategoryId,FilterOrder, 
             *                                  ShopId, search keyword.
             *             pClearCookieValue: Contains cookie value to clear(eg. If cookie value contains BRAND then it clear brand etc.)
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */


            try
            {
                int lPageIndex = 0, lPageSize = 0;
                decimal lMinPrice = 0, lMaxPrice = 0;

                List<string> lListFilterOrder = new List<string>();

                //int.TryParse(Convert.ToString(formCollection["categoryId"]), out lCatId);
                int.TryParse(Convert.ToString(formCollection["pageSize"]), out lPageSize);
                int.TryParse(Convert.ToString(formCollection["pageIndex"]), out lPageIndex);
                decimal.TryParse(Convert.ToString(formCollection["minPrice"]), out lMinPrice);
                decimal.TryParse(Convert.ToString(formCollection["maxPrice"]), out lMaxPrice);

                if (formCollection["FilterOrder"] != null)
                {
                    lListFilterOrder = this.GenerateList(Convert.ToString(formCollection["FilterOrder"]));

                    if (lListFilterOrder.Contains(pClearCookieValue))
                        lListFilterOrder.Remove(pClearCookieValue);
                }

                ProductViewModel ClearProductViewModel = new ProductViewModel();

                if (pClearCookieValue == "CAT")
                {
                    ClearProductViewModel = this.ClearIndividualFilter(model, 1);
                }

                if (pClearCookieValue == "BRAND")
                    ClearProductViewModel = this.ClearIndividualFilter(model, 2);

                if (pClearCookieValue == "COLOR")
                    ClearProductViewModel = this.ClearIndividualFilter(model, 3);

                if (pClearCookieValue == "SIZE")
                    ClearProductViewModel = this.ClearIndividualFilter(model, 4);

                if (pClearCookieValue == "DIMENSION")
                    ClearProductViewModel = this.ClearIndividualFilter(model, 5);

                if (pClearCookieValue == "CDH")
                    ClearProductViewModel = this.ClearIndividualFilter(model, 6);

                if (pClearCookieValue == "PRICE")
                    ClearProductViewModel = this.ClearIndividualFilter(model, 7);

                ProductViewModel refineProductViewModel = this.FilterRefinement(ClearProductViewModel, lListFilterOrder, Convert.ToString(formCollection["keyword"]), lCatId, model.shopDetails.ShopBasicDetails.ShopID);

                return refineProductViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:ClearRefinement]", "Can't clear refinement !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:ClearRefinement]", "Can't clear refinement !" + Environment.NewLine + ex.Message);
            }
        }

        private ProductWithRefinementViewModel GetSearchData(int pCityID, int pFranchiseID, int pShopID, string pKeyword, int pCategoryID, string pBrandIDs, string pColors, string pSizes, string pDimensions, string pSpecificationIDs, string pSpecificationValues, decimal pMinPrice, decimal pMaxPrice, int pPageIndex, int pPageSize, int pPageCount, int pProductCount, bool pIsScroll, int SortVal)// Added pFranchiseID
        {

            /*
               Indents:
             * Description: This is used to customer login , this will be call from payment process or normal login
             
             * Parameters: These parameters are used for getting items from database:
             *              int pCityID, int pShopID, string pKeyword, int pCategoryID, string pBrandIDs, string pColors, 
             *              string pSizes, string pDimensions, string pSpecificationIDs, string pSpecificationValues, 
             *              decimal pMinPrice, decimal pMaxPrice, int pPageIndex, int pPageSize, int pPageCount, 
             *              int pProductCount, bool pIsScroll
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Set value to productSearchViewModel and pass to GetProductList() of BusinessLogicLayer
             */


            try
            {


                ProductSearchViewModel productSearchViewModel = new ProductSearchViewModel();

                productSearchViewModel.CityID = pCityID;
                productSearchViewModel.FranchiseID = pFranchiseID;////added
                productSearchViewModel.ShopID = pShopID;
                productSearchViewModel.Keyword = pKeyword;
                TempData["keyword"] = pKeyword;
                productSearchViewModel.CategoryID = pCategoryID;
                productSearchViewModel.BrandIDs = pBrandIDs;
                productSearchViewModel.ColorIDs = pColors;
                productSearchViewModel.SizeIDs = pSizes;
                productSearchViewModel.DimensionIDs = pDimensions;
                productSearchViewModel.MinPrice = pMinPrice;
                productSearchViewModel.MaxPrice = pMaxPrice;
                productSearchViewModel.SpecificationIDs = pSpecificationIDs;
                productSearchViewModel.SpecificationValues = pSpecificationValues;
                productSearchViewModel.PageIndex = pPageIndex;
                productSearchViewModel.PageSize = pPageSize;
                productSearchViewModel.IsScroll = pIsScroll;
                productSearchViewModel.SortVal = SortVal;

                //============================ get shopping cart products shopStockID and into datatable =========================
                DataTable dt = new DataTable();
                dt.Columns.Add("ShopStockID");
                if (Request.Cookies["ShoppingCartCookie"] != null && Request.Cookies["ShoppingCartCookie"].Value != string.Empty)
                {
                    string[] individualItemCookie = Request.Cookies["ShoppingCartCookie"].Value.Split(',');
                    foreach (string item in individualItemCookie)
                    {
                        if (item != string.Empty)
                        {
                            string[] individualItemDetailsCookie = item.Split('$');
                            dt.Rows.Add(individualItemDetailsCookie[0]);
                        }
                    }
                }
                productSearchViewModel.ShopStockIDList = dt;

                //============================ get shopping cart products shopStockID and into datatable =========================


                ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);

                // 1
                ProductWithRefinementViewModel productWithRefinementViewModel = productList.GetProductList(productSearchViewModel, false);

                return productWithRefinementViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetSearchData]", "Can't get search data !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetSearchData]", "Can't get search data !" + Environment.NewLine + ex.Message);
            }
        }

        private ProductViewModel GetRefinement(ProductWithRefinementViewModel pProductWithRefinementViewModel, long? shopID, List<string> listCheckedBrand, List<string> listCheckedColor, List<string> listCheckedSize, List<string> listCheckedDimension, int cityId, int franchiseId)////added franchiseId
        {
            /*
               Indents:
             * Description: This method is used to get distinct refinement according to the product list
             
             * Parameters: listCheckedBrand: Contains list of checked brand
             *             listCheckedColor: Contains list of checked color
             *             listCheckedSize: Contains list of checked size
             *             listCheckedDimension: Contains list of checked dimension
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Create object of ProductViewModel
             *        2) Set  productViewModel.productList = pProductWithRefinementViewModel.productList (because productViewModel contains 
             *            property for ProductList and we have product list in pProductWithRefinementViewModel so we set this property)
             *        3) Get list of refienement from pProductWithRefinementViewModel
             *        4) Get distinct category and store in category list
             *        5) Get distinct brand and store in brand list
             *        6) Get distinct color and store in color list
             *        7) Get distinct size and store in size list
             *        8) Get distinct dimension and store in dimension list
             *        9) Get distinct detail head and store in detail head list
             *        10) Get generate price list and store in price list
             *        11) if lShopID > 0 that means we are displaying  shop product so we have to  bind shop details in model also.
             */


            try
            {
                ProductViewModel productViewModel = new ProductViewModel(); // 1

                productViewModel.productList = pProductWithRefinementViewModel.productList; // 2

                List<ProductRefinementsViewModel> lListProductRefinementsViewModel = pProductWithRefinementViewModel.productRefinements; // 3

                long lShopID = 0;
                long.TryParse(Convert.ToString(shopID), out lShopID);

                // 4
                List<RefineCategoryViewModel> lCategoryList = new List<RefineCategoryViewModel>();

                //Tejaswee 08-01-2016
                //Get all shop products and then find category , this will return all categories in which shop deal
                //===========================================================================================================================
                //var lCategories = lListProductRefinementsViewModel.Select(x => new { x.CategoryID, x.CategoryName, x.CategoryOrderSequence })
                //                                             .Distinct()
                //                                             .OrderBy(x => x.CategoryOrderSequence)
                //                                             .ToList();
                //
                //if (lShopID > 0)
                //{


                //    //ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                //    //lCategoryList = productList.GetShopCategoryList(Convert.ToInt32(lShopID));

                //    ProductWithRefinementViewModel productWithRefinementViewModel = this.GetSearchData(cityId, Convert.ToInt32(lShopID), string.Empty, 0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 10000000, 1, 12, 0, 0, true);
                //    lCategories = productWithRefinementViewModel.productRefinements.Select(x => new { x.CategoryID, x.CategoryName, x.CategoryOrderSequence })
                //                                                                    .Distinct()
                //                                                                    .OrderBy(x => x.CategoryOrderSequence)
                //                                                                    .ToList();
                //lCategories = (from n in productWithRefinementViewModel.productRefinements
                //               join SMP in db.ShopMenuPriorities on n.CategoryID equals SMP.CategoryID
                //               where SMP.ShopID == n.ShopID
                //               select new CategoryRefinementsViewModel
                //               {
                //                   CategoryID = n.CategoryID,
                //                   CategoryName = n.CategoryName,
                //                   OrderSequence = SMP.SequenceOrder
                //               }).Distinct().OrderBy(x => x.OrderSequence).ToList();
                //}


                //if (lCategories.Count() > 0)
                //{
                //    for (int i = 0; i < lCategories.Count(); i++)
                //    {
                //        RefineCategoryViewModel lCategory = new RefineCategoryViewModel();

                //        lCategory.ID = Convert.ToInt32(lCategories[i].CategoryID);
                //        lCategory.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lCategories[i].CategoryName));

                //        lCategoryList.Add(lCategory);
                //    }
                //}
                //===========================================================================================================================
                //===========================================================================================================================
                if (lShopID > 0)
                {
                    ProductList productList = new ProductList(System.Web.HttpContext.Current.Server);
                    lCategoryList = productList.GetShopCategoryList(Convert.ToInt32(lShopID));
                }
                else
                {
                    var lCategories = lListProductRefinementsViewModel.Select(x => new { x.CategoryID, x.CategoryName, x.CategoryOrderSequence })
                                                             .Distinct()
                                                             .OrderBy(x => x.CategoryOrderSequence)
                                                             .ToList();

                    //- Start Changes made by Avi Verma. Date : 24-June-2016
                    //- Reason : Select Only Active Categories in selected city.
                    List<Category> lAllCategories = db.Categories.ToList();
                    var lCategoriesWithParent = (from acl1 in lAllCategories
                                                 join acl2 in lAllCategories on acl1.ID equals acl2.ParentCategoryID
                                                 join acl3 in lAllCategories on acl2.ID equals acl3.ParentCategoryID
                                                 join cat in lCategories on acl3.ID equals cat.CategoryID
                                                 select new { cat.CategoryID, cat.CategoryName, cat.CategoryOrderSequence, acl2.ID, acl2.ParentCategoryID }).ToList();

                    // int lFranchiseID = db.Franchises.Where(x => x.ID != 1).FirstOrDefault(x => x.BusinessDetail.Pincode.CityID == cityId && x.BusinessDetail.BusinessTypeID == 2).ID;//hide

                    // List<FranchiseMenu> lFranchiseMenus = db.FranchiseMenus.Where(x => x.FranchiseID == lFranchiseID && x.IsActive == true).ToList();
                    List<FranchiseMenu> lFranchiseMenus = db.FranchiseMenus.Where(x => x.FranchiseID == franchiseId && x.IsActive == true).ToList();

                    lCategories = (from cat in lCategoriesWithParent
                                   join fml3 in lFranchiseMenus on cat.CategoryID equals fml3.CategoryID
                                   join fml2 in lFranchiseMenus on cat.ID equals fml2.CategoryID
                                   join fml1 in lFranchiseMenus on cat.ParentCategoryID equals fml1.CategoryID
                                   select new { cat.CategoryID, cat.CategoryName, cat.CategoryOrderSequence }).ToList();
                    //- End Changes made by Avi Verma. Date : 24-June-2016



                    if (lCategories.Count() > 0)
                    {
                        for (int i = 0; i < lCategories.Count(); i++)
                        {
                            RefineCategoryViewModel lCategory = new RefineCategoryViewModel();

                            lCategory.ID = Convert.ToInt32(lCategories[i].CategoryID);
                            lCategory.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lCategories[i].CategoryName));

                            lCategoryList.Add(lCategory);
                        }
                    }
                }

                //===========================================================================================================================


                // 5
                var lBrands = lListProductRefinementsViewModel.Select(x => new { x.BrandID, x.BrandName })
                                                        .Distinct()
                                                       .ToList();


                List<RefineBrandViewModel> lBrandList = new List<RefineBrandViewModel>();

                if (lBrands.Count() > 0)
                {
                    for (int i = 0; i < lBrands.Count(); i++)
                    {
                        RefineBrandViewModel lBrand = new RefineBrandViewModel();

                        lBrand.ID = Convert.ToInt32(lBrands[i].BrandID);
                        lBrand.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lBrands[i].BrandName));
                        if (listCheckedBrand != null)
                            lBrand.IsSelected = listCheckedBrand.Contains((Convert.ToString(lBrands[i].BrandID)));

                        lBrandList.Add(lBrand);
                    }
                }

                // 6
                var lColors = lListProductRefinementsViewModel.Select(x => new { x.ColorID, x.Color })
                                                        .Distinct()
                                                        .ToList();

                List<RefineColorViewModel> lColorList = new List<RefineColorViewModel>();

                if (lColors.Count() > 0)
                {
                    for (int i = 0; i < lColors.Count(); i++)
                    {
                        RefineColorViewModel lColor = new RefineColorViewModel();

                        lColor.ID = Convert.ToInt32(lColors[i].ColorID);
                        lColor.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lColors[i].Color));
                        if (listCheckedColor != null)
                            lColor.IsSelected = listCheckedColor.Contains((Convert.ToString(lColors[i].ColorID)));

                        lColorList.Add(lColor);
                    }
                }


                // 7
                var lSizes = lListProductRefinementsViewModel.Select(x => new { x.SizeID, x.Size })
                                                        .Distinct()
                                                        .ToList();

                List<RefineSizeViewModel> lSizeList = new List<RefineSizeViewModel>();

                if (lSizes.Count() > 0)
                {
                    for (int i = 0; i < lSizes.Count(); i++)
                    {
                        RefineSizeViewModel lSize = new RefineSizeViewModel();

                        lSize.ID = Convert.ToInt32(lSizes[i].SizeID);
                        lSize.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lSizes[i].Size));
                        if (listCheckedSize != null)
                            lSize.IsSelected = listCheckedSize.Contains((Convert.ToString(lSizes[i].SizeID)));

                        lSizeList.Add(lSize);
                    }
                }

                // 8
                var lDimensions = lListProductRefinementsViewModel.Select(x => new { x.DimensionID, x.Dimension })
                                                        .Distinct()
                                                        .ToList();

                List<RefineDimensionViewModel> lDimensionList = new List<RefineDimensionViewModel>();

                if (lDimensions.Count() > 0)
                {
                    for (int i = 0; i < lDimensions.Count(); i++)
                    {
                        RefineDimensionViewModel lDimension = new RefineDimensionViewModel();

                        lDimension.ID = Convert.ToInt32(lDimensions[i].DimensionID);
                        lDimension.Name = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lDimensions[i].Dimension));
                        if (listCheckedDimension != null)
                            lDimension.IsSelected = listCheckedDimension.Contains((Convert.ToString(lDimensions[i].DimensionID)));

                        lDimensionList.Add(lDimension);
                    }
                }


                // 9
                var lDetailsHeads = lListProductRefinementsViewModel.Select(x => new { x.ProductSpecificationID, x.SpecificationName, x.SpecificationID, x.SpecificationValue, })
                                                                 .Distinct()
                                                                 .ToList();

                List<RefineDetailHeadViewModel> lDetailHeadList = new List<RefineDetailHeadViewModel>();

                if (lDetailsHeads.Count() > 0)
                {
                    for (int i = 0; i < lDetailsHeads.Count(); i++)
                    {
                        /*========= Tejaswee 23/8/2015 ===================*/
                        if (lDetailsHeads[i].SpecificationID != 0 && lDetailsHeads[i].SpecificationValue != string.Empty)
                        {


                            RefineDetailHeadViewModel lDetailHead = new RefineDetailHeadViewModel();

                            lDetailHead.ProductSpecificationID = Convert.ToInt32(lDetailsHeads[i].ProductSpecificationID);
                            lDetailHead.SpecificationName = CommonFunctions.ConvertStringToCamelCase(Convert.ToString(lDetailsHeads[i].SpecificationName));
                            lDetailHead.SpecificationID = Convert.ToInt32(lDetailsHeads[i].SpecificationID);
                            lDetailHead.SpecificationValue = Convert.ToString(lDetailsHeads[i].SpecificationValue);

                            lDetailHeadList.Add(lDetailHead);
                        }
                    }
                }

                // 10
                var lPrices = lListProductRefinementsViewModel.Select(x => new { x.SaleRate })
                                                                    .Distinct()
                                                                    .ToList();

                List<decimal> lPriceList = new List<decimal>();

                if (lPrices.Count() > 0)
                {
                    for (int i = 0; i < lPrices.Count(); i++)
                    {
                        decimal lPrice = 0;
                        lPrice = Convert.ToDecimal(lPrices[i].SaleRate);
                        lPriceList.Add(lPrice);
                    }
                }

                //lPriceList.Add(520);
                //lPriceList.Add(1500);
                //lPriceList.Add(2500);
                //lPriceList.Add(3500);
                //lPriceList.Add(10200);

                List<RefinePriceViewModel> lNewPriceList = this.BindPriceRefinement(lPriceList, lListProductRefinementsViewModel);

                productViewModel.categoryList = lCategoryList;
                productViewModel.brandList = lBrandList;
                productViewModel.colorList = lColorList;
                productViewModel.sizeList = lSizeList;
                productViewModel.dimensionList = lDimensionList;
                productViewModel.detailHeadList = lDetailHeadList;
                productViewModel.priceList = lNewPriceList;

                // 11
                ShopDetails shopDetails = new ShopDetails(System.Web.HttpContext.Current.Server);

                //long lShopID = 0;
                // long.TryParse(Convert.ToString(Request.QueryString["shopID"]), out lShopID);
                long.TryParse(Convert.ToString(shopID), out lShopID);

                if (lShopID > 0)
                {
                    ViewShopDetailsViewModel viewShopDetailsViewModel = shopDetails.GetShopDetails(lShopID, franchiseId);////added cityId->franchiseId
                    productViewModel.shopDetails = viewShopDetailsViewModel;
                    if (productViewModel.shopDetails.ShopBasicDetails != null)
                    {
                        TempData["shopDescription"] = productViewModel.shopDetails.ShopBasicDetails.ShopDescription;
                        //GetShopDescriptionFilePath
                        productViewModel.shopDetails.ShopDescriptionFilePath = string.Empty;
                    }

                }

                return productViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetRefinement]", "Can't get refinement !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetRefinement]", "Can't get refinement !" + Environment.NewLine + ex.Message);
            }
        }

        private ProductViewModel FilterRefinement(ProductViewModel model, List<string> pListFilterOrder, string pSearchKeyword, int pSelectedCategoryID, long? shopID)
        {
            /*
               Indents:
             * Description: This method is used to filter refinement according to selected criteria. This method is callled PostRequest() when
             *              Form is posted to server (On ckeck event of checkbox present in left side of page)
             
             * Parameters: ProductViewModel model: Contains product list and all refinement.
             *             List<string> pListFilterOrder : Contains list of filter order.
             *             pSearchKeyword: Searck keyword
             *             pSelectedCategoryID: Contains selectedCategoryId
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Get list of checked brand
             *        2) Get list of checked color
             *        3) Get list of checked size
             *        4) Get list of checked dimension
             *        5) Get list of checked detail head 
             *        6) Get list of checked price
             *        7) Generate comma separated list of all checked refinement.
             *        8) Get minimum and maximum value of selected price refinement.
             *        9) Get CityId from cookie
             *        10)  Call GetSearchData() to get data from database according to selected criteria.
             *        11) Get refinement list from productWithRefinementViewModel
             *        12) Filter refinement according to filter order
             *        13) After filter refinement according to filter order, get list of refinements and generate new refinement by calling
             *              GetRefinement() and return model to view.
             */



            try
            {
                List<string> lListCheckedBrand = this.GetCheckSelectedRefinement(model, 2);  // 1
                List<string> lListCheckedColor = this.GetCheckSelectedRefinement(model, 3);  // 2
                List<string> lListCheckedSize = this.GetCheckSelectedRefinement(model, 4);  // 3
                List<string> lListCheckedDimension = this.GetCheckSelectedRefinement(model, 5);  //4

                Dictionary<string, string> lDictionarySpecification = this.GetCheckSelectedDetailHead(model); // 5
                Dictionary<string, string> lDictionaryPrice = this.GetCheckSelectedPrice(model); // 6

                //7
                string lSelectedBrandIDs = this.GenerateString(lListCheckedBrand);
                string lSelectedColors = this.GenerateString(lListCheckedColor);
                string lSelectedSizes = this.GenerateString(lListCheckedSize);
                string lSelectedDimensions = this.GenerateString(lListCheckedDimension);
                string lSelectedSpecificationIds = Convert.ToString(lDictionarySpecification["IDs"]);
                string lSelectedSpecificationValues = Convert.ToString(lDictionarySpecification["Values"]);

                // 8
                string lSelectedMinPrices = string.Empty, lSelectedMaxPrices = string.Empty;
                decimal lMinPrice = 0, lMaxPrice = 0;

                if (lDictionaryPrice.Count() > 0)
                {
                    lSelectedMinPrices = Convert.ToString(lDictionaryPrice["Mins"]);
                    lSelectedMaxPrices = Convert.ToString(lDictionaryPrice["Maxs"]);

                    lMinPrice = Convert.ToDecimal(lDictionaryPrice["Min"]);
                    lMaxPrice = Convert.ToDecimal(lDictionaryPrice["Max"]);
                }

                ViewBag.Brands = lSelectedBrandIDs;
                ViewBag.Colors = lSelectedColors;
                ViewBag.Sizes = lSelectedSizes;
                ViewBag.Dimensions = lSelectedDimensions;
                ViewBag.SpecificationIDs = lSelectedSpecificationIds;
                ViewBag.SpecificationValues = lSelectedSpecificationValues;
                ViewBag.MinPrices = lMinPrice;
                ViewBag.MaxPrices = lMaxPrice;

                long lShopID = 0;
                long.TryParse(Convert.ToString(shopID), out lShopID);

                ProductViewModel lProductViewModel = new ProductViewModel();


                // 9
                int cityId = 0;
                int franchiseId = 0;////added
                if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
                {
                    string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                    string[] arr = cookieValue.Split('$');
                    cityId = Convert.ToInt32(arr[0]);
                    franchiseId = Convert.ToInt32(arr[2]);////added
                }

                // 10  4-5-2016
                //ProductWithRefinementViewModel productWithRefinementViewModel = this.GetSearchData(cityId, Convert.ToInt32(lShopID), pSearchKeyword, pSelectedCategoryID, lSelectedBrandIDs, lSelectedColors, lSelectedSizes, lSelectedDimensions, lSelectedSpecificationIds, lSelectedSpecificationValues, lMinPrice, lMaxPrice, 1, 12, 0, 0, true);
                ProductWithRefinementViewModel productWithRefinementViewModel = this.GetSearchData(cityId, franchiseId, Convert.ToInt32(lShopID), pSearchKeyword, pSelectedCategoryID, lSelectedBrandIDs, lSelectedColors, lSelectedSizes, lSelectedDimensions, lSelectedSpecificationIds, lSelectedSpecificationValues, lMinPrice, lMaxPrice, 1, 12, 0, 0, false, 0);////added franchiseId

                ViewBag.PageCount = productWithRefinementViewModel.searchCount.PageCount;
                ViewBag.ProductCount = productWithRefinementViewModel.searchCount.ProductCount;

                ViewBag.PageCntForPaging = productWithRefinementViewModel.searchCount.PageCount;
                PartialProductListParam partialProductListParam = new PartialProductListParam();
                partialProductListParam.productList = productWithRefinementViewModel.productList;
                ViewBag.ProductList = partialProductListParam;



                //11
                List<ProductRefinementsViewModel> lProductRefinementViewModel = productWithRefinementViewModel.productRefinements;

                //12
                foreach (string str in pListFilterOrder)
                {
                    if (str.Equals("CAT"))
                    {
                        var results = (from x in lProductRefinementViewModel
                                       where (x.CategoryID == pSelectedCategoryID)
                                       select x).Distinct();
                        lProductRefinementViewModel = results.ToList();
                    }

                    if (str.Equals("BRAND"))
                    {
                        var results = (from x in lProductRefinementViewModel
                                       where lListCheckedBrand.Contains(x.BrandID.ToString())
                                       select x).Distinct();
                        lProductRefinementViewModel = results.ToList();
                    }

                    if (str.Equals("COLOR"))
                    {
                        var results = (from x in lProductRefinementViewModel
                                       where lListCheckedColor.Contains(x.ColorID.ToString())
                                       select x).Distinct();
                        lProductRefinementViewModel = results.ToList();
                    }

                    if (str.Equals("SIZE"))
                    {
                        var results = (from x in lProductRefinementViewModel
                                       where lListCheckedSize.Contains(x.SizeID.ToString())
                                       select x).Distinct();
                        lProductRefinementViewModel = results.ToList();
                    }

                    if (str.Equals("DIMENSION"))
                    {
                        var results = (from x in lProductRefinementViewModel
                                       where lListCheckedDimension.Contains(x.DimensionID.ToString())
                                       select x).Distinct();
                        lProductRefinementViewModel = results.ToList();
                    }
                    if (str.Equals("PRICE"))
                    {
                        var results = (from x in lProductRefinementViewModel
                                       where (x.SaleRate >= lMinPrice && x.SaleRate <= lMaxPrice)
                                       select x).Distinct();
                        lProductRefinementViewModel = results.ToList();
                    }
                }
                // 13
                productWithRefinementViewModel.productRefinements = lProductRefinementViewModel;
                lProductViewModel = this.GetRefinement(productWithRefinementViewModel, shopID, lListCheckedBrand, lListCheckedColor, lListCheckedSize, lListCheckedDimension, cityId, franchiseId);// Added franchiseId

                return lProductViewModel;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:FilterRefinement]", "Can't filter refinement !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:FilterRefinement]", "Can't filter refinement !" + Environment.NewLine + ex.Message);
            }
        }

        private List<string> GetCheckSelectedRefinement(ProductViewModel model, int pFlag)
        {
            /*
               Indents:
             * Description: This method is used to return checked refinement value in comma separated string.
             *              1. For category
             *              2. Brand
             *              3. Color
             *              4. Size
             *              5. Dimension
             *              
             
             * Parameters: ProductViewModel model: This model contains list of all posted value like Brand, Color, Size etc.
             *             pFlag: This flag indicate for which refinement we want comma separated string.
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                string lCheckSelected = string.Empty;
                List<string> lListChecked = new List<string>();

                if (pFlag == 1)
                {
                    for (int i = 0; i < model.categoryList.Count(); i++)
                    {

                    }
                }
                else if (pFlag == 2)
                {
                    if (model.brandList != null)
                    {
                        for (int i = 0; i < model.brandList.Count(); i++)
                        {
                            if (model.brandList[i].IsSelected == true)
                            {
                                lCheckSelected += model.brandList[i].ID + ",";
                            }
                        }
                    }
                }
                else if (pFlag == 3)
                {
                    if (model.colorList != null)
                    {
                        for (int i = 0; i < model.colorList.Count(); i++)
                        {
                            if (model.colorList[i].IsSelected == true)
                            {
                                lCheckSelected += model.colorList[i].ID + ",";
                            }
                        }
                    }
                }
                else if (pFlag == 4)
                {
                    if (model.sizeList != null)
                    {
                        for (int i = 0; i < model.sizeList.Count(); i++)
                        {
                            if (model.sizeList[i].IsSelected == true)
                            {
                                lCheckSelected += model.sizeList[i].ID + ",";
                            }
                        }
                    }
                }
                else if (pFlag == 5)
                {
                    if (model.dimensionList != null)
                    {
                        for (int i = 0; i < model.dimensionList.Count(); i++)
                        {
                            if (model.dimensionList[i].IsSelected == true)
                            {
                                lCheckSelected += model.dimensionList[i].ID + ",";
                            }
                        }
                    }
                }

                if (!lCheckSelected.Equals(string.Empty))
                {
                    lCheckSelected = lCheckSelected.Remove(lCheckSelected.LastIndexOf(","), 1);
                    lListChecked = this.GenerateList(lCheckSelected);
                }

                return lListChecked;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetCheckSelectedRefinement]", "Can't get check selected list !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetCheckSelectedRefinement]", "Can't get check selected list !" + Environment.NewLine + ex.Message);
            }
        }

        private Dictionary<string, string> GetCheckSelectedDetailHead(ProductViewModel model)
        {
            /*
              Indents:
            * Description: This method is used to return checked selected detail head value in comma separated string.
                           Return Dictionary<string, string> contains Specification Id's as key and specification Value's as value.
            *              
             
            * Parameters: ProductViewModel model: This model contains list of all posted value like Brand, Color, Size etc.
            *           
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                string lSelectedSpecificationIds = string.Empty;
                string lSelectedSpecificationValues = string.Empty;

                List<string> lListSelectedSpecificationIds = new List<string>();
                List<string> lListSelectedSpecificationValues = new List<string>();

                if (model.detailHeadList != null)
                {
                    for (int i = 0; i < model.detailHeadList.Count(); i++)
                    {
                        if (model.detailHeadList[i].IsSelected == true)
                        {
                            if (!lListSelectedSpecificationIds.Contains(model.detailHeadList[i].SpecificationID.ToString()))
                                lListSelectedSpecificationIds.Add(model.detailHeadList[i].SpecificationID.ToString());

                            if (!lListSelectedSpecificationValues.Contains(model.detailHeadList[i].SpecificationValue.ToString()))
                                lListSelectedSpecificationValues.Add(model.detailHeadList[i].SpecificationValue.ToString());
                        }
                    }
                }

                if (lListSelectedSpecificationIds.Count() > 0 && lListSelectedSpecificationValues.Count() > 0)
                {
                    lSelectedSpecificationIds = this.GenerateString(lListSelectedSpecificationIds);
                    lSelectedSpecificationValues = this.GenerateString(lListSelectedSpecificationValues);
                }

                Dictionary<string, string> lDictionarySpecification = new Dictionary<string, string>();
                lDictionarySpecification.Add("IDs", lSelectedSpecificationIds);
                lDictionarySpecification.Add("Values", lSelectedSpecificationValues);

                return lDictionarySpecification;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetCheckSelectedDetailHead]", "Can't get check selected detail head !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetCheckSelectedDetailHead]", "Can't get check selected detail head !" + Environment.NewLine + ex.Message);
            }

        }

        private Dictionary<string, string> GetCheckSelectedPrice(ProductViewModel model)
        {
            /*
              Indents:
            * Description: This method is used to check which price interval is selected and according to selected interval find 
             *             minimum and maximum price value.
                           Return Dictionary<string, string> contains Min value as key and Max value as value.
            *              
             
            * Parameters: ProductViewModel model: This model contains list of all posted value like Brand, Color, Size etc.
            *           
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */
            try
            {
                string lSelectedMinPrices = string.Empty;
                string lSelectedMaxPrices = string.Empty;

                List<decimal> lListSelectedMinPrices = new List<decimal>();
                List<decimal> lListSelectedMaxPrices = new List<decimal>();

                if (model.priceList != null)
                {
                    for (int i = 0; i < model.priceList.Count(); i++)
                    {
                        if (model.priceList[i].IsSelected == true)
                        {
                            lListSelectedMinPrices.Add(model.priceList[i].Min);
                            lListSelectedMaxPrices.Add(model.priceList[i].Max);
                        }
                    }
                }
                Dictionary<string, string> lDictionaryPrice = new Dictionary<string, string>();

                if (lListSelectedMinPrices.Count() > 0 && lListSelectedMaxPrices.Count() > 0)
                {
                    lSelectedMinPrices = this.GenerateString(lListSelectedMinPrices);
                    lSelectedMaxPrices = this.GenerateString(lListSelectedMaxPrices);

                    lDictionaryPrice.Add("Min", lListSelectedMinPrices.Min().ToString());
                    lDictionaryPrice.Add("Max", lListSelectedMaxPrices.Max().ToString());
                    lDictionaryPrice.Add("Mins", lSelectedMinPrices);
                    lDictionaryPrice.Add("Maxs", lSelectedMaxPrices);
                }

                return lDictionaryPrice;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetCheckSelectedPrice]", "Can't get check selected price !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetCheckSelectedPrice]", "Can't get check selected price !" + Environment.NewLine + ex.Message);
            }
        }

        private List<string> GenerateList(string val)
        {
            /*
               Indents:
             * Description: This method is used to generate list from comma separated value
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */


            try
            {
                List<string> listValues = new List<string>();

                if (val != string.Empty)
                {
                    listValues = val.Split(',').ToList();
                }

                return listValues;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GenerateList]", "Can't generate list !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GenerateList]", "Can't generate list !" + Environment.NewLine + ex.Message);
            }
        }

        private List<string> GetFilterOrder(ProductViewModel model, List<string> pListFilterOrder, string pCookieValue)
        {


            try
            {
                int pCount = 0;

                if (pCookieValue == "BRAND")
                {
                    for (int i = 0; i < model.brandList.Count(); i++)
                    {
                        if (model.brandList[i].IsSelected == true)
                            pCount++;
                    }
                }

                if (pCookieValue == "COLOR")
                {
                    for (int i = 0; i < model.colorList.Count(); i++)
                    {
                        if (model.colorList[i].IsSelected == true)
                            pCount++;
                    }

                }

                if (pCookieValue == "SIZE")
                {
                    for (int i = 0; i < model.sizeList.Count(); i++)
                    {
                        if (model.sizeList[i].IsSelected == true)
                            pCount++;
                    }
                }

                if (pCookieValue == "DIMENSION")
                {
                    for (int i = 0; i < model.dimensionList.Count(); i++)
                    {
                        if (model.dimensionList[i].IsSelected == true)
                            pCount++;
                    }
                }

                if (pCookieValue == "CDH")
                {
                    for (int i = 0; i < model.detailHeadList.Count(); i++)
                    {
                        if (model.detailHeadList[i].IsSelected == true)
                            pCount++;
                    }
                }

                if (pCookieValue == "PRICE")
                {
                    for (int i = 0; i < model.priceList.Count(); i++)
                    {
                        if (model.priceList[i].IsSelected == true)
                            pCount++;
                    }
                }

                if (pCount == 0)
                    pListFilterOrder.Remove(pCookieValue);

                return pListFilterOrder;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetFilterOrder]", "Can't get filter order !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetFilterOrder]", "Can't get filter order !" + Environment.NewLine + ex.Message);
            }
        }

        private string GenerateString(List<string> pListCheckSelected)
        {
            /*
              Indents:
            * Description: This method is used to get comma separated string from List<string>
             *             
             
            * Parameters: List<string> pListCheckSelected: list of selected values
            *           
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */

            try
            {
                string lSelectedString = string.Empty;

                for (int i = 0; i < pListCheckSelected.Count(); i++)
                {
                    lSelectedString += pListCheckSelected[i].ToString() + ",";
                }
                if (!lSelectedString.Equals(string.Empty))
                    lSelectedString = lSelectedString.Remove(lSelectedString.LastIndexOf(","), 1);

                return lSelectedString;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GenerateString]", "Can't generate string !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GenerateString]", "Can't generate string !" + Environment.NewLine + ex.Message);
            }
        }

        private string GenerateString(List<decimal> pListCheckSelected)
        {
            try
            {
                string lSelectedString = string.Empty;

                for (int i = 0; i < pListCheckSelected.Count(); i++)
                {
                    lSelectedString += pListCheckSelected[i].ToString() + ",";
                }
                if (!lSelectedString.Equals(string.Empty))
                    lSelectedString = lSelectedString.Remove(lSelectedString.LastIndexOf(","), 1);

                return lSelectedString;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GenerateString]", "Can't generate string !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GenerateString]", "Can't generate string !" + Environment.NewLine + ex.Message);
            }
        }

        private List<RefinePriceViewModel> BindPriceRefinement(List<decimal> pPriceList, List<ProductRefinementsViewModel> pProductRefinementsViewModel)
        {
            try
            {
                List<RefinePriceViewModel> lPriceList = new List<RefinePriceViewModel>();
                List<RefinePriceViewModel> lNewPriceList = new List<RefinePriceViewModel>();

                int minimumSaleRate = 0, maximumSaleRate = 0;
                if (pPriceList.Count() > 0)
                {
                    minimumSaleRate = Convert.ToInt32(Math.Round(Convert.ToDouble(pPriceList.Min())));
                    maximumSaleRate = Convert.ToInt32(Math.Round(Convert.ToDouble(pPriceList.Max())));

                    if (maximumSaleRate - minimumSaleRate > 50)
                    {
                        int newMinValue, newMaxValue, updatedMaxValue = 0;


                        this.GetRoundValue(minimumSaleRate, maximumSaleRate, out newMinValue, out newMaxValue);
                        DataTable dtPriceSlab = new DataTable();
                        dtPriceSlab = this.GeneratePriceSlab(newMinValue, newMaxValue, out updatedMaxValue);

                        DataRow drFirst = dtPriceSlab.NewRow();
                        drFirst["MIN_VALUE"] = minimumSaleRate;
                        drFirst["MAX_VALUE"] = newMinValue;

                        // drFirst["MIN_VALUE"] = Convert.ToInt32(Math.Round(minimumSaleRate / 10d, 0) * 10);
                        //drFirst["MAX_VALUE"] = Convert.ToInt32(Math.Round(newMinValue / 10d, 0) * 10);

                        drFirst["SLAB"] = "Rs. " + newMinValue + " & " + "below";
                        //drFirst["SLAB"] = "Rs. " + Convert.ToInt32(Math.Round(newMinValue / 10d, 0) * 10) + " & " + "below";
                        dtPriceSlab.Rows.InsertAt(drFirst, 0);



                        if (maximumSaleRate > newMaxValue)
                        {
                            DataRow drLast = dtPriceSlab.NewRow();
                            //drLast["MIN_VALUE"] = newMaxValue + 1;
                            //drLast["MAX_VALUE"] = maximumSaleRate;
                            drLast["MIN_VALUE"] = Convert.ToInt32(Math.Round(updatedMaxValue / 100d, 0) * 1000) + 1;
                            drLast["MAX_VALUE"] = Convert.ToInt32(Math.Round(maximumSaleRate / 100d, 0) * 1000);
                            //changes by ansar/AJ
                            //drLast["SLAB"] = "Rs. " + newMaxValue + " & " + "above"; 
                            drLast["SLAB"] = "Rs. " + (Convert.ToInt32(Math.Round(updatedMaxValue / 100d, 0) * 100) + 1) + " & " + "above";
                            dtPriceSlab.Rows.Add(drLast);
                        }

                        //DataSet dsRefinement = new DataSet();

                        //if (HttpContext.Current.Cache["Refinement"] != null)
                        //{
                        //    dsRefinement = (DataSet)HttpContext.Current.Cache["Refinement"];

                        //DataTable dtAllPrice = dsRefinement.Tables[0];

                        DataTable dtNewPriceSlab = new DataTable();

                        dtNewPriceSlab.Columns.Add("MIN_VALUE");
                        dtNewPriceSlab.Columns.Add("MAX_VALUE");
                        dtNewPriceSlab.Columns.Add("SLAB");

                        foreach (DataRow dr in dtPriceSlab.Rows)
                        {
                            var result = pProductRefinementsViewModel.Select(x => new { x.SaleRate }).Where(x => x.SaleRate >= Convert.ToDecimal(dr["MIN_VALUE"]) && x.SaleRate <= Convert.ToDecimal(dr["MAX_VALUE"]));

                            // Logic To find existence price

                            //if (result.Length > 0)
                            //{
                            //    DataRow dRow = dtNewPriceSlab.NewRow();

                            //    dRow["MIN_VALUE"] = dr["MIN_VALUE"];
                            //    dRow["MAX_VALUE"] = dr["MAX_VALUE"];
                            //    dRow["SLAB"] = dr["SLAB"];

                            //    dtNewPriceSlab.Rows.Add(dRow);
                            //}
                        }

                        lPriceList = (from DataRow dr in dtPriceSlab.Rows
                                      select new RefinePriceViewModel()
                                      {
                                          Max = Convert.ToInt32(dr["MAX_VALUE"]),
                                          Min = Convert.ToInt32(dr["MIN_VALUE"]),
                                          Slab = dr["SLAB"].ToString()
                                      }).ToList();


                        foreach (var item in lPriceList)
                        {
                            var results = (from x in pProductRefinementsViewModel
                                           where (x.SaleRate >= item.Min && x.SaleRate <= item.Max)
                                           select x).ToList();

                            if (results.Count() > 0)
                            {
                                item.IsProductAvailable = true;
                                lNewPriceList.Add(item);

                            }
                            else
                            {
                                item.IsProductAvailable = false;
                                lNewPriceList.Add(item);
                            }
                        }
                    }
                }
                return lNewPriceList;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:BindPriceRefinement]", "Can't bind price refinement !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:BindPriceRefinement]", "Can't bind price refinement !" + Environment.NewLine + ex.Message);
            }
        }

        private DataTable GeneratePriceSlab(int minimumSaleRate, int maximumSaleRate, out int updatedMaxValue)
        {
            /*
              Indents:
            * Description: This method is used to generate price slab according to min and max values
             *             
             
            * Parameters: minimumSaleRate: Contains minimum value, maximumSaleRate: Contains maximum value
            *           
             
            * Precondition: 
            * Postcondition:
            * Logic: 
            */
            try
            {
                int interval = 0;
                DataTable dtPriceSlab = new DataTable();
                dtPriceSlab.Columns.Add("MIN_VALUE");
                dtPriceSlab.Columns.Add("MAX_VALUE");
                dtPriceSlab.Columns.Add("SLAB");

                int flag = interval, firstEnd, secondEnd = minimumSaleRate;
                interval = (maximumSaleRate - minimumSaleRate) * 20 / 100;

                //if (interval < 1000)
                //{
                //--interval = Convert.ToInt32(Math.Round(interval / 100d, 0) * 100);

                //minimumSaleRate = Convert.ToInt32(Math.Round(minimumSaleRate / 100d, 0) * 100);

                while (maximumSaleRate > secondEnd)
                {
                    firstEnd = secondEnd + 1;
                    //--secondEnd = Convert.ToInt32(Math.Round(secondEnd / 100d, 0) * 100) + interval;
                    secondEnd = secondEnd + interval;
                    //------------------------------------------------------------------
                    int temp = 5;
                    decimal Dvalue = 0;
                    while (secondEnd > temp)
                    {
                        Dvalue = temp;
                        temp = temp * 10;
                    }
                    decimal rTemp = (Convert.ToInt32(Math.Ceiling(secondEnd / Dvalue))) * Dvalue;
                    secondEnd = Convert.ToInt32(rTemp);

                    //------------------------------------------------------------------
                    if (secondEnd <= maximumSaleRate)
                    {
                        DataRow dr = dtPriceSlab.NewRow();
                        dr["MIN_VALUE"] = firstEnd;
                        dr["MAX_VALUE"] = secondEnd;
                        // dr["SLAB"] = "Rs. " + firstEnd + " - " + "Rs. " + Convert.ToInt32(Math.Round(secondEnd / 100d, 0) * 100);
                        dr["SLAB"] = "Rs. " + firstEnd + " - " + "Rs. " + secondEnd;
                        dtPriceSlab.Rows.Add(dr);

                    }
                    else
                    {
                        maximumSaleRate = firstEnd;

                    }
                }
                // }
                //else
                //{
                //    interval = Convert.ToInt32(Math.Round(interval / 1000d, 0) * 1000);
                //    maximumSaleRate = Convert.ToInt32(Math.Round(maximumSaleRate / 1000d, 0) * 1000);
                //    while (maximumSaleRate > secondEnd)
                //    {
                //        firstEnd = secondEnd + 1;
                //        secondEnd = Convert.ToInt32(Math.Round(secondEnd / 1000d, 0) * 1000) + interval;
                //        //------------------------------------------------------------------
                //        int temp = 5;
                //        decimal Dvalue = 0;
                //        while (secondEnd > temp)
                //        {
                //            Dvalue = temp;
                //            temp = temp * 10;
                //        }
                //        decimal rTemp = (Convert.ToInt32(Math.Ceiling(secondEnd / Dvalue))) * Dvalue;
                //        secondEnd = Convert.ToInt32(rTemp);

                //        //------------------------------------------------------------------
                //        if (secondEnd <= maximumSaleRate)
                //        {
                //            DataRow dr = dtPriceSlab.NewRow();
                //            dr["MIN_VALUE"] = firstEnd;
                //            dr["MAX_VALUE"] = secondEnd;
                //            dr["SLAB"] = "Rs. " + firstEnd + " - " + "Rs. " + Convert.ToInt32(Math.Round(secondEnd / 1000d, 0) * 1000);
                //            dtPriceSlab.Rows.Add(dr);

                //        }
                //        else
                //        {
                //            maximumSaleRate = firstEnd;

                //        }
                //    }

                //}

                //minimumSaleRate = Convert.ToInt32(Math.Round(minimumSaleRate / 100d, 0) * 100);




                updatedMaxValue = maximumSaleRate;
                return dtPriceSlab;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GeneratePriceSlab]", "Can't bind price datatable !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GeneratePriceSlab]", "Can't bind price datatable !" + Environment.NewLine + ex.Message);
            }

        }

        //private DataTable GeneratePriceSlab(int minimumSaleRate, int maximumSaleRate)
        //{
        //    /*
        //      Indents:
        //    * Description: This method is used to generate price slab according to min and max values
        //     *             

        //    * Parameters: minimumSaleRate: Contains minimum value, maximumSaleRate: Contains maximum value
        //    *           

        //    * Precondition: 
        //    * Postcondition:
        //    * Logic: 
        //    */
        //    try
        //    {
        //        int interval = 0;

        //        interval = (maximumSaleRate - minimumSaleRate) * 20 / 100;

        //        DataTable dtPriceSlab = new DataTable();
        //        dtPriceSlab.Columns.Add("MIN_VALUE");
        //        dtPriceSlab.Columns.Add("MAX_VALUE");
        //        dtPriceSlab.Columns.Add("SLAB");

        //        int flag = interval, firstEnd, secondEnd = minimumSaleRate;

        //        while (maximumSaleRate > secondEnd)
        //        {
        //            firstEnd = secondEnd + 1;
        //            secondEnd = secondEnd + interval;

        //            DataRow dr = dtPriceSlab.NewRow();
        //            dr["MIN_VALUE"] = firstEnd;
        //            dr["MAX_VALUE"] = secondEnd;
        //            dr["SLAB"] = "Rs. " + firstEnd + " - " + "Rs. " + secondEnd;
        //            dtPriceSlab.Rows.Add(dr);
        //        }

        //        return dtPriceSlab;
        //    }
        //    catch (MyException myEx)
        //    {
        //        throw new BusinessLogicLayer.MyException("[ProductController][M:GeneratePriceSlab]", "Can't bind price datatable !" + Environment.NewLine + myEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[ProductController][M:GeneratePriceSlab]", "Can't bind price datatable !" + Environment.NewLine + ex.Message);
        //    }

        //}

        private void GetRoundValue(int minimumSaleRate, int maximumSaleRate, out int newMinValue, out int newMaxValue)
        {
            /*
              Indents:
            * Description: This method is used to get round value from passing min and max value.
             *             
             
            * Parameters: minimumSaleRate: Contains minimum value, maximumSaleRate: Contains maximum value
            *           
             
            * Precondition: 
            * Postcondition:
            * Logic: 1) Get length of minimumSaleRate and maximumSaleRate sale rate
             *       2) Create two object of StringBuilder for sbMin, sbMax  and append min sale and max sale rate respectively.
             *       3) Pass length of min sale rate to switch case and get newMinValue 
             *          3.1)  if length is equal to 1,2,3 then remove last digit from sbMin and add 10 to this value.
             *          3.2) else remove last two digits and add 100 to this value
             *       4) Pass length of max sale rate to switch case and get newMaxValue 
             *          3.1)  if length is equal to 1,2,3 then remove last digit from sbMax and add 10 to this value.
             *          3.2) else remove last two digits and add 100 to this value
            */

            try
            {
                // 1
                int minLength = minimumSaleRate.ToString().Length;
                int maxLength = maximumSaleRate.ToString().Length;

                // 2
                StringBuilder sbMin = new StringBuilder();
                StringBuilder sbMax = new StringBuilder();

                sbMin.Append(minimumSaleRate.ToString());
                sbMax.Append(maximumSaleRate.ToString());

                // 4
                switch (minLength)
                {
                    case 1:
                        newMinValue = 50;
                        break;
                    case 2:
                        //  if (maximumSaleRate >= 1000)
                        //    newMinValue = 500;
                        //else
                        // newMinValue =100;
                        //break;
                        newMinValue = 100;
                        break;
                    case 3:
                        //newMinValue = Convert.ToInt32(sbMin.Remove(minLength - 1, 1).Append("0").ToString());
                        //newMinValue += 10;

                        if (minimumSaleRate <= 500)
                            newMinValue = 500;
                        else
                            newMinValue = 1000;
                        break;
                    default:
                        newMinValue = Convert.ToInt32(sbMin.Remove(minLength - 2, 2).Append("00").ToString());
                        newMinValue += 100;
                        break;
                }

                // 5
                switch (maxLength)
                {
                    case 1:
                    case 2:
                    case 3:
                        newMaxValue = Convert.ToInt32(sbMax.Remove(maxLength - 1, 1).Append("0").ToString());
                        break;
                    default:
                        newMaxValue = Convert.ToInt32(sbMax.Remove(maxLength - 2, 2).Append("00").ToString());
                        break;
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetRoundValue]", "Can't round price !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:GetRoundValue]", "Can't round price !" + Environment.NewLine + ex.Message);
            }
        }

        private ProductViewModel ClearIndividualFilter(ProductViewModel model, int pFlag)
        {
            /*
               Indents:
             * Description: This method is used to clear filter according tp flag
             
             * Parameters: 
             
             * Precondition: 
             * Postcondition:
             * Logic: 
             */

            try
            {
                //Clear category
                if (pFlag == 1)
                {

                }
                // Clear Brand
                else if (pFlag == 2)
                {
                    model.brandList.ForEach(x => x.IsSelected = false);
                }
                // Clear Color
                else if (pFlag == 3)
                {
                    model.colorList.ForEach(x => x.IsSelected = false);
                }
                // Clear Size
                else if (pFlag == 4)
                {
                    model.sizeList.ForEach(x => x.IsSelected = false);
                }
                // Clear Dimension
                else if (pFlag == 5)
                {
                    model.dimensionList.ForEach(x => x.IsSelected = false);
                }
                // Clear detail Head
                else if (pFlag == 6)
                {
                    model.detailHeadList.ForEach(x => x.IsSelected = false);
                }
                // Clear Price
                else if (pFlag == 7)
                {
                    model.priceList.ForEach(x => x.IsSelected = false);
                }

                return model;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:ClearIndividualFilter]", "Can't clear filter !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:ClearIndividualFilter]", "Can't clear filter !" + Environment.NewLine + ex.Message);
            }
        }

        private List<string> ManageFilterOrder(ProductViewModel model, string pCookieValue, string pViewBagValue)
        {
            /*
               Indents:
             * Description: This method is used to manage filter order.
             
             * Parameters: ProductViewModel model: Contains all model posted values
             *             pCookieValue: Contains value for which refinement section is clicked
             *             pViewBagValue: Contains filter order value
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Generate list of filter order
             *        2) If lListFilterOrder.Count() == 0 then add pCookieValue to filter order 
             *        3) Else If filter order contains value then 
             *          3.1) If filter order not contains pCookieValue value then add it to filter order
             *          3.2) Else remove pCookieValue from filter order
             */

            try
            {
                // 1
                List<string> lListFilterOrder = this.GenerateList(pViewBagValue);

                // if viewBag value is null then set current cookie value to filter order

                // 2
                if (lListFilterOrder.Count() == 0)
                {
                    pViewBagValue = pCookieValue;
                    lListFilterOrder.Add(pCookieValue);
                    ViewBag.FilterOrder = pViewBagValue;
                }
                // 3
                else
                {
                    // if viewBag not contains current cookie value then append to filter order
                    if (!lListFilterOrder.Contains(pCookieValue)) // 3.1
                    {
                        pViewBagValue = pViewBagValue + "," + pCookieValue;
                        lListFilterOrder.Add(pCookieValue);
                        ViewBag.FilterOrder = pViewBagValue;
                    }
                    else // 3.2
                    {
                        lListFilterOrder = this.GetFilterOrder(model, lListFilterOrder, pCookieValue);

                        pViewBagValue = string.Empty;

                        for (int i = 0; i < lListFilterOrder.Count(); i++)
                        {
                            pViewBagValue += lListFilterOrder[i].ToString() + ",";
                        }
                        if (pViewBagValue != string.Empty)
                            pViewBagValue = pViewBagValue.Remove(pViewBagValue.LastIndexOf(","), 1);

                        ViewBag.FilterOrder = pViewBagValue;
                    }
                }
                return lListFilterOrder;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:ManageFilterOrder]", "Can't manage filter order !" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:ManageFilterOrder]", "Can't manage filter order !" + Environment.NewLine + ex.Message);
            }
        }

        public ProductViewModel GetProducts(int cityId, int franchiseId, int? parentCategoryId, int? selectedCategoryId, string item, int? shopID)// added franchiseId
        {
            /*
               Indents:
             * Description: This method is called by Products action method and this method return object of ProductViewModel(WHich
             * contains ProductList and all search related refinements).
             
             * Parameters: cityId: used for list products of selected city
             *             parentCategoryId: Same as Described in Products action method
             *             selectedCategoryId: Same as Described in Products action method
             *             item: Same as Described in Products action method
             *             shopID: Same as Described in Products action method
             * Precondition: 
             * Postcondition:
             * Logic: 1) if parentCategoryId > 0 that means user has selected parentCategoryId from suggesstion box of search
             *        2) if selectedCategoryId > 0 that means user has selected sub category from category section of refinements.
             *        3) if shopID == null that means search items in all shops present in eZeelo
             *        4) if (item != null) Set Title of page
             *        5) if (selectedCategoryId != null) then search item in sub category otherwise search item in parent category
             *        6) Call GetSearchData() for getting items from database according to selected criteria which will return 
             *           productWithRefinementViewModel which contains Three list:
             *           SearchCount: PageCount, ProductCount 
             *           ProductList: Contains list of product
             *           RefinementList: Contains list of refinement
             *        7) Afer getting productWithRefinementViewModel, we will pass it GetRefinement() to get individual refinements.
             *        8) Get search count from productWithRefinementViewModel.SearchCount and store in ViewBag.       
             *        9) Set title for eShop(Done by tejaswee)
             *        
             */

            ProductViewModel productViewModel = new ProductViewModel();

            try
            {

                if (parentCategoryId > 0)  // 1
                    ViewBag.ParentCategory = parentCategoryId;
                else
                    ViewBag.ParentCategory = 0;

                if (selectedCategoryId > 0)  // 2
                {
                    ViewBag.SelectedCategory = selectedCategoryId;
                    ViewBag.FilterOrder = "CAT";
                }
                else
                {
                    ViewBag.SelectedCategory = 0;
                    ViewBag.FilterOrder = "";
                }

                if (shopID == null) // 3
                    ViewBag.ShopID = 0;
                else
                {
                    ViewBag.ShopID = shopID;

                }

                if (item != null) // 4 Set Tile of product page done by Tejaswee
                {
                    ViewBag.Keyword = item;
                    //    ViewBag.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ToLower()) + " | eZeelo";
                }

                int lCatId = 0;

                if (selectedCategoryId != null) // 5
                    lCatId = Convert.ToInt32(selectedCategoryId);
                else if (parentCategoryId != null)
                    lCatId = Convert.ToInt32(parentCategoryId);

                // 6
                // ProductWithRefinementViewModel productWithRefinementViewModel = this.GetSearchData(cityId, Convert.ToInt32(shopID), item, lCatId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 1, 12, 0, 0, false,0);//hide
                ProductWithRefinementViewModel productWithRefinementViewModel = this.GetSearchData(cityId, franchiseId, Convert.ToInt32(shopID), item, lCatId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 1, 12, 0, 0, false, 0);//--added by Ashish for multiple franchise in same city--//
                //7
                // Save_SiteKeywordSearch saveKeyword = new Save_SiteKeywordSearch();
                TrackSearch_Keywords saveKeyword = new TrackSearch_Keywords();

                if (item != "")
                {

                    long UserID = Convert.ToInt64(Session["UID"]);

                    saveKeyword.UserloginID = UserID;

                    saveKeyword.Keyword = item;
                    saveKeyword.Network_IP = CommonFunctions.GetClientIP();
                    saveKeyword.Create_Date = System.DateTime.Now;

                    saveKeyword.City_ID = cityId;
                    saveKeyword.Franchise_ID = franchiseId;
                    saveKeyword.Category_ID = lCatId;

                    ///var isCount = (int)productWithRefinementViewModel.searchCount;
                    if (productWithRefinementViewModel.searchCount.ProductCount > 0)
                    {
                        saveKeyword.IsResult = true;
                    }
                    else
                    {
                        saveKeyword.IsResult = false;
                    }

                    saveKeyword.DeviceType = "x";
                    saveKeyword.Device_ID = "x";
                    // db.Save_SiteKeywordSearchs.Add(saveKeyword);
                    db.TrackSearch_Keywords.Add(saveKeyword);

                    db.SaveChanges();
                }

                //===========================added by amit for save search keyword=============================}//


                productViewModel = this.GetRefinement(productWithRefinementViewModel, shopID, null, null, null, null, cityId, franchiseId);// Added franchiseId

                // 8
                SearchCountViewModel searchCountViewModel = productWithRefinementViewModel.searchCount;

                ViewBag.PageCount = searchCountViewModel.PageCount;
                ViewBag.ProductCount = searchCountViewModel.ProductCount;

                ViewBag.PageCntForPaging = productWithRefinementViewModel.searchCount.PageCount;

                //9
                //Commented by Tejaswee because shops best deal not display on shop page
                //if (shopID != 0)
                //{
                //    if (productViewModel.shopDetails.ShopBasicDetails != null)
                //    {
                //        // Tejaswee
                //      //  ViewBag.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(productViewModel.shopDetails.ShopBasicDetails.Name.ToLower()) + " | eZeelo";

                //        //Load shop's best deal products

                //        BestDealProducts bestDealProducts = new BestDealProducts();
                //        BestDealProductSearchViewModel bestDealProductSearchViewModel = new BestDealProductSearchViewModel();
                //        bestDealProductSearchViewModel.CityID = cityId;
                //        bestDealProductSearchViewModel.ShopID = Convert.ToInt64(shopID);
                //        bestDealProductSearchViewModel.OfferID = 0;
                //        bestDealProductSearchViewModel.OfferType = "";
                //        bestDealProductSearchViewModel.CategoryID = 0;
                //        bestDealProductSearchViewModel.BrandID = 0;
                //        bestDealProductSearchViewModel.PageIndex = 1;
                //        bestDealProductSearchViewModel.PageSize = 20;

                //        ProductStockVarientViewModel productStockVarientViewModel = bestDealProducts.GetProductList(bestDealProductSearchViewModel);
                //        productViewModel.shopBestDeals = productStockVarientViewModel;
                //    }


                //}
                return productViewModel;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductController][GET:GetProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return productViewModel;
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductController][GET:GetProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                return productViewModel;
            }
            //catch (MyException myEx)
            //{
            //    throw new BusinessLogicLayer.MyException("[ProductController][M:GetProducts]", "Problem in getting products !!" + Environment.NewLine + myEx.Message);
            //}
            //catch (Exception ex)
            //{
            //    throw new BusinessLogicLayer.MyException("[ProductController][M:GetProducts]", "Problem in getting products !!" + Environment.NewLine + ex.Message);
            //}
        }

        public ProductViewModel PostProducts(ProductViewModel model, FormCollection form)
        {
            /*
               Indents:
             * Description: This method is used to get items from database when form is posted.
             
             * Parameters: ProductViewModel model: This model contains list of products, and all refinement list.
             *             FormCollection form: FormCollection Contains ParentCategoryId, SelectedCategoryId,FilterOrder, 
             *                                  ShopId, search keyword.
             
             * Precondition: 
             * Postcondition:
             * Logic: 1) Create object of ProductViewModel
             *        2) Get search keyword from form collection
             *        3) Get ParentCategoryId and SelectedCategoryId from form collection
             *        4) Get Corrent Selected category Id
             *        5) Get Filter order from form collection
             *        6) we are using Cookies["CheckCookie"] to check which checkbox is selected. it may be of BRAND, COLOR, SIZE etc.
             *        7) if (CheckCookie != null) then get cookie value
             *        7.1) Check cookie value if start with 'C_' that means we press clear button to clear refinement. so call ClearRefinement()
             *             to clear selectef refinement and return.
             *        7.2) Else manage filter order
             *        8) Call FilterRefinement() to filter refinement according to selected criteria
             *        9) Bind shop details to model
             *        10) Check cookie value and according to cookie value not rebind respective property of model and bind this property from 
             *              model.
             */


            try
            {
                ProductViewModel refineProductViewModel = new ProductViewModel(); // 1

                string lSearchKeyword = form["Keyword"].ToString(); // 2

                // 3
                int lParentCategory = 0, lSelectedCategory = 0;

                int.TryParse(Convert.ToString(form["ParentCategory"]), out lParentCategory);
                int.TryParse(Convert.ToString(Request.QueryString["selectedCategoryId"]), out lSelectedCategory);

                // 4
                int lCategory = 0;

                if (lSelectedCategory > 0)
                    lCategory = lSelectedCategory;
                else
                    lCategory = lParentCategory;

                //use for showing sorting dropdown... 
                //If category Id > 0 then sorting dropdown will show
                TempData["CatID"] = lCategory;

                //5
                string lViewBagValue = form["FilterOrder"].ToString();

                ViewBag.Keyword = lSearchKeyword;
                ViewBag.ParentCategory = lParentCategory;
                ViewBag.SelectedCategory = lSelectedCategory;

                List<string> lListFilterOrder = new List<string>();

                string lCookieValue = string.Empty;

                // 6 Check cookie exist or not
                HttpCookie CheckCookie = this.Request.Cookies["CheckCookie"];

                // 7 if cookie exists then get value and set filter order
                if (CheckCookie != null)
                {
                    // Get Cookie value
                    lCookieValue = CheckCookie.Value;

                    if (CheckCookie.Value.StartsWith("C_")) // 7.1
                    {
                        refineProductViewModel = this.ClearRefinement(model, form, lCookieValue.Substring(2), lCategory);
                        return refineProductViewModel;
                    }
                    else // 7.2
                    {
                        // Set cookie to null
                        if (this.ControllerContext.HttpContext.Request.Cookies["CheckedCookie"] != null)
                        {
                            CheckCookie.Expires = DateTime.Now.AddDays(-1);
                            this.ControllerContext.HttpContext.Response.Cookies.Add(CheckCookie);
                        }
                        if (CheckCookie.Expires < DateTime.Now)
                        {
                            this.ControllerContext.HttpContext.Request.Cookies.Remove("CheckedCookie");
                        }

                        // if viewBag value is null then set current cookie value to filter order

                        lListFilterOrder = this.ManageFilterOrder(model, lCookieValue, lViewBagValue);
                    }
                }

                long lShopID = 0;
                long.TryParse(form["ShopID"], out lShopID);
                //if (model.shopDetails != null)
                //{
                //    long.TryParse(Convert.ToString(model.shopDetails.ShopBasicDetails.ShopID), out lShopID);
                //}

                // 8
                refineProductViewModel = this.FilterRefinement(model, lListFilterOrder, lSearchKeyword, lCategory, lShopID);

                // 9
                ShopDetails shopDetails = new ShopDetails(System.Web.HttpContext.Current.Server);

                if (lShopID > 0)
                {
                    ViewShopDetailsViewModel viewShopDetailsViewModel = shopDetails.GetShopDetails(lShopID);
                    refineProductViewModel.shopDetails = viewShopDetailsViewModel;
                }

                if (lListFilterOrder.Count() == 0)
                    lCookieValue = string.Empty;

                ModelState.Clear();

                model.shopDetails = refineProductViewModel.shopDetails;

                model.productList = refineProductViewModel.productList;


                // 10
                if (lCookieValue != "BRAND")
                    model.brandList = refineProductViewModel.brandList;

                model.categoryList = refineProductViewModel.categoryList;

                if (lCookieValue != "COLOR")
                    model.colorList = refineProductViewModel.colorList;

                if (lCookieValue != "SIZE")
                    model.sizeList = refineProductViewModel.sizeList;

                if (lCookieValue != "DIMENSION")
                    model.dimensionList = refineProductViewModel.dimensionList;

                if (lCookieValue != "CDH")
                    model.detailHeadList = refineProductViewModel.detailHeadList;

                if (lCookieValue != "PRICE")
                    model.priceList = refineProductViewModel.priceList;

                return model;
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:PostProducts]", "Problem in getting products on posting form !!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductController][M:PostProducts]", "Problem in getting products on posting form !!" + Environment.NewLine + ex.Message);
            }
        }

        public void SetIsSelected()
        {

        }

        public ActionResult ShowMap()
        {
            return View("ShowGoogleMap");
        }
        // Tejaswee
        #region ShoppingCart
        public JsonResult AddProductInShoppingCart(long shopStockId, long itemId, int quantity)
        {

            ShoppingCartInitialization obj = new ShoppingCartInitialization();
            string status = obj.SetCookie(shopStockId, itemId, quantity);
            //Start Yashaswi 04-02-2019 if current available quantity is less than required quantity
            string resultstring = "";
            if (status == "4")
            {
                ShopStock objshopStock = db.ShopStocks.FirstOrDefault(p => p.ID == shopStockId);
                resultstring = objshopStock.Qty.ToString() + "#";
            }
            else
            {
                resultstring = status.ToString(); ;
            }
            //Start Yashaswi 04-02-2019 if current available quantity is less than required quantity

            //================ Manoj Yadav =========================
            long UserID = Convert.ToInt64(Session["UID"]);
            string MobileNo = db.UserLogins.Where(x => x.ID == UserID).Select(x => x.Mobile).FirstOrDefault();
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                Nullable<long> lCartID = null;
                if (ControllerContext.HttpContext.Request.Cookies["CartID"] != null)
                {
                    lCartID = Convert.ToInt64(ControllerContext.HttpContext.Request.Cookies["CartID"].Value.ToString());
                }
                //TrackCartBusiness.InsertCartDetails(lCartID, quantity, UserID, shopStockId, MobileNo, "SHOPING_CART", "", "", "", "", cookieValue.Split('$')[1], "");
                //- For Manoj
                TrackCartBusiness.InsertCartDetails(lCartID, quantity, UserID, shopStockId, MobileNo, "SHOPING_CART", "", "", "", "", cookieValue.Split('$')[1], "", Convert.ToInt32(cookieValue.Split('$')[2]));
            }

            return Json(resultstring, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProductInShoppingCartFromPreview(long shopStockId, long itemId, string couponCode, double couponAmount, double couponPercent)
        {
            ShoppingCartInitialization obj = new ShoppingCartInitialization();
            string status = obj.SetCookie(shopStockId, itemId, 1);
            //Start Yashaswi 04-02-2019 if current available quantity is less than required quantity
            string resultstring;
            if (status == "4")
            {
                ShopStock objshopStock = db.ShopStocks.FirstOrDefault(p => p.ID == shopStockId);
                resultstring = objshopStock.Qty.ToString() + "#";
            }
            else
            {
                resultstring = status.ToString(); ;
            }
            //Start Yashaswi 04-02-2019 if current available quantity is less than required quantity
            BusinessLogicLayer.CouponManagement coupon = new BusinessLogicLayer.CouponManagement();

            coupon.SetCouponCookie(couponCode, shopStockId, couponAmount, couponPercent);

            return Json(resultstring, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private string GetCategoryDescription(string CategoryName)
        {
            try
            {

                BusinessLogicLayer.SEOManagement seoContent = new SEOManagement(System.Web.HttpContext.Current.Server);

                return seoContent.GetSEOContent(CategoryName, SEOManagement.CONTENT_FOR.CATEGORY, SEOManagement.CATEGEORY_LEVEL.LEVEL2);

                //return System.IO.File.ReadAllText(Server.MapPath("~/cat_desc/level2/" + CategoryName + ".txt"));
            }
            catch { return string.Empty; }
        }

        //MAde by Harshada
        #region Corporate Gift

        public ActionResult CorporateBuy(long itemId, long shopstockId)
        {
            try
            {
                if (Session["UID"] != null)
                {

                    ShopProductVarientViewModelCollection shopProductCollection = new ShopProductVarientViewModelCollection();
                    CorporateGift cg = new CorporateGift();
                    shopProductCollection = cg.GetProductDetails(itemId, shopstockId, fConnectionString);
                    List<CorporateShippingFacility> lCorporateShippingFacilities = new List<CorporateShippingFacility>();


                    lCorporateShippingFacilities = db.CorporateShippingFacilities.ToList();
                    shopProductCollection.lCorporateShippingFacility = lCorporateShippingFacilities;
                    if (shopProductCollection.lShopProductVarientViewModel.Count() == 1)
                    {
                        decimal SaleRate = shopProductCollection.lShopProductVarientViewModel[0].SaleRate;
                        ViewBag.SaleRate = SaleRate;
                    }

                    TempData["CorporateGiftCartDetail"] = shopProductCollection;
                    TempData.Keep();
                    return View("CorporateAddress", shopProductCollection);
                }
                else
                {
                    return RedirectToAction("login", "Login");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CorporateBuy][POST:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CorporateBuy][POST:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        public ActionResult GetArea(string Pincode)
        {
            /*This Action Responces to AJAX Call
             * After entering Pincode returens City, District and State Information
             * */

            var errorMsg = "";
            ModelLayer.Models.Pincode pincode = db.Pincodes.FirstOrDefault(p => p.Name == Pincode);
            if (!(db.Pincodes.Any(p => p.Name == Pincode)))
            {
                errorMsg = "1";
                return Json(errorMsg);
            }


            var lData = (from a in db.Areas
                         where a.PincodeID == pincode.ID
                         select new Area { ID = a.ID, Name = a.Name, PincodeID = pincode.ID }).ToList();

            return Json(lData);



        }



        //public JsonResult MakeOrder(decimal SaleRate, int TotalQuantity, List<CorporateFacilityDetailsViewModel> CorporateFacilityDetailsViewModel)
        //{
        //    string Message = "";
        //    try
        //    {
        //        if (Session["UID"] != null)
        //        {
        //            decimal OrderAmount = SaleRate * TotalQuantity;
        //            decimal DeliveryCharges = 0.5M;
        //            decimal TotalDeliveryCharges = DeliveryCharges * TotalQuantity;
        //            decimal PayableAmount = OrderAmount + TotalDeliveryCharges;
        //            decimal FacilityCharges=0;
        //            decimal TotalFacilityCharges = 0;
        //            if (CorporateFacilityDetailsViewModel != null)
        //            {
        //                foreach (var v in CorporateFacilityDetailsViewModel)
        //                {
        //                     TotalFacilityCharges = 0;
        //                    FacilityCharges = v.ShippingFacilityCharges;
        //                    TotalFacilityCharges = FacilityCharges * TotalQuantity;
        //                    PayableAmount = PayableAmount + TotalFacilityCharges;
        //                }
        //            }

        //            long UserID = Convert.ToInt64(Session["UID"]);
        //            ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
        //            lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CorporateGiftCartDetail"];
        //            lShoppingCartCollection.lShoppingCartOrderDetails = new ShoppingCartOrderDetails();

        //            lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount = OrderAmount;
        //            lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = PayableAmount;

        //            List<CorporateDetail> lCorporateDetail = new List<CorporateDetail>();
        //            CorporateDetail CorporateDetail = new ModelLayer.Models.ViewModel.CorporateDetail();
        //            lCorporateDetail.Add(CorporateDetail);
        //            lCorporateDetail[0].TotalQuantity = TotalQuantity;
        //            lCorporateDetail[0].DeliveryCharges = TotalDeliveryCharges;
        //            lCorporateDetail[0].TotalProductAmount = OrderAmount;
        //            lCorporateDetail[0].TotalFacilityCharge = TotalFacilityCharges;

        //            var Detail = (from pd in db.PersonalDetails
        //                          join
        //                              ul in db.UserLogins on pd.UserLoginID equals ul.ID
        //                          where pd.UserLoginID == UserID
        //                          select new
        //                          {
        //                              pd.AlternateMobile,
        //                              ul.Mobile,


        //                          });
        //            foreach (var v in Detail)
        //            {

        //                lCorporateDetail[0].FromPrimaryMob = v.Mobile;
        //                lCorporateDetail[0].SecondaryMob = v.AlternateMobile;
        //            }
        //            TempData["CorporateFacilityDetail"] = CorporateFacilityDetailsViewModel;
        //            TempData["CorporateGiftCartDetail"] = lShoppingCartCollection;
        //            TempData["CorporateAddressDetail"] = lCorporateDetail;
        //            TempData.Keep();
        //            if (TempData["CorporateGiftCartDetail"] != null)
        //            {
        //                Message = "1";
        //            }



        //        }

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    if (Message == "1")
        //    {
        //        return Json(new { ok = true, newurl = Url.Action("CustomerPaymentProcess", "PaymentProcess", new { CorporateGift = "1" }) });

        //    }
        //    else
        //    {
        //        return Json("");
        //    }

        //}



        [HttpPost]
        public ActionResult MakeOrder(decimal SaleRate, int TotalQuantity, List<CorporateFacilityDetailsViewModel> CorporateFacilityDetailsViewModel)
        {
            string Message = "";
            try
            {
                if (Session["UID"] != null)
                {
                    decimal OrderAmount = SaleRate * TotalQuantity;
                    decimal DeliveryCharges = 50M;
                    decimal TotalDeliveryCharges = DeliveryCharges * TotalQuantity;
                    decimal PayableAmount = OrderAmount + TotalDeliveryCharges;
                    decimal FacilityCharges = 0;
                    decimal TotalFacilityCharges = 0;
                    if (CorporateFacilityDetailsViewModel != null)
                    {
                        foreach (var v in CorporateFacilityDetailsViewModel)
                        {
                            TotalFacilityCharges = 0;
                            FacilityCharges = v.ShippingFacilityCharges;
                            TotalFacilityCharges = FacilityCharges * TotalQuantity;
                            PayableAmount = PayableAmount + TotalFacilityCharges;
                        }
                    }

                    long UserID = Convert.ToInt64(Session["UID"]);
                    ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                    lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CorporateGiftCartDetail"];
                    lShoppingCartCollection.lShoppingCartOrderDetails = new ShoppingCartOrderDetails();

                    lShoppingCartCollection.lShoppingCartOrderDetails.TotalOrderAmount = OrderAmount;
                    lShoppingCartCollection.lShoppingCartOrderDetails.PayableAmount = PayableAmount;

                    List<CorporateDetail> lCorporateDetail = new List<CorporateDetail>();
                    CorporateDetail CorporateDetail = new ModelLayer.Models.ViewModel.CorporateDetail();
                    lCorporateDetail.Add(CorporateDetail);
                    lCorporateDetail[0].TotalQuantity = TotalQuantity;
                    lCorporateDetail[0].TotalDeliveryCharge = TotalDeliveryCharges;
                    lCorporateDetail[0].TotalProductAmount = OrderAmount;
                    lCorporateDetail[0].TotalFacilityCharge = TotalFacilityCharges;
                    lCorporateDetail[0].TotalOrderAmount = TotalDeliveryCharges + OrderAmount + TotalFacilityCharges;



                    var Detail = (from pd in db.PersonalDetails
                                  join
                                      ul in db.UserLogins on pd.UserLoginID equals ul.ID
                                  where pd.UserLoginID == UserID
                                  select new
                                  {
                                      pd.AlternateMobile,
                                      ul.Mobile,


                                  });
                    foreach (var v in Detail)
                    {

                        lCorporateDetail[0].FromPrimaryMob = v.Mobile;
                        lCorporateDetail[0].SecondaryMob = v.AlternateMobile;
                    }
                    TempData["CorporateFacilityDetail"] = CorporateFacilityDetailsViewModel;
                    TempData["CorporateGiftCartDetail"] = lShoppingCartCollection;
                    TempData["CorporateAddressDetail"] = lCorporateDetail;
                    TempData.Keep();
                    if (TempData["CorporateGiftCartDetail"] != null)
                    {
                        Message = "1";
                    }



                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[MakeOrder][POST:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[MakeOrder][POST:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            if (Message == "1")
            {
                return Json(new { ok = true, newurl = Url.Action("CustomerPaymentProcess", "PaymentProcess", new { CorporateGift = "1" }) });
                //return Json(new { ok = true, newurl = Url.Action("ThankYouCorporate1", "Product") });

            }
            else
            {
                return Json("");
            }

        }
        public ActionResult ThankYouCorporate1()
        {
            try
            {
                List<CorporateDetail> lCorporateDetail = new List<CorporateDetail>();
                lCorporateDetail = (List<CorporateDetail>)TempData["CorporateAddressDetail"];
                TempData.Keep();
                //return View("ThankYouCorporate1", lCorporateDetail);
                return View("Testing", lCorporateDetail);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ThankYouCorporate1][POST:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ThankYouCorporate1][POST:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        public ActionResult PlaceCorporateOrder(List<CorporateDetail> CorporateDetails)
        {
            long CustomerOrderID = 0;
            long userLoginID = Convert.ToInt64(Session["UID"]);
            try
            {
                OrderViewModel orderViewModel = new OrderViewModel();
                ShopProductVarientViewModelCollection lShoppingCartCollection = new ShopProductVarientViewModelCollection();
                lShoppingCartCollection = (ShopProductVarientViewModelCollection)TempData["CorporateGiftCartDetail"];

                List<CorporateDetail> corporateAddress = new List<CorporateDetail>();
                corporateAddress = (List<CorporateDetail>)TempData["CorporateAddressDetail"];
                CorporateDetails[0].FromPrimaryMob = corporateAddress[0].FromPrimaryMob;
                CorporateDetails[0].TotalDeliveryCharge = corporateAddress[0].TotalDeliveryCharge;
                CorporateDetails[0].TotalFacilityCharge = corporateAddress[0].TotalFacilityCharge;
                CorporateDetails[0].TotalProductAmount = corporateAddress[0].TotalProductAmount;
                CorporateDetails[0].TotalQuantity = corporateAddress[0].TotalQuantity;

                List<CorporateCustomerShippingAddressViewModel> corporateshippingAddress = new List<CorporateCustomerShippingAddressViewModel>();
                foreach (var v in CorporateDetails)
                {
                    CorporateCustomerShippingAddressViewModel lCorporateCustomerShippingAddressViewModel = new CorporateCustomerShippingAddressViewModel();
                    lCorporateCustomerShippingAddressViewModel.ToName = v.Name;
                    lCorporateCustomerShippingAddressViewModel.Quantity = v.Quantity;
                    lCorporateCustomerShippingAddressViewModel.PrimaryMobile = v.PrimaryMob;
                    lCorporateCustomerShippingAddressViewModel.SecondaryMobile = v.SecondaryMob;
                    lCorporateCustomerShippingAddressViewModel.ShippingAddress = v.Address;
                    lCorporateCustomerShippingAddressViewModel.PincodeID = v.PincodeID;
                    lCorporateCustomerShippingAddressViewModel.AreaID = v.AreaID;
                    lCorporateCustomerShippingAddressViewModel.DeliveryCharges = v.DeliveryCharges;
                    lCorporateCustomerShippingAddressViewModel.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(4);


                    corporateshippingAddress.Add(lCorporateCustomerShippingAddressViewModel);
                }
                List<CorporateFacilityDetailsViewModel> corporatefacility = new List<CorporateFacilityDetailsViewModel>();
                corporatefacility = (List<CorporateFacilityDetailsViewModel>)TempData["CorporateFacilityDetail"];
                BusinessLogicLayer.CustomerOrder custorder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                CustomerOrderID = custorder.PlaceCorporateOrder(lShoppingCartCollection, CorporateDetails, corporateshippingAddress, corporatefacility, Convert.ToInt64(Session["UID"]));
                BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);

                if (CustomerOrderID > 0)
                {
                    orderPlaced.SendSMSToCustomer(userLoginID, CustomerOrderID);
                    orderPlaced.SendMailToCustomer(userLoginID, CustomerOrderID);
                    orderPlaced.SendMailToMerchant(userLoginID, CustomerOrderID);
                    return Json(new { ok = true, newurl = Url.Action("ThankYou", "Product") });
                }
                else
                {
                    return Json(new { ok = false, newurl = Url.Action("Error") });
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PlaceCorporateOrder][POST:Products]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[PlaceCorporateOrder][POST:Products]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return Json("");
        }


        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult ContinueShopping()
        {
            string Url = "";
            if (Request.Cookies["UrlCookie"] != null)
            {
                Url = Request.Cookies["UrlCookie"].Value.ToString();
            }
            return Redirect(Url);
        }
        #endregion


        #region ----- Compare Product Code -----
        /// <summary>
        /// Status is 1 := Product Added All Ok  
        /// Status is 2 := Product Already Added
        /// Status is 3 := Product limit exceed
        /// Status is 4 := Product Category Not Similer
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        /// 
        //[HttpPost]
        public JsonResult AddProductInCompareList(ProductCompareClass myData)
        {
            CompareProduct compareProduct = new CompareProduct();
            //List<CampareProductViewModel> campareProduct = new List<CampareProductViewModel>();
            int CompareCount;


            if (HttpContext.Request.Cookies["CompareCount"] != null)
            {
                CompareCount = Convert.ToInt32(HttpContext.Request.Cookies["CompareCount"].Value.ToString());
            }
            else
            {
                CompareCount = 0;
            }
            int status = 0;


            if (CompareCount < 3)
            {
                int oprResult = compareProduct.SetCookies(myData.itemId, myData.ImgPath, myData.ProductName, myData.ShopStockID);
                if (oprResult == 1)
                {
                    HttpContext.Response.Cookies["CompareCount"].Value = (++CompareCount).ToString();
                    HttpContext.Response.Cookies.Add(HttpContext.Response.Cookies["CompareCount"]);
                    HttpContext.Response.Cookies["CompareCount"].Expires = System.DateTime.Now.AddDays(30);
                    status = 1;
                }
                else
                {
                    status = oprResult;
                }
            }
            else
            {
                status = 3;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddProductInCompareList(long itemId)
        //{
        //    return Json("");
        //}
        public JsonResult DeleteFromCompareProduct(long itemID)
        {
            CompareProduct compareProduct = new CompareProduct();

            //int oprResult = compareProduct.DeleteItemCookie(itemID.ToString());

            int oprResult = compareProduct.DeleteItemCookie(itemID.ToString());


            return Json(oprResult, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult cookiesList(string cookiesList)
        {
            List<CookiesDisplayClass> lst = new List<CookiesDisplayClass>();
            try
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;

                string[] Cookies_Split = cook.Split(',');
                //string[] ItemID_Split = Cookies_Split[0].Split('$');

                foreach (var item in Cookies_Split)
                {
                    CookiesDisplayClass obj = new CookiesDisplayClass();
                    string[] ItemID_Split = item.Split('$');
                    obj.ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
                    obj.ImagePath = ItemID_Split[1].ToString();
                    obj.ShopStockID = Convert.ToInt64(ItemID_Split[3].ToString());
                    obj.Name = db.Products.Where(x => x.ID == obj.ProductID).FirstOrDefault().Name; //ItemID_Split[2].ToString();
                    obj.CategoryID = db.Products.Where(x => x.ID == obj.ProductID).FirstOrDefault().CategoryID;
                    obj.CategoryName = db.Products.Where(x => x.ID == obj.ProductID).FirstOrDefault().Category.Name;
                    lst.Add(obj);
                }
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(lst, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion





    }

    public class CookiesDisplayClass
    {
        public Int64 ProductID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public Int64 ShopStockID { get; set; }
        public Int64 CategoryID { get; set; }

        public string CategoryName { get; set; }
    }

    public class ProductCompareClass
    {
        public long itemId { get; set; }
        public string ImgPath { get; set; }
        public string ProductName { get; set; }
        public long ShopStockID { get; set; }
    }
}