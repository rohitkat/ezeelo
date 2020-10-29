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
    public class WastageReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
       
        public ActionResult GetWastageItemReport(WastageReportViewModelList obj1)
        {
            WastageReportViewModelList obj = new WastageReportViewModelList();
            List<WastageReportViewModel> list = new List<WastageReportViewModel>();
            
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


            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
           // obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
            
            //ViewBag.PossibleSuppliers = new SelectList(db.Warehouses.ToList(), "ID", "Name");


            var ListRep = db.WarehouseWastageStock
                .Join(db.WarehouseStocks, wws => wws.WarehouseStockID, ws => ws.ID, (wws, ws) => new { wws, ws })
                .Join(db.Products, wss => wss.ws.ProductID, p => p.ID, (wss, p) => new { wss, p })
            
              
                .Select(o => new
                {
                    item_name = o.p.Name,
                
                    ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
                        (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wss.ws.ProductVarientID).SizeID)).Name,
                    HSNCode = o.p.HSNCode,
                    Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.p.BrandID).Name,
                    SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
                    (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
                    (db.Invoices.FirstOrDefault(ss => ss.ID == o.wss.ws.InvoiceID)).PurchaseOrderID
                    )).SupplierID).Name,
                    batch_code = o.wss.ws.BatchCode,
          SKUID=o.wss.ws.ProductVarientID,
                    waste_qty = o.wss.wws.WastageQuantity,
                    WasteAmt = o.wss.wws.WastageQuantity * o.wss.ws.BuyRatePerUnit,
                    SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wss.wws.SubReasonID)).Reason,
                    Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wss.wws.SubReasonID)).ParentReasonId))).Reason,
                    Remark = o.wss.wws.Remark,
                    WarehouseID = o.wss.ws.WarehouseID,
                    CreateDate = o.wss.wws.CreateDate
                })
                .ToList();

            obj.lWastageReportViewModel = ListRep.ToList().Select(p => new WastageReportViewModel
            {
                item_name = p.item_name  ,
                SKUID =p.SKUID,
                ProductVarient = p.ProductVarient,
                HSNCode = p.HSNCode,
                Manufacturer = p.Manufacturer,
                SupplierName = p.SupplierName,
                batch_code = p.batch_code,
                waste_qty = p.waste_qty,
                WasteAmt = p.WasteAmt,
                SubReason = p.SubReason,
                Reason = p.Reason,
                Remark = p.Remark,
                WarehouseID = p.WarehouseID,
                CreateDate = p.CreateDate,

            }).ToList();
            

            if (obj1.WarehouseID != null)
            {
                obj.lWastageReportViewModel= obj.lWastageReportViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
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
            DateTime tDate1 = Convert.ToDateTime(obj1.ToDate).AddHours(23).AddMinutes(59);





            var result = obj.lWastageReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                       x.CreateDate <= tDate1).ToList();




            obj.lWastageReportViewModel = result.ToList();


            return View(obj);
        }



        public ActionResult GetWastageItemReportPrint(WastageReportViewModelList obj1) //, int? print
        {
            try
            {
                WastageReportViewModelList obj = new WastageReportViewModelList();
                List<WastageReportViewModel> list = new List<WastageReportViewModel>();

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
                    obj1.ToDate = obj1.ToDate;
                }

                DateTime fDate = Convert.ToDateTime(obj1.FromDate);
                DateTime tDate = Convert.ToDateTime(obj1.ToDate);
                obj.WarehouseID = obj1.WarehouseID;
                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                obj.lWastageReportViewModel = list.ToList();

                return View("GetWastageItemReportPrint", obj1);
            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }




        public ActionResult Export(string FromDate, string ToDate, int option ,long WarehouseID) //, int? print
        {
            WastageReportViewModelList obj = new WastageReportViewModelList();
            try
            {
             
                List<WastageReportViewModel> list = new List<WastageReportViewModel>();



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
                List<WastageReportViewModel> list123 = GetRecord(fDate, tDate, option,WarehouseID).ToList();
           
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

                //DateTime fDate = Convert.ToDateTime(obj.FromDate);
                //DateTime tDate = Convert.ToDateTime(obj.ToDate);

                //if (WarehouseID > 0)
                //{
                //    list = GetRecord(fDate, tDate, WarehouseID);
                //}
                ViewBag.PossibleSuppliers = new SelectList(db.Warehouses.ToList(), "ID", "Name");
                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                obj.lWastageReportViewModel = list123.ToList();
                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23);
                var result = obj.lWastageReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                                           x.CreateDate <= tDate1).ToList();




                obj.lWastageReportViewModel = result.ToList();

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
                    dt.Columns.Add("Quantity", typeof(int));
                    dt.Columns.Add("Reason", typeof(string));
                    dt.Columns.Add("SubReason", typeof(string));
                    dt.Columns.Add("Remark", typeof(string));
                    //dt.Columns.Add("QtyReceived", typeof(int));
                    dt.Columns.Add("Amount", typeof(decimal));
                    dt.Columns.Add("Batch_Code", typeof(string));
                    dt.Columns.Add("WastageDate", typeof(DateTime));



                    int i = 0;
                    foreach (var row in obj.lWastageReportViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.HSNCode, row.item_name,row.SKUID,row.Manufacturer,row.ProductVarient ,
                       row.SupplierName, row.waste_qty,row.Reason,row.SubReason,row.Remark,row.WasteAmt,row.batch_code,row.CreateDate  }, false);
                    }

                    ExportExcelCsv ExportExcelCsv = new ExportExcelCsv(System.Web.HttpContext.Current.Server);
                    if (option == 1)
                    {
                        ExportExcelCsv.ExportToExcel(dt, "GetWastageItemReport");
                    }
                    else if (option == 2)
                    {
                        ExportExcelCsv.ExportToCSV(dt, "GetWastageItemReport");
                    }
                    else if (option == 3)
                    {
                        ExportExcelCsv.ExportToPDF(dt, "GetWastageItemReport");
                    }
                }
                else
                {
                    return RedirectToAction("GetWastageItemReport", obj);
                }

            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return RedirectToAction("GetWastageItemReport",obj);
        }



        public List<WastageReportViewModel> GetRecord(DateTime fDate, DateTime tDate, int option,long WarehouseID )
        {

            List<WastageReportViewModel> obj = new List<WastageReportViewModel>();

            var ListRep = db.WarehouseWastageStock
                 .Join(db.WarehouseStocks, wws => wws.WarehouseStockID, ws => ws.ID, (wws, ws) => new { wws, ws })
                 .Join(db.Products, wss => wss.ws.ProductID, p => p.ID, (wss, p) => new { wss, p })
                 .Select(o => new
                 {
                     item_name = o.p.Name,
                     SKUID=o.p.ID,
                     ProductVarient = db.Sizes.FirstOrDefault(s => s.ID ==
                         (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wss.ws.ProductVarientID).SizeID)).Name,
                     HSNCode = o.p.HSNCode,
                     Manufacturer = db.Brands.FirstOrDefault(b => b.ID == o.p.BrandID).Name,
                     SupplierName = db.Suppliers.FirstOrDefault(s => s.ID ==
                     (db.PurchaseOrders.FirstOrDefault(po => po.ID ==
                     (db.Invoices.FirstOrDefault(ss => ss.ID == o.wss.ws.InvoiceID)).PurchaseOrderID
                     )).SupplierID).Name,
                     batch_code = o.wss.ws.BatchCode,
                     waste_qty = o.wss.wws.WastageQuantity,
                     WasteAmt = o.wss.wws.WastageQuantity * o.wss.ws.BuyRatePerUnit,
                     SubReason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == o.wss.wws.SubReasonID)).Reason,
                     Reason = (db.WarehouseReasons.FirstOrDefault(wr => wr.ID == ((db.WarehouseReasons.FirstOrDefault(wrr => wrr.ID == o.wss.wws.SubReasonID)).ParentReasonId))).Reason,
                     Remark = o.wss.wws.Remark,
                     WarehouseID = o.wss.ws.WarehouseID,
                     CreateDate = o.wss.wws.CreateDate
                 })
                 .ToList();

            obj= ListRep.ToList().Select(p => new WastageReportViewModel
            {
                item_name = p.item_name,
              SKUID=p.SKUID,
                ProductVarient = p.ProductVarient,
                HSNCode = p.HSNCode,
                Manufacturer = p.Manufacturer,
                SupplierName = p.SupplierName,
                batch_code = p.batch_code,
                waste_qty = p.waste_qty,
                WasteAmt = p.WasteAmt,
                SubReason = p.SubReason,
                Reason = p.Reason,
                Remark = p.Remark,
                WarehouseID = p.WarehouseID,
                CreateDate = p.CreateDate,

            }).Where(X=>X.WarehouseID==WarehouseID).ToList();

        

            return obj;

        }

        ////
        //// GET: /WastageReport/

        //public ActionResult WastageReportNote(WastageReportViewModelList  obj) //, int? print
        //{
        //    try
        //    {
        //        WastageReportViewModelList objPO = new WastageReportViewModelList();
        //        List<WastageReportViewModel> obj_WastageEntry = new List<WastageReportViewModel>();

        //        if (Session["USER_NAME"] != null)
        //        { }
        //        else
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }

        //        long WarehouseID = 0;
        //        if (Session["WarehouseID"] != null)
        //        {
        //            WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
        //        }

        //        if (string.IsNullOrEmpty( obj.FromDate))
        //        {
        //            objPO.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
        //        }
        //        else
        //        {
        //            objPO.FromDate =  obj.FromDate;
        //        }




        //        if (string.IsNullOrEmpty( obj.ToDate))
        //        {
        //            objPO.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
        //        }
        //        else
        //        {
        //            objPO.ToDate =  obj.ToDate;
        //        }

        //        DateTime fDate = Convert.ToDateTime(objPO.FromDate);
        //        DateTime tDate = Convert.ToDateTime(objPO.ToDate);
        //        objPO.WarehouseID =  obj.WarehouseID;

        //        if (objPO.WarehouseID > 0)
        //        {
        //            obj_WastageEntry = GetRecord(fDate, tDate,  obj.WarehouseID);
        //        }




        //        //if (string.IsNullOrEmpty( obj.ToDate))
        //        //{
        //        //    objPO.WarehouseID = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
        //        //}
        //        //else

        //        objPO.WarehouseID =  obj.WarehouseID;


        //        //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
        //        objPO.lWastageReportViewModel = obj_WastageEntry.ToList();

        //        return View("WastageReport", objPO);
        //    }
        //    catch (Exception ex)
        //    {
        //        new Exception("Some unknown error encountered!");
        //    }
        //    return View();
        //}



       
        ////public ActionResult WarehouseZone()
        ////{
        ////    WarehouseZone oc = new WarehouseZone();
        ////    oc.ZoneList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
        ////    return View(oc);
        ////}
        //public ActionResult getFEF()
        //{
        //    WastageReportViewModel oc = new WastageReportViewModel();
        //    ViewBag.FrenchiseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");
        //    return View(oc);
        //}
        //public ActionResult WastageReport()
        //{


        //    if (Session["USER_NAME"] == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }

        //    List<WastageReportViewModel> obj_WastageEntry = new List<WastageReportViewModel>();
        //    long WarehouseID = 10;
        //    if (Session["WarehouseID"] != null)
        //    {
        //        WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
        //        WarehouseID = Convert.ToInt32(db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.ID).First());
        //    }
        //    //long WarehouseID = 0;
        //    //if (Session["USER_LOGIN_ID"] != null)
        //    //{
        //    //    WarehouseID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
        //    //    WarehouseID = Convert.ToInt32(db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.ID).First());
        //    //}
        //    WastageReportViewModelList obj = new WastageReportViewModelList();

        //    //if (Session["USER_NAME"] == null)
        //    //{
        //    //    return RedirectToAction("Index", "Login");
        //    //}
        //    //List<WastageEntry> obj_WastageEntry = new List<WastageEntry>();

        //    ///stored procedure calling mathod
        //    //var idParam = new SqlParameter
        //    //{
        //    //    ParameterName = "WarehouseID",
        //    //    Value = 10,
        //    //    SqlDbType = SqlDbType.BigInt
        //    //};
        //    // Dictionary<long ,long> MM=new Dictionary<long,long>

        //    //SqlParameter parameter1 = new SqlParameter("@WarehouseID", idParam);
        //    //db.Database.SqlQuery<WastageReportViewModel>("exec  WastageReport  @WarehouseID", idParam).ToList();
        //    // List<WastageReportViewModel> lidt = db.Database.SqlQuery<WastageReportViewModel>("exec  WastageReport").ToList();
        //    //List<WastageReportViewModel> lWastageReportViewModel = new List<WastageReportViewModel>();
        //    ///stored procedure calling mathod
        //    //List<WastageReportViewModel> obj_WastageEntry =new List<WastageReportViewModel>();


        //    obj_WastageEntry = db.Database.SqlQuery<WastageReportViewModel>(@"select Product.Name + ' '+ Size.Name as item_name " +
        //                     ",Product.HSNCode as HSNCode ,Product.ID as id,WarehouseWastageStock.ID  as waste_id " +
        //                     ",WarehouseWastageStock.WastageQuantity as waste_qty,WarehouseStock.BatchCode as batch_code " +
        //                     ",WarehouseWastageStock.CreateDate as createdDate,wr1.Reason+ ', '+ wr.Reason as Remark,Brand.NAME as Manufacturer,SubReasonId as SubReason " +
        //                     "from WarehouseWastageStock " +
        //                     "join WarehouseStock     " +
        //                     "on WarehouseWastageStock.WarehouseStockid=WarehouseStock.id " +
        //                     "join Product " +
        //                     "on WarehouseStock.ProductID= Product.id " +
        //                     "join WarehouseReason as wr " +
        //                     "on wr.ID=WarehouseWastageStock.SubReasonId " +
        //                     "join WarehouseReason as wr1 " +
        //                     "on wr1.ID=wr.ParentReasonId " +
        //                     "join ProductVarient " +
        //                     "on ProductVarient.id=WarehouseStock.ProductVarientID " +
        //                     "join Size " +
        //                     "on Size.ID=ProductVarient.SizeID " +
        //                     "join Brand " +
        //                     "on  Brand.ID=Product.BrandID " +
        //                  "join WarehouseReason as wr3 " +
        //                     "on wr3.ID=WarehouseWastageStock.SubReasonId " +

        //                     "WHERE WarehouseStock.WarehouseID=" + WarehouseID + "" +
        //                     " order by waste_id desc").ToList<WastageReportViewModel>();


        //    //obj.WarehouseList = new SelectList(db.Warehouses.ToList(), "ID", "Name");

        //    //ViewBag.Customers = Warehouse.GetCustomers().Select(c => new SelectListItem { Text = c.TextProperty, Value = c.ValueProperty }).ToList();




        //    // foreach (var item in obj_WastageEntry)
        //    //    {
        //    //        item.item_image_Path = ImageDisplay.SetProductThumbPath(item.id, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
        //    //    }
        //    //obj.lWastageReportViewModel = obj_WastageEntry.OrderByDescending(p => p.waste_id).ToList();

        //    obj.lWastageReportViewModel = obj_WastageEntry;
        //    return View(obj);

        //    //var emplist = new List<WastageReportViewModel>();
        //    //return  View (emplist);

        //}

        //public List<WastageReportViewModel> GetRecord(DateTime fDate, DateTime tDate, long WarehouseID)
        //{
        //    List<WastageReportViewModel> obj_WastageEntry = new List<WastageReportViewModel>();

        //    //yashaswi 19/3/2018

        //    if (Session["WarehouseID"] != null)
        //    {
        //        WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
        //    }
        //    obj_WastageEntry = db.Database.SqlQuery<WastageReportViewModel>(@"select Product.Name + ' '+ Size.Name as item_name " +
        //                     ",Product.HSNCode as HSNCode ,Product.ID as id,WarehouseWastageStock.ID  as waste_id " +
        //                     ",WarehouseWastageStock.WastageQuantity as waste_qty,WarehouseStock.BatchCode as batch_code " +
        //                     ",WarehouseWastageStock.CreateDate as createdDate,wr1.Reason+ ', '+ wr.Reason as Remark,Brand.NAME as Manufacturer,SubReasonId as SubReason " +
        //                     "from WarehouseWastageStock " +
        //                     "join WarehouseStock     " +
        //                     "on WarehouseWastageStock.WarehouseStockid=WarehouseStock.id " +
        //                     "join Product " +
        //                     "on WarehouseStock.ProductID= Product.id " +
        //                     "join WarehouseReason as wr " +
        //                     "on wr.ID=WarehouseWastageStock.SubReasonId " +
        //                     "join WarehouseReason as wr1 " +
        //                     "on wr1.ID=wr.ParentReasonId " +
        //                     "join ProductVarient " +
        //                     "on ProductVarient.id=WarehouseStock.ProductVarientID " +
        //                     "join Size " +
        //                     "on Size.ID=ProductVarient.SizeID " +
        //                     "join Brand " +
        //                     "on  Brand.ID=Product.BrandID " +
        //                  "join WarehouseReason as wr3 " +
        //                     "on wr3.ID=WarehouseWastageStock.SubReasonId " +

        //                     "WHERE WarehouseStock.WarehouseID=" + WarehouseID + "" +   //yashaswi 19/3/2018
        //                     " order by waste_id desc").ToList<WastageReportViewModel>();
        //    foreach (var item in obj_WastageEntry)
        //        //    {
        //        //        item.item_image_Path = ImageDisplay.SetProductThumbPath(item.id, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
        //        //    }
        //        obj_WastageEntry = obj_WastageEntry.OrderByDescending(p => p.waste_id).ToList();


        //    return obj_WastageEntry;




        //}









    }
}











