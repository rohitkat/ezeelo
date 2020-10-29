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
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class OrderTransactionReportController : Controller
    {

        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        //
        // GET: /OrderTransaction/
        [SessionExpire]
        [CustomAuthorize(Roles = "OrderTransactionReport/CanRead")]
        public ActionResult Index()
        {
            long FranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);//GetFranchiseID();
            ViewBag.shopID = new SelectList((from s in db.Shops
                                            where s.FranchiseID == FranchiseID
                                            select new StateCityFranchiseMerchantViewModel
                                            {
                                                MerchantName = s.Name,
                                                MerchantID = s.ID
                                            }).OrderBy(x=>x.MerchantName), "MerchantID", "MerchantName");

            return View();
        }



        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "OrderTransactionReport/CanRead")]
        public ActionResult GetReport(int page, int pagecount, string fromDate, string toDate, long? shopID)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);  //GetFranchiseID();
                ViewBag.shopID = shopID;
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
                    OrderTransactionReportModel = db.Database.SqlQuery<OrderTransactionReportViewModel>(
                   "exec ezeelo.dbo.[ReportOrderTansaction] @FranchiseID ,@Fromdate,@Todate",
                   new Object[] {
                              new SqlParameter("@FranchiseID", FranchiseID),
                       new SqlParameter("@Fromdate", frmd),
                       new SqlParameter("@Todate", tod)}
                      ).ToList();

                    //DataTable lDataTableCustomerOrder = new DataTable();
                    //ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                    //string conn = readCon.DB_CONNECTION;
                    //SqlConnection con = new SqlConnection(conn);
                    //SqlCommand sqlComm = new SqlCommand("ReportOrderTansaction", con);
                    ////sqlComm.CommandTimeout = 1000;
                    //sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                    //sqlComm.Parameters.AddWithValue("@Fromdate", SqlDbType.DateTime).Value = frmd;
                    //sqlComm.Parameters.AddWithValue("@Todate", SqlDbType.DateTime).Value = tod;
                    //sqlComm.CommandType = CommandType.StoredProcedure;
                    //con.Open();

                    //SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                    //DataTable dt = new DataTable();
                    //da.Fill(dt);

                    //OrderTransactionReportModel = BusinessLogicLayer.Helper.CreateListFromTable<OrderTransactionReportViewModel>(dt);

                    OrderTransactionReportModel = OrderTransactionReportModel.Where(x => x.CreateDate >= frmd &&
                                               x.CreateDate <= tod && x.ShopID == (shopID == 0 ? x.ShopID : shopID) && x.ShopFranchiseID == FranchiseID).ToList();

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
                    //                                       ShopOrderCode=gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ShopOrderCode,
                    //                                       ProductName=gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).ProductName,
                    //                                       Qty=gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).Qty,
                    //                                       SaleRate=gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).SaleRate,
                    //                                       PayableAmount=gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).PayableAmount
                    //                                       //TotalRecord = gcs.FirstOrDefault(x => x.COID == gcs.Key.COID).TotalRecord,
                    //                                   }).ToList();
                    //
                    //    #endregion
                }
                catch (Exception ex)
                {
                    throw;
                }

                TotalCount = OrderTransactionReportModel.Count();
                ViewBag.TotalCount = TotalCount;
                OrderTransactionReportModel = OrderTransactionReportModel.OrderByDescending(x => x.CreateDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = OrderTransactionReportModel.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(OrderTransactionReportModel.OrderBy(x=>x.OrderCode));
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderTransactionReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        public ActionResult Export(string fromDate, string toDate, int option, int print, long? shopID)
        {
            try
            {
                //List<OrderTransactionReportViewModel> OrderReport = new List<OrderTransactionReportViewModel>();
                //OrderReport = this.Getdata(fromDate, toDate, shopID);
                //if (print == 1)
                //{
                //    return View("ForPrint", OrderReport);
                //}



                  /// aDDED BY YASH///
                //DataTable dt = new DataTable();
                //dt.Columns.Add("Sr.No.", typeof(long));
                //dt.Columns.Add("Order Code", typeof(string));
                //dt.Columns.Add("Shop Order Code", typeof(string));
                //dt.Columns.Add("Shop Name", typeof(string));                
                //dt.Columns.Add("Customer", typeof(string));
                //dt.Columns.Add("Is Leader", typeof(string));
                //dt.Columns.Add("Pincode", typeof(string));                
                //dt.Columns.Add("City", typeof(string));
                //dt.Columns.Add("Email", typeof(string));
                //dt.Columns.Add("Primary Mobile", typeof(string));
                //dt.Columns.Add("Product Name", typeof(string));
                //dt.Columns.Add("Category3", typeof(string));
                //dt.Columns.Add("Category2", typeof(string));
                //dt.Columns.Add("Category1", typeof(string));
                //dt.Columns.Add("Qty", typeof(int));
                //dt.Columns.Add("Size", typeof(string));
                //dt.Columns.Add("SaleRate", typeof(decimal));                
                //dt.Columns.Add("Total Amount", typeof(decimal));
                //dt.Columns.Add("Retail Points", typeof(decimal));
                ////dt.Columns.Add("Order Amount", typeof(decimal));                
                //dt.Columns.Add("Order Placed Date", typeof(DateTime));
                //dt.Columns.Add("Order Status", typeof(string));
                //dt.Columns.Add("Payment Mode", typeof(string));
                //dt.Columns.Add("Delivery Date", typeof(DateTime));
                //dt.Columns.Add("Delivery Slot", typeof(string));
                //dt.Columns.Add("Delivery Type", typeof(string));
                //dt.Columns.Add("Shipping Address", typeof(string));
                ////Added by Zubair on 16-12-2017
                //dt.Columns.Add("Address Field Mobile", typeof(string));
                //dt.Columns.Add("JoiningDate", typeof(string));
                //dt.Columns.Add("LastPurchaseDate", typeof(DateTime));
                //dt.Columns.Add("DeviceType", typeof(string));
                ////End

                //decimal totAmount = 0;
                //decimal oldTotAmount = 0;
                //int rowCount = OrderReport.Count;
                //int i = 0;
                //int j = 0;
                //ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                //foreach (var row in OrderReport)
                //{
                //    //i = i + 1;
                //    ////dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                //    ////row.City,row.CreateDate}, false);
                //    //dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopOrderCode, row.ShopName, row.Customer, row.Pincode, row.City,
                //    //    row.RegEmail, row.RegMobile, row.ProductName,row.Category3,row.Category2,row.Category1, row.Qty,row.Size, row.SaleRate, row.PayableAmount, row.CreateDate,row.OrderStatus,
                //    //    row.PaymentMode,row.DeliveryDate, row.DeliveryTime,row.DeliveryType,row.ShippingAddress }, false);

                //    //Added by Zubair on 15-06-2017
                //    // for calculating Delivery charge if Total Amount on single ordercode is less than 350

                //    i = i + 1;
                //    j = j + 1;

                //    if (Session["OrderCode"] != null && Session["OrderCode"].ToString() == row.OrderCode)
                //    {
                //        totAmount += Convert.ToDecimal(row.PayableAmount);
                //        Session["oldOrderCode"] = Session["OrderCode"];
                //    }
                //    else
                //    {
                //        Session["OrderCode"] = row.OrderCode;

                //        oldTotAmount = totAmount;
                //        totAmount = 0;
                //        totAmount += Convert.ToDecimal(row.PayableAmount);
                //    }

                //    if (oldTotAmount < 350 && Session["oldOrderCode"] != null && Session["oldOrderCode"].ToString() != row.OrderCode)
                //    {
                //        dt.LoadDataRow(new object[] {i, Session["oldOrderCode"], "Delivery Charge", "", "", "", "","",
                //        "", "", "","","","",0,"", 0, 25,0, row.CreateDate,"",
                //        "",row.DeliveryDate, row.DeliveryTime,"","","","",row.LastPurchaseDate,"" }, false);

                //        i = i + 1;
                //        dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopOrderCode, row.ShopName, row.Customer, row.IsLeader, row.Pincode, row.City,
                //        row.RegEmail, row.RegMobile, row.ProductName,row.Category3,row.Category2,row.Category1, row.Qty,row.Size, row.SaleRate, row.PayableAmount, row.RetailPoints, row.CreateDate,row.OrderStatus,
                //        row.PaymentMode,row.DeliveryDate, row.DeliveryTime,row.DeliveryType,row.ShippingAddress,row.PrimaryMobile,row.JoiningDate,row.LastPurchaseDate,row.DeviceType }, false);
                //        Session["oldOrderCode"] = Session["OrderCode"];
                //    }
                //    else
                //    {
                //        dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopOrderCode, row.ShopName, row.Customer, row.IsLeader,row.Pincode, row.City,
                //        row.RegEmail, row.RegMobile, row.ProductName,row.Category3,row.Category2,row.Category1, row.Qty,row.Size, row.SaleRate, row.PayableAmount, row.RetailPoints, row.CreateDate,row.OrderStatus,
                //        row.PaymentMode,row.DeliveryDate, row.DeliveryTime,row.DeliveryType,row.ShippingAddress,row.PrimaryMobile,row.JoiningDate,row.LastPurchaseDate,row.DeviceType }, false);
                //        Session["oldOrderCode"] = Session["OrderCode"];
                //    }

                //    if (totAmount < 350 && j == rowCount)
                //    {
                //        i = i + 1;
                //        dt.LoadDataRow(new object[] {i, row.OrderCode, "Delivery Charge", "",  "","",  "",  "",
                //         "",  "", "","","","",0,"", 0, 25,0, row.CreateDate,"",
                //        "",row.DeliveryDate, row.DeliveryTime,"","","","",row.LastPurchaseDate,"" }, false);
                //    }
                //}
               // dt = Common.Helper.ToDataTable(OrderReport);


               // EndBY YASH


                List<OrderTransactionReportViewModel> OrderReport = new List<OrderTransactionReportViewModel>();
                OrderReport = this.Getdata(fromDate, toDate, shopID);
                if (print == 1)
                {
                    return View("ForPrint", OrderReport);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));

                dt.Columns.Add("Order Code", typeof(string));
                dt.Columns.Add("Business Booster Order", typeof(string));
                dt.Columns.Add("Batch  Code", typeof(string));    ///Added by Priti
                dt.Columns.Add("Shop Order Code", typeof(string));
                dt.Columns.Add("Shop Name", typeof(string));
                dt.Columns.Add("Customer", typeof(string));
                dt.Columns.Add("Is Leader", typeof(string));
                dt.Columns.Add("Pincode", typeof(string));
                dt.Columns.Add("City", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Primary Mobile", typeof(string));
                dt.Columns.Add("SKUID", typeof(long));    ///added by Priti
                dt.Columns.Add("Product Name", typeof(string));
                dt.Columns.Add("Category3", typeof(string));
                dt.Columns.Add("Category2", typeof(string));
                dt.Columns.Add("Category1", typeof(string));
                dt.Columns.Add("Qty", typeof(int));
                dt.Columns.Add("Size", typeof(string));
                dt.Columns.Add("MRP", typeof(decimal));
                dt.Columns.Add("GST", typeof(int));
                dt.Columns.Add("SaleRate", typeof(decimal));
                dt.Columns.Add("Total Amount", typeof(decimal));
                dt.Columns.Add("Retail Points", typeof(decimal));
                dt.Columns.Add("Wallet Amount Used", typeof(decimal));//Yashaswi 21-01-2019 showing Complete order wise amount not product wise
                //dt.Columns.Add("Order Amount", typeof(decimal));                
                dt.Columns.Add("Order Placed Date", typeof(DateTime));
                dt.Columns.Add("Order Status", typeof(string));
                dt.Columns.Add("Payment Mode", typeof(string));
                dt.Columns.Add("Delivery Date", typeof(DateTime));
                dt.Columns.Add("Delivery Slot", typeof(string));
                dt.Columns.Add("Delivery Type", typeof(string));
                dt.Columns.Add("Shipping Address", typeof(string));
                //Added by Zubair on 16-12-2017
                dt.Columns.Add("Address Field Mobile", typeof(string));
                dt.Columns.Add("JoiningDate", typeof(string));
                dt.Columns.Add("LastPurchaseDate", typeof(DateTime));
                dt.Columns.Add("DeviceType", typeof(string));
                //End

                decimal totAmount = 0;
                decimal oldTotAmount = 0;
                int rowCount = OrderReport.Count;
                int i = 0;
                int j = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (var row in OrderReport)
                {
                    //i = i + 1;
                    ////dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopName, row.OrderAmount, row.Customer,row.RegEmail,row.RegMobile,row.Pincode,row.ShippingAddress,
                    ////row.City,row.CreateDate}, false);
                    //dt.LoadDataRow(new object[] { i, row.OrderCode, row.ShopOrderCode, row.ShopName, row.Customer, row.Pincode, row.City,
                    //    row.RegEmail, row.RegMobile, row.ProductName,row.Category3,row.Category2,row.Category1, row.Qty,row.Size, row.SaleRate, row.PayableAmount, row.CreateDate,row.OrderStatus,
                    //    row.PaymentMode,row.DeliveryDate, row.DeliveryTime,row.DeliveryType,row.ShippingAddress }, false);

                    //Added by Zubair on 15-06-2017
                    // for calculating Delivery charge if Total Amount on single ordercode is less than 350

                    i = i + 1;
                    j = j + 1;

                    if (Session["OrderCode"] != null && Session["OrderCode"].ToString() == row.OrderCode)
                    {
                        totAmount += Convert.ToDecimal(row.PayableAmount);
                        Session["oldOrderCode"] = Session["OrderCode"];
                    }
                    else
                    {
                        Session["OrderCode"] = row.OrderCode;

                        oldTotAmount = totAmount;
                        totAmount = 0;
                        totAmount += Convert.ToDecimal(row.PayableAmount);
                    }

                    if (oldTotAmount < 350 && Session["oldOrderCode"] != null && Session["oldOrderCode"].ToString() != row.OrderCode)
                    {
                        dt.LoadDataRow(new object[] {i, Session["oldOrderCode"],row.IsBusinessBoosterPlanOrder, row.BatchCode, "Delivery Charge", "", "","", "", "",
                        "", "",0, "","","","",0,"", 0,0,0, 25,0,0, row.CreateDate,"",
                        "",row.DeliveryDate, row.DeliveryTime,"","","","",row.LastPurchaseDate,"" }, false);

                        i = i + 1;
                        dt.LoadDataRow(new object[] { i,row.OrderCode,row.IsBusinessBoosterPlanOrder, row.BatchCode,row.ShopOrderCode, row.ShopName, row.Customer, row.IsLeader, row.Pincode, row.City,
                        row.RegEmail, row.RegMobile,row.SKUID , row.ProductName,row.Category3,row.Category2,row.Category1, row.Qty,row.Size,row.MRP, row.GST,row.SaleRate, row.PayableAmount, row.RetailPoints,row.WalleteAmountUsed, row.CreateDate,row.OrderStatus,
                        row.PaymentMode,row.DeliveryDate, row.DeliveryTime,row.DeliveryType,row.ShippingAddress,row.PrimaryMobile,row.JoiningDate,row.LastPurchaseDate,row.DeviceType }, false);
                        Session["oldOrderCode"] = Session["OrderCode"];
                    }
                    else
                    {
                        dt.LoadDataRow(new object[] { i, row.OrderCode,row.IsBusinessBoosterPlanOrder,row.BatchCode, row.ShopOrderCode, row.ShopName, row.Customer, row.IsLeader,row.Pincode, row.City,
                        row.RegEmail, row.RegMobile,row.SKUID, row.ProductName,row.Category3,row.Category2,row.Category1, row.Qty,row.Size,row.MRP, row.GST, row.SaleRate, row.PayableAmount, row.RetailPoints,row.WalleteAmountUsed, row.CreateDate,row.OrderStatus,
                        row.PaymentMode,row.DeliveryDate, row.DeliveryTime,row.DeliveryType,row.ShippingAddress,row.PrimaryMobile,row.JoiningDate,row.LastPurchaseDate,row.DeviceType }, false);
                        Session["oldOrderCode"] = Session["OrderCode"];
                    }

                    if (totAmount < 350 && j == rowCount)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] {i, row.OrderCode,row.IsBusinessBoosterPlanOrder,row.BatchCode, "Delivery Charge", "",  "","",  "",  "",
                         "",  "",row.SKUID, "","","","",0,"", 0,0, 0,25,0,0, row.CreateDate,"",
                        "",row.DeliveryDate, row.DeliveryTime,"","","","",row.LastPurchaseDate,"" }, false);
                    }
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
                SqlCommand sqlComm = new SqlCommand("ReportOrderTansaction", con);
                sqlComm.Parameters.AddWithValue("@FranchiseID", SqlDbType.Int).Value = FranchiseID;
                sqlComm.Parameters.AddWithValue("@Fromdate", SqlDbType.DateTime).Value = frmd;
                sqlComm.Parameters.AddWithValue("@Todate", SqlDbType.DateTime).Value = tod;
                sqlComm.CommandType = CommandType.StoredProcedure;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                OrderTransactionReportModel = BusinessLogicLayer.Helper.CreateListFromTable<OrderTransactionReportViewModel>(dt);

                OrderTransactionReportModel = OrderTransactionReportModel.Where(x => x.CreateDate >= frmd &&
                                           x.CreateDate <= tod && x.ShopID == (shopID == 0 ? x.ShopID : shopID) && x.ShopFranchiseID == FranchiseID).ToList();

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
