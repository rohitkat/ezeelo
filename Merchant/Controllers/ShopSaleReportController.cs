using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Merchant.Models;

namespace Merchant.Controllers
{
    public class ShopSaleReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 40;
        //
        // GET: /ShopSaleReport/
        public ActionResult Index()
        {
            return View();
        }


        private long GetShopID()
        {
            EzeeloDBContext db = new EzeeloDBContext();
            //Session["USER_LOGIN_ID"] = 2;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopSaleReportController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }


        [HttpPost]
        [SessionExpire]
        //[Authorize(Roles = "ShopSaleReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate)
        {
            List<ShopSaleViewModel> viewTotalOrderShopWiseViewModelList = new List<ShopSaleViewModel>();
            try
            {
                long ShopID = GetShopID();
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                int TotalCount = 0;
                int TotalPages = 0;
                int pageNumber = page;
                string from = fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                frmd = Convert.ToDateTime(frmd.ToShortDateString());

                ViewBag.fromDate = frmd;
                string to = toDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());

                //if (page == 1 && pagecount == 0)
               // {
                    tod = tod.AddDays(1);
                //}

                ViewBag.toDate = tod;
                var ShopSale = (from COD in db.CustomerOrderDetails
                                join Co in db.CustomerOrders on COD.CustomerOrderID equals Co.ID
                                join shp in db.Shops on COD.ShopID equals shp.ID
                                where COD.ShopID.Equals(ShopID)&&
                                 (COD.CreateDate >= frmd && COD.CreateDate <= tod)
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
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Shop Sale Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopSaleReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Shop Sale Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopSaleReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(viewTotalOrderShopWiseViewModelList);
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
