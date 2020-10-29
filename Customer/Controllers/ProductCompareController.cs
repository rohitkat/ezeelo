using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Gandhibagh.Controllers
{
    public class ProductCompareController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        //
        // GET: /ProductCompare/
        public ActionResult Index(Int64 categoryID = 0)
        {
            try
            {
                URLCookie.SetCookies();
                CompareProductPage obj = new CompareProductPage();
                Category lcategory = db.Categories.Where(x => x.ID == categoryID && x.Level == 3).FirstOrDefault();
                if (lcategory == null)
                {
                    return HttpNotFound("Oopss! Somthing is missing");
                }
                else
                {
                    if (TempData["status"] != null)
                    {
                        int status = Convert.ToInt32(TempData["status"]);
                        if (status == 2)
                        {
                            ViewBag.Message = "Product Already Added";
                        }
                        else if (status == 3)
                        {
                            ViewBag.Message = "Compare Product limit exceed";

                        }
                        else if (status == 4)
                        {
                            ViewBag.Message = "Compare Product Category Not Similar";
                        }
                        else if (status == 5)
                        {
                            ViewBag.Message = "Unable to Add in Compare Product!!!";

                        }
                    }

                    obj.categoryID = categoryID;
                    obj.CategoryName = lcategory.Name;  //db.Categories.Where(x => x.ID == categoryID).FirstOrDefault().Name;
                    obj.CookiesCount = BusinessLogicLayer.CompareProduct.CookiesCount();
                }
                //obj.compareProductDetails = this.CompareProductHeadsMethode(40285);
                return View(obj);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        public ActionResult CompareProductHeads(int CookiesCount)
        {
            Int64 ProductID = 0;
            BusinessLogicLayer.CompareProduct compareProduct = new CompareProduct();
            if (!compareProduct.IsCookiesNULL())
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;
                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[0].Split('$');
                ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
            }

            CompareProduct_detail obj = new CompareProduct_detail();
            CompareProductPage lst = new CompareProductPage();
            DataTable dt = new DataTable();
            dt = obj.Select_CompareProduct(ProductID, System.Web.HttpContext.Current.Server);
            lst.compareProductDetails = (from n in dt.AsEnumerable()
                                         select new CompareProductDetails
                                         {
                                             ID = n.Field<int>("id"),
                                             SpecificationID = n.Field<Int64>("SpecificationID"),
                                             SpecificationHeaderName = n.Field<string>("SpecificationName"),
                                             SpecificationParentID = n.Field<Int64?>("ParentID") == null ? null : n.Field<Int64?>("ParentID"),
                                             SpecificationValue = n.Field<string>("SpecificationValue"),
                                             Specificationlevel = n.Field<int>("sp_Level")
                                         }).ToList();


            return PartialView("_CompareCategoryHeads", lst);
        }
        public ActionResult CompareProductSpecification(int CookiesCount)
        {
            long ProductID = 0;
            if (CookiesCount == 1)
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;
                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[0].Split('$');
                ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
            }
            else if (CookiesCount == 2)
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;
                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[1].Split('$');
                ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
            }
            else if (CookiesCount == 3)
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;
                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[2].Split('$');
                ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
            }

            CompareProduct_detail obj = new CompareProduct_detail();
            CompareProductPage lst = new CompareProductPage();
            DataTable dt = new DataTable();
            if (ProductID > 0)
            {
                dt = obj.Select_CompareProduct(ProductID, System.Web.HttpContext.Current.Server);
                lst.compareProductDetails = (from n in dt.AsEnumerable()
                                             orderby n.Field<int>("id")
                                             select new CompareProductDetails
                                             {
                                                 ID = n.Field<int>("id"),
                                                 SpecificationID = n.Field<Int64>("SpecificationID"),
                                                 SpecificationHeaderName = n.Field<string>("SpecificationName"),
                                                 //SpecificationParentID = n.Field<long?>("ParentID"),
                                                 SpecificationParentID = n.Field<Int64>("ParentID"),
                                                 SpecificationValue = n.Field<string>("SpecificationValue"),
                                                 Specificationlevel = n.Field<int>("sp_Level")
                                             }).ToList();
            }
            return PartialView("_CompareProductDetails", lst);
        }
        public ActionResult ProductPreviewDetail(int CookiesCount)
        {
            CompareProductPreview previewItemViewModel = new CompareProductPreview();
            try
            {

                long itemID = 0, shopID = 0, shopStockID = 0;
                string ImagePath = string.Empty;
                if (CookiesCount == 1)
                {
                    string cook = HttpContext.Request.Cookies["ProductID"].Value;
                    string[] Cookies_Split = cook.Split(',');
                    string[] ItemID_Split = Cookies_Split[0].Split('$');
                    itemID = Convert.ToInt64(ItemID_Split[0].ToString());
                    shopStockID = Convert.ToInt64(ItemID_Split[3].ToString());
                    ImagePath = ItemID_Split[1].ToString();
                    shopID = db.ShopStocks.Where(x => x.ID == shopStockID).FirstOrDefault().ShopProduct.ShopID;
                }
                else if (CookiesCount == 2)
                {
                    string cook = HttpContext.Request.Cookies["ProductID"].Value;
                    string[] Cookies_Split = cook.Split(',');
                    string[] ItemID_Split = Cookies_Split[1].Split('$');
                    itemID = Convert.ToInt64(ItemID_Split[0].ToString());
                    shopStockID = Convert.ToInt64(ItemID_Split[3].ToString());
                    ImagePath = ItemID_Split[1].ToString();
                    shopID = db.ShopStocks.Where(x => x.ID == shopStockID).FirstOrDefault().ShopProduct.ShopID;
                }
                else if (CookiesCount == 3)
                {
                    string cook = HttpContext.Request.Cookies["ProductID"].Value;
                    string[] Cookies_Split = cook.Split(',');
                    string[] ItemID_Split = Cookies_Split[2].Split('$');
                    itemID = Convert.ToInt64(ItemID_Split[0].ToString());
                    shopStockID = Convert.ToInt64(ItemID_Split[3].ToString());
                    ImagePath = ItemID_Split[1].ToString();
                    shopID = db.ShopStocks.Where(x => x.ID == shopStockID).FirstOrDefault().ShopProduct.ShopID;
                }

                previewItemViewModel = this.getCompareProductPreviewDetail(itemID, shopStockID, shopID);
                //previewItemViewModel.Image_Path = ImagePath;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductCompareController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductCompareController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            return PartialView("_ProcductComparePreviewDetail", previewItemViewModel);
        }

        public ActionResult Product_CompareEmptySpan(int? CookiesCount)
        {
            long ProductID = 0;
            if (CookiesCount != 1)
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;
                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[0].Split('$');
                ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
            }
            CompareProduct_detail obj = new CompareProduct_detail();
            CompareProductPage lst = new CompareProductPage();
            DataTable dt = new DataTable();
            if (ProductID > 0)
            {
                dt = obj.Select_CompareProduct(ProductID, System.Web.HttpContext.Current.Server);
                lst.compareProductDetails = (from n in dt.AsEnumerable()
                                             orderby n.Field<int>("id")
                                             select new CompareProductDetails
                                             {
                                                 ID = n.Field<int>("id"),
                                                 SpecificationID = n.Field<Int64>("SpecificationID"),
                                                 SpecificationHeaderName = n.Field<string>("SpecificationName"),
                                                 //SpecificationParentID = n.Field<long?>("ParentID"),
                                                 SpecificationParentID = n.Field<Int64>("ParentID"),
                                                 SpecificationValue = "",
                                                 Specificationlevel = n.Field<int>("sp_Level")
                                             }).ToList();
            }
            return PartialView("_Product_CompareEmptySpan", lst);
        }

        public ActionResult EmptyView(long CategoryID)
        {
            this.ViewBagsForBrandAndProduct(CategoryID);
            DropDownListBag obj = new DropDownListBag();
            obj.CategoryID = CategoryID;
            obj.CategoryName = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().Name;
            return PartialView("_CompareProductEmpty", obj);
        }
        public ActionResult EmptyView2(long CategoryID)
        {
            this.ViewBagsForBrandAndProduct(CategoryID);
            DropDownListBag obj = new DropDownListBag();
            obj.CategoryID = CategoryID;
            obj.CategoryName = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().Name;
            return PartialView("_CompareProductEmpty2", obj);
        }
        public ActionResult EmptyView3(long CategoryID)
        {
            this.ViewBagsForBrandAndProduct(CategoryID);
            DropDownListBag obj = new DropDownListBag();
            obj.CategoryID = CategoryID;
            obj.CategoryName = db.Categories.Where(x => x.ID == CategoryID).FirstOrDefault().Name;
            return PartialView("_CompareProductEmpty3", obj);
        }

        public void DeleteFromCompareProduct(long itemID)
        {
            CompareProduct compareProduct = new CompareProduct();
            long CategoryID = 0;
            string catname = string.Empty;
            string categoryName = string.Empty;
            string cityName = "";
            int franchiseIDs = 0;////added
            if (HttpContext.Request.Cookies["CityCookie"].Value != null)
            {
                cityName = HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                franchiseIDs =Convert.ToInt32( HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2]); ////added
            }
            if (HttpContext.Request.Cookies["ProductID"] != null)
            {
                string cook = HttpContext.Request.Cookies["ProductID"].Value;

                string[] Cookies_Split = cook.Split(',');
                string[] ItemID_Split = Cookies_Split[0].Split('$');
                long ProductID = Convert.ToInt64(ItemID_Split[0].ToString());
                
               CategoryID= CommonFunctions.GetCatNameID(ProductID, ref catname);
                if(catname.Length>30)
                {
                    categoryName = catname.ToLower().Substring(0, 30).Replace(' ', '-').Replace("&", "%20").Replace('.','-');
                }
                else
                {
                    categoryName = catname.ToLower().Replace(' ', '-').Replace("&", "%20").Replace('.','-');
                }
                //CategoryID = db.Products.Where(x => x.ID == ProductID).FirstOrDefault().CategoryID;
            }

            int oprResult = compareProduct.DeleteItemCookie(itemID.ToString());
            //Response.Redirect("Index?categoryID=" + CategoryID.ToString(), false);
            Response.RedirectToRoute("CompareProduct", new { categoryID = CategoryID, categoryName = categoryName, city = cityName, franchiseId = franchiseIDs });////added  franchiseId = franchiseIDs
            //RedirectToAction("Index", "ProductCompare", 0);

        }

        public void ViewBagsForBrandAndProduct(long CategoryID)
        {
            List<ViewModelBrand> lbrand = new List<ViewModelBrand>();
            lbrand = (from n in db.Brands
                      join p in db.Products on n.ID equals p.BrandID
                      where p.CategoryID == CategoryID
                      select new ViewModelBrand
                      {
                          BrandID = n.ID,
                          BrandName = n.Name
                      }).Distinct().ToList();

            List<ViewModelProduct> lproduct = new List<ViewModelProduct>();
            //lproduct = (from n in db.Products
            //            where n.CategoryID == CategoryID
            //            select new ViewModelProduct
            //            {
            //                ProductID = n.ID,
            //                ProductName = n.Name
            //            }).Distinct().ToList();

            lproduct.Add(new ViewModelProduct { ProductName = "---Select Product ---", ProductID = 0 });


            ViewBag.Brand = new SelectList(lbrand.ToList(), "BrandID", "BrandName"); ;
            ViewBag.ProductID = new SelectList(lproduct, "ProductID", "ProductName");


        }
        /*Pradnyakar Code*/
        private IEnumerable<ShopProductVarientViewModel> getShopProductVarient(long itemID, long lShopID, long? lCustLoginID,  int franchiseID)////added  long cityID->int franchiseID
        {
            ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);
            IEnumerable<ShopProductVarientViewModel> listStockVarient = productDetails.GetStockVarients(itemID, lShopID, lCustLoginID, franchiseID);// added ->cityID franchiseID
            return listStockVarient;
        }

        private List<ProductSellersViewModel> getProductSellers(long itemID, long cityID, long areaID, int franchiseID)////added  long cityID->int franchiseID old
        {
            ProductDetails productDetails = new ProductDetails(System.Web.HttpContext.Current.Server);
            List<ProductSellersViewModel> listProductSellers = productDetails.GetSellersDealsInProduct(itemID, cityID, areaID, franchiseID);// added ->cityID->franchiseID old
            return listProductSellers;
        }
        private CompareProductPreview getCompareProductPreviewDetail(long productID, long shopStockID, long shopID)
        {
            CompareProductPreview compareProductPreview = new CompareProductPreview();
            long lShopID = shopID, cityID = 0, areaID = 0;
            int franchiseIDs = 0;////added
            if (ControllerContext.HttpContext.Request.Cookies["CityCookie"] != null)
            {
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                string[] arr = cookieValue.Split('$');
                cityID = Convert.ToInt64(arr[0]);
                franchiseIDs = Convert.ToInt32(arr[2]);////added
            }

            compareProductPreview.ProductID = productID;
            compareProductPreview.ShopStockID = shopStockID;
            compareProductPreview.ShopID = shopID;
            compareProductPreview.SellerName = db.Shops.Where(x => x.ID == shopID).FirstOrDefault().Name;
            compareProductPreview.ProductName = db.Products.Where(x => x.ID == productID).FirstOrDefault().Name;
            compareProductPreview.SaleRate = db.ShopStocks.Where(x => x.ID == shopStockID).FirstOrDefault().RetailerRate;
            if (db.ShopStocks.Where(x => x.ID == shopStockID).FirstOrDefault().StockStatus &&
                db.ShopStocks.Where(x => x.ID == shopStockID).FirstOrDefault().Qty > 0
                )
            {
                compareProductPreview.IsInstock = true;
            }
            else
            {
                compareProductPreview.IsInstock = false;
            }


            compareProductPreview.SellersCount = (getProductSellers(productID, cityID, areaID, franchiseIDs)).Count();////added cityID-> franchiseIDs old



            BusinessLogicLayer.Review review = new Review();
            DisplayReviewsViewModel productReviews = new DisplayReviewsViewModel();
            productReviews = review.GetReviews(productID, BusinessLogicLayer.Review.REVIEWS.PRODUCT);
            productReviews.AvgPoints.OwnerID = productID;
            compareProductPreview.productReviews = productReviews;



            if (productID != null)
            {
                var color = (from pv in db.ProductVarients
                             join c in db.Colors on pv.ColorID equals c.ID
                             where pv.ProductID == productID

                             select new
                             {
                                 name = c.Name

                             }).FirstOrDefault();
                //Tejaswee (5-11-2015)
                if (color != null && color.name != "N/A")
                {
                    //compareProductPreview.Image_Path = ImageDisplay.LoadProductThumbnails(productID, color.name, string.Empty, ProductUpload.THUMB_TYPE.SD);
                    compareProductPreview.Image_Path = ImageDisplay.SetProductThumbPath(productID, color.name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                else
                {
                    //compareProductPreview.Image_Path = ImageDisplay.LoadProductThumbnails(productID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD);
                    compareProductPreview.Image_Path = ImageDisplay.SetProductThumbPath(productID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
            }
            return compareProductPreview;
        }

        public JsonResult ProductListByBrand(long BrandID, long CategoryID)
        {
            List<ViewModelProduct> lproduct = new List<ViewModelProduct>();
            lproduct = (//from n in db.Products
                        from pv in db.ProductVarients
                        join ss in db.ShopStocks on pv.ID equals ss.ProductVarientID
                        join p in db.Products on pv.ProductID equals p.ID
                        join SP in db.ShopProducts on p.ID equals SP.ProductID
                        join S in db.Shops on SP.ShopID equals S.ID
                        join BD in db.BusinessDetails on S.BusinessDetailID equals BD.ID
                        join UL in db.UserLogins on BD.UserLoginID equals UL.ID
                        where pv.IsActive == true
                        where ss.StockStatus == true //&& ss.ID != ShopStockID
                        where ss.IsActive == true
                        where p.IsActive == true
                        //where SP.IsActive == true
                        where S.IsActive == true
                        where BD.IsActive == true
                        where UL.IsLocked == false
                        where p.CategoryID == CategoryID && p.BrandID == BrandID
                        select new ViewModelProduct
                        {
                            ProductID = p.ID,
                            ProductName = p.Name
                        }).Distinct().ToList();

            return Json(lproduct, JsonRequestBehavior.AllowGet);
        }

        public void AddProductInCompareList(long? ProductID, long? ProductID2, long? ProductID3)
        {
            long CategoryID = 0;
            string catname = string.Empty;
            string categoryName = string.Empty;
            string cityName = "";
            long ProdId = 0;
            int franchiseIds = 0;////added
            if (HttpContext.Request.Cookies["CityCookie"].Value != null)
            {
                cityName = HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[1].ToLower();
                franchiseIds =Convert.ToInt32( HttpContext.Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
            }
            if (ProductID2 != null)
            {
                ProductID = ProductID2;
                ProdId = (long)ProductID2;
            }
            else if (ProductID3 != null)
            {
                ProductID = ProductID3;
                ProdId = (long)ProductID3;
            }

            CompareProduct compareProduct = new CompareProduct();
            int CompareCount;
            CategoryID = CommonFunctions.GetCatNameID(ProdId, ref catname);
            if (catname.Length > 30)
            {
                categoryName = Regex.Replace(catname.Substring(0, 30), @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and").ToLower();
            }
            else
            {
                categoryName = Regex.Replace(catname, @"[\/\\#,+()$~%.':*?<>{} ]", "-").Replace("&", "and").ToLower();
            }
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
                int oprResult = compareProduct.SetCookies(Convert.ToInt64(ProductID));
              
                if (oprResult == 1)
                {
                    HttpContext.Response.Cookies["CompareCount"].Value = (++CompareCount).ToString();
                    HttpContext.Response.Cookies.Add(HttpContext.Response.Cookies["CompareCount"]);
                    HttpContext.Response.Cookies["CompareCount"].Expires = System.DateTime.Now.AddDays(30);
                    status = 1;
                    if (CategoryID < 1)
                    {
                        CategoryID = CommonFunctions.GetCatNameID(Convert.ToInt64(ProductID), ref catname);
                    }

                    //Response.Redirect("Index?categoryID=" + CategoryID.ToString(), false);

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

            TempData["status"] = status;
            //Response.Redirect("Index?categoryID=" + CategoryID.ToString(), false);
            if (CategoryID > 0)
            {
                Response.RedirectToRoute("CompareProduct", new { categoryID = CategoryID, categoryName = catname, city = cityName, franchiseId = franchiseIds });////added franchiseId=franchiseIds 
            }
            else
            {
                Response.RedirectToRoute("CompareProduct1", new { city = cityName, franchiseId = franchiseIds });////added franchiseId=franchiseIds 
            }
            //return View();
        }



    }
    public class CompareProductDetails
    {
        public int ID { get; set; }
        public long SpecificationID { get; set; }
        public string SpecificationHeaderName { get; set; }

        public long? SpecificationParentID { get; set; }

        public string SpecificationValue { get; set; }
        public int Specificationlevel { get; set; }


    }
    public class CompareProductPage
    {
        public Int64 categoryID { get; set; }
        public string CategoryName { get; set; }
        public int CookiesCount { get; set; }
        public List<CompareProductDetails> compareProductDetails { get; set; }

    }
    public class CompareProductPreview
    {
        public string ProductName { get; set; }
        public int SellersCount { get; set; }
        public string GeneralSpecifications { get; set; }
        public string Image_Path { get; set; }
        public bool IsInstock { get; set; }
        public long ProductID { get; set; }
        public long ShopStockID { get; set; }
        public long ShopID { get; set; }
        public string SellerName { get; set; }
        public decimal SaleRate { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public DisplayReviewsViewModel productReviews { get; set; }



    }
    public class DropDownListBag
    {
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }

    }
    public class ViewModelBrand
    {

        public long BrandID { get; set; }

        [Display]
        public string BrandName { get; set; }

    }
    public class ViewModelProduct
    {
        public long ProductID { get; set; }
        [Display]
        public string ProductName { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }

}