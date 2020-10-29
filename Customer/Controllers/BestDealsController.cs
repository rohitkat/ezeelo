using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models.Enum;
using BusinessLogicLayer;
using System.Data;
using Gandhibagh.Models;

namespace Gandhibagh.Controllers
{

    public class BestDealsController : Controller
    {

        EzeeloDBContext db = new EzeeloDBContext();
        PriceAndOffers po = new PriceAndOffers(System.Web.HttpContext.Current.Server);
        public class WebmethodParams
        {
            public string cityID { get; set; }
            public int offerID { get; set; }
            public string offerType { get; set; }
            public string categoryID { get; set; }
            public string brandID { get; set; }
            public string pageIndex { get; set; }
            public string pageSize { get; set; }
            public int FranchiseID { get; set; }////added
        }
        public ActionResult Index()
        {
            try
            {
                //Set Cookie for Url saving & Use in Continue shopping
                URLCookie.SetCookies();

                this.GetOfferHeadings();
                // BestDealProductCollection b = new BestDealProductCollection();
                //b = SetProducts();
                // //this.GetProducts(1, "FLAT_DISCOUNT_OFFER");
                // ViewBag.offerProducts = b;
                // //BestDealsViewModel b = new BestDealsViewModel();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading Best Deal Page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BestDealsController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading Best Deal Page!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BestDealsController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            ProductStockVarientViewModel objProductStockVarientViewModel = null;
            return View(objProductStockVarientViewModel);
        }

        public void GetOfferHeadings()
        {
            try
            {
                BestDealHeadingCollection bdhc = new BestDealHeadingCollection();
                
                
                
                
                bdhc = po.GetTodaysDeals();
                ViewBag.offerHeadings = bdhc;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[BestDealsController][GetOfferHeadings]", "Can't get Best Deal headings!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Method call on offer buton click
        /// </summary>
        /// <param name="OfferID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ActionResult GetProducts(string OfferID, string OfferType)
        {
            ProductStockVarientViewModel objProductStockVarientViewModel = new ProductStockVarientViewModel();
            try
            {
                int lOfferId = 0;
                int.TryParse(OfferID, out lOfferId);
                BestDealProductSearchViewModel bdps = new BestDealProductSearchViewModel();
                bdps.CityID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[0]);
                bdps.FranchiseID = Convert.ToInt32(Request.Cookies["CityCookie"].Value.Split('$')[2]);////added
                bdps.OfferID = lOfferId;
                bdps.OfferType = OfferType;
                bdps.CategoryID = 0;
                bdps.BrandID = 0;
                bdps.PageSize = 12;
                bdps.PageIndex = 1;
                objProductStockVarientViewModel = po.GetProducts(bdps);
                ViewBag.PageCount = objProductStockVarientViewModel.searchCount.PageCount;
                ViewBag.ProductCount = objProductStockVarientViewModel.searchCount.ProductCount;
                GetOfferHeadings();
                
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading best deal products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[BestDealsController][GET:GetProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading best deal products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[BestDealsController][GET:GetProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View("Index", objProductStockVarientViewModel);
            
        }

        public BestDealProductCollection SetProducts()
        {
            BestDealProductCollection bdpc = new BestDealProductCollection();
            try
            {
                BestDealsViewModel lBestDealProducts = new BestDealsViewModel();
                // DataSet ds = lBestDealProducts.GetBestDealProducts(offerID, tableName);
                var qry = (from d in db.Products
                           select d).ToList();
                List<BestDealsViewModel> listBestDealsViewModel = new List<BestDealsViewModel>();
                for (int i = 0; i < qry.Count(); i++)
                {
                    BestDealsViewModel lBestDealsViewModel = new BestDealsViewModel();
                    lBestDealsViewModel.ProductThumbPath = "";
                    lBestDealsViewModel.CategoryID = qry[i].CategoryID;
                    lBestDealsViewModel.CategoryName = qry[i].Category.ToString();
                    lBestDealsViewModel.ProductID = qry[i].ID;
                    //lBestDealsViewModel.MRP = qry[i].;
                    //lBestDealsViewModel.Name = qry[i].CategoryID;
                    //lBestDealsViewModel.SaleRate = qry[i].CategoryID;
                    //lBestDealsViewModel.ShopStockID = qry[i].CategoryID;
                    //lBestDealsViewModel.StockStatus = qry[i].CategoryID;
                    //lBestDealsViewModel.offerID = qry[i].CategoryID;
                    //lBestDealsViewModel.ProductID = qry[i].CategoryID;
                    listBestDealsViewModel.Add(lBestDealsViewModel);
                }
               
                bdpc.bestDealsViewModel = listBestDealsViewModel;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[BestDealsController][SetProducts]", "Can't get Best Deal headings!" + Environment.NewLine + ex.Message);
            }
            return bdpc;
        }


        public JsonResult GetBestDealProducts(WebmethodParams myParam)
        {
            System.Threading.Thread.Sleep(1000);

            int lCityId = 0, lPageIndex = 0, lPageSize = 0, lOfferId = 0, lFranchiseId;
            string lOfferType = string.Empty;
            long lCategoryId = 0, lBrandId = 0;
            int.TryParse(Convert.ToString(myParam.cityID), out lCityId);
            int.TryParse(Convert.ToString(myParam.offerID), out lOfferId);
            lOfferType = myParam.offerType.ToString();
            long.TryParse(Convert.ToString(myParam.categoryID), out lCategoryId);
            long.TryParse(Convert.ToString(myParam.brandID), out lBrandId);
            int.TryParse(Convert.ToString(myParam.pageSize), out lPageSize);
            int.TryParse(Convert.ToString(myParam.pageIndex), out lPageIndex);
            int.TryParse(Convert.ToString(myParam.FranchiseID), out lFranchiseId);////added

            BestDealProductSearchViewModel bdps = new BestDealProductSearchViewModel();
            bdps.CityID = lCityId;
            bdps.OfferID = lOfferId;
            bdps.OfferType = lOfferType;
            bdps.CategoryID = lCategoryId;
            bdps.BrandID = lBrandId;
            bdps.PageSize = lPageSize;
            bdps.PageIndex = lPageIndex;
            bdps.FranchiseID = lFranchiseId;

            ProductStockVarientViewModel objProductStockVarientViewModel = po.GetProducts(bdps);

            return Json(objProductStockVarientViewModel.ProductInfo, JsonRequestBehavior.AllowGet);
        }

    }
}