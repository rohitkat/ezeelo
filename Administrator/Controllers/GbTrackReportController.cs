using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using PagedList;
using PagedList.Mvc;
using BusinessLogicLayer;
namespace Administrator.Controllers
{
    public class GbTrackReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 300;
        // GET: /GbTrackReport/
        public ActionResult Index(int? page, string StartDate, string EndDate, string UserName)
        {
            List<GBTrackReportViewModel> listGbTrackReportViewModel = new List<GBTrackReportViewModel>();
            int pageNumber = (page ?? 1);
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
                
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
                ViewBag.UserName = UserName;

                var GbTrack = (from gb in db.GBTracks
                               join Pd in db.PersonalDetails on gb.UserLoginId equals Pd.UserLoginID into PD_temp
                               from pdd in PD_temp.DefaultIfEmpty()
                               where gb.InTime >= frmd && gb.InTime <= tod
                               select new
                               {
                                   PageURL = gb.PageURL,
                                   Name = pdd.FirstName + " " + pdd.MiddleName + " " + pdd.LastName,
                                   InTime = gb.InTime,
                                   OutTime = gb.OutTime,
                                   NetworkIP = gb.NetworkIP,
                                   DeviceType = gb.DeviceType,

                               }).OrderByDescending(x=>x.InTime).ToList();

                if (UserName != null && UserName != "")
                {
                    GbTrack = GbTrack.Where(x => x.Name != null && x.Name.ToLower().Trim().Contains(UserName.ToLower().Trim())).OrderByDescending(x=>x.InTime).ToList();
                }        

                ViewBag.TotalCount = GbTrack.Count();
                foreach (var item in GbTrack)
                {
                    GBTrackReportViewModel track = new GBTrackReportViewModel();
                    track.PageURL = item.PageURL;
                    track.Name = item.Name;
                    track.InTime = item.InTime;
                    track.OutTime = item.OutTime;
                    track.NetworkIP = item.NetworkIP;
                    track.DeviceType = item.DeviceType;
                    listGbTrackReportViewModel.Add(track);
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading GbTrack Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[GbTrackReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading GbTrack Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[GbTrackReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View(listGbTrackReportViewModel.OrderBy(x=>x.ID).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult ExportPrint(int ExportOption, string StartDate, string EndDate, string UserName)
        {
            List<GBTrackReportViewModel> listGbTrackReportViewModel = new List<GBTrackReportViewModel>();
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
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
                ViewBag.UserName = UserName;

                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("PageUrl", typeof(string));
                tblProduct.Columns.Add("User Name", typeof(string));
                tblProduct.Columns.Add("In Date/Time", typeof(DateTime));
                tblProduct.Columns.Add("Out Date/Time", typeof(DateTime));
                tblProduct.Columns.Add("Network IP", typeof(string));
              //  tblProduct.Columns.Add("Device Type", typeof(string));

                var GbTrack = (from gb in db.GBTracks
                               join Pd in db.PersonalDetails on gb.UserLoginId equals Pd.UserLoginID into PD_temp
                               from pdd in PD_temp.DefaultIfEmpty()
                               where gb.InTime >= frmd && gb.InTime <= tod
                               select new
                               {
                                   PageURL = gb.PageURL,
                                   Name = pdd.FirstName + " " + pdd.MiddleName + " " + pdd.LastName,
                                   InTime = gb.InTime,
                                   OutTime = gb.OutTime,
                                   NetworkIP = gb.NetworkIP,
                               //    DeviceType = gb.DeviceType,

                               }).OrderByDescending(x => x.InTime).ToList();

                if (UserName != null && UserName != "")
                {
                    GbTrack = GbTrack.Where(x => x.Name != null && x.Name.ToLower().Trim().Contains(UserName.ToLower().Trim())).OrderByDescending(x => x.InTime).ToList();
                }        
             
                int i = 0;
                foreach (var row in GbTrack)
                {
                    i = i + 1;
                    //tblProduct.LoadDataRow(new object[] {i, row.PageURL, row.Name, row.InTime, row.OutTime, row.NetworkIP, row.DeviceType}, false);
                    tblProduct.LoadDataRow(new object[] { i, row.PageURL, row.Name, row.InTime, row.OutTime, row.NetworkIP }, false);
                  
                   
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (ExportOption == 1)
                {
                    ExportExcelCsv.ExportToExcel(tblProduct, "GB Track Report");
                }
                else if (ExportOption == 2)
                {
                    ExportExcelCsv.ExportToCSV(tblProduct, "GB Track Report");
                }
                else if (ExportOption == 3)
                {
                    ExportExcelCsv.ExportToPDF(tblProduct, "GB Track Report");
                }
              
              
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading GbTrack Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[GbTrackReportController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading GbTrack Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[GbTrackReportController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

    }
}
