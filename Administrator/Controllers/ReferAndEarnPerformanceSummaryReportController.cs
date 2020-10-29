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

namespace Administrator.Controllers
{
    public class ReferAndEarnPerformanceSummaryReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 50;

        // GET: /EarnAndReferDetailReport/
        public ActionResult Index(int? page,int? option, string SearchUserName, string SearchEmail, string SearchMobile, string SearchSchemeName)
        {
            List<EarnAndReferDetailReportViewModel> lEarnAndReferDetailReportViewModel = new List<EarnAndReferDetailReportViewModel>();
            try
            {
                int TotalCount = 0;
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchUserName = SearchUserName;
                ViewBag.SearchMobile = SearchMobile;
                ViewBag.SearchEmail = SearchEmail;
                ViewBag.SearchSchemeName = SearchSchemeName;
              
                //ReadConfig readCon = new ReadConfig(System.Web.HttpContext.Current.Server);
                //SqlConnection con = new SqlConnection(readCon.DB_CONNECTION);
                //SqlDataAdapter sda = new SqlDataAdapter("Select_ReferAndEarnReport", con);
                //DataTable dt = new DataTable();
                //sda.Fill(dt);
                DataTable dt = new DataTable();
                ReadConfig config = new ReadConfig(System.Web.HttpContext.Current.Server);
                DataAccessLayer.DbOperations dbOpr = new DataAccessLayer.GetData(config.DB_CONNECTION);
                //List<object> paramValues = new List<object>();
                //paramValues.Add(0);
                //dt = dbOpr.GetRecords("ReportReferAndEarnSchemeProfitAndLossSummary", paramValues);
                dt = dbOpr.GetRecords("ReportReferAndEarnSchemeProfitAndLossSummary");

                lEarnAndReferDetailReportViewModel = (from n in dt.AsEnumerable()
                                                      select new EarnAndReferDetailReportViewModel
                                                     {
                                                         SchemeName = n.Field<string>("Name"),
                                                         TotalBudgetAmount = n.Field<decimal?>("TotalBudgetAmt"),
                                                         RemainingAmount = n.Field<decimal?>("RemainingAmt"),
                                                         OrderAmount = n.Field<decimal?>("OrderAmount"),

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

                TotalCount = lEarnAndReferDetailReportViewModel.Count();
                ViewBag.TotalCount = TotalCount;

                int i = 0;
                DataTable tblProduct = new DataTable();
                tblProduct.Columns.Add("Sr.No.", typeof(long));
                tblProduct.Columns.Add("SchemeName", typeof(string));
                tblProduct.Columns.Add("TotalBudgetAmount", typeof(decimal));
                tblProduct.Columns.Add("RemainingAmount", typeof(decimal));
                tblProduct.Columns.Add("OrderAmount", typeof(decimal));
                foreach (var row in lEarnAndReferDetailReportViewModel)
                {
                    i = i + 1;
                    tblProduct.LoadDataRow(new object[] { i, row.SchemeName, row.TotalBudgetAmount, row.RemainingAmount, row.OrderAmount }, false);
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

                //if((SearchEmail!=null && SearchEmail!=string.Empty )|| SearchMobile !=string.Empty || SearchUserName!=string.Empty || SearchSchemeName!=string.Empty)
                //{
                //    pageNumber = 1;
                //}

                return View(lEarnAndReferDetailReportViewModel.ToList().ToPagedList(pageNumber, pageSize));

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

       
     
    }
}
