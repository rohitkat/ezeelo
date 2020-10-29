using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Inventory.Controllers
{

    public class ReorderLevelReportsController : Controller
    {


        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();

        //
        // GET: /ReorderLevelReports/

        public ActionResult GetReorderLevel(ReorderLevelReportViewModelList obj1, int? Status) //Added by Rumana on 26-04-2019
        {
            var OrderStatus = Status;
            if (OrderStatus == null)
            {
                OrderStatus = 7;
            }
            ViewBag.DropdownSelected = OrderStatus;
            ReorderLevelReportViewModelList obj = new ReorderLevelReportViewModelList();
            List<ReorderLevelReportViewModel> list = new List<ReorderLevelReportViewModel>();
            List<ReorderLevelReportViewModelOnPlaced> objWRLVM = new List<ReorderLevelReportViewModelOnPlaced>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = "07/01/2018";
                //obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.FromDate = obj1.FromDate;
            }

            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.Now.AddHours(5.5).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }

            DateTime fDate = Convert.ToDateTime(obj.FromDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
            obj.WarehouseID = obj1.WarehouseID;

            //obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");

            //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

            var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();


            //string reportList;

            var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>(
      "exec dbo.[ReorderLevelReport] @WarehouseID,@FromDate,@ToDate, @OrderStatus",
         new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate),new SqlParameter("@OrderStatus", OrderStatus)}
).ToList();

            obj.lReorderLevelReportViewModel = reportList;
            if (obj1.WarehouseID == 0)
                {
                    obj.lReorderLevelReportViewModel = obj.lReorderLevelReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
                }
                if (obj1.WarehouseID != null)
                {
                    obj.lReorderLevelReportViewModel = obj.lReorderLevelReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
                }

                //DateTime fDate1 = Convert.ToDateTime(obj1.FromDate);
                //DateTime tDate1 = Convert.ToDateTime(obj1.ToDate).AddHours(23).AddMinutes(59);

                //if (obj1.FromDate != null && obj1.ToDate != null)
                //{
                //    obj.lReorderLevelReportViewModel = obj.lReorderLevelReportViewModel.Where(x => x.ReorderLevelHitDate >= fDate1 &&
                //                          x.ReorderLevelHitDate <= tDate1).ToList(); //Added by Rumana for getting ReorderLevelReport list as per ReorderLevelHitDate on 26-04-2019
                //}
                obj.lReorderLevelReportViewModel = obj.lReorderLevelReportViewModel.OrderByDescending(x => x.ReorderLevelHitDate).ToList();
         

            return View(obj);
        }
        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID, int? Status) //, int? print
        {
            try
            {
                ReorderLevelReportViewModelList obj = new ReorderLevelReportViewModelList();
                List<ReorderLevelReportViewModel> list = new List<ReorderLevelReportViewModel>();


                // long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}


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


                //obj.lWastageReportViewModel = list.ToList();
                obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");

                //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });


                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();
                if (Status == null)
                {
                    Status = 7;
                }
                ViewBag.DropdownSelected = Status;
                var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>(
"exec dbo.[ReorderLevelReport] @WarehouseID,@FromDate,@ToDate,@Status",
new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
                      new SqlParameter("@FromDate", fDate),
                     new SqlParameter("@ToDate", tDate),new SqlParameter("@Status", Status)}
 ).ToList();// Status Parameter Added by Rumana on 26-04-2019


                obj.lReorderLevelReportViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lReorderLevelReportViewModel = obj.lReorderLevelReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);

                var result = obj.lReorderLevelReportViewModel.Where(x => x.ReorderLevelHitDate >= fDate1 &&
                                 x.ReorderLevelHitDate <= tDate1).ToList(); //Added by Rumana for getting ReorderLevelReport list as per ReorderLevelHitDate on 26-04-2019
                obj.lReorderLevelReportViewModel = result.ToList();
                //var result = obj.lReorderLevelReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                //                      x.CreateDate <= tDate1).ToList();
                //obj.lReorderLevelReportViewModel = result.ToList();


                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("SKUName", typeof(string));
                    dt.Columns.Add("SKUUnit", typeof(string));
                    dt.Columns.Add("Manufacturer", typeof(string));
                    //dt.Columns.Add("ProductVarient", typeof(string));
                    dt.Columns.Add("SupplierID", typeof(long));
                    dt.Columns.Add("AvailableQuantity", typeof(int));
                    dt.Columns.Add("ReorderLevel", typeof(int));
                    //dt.Columns.Add("CreateDate", typeof(DateTime));
                    dt.Columns.Add("ReorderLevelHitDate", typeof(DateTime));




                    int i = 0;
                    foreach (var row in obj.lReorderLevelReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUName, row.SKUUnit,row.Manifecturer,row.SupplierID ,
                       row.AvailableQuantity, row.ReorderLevel,row.ReorderLevelHitDate}, false); //Changed by Rumana CreatedDate to ReorderLevelHitDate on 26-04-2019 
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetReorderLevel");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetReorderLevel");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetReorderLevel");
                    }
                }
                else
                {
                    return View("GetReorderLevel", obj);
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