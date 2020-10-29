using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using System.Collections.ObjectModel;
using ModelLayer.Models.ViewModel;
using Franchise.Models;
using PagedList;
using PagedList.Mvc;
using BusinessLogicLayer;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
//using RazorPDF;
//using iTextSharp.text.html;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
using System.Web.Hosting;
using System.Data;


namespace Franchise.Controllers
{
    public class TrackCartReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;

        [SessionExpire]
        [CustomAuthorize(Roles = "TrackCartReport/CanRead")]
        public ActionResult Index(int? page, string StartDate, string EndDate, string SearchProName, string SearchMobile, string SearchEmail, string SearchCustName, string SearchStage, string CityList)
        {
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
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
                ViewBag.CityList = new SelectList(db.TrackCarts.Select(x => x.City).Distinct().ToList());
                ViewBag.selectedCity = CityList;
                ViewBag.SearchProName= SearchProName;
                ViewBag.SearchMobile= SearchMobile;
                ViewBag.SearchEmail= SearchEmail;
                ViewBag.SearchCustName= SearchCustName;
                ViewBag.SearchStage = SearchStage;
               
                List<TrackCartReportViewModel> listTrackCartReportViewModel = new List<TrackCartReportViewModel>();
                var TrackCartReport = (from PD in db.PersonalDetails
                                       join UL in db.UserLogins on PD.UserLoginID equals UL.ID
                                       join TC in db.TrackCarts on PD.UserLoginID equals TC.UserLoginID
                                       join ST in db.ShopStocks on TC.ShopStockID equals ST.ID
                                       join PV in db.ProductVarients on ST.ProductVarientID equals PV.ID
                                       join CO in db.Colors on PV.ColorID equals CO.ID
                                       join SZ in db.Sizes on PV.SizeID equals SZ.ID
                                       join SP in db.ShopProducts on ST.ShopProductID equals SP.ID
                                       join P in db.Products on SP.ProductID equals P.ID
                                       where TC.CreateDate >= frmd && TC.CreateDate <= tod
                                       select new
                                       {
                                           // ID = TC.UserLoginID,
                                           Date = TC.CreateDate,
                                           CustomerName = PD.FirstName + "" + PD.LastName,
                                           Address = PD.Address,
                                           Mobile = TC.Mobile,
                                           Email=UL.Email,
                                           Price=ST.MRP,
                                           SaleRate=ST.RetailerRate,
                                           LandingPrice=ST.WholeSaleRate,
                                           ProductName = P.Name,
                                           ProductColor = CO.Name,
                                           ProductSize = SZ.Name,
                                           Stage = TC.Stage,
                                           City = TC.City

                                       }).OrderByDescending(x => x.Date).ToList();


                if (SearchMobile != null && SearchMobile != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.Mobile != null && x.Mobile.Trim().StartsWith(SearchMobile.Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchEmail != null && SearchEmail != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.Email != null && x.Email.ToLower().Trim().Contains(SearchEmail.ToLower().Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchProName != null && SearchProName != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.ProductName != null && x.ProductName.ToLower().Trim().Contains(SearchProName.ToLower().Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchCustName != null && SearchCustName != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.CustomerName != null && x.CustomerName.ToLower().Trim().Contains(SearchCustName.ToLower().Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchStage != null && SearchStage != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.Stage != null && x.Stage.Trim().Contains(SearchStage.Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (CityList != null && CityList != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.City != null && x.City.Trim().Contains(CityList.Trim())).OrderByDescending(x => x.Date).ToList();
                }

                foreach (var item in TrackCartReport)
                {
                    TrackCartReportViewModel track = new TrackCartReportViewModel();
                    track.Date = item.Date;
                    track.CustomerName = item.CustomerName;
                    track.Adress = item.Address;
                    track.Mobile = item.Mobile;
                    track.Email = item.Email;
                    track.Price = item.Price;
                    track.SaleRate = item.SaleRate;
                    track.LandingPrice = (item.LandingPrice==null?0:(decimal)item.LandingPrice);
                    track.ProductName = item.ProductName;
                    track.ProductColor = item.ProductColor;
                    track.ProductSize = item.ProductSize;
                    track.Stage = item.Stage;
                    track.City = item.City;
                    listTrackCartReportViewModel.Add(track);
                }
                TotalCount = TrackCartReport.Count();
                ViewBag.TotalCount = TotalCount;
               

                return View(listTrackCartReportViewModel.ToList().OrderByDescending(x => x.Date).ToPagedList(pageNumber, pageSize));

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
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "TrackCartReport/CanRead")]
        public ActionResult Export(string StartDate, string EndDate, int option, string SearchProName, string SearchMobile, string SearchEmail, string SearchCustName, string SearchStage, string CityList)
        {
           // var TrackCartReport = (dynamic)null;
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
                //int pageNumber = (page ?? 1);
                //ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;
                ViewBag.CityList = new SelectList(db.TrackCarts.Select(x => x.City).Distinct().ToList());
                ViewBag.selectedCity = CityList;
                ViewBag.SearchProName = SearchProName;
                ViewBag.SearchMobile = SearchMobile;
                ViewBag.SearchEmail = SearchEmail;
                ViewBag.SearchCustName = SearchCustName;
                ViewBag.SearchStage = SearchStage;

                List<TrackCartReportViewModel> listTrackCartReportViewModel = new List<TrackCartReportViewModel>();
                var TrackCartReport = (from PD in db.PersonalDetails
                                       join UL in db.UserLogins on PD.UserLoginID equals UL.ID
                                       join TC in db.TrackCarts on PD.UserLoginID equals TC.UserLoginID
                                       join ST in db.ShopStocks on TC.ShopStockID equals ST.ID
                                       join SP in db.ShopProducts on ST.ShopProductID equals SP.ID
                                       join P in db.Products on SP.ProductID equals P.ID
                                       where TC.CreateDate >= frmd && TC.CreateDate <= tod
                                       select new
                                       {
                                           // ID = TC.UserLoginID,
                                           Date = TC.CreateDate,
                                           CustomerName = PD.FirstName + "" + PD.LastName,
                                           Address = PD.Address,
                                           Mobile = TC.Mobile,
                                           Email = UL.Email,
                                           Price = ST.MRP,
                                           SaleRate = ST.RetailerRate,
                                           LandingPrice = ST.WholeSaleRate,
                                           ProductName = P.Name,
                                           Stage = TC.Stage,
                                           City = TC.City

                                       }).OrderByDescending(x => x.Date).ToList();


                if (SearchMobile != null && SearchMobile != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.Mobile != null && x.Mobile.Trim().StartsWith(SearchMobile.Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchEmail != null && SearchEmail != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.Email != null && x.Email.ToLower().Trim().Contains(SearchEmail.ToLower().Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchProName != null && SearchProName != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.ProductName != null && x.ProductName.ToLower().Trim().Contains(SearchProName.ToLower().Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchCustName != null && SearchCustName != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.CustomerName != null && x.CustomerName.ToLower().Trim().Contains(SearchCustName.ToLower().Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (SearchStage != null && SearchStage != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.Stage != null && x.Stage.Trim().Contains(SearchStage.Trim())).OrderByDescending(x => x.Date).ToList();
                }
                if (CityList != null && CityList != "")
                {
                    TrackCartReport = TrackCartReport.Where(x => x.City != null && x.City.Trim().Contains(CityList.Trim())).OrderByDescending(x => x.Date).ToList();
                }

                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("Date", typeof(DateTime));
                tblProduct.Columns.Add("Customer Name", typeof(string));
                tblProduct.Columns.Add("Address", typeof(string));
                tblProduct.Columns.Add("Mobil", typeof(string));
                tblProduct.Columns.Add("Email", typeof(string));
                tblProduct.Columns.Add("Price", typeof(decimal));
                tblProduct.Columns.Add("Sale Rate", typeof(decimal));
                tblProduct.Columns.Add("Landing Price", typeof(decimal));
                tblProduct.Columns.Add("Product Name", typeof(string));
                tblProduct.Columns.Add("Mobile No", typeof(string));
                tblProduct.Columns.Add("Stage", typeof(string));
                tblProduct.Columns.Add("City Name", typeof(string));
                int i = 0;
                foreach (var row in TrackCartReport)
                {
                    i = i + 1;
                    tblProduct.LoadDataRow(new object[] { i, row.Date, row.CustomerName, row.Address, row.Mobile, row.Email, row.Price, row.SaleRate, row.LandingPrice, row.ProductName, row.Mobile, row.Stage, row.City }, false);
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
            return View();

        }

        public ActionResult PDF(string StartDate, string EndDate)
        {
            try
            {

                DateTime lStartDate = DateTime.Now;
                DateTime lEndDate = DateTime.Now;
                if (StartDate != "")
                {

                    lStartDate = CommonFunctions.GetProperDateTime(StartDate);

                }
                if (EndDate != "")
                {

                    lEndDate = CommonFunctions.GetProperDateTime(EndDate);

                }

                int TotalCount = 0;
                ViewBag.PageSize = pageSize;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                List<TrackCartReportViewModel> listTrackCartReportViewModel = new List<TrackCartReportViewModel>();
                var TrackCartReport = (from PD in db.PersonalDetails
                                       join TC in db.TrackCarts on PD.UserLoginID equals TC.UserLoginID
                                       join ST in db.ShopStocks on TC.ShopStockID equals ST.ID
                                       join SP in db.ShopProducts on ST.ShopProductID equals SP.ID
                                       join P in db.Products on SP.ProductID equals P.ID
                                       where TC.CreateDate >= lStartDate && TC.CreateDate <= lEndDate
                                       select new
                                       {
                                           // ID = TC.UserLoginID,
                                           CustomerName = PD.FirstName + "" + PD.LastName,
                                           ProductName = P.Name,
                                           Mobile = TC.Mobile,
                                           Stage = TC.Stage,
                                           Date = TC.CreateDate,
                                           City = TC.City

                                       }).ToList();

                foreach (var item in TrackCartReport)
                {
                    TrackCartReportViewModel track = new TrackCartReportViewModel();
                    track.CustomerName = item.CustomerName;
                    track.ProductName = item.ProductName;
                    track.Mobile = item.Mobile;
                    track.Stage = item.Stage;
                    track.Date = item.Date;
                    track.City = item.City;
                    listTrackCartReportViewModel.Add(track);
                }
                TotalCount = TrackCartReport.Count();
                ViewBag.TotalCount = TotalCount;

                return View(listTrackCartReportViewModel.ToList().OrderBy(x => x.Date));

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
            return View("Index");
        }

    }
}
