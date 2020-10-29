using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
    public class ShopSaleReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 30;
        //
        // GET: /ShopSaleReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopSaleReport/CanRead")]
        public ActionResult Index()
        {
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);// GetFranchiseID();
            ViewBag.shopID = new SelectList(from s in db.Shops
                                            where s.FranchiseID == FranchiseID
                                            select new StateCityFranchiseMerchantViewModel
                                            {
                                                MerchantName = s.Name,
                                                MerchantID = s.ID
                                            }, "MerchantID", "MerchantName");
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopSaleReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? shopID)
        {
            List<ShopSaleViewModel> viewTotalOrderShopWiseViewModelList = new List<ShopSaleViewModel>();
            try
            {
                long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]); //GetFranchiseID();
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.shopID = shopID;
                int TotalCount = 0;
                int TotalPages = 0;
                int pageNumber = page;
                string from = fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                frmd = Convert.ToDateTime(frmd.ToShortDateString());

                //ViewBag.fromDate = frmd;
                string to = toDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());

                //if (page == 1 && pagecount == 0)
                //{
                    tod = tod.AddDays(1);
               // }

                //ViewBag.toDate = tod;
                var TestShopSale = (from COD in db.CustomerOrderDetails
                                    join Co in db.CustomerOrders on COD.CustomerOrderID equals Co.ID
                                    join shp in db.Shops on COD.ShopID equals shp.ID
                                    where shp.FranchiseID == FranchiseID
                                    && COD.ShopID == (shopID == 0 ? COD.ShopID : shopID)
                                    && (COD.CreateDate >= frmd && COD.CreateDate <= tod)
                                    select new
                                    {
                                        COD.ID
                                    })
                                       .ToList();
                if (TestShopSale.Count > 0)
                {
                    var ShopSale = (from COD in db.CustomerOrderDetails
                                    join Co in db.CustomerOrders on COD.CustomerOrderID equals Co.ID
                                    join shp in db.Shops on COD.ShopID equals shp.ID
                                    where shp.FranchiseID == FranchiseID
                                    && COD.ShopID == (shopID == 0 ? COD.ShopID : shopID)
                                    && (COD.CreateDate >= frmd && COD.CreateDate <= tod)
                                    group COD by new
                                    {
                                        Co.OrderCode,
                                        shp.Name,
                                        COD.Shop.Address,
                                        COD.Shop.IsDeliveryOutSource,
                                        COD.Shop.ContactPerson,
                                        COD.Shop.NearestLandmark,
                                        COD.Shop.OpeningTime,
                                        COD.Shop.ClosingTime,
                                        COD.Shop.WeeklyOff,
                                        COD.Shop.IsFreeHomeDelivery,
                                        COD.Shop.MinimumAmountForFreeDelivery,
                                        COD.Shop.IsLive,
                                        COD.Shop.CreateDate

                                    }
                                        into grp
                                        select new
                                        {
                                            grp.Key.OrderCode,
                                            grp.Key.Name,
                                            grp.Key.Address,
                                            grp.Key.IsDeliveryOutSource,
                                            grp.Key.ContactPerson,
                                            grp.Key.NearestLandmark,
                                            grp.Key.OpeningTime,
                                            grp.Key.ClosingTime,
                                            grp.Key.WeeklyOff,
                                            grp.Key.IsFreeHomeDelivery,
                                            grp.Key.MinimumAmountForFreeDelivery,
                                            grp.Key.IsLive,
                                            grp.Key.CreateDate,
                                            TotalOrder = grp.Count()
                                        }).ToList().Last();
                    ShopSaleViewModel viewTotalOrderShopWiseViewModel = new ShopSaleViewModel();
                    viewTotalOrderShopWiseViewModel.CustomerOrderCode = ShopSale.OrderCode;
                    viewTotalOrderShopWiseViewModel.ShopName = ShopSale.Name;
                    viewTotalOrderShopWiseViewModel.Address = ShopSale.Address;
                    viewTotalOrderShopWiseViewModel.IsDeliveryOutSource = ShopSale.IsDeliveryOutSource;
                    viewTotalOrderShopWiseViewModel.ContactPerson = ShopSale.ContactPerson;
                    viewTotalOrderShopWiseViewModel.NearestLandmark = ShopSale.NearestLandmark;
                    viewTotalOrderShopWiseViewModel.OpeningTime = ShopSale.OpeningTime;
                    viewTotalOrderShopWiseViewModel.ClosingTime = ShopSale.ClosingTime;
                    viewTotalOrderShopWiseViewModel.WeeklyOff = ShopSale.WeeklyOff;
                    viewTotalOrderShopWiseViewModel.IsFreeHomeDelivery = ShopSale.IsFreeHomeDelivery;
                    viewTotalOrderShopWiseViewModel.MinimumAmountForFreeDelivery = ShopSale.MinimumAmountForFreeDelivery;
                    viewTotalOrderShopWiseViewModel.IsLive = ShopSale.IsLive;
                    viewTotalOrderShopWiseViewModel.TotalOrder = ShopSale.TotalOrder;
                    viewTotalOrderShopWiseViewModel.OrderedDate = ShopSale.CreateDate;
                    viewTotalOrderShopWiseViewModelList.Add(viewTotalOrderShopWiseViewModel);
                    TotalCount = viewTotalOrderShopWiseViewModelList.Count();
                    ViewBag.TotalCount = TotalCount;
                    viewTotalOrderShopWiseViewModelList = viewTotalOrderShopWiseViewModelList.OrderByDescending(x => x.OrderedDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    ViewBag.PageSize = viewTotalOrderShopWiseViewModelList.Count;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    ViewBag.TotalPages = TotalPages;
                    return View(viewTotalOrderShopWiseViewModelList);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading shop sale Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopSaleReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading shop sale Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopSaleReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(viewTotalOrderShopWiseViewModelList);
        }

        //private long GetFranchiseID()
        //{
        //    //Session["USER_LOGIN_ID"] = 1;
        //    long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
        //    long BusinessDetailID = 0;
        //    long FranchiseID = 0;
        //    try
        //    {
        //        if (UserLoginID > 0)
        //        {
        //            BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
        //            FranchiseID = Convert.ToInt32(db.Franchises.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
        //    }
        //    return FranchiseID;
        //}
        public JsonResult GetCityByStateId(int stateID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<District> ldistrict = new List<District>();
            List<City> lcity = new List<City>();
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            ldistrict = db.Districts.Where(x => x.StateID == stateID).ToList();
            foreach (var x in ldistrict)
            {

                lcity = db.Cities.Where(c => c.DistrictID == x.ID).ToList();
                foreach (var c in lcity)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.CityID = c.ID;
                    SCFM.CityName = c.Name;
                    city.Add(SCFM);
                }
            }

            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetFranchiseByCityId(int cityID)
        {
            List<StateCityFranchiseMerchantViewModel> franchise = new List<StateCityFranchiseMerchantViewModel>();

            var lFranchise = from f in db.Franchises
                             join pin in db.Pincodes on f.PincodeID equals pin.ID
                             join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                             join c in db.Cities on pin.CityID equals c.ID
                             where c.ID == cityID
                             select new StateCityFranchiseMerchantViewModel
                             {
                                 FranchiseName = bd.Name,
                                 FranchiseID = f.ID
                             };

            foreach (var c in lFranchise)
            {
                StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                SCFM.FranchiseID = c.FranchiseID;
                SCFM.FranchiseName = c.FranchiseName;
                franchise.Add(SCFM);
            }


            return Json(franchise.Distinct().OrderBy(x => x.FranchiseName).ToList(), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ShopSaleReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ShopSaleReport/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ShopSaleReport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ShopSaleReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ShopSaleReport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ShopSaleReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ShopSaleReport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
