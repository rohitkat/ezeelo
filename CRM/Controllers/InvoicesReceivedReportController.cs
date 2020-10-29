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
using CRM.Models;


using PagedList;
using PagedList.Mvc;

using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;

namespace CRM.Controllers
{
    public class InvoicesReceivedReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        //CommonController Obj_Common = new CommonController();
        // GET: /InvoicesReceived/
        public ActionResult GetInvoicesReceivedReport(InvoicesReceivedReportViewModelList obj1)
        {
            InvoicesReceivedReportViewModelList obj = new InvoicesReceivedReportViewModelList();
            List<InvoicesReceivedReportViewModel> list = new List<InvoicesReceivedReportViewModel>();

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

            obj.WarehouseID = obj1.WarehouseID;


         
         
            //var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            //obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");


            var reportList = db.Database.SqlQuery<InvoicesReceivedReportViewModel>(
 "exec dbo.[NewInvoicesReceived] @FromDate,@ToDate",
 new Object[] {
                  //new SqlParameter("@WarehouseID", SupplierID),
           new SqlParameter("@FromDate", fDate1),
           new SqlParameter("@ToDate", tDate1)}
    ).ToList();





            //Add by Priti for fv and dv Franchise CITY
            //List<InvoicesReceivedReportViewModel> lInvoicesReceivedReportViewModel1 = new List<InvoicesReceivedReportViewModel>();
            //var EntityFV = reportList.Select(x => x.IsFulfillmentCenter).ToList();
            //if (reportList.Where(x => x.IsFulfillmentCenter).Select(x => x.IsFulfillmentCenter).ToList().Count > 0)
            //{



            //    lInvoicesReceivedReportViewModel1 = (from z in db.WarehouseFranchises
            //                                         join wf in db.Franchises on z.FranchiseID equals wf.ID
            //                                         join W in db.Warehouses on z.WarehouseID equals W.ID
            //                                         join sh in db.Pincodes on W.PincodeID equals sh.ID
            //                                         join c in db.Cities on sh.CityID equals c.ID
            //                                         join po in db.PurchaseOrders on  W.ID equals po.WarehouseID

            //                                         select new InvoicesReceivedReportViewModel
            //                                         {
            //                                             Franchises = wf.ContactPerson,
            //                                             City = c.Name,
            //                                             WarehouseID = W.ID,
            //                                      SupplierID= po.SupplierID
            //                                         }).ToList();
            //    if (lInvoicesReceivedReportViewModel1 != null && lInvoicesReceivedReportViewModel1.Count > 0)
            //    {
            //        foreach (var item in reportList)
            //        {
            //            item.City = lInvoicesReceivedReportViewModel1.Where(x => x.SupplierID == item.SupplierID).FirstOrDefault().City;
            //            item.Franchises = lInvoicesReceivedReportViewModel1.Where(x => x.SupplierID== item.SupplierID).FirstOrDefault().Franchises;
            //        }
            //    }


            //}
            List<InvoicesReceivedReportViewModel> InvoicesReceivedReportViewModel1 = new List<InvoicesReceivedReportViewModel>();
            var EntityFV = reportList.Select(x => x.IsFulfillmentCenter).ToList();
            if (reportList.Where(x => x.IsFulfillmentCenter).Select(x => x.IsFulfillmentCenter).ToList().Count > 0)
            {



                InvoicesReceivedReportViewModel1 = (from z in db.WarehouseFranchises
                                              join wf in db.Franchises on z.FranchiseID equals wf.ID
                                              join W in db.Warehouses on z.WarehouseID equals W.ID
                                              join sh in db.Pincodes on W.PincodeID equals sh.ID
                                              join c in db.Cities on sh.CityID equals c.ID

                                              select new InvoicesReceivedReportViewModel
                                              {
                                                  Franchises = wf.ContactPerson,
                                                  City = c.Name,
                                                  WarehouseID = W.ID
                                              }).ToList();
                if (InvoicesReceivedReportViewModel1 != null && InvoicesReceivedReportViewModel1.Count > 0)
                {
                    foreach (var item in reportList)
                    {

                        string City = null;
                        item.City = InvoicesReceivedReportViewModel1.FirstOrDefault(x =>  x.WarehouseID == item.WarehouseID)?.City;
                        if (item.City != null)
                        {
                            City = item.City;
                        }
                        //item.City = InvoicesReceivedReportViewModel1.FirstOrDefault(x => x.WarehouseID == item.WarehouseID).City;
                        item.Franchises = InvoicesReceivedReportViewModel1.FirstOrDefault(x => x.WarehouseID == item.WarehouseID)?.Franchises;
                    }
                }


                obj.lInvoicesReceivedReportViewModel = reportList;
            }

            obj.lInvoicesReceivedReportViewModel = reportList.ToList();

                obj.lInvoicesReceivedReportViewModel = reportList.ToList();
            var CGSTAmount1 = reportList.Select(x => x.CGSTAmount).ToList();
            var SGSTAmount1 = reportList.Select(y => y.SGSTAmount).ToList();
            var quantity = reportList.Select(z => z.Quantity).ToList();
            foreach (InvoicesReceivedReportViewModel item in reportList)
            {
                decimal? CGSTAmount = item.CGSTAmount * Convert.ToDecimal(item.Quantity);
                decimal? SGSTAmount = item.SGSTAmount * Convert.ToDecimal(item.Quantity);
                decimal? ProductAmount = item.Quantity * item.UnitPrice;
                decimal? TaxableValue = (ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
                TaxableValue = Math.Round((decimal)TaxableValue, 2);
                decimal? GSTAmount = ProductAmount - TaxableValue;
                item.SGSTAmount = GSTAmount / 2;
                item.CGSTAmount = GSTAmount / 2;
            }

            //DateTime fDate2 = Convert.ToDateTime(obj.FromDate);
            //DateTime tDate2 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);

            //var result = obj.lInvoicesReceivedReportViewModel.Where(x => x.CreateDate >= fDate2 &&
            //                      x.CreateDate <= tDate2).ToList();
            //obj.lInvoicesReceivedReportViewModel = result.ToList();
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

              

             //   var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            

                //obj.WarehouseID = obj.WarehouseID;

                //obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
               

         
                //var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                var reportList = db.Database.SqlQuery<InvoicesReceivedReportViewModel>(
 "exec dbo.[NewInvoicesReceived] @FromDate,@ToDate",
 new Object[] {
                 // new SqlParameter("@WarehouseID", SupplierID),
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
                obj.lInvoicesReceivedReportViewModel = result.ToList();
                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));

                    dt.Columns.Add("SKUName", typeof(string));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("HSN Code", typeof(string));

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
                    dt.Columns.Add("Entity", typeof(string));
                    dt.Columns.Add("City", typeof(string));
                    dt.Columns.Add("Franchise", typeof(string));
                    //dt.Columns.Add("QtyReceived", typeof(int));
                    //dt.Columns.Add("Batch_Code", typeof(string));

                    int i = 0;
                    foreach (var row in obj.lInvoicesReceivedReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUName, row.SKUID, row.HSNCode,row.InvoiceCode,row.CreateDate,row.UnitPrice,
                      row.MRP, row.Amount,row.GSTInPer,row.CGSTAmount,row.SGSTAmount, row.Quantity, row.Supplier,row.Status,row.PONumber,row.Entity,row.City,row.Franchises}, false);
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