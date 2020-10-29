using BusinessLogicLayer;
using CRM.Models;
using CRM.Models.ViewModel;
using CRM.SalesOrder;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM.Controllers
{
    public class OutOfStockReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        //CommonController Obj_Common = new CommonController();

        // GET: /OutOfStockReport/
        public ActionResult GetOutOfStockReport(OutOfStockViewModelList obj1)
        {
            OutOfStockViewModelList obj = new OutOfStockViewModelList();
            List<OutOfStockViewModel> list = new List<OutOfStockViewModel>();
            //long WarehouseID = 0;
            //if (Session["WarehouseID"] != null)
            //{
            //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            //}

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

            DateTime _FromDate = ConvertDateFromStringToDate(obj.FromDate);
            DateTime _ToDate = ConvertDateFromStringToDate(obj.ToDate);
            DateTime fDate1 = Convert.ToDateTime(_FromDate);
            DateTime tDate1 = Convert.ToDateTime(_ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);


            //obj.WarehouseID = obj1.WarehouseID;



            var reportList = db.Database.SqlQuery<OutOfStockViewModel>(
           "exec dbo.[NewOutofStockReport] @FromDate,@ToDate",
              new Object[] {
                      //new SqlParameter("@WarehouseID", WarehouseID) ,
            new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
   ).ToList();

            //obj.lOutOfStockViewModel = reportList;


            //Add by Priti for fv and dv Franchise CITY
            List<OutOfStockViewModel> lOutOfStockViewModel1 = new List<OutOfStockViewModel>();
            var EntityFV = reportList.Select(x => x.IsFulfillmentCenter).ToList();
            if (reportList.Where(x => x.IsFulfillmentCenter).Select(x => x.IsFulfillmentCenter).ToList().Count > 0)
            {



                lOutOfStockViewModel1 = (from z in db.WarehouseFranchises
                                         join wf in db.Franchises on z.FranchiseID equals wf.ID
                                         join W in db.Warehouses on z.WarehouseID equals W.ID
                                         join sh in db.Pincodes on W.PincodeID equals sh.ID
                                         join c in db.Cities on sh.CityID equals c.ID

                                         select new OutOfStockViewModel
                                         {
                                             Franchises = wf.ContactPerson,
                                             City = c.Name,
                                             WarehouseID = W.ID
                                         }).ToList();

                if (lOutOfStockViewModel1 != null && lOutOfStockViewModel1.Count > 0)
                {
                    //obj.lOutOfStockViewModel = new List<OutOfStockViewModel>();
                    foreach (var item in reportList)
                    {
                        item.City = lOutOfStockViewModel1.Where(x => x.WarehouseID == item.WarehouseID).FirstOrDefault().City;
                        item.Franchises = lOutOfStockViewModel1.Where(x => x.WarehouseID == item.WarehouseID).FirstOrDefault().Franchises;
                        //OutOfStockViewModel objModel = new OutOfStockViewModel();

                    }
                }


               
            }
            obj.lOutOfStockViewModel = reportList.ToList();
            //End by Priti


            //List<OutOfStockViewModel>lOutOfStockViewModel = new List<OutOfStockViewModel>();



            //var EntityFV = reportList.Select(x => x.IsFulfillmentCenter).ToList();
            //if (reportList.Where(x => x.IsFulfillmentCenter).Select(x => x.IsFulfillmentCenter).ToList().Count > 0)
            //{



            //    lOutOfStockViewModel = (from z in db.Franchises
            //                            select new OutOfStockViewModel
            //                            {
            //                                Franchises = z.ContactPerson
            //                            }).ToList();

            //    reportList = lOutOfStockViewModel.ToList();
            //    obj.lOutOfStockViewModel = reportList;
            //}




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

        public ActionResult Export(string FromDate, string ToDate, int option) //, int? print
        {
            try
            {
                OutOfStockViewModelList obj = new OutOfStockViewModelList();
                List<OutOfStockViewModel> list = new List<OutOfStockViewModel>();



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

                //if (Session["USER_NAME"] != null)
                //{ }
                //else
                //{
                //    return RedirectToAction("Index", "Login");
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

                //obj.WarehouseID = WarehouseID;


                //obj.lWastageReportViewModel = list.ToList();
                obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");




                obj.WarehouseID = obj.WarehouseID;
                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);



                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();


                var reportList = db.Database.SqlQuery<OutOfStockViewModel>(
    "exec dbo.[NewOutofStockReport] @FromDate,@ToDate",
    new Object[] {
        //new SqlParameter("@WarehouseID", obj.WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
       ).ToList();
                obj.lOutOfStockViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddSeconds(59);


                var result = obj.lOutOfStockViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lOutOfStockViewModel = result.ToList();


                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("SKU Name", typeof(string));
                    dt.Columns.Add("SKU Unit", typeof(string));
                    dt.Columns.Add("HSNCode", typeof(string));
                    dt.Columns.Add("Brand Name", typeof(string));
                    dt.Columns.Add("Batch Code", typeof(string));
                    dt.Columns.Add("Invoice Code", typeof(string));


                    dt.Columns.Add("InitialQuantity", typeof(int));
                    dt.Columns.Add("AvailableQuantity", typeof(int));
                    dt.Columns.Add("SupplierName", typeof(string));


                    dt.Columns.Add("Category1", typeof(string));
                    dt.Columns.Add("Category2", typeof(string));
                    dt.Columns.Add("Category3", typeof(string));
                    dt.Columns.Add("Amount", typeof(decimal));
                    dt.Columns.Add("PurchaseDate", typeof(DateTime));
                    dt.Columns.Add("BuyRatePerUnit", typeof(decimal));

                    dt.Columns.Add("SaleRate", typeof(decimal));

                    dt.Columns.Add("MRP", typeof(decimal));
                    dt.Columns.Add("RetailPoint", typeof(decimal));
                    dt.Columns.Add("ExpiryDate", typeof(DateTime));

                    dt.Columns.Add("ReorderLevel", typeof(int));
                    dt.Columns.Add("ReorderHitDate", typeof(DateTime));

                    dt.Columns.Add("OutOfStockDate", typeof(DateTime));

                    dt.Columns.Add("Entity", typeof(string));
                    dt.Columns.Add("City", typeof(string));
                    dt.Columns.Add("Franchise", typeof(string));



                    int i = 0;
                    foreach (var row in obj.lOutOfStockViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUID, row.SKUName,row.SKUUnit,row.HSNCode,row.Manifecturer ,row.BatchCode,row.InvoiceCode,row.InitialQuantity,row.AvailableInStock,
                       row.SupplierName, row.Category1,row.Category2,row.Category3,row.Amount,row.CreateDate,row.BuyRatePerUnit,row.SaleRate,row.MRP,row.RetailPoint,row.ExpiryDate,row.ReorderLevel,row.ReorderHitDate ,row.OutOfStockDate,row.Entity,row.City,row.Franchises}, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetOutOfStockReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetOutOfStockReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetOutOfStockReport");
                    }
                }
                else
                {
                    return RedirectToAction("GetOutOfStockReport", obj);
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