using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using Inventory.Models;
using ModelLayer.Models.ViewModel;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Web.Services;
using BusinessLogicLayer;
using System.Transactions;
using System.Web.Configuration;
using System.Globalization;

namespace Inventory.Controllers
{
    public class InvoicesReceivedReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        // GET: /InvoicesReceived/
        public ActionResult GetInvoicesReceivedReport(InvoicesReceivedReportViewModelList obj1)
        {
            InvoicesReceivedReportViewModelList obj = new InvoicesReceivedReportViewModelList();
            List<InvoicesReceivedReportViewModel> list = new List<InvoicesReceivedReportViewModel>();

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }
           
            DateTime fDate = Convert.ToDateTime(obj.FromDate);
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            obj.WarehouseID = obj1.WarehouseID;


            //ViewBag.SelectedWarehouse = 0;
            //if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
            //{
            //    //display all warehouse Or Selected
            //    if (WarehouseID != null && WarehouseID != 0)
            //    {


            //        obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            //        list = list.Where(p => p.WarehouseID == WarehouseID).ToList();
            //        ViewBag.SelectedWarehouse = WarehouseID;
            //    }
            //    ViewBag.DV = "0";
            //}
            //else
            //{
            //    ViewBag.DV = "1";
            //    list = list.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}

          ///  obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
          ///  
            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
            

            var reportList = db.Database.SqlQuery<InvoicesReceivedReportViewModel>(
 "exec dbo.[InvoicesReceived] @WarehouseID ,@FromDate,@ToDate",
 new Object[] {
                  new SqlParameter("@WarehouseID", SupplierID), 
           new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
    ).ToList();
            obj.lInvoicesReceivedReportViewModel = reportList.ToList();
            var CGSTAmount1 = reportList.Select(x => x.CGSTAmount).ToList();
            var SGSTAmount1 = reportList.Select(y => y.SGSTAmount).ToList();
         var   quantity = reportList.Select(z => z.Quantity).ToList();
            foreach (InvoicesReceivedReportViewModel item in reportList)
            {
                decimal? CGSTAmount = item.CGSTAmount * Convert.ToDecimal (item.Quantity);
                decimal? SGSTAmount = item.SGSTAmount * Convert.ToDecimal(item.Quantity);
                decimal? ProductAmount = item.Quantity * item.UnitPrice;
               decimal ?TaxableValue = (ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
            TaxableValue = Math.Round((decimal)TaxableValue, 2);
                decimal? GSTAmount = ProductAmount - TaxableValue;
              item.SGSTAmount = GSTAmount / 2;
                item.CGSTAmount = GSTAmount / 2;
            }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
            DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);

            var result = obj.lInvoicesReceivedReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                  x.CreateDate <= tDate1).ToList();
            obj.lInvoicesReceivedReportViewModel = result.ToList();
            return View(obj);

        }


        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID) //, int? print
        {
            try
            {
                InvoicesReceivedReportViewModelList obj = new InvoicesReceivedReportViewModelList();
                List<InvoicesReceivedReportViewModel> list = new List<InvoicesReceivedReportViewModel>();

              
                if (string.IsNullOrEmpty(FromDate))
                {
                    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.FromDate = FromDate;
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.ToDate = ToDate;
                }

                DateTime fDate = Convert.ToDateTime(obj.FromDate);
                DateTime tDate = Convert.ToDateTime(obj.ToDate);

                if (Session["USER_NAME"] != null)
                { }
                else
                {
                    return RedirectToAction("Index", "Login");
                }



                if (string.IsNullOrEmpty(FromDate))
                {
                    obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.FromDate = FromDate;
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    obj.ToDate = ToDate;
                }

                obj.WarehouseID = WarehouseID;

                //obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");    //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

                var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();

                obj.WarehouseID = obj.WarehouseID;

                obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
                  //obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
               // obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");

                //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });


                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();
                  var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                var reportList = db.Database.SqlQuery<InvoicesReceivedReportViewModel>(
 "exec dbo.[InvoicesReceived] @WarehouseID ,@FromDate,@ToDate",
 new Object[] {
                  new SqlParameter("@WarehouseID", SupplierID), 
           new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
    ).ToList();
                

                obj.lInvoicesReceivedReportViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lInvoicesReceivedReportViewModel = obj.lInvoicesReceivedReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);
                var result = obj.lInvoicesReceivedReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lInvoicesReceivedReportViewModel= result.ToList();
                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));

                    dt.Columns.Add("SKUName", typeof(string));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("HSN Code" ,typeof(string));

                    dt.Columns.Add("Invoice No.", typeof(string));

                    dt.Columns.Add("Invoice Date", typeof(DateTime));

                    dt.Columns.Add("Unit Price", typeof(decimal));
                    dt.Columns.Add("MRP", typeof(decimal));
                    //dt.Columns.Add("Supplier Name", typeof(string));
                    dt.Columns.Add("Invoice Value", typeof(Decimal));
                    dt.Columns.Add("GST%", typeof(int));
                    dt.Columns.Add("CGSTAmt", typeof(Decimal));
                    dt.Columns.Add("SGSTAmt", typeof(Decimal));
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("SupplierName", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("PO NO.", typeof(string));
                    //dt.Columns.Add("QtyReceived", typeof(int));
                    //dt.Columns.Add("Batch_Code", typeof(string));

                    int i = 0;
                    foreach (var row in obj.lInvoicesReceivedReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUName, row.SKUID, row.HSNCode,row.InvoiceCode,row.CreateDate,row.UnitPrice,
                      row.MRP, row.Amount,row.GSTInPer,row.CGSTAmount,row.SGSTAmount, row.Quantity, row.Supplier,row.Status,row.PONumber}, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetInvoicesReceivedReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetInvoicesReceivedReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetInvoicesReceivedReport");
                    }
                }
                else
                {
                    return RedirectToAction("GetInvoicesReceivedReport", obj);
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