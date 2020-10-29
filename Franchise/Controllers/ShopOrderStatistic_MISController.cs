using BusinessLogicLayer;
using Franchise.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Franchise.Controllers
{
    public class ShopOrderStatistic_MISController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 200;
        //
        // GET: /ShopOrderStatistic_MIS/
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopOrderStatistic_MIS/CanRead")]
        public ActionResult Index()
        {
            try
            {             
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
        [SessionExpire]
        [CustomAuthorize(Roles = "ShopOrderStatistic_MIS/CanRead")]
        public ActionResult GetReport(string fromDate, string toDate)
        {
            int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            List<ShopOrderStatistics> ls = new List<ShopOrderStatistics>();
            List<object> paramValues = new List<object>();
            DataTable dt = new DataTable();
            BusinessLogicLayer.ReportManagement obj = new BusinessLogicLayer.ReportManagement();

            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            paramValues.Add(FranchiseID);
            if (!String.IsNullOrEmpty(fromDate))
            {
                string from = fromDate.ToString();
                string[] f = from.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                fDate = Convert.ToDateTime(frmd.ToShortDateString());
                paramValues.Add(fDate);
            }
            else
            {
                paramValues.Add(DBNull.Value);
            }

            if (!String.IsNullOrEmpty(toDate))
            {
                string to = toDate.ToString();
                string[] f = to.Split('/');
                string[] ftime = f[2].Split(' ');
                DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                tDate = Convert.ToDateTime(frmd.ToShortDateString());
                paramValues.Add(tDate);
            }
            else
            {
                paramValues.Add(DBNull.Value);
            }

            dt = obj.ShopOrderStatistic_MIS(paramValues, System.Web.HttpContext.Current.Server);

            ls = (from n in dt.AsEnumerable()
                  select new ShopOrderStatistics
                  {
                      ShopID = n.Field<Int64>("ShopID"),
                      ShopName = n.Field<string>("ShopName"),
                      FromDate = n.Field<string>("FromDate"),
                      ToDate = n.Field<string>("ToDate"),
                      CancelledOrdeCount = n.Field<Int32>("CancelledOrdeCount"),
                      DeliveredOrder = n.Field<Int32>("DeliveredOrder"),
                      OrderCount = n.Field<Int32>("OrderCount")

                  }).ToList();


            return PartialView("_GetReport", ls);
            //return Json(ls, JsonRequestBehavior.AllowGet);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "ShopOrderStatistic_MIS/CanExport")]
        public ActionResult Export(string fromDate, string toDate, int option)
        {
            try
            {

                int FranchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                List<ShopOrderStatistics> ls = new List<ShopOrderStatistics>();
                List<object> paramValues = new List<object>();
                DataTable dtSet = new DataTable();
                BusinessLogicLayer.ReportManagement obj = new BusinessLogicLayer.ReportManagement();

                DateTime fDate = new DateTime();
                DateTime tDate = new DateTime();

                paramValues.Add(FranchiseID);
                if (!String.IsNullOrEmpty(fromDate))
                {
                    string from = fromDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    fDate = Convert.ToDateTime(frmd.ToShortDateString());
                    paramValues.Add(fDate);
                }
                else
                {
                    paramValues.Add(DBNull.Value);
                }

                if (!String.IsNullOrEmpty(toDate))
                {
                    string to = toDate.ToString();
                    string[] f = to.Split('/');
                    string[] ftime = f[2].Split(' ');
                    DateTime frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    tDate = Convert.ToDateTime(frmd.ToShortDateString());
                }
                else
                {
                    paramValues.Add(DBNull.Value);
                }
                dtSet = obj.ShopOrderStatistic_MIS(paramValues, System.Web.HttpContext.Current.Server);
                //if (print == 1)
                //{
                //    return View("ForPrint", OrderReport);
                //}
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("ShopID", typeof(string));
                dt.Columns.Add("ShopName", typeof(string));
                dt.Columns.Add("OrderCount", typeof(Int64));
                dt.Columns.Add("TotalAmount", typeof(decimal));
                dt.Columns.Add("FromDate", typeof(string));
                dt.Columns.Add("ToDate", typeof(string));
                dt.Columns.Add("DeliveredOrder", typeof(Int64));
                dt.Columns.Add("DeliveredAmount", typeof(decimal));
                dt.Columns.Add("CancelledOrdeCount", typeof(Int64));               
                dt.Columns.Add("CancelledAmount", typeof(decimal));

                int i = 0;
                ViewBag.stateID = new SelectList(db.States, "ID", "Name");
                foreach (DataRow row in dtSet.Rows)
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] { i, row["ShopID"], 
                        row["ShopName"], 
                        row["OrderCount"], 
                       row["FromDate"],
                       row["ToDate"],
                        row["CancelledOrdeCount"],
                       row["DeliveredOrder"],
                        row["TotalAmount"],
                        row["DeliveredAmount"],
                       row["CancelledAmount"]}, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Shop Order Statistics Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Shop Order Statistics Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Shop Order Statistics Report");
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
	}
}