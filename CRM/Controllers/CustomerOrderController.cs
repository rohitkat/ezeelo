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
using System.Web;
using System.Web.Configuration;

namespace CRM.Controllers
{
    public class CustomerOrderController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        //private string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        private int pageSize = 10;

        public void SessionDetails()
        {
            customerCareSessionViewModel.UserLoginID = Convert.ToInt64(Session["ID"]);
            customerCareSessionViewModel.Username = Session["UserName"].ToString();

            long businessDetailID = db.BusinessDetails.Where(x => x.UserLoginID == customerCareSessionViewModel.UserLoginID).Select(x => x.ID).FirstOrDefault();

            long FRID = db.Franchises.Where(x => x.BusinessDetailID == businessDetailID && x.IsActive == true).Select(y => y.ID).FirstOrDefault();
            Session["   "] = FRID;

            if (!Common.Common.GetAllLoginDetailFromSession(ref customerCareSessionViewModel))
            {
                if (Session["ID"] != null)
                {
                    Session["ID"] = null;
                }

                TempData["ServerMsg"] = "You are not CustomerCare Person";
                Response.Redirect(System.Web.Configuration.WebConfigurationManager.AppSettings["UrlForInvalidCustomerCare"]);
            }
        }

        [SessionExpire]
        public ActionResult Index(string FromDate, string ToDate, int? OrderStatus, int? page, string SearchString = "", string SearchCityString = "", string SearchFranchiseString = "") ////added string SearchFranchiseString=""
        {
            DateTime dt = new DateTime(1, 1, 1);
            SessionDetails();
            int pageNumber = (page ?? 1);
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchString = SearchString;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.OrderStatus = OrderStatus;
            ViewBag.SearchCityString = SearchCityString;
            ViewBag.SearchFranchiseString = SearchFranchiseString;////added for Multiple MCO in same city


            var customerorders = (from CO in db.CustomerOrders
                                  join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                  join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                                  join SH in db.Shops on COD.ShopID equals SH.ID//Added by Harshada
                                  join FR in db.Franchises on SH.FranchiseID equals FR.ID
                                  join BD in db.BusinessDetails on FR.BusinessDetailID equals BD.ID
                                  join P in db.Pincodes on SH.PincodeID equals P.ID
                                  join C in db.Cities on P.CityID equals C.ID
                                  //join GT in db.GetwayPaymentTransactions on CO.ID equals GT.CustomerOrderID
                                  //End Added by Harshada
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

                                      CustomerOrderDetailStatus = COD.OrderStatus,
                                      // Salutation = PD.Salutation.Name,
                                      // PersonName = CO.PersonalDetail == null ? "" : CO.PersonalDetail.FirstName,
                                      // PersonName = PD.FirstName, //- changed by Avi verma. [ Reason : Lastname may be null, and null + add any thing becomes null ]
                                      FirstName = PD.FirstName,
                                      MiddleName = PD.MiddleName,
                                      LastName = PD.LastName,


                                      DeliveryDate = CO.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate == null ? dt : COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                                      DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,

                                      FranchiseName = FR.ContactPerson,////added
                                      CityName = C.Name
                                      //---------------------
                                  }).Distinct().OrderByDescending(x => x.ID).ToList();


            //**************commented by harshada on 5/1/2017****************//
            //var customerorders = (from CO in db.CustomerOrders
            //                      join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
            //                      join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
            //                      join SH in db.Shops on COD.ShopID equals SH.ID//Added by Harshada
            //                      join FR in db.Franchises on SH.FranchiseID equals FR.ID
            //                      join BD in db.BusinessDetails on FR.BusinessDetailID equals BD.ID
            //                      join P in db.Pincodes on SH.PincodeID equals P.ID
            //                      join C in db.Cities on P.CityID equals C.ID
            //                      //join GT in db.GetwayPaymentTransactions on CO.ID equals GT.CustomerOrderID
            //                      //End Added by Harshada
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
            //                          //PersonName = PD.FirstName + " " + PD.LastName,//Added by Mohit 19-10-15

            //                          PersonName = PD.FirstName, //- changed by Avi verma. [ Reason : Lastname may be null, and null + add any thing becomes null ]
            //                          FirstName = PD.FirstName,
            //                          MiddleName = PD.MiddleName,
            //                          LastName = PD.LastName,
            //                          //db.SubscriptionPlanUsedBies.Find(CO.ID) ? "This Value" : (e.col2.HasValue ? "Other Value" : null)
            //                          //(if db.SubscriptionPlanUsedBies.Find(CO.ID) then "*" else "")

