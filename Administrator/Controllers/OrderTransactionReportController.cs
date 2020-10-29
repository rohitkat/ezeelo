using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Administrator.Models;
using System.Data;
using System.Data.SqlClient;

//<copyright file="OrderTransactionReport.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
//    </copyright>
//    <author>Harshada Raghorte</author>
namespace Administrator.Controllers
{

    public class OrderTransactionReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 200;
        //
        // GET: /OrderTransactionReport/
        [SessionExpire]
        [CustomAuthorize(Roles = "OrderTransactionReport/CanRead")]
        public ActionResult Index()
        {
            try
            {
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                return View();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Index View!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }


        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OrderTransactionReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? merchantID)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;

                int TotalCount = 0;
                int TotalPages = 0;
                ViewBag.merchantID = merchantID;
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
                                               x.CreateDate <= tod && x.ShopID == (merchantID == 0 ? x.ShopID : merchantID)).ToList();

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
                                                       ShopName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopName
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
                    + "[ OrderTransactionReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ OrderTransactionReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        public JsonResult GetCityByStateId(int stateID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<District> ldistrict = new List<District>();
            List<City> lcity = new List<City>();
            List<StateCityFranchiseMerchantViewModel> city = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                ldistrict = db.Districts.Where(x => x.StateID == stateID).ToList();
                foreach (var x in ldistrict)
                {

                    lcity = db.Cities.Where(c => c.DistrictID == x.ID).ToList();
                    foreach (var c in lcity)
                    {
                        StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                        SCFM.CityID = c.ID;
                        SCFM.CityName = c.Name;
                        city.Add(SCFM);
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetCityByStateId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling City Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetCityByStateId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return Json(city.Distinct().OrderBy(x => x.CityName).ToList(), JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetFranchiseByCityId(int cityID)
        {
            List<StateCityFranchiseMerchantViewModel> franchise = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                var lFranchise = from f in db.Franchises
                                 join pin in db.Pincodes on f.PincodeID equals pin.ID
                                 join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                 join c in db.Cities on pin.CityID equals c.ID
                                 where c.ID == cityID
                                 select new StateCityFranchiseMerchantViewModel
                                 {
                                     FranchiseName = bd.Name,
                                     FranchiseID = f.ID
                                 };

                foreach (var c in lFranchise)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.FranchiseID = c.FranchiseID;
                    SCFM.FranchiseName = c.FranchiseName;
                    franchise.Add(SCFM);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling franchise Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetFranchiseByCityId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling franchise Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetFranchiseByCityId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return Json(franchise.Distinct().OrderBy(x => x.FranchiseName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMerchantByFranchiseId(int franchiseID)
        {
            List<StateCityFranchiseMerchantViewModel> merchant = new List<StateCityFranchiseMerchantViewModel>();
            try
            {
                var lMerchant = (from s in db.Shops
                                 where s.FranchiseID == franchiseID
                                 select new StateCityFranchiseMerchantViewModel
                                 {
                                     MerchantName = s.Name,
                                     MerchantID = s.ID
                                 }).Distinct();

                foreach (var c in lMerchant)
                {
                    StateCityFranchiseMerchantViewModel SCFM = new StateCityFranchiseMerchantViewModel();
                    SCFM.MerchantID = c.MerchantID;
                    SCFM.MerchantName = c.MerchantName;
                    merchant.Add(SCFM);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Merchant Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetMerchantByFranchiseId]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in filling Merchant Dropdown!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetMerchantByFranchiseId]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }


            return Json(merchant.Distinct().OrderBy(x => x.MerchantName).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(string fromDate, string toDate, int option, int print, long? merchantID)
        {
            try
            {
                List<OrderTransactionReportViewModel> OrderReport = new List<OrderTransactionReportViewModel>();
                OrderReport = this.Getdata(fromDate, toDate, merchantID);
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
                dt.Columns.Add("Order Placed Date", typeof(DateTime));

                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in OrderReport)
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                    row.City,row.CreateDate}, false);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return View("Index");

        }

        public List<OrderTransactionReportViewModel> Getdata(string fromDate, string toDate, long? shopID)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            ViewBag.shopID = new SelectList(from s in db.Shops
                                            where s.FranchiseID == FranchiseID
                                            select new StateCityFranchiseMerchantViewModel
                                            {
                                                MerchantName = s.Name,
                                                MerchantID = s.ID
                                            }, "MerchantID", "MerchantName");
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
                                                   ShopName = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopName
                                                   //,
                                                   //TotalRecord = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalRecord,
                                               }).OrderBy(x=>x.OrderCode).ToList();
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
