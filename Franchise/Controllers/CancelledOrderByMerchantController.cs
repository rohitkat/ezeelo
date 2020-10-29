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
using System.Collections.Generic;
using System;
using Franchise.Models;

namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class CancelledOrderByMerchantController : Controller
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
        [CustomAuthorize(Roles = "CancelledOrderByMerchant/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? OrderStatus, int? page, string SearchString = "")
        {
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchString = SearchString;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                var customerorders = (from CO in db.CustomerOrders
                                      join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                      join CUS in db.PersonalDetails on CO.UserLoginID equals (CUS.UserLoginID)
                                      join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                      join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                      join S in db.Shops on SP.ShopID equals S.ID
                                      where S.FranchiseID == franchiseID
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
                    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);

                    customerorders = customerorders.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
                }

                SearchString = SearchString.Trim();
                if (SearchString != "")
                {
                    return View(customerorders.Where(x => x.OrderCode.ToString().ToUpper().Contains(SearchString.ToUpper())).ToPagedList(pageNumber, pageSize));
                }
                return View(customerorders.ToList().OrderByDescending(x => x.ID).ToPagedList(pageNumber, pageSize));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CancelledOrderByMerchant][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CancelledOrderByMerchant][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        [SessionExpire]
        [CustomAuthorize(Roles = "CancelledOrderByMerchant/CanRead")]
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
