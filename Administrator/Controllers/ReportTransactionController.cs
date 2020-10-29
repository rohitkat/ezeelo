using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer.Account;
using ModelLayer.Models.ViewModel.Report.Account;
using ModelLayer.Models;
using Administrator.Models;

using PagedList;
using PagedList.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;

namespace Administrator.Controllers
{
    public class ReportTransactionController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private int pageSize = 10;

        StringBuilder errStr = new StringBuilder("=====================================================================================" +
        Environment.NewLine
        + "ErrorLog Controller : BankController" + Environment.NewLine);


        private List<ReportTransactionInputViewModel> filterData(List<ReportTransactionInputViewModel> pReportTransactionViewModels)
        {
            //if (customerCareSessionViewModel.EmployeeCode != null && customerCareSessionViewModel.EmployeeCode.Contains("GBCC")) // CRM can view all data.
            //{
            //    return pReportTransactionViewModels;
            //}
            //switch (customerCareSessionViewModel.BusinessTypeId) //- Shop can only view their data.
            //{
            //    case (int)Common.Constant.BusinessType.MERCHANT_SHOP:
            //        pReportTransactionViewModels = pReportTransactionViewModels.Where(x => x.ShopID == customerCareSessionViewModel.ShopID).ToList();
            //        Shop lShop = (from rtvm in pReportTransactionViewModels
            //                      join shp in db.Shops on rtvm.ShopID equals shp.ID
            //                      select new Shop
            //                      {
            //                          ID = shp.ID,
            //                          Name = shp.Name,
            //                          Address = shp.Address,
            //                          ContactPerson = shp.ContactPerson,
            //                          Mobile = shp.Mobile,
            //                          Email = shp.Email,
            //                          VAT = shp.VAT,
            //                          TIN = shp.TIN
            //                      }).ToList().FirstOrDefault();
            //        ViewBag.Shop = lShop;
            //        break;
            //}
            return pReportTransactionViewModels;
        }

        //[SessionExpire]
        //public ActionResult TransactionByMCOGroup()
        //{
        //    SessionDetails();
        //    Report lReport = new Report();
        //    List<ReportTransactionInputViewModel> lReportTransactionViewModels = filterData(lReport.ReportTransactionViewModel());
        //    List<TransactionByMCOGroupViewModel> lTransactionByMCOGroupViewModels = (from rtvm in lReportTransactionViewModels
        //                                                                               group rtvm by new { rtvm.MCOCustomerID, rtvm.MCOShopID, rtvm.MCODeliveryID } into g
        //                                                                               select new TransactionByMCOGroupViewModel
        //                                                                               {
        //                                                                                   QtyAskedByCustomer = 
        //                                                                               }).ToList();

        //    //List<Franchise> lFranchises = (from tbmgvm in lTransactionByMCOGroupViewModels
        //    //                     select new Shop
        //    //                     {
        //    //                         ID = tbmgvm.ShopID,
        //    //                         Name = tbsgvm.ShopName,
        //    //                     }).ToList();

        //    //ViewBag.Shops = new SelectList(lShops, "ID", "Name");
        //    return View(lTransactionByMCOGroupViewModels);
        //}

