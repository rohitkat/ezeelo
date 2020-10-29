using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.Data.Entity;
using System.Transactions;

namespace Inventory.Controllers
{
    public class WarehouseReturnStockController : Controller
    {
        /// <summary>
        /// Yashaswi 17-3-2018
        /// </summary>
        ///
        CommonController obj_CommonController = new CommonController();
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Invoice_SupplierList(long? SupplierID)
        {

            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ///Bind Supplier List
            Invoice_SupplierViewModel obj_Invoice_SupplierViewModel = new Invoice_SupplierViewModel();


            ///Bind Invoice
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            //Yashaswi 9/4/2018
            obj_Invoice_SupplierViewModel.SupplierList = new SelectList(obj_CommonController.GetSupplierLIst(WarehouseID), "ID", "Name");
            List<Invoice> obj_Invoice = new List<Invoice>();
            obj_Invoice_SupplierViewModel.obj_InvoiceViewModel = (from o in db.Invoices
                                                                  join p in db.PurchaseOrders on o.PurchaseOrderID equals p.ID
                                                                  join s in db.Suppliers on p.SupplierID equals s.ID
                                                                  where p.WarehouseID == WarehouseID && o.IsApproved == true
                                                                  select new InvoiceViewModel
                                                                  {
                                                                      InvoiceID = o.ID,
                                                                      SupplierName = s.Name,
                                                                      SupplierID = s.ID,
                                                                      InvoiceDate = o.InvoiceDate,
                                                                      TotalAmount = o.TotalAmount,
                                                                      InvoiceCode = o.InvoiceCode,
                                                                      TotalItems = db.InvoiceDetails.Where(x => x.InvoiceID == o.ID).Select(x => x.ID).Count(),
                                                                      IsApproved = o.IsApproved
                                                                  }).OrderByDescending(x => x.InvoiceDate).ToList();
            if (SupplierID != null)
            {
                obj_Invoice_SupplierViewModel.obj_InvoiceViewModel = obj_Invoice_SupplierViewModel.obj_InvoiceViewModel.Where(p => p.SupplierID == SupplierID).ToList();
                ViewBag.SupplierName = db.Suppliers.First(p => p.ID == SupplierID).Name;
            }
            return View(obj_Invoice_SupplierViewModel);

        }
        public ActionResult Create(long InvoiceID)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(GetDisplayInvoiceViewModel(InvoiceID));
        }

        public InvoiceViewModel GetDisplayInvoiceViewModel(long InvoiceID)
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

