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
using Inventory.Common;
using System.Web.Configuration;
using System.IO;
using System.Net;


namespace Inventory.Controllers
{
    public class InvoicesController : Controller
    {
        //
        // GET: /Invoice/
        public class ForLoopClass //----------------use this class for loop purpose in below functions--------------
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController obj_comm = new CommonController();
        //
        // GET: /PurchaseOrders/

        public ActionResult PurchaseOrderList()
        {
            PurchaseOrderViewModelList objPo = new PurchaseOrderViewModelList();
            List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
            CommonController obj_comm = new CommonController();
            try
            {
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

                var wSupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();
                //string str = string.Join(",", db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == 4)
                //                                 .Select(p => p.ProductNickname.ToString()));

                if (WarehouseID > 0)
                {
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join pd in db.PurchaseOrderReply on o.ID equals pd.PurchaseOrderID into tmpGroups
                                               from pd in tmpGroups.DefaultIfEmpty()
                                               where w.ID == WarehouseID && (pd.IsReplied == true || s.WarehouseID == null)
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   //Items = string.Join(",", db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == o.ID)
                                                   //  .Select(p => p.ProductNickname.ToString())),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate
                                               }).Distinct().OrderByDescending(o => o.OrderDate).ToList();
                }
                else
                {
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join pd in db.PurchaseOrderReply on o.ID equals pd.PurchaseOrderID
                                               where pd.IsReplied == true || s.WarehouseID == null
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate
                                               }).Distinct().OrderByDescending(o => o.OrderDate).ToList();
                }
                Session["lPurchaseOrderDetailsViewModel"] = null;
                Session["Amount"] = null;

                //ViewBag.PossibleSuppliers = db.Suppliers.Where(x => x.IsActive == true && x.ContactPerson != "" && x.ID != wSupplierID);
                //yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = obj_comm.GetSupplierLIst(WarehouseID);
                objPo.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();
                return View("PurchaseOrderList", objPo);
            }
            catch (Exception ex)
            {

            }
            return View("PurchaseOrderList", lPurchaseOrderViewModel);
        }


        public ActionResult Search(long? SupplierID)
        {
            PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
            List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
            try
            {

                if (Session["USER_NAME"] != null)
                { }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                List<QuotationSupplierListViewModel> lQuotationSupplierListViewModel = new List<QuotationSupplierListViewModel>();
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }
                var wSupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID).Select(x => x.ID).FirstOrDefault();

                if (SupplierID != null && SupplierID > 0)
                {
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join pd in db.PurchaseOrderReply on o.ID equals pd.PurchaseOrderID into tmpGroups
                                               from pd in tmpGroups.DefaultIfEmpty()
                                               where w.ID == WarehouseID && (pd.IsReplied == true || s.WarehouseID == null)
                                               && o.SupplierID == SupplierID
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   //Items = string.Join(",", db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == o.ID)
                                                   //  .Select(p => p.ProductNickname.ToString())),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate
                                               }).OrderByDescending(o => o.OrderDate).ToList();
                }
                else
                {
                    return RedirectToAction("PurchaseOrderList");
                }

                Session["lQuotationItemDetailViewModel"] = null;

                //ViewBag.PossibleSuppliers = db.Suppliers.Where(x => x.IsActive == true && x.ContactPerson != "" && x.ID != wSupplierID);
                //yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = obj_comm.GetSupplierLIst(WarehouseID);
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("PurchaseOrderList", objPO);
        }

        public ActionResult Index(long? PurchaseOrderID)
        {
            List<InvoiceViewModel> lInvoiceViewModel = new List<InvoiceViewModel>();
            try
            {
                ViewBag.PurchaseOrderID = PurchaseOrderID;

                ViewBag.IsPOCompleted = IsPOCompleted(Convert.ToInt64(PurchaseOrderID));
                ViewBag.PurchaseOrderCode = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.PurchaseOrderCode).FirstOrDefault();

                lInvoiceViewModel = (from o in db.Invoices
                                     where o.PurchaseOrderID == PurchaseOrderID
                                     select new InvoiceViewModel
                                     {
                                         InvoiceID = o.ID,
                                         PurchaseOrderID = o.PurchaseOrderID,
                                         OrderAmount = o.OrderAmount,
                                         InvoiceDate = o.InvoiceDate,
                                         InvoiceCode = o.InvoiceCode,
                                         TotalDiscountAmount = o.TotalDiscountAmount,
                                         ShippingCharge = o.ShippingCharge,
                                         CustomDutyCharge = o.CustomDutyCharge,
                                         OperatingCost = o.OperatingCost,
                                         AdditionalCost = o.AdditionalCost,
                                         TotalAmount = o.TotalAmount,
                                         TotalItems = db.InvoiceDetails.Where(x => x.InvoiceID == o.ID).Select(x => x.ID).Count(),
                                         IsApproved = o.IsApproved
                                     }).OrderByDescending(o => o.InvoiceDate).ToList();

                //Yashaswi Display Invoice Attachment 
                CommonController obj_commonController = new CommonController();
                lInvoiceViewModel.ForEach(p =>
                {
                    if (db.InvoiceAttachment.Any(q => q.InvoiceID == p.InvoiceID))
                    {
                        p.Invoice_Attachment = obj_commonController.GetFileName(db.InvoiceAttachment.SingleOrDefault(q => q.InvoiceID == p.InvoiceID).FileName);
                        p.Invoice_AttachmentFileName = db.InvoiceAttachment.SingleOrDefault(q => q.InvoiceID == p.InvoiceID).FileName;
                    }
                });

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
            return View("Index", lInvoiceViewModel);
        }

        //
        // GET: /PurchaseOrders/Details/5

        public ViewResult Details(long InvoiceID)
        {
            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

            if (Invoice != null && Invoice.ToString() != "")
            {
                if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
                {
                    ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                }
                InvoiceViewModel.InvoiceID = InvoiceID;
                InvoiceViewModel.InvoiceCode = Invoice.InvoiceCode;
                InvoiceViewModel.PurchaseOrderID = PurchaseOrderID;
                InvoiceViewModel.WarehouseID = purchaseOrder.WarehouseID;
                InvoiceViewModel.WarehouseName = db.Warehouses.Where(x => x.ID == purchaseOrder.WarehouseID).Select(x => x.Name).FirstOrDefault();
                InvoiceViewModel.SupplierID = purchaseOrder.SupplierID;
                InvoiceViewModel.SupplierName = db.Suppliers.Where(x => x.ID == purchaseOrder.SupplierID).Select(x => x.Name).FirstOrDefault();
                InvoiceViewModel.InvoiceDate = Invoice.InvoiceDate;
                InvoiceViewModel.TotalDiscountAmount = Convert.ToDecimal(Invoice.TotalDiscountAmount);
                InvoiceViewModel.ShippingCharge = Convert.ToDecimal(Invoice.ShippingCharge);
                InvoiceViewModel.CustomDutyCharge = Convert.ToDecimal(Invoice.CustomDutyCharge);
                InvoiceViewModel.OperatingCost = Convert.ToDecimal(Invoice.OperatingCost);
                InvoiceViewModel.AdditionalCost = Convert.ToDecimal(Invoice.AdditionalCost);
                InvoiceViewModel.OrderAmount = Convert.ToDecimal(Invoice.OrderAmount);
                InvoiceViewModel.TotalAmount = Convert.ToDecimal(Invoice.TotalAmount);
                InvoiceViewModel.Remark = Invoice.Remark == null ? "" : Invoice.Remark;
                InvoiceViewModel.IsActive = Invoice.IsActive;
            }

            //List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
            //lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

            List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
            lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();

            List<InvoiceDetailViewModel> lInvoiceDetailViewModelList = new List<InvoiceDetailViewModel>();


            foreach (InvoiceDetail item in lInvoiceDetaillist)
            {
                InvoiceDetailViewModel objPOD = new InvoiceDetailViewModel();
                //foreach (var inv in lInvoiceDetaillist)
                //{
                //    if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
                //    {
                objPOD.ReceivedQuantity = item.ReceivedQuantity;
                objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                objPOD.MRP = item.MRP;
                objPOD.SaleRate = item.SaleRate;
                objPOD.GSTInPer = item.GSTInPer;
                objPOD.CGSTAmount = Convert.ToDecimal(item.CGSTAmount * item.ReceivedQuantity);
                objPOD.SGSTAmount = Convert.ToDecimal(item.SGSTAmount * item.ReceivedQuantity);
                objPOD.IGSTAmount = Convert.ToDecimal(item.IGSTAmount * item.ReceivedQuantity);
                objPOD.Amount = Convert.ToDecimal(item.Amount);
                objPOD.ExpiryDate = item.ExpiryDate;
                objPOD.Remark = item.Remark == null ? "" : item.Remark;

                //var expirydate = db.WarehouseStocks.Where(x => x.InvoiceID == InvoiceID && x.ProductID == inv.ProductID && x.ProductVarientID == inv.ProductVarientID).Select(x => x.ExpiryDate).FirstOrDefault();
                //objPOD.ExpiryDate = expirydate;
                //    }
                //}

                objPOD.PurchaseOrderDetailsID = item.ID;
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

                lInvoiceDetailViewModelList.Add(objPOD);
            }
            if (lInvoiceDetailViewModelList.Count > 0)
            {
                Session["lInvoiceDetailViewModel"] = lInvoiceDetailViewModelList;
                InvoiceViewModel.lInvoiceDetailViewModels = lInvoiceDetailViewModelList;
            }
            //Yashaswi To display Attachment
            CommonController obj_commonController = new CommonController();
            if (db.InvoiceAttachment.Any(p => p.InvoiceID == InvoiceID))
            {
                InvoiceViewModel.Invoice_Attachment = obj_commonController.GetFileName(db.InvoiceAttachment.SingleOrDefault(p => p.InvoiceID == InvoiceID).FileName);
                InvoiceViewModel.Invoice_AttachmentFileName = db.InvoiceAttachment.SingleOrDefault(p => p.InvoiceID == InvoiceID).FileName;
            }
            return View(InvoiceViewModel);
        }

        //
        // GET: /PurchaseOrders/Create

        public ActionResult Create(long PurchaseOrderID)
        {
            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            RateMatrixController objPR = new RateMatrixController();
            try
            {
                long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
                //Yashaswi 9/4/2018
                ViewBag.PossibleWarehouses = obj_comm.GetFVList(warehouseID);

                ViewBag.WarehouseID = warehouseID;
                Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == warehouseID);

                //Yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = obj_comm.GetSupplierLIst(warehouseID);
                Session["lPurchaseOrderDetailsViewModel"] = null;
                Session["Amount"] = null;

                var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);

                if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
                {
                    ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                }

                if (purchaseOrder != null && purchaseOrder.ToString() != "")
                {
                    InvoiceViewModel.PurchaseOrderID = PurchaseOrderID;
                    InvoiceViewModel.WarehouseID = purchaseOrder.WarehouseID;
                    InvoiceViewModel.SupplierID = purchaseOrder.SupplierID;
                    //Session["Amount"] = purchaseOrder.Amount;
                    InvoiceViewModel.InvoiceDate = System.DateTime.Now;
                    InvoiceViewModel.IsActive = purchaseOrder.IsActive;
                }

                List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
                lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

                List<InvoiceDetailViewModel> lInvoiceDetailViewModelList = new List<InvoiceDetailViewModel>();

                foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
                {
                    InvoiceDetailViewModel objPOD = new InvoiceDetailViewModel();
                    Rate objRate = new Rate();
                    int receivedQuantity = 0;
                    long poid = db.InvoiceDetails.Where(x => x.PurchaseOrderDetailID == item.ID).Select(x => x.PurchaseOrderDetailID).FirstOrDefault();
                    if (poid > 0)
                    {
                        receivedQuantity = Convert.ToInt32(db.InvoiceDetails.Where(x => x.PurchaseOrderDetailID == item.ID).Sum(x => x.ReceivedQuantity == null ? 0 : x.ReceivedQuantity));
                    }
                    if (receivedQuantity != item.Quantity)
                    {
                        objPOD.PurchaseOrderDetailsID = item.ID;
                        objPOD.ProductID = item.ProductID;
                        objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                        objPOD.Quantity = item.Quantity - receivedQuantity;
                        objPOD.BuyRatePerUnit = item.UnitPrice;
                        objPOD.ProductAmount = item.Quantity * item.UnitPrice;
                        objPOD.Nickname = item.ProductNickname;
                        PurchaseOrderDetail objPurchaseOrderDetail = db.PurchaseOrderDetails.FirstOrDefault(p => p.ID == item.ID);
                        if (obj_warehouse.Entity.Trim() == "EVW")
                        {
                            objRate = objPR.GetRateForGRN_EVW(item.RateMatrixId.Value, purchaseOrder.DVId.Value, purchaseOrder.FVId.Value, purchaseOrder.WarehouseID);
                            objPurchaseOrderDetail.RateMatrixExtensionId = objRate.RateMatrixExtensionId;
                            db.SaveChanges();
                        }
                        else if (obj_warehouse.Entity.Trim() == "DV")
                        {
                            objRate = objPR.GetRateForGRN_DV(item.RateMatrixId.Value, purchaseOrder.DVId.Value, purchaseOrder.FVId.Value, purchaseOrder.WarehouseID, objPurchaseOrderDetail.RateMatrixExtensionId.Value);
                        }
                        else if (obj_warehouse.Entity.Trim() == "FV")
                        {
                            objRate = objPR.GetRateForGRN_FV(item.RateMatrixId.Value, purchaseOrder.DVId.Value, purchaseOrder.FVId.Value, purchaseOrder.WarehouseID, objPurchaseOrderDetail.RateMatrixExtensionId.Value);
                        }

                        objPOD.MRP = Convert.ToDecimal(objRate.MRP);
                        objPOD.SaleRate = Convert.ToDecimal(objRate.SaleRate);
                        objPOD.GSTInPer = Convert.ToInt32(objRate.GST);
                        objPOD.CGSTAmount = Convert.ToDecimal(objRate.TotalGST / Convert.ToDouble(2));
                        objPOD.SGSTAmount = Convert.ToDecimal(objRate.TotalGST / Convert.ToDouble(2));

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

                        lInvoiceDetailViewModelList.Add(objPOD);
                    }
                }

                if (lInvoiceDetailViewModelList.Count > 0)
                {
                    Session["lInvoiceDetailViewModel"] = lInvoiceDetailViewModelList;
                    InvoiceViewModel.lInvoiceDetailViewModels = lInvoiceDetailViewModelList;
                }
                else
                {
                    Session["Info"] = purchaseOrder.PurchaseOrderCode + "Order has completed! You can't create new Invoice."; //yashaswi31/3/2018
                    return RedirectToAction("Index", new { PurchaseOrderID = PurchaseOrderID });
                }

                ViewBag.PurchaseOrderID = PurchaseOrderID;

            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            ViewBag.PurchaseOrderID = PurchaseOrderID;
            return View(InvoiceViewModel);
        }

        private bool IsPOCompleted(long PurchaseOrderID)
        {
            bool flag = true;
            try
            {
                int receivedQuantity = 0;
                List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
                lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();
                foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
                {
                    receivedQuantity = Convert.ToInt32(db.InvoiceDetails.Where(x => x.PurchaseOrderDetailID == item.ID).DefaultIfEmpty().Sum(x => x.ReceivedQuantity == null ? 0 : x.ReceivedQuantity));
                    if (receivedQuantity != item.Quantity)
                    {
                        flag = false;
                        return flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PurchaseOrderReplyController][IsPOCompleted]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return flag;
        }

        public PartialViewResult Add(int ProductID, long ProductVarientID, int Quantity, string Nickname, decimal UnitPrice)
        {
            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModel = new List<PurchaseOrderDetailsViewModel>();

            if (Session["lInvoiceDetailViewModel"] != null)
            {
                lPurchaseOrderDetailsViewModel = (List<PurchaseOrderDetailsViewModel>)Session["lInvoiceDetailViewModel"];
            }

            PurchaseOrderDetailsViewModel objd = new PurchaseOrderDetailsViewModel();

            var itemName = (from p in db.Products
                            join v in db.ProductVarients on p.ID equals v.ProductID
                            join s in db.Sizes on v.SizeID equals s.ID
                            where v.ID == ProductVarientID
                            select new PurchaseOrderDetailsViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

            objd.StockThumbPath = ImageDisplay.SetProductThumbPath(ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            objd.PurchaseOrderDetailsID = 0;
            objd.ProductID = ProductID;
            objd.ProductVarientID = ProductVarientID;
            objd.UnitPrice = UnitPrice;
            objd.ProductAmount = Quantity * UnitPrice;

            foreach (var item in itemName)
            {
                objd.ItemName = item.ItemName.ToString();
                objd.HSNCode = Convert.ToString(item.HSNCode);
            }
            objd.Quantity = Quantity;
            objd.Nickname = Nickname;

            decimal pAmount = UnitPrice * Quantity;

            if (Session["Amount"] != null)
            {
                decimal Amount = Convert.ToDecimal(Session["Amount"].ToString());
                Session["Amount"] = Amount + pAmount;
            }
            else
            {
                Session["Amount"] = pAmount;
            }


            var vId = lPurchaseOrderDetailsViewModel.Find(x => x.ProductVarientID == ProductVarientID);
            if (vId == null)
            {
                lPurchaseOrderDetailsViewModel.Add(objd);
            }


            Session["lInvoiceDetailViewModel"] = lPurchaseOrderDetailsViewModel;

            return PartialView("ItemDetails", lPurchaseOrderDetailsViewModel);
        }


        public PartialViewResult Remove(long ProductVarientID)
        {
            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModel = new List<PurchaseOrderDetailsViewModel>();

            if (Session["lInvoiceDetailViewModel"] != null)
            {
                lPurchaseOrderDetailsViewModel = (List<PurchaseOrderDetailsViewModel>)Session["lInvoiceDetailViewModel"];
                var id = lPurchaseOrderDetailsViewModel.First(x => x.ProductVarientID == ProductVarientID);

                decimal UnitPrice = id.UnitPrice;
                decimal Quantity = id.Quantity;

                decimal pAmount = UnitPrice * Quantity;
                decimal Amount = Convert.ToDecimal(Session["Amount"].ToString());
                Session["Amount"] = Amount - pAmount;

                lPurchaseOrderDetailsViewModel.Remove(id);
                Session["lInvoiceDetailViewModel"] = lPurchaseOrderDetailsViewModel;
            }

            return PartialView("ItemDetails", lPurchaseOrderDetailsViewModel);
        }

        //
        // POST: /PurchaseOrders/Create


        [HttpPost]
        public ActionResult Create(InvoiceViewModel Invoice, HttpPostedFileBase file)
        {
            List<InvoiceDetailViewModel> lInvoiceDetailViewModel = new List<InvoiceDetailViewModel>();

            lInvoiceDetailViewModel = Invoice.lInvoiceDetailViewModels;

            if (lInvoiceDetailViewModel.Count > 0)
            {

                if (Session["Entity"] != null && Convert.ToString(Session["Entity"].ToString()) != "EVW")
                {
                    var invoiceCode = db.PurchaseOrderReply.Where(x => x.InvoiceCode == Invoice.InvoiceCode && x.PurchaseOrderID == Invoice.PurchaseOrderID && x.IsReplied == true).Select(y => y.ID).FirstOrDefault();
                    if (invoiceCode == 0)
                    {
                        Session["Error"] = "Invoice Code is Invalid!"; //Zubair on 05-04-2018
                        return View(Invoice);
                    }
                }

                if (ModelState.IsValid)
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            if (Invoice.OrderAmount > 0)
                            {
                                //Insert into PurchaseOrder Table
                                Invoice objInvoice = new Invoice();
                                Invoice.TotalAmount = (Convert.ToDecimal(Invoice.OrderAmount) + Convert.ToDecimal(Invoice.ShippingCharge) + Convert.ToDecimal(Invoice.CustomDutyCharge) + Convert.ToDecimal(Invoice.OperatingCost) + Convert.ToDecimal(Invoice.AdditionalCost)) - Convert.ToDecimal(Invoice.TotalDiscountAmount);
                                objInvoice.PurchaseOrderID = Invoice.PurchaseOrderID;
                                objInvoice.InvoiceDate = Invoice.InvoiceDate;
                                objInvoice.InvoiceCode = Invoice.InvoiceCode;
                                objInvoice.TotalDiscountAmount = Invoice.TotalDiscountAmount;
                                objInvoice.OrderAmount = Invoice.OrderAmount;
                                objInvoice.ShippingCharge = Invoice.ShippingCharge;
                                objInvoice.CustomDutyCharge = Invoice.CustomDutyCharge;
                                objInvoice.OperatingCost = Invoice.OperatingCost;
                                objInvoice.AdditionalCost = Invoice.AdditionalCost;
                                objInvoice.TotalAmount = Invoice.TotalAmount;
                                objInvoice.Remark = Invoice.Remark;
                                objInvoice.IsActive = true;
                                objInvoice.CreateDate = DateTime.Now;
                                objInvoice.CreateBy = GetPersonalDetailID();
                                objInvoice.NetworkIP = CommonFunctions.GetClientIP();
                                objInvoice.DeviceID = "X";
                                objInvoice.DeviceType = "X";
                                db.Invoices.Add(objInvoice);
                                db.SaveChanges();

                                long InvoiceID = objInvoice.ID;

                                //Insert into InvoiceDetail Table
                                InvoiceDetail objInvoiceDetail = new InvoiceDetail();
                                foreach (InvoiceDetailViewModel item in lInvoiceDetailViewModel)
                                {
                                    if (item.ReceivedQuantity > 0)
                                    {
                                        objInvoiceDetail.InvoiceID = InvoiceID;
                                        objInvoiceDetail.PurchaseOrderDetailID = item.PurchaseOrderDetailsID;
                                        objInvoiceDetail.ProductID = item.ProductID;
                                        objInvoiceDetail.ProductVarientID = item.ProductVarientID;
                                        objInvoiceDetail.IsExtraItem = false;
                                        objInvoiceDetail.BuyRatePerUnit = item.BuyRatePerUnit;
                                        objInvoiceDetail.MRP = item.MRP;
                                        objInvoiceDetail.SaleRate = item.SaleRate;
                                        objInvoiceDetail.ReceivedQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                        objInvoiceDetail.Amount = Convert.ToDecimal(item.Amount);
                                        objInvoiceDetail.GSTInPer = item.GSTInPer;
                                        objInvoiceDetail.CGSTAmount = Convert.ToDecimal(item.CGSTAmount);
                                        objInvoiceDetail.SGSTAmount = Convert.ToDecimal(item.SGSTAmount);
                                        objInvoiceDetail.IGSTAmount = Convert.ToDecimal(item.IGSTAmount);
                                        objInvoiceDetail.ExpiryDate = item.ExpiryDate;
                                        objInvoiceDetail.Remark = item.Remark;
                                        objInvoiceDetail.IsActive = true;
                                        db.InvoiceDetails.Add(objInvoiceDetail);
                                        db.SaveChanges();
                                    }
                                }
                                //start Yashaswi 21/3/2018 Invoice Attachment
                                Save_InvoiceAttachment(file, InvoiceID);
                                //end
                                tscope.Complete();
                                Session["Success"] = "Invoice Created Successfully."; //yashaswi 31/3/2018
                            }
                        }
                        catch (Exception ex)
                        {
                            Transaction.Current.Rollback();
                            tscope.Dispose();
                            throw ex;
                        }
                    }

                    return RedirectToAction("Index", new { PurchaseOrderID = Invoice.PurchaseOrderID });
                }
            }
            //}
            //Yashaswi 9/4/2018
            ViewBag.PossibleWarehouses = obj_comm.GetFVList(Convert.ToInt64(Session["WarehouseID"]));
            ViewBag.PossibleSuppliers = obj_comm.GetSupplierLIst(Convert.ToInt64(Session["WarehouseID"]));
            return View(Invoice);
        }


        //yashaswi 26/3/2018 Invoice Attchment
        public void Save_InvoiceAttachment(HttpPostedFileBase file, long InvoiceID)
        {
            CommonController obj_commonController = new CommonController();

            if (file != null)
            {
                bool IsSaved = false;
                string Filename = "";
                string Ext = "";
                IsSaved = obj_commonController.UploadImage((int)Constants.Inventory_Image_Type.INVOICE, file, InvoiceID, System.Web.HttpContext.Current.Server, out Filename, out Ext);
                if (IsSaved)
                {
                    InvoiceAttachment obj_InvoiceAttachment = new InvoiceAttachment();
                    obj_InvoiceAttachment.InvoiceID = InvoiceID;
                    obj_InvoiceAttachment.IsActive = true;
                    obj_InvoiceAttachment.FileName = Filename;
                    obj_InvoiceAttachment.Extention = Ext;
                    db.InvoiceAttachment.Add(obj_InvoiceAttachment);
                    db.SaveChanges();
                }

            }
        }

        private string GetNextPurchaseOrderCode()
        {
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "WHPO" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lWHPO = GetNextWarehousePurchaseOrderCode();
                if (lWHPO > 0)
                {
                    newOrderCode = lOrderPrefix + lWHPO.ToString("00000");
                    return newOrderCode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        private int GetNextWarehousePurchaseOrderCode()
        {
            int lWHPO = -1;

            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("SelectNextWarehousePurchaseOrderCode", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                con.Open();
                //object o = sqlComm.ExecuteScalar();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    lWHPO = Convert.ToInt32(dt.Rows[0][0]);
                }
                con.Close();
                return lWHPO;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PurchaseOrders -> GetNextWarehousePurchaseOrderCode]", "Problem in getting EZPO" + Environment.NewLine + ex.Message);
            }
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

        //
        // GET: /PurchaseOrders/Edit/5

        public ActionResult Edit(long InvoiceID)
        {
            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;
            long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
            ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == warehouseID);
            ViewBag.WarehouseID = warehouseID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
            {
                ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
            }
            var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

            if (Invoice != null && Invoice.ToString() != "")
            {
                InvoiceViewModel.InvoiceID = InvoiceID;
                InvoiceViewModel.PurchaseOrderID = PurchaseOrderID;
                InvoiceViewModel.WarehouseID = purchaseOrder.WarehouseID;
                InvoiceViewModel.SupplierID = purchaseOrder.SupplierID;
                InvoiceViewModel.InvoiceDate = Invoice.InvoiceDate;
                InvoiceViewModel.InvoiceCode = Invoice.InvoiceCode;
                InvoiceViewModel.TotalDiscountAmount = Invoice.TotalDiscountAmount;
                InvoiceViewModel.ShippingCharge = Invoice.ShippingCharge;
                InvoiceViewModel.CustomDutyCharge = Invoice.CustomDutyCharge;
                InvoiceViewModel.OperatingCost = Invoice.OperatingCost;
                InvoiceViewModel.AdditionalCost = Invoice.AdditionalCost;
                InvoiceViewModel.OrderAmount = Invoice.OrderAmount;
                InvoiceViewModel.Remark = Invoice.Remark;
                InvoiceViewModel.IsActive = Invoice.IsActive;
            }

            List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
            lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

            List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
            lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();

            List<InvoiceDetailViewModel> lInvoiceDetailViewModelList = new List<InvoiceDetailViewModel>();

            //foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
            //{

            foreach (var inv in lInvoiceDetaillist)
            {
                int receivedQuantity = 0;
                int requiredQuantity = 0;

                requiredQuantity = Convert.ToInt32(db.PurchaseOrderDetails.Where(x => x.ID == inv.PurchaseOrderDetailID).Select(x => x.Quantity).FirstOrDefault());
                receivedQuantity = Convert.ToInt32(db.InvoiceDetails.Where(x => x.PurchaseOrderDetailID == inv.PurchaseOrderDetailID).Sum(x => x.ReceivedQuantity == null ? 0 : x.ReceivedQuantity));


                InvoiceDetailViewModel objPOD = new InvoiceDetailViewModel();
                //if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
                //{
                objPOD.ReceivedQuantity = inv.ReceivedQuantity;
                objPOD.MRP = inv.MRP;
                objPOD.SaleRate = inv.SaleRate;
                objPOD.GSTInPer = inv.GSTInPer;
                objPOD.CGSTAmount = inv.CGSTAmount;
                objPOD.SGSTAmount = inv.SGSTAmount;
                objPOD.IGSTAmount = inv.IGSTAmount;
                objPOD.Amount = Convert.ToDecimal(inv.Amount);
                objPOD.ExpiryDate = inv.ExpiryDate;
                objPOD.Remark = inv.Remark;

                objPOD.PurchaseOrderDetailsID = inv.PurchaseOrderDetailID;
                objPOD.ProductID = inv.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(inv.ProductVarientID);
                objPOD.Quantity = requiredQuantity - receivedQuantity + inv.ReceivedQuantity;
                objPOD.BuyRatePerUnit = inv.BuyRatePerUnit;
                //objPOD.ProductAmount = item.Quantity * item.UnitPrice;                      

                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(inv.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == inv.ProductVarientID
                                select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }

                //int ReorderLevel = db.WarehouseReorderLevels.Where(x => x.WarehouseID == purchaseOrder.WarehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID).Select(x => x.ReorderLevel).FirstOrDefault();
                //objPOD.ReorderLevel = ReorderLevel;

                lInvoiceDetailViewModelList.Add(objPOD);
                //}
            }


            //}
            if (lInvoiceDetailViewModelList.Count > 0)
            {
                Session["lInvoiceDetailViewModel"] = lInvoiceDetailViewModelList;
            }

            InvoiceViewModel.lInvoiceDetailViewModels = lInvoiceDetailViewModelList;


            ViewBag.PurchaseOrderID = PurchaseOrderID;

            //Start yashaswi 26/3/2018 Check for Attachment
            CommonController obj_commonController = new CommonController();
            if (db.InvoiceAttachment.Any(p => p.InvoiceID == InvoiceID))
            {
                InvoiceViewModel.Invoice_Attachment = obj_commonController.GetFileName(db.InvoiceAttachment.SingleOrDefault(p => p.InvoiceID == InvoiceID).FileName);
                InvoiceViewModel.Invoice_AttachmentFileName = db.InvoiceAttachment.SingleOrDefault(p => p.InvoiceID == InvoiceID).FileName;
            }
            //End
            return View(InvoiceViewModel);
        }

        //
        // POST: /PurchaseOrders/Edit/5

        [HttpPost]
        public ActionResult Edit(InvoiceViewModel Invoice, HttpPostedFileBase file)
        {
            List<InvoiceDetailViewModel> lInvoiceDetailViewModel = new List<InvoiceDetailViewModel>();

            //if (Session["lPurchaseOrderDetailsViewModel"] != null && Session["Amount"] != null)
            //{
            lInvoiceDetailViewModel = Invoice.lInvoiceDetailViewModels;
            if (lInvoiceDetailViewModel.Count > 0)
            {
                if (Session["Entity"] != null && Convert.ToString(Session["Entity"].ToString()) != "EVW")
                {
                    var invoiceCode = db.PurchaseOrderReply.Where(x => x.InvoiceCode == Invoice.InvoiceCode && x.PurchaseOrderID == Invoice.PurchaseOrderID && x.IsReplied == true).Select(y => y.ID).FirstOrDefault();
                    if (invoiceCode == 0)
                    {
                        Session["Error"] = "Invoice Code is Invalid!"; //Zubair on 05-04-2018
                        return View(Invoice);
                    }
                }

                if (ModelState.IsValid)
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            if (Invoice.OrderAmount > 0)
                            {
                                //Insert into PurchaseOrder Table                                 

                                var lInvoice = new Invoice()
                                {
                                    ID = Invoice.InvoiceID,
                                    TotalAmount = (Convert.ToDecimal(Invoice.OrderAmount) + Convert.ToDecimal(Invoice.ShippingCharge) + Convert.ToDecimal(Invoice.CustomDutyCharge) + Convert.ToDecimal(Invoice.OperatingCost) + Convert.ToDecimal(Invoice.AdditionalCost)) - Convert.ToDecimal(Invoice.TotalDiscountAmount),
                                    InvoiceDate = Invoice.InvoiceDate,
                                    InvoiceCode = Invoice.InvoiceCode,
                                    TotalDiscountAmount = Invoice.TotalDiscountAmount,
                                    OrderAmount = Invoice.OrderAmount,
                                    ShippingCharge = Invoice.ShippingCharge,
                                    CustomDutyCharge = Invoice.CustomDutyCharge,
                                    OperatingCost = Invoice.OperatingCost,
                                    AdditionalCost = Invoice.AdditionalCost,
                                    Remark = Invoice.Remark,
                                    ModifyDate = DateTime.Now,
                                    ModifyBy = GetPersonalDetailID(),
                                    NetworkIP = CommonFunctions.GetClientIP()
                                };

                                db.Invoices.Attach(lInvoice);
                                db.Entry(lInvoice).Property(x => x.TotalAmount).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.InvoiceDate).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.InvoiceCode).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.TotalDiscountAmount).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.OrderAmount).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.ShippingCharge).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.CustomDutyCharge).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.OperatingCost).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.AdditionalCost).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.Remark).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.ModifyDate).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.ModifyBy).IsModified = true;
                                db.Entry(lInvoice).Property(x => x.NetworkIP).IsModified = true;
                                db.SaveChanges();


                                //Insert into InvoiceDetail Table
                                InvoiceDetail objInvoiceDetail = new InvoiceDetail();
                                foreach (InvoiceDetailViewModel item in lInvoiceDetailViewModel)
                                {
                                    long id = 0;
                                    id = db.InvoiceDetails.Where(x => x.InvoiceID == Invoice.InvoiceID && x.PurchaseOrderDetailID == item.PurchaseOrderDetailsID).Select(x => x.ID).FirstOrDefault();
                                    if (id > 0)
                                    {
                                        if (item.ReceivedQuantity > 0)
                                        {
                                            var lInvoiceDetail = new InvoiceDetail()
                                            {
                                                ID = id,
                                                PurchaseOrderDetailID = item.PurchaseOrderDetailsID,
                                                ProductID = item.ProductID,
                                                ProductVarientID = item.ProductVarientID,
                                                BuyRatePerUnit = item.BuyRatePerUnit,
                                                MRP = item.MRP,
                                                SaleRate = item.SaleRate,
                                                ReceivedQuantity = Convert.ToInt32(item.ReceivedQuantity),
                                                Amount = item.Amount,
                                                GSTInPer = item.GSTInPer,
                                                CGSTAmount = item.CGSTAmount,
                                                SGSTAmount = item.SGSTAmount,
                                                IGSTAmount = item.IGSTAmount,
                                                ExpiryDate = item.ExpiryDate,
                                                Remark = item.Remark

                                            };

                                            db.InvoiceDetails.Attach(lInvoiceDetail);
                                            db.Entry(lInvoiceDetail).Property(x => x.BuyRatePerUnit).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.MRP).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.SaleRate).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.ReceivedQuantity).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.Amount).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.GSTInPer).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.CGSTAmount).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.SGSTAmount).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.IGSTAmount).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.ExpiryDate).IsModified = true;
                                            db.Entry(lInvoiceDetail).Property(x => x.Remark).IsModified = true;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var delobj = db.InvoiceDetails.Where(p => p.ID == id).SingleOrDefault();
                                            db.InvoiceDetails.Remove(delobj);
                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (item.ReceivedQuantity > 0)
                                        {
                                            objInvoiceDetail.InvoiceID = Invoice.InvoiceID;
                                            objInvoiceDetail.PurchaseOrderDetailID = item.PurchaseOrderDetailsID;
                                            objInvoiceDetail.ProductID = item.ProductID;
                                            objInvoiceDetail.ProductVarientID = item.ProductVarientID;
                                            objInvoiceDetail.IsExtraItem = false;
                                            objInvoiceDetail.BuyRatePerUnit = item.BuyRatePerUnit;
                                            objInvoiceDetail.ReceivedQuantity = Convert.ToInt32(item.ReceivedQuantity);
                                            objInvoiceDetail.Amount = Convert.ToDecimal(item.Amount);
                                            objInvoiceDetail.GSTInPer = item.GSTInPer;
                                            objInvoiceDetail.CGSTAmount = Convert.ToDecimal(item.CGSTAmount);
                                            objInvoiceDetail.SGSTAmount = Convert.ToDecimal(item.SGSTAmount);
                                            objInvoiceDetail.IGSTAmount = Convert.ToDecimal(item.IGSTAmount);
                                            objInvoiceDetail.IGSTAmount = Convert.ToDecimal(item.ExpiryDate);
                                            objInvoiceDetail.Remark = item.Remark;
                                            objInvoiceDetail.IsActive = true;
                                            db.InvoiceDetails.Add(objInvoiceDetail);
                                            db.SaveChanges();
                                        }
                                    }
                                }

                                //start Yashaswi 21/3/2018 Invoice Attachment
                                if (file != null)
                                {
                                    var Invoice_Attchment = db.InvoiceAttachment.Where(p => p.InvoiceID == Invoice.InvoiceID).SingleOrDefault();
                                    if (Invoice_Attchment != null)
                                    {
                                        db.InvoiceAttachment.Remove(Invoice_Attchment);
                                        db.SaveChanges();
                                    }
                                    Save_InvoiceAttachment(file, Invoice.InvoiceID);
                                }

                                tscope.Complete();
                                Session["Success"] = "Invoice Updated Successfully."; //yashaswi 31/3/2018
                            }
                            //else
                            //{
                            //    //Delete from Invoice items
                            //    var delInvoiceDetail = db.InvoiceDetails.Where(p => p.InvoiceID == Invoice.InvoiceID).SingleOrDefault();
                            //    db.InvoiceDetails.Remove(delInvoiceDetail);
                            //    db.SaveChanges();

                            //    //Delete Invoice
                            //    var delInvoice = db.Invoices.Where(p => p.ID == Invoice.InvoiceID).SingleOrDefault();
                            //    db.Invoices.Remove(delInvoice);
                            //    db.SaveChanges();
                            //}
                        }
                        catch (Exception)
                        {
                            Transaction.Current.Rollback();
                            tscope.Dispose();
                            return View(Invoice);
                        }
                    }

                    return RedirectToAction("Index", new { PurchaseOrderID = Invoice.PurchaseOrderID });
                }
                ViewBag.PurchaseOrderID = Invoice.PurchaseOrderID;
            }
            //}
            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleSuppliers = db.Suppliers;

            return View(Invoice);
        }



        //
        // GET: /PurchaseOrders/PrintOrder/5
        public ViewResult PrintOrder(long id)
        {
            PurchaseOrderViewModel PurchaseOrderViewModel = new PurchaseOrderViewModel();

            ViewBag.PossibleSuppliers = db.Suppliers;
            Session["lPurchaseOrderDetailsViewModel"] = null;

            PurchaseOrderViewModel = (from po in db.PurchaseOrders
                                      join s in db.Suppliers on po.SupplierID equals s.ID
                                      join w in db.Warehouses on po.WarehouseID equals w.ID
                                      join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                      where po.ID == id

                                      select new PurchaseOrderViewModel
                                      {
                                          PurchaseOrderID = po.ID,
                                          PurchaseOrderCode = po.PurchaseOrderCode,
                                          WarehouseName = w.Name,
                                          SupplierName = s.Name,
                                          OrderDate = po.OrderDate,
                                          ExpetedDeliveryDate = po.ExpetedDeliveryDate,
                                          Amount = po.Amount == null ? 0 : po.Amount,
                                          Remark = po.Remark,
                                          IsActive = po.IsActive,
                                          SupplierCode = s.SupplierCode,
                                          SupplierContactPerson = s.ContactPerson,
                                          SupplierAddress = s.Address,
                                          SupplierMobile = s.Mobile,
                                          WarehouseContactPerson = b.ContactPerson,
                                          WarehouseAddress = b.Address,
                                          WarehouseMobile = b.Mobile

                                      }).FirstOrDefault();



            List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
            lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == id).ToList();


            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModelList = new List<PurchaseOrderDetailsViewModel>();

            foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
            {
                PurchaseOrderDetailsViewModel objPOD = new PurchaseOrderDetailsViewModel();
                objPOD.Quantity = item.Quantity;
                objPOD.Nickname = item.ProductNickname;
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                objPOD.UnitPrice = item.UnitPrice;
                objPOD.ProductAmount = item.Quantity * item.UnitPrice;
                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == item.ProductVarientID
                                select new PurchaseOrderDetailsViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }

                lPurchaseOrderDetailsViewModelList.Add(objPOD);
            }

            if (lPurchaseOrderDetailsViewModelList.Count > 0)
            {
                Session["lPurchaseOrderDetailsViewModel"] = lPurchaseOrderDetailsViewModelList;
            }


            ViewBag.PurchaseOrderID = id;
            return View(PurchaseOrderViewModel);
        }


        //
        // GET: /PurchaseOrders/Delete/5

        public ActionResult Delete(long InvoiceID, long PurchaseOrderiD)
        {
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    //    //Delete from Invoice items
                    var delInvoiceDetail = db.InvoiceDetails.Where(p => p.InvoiceID == InvoiceID).ToList();
                    foreach (var id in delInvoiceDetail)
                    {
                        db.InvoiceDetails.Remove(id);
                        db.SaveChanges();
                    }

                    tscope.Complete();
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                }
            }

            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    //Delete Invoice
                    var delInvoice = db.Invoices.Where(p => p.ID == InvoiceID).SingleOrDefault();
                    db.Invoices.Remove(delInvoice);
                    db.SaveChanges();
                    tscope.Complete();
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                }
            }

            return RedirectToAction("Index", new { PurchaseOrderID = PurchaseOrderiD });
        }

        //
        // POST: /PurchaseOrders/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            PurchaseOrder purchaseorder = db.PurchaseOrders.Single(x => x.ID == id);
            db.PurchaseOrders.Remove(purchaseorder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Methods called in jquery

        //yashaswi 26-3-2018
        public FileStreamResult Download(string fileName)
        {
            string Path = WebConfigurationManager.AppSettings["INVENTORY_ROOT_IMAGE_HTTP"] + WebConfigurationManager.AppSettings["DIRECTORY"] + "/" + WebConfigurationManager.AppSettings["FOLDER_INVOICE"] + "/";
            string aURL = Path + fileName;
            Stream rtn = null;
            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(aURL);
            HttpWebResponse aResponse = (HttpWebResponse)aRequest.GetResponse();
            rtn = aResponse.GetResponseStream();
            return File(rtn, "image/jpeg", fileName);
        }
        //End 
        public JsonResult BindParentCategory(int WarehouseID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            if (WarehouseID > 0)
            {
                lCategory = db.Categories.Where(x => x.ParentCategoryID == null && x.IsActive == true).ToList();

                foreach (var c in lCategory)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNextLevelCategory(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            //var ParentCat = db.Categories.Where(x => x.ParentCategoryID == categoryID).FirstOrDefault();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == categoryID && x.IsActive == true).ToList();

            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductList(int categoryID, int WarehouseID)
        {
            List<Product> lProduct = new List<Product>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            var fList = db.WarehouseFranchises.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.FranchiseID).ToList();
            if (fList.Count > 0)
            {
                var query = (from p in db.Products
                             join sp in db.ShopProducts on p.ID equals sp.ProductID
                             join s in db.Shops on sp.ShopID equals s.ID
                             join f in db.Franchises on s.FranchiseID equals f.ID
                             where (fList.Contains(f.ID)) && p.CategoryID == categoryID && p.IsActive == true
                             && sp.IsActive == true && s.IsActive == true && f.IsActive == true
                             select new { Name = p.Name, ID = p.ID });

                lProduct = query.ToList().Select(p => new Product
                {
                    ID = p.ID,
                    Name = p.Name
                }).ToList();


                //lProduct = db.Products.Where(x => x.CategoryID == categoryID && x.IsActive == true).ToList();

                foreach (var c in lProduct)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductVarientList(int ProductID)
        {
            List<Product> lProduct = new List<Product>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();


            var query = (from vp in db.ProductVarients
                         join p in db.Products on vp.ProductID equals p.ID
                         join s in db.Sizes on vp.SizeID equals s.ID
                         where vp.ProductID == ProductID && vp.IsActive == true
                         select new { VarientName = p.Name + " (" + s.Name + ")", ID = vp.ID });// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

            lProduct = query.ToList().Select(p => new Product
            {
                ID = p.ID,
                Name = p.VarientName
            }).ToList();

            foreach (var c in lProduct)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public string GetProductVarientDetail(int ProductVarientID)
        {

            //String daresult = null;
            DataTable dt = new DataTable();
            SqlConnection objcon = new SqlConnection(fConnectionString);
            SqlCommand objcmd = new SqlCommand("GetProductVarientDetail", objcon);
            objcmd.CommandType = CommandType.StoredProcedure;
            SqlParameter[] objParam = new SqlParameter[1];
            objParam[0] = new SqlParameter("@ProductVarientID", ProductVarientID);

            int i = 0;
            for (i = 0; i < objParam.Length; i++)
                objcmd.Parameters.Add(objParam[i]);

            objcon.Open();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = objcmd;
            da.Fill(dt);

            objcon.Close();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            //daresult = DataSetToJSON(ds);
            return ds.GetXml();
        }
        public string DataSetToJSON(DataSet ds)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DataTable dt in ds.Tables)
            {
                object[] arr = new object[dt.Rows.Count + 1];

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    arr[i] = dt.Rows[i].ItemArray;
                }

                dict.Add(dt.TableName, arr);
            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(dict);
        }

        #endregion End Methods


        public ActionResult GetId(long id)
        {
            Session["id"] = id;
            return RedirectToAction("Print2");
        }
        //GET: /PurchaseOrderReply/PrintOrder/5
        public ViewResult Print2()
        {
            long id = long.Parse(Session["id"].ToString());
            PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();

            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            ViewBag.PossibleSuppliers = db.Suppliers.Where(x => x.WarehouseID != WarehouseID).ToList();

            //Session["lPurchaseOrderDetailsViewModel"] = null;

            PurchaseOrderReplyViewModel = (from po in db.PurchaseOrderReply
                                           join p in db.PurchaseOrders on po.PurchaseOrderID equals p.ID
                                           join s in db.Suppliers on p.SupplierID equals s.ID
                                           join w in db.Warehouses on p.WarehouseID equals w.ID
                                           join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                           join pd in db.PersonalDetails on po.CreateBy equals pd.ID///Added by Priti  9/11/2018
                                                                                                    ///

                                           //join st in db.States on w.StateID equals st.ID   ////Added by Priti
                                           //join st1 in db.States on s.StateID equals st1.ID ////Addded by Priti
                                           where po.ID == id

                                           select new PurchaseOrderReplyViewModel
                                           {
                                               PurchaseOrderID = po.ID,
                                               FSSILicenseNo = w.FSSILicenseNo,           ///Added BY Priti  on  8/9/2018
                                               DVFSSILicenseNo = s.FSSILicenseNo,           ///Added BY Priti  on  8/9/2018
                                               InsecticidesLicenseNo = w.InsecticidesLicenseNo,     ////  ///Added BY Priti  on  8/9/2018
                                               PurchaseOrderCode = p.PurchaseOrderCode,
                                               // StateCode = st.StateCode,       ///FV State cODE  
                                               // StateCode1 = st1.ID,   ///  DV State Code
                                               InvoiceCode = po.InvoiceCode,
                                               SupplierPanNo = s.PAN,    //Added by Priti on 9-11-2018
                                               WarehouseName = w.Name,
                                               SupplierName = s.Name,
                                               SupplierGSTNumber = s.GSTNumber,
                                               WarehouseGSTNumber = w.GSTNumber,  ////Added by Priti on 15/10/2018
                                               SupplierFax = s.FAX,
                                               SupplierEmail = s.Email,
                                               CreateDate = po.CreateDate,
                                               DeliveryDateTime = po.DeliveryDateTime,
                                               OrderAmount = po.OrderAmount == null ? 0 : po.OrderAmount,
                                               GSTAmount = po.GSTAmount == null ? 0 : po.GSTAmount,
                                               ShippingCharge = po.ShippingCharge == null ? 0 : po.ShippingCharge,
                                               AdditionalCost = po.AdditionalCost == null ? 0 : po.AdditionalCost,
                                               CustomDutyCharge = po.CustomDutyCharge == null ? 0 : po.CustomDutyCharge,
                                               OperatingCost = po.OperatingCost == null ? 0 : po.OperatingCost,
                                               TotalDiscountAmount = po.TotalDiscountAmount == null ? 0 : po.TotalDiscountAmount,
                                               TotalAmount = po.TotalAmount,
                                               Remark = po.Remark,
                                               IsActive = po.IsActive,
                                               SupplierCode = s.SupplierCode,
                                               SupplierContactPerson = s.ContactPerson,
                                               SupplierAddress = s.Address,
                                               SupplierMobile = s.Mobile,


                                               CreateByName = pd.FirstName,     ///Added by Priti on 9-11-2018
                                               ModifyByName = pd.FirstName,          ///Added by Priti on 9-11-2018
                                               WarehouseContactPerson = b.ContactPerson,
                                               WarehouseAddress = b.Address,
                                               WarehouseMobile = b.Mobile,
                                               WarehosueEmail = b.Email,
                                               ExpetedDeliveryDate = p.ExpetedDeliveryDate,
                                               Entity = w.Entity,
                                               WarehousePanNo = w.PAN

                                           }).FirstOrDefault();



            List<PurchaseOrderReplyDetail> lPurchaseOrderReplyDetailslist = new List<PurchaseOrderReplyDetail>();
            lPurchaseOrderReplyDetailslist = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == id).ToList();

            //try
            //{
            string InvoiceCode = db.PurchaseOrderReply.FirstOrDefault(p => p.ID == id).InvoiceCode;
            long PurchaseOrderId = db.PurchaseOrderReply.FirstOrDefault(p => p.ID == id).PurchaseOrderID;

            long InvoiceId = 0;
            Invoice objInvoice = db.Invoices.FirstOrDefault(p => p.InvoiceCode == InvoiceCode);
            if (objInvoice != null)
            {
                InvoiceId = objInvoice.ID;
            }
            long PWarehouseId = db.PurchaseOrders.FirstOrDefault(p => p.ID == PurchaseOrderId).WarehouseID;
            List<WarehouseReturnStock> objWarehouseReturnStock = db.WarehouseReturnStock.Where(p => p.InvoiceId == InvoiceId).ToList();



            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailsViewModelList = new List<PurchaseOrderReplyDetailViewModel>();

            foreach (PurchaseOrderReplyDetail item in lPurchaseOrderReplyDetailslist)
            {
                if (objWarehouseReturnStock != null && objWarehouseReturnStock.Count() != 0)
                {
                    WarehouseStock objWarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.WarehouseID == PWarehouseId && p.ProductID == item.ProductID && p.ProductVarientID == item.ProductVarientID && p.InvoiceID == InvoiceId);
                    foreach (var stk in objWarehouseReturnStock)
                    {
                        if (objWarehouseStock != null)
                        {
                            WarehouseReturnStockDetails objWarehouseReturnStockDetails = db.WarehouseReturnStockDetails.FirstOrDefault(p => p.WarehouseReturnStockId == stk.ID && p.WarehouseStockId == objWarehouseStock.ID);
                            if (objWarehouseReturnStockDetails != null)
                            {
                                item.Quantity = item.Quantity - objWarehouseReturnStockDetails.Quantity;
                                if (item.Quantity < 0)
                                {
                                    item.Quantity = 0;
                                }
                            }
                        }
                    }
                }
                if (item.Quantity > 0)
                {
                    PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();
                    objPOD.Quantity = item.Quantity;
                    objPOD.ProductNickname = item.ProductNickname;
                    objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                    objPOD.ProductAmount = item.Quantity * item.BuyRatePerUnit;
                    objPOD.GSTInPer = item.GSTInPer;
                    objPOD.MRP = item.MRP;

                    objPOD.SGSTAmount = item.Quantity * item.CGSTAmount;

                    objPOD.TaxableValue = (objPOD.ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
                    objPOD.TaxableValue = Math.Round(objPOD.TaxableValue, 2);
                    decimal GSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
                    objPOD.SGSTAmount = GSTAmount / 2;
                    objPOD.CGSTAmount = GSTAmount / 2;

                    var itemName = (from p in db.Products
                                    join b in db.Brands on p.BrandID equals b.ID   ///added by Priti
                                    join v in db.ProductVarients on p.ID equals v.ProductID
                                    join s in db.Sizes on v.SizeID equals s.ID
                                    where v.ID == item.ProductVarientID
                                    select new PurchaseOrderReplyDetailViewModel { ItemName = p.Name, SKUUnit = s.Name, HSNCode = p.HSNCode, SKUID = v.ID, BrandID = b.ID, BrandName = b.Name }).ToList();

                    foreach (var i in itemName)
                    {
                        objPOD.ItemName = i.ItemName.ToString();
                        objPOD.HSNCode = Convert.ToString(i.HSNCode);
                        objPOD.SKUID = i.SKUID;
                        objPOD.SKUUnit = i.SKUUnit;
                        objPOD.BrandID = i.BrandID;
                        objPOD.BrandName = i.BrandName;

                    }

                    lPurchaseOrderReplyDetailsViewModelList.Add(objPOD);
                }
            }

            if (lPurchaseOrderReplyDetailsViewModelList.Count > 0)
            {
                PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailsViewModelList;
            }

            ViewBag.PurchaseOrderID = id;
            PurchaseOrderReplyDetailViewModel objPOD1 = new PurchaseOrderReplyDetailViewModel();


            var rEsult = from a in lPurchaseOrderReplyDetailsViewModelList group a by a.BrandID into g orderby g.Key select g;




            var Result1 = (from po in db.PurchaseOrders
                           join pd in db.PersonalDetails on po.CreateBy equals pd.UserLoginID       ///Added by Priti  28/9/2018
                                                                                                    ///Added by Priti  28/9/2018



                           select new PurchaseOrderReplyDetailViewModel { CreateByName = pd.FirstName + " (" + pd.LastName + ")", ModifyByName = pd.FirstName }).ToList();
            var wordlist = new List<string> { "test", "one", "two" };
            var grouped = wordlist.GroupBy(i => i)
            .Select(i => new
            {
                Word = i.Key,
                Count = i.Count()
            });





            List<PurchaseOrderReplyDetailViewModel> rEsult1 = lPurchaseOrderReplyDetailsViewModelList.GroupBy(I => I.BrandID).Select(cl => new PurchaseOrderReplyDetailViewModel
            {
                ItemName = cl.First().ItemName,
                BrandName = cl.First().BrandName,
                Quantity = cl.Sum(c => c.Quantity),
                ProductAmount = cl.Sum(c => c.ProductAmount),
                BrandID = cl.First().BrandID,
                MRP = (cl.Sum(c => c.MRP * c.Quantity)),

            }).ToList();

            Session["lPurchaseOrderReplyDetailViewModelsAnnextureViewModel"] = rEsult1;


            List<GSTClass> GSTlist = new List<GSTClass>();

            if (PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels == null)
            {
                PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = new List<PurchaseOrderReplyDetailViewModel>();
            }

            GSTlist = PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels
                .GroupBy(p => p.GSTInPer)
                .Select(q =>
                new GSTClass
                {
                    GST = q.Key,
                    CGST = (q.Key / 2.0),
                    CGSTAmt = q.Sum(w => w.CGSTAmount),
                    SGST = (q.Key / 2.0),
                    SGSTAmt = q.Sum(w => w.SGSTAmount),

                    TaxableAmt = q.Sum(w => w.ProductAmount) - (q.Sum(w => w.CGSTAmount) + q.Sum(w => w.SGSTAmount)),
                    TotalAmt = q.Sum(w => w.ProductAmount),
                    TaxAmt = q.Sum(w => w.CGSTAmount) + q.Sum(w => w.SGSTAmount)

                }).OrderBy(q => q.GST).ToList();


            PurchaseOrderReplyViewModel.lGst = GSTlist;

            return View(PurchaseOrderReplyViewModel);
        }




        //Added by Priti on 3-11-2018 for New Formate of Invoice
        //public ActionResult GetId1(long id)
        //{
        //    Session["id"] = id;
        //    return RedirectToAction("Print");
        //}

        //public ViewResult Print()//long id
        //{
        //    long id = long.Parse(Session["id"].ToString());
        //    PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();

        //    int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
        //    ViewBag.PossibleSuppliers = db.Suppliers.Where(x => x.WarehouseID != WarehouseID).ToList();

        //    //Session["lPurchaseOrderDetailsViewModel"] = null;

        //    PurchaseOrderReplyViewModel = (from po in db.PurchaseOrderReply
        //                                   join p in db.PurchaseOrders on po.PurchaseOrderID equals p.ID
        //                                   join s in db.Suppliers on p.SupplierID equals s.ID
        //                                   join w in db.Warehouses on p.WarehouseID equals w.ID
        //                                   join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
        //                                   join pd in db.PersonalDetails on po.CreateBy equals pd.ID///Added by Priti  9/11/2018
        //                                                                                            ///

        //                                   //join st in db.States on w.StateID equals st.ID   ////Added by Priti
        //                                   //join st1 in db.States on s.StateID equals st1.ID ////Addded by Priti
        //                                   where po.ID == id

        //                                   select new PurchaseOrderReplyViewModel
        //                                   {
        //                                       PurchaseOrderID = po.ID,
        //                                       FSSILicenseNo = w.FSSILicenseNo,           ///Added BY Priti  on  8/9/2018
        //                                       DVFSSILicenseNo = s.FSSILicenseNo,           ///Added BY Priti  on  8/9/2018
        //                                       InsecticidesLicenseNo = w.InsecticidesLicenseNo,     ////  ///Added BY Priti  on  8/9/2018
        //                                       PurchaseOrderCode = p.PurchaseOrderCode,
        //                                       // StateCode = st.StateCode,       ///FV State cODE  
        //                                       // StateCode1 = st1.ID,   ///  DV State Code
        //                                       InvoiceCode = po.InvoiceCode,
        //                                       SupplierPanNo = s.PAN,    //Added by Priti on 9-11-2018
        //                                       WarehouseName = w.Name,
        //                                       SupplierName = s.Name,
        //                                       SupplierGSTNumber = s.GSTNumber,
        //                                       WarehouseGSTNumber = w.GSTNumber,  ////Added by Priti on 15/10/2018
        //                                       SupplierFax = s.FAX,
        //                                       SupplierEmail = s.Email,
        //                                       CreateDate = po.CreateDate,
        //                                       DeliveryDateTime = po.DeliveryDateTime,
        //                                       OrderAmount = po.OrderAmount == null ? 0 : po.OrderAmount,
        //                                       GSTAmount = po.GSTAmount == null ? 0 : po.GSTAmount,
        //                                       ShippingCharge = po.ShippingCharge == null ? 0 : po.ShippingCharge,
        //                                       AdditionalCost = po.AdditionalCost == null ? 0 : po.AdditionalCost,
        //                                       CustomDutyCharge = po.CustomDutyCharge == null ? 0 : po.CustomDutyCharge,
        //                                       OperatingCost = po.OperatingCost == null ? 0 : po.OperatingCost,
        //                                       TotalDiscountAmount = po.TotalDiscountAmount == null ? 0 : po.TotalDiscountAmount,
        //                                       TotalAmount = po.TotalAmount,
        //                                       Remark = po.Remark,
        //                                       IsActive = po.IsActive,
        //                                       SupplierCode = s.SupplierCode,
        //                                       SupplierContactPerson = s.ContactPerson,
        //                                       SupplierAddress = s.Address,
        //                                       SupplierMobile = s.Mobile,


        //                                       CreateByName = pd.FirstName,     ///Added by Priti on 9-11-2018
        //                                       ModifyByName = pd.FirstName,          ///Added by Priti on 9-11-2018
        //                                       WarehouseContactPerson = b.ContactPerson,
        //                                       WarehouseAddress = b.Address,
        //                                       WarehouseMobile = b.Mobile,
        //                                       WarehosueEmail = b.Email,
        //                                       ExpetedDeliveryDate = p.ExpetedDeliveryDate

        //                                   }).FirstOrDefault();



        //    List<PurchaseOrderReplyDetail> lPurchaseOrderReplyDetailslist = new List<PurchaseOrderReplyDetail>();
        //    lPurchaseOrderReplyDetailslist = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == id).ToList();

        //    //try
        //    //{
        //    string InvoiceCode = db.PurchaseOrderReply.FirstOrDefault(p => p.ID == id).InvoiceCode;
        //    long PurchaseOrderId = db.PurchaseOrderReply.FirstOrDefault(p => p.ID == id).PurchaseOrderID;

        //    long InvoiceId = 0;
        //    Invoice objInvoice = db.Invoices.FirstOrDefault(p => p.InvoiceCode == InvoiceCode);
        //    if (objInvoice != null)
        //    {
        //        InvoiceId = objInvoice.ID;
        //    }
        //    long PWarehouseId = db.PurchaseOrders.FirstOrDefault(p => p.ID == PurchaseOrderId).WarehouseID;
        //    List<WarehouseReturnStock> objWarehouseReturnStock = db.WarehouseReturnStock.Where(p => p.InvoiceId == InvoiceId).ToList();


        //    //}
        //    //catch
        //    //{

        //    //}


        //    List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailsViewModelList = new List<PurchaseOrderReplyDetailViewModel>();

        //    foreach (PurchaseOrderReplyDetail item in lPurchaseOrderReplyDetailslist)
        //    {
        //        if (objWarehouseReturnStock != null && objWarehouseReturnStock.Count() != 0)
        //        {
        //            WarehouseStock objWarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.WarehouseID == PWarehouseId && p.ProductID == item.ProductID && p.ProductVarientID == item.ProductVarientID && p.InvoiceID == InvoiceId);
        //            foreach (var stk in objWarehouseReturnStock)
        //            {
        //                if (objWarehouseStock != null)
        //                {
        //                    WarehouseReturnStockDetails objWarehouseReturnStockDetails = db.WarehouseReturnStockDetails.FirstOrDefault(p => p.WarehouseReturnStockId == stk.ID && p.WarehouseStockId == objWarehouseStock.ID);
        //                    if (objWarehouseReturnStockDetails != null)
        //                    {
        //                        item.Quantity = item.Quantity - objWarehouseReturnStockDetails.Quantity;
        //                        if (item.Quantity < 0)
        //                        {
        //                            item.Quantity = 0;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (item.Quantity > 0)
        //        {
        //            PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();
        //            objPOD.Quantity = item.Quantity;
        //            objPOD.ProductNickname = item.ProductNickname;
        //            objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
        //            objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
        //            objPOD.ProductAmount = item.Quantity * item.BuyRatePerUnit;
        //            objPOD.GSTInPer = item.GSTInPer;
        //            objPOD.MRP = item.MRP;

        //            objPOD.SGSTAmount = item.Quantity * item.CGSTAmount;

        //            objPOD.TaxableValue = (objPOD.ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
        //            objPOD.TaxableValue = Math.Round(objPOD.TaxableValue, 2);
        //            decimal GSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
        //            objPOD.SGSTAmount = GSTAmount / 2;
        //            objPOD.CGSTAmount = GSTAmount / 2;

        //            var itemName = (from p in db.Products
        //                            join b in db.Brands on p.BrandID equals b.ID   ///added by Priti
        //                            join v in db.ProductVarients on p.ID equals v.ProductID
        //                            join s in db.Sizes on v.SizeID equals s.ID
        //                            where v.ID == item.ProductVarientID
        //                            select new PurchaseOrderReplyDetailViewModel { ItemName = p.Name, SKUUnit = s.Name, HSNCode = p.HSNCode, SKUID = v.ID, BrandID = b.ID, BrandName = b.Name }).ToList();

        //            foreach (var i in itemName)
        //            {
        //                objPOD.ItemName = i.ItemName.ToString();
        //                objPOD.HSNCode = Convert.ToString(i.HSNCode);
        //                objPOD.SKUID = i.SKUID;
        //                objPOD.SKUUnit = i.SKUUnit;
        //                objPOD.BrandID = i.BrandID;
        //                objPOD.BrandName = i.BrandName;

        //            }

        //            lPurchaseOrderReplyDetailsViewModelList.Add(objPOD);
        //        }
        //    }

        //    if (lPurchaseOrderReplyDetailsViewModelList.Count > 0)
        //    {
        //        PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailsViewModelList;
        //    }

        //    ViewBag.PurchaseOrderID = id;
        //    PurchaseOrderReplyDetailViewModel objPOD1 = new PurchaseOrderReplyDetailViewModel();


        //    var rEsult = from a in lPurchaseOrderReplyDetailsViewModelList group a by a.BrandID into g orderby g.Key select g;




        //    var Result1 = (from po in db.PurchaseOrders
        //                   join pd in db.PersonalDetails on po.CreateBy equals pd.UserLoginID       ///Added by Priti  28/9/2018
        //                                                                                            ///Added by Priti  28/9/2018



        //                   select new PurchaseOrderReplyDetailViewModel { CreateByName = pd.FirstName + " (" + pd.LastName + ")", ModifyByName = pd.FirstName }).ToList();
        //    var wordlist = new List<string> { "test", "one", "two" };
        //    var grouped = wordlist.GroupBy(i => i)
        //    .Select(i => new
        //    {
        //        Word = i.Key,
        //        Count = i.Count()
        //    });





        //    List<PurchaseOrderReplyDetailViewModel> rEsult1 = lPurchaseOrderReplyDetailsViewModelList.GroupBy(I => I.BrandID).Select(cl => new PurchaseOrderReplyDetailViewModel
        //    {
        //        ItemName = cl.First().ItemName,
        //        BrandName = cl.First().BrandName,
        //        Quantity = cl.Sum(c => c.Quantity),
        //        ProductAmount = cl.Sum(c => c.ProductAmount),
        //        BrandID = cl.First().BrandID,
        //        MRP = (cl.Sum(c => c.MRP * c.Quantity)),

        //    }).ToList();

        //    Session["lPurchaseOrderReplyDetailViewModelsAnnextureViewModel"] = rEsult1;


        //    List<GSTClass> GSTlist = new List<GSTClass>();

        //    if (PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels == null)
        //    {
        //        PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = new List<PurchaseOrderReplyDetailViewModel>();
        //    }

        //    GSTlist = PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels
        //        .GroupBy(p => p.GSTInPer)
        //        .Select(q =>
        //        new GSTClass
        //        {
        //            GST = q.Key,
        //            CGST = (q.Key / 2.0),
        //            CGSTAmt = q.Sum(w => w.CGSTAmount),
        //            SGST = (q.Key / 2.0),
        //            SGSTAmt = q.Sum(w => w.SGSTAmount),

        //            TaxableAmt = q.Sum(w => w.ProductAmount) - (q.Sum(w => w.CGSTAmount) + q.Sum(w => w.SGSTAmount)),
        //            TotalAmt = q.Sum(w => w.ProductAmount),
        //            TaxAmt = q.Sum(w => w.CGSTAmount) + q.Sum(w => w.SGSTAmount)

        //        }).OrderBy(q => q.GST).ToList();


        //    PurchaseOrderReplyViewModel.lGst = GSTlist;

        //    return View(PurchaseOrderReplyViewModel);
        //}



        public ViewResult PrintGR(long InvoiceID)
        {
            //long id = long.Parse(Session["id"].ToString());
            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();

            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

            Warehouse objW = db.Warehouses.Where(x => x.ID == purchaseOrder.WarehouseID).FirstOrDefault();
            Supplier objS = db.Suppliers.Where(x => x.ID == purchaseOrder.SupplierID).FirstOrDefault();
            if (Invoice != null && Invoice.ToString() != "")
            {
                InvoiceViewModel.InvoiceID = InvoiceID;
                InvoiceViewModel.InvoiceCode = Invoice.InvoiceCode;
                InvoiceViewModel.PurchaseOrderCode = purchaseOrder.PurchaseOrderCode;
                InvoiceViewModel.PurchaseOrderID = PurchaseOrderID;
                InvoiceViewModel.WarehouseID = purchaseOrder.WarehouseID;
                InvoiceViewModel.WarehouseName = objW.Name;
                InvoiceViewModel.SupplierID = purchaseOrder.SupplierID;
                InvoiceViewModel.SupplierName = objS.Name;
                InvoiceViewModel.SupplierGSTNumber = objS.GSTNumber;
                InvoiceViewModel.SupplierFax = objS.FAX;
                InvoiceViewModel.SupplierEmail = objS.Email;
                InvoiceViewModel.SupplierCode = objS.SupplierCode;
                InvoiceViewModel.SupplierContactPerson = objS.ContactPerson;
                InvoiceViewModel.SupplierAddress = objS.Address;
                InvoiceViewModel.SupplierMobile = objS.Mobile;
                InvoiceViewModel.InvoiceDate = Invoice.InvoiceDate;
                InvoiceViewModel.TotalDiscountAmount = Convert.ToDecimal(Invoice.TotalDiscountAmount);
                InvoiceViewModel.ShippingCharge = Convert.ToDecimal(Invoice.ShippingCharge);
                InvoiceViewModel.CustomDutyCharge = Convert.ToDecimal(Invoice.CustomDutyCharge);
                InvoiceViewModel.OperatingCost = Convert.ToDecimal(Invoice.OperatingCost);
                InvoiceViewModel.AdditionalCost = Convert.ToDecimal(Invoice.AdditionalCost);
                InvoiceViewModel.OrderAmount = Convert.ToDecimal(Invoice.OrderAmount);
                InvoiceViewModel.TotalAmount = Convert.ToDecimal(Invoice.TotalAmount);
                InvoiceViewModel.Remark = Invoice.Remark == null ? "" : Invoice.Remark;
                InvoiceViewModel.IsActive = Invoice.IsActive;
            }

            //List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
            //lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

            List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
            lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();

            List<InvoiceDetailViewModel> lInvoiceDetailViewModelList = new List<InvoiceDetailViewModel>();


            foreach (InvoiceDetail item in lInvoiceDetaillist)
            {
                InvoiceDetailViewModel objPOD = new InvoiceDetailViewModel();
                //foreach (var inv in lInvoiceDetaillist)
                //{
                //    if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
                //    {
                objPOD.ReceivedQuantity = item.ReceivedQuantity;
                objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                objPOD.MRP = item.MRP;
                objPOD.SaleRate = item.SaleRate;
                objPOD.GSTInPer = item.GSTInPer;
                //objPOD.CGSTAmount = Convert.ToDecimal(item.CGSTAmount * item.ReceivedQuantity);
                //objPOD.SGSTAmount = Convert.ToDecimal(item.SGSTAmount * item.ReceivedQuantity);

                objPOD.ProductAmount = item.ReceivedQuantity * item.BuyRatePerUnit;


                ///Added by Priti on 16-10-2018
                objPOD.TaxableValue = (objPOD.ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
                objPOD.TaxableValue = Math.Round(objPOD.TaxableValue, 2);
                decimal GSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
                objPOD.SGSTAmount = GSTAmount / 2;

                objPOD.CGSTAmount = GSTAmount / 2;


                //var CGSTAmount1 = objPOD.ProductAmount - objPOD.TaxableValue;
                //decimal? CGSTAmount = Math.Truncate(CGSTAmount1 * 100) / 100;
                //decimal CGSTAmount2 = Convert.ToDecimal(CGSTAmount1 / 2);
                //objPOD.CGSTAmount = Math.Truncate(CGSTAmount2 * 100) / 100;
                ////objPOD.CGSTAmount = Math.Round(CGSTAmount2, 2);

                //var SGSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;

                //decimal SGSTAmount2 = Convert.ToDecimal(CGSTAmount1 / 2);
                //objPOD.SGSTAmount = Math.Truncate(SGSTAmount2 * 100) / 100;


                //var gstresult = objPOD.SGSTAmount + objPOD.CGSTAmount;

                //end  by Priti on 16-10-2018



                objPOD.IGSTAmount = Convert.ToDecimal(item.IGSTAmount * item.ReceivedQuantity);
                objPOD.Amount = Convert.ToDecimal(item.Amount);
                objPOD.ExpiryDate = item.ExpiryDate;
                objPOD.Remark = item.Remark == null ? "" : item.Remark;

                //var expirydate = db.WarehouseStocks.Where(x => x.InvoiceID == InvoiceID && x.ProductID == inv.ProductID && x.ProductVarientID == inv.ProductVarientID).Select(x => x.ExpiryDate).FirstOrDefault();
                //objPOD.ExpiryDate = expirydate;
                //    }
                //}

                objPOD.PurchaseOrderDetailsID = item.ID;
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

                lInvoiceDetailViewModelList.Add(objPOD);
            }
            if (lInvoiceDetailViewModelList.Count > 0)
            {
                Session["lInvoiceDetailViewModel"] = lInvoiceDetailViewModelList;
                InvoiceViewModel.lInvoiceDetailViewModels = lInvoiceDetailViewModelList;
            }
            return View(InvoiceViewModel);
        }

        //public ViewResult PrintGR(long InvoiceID)
        //{
        //    //long id = long.Parse(Session["id"].ToString());
        //    InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();

        //    long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
        //    ViewBag.PurchaseOrderID = PurchaseOrderID;


        //    var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
        //    var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

        //    Warehouse objW = db.Warehouses.Where(x => x.ID == purchaseOrder.WarehouseID).FirstOrDefault();
        //    Supplier objS = db.Suppliers.Where(x => x.ID == purchaseOrder.SupplierID).FirstOrDefault();
        //    if (Invoice != null && Invoice.ToString() != "")
        //    {
        //        InvoiceViewModel.InvoiceID = InvoiceID;
        //        InvoiceViewModel.InvoiceCode = Invoice.InvoiceCode;
        //        InvoiceViewModel.PurchaseOrderCode = purchaseOrder.PurchaseOrderCode;
        //        InvoiceViewModel.PurchaseOrderID = PurchaseOrderID;
        //        InvoiceViewModel.WarehouseID = purchaseOrder.WarehouseID;
        //        InvoiceViewModel.WarehouseName = objW.Name;
        //        InvoiceViewModel.SupplierID = purchaseOrder.SupplierID;
        //        InvoiceViewModel.SupplierName = objS.Name;
        //        InvoiceViewModel.SupplierGSTNumber = objS.GSTNumber;
        //        InvoiceViewModel.SupplierFax = objS.FAX;
        //        InvoiceViewModel.SupplierEmail = objS.Email;
        //        InvoiceViewModel.SupplierCode = objS.SupplierCode;
        //        InvoiceViewModel.SupplierContactPerson = objS.ContactPerson;
        //        InvoiceViewModel.SupplierAddress = objS.Address;
        //        InvoiceViewModel.SupplierMobile = objS.Mobile;
        //        InvoiceViewModel.InvoiceDate = Invoice.InvoiceDate;
        //        InvoiceViewModel.TotalDiscountAmount = Convert.ToDecimal(Invoice.TotalDiscountAmount);
        //        InvoiceViewModel.ShippingCharge = Convert.ToDecimal(Invoice.ShippingCharge);
        //        InvoiceViewModel.CustomDutyCharge = Convert.ToDecimal(Invoice.CustomDutyCharge);
        //        InvoiceViewModel.OperatingCost = Convert.ToDecimal(Invoice.OperatingCost);
        //        InvoiceViewModel.AdditionalCost = Convert.ToDecimal(Invoice.AdditionalCost);
        //        InvoiceViewModel.OrderAmount = Convert.ToDecimal(Invoice.OrderAmount);
        //        InvoiceViewModel.TotalAmount = Convert.ToDecimal(Invoice.TotalAmount);
        //        InvoiceViewModel.Remark = Invoice.Remark == null ? "" : Invoice.Remark;
        //        InvoiceViewModel.IsActive = Invoice.IsActive;
        //    }

        //    //List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
        //    //lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

        //    List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
        //    lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();

        //    List<InvoiceDetailViewModel> lInvoiceDetailViewModelList = new List<InvoiceDetailViewModel>();


        //    foreach (InvoiceDetail item in lInvoiceDetaillist)
        //    {
        //        InvoiceDetailViewModel objPOD = new InvoiceDetailViewModel();
        //        //foreach (var inv in lInvoiceDetaillist)
        //        //{
        //        //    if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
        //        //    {
        //        objPOD.ReceivedQuantity = item.ReceivedQuantity;
        //        objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
        //        objPOD.MRP = item.MRP;
        //        objPOD.SaleRate = item.SaleRate;
        //        objPOD.GSTInPer = item.GSTInPer;
        //        objPOD.CGSTAmount = Convert.ToDecimal(item.CGSTAmount * item.ReceivedQuantity);
        //        objPOD.SGSTAmount = Convert.ToDecimal(item.SGSTAmount * item.ReceivedQuantity);
        //        objPOD.IGSTAmount = Convert.ToDecimal(item.IGSTAmount * item.ReceivedQuantity);
        //        objPOD.Amount = Convert.ToDecimal(item.Amount);
        //        objPOD.ExpiryDate = item.ExpiryDate;
        //        objPOD.Remark = item.Remark == null ? "" : item.Remark;

        //        //var expirydate = db.WarehouseStocks.Where(x => x.InvoiceID == InvoiceID && x.ProductID == inv.ProductID && x.ProductVarientID == inv.ProductVarientID).Select(x => x.ExpiryDate).FirstOrDefault();
        //        //objPOD.ExpiryDate = expirydate;
        //        //    }
        //        //}

        //        objPOD.PurchaseOrderDetailsID = item.ID;
        //        objPOD.ProductID = item.ProductID;
        //        objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);

        //        objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

        //        var itemName = (from p in db.Products
        //                        join v in db.ProductVarients on p.ID equals v.ProductID
        //                        join s in db.Sizes on v.SizeID equals s.ID
        //                        where v.ID == item.ProductVarientID
        //                        select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();

        //        foreach (var i in itemName)
        //        {
        //            objPOD.ItemName = i.ItemName.ToString();
        //            objPOD.HSNCode = Convert.ToString(i.HSNCode);
        //        }

        //        lInvoiceDetailViewModelList.Add(objPOD);
        //    }
        //    if (lInvoiceDetailViewModelList.Count > 0)
        //    {
        //        Session["lInvoiceDetailViewModel"] = lInvoiceDetailViewModelList;
        //        InvoiceViewModel.lInvoiceDetailViewModels = lInvoiceDetailViewModelList;
        //    }
        //    return View(InvoiceViewModel);
        //}


        public ActionResult CheckInvoiceCode(string InvoiceCode, long PurchaseOrderID)
        {

            //For Other warehouses, check Invoice Code in database
            if (Session["Entity"] != null && Convert.ToString(Session["Entity"].ToString()) != "EVW")
            {
                var invoiceCode = db.PurchaseOrderReply.Where(x => x.InvoiceCode == InvoiceCode && x.PurchaseOrderID == PurchaseOrderID && x.IsReplied == true).Select(y => y.ID).FirstOrDefault();

                if (invoiceCode == 0)
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            else // For Ezeelo it will not check Invoice code because code will come from outside supplier
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
        }

    }
}