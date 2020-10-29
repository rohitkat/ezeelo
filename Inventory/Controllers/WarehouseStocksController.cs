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
using BusinessLogicLayer;
using System.Transactions;
using System.Data.Entity.Validation;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace Inventory.Controllers
{
    public class WarehouseStocksController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController obj_comm = new CommonController();
        //
        // GET: /WarehouseStocks/

        public ActionResult Index()
        {
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<InvoiceViewModel> lInvoiceViewModel = new List<InvoiceViewModel>();
            try
            {
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }

                List<Supplier> lSupplier = new List<Supplier>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lSupplier = db.Suppliers.ToList();

                foreach (var c in lSupplier)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.PossiblePrentCategory = forloopclasses.ToList();

                //ViewBag.PossibleSuppliers = db.Suppliers;
                //Yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = obj_comm.GetSupplierLIst(WarehouseID);

                if (WarehouseID > 0)
                {
                    lInvoiceViewModel = (from o in db.Invoices
                                         join p in db.PurchaseOrders on o.PurchaseOrderID equals p.ID
                                         join s in db.Suppliers on p.SupplierID equals s.ID
                                         where p.WarehouseID == WarehouseID
                                         select new InvoiceViewModel
                                         {
                                             InvoiceID = o.ID,
                                             InvoiceCode = o.InvoiceCode,
                                             SupplierName = s.Name,
                                             PurchaseOrderID = o.PurchaseOrderID,
                                             PurchaseOrderCode = p.PurchaseOrderCode,
                                             OrderAmount = o.OrderAmount,
                                             InvoiceDate = o.InvoiceDate,
                                             TotalDiscountAmount = o.TotalDiscountAmount,
                                             ShippingCharge = o.ShippingCharge,
                                             CustomDutyCharge = o.CustomDutyCharge,
                                             OperatingCost = o.OperatingCost,
                                             AdditionalCost = o.AdditionalCost,
                                             TotalAmount = o.TotalAmount,
                                             TotalItems = db.InvoiceDetails.Where(x => x.InvoiceID == o.ID).Select(x => x.ID).Count(),
                                             IsApproved = o.IsApproved
                                         }).OrderByDescending(x => x.InvoiceID).ToList(); //.OrderBy(o=>o.IsApproved)

                }
                if (Session["USER_NAME"] != null)
                {

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                objWS.lInvoiceViewModelList = lInvoiceViewModel;
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("Index", objWS);
        }

        public ActionResult Search(long? SupplierID)
        {
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<InvoiceViewModel> lInvoiceViewModel = new List<InvoiceViewModel>();
            try
            {
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }

                if (SupplierID != null && SupplierID > 0)
                {
                    lInvoiceViewModel = (from o in db.Invoices
                                         join p in db.PurchaseOrders on o.PurchaseOrderID equals p.ID
                                         join s in db.Suppliers on p.SupplierID equals s.ID
                                         where p.WarehouseID == WarehouseID && s.ID == SupplierID
                                         select new InvoiceViewModel
                                         {
                                             InvoiceID = o.ID,
                                             SupplierName = s.Name,
                                             PurchaseOrderID = o.PurchaseOrderID,
                                             OrderAmount = o.OrderAmount,
                                             InvoiceDate = o.InvoiceDate,
                                             TotalDiscountAmount = o.TotalDiscountAmount,
                                             ShippingCharge = o.ShippingCharge,
                                             CustomDutyCharge = o.CustomDutyCharge,
                                             OperatingCost = o.OperatingCost,
                                             AdditionalCost = o.AdditionalCost,
                                             TotalAmount = o.TotalAmount,
                                             TotalItems = db.InvoiceDetails.Where(x => x.InvoiceID == o.ID).Select(x => x.ID).Count(),
                                             IsApproved = o.IsApproved
                                         }).OrderByDescending(o => o.InvoiceDate).ToList();

                }
                else
                {
                    return RedirectToAction("Index");
                }

                if (Session["USER_NAME"] != null)
                {

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                objWS.lInvoiceViewModelList = lInvoiceViewModel;
                //ViewBag.PossibleSuppliers = db.Suppliers;
                //Yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = obj_comm.GetSupplierLIst(WarehouseID);
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            //return Json(objWS, JsonRequestBehavior.AllowGet);
            return View("Index", objWS);
        }



        //
        // GET: /WarehouseStocks/Details/5

        public ViewResult Details(long id)
        {
            WarehouseStock warehousestock = db.WarehouseStocks.Single(x => x.ID == id);
            return View(warehousestock);
        }

        //
        // GET: /WarehouseStocks/Create

        public ActionResult Create()
        {
            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleInvoices = db.Invoices;
            ViewBag.PossibleProducts = db.Products;
            ViewBag.PossibleProductVarients = db.ProductVarients;
            ViewBag.PossibleSizes = db.Sizes;
            return View();
        }

        //
        // POST: /WarehouseStocks/Create

        [HttpPost]
        public ActionResult Create(WarehouseStock warehousestock)
        {
            if (ModelState.IsValid)
            {
                db.WarehouseStocks.Add(warehousestock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleInvoices = db.Invoices;
            ViewBag.PossibleProducts = db.Products;
            ViewBag.PossibleProductVarients = db.ProductVarients;
            ViewBag.PossibleSizes = db.Sizes;
            return View(warehousestock);
        }

        //
        // GET: /WarehouseStocks/Edit/5

        //"long? ShopID"==>If in future 1 fulfillment center have more than 1 shop then we can utilizy this parameter 
        public ActionResult Edit(long InvoiceID, long? ShopID)
        {
            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;
            long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
            ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == warehouseID);
            ViewBag.WarehouseID = warehouseID;
            try
            {

                bool isFulfillmentCenter = db.Warehouses.Where(x => x.ID == warehouseID).Select(x => x.IsFulfillmentCenter).FirstOrDefault();

                //Get list of all shops which are under franchise of warehouse
                var lfranhiseID = db.WarehouseFranchises.Where(x => x.WarehouseID == warehouseID && x.IsActive == true).Select(x => x.FranchiseID).ToList();
                var AllShops = db.Shops.Where(x => x.IsActive == true).ToList();
                if (lfranhiseID != null && lfranhiseID.Count > 0)
                {
                    foreach (var id in lfranhiseID)
                    {
                        AllShops = AllShops.Where(x => x.FranchiseID == id).ToList();
                    }
                    ViewBag.AllShops = AllShops;
                }
                else
                {
                    ViewBag.AllShops = null;
                }
                //End

                //confirm that each Fulfillment center can have only one shop
                if (ShopID == null && AllShops.Count > 0)
                {
                    ShopID = AllShops.Select(x => x.ID).FirstOrDefault();
                    InvoiceViewModel.ShopID = ShopID;
                    InvoiceViewModel.ShopName = AllShops.Where(x => x.ID == ShopID).Select(x => x.Name).FirstOrDefault();
                    InvoiceViewModel.ShopAddress = AllShops.Where(x => x.ID == ShopID).Select(x => x.Address).FirstOrDefault();
                }
                //End

                var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
                if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
                {
                    ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                }
                var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

                if (Invoice != null && Invoice.ToString() != "")
                {
                    InvoiceViewModel.InvoiceID = InvoiceID;
                    InvoiceViewModel.InvoiceCode = Invoice.InvoiceCode;
                    InvoiceViewModel.PurchaseOrderID = PurchaseOrderID;
                    InvoiceViewModel.WarehouseID = purchaseOrder.WarehouseID;
                    InvoiceViewModel.IsFulfillmentCenter = isFulfillmentCenter;
                    InvoiceViewModel.WarehouseName = db.Warehouses.Where(x => x.ID == purchaseOrder.WarehouseID).Select(x => x.Name).FirstOrDefault();
                    InvoiceViewModel.SupplierID = purchaseOrder.SupplierID;
                    InvoiceViewModel.SupplierName = db.Suppliers.Where(x => x.ID == purchaseOrder.SupplierID).Select(x => x.Name).FirstOrDefault();
                    InvoiceViewModel.InvoiceDate = Invoice.InvoiceDate;
                    InvoiceViewModel.TotalDiscountAmount = Invoice.TotalDiscountAmount;
                    InvoiceViewModel.ShippingCharge = Invoice.ShippingCharge;
                    InvoiceViewModel.CustomDutyCharge = Invoice.CustomDutyCharge;
                    InvoiceViewModel.OperatingCost = Invoice.OperatingCost;
                    InvoiceViewModel.AdditionalCost = Invoice.AdditionalCost;
                    InvoiceViewModel.OrderAmount = Invoice.OrderAmount;
                    InvoiceViewModel.TotalAmount = Invoice.TotalAmount;
                    InvoiceViewModel.Remark = Invoice.Remark;
                    InvoiceViewModel.IsActive = Invoice.IsActive;
                    //Yashaswi To display Attachment
                    CommonController obj_commonController = new CommonController();
                    if (db.InvoiceAttachment.Any(p => p.InvoiceID == InvoiceID))
                    {
                        InvoiceViewModel.Invoice_Attachment = obj_commonController.GetFileName(db.InvoiceAttachment.SingleOrDefault(p => p.InvoiceID == InvoiceID).FileName);
                        InvoiceViewModel.Invoice_AttachmentFileName = db.InvoiceAttachment.SingleOrDefault(p => p.InvoiceID == InvoiceID).FileName;
                    }
                }

                List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
                lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

                List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
                lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();

                List<InvoiceDetailViewModel> lInvoiceDetailViewModelList = new List<InvoiceDetailViewModel>();


                int countItemExistsInShop = 0;

                foreach (var item in lInvoiceDetaillist)
                {
                    InvoiceDetailViewModel objPOD = new InvoiceDetailViewModel();
                    objPOD.ReceivedQuantity = item.ReceivedQuantity;
                    objPOD.CGSTAmount = Convert.ToDecimal(item.CGSTAmount == null ? 0 : item.CGSTAmount);
                    objPOD.SGSTAmount = Convert.ToDecimal(item.SGSTAmount == null ? 0 : item.SGSTAmount);
                    objPOD.IGSTAmount = Convert.ToDecimal(item.IGSTAmount == null ? 0 : item.IGSTAmount);
                    objPOD.Amount = Convert.ToDecimal(item.Amount);
                    objPOD.ExpiryDate = item.ExpiryDate;
                    objPOD.Remark = item.Remark == null ? "" : item.Remark;

                    objPOD.PurchaseOrderDetailsID = item.ID;
                    objPOD.ProductID = item.ProductID;
                    objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                    objPOD.Quantity = item.ReceivedQuantity;
                    objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                    objPOD.MRP = item.MRP;
                    objPOD.SaleRate = item.SaleRate;

                    objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                    var itemName = (from p in db.Products
                                    join v in db.ProductVarients on p.ID equals v.ProductID
                                    join s in db.Sizes on v.SizeID equals s.ID
                                    where v.ID == item.ProductVarientID
                                    select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                    foreach (var i in itemName)
                    {
                        objPOD.ItemName = i.ItemName.ToString();
                        objPOD.HSNCode = Convert.ToString(i.HSNCode);
                    }

                    int ReorderLevel = db.WarehouseReorderLevels.Where(x => x.WarehouseID == purchaseOrder.WarehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID).Select(x => x.ReorderLevel).FirstOrDefault();
                    objPOD.ReorderLevel = ReorderLevel;

                    if (isFulfillmentCenter == true)
                    {
                        if (ShopID != null || Convert.ToInt64(ShopID) > 0)
                        {
                            long shopProductID = db.ShopProducts.Where(x => x.ShopID == ShopID && x.ProductID == item.ProductID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                            if (shopProductID > 0)
                            {
                                long shopStockID = db.ShopStocks.Where(x => x.ShopProductID == shopProductID && x.ProductVarientID == item.ProductVarientID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();
                                if (shopStockID > 0)
                                {
                                    objPOD.IsItemExistsInShop = true;
                                    objPOD.ShopStockID = shopStockID;
                                    countItemExistsInShop = countItemExistsInShop + 1;
                                }
                                else
                                {
                                    objPOD.IsItemExistsInShop = false;
                                    objPOD.ShopStockID = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        objPOD.IsItemExistsInShop = true;
                        objPOD.ShopStockID = 0;
                    }

                    lInvoiceDetailViewModelList.Add(objPOD);

                    //var expirydate = db.WarehouseStocks.Where(x => x.InvoiceID == InvoiceID && x.ProductID == inv.ProductID && x.ProductVarientID == inv.ProductVarientID).Select(x => x.ExpiryDate).FirstOrDefault();
                    //objPOD.ExpiryDate = expirydate;                   
                }


                if (lInvoiceDetaillist.Count == countItemExistsInShop || isFulfillmentCenter == false)
                {
                    ViewBag.IsReadyToApprove = true;
                }
                else
                {
                    ViewBag.IsReadyToApprove = false;
                }

                if (lInvoiceDetailViewModelList.Count > 0)
                {
                    Session["lInvoiceDetailViewModel"] = lInvoiceDetailViewModelList;
                }

                InvoiceViewModel.lInvoiceDetailViewModels = lInvoiceDetailViewModelList;

                ViewBag.PurchaseOrderID = PurchaseOrderID;
                return View(InvoiceViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.PurchaseOrderID = PurchaseOrderID;
                return View(InvoiceViewModel);
            }

            return View(InvoiceViewModel);
        }

        //
        // POST: /WarehouseStocks/Edit/5

        //
        // POST: /WarehouseStocks/Edit/5
        [HttpPost]
        public ActionResult Edit(InvoiceViewModel Invoice)
        {
            List<InvoiceDetailViewModel> lInvoiceDetailViewModel = new List<InvoiceDetailViewModel>();

            //if (Session["lPurchaseOrderDetailsViewModel"] != null && Session["Amount"] != null)
            //{
            lInvoiceDetailViewModel = Invoice.lInvoiceDetailViewModels;
            if (lInvoiceDetailViewModel.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                            bool flag = false;
                            //Insert into WarehouseStock table
                            WarehouseStock objStock = new WarehouseStock();

                            WarehouseReorderLevel objWarehouseReorderLevel = new WarehouseReorderLevel();
                            long PurchaseOrderID = db.Invoices.Where(x => x.ID == Invoice.InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                            long vendorID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.SupplierID).FirstOrDefault();
                            long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();

                            DateTime datevalue = Invoice.InvoiceDate;

                            String dy = datevalue.Day.ToString();
                            String mn = datevalue.Month.ToString();
                            String yy = datevalue.Year.ToString();

                            foreach (InvoiceDetailViewModel item in lInvoiceDetailViewModel)
                            {
                                int sizeID = db.ProductVarients.Where(x => x.ID == item.ProductVarientID).Select(x => x.SizeID).FirstOrDefault();

                                //Insert for Fulfullment center
                                if (Invoice.IsFulfillmentCenter == true)
                                {
                                    if (item.IsItemExistsInShop == true && item.ShopStockID > 0)
                                    {
                                        //yashaswi 23/4/2018
                                        //item.PurchaseOrderDetailsID contain invoicedetailId
                                        double BusinessPoints = 0;
                                        long purchaseOderdetailId = 0;
                                        long RateMatrixExtensionId = 0;
                                        InvoiceDetail obj_invoicedetail = db.InvoiceDetails.FirstOrDefault(i => i.ID == item.PurchaseOrderDetailsID);
                                        if (obj_invoicedetail != null)
                                        {
                                            purchaseOderdetailId = obj_invoicedetail.PurchaseOrderDetailID;
                                            PurchaseOrderDetail obj_PurchaseOrderDetail = db.PurchaseOrderDetails.FirstOrDefault(p => p.ID == purchaseOderdetailId);
                                            if (obj_PurchaseOrderDetail != null)
                                            {
                                                RateMatrixExtensionId = (obj_PurchaseOrderDetail.RateCalculationId == null) ? 0 : (long)obj_PurchaseOrderDetail.RateMatrixExtensionId;
                                            }
                                        }
                                        BusinessPoints = db.RateMatrixExtension.FirstOrDefault(p => p.ID == RateMatrixExtensionId).RetailPoint;
                                        //End

                                        //Insert into WarehouseStock table
                                        objStock.WarehouseID = warehouseID;
                                        objStock.InvoiceID = Invoice.InvoiceID;
                                        objStock.BatchCode = "B" + dy + mn + yy + "I" + Invoice.InvoiceID;
                                        //objStock.BatchCode = "EZB" + dy + mn + yy + "W" + warehouseID + "S" + vendorID + "I" + Invoice.InvoiceID;
                                        objStock.ProductID = item.ProductID;
                                        objStock.ProductVarientID = item.ProductVarientID;
                                        objStock.BuyRatePerUnit = item.BuyRatePerUnit;
                                        objStock.MRP = item.MRP;
                                        objStock.SaleRatePerUnit = Convert.ToDecimal(item.SaleRate);
                                        objStock.InitialQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                        objStock.AvailableQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                        objStock.StockStatus = true;
                                        objStock.SizeID = sizeID;
                                        objStock.ExpiryDate = item.ExpiryDate;
                                        //yashaswi 23/4/2018
                                        objStock.BusinessPoints = (decimal)BusinessPoints;
                                        objStock.CreateDate = DateTime.Now;
                                        objStock.CreateBy = GetPersonalDetailID();
                                        objStock.NetworkIP = CommonFunctions.GetClientIP();
                                        objStock.DeviceID = "X";
                                        objStock.DeviceType = "X";
                                        db.WarehouseStocks.Add(objStock);
                                        db.SaveChanges();

                                        long WarehouseStockID = objStock.ID;


                                        //Insert into WarehouseReorderLevel table
                                        long id = 0;
                                        id = db.WarehouseReorderLevels.Where(x => x.WarehouseID == warehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID).Select(x => x.ID).FirstOrDefault();
                                        if (id > 0)
                                        {
                                            int availableQuantity = db.WarehouseReorderLevels.Where(x => x.WarehouseID == warehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID).Select(x => x.AvailableQuantity).FirstOrDefault();
                                            var lWarehouseReorderLevel = new WarehouseReorderLevel()
                                            {
                                                ID = id,
                                                AvailableQuantity = availableQuantity + Convert.ToInt32(item.ReceivedQuantity),
                                                ReorderLevel = Convert.ToInt32(item.ReorderLevel)
                                            };

                                            db.WarehouseReorderLevels.Attach(lWarehouseReorderLevel);
                                            db.Entry(lWarehouseReorderLevel).Property(x => x.AvailableQuantity).IsModified = true;
                                            db.Entry(lWarehouseReorderLevel).Property(x => x.ReorderLevel).IsModified = true;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            objWarehouseReorderLevel.WarehouseID = warehouseID;
                                            objWarehouseReorderLevel.ProductID = item.ProductID;
                                            objWarehouseReorderLevel.ProductVarientID = item.ProductVarientID;
                                            objWarehouseReorderLevel.AvailableQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                            objWarehouseReorderLevel.ReorderLevel = Convert.ToInt32(item.ReorderLevel);
                                            db.WarehouseReorderLevels.Add(objWarehouseReorderLevel);
                                            db.SaveChanges();
                                        }


                                        //Update quantity in ShopStock
                                        flag = true;
                                        int availableQty = db.ShopStocks.Where(x => x.ID == item.ShopStockID).Select(x => x.Qty).FirstOrDefault();

                                        ///if quanitity is 0 then directly update quantity from invoice
                                        if (availableQty == 0)
                                        {
                                            var lShopStock = new ShopStock()
                                            {
                                                ID = item.ShopStockID,
                                                ProductVarientID = item.ProductVarientID,
                                                Qty = availableQty + Convert.ToInt32(item.ReceivedQuantity),
                                                MRP = Convert.ToDecimal(item.MRP),
                                                RetailerRate = Convert.ToDecimal(item.SaleRate),
                                                StockStatus = true,
                                                WarehouseStockID = WarehouseStockID,
                                                //yashaswi 23/4/2018
                                                BusinessPoints = (decimal)BusinessPoints,
                                                CashbackPoints = prod.getCasbackPointsOnProductFromWarehouse(WarehouseStockID),
                                                ModifyDate = DateTime.Now,
                                                ModifyBy = GetPersonalDetailID(),
                                                NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP(),
                                                DeviceID = "x",
                                                DeviceType = "x",
                                            };

                                            db.ShopStocks.Attach(lShopStock);
                                            db.Entry(lShopStock).Property(x => x.Qty).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.MRP).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.RetailerRate).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.StockStatus).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.WarehouseStockID).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.BusinessPoints).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.CashbackPoints).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.ModifyDate).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.ModifyBy).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.NetworkIP).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.DeviceID).IsModified = true;
                                            db.Entry(lShopStock).Property(x => x.DeviceType).IsModified = true;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    //Insert record for Distributor center
                                    flag = true;

                                    objStock.WarehouseID = warehouseID;
                                    objStock.InvoiceID = Invoice.InvoiceID;
                                    objStock.BatchCode = "B" + dy + mn + yy + "I" + Invoice.InvoiceID;
                                    //objStock.BatchCode = "B" + dy + mn + yy+"W" + warehouseID + "S" + vendorID + "I" + Invoice.InvoiceID;
                                    objStock.ProductID = item.ProductID;
                                    objStock.ProductVarientID = item.ProductVarientID;
                                    objStock.BuyRatePerUnit = item.BuyRatePerUnit;
                                    objStock.MRP = item.MRP;
                                    objStock.SaleRatePerUnit = Convert.ToDecimal(item.SaleRate);
                                    objStock.InitialQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                    objStock.AvailableQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                    objStock.StockStatus = true;
                                    objStock.SizeID = sizeID;
                                    objStock.ExpiryDate = item.ExpiryDate;
                                    objStock.CreateDate = DateTime.Now;
                                    objStock.CreateBy = GetPersonalDetailID();
                                    objStock.NetworkIP = CommonFunctions.GetClientIP();
                                    objStock.DeviceID = "X";
                                    objStock.DeviceType = "X";
                                    db.WarehouseStocks.Add(objStock);
                                    db.SaveChanges();

                                    //Insert into WarehouseReorderLevel table
                                    long id = 0;
                                    id = db.WarehouseReorderLevels.Where(x => x.WarehouseID == warehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID).Select(x => x.ID).FirstOrDefault();
                                    if (id > 0)
                                    {
                                        int availableQuantity = db.WarehouseReorderLevels.Where(x => x.WarehouseID == warehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID).Select(x => x.AvailableQuantity).FirstOrDefault();
                                        var lWarehouseReorderLevel = new WarehouseReorderLevel()
                                        {
                                            ID = id,
                                            AvailableQuantity = availableQuantity + Convert.ToInt32(item.ReceivedQuantity),
                                            ReorderLevel = Convert.ToInt32(item.ReorderLevel)
                                        };

                                        db.WarehouseReorderLevels.Attach(lWarehouseReorderLevel);
                                        db.Entry(lWarehouseReorderLevel).Property(x => x.AvailableQuantity).IsModified = true;
                                        db.Entry(lWarehouseReorderLevel).Property(x => x.ReorderLevel).IsModified = true;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        objWarehouseReorderLevel.WarehouseID = warehouseID;
                                        objWarehouseReorderLevel.ProductID = item.ProductID;
                                        objWarehouseReorderLevel.ProductVarientID = item.ProductVarientID;
                                        objWarehouseReorderLevel.AvailableQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                        objWarehouseReorderLevel.ReorderLevel = Convert.ToInt32(item.ReorderLevel);
                                        db.WarehouseReorderLevels.Add(objWarehouseReorderLevel);
                                        db.SaveChanges();
                                    }
                                }
                            }


                            if (flag == true)
                            {
                                //Insert into Invoice Table  
                                var lInvoice = new Invoice()
                                {
                                    ID = Invoice.InvoiceID,
                                    InvoiceCode = Invoice.InvoiceCode,
                                    IsApproved = true,
                                    ApprovedBy = GetPersonalDetailID()
                                };

                                db.Invoices.Attach(lInvoice);
                                db.Entry(lInvoice).Property(x => x.IsApproved).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.ApprovedBy).IsModified = true;
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (DbEntityValidationException ex)
                                {
                                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                                    {
                                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                                        {
                                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                        }
                                    }
                                }
                            }

                            tscope.Complete();
                            Session["Success"] = "Invoice Approved Successfully."; //yashaswi 31/3/2018
                        }
                        catch (Exception)
                        {
                            Transaction.Current.Rollback();
                            tscope.Dispose();
                        }
                    }

                    return RedirectToAction("Index");
                }
            }
            //}
            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleSuppliers = db.Suppliers;
            return View(Invoice);
        }


        public long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt64(Session["USER_LOGIN_ID"]);
            long PersonalDetailID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PlacedController][GetShopID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }



        public ActionResult Stock()
        {
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<WarehouseReorderLevel> lWarehouseReorderLevel = new List<WarehouseReorderLevel>();
            List<WarehouseReorderLevelViewModel> objWRLVM = new List<WarehouseReorderLevelViewModel>();
            try
            {
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }

                if (WarehouseID > 0)
                {
                    lWarehouseReorderLevel = db.WarehouseReorderLevels.Where(x => x.WarehouseID == WarehouseID).OrderByDescending(x => x.AvailableQuantity).ToList();
                    objWS.WarehouseName = db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.Name).FirstOrDefault();
                    var lWarehouseStocks = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID).ToList();//Added by Sonali for Available stock qty on 16-03-2019
                    foreach (var item in lWarehouseReorderLevel)
                    {
                        WarehouseReorderLevelViewModel objPOD = new WarehouseReorderLevelViewModel();
                        objPOD.ID = item.ID;
                        objPOD.AvailableQuantity = lWarehouseStocks.Where(x => x.ProductVarientID == item.ProductVarientID).Select(x => x.AvailableQuantity).Sum();//Added by Sonali for Available stock qty on 16-03-2019
                        objPOD.ReorderLevel = item.ReorderLevel;
                        objPOD.ProductID = item.ProductID;
                        objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);

                        objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                        var itemName = (from p in db.Products
                                        join v in db.ProductVarients on p.ID equals v.ProductID
                                        join s in db.Sizes on v.SizeID equals s.ID
                                        where v.ID == item.ProductVarientID
                                        select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                        foreach (var i in itemName)
                        {
                            objPOD.ItemName = i.ItemName.ToString();
                            objPOD.HSNCode = Convert.ToString(i.HSNCode);
                        }
                        objWRLVM.Add(objPOD);
                    }

                }
                if (Session["USER_NAME"] != null)
                {

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                //Yashaswi 31-7-2018
                Session["WarehouseReorderLevelViewModel"] = objWRLVM;
                objWS.lWarehouseReorderLevelViewModel = objWRLVM;
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("Stock", objWS);
        }



        public ActionResult StockBatchwise(long ID)
        {
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<WarehouseStock> lWarehouseStock = new List<WarehouseStock>();
            List<WarehouseStockViewModel> objWRLVM = new List<WarehouseStockViewModel>();
            try
            {
                //long WarehouseID = 0;
                //if (Session["WarehouseID"] != null)
                //{
                //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                //}
                var query = db.WarehouseReorderLevels.Where(x => x.ID == ID).FirstOrDefault();
                objWS.WarehouseName = db.Warehouses.Where(x => x.ID == query.WarehouseID).Select(x => x.Name).FirstOrDefault();

                //Get all batches of product from stock
                lWarehouseStock = db.WarehouseStocks.Where(x => x.WarehouseID == query.WarehouseID && x.ProductID == query.ProductID && x.ProductVarientID == query.ProductVarientID && x.AvailableQuantity > 0).OrderBy(x => x.AvailableQuantity).ToList();

                if (query.WarehouseID > 0)
                {

                    foreach (var item in lWarehouseStock)
                    {
                        WarehouseStockViewModel objWarehouseStockViewModel = new WarehouseStockViewModel();
                        objWarehouseStockViewModel.ID = item.ID;
                        objWarehouseStockViewModel.BatchCode = item.BatchCode;
                        objWarehouseStockViewModel.ProductID = item.ProductID;
                        objWarehouseStockViewModel.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                        objWarehouseStockViewModel.MRP = Convert.ToDecimal(item.MRP);
                        objWarehouseStockViewModel.BuyRatePerUnit = item.BuyRatePerUnit;
                        objWarehouseStockViewModel.SaleRatePerUnit = Convert.ToDecimal(item.SaleRatePerUnit);
                        objWarehouseStockViewModel.InitialQuantity = item.InitialQuantity;
                        objWarehouseStockViewModel.AvailableQuantity = item.AvailableQuantity;
                        objWarehouseStockViewModel.ExpiryDate = item.ExpiryDate;


                        objWarehouseStockViewModel.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

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

                        //
                        long PurchaseOrederId = db.Invoices.FirstOrDefault(p => p.ID == item.InvoiceID).PurchaseOrderID;
                        var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrederId);
                        if (purchaseOrder.DVId != 0 && purchaseOrder.FVId != 0)
                        {
                            objWarehouseStockViewModel.PurchaseFor = "DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                        }
                        objWRLVM.Add(objWarehouseStockViewModel);
                    }
                }
                if (Session["USER_NAME"] != null)
                {

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("StockBatchwise", objWRLVM);
        }


        //
        // GET: /WarehouseStocks/Delete/5

        public ActionResult Delete(long id)
        {
            WarehouseStock warehousestock = db.WarehouseStocks.Single(x => x.ID == id);
            return View(warehousestock);
        }

        //
        // POST: /WarehouseStocks/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            WarehouseStock warehousestock = db.WarehouseStocks.Single(x => x.ID == id);
            db.WarehouseStocks.Remove(warehousestock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel()
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            string FileName = "";
            Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
            if (obj != null)
            {
                FileName = obj.Name;
            }
            FileName = FileName + " Product List";
            List<ProductReorderExport> list = new List<ProductReorderExport>();
            if (Session["WarehouseReorderLevelViewModel"] != null)
            {
                List<WarehouseReorderLevelViewModel> objWRLVM = (List<WarehouseReorderLevelViewModel>)Session["WarehouseReorderLevelViewModel"];

                if (objWRLVM.Count != 0)
                {
                    foreach (var item in objWRLVM)
                    {
                        ProductReorderExport o = new ProductReorderExport();
                        o.ProductName = item.ItemName;
                        o.AvailableQty = item.AvailableQuantity;
                        o.ReorderQyt = item.ReorderLevel;
                        list.Add(o);
                    }
                }
            }


            var gv = new GridView();
            gv.DataSource = list;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Stock");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}