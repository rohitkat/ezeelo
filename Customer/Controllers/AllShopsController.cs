using BusinessLogicLayer;
using Gandhibagh.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Gandhibagh.Controllers
{
    public class AllShopsController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();

        public ActionResult Index()
        {
            //Set Cookie for Url saving & Use in Continue shopping
            URLCookie.SetCookies();
            //AllShopsCollectionViewModel obj = new AllShopsCollectionViewModel();
            AllShops lAllShops = new AllShops();
            List<FranchiseMenu_ShopPriorityListViewModel> lcollection = new List<FranchiseMenu_ShopPriorityListViewModel>();
            try
            {
                //--------------Code added on 02-11-2015---For citywiese shoplist------------------//
                string cookieValue = ControllerContext.HttpContext.Request.Cookies["CityCookie"].Value;
                string[] arr = cookieValue.Split('$');
                long cityId = Convert.ToInt32(arr[0]);
                int franchiseId = Convert.ToInt32(arr[2]);////added by Ashish for multiple franchise in same city.
                FranchiseMenu_ShopPriorityList obj = new FranchiseMenu_ShopPriorityList();
               // lAllShops.CategoryList = obj.selectFranchiseMenu_ShopPriorityList(System.Web.HttpContext.Current.Server, cityId, 0);////hide
                lAllShops.CategoryList = obj.selectFranchiseMenu_ShopPriorityList(System.Web.HttpContext.Current.Server, cityId, 0, franchiseId);////added by Ashish for multiple franchise in same city.

                long catID = (lAllShops.CategoryList.FirstOrDefault().FirstLevel_ID.HasValue) ? lAllShops.CategoryList.FirstOrDefault().FirstLevel_ID.Value : 0;
                
                //lAllShops.ShopList = obj.selectFranchiseMenu_ShopPriorityList(System.Web.HttpContext.Current.Server, cityId, catID);////hide
                lAllShops.ShopList = obj.selectFranchiseMenu_ShopPriorityList(System.Web.HttpContext.Current.Server, cityId, catID, franchiseId);////added by Ashish for multiple franchise in same city.

                lAllShops.ShopList = lAllShops.ShopList.GroupBy(test => test.ShopID)
                   .Select(grp => grp.First())
                   .ToList();

                foreach (var item in lAllShops.ShopList)
                {
                    item.ShopLogoPath = ImageDisplay.LoadShopLogo((Int64)item.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                }

                return View(lAllShops);







                //========================================================================================
                //var qry = (from d in db.Shops
                //           // join p in db.Pincodes on d.PincodeID equals p.ID
                //           where d.IsLive == true && d.IsActive == true && d.Pincode.CityID == cityId
                //           select d).ToList();
                //List<AllShopsViewModel> listAllShopsViewModel = new List<AllShopsViewModel>();
                //for (int i = 0; i < qry.Count(); i++)
                //{
                //    AllShopsViewModel lAllShopsViewModel = new AllShopsViewModel();
                //    lAllShopsViewModel.ShopID = qry[i].ID;
                //    lAllShopsViewModel.ShopImageThumbPath = ImageDisplay.LoadShopLogo(qry[i].ID, ProductUpload.IMAGE_TYPE.Approved);
                //    lAllShopsViewModel.ShopName = qry[i].Name;
                //    listAllShopsViewModel.Add(lAllShopsViewModel);
                //}
                //obj.lAllShopsCollectionViewModel = listAllShopsViewModel;
                //return View(obj);
                //========================================================================================
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's something wrong with loading all shops!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[AllShopsController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's something wrong with loading all shops!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[AllShopsController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(lAllShops);
        }


        public ActionResult GetCategoryWiseShopList(ShopListWebmethodParams myParam)
        {
            try
            {
                AllShops lAllShops = new AllShops();
                FranchiseMenu_ShopPriorityList obj = new FranchiseMenu_ShopPriorityList();
                lAllShops.ShopList = obj.selectFranchiseMenu_ShopPriorityList(System.Web.HttpContext.Current.Server, myParam.ShopcityId, myParam.ShopcategoryId, myParam.ShopfranchiseId);////added myParam.ShopfranchiseId for multiple franchise in same city.

                lAllShops.ShopList = lAllShops.ShopList.GroupBy(test => test.ShopID)
                   .Select(grp => grp.First())
                   .ToList();
                foreach (var item in lAllShops.ShopList)
                {
                    item.ShopLogoPath = ImageDisplay.LoadShopLogo((Int64)item.ShopID, ProductUpload.IMAGE_TYPE.Approved);
                }
                return PartialView("_ShopList", lAllShops);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

    public class ShopListWebmethodParams
    {
        public int ShopcityId { get; set; }
        public int ShopcategoryId { get; set; }
        public int ShopfranchiseId { get; set; }////added
    }
}