using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;

using System.Data.SqlClient;
using System.Data.SqlTypes;

using BusinessLogicLayer.Account;
using ModelLayer.Models.ViewModel.Report.Account;
using Franchise.Models;


using PagedList;
using PagedList.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;

namespace Franchise.Controllers
{
    public class FVSaleMarginProductWiseReportController : Controller
    {


        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();


        // GET: FVSaleMarginProductWiseReport
        public ActionResult Index(SampleofFVSaleMarginProduct_WiseReportViewModelList obj1)
        {
            SampleofFVSaleMarginProduct_WiseReportViewModelList obj = new SampleofFVSaleMarginProduct_WiseReportViewModelList();

            List<SampleofFVSaleMarginProduct_WiseReportViewModel> list = new List<SampleofFVSaleMarginProduct_WiseReportViewModel>();

            long FrenchiseID = 0;
            if (Session["FRANCHISE_ID"] != null)
            {
                FrenchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
            }

            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("dd/MM/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }

            DateTime? DeliveryDate = DateTime.Now;
            DateTime _FromDate = ConvertDateFromStringToDate(obj.FromDate);
            DateTime _ToDate = ConvertDateFromStringToDate(obj.ToDate);
            DateTime fDate1 = Convert.ToDateTime(_FromDate);
            DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);


            //DateTime fDate = Convert.ToDateTime(obj.FromDate);
            //DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);




            var TotalGstAmount = 0;
            var TotalAmount = 0;


            //var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            //var CreateDate = DateTime.Parse("03/17/2017 ");
            //var FrenchiseID = Convert.ToInt64(10);
            var list3 = db.Database.SqlQuery<SampleofFVSaleMarginProduct_WiseReportViewModel>(
          "exec dbo.[FVSaleMarginProductWiseReport] @FranchiseID,@FromDate,@ToDate",
          new Object[] { new SqlParameter("@FranchiseID",FrenchiseID),

           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
     ).ToList();


            foreach (var row in list3)
            {
                var Result1 = row.TotalAmount - row.TotalGstAmount;

            }

            obj.lSampleofFVSaleMarginProduct_WiseReportViewModel = list3;

            //var result = obj.lSampleofFVSaleMarginProduct_WiseReportViewModel.Where(x => x.OrderPlacedDate >= fDate1 &&
            //                           x.OrderPlacedDate <= tDate1).ToList();
            // obj.lSampleofFVSaleMarginProduct_WiseReportViewModel = result.ToList();

            return View(obj);

        }



        [NonAction]
        public DateTime ConvertDateFromStringToDate(string _date)
        {

            DateTime datetime = DateTime.Now;
            String[] SplitStringDate = _date.Split('/');
            datetime = new DateTime(Convert.ToInt32(SplitStringDate[2]), Convert.ToInt32(SplitStringDate[1]), Convert.ToInt32(SplitStringDate[0]));
            return datetime;
        }

        public ActionResult Export(string FromDate, string ToDate, string option, SampleofFVSaleMarginProduct_WiseReportViewModelList obj1) //, int? print

        {
            int optionvalue = Convert.ToInt32(1);
            try
            {
                SampleofFVSaleMarginProduct_WiseReportViewModelList obj = new SampleofFVSaleMarginProduct_WiseReportViewModelList();

                List<SampleofFVSaleMarginProduct_WiseReportViewModel> list = new List<SampleofFVSaleMarginProduct_WiseReportViewModel>();

                long FrenchiseID = 0;
                if (Session["FRANCHISE_ID"] != null)
                {
                    FrenchiseID = Convert.ToInt64(Session["FRANCHISE_ID"]);
                }




                if (string.IsNullOrEmpty(obj.FromDate))
                {
                    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.FromDate = obj1.FromDate;
                }
                if (string.IsNullOrEmpty(obj1.ToDate))
                {
                    obj1.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.ToDate = obj1.ToDate;
                }


                ViewBag.PossibleSuppliers = new SelectList(db.Warehouses.ToList(), "ID", "Name");

                var FromDate1 = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                var ToDate1 = Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                DateTime _FromDate = ConvertDateFromStringToDate(FromDate);
                DateTime _ToDate = ConvertDateFromStringToDate(ToDate);
                DateTime fDate1 = Convert.ToDateTime(_FromDate);
                DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);


                var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

                var list3 = db.Database.SqlQuery<SampleofFVSaleMarginProduct_WiseReportViewModel>(
        "exec dbo.[FVSaleMarginProductWiseReport] @FranchiseID,@FromDate,@ToDate",
        new Object[] { new SqlParameter("@FranchiseID",FrenchiseID),

           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
   ).ToList();

                obj.lSampleofFVSaleMarginProduct_WiseReportViewModel = list3;
                var result = obj.lSampleofFVSaleMarginProduct_WiseReportViewModel.Where(x => x.OrderPlacedDate >= fDate1 &&
           x.OrderPlacedDate <= tDate1).ToList();
                obj.lSampleofFVSaleMarginProduct_WiseReportViewModel = result.ToList();


                if (optionvalue != null && optionvalue > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("Order Date", typeof(DateTime));

                    dt.Columns.Add("Order Number", typeof(string));
                    dt.Columns.Add("Order Delivery Date", typeof(DateTime));
                    dt.Columns.Add("Customer Name", typeof(string));

                    dt.Columns.Add("PaymentMode", typeof(string));
                    dt.Columns.Add("Wallet Amount Used", typeof(decimal));

                    dt.Columns.Add("Delivery Charge", typeof(decimal));
                    dt.Columns.Add("SKUID", typeof(long));

                    dt.Columns.Add("HSNCode", typeof(string));
                    dt.Columns.Add("SKU Name", typeof(string));
                    dt.Columns.Add("SKUUnit", typeof(string));
                    dt.Columns.Add("BrandName", typeof(string));
                    dt.Columns.Add("BatchCode", typeof(string)); //new 


                    dt.Columns.Add("Quantity Sold in Order", typeof(int));
                    dt.Columns.Add("PurchaseAmount", typeof(decimal));
                    dt.Columns.Add(" Sale Amount", typeof(decimal));
                    dt.Columns.Add(" BuyRatePerUnit", typeof(decimal));

                    dt.Columns.Add(" SaleRatePerUnit", typeof(decimal));
                    dt.Columns.Add(" MarginAmount", typeof(decimal));
                    dt.Columns.Add("MRP", typeof(decimal));

                    dt.Columns.Add("GST %", typeof(decimal));
                    dt.Columns.Add("CGST Amt on Sale", typeof(decimal));
                    //dt.Columns.Add("SGST %", typeof(decimal));
                    dt.Columns.Add("SGST Amt on Sale", typeof(decimal));
                    //dt.Columns.Add("TotalAmount", typeof(decimal));
                    dt.Columns.Add("Retailpoint", typeof(decimal));
                    dt.Columns.Add("DV Invoice Code", typeof(string));
                    dt.Columns.Add("DV Invoice Date", typeof(DateTime));

                    dt.Columns.Add("InitialQuantity", typeof(int));
                    dt.Columns.Add("AvailableInStock", typeof(int));

                    dt.Columns.Add("Received Date", typeof(DateTime));
                    dt.Columns.Add("Supplier Name", typeof(string));
                    dt.Columns.Add("Last Purchase Date", typeof(DateTime));
                    dt.Columns.Add("PO Code", typeof(string));

                    //dt.Columns.Add("City", typeof(string));
                    //dt.Columns.Add("PincodeID", typeof(int));
                    //dt.Columns.Add("Email", typeof(string));
                    //dt.Columns.Add("Mobile", typeof(string));

                    //dt.Columns.Add("ShippingAddress", typeof(string));
                    //dt.Columns.Add("JoiningDate", typeof(DateTime));


                    //dt.Columns.Add("CustomerOrderID", typeof(long));

                    //dt.Columns.Add("SaleRate", typeof(string));
                    //dt.Columns.Add("OrderStatus", typeof(string));

                    //dt.Columns.Add("HSNCode", typeof(string));
                    //dt.Columns.Add("SaleRate", typeof(string));
                    //dt.Columns.Add("PaymentMode", typeof(string));



                    int i = 0;
                    foreach (var row in obj.lSampleofFVSaleMarginProduct_WiseReportViewModel)
                    {
                        i = i + 1;



                        dt.LoadDataRow(new object[] { i,    row.OrderPlacedDate ,row.OrderCode,
                         row.DeliveryDate,
                      row.Customer,
                     row.PaymentMode,
                     row.MLMAmountUsed,
                       row.DeliveryCharge,
                       row.SKUID,
                      row.HSNCode,
                         row.SKUName,
                         row.SKUUnit,
                         row.BrandName,
                        row.BatchCode,
                         row.Qty,
                         row.PurchaseAmount,
                         row.SaleAmount,
                         row.BuyRatePerUnit,
                         row.SaleRatePerUnit,
                         row.MarginAmount,
                         row.MRP,

                         row.GSTPercentage,
                         row.CGSTAmt,
                            //row.SGST,
                            row.SGSTAmt,
                            //row.TotalAmount,
                            row.Retailpoint,
                            row.DVInvoiceCode,
                            row.DVInvoiceDate,
                            row.InitialQuantity,
                            row.AvailableQuantity,
                            row.ReceivedDate,
                            row.SupplierName,
                            row.LastPurchaseDate,
                            row.POCode,

                           }, false);


                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (optionvalue == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "Index");
                    }
                    else if (optionvalue == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "Index");
                    }
                    else if (optionvalue == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index", obj);
                }
            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }


    }
}




