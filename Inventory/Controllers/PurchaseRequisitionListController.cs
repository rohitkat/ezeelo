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

namespace Inventory.Controllers
{
    public class PurchaseRequisitionListController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        //
        // GET: /PurchaseRequisitionList/
        public ActionResult GetPurchaseRequisitionList(PurchaseRequisitionListReportViewModelList obj1)
        {
            PurchaseRequisitionListReportViewModelList obj = new PurchaseRequisitionListReportViewModelList();
            List<PurchaseRequisitionListReportViewModel> lPurchaseRequisitionListReportViewModel = new List<PurchaseRequisitionListReportViewModel>();


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


            //          var reportList = db.Database.SqlQuery<PurchaseRequisitionListReportViewModel>(
            //"exec dbo.[PurchaseRequisitionListReport] @WarehouseID,@FromDate,@ToDate",
            //new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
            //          new SqlParameter("@FromDate", fDate),
            //         new SqlParameter("@ToDate", tDate)}
            //   ).ToList();



            //string str = string.Join(",", db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == 4)
            //                                 .Select(p => p.ProductNickname.ToString()));

            if (WarehouseID > 0)
            {
                lPurchaseRequisitionListReportViewModel = (from o in db.Quotations
                                                           join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                                           join qd in db.QuotationItemDetails on o.ID equals qd.QuotationID
                                                           join p in db.Products on qd.ProductID equals p.ID
                                                           join v in db.ProductVarients on qd.ProductVarientID equals v.ID
                                                           join s in db.Sizes on v.SizeID equals s.ID
                                                           join b in db.Brands on p.BrandID equals b.ID
                                                           join sda in db.QuotationSupplierLists on o.ID equals sda.QuotationID   ////sigle join  
                                                           join sup in db.Suppliers on sda.SupplierID equals sup.ID
                                                           join rt in db.RateCalculations on p.ID equals rt.ID
                                                           join wrl in db.WarehouseReorderLevels on

                                                           new { wid = w.ID, pid = qd.ProductID, vid = qd.ProductVarientID } equals
                                                           new { wid = wrl.WarehouseID, pid = wrl.ProductID, vid = wrl.ProductVarientID }

                                                           join ws in db.WarehouseStocks on
                                                          new { wId = w.ID, PID = qd.ProductID, PVId = qd.ProductVarientID } equals
                                                         new { wId = ws.WarehouseID, PID = ws.ProductID, PVId = ws.ProductVarientID }   ////multiple join for warehousse to product and prod varient 

                                                           where w.ID == WarehouseID

                                                           select new PurchaseRequisitionListReportViewModel
                                                           {
                                                               // QuotationID = o.ID,
                                                               // WarehouseName = w.Name,

                                                               QuotationCode = o.QuotationCode,
                                                               IsSent = o.IsSent,
                                                               SKUName = p.Name,
                                                               SKUID = v.ID,
                                                               BatchCode = ws.BatchCode,
                                                               MRP = rt.MRP,
                                                               SalePrice = rt.DecidedSalePrice,
                                                               HSNCode = p.HSNCode,
                                                               SKUUnit = s.Name,
                                                               Manufacturer = b.Name,
                                                               Supplier = sup.Name,
                                                               ReorderHitDate = wrl.ReorderHitDate,
                                                               Quantity = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),
                                                               QuotationRequestDate = o.QuotationRequestDate,
                                                               QuotationDate = o.CreateDate,

                                                               LastPurchaseQuantity = (from pr in db.PurchaseOrders
                                                                                       join pd in db.PurchaseOrderDetails on pr.ID equals pd.PurchaseOrderID
                                                                                       where pd.ProductID == qd.ProductID && pd.ProductVarientID == qd.ProductVarientID
                                                                                       select new { pd.Quantity, pr.CreateDate })
                                                                                      .OrderByDescending(x => x.CreateDate)
                                                                                      .Take(1)
                                                                                      .Select(x => (int)x.Quantity).FirstOrDefault(),
                                                               //LastPurchaseQuantity = db.PurchaseOrderDetails.Where(x => x.ProductID == qd.ProductID && x.ProductVarientID == qd.ProductVarientID).OrderByDescending(x => x.createDATE).Select(x => x.Quantity).SingleOrDefault(),
                                                               ExpectedReplyDate = o.ExpectedReplyDate
                                                           }).OrderByDescending(o => o.QuotationRequestDate).Distinct().ToList();

            }
            else
            {
                lPurchaseRequisitionListReportViewModel = (from o in db.Quotations
                                                           join w in db.Warehouses on o.RequestFromWarehouseID equals w.ID
                                                           join qd in db.QuotationItemDetails on o.ID equals qd.QuotationID
                                                           join p in db.Products on qd.ProductID equals p.ID
                                                           join v in db.ProductVarients on qd.ProductVarientID equals v.ID
                                                           join s in db.Sizes on v.SizeID equals s.ID
                                                           join b in db.Brands on p.BrandID equals b.ID
                                                           join sda in db.QuotationSupplierLists on o.ID equals sda.QuotationID
                                                           join sup in db.Suppliers on sda.SupplierID equals sup.ID
                                                           join rt in db.RateCalculations on p.ID equals rt.ID
                                                           join wrl in db.WarehouseReorderLevels on

                                                           new { wid = w.ID, pid = qd.ProductID, vid = qd.ProductVarientID } equals
                                                           new { wid = wrl.WarehouseID, pid = wrl.ProductID, vid = wrl.ProductVarientID }
                                                           join ws in db.WarehouseStocks on
                                                          new { wId = w.ID, PID = qd.ProductID, PVId = qd.ProductVarientID } equals
                                                         new { wId = ws.WarehouseID, PID = ws.ProductID, PVId = ws.ProductVarientID }
                                                           select new PurchaseRequisitionListReportViewModel
                                                           {
                                                               //QuotationID = o.ID,
                                                               // WarehouseName = w.Name,
                                                               QuotationCode = o.QuotationCode,
                                                               IsSent = o.IsSent,
                                                               SKUName = p.Name,
                                                               SKUID = v.ID,
                                                               HSNCode = p.HSNCode,
                                                               SKUUnit = s.Name,
                                                               MRP = rt.MRP,
                                                               SalePrice = rt.DecidedSalePrice,
                                                               ReorderHitDate = wrl.ReorderHitDate,
                                                               BatchCode = ws.BatchCode,
                                                               Manufacturer = b.Name,
                                                               Supplier = sup.Name,
                                                               QuotationDate = o.CreateDate,
                                                               LastPurchaseQuantity = (from pr in db.PurchaseOrders
                                                                                       join pd in db.PurchaseOrderDetails on pr.ID equals pd.PurchaseOrderID
                                                                                       where pd.ProductID == qd.ProductID && pd.ProductVarientID == qd.ProductVarientID
                                                                                       select new { pd.Quantity, pr.CreateDate })
                                                                                      .OrderByDescending(x => x.CreateDate)
                                                                                      .Take(1)
                                                                                      .Select(x => (int)x.Quantity).FirstOrDefault(),
                                                               //  OutOfStockDate= db.CustomerOrderDetails.Where(x=>x.WarehouseStockID)
                                                               Quantity = db.QuotationItemDetails.Where(x => x.QuotationID == o.ID).Select(x => x.ID).Count(),

                                                           }).OrderByDescending(o => o.QuotationRequestDate).Distinct().ToList();

            }
            obj.lPurchaseRequisitionListReportViewModel = lPurchaseRequisitionListReportViewModel.ToList();

