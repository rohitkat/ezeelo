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
    public class OutOfStockReportController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        //
        // GET: /OutOfStockReport/
        public ActionResult GetOutOfStockReport(OutOfStockViewModelList obj1, int? Status)
        {
            var OrderStatus = Status;
            if (OrderStatus == null)
            {
                OrderStatus = 7;
            }
            ViewBag.DropdownSelected = OrderStatus;
            OutOfStockViewModelList obj = new OutOfStockViewModelList();
            List<OutOfStockViewModel> list = new List<OutOfStockViewModel>();
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


            //obj.lWastageReportViewModel = list.ToList();
            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");




            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(obj.WarehouseID);
            //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

            //var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
            // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();


            var reportList = db.Database.SqlQuery<OutOfStockViewModel>(
           "exec dbo.[OutofStockReport] @WarehouseID,@FromDate,@ToDate, @OrderStatus",
              new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
            new SqlParameter("@FromDate", fDate),
           new SqlParameter("@ToDate", tDate),new SqlParameter("@OrderStatus", OrderStatus)}
   ).ToList();

            obj.lOutOfStockViewModel = reportList;
            long WarehouseID1 = 0;
            if (obj1.WarehouseID == 0)
            {
                WarehouseID1 = WarehouseID;
                obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();

            }
            else
            {
                WarehouseID1 = obj1.WarehouseID;



                if (WarehouseID1 == 0)
                {
                    obj.lOutOfStockViewModel = reportList.Where(p => p.WarehouseID == WarehouseID).ToList();
                }


                if (WarehouseID1 != null)
                {
                    obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                //DateTime fDate1 = Convert.ToDateTime(obj.FromDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                //DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                ////var result = obj.lReorderLevelReportViewModel.Where(x => x.CreateDate >= fDate1 &&
                ////                           x.CreateDate <= tDate1 && x.AvailableQuantity= 0).ToList();

                //var result = obj.lOutOfStockViewModel.Where(x => x.CreateDate >= fDate1 &&
                //                      x.CreateDate <= tDate1).ToList();
                //obj.lOutOfStockViewModel = result.ToList();
                
                if (obj1.IsChecked == true)
                {
                    var OutOfStockFromWeb = obj.lOutOfStockViewModel.Where(x => x.OutOfStockDate != null).ToList();
                    obj.lOutOfStockViewModel = OutOfStockFromWeb.ToList();
                }
            }
            return View(obj);
        }
//        public ActionResult GetOutOfStockReport(OutOfStockViewModelList obj1, int? Status) //Added by Rumana on 24-04-2019
//        {
//            long WarehouseID1 = 0;
//            OutOfStockViewModelList obj = new OutOfStockViewModelList();
//            List<OutOfStockViewModel> list = new List<OutOfStockViewModel>();
//            List<OutOfStocKReportViewModelOnPlaced> objWRLVM = new List<OutOfStocKReportViewModelOnPlaced>();
//            long WarehouseID = 0;
//            if (Session["WarehouseID"] != null)
//            {
//                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
//            }

//            if (string.IsNullOrEmpty(obj1.FromDate))
//            {
//                obj.FromDate = obj1.FromDate;
//                //obj.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
//            }
//            else
//            {
//                obj.FromDate = obj1.FromDate;
//            }

//            if (string.IsNullOrEmpty(obj1.ToDate))
//            {
//                obj.ToDate = obj1.ToDate;
//                //obj.ToDate = DateTime.Now.AddHours(5.5).ToString("MM/dd/yyyy");
//            }
//            else
//            {
//                obj.ToDate = obj1.ToDate;
//            }

//            DateTime fDate = Convert.ToDateTime(obj.FromDate).AddHours(23).AddMinutes(59).AddSeconds(59); ;
//            DateTime tDate = Convert.ToDateTime(obj.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);
//            obj.WarehouseID = obj1.WarehouseID;


//            //obj.lWastageReportViewModel = list.ToList();
//            obj.WarehouseList = new SelectList(db.Warehouses.Where(p => p.ID == WarehouseID).ToList(), "ID", "Name");
            

//            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(obj.WarehouseID);
//            //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
//            //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });

//            var CreateDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
//            //Added by Rumana on 24-04-2019
            
//            ViewBag.DropdownSelected = Status;

//            var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();
//            obj.lOutOfStockViewModel = db.Database.SqlQuery<OutOfStockViewModel>(
//"exec dbo.[_NewOutofStockReport] @WarehouseID,@Status",
//new Object[] { new SqlParameter("@WarehouseID", WarehouseID), new SqlParameter("@Status", Status) }
//).ToList(); // Status Parameter Added by Rumana on 26-04-2019

//            obj.lOutOfStockViewModel = db.Database.SqlQuery<OutOfStockViewModel>("exec dbo.[_NewOutofStockReport]").ToList(); // Status Parameter Added by Rumana on 26-04-2019
//                WarehouseID1 = 0;
//                if (obj1.WarehouseID == 0)
//                {
//                    WarehouseID1 = WarehouseID;
//                    obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();

//                }
//                else
//                {
//                    WarehouseID1 = obj1.WarehouseID;



