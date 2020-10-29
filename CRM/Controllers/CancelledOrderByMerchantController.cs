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
using System.Collections.Generic;
using System;
using CRM.Models;

namespace CRM.Controllers
{
    public class CancelledOrderByMerchantController : Controller
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

        [SessionExpire]
        public ActionResult Index(string FromDate, string ToDate, int? OrderStatus, int? page, string SearchString = "")
        {
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            var customerorders = (from CO in db.CustomerOrders
                                  join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                  join CUS in db.PersonalDetails on CO.UserLoginID equals (CUS.UserLoginID)
                                   select new CancelledOrderByMerchantViewModel
                                  {
                                      ID = CO.ID,
                                      CustomerName = CUS.Salutation.Name + " " + CUS.FirstName + " " + CUS.LastName,//Added by Mohit 19-10-15
                                      OrderCode = CO.OrderCode,
                                      UserLoginID = CO.UserLoginID,
                                      ReferenceCustomerOrderID = CO.ReferenceCustomerOrderID,
                                      OrderAmount = CO.OrderAmount,
                                      NoOfPointUsed = CO.NoOfPointUsed,
                                      ValuePerPoint = CO.ValuePerPoint,
                                      CoupenCode = CO.CoupenCode,
                                      CoupenAmount = CO.CoupenAmount,
                                      PAN = CO.PAN,
                                      PaymentMode = CO.PaymentMode,
                                      PayableAmount = CO.PayableAmount,
                                      PrimaryMobile = CO.PrimaryMobile,
                                      SecondoryMobile = CO.SecondoryMobile,
                                      ShippingAddress = CO.ShippingAddress,
                                      PincodeID = CO.PincodeID,
                                      AreaID = CO.AreaID,
                                      CreateDate = CO.CreateDate,
                                      CreateBy = CO.CreateBy,
                                      ModifyDate = CO.ModifyDate,
                                      ModifyBy = CO.ModifyBy,
                                      NetworkIP = CO.NetworkIP,
                                      DeviceType = CO.DeviceType,
                                      DeviceID = CO.DeviceID,

                                      CustomerOrderDetailStatus = COD.OrderStatus
                                  }).Distinct().ToList();

            //var OrderStatus = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).ToList();

            //-----------------Added By Mohit----On 19-10-15---------------------------------//
            List<SubscriptionPlanUsedBy> lSubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
            lSubscriptionPlanUsedBies = db.SubscriptionPlanUsedBies.ToList();

            foreach (CancelledOrderByMerchantViewModel lCancelledOrderByMerchantViewModel in customerorders)
            {
                SubscriptionPlanUsedBy lSubscriptionPlanUsedBy = lSubscriptionPlanUsedBies.FirstOrDefault(x => x.CustomerOrderID == lCancelledOrderByMerchantViewModel.ID);
                if (lSubscriptionPlanUsedBy != null)
                {
                    lCancelledOrderByMerchantViewModel.CustomerName = lCancelledOrderByMerchantViewModel.CustomerName + "*";
                }

            }
            //-----------------End of Code By Mohit----On 19-10-15---------------------------------//



            int lCancelled = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED);

            Dictionary<string, int> lOrderStatus = new Dictionary<string, int>();
            lOrderStatus.Add("Cancelled", lCancelled);


            ViewBag.OrderStatusCount = lOrderStatus;

            customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED).ToList();
                 

            if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
            {
                DateTime lFromDate = DateTime.Now;
                if (DateTime.TryParse(FromDate, out lFromDate)) { }

                DateTime lToDate = DateTime.Now;
                if (DateTime.TryParse(ToDate, out lToDate)) { }

                customerorders = customerorders.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
            }

            SearchString = SearchString.Trim();
            if (SearchString != "")
            {
                return View(customerorders.Where(x => x.OrderCode.ToString().ToUpper().Contains(SearchString.ToUpper())).ToPagedList(pageNumber, pageSize));
            }
            return View(customerorders.ToList().OrderByDescending(x => x.ID).ToPagedList(pageNumber, pageSize));
        }
        [SessionExpire]
        // GET: /CustomerOrder/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerOrder customerorder = db.CustomerOrders.Find(id);
            if (customerorder == null)
            {
                return HttpNotFound();
            }
            return View(customerorder);
        }
        [SessionExpire]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
