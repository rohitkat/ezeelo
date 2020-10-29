using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using Administrator.Models;
using PagedList;
using PagedList.Mvc;
using BusinessLogicLayer;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Data;

namespace Administrator.Controllers
{
    public class TrackSearchReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;

        [SessionExpire]
        [Authorize(Roles = "TrackSearchReport/CanRead")]
        public ActionResult Index(int? page, string StartDate, string EndDate)
        {
            try
            {
                DateTime frmd = new DateTime();
                DateTime tod = new DateTime();
                if (StartDate != "" && StartDate!=null )
                {
                    string from = StartDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    frmd = Convert.ToDateTime(frmd.ToShortDateString());

                }
                if (EndDate != ""&& EndDate != null)
                {
                    string to = EndDate.ToString();
                    string[] t = to.Split('/');
                    string[] ttime = t[2].Split(' ');
                    tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                    tod = Convert.ToDateTime(tod.ToShortDateString());
                    tod = tod.AddDays(1);

                }

                int TotalCount = 0;
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                List<TrackSearchReportViewModel> listTrackSearchReportViewModel = new List<TrackSearchReportViewModel>();
                var TrackSearchReport = (from TS in db.TrackSearches
                                         join PD in db.PersonalDetails on TS.UserLoginID equals PD.UserLoginID into PD_join
                                         from PD in PD_join.DefaultIfEmpty()
                                         join CT in db.Categories on TS.CategoryID equals CT.ID into CT_join
                                         from CT in CT_join.DefaultIfEmpty()
                                         join SP in db.Shops on TS.ShopID equals SP.ID into SP_join
                                         from SP in SP_join.DefaultIfEmpty()
                                         where TS.CreateDate >= frmd && TS.CreateDate <= tod
                                         select new
                                         {
                                             UserName = PD.FirstName + "" + PD.LastName,
                                             CategoryName = CT.Name,
                                             ShopName = SP.Name,
                                             ProductName = TS.ProductName,
                                             Date = TS.CreateDate,
                                             City = TS.City,
                                             //FranchiseID=TS.FranchiseID,////added
                                             Franchises = db.Franchises.Where(x => x.ID == TS.FranchiseID).Select(x => x.ContactPerson).FirstOrDefault().ToString()  ////added
                                         }).ToList();

                foreach (var item in TrackSearchReport)
                {
                    TrackSearchReportViewModel track = new TrackSearchReportViewModel();
                    track.UserName = item.UserName;
                    track.CategoryName = item.CategoryName;
                    track.ShopName = item.ShopName;
                    track.ProductName = item.ProductName;
                    track.Date = item.Date;
                    track.City = item.City;
                   // track.FranchiseID =Convert.ToInt32(item.FranchiseID);////added
                    track.Franchises = item.Franchises.ToString();////added

                    listTrackSearchReportViewModel.Add(track);
                }
                TotalCount = TrackSearchReport.Count();
                ViewBag.TotalCount = TotalCount;

                return View(listTrackSearchReportViewModel.ToList().OrderByDescending(x => x.Date).ToPagedList(pageNumber, pageSize));

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopDetailsReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Shop Detail Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopDetailsReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [Authorize(Roles = "TrackSearchReport/CanRead")]
        public ActionResult Export(string StartDate, string EndDate, int option)
        {
            var TrackSearchReport = (dynamic)null;
            try
            {
                DateTime frmd = new DateTime();
                DateTime tod = new DateTime();
                if (StartDate != "" && StartDate != null)
                {
                    string from = StartDate.ToString();
                    string[] f = from.Split('/');
                    string[] ftime = f[2].Split(' ');
                    frmd = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                    frmd = Convert.ToDateTime(frmd.ToShortDateString());

                }
                if (EndDate != "" && EndDate != null)
                {
                    string to = EndDate.ToString();
                    string[] t = to.Split('/');
                    string[] ttime = t[2].Split(' ');
                    tod = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                    tod = Convert.ToDateTime(tod.ToShortDateString());
                    tod = tod.AddDays(1);

                }

                int TotalCount = 0;
                int pageNumber = 1;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                TrackSearchReport = (from TS in db.TrackSearches
                                     join PD in db.PersonalDetails on TS.UserLoginID equals PD.UserLoginID into PD_join
                                     from PD in PD_join.DefaultIfEmpty()
                                     join CT in db.Categories on TS.CategoryID equals CT.ID into CT_join
                                     from CT in CT_join.DefaultIfEmpty()
                                     join SP in db.Shops on TS.ShopID equals SP.ID into SP_join
                                     from SP in SP_join.DefaultIfEmpty()
                                     where TS.CreateDate >= frmd && TS.CreateDate <= tod
                                     select new
                                     {
                                         UserName = PD.FirstName + "" + PD.LastName,
                                         CategoryName = CT.Name,
                                         ShopName = SP.Name,
                                         ProductName = TS.ProductName,
                                         Date = TS.CreateDate,
                                         City = TS.City,
                                         ////FranchiseID = TS.FranchiseID ////added
                                         Franchises = db.Franchises.Where(x => x.ID == TS.FranchiseID).Select(x => x.ContactPerson).FirstOrDefault().ToString()  ////added

                                     }).ToList();

                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("Customer Name", typeof(string));
                tblProduct.Columns.Add("Shop Name", typeof(string));
                tblProduct.Columns.Add("Product Name", typeof(string));
                tblProduct.Columns.Add("Date", typeof(DateTime));
                tblProduct.Columns.Add("City Name", typeof(string));
                //tblProduct.Columns.Add("FranchiseID", typeof(string));////added
                tblProduct.Columns.Add("Franchises", typeof(string));////added
                int i = 0;
                foreach (var row in TrackSearchReport)
                {
                    i = i + 1;
                    tblProduct.LoadDataRow(new object[] { i, row.UserName, row.ShopName, row.ProductName, row.Date, row.City, row.Franchises }, false);////added row.FranchiseID
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(tblProduct, "Track Cart Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(tblProduct, "Track Cart Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(tblProduct, "Track Cart Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Track Cart Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[TrackCartReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Track Cart Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[TrackCartReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View("Index", TrackSearchReport);

        }
    }
}
