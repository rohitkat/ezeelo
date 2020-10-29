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
    public class ForLoopClass //----------------use this class for loop purpose in below functions--------------
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class PurchaseOrdersController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();

        //
        // GET: /PurchaseOrders/

        public ActionResult Index()
        {
            PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
            List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
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
                                           //join pd in db.PurchaseOrderDetails on o.ID equals pd.PurchaseOrderID
                                           where w.ID == WarehouseID
                                           select new PurchaseOrderViewModel
                                           {
                                               PurchaseOrderID = o.ID,
                                               WarehouseName = w.Name,
                                               SupplierName = s.Name,
                                               PurchaseOrderCode = o.PurchaseOrderCode,
                                               IsSent = o.IsSent,
                                               IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                               TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                               OrderedQuantity = db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == o.ID).Select(p => p.Quantity).DefaultIfEmpty(0).Sum(),
                                               ReceivedQuantity = (from i in db.InvoiceDetails
                                                                   join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                                   join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                                   where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                                   select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).DefaultIfEmpty(0).Sum(),
                                               OrderDate = o.OrderDate,
                                               Amount = o.Amount == null ? 0 : o.Amount,
                                               ExpetedDeliveryDate = o.ExpetedDeliveryDate
                                           }).OrderByDescending(x => x.PurchaseOrderID).ToList(); //.OrderByDescending(o => o.OrderDate)
            }
            else
            {
                lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                           join w in db.Warehouses on o.WarehouseID equals w.ID
                                           join s in db.Suppliers on o.SupplierID equals s.ID
                                           select new PurchaseOrderViewModel
                                           {
                                               PurchaseOrderID = o.ID,
                                               WarehouseName = w.Name,
                                               SupplierName = s.Name,
                                               PurchaseOrderCode = o.PurchaseOrderCode,
                                               IsSent = o.IsSent,
                                               IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                               TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                               OrderedQuantity = db.PurchaseOrderDetails.Where(p => p.PurchaseOrderID == o.ID).Select(p => p.Quantity).DefaultIfEmpty(0).Sum(),
                                               ReceivedQuantity = (from i in db.InvoiceDetails
                                                                   join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                                   join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                                   where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                                   where pd.PurchaseOrderID == o.ID
                                                                   select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).DefaultIfEmpty(0).Sum(),
                                               OrderDate = o.OrderDate,
                                               Amount = o.Amount == null ? 0 : o.Amount,
                                               ExpetedDeliveryDate = o.ExpetedDeliveryDate
                                           }).OrderByDescending(x => x.PurchaseOrderID).ToList(); //.OrderByDescending(o => o.OrderDate)
            }
            Session["lPurchaseOrderDetailsViewModel"] = null;
            Session["Amount"] = null;

            if (Session["USER_NAME"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

            //yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            if (Session["Entity"].ToString() == "EVW")
            {
                ViewBag.PossibleWarehouse = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == WarehouseID && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID))).ToList();
            }
            else if (Session["Entity"].ToString() == "DV")
            {
                EVWsDV vWsDV = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == WarehouseID && p.IsActive == true);
                if (vWsDV != null)
                {
                    ViewBag.PossibleWarehouse = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == vWsDV.WarehouseId_EVW && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID))).ToList();
                }
                else
                {
                    Session["Warning"] = "This warehouse is not register within any EVW";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (Session["Entity"].ToString() == "FV")
            {
                long DVId = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID).DistributorId.Value;
                EVWsDV vWsDV = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == DVId && p.IsActive == true);
                if (vWsDV != null)
                {
                    ViewBag.DVId = DVId;
                    ViewBag.PossibleWarehouse = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == vWsDV.WarehouseId_EVW && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID))).ToList();
                }
                else
                {
                    Session["Warning"] = "This warehouse is not register within any EVW";
                    return RedirectToAction("Index", "Home");
                }
            }
            objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();
            return View("Index", objPO);
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
                                               //join pd in db.PurchaseOrderDetails on o.ID equals pd.PurchaseOrderID
                                               where w.ID == WarehouseID && o.SupplierID == SupplierID
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   IsSent = o.IsSent,
                                                   IsAcceptedBySupplier = o.IsAcceptedBySupplier,
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
                    return RedirectToAction("Index");
                }

                Session["lQuotationItemDetailViewModel"] = null;
                //yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("Index", objPO);
        }

        //
        // GET: /PurchaseOrders/Details/5

        public ViewResult Details(long id)
        {
            PurchaseOrderViewModel PurchaseOrderViewModel = new PurchaseOrderViewModel();

            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);

            Session["lPurchaseOrderDetailsViewModel"] = null;

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
                                          DVId = (po.DVId == null || po.DVId == 0) ? 0 : po.DVId.Value,
                                          FVId = (po.FVId == null || po.FVId == 0) ? 0 : po.FVId.Value,
                                      }).FirstOrDefault();

            if (PurchaseOrderViewModel.DVId != 0 && PurchaseOrderViewModel.FVId != 0)
            {
                ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == PurchaseOrderViewModel.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == PurchaseOrderViewModel.FVId).Name;
            }
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
        // GET: /PurchaseOrders/Create

        public ActionResult Create(long Dvid, long FvId)
        {
            PurchaseOrderViewModel objPO = new PurchaseOrderViewModel();
            if (Session["WarehouseID"] != null && Convert.ToInt32(Session["WarehouseID"]) > 0)
            {
                int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
                ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == WarehouseID);
                ViewBag.WarehouseID = WarehouseID;

                List<Category> lCategory = new List<Category>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lCategory = db.Categories.Where(x => x.ParentCategoryID == null && x.IsActive == true).ToList();

                foreach (var c in lCategory)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.PossiblePrentCategory = forloopclasses.ToList();

                //yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            }
            else
            {
                if (Session["USER_NAME"] != null)
                {
                    ViewBag.PossibleWarehouses = db.Warehouses;
                    ViewBag.PossibleSuppliers = db.Suppliers;
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }

            objPO.OrderDate = DateTime.Now;
            objPO.ExpetedDeliveryDate = DateTime.Now;
            //Session["lPurchaseOrderDetailsViewModel"] = null;
            //Session["Amount"] = null;
            if (Session["Entity"] != null)
            {
                objPO.DVId = Dvid;
                objPO.FVId = FvId;
                Session["SelectedDvId"] = Dvid;
                Session["SelectedFvId"] = FvId;
                ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == Dvid).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == FvId).Name;
            }
            return View(objPO);
        }

        public PartialViewResult Add(int ProductID, long ProductVarientID, int Quantity, string Nickname, decimal UnitPrice, string RateCalculationID, string RateMatrixExtensionId)
        {
            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModel = new List<PurchaseOrderDetailsViewModel>();

            if (Session["lPurchaseOrderDetailsViewModel"] != null)
            {
                lPurchaseOrderDetailsViewModel = (List<PurchaseOrderDetailsViewModel>)Session["lPurchaseOrderDetailsViewModel"];
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
            objd.RateCalculationID = RateCalculationID == null ? 0 : RateCalculationID == "" ? 0 : Convert.ToInt64(RateCalculationID);
            objd.RateMatrixExtensionId = RateMatrixExtensionId == null ? 0 : RateMatrixExtensionId == "" ? 0 : Convert.ToInt64(RateMatrixExtensionId);
            foreach (var item in itemName)
            {
                objd.ItemName = item.ItemName.ToString();
                objd.HSNCode = Convert.ToString(item.HSNCode);
            }
            objd.Quantity = Quantity;
            objd.Nickname = Nickname;

            ViewBag.flag = false;
            var vId = lPurchaseOrderDetailsViewModel.Find(x => x.ProductVarientID == ProductVarientID);
            if (vId == null)
            {
                lPurchaseOrderDetailsViewModel.Add(objd);
                decimal pAmount = UnitPrice * Convert.ToDecimal(Quantity);

                if (Session["Amount"] != null)
                {
                    decimal Amount = Convert.ToDecimal(Session["Amount"].ToString());
                    Session["Amount"] = Amount + pAmount;
                }
                else
                {
                    Session["Amount"] = pAmount;
                }
                ViewBag.flag = true;
            }


            Session["lPurchaseOrderDetailsViewModel"] = lPurchaseOrderDetailsViewModel;

            return PartialView("ItemDetails", lPurchaseOrderDetailsViewModel);
        }


        public PartialViewResult Remove(long ProductVarientID)
        {
            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModel = new List<PurchaseOrderDetailsViewModel>();

            if (Session["lPurchaseOrderDetailsViewModel"] != null)
            {
                lPurchaseOrderDetailsViewModel = (List<PurchaseOrderDetailsViewModel>)Session["lPurchaseOrderDetailsViewModel"];
                var id = lPurchaseOrderDetailsViewModel.First(x => x.ProductVarientID == ProductVarientID);

                decimal UnitPrice = id.UnitPrice;
                decimal Quantity = id.Quantity;

                //Added by Priti  on  28-2-2019
                double? MRP = id.MRP;
                double? DecidedSalePrice = id.DecidedSalePrice;
                double? Margin = id.Margin;


                decimal pAmount = UnitPrice * Quantity;
                decimal Amount = Convert.ToDecimal(Session["Amount"].ToString());
                Session["Amount"] = Amount - pAmount;

                lPurchaseOrderDetailsViewModel.Remove(id);
                Session["lPurchaseOrderDetailsViewModel"] = lPurchaseOrderDetailsViewModel;
            }

            return PartialView("ItemDetails", lPurchaseOrderDetailsViewModel);
        }

        //
        // POST: /PurchaseOrders/Create


        [HttpPost]
        public ActionResult Create(PurchaseOrderViewModel purchaseorder)
        {
            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModel = new List<PurchaseOrderDetailsViewModel>();

            if (Session["lPurchaseOrderDetailsViewModel"] != null && Session["Amount"] != null)
            {
                lPurchaseOrderDetailsViewModel = (List<PurchaseOrderDetailsViewModel>)Session["lPurchaseOrderDetailsViewModel"];
                if (lPurchaseOrderDetailsViewModel.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        using (TransactionScope tscope = new TransactionScope())
                        {
                            try
                            {
                                //Insert into PurchaseOrder Table
                                PurchaseOrder objPurchaseOrder = new PurchaseOrder();
                                objPurchaseOrder.WarehouseID = purchaseorder.WarehouseID;
                                objPurchaseOrder.SupplierID = purchaseorder.SupplierID;
                                objPurchaseOrder.PurchaseOrderCode = GetNextPurchaseOrderCode();
                                objPurchaseOrder.OrderDate = purchaseorder.OrderDate;
                                objPurchaseOrder.ExpetedDeliveryDate = purchaseorder.ExpetedDeliveryDate;
                                objPurchaseOrder.Amount = Convert.ToDecimal(Session["Amount"].ToString());
                                objPurchaseOrder.Remark = purchaseorder.Remark;
                                objPurchaseOrder.DVId = Convert.ToInt64(Session["SelectedDvId"]);
                                objPurchaseOrder.FVId = Convert.ToInt64(Session["SelectedFvId"]);
                                //Added by Priti
                                //objPurchaseOrder.MRP = db.RateCalculations.FirstOrDefault(p => p.ID == purchaseorder.EzeeloMRP ).MRP;





                                objPurchaseOrder.IsSent = purchaseorder.IsSent;
                                if (purchaseorder.IsSent == true)
                                {
                                    objPurchaseOrder.ModifyDate = DateTime.Now;
                                    objPurchaseOrder.ModifyBy = GetPersonalDetailID();
                                }
                                objPurchaseOrder.IsActive = true;
                                objPurchaseOrder.CreateDate = DateTime.Now;
                                objPurchaseOrder.CreateBy = GetPersonalDetailID();
                                objPurchaseOrder.NetworkIP = CommonFunctions.GetClientIP();
                                objPurchaseOrder.DeviceID = "X";
                                objPurchaseOrder.DeviceType = "X";
                                db.PurchaseOrders.Add(objPurchaseOrder);
                                db.SaveChanges();

                                long PurchaseOrderID = objPurchaseOrder.ID;

                                //Insert into PurchaseOrderDetail Table
                                PurchaseOrderDetail objPurchaseOrderDetail = new PurchaseOrderDetail();
                                foreach (PurchaseOrderDetailsViewModel item in lPurchaseOrderDetailsViewModel)
                                {
                                    objPurchaseOrderDetail.PurchaseOrderID = PurchaseOrderID;
                                    objPurchaseOrderDetail.ProductID = item.ProductID;
                                    objPurchaseOrderDetail.ProductNickname = item.Nickname;
                                    objPurchaseOrderDetail.ProductVarientID = item.ProductVarientID;
                                    objPurchaseOrderDetail.Quantity = item.Quantity;
                                    objPurchaseOrderDetail.UnitPrice = item.UnitPrice;
                                    objPurchaseOrderDetail.RateCalculationId = 0;
                                    objPurchaseOrderDetail.RateMatrixId = item.RateCalculationID == null ? 0 : item.RateCalculationID;
                                    objPurchaseOrderDetail.RateMatrixExtensionId = item.RateMatrixExtensionId;
                                    objPurchaseOrderDetail.IsActive = true;

                                    db.PurchaseOrderDetails.Add(objPurchaseOrderDetail);
                                    db.SaveChanges();
                                }

                                tscope.Complete();

                                //Yashaswi 02/05/2018
                                //Send Email To Supplier
                                if (objPurchaseOrder.IsSent == true)
                                {
                                    using (TransactionScope tscope1 = new TransactionScope())
                                    {
                                        SendMailForPrurchaseOrder(objPurchaseOrder.ID);
                                        tscope1.Complete();
                                    }
                                }
                                Session["Success"] = "Purchase Order Created Successfully.";//yashaswi 30/3/2018
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
            }
            ViewBag.PossibleWarehouses = db.Warehouses;
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            return View(purchaseorder);
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
            string lOrderPrefix = "PO" + lDay.ToString("00") + lMonth.ToString("00") + lYear.ToString().Substring(2, 2);

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

        public ActionResult Edit(long id)
        {
            PurchaseOrderViewModel PurchaseOrderViewModel = new PurchaseOrderViewModel();
            if (Session["WarehouseID"] != null && Convert.ToInt32(Session["WarehouseID"]) > 0)
            {
                int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
                ViewBag.PossibleWarehouses = db.Warehouses.Where(x => x.ID == WarehouseID);
                ViewBag.WarehouseID = WarehouseID;

                List<Category> lCategory = new List<Category>();
                List<ForLoopClass> forloopclasses = new List<ForLoopClass>();

                lCategory = db.Categories.Where(x => x.ParentCategoryID == null && x.IsActive == true).ToList();

                foreach (var c in lCategory)
                {
                    ForLoopClass av = new ForLoopClass();
                    av.ID = c.ID;
                    av.Name = c.Name;
                    forloopclasses.Add(av);
                }

                ViewBag.PossiblePrentCategory = forloopclasses.ToList();

                //Yashaswi 9/4/2018
                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            }
            else
            {
                ViewBag.PossibleWarehouses = db.Warehouses;
                //return RedirectToAction("Index", "Login");
            }

            Session["lPurchaseOrderDetailsViewModel"] = null;
            Session["Amount"] = null;

            var purchaseOrder = db.PurchaseOrders.Single(x => x.ID == id);

            if (purchaseOrder != null && purchaseOrder.ToString() != "")
            {
                PurchaseOrderViewModel.PurchaseOrderID = id;
                PurchaseOrderViewModel.WarehouseID = purchaseOrder.WarehouseID;
                PurchaseOrderViewModel.SupplierID = purchaseOrder.SupplierID;
                PurchaseOrderViewModel.OrderDate = purchaseOrder.OrderDate;
                PurchaseOrderViewModel.ExpetedDeliveryDate = purchaseOrder.ExpetedDeliveryDate;
                PurchaseOrderViewModel.Amount = purchaseOrder.Amount;
                PurchaseOrderViewModel.PurchaseOrderCode = purchaseOrder.PurchaseOrderCode;
                Session["Amount"] = purchaseOrder.Amount;
                PurchaseOrderViewModel.Remark = purchaseOrder.Remark;
                PurchaseOrderViewModel.IsSent = purchaseOrder.IsSent;
                PurchaseOrderViewModel.IsActive = purchaseOrder.IsActive;
                PurchaseOrderViewModel.DVId = purchaseOrder.DVId.Value;
                PurchaseOrderViewModel.FVId = purchaseOrder.FVId.Value;
                if (purchaseOrder.DVId != null && purchaseOrder.FVId != null)
                {
                    ViewBag.SelectedDvName = "Rate matrix display according to rate save for DV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.DVId).Name + " and FV:- " + db.Warehouses.FirstOrDefault(p => p.ID == purchaseOrder.FVId).Name;
                }
            }

            List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
            lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == id).ToList();


            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModelList = new List<PurchaseOrderDetailsViewModel>();

            foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
            {
                PurchaseOrderDetailsViewModel objPOD = new PurchaseOrderDetailsViewModel();
                objPOD.PurchaseOrderDetailsID = item.ID;
                objPOD.ProductID = item.ProductID;
                objPOD.ProductVarientID = Convert.ToInt64(item.ProductVarientID);
                objPOD.Quantity = item.Quantity;
                objPOD.UnitPrice = item.UnitPrice;
                objPOD.RateCalculationID = item.RateMatrixId;
                objPOD.RateMatrixExtensionId = item.RateMatrixExtensionId;
                objPOD.ProductAmount = item.Quantity * item.UnitPrice;
                objPOD.Nickname = item.ProductNickname;
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);

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
        // POST: /PurchaseOrders/Edit/5

        [HttpPost]
        public ActionResult Edit(PurchaseOrderViewModel purchaseorder)
        {
            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModel = new List<PurchaseOrderDetailsViewModel>();

            if (Session["lPurchaseOrderDetailsViewModel"] != null && Session["Amount"] != null)
            {
                lPurchaseOrderDetailsViewModel = (List<PurchaseOrderDetailsViewModel>)Session["lPurchaseOrderDetailsViewModel"];
                if (lPurchaseOrderDetailsViewModel.Count > 0)
                {
                    if (ModelState.IsValid)
                    {
                        using (TransactionScope tscope = new TransactionScope())
                        {
                            try
                            {
                                //Update PurchaseOrder
                                var lPurchaseOrder = new PurchaseOrder()
                                {
                                    ID = purchaseorder.PurchaseOrderID,
                                    WarehouseID = purchaseorder.WarehouseID,
                                    SupplierID = purchaseorder.SupplierID,
                                    OrderDate = purchaseorder.OrderDate,
                                    ExpetedDeliveryDate = purchaseorder.ExpetedDeliveryDate,
                                    Amount = Convert.ToDecimal(Session["Amount"].ToString()),
                                    Remark = purchaseorder.Remark,
                                    IsSent = purchaseorder.IsSent,
                                    ModifyDate = DateTime.Now,
                                    ModifyBy = GetPersonalDetailID(),
                                    IsActive = purchaseorder.IsActive,
                                    NetworkIP = CommonFunctions.GetClientIP(),
                                    DeviceID = "X",
                                    DeviceType = "X"
                                };

                                db.PurchaseOrders.Attach(lPurchaseOrder);
                                db.Entry(lPurchaseOrder).Property(x => x.WarehouseID).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.SupplierID).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.OrderDate).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.ExpetedDeliveryDate).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.Amount).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.Remark).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.IsSent).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.ModifyDate).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.ModifyBy).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.IsActive).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.NetworkIP).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.DeviceID).IsModified = true;
                                db.Entry(lPurchaseOrder).Property(x => x.DeviceType).IsModified = true;
                                db.SaveChanges();


                                DataTable dtPurchaseOrderDetail = new DataTable();
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("PurchaseOrderDetailID", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("PurchaseOrderID", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("ProductID", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("ProductNickname", typeof(string)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("ProductVarientID", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("Quantity", typeof(int)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("UnitPrice", typeof(decimal)));

                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("RateCalculationID", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("RateMatrixId", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("RateMatrixExtensionId", typeof(long)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("Remark", typeof(string)));
                                dtPurchaseOrderDetail.Columns.Add(new DataColumn("IsActive", typeof(bool)));


                                //Insert/Update into PurchaseOrderDetail Table
                                PurchaseOrderDetail objPurchaseOrderDetail = new PurchaseOrderDetail();
                                foreach (PurchaseOrderDetailsViewModel item in lPurchaseOrderDetailsViewModel)
                                {
                                    DataRow dr = dtPurchaseOrderDetail.NewRow();
                                    dr["PurchaseOrderDetailID"] = item.PurchaseOrderDetailsID;
                                    dr["PurchaseOrderID"] = purchaseorder.PurchaseOrderID;
                                    dr["ProductID"] = item.ProductID;
                                    dr["ProductNickname"] = item.Nickname;
                                    dr["ProductVarientID"] = item.ProductVarientID;
                                    dr["Quantity"] = item.Quantity;
                                    dr["UnitPrice"] = item.UnitPrice;


                                    dr["RateCalculationID"] = 0;
                                    dr["RateMatrixId"] = item.RateCalculationID;
                                    dr["RateMatrixExtensionId"] = item.RateMatrixExtensionId;
                                    dr["Remark"] = item.ProductRemark;
                                    dr["IsActive"] = true;

                                    dtPurchaseOrderDetail.Rows.Add(dr);
                                }

                                SqlConnection con = new SqlConnection(fConnectionString);
                                SqlCommand cmd = new SqlCommand("UpdatePurchaseOrderDetail", con);
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlParameter[] objParam = new SqlParameter[2];
                                objParam[0] = new SqlParameter("@PurchaseOrderID", purchaseorder.PurchaseOrderID);
                                objParam[1] = new SqlParameter("@TblTypePurchaseOrderDetail", dtPurchaseOrderDetail);

                                int i = 0;
                                for (i = 0; i < objParam.Length; i++)
                                {
                                    cmd.Parameters.Add(objParam[i]);
                                }

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();

                                tscope.Complete();
                                Session["Success"] = "Purchase Order Updated Successfully.";//yashaswi 30/3/2018
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
            }
            ViewBag.PossibleWarehouses = db.Warehouses;
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
            return View(purchaseorder);
        }


        //
        // GET: /PurchaseOrders/PrintOrder/5
        public ViewResult PrintOrder(long? id)
        {
            PurchaseOrderViewModel PurchaseOrderViewModel = new PurchaseOrderViewModel();
            //Yashaswi 2/5/2018
            //When Request made from Email
            if (id == null)
            {
                id = long.Parse(Session["PO_ID_FROM_EMAIL"].ToString());
            }
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);

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


        //New Formate of PO BY Priti
        public ActionResult PrintOrder1(long? id)
        {
            PurchaseOrderViewModel PurchaseOrderViewModel = new PurchaseOrderViewModel();
            //Yashaswi 2/5/2018
            //When Request made from Email
            if (id == null)
            {
                id = long.Parse(Session["PO_ID_FROM_EMAIL"].ToString());
            }
            int WarehouseID = Convert.ToInt32(Session["WarehouseID"]);
            //Yashaswi 9/4/2018
            ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);

            Session["lPurchaseOrderDetailsViewModel"] = null;

            PurchaseOrderViewModel = (from po in db.PurchaseOrders

                                      join s in db.Suppliers on po.SupplierID equals s.ID
                                      join w in db.Warehouses on po.WarehouseID equals w.ID
                                      join pd in db.PersonalDetails on po.CreateBy equals pd.ID///Added by Priti  28/9/2018
                                      //  join pt in db.PaymentInTerms on s.PaymentInTermsID equals pt.ID
                                      //join st in db.States on w.StateID equals st.ID   ////Added by Priti
                                      //join st1 in db.States on s.StateID equals st1.ID ////Addded by Priti
                                      //join p in db.PurchaseOrderReply on po.ID equals p.PurchaseOrderID   ///Added by Priti
                                      join b in db.BusinessDetails on w.BusinessDetailID equals b.ID
                                      where po.ID == id



                                      select new PurchaseOrderViewModel
                                      {
                                          PurchaseOrderID = po.ID,
                                          //FSSILicenseNo = w.FSSILicenseNo, ///Added by Priti
                                          FSSILicenseNo = s.FSSILicenseNo,
                                          //PaymentTermsID=db.Suppliers.Where(X=>X.ID==s.ID).Select(Y=>Y.PaymentInTermsID).FirstOrDefault()  ,
                                          PaymentTermsID = db.Suppliers.Where(x => x.PaymentInTermsID != null).Select(y => y.PaymentInTermsID).FirstOrDefault(),
                                          PaymentInTermsname = db.PaymentInTerms.FirstOrDefault(x => x.ID == (db.Suppliers.FirstOrDefault(Y => Y.PaymentInTermsID != null).PaymentInTermsID)).Name,
                                          // PaymentInTermsname = db.PaymentInTerms.Where(x => x.ID == PaymentTermsID).Select(y => y.PaymentInTermsID).FirstOrDefault(),
                                          PurchaseOrderCode = po.PurchaseOrderCode,
                                          WarehouseName = w.Name,
                                          SupplierName = s.Name,
                                          SupplierEmail = s.Email,

                                          StateID = w.StateID,
                                          // StateCode = st.StateCode,       ///FV State cODE  
                                          // StateCode1 = st1.ID,   ///  DV State Code
                                          WarehouseGSTNumber = w.GSTNumber,
                                          SupplierGSTNumber = s.GSTNumber,
                                          OrderDate = po.OrderDate,
                                          CreateDate = po.CreateDate,
                                          ExpetedDeliveryDate = po.ExpetedDeliveryDate,
                                          Amount = po.Amount == null ? 0 : po.Amount,
                                          Remark = po.Remark,
                                          // TINNumber=st.TINNumber,
                                          CreateBy = po.CreateBy,
                                          // StateCode1=w.StateID,
                                          IsActive = po.IsActive,
                                          SupplierCode = s.SupplierCode,
                                          SupplierContactPerson = s.ContactPerson,
                                          SupplierAddress = s.Address,
                                          SupplierMobile = s.Mobile,
                                          SupplierPanNumber = s.PAN,
                                          CreateByName = pd.FirstName,
                                          ModifyByName = pd.FirstName,
                                          WarehouseContactPerson = b.ContactPerson,
                                          WarehouseAddress = b.Address,
                                          WarehouseMobile = b.Mobile


                                      }).FirstOrDefault();



            List<PurchaseOrderDetail> lPurchaseOrderDetailslist = new List<PurchaseOrderDetail>();
            lPurchaseOrderDetailslist = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == id).ToList();


            List<PurchaseOrderDetailsViewModel> lPurchaseOrderDetailsViewModelList = new List<PurchaseOrderDetailsViewModel>();

            //var Result = (from po in db.PurchaseOrders
            //              join pd in db.PersonalDetails on po.CreateBy equals pd.UserLoginID       ///Added by Priti  28/9/2018
            //              where po.ID == id                                                              ///

            //              select new PurchaseOrderDetailsViewModel { CreateByName = pd.FirstName + " (" + pd.LastName + ")", ModifyByName = pd.FirstName }).ToList();
            foreach (PurchaseOrderDetail item in lPurchaseOrderDetailslist)
            {
                PurchaseOrderDetailsViewModel objPOD = new PurchaseOrderDetailsViewModel();
                objPOD.Quantity = item.Quantity;
                objPOD.Nickname = item.ProductNickname;
                objPOD.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                objPOD.UnitPrice = item.UnitPrice;
                objPOD.MRP = (item.RateCalculationId == 0 && item.RateMatrixId != null) ? db.RateMatrix.FirstOrDefault(p => p.ID == item.RateMatrixId).MRP : (db.RateCalculations.FirstOrDefault(p => p.ID == item.RateCalculationId).MRP);
                objPOD.GSTInPer = (item.RateCalculationId == 0 && item.RateMatrixId != null) ? db.RateMatrix.FirstOrDefault(p => p.ID == item.RateMatrixId).GSTInPer : db.RateCalculations.FirstOrDefault(p => p.ID == item.RateCalculationId).GSTInPer;




                objPOD.ProductAmount = item.Quantity * item.UnitPrice;



                var itemName = (from p in db.Products
                                join pr in db.PurchaseOrderDetails on p.ID equals pr.ProductID       ///Added by Priti  28/9/2018
                                //join R in db.RateCalculations on pr.RateCalculationId equals R.ID

                                //join pro in db.PurchaseOrders on pr.PurchaseOrderID equals pro.ID         ///Added by Priti  28/9/2018
                                //join pry in db.PurchaseOrderReply on pro.ID equals pry.PurchaseOrderID      ///Added by Priti  28/9/2018
                                join b in db.Brands on p.BrandID equals b.ID   ///added by Priti
                                join v in db.ProductVarients on p.ID equals v.ProductID
                                join s in db.Sizes on v.SizeID equals s.ID

                                where v.ID == item.ProductVarientID
                                select new PurchaseOrderDetailsViewModel { ItemName = p.Name, SKUUnit = s.Name, HSNCode = p.HSNCode, EANCode = p.EANCode, SKUID = v.ID, BrandID = b.ID, BrandName = b.Name }).ToList();

                foreach (var i in itemName)
                {
                    objPOD.ItemName = i.ItemName.ToString();
                    objPOD.SKUUnit = i.SKUUnit.ToString();
                    objPOD.HSNCode = Convert.ToString(i.HSNCode);
                    objPOD.EANCode = Convert.ToString(i.EANCode);
                    //objPOD.MRP = i.MRP;
                    //objPOD.GSTInPer = i.GSTInPer;
                    objPOD.SKUID = i.SKUID;
                    objPOD.BrandID = i.BrandID;
                    objPOD.ModifiedDate = i.ModifiedDate;
                    objPOD.BrandName = Convert.ToString(i.BrandName);
                    objPOD.CGSTAmount = i.CGSTAmount;
                    objPOD.DiscountAmount = i.DiscountAmount;
                    objPOD.DiscountInPer = i.DiscountInPer;
                    //objPOD.CGSTAmount = objPOD.Quantity * i.CGSTAmount;
                    objPOD.TaxableValue = i.TaxableValue;


                    objPOD.TaxableValue = (objPOD.ProductAmount * 100) / (objPOD.GSTInPer + 100);
                    objPOD.TaxableValue = Math.Round(objPOD.TaxableValue, 2);

                    //objPOD.TaxableValue = Math.Truncate(objPOD.TaxableValue * 100) / 100;



                    objPOD.BasicCost = (objPOD.TaxableValue) / (objPOD.Quantity);
                    objPOD.BasicCost = Math.Round(objPOD.BasicCost, 2);
                    objPOD.GSTAmount = objPOD.ProductAmount - objPOD.TaxableValue;




                }
                //var Result = (from po in db.PurchaseOrders
                //              join pd in db.PersonalDetails on po.CreateBy equals pd.UserLoginID       ///Added by Priti  28/9/2018
                //              ///Added by Priti  28/9/2018



                //              select new PurchaseOrderDetailsViewModel { CreateByName = pd.FirstName + " (" + pd.LastName + ")", ModifyByName = pd.FirstName }).ToList();

                //foreach (var i in itemName)
                //{
                //    objPOD.CreateByName = i.CreateByName.ToString();
                //    objPOD.ModifyByName = i.ModifyByName.ToString();

                //}

                //var BrandIds = lPurchaseOrderDetailsViewModelList.GroupBy(x => x.BrandID);
                //if (BrandIds != null)
                //{
                //    List<PoBrandViewModel> PoBrandList = new List<PoBrandViewModel>();
                //    double MRPCount = 0;
                //    foreach (var j in BrandIds)
                //    {
                //        PoBrandViewModel PoBrand = new PoBrandViewModel();
                //        PoBrand.BrandId = j.Key;
                //        PoBrand.MRP = lPurchaseOrderDetailsViewModelList.Where(x => x.BrandID == j.Key).Sum(x => x.MRP);
                //        PoBrand.Qty = lPurchaseOrderDetailsViewModelList.Where(x => x.BrandID == j.Key).Sum(x => x.Quantity);
                //        PoBrandList.Add(PoBrand);
                //    }
                //}

                //var details1 = lPurchaseOrderDetailsViewModelList.GroupBy(n => n.BrandID, n =>n.BrandName).
                //           Select(group =>
                //           new
                //           {
                //               BrandID = group.Key,

                //              Count = group.),

                //           });

                //foreach (var b in details1)
                //{

                //     objPOD.BrandID=b.BrandID;

                //}


                //List<PurchaseOrderDetailsViewModel> details1 = lPurchaseOrderDetailsViewModelList.GroupBy(n => n.BrandID).
                //           Select(group =>
                //          new
                //          {
                //              BrandID = group.Key,

                //              //List = group.ToList(),

                //          });

                lPurchaseOrderDetailsViewModelList.Add(objPOD);
            }


            //var details = db.Products.GroupBy(n => n.BrandID).
            //                 Select(group =>
            //                 new
            //                    {
            //                        BrandID = group.Key,
            //                        Count = group.Count()
            //                    }).ToList();



            var details = lPurchaseOrderDetailsViewModelList.GroupBy(n => n.ModifiedDate).
                      Select(group =>
                      new
                      {
                          ModifiedDate = group.Key,
                          Count = group.Count()
                      }).ToList();

            if (lPurchaseOrderDetailsViewModelList.Count > 0)
            {
                Session["lPurchaseOrderDetailsViewModel"] = lPurchaseOrderDetailsViewModelList;
            }



            PurchaseOrderDetailsViewModel objPOD1 = new PurchaseOrderDetailsViewModel();


            var rEsult = from a in lPurchaseOrderDetailsViewModelList group a by a.BrandID into g orderby g.Key select g;




            var Result1 = (from po in db.PurchaseOrders
                           join pd in db.PersonalDetails on po.CreateBy equals pd.UserLoginID       ///Added by Priti  28/9/2018
                                                                                                    ///Added by Priti  28/9/2018



                           select new PurchaseOrderDetailsViewModel { CreateByName = pd.FirstName + " (" + pd.LastName + ")", ModifyByName = pd.FirstName }).ToList();
            var wordlist = new List<string> { "test", "one", "two" };
            var grouped = wordlist.GroupBy(i => i)
            .Select(i => new
            {
                Word = i.Key,
                Count = i.Count()
            });





            List<PurchaseOrderDetailsViewModel> rEsult1 = lPurchaseOrderDetailsViewModelList.GroupBy(I => I.BrandID).Select(cl => new PurchaseOrderDetailsViewModel
            {
                ItemName = cl.First().ItemName,
                BrandName = cl.First().BrandName,
                Quantity = cl.Sum(c => c.Quantity),
                ProductAmount = cl.Sum(c => c.ProductAmount),
                BrandID = cl.First().BrandID,
                MRP = cl.Sum(c => c.MRP * c.Quantity),

            }).ToList();

            Session["lPurchaseOrderDetailsAnnextureViewModel"] = rEsult1;
            //foreach (var CB in rEsult1)
            //{
            //    objPOD1.BrandID = CB.BrandID;
            //    objPOD1.BrandName = CB.BrandName;
            //    objPOD1.Quantity = CB.Quantity;
            //    objPOD1.MRP = CB.MRP;
            //}

            ViewBag.PurchaseOrderID = id;
            return View(PurchaseOrderViewModel);

        }
        //
        // GET: /PurchaseOrders/Delete/5

        public ActionResult Delete(long id)
        {
            PurchaseOrder purchaseorder = db.PurchaseOrders.Single(x => x.ID == id);
            return View(purchaseorder);
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

        public ActionResult Send(long id)
        {
            using (TransactionScope tscope = new TransactionScope())
            {
                try
                {
                    //Yashaswi 02/05/2018
                    //Send Email To Supplier
                    //Update PurchaseOrder
                    PurchaseOrder lPurchaseOrder = db.PurchaseOrders.FirstOrDefault(p => p.ID == id);
                    lPurchaseOrder.IsSent = true;
                    lPurchaseOrder.ModifyDate = DateTime.Now;
                    lPurchaseOrder.ModifyBy = GetPersonalDetailID();
                    db.SaveChanges();

                    SendMailForPrurchaseOrder(id);

                    tscope.Complete();
                }
                catch (Exception)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                }
            }

            return RedirectToAction("Index");
        }


        #region Methods called in jquery
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
            //Yashaswi 9/4/2018
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (WarehouseID != EzeeloWarehouseId)
            {

                var fList = db.WarehouseFranchises.Where(x => x.WarehouseID == WarehouseID && x.IsActive == true).Select(x => x.FranchiseID).ToList();

                if (fList.Count > 0)
                {
                    var query = (from p in db.Products
                                 join sp in db.ShopProducts on p.ID equals sp.ProductID
                                 join s in db.Shops on sp.ShopID equals s.ID
                                 join f in db.Franchises on s.FranchiseID equals f.ID
                                 where (fList.Contains(f.ID)) && p.CategoryID == categoryID && p.IsActive == true
                                 && sp.IsActive == true && s.IsActive == true && f.IsActive == true
                                 select new { Name = p.Name, ID = p.ID }).Distinct();

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
            }
            else
            {
                //var fList = db.WarehouseFranchises.Where(x => x.IsActive == true).Select(x => x.FranchiseID).ToList();

                //if (fList.Count > 0)
                //{
                var query = (from p in db.Products
                             join sp in db.ShopProducts on p.ID equals sp.ProductID
                             join s in db.Shops on sp.ShopID equals s.ID
                             join f in db.Franchises on s.FranchiseID equals f.ID
                             where p.CategoryID == categoryID && p.IsActive == true
                             && sp.IsActive == true && s.IsActive == true && f.IsActive == true
                             select new { Name = p.Name, ID = p.ID }).Distinct();

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
                //}
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductVarientList(int ProductID)
        {
            List<Product> lProduct = new List<Product>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();


            var query = (from vp in db.ProductVarients
                         join p in db.Products on vp.ProductID equals p.ID
                         join sp in db.ShopProducts on p.ID equals sp.ProductID
                         join sh in db.Shops on sp.ShopID equals sh.ID
                         join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                         join s in db.Sizes on vp.SizeID equals s.ID
                         where vp.ProductID == ProductID && sp.ProductID == ProductID && ss.ProductVarientID == vp.ID && vp.IsActive == true && s.IsActive == true
                         select new { VarientName = p.Name + " (" + s.Name + ")", ID = vp.ID }).Distinct();// Remove  by Rumana on1/04/2019

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
            SqlParameter[] objParam = new SqlParameter[2];
            objParam[0] = new SqlParameter("@ProductVarientID", ProductVarientID);
            objParam[1] = new SqlParameter("@WarehouseID", Convert.ToInt64(Session["WarehouseID"].ToString()));

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


        //Yashaswi 2/5/2018
        public ActionResult GetId(long id)
        {
            Session["PO_ID_FROM_EMAIL"] = id;
            return RedirectToAction("PrintOrder");
        }
        public ActionResult SendMailForPrurchaseOrder(long ID)
        {
            try
            {
                PurchaseOrder obj_PO = db.PurchaseOrders.FirstOrDefault(p => p.ID == ID);
                Supplier obj_Supplier = db.Suppliers.FirstOrDefault(s => s.ID == obj_PO.SupplierID);
                Warehouse obj_Warehouse = db.Warehouses.FirstOrDefault(w => w.ID == obj_PO.WarehouseID);

                var URI = @"" + WebConfigurationManager.AppSettings["INVENTORY_ROOT"] + "PurchaseOrders/GetId?id=" + ID + "";

                Dictionary<string, string> dictEmailValues = new Dictionary<string, string>();

                dictEmailValues.Add("<!--PURCHASE_ORDER_CODE-->", obj_PO.PurchaseOrderCode);
                dictEmailValues.Add("<!--SUPPLIER_NAME-->", obj_Supplier.Name);
                dictEmailValues.Add("<!--WAREHOUSE_NAME-->", obj_Warehouse.Name);
                dictEmailValues.Add("<!--ORDER_DATE-->", obj_PO.OrderDate.ToString("dd/MM/yyyy"));
                dictEmailValues.Add("<!--EXPECTED_DELIVERY_DATE-->", obj_PO.ExpetedDeliveryDate.ToString("dd/MM/yyyy"));
                dictEmailValues.Add("<!--URL-->", URI);

                BusinessLogicLayer.GateWay gateWay = new BusinessLogicLayer.Email(System.Web.HttpContext.Current.Server);
                string EmailID = obj_Supplier.Email;

                gateWay.SendEmail(BusinessLogicLayer.GateWay.EmailGateWays.GANDHIBAGH, BusinessLogicLayer.GateWay.SenderMail.INFO, BusinessLogicLayer.GateWay.EMailTypes.PURCHASE_ORDER, new string[] { EmailID, "sales@ezeelo.com" }, dictEmailValues, true);
                Session["Success"] = "Mail Sent Successfully.";

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[PurchaseOrdersController][M:SendMailForPrurchaseOrder]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Gandhibagh, System.Web.HttpContext.Current.Server);
                Session["Error"] = "Mail not sent! Please try again.";
            }

            return RedirectToAction("InvoiceList");
        }

        public JsonResult GetFVList(string DvId)
        {
            long DVid = Convert.ToInt64(DvId);
            List<Warehouse> list = db.Warehouses.Where(p => p.IsActive == true && p.DistributorId == DVid).ToList();
            var Fvlist = list.Select(p => new
            {
                ID = p.ID,
                Name = p.Name
            }).ToList();

            return Json(Fvlist.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}