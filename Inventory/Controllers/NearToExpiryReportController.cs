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
    public class NearToExpiryReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();


        // GET: /NearToExpiry/
        public ActionResult GetNearToExpiry(NearToExpiryReportViewModelList obj1, string NeartoExpiry)
        {
            NearToExpiryReportViewModelList obj = new NearToExpiryReportViewModelList();
            List<NearToExpiryReportViewModel> list = new List<NearToExpiryReportViewModel>();
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
            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);
            obj.WarehouseID = obj1.WarehouseID;

            //obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");

            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");


            var CurrentDate = System.DateTime.Now;
            var CurrentDate_ = CurrentDate.ToShortDateString();

            var reportList = db.Database.SqlQuery<NearToExpiryReportViewModel>(
            "exec dbo.[NEARTOEXPIRY] @WarehouseID,@CurrentDate",
             new Object[] { new SqlParameter("@WarehouseID",WarehouseID),
              new SqlParameter("@CurrentDate",CurrentDate_) }
               ).ToList();


            var ExpiryDate = reportList.Where(x => x.DaysLefttoExpire <= 30).ToList();
            List<NearToExpiryReportViewModel> result1 = new List<NearToExpiryReportViewModel>();
            DateTime today = DateTime.Today;

            if (NeartoExpiry == "0")
            {
                result1 = reportList.Where(x => x.DaysLefttoExpire <= 30).ToList();
            }

          
                else
                    if (NeartoExpiry == "1")
                    {
                        result1 = reportList.Where(x => x.DaysLefttoExpire <= 60).ToList();
                    }
                    else
                        if (NeartoExpiry == "2")
                        {
                            result1 = reportList.Where(x => x.DaysLefttoExpire <= 90).ToList();
                        }
                        else
                            if (NeartoExpiry == "3")
                            {
                                result1 = reportList.Where(x => x.DaysLefttoExpire <= 180).ToList();
                            }

                        
                                else
                                    if (NeartoExpiry == "4")
                                    {
                                        result1 = reportList.Where(x => x.DaysLefttoExpire <= 365).ToList();
                                    }
                                    else
                                        if (NeartoExpiry == "5")
                                        {
                                            result1 = reportList.Where(x => x.DaysLefttoExpire >= 365).ToList();
                                        }



            //if (ExpiryDate =< today)
            //    ExpiryDate = ExpiryDate.AddYears(1);

            //int numDays = (next - today).Days;


            obj.lNearToExpiryReportViewModel = result1.ToList();


            //obj.lNearToExpiryReportViewModel = reportList;

            //if (obj1.WarehouseID != null)
            //{
            //    obj.lNearToExpiryReportViewModel =obj.lNearToExpiryReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
            //}
            if (obj1.WarehouseID != null && NeartoExpiry != null)
            {
                obj.lNearToExpiryReportViewModel = obj.lNearToExpiryReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
            }


            DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
            DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);


            //var result = obj.lNearToExpiryReportViewModel.Where(x => x.CreateDate >= fDate1 &&
            //                      x.CreateDate <= tDate1).ToList();
            //obj.lNearToExpiryReportViewModel = result.ToList();

            return View(obj);


        }



        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID) //, int? print
        {
            try
            {
                NearToExpiryReportViewModelList obj = new NearToExpiryReportViewModelList();
                List<NearToExpiryReportViewModel> list = new List<NearToExpiryReportViewModel>();


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

                obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");

                obj.WarehouseID = obj.WarehouseID;

                var CurrentDate = System.DateTime.Now;
                var CurrentDate_ = CurrentDate.ToShortDateString();

                var reportList = db.Database.SqlQuery<NearToExpiryReportViewModel>(
                "exec dbo.[NEARTOEXPIRY] @WarehouseID",
                 new Object[] { new SqlParameter("@WarehouseID",obj.WarehouseID),
              new SqlParameter("@CurrentDate",CurrentDate) ,  }
                   ).ToList();



                obj.lNearToExpiryReportViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lNearToExpiryReportViewModel = obj.lNearToExpiryReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);


                var result = obj.lNearToExpiryReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lNearToExpiryReportViewModel = result.ToList();


                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("SKUName", typeof(string));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("SKUUnit", typeof(string));
                    dt.Columns.Add("Batch Code", typeof(string));
                    dt.Columns.Add("Invoice no", typeof(string));

                    dt.Columns.Add("Manufacturer", typeof(string));
                    //dt.Columns.Add("ProductVarient", typeof(string));
                    dt.Columns.Add("Supplier", typeof(string));
                    dt.Columns.Add("AvailableQuantity", typeof(int));
                    //dt.Columns.Add("ReorderLevel", typeof(int));
                    dt.Columns.Add("ExpiryDate", typeof(DateTime));
                    dt.Columns.Add("Amount", typeof(Decimal));
                    dt.Columns.Add("DaysLefttoExpire", typeof(int));
                    dt.Columns.Add("LocationID", typeof(long));
                    dt.Columns.Add("Invoice Date", typeof(DateTime));
                    dt.Columns.Add("PurchaseDate", typeof(DateTime));

                    int i = 0;
                    foreach (var row in obj.lNearToExpiryReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUName,row.SKUID, row.SKUUnit, row.BatchCode,row.InvoiceNO,row.Manifecturer,row.SupplierName ,
                       row.AvailableQuantity,
                       //row.ReorderLevel,
                       row.ExpiryDate,row.Amount,row.DaysLefttoExpire,row.LocationID,row.InvoiceDate,  row.PurchaseDate}, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetNearToExpiry");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetNearToExpiry");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetNearToExpiry");
                    }
                }
                else
                {
                    return View("GetNearToExpiry", obj);
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

