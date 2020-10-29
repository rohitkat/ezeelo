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


namespace Franchise.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class CustomerOrderController : Controller
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
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[SessionDetails]", "Can't assign Session Details..!" + Environment.NewLine + myEx.Message);
            }
            //if (!Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel))
            //{
            //    if (Session["ID"] != null)
            //    {
            //        Session["ID"] = null;
            //    }
            //    TempData["ServerMsg"] = "You are not CustomerCare Person";
            //    Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidCustomerCare"]);
            //}
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? OrderStatus, int? page, string SearchString = "")
        {
            try
            {
                DateTime dt = new DateTime(1, 1, 1);
                SessionDetails();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchString = SearchString;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.OrderStatus = OrderStatus;

                //Added by Zubair on 18-06-2018
                if (string.IsNullOrEmpty(FromDate))
                {
                    FromDate = DateTime.Now.AddDays(-6).ToString("dd/MM/yyyy");
                    //End
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    ToDate = DateTime.UtcNow.AddHours(5.5).ToString("dd/MM/yyyy");
                }
                //End

                DateTime fDate = DateTime.Now.AddDays(-6);
                DateTime tDate = DateTime.Now;
                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                    fDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    tDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                }

                //**********Code Added by avi verma on 1/5/2017(bcoz Middle & Last name should remove from reg )****************//
                var customerorders = (from COD in db.CustomerOrderDetails
                                      join CO in db.CustomerOrders on COD.CustomerOrderID equals (CO.ID)
                                      join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                      join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                      join S in db.Shops on SP.ShopID equals S.ID
                                      where S.FranchiseID == franchiseID
                                      select new CustomerOrderViewModel
                                      {
                                          ID = CO.ID,
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
                                          WalletAmountUsed = CO.MLMAmountUsed,   // added by amit
                                          CustomerOrderDetailStatus = COD.OrderStatus,
                                          //Salutation = CO.PersonalDetail != null ? CO.Salutation.Name,
                                          PersonName = CO.PersonalDetail == null ? "" : CO.PersonalDetail.FirstName,

                                          //-------------------- Added by Tejaswee for delivery schedule (26-11-2015)
                                          //DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate == null ? dt : COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                                          //DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName

                                          DeliveryDate = CO.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate == null ? dt : COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                                          DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
                                          IsBusinessBoosterPlan = db.BoosterPlanSubscribers.Any(bp => bp.CustomerOrderId == CO.ID)

                                      }).Distinct().OrderByDescending(x => x.ID).ToList();
                //**********End Code Added by avi verma on 1/5/2017(bcoz Middle & Last name should remove from reg )****************//

                //********commented by avi verma on 1/5/2017*******//
                //var customerorders = (from CO in db.CustomerOrders
                //                      join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                //                      join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                //                      join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                //                      join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                //                      join S in db.Shops on SP.ShopID equals S.ID
                //                      where S.FranchiseID == franchiseID
                //                      select new CustomerOrderViewModel
                //                      {
                //                          ID = CO.ID,
                //                          OrderCode = CO.OrderCode,
                //                          UserLoginID = CO.UserLoginID,
                //                          ReferenceCustomerOrderID = CO.ReferenceCustomerOrderID,
                //                          OrderAmount = CO.OrderAmount,
                //                          NoOfPointUsed = CO.NoOfPointUsed,
                //                          ValuePerPoint = CO.ValuePerPoint,
                //                          CoupenCode = CO.CoupenCode,
                //                          CoupenAmount = CO.CoupenAmount,
                //                          PAN = CO.PAN,
                //                          PaymentMode = CO.PaymentMode,
                //                          PayableAmount = CO.PayableAmount,
                //                          PrimaryMobile = CO.PrimaryMobile,
                //                          SecondoryMobile = CO.SecondoryMobile,
                //                          ShippingAddress = CO.ShippingAddress,
                //                          PincodeID = CO.PincodeID,
                //                          AreaID = CO.AreaID,
                //                          CreateDate = CO.CreateDate,
                //                          CreateBy = CO.CreateBy,
                //                          ModifyDate = CO.ModifyDate,
                //                          ModifyBy = CO.ModifyBy,
                //                          NetworkIP = CO.NetworkIP,
                //                          DeviceType = CO.DeviceType,
                //                          DeviceID = CO.DeviceID,

                //                          CustomerOrderDetailStatus = COD.OrderStatus,
                //                          Salutation = PD.Salutation.Name,
                //                          PersonName = PD.FirstName + " " + PD.LastName ,//Added by Mohit 19-10-15
                //                          //db.SubscriptionPlanUsedBies.Find(CO.ID) ? "This Value" : (e.col2.HasValue ? "Other Value" : null)
                //                          //(if db.SubscriptionPlanUsedBies.Find(CO.ID) then "*" else "")

                //                           //-------------------- Added by Tejaswee for delivery schedule (26-11-2015)
                //                          //DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                //                          DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate == null ? dt : COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                //                          DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName

                //                      }).Distinct().OrderByDescending(x => x.ID).ToList();


                List<GetwayPaymentTransaction> lGetwayPaymentTransactions = db.GetwayPaymentTransactions.ToList();
                lGetwayPaymentTransactions = (from gt in lGetwayPaymentTransactions
                                              join co in customerorders on gt.CustomerOrderID equals co.ID
                                              select new GetwayPaymentTransaction
                                              {
                                                  ID = gt.ID,
                                                  PaymentMode = gt.PaymentMode,
                                                  FromUID = gt.FromUID,
                                                  ToUID = gt.ToUID,
                                                  AccountTransactionId = gt.AccountTransactionId,
                                                  PaymentGetWayTransactionId = gt.PaymentGetWayTransactionId,
                                                  Status = gt.Status,
                                                  Description = gt.Description,
                                                  TransactionDate = gt.TransactionDate,
                                                  IsActive = gt.IsActive,
                                                  CreateDate = gt.CreateDate,
                                                  CreateBy = gt.CreateBy,
                                                  ModifyDate = gt.ModifyDate,
                                                  ModifyBy = gt.ModifyBy,
                                                  CustomerOrderID = gt.CustomerOrderID,
                                                  NetworkIP = gt.NetworkIP,
                                                  DeviceType = gt.DeviceType,
                                                  DeviceID = gt.DeviceID
                                              }).ToList();
                ViewBag.GetwayPaymentTransactions = lGetwayPaymentTransactions;

                //var OrderStatus = db.CustomerOrderDetails.Include(c => c.CustomerOrder).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Shop).Include(c => c.ShopStock).ToList();
                //foreach(var x in customerorders)
                //{
                //    x.personalDetail = db.PersonalDetails.FirstOrDefault(y => y.UserLoginID == x.UserLoginID);
                //}
                //-----------------Added By Mohit----On 19-10-15---------------------------------//
                List<SubscriptionPlanUsedBy> lSubscriptionPlanUsedBies = new List<SubscriptionPlanUsedBy>();
                lSubscriptionPlanUsedBies = db.SubscriptionPlanUsedBies.ToList();

                foreach (CustomerOrderViewModel lCustomerOrderViewModel in customerorders)
                {
                    SubscriptionPlanUsedBy lSubscriptionPlanUsedBy = lSubscriptionPlanUsedBies.FirstOrDefault(x => x.CustomerOrderID == lCustomerOrderViewModel.ID);
                    if (lSubscriptionPlanUsedBy != null)
                    {
                        lCustomerOrderViewModel.PersonName = lCustomerOrderViewModel.PersonName + "*";
                    }

                }
                //-----------------End of Code By Mohit----On 19-10-15---------------------------------//
                int lCount = customerorders.Count();
                //- Added by Avi Verma. Date : 30-July-2016.
                //- Reason : New Status OnlinePaymentPending introduced.
                int lOnlinePaymentPending = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING);

                int lPlaced = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.PLACED);
                int lConfirm = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.CONFIRM);
                int lPacked = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.PACKED);
                int lDispatchFromShop = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_SHOP);
                int lInGodown = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.IN_GODOWN);
                int lDispatchFromGodown = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN);
                int lDelivered = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED);
                int lReturned = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.RETURNED);
                int lCancelled = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED);
                int lAbandoned = customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.ABANDONED); //Added by Zubair on 23-11-2017

                Dictionary<string, int> lOrderStatus = new Dictionary<string, int>();
                lOrderStatus.Add("OnlinePaymentPending", lOnlinePaymentPending);
                lOrderStatus.Add("Placed", lPlaced);
                lOrderStatus.Add("Confirm", lConfirm);
                lOrderStatus.Add("Packed", lPacked);
                lOrderStatus.Add("DispatchFromShop", lDispatchFromShop);
                lOrderStatus.Add("InGodown", lInGodown);
                lOrderStatus.Add("DispatchFromGodown", lDispatchFromGodown);
                lOrderStatus.Add("Delivered", lDelivered);
                lOrderStatus.Add("Returned", lReturned);
                lOrderStatus.Add("Cancelled", lCancelled);
                lOrderStatus.Add("Abandoned", lAbandoned); //Added by Zubair on 23-11-2017

                ViewBag.OrderStatusCount = lOrderStatus;


                //var customerorders = db.CustomerOrders.Include(c => c.Area).Include(c => c.CustomerOrder2).Include(c => c.PersonalDetail).Include(c => c.PersonalDetail1).Include(c => c.Pincode).Include(c => c.UserLogin).OrderByDescending(x => x.ID).ToList();
                switch (OrderStatus)
                {
                    case (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.PLACED:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.PLACED).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.CONFIRM:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.CONFIRM).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.PACKED:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.PACKED).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_SHOP:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_SHOP).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.IN_GODOWN:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.IN_GODOWN).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.DISPATCHED_FROM_GODOWN).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.DELIVERED:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.DELIVERED).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.RETURNED:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.RETURNED).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.CANCELLED:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED).ToList();
                        break;
                    case (int)Common.Constant.ORDER_STATUS.ABANDONED: //Added by Zubair on 23-11-2017
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.ABANDONED).ToList();
                        break;
                    default:
                        customerorders = customerorders.Where(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.PLACED).ToList();
                        break;
                }

                if ((FromDate != null && FromDate != "") || (ToDate != null && ToDate != ""))
                {
                    //DateTime lFromDate = DateTime.Now;
                    //if (DateTime.TryParse(FromDate, out lFromDate)) { }

                    //DateTime lToDate = DateTime.Now;
                    //if (DateTime.TryParse(ToDate, out lToDate)) { }
                    DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                    DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                    customerorders = customerorders.Where(x => x.CreateDate.Date >= lFromDate.Date && x.CreateDate.Date <= lToDate.Date).ToList();
                }

                SearchString = SearchString.Trim();
                if (SearchString != "")
                {
                    return View(customerorders.Where(x => x.OrderCode.ToString().ToUpper().Contains(SearchString.ToUpper())).ToPagedList(pageNumber, pageSize));
                }
                //Yashaswi 31-7-2018
                try
                {
                    if (OrderStatus == 9)
                    {
                        foreach (var item in customerorders)
                        {
                            item.Reason = db.CustomerOrderUserDefinedLogs.FirstOrDefault(p => p.CustomerOrderID == item.ID).Description;
                        }
                    }
                }
                catch
                {

                }
                //End Yashaswi

                return View(customerorders.ToPagedList(pageNumber, pageSize));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrder][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrder][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CustomerOrder/CanRead")]
        // GET: /CustomerOrder/Details/5
        public ActionResult Details(long? id)
        {
            try
            {
                SessionDetails();
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
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrder][GET:Details]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrder][GET:Details]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        //// GET: /CustomerOrder/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name");
        //    ViewBag.ReferenceCustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode");
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName");
        //    ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name");
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile");
        //    return View();
        //}

        //// POST: /CustomerOrder/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="ID,OrderCode,UserLoginID,ReferenceCustomerOrderID,OrderAmount,NoOfPointUsed,ValuePerPoint,CoupenCode,CoupenAmount,PAN,PaymentMode,PayableAmount,PrimaryMobile,SecondoryMobile,ShippingAddress,PincodeID,AreaID,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] CustomerOrder customerorder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CustomerOrders.Add(customerorder);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", customerorder.AreaID);
        //    ViewBag.ReferenceCustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorder.ReferenceCustomerOrderID);
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.ModifyBy);
        //    ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", customerorder.PincodeID);
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customerorder.UserLoginID);
        //    return View(customerorder);
        //}

        //// GET: /CustomerOrder/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CustomerOrder customerorder = db.CustomerOrders.Find(id);
        //    if (customerorder == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", customerorder.AreaID);
        //    ViewBag.ReferenceCustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorder.ReferenceCustomerOrderID);
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.ModifyBy);
        //    ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", customerorder.PincodeID);
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customerorder.UserLoginID);
        //    return View(customerorder);
        //}

        //// POST: /CustomerOrder/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="ID,OrderCode,UserLoginID,ReferenceCustomerOrderID,OrderAmount,NoOfPointUsed,ValuePerPoint,CoupenCode,CoupenAmount,PAN,PaymentMode,PayableAmount,PrimaryMobile,SecondoryMobile,ShippingAddress,PincodeID,AreaID,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID")] CustomerOrder customerorder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(customerorder).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AreaID = new SelectList(db.Areas, "ID", "Name", customerorder.AreaID);
        //    ViewBag.ReferenceCustomerOrderID = new SelectList(db.CustomerOrders, "ID", "OrderCode", customerorder.ReferenceCustomerOrderID);
        //    ViewBag.CreateBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.CreateBy);
        //    ViewBag.ModifyBy = new SelectList(db.PersonalDetails, "ID", "FirstName", customerorder.ModifyBy);
        //    ViewBag.PincodeID = new SelectList(db.Pincodes, "ID", "Name", customerorder.PincodeID);
        //    ViewBag.UserLoginID = new SelectList(db.UserLogins, "ID", "Mobile", customerorder.UserLoginID);
        //    return View(customerorder);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string CountPending()
        {
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            var customerorders = (from CO in db.CustomerOrders
                                  join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                  join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                                  join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                  join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                  join S in db.Shops on SP.ShopID equals S.ID
                                  where S.FranchiseID == franchiseID && COD.OrderStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING
                                  select new CustomerOrderViewModel
                                  {
                                      ID = CO.ID,

                                  }).Distinct().OrderByDescending(x => x.ID).ToList();
            return customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING).ToString();
        }
        //Added by Rumana on 19/04/2019
        public string CountWalletRefund()
        {
            int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            var customerorders = (from CO in db.CustomerOrders
                                  join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                  join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                                  join SS in db.ShopStocks on COD.ShopStockID equals SS.ID
                                  join SP in db.ShopProducts on SS.ShopProductID equals SP.ID
                                  join S in db.Shops on SP.ShopID equals S.ID
                                  //join WA in db.EWalletRefund_Tables on CO.ID equals WA.CustomerOrderId
                                  where S.FranchiseID == franchiseID && COD.OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED && CO.MLMAmountUsed > 0
                                  select new CustomerOrderViewModel
                                  {
                                      ID = CO.ID,

                                  }).Distinct().OrderByDescending(x => x.ID).ToList();
            List<long> Ids = customerorders.Select(x => x.ID).Distinct().ToList();
            var EwalletRefundCount = db.eWalletRefund_Table.Where(x => Ids.Contains(x.CustomerOrderId)).ToList();
            return EwalletRefundCount.Count().ToString();
        }
        //Ended by Rumana on 19/04/2019
    }
}
