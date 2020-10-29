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

namespace Inventory.Controllers
{
    public class PurchaseOrderReplyController : Controller
    {

        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();

        //
        // GET: /PurchaseOrderReply/
        public ActionResult Index()
        {
            if (Session["USER_NAME"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
            List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            if (SupplierID != null && Convert.ToInt64(SupplierID) > 0)
            {
                lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                           join w in db.Warehouses on o.WarehouseID equals w.ID
                                           where o.SupplierID == SupplierID && o.IsActive == true
                                           && o.IsSent == true
                                           select new PurchaseOrderViewModel
                                           {
                                               PurchaseOrderID = o.ID,
                                               WarehouseName = w.Name,
                                               PurchaseOrderCode = o.PurchaseOrderCode,
                                               TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                               ModifyDate = o.ModifyDate,//get sent date
                                               ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                               IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                               SupplierID = o.SupplierID,
                                               DeliveryDate = o.DeliveryDate
                                           }).OrderByDescending(o => o.PurchaseOrderID).ToList(); //.OrderBy(o => o.IsAcceptedBySupplier)
            }

            Session["lPurchaseOrderReplyDetailViewModel"] = null;
            //Yashaswi 9/4/2018
            if (WarehouseID == Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]))
            {
                ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.IsFulfillmentCenter == false && x.ID != WarehouseID && x.IsActive == true).ToList();
            }
            else
            {
                ViewBag.PossibleWarehouses = Obj_Common.GetFVList(WarehouseID);
            }
            objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();


