using Franchise.Models;
using Franchise.Models.ViewModel;
using Franchise.SalesOrder;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Data;

namespace Franchise.Controllers
{
    public class CustomerOrderAlignmentController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private CustomerCareSessionViewModel customerCareSessionViewModel = new CustomerCareSessionViewModel();
        private int pageSize = 20;

        //
        // GET: /CustomerOrder/       
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
        [CustomAuthorize(Roles = "CustomerOrderAlignment/CanRead")]
        public ActionResult Index(string FromDate, string ToDate, int? page, int? exportType, long ULID = 0, string OrderStatus = null, string OrderCode = null,
                                  string DeliveryType = null, string Customer = null, decimal TotalAmountMin = 0, decimal TotalAmountMax = 10000000, string Mobile = null,
                                  string lPaymentMode = null, string DeliveryTime = null, string submit = null)
        {
            try
            {
                if (string.IsNullOrEmpty(FromDate))
                {
                    //commented by Zubair and modified for code optimization
                    //FromDate = DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy, hh:mm tt");
                    //Commented by Yashaswi to display last one month data
                    //FromDate = DateTime.Now.AddDays(-6).ToString("dd/MM/yyyy, hh:mm tt");
                    FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy, hh:mm tt");
                    //End
                    //End
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy, hh:mm tt");
                }

                SessionDetails();
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.ULID = ULID;
                ViewBag.Customer = Customer;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.TotalAmountMin = TotalAmountMin;
                ViewBag.TotalAmountMax = TotalAmountMax;
                ViewBag.Mobile = Mobile;
                ViewBag.DeliveryTime = DeliveryTime;
                //ViewBag.City = City;

                List<SelectListViewModel> ldata = new List<SelectListViewModel>();
                ldata.Add(new SelectListViewModel { ID = "Normal", Name = "Normal" });
                ldata.Add(new SelectListViewModel { ID = "Express", Name = "Express" });
                ViewBag.DeliveryType = new SelectList(ldata, "ID", "Name", DeliveryType);
                ViewBag.SelectedDeliveryType = DeliveryType;

                ldata = new List<SelectListViewModel>();
                ldata.Add(new SelectListViewModel { ID = "COD", Name = "COD" });
                ldata.Add(new SelectListViewModel { ID = "ONLINE", Name = "ONLINE" });
                ViewBag.PaymentMode = new SelectList(ldata, "ID", "Name", lPaymentMode);
                ViewBag.SelectedPaymentMode = lPaymentMode;

                ldata = new List<SelectListViewModel>();
                ldata.Add(new SelectListViewModel { Name = "PLACED", ID = "1" });
                ldata.Add(new SelectListViewModel { Name = "CONFIRM", ID = "2" });
                ldata.Add(new SelectListViewModel { Name = "PACKED", ID = "3" });
                ldata.Add(new SelectListViewModel { Name = "DISPATCH_FROM_SHOP", ID = "4" });
                ldata.Add(new SelectListViewModel { Name = "IN_GODOWN", ID = "5" });
                ldata.Add(new SelectListViewModel { Name = "DISPATCH_FROM_GODOWN", ID = "6" });
                ldata.Add(new SelectListViewModel { Name = "DELIVERED", ID = "7" });
                ldata.Add(new SelectListViewModel { Name = "RETURNED", ID = "8" });
                ldata.Add(new SelectListViewModel { Name = "CANCELLED", ID = "9" });
                ldata.Add(new SelectListViewModel { Name = "ABANDONED", ID = "10" }); //Added by Zubair on 01-12-2017
                ViewBag.OrderStatus = new SelectList(ldata, "ID", "Name", OrderStatus);
                ViewBag.SelectedOrderStatus = OrderStatus;

                List<OrderListStatusCount> lStatusCounts = new List<OrderListStatusCount>();
                List<NewCustomerOrderViewModel> lCustomerOrderViewModels = new List<NewCustomerOrderViewModel>();
                if (OrderCode == null || OrderCode == string.Empty)
                {
                    OrderCode = "";
                }
                if (OrderStatus == null || OrderStatus == string.Empty)
                {
                    OrderStatus = "";
                }
                if (Customer == null || Customer == string.Empty)
                {
                    Customer = "";
                }

                if (TotalAmountMin < 0)
                {
                    TotalAmountMin = 0;
                }
                if (TotalAmountMax < 0)
                {
                    TotalAmountMin = 0;
                }
                if (Mobile == null || Mobile == "")
                {
                    Mobile = "";
                }

                if (lPaymentMode == null || lPaymentMode == string.Empty)
                {
                    lPaymentMode = "";
                }

                if (DeliveryTime == null || DeliveryTime == string.Empty)
                {
                    DeliveryTime = "";
                }


                if (exportType != null && submit != null && submit.Trim().ToLower().Equals("export"))
                {
                    lCustomerOrderViewModels = Get(FromDate, ToDate, OrderCode, OrderStatus, Customer, TotalAmountMin, TotalAmountMax, Mobile,
                     lPaymentMode, DeliveryTime, out lStatusCounts, ULID, 1, 15000);
                }
                else
                {
                    lCustomerOrderViewModels = Get(FromDate, ToDate, OrderCode, OrderStatus, Customer, TotalAmountMin, TotalAmountMax, Mobile,
                     lPaymentMode, DeliveryTime, out lStatusCounts, ULID, pageNumber, pageSize);
                }


                NewOrderStatusViewModel lOrderStatusViewModel = new NewOrderStatusViewModel();
                if (lStatusCounts.Count == 0 || lStatusCounts == null)
                {
                    lStatusCounts.Add(new OrderListStatusCount());
                }
                lOrderStatusViewModel.PACKED = lStatusCounts.FirstOrDefault().Packed;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "PACKED");
                lOrderStatusViewModel.CONFIRM = lStatusCounts.FirstOrDefault().Confirm; //lCustomerOrderViewModels.Count(x => x.OrderStatus == "CONFIRM");
                lOrderStatusViewModel.PLACED = lStatusCounts.FirstOrDefault().Placed;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "PLACED");
                lOrderStatusViewModel.DISPATCH_FROM_SHOP = lStatusCounts.FirstOrDefault().DispFromShop;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "DISPATCH_FROM_SHOP");
                lOrderStatusViewModel.IN_GODOWN = lStatusCounts.FirstOrDefault().InGodown;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "IN_GODOWN");
                lOrderStatusViewModel.DISPATCH_FROM_GODOWN = lStatusCounts.FirstOrDefault().Delivered;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "DISPATCH_FROM_GODOWN");
                lOrderStatusViewModel.DELIVERED = lStatusCounts.FirstOrDefault().Delivered;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "DELIVERED");
                lOrderStatusViewModel.RETURNED = lStatusCounts.FirstOrDefault().Returned;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "RETURNED");
                lOrderStatusViewModel.CANCELLED = lStatusCounts.FirstOrDefault().Canceled;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "CANCELLED");
                lOrderStatusViewModel.ABANDONED = lStatusCounts.FirstOrDefault().Abondand;// lCustomerOrderViewModels.Count(x => x.OrderStatus == "ABANDONED"); //Added by Zubair on 01-12-2017
                lOrderStatusViewModel.TotalOrder = lStatusCounts.FirstOrDefault().Total;// lCustomerOrderViewModels.Count;                    
                ViewBag.PageCount = lStatusCounts.FirstOrDefault().PageCount;

                ViewBag.OrderStatusCount = lOrderStatusViewModel;
                if (exportType != null && submit != null && submit.Trim().ToLower().Equals("export"))
                {
                    Export((int)exportType, lCustomerOrderViewModels);
                }

                return View(lCustomerOrderViewModels.OrderByDescending(x => x.CreateDate).ToPagedList(1, pageSize));//.ToPagedList(pageNumber, pageSize)
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderAlignment][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderAlignment][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        private void Export(int exportType, List<NewCustomerOrderViewModel> lCustomerOrderViewModels)
        {
            try
            {
                //Yashaswi 31-7-2018 Add isLeader and RetailPoint
                var lCustomerOrderList = (from CO in lCustomerOrderViewModels
                                          select new
                                          {
                                              CO.OrderCode,
                                              CO.PayableAmount,
                                              CO.RetailPoints,
                                              CO.CashbackPoints,
                                              CO.Customer,
                                              CO.IsLeader,
                                              CO.RegMobile,
                                              CO.City,
                                              CO.OrderStatus,
                                              OrderDate = CO.CreateDate,
                                              CO.DeliveryDate,
                                              CO.DeliveryTime,
                                              CO.DeliveryType,
                                              CO.DeviceType,
                                              CO.PaymentMode,
                                              WalletAmountUsed = CO.MLMAmountUsed,
                                              CO.IsBusinessBoosterPlanOrder,
                                          }).ToList();

                BusinessLogicLayer.ExportExcelCsv ExportExcelCsv = new BusinessLogicLayer.ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (exportType == 1)
                {
                    ExportExcelCsv.ExportToExcel(Common.Helper.ToDataTable(lCustomerOrderList), "Customer Orders Report");
                }
                else if (exportType == 2)
                {
                    ExportExcelCsv.ExportToCSV(Common.Helper.ToDataTable(lCustomerOrderList), "Customer Orders Report");
                }
                else if (exportType == 3)
                {
                    ExportExcelCsv.ExportToPDF(Common.Helper.ToDataTable(lCustomerOrderList), "Customer Orders Report");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderAlignment][Export]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderAlignment][Export]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
        }

        public List<NewCustomerOrderViewModel> Get(string FromDate, string ToDate, string OrderCode, string OrderStatus, string Customer, decimal? TotalAmountMin, decimal? TotalAmountMax, string RegMobile,
            string PaymentMode, string DeliveryTime, out List<OrderListStatusCount> lStatusCounts, long ULID = 0, int page = 1, int pageSize = 50)
        {
            List<NewCustomerOrderViewModel> lCustomerOrderViewModels = new List<NewCustomerOrderViewModel>();
            lStatusCounts = new List<OrderListStatusCount>();
            try
            {
                CustomerDetailReport lOrder = new CustomerDetailReport();
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetExactDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetExactDateTime(ToDate);

                if (ULID == 0)
                {
                    lCustomerOrderViewModels = lOrder.ListOrders(null, franchiseID, lFromDate, lToDate, page, pageSize, OrderCode, OrderStatus, Customer, TotalAmountMin, TotalAmountMax, RegMobile,
                    PaymentMode, DeliveryTime, out lStatusCounts);
                }
                else
                {
                    lCustomerOrderViewModels = lOrder.ListOrders(ULID, franchiseID, lFromDate, lToDate, page, pageSize, OrderCode, OrderStatus, Customer, TotalAmountMin, TotalAmountMax, RegMobile,
                    PaymentMode, DeliveryTime, out lStatusCounts);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[CustomerOrderAlignment][Get]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[CustomerOrderAlignment][Get]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return lCustomerOrderViewModels;
        }


    }

    public class SelectListViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}