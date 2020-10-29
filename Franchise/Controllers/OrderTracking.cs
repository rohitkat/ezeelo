using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using System.Collections;
using Franchise.Models.ViewModel;
using Franchise.Models;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class OrderTrackingController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            try
            {
                customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
                customerCareSessionViewModel.Username = Session["USER_NAME"].ToString();
                //Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "OrderTracking/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? page, long? CurrentOrderStatus, string SearchString = "", string Shop = "", string Delivery = "")
        {
            try
            {
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchString = SearchString;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Shop = Shop;
                ViewBag.Delivery = Delivery;
                ViewBag.CurrentOrderStatus1 = CurrentOrderStatus;

                var OrderStatus = (from COD in db.CustomerOrderDetails
                                   join SHP in db.Shops on COD.ShopID equals SHP.ID
                                   join BDSHP in db.BusinessDetails on SHP.BusinessDetailID equals BDSHP.ID
                                   join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
                                   join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
                                   join BDDP in db.BusinessDetails on DP.BusinessDetailID equals BDDP.ID
                                   where SHP.FranchiseID == franchiseID
                                   select new OrderTrackViewModel
                                   {
                                       ShopOrderCode = COD.ShopOrderCode,
                                       OrderStatus = COD.OrderStatus,
                                       ShopID = COD.ShopID,
                                       ShopName = SHP.Name,
                                       DeliveryPartnerID = DOD.DeliveryPartnerID,
                                       DeliveryPartnerName = BDDP.Name,
                                       CreateDate = COD.CreateDate

                                   }).Distinct().ToList();


                var lOrderStatus = Common.Common.DropDownListOrderStatus();
                ViewBag.CurrentOrderStatus = new SelectList(lOrderStatus, "Value", "Text", CurrentOrderStatus);
                if (CurrentOrderStatus != null)
                {
                    OrderStatus = OrderStatus.Where(x => x.OrderStatus == CurrentOrderStatus).ToList();
                }
                SearchString = SearchString.Trim();
                if (SearchString != "")
                {
                    OrderStatus = OrderStatus.Where(x => x.ShopOrderCode.ToString().ToUpper().Contains(SearchString.ToString().ToUpper())).ToList();
                }
                Shop = Shop.Trim();
                if (Shop != "")
                {
                    OrderStatus = OrderStatus.Where(x => x.ShopName.ToUpper().Contains(Shop.ToUpper())).ToList();
                }
                Delivery = Delivery.Trim();
                if (Delivery != "")
                {
                    OrderStatus = OrderStatus.Where(x => x.DeliveryPartnerName.ToUpper().Contains(Delivery.ToUpper())).ToList();
                }

                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                    DateTime lFromDate = CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = CommonFunctions.GetProperDateTime(ToDate);

                    OrderStatus = OrderStatus.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
                }
                return View(OrderStatus.ToList().OrderByDescending(x => x.CreateDate).ToPagedList(pageNumber, pageSize));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTracking][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTracking][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        public JsonResult GetShops()
        {
            var Shops = (from sp in db.Shops
                         select new ShopViewModel
                         {
                             ID = sp.ID,
                             Name = sp.Name
                         }).ToList();
            return Json(Shops, JsonRequestBehavior.AllowGet);

        }
    }
}