            return View("Index", objPO);
        }


        public ActionResult Search(long? RequestFromWarehouseID)
        {
            PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
            if (Session["USER_NAME"] != null)
            { }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

            if (SupplierID != null && Convert.ToInt64(SupplierID) > 0)
            {
                if (RequestFromWarehouseID != null && RequestFromWarehouseID > 0)
                {
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               where o.SupplierID == SupplierID && o.IsActive == true
                                               && o.IsSent == true && o.WarehouseID == RequestFromWarehouseID
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   ModifyDate = o.ModifyDate,//get sent date
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                                   IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                                   SupplierID = o.SupplierID,
                                                   DeliveryDate = o.DeliveryDate
                                               }).OrderByDescending(o => o.PurchaseOrderID).ToList(); //.OrderBy(o => o.IsAcceptedBySupplier)
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }


            Session["lQuotationItemDetailViewModel"] = null;
            objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();
            //Yashaswi 9/4/2018
            ViewBag.PossibleWarehouses = Obj_Common.GetFVList(WarehouseID);
            //ViewBag.PossibleWarehouses = new SelectList(db.Warehouses.Where(x => x.ID != WarehouseID), "WarehouseID", "Name", RequestFromWarehouseID);

            return View("Index", objPO);
        }

        //
        // GET: /PurchaseOrderReply/Details/5
        public ActionResult View(int id)
        {
            PurchaseOrderViewModel PurchaseOrderViewModel = new PurchaseOrderViewModel();

            var tempQuantity = new List<PurchaseOrderReplyDetailViewModel>();
            int sumTempQuantity = 0;
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == id);
            if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
            {
                ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " under FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
            }
            int WarehouseLevel = 0;
            Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == purchaseOrder.WarehouseID);
            if (obj_warehouse != null)
            {
                if (obj_warehouse.IsFulfillmentCenter)
                {
                    WarehouseLevel = 3; //For FV
                }
                else
                {
                    WarehouseLevel = 2; //For DV
                }
                //}
            }
            Session["lPurchaseOrderReplyDetailViewModel"] = null;

            PurchaseOrderViewModel = (from po in db.PurchaseOrders
                                      join s in db.Suppliers on po.SupplierID equals s.ID
                                      join w in db.Warehouses on po.WarehouseID equals w.ID
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
                                          IsSent = po.IsSent,
                                          IsAcceptedBySupplier = po.IsAcceptedBySupplier,
                                          IsActive = po.IsActive,
                                          InvoiceCount = db.PurchaseOrderReply.Where(x => x.PurchaseOrderID == id).Select(x => (int)x.ID).FirstOrDefault()

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
                objPOD.UnitPrice = item.UnitPrice == null ? 0 : item.UnitPrice;
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
                double PurchasePrice = 0;
                RateMatrixExtension objrateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ID == item.RateMatrixExtensionId);
                if (WarehouseLevel == 2) //For DV
                {
                    PurchasePrice = objrateMatrixExtension.DVPurchasePrice;
                }
                else if (WarehouseLevel == 3) //For FV
                {
                    PurchasePrice = objrateMatrixExtension.FVPurchasePrice;
                }

                //if want to take total quantity of item for all PurchaseOrder invoices
                int availableQuantity = 0;
                decimal WSaleRatePerUnit = Convert.ToDecimal(PurchasePrice);

                tempQuantity = (from pd in db.PurchaseOrderReplyDetails
                                join pr in db.PurchaseOrderReply on pd.PurchaseOrderReplyID equals pr.ID
                                join pod in db.PurchaseOrderDetails on pr.PurchaseOrderID equals pod.PurchaseOrderID
                                join po in db.PurchaseOrders on pr.PurchaseOrderID equals po.ID
                                where pd.ProductID == item.ProductID && pd.ProductVarientID == item.ProductVarientID
                                && pod.RateMatrixId == item.RateMatrixId
                                && pod.RateMatrixExtensionId == item.RateMatrixExtensionId
                                && po.SupplierID == purchaseOrder.SupplierID
                                //&& pd.BuyRatePerUnit == item.UnitPrice 
                                && item.UnitPrice == WSaleRatePerUnit && pr.IsReplied == false
                                select new PurchaseOrderReplyDetailViewModel { TempQuantity = pd.Quantity }).ToList();

                var availableQty = (from ws in db.WarehouseStocks
                                    join i in db.Invoices on ws.InvoiceID equals i.ID
                                    join po in db.PurchaseOrders on i.PurchaseOrderID equals po.ID
                                    join pod in db.PurchaseOrderDetails on po.ID equals pod.PurchaseOrderID
                                    where ws.WarehouseID == WarehouseID && ws.ProductID == item.ProductID && ws.ProductVarientID == item.ProductVarientID
                                     && pod.RateMatrixId == item.RateMatrixId
                                     && pod.RateMatrixExtensionId == item.RateMatrixExtensionId
                                    && item.UnitPrice == WSaleRatePerUnit
                                    select new WarehouseStockViewModel { AvailableQuantity = ws.AvailableQuantity }
                                             ).Select(x => (int)x.AvailableQuantity).DefaultIfEmpty(0).Sum();



                availableQuantity = Convert.ToInt32(availableQty);

                // Deduct tempQuantity from availableQuantity
                if (tempQuantity.Count > 0)
                {
                    sumTempQuantity = tempQuantity.Sum(x => x.TempQuantity);
                    availableQuantity = availableQuantity - sumTempQuantity;
                }

                objPOD.AvailableQuantity = availableQuantity;
                lPurchaseOrderDetailsViewModelList.Add(objPOD);
            }

            if (lPurchaseOrderDetailsViewModelList.Count > 0)
            {
                Session["lPurchaseOrderReplyDetailViewModel"] = lPurchaseOrderDetailsViewModelList;
            }

            ViewBag.PurchaseOrderID = id;
            return View(PurchaseOrderViewModel);
        }


        public ActionResult Details(int PurchaseOrderReplyID)
        {
            PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();
            long PurchaseOrderID = db.PurchaseOrderReply.Where(x => x.ID == PurchaseOrderReplyID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;


            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
            if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
            {
                ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " under FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
            }
            var PurchaseOrderReply = db.PurchaseOrderReply.Single(x => x.ID == PurchaseOrderReplyID);

            if (PurchaseOrderReply != null && PurchaseOrderReply.ToString() != "")
            {
                PurchaseOrderReplyViewModel.PurchaseOrderReplyID = PurchaseOrderReplyID;
                PurchaseOrderReplyViewModel.InvoiceCode = PurchaseOrderReply.InvoiceCode;
                PurchaseOrderReplyViewModel.PurchaseOrderID = PurchaseOrderID;
                PurchaseOrderReplyViewModel.WarehouseID = purchaseOrder.WarehouseID;
                PurchaseOrderReplyViewModel.WarehouseName = db.Warehouses.Where(x => x.ID == purchaseOrder.WarehouseID).Select(x => x.Name).FirstOrDefault();
                PurchaseOrderReplyViewModel.SupplierID = purchaseOrder.SupplierID;
                PurchaseOrderReplyViewModel.SupplierName = db.Suppliers.Where(x => x.ID == purchaseOrder.SupplierID).Select(x => x.Name).FirstOrDefault();
                //PurchaseOrderReplyViewModel.InvoiceDate = Invoice.InvoiceDate;
                PurchaseOrderReplyViewModel.TotalDiscountAmount = Convert.ToDecimal(PurchaseOrderReply.TotalDiscountAmount);
                PurchaseOrderReplyViewModel.ShippingCharge = Convert.ToDecimal(PurchaseOrderReply.ShippingCharge);
                PurchaseOrderReplyViewModel.CustomDutyCharge = Convert.ToDecimal(PurchaseOrderReply.CustomDutyCharge);
                PurchaseOrderReplyViewModel.OperatingCost = Convert.ToDecimal(PurchaseOrderReply.OperatingCost);
                PurchaseOrderReplyViewModel.AdditionalCost = Convert.ToDecimal(PurchaseOrderReply.AdditionalCost);
                PurchaseOrderReplyViewModel.OrderAmount = Convert.ToDecimal(PurchaseOrderReply.OrderAmount);
                PurchaseOrderReplyViewModel.TotalAmount = Convert.ToDecimal(PurchaseOrderReply.TotalAmount);
                PurchaseOrderReplyViewModel.Remark = PurchaseOrderReply.Remark == null ? "" : PurchaseOrderReply.Remark;
                PurchaseOrderReplyViewModel.DispatchDate = PurchaseOrderReply.DispatchDate;
                PurchaseOrderReplyViewModel.DriverName = PurchaseOrderReply.DriverName;
                PurchaseOrderReplyViewModel.DriverMobileNumber = PurchaseOrderReply.DriverMobileNumber;
                PurchaseOrderReplyViewModel.DriverLicenceNumber = PurchaseOrderReply.DriverLicenceNumber;
                PurchaseOrderReplyViewModel.VehicleNumber = PurchaseOrderReply.VehicleNumber;
                PurchaseOrderReplyViewModel.VehicleType = PurchaseOrderReply.VehicleType;
                PurchaseOrderReplyViewModel.LogisticCompanyName = PurchaseOrderReply.LogisticCompanyName;
                PurchaseOrderReplyViewModel.LogisticCompanyAddress = PurchaseOrderReply.LogisticCompanyAddress;
                PurchaseOrderReplyViewModel.LogisticContactPerson = PurchaseOrderReply.LogisticContactPerson;
                PurchaseOrderReplyViewModel.LogisticContactNumber = PurchaseOrderReply.LogisticContactNumber;
                PurchaseOrderReplyViewModel.EWayBillNumber = PurchaseOrderReply.EWayBillNumber;
                PurchaseOrderReplyViewModel.TrackingNumber = PurchaseOrderReply.TrackingNumber;
                PurchaseOrderReplyViewModel.IsActive = PurchaseOrderReply.IsActive;
                PurchaseOrderReplyViewModel.CreateDate = PurchaseOrderReply.CreateDate;
                PurchaseOrderReplyViewModel.DeliveryDateTime = PurchaseOrderReply.DeliveryDateTime;
            }

            List<PurchaseOrderReplyDetail> lPurchaseOrderReplyDetaillist = new List<PurchaseOrderReplyDetail>();
            lPurchaseOrderReplyDetaillist = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == PurchaseOrderReplyID).ToList();

            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModelList = new List<PurchaseOrderReplyDetailViewModel>();


            foreach (PurchaseOrderReplyDetail item in lPurchaseOrderReplyDetaillist)
            {
                PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();
                //foreach (var inv in lInvoiceDetaillist)
                //{
                //    if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
                //    {
                objPOD.Quantity = item.Quantity;
                objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                objPOD.MRP = item.MRP;
                objPOD.SaleRate = item.SaleRate;
                objPOD.GSTInPer = item.GSTInPer;
                objPOD.CGSTAmount = item.Quantity * item.CGSTAmount;
                objPOD.SGSTAmount = item.Quantity * item.SGSTAmount;
                objPOD.ProductAmount = Convert.ToDecimal(item.Amount);
                //objPOD.ExpiryDate = item.ExpiryDate;
                objPOD.ProductRemark = item.Remark == null ? "" : item.Remark;

                //var expirydate = db.WarehouseStocks.Where(x => x.InvoiceID == InvoiceID && x.ProductID == inv.ProductID && x.ProductVarientID == inv.ProductVarientID).Select(x => x.ExpiryDate).FirstOrDefault();
                //objPOD.ExpiryDate = expirydate;
                //    }
                //}

                objPOD.PurchaseOrderReplyDetailsID = item.ID;
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

                lPurchaseOrderReplyDetailViewModelList.Add(objPOD);
            }
            if (lPurchaseOrderReplyDetailViewModelList.Count > 0)
            {
                Session["lPurchaseOrderReplyDetailViewModel"] = lPurchaseOrderReplyDetailViewModelList;
                PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailViewModelList;
            }

            return View(PurchaseOrderReplyViewModel);
        }


        //
        // GET: /PurchaseOrderReply/Accept
        public ActionResult Accept(int id)
        {
            PurchaseOrderReplyViewModel objPO = new PurchaseOrderReplyViewModel();
            var purchaseOrderDetail = db.PurchaseOrders.Where(x => x.ID == id).FirstOrDefault();
            objPO.PurchaseOrderID = id;
            objPO.DeliveryDateTime = System.DateTime.Now;
            objPO.DispatchDate = System.DateTime.Now;
            objPO.ExpetedDeliveryDate = purchaseOrderDetail.ExpetedDeliveryDate;
            objPO.PurchaseOrderCode = purchaseOrderDetail.PurchaseOrderCode;
            return View(objPO);
        }


        //
        // GET: /PurchaseOrderReply/Accept
        [HttpPost]
        public ActionResult Accept(PurchaseOrderReplyViewModel purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope tscope = new TransactionScope())
                {
                    try
                    {
                        //Update PurchaseOrder Table                       
                        var lPurchaseOrder = new PurchaseOrder()
                        {
                            ID = purchaseOrder.PurchaseOrderID,
                            IsAcceptedBySupplier = 1,
                            AcceptedByID = GetPersonalDetailID(),
                            SupplierRemark = purchaseOrder.SupplierRemark,
                            DeliveryDate = purchaseOrder.DeliveryDateTime
                        };

                        db.PurchaseOrders.Attach(lPurchaseOrder);
                        db.Entry(lPurchaseOrder).Property(x => x.IsAcceptedBySupplier).IsModified = true;
                        db.Entry(lPurchaseOrder).Property(x => x.AcceptedByID).IsModified = true;
                        db.Entry(lPurchaseOrder).Property(x => x.SupplierRemark).IsModified = true;
                        db.Entry(lPurchaseOrder).Property(x => x.DeliveryDate).IsModified = true;
                        db.SaveChanges();

                        tscope.Complete();
                        Session["Success"] = purchaseOrder.PurchaseOrderCode + " Order Accepted Successfully."; //Yashaswi 31/3/2018
                    }
                    catch
                    {
                        Transaction.Current.Rollback();
                        tscope.Dispose();
                    }
                }
                return RedirectToAction("View", new { id = purchaseOrder.PurchaseOrderID });
            }
            return View(purchaseOrder);
        }


        // GET: /PurchaseOrderReply/Reject
        public ActionResult Reject(int id)
        {
            PurchaseOrderReplyViewModel objPO = new PurchaseOrderReplyViewModel();
            var purchaseOrderDetail = db.PurchaseOrders.Where(x => x.ID == id).FirstOrDefault();
            objPO.PurchaseOrderID = id;
            objPO.PurchaseOrderCode = purchaseOrderDetail.PurchaseOrderCode;
            objPO.DeliveryDateTime = System.DateTime.Now;
            objPO.DispatchDate = System.DateTime.Now;
            objPO.ExpetedDeliveryDate = purchaseOrderDetail.ExpetedDeliveryDate;
            return View(objPO);
        }

        [HttpPost]
        public ActionResult Reject(PurchaseOrderReplyViewModel purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope tscope = new TransactionScope())
                {
                    try
                    {
                        if (purchaseOrder.SupplierRemark == null || purchaseOrder.SupplierRemark == "")
                        {
                            Session["Error"] = "Kindly provide remark!";
                            return RedirectToAction("Reject", purchaseOrder.PurchaseOrderID);
                        }
                        //Update PurchaseOrder Table                       
                        var lPurchaseOrder = new PurchaseOrder()
                        {
                            ID = purchaseOrder.PurchaseOrderID,
                            IsAcceptedBySupplier = 2,
                            AcceptedByID = GetPersonalDetailID(),
                            DeliveryDate = System.DateTime.Now,
                            SupplierRemark = purchaseOrder.SupplierRemark
                        };

                        db.PurchaseOrders.Attach(lPurchaseOrder);
                        db.Entry(lPurchaseOrder).Property(x => x.IsAcceptedBySupplier).IsModified = true;
                        db.Entry(lPurchaseOrder).Property(x => x.AcceptedByID).IsModified = true;
                        db.Entry(lPurchaseOrder).Property(x => x.DeliveryDate).IsModified = true;
                        db.Entry(lPurchaseOrder).Property(x => x.SupplierRemark).IsModified = true;
                        db.SaveChanges();

                        tscope.Complete();
                        Session["Success"] = purchaseOrder.PurchaseOrderCode + " Order Rejected Successfully.";
                    }
                    catch (Exception ex)
                    {
                        Transaction.Current.Rollback();
                        tscope.Dispose();
                    }
                }

                var PoDetail = (from po in db.PurchaseOrders
                                join w in db.Warehouses on po.WarehouseID equals w.ID
                                join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                join s in db.Suppliers on po.SupplierID equals s.ID
                                where po.ID == purchaseOrder.PurchaseOrderID
                                select new PurchaseOrderReplyViewModel
                                {
                                    PurchaseOrderCode = po.PurchaseOrderCode,
                                    WarehosueEmail = b.Email,
                                    WarehouseName = w.Name,
                                    WarehouseContactPerson = b.ContactPerson,
                                    SupplierID = po.SupplierID,
                                    SupplierName = s.Name,
                                    CreateDate = po.OrderDate

                                }).FirstOrDefault();


                var URI = @"" + WebConfigurationManager.AppSettings["INVENTORY_ROOT"] + "PurchaseOrders/GetId?id=" + purchaseOrder.PurchaseOrderID + "";

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--PURCHASE_ORDER_CODE-->", PoDetail.PurchaseOrderCode);
                dictEmailValues.Add("<!--REASON-->", purchaseOrder.SupplierRemark);
                dictEmailValues.Add("<!--SUPPLIER_NAME-->", PoDetail.SupplierName);
                dictEmailValues.Add("<!--WAREHOUSE_NAME-->", PoDetail.WarehouseName);
                dictEmailValues.Add("<!--ORDER_DATE-->", PoDetail.CreateDate.ToString("dd MMM yyyy"));
                dictEmailValues.Add("<!--REJECT_DATE-->", System.DateTime.Now.ToString("dd MMM yyyy"));
                dictEmailValues.Add("<!--URL-->", URI);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = PoDetail.WarehosueEmail;

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PURCHASE_ORDER_REFUSAL, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);

                return RedirectToAction("Index");
            }
            return View(purchaseOrder);
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
                throw new BusinessLogicLayer.MyException("[PurchaseOrderReplyController][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }


        //
        // GET: /PurchaseOrderReply/Create
        public ActionResult CreateInvoice(long PurchaseOrderID)
        {
            List<PurchaseOrderReplyViewModel> lPurchaseOrderReplyViewModel = new List<PurchaseOrderReplyViewModel>();
            try
            {
                //Check session
                if (Session["USER_NAME"] != null)
                { }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.IsPOCompleted = IsPOCompleted(PurchaseOrderID);

                ViewBag.PurchaseOrderID = PurchaseOrderID;
                ViewBag.PurchaseOrderCode = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.PurchaseOrderCode).FirstOrDefault();

                lPurchaseOrderReplyViewModel = (from o in db.PurchaseOrderReply
                                                where o.PurchaseOrderID == PurchaseOrderID
                                                select new PurchaseOrderReplyViewModel
                                                {

                                                    InvoiceCode = o.InvoiceCode,
                                                    PurchaseOrderReplyID = o.ID,
                                                    PurchaseOrderID = o.PurchaseOrderID,
                                                    OrderAmount = o.OrderAmount,
                                                    ReplyDate = o.ReplyDate,
                                                    TotalDiscountAmount = o.TotalDiscountAmount,
                                                    ShippingCharge = o.ShippingCharge,
                                                    CustomDutyCharge = o.CustomDutyCharge,
                                                    OperatingCost = o.OperatingCost,
                                                    AdditionalCost = o.AdditionalCost,
                                                    TotalAmount = o.TotalAmount,
                                                    TotalItems = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == o.ID).Select(x => x.ID).Count(),
                                                    IsReplied = o.IsReplied,
                                                    CreateDate = o.CreateDate
                                                }).OrderByDescending(o => o.ReplyDate).ToList();
            }
            catch (Exception ex)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("CreateInvoice", lPurchaseOrderReplyViewModel);
        }

        private bool IsPOCompleted(long PurchaseOrderID)
        {
            bool flag = true;
            try
            {
                int sentQuantity = 0;
                List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
                lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();
                foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
                {
                    sentQuantity = Convert.ToInt32(db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderDetailID == item.ID).DefaultIfEmpty().Sum(x => x.Quantity == null ? 0 : x.Quantity));
                    if (sentQuantity != item.Quantity)
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


        [HandleError(View = "Error")]
        public ActionResult Create(long PurchaseOrderID)
        {
            PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();
            try
            {
                //long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
                //ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == warehouseID);
                //ViewBag.WarehouseID = warehouseID;

                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }


                ViewBag.PossibleSuppliers = db.Suppliers;
                Session["lPurchaseOrderReplyDetailViewModel"] = null;
                Session["Amount"] = null;

                var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
                if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
                {
                    ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " under FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                }
                if (purchaseOrder != null && purchaseOrder.ToString() != "")
                {
                    PurchaseOrderReplyViewModel.PurchaseOrderID = PurchaseOrderID;
                    PurchaseOrderReplyViewModel.WarehouseID = purchaseOrder.WarehouseID;
                    PurchaseOrderReplyViewModel.SupplierID = purchaseOrder.SupplierID;
                    PurchaseOrderReplyViewModel.ReplyDate = System.DateTime.Now;
                    PurchaseOrderReplyViewModel.IsActive = purchaseOrder.IsActive;
                }

                int WarehouseLevel = 0;
                Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == purchaseOrder.WarehouseID);
                if (obj_warehouse != null)
                {
                    if (obj_warehouse.IsFulfillmentCenter)
                    {
                        WarehouseLevel = 3; //For FV
                    }
                    else
                    {
                        WarehouseLevel = 2; //For DV
                    }
                }
                List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
                lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

                List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModelList = new List<PurchaseOrderReplyDetailViewModel>();

                foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
                {
                    PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();

                    int sentQuantity = 0;
                    var tempQuantity = new List<PurchaseOrderReplyDetailViewModel>();
                    int sumTempQuantity = 0;
                    long poid = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderDetailID == item.ID).Select(x => x.PurchaseOrderDetailID).FirstOrDefault();
                    if (poid > 0)
                    {
                        sentQuantity = Convert.ToInt32(db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderDetailID == item.ID).Sum(x => x.Quantity == null ? 0 : x.Quantity));

                        //take quantity of item from invoice which is created but not sent 
                        //if want to take total quanitity of item within a single PurchaseOrder invoices
                    }
                    if (sentQuantity != item.Quantity)
                    {
                        objPOD.PurchaseOrderReplyDetailsID = 0;
                        objPOD.PurchaseOrderDetailID = item.ID;
                        objPOD.ProductID = item.ProductID;
                        objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                        objPOD.RequiredQuantity = item.Quantity - sentQuantity;
                        objPOD.BuyRatePerUnit = item.UnitPrice;
                        objPOD.ProductAmount = objPOD.Quantity * item.UnitPrice;
                        objPOD.ProductNickname = item.ProductNickname;

                        RateMatrixExtension objrateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ID == item.RateMatrixExtensionId);
                        RateMatrix objrateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == objrateMatrixExtension.RateMatrixId);
                        double WarehouseSaleRatePerUnit = 0;
                        if (objrateMatrixExtension != null)
                        {
                            objPOD.MRP = Convert.ToDecimal(objrateMatrix.MRP);
                            objPOD.GSTInPer = Convert.ToInt32(objrateMatrix.GSTInPer);
                            if (WarehouseLevel == 2) //For DV
                            {
                                WarehouseSaleRatePerUnit = objrateMatrixExtension.DVPurchasePrice;
                                objPOD.SaleRate = Convert.ToDecimal(objrateMatrixExtension.DVSalePrice);
                            }
                            else if (WarehouseLevel == 3) //For FV
                            {
                                WarehouseSaleRatePerUnit = objrateMatrixExtension.FVPurchasePrice;
                                objPOD.SaleRate = Convert.ToDecimal(objrateMatrixExtension.FVSalePrice);
                            }
                            double GSTAmount = 0;
                            GSTAmount = Math.Round((double)(WarehouseSaleRatePerUnit - (WarehouseSaleRatePerUnit / (1 + (double)objPOD.GSTInPer / 100))), 2);
                            objPOD.CGSTAmount = Convert.ToDecimal(GSTAmount / Convert.ToDouble(2));
                            objPOD.SGSTAmount = Convert.ToDecimal(GSTAmount / Convert.ToDouble(2));
                        }


                        //objPOD.ReceivedQuantity = Convert.ToInt32("");
                        objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

                        var itemName = (from p in db.Products
                                        join v in db.ProductVarients on p.ID equals v.ProductID
                                        join s in db.Sizes on v.SizeID equals s.ID
                                        where v.ID == item.ProductVarientID
                                        select new PurchaseOrderReplyDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                        foreach (var i in itemName)
                        {
                            objPOD.ItemName = i.ItemName.ToString();
                            objPOD.HSNCode = Convert.ToString(i.HSNCode);
                        }

                        int availableQuantity = 0;
                        decimal WSaleRatePerUnit = Convert.ToDecimal(WarehouseSaleRatePerUnit);

                        //if want to take total quantity of item for all PurchaseOrder invoices

                        tempQuantity = (from pd in db.PurchaseOrderReplyDetails
                                        join pr in db.PurchaseOrderReply on pd.PurchaseOrderReplyID equals pr.ID
                                        join pod in db.PurchaseOrderDetails on pr.PurchaseOrderID equals pod.PurchaseOrderID
                                        join po in db.PurchaseOrders on pr.PurchaseOrderID equals po.ID
                                        where pd.ProductID == item.ProductID && pd.ProductVarientID == item.ProductVarientID
                                         && pod.RateMatrixId == item.RateMatrixId
                                        && pod.RateMatrixExtensionId == item.RateMatrixExtensionId
                                        && po.SupplierID == purchaseOrder.SupplierID
                                        //&& pd.BuyRatePerUnit == item.UnitPrice 
                                        && item.UnitPrice == WSaleRatePerUnit && pr.IsReplied == false
                                        select new PurchaseOrderReplyDetailViewModel { TempQuantity = pd.Quantity }).ToList();

                        var availableQty = (from ws in db.WarehouseStocks
                                            join i in db.Invoices on ws.InvoiceID equals i.ID
                                            join po in db.PurchaseOrders on i.PurchaseOrderID equals po.ID
                                            join pod in db.PurchaseOrderDetails on po.ID equals pod.PurchaseOrderID
                                            where ws.WarehouseID == WarehouseID && ws.ProductID == item.ProductID && ws.ProductVarientID == item.ProductVarientID
                                             && pod.RateMatrixId == item.RateMatrixId
                                                && pod.RateMatrixExtensionId == item.RateMatrixExtensionId
                                            && item.UnitPrice == WSaleRatePerUnit
                                            select new WarehouseStockViewModel { AvailableQuantity = ws.AvailableQuantity }
                                                     ).Select(x => (int)x.AvailableQuantity).DefaultIfEmpty(0).Sum();

                        //var a = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID
                        //   && item.UnitPrice == WSaleRatePerUnit).Sum(x => (int?)x.AvailableQuantity);
                        //
                        availableQuantity = Convert.ToInt32(availableQty);

                        // Deduct tempQuantity from availableQuantity
                        if (tempQuantity.Count > 0)
                        {
                            sumTempQuantity = tempQuantity.Sum(x => x.TempQuantity);
                            availableQuantity = availableQuantity - sumTempQuantity;
                        }

                        objPOD.AvailableQuantity = availableQuantity;
                        lPurchaseOrderReplyDetailViewModelList.Add(objPOD);
                    }
                }

                if (lPurchaseOrderReplyDetailViewModelList.Count > 0)
                {
                    Session["lPurchaseOrderReplyDetailViewModel"] = lPurchaseOrderReplyDetailViewModelList;
                    PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailViewModelList;
                }
                else
                {
                    Session["Info"] = purchaseOrder.PurchaseOrderCode + " Order is completed! You can't create new Invoice."; //yashaswi31/3/2018
                    return RedirectToAction("Index", new { PurchaseOrderID = PurchaseOrderID });
                }

                ViewBag.PurchaseOrderID = PurchaseOrderID;

            }
            catch (Exception ex)
            {
                throw new Exception("Some unknown error encountered!");
                //return View();

                //throw new BusinessLogicLayer.MyException("[PurchaseOrderReplyController][Create]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return View(PurchaseOrderReplyViewModel);
        }


        //
        // POST: /PurchaseOrderReply/Create
        [HttpPost]
        public ActionResult Create(PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel)
        {
            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModel = new List<PurchaseOrderReplyDetailViewModel>();

            //if (Session["lPurchaseOrderDetailsViewModel"] != null && Session["Amount"] != null)
            //{
            lPurchaseOrderReplyDetailViewModel = PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels;
            if (lPurchaseOrderReplyDetailViewModel.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            if (PurchaseOrderReplyViewModel.OrderAmount > 0)
                            {
                                //Insert into PurchaseOrder Table
                                PurchaseOrderReply objPurchaseOrderReply = new PurchaseOrderReply();
                                PurchaseOrderReplyViewModel.TotalAmount = (Convert.ToDecimal(PurchaseOrderReplyViewModel.OrderAmount) + Convert.ToDecimal(PurchaseOrderReplyViewModel.ShippingCharge) + Convert.ToDecimal(PurchaseOrderReplyViewModel.CustomDutyCharge) + Convert.ToDecimal(PurchaseOrderReplyViewModel.OperatingCost) + Convert.ToDecimal(PurchaseOrderReplyViewModel.AdditionalCost)) - Convert.ToDecimal(PurchaseOrderReplyViewModel.TotalDiscountAmount);
                                objPurchaseOrderReply.PurchaseOrderID = PurchaseOrderReplyViewModel.PurchaseOrderID;
                                objPurchaseOrderReply.InvoiceCode = InvoiceCode();
                                objPurchaseOrderReply.IsReplied = false;
                                //if (PurchaseOrderReplyViewModel.IsReplied==true)
                                //{
                                //    objPurchaseOrderReply.ReplyDate = PurchaseOrderReplyViewModel.ReplyDate;
                                //    objPurchaseOrderReply.RepliedBy = GetPersonalDetailID();
                                //}
                                objPurchaseOrderReply.DeliveryDateTime = Convert.ToDateTime(PurchaseOrderReplyViewModel.DeliveryDateTime);
                                objPurchaseOrderReply.TotalDiscountAmount = Convert.ToDecimal(PurchaseOrderReplyViewModel.TotalDiscountAmount);
                                objPurchaseOrderReply.OrderAmount = PurchaseOrderReplyViewModel.OrderAmount;
                                objPurchaseOrderReply.ShippingCharge = Convert.ToDecimal(PurchaseOrderReplyViewModel.ShippingCharge);
                                objPurchaseOrderReply.CustomDutyCharge = Convert.ToDecimal(PurchaseOrderReplyViewModel.CustomDutyCharge);
                                objPurchaseOrderReply.OperatingCost = Convert.ToDecimal(PurchaseOrderReplyViewModel.OperatingCost);
                                objPurchaseOrderReply.AdditionalCost = Convert.ToDecimal(PurchaseOrderReplyViewModel.AdditionalCost);
                                objPurchaseOrderReply.TotalAmount = PurchaseOrderReplyViewModel.TotalAmount;
                                objPurchaseOrderReply.Remark = PurchaseOrderReplyViewModel.Remark;
                                objPurchaseOrderReply.DispatchDate = PurchaseOrderReplyViewModel.DispatchDate;
                                objPurchaseOrderReply.DriverName = PurchaseOrderReplyViewModel.DriverName;
                                objPurchaseOrderReply.DriverMobileNumber = PurchaseOrderReplyViewModel.DriverMobileNumber;
                                objPurchaseOrderReply.DriverLicenceNumber = PurchaseOrderReplyViewModel.DriverLicenceNumber;
                                objPurchaseOrderReply.VehicleNumber = PurchaseOrderReplyViewModel.VehicleNumber;
                                objPurchaseOrderReply.VehicleType = PurchaseOrderReplyViewModel.VehicleType;
                                objPurchaseOrderReply.LogisticCompanyName = PurchaseOrderReplyViewModel.LogisticCompanyName;
                                objPurchaseOrderReply.LogisticCompanyAddress = PurchaseOrderReplyViewModel.LogisticCompanyAddress;
                                objPurchaseOrderReply.LogisticContactPerson = PurchaseOrderReplyViewModel.LogisticContactPerson;
                                objPurchaseOrderReply.LogisticContactNumber = PurchaseOrderReplyViewModel.LogisticContactNumber;
                                objPurchaseOrderReply.EWayBillNumber = PurchaseOrderReplyViewModel.EWayBillNumber;
                                objPurchaseOrderReply.TrackingNumber = PurchaseOrderReplyViewModel.TrackingNumber;
                                objPurchaseOrderReply.IsActive = true;
                                objPurchaseOrderReply.CreateDate = DateTime.Now;
                                objPurchaseOrderReply.CreateBy = GetPersonalDetailID();
                                objPurchaseOrderReply.NetworkIP = CommonFunctions.GetClientIP();
                                objPurchaseOrderReply.DeviceID = "X";
                                objPurchaseOrderReply.DeviceType = "X";
                                db.PurchaseOrderReply.Add(objPurchaseOrderReply);
                                db.SaveChanges();

                                long PurchaseOrderReplyID = objPurchaseOrderReply.ID;

                                //Insert into InvoiceDetail Table
                                PurchaseOrderReplyDetail objPurchaseOrderReplyDetail = new PurchaseOrderReplyDetail();
                                decimal TotalGSTAmount = 0;
                                foreach (PurchaseOrderReplyDetailViewModel item in lPurchaseOrderReplyDetailViewModel)
                                {
                                    if (item.Quantity > 0)
                                    {
                                        TotalGSTAmount = TotalGSTAmount + Convert.ToDecimal(item.CGSTAmount) + Convert.ToDecimal(item.SGSTAmount) + Convert.ToDecimal(item.IGSTAmount);
                                        objPurchaseOrderReplyDetail.PurchaseOrderReplyID = PurchaseOrderReplyID;
                                        objPurchaseOrderReplyDetail.PurchaseOrderDetailID = item.PurchaseOrderDetailID;
                                        objPurchaseOrderReplyDetail.ProductID = item.ProductID;
                                        objPurchaseOrderReplyDetail.ProductVarientID = item.ProductVarientID;
                                        objPurchaseOrderReplyDetail.IsExtraItem = false;
                                        objPurchaseOrderReplyDetail.BuyRatePerUnit = item.BuyRatePerUnit;
                                        objPurchaseOrderReplyDetail.MRP = item.MRP;
                                        objPurchaseOrderReplyDetail.SaleRate = item.SaleRate;
                                        objPurchaseOrderReplyDetail.Quantity = Convert.ToInt32(item.Quantity);
                                        objPurchaseOrderReplyDetail.Amount = Convert.ToDecimal(item.Amount);
                                        objPurchaseOrderReplyDetail.GSTInPer = item.GSTInPer;
                                        objPurchaseOrderReplyDetail.CGSTAmount = Convert.ToDecimal(item.CGSTAmount);
                                        objPurchaseOrderReplyDetail.SGSTAmount = Convert.ToDecimal(item.SGSTAmount);
                                        objPurchaseOrderReplyDetail.IGSTAmount = Convert.ToDecimal(item.IGSTAmount);
                                        objPurchaseOrderReplyDetail.Remark = item.ProductRemark;
                                        objPurchaseOrderReplyDetail.IsActive = true;
                                        db.PurchaseOrderReplyDetails.Add(objPurchaseOrderReplyDetail);
                                        db.SaveChanges();
                                    }
                                }

                                //Update GSTAmount
                                var PurchaseOrderReply = db.PurchaseOrderReply.First(x => x.ID == PurchaseOrderReplyID);
                                PurchaseOrderReply.GSTAmount = TotalGSTAmount;
                                db.Entry(PurchaseOrderReply).State = EntityState.Modified;

                                db.SaveChanges();

                                tscope.Complete();
                                Session["Success"] = "Invoice Created Successfully."; //yashaswi31/3/2018
                            }
                        }
                        catch (Exception)
                        {
                            Transaction.Current.Rollback();
                            tscope.Dispose();
                        }
                    }

                    return RedirectToAction("CreateInvoice", new { PurchaseOrderID = PurchaseOrderReplyViewModel.PurchaseOrderID });
                }
            }
            //}
            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleSuppliers = db.Suppliers;
            return View(PurchaseOrderReplyViewModel);
        }


        private string InvoiceCode()
        {
            string newOrderCode = string.Empty;
            int lYear = 0;
            int lMonth = 0;
            int lDay = 0;
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Year.ToString(), out lYear);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Month.ToString(), out lMonth);
            int.TryParse(DateTime.UtcNow.AddHours(5.5).Day.ToString(), out lDay);
            string lOrderPrefix = "EZIN" + lYear.ToString().Substring(2, 2) + lMonth.ToString("00") + lDay.ToString("00");

            try
            {
                OrderManagement lOrderManagement = new OrderManagement();
                int lWHPO = GetNextInvoiceCode();
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

        private int GetNextInvoiceCode()
        {
            int lEZIN = -1;

            try
            {
                DataTable lDataTableCustomerOrder = new DataTable();
                SqlConnection con = new SqlConnection(fConnectionString);
                SqlCommand sqlComm = new SqlCommand("SelectNextInvoiceCode", con);
                sqlComm.CommandType = CommandType.StoredProcedure;
                //sqlComm.Parameters.AddWithValue("@pFranchiseID", SqlDbType.Int).Value = pFranchiseID;
                con.Open();
                //object o = sqlComm.ExecuteScalar();
                SqlDataAdapter da = new SqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    lEZIN = Convert.ToInt32(dt.Rows[0][0]);
                }
                con.Close();
                return lEZIN;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[PurchaseOrderReply -> GetNextInvoiceCode]", "Problem in getting EZIN" + Environment.NewLine + ex.Message);
            }
        }

        //
        // GET: /PurchaseOrderReply/Edit/5
        public ActionResult Edit(long PurchaseOrderReplyID)
        {
            PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();
            try
            {
                long PurchaseOrderID = db.PurchaseOrderReply.Where(x => x.ID == PurchaseOrderReplyID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                ViewBag.PurchaseOrderID = PurchaseOrderID;
                long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
                ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == warehouseID);
                ViewBag.WarehouseID = warehouseID;

                //Check session
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
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                int WarehouseLevel = 0;
                Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == warehouseID);
                if (obj_warehouse != null)
                {
                    if (obj_warehouse.IsFulfillmentCenter)
                    {
                        WarehouseLevel = 3; //For FV
                    }
                    else
                    {
                        WarehouseLevel = 2; //For DV
                    }
                }

                var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == PurchaseOrderID);
                if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
                {
                    ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " under FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                }
                var PurchaseOrderReply = db.PurchaseOrderReply.Single(x => x.ID == PurchaseOrderReplyID);

                if (PurchaseOrderReply != null && PurchaseOrderReply.ToString() != "")
                {
                    PurchaseOrderReplyViewModel.PurchaseOrderReplyID = PurchaseOrderReplyID;
                    PurchaseOrderReplyViewModel.PurchaseOrderID = PurchaseOrderID;
                    PurchaseOrderReplyViewModel.WarehouseID = purchaseOrder.WarehouseID;
                    PurchaseOrderReplyViewModel.SupplierID = purchaseOrder.SupplierID;
                    PurchaseOrderReplyViewModel.CreateDate = purchaseOrder.CreateDate;
                    PurchaseOrderReplyViewModel.InvoiceCode = PurchaseOrderReply.InvoiceCode;
                    PurchaseOrderReplyViewModel.TotalDiscountAmount = PurchaseOrderReply.TotalDiscountAmount;
                    PurchaseOrderReplyViewModel.ShippingCharge = PurchaseOrderReply.ShippingCharge;
                    PurchaseOrderReplyViewModel.CustomDutyCharge = PurchaseOrderReply.CustomDutyCharge;
                    PurchaseOrderReplyViewModel.OperatingCost = PurchaseOrderReply.OperatingCost;
                    PurchaseOrderReplyViewModel.AdditionalCost = PurchaseOrderReply.AdditionalCost;
                    PurchaseOrderReplyViewModel.OrderAmount = PurchaseOrderReply.OrderAmount;
                    PurchaseOrderReplyViewModel.DeliveryDateTime = PurchaseOrderReply.DeliveryDateTime;
                    PurchaseOrderReplyViewModel.Remark = PurchaseOrderReply.Remark;
                    PurchaseOrderReplyViewModel.DispatchDate = PurchaseOrderReply.DispatchDate;
                    PurchaseOrderReplyViewModel.DriverName = PurchaseOrderReply.DriverName;
                    PurchaseOrderReplyViewModel.DriverMobileNumber = PurchaseOrderReply.DriverMobileNumber;
                    PurchaseOrderReplyViewModel.DriverLicenceNumber = PurchaseOrderReply.DriverLicenceNumber;
                    PurchaseOrderReplyViewModel.VehicleNumber = PurchaseOrderReply.VehicleNumber;
                    PurchaseOrderReplyViewModel.VehicleType = PurchaseOrderReply.VehicleType;
                    PurchaseOrderReplyViewModel.LogisticCompanyName = PurchaseOrderReply.LogisticCompanyName;
                    PurchaseOrderReplyViewModel.LogisticCompanyAddress = PurchaseOrderReply.LogisticCompanyAddress;
                    PurchaseOrderReplyViewModel.LogisticContactPerson = PurchaseOrderReply.LogisticContactPerson;
                    PurchaseOrderReplyViewModel.LogisticContactNumber = PurchaseOrderReply.LogisticContactNumber;
                    PurchaseOrderReplyViewModel.EWayBillNumber = PurchaseOrderReply.EWayBillNumber;
                    PurchaseOrderReplyViewModel.TrackingNumber = PurchaseOrderReply.TrackingNumber;
                    PurchaseOrderReplyViewModel.IsActive = PurchaseOrderReply.IsActive;
                }

                List<PurchaseOrderReplyDetail> lPurchaseOrderReplyDetailslist = new List<PurchaseOrderReplyDetail>();
                lPurchaseOrderReplyDetailslist = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == PurchaseOrderReplyID).ToList();

                List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
                lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID).ToList();

                List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModelList = new List<PurchaseOrderReplyDetailViewModel>();


                foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
                {
                    PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();

                    int sentQuantity = 0;
                    var tempQuantity = new List<PurchaseOrderReplyDetailViewModel>();
                    int sumTempQuantity = 0;
                    long poid = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderDetailID == item.ID).Select(x => x.PurchaseOrderDetailID).FirstOrDefault();
                    if (poid > 0)
                    {
                        sentQuantity = Convert.ToInt32(db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderDetailID == item.ID).Sum(x => x.Quantity == null ? 0 : x.Quantity));
                    }

                    foreach (var inv in lPurchaseOrderReplyDetailslist)
                    {
                        if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
                        {
                            objPOD.PurchaseOrderReplyDetailsID = inv.ID;
                            objPOD.Quantity = inv.Quantity;
                            objPOD.MRP = inv.MRP;
                            objPOD.SaleRate = inv.SaleRate;
                            objPOD.GSTInPer = inv.GSTInPer;
                            objPOD.CGSTAmount = inv.CGSTAmount;
                            objPOD.SGSTAmount = inv.SGSTAmount;
                            objPOD.IGSTAmount = inv.IGSTAmount;
                            objPOD.Amount = Convert.ToDecimal(inv.Amount);
                            objPOD.ProductRemark = inv.Remark;
                        }
                    }

                    RateMatrixExtension objrateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ID == item.RateMatrixExtensionId);
                    RateMatrix objrateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == objrateMatrixExtension.RateMatrixId);
                    double WarehouseSaleRatePerUnit = 0;
                    if (WarehouseLevel == 2) //For DV
                    {
                        WarehouseSaleRatePerUnit = objrateMatrixExtension.DVPurchasePrice;
                    }
                    else if (WarehouseLevel == 3) //For FV
                    {
                        WarehouseSaleRatePerUnit = objrateMatrixExtension.FVPurchasePrice;
                    }

                    objPOD.PurchaseOrderDetailID = item.ID;
                    objPOD.ProductID = item.ProductID;
                    objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                    objPOD.RequiredQuantity = item.Quantity - sentQuantity + objPOD.Quantity;
                    objPOD.BuyRatePerUnit = item.UnitPrice;
                    //objPOD.ProductAmount = item.Amount;                      

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

                    int availableQuantity = 0;
                    decimal WSaleRatePerUnit = Convert.ToDecimal(WarehouseSaleRatePerUnit);

                    //if want to take total quanitity of item for all PurchaseOrder invoices
                    tempQuantity = (from pd in db.PurchaseOrderReplyDetails
                                    join pr in db.PurchaseOrderReply on pd.PurchaseOrderReplyID equals pr.ID
                                    join pod in db.PurchaseOrderDetails on pr.PurchaseOrderID equals pod.PurchaseOrderID
                                    join po in db.PurchaseOrders on pr.PurchaseOrderID equals po.ID
                                    where pd.ProductID == item.ProductID && pd.ProductVarientID == item.ProductVarientID
                                    && pod.RateMatrixId == item.RateMatrixId
                                    && pod.RateMatrixExtensionId == item.RateMatrixExtensionId
                                    && po.SupplierID == purchaseOrder.SupplierID
                                    //&& pd.BuyRatePerUnit == item.UnitPrice 
                                    && item.UnitPrice == WSaleRatePerUnit && pr.IsReplied == false
                                    select new PurchaseOrderReplyDetailViewModel { TempQuantity = pd.Quantity }).ToList();


                    //Take quantity of item from stock
                    var availableQty = (from ws in db.WarehouseStocks
                                        join i in db.Invoices on ws.InvoiceID equals i.ID
                                        join po in db.PurchaseOrders on i.PurchaseOrderID equals po.ID
                                        join pod in db.PurchaseOrderDetails on po.ID equals pod.PurchaseOrderID
                                        where ws.WarehouseID == WarehouseID && ws.ProductID == item.ProductID && ws.ProductVarientID == item.ProductVarientID
                                         && pod.RateMatrixId == item.RateMatrixId
                                        && pod.RateMatrixExtensionId == item.RateMatrixExtensionId
                                        && item.UnitPrice == WSaleRatePerUnit
                                        select new WarehouseStockViewModel { AvailableQuantity = ws.AvailableQuantity }
                                                          ).Select(x => (int)x.AvailableQuantity).DefaultIfEmpty(0).Sum();

                    //var a = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID
                    //   && x.SaleRatePerUnit == item.UnitPrice).Sum(x => (int?)x.AvailableQuantity);
                    //
                    availableQuantity = Convert.ToInt32(availableQty);

                    // Deduct tempQuantity from availableQuantity
                    if (tempQuantity.Count > 0)
                    {
                        sumTempQuantity = tempQuantity.Sum(x => x.TempQuantity);
                        availableQuantity = availableQuantity - sumTempQuantity + objPOD.Quantity;
                    }

                    objPOD.AvailableQuantity = availableQuantity;

                    lPurchaseOrderReplyDetailViewModelList.Add(objPOD);
                }
                if (lPurchaseOrderReplyDetailViewModelList.Count > 0)
                {
                    Session["lPurchaseOrderReplyDetailViewModel"] = lPurchaseOrderReplyDetailViewModelList;
                }

                PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailViewModelList;

                ViewBag.PurchaseOrderID = PurchaseOrderID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(PurchaseOrderReplyViewModel);
        }

        //Edit
        // POST: /PurchaseOrderReply/Edit/5
        [HttpPost]
        public ActionResult Edit(PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel)
        {
            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModel = new List<PurchaseOrderReplyDetailViewModel>();

            //if (Session["lPurchaseOrderDetailsViewModel"] != null && Session["Amount"] != null)
            //{
            lPurchaseOrderReplyDetailViewModel = PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels;
            if (lPurchaseOrderReplyDetailViewModel.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            if (PurchaseOrderReplyViewModel.OrderAmount > 0)
                            {
                                //Insert into PurchaseOrderReply Table                                 

                                var lPurchaseOrderReply = new PurchaseOrderReply()
                                {
                                    ID = PurchaseOrderReplyViewModel.PurchaseOrderReplyID,
                                    TotalAmount = (Convert.ToDecimal(PurchaseOrderReplyViewModel.OrderAmount) + Convert.ToDecimal(PurchaseOrderReplyViewModel.ShippingCharge) + Convert.ToDecimal(PurchaseOrderReplyViewModel.CustomDutyCharge) + Convert.ToDecimal(PurchaseOrderReplyViewModel.OperatingCost) + Convert.ToDecimal(PurchaseOrderReplyViewModel.AdditionalCost)) - Convert.ToDecimal(PurchaseOrderReplyViewModel.TotalDiscountAmount),
                                    DeliveryDateTime = Convert.ToDateTime(PurchaseOrderReplyViewModel.DeliveryDateTime),
                                    //InvoiceCode = PurchaseOrderReplyViewModel.InvoiceCode,
                                    TotalDiscountAmount = Convert.ToDecimal(PurchaseOrderReplyViewModel.TotalDiscountAmount),
                                    OrderAmount = PurchaseOrderReplyViewModel.OrderAmount,
                                    ShippingCharge = Convert.ToDecimal(PurchaseOrderReplyViewModel.ShippingCharge),
                                    CustomDutyCharge = Convert.ToDecimal(PurchaseOrderReplyViewModel.CustomDutyCharge),
                                    OperatingCost = Convert.ToDecimal(PurchaseOrderReplyViewModel.OperatingCost),
                                    AdditionalCost = Convert.ToDecimal(PurchaseOrderReplyViewModel.AdditionalCost),
                                    Remark = PurchaseOrderReplyViewModel.Remark,
                                    DispatchDate = PurchaseOrderReplyViewModel.DispatchDate,
                                    DriverName = PurchaseOrderReplyViewModel.DriverName,
                                    DriverMobileNumber = PurchaseOrderReplyViewModel.DriverMobileNumber,
                                    DriverLicenceNumber = PurchaseOrderReplyViewModel.DriverLicenceNumber,
                                    VehicleNumber = PurchaseOrderReplyViewModel.VehicleNumber,
                                    VehicleType = PurchaseOrderReplyViewModel.VehicleType,
                                    LogisticCompanyName = PurchaseOrderReplyViewModel.LogisticCompanyName,
                                    LogisticCompanyAddress = PurchaseOrderReplyViewModel.LogisticCompanyAddress,
                                    LogisticContactPerson = PurchaseOrderReplyViewModel.LogisticContactPerson,
                                    LogisticContactNumber = PurchaseOrderReplyViewModel.LogisticContactNumber,
                                    EWayBillNumber = PurchaseOrderReplyViewModel.EWayBillNumber,
                                    TrackingNumber = PurchaseOrderReplyViewModel.TrackingNumber,
                                    ModifyDate = DateTime.Now,
                                    ModifyBy = GetPersonalDetailID(),
                                    NetworkIP = CommonFunctions.GetClientIP()
                                };

                                db.PurchaseOrderReply.Attach(lPurchaseOrderReply);
                                db.Entry(lPurchaseOrderReply).Property(x => x.TotalAmount).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.DeliveryDateTime).IsModified = true;
                                //db.Entry(lPurchaseOrderReply).Property(x => x.InvoiceCode).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.TotalDiscountAmount).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.OrderAmount).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.ShippingCharge).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.CustomDutyCharge).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.OperatingCost).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.AdditionalCost).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.Remark).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.DispatchDate).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.DriverName).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.DriverMobileNumber).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.DriverLicenceNumber).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.VehicleNumber).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.VehicleType).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.LogisticCompanyName).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.LogisticCompanyAddress).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.LogisticContactPerson).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.LogisticContactNumber).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.EWayBillNumber).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.TrackingNumber).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.ModifyDate).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.ModifyBy).IsModified = true;
                                db.Entry(lPurchaseOrderReply).Property(x => x.NetworkIP).IsModified = true;
                                db.SaveChanges();


                                //Insert into PurchaseOrderReplyDetail Table
                                PurchaseOrderReplyDetail objPurchaseOrderReplyDetail = new PurchaseOrderReplyDetail();
                                foreach (PurchaseOrderReplyDetailViewModel item in lPurchaseOrderReplyDetailViewModel)
                                {
                                    long id = 0;
                                    //id = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == PurchaseOrderReplyViewModel.PurchaseOrderReplyID && x.PurchaseOrderDetailID == item.PurchaseOrderDetailID).Select(x => x.ID).FirstOrDefault();
                                    id = db.PurchaseOrderReplyDetails.Where(x => x.ID == item.PurchaseOrderReplyDetailsID).Select(x => x.ID).FirstOrDefault();
                                    if (id > 0)
                                    {
                                        if (item.Quantity > 0)
                                        {
                                            var lPurchaseOrderReplyDetail = new PurchaseOrderReplyDetail()
                                            {
                                                ID = id,
                                                PurchaseOrderDetailID = item.PurchaseOrderDetailID,
                                                ProductID = item.ProductID,
                                                ProductVarientID = item.ProductVarientID,
                                                BuyRatePerUnit = item.BuyRatePerUnit,
                                                MRP = item.MRP,
                                                SaleRate = item.SaleRate,
                                                Quantity = Convert.ToInt32(item.Quantity),
                                                Amount = item.Amount,
                                                GSTInPer = item.GSTInPer,
                                                CGSTAmount = Convert.ToDecimal(item.CGSTAmount),
                                                SGSTAmount = Convert.ToDecimal(item.SGSTAmount),
                                                IGSTAmount = Convert.ToDecimal(item.IGSTAmount),
                                                //ExpiryDate = item.ExpiryDate,
                                                Remark = item.ProductRemark
                                            };

                                            db.PurchaseOrderReplyDetails.Attach(lPurchaseOrderReplyDetail);
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.BuyRatePerUnit).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.MRP).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.SaleRate).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.Quantity).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.Amount).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.GSTInPer).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.CGSTAmount).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.SGSTAmount).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.IGSTAmount).IsModified = true;
                                            //db.Entry(lPurchaseOrderReplyDetail).Property(x => x.ExpiryDate).IsModified = true;
                                            db.Entry(lPurchaseOrderReplyDetail).Property(x => x.Remark).IsModified = true;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            var delobj = db.PurchaseOrderReplyDetails.Where(p => p.ID == id).SingleOrDefault();
                                            db.PurchaseOrderReplyDetails.Remove(delobj);
                                            db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (item.Quantity > 0)
                                        {
                                            objPurchaseOrderReplyDetail.PurchaseOrderReplyID = PurchaseOrderReplyViewModel.PurchaseOrderReplyID;
                                            objPurchaseOrderReplyDetail.PurchaseOrderDetailID = item.PurchaseOrderReplyDetailsID;
                                            objPurchaseOrderReplyDetail.ProductID = item.ProductID;
                                            objPurchaseOrderReplyDetail.ProductVarientID = item.ProductVarientID;
                                            objPurchaseOrderReplyDetail.IsExtraItem = false;
                                            objPurchaseOrderReplyDetail.BuyRatePerUnit = item.BuyRatePerUnit;
                                            objPurchaseOrderReplyDetail.MRP = item.MRP;
                                            objPurchaseOrderReplyDetail.SaleRate = item.SaleRate;
                                            objPurchaseOrderReplyDetail.Quantity = Convert.ToInt32(item.Quantity);
                                            objPurchaseOrderReplyDetail.Amount = Convert.ToDecimal(item.Amount);
                                            objPurchaseOrderReplyDetail.Amount = Convert.ToDecimal(item.GSTInPer);
                                            objPurchaseOrderReplyDetail.CGSTAmount = Convert.ToDecimal(item.CGSTAmount);
                                            objPurchaseOrderReplyDetail.SGSTAmount = Convert.ToDecimal(item.SGSTAmount);
                                            objPurchaseOrderReplyDetail.IGSTAmount = Convert.ToDecimal(item.IGSTAmount);
                                            //objPurchaseOrderReplyDetail.IGSTAmount = Convert.ToDecimal(item.ExpiryDate);
                                            objPurchaseOrderReplyDetail.Remark = item.ProductRemark;
                                            objPurchaseOrderReplyDetail.IsActive = true;
                                            db.PurchaseOrderReplyDetails.Add(objPurchaseOrderReplyDetail);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                tscope.Complete();
                                Session["Success"] = "Invoice updated Successfully."; //yashaswi 31/3/2018
                            }
                            else
                            {
                                return RedirectToAction("Edit", new { PurchaseOrderReplyID = PurchaseOrderReplyViewModel.PurchaseOrderReplyID });
                                ////Delete from Invoice items
                                //var delInvoiceDetail = db.InvoiceDetails.Where(p => p.InvoiceID == Invoice.InvoiceID).SingleOrDefault();
                                //db.InvoiceDetails.Remove(delInvoiceDetail);
                                //db.SaveChanges();

                                ////Delete Invoice
                                //var delInvoice = db.Invoices.Where(p => p.ID == Invoice.InvoiceID).SingleOrDefault();
                                //db.Invoices.Remove(delInvoice);
                                //db.SaveChanges();
                            }
                        }
                        catch (Exception)
                        {
                            Transaction.Current.Rollback();
                            tscope.Dispose();
                            return View(PurchaseOrderReplyViewModel);
                        }
                    }

                    return RedirectToAction("CreateInvoice", new { PurchaseOrderID = PurchaseOrderReplyViewModel.PurchaseOrderID });
                }
            }
            //}
            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleSuppliers = db.Suppliers;
            return View(PurchaseOrderReplyViewModel);
        }


        //
        // GET: /PurchaseOrderReply/Create
        public ActionResult InvoiceList()
        {
            List<PurchaseOrderReplyViewModel> lPurchaseOrderReplyViewModel = new List<PurchaseOrderReplyViewModel>();
            try
            {
                //Check session
                if (Session["USER_NAME"] != null && Session["WarehouseID"] != null)
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

                var SupplierID = db.Suppliers.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.ID).FirstOrDefault();

                if (SupplierID != null && Convert.ToInt64(SupplierID) > 0)
                {
                    long supplierId = Convert.ToInt64(SupplierID);
                    lPurchaseOrderReplyViewModel = (from o in db.PurchaseOrderReply
                                                    join po in db.PurchaseOrders on o.PurchaseOrderID equals po.ID
                                                    join w in db.Warehouses on po.WarehouseID equals w.ID
                                                    where po.SupplierID == supplierId
                                                    select new PurchaseOrderReplyViewModel
                                                    {
                                                        PurchaseOrderCode = po.PurchaseOrderCode,
                                                        InvoiceCode = o.InvoiceCode,
                                                        PurchaseOrderReplyID = o.ID,
                                                        PurchaseOrderID = o.PurchaseOrderID,
                                                        OrderAmount = o.OrderAmount,
                                                        ReplyDate = o.ReplyDate,
                                                        TotalDiscountAmount = o.TotalDiscountAmount,
                                                        ShippingCharge = o.ShippingCharge,
                                                        CustomDutyCharge = o.CustomDutyCharge,
                                                        OperatingCost = o.OperatingCost,
                                                        AdditionalCost = o.AdditionalCost,
                                                        TotalAmount = o.TotalAmount,
                                                        TotalItems = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == o.ID).Select(x => x.ID).Count(),
                                                        IsReplied = o.IsReplied,
                                                        CreateDate = o.CreateDate,
                                                        WarehouseName = w.Name
                                                    }).OrderByDescending(o => o.ReplyDate).OrderBy(o => o.IsReplied).ToList();
                }
            }
            catch (Exception ex)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("InvoiceList", lPurchaseOrderReplyViewModel);
        }

        //     

        //
        // POST: /PurchaseOrderReply/Delete/5
        //[HttpPost]
        public ActionResult Delete(long PurchaseOrderReplyID, long PurchaseOrderiD)
        {
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    //    //Delete from PurchaseOrderReplyDetails items
                    var delPurchaseOrderReplyDetail = db.PurchaseOrderReplyDetails.Where(p => p.PurchaseOrderReplyID == PurchaseOrderReplyID).FirstOrDefault();
                    db.PurchaseOrderReplyDetails.Remove(delPurchaseOrderReplyDetail);
                    db.SaveChanges();
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
                    //Delete PurchaseOrderReply
                    var delPurchaseOrderReply = db.PurchaseOrderReply.Where(p => p.ID == PurchaseOrderReplyID).SingleOrDefault();
                    db.PurchaseOrderReply.Remove(delPurchaseOrderReply);
                    db.SaveChanges();

                    tscope.Complete();
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                }
            }
            return RedirectToAction("CreateInvoice", new { PurchaseOrderID = PurchaseOrderiD });
        }


        //
        // GET: /PurchaseOrderReply/PrintOrder/5

        //[HttpPost]
        public ActionResult GetId(long id)
        {
            Session["id"] = id;
            return RedirectToAction("Print_Invoice");
        }

        //Added by Priti on 3-11-2018 for New Formate of Invoice
        public ActionResult GetId1(long id)
        {
            Session["id"] = id;
            return RedirectToAction("Print1");
        }

        public ViewResult Print1()//long id
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
                                               Entity = w.Entity, //by Priti
                                               WarehousePanNo = w.PAN   //by Priti

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


            //}
            //catch
            //{

            //}


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






        /// <summary>
        /// New Format  of Invoice  by Priti
        /// </summary>
        /// <returns></returns>


        //Added by Priti on 3-11-2018 for New Formate of Invoice
        public ActionResult GetId2(long id)
        {
            Session["id"] = id;
            return RedirectToAction("Print2");
        }

        public ViewResult Print2()//long id
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
                                               Entity = w.Entity,  //by priti
                                               WarehousePanNo = w.PAN  //by priti

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


            //}
            //catch
            //{

            //}


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























        //old Format of Invoice with Return quantity bill

        public ViewResult Print_Invoice()//long id
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

            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailsViewModelList = new List<PurchaseOrderReplyDetailViewModel>();

            foreach (PurchaseOrderReplyDetail item in lPurchaseOrderReplyDetailslist)
            {
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


        //public ViewResult Print()//long id
        //{
        //    long id = long.Parse(Session["id"].ToString());
        //    PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();

        //    int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
        //    ViewBag.PossibleSuppliers = db.Suppliers.Where(x => x.WarehouseID != WarehouseID).ToList();

        //    //Session["lPurchaseOrderDetailsViewModel"] = null;

        //    PurchaseOrderReplyViewModel = (from po in db.PurchaseOrderReply
        //                                   join p in db.PurchaseOrders on po.PurchaseOrderID equals p.ID
        //                              join s in db.Suppliers on p.SupplierID equals s.ID
        //                              join w in db.Warehouses on p.WarehouseID equals w.ID
        //                              join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
        //                              where po.ID == id

        //                                   select new PurchaseOrderReplyViewModel
        //                              {
        //                                  PurchaseOrderID = po.ID,
        //                                  PurchaseOrderCode = p.PurchaseOrderCode,
        //                                  InvoiceCode = po.InvoiceCode,
        //                                  WarehouseName = w.Name,
        //                                  SupplierName = s.Name,
        //                                  SupplierGSTNumber = s.GSTNumber,
        //                                  SupplierFax = s.FAX,
        //                                  SupplierEmail = s.Email,
        //                                  CreateDate = po.CreateDate,
        //                                  DeliveryDateTime = po.DeliveryDateTime,
        //                                  OrderAmount = po.OrderAmount == null ? 0 : po.OrderAmount,
        //                                  GSTAmount = po.GSTAmount == null ? 0 : po.GSTAmount,
        //                                  ShippingCharge = po.ShippingCharge == null ? 0 : po.ShippingCharge,
        //                                  AdditionalCost = po.AdditionalCost == null ? 0 : po.AdditionalCost,
        //                                  CustomDutyCharge = po.CustomDutyCharge == null ? 0 : po.CustomDutyCharge,
        //                                  OperatingCost = po.OperatingCost == null ? 0 : po.OperatingCost,
        //                                  TotalDiscountAmount = po.TotalDiscountAmount == null ? 0 : po.TotalDiscountAmount,
        //                                  TotalAmount = po.TotalAmount,
        //                                  Remark = po.Remark,
        //                                  IsActive = po.IsActive,
        //                                  SupplierCode = s.SupplierCode,
        //                                  SupplierContactPerson = s.ContactPerson,
        //                                  SupplierAddress = s.Address,
        //                                  SupplierMobile = s.Mobile,
        //                                  WarehouseContactPerson = b.ContactPerson,
        //                                  WarehouseAddress = b.Address,
        //                                  WarehouseMobile = b.Mobile,
        //                                  WarehosueEmail = b.Email,
        //                                  ExpetedDeliveryDate = p.ExpetedDeliveryDate

        //                              }).FirstOrDefault();



        //    List<PurchaseOrderReplyDetail> lPurchaseOrderReplyDetailslist = new List<PurchaseOrderReplyDetail>();
        //    lPurchaseOrderReplyDetailslist = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == id).ToList();


        //    List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailsViewModelList = new List<PurchaseOrderReplyDetailViewModel>();

        //    foreach (PurchaseOrderReplyDetail item in lPurchaseOrderReplyDetailslist)
        //    {
        //        PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();
        //        objPOD.Quantity = item.Quantity;
        //        objPOD.ProductNickname = item.ProductNickname;
        //        objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
        //        objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
        //        objPOD.ProductAmount = item.Quantity * item.BuyRatePerUnit;
        //        objPOD.GSTInPer = item.GSTInPer;
        //        objPOD.CGSTAmount = item.Quantity * item.CGSTAmount;
        //        objPOD.SGSTAmount = item.Quantity * item.SGSTAmount;
        //        var itemName = (from p in db.Products
        //                        join v in db.ProductVarients on p.ID equals v.ProductID
        //                        join s in db.Sizes on v.SizeID equals s.ID
        //                        where v.ID == item.ProductVarientID
        //                        select new PurchaseOrderReplyDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();

        //        foreach (var i in itemName)
        //        {
        //            objPOD.ItemName = i.ItemName.ToString();
        //            objPOD.HSNCode = Convert.ToString(i.HSNCode);
        //        }

        //        lPurchaseOrderReplyDetailsViewModelList.Add(objPOD);
        //    }

        //    if (lPurchaseOrderReplyDetailsViewModelList.Count > 0)
        //    {
        //        PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailsViewModelList;
        //        //Session["lPurchaseOrderReplyDetailsViewModel"] = lPurchaseOrderReplyDetailsViewModelList;
        //    }


        //    ViewBag.PurchaseOrderID = id;
        //    return View(PurchaseOrderReplyViewModel);
        //}
        public ViewResult Print()//long id
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
                                               WarehouseContactPerson = b.ContactPerson,
                                               WarehouseAddress = b.Address,
                                               WarehouseMobile = b.Mobile,
                                               WarehosueEmail = b.Email,
                                               ExpetedDeliveryDate = p.ExpetedDeliveryDate

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



            //}
            //catch
            //{

            //}


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

                    //objPOD.CGSTAmount = item.Quantity * item.CGSTAmount;
                    objPOD.SGSTAmount = item.Quantity * item.CGSTAmount;



                    ///Added by Priti on 16-10-2018
                    //decimal? TaxableValue1 = (objPOD.ProductAmount * 100) / (item.GSTInPer + 100);
                    //objPOD.TaxableValue = (objPOD.ProductAmount * 100) / (item.GSTInPer + 100);
                    //decimal? GSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
                    //decimal? GSTAmount1 = GSTAmount / 2;

                    //            decimal? CGSTAmount1 = objPOD.ProductAmount - objPOD.TaxableValue;

                    //     decimal CGSTAmount2 = Convert.ToDecimal(CGSTAmount1 / 2);
                    //     //objPOD.CGSTAmount = Math.Round(CGSTAmount2, 2);

                    objPOD.TaxableValue = (objPOD.ProductAmount * 100) / ((item.GSTInPer ?? 0) + 100);
                    objPOD.TaxableValue = Math.Round(objPOD.TaxableValue, 2);
                    decimal GSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
                    objPOD.SGSTAmount = GSTAmount / 2;
                    objPOD.CGSTAmount = GSTAmount / 2;

                    //decimal? SGSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
                    //decimal SGSTAMT = Convert.ToDecimal(SGSTAmount / 2);

                    //objPOD.SGSTAmount = Math.Truncate(SGSTAMT * 100) / 100;

                    //decimal? CGSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;
                    //decimal CGSTAMT = Convert.ToDecimal(SGSTAmount / 2);

                    //objPOD.CGSTAmount = Math.Truncate(CGSTAMT * 100) / 100;

                    //decimal SGSTAmount2 = Convert.ToDecimal(CGSTAmount1 / 2);
                    //objPOD.SGSTAmount = Math.Round(CGSTAmount2, 2);

                    //end  by Priti on 16-10-2018
                    //objPOD.SGSTAmount = item.Quantity * item.SGSTAmount;
                    var itemName = (from p in db.Products
                                    join v in db.ProductVarients on p.ID equals v.ProductID
                                    join s in db.Sizes on v.SizeID equals s.ID
                                    where v.ID == item.ProductVarientID
                                    select new PurchaseOrderReplyDetailViewModel { ItemName = p.Name + " (" + s.Name + ")", HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

                    foreach (var i in itemName)
                    {
                        objPOD.ItemName = i.ItemName.ToString();
                        objPOD.HSNCode = Convert.ToString(i.HSNCode);
                    }

                    lPurchaseOrderReplyDetailsViewModelList.Add(objPOD);
                }
            }

            if (lPurchaseOrderReplyDetailsViewModelList.Count > 0)
            {
                PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailsViewModelList;
                //Session["lPurchaseOrderReplyDetailsViewModel"] = lPurchaseOrderReplyDetailsViewModelList;
            }


            ViewBag.PurchaseOrderID = id;
            //Yashaswi 2-8-2018 For GST

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


        public ActionResult Approve(long PurchaseOrderReplyID)
        {
            PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();
            long PurchaseOrderID = db.PurchaseOrderReply.Where(x => x.ID == PurchaseOrderReplyID).Select(x => x.PurchaseOrderID).FirstOrDefault();
            ViewBag.PurchaseOrderID = PurchaseOrderID;
            long warehouseID = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
            //ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == warehouseID);

            //Check session
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
            else
            {
                return RedirectToAction("Index", "Login");
            }

            PurchaseOrderReplyViewModel = (from po in db.PurchaseOrderReply
                                           join p in db.PurchaseOrders on po.PurchaseOrderID equals p.ID
                                           join s in db.Suppliers on p.SupplierID equals s.ID
                                           join w in db.Warehouses on p.WarehouseID equals w.ID
                                           join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                           where po.ID == PurchaseOrderReplyID

                                           select new PurchaseOrderReplyViewModel
                                           {
                                               PurchaseOrderID = po.ID,
                                               PurchaseOrderCode = p.PurchaseOrderCode,
                                               InvoiceCode = po.InvoiceCode,
                                               WarehouseName = w.Name,
                                               SupplierName = s.Name,
                                               SupplierGSTNumber = s.GSTNumber,
                                               SupplierFax = s.FAX,
                                               SupplierEmail = s.Email,
                                               CreateDate = po.CreateDate,
                                               DeliveryDateTime = po.DeliveryDateTime,
                                               OrderAmount = po.OrderAmount == null ? 0 : po.OrderAmount,
                                               GSTAmount = po.GSTAmount == null ? 0 : po.GSTAmount,
                                               TotalDiscountAmount = po.TotalDiscountAmount == null ? 0 : po.TotalDiscountAmount,
                                               ShippingCharge = po.ShippingCharge == null ? 0 : po.ShippingCharge,
                                               AdditionalCost = po.AdditionalCost == null ? 0 : po.AdditionalCost,
                                               OperatingCost = po.OperatingCost == null ? 0 : po.OperatingCost,
                                               CustomDutyCharge = po.CustomDutyCharge == null ? 0 : po.CustomDutyCharge,
                                               TotalAmount = po.TotalAmount,
                                               Remark = po.Remark,
                                               DispatchDate = po.DispatchDate,
                                               DriverName = po.DriverName,
                                               DriverMobileNumber = po.DriverMobileNumber,
                                               DriverLicenceNumber = po.DriverLicenceNumber,
                                               VehicleNumber = po.VehicleNumber,
                                               VehicleType = po.VehicleType,
                                               LogisticCompanyName = po.LogisticCompanyName,
                                               LogisticCompanyAddress = po.LogisticCompanyAddress,
                                               LogisticContactPerson = po.LogisticContactPerson,
                                               LogisticContactNumber = po.LogisticContactNumber,
                                               EWayBillNumber = po.EWayBillNumber,
                                               TrackingNumber = po.TrackingNumber,
                                               IsActive = po.IsActive,
                                               SupplierCode = s.SupplierCode,
                                               SupplierContactPerson = s.ContactPerson,
                                               SupplierAddress = s.Address,
                                               SupplierMobile = s.Mobile,
                                               WarehouseContactPerson = b.ContactPerson,
                                               WarehouseAddress = b.Address,
                                               WarehouseMobile = b.Mobile,
                                               WarehosueEmail = b.Email,
                                               ExpetedDeliveryDate = p.ExpetedDeliveryDate,
                                               DVId = p.DVId.Value,
                                               FVId = p.FVId.Value
                                           }).FirstOrDefault();
            if (PurchaseOrderReplyViewModel.DVId != null && PurchaseOrderReplyViewModel.FVId != null)
            {
                ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == PurchaseOrderReplyViewModel.DVId).Name + " under FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == PurchaseOrderReplyViewModel.FVId).Name;
            }
            List<PurchaseOrderReplyDetail> lPurchaseOrderReplyDetailslist = new List<PurchaseOrderReplyDetail>();
            lPurchaseOrderReplyDetailslist = db.PurchaseOrderReplyDetails.Where(x => x.PurchaseOrderReplyID == PurchaseOrderReplyID).ToList();


            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModelList = new List<PurchaseOrderReplyDetailViewModel>();


            foreach (var item in lPurchaseOrderReplyDetailslist)
            {
                PurchaseOrderReplyDetailViewModel objPOD = new PurchaseOrderReplyDetailViewModel();
                //if (inv.ProductID == item.ProductID && inv.ProductVarientID == item.ProductVarientID)
                //{
                objPOD.PurchaseOrderReplyDetailsID = item.ID;
                objPOD.Quantity = item.Quantity;
                objPOD.MRP = item.MRP;
                objPOD.SaleRate = item.SaleRate;
                objPOD.CGSTAmount = item.Quantity * item.CGSTAmount;
                objPOD.SGSTAmount = item.Quantity * item.SGSTAmount;
                objPOD.IGSTAmount = item.Quantity * item.IGSTAmount;
                objPOD.Amount = Convert.ToDecimal(item.Amount);
                objPOD.ProductRemark = item.Remark;


                objPOD.PurchaseOrderDetailID = item.PurchaseOrderDetailID;
                objPOD.PurchaseOrderReplyDetailsID = item.ID;
                objPOD.ProductID = item.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                //objPOD.RequiredQuantity = item.Quantity - sentQuantity + objPOD.Quantity;
                objPOD.BuyRatePerUnit = item.BuyRatePerUnit;
                //objPOD.ProductAmount = item.Amount;                      

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

                lPurchaseOrderReplyDetailViewModelList.Add(objPOD);
            }
            if (lPurchaseOrderReplyDetailViewModelList.Count > 0)
            {
                PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels = lPurchaseOrderReplyDetailViewModelList;
            }

            return View(PurchaseOrderReplyViewModel);
        }


        //
        // POST: /PurchaseOrderReply/Approve/5
        [HttpPost]
        public ActionResult Approve(PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel)
        {
            List<PurchaseOrderReplyDetailViewModel> lPurchaseOrderReplyDetailViewModel = new List<PurchaseOrderReplyDetailViewModel>();

            //Check session
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
            else
            {
                return RedirectToAction("Index", "Login");
            }

            lPurchaseOrderReplyDetailViewModel = PurchaseOrderReplyViewModel.lPurchaseOrderReplyDetailViewModels;
            if (lPurchaseOrderReplyDetailViewModel.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope tscope = new TransactionScope())
                    {
                        try
                        {
                            //Update PurchaseOrderReply Table                                 

                            var lPurchaseOrderReply = new PurchaseOrderReply()
                            {
                                ID = PurchaseOrderReplyViewModel.PurchaseOrderReplyID,
                                ReplyDate = System.DateTime.UtcNow,
                                IsReplied = true,
                                RepliedBy = GetPersonalDetailID()
                            };

                            db.PurchaseOrderReply.Attach(lPurchaseOrderReply);
                            db.Entry(lPurchaseOrderReply).Property(x => x.ReplyDate).IsModified = true;
                            db.Entry(lPurchaseOrderReply).Property(x => x.IsReplied).IsModified = true;
                            db.Entry(lPurchaseOrderReply).Property(x => x.RepliedBy).IsModified = true;
                            db.SaveChanges();

                            //Yashaswi 21/04/2018
                            Warehouse obj_Warehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseID);
                            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                            //

                            //Insert into PurchaseOrderReplyDetail Table
                            PurchaseOrderReplyDetail objPurchaseOrderReplyDetail = new PurchaseOrderReplyDetail();
                            WarehouseStockDeliveryDetail objWarehouseStockDeliveryDetail = new WarehouseStockDeliveryDetail();
                            foreach (PurchaseOrderReplyDetailViewModel item in lPurchaseOrderReplyDetailViewModel)
                            {
                                int availableQuantity = 0;
                                long? RateMatrixId = 0;
                                long? RateMatrixExtensionId = 0;
                                PurchaseOrderDetail objPurchaseOrderDetail = db.PurchaseOrderDetails.Where(x => x.ID == item.PurchaseOrderDetailID).FirstOrDefault();
                                RateMatrixId = objPurchaseOrderDetail.RateMatrixId;
                                RateMatrixExtensionId = objPurchaseOrderDetail.RateMatrixExtensionId;

                                //Take quantity of item from stock
                                //var WarehouseStock = db.WarehouseStocks.Where(x => x.WarehouseID == WarehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID
                                //   && x.SaleRatePerUnit == item.BuyRatePerUnit && x.AvailableQuantity > 0).ToList().OrderBy(x => x.ID);

                                long PurchaseOrderID = db.PurchaseOrderDetails.Where(x => x.ID == item.PurchaseOrderDetailID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                                long WarehouseIDC = db.PurchaseOrders.Where(x => x.ID == PurchaseOrderID).Select(x => x.WarehouseID).FirstOrDefault();
                                int WarehouseLevel = 0;
                                Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseIDC);
                                if (obj_warehouse != null)
                                {
                                    if (obj_warehouse.IsFulfillmentCenter)
                                    {
                                        WarehouseLevel = 3; //For FV
                                    }
                                    else
                                    {
                                        WarehouseLevel = 2; //For DV
                                    }
                                    //}
                                }

                                RateMatrixExtension objrateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ID == RateMatrixExtensionId);
                                double WarehouseSaleRatePerUnit = 0;
                                if (WarehouseLevel == 2) //For DV
                                {
                                    WarehouseSaleRatePerUnit = objrateMatrixExtension.DVPurchasePrice;
                                }
                                else if (WarehouseLevel == 3) //For FV
                                {
                                    WarehouseSaleRatePerUnit = objrateMatrixExtension.FVPurchasePrice;
                                }

                                decimal WSaleRatePerUnit = Convert.ToDecimal(WarehouseSaleRatePerUnit);

                                //Changes made by Zubair on 23-04-2018
                                var WarehouseStock = (from ws in db.WarehouseStocks
                                                      join i in db.Invoices on ws.InvoiceID equals i.ID
                                                      join po in db.PurchaseOrders on i.PurchaseOrderID equals po.ID
                                                      join pod in db.PurchaseOrderDetails on po.ID equals pod.PurchaseOrderID
                                                      where ws.WarehouseID == WarehouseID && ws.ProductID == item.ProductID && ws.ProductVarientID == item.ProductVarientID
                                                      && pod.RateMatrixId == RateMatrixId
                                                        && pod.RateMatrixExtensionId == RateMatrixExtensionId
                                                      && item.BuyRatePerUnit == WSaleRatePerUnit
                                                      select ws
                                                 ).ToList().OrderBy(x => x.ID);

                                availableQuantity = Convert.ToInt32(WarehouseStock.Sum(x => x.AvailableQuantity));

                                if (item.Quantity <= availableQuantity)
                                {
                                    int RemainingQuantity = item.Quantity;
                                    foreach (WarehouseStock batch in WarehouseStock)
                                    {
                                        if (RemainingQuantity > 0)
                                        {
                                            int BatchAvailableQuantity = batch.AvailableQuantity;

                                            if (BatchAvailableQuantity <= RemainingQuantity)
                                            {
                                                objWarehouseStockDeliveryDetail.PurchaseOrderReplyDetailID = item.PurchaseOrderReplyDetailsID;
                                                objWarehouseStockDeliveryDetail.WarehouseStockID = batch.ID;
                                                objWarehouseStockDeliveryDetail.Quantity = BatchAvailableQuantity;

                                                RemainingQuantity = item.Quantity - BatchAvailableQuantity;
                                            }
                                            else
                                            {
                                                objWarehouseStockDeliveryDetail.PurchaseOrderReplyDetailID = item.PurchaseOrderReplyDetailsID;
                                                objWarehouseStockDeliveryDetail.WarehouseStockID = batch.ID;
                                                objWarehouseStockDeliveryDetail.Quantity = RemainingQuantity;

                                                RemainingQuantity = 0;
                                            }


                                            db.WarehouseStockDeliveryDetails.Add(objWarehouseStockDeliveryDetail);
                                            db.SaveChanges();


                                            //Deduct batch available quantity from WarehouseStock Table   

                                            WarehouseStock objStock = db.WarehouseStocks.First(x => x.ID == batch.ID);

                                            objStock.AvailableQuantity = BatchAvailableQuantity - objWarehouseStockDeliveryDetail.Quantity;
                                            objStock.ModifyDate = System.DateTime.UtcNow;
                                            objStock.ModifyBy = GetPersonalDetailID();
                                            db.Entry(objStock).State = EntityState.Modified;


                                            //Insert into WarehouseStockLog table
                                            CommonController obj_CommonController = new CommonController();
                                            obj_CommonController.WarehouseStockLog(objWarehouseStockDeliveryDetail.WarehouseStockID, (int)Inventory.Common.Constants.Warehouse_Stock_Log_Status.OUTWARD, GetPersonalDetailID(), objWarehouseStockDeliveryDetail.Quantity);


                                            //Deduct total available quantity from WarehouseReorderLevel Table    
                                            WarehouseReorderLevel objWarehouseReorderLevel = db.WarehouseReorderLevels.First(x => x.WarehouseID == WarehouseID && x.ProductID == item.ProductID && x.ProductVarientID == item.ProductVarientID);
                                            int totalQty = objWarehouseReorderLevel.AvailableQuantity;
                                            objWarehouseReorderLevel.AvailableQuantity = totalQty - objWarehouseStockDeliveryDetail.Quantity;
                                            db.Entry(objWarehouseReorderLevel).State = EntityState.Modified;
                                            db.SaveChanges();
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
                            return View(PurchaseOrderReplyViewModel);
                        }
                    }

                    return RedirectToAction("InvoiceList");
                }
            }
            //}
            ViewBag.PossibleWarehouses = db.Warehouses;
            ViewBag.PossibleSuppliers = db.Suppliers;
            return View(PurchaseOrderReplyViewModel);
        }


        public ActionResult SendMailForInvoice(long ID)
        {
            try
            {
                //PurchaseOrderReply objPurchaseOrderReply = db.PurchaseOrderReply.SingleOrDefault(p => p.ID == ID);
                //if (objPurchaseOrderReply != null)
                //{
                PurchaseOrderReplyViewModel PurchaseOrderReplyViewModel = new PurchaseOrderReplyViewModel();
                PurchaseOrderReplyViewModel = (from po in db.PurchaseOrderReply
                                               join p in db.PurchaseOrders on po.PurchaseOrderID equals p.ID
                                               join s in db.Suppliers on p.SupplierID equals s.ID
                                               join w in db.Warehouses on p.WarehouseID equals w.ID
                                               join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                               where po.ID == ID

                                               select new PurchaseOrderReplyViewModel
                                               {
                                                   PurchaseOrderID = po.ID,
                                                   PurchaseOrderCode = p.PurchaseOrderCode,
                                                   InvoiceCode = po.InvoiceCode,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   SupplierGSTNumber = s.GSTNumber,
                                                   SupplierFax = s.FAX,
                                                   SupplierEmail = s.Email,
                                                   CreateDate = po.CreateDate,
                                                   DeliveryDateTime = po.DeliveryDateTime,
                                                   OrderAmount = po.OrderAmount == null ? 0 : po.OrderAmount,
                                                   GSTAmount = po.GSTAmount == null ? 0 : po.GSTAmount,
                                                   ShippingCharge = po.ShippingCharge == null ? 0 : po.ShippingCharge,
                                                   AdditionalCost = po.AdditionalCost == null ? 0 : po.AdditionalCost,
                                                   TotalAmount = po.TotalAmount,
                                                   Remark = po.Remark,
                                                   IsActive = po.IsActive,
                                                   SupplierCode = s.SupplierCode,
                                                   SupplierContactPerson = s.ContactPerson,
                                                   SupplierAddress = s.Address,
                                                   SupplierMobile = s.Mobile,
                                                   WarehouseContactPerson = b.ContactPerson,
                                                   WarehouseAddress = b.Address,
                                                   WarehouseMobile = b.Mobile,
                                                   WarehosueEmail = b.Email,
                                                   ExpetedDeliveryDate = p.ExpetedDeliveryDate,
                                                   ReplyDate = po.ReplyDate,
                                                   DispatchDate = po.DispatchDate,
                                                   DriverName = po.DriverName,
                                                   DriverMobileNumber = po.DriverMobileNumber,
                                                   DriverLicenceNumber = po.DriverLicenceNumber,
                                                   VehicleNumber = po.VehicleNumber,
                                                   VehicleType = po.VehicleType,
                                                   LogisticCompanyName = po.LogisticCompanyName,
                                                   LogisticCompanyAddress = po.LogisticCompanyAddress,
                                                   LogisticContactPerson = po.LogisticContactPerson,
                                                   LogisticContactNumber = po.LogisticContactNumber,
                                                   EWayBillNumber = po.EWayBillNumber,
                                                   TrackingNumber = po.TrackingNumber
                                               }).FirstOrDefault();


                var URI = @"" + WebConfigurationManager.AppSettings["INVENTORY_ROOT"] + "Invoices/GetId?id=" + ID + "";

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();
                ///<!--QUOTATION_NO-->,<!--SUPPLIER_NAME-->,<!--QUOTATION_DATE-->,<!--QUOTATION_REPLY_DATE-->,
                ///<!--WAREHOUSE_NAME-->,<!--URL-->

                dictEmailValues.Add("<!--Invoice_No-->", PurchaseOrderReplyViewModel.InvoiceCode);
                dictEmailValues.Add("<!--PurchaseOrder_No-->", PurchaseOrderReplyViewModel.PurchaseOrderCode);
                dictEmailValues.Add("<!--SUPPLIER_NAME-->", PurchaseOrderReplyViewModel.SupplierName);
                dictEmailValues.Add("<!--SUPPLIER_EMAIL-->", PurchaseOrderReplyViewModel.SupplierEmail);
                dictEmailValues.Add("<!--WAREHOUSE_NAME-->", PurchaseOrderReplyViewModel.WarehouseName);
                dictEmailValues.Add("<!--INVOICE_DATE-->", Convert.ToDateTime(PurchaseOrderReplyViewModel.ReplyDate).ToShortDateString());
                dictEmailValues.Add("<!--DISPATCH_DATE-->", Convert.ToDateTime(PurchaseOrderReplyViewModel.DispatchDate).ToShortDateString());
                dictEmailValues.Add("<!--DELIVERY_DATE-->", Convert.ToDateTime(PurchaseOrderReplyViewModel.DeliveryDateTime).ToShortDateString());
                dictEmailValues.Add("<!--DriverName-->", PurchaseOrderReplyViewModel.DriverName);
                dictEmailValues.Add("<!--DriverMobileNumber-->", PurchaseOrderReplyViewModel.DriverMobileNumber);
                dictEmailValues.Add("<!--DriverLicenceNumber-->", PurchaseOrderReplyViewModel.DriverLicenceNumber);
                dictEmailValues.Add("<!--VehicleNumber-->", PurchaseOrderReplyViewModel.VehicleNumber);
                dictEmailValues.Add("<!--VehicleType-->", PurchaseOrderReplyViewModel.VehicleType);
                dictEmailValues.Add("<!--LogisticCompanyName-->", PurchaseOrderReplyViewModel.LogisticCompanyName);
                dictEmailValues.Add("<!--LogisticCompanyAddress-->", PurchaseOrderReplyViewModel.LogisticCompanyAddress);
                dictEmailValues.Add("<!--LogisticContactPerson-->", PurchaseOrderReplyViewModel.LogisticContactPerson);
                dictEmailValues.Add("<!--LogisticContactNumber-->", PurchaseOrderReplyViewModel.LogisticContactNumber);
                dictEmailValues.Add("<!--EWayBillNumber-->", PurchaseOrderReplyViewModel.EWayBillNumber);
                dictEmailValues.Add("<!--TrackingNumber-->", PurchaseOrderReplyViewModel.TrackingNumber);
                dictEmailValues.Add("<!--URL-->", URI);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = PurchaseOrderReplyViewModel.WarehosueEmail;

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.SEND_INVOICE, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                Session["Success"] = "Mail Sent Successfully.";
                //}
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PurchaseOrderReplyController][M:SendMailForInvoice]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                Session["Error"] = "Mail not sent! Please try again.";
            }

            return RedirectToAction("InvoiceList");
        }

    }

}
