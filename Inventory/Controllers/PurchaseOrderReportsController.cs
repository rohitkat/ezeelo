using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Data;

namespace Inventory.Controllers
{
    public class PurchaseOrderReportsController : Controller
    {
        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController Obj_Common = new CommonController();
        //
        // GET: /PurchaseOrderReports/



        [HandleError(View="Error")]
        public ActionResult PurchaseOrderHistory(PurchaseOrderViewModelList objPOV) //, int? print
        {
            try
            {
                PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel2 = new List<PurchaseOrderViewModel>(); //Added by Rumana 12/04/2019
                if (Session["USER_NAME"] != null)
                {  }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }

                
                if (string.IsNullOrEmpty(objPOV.FromDate))
                {
                    objPO.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.FromDate = objPOV.FromDate;
                }
                              
                if (string.IsNullOrEmpty(objPOV.ToDate))
                {
                    objPO.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.ToDate = objPOV.ToDate;
                }

                DateTime fDate = Convert.ToDateTime(objPO.FromDate);
                DateTime tDate = Convert.ToDateTime(objPO.ToDate);

                if (WarehouseID > 0)
                {
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join pod in db.PurchaseOrderDetails on o.ID equals pod.PurchaseOrderID
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join p in db.Products on pod.ProductID equals p.ID
                                               join v in db.ProductVarients on p.ID equals v.ProductID
                                               join z in db.Sizes on v.SizeID equals z.ID
                                               join r in db.RateCalculations on pod.RateCalculationId equals r.ID
                                               where o.WarehouseID == WarehouseID && (o.SupplierID == objPOV.SupplierID || objPOV.SupplierID == 0) 
                                               && pod.ProductVarientID==v.ID
                                               && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) >= fDate.Date && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) <= tDate.Date
                                               select new PurchaseOrderViewModel
                                               {           
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   IsSent = o.IsSent,
                                                   IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   OrderedQuantity = db.PurchaseOrderDetails.Where(pd => pd.PurchaseOrderID == o.ID).Select(pd => pd.Quantity).DefaultIfEmpty(0).Sum(),
                                                   ReceivedQuantity = (from i in db.InvoiceDetails
                                                                       join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                                       join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                                       where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                                       select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).DefaultIfEmpty(0).Sum(),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                                   ItemName = p.Name + " (" + z.Name + ")",
                                                   HSNCode = p.HSNCode,
                                                   Quantity = pod.Quantity,
                                                   //GRNQuantity = (from i in db.InvoiceDetails
                                                   //               join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                   //               join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                   //               where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                   //               select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).FirstOrDefault(),
                                                   UnitPrice = pod.UnitPrice,
                                                   TotalAmount = pod.UnitPrice* pod.Quantity,
                                                   EzeeloMRP = r.MRP

                                               }).OrderByDescending(x => x.PurchaseOrderID).ToList(); //.OrderByDescending(o => o.OrderDate)
                    //Added by Rumana 12/04/2019
                    lPurchaseOrderViewModel2 = (from o in db.PurchaseOrders
                                               join pod in db.PurchaseOrderDetails on o.ID equals pod.PurchaseOrderID
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join p in db.Products on pod.ProductID equals p.ID
                                               join v in db.ProductVarients on p.ID equals v.ProductID
                                               join z in db.Sizes on v.SizeID equals z.ID
                                               join rm in db.RateMatrix on pod.RateMatrixId equals rm.ID
                                               where o.WarehouseID == WarehouseID && (o.SupplierID == objPOV.SupplierID || objPOV.SupplierID == 0)
                                               && pod.ProductVarientID == v.ID
                                               && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) >= fDate.Date && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) <= tDate.Date
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   IsSent = o.IsSent,
                                                   IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   OrderedQuantity = db.PurchaseOrderDetails.Where(pd => pd.PurchaseOrderID == o.ID).Select(pd => pd.Quantity).DefaultIfEmpty(0).Sum(),
                                                   ReceivedQuantity = (from i in db.InvoiceDetails
                                                                       join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                                       join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                                       where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                                       select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).DefaultIfEmpty(0).Sum(),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                                   ItemName = p.Name + " (" + z.Name + ")",
                                                   HSNCode = p.HSNCode,
                                                   Quantity = pod.Quantity,
                                                   //GRNQuantity = (from i in db.InvoiceDetails
                                                   //               join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                   //               join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                   //               where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                   //               select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).FirstOrDefault(),
                                                   UnitPrice = pod.UnitPrice,
                                                   TotalAmount = pod.UnitPrice * pod.Quantity,
                                                   EzeeloMRP = rm.MRP

                                               }).OrderByDescending(x => x.PurchaseOrderID).ToList(); //.OrderByDescending(o => o.OrderDate)
                    lPurchaseOrderViewModel.AddRange(lPurchaseOrderViewModel2);
                    //Ended by Rumana 12/04/2019
                }

                //ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);



                ///Added by Priti on 28/9/2018
                List<Supplier> ONJ = Obj_Common.GetSupplierLIst(WarehouseID);
                ONJ.Add(new Supplier
                {
                    Name = "All Supplier",
                    ID = 0

                });

                ViewBag.PossibleSuppliers = ONJ.ToList();

                /// end Added by Priti on 28/9/2018
             
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();
                               
                return View("PurchaseOrderHistory", objPO);
            }
            catch(Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }


        //[HandleError(View = "Error")]
        public ActionResult PurchaseOrderHistoryPrint(PurchaseOrderViewModelList objPOV,string ToDate,string FromDate) //, int? print
        {
            try
            {
                PurchaseOrderViewModelList objPO = new PurchaseOrderViewModelList();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel = new List<PurchaseOrderViewModel>();
                List<PurchaseOrderViewModel> lPurchaseOrderViewModel2 = new List<PurchaseOrderViewModel>(); //Added by Rumana 12/04/2019

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


                if (string.IsNullOrEmpty(objPOV.FromDate))
                {
                    objPO.FromDate = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.FromDate = objPOV.FromDate;
                }

                if (string.IsNullOrEmpty(objPOV.ToDate))
                {
                    objPO.ToDate = DateTime.UtcNow.AddHours(5.5).ToString("MM/dd/yyyy");
                }
                else
                {
                    objPO.ToDate = objPOV.ToDate;
                }

                DateTime fDate = Convert.ToDateTime(objPO.FromDate);
                DateTime tDate = Convert.ToDateTime(objPO.ToDate);
                if (WarehouseID > 0)
                {
                    lPurchaseOrderViewModel = (from o in db.PurchaseOrders
                                               join pod in db.PurchaseOrderDetails on o.ID equals pod.PurchaseOrderID
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join p in db.Products on pod.ProductID equals p.ID
                                               join v in db.ProductVarients on p.ID equals v.ProductID
                                               join z in db.Sizes on v.SizeID equals z.ID
                                               join r in db.RateCalculations on pod.RateCalculationId equals r.ID
                                               where o.WarehouseID == WarehouseID && (o.SupplierID == objPOV.SupplierID || objPOV.SupplierID == 0)
                                               && pod.ProductVarientID == v.ID
                                               && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) >= fDate.Date && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) <= tDate.Date
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   IsSent = o.IsSent,
                                                   IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   OrderedQuantity = db.PurchaseOrderDetails.Where(pd => pd.PurchaseOrderID == o.ID).Select(pd => pd.Quantity).DefaultIfEmpty(0).Sum(),
                                                   ReceivedQuantity = (from i in db.InvoiceDetails
                                                                       join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                                       join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                                       where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                                       select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).DefaultIfEmpty(0).Sum(),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                                   ItemName = p.Name + " (" + z.Name + ")",
                                                   HSNCode = p.HSNCode,
                                                   Quantity = pod.Quantity,
                                                   //GRNQuantity = (from i in db.InvoiceDetails
                                                   //               join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                   //               join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                   //               where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                   //               select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).FirstOrDefault(),
                                                   UnitPrice = pod.UnitPrice,
                                                   TotalAmount = pod.UnitPrice * pod.Quantity,
                                                   EzeeloMRP = r.MRP

                                               }).OrderByDescending(x => x.PurchaseOrderID).ToList(); //.OrderByDescending(o => o.OrderDate)
                    //Added by Rumana 12/04/2019
                    lPurchaseOrderViewModel2 = (from o in db.PurchaseOrders
                                               join pod in db.PurchaseOrderDetails on o.ID equals pod.PurchaseOrderID
                                               join w in db.Warehouses on o.WarehouseID equals w.ID
                                               join s in db.Suppliers on o.SupplierID equals s.ID
                                               join p in db.Products on pod.ProductID equals p.ID
                                               join v in db.ProductVarients on p.ID equals v.ProductID
                                               join z in db.Sizes on v.SizeID equals z.ID
                                               join rm in db.RateMatrix on pod.RateMatrixId equals rm.ID
                                                where o.WarehouseID == WarehouseID && (o.SupplierID == objPOV.SupplierID || objPOV.SupplierID == 0)
                                               && pod.ProductVarientID == v.ID
                                               && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) >= fDate.Date && System.Data.Entity.DbFunctions.TruncateTime(o.CreateDate) <= tDate.Date
                                               select new PurchaseOrderViewModel
                                               {
                                                   PurchaseOrderID = o.ID,
                                                   WarehouseName = w.Name,
                                                   SupplierName = s.Name,
                                                   PurchaseOrderCode = o.PurchaseOrderCode,
                                                   IsSent = o.IsSent,
                                                   IsAcceptedBySupplier = o.IsAcceptedBySupplier,
                                                   TotalItems = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == o.ID).Select(x => x.ID).Count(),
                                                   OrderedQuantity = db.PurchaseOrderDetails.Where(pd => pd.PurchaseOrderID == o.ID).Select(pd => pd.Quantity).DefaultIfEmpty(0).Sum(),
                                                   ReceivedQuantity = (from i in db.InvoiceDetails
                                                                       join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                                       join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                                       where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                                       select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).DefaultIfEmpty(0).Sum(),
                                                   OrderDate = o.OrderDate,
                                                   Amount = o.Amount == null ? 0 : o.Amount,
                                                   ExpetedDeliveryDate = o.ExpetedDeliveryDate,
                                                   ItemName = p.Name + " (" + z.Name + ")",
                                                   HSNCode = p.HSNCode,
                                                   Quantity = pod.Quantity,
                                                   //GRNQuantity = (from i in db.InvoiceDetails
                                                   //               join pd in db.PurchaseOrderDetails on i.PurchaseOrderDetailID equals pd.ID
                                                   //               join inv in db.Invoices on i.InvoiceID equals inv.ID
                                                   //               where pd.PurchaseOrderID == o.ID && inv.IsApproved == true
                                                   //               select new InvoiceDetailViewModel { ReceivedQuantity = i.ReceivedQuantity }).Select(x => (int)x.ReceivedQuantity).FirstOrDefault(),
                                                   UnitPrice = pod.UnitPrice,
                                                   TotalAmount = pod.UnitPrice * pod.Quantity,
                                                   EzeeloMRP = rm.MRP

                                               }).OrderByDescending(x => x.PurchaseOrderID).ToList(); //.OrderByDescending(o => o.OrderDate)
                    lPurchaseOrderViewModel.AddRange(lPurchaseOrderViewModel2);
                //Ended by Rumana 12/04/2019
                }

                ViewBag.PossibleSuppliers = Obj_Common.GetSupplierLIst(WarehouseID);
                
                objPO.lPurchaseOrderViewModel = lPurchaseOrderViewModel.ToList();

                return View("PurchaseOrderHistoryPrint",objPO);
            }
            catch (Exception ex)
            {
                new Exception("Some unknown error encountered!");
            }
            return View();
        }          

	}
}