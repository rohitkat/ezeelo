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
using System.Data.SqlClient;
using BusinessLogicLayer;
using PagedList;
using PagedList.Mvc;
using Franchise.Models;

namespace Franchise.Controllers
{
    public class ReferAndEarnTransactionReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;

        // GET: /EarnAndReferDetailReport/

        //********EarnAndReferDetailReport Code Added by harshada on 23/12/2016**********//
        //***********Fetching CityID of franchise**********//
        public long GetCityId(long FranchiseID)
        {
            
            long lCityID = (from CT in db.Cities
                           join PC in db.Pincodes on CT.ID equals PC.CityID
                           join FR in db.Franchises on PC.ID equals FR.PincodeID
                           where FR.ID == FranchiseID
                           select CT.ID).FirstOrDefault();



            return lCityID;

           
        }
        //***********End Fetching CityID of franchise**********//
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult GetReport(int? page, int option, string SearchUserName, string SearchEmail, string SearchMobile, string SearchSchemeName, int Mode, string FromDate, string ToDate, int print)   //---parameters Added by harshada i.e. Mode,FromDate, ToDate,  FranchiseID,  CityID,print
        {
            List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
            try
            {
                DateTime fromdt = CommonFunctions.GetProperDateTime(FromDate);
                string lFromDate = fromdt.ToShortDateString();
                DateTime dt = Convert.ToDateTime(lFromDate);
                string FromDate1 = dt.ToString("dd-MMM-yyyy");

                DateTime todt = CommonFunctions.GetProperDateTime(ToDate);
                string lToDate = todt.ToShortDateString();
                DateTime dt1 = Convert.ToDateTime(lToDate);
                string ToDate1 = dt1.ToString("dd-MMM-yyyy");
 
                int TotalCount = 0;
                int pageNumber = (page ?? 1);
                ViewBag.pagenumber = pageNumber;
                ViewBag.pagesize = pageSize;
                ViewBag.searchusername = SearchUserName;
                ViewBag.searchmobile = SearchMobile;
                ViewBag.searchemail = SearchEmail;
                ViewBag.searchschemename = SearchSchemeName;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                long lFranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                long lCityID = GetCityId(lFranchiseID);
                lEarnAndReferDetailReportViewModel = Getdata(SearchUserName, SearchEmail, SearchMobile, SearchSchemeName, Mode, FromDate1, lFranchiseID, lCityID, ToDate1);
                TotalCount = lEarnAndReferDetailReportViewModel.Count();
                int i = 0;
                ViewBag.TotalCount = TotalCount; 
                if (print == 1)
                {
                    return View("ForPrint", lEarnAndReferDetailReportViewModel.ToList());
                }
              
                return View(lEarnAndReferDetailReportViewModel.ToList());

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the ReferAndEarnDtailReport!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[EarnAndReferDetailReport][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the RReferAndEarnDtailReport!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[EarnAndReferDetailReport][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
            }

            return View();
        }
        public ActionResult Export(int? page, int option, string SearchUserName, string SearchEmail, string SearchMobile, string SearchSchemeName, int Mode, string FromDate, string ToDate)
        {
            List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
            DateTime fromdt = CommonFunctions.GetProperDateTime(FromDate);
            string lFromDate = fromdt.ToShortDateString();
            DateTime dt = Convert.ToDateTime(lFromDate);
            string FromDate1 = dt.ToString("dd-MMM-yyyy");

            DateTime todt = CommonFunctions.GetProperDateTime(ToDate);
            string lToDate = todt.ToShortDateString();
            DateTime dt1 = Convert.ToDateTime(lToDate);
            string ToDate1 = dt1.ToString("dd-MMM-yyyy");
            
            int TotalCount = 0;
            int pageNumber = (page ?? 1);
            ViewBag.pagenumber = pageNumber;
            ViewBag.pagesize = pageSize;
            long lFranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            long lCityID = GetCityId(lFranchiseID);
            lEarnAndReferDetailReportViewModel = Getdata(SearchUserName, SearchEmail, SearchMobile, SearchSchemeName, Mode, FromDate1, lFranchiseID, lCityID, ToDate1);
            TotalCount = lEarnAndReferDetailReportViewModel.Count();
            int i = 0;
            ViewBag.TotalCount = TotalCount;
            DataTable tblProduct = new DataTable();
            tblProduct.Columns.Add("Sr.No.", typeof(long));
            tblProduct.Columns.Add("ReferrerName", typeof(string));
            tblProduct.Columns.Add("ReferredDate", typeof(string));
            tblProduct.Columns.Add("EmailReferee", typeof(string));
            tblProduct.Columns.Add("MobileReferee", typeof(string));
            tblProduct.Columns.Add("RefereeStatus", typeof(string));
            tblProduct.Columns.Add("EarnAmount", typeof(decimal));
            tblProduct.Columns.Add("OrderAmount", typeof(decimal));
            tblProduct.Columns.Add("OrderCode", typeof(string));
            tblProduct.Columns.Add("FranchiseName", typeof(string));
            tblProduct.Columns.Add("SchemeName", typeof(string));
            foreach (var row in lEarnAndReferDetailReportViewModel)
            {
                i = i + 1;
                tblProduct.LoadDataRow(new object[] {i, row.UserName, row.ReferredDate,row.Email,row.Mobile, row.RefereeStatus, row.EarnAmount, row.OrderAmount, row.OrderCode,
                 row.FranchiseName,row.SchemeName}, false);
            }
            ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
            if (option == 1)
            {
                ExportExcelCsv.ExportToExcel(tblProduct, "Refer and Earn Detail Report");
            }
            else if (option == 2)
            {
                ExportExcelCsv.ExportToCSV(tblProduct, "Refer and Earn Detail Report");
            }
            else if (option == 3)
            {
                ExportExcelCsv.ExportToPDF(tblProduct, "Refer and Earn Detail Report");
            }

            return View("Index");
        }

        public List<EarnAndReferDetailReportViewModel> Getdata(string SearchUserName, string SearchEmail, string SearchMobile, string SearchSchemeName, int Mode, string FromDate, long? FranchiseID, long? CityID, string ToDate)
        {
            List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
            DataTable dt = new DataTable();
            ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
            DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                List<object> paramValues = new List<object>();
            paramValues.Add(CityID);
            paramValues.Add(FromDate);
            paramValues.Add(ToDate);
            paramValues.Add(FranchiseID);
            paramValues.Add(Mode);

            dt = dbOpr.GetRecords("ReportReferAndEarnTransaction", paramValues);

                lEarnAndReferDetailReportViewModel = (from n in dt.AsEnumerable()
                                                      select new EarnAndReferDetailReportViewModel
                                                     {
                                                      UserName = n.Field<string>("FirstName"),
                                                         Email = n.Field<string>("Email"),
                                                         Mobile = n.Field<string>("Mobile"),
                                                         ReferenceID = n.Field<long?>("ReferenceID"),
                                                         EarnAmount = n.Field<decimal?>("EarnAmount"),
                                                      OrderAmount = n.Field<decimal?>("TotalOrderAmount"),
                                                      SchemeName = n.Field<string>("Name"),
                                                      OrderCode = n.Field<string>("OrderCode"),
                                                      ReferredDate = n.Field<DateTime>("CreateDate"),
                                                      FranchiseName = n.Field<string>("FranchiseName"),
                                                      RegDate = n.Field<DateTime?>("RegDate")

                                                     }).ToList();

                if (SearchMobile != null && SearchMobile != "")
                {
                    lEarnAndReferDetailReportViewModel = lEarnAndReferDetailReportViewModel.Where(x => x.Mobile != null && x.Mobile.Trim().StartsWith(SearchMobile.Trim())).ToList();
                }
                if (SearchEmail != null && SearchEmail != "")
                {
                    lEarnAndReferDetailReportViewModel = lEarnAndReferDetailReportViewModel.Where(x => x.Email != null && x.Email.ToLower().Trim().Contains(SearchEmail.ToLower().Trim())).ToList();
                }
                if (SearchUserName != null && SearchUserName != "")
                {
                    lEarnAndReferDetailReportViewModel = lEarnAndReferDetailReportViewModel.Where(x => x.UserName != null && x.UserName.ToLower().Trim().Contains(SearchUserName.ToLower().Trim())).ToList();
                }
                if (SearchSchemeName != null && SearchSchemeName != "")
                {
                    lEarnAndReferDetailReportViewModel = lEarnAndReferDetailReportViewModel.Where(x => x.SchemeName != null && x.SchemeName.ToLower().Trim().Contains(SearchSchemeName.ToLower().Trim())).ToList();
                }


                foreach (var row in lEarnAndReferDetailReportViewModel)
                {
                if (Mode == 1)
                {
                    if (row.RegDate != null)
                {
                        row.RefereeStatus = "Registered";
                }
                    else
                {
                        row.RefereeStatus = "Not Registered";
                }
            }
                else if (Mode == 2)
            {
                    row.RefereeStatus = "Registered";
            }
                else if (Mode == 3)
            {
                    row.RefereeStatus = "Not Registered";

                }
            }
            return lEarnAndReferDetailReportViewModel;
        }

    }
}
