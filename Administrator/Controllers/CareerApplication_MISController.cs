using Administrator.Models;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Administrator.Controllers
{
    /// <summary>
    /// Developed By : Pradnyakar Badge
    /// Purpose : To List all the Application and Resume within date parameter
    /// also provide facility to download CV from this report
    /// </summary>
    public class CareerApplication_MISController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 200;
        //
        // GET: /ShopOrderStatistic_MIS/
        [SessionExpire]
        [CustomAuthorize(Roles = "CareerApplication_MIS/CanRead")]
        public ActionResult Index()
        {
            try
            {
                ViewBag.PostAppliedID = new SelectList(db.Careers.Where(x => x.IsActive == true).ToList(), "ID", "Jobtitle");
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
        [CustomAuthorize(Roles = "CareerApplication_MIS/CanRead")]
        public ActionResult GetReport(int? PostAppliedID, string fromDate, string toDate)
        {
            List<CareerAppicationPostViewModel> ls = new List<CareerAppicationPostViewModel>();
            List<object> paramValues = new List<object>();
            DataTable dt = new DataTable();
            BusinessLogicLayer.CareerDetails obj = new BusinessLogicLayer.CareerDetails();

            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            paramValues.Add(PostAppliedID);
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

            ls = obj.getApplicationList(paramValues, System.Web.HttpContext.Current.Server);

            return PartialView("_ApplicationGetReport", ls);
            //return Json(ls, JsonRequestBehavior.AllowGet);
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "CareerApplication_MIS/CanExport")]
        public ActionResult Export(int? PostAppliedID, string fromDate, string toDate, int option)
        {
            try
            {
                ViewBag.PostAppliedID = new SelectList(db.Careers.Where(x => x.IsActive == true).ToList(), "ID", "Jobtitle");
                List<CareerAppicationPostViewModel> ls = new List<CareerAppicationPostViewModel>();
                List<object> paramValues = new List<object>();
                DataTable dtSet = new DataTable();
                BusinessLogicLayer.CareerDetails obj = new BusinessLogicLayer.CareerDetails();

                DateTime fDate = new DateTime();
                DateTime tDate = new DateTime();

                paramValues.Add(PostAppliedID);
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
                ls = obj.getApplicationList(paramValues, System.Web.HttpContext.Current.Server);

                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("ApplicationID", typeof(long));
                dt.Columns.Add("CareerID", typeof(int));
                dt.Columns.Add("Jobtitle", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Mobile", typeof(string));
                dt.Columns.Add("TotalExpience", typeof(string));
                dt.Columns.Add("CurrentCTC", typeof(string));
                dt.Columns.Add("ExpectedCTC", typeof(string));
                dt.Columns.Add("ResumePath", typeof(string));
                dt.Columns.Add("Remarks", typeof(string));

                int i = 0;

                foreach (CareerAppicationPostViewModel row in ls.ToList())
                {
                    i = i + 1;
                    dt.LoadDataRow(new object[] { i, row.ID, 
                        row.CareerID, 
                        row.Jobtitle, 
                        row.Name,
                        row.Email,
                        row.Mobile,
                        row.TotalExpience,
                        row.CurrentCTC,
                        row.ExpectedCTC,
                        row.ResumePath,
                        row.Remarks

                    }, false);
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Career Applied Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Career Applied Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Career Applied Report");
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