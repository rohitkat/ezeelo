using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using System.Globalization;

namespace Inventory.Controllers
{
    public class HomeController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public ActionResult Index(DateTime? FromDate, DateTime? ToDate, int? filter, int? f_hr, int? f_min, string f_tt, int? t_hr, int? t_min, string t_tt)
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
            RateMatrixController obj_RateMatrixController = new RateMatrixController();
            ViewBag.Salutation = new SelectList(db.Salutations, "ID", "Name", 1);
            ViewBag.SecurityQuestion = new SelectList(db.SecurityQuestions.Where(c => c.IsActive == true), "ID", "Question");
            //Start Yashaswi/2/4/2018
            try
            {
                double CurrentStockValue;
                long ProductQty;
                long SKU;
                double WastageValue;
                long WastageSKU;
                double ReturnValue;
                double ReorderValue;
                long OrderPlaced;
                long OrderConfirmed;
                long OrderPacked;
                long OrderDispacthed;
                long orderDelivered;
                long PendingPOApproval;
                long PODispacthed;
                long POReceived;

                DateTime _FromDate;
                DateTime _ToDate;
                filter = filter ?? 7; //Set Default value

                f_hr = f_hr ?? 0;
                f_min = f_min ?? 0;
                t_hr = t_hr ?? 11; //5/6/2018
                t_min = t_min ?? 59; //5/6/2018
                f_tt = f_tt ?? "AM";
                t_tt = t_tt ?? "PM";
                switch (filter)
                {
                    case 0:
                        _FromDate = Convert.ToDateTime(FromDate);
                        _ToDate = Convert.ToDateTime(ToDate);
                        break;
                    case 1://Today
                        _FromDate = DateTime.Now.Date;
                        _ToDate = DateTime.Now.Date;
                        break;
                    case 2://This week
                        _FromDate = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
                        _ToDate = _FromDate.AddDays(6);
                        break;
                    case 3://Current Month
                        _FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        _ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                        break;
                    case 4://Quarter
                        int _M = 0;
                        if (DateTime.Now.Month == 1)
                        {
                            _M = 11;
                        }
                        else if (DateTime.Now.Month == 2)
                        {
                            _M = 12;
                        }
                        else
                        {
                            _M = DateTime.Now.Month - 3;
                        }
                        _FromDate = new DateTime(DateTime.Now.Year, _M, 1);
                        _ToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                        break;
                    case 5://Year
                        _FromDate = new DateTime(DateTime.Now.Year, 1, 1);
                        _ToDate = new DateTime(DateTime.Now.Year, 12, 31);
                        break;
                    case 6://Till Date
                        _FromDate = new DateTime(2018, 1, 1);
                        _ToDate = DateTime.Now.Date;
                        break;

                    default://Till Date
                        _FromDate = new DateTime(2018, 1, 1);
                        _ToDate = DateTime.Now.Date;
                        break;

                }
                if (f_tt == "AM")
                {
                    _FromDate = _FromDate.AddHours((double)f_hr);
                }
                else
                {
                    _FromDate = _FromDate.AddHours((double)f_hr + 12);
                }
                _FromDate = _FromDate.AddMinutes((double)f_min);

                if (t_tt == "AM")
                {
                    _ToDate = _ToDate.AddHours((double)t_hr);
                }
                else
                {
                    _ToDate = _ToDate.AddHours((double)t_hr + 12);
                }
                _ToDate = _ToDate.AddMinutes((double)t_min);

                GetDashboardValue(_FromDate, _ToDate, out  CurrentStockValue, out  ProductQty, out SKU
          , out  WastageValue, out  WastageSKU, out  ReturnValue, out  ReorderValue
          , out  OrderPlaced, out  OrderConfirmed, out  OrderPacked, out  OrderDispacthed
          , out  orderDelivered, out  PendingPOApproval, out  PODispacthed, out  POReceived, filter);

                ViewBag.CurrentStockValue = CurrentStockValue;
                ViewBag.ProductQty = ProductQty;
                ViewBag.SKU = SKU;
                ViewBag.WastageValue = WastageValue;
                ViewBag.WastageSKU = WastageSKU;
                ViewBag.ReturnValue = ReturnValue;
                ViewBag.ReorderValue = ReorderValue;
                ViewBag.OrderPlaced = OrderPlaced;
                ViewBag.OrderConfirmed = OrderConfirmed;
                ViewBag.OrderPacked = OrderPacked;
                ViewBag.OrderDispacthed = OrderDispacthed;
                ViewBag.orderDelivered = orderDelivered;
                ViewBag.PendingPOApproval = PendingPOApproval;
                ViewBag.PODispacthed = PODispacthed;
                ViewBag.POReceived = POReceived;
                //ViewBag.Current_Rate = (new ProductRateController()).GetCurrentProductRateList().Count();
                ViewBag.Current_Rate = obj_RateMatrixController.GetCurrentProductRateList(WarehouseID).Count();//Added by Rumana 12/04/2019

                ViewBag.FromDate = _FromDate.ToString("dd/MM/yyyy");
                ViewBag.ToDate = _ToDate.ToString("dd/MM/yyyy");
                ViewBag.Filter = filter;
                ViewBag.f_hr = f_hr;
                ViewBag.f_min = f_min;
                ViewBag.t_hr = t_hr;
                ViewBag.t_min = t_min;
                ViewBag.f_tt = f_tt;
                ViewBag.t_tt = t_tt;

            }
            catch
            {
                ViewBag.CurrentStockValue = 0;
                ViewBag.ProductQty = 0;
                ViewBag.SKU = 0;
                ViewBag.WastageValue = 0;
                ViewBag.WastageSKU = 0;
                ViewBag.ReturnValue = 0;
                ViewBag.ReorderValue = 0;
                ViewBag.OrderPlaced = 0;
                ViewBag.OrderConfirmed = 0;
                ViewBag.OrderPacked = 0;
                ViewBag.OrderDispacthed = 0;
                ViewBag.orderDelivered = 0;
                ViewBag.PendingPOApproval = 0;
                ViewBag.PODispacthed = 0;
                ViewBag.POReceived = 0;
            }
            string msgOrdPlace = "";
            string msgOrdConf = "";
            string msgOrdPack = "";
            string msgOrdDis = "";
            string msgOrdDel = "";
            string msgPOPenApr = "";
            string msgPoDis = "";
            string msgPoRec = "";