        [SessionExpire]
        [CustomAuthorize(Roles = "ReportTransaction/CanRead")]
        public ActionResult TransactionByShopGroup(string FromDate, string ToDate, FormCollection form)
        {
            try
            {
                #region code
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                if (FromDate == null || FromDate == "")
                {
                    //FromDate = DateTime.Now.ToString("M/dd/yyyy");
                }
                if (ToDate == null || ToDate == "")
                {
                    //ToDate = DateTime.Now.ToString("M/dd/yyyy");
                }
                //if (DateTime.TryParse(FromDate, out lFromDate)) { }
                //if (DateTime.TryParse(ToDate, out lToDate)) { }
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;

                ViewBag.lFromDate = lFromDate;
                ViewBag.lToDate = lToDate;

                TempData["FromDate"] = ViewBag.FromDate;
                TempData["ToDate"] = ViewBag.ToDate;
                TempData["Franchise"] = ViewBag.Franchise;

                Report lReport = new Report();

                int lFranchiseID = 0;
                if (form.AllKeys.Contains("Franchise"))
                {
                    if (int.TryParse(form["Franchise"].ToString(), out lFranchiseID)) { }
                    TempData["FranchiseID"] = lFranchiseID;
                }
                List<Franchise> lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson.Length > 0).OrderBy(x => x.ContactPerson).ToList();
                ViewBag.Franchise = new SelectList(lFranchises, "ID", "ContactPerson", lFranchiseID);

                //long lShopID = 0;
                //if(form.AllKeys.Contains("Shop"))
                //{
                //    if (long.TryParse(form["Shop"].ToString(), out lShopID)) { }
                //}
                //List<Shop> lShops = db.Shops.Where(x => x.IsActive == true && x.FranchiseID == lFranchiseID && x.Name.Length > 0).OrderBy(x => x.Name).ToList();
                //ViewBag.Shop = new SelectList(lShops, "ID", "Name", lShopID);


                List<ReportTransactionInputViewModel> lReportTransactionViewModels = filterData(lReport.ReportTransactionViewModel(lFromDate, lToDate));
                lReportTransactionViewModels = lReportTransactionViewModels.
                                                Where(x => x.MCOShopID == lFranchiseID).
                                                Where(x => x.CODCreateDate.Date >= lFromDate.Date && x.CODCreateDate.Date <= lToDate.Date).ToList();
                List<TransactionByShopGroupViewModel> lTransactionByShopGroupViewModels = (from rtvm in lReportTransactionViewModels
                                                                                           group rtvm by rtvm.ShopID into g
                                                                                           select new TransactionByShopGroupViewModel
                                                                                           {
                                                                                               ShopID = g.Key,
                                                                                               ShopName = g.FirstOrDefault(x => x.ShopID == g.Key).ShopName,
                                                                                               //Product = ,
                                                                                               //ShopStockID = ,
                                                                                               QtyAskedByCustomer = g.Sum(x => x.Qty),
                                                                                               //MRPPerUnit = g.Sum(x => x.TotalMRP),
                                                                                               //SaleRatePerUnit = ,
                                                                                               //OfferInPercentByShopPerUnit = ,
                                                                                               //OfferInRsByShopPerUnit = ,
                                                                                               //IsInclusiveOfTAX = ,
                                                                                               //ServiceTAX = ,
                                                                                               IsShopHandleOtherTAX = g.FirstOrDefault(x => x.ShopID == g.Key).IsShopHandleOtherTAX,
                                                                                               SumOfOtherTAX = g.Sum(x => x.SumOfOtherTAX),
                                                                                               //LandingPriceByShopPerUnit = ,
                                                                                               //ChargeINPercentByGBPerUnit = ,

                                                                                               //-- ShopOut -----------------------------------------------------------------------------------
                                                                                               QtyShopOut = g.Sum(x => x.QtyShop),
                                                                                               TotalMRP = g.Sum(x => x.TotalMRP),
                                                                                               TotalSaleRate = g.Sum(x => x.TotalSaleRate),
                                                                                               ShopTotalOffer = g.Sum(x => x.ShopTotalOffer),
                                                                                               NewSaleRateAfterOffer = g.Sum(x => x.NewSaleRateAfterOffer),
                                                                                               TotalShopFinalPrice = g.Sum(x => x.TotalShopFinalPrice),
                                                                                               ShopReceivable = g.Sum(x => x.ShopReceivable),
                                                                                               IsShopHandleOtherTAX1 = g.FirstOrDefault(x => x.ShopID == g.Key).IsShopHandleOtherTAX1,
                                                                                               OtherTAXPayableReceivableFromMerchant = g.Sum(x => x.OtherTAXPayableReceivableFromMerchant),
                                                                                               SumOfAmountShopReceivableAfterOtherTAX = g.Sum(x => x.SumOfAmountShopReceivableAfterOtherTAX),
                                                                                               GBReceivableAmount = g.Sum(x => x.GBReceivableAmount),
                                                                                               GBTransactionFee = g.Sum(x => x.GBTransactionFee),
                                                                                               GBServiceTAXOnTransactionFee = g.Sum(x => x.GBServiceTAXOnTransactionFee),
                                                                                               FinalShopReceivableAfterAllDone = g.Sum(x => x.FinalShopReceivableAfterAllDone),
                                                                                               ProcessRemark = ""
                                                                                           }).OrderBy(x => x.ShopName).ToList();

                TempData["TransactionByShopGroupViewModels"] = lTransactionByShopGroupViewModels;
                TempData.Keep();

                return View(lTransactionByShopGroupViewModels);
                #endregion
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[ReportTransaction] :- TransactionByShopGroup[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Report!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Report!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "ReportTransaction/CanRead")]
        public ActionResult TransactionByOrderGroup(string SearchString, int? page, string FromDate, string ToDate, FormCollection form)
        {
            try
            {
                #region code
                pageSize = 300;
                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.SearchString = SearchString;
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
                if (FromDate == null || FromDate == "")
                {
                    //FromDate = DateTime.Now.ToString("M/dd/yyyy");
                }
                if (ToDate == null || ToDate == "")
                {
                    //ToDate = DateTime.Now.ToString("M/dd/yyyy");
                }
                //if (DateTime.TryParse(FromDate, out lFromDate)) { }
                //if (DateTime.TryParse(ToDate, out lToDate)) { }
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;

                ViewBag.lFromDate = lFromDate;
                ViewBag.lToDate = lToDate;

                TempData["FromDate"] = ViewBag.FromDate;
                TempData["ToDate"] = ViewBag.ToDate;
                TempData["Franchise"] = ViewBag.Franchise;

                Report lReport = new Report();

                int lFranchiseID = -1;
                if (form.AllKeys.Contains("Franchise"))
                {
                    if (int.TryParse(form["Franchise"].ToString(), out lFranchiseID)) { }
                    TempData["FranchiseID"] = lFranchiseID;
                }
                else if (TempData["FranchiseID"] != null)
                {
                    if (int.TryParse(TempData["FranchiseID"].ToString(), out lFranchiseID)) { }
                }
                List<Franchise> lFranchises = db.Franchises.Where(x => x.IsActive == true && x.ContactPerson.Length > 0).OrderBy(x => x.ContactPerson).ToList();
                ViewBag.Franchise = new SelectList(lFranchises, "ID", "ContactPerson", lFranchiseID);

                List<ReportTransactionInputViewModel> lReportTransactionViewModels = filterData(lReport.ReportTransactionViewModel(lFromDate, lToDate));
                lReportTransactionViewModels = lReportTransactionViewModels.
                                                Where(x => x.MCOShopID == lFranchiseID).
                                                Where(x => x.CODCreateDate.Date >= lFromDate.Date && x.CODCreateDate.Date <= lToDate.Date).OrderBy(x => x.OrderCode).ToList();
                TempData.Keep();
                return View(lReportTransactionViewModels.ToList().ToPagedList(pageNumber, pageSize));
                #endregion

            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[ReportTransaction] :- TransactionByOrderGroup[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Report!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Report!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }

        }


        [SessionExpire]
        [CustomAuthorize(Roles = "ReportTransaction/CanRead")]
        public ActionResult TransactionDetail(string SearchString, int? page, long pShopID)
        {
            try
            {
                DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(TempData["FromDate"].ToString());
                DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(TempData["ToDate"].ToString());

                //if (DateTime.TryParse(TempData["FromDate"].ToString(), out lFromDate)) { }
                //if (DateTime.TryParse(TempData["ToDate"].ToString(), out lToDate)) { }

                int pageNumber = (page ?? 1);
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;

                Report lReport = new Report();
                List<ReportTransactionInputViewModel> lReportTransactionViewModels = filterData(lReport.ReportTransactionViewModel(lFromDate, lToDate));
                lReportTransactionViewModels = lReportTransactionViewModels.Where(x => x.ShopID == pShopID)
                                                                           .Where(x => x.CODCreateDate.Date >= lFromDate.Date && x.CODCreateDate.Date <= lToDate.Date).ToList();
                if (SearchString != null && SearchString != "")
                {
                    lReportTransactionViewModels = lReportTransactionViewModels.Where(x => x.Product.Contains(SearchString) || x.OrderCode.Contains(SearchString) || x.ShopOrderCode.Contains(SearchString)).ToList();
                }
                TempData.Keep();
                return View(lReportTransactionViewModels.OrderByDescending(x => x.CODCreateDate).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                errStr.Append("Method Name[ReportTransaction] :- TransactionDetail[HttpGet]" + Environment.NewLine +
                               "ON Dated" + DateTime.UtcNow.AddHours(5.30).TimeOfDay + Environment.NewLine +
                                   ex.Message.ToString() + Environment.NewLine +
                         "====================================================================================="
                               );
                ModelState.AddModelError("Message", "Sorry! Problem in Generate Report!!");
                ErrorLog.ErrorLogFile("Sorry! Problem in Generate Report!! " + Environment.NewLine + errStr.ToString()
                    , ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                return View();
            }
        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        public ActionResult ExportData(Boolean? pIsExportSummary, long? pShopID, long? pFranchiseID, string FromDate, string ToDate)
        {
            DateTime lFromDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(FromDate);
            DateTime lToDate = BusinessLogicLayer.CommonFunctions.GetProperDateTime(ToDate);
            if (FromDate == null || FromDate == "")
            {
                //FromDate = DateTime.Now.ToString("M/dd/yyyy");
            }
            if (ToDate == null || ToDate == "")
            {
                //ToDate = DateTime.Now.ToString("M/dd/yyyy");
            }
            //if (DateTime.TryParse(FromDate, out lFromDate)) { }
            //if (DateTime.TryParse(ToDate, out lToDate)) { }


            DataTable dt = new DataTable();
            string lExportFileName = DateTime.Now.ToShortDateString();

            if (pIsExportSummary != null && pIsExportSummary == true)
            {
                List<TransactionByShopGroupViewModel> lTransactionByShopGroupViewModels = new List<TransactionByShopGroupViewModel>();
                if (TempData["TransactionByShopGroupViewModels"] != null)
                {
                    lTransactionByShopGroupViewModels = (List<TransactionByShopGroupViewModel>)TempData["TransactionByShopGroupViewModels"];
                }
                dt = ToDataTable(lTransactionByShopGroupViewModels);
                lExportFileName = "SummaryReport_" + DateTime.Now.ToShortDateString();
            }
            else
            {
                Report lReport = new Report();
                List<ReportTransactionInputViewModel> lReportTransactionViewModels = filterData(lReport.ReportTransactionViewModel(lFromDate, lToDate));

                if (pShopID != null)
                {
                    lReportTransactionViewModels = lReportTransactionViewModels.Where(x => x.ShopID == pShopID)
                                                                               .Where(x => x.CODCreateDate.Date >= lFromDate.Date && x.CODCreateDate.Date <= lToDate.Date).ToList();
                    string lShopName = lReportTransactionViewModels.FirstOrDefault().ShopName;
                    lShopName = lShopName.Replace(" ", "_");
                    lExportFileName = lShopName + "_" + DateTime.Now.ToShortDateString();
                }
                else if (pFranchiseID != null)
                {
                    lReportTransactionViewModels = lReportTransactionViewModels.Where(x => x.MCOShopID == pFranchiseID)
                                                               .Where(x => x.CODCreateDate.Date >= lFromDate.Date && x.CODCreateDate.Date <= lToDate.Date).ToList();
                    lExportFileName = "DetailedReport_" + DateTime.Now.ToShortDateString();
                }
                if (lReportTransactionViewModels.Count <= 0)
                {
                    //- no records found.
                    return View("Index", "ExportData");
                }
                List<AccountExportViewModel> lAccountExportViewModels = getExportList(lReportTransactionViewModels);
                dt = ToDataTable(lAccountExportViewModels);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + lExportFileName + ".xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            return RedirectToAction("Index", "ExportData");
        }


        private List<AccountExportViewModel> getExportList(List<ReportTransactionInputViewModel> pReportTransactionViewModels)
        {
            List<AccountExportViewModel> lAccountExportViewModels = (from rtvm in pReportTransactionViewModels
                                                                     select new AccountExportViewModel
                                                                     {
                                                                         //-- extra added on 27-april-2016, as required by nilavana. ---------------------------------
                                                                         CustomerName = rtvm.CustomerName,
                                                                         //public string CustomerRegMobile { get; set; }
                                                                         //public long CustomerPersonalDetailID { get; set; }
                                                                         //public string CustomerName { get; set; }
                                                                         //public string CustomerShippingAddress { get; set; }
                                                                         //-- Extra For Display -------------------------------------------------------------------------
                                                                         OrderCode = rtvm.OrderCode,
                                                                         ShopOrderCode = rtvm.ShopOrderCode,
                                                                         CODCreateDate = rtvm.CODCreateDate.ToString("dd-MMM-yyyy HH:mm:ss"),
                                                                         PaymentMode = rtvm.PaymentMode,

                                                                         //-- input -------------------------------------------------------------------------------------
                                                                         Product = rtvm.Product,
                                                                         ShopName = rtvm.ShopName,
                                                                         Qty = rtvm.Qty,
                                                                         Size = rtvm.Size,
                                                                         MRPPerUnit = rtvm.MRPPerUnit,
                                                                         SaleRatePerUnit = rtvm.SaleRatePerUnit,
                                                                         IsInclusiveOfTAX = rtvm.IsInclusiveOfTAX,
                                                                         ServiceTAX = rtvm.ServiceTAX,
                                                                         LandingPriceByShopPerUnit = rtvm.LandingPriceByShopPerUnit,
                                                                         ChargeINPercentByGBPerUnit = rtvm.ChargeINPercentByGBPerUnit,

                                                                         //-- ShopOut -----------------------------------------------------------------------------------
                                                                         QtyShop = rtvm.QtyShop,
                                                                         TotalMRP = rtvm.TotalMRP,
                                                                         TotalSaleRate = rtvm.TotalSaleRate,
                                                                         GBReceivableAmount = rtvm.GBReceivableAmount,
                                                                         GBTransactionFee = rtvm.GBTransactionFee,
                                                                         GBServiceTAXOnTransactionFee = rtvm.GBServiceTAXOnTransactionFee,
                                                                         FinalShopReceivableAfterAllDone = rtvm.FinalShopReceivableAfterAllDone
                                                                     }).ToList();
            return lAccountExportViewModels;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }








    }
}