//                    if (WarehouseID1 == 0)
//                    {
//                        obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == WarehouseID).ToList();
//                    }


//                    if (WarehouseID1 != null)
//                    {
//                        obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
//                    }

//                }
//                DateTime fDate1 = Convert.ToDateTime(obj1.FromDate);
//                DateTime tDate1 = Convert.ToDateTime(obj1.ToDate).AddHours(23).AddMinutes(59);
//                if (obj1.FromDate != null && obj1.ToDate != null)
//                {
//                    obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(x => x.OutOfStockDate >= fDate1 && x.OutOfStockDate <= tDate1).ToList(); //Added by Rumana for getting OutofStock list as per OutOfStockDate on 26-04-2019

//                }
//                if (obj1.IsChecked == true)
//                {
//                    var OutOfStockFromWeb = obj.lOutOfStockViewModel.Where(x => x.OutOfStockDate != null).ToList();
//                    obj.lOutOfStockViewModel = OutOfStockFromWeb.ToList();
//                }
    
          
            
//            return View(obj);
//        }
        public ActionResult StockBatchwise(long ID, long ShopStockID)
        {
            BatchAllotmentViewModel objBatch = new BatchAllotmentViewModel();
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<WarehouseStock> lWarehouseStock = new List<WarehouseStock>();
            List<WarehouseStockViewModel> objWRLVM = new List<WarehouseStockViewModel>();
            try
            {
                if (Session["USER_NAME"] != null)
                { }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                var query = db.WarehouseReorderLevels.Where(x => x.ID == ID).FirstOrDefault();
                objWS.WarehouseName = db.Warehouses.Where(x => x.ID == query.WarehouseID).Select(x => x.Name).FirstOrDefault();
                ViewBag.WarehouseReorderLevelID = ID;
                ViewBag.ShopName = db.ShopStocks.Where(x => x.ID == ShopStockID).Select(x => x.ShopProduct.Shop.Name).FirstOrDefault();

                //Get all batches of product from stock
                //lWarehouseStock = db.WarehouseStocks.Where(x => x.WarehouseID == query.WarehouseID && x.ProductID == query.ProductID && x.ProductVarientID == query.ProductVarientID).OrderBy(x => x.AvailableQuantity).ToList();

                var BatchQuantity = (from ws in db.WarehouseStocks
                                     join ss in db.ShopStocks on ws.ID equals ss.WarehouseStockID
                                     into ps
                                     from ss in ps.DefaultIfEmpty()
                                     where ws.WarehouseID == query.WarehouseID && ws.ProductID == query.ProductID
                                     && ws.ProductVarientID == query.ProductVarientID && ws.AvailableQuantity > 0
                                     //&& w.IsFulfillmentCenter == true
                                     select new WarehouseStockViewModel
                                     {
                                         ID = ws.ID,
                                         WarehouseStockID = ss.WarehouseStockID == null ? 0 : ss.WarehouseStockID,
                                         WarehouseID = ws.ID,
                                         BatchCode = ws.BatchCode,
                                         MRP = ws.MRP,
                                         BuyRatePerUnit = ws.BuyRatePerUnit,
                                         SaleRatePerUnit = ws.SaleRatePerUnit,
                                         InitialQuantity = ws.InitialQuantity,
                                         AvailableQuantity = ws.AvailableQuantity,
                                         ExpiryDate = ws.ExpiryDate,
                                         ShopStockID = ShopStockID,
                                         ProductID = ws.ProductID,
                                         ProductVarientID = ws.ProductVarientID,
                                         isAllottedToShop = ss.WarehouseStockID == null ? false : true
                                     }).OrderByDescending(x => x.AvailableQuantity).OrderBy(x => x.ExpiryDate).OrderByDescending(x => x.WarehouseStockID).ToList();

                if (query.WarehouseID > 0)
                {

                    foreach (var item in BatchQuantity)
                    {
                        WarehouseStockViewModel objWarehouseStockViewModel = new WarehouseStockViewModel();
                        objWarehouseStockViewModel.ID = item.ID;
                        objWarehouseStockViewModel.WarehouseStockID = item.isAllottedToShop == true ? item.WarehouseStockID : 0;
                        objWarehouseStockViewModel.BatchCode = item.BatchCode;
                        objWarehouseStockViewModel.ProductID = item.ProductID;
                        objWarehouseStockViewModel.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                        objWarehouseStockViewModel.MRP = Convert.ToDecimal(item.MRP);
                        objWarehouseStockViewModel.BuyRatePerUnit = item.BuyRatePerUnit;
                        objWarehouseStockViewModel.SaleRatePerUnit = Convert.ToDecimal(item.SaleRatePerUnit);
                        objWarehouseStockViewModel.InitialQuantity = item.InitialQuantity;
                        objWarehouseStockViewModel.AvailableQuantity = item.AvailableQuantity;
                        objWarehouseStockViewModel.ExpiryDate = item.ExpiryDate;
                        objWarehouseStockViewModel.isAllottedToShop = item.isAllottedToShop;
                        objWarehouseStockViewModel.InShopQty = db.ShopStocks.Where(x => x.ID == item.ShopStockID && x.WarehouseStockID == item.WarehouseStockID).Select(x => x.Qty).FirstOrDefault();
                        objWarehouseStockViewModel.PlacedQty = item.WarehouseStockID == null ? 0 : db.CustomerOrderDetails.Where(x => x.WarehouseStockID == item.ID && x.OrderStatus != 0 && x.OrderStatus != 7 && x.OrderStatus != 8 && x.OrderStatus != 9 && x.OrderStatus != 10).Select(x => (int)x.Qty).DefaultIfEmpty(0).Sum();
                        objWarehouseStockViewModel.InStockQty = item.AvailableQuantity - objWarehouseStockViewModel.PlacedQty;
                        objWarehouseStockViewModel.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        objWarehouseStockViewModel.ShopStockID = item.ShopStockID;

                        var itemName = (from p in db.Products
                                        join v in db.ProductVarients on p.ID equals v.ProductID
                                        join s in db.Sizes on v.SizeID equals s.ID
                                        where v.ID == item.ProductVarientID
                                        select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                        foreach (var i in itemName)
                        {
                            objWarehouseStockViewModel.ItemName = i.ItemName.ToString();
                            objWarehouseStockViewModel.HSNCode = Convert.ToString(i.HSNCode);
                        }
                        //if (item.isAllottedToShop == false && item.AvailableQuantity <= 0)
                        //{

                        //}
                        //else
                        //{
                        objWRLVM.Add(objWarehouseStockViewModel);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            //objBatch.lWarehouseStockViewModels = objWRLVM;
            return View("StockBatchwise", objWRLVM);
        }
        public ActionResult Export(string FromDate, string ToDate, int option, long WarehouseID, int? Status) //, int? print
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
                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);


                //   var FromDate = Convert.ToDateTime("06-13-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });
                //    var ToDate=     Convert.ToDateTime("08-23-2018", new DateTimeFormatInfo { FullDateTimePattern = "MM-dd-yyyy" });


                // var reportList = db.Database.SqlQuery<ReorderLevelReportViewModel>("exec ReorderLevelReport").ToList();


                //            var reportList = db.Database.SqlQuery<OutOfStockViewModel>(
                //"exec dbo.[OutofStockReport] @WarehouseID,@FromDate,@ToDate",
                //new Object[] { new SqlParameter("@WarehouseID", obj.WarehouseID) ,
                //        new SqlParameter("@FromDate", fDate),
                //       new SqlParameter("@ToDate", tDate)}
                //   ).ToList();
                if (Status == null)
                {
                    Status = 7;
                }
                ViewBag.DropdownSelected = Status;
                var reportList = db.Database.SqlQuery<OutOfStockViewModel>(
           "exec dbo.[OutofStockReport] @WarehouseID,@FromDate,@ToDate,@Status",
           new Object[] { new SqlParameter("@WarehouseID", WarehouseID) ,
                      new SqlParameter("@FromDate", fDate),
                     new SqlParameter("@ToDate", tDate),new SqlParameter("@Status", Status)}
            ).ToList(); // Status Parameter Added by Rumana on 26-04-2019
                obj.lOutOfStockViewModel = reportList;

                if (obj.WarehouseID != null)
                {
                    obj.lOutOfStockViewModel = obj.lOutOfStockViewModel.Where(p => p.WarehouseID == obj.WarehouseID).ToList();
                }

                DateTime fDate1 = Convert.ToDateTime(obj.FromDate);
                DateTime tDate1 = Convert.ToDateTime(obj.ToDate).AddHours(23).AddSeconds(59);


                //var result = obj.lOutOfStockViewModel.Where(x => x.CreateDate >= fDate1 &&
                //                      x.CreateDate <= tDate1).ToList();
                //obj.lOutOfStockViewModel = result.ToList();
                var result = obj.lOutOfStockViewModel.Where(x => x.OutOfStockDate >= fDate1 && x.OutOfStockDate <= tDate1).ToList(); //Added by Rumana for getting OutofStock list as per OutOfStockDate on 26-04-2019
                obj.lOutOfStockViewModel = result.ToList();
                //Added by Rumana for displaying on Out of stock product list from web on 22-04-2019
                if (obj.IsChecked == true)
                {
                    var OutOfStockFromWeb = obj.lOutOfStockViewModel.Where(x => x.OutOfStockDate != null).ToList();
                    obj.lOutOfStockViewModel = OutOfStockFromWeb.ToList();
                }

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




                    int i = 0;
                    foreach (var row in obj.lOutOfStockViewModel)
                    {
                        i = i + 1;
                        dt.LoadDataRow(new object[] { i, row.SKUID, row.SKUName,row.SKUUnit,row.HSNCode,row.Manifecturer ,row.BatchCode,row.InvoiceCode,row.InitialQuantity,row.AvailableInStock,
                       row.SupplierName, row.Category1,row.Category2,row.Category3,row.Amount,row.CreateDate,row.BuyRatePerUnit,row.SaleRate,row.MRP,row.RetailPoint,row.ExpiryDate,row.ReorderLevel,row.ReorderHitDate ,row.OutOfStockDate}, false);
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