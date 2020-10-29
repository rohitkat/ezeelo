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
using System.Data;
using System.Data.SqlClient;

namespace Merchant.Controllers
{
    public class OrderTransactionReportController : Controller
    {

        //
        // GET: /OrderTransactionReport/
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 200;
        //
        // GET: /OrderTransactionReport/
        // [Authorize(Roles = "OrderTransactionReport/CanRead")]
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
                throw new BusinessLogicLayer.MyException("[OrderTransactionReportController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }

        [HttpPost]
        [SessionExpire]
        //[Authorize(Roles = "OrderTransactionReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate)
        {
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
                string to = toDate.ToString();
                string[] t = to.Split('/');
                string[] ttime = t[2].Split(' ');
                DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                tod = Convert.ToDateTime(tod.ToShortDateString());
                tod = tod.AddDays(1);
                List<OrderTransactionReportViewModel> OrderTransactionReportModel = new List<OrderTransactionReportViewModel>();
                try
                {
                    DataTable lDataTableCustomerOrder = new DataTable();
                    ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                    string conn = readCon.DB_CONNECTION;
                    SqlConnection con = new SqlConnection(conn);
                    SqlCommand sqlComm = new SqlCommand("ReportCustomerOrder", con);
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    OrderTransactionReportModel = BusinessLogicLayer.Helper.CreateListFromTable<OrderTransactionReportViewModel>(dt);

                    OrderTransactionReportModel = OrderTransactionReportModel.Where(x => x.CreateDate >= frmd &&
                                               x.CreateDate <= tod && x.ShopID == (ShopID == 0 ? x.ShopID : ShopID)).ToList();

                    #region process
                    OrderTransactionReportModel = (from COVM in OrderTransactionReportModel
                                                   group COVM by new
                                                   {
                                                       COVM.COID,
                                                       COVM.OrderStatus
                                                   } into gcs
                                                   select new OrderTransactionReportViewModel
                                                   {
                                                       COID = gcs.Key.COID,
                                                       OrderCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderCode,
                                                       OrderStatus = gcs.Key.OrderStatus,
                                                       UserLoginID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).UserLoginID,
                                                       Customer = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Customer,
                                                       RegMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegMobile,
                                                       RegEmail = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegEmail,
                                                       ReferenceCustomerOrderID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ReferenceCustomerOrderID,
                                                       OrderAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderAmount,
                                                       NoOfPointUsed = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NoOfPointUsed,
                                                       ValuePerPoint = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ValuePerPoint,
                                                       CoupenCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenCode,
                                                       CoupenAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenAmount,
                                                       PAN = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PAN,
                                                       PaymentMode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PaymentMode,
                                                       PayableAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PayableAmount,
                                                       PrimaryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PrimaryMobile,
                                                       SecondoryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).SecondoryMobile,
                                                       ShippingAddress = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShippingAddress,
                                                       Area = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Area,
                                                       AreaID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).AreaID,
                                                       Pincode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Pincode,
                                                       PincodeID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PincodeID,
                                                       City = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).City,
                                                       CityID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CityID,
                                                       CreateDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateDate,
                                                       CreateBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateBy,
                                                       CreatedByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreatedByUser,
                                                       ModifyDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyDate,
                                                       ModifyBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyBy,
                                                       ModifyByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyByUser,
                                                       NetworkIP = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NetworkIP,
                                                       DeviceType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceType,
                                                       DeviceID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceID,
                                                       DeliveryDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryDate,
                                                       DeliveryTime = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryTime,
                                                       DeliveryType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryType.ToString().ToLower(),
                                                       ShopName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopName,
                                                       ShopFranchiseName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopFranchiseName ////added for Multiple MCO in Same City
                                                       //,
                                                       //TotalRecord = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalRecord,
                                                   }).ToList();
                }
                    #endregion

                catch (Exception)
                {
                    throw;
                }

                TotalCount = OrderTransactionReportModel.Count();
                ViewBag.TotalCount = TotalCount;
                OrderTransactionReportModel = OrderTransactionReportModel.OrderByDescending(x => x.CreateDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = OrderTransactionReportModel.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(OrderTransactionReportModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
        public ActionResult Export(string fromDate, string toDate, int option, int print)
        {
            try
            {
                List<OrderTransactionReportViewModel> OrderReport = new List<OrderTransactionReportViewModel>();
                OrderReport = this.Getdata(fromDate, toDate);
                if (print == 1)
                {
                    return View("ForPrint", OrderReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Order Code", typeof(string));
                dt.Columns.Add("Shop Name", typeof(string));
                dt.Columns.Add("Oeder Amount", typeof(decimal));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Primary Mobile", typeof(string));
                dt.Columns.Add("Pincode", typeof(string));
                dt.Columns.Add("Shipping Address", typeof(string));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("Franchise", typeof(string)); ////added for Multiple MCO in Same City
                dt.Columns.Add("Order Placed Date", typeof(DateTime));

                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in OrderReport)
                {
                    i = i + 1;
                    //dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                    //row.City,row.CreateDate}, false);////hide
                    dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                    row.City,row.ShopFranchiseName,row.CreateDate}, false);////added for Multiple MCO in same city
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Order Transaction Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Order Transaction Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Order Transaction Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View("Index");

        }

        public List<OrderTransactionReportViewModel> Getdata(string fromDate, string toDate)
        {
            long shopID = GetShopID();

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;


            string from = fromDate.ToString();
            string[] f = from.Split('/');
            string[] ftime = f[2].Split(' ');
            DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
            frmd = Convert.ToDateTime(frmd.ToShortDateString());
            string to = toDate.ToString();
            string[] t = to.Split('/');
            string[] ttime = t[2].Split(' ');
            DateTime tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
            tod = Convert.ToDateTime(tod.ToShortDateString());
            tod = tod.AddDays(1);
            List<OrderTransactionReportViewModel> OrderTransactionReportModel = new List<OrderTransactionReportViewModel>();
            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("ReportCustomerOrder", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                OrderTransactionReportModel = BusinessLogicLayer.Helper.CreateListFromTable<OrderTransactionReportViewModel>(dt);

                OrderTransactionReportModel = OrderTransactionReportModel.Where(x => x.CreateDate >= frmd &&
                                           x.CreateDate <= tod && x.ShopID == (shopID == 0 ? x.ShopID : shopID)).ToList();

                #region process
                OrderTransactionReportModel = (from COVM in OrderTransactionReportModel
                                               group COVM by new
                                               {
                                                   COVM.COID,
                                                   COVM.OrderStatus
                                               } into gcs
                                               select new OrderTransactionReportViewModel
                                               {
                                                   COID = gcs.Key.COID,
                                                   OrderCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderCode,
                                                   OrderStatus = gcs.Key.OrderStatus,
                                                   UserLoginID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).UserLoginID,
                                                   Customer = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Customer,
                                                   RegMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegMobile,
                                                   RegEmail = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegEmail,
                                                   ReferenceCustomerOrderID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ReferenceCustomerOrderID,
                                                   OrderAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderAmount,
                                                   NoOfPointUsed = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NoOfPointUsed,
                                                   ValuePerPoint = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ValuePerPoint,
                                                   CoupenCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenCode,
                                                   CoupenAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenAmount,
                                                   PAN = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PAN,
                                                   PaymentMode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PaymentMode,
                                                   PayableAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PayableAmount,
                                                   PrimaryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PrimaryMobile,
                                                   SecondoryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).SecondoryMobile,
                                                   ShippingAddress = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShippingAddress,
                                                   Area = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Area,
                                                   AreaID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).AreaID,
                                                   Pincode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Pincode,
                                                   PincodeID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PincodeID,
                                                   City = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).City,
                                                   CityID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CityID,
                                                   CreateDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateDate,
                                                   CreateBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateBy,
                                                   CreatedByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreatedByUser,
                                                   ModifyDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyDate,
                                                   ModifyBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyBy,
                                                   ModifyByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyByUser,
                                                   NetworkIP = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NetworkIP,
                                                   DeviceType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceType,
                                                   DeviceID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceID,
                                                   DeliveryDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryDate,
                                                   DeliveryTime = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryTime,
                                                   DeliveryType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryType.ToString().ToLower(),
                                                   ShopName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopName,
                                                   ShopFranchiseName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopFranchiseName ////added for Multiple MCO in Same City
                                                   //,
                                                   //TotalRecord = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalRecord,
                                               }).ToList();
            }
                #endregion

            catch (Exception)
            {
                throw;
            }


            return OrderTransactionReportModel;
        }
    }
}
