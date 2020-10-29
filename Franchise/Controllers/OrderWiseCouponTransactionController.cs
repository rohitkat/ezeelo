using BusinessLogicLayer;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Globalization;
using System.Data;

namespace Franchise.Controllers
{
    public class OrderWiseCouponTransactionController : Controller
    {
        EzeeloDBContext db = new EzeeloDBContext();
        private int PageSize = 300;
        private static string fConnectionString = System.Configuration.ConfigurationSettings.AppSettings["DB_CON"].ToString();
        //
        // GET: /OrderWiseCouponTransaction/
        protected void BindSchemeCodes()
        {
            ViewBag.SchemeID = new SelectList(db.SchemeTypes.Where(x => x.ID != 1 && x.IsActive == true), "ID", "Name");
            //ViewBag.CouponCode = new SelectList(db.CoupenLists.Where(x => x.ID != 1 && x.IsActive == true), "CoupenCode", "CoupenCode");
        }
        public JsonResult BindSchemeCodes(int CouponScheme)
        {
            long lFranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            List<CoupenList> lCoupenLists = new List<CoupenList>();
            List<SelectCouponListFromScheme> lCoupenCodes = new List<SelectCouponListFromScheme>();
            lCoupenLists = db.CoupenLists.Where(x => x.SchemeTypeID == CouponScheme && x.FranchiseID == lFranchiseID).ToList();
            foreach (var c in lCoupenLists)
            {
                SelectCouponListFromScheme selectCouponListFromScheme = new SelectCouponListFromScheme();
                selectCouponListFromScheme.ID = c.ID;
                selectCouponListFromScheme.CoupenCode = c.CoupenCode;
                lCoupenCodes.Add(selectCouponListFromScheme);
            }


            //  SelectList CodeList = new SelectList(db.CoupenLists.Where(x => x.SchemeTypeID == CouponScheme && x.IsActive == true), "ID", "CoupenCode");
            // var CouponCodeList = db.CoupenLists.Where(x => x.SchemeTypeID == CouponScheme);
            return Json(lCoupenCodes.Distinct().OrderBy(x => x.ID).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            BindSchemeCodes();
            return View();
        }
        [HttpPost]
        public ActionResult GetReport(int page, int pagecount, DateTime FromDate, DateTime ToDate, string CouponCode, int? CouponScheme, int print)
        {

            string lFromDate = FromDate.ToString("yyyy/dd/MM");
            string lToDate = ToDate.ToString("yyyy/dd/MM");
            string lCouponCode = "";
            if (CouponCode == "Select Coupon Code")
            {
                CouponCode = lCouponCode;
            }
            try
            {
                int lMode = 0;
                long lFranchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                ViewBag.fromDate = lFromDate;
                ViewBag.toDate = lToDate;
                ViewBag.CouponCode = CouponCode;
                ViewBag.CouponScheme = CouponScheme;
                ViewBag.franchiseID = lFranchiseID;

                int TotalCount = 0;
                int TotalPages = 0;
                int pageNumber = page;
                OrderWiseCouponTransactionReport orderWiseCouponTransactionReport = new OrderWiseCouponTransactionReport(fConnectionString);
                if (FromDate != null && ToDate != null)
                {
                    if (CouponCode == "" && CouponScheme != null)
                    {
                        lMode = 2;
                    }
                    else if (CouponCode != "" && CouponScheme != null)
                    {
                        lMode = 3;
                    }
                    else
                    {
                        lMode = 1;
                    }
                }

                List<OrderWiseCouponTransactionViewModel> OrderWiseCouponTransactionViewModels = new List<OrderWiseCouponTransactionViewModel>();
                OrderWiseCouponTransactionViewModels = orderWiseCouponTransactionReport.GetOrderWiseCouponTransaction(lFromDate, lToDate, CouponCode, CouponScheme, lMode, lFranchiseID);
                BindSchemeCodes();
                if (print == 1)
                {
                    return View("ForPrint", OrderWiseCouponTransactionViewModels);
                }
                TotalCount = OrderWiseCouponTransactionViewModels.Count();
                ViewBag.TotalCount = TotalCount;
                OrderWiseCouponTransactionViewModels = OrderWiseCouponTransactionViewModels.OrderByDescending(x => x.OrderDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = OrderWiseCouponTransactionViewModels.Count;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                return View(OrderWiseCouponTransactionViewModels);
                //return View("Index", OrderWiseCouponTransactionViewModels);
            }
            catch
            {
                throw;
            }


        }

        public ActionResult Export(DateTime fromDate, DateTime toDate, string CouponCode,int? CouponScheme, int option, int print)
        {
            int lMode = 0;
            try
            {
                string lFromDate = fromDate.ToString("yyyy/dd/MM");
                string lToDate = toDate.ToString("yyyy/dd/MM");
                string lCouponCode = "";
                if (CouponCode == "Select Coupon Code")
                {
                    CouponCode = lCouponCode;
                }
                List<OrderWiseCouponTransactionViewModel> ReportData = new List<OrderWiseCouponTransactionViewModel>();
                long lFranchiseID = Convert.ToInt16(Session["FRANCHISE_ID"]);
                if (fromDate != null && toDate != null)
                {
                    if (CouponCode == "" && CouponScheme != null)
                    {
                        lMode = 2;
                    }
                    else if (CouponCode != "" && CouponScheme != null)
                    {
                        lMode = 3;
                    }
                    else
                    {
                        lMode = 1;
                    }
                } 
                BindSchemeCodes();
                ReportData = this.Getdata(lFromDate, lToDate, CouponCode, CouponScheme, lMode, lFranchiseID);
                if (print == 1)
                {
                    return View("ForPrint", ReportData);
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("Sr.No.", typeof(long));
                dt.Columns.Add("Coupen Code", typeof(string));
                dt.Columns.Add("Coupon Scheme", typeof(string));
                dt.Columns.Add("Coupen Amount", typeof(decimal));
                dt.Columns.Add("Order Code", typeof(string));
                dt.Columns.Add("Customer", typeof(string));
                dt.Columns.Add("Mobile", typeof(string));
                dt.Columns.Add("Order Date", typeof(DateTime));
                dt.Columns.Add("Order Amount", typeof(decimal));
                dt.Columns.Add("Payable Amount", typeof(decimal));
                int i = 0;
                foreach (var row in ReportData)
                {
                    i = i + 1;
                   
                        dt.LoadDataRow(new object[] {i, row.CouponCode, row.CouponScheme,row.CouponAmount, row.OrderCode, row.CustomerName, row.PrimaryMobile, row.OrderDate,
                       row.OrderAmount,row.PayableAmount}, false);
                    
                }
                ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                if (option == 1)
                {
                    ExportExcelCsv.ExportToExcel(dt, "Order Wise Coupon Transaction Report");
                }
                else if (option == 2)
                {
                    ExportExcelCsv.ExportToCSV(dt, "Order Wise Coupon Transaction Report");
                }
                else if (option == 3)
                {
                    ExportExcelCsv.ExportToPDF(dt, "Order Wise Coupon Transaction Report");
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting OrderWise Coupon Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OrderWiseCouponTransactionController][POST:GetReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading OrderWise Coupon Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OrderWiseCouponTransactionController][POST:GetReport]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View("Index");

        }
        //fromDate, toDate, CouponCode,CouponScheme 
        public List<OrderWiseCouponTransactionViewModel> Getdata(string fromDate, string toDate, string CouponCode, int? CouponScheme,int Mode,long FranchiseID)
        {
            OrderWiseCouponTransactionReport orderWiseCouponTransactionReport = new OrderWiseCouponTransactionReport(fConnectionString);
           // int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            BindSchemeCodes();                
            List<OrderWiseCouponTransactionViewModel> OrderWiseCouponTransactionViewModels = new List<OrderWiseCouponTransactionViewModel>();
            OrderWiseCouponTransactionViewModels = orderWiseCouponTransactionReport.GetOrderWiseCouponTransaction(fromDate, toDate, CouponCode, CouponScheme, Mode, FranchiseID);
            return OrderWiseCouponTransactionViewModels;
        }
    }
}