            if (obj1.WarehouseID != 0)
            {
                obj.lPurchaseRequisitionListReportViewModel = lPurchaseRequisitionListReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
            }


            if (obj1.FromDate == null && obj1.ToDate == null)
            {
                if (obj1.WarehouseID != 0)
                {
                    obj.lPurchaseRequisitionListReportViewModel = lPurchaseRequisitionListReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }
            }
            else
            {
                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59);


                var result = obj.lPurchaseRequisitionListReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lPurchaseRequisitionListReportViewModel = result.ToList();

            }
            ViewBag.PossibleWarehouses = Obj_Common.GetFVList(WarehouseID);


            return View(obj);
        }


        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID) //, int? print
        {
            try
            {
                PurchaseRequisitionListReportViewModelList obj = new PurchaseRequisitionListReportViewModelList();
                List<PurchaseRequisitionListReportViewModel> list = new List<PurchaseRequisitionListReportViewModel>();


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

                obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();



                var reportList = db.Database.SqlQuery<PurchaseRequisitionListReportViewModel>(
      "exec dbo.[PurchaseRequisitionListReport] @WarehouseID,@FromDate,@ToDate",
      new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate)}
         ).ToList();


                obj.lPurchaseRequisitionListReportViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lPurchaseRequisitionListReportViewModel = obj.lPurchaseRequisitionListReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);


                var result = obj.lPurchaseRequisitionListReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                      x.CreateDate <= tDate1).ToList();
                obj.lPurchaseRequisitionListReportViewModel = result.ToList();


                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("SKUName", typeof(string));
                    dt.Columns.Add("SKUUnit", typeof(string));

                    dt.Columns.Add("HSN Code", typeof(string));
                    dt.Columns.Add("Brand Name", typeof(string));
                    //dt.Columns.Add("ProductVarient", typeof(string));
                    dt.Columns.Add("Supplier", typeof(string));
                    dt.Columns.Add("Quantity", typeof(int));

                    dt.Columns.Add("QuotationDate", typeof(DateTime));
                    dt.Columns.Add("Amount", typeof(Double));
                    dt.Columns.Add("LastPurchaseQuantity", typeof(int));
                    dt.Columns.Add("MRP", typeof(Double));
                    dt.Columns.Add("SalePrice", typeof(Double));
                    dt.Columns.Add("ReorderHitDate", typeof(DateTime));






                    int i = 0;
                    foreach (var row in obj.lPurchaseRequisitionListReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUID, row.SKUName, row.SKUUnit, row.HSNCode,row.Manufacturer,row.Supplier ,
                       row.Quantity, row.QuotationDate,row.Amount,row.LastPurchaseQuantity,row.MRP,row.SalePrice,row.ReorderHitDate}, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetPurchaseRequisitionList");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetPurchaseRequisitionList");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetPurchaseRequisitionList");
                    }
                }
                else
                {
                    return View("GetPurchaseRequisitionList", obj);
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