            if ((int)Session["WarehouseLevel"] == 1) //Ezeelo 
            {
                msgOrdPlace = "Count of PO when PO is generated from DV within date Range.";
                msgOrdConf = "Count of PO(created by DV) when PO is accepted by Ezeelo within date Range.";
                msgOrdPack = "Count of Invoice(created by DV) when Invoice is created against PO by Ezeelo within date Range.";
                msgOrdDis = "Count of Invoice(created by DV) when Invoice is confirmed by Ezeelo within date Range.";
                msgPOPenApr = "";
                msgPoDis = "";
                msgPoRec = "";
            }
            else if ((int)Session["WarehouseLevel"] == 2) //DV
            {
                msgOrdPlace = "Count of PO when PO is generated from FV within date Range.";
                msgOrdConf = "Count of PO(created by FV) when PO is accepted by DV within date Range.";
                msgOrdPack = "Count of Invoice(created by FV) when Invoice is created against PO by DV within date Range.";
                msgOrdDis = "Count of Invoice(created by FV) when Invoice is confirmed by DV within date Range.";
                msgPOPenApr = "Count of PO is generated from DV but pending for approval from Ezeelo within date Range.";
                msgPoDis = "Count of Invoice(created by DV) PO is accepted and Invoice is confirmed by Ezeelo within date Range.";
                msgPoRec = "Count of Invoice which Order received And GRN create at DV within date Range.";
            }
            else if ((int)Session["WarehouseLevel"] == 3) //FV
            {
                msgOrdPlace = "";
                msgOrdConf = "";
                msgOrdPack = "";
                msgOrdDis = "";
                msgPOPenApr = "Count of PO is generated from FvV but pending for approval from DV within date Range.";
                msgPoDis = "Count of Invoice(created by FV) which is accepted and Invoice is confirmed by DV within date Range.";
                msgPoRec = "Count of Invoice which Order received And GRN create at FV within date Range.";
            }
            ViewBag.msgOrdPlace = msgOrdPlace;
            ViewBag.msgOrdConf = msgOrdConf;
            ViewBag.msgOrdPack = msgOrdPack;
            ViewBag.msgOrdDis = msgOrdDis;
            ViewBag.msgOrdDel = msgOrdDel;
            ViewBag.msgPOPenApr = msgPOPenApr;
            ViewBag.msgPoDis = msgPoDis;
            ViewBag.msgPoRec = msgPoRec;
            return View();
        }

        public void GetDashboardValue(DateTime frmDt, DateTime ToDt, out double CurrentStockValue, out long ProductQty, out long SKU
          , out double WastageValue, out long WastageSKU, out double ReturnValue, out double ReorderValue
          , out long OrderPlaced, out long OrderConfirmed, out long OrderPacked, out long OrderDispacthed
          , out long orderDelivered, out long PendingPOApproval, out long PODispacthed, out long POReceived, int? filter)
        {
            DateTime FromDate = frmDt;
            DateTime ToDate = ToDt;
            DateTime FromDateDflt = frmDt;
            DateTime ToDateDflt = ToDt;
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            //CurrentStockValue
            try
            {
                CurrentStockValue = Convert.ToDouble(db.WarehouseStocks.Where(p => p.WarehouseID == WarehouseID && p.CreateDate >= FromDate && p.CreateDate <= ToDate)
                  .Select(p => new
                  {
                      Value = p.AvailableQuantity * p.BuyRatePerUnit
                  }).Sum(p => p.Value).ToString());
            }
            catch
            {
                CurrentStockValue = 0;
            }

            //Product Qty
            try
            {
                ProductQty = db.WarehouseStocks.Where(p => p.WarehouseID == WarehouseID && p.CreateDate >= FromDate && p.CreateDate <= ToDate && p.AvailableQuantity > 0)
                    .Select(p => new
                    {
                        ProductId = p.ProductID
                    }).Distinct().Count();

            }
            catch
            {
                ProductQty = 0;
            }

            //Product Varient Qty
            try
            {
                SKU = db.WarehouseStocks.Where(p => p.WarehouseID == WarehouseID && p.CreateDate >= FromDate && p.CreateDate <= ToDate && p.AvailableQuantity > 0)
                    .Select(p => new
                    {
                        ProductId = p.ProductID,
                        ProductVarientId = p.ProductVarientID
                    }).Distinct().Count();
            }
            catch
            {
                SKU = 0;
            }

            //WastageValue/WastageSKU
            try
            {
                var WasteQuery = db.WarehouseWastageStock.Where(w => w.CreateDate >= FromDate && w.CreateDate <= ToDate && w.WastageQuantity != 0)
                       .Join(db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseID), w => w.WarehouseStockID, ws => ws.ID,
                       (w, ws) => new
                       {
                           value = w.WastageQuantity * ws.BuyRatePerUnit
                       });
                if (WasteQuery != null && WasteQuery.Count() != 0)
                {
                    WastageValue = (double)WasteQuery.Sum(p => p.value);
                }
                else
                {
                    WastageValue = 0;
                }

                WastageSKU = db.WarehouseWastageStock.Where(w => w.CreateDate >= FromDate && w.CreateDate <= ToDate && w.WastageQuantity != 0)
                       .Join(db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseID), w => w.WarehouseStockID, ws => ws.ID,
                       (w, ws) => new
                       {
                           SKU = ws.ID
                       }).Count();
            }
            catch
            {
                WastageValue = 0;
                WastageSKU = 0;
            }

            //Return Value
            try
            {
                if (filter == 7)
                {
                    //If default Condition
                    FromDateDflt = DateTime.Today.AddDays((int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)DateTime.Today.DayOfWeek);
                    ToDateDflt = FromDateDflt.AddDays(6);
                }
                else
                {
                    FromDateDflt = frmDt;
                    ToDateDflt = ToDt;
                }
                var ReturnQuery = db.WarehouseReturnStock.Where(wrs => wrs.CreateDate >= FromDateDflt && wrs.CreateDate <= ToDateDflt)
                     .Join(db.WarehouseReturnStockDetails, wrs => wrs.ID, wrsd => wrsd.WarehouseReturnStockId, (wrs, wrsd) => new { wrs, wrsd })
                     .Join(db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseID), p => p.wrsd.WarehouseStockId, ws => ws.ID, (p, ws) => new { p, ws })
                     .Select(o => new
                     {
                         value = o.p.wrsd.Quantity * o.p.wrsd.ReturnRatePerUnit
                     });

                if (ReturnQuery != null && ReturnQuery.Count() != 0)
                {
                    ReturnValue = (double)ReturnQuery.Sum(p => p.value);
                }
                else
                {
                    ReturnValue = 0;
                }
            }
            catch
            {
                ReturnValue = 0;
            }

            //Reorder Value
            try
            {
                ReorderValue = Convert.ToDouble(db.WarehouseReorderLevels.Where(p => p.WarehouseID == WarehouseID && p.AvailableQuantity <= p.ReorderLevel)
                      .Join(db.WarehouseStocks.Where(ws => ws.CreateDate >= FromDate && ws.CreateDate <= ToDate), wrl => new { WarehouseId = wrl.WarehouseID, ProductID = wrl.ProductID, ProductVarientID = wrl.ProductVarientID }
                      , ws => new { WarehouseId = ws.WarehouseID, ProductID = ws.ProductID, ProductVarientID = ws.ProductVarientID }, (wrl, ws) => new { wrl, ws })
                      .Select(p => new
                      {
                          value = p.wrl.ReorderLevel * p.ws.BuyRatePerUnit
                      }).Sum(p => p.value).ToString());
            }
            catch
            {
                ReorderValue = 0;
            }

            //For Seller [Ezeelo And Dv]
            //List<long> abc = null;
            //var POCreater = abc;
            //long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            //if (EzeeloWarehouseId == WarehouseID)
            //{
            //    POCreater = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.ID != EzeeloWarehouseId).Select(p => p.ID).ToList();
            //}
            //else
            //{
            //    Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
            //    if (obj.IsFulfillmentCenter == false)
            //    {
            //        POCreater = db.Warehouses.Where(p => p.IsFulfillmentCenter == true && p.DistributorId == WarehouseID).Select(p => p.ID).ToList();
            //    }
            //}
            if ((int)Session["WarehouseLevel"] == 1 || (int)Session["WarehouseLevel"] == 2)
            {

                long SupplierId = db.Suppliers.FirstOrDefault(p => p.WarehouseID == WarehouseID).ID;
                //Order Placed
                try
                {
                    OrderPlaced = db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 0 && p.IsSent == true && p.CreateDate >= FromDate && p.CreateDate <= ToDate).ToList().Count();
                }
                catch
                {
                    OrderPlaced = 0;
                }

                //Order Confirmed
                try
                {
                    OrderConfirmed = db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1 && p.CreateDate >= FromDate && p.CreateDate <= ToDate
                        && !(db.PurchaseOrderReply.Select(m => m.PurchaseOrderID).Contains(p.ID))
                        ).ToList().Count();
                }
                catch
                {
                    OrderConfirmed = 0;
                }

                //Order Packed
                try
                {
                    //Yashaswi 11-6-2018
                    // according to TKT -Pal  46 11-6-2018
                    // count  according to invoice suggest by  ashish  sir
                    //OrderPacked = db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1 && p.CreateDate >= FromDate && p.CreateDate <= ToDate)
                    //         .Join(db.PurchaseOrderReply.Where(por => por.IsReplied == false), p => p.ID, por => por.PurchaseOrderID, (p, por) => new { p, por })
                    //         .Count();
                    OrderPacked = db.PurchaseOrderReply.Where(por => por.IsReplied == false && por.CreateDate >= FromDate && por.CreateDate <= ToDate)
                        .Join(db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1), por => por.PurchaseOrderID, p => p.ID, (por, p) => new { por, p })
                        .ToList().Count();

                }
                catch
                {
                    OrderPacked = 0;
                }

                //Order Dispatched
                try
                {
                    //Yashaswi 11-6-2018
                    // according to TKT -Pal  46 11-6-2018
                    // count  according to invoice suggest by  ashish sir
                    //OrderDispacthed = db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1 && p.CreateDate >= FromDate && p.CreateDate <= ToDate
                    //    && !(db.Invoices.Select(i => i.PurchaseOrderID).Contains(p.ID)))
                    //         .Join(db.PurchaseOrderReply.Where(por => por.IsReplied == true), p => p.ID, por => por.PurchaseOrderID, (p, por) => new { p, por })
                    //         .Count();
                    OrderDispacthed = db.PurchaseOrderReply.Where(por => por.IsReplied == true && por.CreateDate >= FromDate && por.CreateDate <= ToDate && !(db.Invoices.Select(i => i.InvoiceCode).Contains(por.InvoiceCode)))
                        .Join(db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1
                        ), por => por.PurchaseOrderID, p => p.ID, (p, por) => new { p, por })
                        .Count();
                }
                catch
                {
                    OrderDispacthed = 0;
                }

                //Order Delivered
                try
                {
                    if (filter == 7)
                    {
                        //If default Condition
                        FromDateDflt = DateTime.Now.Date;
                        ToDateDflt = DateTime.Now.Date;
                    }
                    else
                    {
                        FromDateDflt = frmDt;
                        ToDateDflt = ToDt;
                    }
                    //Yashaswi 11-6-2018
                    // according to TKT -Pal  46 11-6-2018
                    // count  according to invoice suggest by  ashish sir
                    //orderDelivered = db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1)
                    //        .Join(db.Invoices.Where(p => p.CreateDate >= FromDate && p.CreateDate <= ToDate), po => po.ID, i => i.PurchaseOrderID, (po, i) => new { po, i })
                    //         .Count();
                    orderDelivered = db.Invoices.Where(i => i.CreateDate >= FromDateDflt && i.CreateDate <= ToDateDflt)
                        .Join(db.PurchaseOrders.Where(p => p.SupplierID == SupplierId && p.IsAcceptedBySupplier == 1),
                        i => i.PurchaseOrderID, po => po.ID, (i, po) => new { i, po })
                        .ToList().Count();

                }
                catch
                {
                    orderDelivered = 0;
                }
            }
            else
            {
                OrderPlaced = 0;
                OrderConfirmed = 0;
                OrderPacked = 0;
                OrderDispacthed = 0;
                orderDelivered = 0;
            }
            //For Buyer [DV and FV]
            //Pending PO Approval
            try
            {
                PendingPOApproval = db.PurchaseOrders.Where(p => p.WarehouseID == WarehouseID && p.IsAcceptedBySupplier == 0 && p.CreateDate >= FromDate && p.CreateDate <= ToDate).Count();
            }
            catch
            {
                PendingPOApproval = 0;
            }

            //PO Dispacth
            try
            {
                //Yashaswi 11-6-2018
                // according to TKT -Pal  46 11-6-2018
                // count  according to invoice suggest by  ashish sir
                //PODispacthed = db.PurchaseOrders.Where(p => p.WarehouseID == WarehouseID && p.IsAcceptedBySupplier == 1 && p.CreateDate >= FromDate && p.CreateDate <= ToDate
                //    && (db.PurchaseOrderReply.Where(por => por.IsReplied == true).Select(por => por.PurchaseOrderID).Contains(p.ID))
                //    && !(db.Invoices.Select(i => i.PurchaseOrderID).Contains(p.ID))
                //    )
                //    .Count();

                PODispacthed = db.PurchaseOrderReply.Where(por => por.IsReplied == true && por.CreateDate >= FromDate && por.CreateDate <= ToDate && !(db.Invoices.Select(i => i.InvoiceCode).Contains(por.InvoiceCode)))
                .Join(db.PurchaseOrders.Where(p => p.WarehouseID == WarehouseID && p.IsAcceptedBySupplier == 1), por => por.PurchaseOrderID, p => p.ID, (por, p) => new { por, p })
                .ToList().Count();

            }
            catch
            {
                PODispacthed = 0;
            }

            //PO Received
            try
            {
                if (filter == 7)
                {
                    //If default Condition
                    FromDateDflt = DateTime.Now.Date;
                    ToDateDflt = DateTime.Now.Date;
                }
                else
                {
                    FromDateDflt = frmDt;
                    ToDateDflt = ToDt;
                }
                //Yashaswi 11-6-2018
                // according to TKT -Pal  46 11-6-2018
                // count  according to invoice suggest by  ashish sir
                //POReceived = db.PurchaseOrders.Where(p => p.WarehouseID == WarehouseID && p.IsAcceptedBySupplier == 1 && p.CreateDate >= FromDate && p.CreateDate <= ToDate)
                //    .Join(db.Invoices.Where(p => p.CreateDate >= FromDate && p.CreateDate <= ToDate), po => po.ID, i => i.PurchaseOrderID, (po, i) => new { po, i })
                //    .Select(p => p.po.ID).Distinct()
                //    .Count();

                POReceived = db.Invoices.Where(i => i.CreateDate >= FromDateDflt && i.CreateDate <= ToDateDflt)
                    .Join(db.PurchaseOrders.Where(po => po.WarehouseID == WarehouseID && po.IsAcceptedBySupplier == 1), i => i.PurchaseOrderID, po => po.ID, (i, po) => new { i, po })
                    .ToList().Count();
            }
            catch
            {
                POReceived = 0;
            }

        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}