            //                          //-------------------- Added by Tejaswee for delivery schedule (26-11-2015)
            //                          DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
            //                          DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
            //                          //Added by Harshada
            //                          //FranchiseName = BD.Name,////hidee
            //                          FranchiseName = FR.ContactPerson,////added
            //                          CityName = C.Name
            //                          //---------------------
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
                lCustomerOrderViewModel.PersonName = "";
                if (lCustomerOrderViewModel.FirstName != null)
                {
                    lCustomerOrderViewModel.PersonName = lCustomerOrderViewModel.FirstName;
                }
                if (lCustomerOrderViewModel.MiddleName != null)
                {
                    lCustomerOrderViewModel.PersonName += " " + lCustomerOrderViewModel.MiddleName;
                }
                if (lCustomerOrderViewModel.LastName != null)
                {
                    lCustomerOrderViewModel.PersonName += " " + lCustomerOrderViewModel.LastName;
                }

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

            SearchCityString = SearchCityString.Trim();
            if (SearchCityString != "")
            {
                return View(customerorders.Where(x => x.CityName.ToString().ToUpper().Contains(SearchCityString.ToUpper())).ToPagedList(pageNumber, pageSize));
            }
            //-- add by Ashish for Multiple MCO in same city-- //
            SearchFranchiseString = SearchFranchiseString.Trim();
            if (SearchFranchiseString != "")
            {
                return View(customerorders.Where(x => x.FranchiseName.ToString().ToUpper().Contains(SearchFranchiseString.ToUpper())).ToPagedList(pageNumber, pageSize));
            }
            // -- End --//
            return View(customerorders.ToPagedList(pageNumber, pageSize));
        }

        [SessionExpire]
        // GET: /CustomerOrder/Details/5
        public ActionResult Details(long? id)
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

        [SessionExpire]
        public ActionResult UpdatePendingOrder(long id, int OrderStatus)
        {
            SessionDetails();
            try
            {
                CustomerOrder lCustomerOrder = db.CustomerOrders.Find(id);
                if (lCustomerOrder == null)
                {
                    return RedirectToAction("Index", "CustomerOrder", new { OrderStatus = (int)CRM.Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING, FromDate = ViewBag.FromDate, ToDate = ViewBag.ToDate });
                }
                List<CustomerOrderDetail> customerOrderDetails = db.CustomerOrderDetails.Where(x => x.CustomerOrderID == lCustomerOrder.ID).ToList();
                foreach (CustomerOrderDetail customerOrderDetail in customerOrderDetails)
                {
                    customerOrderDetail.OrderStatus = OrderStatus;
                    customerOrderDetail.ModifyDate = DateTime.Now;
                    customerOrderDetail.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                    //EzeeloDBContext db1 = new EzeeloDBContext();
                    db.Entry(customerOrderDetail).State = EntityState.Modified;
                    db.SaveChanges();

                    //stock less
                    if (OrderStatus == (int)Common.Constant.ORDER_STATUS.CANCELLED)
                    {
                        BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                        customerOrder.ManageStock(customerOrderDetail.ShopStockID, -customerOrderDetail.Qty, customerOrderDetail.WarehouseStockID, 0, 0); //Last parameter added by Zubair for Inventory on 28-03-2018
                    }

                    //Added by Zubair on 23-11-2017
                    //stock less
                    if (OrderStatus == (int)Common.Constant.ORDER_STATUS.ABANDONED)
                    {
                        BusinessLogicLayer.CustomerOrder customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                        customerOrder.ManageStock(customerOrderDetail.ShopStockID, customerOrderDetail.Qty, customerOrderDetail.WarehouseStockID, 0, 0); //Last parameter added by Zubair for Inventory on 28-03-2018
                        BoosterPlanSubscriber boosterPlan = db.BoosterPlanSubscribers.FirstOrDefault(p => p.CustomerOrderId == id);
                        if (boosterPlan != null)
                        {
                            boosterPlan.IsActive = false;
                            boosterPlan.ModifyDate = DateTime.Now;
                            boosterPlan.ModifyBy = customerCareSessionViewModel.PersonalDetailID;
                            db.SaveChanges();
                        }

                    }
                    //End

                    //----------------------------------- Insert into CustomerOrderHistory -//
                    //EzeeloDBContext db2 = new EzeeloDBContext();
                    CustomerOrderHistory lCustomerOrderHistory = new CustomerOrderHistory();
                    lCustomerOrderHistory.CustomerOrderID = customerOrderDetail.CustomerOrderID;
                    lCustomerOrderHistory.ShopStockID = customerOrderDetail.ShopStockID;
                    lCustomerOrderHistory.Status = customerOrderDetail.OrderStatus;
                    lCustomerOrderHistory.CreateBy = customerCareSessionViewModel.PersonalDetailID;
                    lCustomerOrderHistory.CreateDate = DateTime.Now;
                    db.CustomerOrderHistories.Add(lCustomerOrderHistory);
                    db.SaveChanges();
                    //db.Dispose();
                }
                if (OrderStatus == (int)Common.Constant.ORDER_STATUS.ABANDONED)
                {
                    if (lCustomerOrder.MLMAmountUsed > 0)
                    {
                        (new BusinessLogicLayer.MLMWalletPoints()).RefundUsedWalletAmount(lCustomerOrder.MLMAmountUsed.Value, lCustomerOrder.ID, 1, lCustomerOrder.UserLoginID, 9);
                    }
                }
                ////Added by Rumana for return MLM Amount Used on Abandoned Status on 20-05-2019
                //if (OrderStatus == (int)Common.Constant.ORDER_STATUS.ABANDONED)
                //{
                //    BusinessLogicLayer.MLMWalletPoints objMLMWalletPoints = new BusinessLogicLayer.MLMWalletPoints();
                //    BusinessLogicLayer.CustomerOrder _customerOrder = new BusinessLogicLayer.CustomerOrder(System.Web.HttpContext.Current.Server);
                //    _customerOrder.EWalletAmountUsed_Return(lCustomerOrder.ID, lCustomerOrder.UserLoginID);
                //    long EWalletRefunfID = _customerOrder.Insert_EwalletRefund_OnAbandonedStatus(lCustomerOrder.ID);

                //    if (EWalletRefunfID != null && EWalletRefunfID!=0)
                //    {
                //        MLMWallet objmlmw = db.MLMWallets.FirstOrDefault(x => x.UserLoginID == lCustomerOrder.UserLoginID);
                //        EWalletRefund_Table objEWalletRefund = db.eWalletRefund_Table.Where(x => x.ID == EWalletRefunfID).FirstOrDefault();

                //        MlmWalletlog objmlmlog = new MlmWalletlog();
                //        objmlmlog.Amount = objEWalletRefund.RequsetAmt;
                //        objmlmlog.IsCredit = true;
                //        objmlmlog.CurrentAmt = objmlmw.Amount;
                //        objmlmlog.UserLoginID = lCustomerOrder.UserLoginID;
                //        objmlmlog.CustomerOrderId = lCustomerOrder.ID;
                //        objmlmlog.CreatedDate = DateTime.Now;
                //        objmlmlog.NetworkID = BusinessLogicLayer.CommonFunctions.GetClientIP();
                //        objmlmlog.DeviceType = "Net Browser";
                //        objmlmlog.DeviceID = "x";
                //        objmlmlog.EwalletRefund_TableID = EWalletRefunfID;
                //        db.mlmWalletlogs.Add(objmlmlog);
                //        db.SaveChanges();
                //    }

                //}
                //----------------New Code for  GCM Alert  date 11-Nov-2016 added by Ashwini Meshram-----------------------------------------------------//
                int Status = 0;
                string lOrderCode;
                long lCustomerOrderID, lCustLoginID;
                string DeviceType = string.Empty;
                DeviceType = lCustomerOrder.DeviceType;
                if (DeviceType == "Mobile")
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            lOrderCode = customerOrderDetails.FirstOrDefault().CustomerOrder.OrderCode;
                            lCustomerOrderID = customerOrderDetails.FirstOrDefault().CustomerOrder.ID;
                            lCustLoginID = customerOrderDetails.FirstOrDefault().CustomerOrder.UserLoginID;

                            //lCustLoginID = 186713;

                            //----------------------------------------------------------------------------------------------//


                            //-----------------------------Code to send data to Customer from data base date 11-Nov-2016----------------------
                            if (OrderStatus != 0)
                            {
                                DateTime CreateDate = DateTime.Now;
                                OrderStatusEnum OrderStatusEnumVal = GetEnumValue<OrderStatusEnum>(OrderStatus);
                                BusinessLogicLayer.OrderStatusSMSandEMAIL lOrderStatusSMSandEMAIL = new BusinessLogicLayer.OrderStatusSMSandEMAIL();
                                lOrderStatusSMSandEMAIL.GCMMsgTemplate(Convert.ToString(OrderStatusEnumVal), lOrderCode, lCustomerOrderID, CreateDate);
                                if (lCustLoginID != 0)
                                {
                                    Status = lOrderStatusSMSandEMAIL.SendNotification(lCustLoginID, Convert.ToString(OrderStatusEnumVal));
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("Error", "There's something wrong with the customer order.");

                            //Code to write error log
                            BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                + Environment.NewLine + ex.Message + Environment.NewLine
                                + "[CustomerOrder][GET:Edit]",
                                BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);
                            return RedirectToAction("Index", "CustomerOrder", new { OrderStatus = (int)CRM.Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING, FromDate = ViewBag.FromDate, ToDate = ViewBag.ToDate });
                        }
                        //--------------------------------------------------------------------------------------------//
                        //------------------------ refresh parent window -//
                        //Response.Write("<script>parent.location.reload();</script>");
                        //ViewBag.Status = Status;
                    }
                    return RedirectToAction("Index", "CustomerOrder", new { OrderStatus = (int)CRM.Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING, FromDate = ViewBag.FromDate, ToDate = ViewBag.ToDate });
                }
                else
                {
                    //------------------------------------------------------------------------------------------------------------------------------------------//
                    BusinessLogicLayer.OrderPlacedSmsAndEmail orderPlaced = new BusinessLogicLayer.OrderPlacedSmsAndEmail(System.Web.HttpContext.Current.Server);

                    orderPlaced.SendSMSToCustomer(lCustomerOrder.UserLoginID, id);
                    orderPlaced.SendMailToCustomer(lCustomerOrder.UserLoginID, id);
                    orderPlaced.SendMailToMerchant(lCustomerOrder.UserLoginID, id);
                    return RedirectToAction("Index", "CustomerOrder", new { OrderStatus = (int)CRM.Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING, FromDate = ViewBag.FromDate, ToDate = ViewBag.ToDate });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's something wrong with the customer order.");

                //Code to write error log
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrder][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.CRM, System.Web.HttpContext.Current.Server);
                return RedirectToAction("Index", "CustomerOrder", new { OrderStatus = (int)CRM.Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING, FromDate = ViewBag.FromDate, ToDate = ViewBag.ToDate });
            }

        }

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
            var customerorders = (from CO in db.CustomerOrders
                                  join COD in db.CustomerOrderDetails on CO.ID equals (COD.CustomerOrderID)
                                  join PD in db.PersonalDetails on CO.UserLoginID equals PD.UserLoginID
                                  join SH in db.Shops on COD.ShopID equals SH.ID//Added by Harshada
                                  join FR in db.Franchises on SH.FranchiseID equals FR.ID
                                  join BD in db.BusinessDetails on FR.BusinessDetailID equals BD.ID
                                  join P in db.Pincodes on SH.PincodeID equals P.ID
                                  join C in db.Cities on P.CityID equals C.ID
                                  where COD.OrderStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING
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

                                      CustomerOrderDetailStatus = COD.OrderStatus,
                                      Salutation = PD.Salutation.Name,
                                      //PersonName = PD.FirstName + " " + PD.LastName,//Added by Mohit 19-10-15

                                      PersonName = PD.FirstName, //- changed by Avi verma. [ Reason : Lastname may be null, and null + add any thing becomes null ]
                                      FirstName = PD.FirstName,
                                      MiddleName = PD.MiddleName,
                                      LastName = PD.LastName,
                                      //db.SubscriptionPlanUsedBies.Find(CO.ID) ? "This Value" : (e.col2.HasValue ? "Other Value" : null)
                                      //(if db.SubscriptionPlanUsedBies.Find(CO.ID) then "*" else "")

                                      //-------------------- Added by Tejaswee for delivery schedule (26-11-2015)
                                      DeliveryDate = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliveryDate,
                                      DeliveryScheduleName = COD.CustomerOrder.OrderDeliveryScheduleDetails.FirstOrDefault().DeliverySchedule.DisplayName,
                                      //Added by Harshada
                                      //FranchiseName = BD.Name,////hide
                                      FranchiseName = FR.ContactPerson,////added
                                      CityName = C.Name
                                      //---------------------
                                  }).Distinct().OrderByDescending(x => x.ID).ToList();
            return customerorders.Count(x => x.CustomerOrderDetailStatus == (int)Common.Constant.ORDER_STATUS.ONLINE_PAYMENT_PENDING).ToString();
        }

        public enum OrderStatusEnum
        {
            NONE = 0,
            ORD_PLAC = 1,
            ORD_CONF = 2,
            ORD_DIL = 7,
            ORD_CANC = 9
        }
        public T GetEnumValue<T>(int intValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an Enumeration type.");
            }
            T OrderStatusEnumVal = ((T[])Enum.GetValues(typeof(T)))[0];

            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                if (Convert.ToInt32(enumValue).Equals(intValue))
                {
                    OrderStatusEnumVal = enumValue;
                    break;
                }
            }
            return OrderStatusEnumVal;
        }
    }
}
