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

namespace Inventory.Controllers
{
    public class ReturnReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();




        // GET: /ReturnReport/

        public ActionResult GetReturnItemReport(ReturnReportViewModelList obj1)
        {
            ReturnReportViewModelList obj = new ReturnReportViewModelList();
            List<ReturnReportViewModel> list = new List<ReturnReportViewModel>();

            if (Session["USER_NAME"] != null)
            { }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
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

            //DateTime fDate = Convert.ToDateTime(obj1.FromDate);
            //DateTime tDate = Convert.ToDateTime(obj1.ToDate);


            obj.WarehouseID = obj1.WarehouseID;

            //if (obj.WarehouseID > 0)
            //{
            //    list = GetRecord(fDate, tDate, obj.WarehouseID);
            //}
            //if (string.IsNullOrEmpty(obj.ToDate))
            //{
            //    obj.WarehouseID = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
            //}
            //else

            obj1.WarehouseID = obj1.WarehouseID;
            //obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");

            //var list2 = (from r in db.WarehouseReturnStockDetails
            //             join ws in db.WarehouseStocks on r.WarehouseStockId equals ws.ID
            //             join POD in db.Products on ws.ProductID equals POD.ID
            //             join v in db.ProductVarients on POD.ID equals v.ProductID
            //             join W in db.Warehouses on ws.WarehouseID equals W.ID
            //             join R in db.WarehouseReturnStock on r.WarehouseStockId equals R.ID

            //             join po in db.PurchaseOrders on ws.WarehouseID equals po.WarehouseID
            //             join s in db.Suppliers on po.SupplierID equals s.ID
            //             join z in db.Sizes on v.SizeID equals z.ID
            //             select new ReturnReportViewModel
            //             {

            //                 ItemName = POD.Name + " (" + z.Name + ")",
            //                 HSNCode = POD.HSNCode,
            //                 Quantity = r.Quantity,
            //                 Amount = r.Quantity * r.ReturnRatePerUnit,
            //                 Manufacturer = db.Brands.FirstOrDefault(b => b.ID == POD.BrandID).Name,
            //                 batch_code = ws.BatchCode,
            //                 SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == r.SubReasonId)).Reason,
            //                 Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == r.SubReasonId)).ParentReasonId))).Reason,
            //                 BatchAvlQty = ws.AvailableQuantity,
            //                 SupplierName = s.Name,
            //                 CreateDate = R.CreateDate,

            //                 WarehouseID =ws.WarehouseID,

            //             }).ToList();





         //   var ListRep = db.WarehouseReturnStockDetails
         //.Join(db.WarehouseStocks, wrs => wrs.WarehouseStockId, ws => ws.ID, (wrs, ws) => new { wrs, ws })
         //.Join(db.Products, wrs => wrs.ws.ProductID, p => p.ID, (wrs, p) => new { wrs, p })
         
         //.Select(o => new
         //{
         //    item_name = o.p.Name,
         //    ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
         //        (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wrs.ws.ProductVarientID).SizeID)).Name,
         //    HSNCode = o.p.HSNCode,
         //    Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.p.BrandID).Name,
         //    SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
         //    (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
         //    (db.Invoices.FirstOrDefault(ss => ss.ID == o.wrs.ws.InvoiceID)).PurchaseOrderID
         //    )).SupplierID).Name,
         //    batch_code = o.wrs.ws.BatchCode,
         //    //waste_qty = o.wrs.wrs.WastageQuantity,
         //    //WasteAmt = o.wrs.wrs.WastageQuantity * o.wrs.ws.BuyRatePerUnit,
         //    SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.SubReasonId)).Reason,
         //    Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wrs.wrs.SubReasonId)).ParentReasonId))).Reason,
         //    Remark = o.wrs.wrs.Remark,
         //    WarehouseID = o.wrs.ws.WarehouseID,
         //    CreateDate = o.wrs.wrs.CreateDate
         //})
         //.ToList();

         //   obj.lReturnReportViewModel = ListRep.ToList().Select(p => new ReturnReportViewModel
         //   {
         //       item_name = p.item_name,
         //       ProductVarient = p.ProductVarient,
         //       HSNCode = p.HSNCode,
         //       Manufacturer = p.Manufacturer,
         //       SupplierName = p.SupplierName,
         //       batch_code = p.batch_code,
         //       //waste_qty = p.waste_qty,
         //       //WasteAmt = p.WasteAmt,
         //       SubReason = p.SubReason,
         //       Reason = p.Reason,
         //       Remark = p.Remark,
         //       WarehouseID = p.WarehouseID,
         //       //CreateDate = p.CreateDate,

         //   }).ToList();









            //Comment by Priti  without Invoice Code OnActionExecuted 15-11-2018
   //          var ListRep = db.WarehouseReturnStockDetails
   //           .Join(db.WarehouseStocks, wrs => wrs.WarehouseStockId, ws => ws.ID, (wrs, ws) => new { wrs, ws })
   //           .Join(db.Products, wrs => wrs.ws.ProductID, p => p.ID, (wrs, p) => new { wrs, p })
   //           .Join (db.WarehouseReturnStock,wrs=>wrs.wrs.wrs.WarehouseReturnStockId, rs=>rs.ID, (wrs,rs)=>new{wrs,rs})
   //           .Join(db.ProductVarients, wrs => wrs.wrs.wrs.ws.ProductVarientID, pv => pv.ID, (wrs, pv) => new { wrs ,pv})

   //             .Select(o => new
   //      {
   //        //item_name = o.wrs.p.Name,
   //          item_name =o.wrs.wrs.p.Name,
   //          //SKUID=o.pv.ID,
   //          SKUID=o.pv.ID,
   //      //ProductVarient= o.wrs.p.HSNCode,
   //          ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
   //                         (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wrs.wrs.wrs.ws.ProductVarientID).SizeID)).Name,
   //     Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.wrs.wrs.p.BrandID).Name,
   //          SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
   //                       (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
   //                       (db.Invoices.FirstOrDefault(ss => ss.ID == o.wrs.wrs.wrs.ws.InvoiceID)).PurchaseOrderID
   //                       )).SupplierID).Name,

   //          //batch_code = o.wrs.wrs.ws.BatchCode,
   //          batch_code=o.wrs.wrs.wrs.ws.BatchCode,
   //          //Quantity =o.wrs.wrs.wrs.Quantity,
   //          Quantity=o.wrs.wrs.wrs.wrs.Quantity,
   //          //Amount = o.wrs.wrs.wrs.Quantity * o.wrs.wrs.ws.BuyRatePerUnit,
   //          Amount=o.wrs.wrs.wrs.wrs.Quantity*o.wrs.wrs.wrs.ws.BuyRatePerUnit,
   //          //SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.SubReasonId)).Reason,
   //         SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.wrs.SubReasonId)).Reason,
   //          Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID ==o.wrs.wrs.wrs.wrs.SubReasonId)).ParentReasonId))).Reason,
   //          Remark = o.wrs.wrs.wrs.wrs.Remark,
   //          //WarehouseID = o.wrs.wrs.ws.WarehouseID,
   //        WarehouseID = o.wrs.wrs.wrs.ws.WarehouseID,
   //          //CreateDate = o.rs.CreateDate,
   //          CreateDate = o.wrs.rs.CreateDate,
   //          //HSNCode = o.wrs.p.HSNCode,
   //HSNCode=o.wrs.wrs.p.HSNCode,
   //          ReturnRatePerUnit=o.wrs.wrs.wrs.wrs.ReturnRatePerUnit

   //      })
   //      .ToList();

   //          obj.lReturnReportViewModel = ListRep.ToList().Select(p => new ReturnReportViewModel
   //         {
   //             item_name = p.item_name  ,
   //             SKUID=p.SKUID,
   //             ProductVarient = p.ProductVarient,
   //             HSNCode = p.HSNCode,
   //             Manufacturer = p.Manufacturer,
   //             SupplierName = p.SupplierName,
   //             batch_code = p.batch_code,
   //             Quantity = p.Quantity,
   //            ReturnRatePerUnit=p.ReturnRatePerUnit,
   //             Amount = p.Amount,
   //             SubReason = p.SubReason,
   //             Reason = p.Reason,
   //             Remark = p.Remark,
   //             WarehouseID = p.WarehouseID,
   //             CreateDate = p.CreateDate,
   //         }).ToList(); 







              /// New Query for Return report by adding Invoice code 
              /// 

            var ListRep = db.WarehouseReturnStockDetails.Where(wrs => wrs.Quantity != 0)       //Added by Priti on Remove 0 Quantity from ReturnReport on 15-11-2018
             .Join(db.WarehouseStocks, wrs => wrs.WarehouseStockId, ws => ws.ID, (wrs, ws) => new { wrs, ws })
             .Join(db.Invoices, ws=> ws.ws.InvoiceID, i=> i.ID,(ws,i)=> new { ws,i})

             .Join(db.Products, wrs=>wrs.ws.ws.ProductID, p => p.ID, (wrs, p) => new { wrs, p })
             .Join(db.WarehouseReturnStock, wrs => wrs.wrs.ws.wrs.WarehouseReturnStockId, rs => rs.ID, (wrs, rs) => new { wrs, rs })
             .Join(db.ProductVarients, wrs =>wrs.wrs.wrs.ws.ws.ProductVarientID, pv => pv.ID, (wrs, pv) => new { wrs, pv })

               .Select(o => new
               {
                   //item_name = o.wrs.p.Name,
                   item_name = o.wrs.wrs.p.Name,
                   //SKUID=o.pv.ID,
                   SKUID = o.pv.ID,
                    InvoiceCode= o.wrs.wrs.wrs.i.InvoiceCode,
                   //ProductVarient= o.wrs.p.HSNCode,
                   ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
                                  (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wrs.wrs.wrs.ws.ws.ProductVarientID).SizeID)).Name,
                   Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.wrs.wrs.p.BrandID).Name,
                   SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
                                (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
                                (db.Invoices.FirstOrDefault(ss => ss.ID == o.wrs.wrs.wrs.ws.ws.InvoiceID)).PurchaseOrderID
                                )).SupplierID).Name,

                   //batch_code = o.wrs.wrs.ws.BatchCode,
                   batch_code = o.wrs.wrs.wrs.ws.ws.BatchCode,
                   //Quantity =o.wrs.wrs.wrs.Quantity,
                   Quantity = o.wrs.wrs.wrs.ws.wrs.Quantity,
                   //Amount = o.wrs.wrs.wrs.Quantity * o.wrs.wrs.ws.BuyRatePerUnit,
                   Amount = o.wrs.wrs.wrs.ws.wrs.Quantity * o.wrs.wrs.wrs.ws.ws.BuyRatePerUnit,
                   //SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.SubReasonId)).Reason,
                   SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.ws.wrs.SubReasonId)).Reason,
                   Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wrs.wrs.wrs.ws.wrs.SubReasonId)).ParentReasonId))).Reason,
                   Remark = o.wrs.wrs.wrs.ws.wrs.Remark,
                   //WarehouseID = o.wrs.wrs.ws.WarehouseID,
                   WarehouseID = o.wrs.wrs.wrs.ws.ws.WarehouseID,
                   //CreateDate = o.rs.CreateDate,
                   CreateDate = o.wrs.rs.CreateDate,
                   //HSNCode = o.wrs.p.HSNCode,
                   HSNCode = o.wrs.wrs.p.HSNCode,
                   ReturnRatePerUnit = o.wrs.wrs.wrs.ws.ws.BuyRatePerUnit

               })
        .ToList();

             obj.lReturnReportViewModel = ListRep.ToList().Select(p => new ReturnReportViewModel
             {
                 item_name = p.item_name,
                 SKUID = p.SKUID,
               InvoiceCode =p.InvoiceCode,
                 ProductVarient = p.ProductVarient,
                 HSNCode = p.HSNCode,
                 Manufacturer = p.Manufacturer,
                 SupplierName = p.SupplierName,
                 batch_code = p.batch_code,
                 Quantity = p.Quantity,
                 ReturnRatePerUnit = p.ReturnRatePerUnit,
                 Amount = p.Amount,
                 SubReason = p.SubReason,
                 Reason = p.Reason,
                 Remark = p.Remark,
                 WarehouseID = p.WarehouseID,
                 CreateDate = p.CreateDate,
                 GST = (db.RateMatrix.FirstOrDefault(r => r.ProductVarientId == p.SKUID) == null) ? (db.RateCalculations.FirstOrDefault(r => r.ProductVarientId == p.SKUID) == null) ? 0 : db.RateCalculations.FirstOrDefault(r => r.ProductVarientId == p.SKUID).GSTInPer : db.RateMatrix.FirstOrDefault(r => r.ProductVarientId == p.SKUID).GSTInPer,

             }).ToList(); 











         
          
            if (obj1.WarehouseID != null)
            {
                obj.lReturnReportViewModel = obj.lReturnReportViewModel.Where(p => p.WarehouseID == obj1.WarehouseID).ToList();
            }
        
             if (string.IsNullOrEmpty(obj1.FromDate))
            {
                obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            }
            else
            {
                obj1.FromDate = obj1.FromDate;
            }
            if (string.IsNullOrEmpty(obj1.ToDate))
            {
                obj.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
            }
            else
            {
                obj.ToDate = obj1.ToDate;
            }

            //DateTime FromDate = DateTime.Now; // Date and time of today


            //DateTime ToDate = DateTime.Now.Date; // Beginning of today


            //DateTime FromDate= DateTime.Now.AddDays(1).Date;
            //////or you can do this to get 11:59:59 of the current day
            //DateTime ToDate = DateTime.Now.AddDays(1).Date.AddSeconds(-1);
    
            DateTime fDate1 = Convert.ToDateTime(obj1.FromDate);
            DateTime tDate1 = Convert.ToDateTime(obj1.ToDate).AddHours(23);



       

             var result = obj.lReturnReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                        x.CreateDate <= tDate1).ToList();

           


            obj.lReturnReportViewModel = result.ToList();


            return View(obj);
        }
        
        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID) //, int? print
        {
            try
            {
                ReturnReportViewModelList obj = new ReturnReportViewModelList();
                List<ReturnReportViewModel> list = new List<ReturnReportViewModel>();

                // long WarehouseID = 0;
                // if (Session["WarehouseID"] != null)
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
                DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23);

                var fdate = Convert.ToDateTime("06-13-2012");
                //obj.lWastageReportViewModel = GetRecord(fDate, tDate,1,10).ToList();
                List<ReturnReportViewModel> list123 = GetRecord(fDate, tDate, option, WarehouseID).ToList();
              

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
             
               // ViewBag.PossibleSuppliers = new SelectList(db.Warehouses.ToList(), "ID", "Name");
                obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
             
                obj.lReturnReportViewModel = list123.ToList();
                if (obj.WarehouseID != null)
                {
                    obj.lReturnReportViewModel = obj.lReturnReportViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }
                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);
                var result = obj.lReturnReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                           x.CreateDate <= tDate1).ToList();




                obj.lReturnReportViewModel = result.ToList();

                if (option != null && option > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Sr.No.", typeof(long));
                    dt.Columns.Add("HSNCode", typeof(string));
                    dt.Columns.Add("SKU Name", typeof(string));
                    dt.Columns.Add("SKUID", typeof(long));
                    dt.Columns.Add("Manufacturer", typeof(string));
                    dt.Columns.Add("ProductVarient", typeof(string));
                    dt.Columns.Add("SupplierName", typeof(string));
                    //dt.Columns.Add("waste_qty", typeof(int));
                    dt.Columns.Add("Reason", typeof(string));
                    dt.Columns.Add("SubReason", typeof(string));
                    dt.Columns.Add("Remark", typeof(string));
                    //dt.Columns.Add("QtyReceived", typeof(int));
                    dt.Columns.Add("Amount", typeof(decimal));
                    dt.Columns.Add("Batch_Code", typeof(string));
                    dt.Columns.Add("Invoice Code", typeof(string));
                    dt.Columns.Add("ReturnDate", typeof(DateTime));
                    dt.Columns.Add("GST", typeof(DateTime));




                    int i = 0;
                    foreach (var row in obj.lReturnReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.HSNCode, row.item_name,row.SKUID,row.Manufacturer,row.ProductVarient ,
                       row.SupplierName,row.Reason,row.SubReason,row.Remark,row.Amount,row.batch_code, row.InvoiceCode,row.CreateDate,row.GST }, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetReturnItemReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetReturnItemReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetReturnItemReport");
                    }
                }
                else
                {
                    return RedirectToAction("GetReturnItemReport", obj);
                }

            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }

        public List<ReturnReportViewModel> GetRecord(DateTime fDate, DateTime tDate, int option, long WarehouseID)
        {

            List<ReturnReportViewModel> obj = new List<ReturnReportViewModel>();

            //   var ListRep = db.WarehouseReturnStockDetails
            //     .Join(db.WarehouseStocks, wrs => wrs.WarehouseStockId, ws => ws.ID, (wrs, ws) => new { wrs, ws })
            //     .Join(db.Products, wrs => wrs.ws.ProductID, p => p.ID, (wrs, p) => new { wrs, p })
            //     .Join(db.WarehouseReturnStock, wrs => wrs.wrs.wrs.WarehouseReturnStockId, rs => rs.ID, (wrs, rs) => new { wrs, rs }
            //     )
            //       .Select(o => new
            //       {
            //           item_name = o.wrs.p.Name,
            //           SKUID=o.wrs.p.ID,
            //           ProductVarient = o.wrs.p.HSNCode,
            //           Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.wrs.p.BrandID).Name,
            //           SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
            //                        (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
            //                        (db.Invoices.FirstOrDefault(ss => ss.ID == o.wrs.wrs.ws.InvoiceID)).PurchaseOrderID
            //                        )).SupplierID).Name,

            //           batch_code = o.wrs.wrs.ws.BatchCode,
            //           Quantity = o.wrs.wrs.wrs.Quantity,
            //           Amount = o.wrs.wrs.wrs.Quantity * o.wrs.wrs.ws.BuyRatePerUnit,
            //           SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.SubReasonId)).Reason,
            //           Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wrs.wrs.wrs.SubReasonId)).ParentReasonId))).Reason,
            //           Remark = o.rs.Remark,
            //           WarehouseID = o.wrs.wrs.ws.WarehouseID,
            //           CreateDate = o.rs.CreateDate,
            //           HSNCode = o.wrs.p.HSNCode,
            //           ReturnRatePerUnit = o.wrs.wrs.ws.BuyRatePerUnit
            //       })
            //.ToList();

            //   obj = ListRep.ToList().Select(p => new ReturnReportViewModel
            //   {
            //       item_name = p.item_name,
            //       SKUID=p.SKUID,
            //       ProductVarient = p.ProductVarient,
            //       HSNCode = p.HSNCode,
            //       Manufacturer = p.Manufacturer,
            //       SupplierName = p.SupplierName,
            //       batch_code = p.batch_code,
            //       Quantity = p.Quantity,
            //       ReturnRatePerUnit = p.ReturnRatePerUnit,
            //       Amount = p.Amount,
            //       SubReason = p.SubReason,
            //       Reason = p.Reason,
            //       Remark = p.Remark,
            //       WarehouseID = p.WarehouseID,
            //       CreateDate = p.CreateDate,

            //   }).Where(X => X.WarehouseID == WarehouseID).ToList();





            //Comment by Priti  without Invoice Code OnActionExecuted 15-11-2018

            //       var ListRep = db.WarehouseReturnStockDetails
            //     .Join(db.WarehouseStocks, wrs => wrs.WarehouseStockId, ws => ws.ID, (wrs, ws) => new { wrs, ws })
            //     //.Join(db.Invoices,wrs=>wrs.ws.InvoiceID, i=>i.ID,( ws,i)=>new { ws,i})
            //     .Join(db.Products, wrs => wrs.ws.ProductID, p => p.ID, (wrs, p) => new { wrs, p })
            //     .Join(db.WarehouseReturnStock, wrs => wrs.wrs.wrs.WarehouseReturnStockId, rs => rs.ID, (wrs, rs) => new { wrs, rs })
            //     .Join(db.ProductVarients, wrs => wrs.wrs.wrs.ws.ProductVarientID, pv => pv.ID, (wrs, pv) => new { wrs, pv })

            //       .Select(o => new
            //       {
            //           //item_name = o.wrs.p.Name,
            //           item_name = o.wrs.wrs.p.Name,
            //           //SKUID=o.pv.ID,
            //           SKUID = o.pv.ID,
            //           //ProductVarient= o.wrs.p.HSNCode,
            //           ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
            //                          (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wrs.wrs.wrs.ws.ProductVarientID).SizeID)).Name,
            //           Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.wrs.wrs.p.BrandID).Name,
            //           SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
            //                        (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
            //                        (db.Invoices.FirstOrDefault(ss => ss.ID == o.wrs.wrs.wrs.ws.InvoiceID)).PurchaseOrderID
            //                        )).SupplierID).Name,

            //           //batch_code = o.wrs.wrs.ws.BatchCode,
            //           batch_code = o.wrs.wrs.wrs.ws.BatchCode,
            //           //Quantity =o.wrs.wrs.wrs.Quantity,
            //           Quantity = o.wrs.wrs.wrs.wrs.Quantity,
            //           //Amount = o.wrs.wrs.wrs.Quantity * o.wrs.wrs.ws.BuyRatePerUnit,
            //           Amount = o.wrs.wrs.wrs.wrs.Quantity * o.wrs.wrs.wrs.ws.BuyRatePerUnit,
            //           //SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.SubReasonId)).Reason,
            //           SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.wrs.SubReasonId)).Reason,
            //           Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wrs.wrs.wrs.wrs.SubReasonId)).ParentReasonId))).Reason,
            //           Remark = o.wrs.wrs.wrs.wrs.Remark,
            //           //WarehouseID = o.wrs.wrs.ws.WarehouseID,
            //           WarehouseID = o.wrs.wrs.wrs.ws.WarehouseID,
            //           //CreateDate = o.rs.CreateDate,
            //           CreateDate = o.wrs.rs.CreateDate,
            //           //HSNCode = o.wrs.p.HSNCode,
            //           HSNCode = o.wrs.wrs.p.HSNCode,
            //           ReturnRatePerUnit = o.wrs.wrs.wrs.wrs.ReturnRatePerUnit

            //       })
            //.ToList();

            //       obj = ListRep.ToList().Select(p => new ReturnReportViewModel
            //       {
            //           item_name = p.item_name,
            //           SKUID = p.SKUID,
            //           ProductVarient = p.ProductVarient,
            //           HSNCode = p.HSNCode,
            //           Manufacturer = p.Manufacturer,
            //           SupplierName = p.SupplierName,
            //           batch_code = p.batch_code,
            //           Quantity = p.Quantity,
            //           ReturnRatePerUnit = p.ReturnRatePerUnit,
            //           Amount = p.Amount,
            //           SubReason = p.SubReason,
            //           Reason = p.Reason,
            //           Remark = p.Remark,
            //           WarehouseID = p.WarehouseID,
            //           CreateDate = p.CreateDate,
            //       }).ToList();



            

            /// New Query for Return report by adding Invoice code 
            /// 

            var ListRep = db.WarehouseReturnStockDetails.Where(wrs => wrs.Quantity != 0)       //Added by Priti on Remove 0 Quantity from ReturnReport on 15-11-2018
              .Join(db.WarehouseStocks, wrs => wrs.WarehouseStockId, ws => ws.ID, (wrs, ws) => new { wrs, ws })
              .Join(db.Invoices, ws => ws.ws.InvoiceID, i => i.ID, (ws, i) => new { ws, i })

              .Join(db.Products, wrs => wrs.ws.ws.ProductID, p => p.ID, (wrs, p) => new { wrs, p })
              .Join(db.WarehouseReturnStock, wrs => wrs.wrs.ws.wrs.WarehouseReturnStockId, rs => rs.ID, (wrs, rs) => new { wrs, rs })
              .Join(db.ProductVarients, wrs => wrs.wrs.wrs.ws.ws.ProductVarientID, pv => pv.ID, (wrs, pv) => new { wrs, pv })

                .Select(o => new
                {
                   //item_name = o.wrs.p.Name,
                   item_name = o.wrs.wrs.p.Name,
                   //SKUID=o.pv.ID,
                   SKUID = o.pv.ID,
                    InvoiceCode = o.wrs.wrs.wrs.i.InvoiceCode,
                   //ProductVarient= o.wrs.p.HSNCode,
                   ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
                                   (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wrs.wrs.wrs.ws.ws.ProductVarientID).SizeID)).Name,
                    Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.wrs.wrs.p.BrandID).Name,
                    SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
                                 (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
                                 (db.Invoices.FirstOrDefault(ss => ss.ID == o.wrs.wrs.wrs.ws.ws.InvoiceID)).PurchaseOrderID
                                 )).SupplierID).Name,

                   //batch_code = o.wrs.wrs.ws.BatchCode,
                   batch_code = o.wrs.wrs.wrs.ws.ws.BatchCode,
                   //Quantity =o.wrs.wrs.wrs.Quantity,
                   Quantity = o.wrs.wrs.wrs.ws.wrs.Quantity,
                   //Amount = o.wrs.wrs.wrs.Quantity * o.wrs.wrs.ws.BuyRatePerUnit,
                   Amount = o.wrs.wrs.wrs.ws.wrs.Quantity * o.wrs.wrs.wrs.ws.ws.BuyRatePerUnit,
                   //SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.SubReasonId)).Reason,
                   SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wrs.wrs.wrs.ws.wrs.SubReasonId)).Reason,
                    Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wrs.wrs.wrs.ws.wrs.SubReasonId)).ParentReasonId))).Reason,
                    Remark = o.wrs.wrs.wrs.ws.wrs.Remark,
                   //WarehouseID = o.wrs.wrs.ws.WarehouseID,
                   WarehouseID = o.wrs.wrs.wrs.ws.ws.WarehouseID,
                   //CreateDate = o.rs.CreateDate,
                   CreateDate = o.wrs.rs.CreateDate,
                   //HSNCode = o.wrs.p.HSNCode,
                   HSNCode = o.wrs.wrs.p.HSNCode,
                    ReturnRatePerUnit = o.wrs.wrs.wrs.ws.ws.BuyRatePerUnit

                })
         .ToList();

            obj = ListRep.ToList().Select(p => new ReturnReportViewModel
            {
                item_name = p.item_name,
                SKUID = p.SKUID,
                InvoiceCode = p.InvoiceCode,
                ProductVarient = p.ProductVarient,
                HSNCode = p.HSNCode,
                Manufacturer = p.Manufacturer,
                SupplierName = p.SupplierName,
                batch_code = p.batch_code,
                Quantity = p.Quantity,
                ReturnRatePerUnit = p.ReturnRatePerUnit,
                Amount = p.Amount,
                SubReason = p.SubReason,
                Reason = p.Reason,
                Remark = p.Remark,
                WarehouseID = p.WarehouseID,
                CreateDate = p.CreateDate,
                GST = (db.RateMatrix.FirstOrDefault(r => r.ProductVarientId == p.SKUID) == null) ? (db.RateCalculations.FirstOrDefault(r => r.ProductVarientId == p.SKUID) == null) ? 0 : db.RateCalculations.FirstOrDefault(r => r.ProductVarientId == p.SKUID).GSTInPer : db.RateMatrix.FirstOrDefault(r => r.ProductVarientId == p.SKUID).GSTInPer,
            }).ToList();

            
            return obj;

        }
        
    }
}