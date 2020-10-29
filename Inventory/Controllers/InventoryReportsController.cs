using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;

namespace Inventory.Controllers
{
    public class InventoryReportsController : Controller
    {

        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();

        //
        // GET: /InventoryReports/
        public ActionResult GetInventoryItemReport(InventoryReportViewModelList obj1, string ExportOption)
        {
            InventoryReportViewModelList obj = new InventoryReportViewModelList();
            List<InventoryReportViewModel> list = new List<InventoryReportViewModel>();

            //if (ExportOption == null && obj1.WarehouseID == 0)
            //{
            //    ExportOption = "1";
            //}
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
            if (obj.Checkbox == false)
            {
                obj.Checkbox = true;
            }
            else
            {
                obj.Checkbox = obj1.Checkbox;
            }


            if (obj1.WarehouseID == 0)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }




            DateTime fDate = Convert.ToDateTime(obj.FromDate);
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);
            obj.WarehouseID = obj1.WarehouseID;

            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");

            // obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");

            //  var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            // var ToDate = Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });



            //  var reportList = db.Database.SqlQuery<InventoryReportViewModel>("exec InventoryReport").ToList();

            var fDate11 = "2018-04-01";
            var CurrentDate = System.DateTime.Now;
            var tDate11 = CurrentDate.ToString();



            List<InventoryReportViewModel> result1 = new List<InventoryReportViewModel>();
            if (obj.Checkbox == true && ExportOption == null)
            {


                var result2 = db.Database.SqlQuery<InventoryReportViewModel>(
        "exec dbo.[InventoryReport] @WarehouseID ,@FromDate,@ToDate",
        new Object[] {
                  new SqlParameter("@WarehouseID", WarehouseID), 
           new SqlParameter("@FromDate", fDate11),
           new SqlParameter("@ToDate", tDate11)}
           ).ToList();

                obj.lInventoryReportViewModel = result2.Where(x => x.AvailableInStock > Convert.ToInt32(ExportOption = "0")).ToList();

            }



            else
            {
                var reportList = db.Database.SqlQuery<InventoryReportViewModel>(
      "exec dbo.[InventoryReport] @WarehouseID ,@FromDate,@ToDate",
      new Object[] {
                  new SqlParameter("@WarehouseID", WarehouseID), 
           new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
         ).ToList();




                var AvailableInStock1 = reportList.Where(x => x.AvailableInStock == x.AvailableInStock).ToList();
                List<InventoryReportViewModel> result = new List<InventoryReportViewModel>();
                if (ExportOption == "0")
                {

                    result = reportList.Where(x => x.AvailableInStock == Convert.ToInt32(ExportOption)).ToList();
                    // obj.lInventoryReportViewModel = result.ToList();
                }
                else
                    if (ExportOption == "1")
                    {
                        result = reportList.Where(x => x.AvailableInStock > Convert.ToInt32(ExportOption = "0")).ToList();
                    }
                    else
                        if (ExportOption == "2")
                        {
                            result = reportList.ToList();
                        }

                obj.lInventoryReportViewModel = result.ToList();

            }


            //obj.lInventoryReportViewModel = result1.ToList();

            //1st time SET  ON INSTOCK
            //      if (obj1.WarehouseID == 0 && ExportOption == null)
            //{
            //    obj.lInventoryReportViewModel = reportList.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}



            //if (obj1.WarehouseID == 0 && obj.Checkbox == true)
            //{
            //    obj.lInventoryReportViewModel = result1.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}
            //if (obj1.WarehouseID == 0 && obj.Checkbox == true)
            //{
            //    obj.lInventoryReportViewModel = result1.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}


            if (obj1.WarehouseID != null)
            {
                obj.lInventoryReportViewModel = obj.lInventoryReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
            }


            //DateTime fDate1 = Convert.ToDateTime(obj1.FromDate);
            //DateTime tDate1 = Convert.ToDateTime(obj1.ToDate).AddHours(23);
            //var result = obj.lInventoryReportViewModel.Where(x => x.CreateDate >= fDate1 &&
            //                        x.CreateDate <= tDate1).ToList();

            //obj.lInventoryReportViewModel = result.ToList();

            return View(obj);
        }



        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID, string ExcelName) //, int? print
        {
            try
            {
                InventoryReportViewModelList obj = new InventoryReportViewModelList();
                List<InventoryReportViewModel> list = new List<InventoryReportViewModel>();
                // long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}
                string FileName = "";

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


                //obj.lWastageReportViewModel = list.ToList();
                obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");

                //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

                var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();

                obj.WarehouseID = obj.WarehouseID;



                //  obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
                obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");

                //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });


                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();

                var reportList = db.Database.SqlQuery<InventoryReportViewModel>(
  "exec dbo.[InventoryReport] @WarehouseID ,@FromDate,@ToDate",
  new Object[] {
                  new SqlParameter("@WarehouseID", obj.WarehouseID), 
           new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
     ).ToList();

                obj.lInventoryReportViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lInventoryReportViewModel = obj.lInventoryReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);


                var result = obj.lInventoryReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lInventoryReportViewModel = result.ToList();


                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("SKUName", typeof(string));
                    dt.Columns.Add("SKUUnit", typeof(string));

                    dt.Columns.Add("Brand Name", typeof(string));
                    dt.Columns.Add("BatchCode", typeof(string));
                    dt.Columns.Add("InvoiceCode", typeof(string));

                    dt.Columns.Add("InitialQuantity", typeof(int));
                    dt.Columns.Add("AvailableInStock", typeof(int));
                    dt.Columns.Add("ReorderLevel", typeof(int));
                    dt.Columns.Add("SupplierName", typeof(string));
                    dt.Columns.Add("Category1", typeof(string));
                    dt.Columns.Add("Category2", typeof(string));
                    dt.Columns.Add("Category3", typeof(string));
                    dt.Columns.Add("Amount", typeof(decimal));
                    dt.Columns.Add("Create Date", typeof(DateTime));
                    dt.Columns.Add("BuyRatePerUnit", typeof(decimal));

                    dt.Columns.Add("SaleRate", typeof(decimal));

                    dt.Columns.Add("MRP", typeof(decimal));
                    dt.Columns.Add("RetailPoint", typeof(decimal));
                    dt.Columns.Add("ExpiryDate", typeof(DateTime));


                    int i = 0;
                    foreach (var row in obj.lInventoryReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i,row.SKUID, row.SKUName, row.SKUUnit,row.Manifecturer,row.BatchCode ,
                  row.InvoiceCode, 
                  row.InitialQuantity,
                  row.AvailableInStock,
                  row.SupplierName,row.Category1,row.Category2,row.Category3, row.Amount,row.CreateDate,
                        row.BuyRatePerUnit,row.SaleRate,row.MRP,row.RetailPoint, row.ExpiryDate}, false);
                    }

           
   
 
                     
          

                
                    Response.AddHeader("content-disposition", "attachment; filename=InventoryReports.xls");


               

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetInventoryItemReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetInventoryItemReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetInventoryItemReport");
                    }
                }
                else
                {
                    return RedirectToAction("GetInventoryItemReport", obj);
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[InventoryReportsController][POST:GetInventoryItemReport]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Exporting Order Transaction Report!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[InventoryReportsController][POST:GetInventoryItemReport]",
                    BusinessLogicLayer.ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
            }
            //catch (Exception ex)
            //{
            //    new Exception("Some unknown error encountered!");
            //}
           
            return View();
        }







    }
}
