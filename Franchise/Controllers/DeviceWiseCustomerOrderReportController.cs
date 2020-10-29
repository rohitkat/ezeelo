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
using System.Data;
using System.Data.SqlClient;

namespace Franchise.Controllers
{
    public class DeviceWiseCustomerOrderReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        string DeviceType;
        //
        // GET: /DeviceWiseCustomerOrderReport/
        public ActionResult Index()
        {
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            return View();
        }

        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate,long ? DeviceID)
        {
          
            try
            {
                if(DeviceID==1)
                {
                    DeviceType = "x";
                }
                else if(DeviceID==2)
                {
                    DeviceType = "Mobile";
                }               
                List<OrderTransactionReportViewModel> ObjReportModel = new List<OrderTransactionReportViewModel>();
               // string DeviceID = Request.Form["Device"].ToString();
                //int FranchiseID = 2;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);  //GetFranchiseID();
                ViewBag.DeviceID = DeviceID;
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
             
                try
                {
                    DataTable lDataTableCustomerOrder = new DataTable();
                    ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                    string conn = readCon.DB_CONNECTION;
                    SqlConnection con = new SqlConnection(conn);
                    SqlCommand sqlComm = new SqlCommand("[Get_DeviceWise_Order_Report]", con);
                    sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                    sqlComm.Parameters.AddWithValue("@Fromdate", SqlDbType.DateTime).Value = frmd;
                    sqlComm.Parameters.AddWithValue("@Todate", SqlDbType.DateTime).Value = tod;
                    sqlComm.Parameters.AddWithValue("@Devicetype", SqlDbType.DateTime).Value = DeviceType;
                    sqlComm.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ObjReportModel = BusinessLogicLayer.Helper.CreateListFromTable<OrderTransactionReportViewModel>(dt);

                    //OrderTransactionReportModel = OrderTransactionReportModel.Where(x => x.CreateDate >= frmd &&
                    //                           x.CreateDate <= tod && x.ShopID == (shopID == 0 ? x.ShopID : shopID) && x.ShopFranchiseID == FranchiseID).ToList();

                }
                catch (Exception)
                {
                    throw;
                }

                TotalCount = ObjReportModel.Count();
                ViewBag.TotalCount = TotalCount;
                ObjReportModel = ObjReportModel.OrderByDescending(x => x.CreateDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = ObjReportModel.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                // return View();
                return View(ObjReportModel.OrderBy(x => x.OrderCode));
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DeviceWiseCustomerOrderReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeviceWiseCustomerOrderReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        public ActionResult Export(string fromDate, string toDate, int option, int print, long? DeviceID)
        {
            try
            {                               
                List<OrderTransactionReportViewModel> OrderReport = new List<OrderTransactionReportViewModel>();
                OrderReport = this.Getdata(fromDate, toDate, DeviceID);
                if (print == 1)
                {
                    return View("ForPrint", OrderReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Order Code", typeof(string));
                 dt.Columns.Add("Customer", typeof(string));
                dt.Columns.Add("Primary Mobile", typeof(string));

                dt.Columns.Add("Payable Amount", typeof(decimal));
                dt.Columns.Add("Order Placed Date", typeof(DateTime));
                dt.Columns.Add("Device Type", typeof(string));
                //dt.Columns.Add("Order Amount", typeof(decimal));                
                
                dt.Columns.Add("Payment Mode", typeof(string));
                
                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in OrderReport)
                {
                    i = i + 1;
                    //dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                    //row.City,row.CreateDate}, false);
                    dt.LoadDataRow(new object[] { i, row.OrderCode, row.Customer,
                       row.RegMobile,row.PayableAmount, row.CreateDate,row.DeviceType,row.PaymentMode }, false);
                }
                // dt = Common.Helper.ToDataTable(OrderReport);
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "WEB/APP Order Transacton Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "WEB/APP Order Transacton Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "WEB/APP Order Transacton Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[DeviceWiseCustomerOrderReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[DeviceWiseCustomerOrderReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View("Index");

        }

        public List<OrderTransactionReportViewModel> Getdata(string fromDate, string toDate, long? DeviceID)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            //ViewBag.shopID = new SelectList(from s in db.Shops
            //                                where s.FranchiseID == FranchiseID
            //                                select new StateCityFranchiseMerchantViewModel
            //                                {
            //                                    MerchantName = s.Name,
            //                                    MerchantID = s.ID
            //                                }, "MerchantID", "MerchantName");

          

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


                if (DeviceID == 1)
                {
                    DeviceType = "x";
                }
                else if (DeviceID == 2)
                {
                    DeviceType = "Mobile";
                }

                ViewBag.DeviceID = DeviceID;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;


                DataTable lDataTableCustomerOrder = new DataTable();
                ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                string conn = readCon.DB_CONNECTION;
                SqlConnection con = new SqlConnection(conn);
                SqlCommand sqlComm = new SqlCommand("Get_DeviceWise_Order_Report", con);
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                sqlComm.Parameters.AddWithValue("@Fromdate", SqlDbType.DateTime).Value = frmd;
                sqlComm.Parameters.AddWithValue("@Todate", SqlDbType.DateTime).Value = tod;
                sqlComm.Parameters.AddWithValue("@Devicetype", SqlDbType.DateTime).Value = DeviceType;
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                OrderTransactionReportModel = BusinessLogicLayer.Helper.CreateListFromTable<OrderTransactionReportViewModel>(dt);

                //OrderTransactionReportModel = OrderTransactionReportModel.Where(x => x.CreateDate >= frmd &&
                //                           x.CreateDate <= tod && x.DeviceID == (DeviceID == 0 ? x.DeviceID : DeviceID) && x.ShopFranchiseID == FranchiseID).ToList();

                //    #region process
                //    OrderTransactionReportModel = (from COVM in OrderTransactionReportModel
                //                                   group COVM by new
                //                                   {
                //                                       COVM.COID,
                //                                       COVM.OrderStatus
                //                                   } into gcs
                //                                   select new OrderTransactionReportViewModel
                //                                   {
                //                                       COID = gcs.Key.COID,
                //                                       OrderCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderCode,
                //                                       OrderStatus = gcs.Key.OrderStatus,
                //                                       UserLoginID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).UserLoginID,
                //                                       Customer = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Customer,
                //                                       RegMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegMobile,
                //                                       RegEmail = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).RegEmail,
                //                                       ReferenceCustomerOrderID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ReferenceCustomerOrderID,
                //                                       OrderAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).OrderAmount,
                //                                       NoOfPointUsed = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NoOfPointUsed,
                //                                       ValuePerPoint = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ValuePerPoint,
                //                                       CoupenCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenCode,
                //                                       CoupenAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CoupenAmount,
                //                                       PAN = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PAN,
                //                                       PaymentMode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PaymentMode,
                //                                       TotalPayableAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalPayableAmount,
                //                                       PrimaryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PrimaryMobile,
                //                                       SecondoryMobile = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).SecondoryMobile,
                //                                       ShippingAddress = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShippingAddress,
                //                                       Area = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Area,
                //                                       AreaID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).AreaID,
                //                                       Pincode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Pincode,
                //                                       PincodeID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PincodeID,
                //                                       City = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).City,
                //                                       CityID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CityID,
                //                                       CreateDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateDate,
                //                                       CreateBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreateBy,
                //                                       CreatedByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).CreatedByUser,
                //                                       ModifyDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyDate,
                //                                       ModifyBy = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyBy,
                //                                       ModifyByUser = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ModifyByUser,
                //                                       NetworkIP = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).NetworkIP,
                //                                       DeviceType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceType,
                //                                       DeviceID = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeviceID,
                //                                       DeliveryDate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryDate,
                //                                       DeliveryTime = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryTime,
                //                                       DeliveryType = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).DeliveryType.ToString().ToLower(),
                //                                       ShopName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopName,
                //                                       ShopOrderCode = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopOrderCode,
                //                                       ProductName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ProductName,
                //                                       Qty = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Qty,
                //                                       SaleRate = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).SaleRate,
                //                                       PayableAmount = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PayableAmount
                //                                       //TotalRecord = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalRecord,
                //                                   }).ToList();               
                //    #endregion
            }
            catch (Exception)
            {
                throw;
            }

            return OrderTransactionReportModel.OrderBy(x => x.OrderCode).ToList();
        }
       
    }
      
	
}