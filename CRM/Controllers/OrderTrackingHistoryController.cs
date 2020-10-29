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
using CRM.Models.ViewModel;
using CRM.Models;

namespace CRM.Controllers
{
    public class OrderTrackingHistoryController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();
            Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel);
        }

        //[SessionExpire]
        //public ActionResult Index(string FromDate, string ToDate, int? page, long? CurrentOrderStatus, string SearchString = "", string Shop = "", string Delivery = "")
        //{
        //    SessionDetails();
        //    int pageNumber = (page ?? 1);
        //    ViewBag.PageNumber = pageNumber;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.SearchString = SearchString;
        //    ViewBag.FromDate = FromDate;
        //    ViewBag.ToDate = ToDate;
        //    ViewBag.Shop = Shop;
        //    ViewBag.Delivery = Delivery;
        //    ViewBag.CurrentOrderStatus1 = CurrentOrderStatus;

        //    var OrderStatus = (from COH in db.CustomerOrderHistories
        //                       join COD in db.CustomerOrderDetails on COH.CustomerOrderID equals COD.CustomerOrderID
        //                       join SHP in db.Shops on COD.ShopID equals SHP.ID
        //                       join BDSHP in db.BusinessDetails on SHP.BusinessDetailID equals BDSHP.ID
        //                       join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
        //                       join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
        //                       join BDDP in db.BusinessDetails on DP.BusinessDetailID equals BDDP.ID
        //                       select new OrderTrackHistoryViewModel
        //                       {
        //                           CustomerOrderID = COH.CustomerOrderID,
        //                           ShopOrderCode = COD.ShopOrderCode,
        //                           OrderStatus = COH.Status,
        //                           ShopID = COD.ShopID,
        //                           ShopName = SHP.Name,
        //                           DeliveryPartnerID = DOD.DeliveryPartnerID,
        //                           DeliveryPartnerName = BDDP.Name,
        //                           CreateDate = COD.CreateDate,
        //                           ModifyDate = COH.CreateDate,
        //                           ModifyBy = COH.PersonalDetail.Salutation.Name + " " + COH.PersonalDetail.FirstName + " " + COH.PersonalDetail.LastName
        //                       }).Distinct().OrderBy(x => x.ShopOrderCode).ToList();

        //    foreach (OrderTrackHistoryViewModel OTH in OrderStatus)
        //    {
        //        if (OTH.OrderStatus > 1)
        //        {
        //            OTH.Duration = GetDuration(OTH.CustomerOrderID, OTH.OrderStatus, OTH.ModifyDate);
        //        }
        //    }


        //    var lOrderStatus = Common.Common.DropDownListOrderStatus();
        //    ViewBag.CurrentOrderStatus = new SelectList(lOrderStatus, "Value", "Text", CurrentOrderStatus);
        //    if (CurrentOrderStatus != null)
        //    {
        //        OrderStatus = OrderStatus.Where(x => x.OrderStatus == CurrentOrderStatus).ToList();
        //    }
        //    SearchString = SearchString.Trim();
        //    if (SearchString != "")
        //    {
        //        OrderStatus = OrderStatus.Where(x => x.ShopOrderCode.ToString().ToUpper().Contains(SearchString.ToString().ToUpper())).ToList();
        //    }
        //    Shop = Shop.Trim();
        //    if (Shop != "")
        //    {
        //        OrderStatus = OrderStatus.Where(x => x.ShopName.ToUpper().Contains(Shop.ToUpper())).ToList();
        //    }
        //    Delivery = Delivery.Trim();
        //    if (Delivery != "")
        //    {
        //        OrderStatus = OrderStatus.Where(x => x.DeliveryPartnerName.ToUpper().Contains(Delivery.ToUpper())).ToList();
        //    }

        //    if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
        //    {
        //        DateTime lFromDate = DateTime.Now;
        //        if (DateTime.TryParse(FromDate, out lFromDate)) { }

        //        DateTime lToDate = DateTime.Now;
        //        if (DateTime.TryParse(ToDate, out lToDate)) { }

        //        OrderStatus = OrderStatus.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
        //    }



        //    return View(OrderStatus.ToList().OrderByDescending(x => x.CreateDate).ToPagedList(pageNumber, pageSize));
        //}

        [SessionExpire]
        public ActionResult Index(string FromDate, string ToDate, int? page, long? CurrentOrderStatus, string SearchString = "", string Shop = "", string Delivery = "")
        {
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.Shop = Shop;
            ViewBag.Delivery = Delivery;
            ViewBag.CurrentOrderStatus1 = CurrentOrderStatus;

            var OrderStatus = (from CO in db.CustomerOrders
                               select new OrderViewModel
                               {
                                   CustomerOrderID = CO.ID,
                                   OrderCode = CO.OrderCode,
                                   CreateDate = CO.CreateDate,
                                   OrderAmount = CO.OrderAmount
                               }).Distinct().ToList();


            var lOrderStatus = Common.Common.DropDownListOrderStatus();
            ViewBag.CurrentOrderStatus = new SelectList(lOrderStatus, "Value", "Text", CurrentOrderStatus);
            //if (CurrentOrderStatus != null)
            //{
            //    OrderStatus = OrderStatus.Where(x => x.OrderStatus == CurrentOrderStatus).ToList();
            //}
            //SearchString = SearchString.Trim();
            //if (SearchString != "")
            //{
            //    OrderStatus = OrderStatus.Where(x => x.ShopOrderCode.ToString().ToUpper().Contains(SearchString.ToString().ToUpper())).ToList();
            //}
            //Shop = Shop.Trim();
            //if (Shop != "")
            //{
            //    OrderStatus = OrderStatus.Where(x => x.ShopName.ToUpper().Contains(Shop.ToUpper())).ToList();
            //}
            //Delivery = Delivery.Trim();
            //if (Delivery != "")
            //{
            //    OrderStatus = OrderStatus.Where(x => x.DeliveryPartnerName.ToUpper().Contains(Delivery.ToUpper())).ToList();
            //}

            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = DateTime.Now;
                if (DateTime.TryParse(FromDate, out lFromDate)) { }

                DateTime lToDate = DateTime.Now;
                if (DateTime.TryParse(ToDate, out lToDate)) { }

                OrderStatus = OrderStatus.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }



            return View(OrderStatus.ToList().OrderByDescending(x => x.CreateDate).ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
        public ActionResult OrderHistory(long CustomerOrderID)
        {
            SessionDetails();

            //from COD in db.CustomerOrderDetails
            //                   join SHP in db.Shops on COD.ShopID equals SHP.ID
            //                   join BDSHP in db.BusinessDetails on SHP.BusinessDetailID equals BDSHP.ID
            //                   join DOD in db.DeliveryOrderDetails on COD.ShopOrderCode equals DOD.ShopOrderCode
            //                   join DP in db.DeliveryPartners on DOD.DeliveryPartnerID equals DP.ID
            //                   join BDDP in db.BusinessDetails on DP.BusinessDetailID equals BDDP.ID


            ////var OrderHistory = (from COH in db.CustomerOrderHistories
            ////                    join CO in db.CustomerOrders on COH.CustomerOrderID equals CO.ID
            ////                    where COH.CustomerOrderID == CustomerOrderID
            ////                    select new OrderTrackHistoryViewModel
            ////                    {
            ////                        //ShopOrderCode=ShopOrderCode,
            ////                        CustomerOrderID=COH.CustomerOrderID,
            ////                        OrderStatus = COH.Status,
            ////                        CreateDate = CO.CreateDate,
            ////                        ModifyDate = COH.CreateDate,
            ////                        ModifyBy = COH.PersonalDetail.Salutation.Name + " " + COH.PersonalDetail.FirstName + " " + COH.PersonalDetail.LastName
            ////                    }).Distinct().ToList();


            var OrderHistory = (from COH in db.CustomerOrderHistories
                                join CO in db.CustomerOrders on COH.CustomerOrderID equals CO.ID                               
                                where COH.CustomerOrderID == CustomerOrderID
                                select new OrderTrackHistoryViewModel
                                {
                                    CustomerOrderID = COH.CustomerOrderID,
                                    //ShopOrderCode = COD.ShopOrderCode,
                                    OrderStatus = COH.Status,                                    
                                    //CreateDate = CO.CreateDate,
                                    ModifyDate = COH.CreateDate,
                                    ModifyBy = COH.PersonalDetail.Salutation.Name + " " + COH.PersonalDetail.FirstName + " " + COH.PersonalDetail.LastName
                                }).Distinct().ToList() ;


            foreach (OrderTrackHistoryViewModel OTH in OrderHistory)
            {
                if (OTH.OrderStatus > 1)
                {
                    OTH.Duration = GetDuration(OTH.CustomerOrderID, OTH.OrderStatus, OTH.ModifyDate);
                }
            }
            return View(OrderHistory);
        }
        private string GetDuration(long CustomerOrderID, int Status, DateTime StatusDate)
        {
            if (StatusDate != null)
            {
                Status--;
                DateTime OldStatusDate = db.CustomerOrderHistories.Where(x => x.CustomerOrderID == CustomerOrderID && x.Status == Status).OrderByDescending(x => x.ID).Select(x => x.CreateDate).FirstOrDefault();
                if (OldStatusDate != null)
                {
                    TimeSpan ts = StatusDate - OldStatusDate;
                    return ts.ToString(@"dd\.hh\:mm\:ss");
                }
            }
            return string.Empty;
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