            if (Invoice != null && Invoice.ToString() != "")
            {
                InvoiceViewModel.InvoiceID = InvoiceID;
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
            List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
            lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();

            List<WarehouseReturnStockDetailsViewModel> list_WarehouseReturnStockDetailsViewModel = new List<WarehouseReturnStockDetailsViewModel>();


            foreach (InvoiceDetail item in lInvoiceDetaillist)
            {
                WarehouseReturnStockDetailsViewModel objPOD = new WarehouseReturnStockDetailsViewModel();
                objPOD.ReceivedQuantity = item.ReceivedQuantity;
                objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                objPOD.Amount = Convert.ToDecimal(item.Amount);
                objPOD.Remark = item.Remark == null ? "" : item.Remark;
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
                objPOD.ReturnQuantity = 0;
                objPOD.ReturnRatePerUnit = objPOD.BuyRatePerUnit;
                objPOD.Amount = objPOD.ReturnQuantity * objPOD.ReturnRatePerUnit;
                objPOD.ReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN).ToList(), "ID", "Reason"); //Yashaswi 11/5/2018
                objPOD.SubReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ID == -1).ToList(), "ID", "Reason");
                WarehouseStock obj_WarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.WarehouseID == WarehouseID && p.InvoiceID == InvoiceID && p.ProductID == objPOD.ProductID && p.ProductVarientID == objPOD.ProductVarientID);
                objPOD.WarehouseStockID = obj_WarehouseStock.ID;
                objPOD.BatchAvlQty = obj_WarehouseStock.AvailableQuantity;
                list_WarehouseReturnStockDetailsViewModel.Add(objPOD);
            }

            InvoiceViewModel.lWarehouseReturnStockDetailsViewModel = list_WarehouseReturnStockDetailsViewModel;
            InvoiceViewModel.AdditionalCharge = 0;
            InvoiceViewModel.TranportationCharge = 0;
            return InvoiceViewModel;
        }

        [HttpPost]
        public ActionResult Create(InvoiceViewModel InvoiceViewModel)
        {
            bool IsSaveSuccess = false;
            long ShopStock_Qty = -1;
            long WarehouseStockID = 0;
            long ShopStockID = 0;
            long PersonalDetailID = 1;
            if (Session["USER_LOGIN_ID"] != null)
            {
                long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
            }

            bool IsValid = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Any(p => p.ReturnQuantity != 0 && p.BatchAvlQty > 0);

            if (InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Any(p => p.ReturnQuantity != 0 && p.ReasonId == null) == true)
            {
                InvoiceViewModel obj_InvoiceViewModel = GetDisplayInvoiceViewModel(InvoiceViewModel.InvoiceID);
                for (int i = 0; i < obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Count(); i++)
                //foreach (var item in obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel)
                {
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit;
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity;
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN).ToList(), "ID", "Reason", InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReasonId); //Yashaswi 11/5/2018
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].SubReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ID == -1).ToList(), "ID", "Reason", InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].SubReasonId);
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Amount = obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit * obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity;
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Remark = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Remark;
                }
                obj_InvoiceViewModel.AdditionalCharge = InvoiceViewModel.AdditionalCharge;
                obj_InvoiceViewModel.TranportationCharge = InvoiceViewModel.TranportationCharge;
                obj_InvoiceViewModel.TotalReturnAmout = Convert.ToDecimal(obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Sum(p => p.ReturnQuantity * p.ReturnRatePerUnit) + obj_InvoiceViewModel.AdditionalCharge + obj_InvoiceViewModel.TranportationCharge); ;
                obj_InvoiceViewModel.Remark = InvoiceViewModel.Remark;
                //obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel;
                Session["Warning"] = "Please Select Reason For Return Item Entry.";//yashaswi 31/3/2018
                ModelState.AddModelError("AdditionalCharge", "Please Select Reason For Return Item Entry.");
                return View(obj_InvoiceViewModel);
            }

            if (IsValid == true)
            {
                using (TransactionScope tscope = new TransactionScope())
                {
                    try
                    {
                        WarehouseReturnStock obj_WarehouseReturnStock = new WarehouseReturnStock();
                        obj_WarehouseReturnStock.InvoiceId = InvoiceViewModel.InvoiceID;
                        obj_WarehouseReturnStock.AdditionalCharge = InvoiceViewModel.AdditionalCharge;
                        obj_WarehouseReturnStock.TransportationCharge = InvoiceViewModel.TranportationCharge;
                        obj_WarehouseReturnStock.Remark = InvoiceViewModel.Remark;
                        obj_WarehouseReturnStock.IsApproved = false; //Yashaswi Inventory Return 20-12-2018
                        obj_WarehouseReturnStock.IsActive = true;
                        obj_WarehouseReturnStock.CreateBy = PersonalDetailID;
                        obj_WarehouseReturnStock.CreateDate = DateTime.Now.Date;
                        obj_WarehouseReturnStock.DeviceID = "X";
                        obj_WarehouseReturnStock.DeviceType = "X";
                        obj_WarehouseReturnStock.NetworkIP = CommonFunctions.GetClientIP();
                        obj_WarehouseReturnStock.ModifyBy = null;
                        obj_WarehouseReturnStock.ModifyDate = null;
                        obj_WarehouseReturnStock.TotalAmount = Convert.ToDecimal(InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Sum(p => p.ReturnQuantity * p.ReturnRatePerUnit) + obj_WarehouseReturnStock.AdditionalCharge + obj_WarehouseReturnStock.TransportationCharge);
                        db.WarehouseReturnStock.Add(obj_WarehouseReturnStock);
                        db.SaveChanges();

                        long WarehouseReturnStockId = obj_WarehouseReturnStock.ID;

                        //Items Entry
                        foreach (var item in InvoiceViewModel.lWarehouseReturnStockDetailsViewModel)
                        {
                            if (item.ReturnQuantity > 0)
                            {
                                WarehouseReturnStockDetails obj_WarehouseReturnStockDetails = new WarehouseReturnStockDetails();
                                obj_WarehouseReturnStockDetails.ReturnRatePerUnit = item.ReturnRatePerUnit;
                                obj_WarehouseReturnStockDetails.Quantity = item.ReturnQuantity;
                                obj_WarehouseReturnStockDetails.SubReasonId = Convert.ToInt64(item.SubReasonId);
                                obj_WarehouseReturnStockDetails.Remark = item.Remark;
                                obj_WarehouseReturnStockDetails.WarehouseStockId = item.WarehouseStockID;
                                obj_WarehouseReturnStockDetails.WarehouseReturnStockId = WarehouseReturnStockId;
                                db.WarehouseReturnStockDetails.Add(obj_WarehouseReturnStockDetails);
                                db.SaveChanges();

                                int returnQty = obj_WarehouseReturnStockDetails.Quantity;
                                WarehouseStock obj_WarehouseStocks = new WarehouseStock();
                                obj_WarehouseStocks = db.WarehouseStocks.First(p => p.ID == obj_WarehouseReturnStockDetails.WarehouseStockId);
                                obj_WarehouseStocks.AvailableQuantity = obj_WarehouseStocks.AvailableQuantity - returnQty;
                                WarehouseReorderLevel obj_WarehouseReorderLevel = new WarehouseReorderLevel();
                                obj_WarehouseReorderLevel = db.WarehouseReorderLevels.First(p => p.WarehouseID == obj_WarehouseStocks.WarehouseID && p.ProductID == obj_WarehouseStocks.ProductID && p.ProductVarientID == obj_WarehouseStocks.ProductVarientID);
                                obj_WarehouseReorderLevel.AvailableQuantity = obj_WarehouseReorderLevel.AvailableQuantity - returnQty;
                                db.Entry(obj_WarehouseStocks).State = EntityState.Modified;
                                db.Entry(obj_WarehouseReorderLevel).State = EntityState.Modified;
                                WarehouseStockID = obj_WarehouseReturnStockDetails.WarehouseStockId;
                                obj_CommonController.ShopStockDeduction(obj_WarehouseReturnStockDetails.WarehouseStockId, returnQty, out ShopStock_Qty, out ShopStockID);

                                db.SaveChanges();
                                obj_CommonController.WarehouseStockLog(obj_WarehouseReturnStockDetails.WarehouseStockId, (int)Inventory.Common.Constants.Warehouse_Stock_Log_Status.RETURN, PersonalDetailID, item.ReturnQuantity); //Yashaswi 2/4/2018

                            }

                        }
                        tscope.Complete();
                        IsSaveSuccess = true;
                        Session["Success"] = "Return Record Saved Successfully"; //yashaswi 31/3/2018

                        if (IsSaveSuccess == true)
                        {
                            using (TransactionScope tscope1 = new TransactionScope())
                            {
                                obj_CommonController.AddBatchToShopStock(ShopStock_Qty, WarehouseStockID, ShopStockID);
                                tscope1.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Transaction.Current.Rollback();
                        tscope.Dispose();
                        throw ex;
                    }
                }


                return RedirectToAction("Index");
            }
            else
            {
                InvoiceViewModel obj_InvoiceViewModel = GetDisplayInvoiceViewModel(InvoiceViewModel.InvoiceID);
                obj_InvoiceViewModel.AdditionalCharge = InvoiceViewModel.AdditionalCharge;
                obj_InvoiceViewModel.TranportationCharge = InvoiceViewModel.TranportationCharge;
                obj_InvoiceViewModel.TotalReturnAmout = InvoiceViewModel.TotalReturnAmout;
                obj_InvoiceViewModel.Remark = InvoiceViewModel.Remark;
                IsValid = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Any(p => p.BatchAvlQty > 0);
                if (IsValid == false)
                {
                    Session["Warning"] = "Return Quantity Cant be 0. You Cant Save This Return Entry.";//yashaswi 31/3/2018
                    ModelState.AddModelError("AdditionalCharge", "You Cant Save This Return Entry."); //For Return Quantity
                }
                else
                {
                    Session["Warning"] = "Return Quantity Cant be 0. Please Insert Return Quantity.";//yashaswi 31/3/2018
                    ModelState.AddModelError("AdditionalCharge", "Return Quantity Cant be 0. Please Insert Return Quantity.");//For Return Quantity
                }

                return View(obj_InvoiceViewModel);
            }
        }
        public ActionResult Index()
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            List<WarehouseReturnListViewModel> list_WarehouseReturnStock = new List<WarehouseReturnListViewModel>();
            list_WarehouseReturnStock = (from r in db.WarehouseReturnStock
                                         join i in db.Invoices on r.InvoiceId equals i.ID
                                         join p in db.PurchaseOrders on i.PurchaseOrderID equals p.ID
                                         join s in db.Suppliers on p.SupplierID equals s.ID
                                         join w in db.Warehouses on p.WarehouseID equals w.ID
                                         where w.ID == WarehouseID
                                         select new WarehouseReturnListViewModel
                                         {
                                             WarehouseName = w.Name,
                                             SupplierName = s.Name,
                                             InvoiceCode = i.InvoiceCode,
                                             TotalItems = db.WarehouseReturnStockDetails.Where(k => k.WarehouseReturnStockId == r.ID && k.Quantity > 0).Count(),
                                             Amount = r.TotalAmount,
                                             ReturnDate = r.CreateDate,
                                             WarehouseReturnStockId = r.ID,
                                             IsApproved = r.IsApproved ////Yashaswi Inventory Return 20-12-2018
                                         }).OrderByDescending(r => r.WarehouseReturnStockId).ToList();

            return View(list_WarehouseReturnStock);
        }

        public InvoiceViewModel GetDisplayReturnViewModel(long WarehouseReturnStockId)
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            long InvoiceID = db.WarehouseReturnStock.First(p => p.ID == WarehouseReturnStockId).InvoiceId;

            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

            if (Invoice != null && Invoice.ToString() != "")
            {
                InvoiceViewModel.ReturnId = WarehouseReturnStockId;
                InvoiceViewModel.InvoiceID = InvoiceID;
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
            List<WarehouseReturnStockDetails> list_WarehouseReturnStockDetails = new List<WarehouseReturnStockDetails>();
            list_WarehouseReturnStockDetails = db.WarehouseReturnStockDetails.Where(p => p.WarehouseReturnStockId == WarehouseReturnStockId).ToList();

            List<WarehouseReturnStockDetailsViewModel> list_WarehouseReturnStockDetailsViewModel = new List<WarehouseReturnStockDetailsViewModel>();


            foreach (WarehouseReturnStockDetails item in list_WarehouseReturnStockDetails)
            {
                WarehouseStock obj_WarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.ID == item.WarehouseStockId && p.WarehouseID == WarehouseID);
                List<InvoiceDetail> list_InvoiceDetail = db.InvoiceDetails.Where(p => p.ProductID == obj_WarehouseStock.ProductID && p.ProductVarientID == obj_WarehouseStock.ProductVarientID && p.InvoiceID == InvoiceID).ToList();

                WarehouseReason obj_WarehouseReasons = db.WarehouseReasons.SingleOrDefault(p => p.ID == item.SubReasonId);
                long? MainReasonId = 0;
                if (obj_WarehouseReasons != null)
                {
                    MainReasonId = obj_WarehouseReasons.ParentReasonId;
                }

                WarehouseReturnStockDetailsViewModel objPOD = new WarehouseReturnStockDetailsViewModel();
                objPOD.ID = item.ID;
                objPOD.ReceivedQuantity = list_InvoiceDetail[0].ReceivedQuantity;
                objPOD.BuyRatePerUnit = list_InvoiceDetail[0].BuyRatePerUnit;
                objPOD.Remark = item.Remark == null ? "" : item.Remark;
                objPOD.ProductID = obj_WarehouseStock.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(obj_WarehouseStock.ProductVarientID);
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(objPOD.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == objPOD.ProductVarientID
                                select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }
                objPOD.ReturnQuantity = item.Quantity;
                objPOD.OldReturnQuantity = item.Quantity;
                objPOD.ReturnRatePerUnit = item.ReturnRatePerUnit;
                objPOD.Amount = Convert.ToDecimal(item.Quantity * item.ReturnRatePerUnit);

                objPOD.ReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN).ToList(), "ID", "Reason", MainReasonId); //Yashaswi 11/5/2018
                objPOD.SubReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId != null).ToList(), "ID", "Reason", item.SubReasonId);
                objPOD.BatchAvlQty = obj_WarehouseStock.AvailableQuantity;
                objPOD.WarehouseStockID = obj_WarehouseStock.ID;
                objPOD.Remark = item.Remark;
                list_WarehouseReturnStockDetailsViewModel.Add(objPOD);
            }
            List<InvoiceDetail> lInvoiceDetaillist = new List<InvoiceDetail>();
            lInvoiceDetaillist = db.InvoiceDetails.Where(x => x.InvoiceID == InvoiceID).ToList();
            foreach (var item in lInvoiceDetaillist)
            {
                WarehouseStock obj = db.WarehouseStocks.SingleOrDefault(p => p.ProductID == item.ProductID && p.ProductVarientID == item.ProductVarientID && p.WarehouseID == WarehouseID && p.InvoiceID == InvoiceID);
                WarehouseReturnStockDetailsViewModel obj1 = list_WarehouseReturnStockDetailsViewModel.SingleOrDefault(p => p.WarehouseStockID == obj.ID);
                if (obj1 == null)
                {
                    WarehouseReturnStockDetailsViewModel objPOD = new WarehouseReturnStockDetailsViewModel();
                    objPOD.ID = 0;
                    objPOD.ReceivedQuantity = item.ReceivedQuantity;
                    objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                    objPOD.Amount = Convert.ToDecimal(item.Amount);
                    objPOD.Remark = item.Remark == null ? "" : item.Remark;
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
                    objPOD.ReturnQuantity = 0;
                    objPOD.OldReturnQuantity = 0;
                    objPOD.ReturnRatePerUnit = objPOD.BuyRatePerUnit;
                    objPOD.Amount = objPOD.ReturnQuantity * objPOD.ReturnRatePerUnit;
                    objPOD.ReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN).ToList(), "ID", "Reason"); //Yashaswi 11/5/2018
                    objPOD.SubReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId != null).ToList(), "ID", "Reason");
                    WarehouseStock obj_WarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.WarehouseID == WarehouseID && p.InvoiceID == item.InvoiceID && p.ProductID == objPOD.ProductID && p.ProductVarientID == objPOD.ProductVarientID);
                    objPOD.WarehouseStockID = obj_WarehouseStock.ID;
                    objPOD.BatchAvlQty = obj_WarehouseStock.AvailableQuantity;
                    list_WarehouseReturnStockDetailsViewModel.Add(objPOD);
                }
            }


            WarehouseReturnStock obj_WarehouseReturnStock = db.WarehouseReturnStock.First(p => p.ID == WarehouseReturnStockId);

            InvoiceViewModel.lWarehouseReturnStockDetailsViewModel = list_WarehouseReturnStockDetailsViewModel;
            InvoiceViewModel.AdditionalCharge = obj_WarehouseReturnStock.AdditionalCharge;
            InvoiceViewModel.TranportationCharge = obj_WarehouseReturnStock.TransportationCharge;
            InvoiceViewModel.TotalReturnAmout = obj_WarehouseReturnStock.TotalAmount;
            InvoiceViewModel.Remark = obj_WarehouseReturnStock.Remark;
            InvoiceViewModel.IsApproved = obj_WarehouseReturnStock.IsApproved;
            return InvoiceViewModel;
        }

        public ActionResult Edit(long WarehouseReturnStockId)
        {
            return View(GetDisplayReturnViewModel(WarehouseReturnStockId));
        }

        [HttpPost]
        public ActionResult Edit(InvoiceViewModel InvoiceViewModel)
        {
            long ShopStock_Qty = -1;
            long WarehouseStockID = 0;
            long ShopStockID = 0;
            long PersonalDetailID = 1;
            if (Session["USER_LOGIN_ID"] != null)
            {
                long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
            }
            bool IsValid = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Any(p => p.ReturnQuantity != 0 && p.BatchAvlQty > 0);
            if (InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Any(p => p.ReturnQuantity != 0 && p.ReasonId == null) == true)
            {
                InvoiceViewModel obj_InvoiceViewModel = GetDisplayReturnViewModel((long)InvoiceViewModel.ReturnId);
                for (int i = 0; i < obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Count(); i++)
                {
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit;
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity;
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN).ToList(), "ID", "Reason", InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReasonId); //Yashaswi 11/5/2018
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].SubReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId != null).ToList(), "ID", "Reason", InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].SubReasonId);
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Amount = obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit * obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity;
                    obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Remark = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Remark;
                }
                obj_InvoiceViewModel.AdditionalCharge = InvoiceViewModel.AdditionalCharge;
                obj_InvoiceViewModel.TranportationCharge = InvoiceViewModel.TranportationCharge;
                obj_InvoiceViewModel.TotalReturnAmout = Convert.ToDecimal(obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Sum(p => p.ReturnQuantity * p.ReturnRatePerUnit) + obj_InvoiceViewModel.AdditionalCharge + obj_InvoiceViewModel.TranportationCharge); ;
                obj_InvoiceViewModel.Remark = InvoiceViewModel.Remark;
                Session["Warning"] = "Please Select Reason For Return Item Entry.";//yashaswi 31/3/2018
                ModelState.AddModelError("AdditionalCharge", "Please Select Reason For Return Item Entry.");
                return View(obj_InvoiceViewModel);
            }
            //if (IsValid == true)
            //{
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    WarehouseReturnStock obj_WarehouseReturnStock = db.WarehouseReturnStock.SingleOrDefault(p => p.ID == InvoiceViewModel.ReturnId);
                    //obj_WarehouseReturnStock.InvoiceId = InvoiceViewModel.InvoiceID;
                    obj_WarehouseReturnStock.AdditionalCharge = InvoiceViewModel.AdditionalCharge;
                    obj_WarehouseReturnStock.TransportationCharge = InvoiceViewModel.TranportationCharge;
                    obj_WarehouseReturnStock.Remark = InvoiceViewModel.Remark;
                    //obj_WarehouseReturnStock.IsApproved = true; //Yashaswi Inventory Return 20-12-2018
                    //obj_WarehouseReturnStock.IsActive = true;
                    //obj_WarehouseReturnStock.CreateBy = PersonalDetailID;
                    //obj_WarehouseReturnStock.CreateDate = DateTime.Now.Date;
                    //obj_WarehouseReturnStock.DeviceID = "X";
                    //obj_WarehouseReturnStock.DeviceType = "X";
                    obj_WarehouseReturnStock.NetworkIP = CommonFunctions.GetClientIP();
                    obj_WarehouseReturnStock.ModifyBy = PersonalDetailID;
                    obj_WarehouseReturnStock.ModifyDate = DateTime.Now.Date;
                    obj_WarehouseReturnStock.TotalAmount = Convert.ToDecimal(InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Sum(p => p.ReturnQuantity * p.ReturnRatePerUnit) + obj_WarehouseReturnStock.AdditionalCharge + obj_WarehouseReturnStock.TransportationCharge);
                    db.Entry(obj_WarehouseReturnStock).State = EntityState.Modified;
                    db.SaveChanges();

                    long WarehouseReturnStockId = obj_WarehouseReturnStock.ID;

                    //Items Entry
                    foreach (var item in InvoiceViewModel.lWarehouseReturnStockDetailsViewModel)
                    {

                        //if (item.ReturnQuantity > 0)
                        //{
                        int qty = 0;
                        if (item.ID == 0)
                        {
                            WarehouseReturnStockDetails obj_WarehouseReturnStockDetails = new WarehouseReturnStockDetails();
                            obj_WarehouseReturnStockDetails.ReturnRatePerUnit = item.ReturnRatePerUnit;
                            obj_WarehouseReturnStockDetails.Quantity = item.ReturnQuantity;
                            obj_WarehouseReturnStockDetails.SubReasonId = Convert.ToInt64(item.SubReasonId);
                            obj_WarehouseReturnStockDetails.Remark = item.Remark;
                            obj_WarehouseReturnStockDetails.WarehouseStockId = item.WarehouseStockID;
                            obj_WarehouseReturnStockDetails.WarehouseReturnStockId = (long)InvoiceViewModel.ReturnId;
                            db.WarehouseReturnStockDetails.Add(obj_WarehouseReturnStockDetails);
                            db.SaveChanges();
                            qty = item.ReturnQuantity;
                        }
                        else
                        {
                            int Old_ReturnQty = 0;
                            int New_ReturnQty = 0;
                            WarehouseReturnStockDetails obj_WarehouseReturnStockDetails = db.WarehouseReturnStockDetails.SingleOrDefault(p => p.ID == item.ID && p.WarehouseReturnStockId == InvoiceViewModel.ReturnId);
                            Old_ReturnQty = obj_WarehouseReturnStockDetails.Quantity;
                            obj_WarehouseReturnStockDetails.ReturnRatePerUnit = item.ReturnRatePerUnit;
                            obj_WarehouseReturnStockDetails.Quantity = item.ReturnQuantity;
                            obj_WarehouseReturnStockDetails.SubReasonId = Convert.ToInt64(item.SubReasonId);
                            obj_WarehouseReturnStockDetails.Remark = item.Remark;
                            obj_WarehouseReturnStockDetails.WarehouseStockId = item.WarehouseStockID;
                            obj_WarehouseReturnStockDetails.WarehouseReturnStockId = (long)InvoiceViewModel.ReturnId;
                            db.Entry(obj_WarehouseReturnStockDetails).State = EntityState.Modified;
                            db.SaveChanges();
                            New_ReturnQty = item.ReturnQuantity;

                            if (New_ReturnQty == 0)
                            {
                                qty = Old_ReturnQty;
                                qty = qty * -1;
                            }
                            else
                            {
                                if (Old_ReturnQty < New_ReturnQty)
                                {
                                    qty = New_ReturnQty - Old_ReturnQty; //deduct
                                }
                                else
                                {
                                    qty = Old_ReturnQty - New_ReturnQty; //Add
                                    qty = qty * -1;
                                }
                            }
                        }
                        WarehouseStock obj_WarehouseStocks = new WarehouseStock();
                        obj_WarehouseStocks = db.WarehouseStocks.First(p => p.ID == item.WarehouseStockID);
                        obj_WarehouseStocks.AvailableQuantity = obj_WarehouseStocks.AvailableQuantity - qty;
                        WarehouseReorderLevel obj_WarehouseReorderLevel = new WarehouseReorderLevel();
                        obj_WarehouseReorderLevel = db.WarehouseReorderLevels.First(p => p.WarehouseID == obj_WarehouseStocks.WarehouseID && p.ProductID == obj_WarehouseStocks.ProductID && p.ProductVarientID == obj_WarehouseStocks.ProductVarientID);
                        obj_WarehouseReorderLevel.AvailableQuantity = obj_WarehouseReorderLevel.AvailableQuantity - qty;
                        db.Entry(obj_WarehouseStocks).State = EntityState.Modified;
                        db.Entry(obj_WarehouseReorderLevel).State = EntityState.Modified;
                        WarehouseStockID = item.WarehouseStockID;
                        obj_CommonController.ShopStockDeduction(item.WarehouseStockID, qty, out ShopStock_Qty, out ShopStockID);
                        //ShopStock obj_ShopStock = db.ShopStocks.SingleOrDefault(p => p.WarehouseStockID == item.WarehouseStockID);
                        //if (obj_ShopStock != null)
                        //{
                        //    obj_ShopStock.Qty = obj_ShopStock.Qty - qty;
                        //    db.Entry(obj_ShopStock).State = EntityState.Modified;
                        //}
                        db.SaveChanges();


                        obj_CommonController.WarehouseStockLog(item.WarehouseStockID, (int)Inventory.Common.Constants.Warehouse_Stock_Log_Status.RETURN_ADD_IN_STOCK, PersonalDetailID, item.ReturnQuantity); //Yashaswi 2/4/2018
                        //}

                    }
                    tscope.Complete();
                    using (TransactionScope tscope1 = new TransactionScope())
                    {
                        obj_CommonController.AddBatchToShopStock(ShopStock_Qty, WarehouseStockID, ShopStockID);
                        tscope1.Complete();
                    }

                    Session["Success"] = "Return Record Updated Successfully"; //yashaswi 31/3/2018
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                    throw ex;
                }
            }
            //}
            //else
            //{
            //    InvoiceViewModel obj_InvoiceViewModel = GetDisplayReturnViewModel((long)InvoiceViewModel.ReturnId);
            //    for (int i = 0; i < obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Count(); i++)
            //    {
            //        obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit;
            //        obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity;
            //        obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId == null && p.ParentCategoryId == (int)ModelLayer.Models.Enum.Reason_ParentCategory.Reason_Parent.RETURN).ToList(), "ID", "Reason", InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReasonId); //Yashaswi 11/5/2018
            //        obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].SubReasonList = new SelectList(db.WarehouseReasons.Where(p => p.ParentReasonId != null).ToList(), "ID", "Reason", InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].SubReasonId);
            //        obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Amount = obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnRatePerUnit * obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].ReturnQuantity;
            //        obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Remark = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel[i].Remark;
            //    }
            //    obj_InvoiceViewModel.AdditionalCharge = InvoiceViewModel.AdditionalCharge;
            //    obj_InvoiceViewModel.TranportationCharge = InvoiceViewModel.TranportationCharge;
            //    obj_InvoiceViewModel.TotalReturnAmout = Convert.ToDecimal(obj_InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Sum(p => p.ReturnQuantity * p.ReturnRatePerUnit) + obj_InvoiceViewModel.AdditionalCharge + obj_InvoiceViewModel.TranportationCharge);
            //    obj_InvoiceViewModel.Remark = InvoiceViewModel.Remark;

            //    IsValid = InvoiceViewModel.lWarehouseReturnStockDetailsViewModel.Any(p => p.BatchAvlQty > 0);
            //    if (IsValid == false)
            //    {
            //        Session["Warning"] = "Return Quantity Cant be 0. You Cant Save This Return Entry.";//yashaswi 31/3/2018
            //        ModelState.AddModelError("AdditionalCharge", "Return Quantity Cant be 0. You Cant Save This Return Entry."); //For Return Quantity
            //    }
            //    else
            //    {
            //        Session["Warning"] = "Return Quantity Cant be 0. Please Insert Return Quantity.";//yashaswi 31/3/2018
            //        ModelState.AddModelError("AdditionalCharge", "Return Quantity Cant be 0. Please Insert Return Quantity.");//For Return Quantity
            //    }

            //    return View(obj_InvoiceViewModel);
            //}
            //return View(GetDisplayReturnViewModel(WarehouseReturnStockId));
        }

        //Start Yashaswi 27/3/2018
        public ActionResult ReturnFromCustomerList()
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            long SupplierId = 0;
            Supplier obj_Supp = db.Suppliers.SingleOrDefault(p => p.WarehouseID == WarehouseID);
            if (obj_Supp != null)
            {
                SupplierId = obj_Supp.ID;
            }
            List<WarehouseReturnListViewModel> list_WarehouseReturnStock = new List<WarehouseReturnListViewModel>();
            list_WarehouseReturnStock = (from r in db.WarehouseReturnStock
                                         join i in db.Invoices on r.InvoiceId equals i.ID
                                         join p in db.PurchaseOrders on i.PurchaseOrderID equals p.ID
                                         join s in db.Suppliers on p.SupplierID equals s.ID
                                         join w in db.Warehouses on p.WarehouseID equals w.ID
                                         // where w.ID == WarehouseID
                                         select new WarehouseReturnListViewModel
                                         {
                                             WarehouseName = w.Name,
                                             SupplierName = s.Name,
                                             InvoiceCode = i.InvoiceCode,
                                             TotalItems = db.WarehouseReturnStockDetails.Where(k => k.WarehouseReturnStockId == r.ID && k.Quantity > 0).Count(),  //Yashaswi 16-11-2018 Add 0 quantity condition
                                             Amount = r.TotalAmount,
                                             ReturnDate = r.CreateDate,
                                             WarehouseReturnStockId = r.ID,
                                             SupplierID = s.ID,
                                             IsApproved = r.IsApproved ////Yashaswi Inventory Return 20-12-2018
                                         }).OrderByDescending(r => r.WarehouseReturnStockId).ToList();
            if (SupplierId != 0)
            {
                list_WarehouseReturnStock = list_WarehouseReturnStock.Where(p => p.SupplierID == SupplierId).OrderByDescending(p => p.ReturnDate).ToList();
            }

            return View(list_WarehouseReturnStock);
        }

        public ActionResult Details(long WarehouseReturnStockId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(GetDisplayReturnViewModel_Supp(WarehouseReturnStockId));
        }

        public InvoiceViewModel GetDisplayReturnViewModel_Supp(long WarehouseReturnStockId)
        {
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            //Yashaswi Inventory Return 20-12-2018
            long PurchaseOrderReplyId =0;
            long InvoiceID_Supp =0;
            WarehouseReturnStock obj1 = db.WarehouseReturnStock.First(p => p.ID == WarehouseReturnStockId);
            long InvoiceID = 0;
            if (obj1 != null)
            {
                InvoiceID = obj1.InvoiceId;
            }


            InvoiceViewModel InvoiceViewModel = new InvoiceViewModel();
            long PurchaseOrderID = db.Invoices.Where(x => x.ID == InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            var Invoice = db.Invoices.Single(x => x.ID == InvoiceID);

            if (Invoice != null && Invoice.ToString() != "")
            {
                InvoiceViewModel.ReturnId = WarehouseReturnStockId;
                InvoiceViewModel.InvoiceID = InvoiceID;
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
                InvoiceViewModel.InvoiceCode = Invoice.InvoiceCode;


                //Yashaswi Inventory Return 20-12-2018
                PurchaseOrderReply objPurchaseOrderReply = db.PurchaseOrderReply.FirstOrDefault(p => p.InvoiceCode == Invoice.InvoiceCode);
                if (objPurchaseOrderReply != null)
                {
                    PurchaseOrderReplyId = objPurchaseOrderReply.ID;
                }
            }
            List<WarehouseReturnStockDetails> list_WarehouseReturnStockDetails = new List<WarehouseReturnStockDetails>();
            list_WarehouseReturnStockDetails = db.WarehouseReturnStockDetails.Where(p => p.WarehouseReturnStockId == WarehouseReturnStockId).ToList();

            List<WarehouseReturnStockDetailsViewModel> list_WarehouseReturnStockDetailsViewModel = new List<WarehouseReturnStockDetailsViewModel>();


            foreach (WarehouseReturnStockDetails item in list_WarehouseReturnStockDetails)
            {
                //To get Product Id && Product Varient id
                WarehouseStock obj_WarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.ID == item.WarehouseStockId);

                //Reason
                WarehouseReason subReason;
                string main_Reason = "", sub_Reason = "";
                if (item.SubReasonId != null)
                {
                    subReason = db.WarehouseReasons.SingleOrDefault(p => p.ID == item.SubReasonId);
                    if (subReason != null)
                    {
                        sub_Reason = subReason.Reason;
                        WarehouseReason mainReason = db.WarehouseReasons.SingleOrDefault(p => p.ID == subReason.ParentReasonId);
                        main_Reason = mainReason.Reason;
                    }
                }


                WarehouseReturnStockDetailsViewModel objPOD = new WarehouseReturnStockDetailsViewModel();
                objPOD.ID = item.ID;
                objPOD.ReturnQuantity = item.Quantity;
                objPOD.ReturnRatePerUnit = item.ReturnRatePerUnit;
                objPOD.Remark = sub_Reason + ", " + main_Reason + ", " + item.Remark;
                objPOD.Amount = Convert.ToDecimal(item.Quantity * item.ReturnRatePerUnit);
                objPOD.ProductID = obj_WarehouseStock.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(obj_WarehouseStock.ProductVarientID);
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(objPOD.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                var itemName = (from p in db.Products
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID
                                where v.ID == objPOD.ProductVarientID
                                select new InvoiceDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                }

                list_WarehouseReturnStockDetailsViewModel.Add(objPOD);

                //Start Yashaswi Inventory Return 20-12-2018
                PurchaseOrderReplyDetail objPurchaseOrderReplyDetail = db.PurchaseOrderReplyDetails.FirstOrDefault(p=>p.PurchaseOrderReplyID == PurchaseOrderReplyId && p.ProductID == obj_WarehouseStock.ProductID && p.ProductVarientID == obj_WarehouseStock.ProductVarientID);
                if (objPurchaseOrderReplyDetail != null)
                {
                    WarehouseStockDeliveryDetail objWarehouseStockDeliveryDetail =db.WarehouseStockDeliveryDetails.FirstOrDefault(p=>p.PurchaseOrderReplyDetailID == objPurchaseOrderReplyDetail.ID);
                    if (objWarehouseStockDeliveryDetail != null)
                    {
                        objPOD.WarehouseStockID_Supp = objWarehouseStockDeliveryDetail.WarehouseStockID;
                        InvoiceID_Supp = db.WarehouseStocks.FirstOrDefault(p => p.ID == objPOD.WarehouseStockID_Supp).InvoiceID;
                    }
                }
                //End Yashaswi Inventory Return 20-12-2018
            }

            //Start Yashaswi Inventory Return 20-12-2018
            Invoice objInvoice_Supp= db.Invoices.FirstOrDefault(p=>p.ID == InvoiceID_Supp);
            InvoiceViewModel.InvoiceCode_Supp = objInvoice_Supp.InvoiceCode;
            InvoiceViewModel.InvoiceId_Supp = objInvoice_Supp.ID;
            InvoiceViewModel.InvoiceDate_Supp = objInvoice_Supp.CreateDate;
            InvoiceViewModel.OrderAmount_Supp = objInvoice_Supp.OrderAmount;
            InvoiceViewModel.TotalAmount_Supp = objInvoice_Supp.TotalAmount;
            //End Yashaswi Inventory Return 20-12-2018

            WarehouseReturnStock obj_WarehouseReturnStock = db.WarehouseReturnStock.First(p => p.ID == WarehouseReturnStockId);

            //Start Yashaswi Inventory Return 20-12-2018
            InvoiceViewModel.IsApproved = obj_WarehouseReturnStock.IsApproved;
            InvoiceViewModel.lWarehouseReturnStockDetailsViewModel = list_WarehouseReturnStockDetailsViewModel;
            InvoiceViewModel.AdditionalCharge = obj_WarehouseReturnStock.AdditionalCharge;
            InvoiceViewModel.TranportationCharge = obj_WarehouseReturnStock.TransportationCharge;
            InvoiceViewModel.TotalReturnAmout = obj_WarehouseReturnStock.TotalAmount;
            InvoiceViewModel.Remark = obj_WarehouseReturnStock.Remark;
            return InvoiceViewModel;
        }
        //End Yashaswi 27/3/2018

        ////Yashaswi Inventory Return 20-12-2018
        //Start Yashaswi 16-11-2018 Accept return bill at seller side
        [HttpPost]
        public ActionResult Accept(InvoiceViewModel obj)
        {
            try
            {
                WarehouseReturnStock obj_WarehouseReturnStock = db.WarehouseReturnStock.First(p => p.ID == obj.ReturnId);
                if (obj_WarehouseReturnStock != null)
                {
                    obj_WarehouseReturnStock.IsApproved = true;
                    obj_WarehouseReturnStock.ApprovedDate = DateTime.Now;
                    obj_WarehouseReturnStock.ApprovedBy = Convert.ToInt64(Session["USER_LOGIN_ID"]);
                    db.SaveChanges();

                    foreach (var item in obj.lWarehouseReturnStockDetailsViewModel)
                    {
                        WarehouseStock objWarehouseStock = db.WarehouseStocks.FirstOrDefault(p=>p.ID == item.WarehouseStockID_Supp);
                        if (objWarehouseStock != null)
                        {
                            objWarehouseStock.AvailableQuantity = objWarehouseStock.AvailableQuantity + item.ReturnQuantity;
                            db.SaveChanges();
                            WarehouseReorderLevel objWarehouseReorderLevel= db.WarehouseReorderLevels.FirstOrDefault(p=>p.WarehouseID == objWarehouseStock.WarehouseID && p.ProductID == objWarehouseStock.ProductID && p.ProductVarientID == objWarehouseStock.ProductVarientID);
                            if (objWarehouseReorderLevel != null)
                            {
                                objWarehouseReorderLevel.AvailableQuantity = objWarehouseReorderLevel.AvailableQuantity + item.ReturnQuantity;
                                db.SaveChanges();
                            }
                        }

                    }
                    Session["Success"] = "Return Bill Accepted Successfully"; //yashaswi 31/3/2018
                    return RedirectToAction("ReturnFromCustomerList");
                }
                else
                {
                    Session["Warning"] = "No Bill Found"; //yashaswi 31/3/2018
                    return RedirectToAction("ReturnFromCustomerList");
                }
            }
            catch (Exception ex)
            {

            }
            Session["Error"] = "Problem facing while accepting Bill."; //yashaswi 31/3/2018
            return RedirectToAction("ReturnFromCustomerList");
        }

        //End Yashaswi 16-11-2018 Accept return bill at seller side


    }
}