using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using BusinessLogicLayer.Account;
using ModelLayer.Models.ViewModel.Report.Account;
using CRM.Models;


using PagedList;
using PagedList.Mvc;

using System.IO;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Text;
using BusinessLogicLayer;


namespace CRM.Controllers
{
    public class ClaimAmountDvEzeeloReportController : Controller
    {

        string fConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EzeeloDBContext"].ConnectionString;
        private EzeeloDBContext db = new EzeeloDBContext();


        // GET: ClaimAmountDvEzeeloReport
        public ActionResult ClaimAmountTowardsDV(string FromDate, string ToDate, long? WarehouseId_, int? Flag)
        {
            List<ClaimAmountTowardsDVViewModel> list = new List<ClaimAmountTowardsDVViewModel>();
            try
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
                long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);

                Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
                //Added by Rumana 02 / 04 / 2019 
                if (FromDate == null)
                {
                    FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                }
                else
                {
                    FromDate = FromDate;
                }
                if (ToDate == null)
                {
                    ToDate = DateTime.Now.ToString("MM/dd/yyyy");
                }
                else
                {
                    ToDate = ToDate;
                }
                DateTime fDate = Convert.ToDateTime(FromDate);
                DateTime tDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddMinutes(59);
                //DateTime fDate = Convert.ToDateTime(FromDate).Date;
                //string from = fDate.ToString("dd/MM/yyyy");
                //string[] f = from.Split('/');
                //string[] ftime = f[2].Split(' ');
                // fDate = CommonFunctions.GetLocalTime(Convert.ToInt32(f[0]), Convert.ToInt32(f[1]), Convert.ToInt32(ftime[0]), 0, 0, 0);
                //fDate = Convert.ToDateTime(fDate.ToShortDateString());

                ////DateTime tDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
                //DateTime tDate = Convert.ToDateTime(ToDate).Date;
                ////tDate = tDate.Date;
                //string to = fDate.ToString("dd/MM/yyyy");
                //string[] t = to.Split('/');
                //string[] ttime = t[2].Split(' ');
                // tDate = CommonFunctions.GetLocalTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(ttime[0]), 0, 0, 0);
                //tDate = Convert.ToDateTime(tDate.ToShortDateString());
                //tDate = tDate.AddDays(1);
                //DateTime fDate1 = DateTime.ParseExact(FromDate, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                //DateTime tDate1 = DateTime.ParseExact(ToDate, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                //DateTime fDate = Convert.ToDateTime(FromDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                //DateTime tDate = Convert.ToDateTime(ToDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                //ViewBag.SelectedFromDate = fDate.Date.ToString("dd/MM/yyyy");
                //ViewBag.SelectedToDate = tDate.Date.ToString("dd/MM/yyyy");
                ViewBag.SelectedWarehouse = 0;
                DateTime ToDt = tDate;
                //Ended by Rumana 02 / 04 / 2019 
                if ((obj_warehouse.IsFulfillmentCenter && obj_warehouse.ID != EzeeloWarehouseId) || Flag == 2)
                {
                    //All FV
                    ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == true && p.IsActive == true).ToList();
                    Flag = 2;
                    list = db.CustomerOrderDetails.Where(cod => cod.OrderStatus == 7)
                      .Join(db.ShopStocks, cod => cod.ShopStockID, ss => ss.ID, (cod, ss) => new { cod, ss })
                      .Join(db.WarehouseStocks, codss => codss.ss.WarehouseStockID, ws => ws.ID, (codss, ws) => new { codss, ws })
                      .Select(p => new ClaimAmountTowardsDVViewModel
                      {
                          Batch = p.ws.BatchCode,
                          Qty = p.codss.cod.Qty,
                          ClaimAmount = (p.codss.cod.SaleRate - p.ws.SaleRatePerUnit) * p.codss.cod.Qty,
                          DecidedSalePrice = p.ws.SaleRatePerUnit,
                          DiversionInSalePrice = p.codss.cod.SaleRate,
                          ProductId = p.ws.ProductID,
                          ProductName = db.Products.FirstOrDefault(q => q.ID == p.ws.ProductID).Name,
                          VarientName = db.Sizes.FirstOrDefault(s => s.ID == db.ProductVarients.FirstOrDefault(v => v.ID == p.ws.ProductVarientID).SizeID).Name,
                          WarehouseId = p.ws.WarehouseID,
                          WarehouseName = db.Warehouses.FirstOrDefault(w => w.ID == p.ws.WarehouseID).Name,
                          POReplyDate = (DateTime)((p.codss.cod.ModifyDate == null) ? p.codss.cod.CreateDate : p.codss.cod.ModifyDate),
                          OrderCode = db.CustomerOrders.FirstOrDefault(co => co.ID == p.codss.cod.CustomerOrderID).OrderCode
                      }).ToList();
                    list = list.Where(p => p.POReplyDate >= fDate && p.POReplyDate <= ToDt).AsEnumerable().ToList();
                    if (Session["IsEzeeloLogin"].ToString() != "1" && Session["BusinessTypeID"].ToString() != "5")
                    {
                        foreach (var item in list)
                        {
                            item.ClaimAmount = item.ClaimAmount * -1;
                        }
                    }
                }
                else
                {
                    //All DV


                    //list = list.Where(p => p.POReplyDate.Month == Month).ToList();


                    ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true && p.ID != EzeeloWarehouseId).ToList();
                    Flag = 1;

                    list = db.WarehouseStockDeliveryDetails
                         .Join(db.WarehouseStocks, wsdd => wsdd.WarehouseStockID, ws => ws.ID,
                         (wsdd, ws) => new
                         {
                             BacthCode = ws.BatchCode,
                             ProductTd = ws.ProductID,
                             ProductName = db.Products.FirstOrDefault(p => p.ID == ws.ProductID).Name,
                             WarehouseId = ws.WarehouseID,
                             productVarientName = db.Sizes.FirstOrDefault(s => s.ID == db.ProductVarients.FirstOrDefault(pv => pv.ID == ws.ProductVarientID && pv.ProductID == ws.ProductID).SizeID).Name,
                             DecidedSalePrice = ws.SaleRatePerUnit,
                             PurchaseOrderReplyDetailId = wsdd.PurchaseOrderReplyDetailID,
                             Qty = wsdd.Quantity,
                             ClaimAmount = wsdd.ClaimAmountPerUnit,
                         })
                         .ToList()
                         .Join(db.PurchaseOrderReplyDetails, x => x.PurchaseOrderReplyDetailId, p => p.ID,
                         (x, p) => new
                         {
                             Batch = x.BacthCode,
                             ClaimAmount = (decimal)((x.ClaimAmount == null) ? 0 : x.ClaimAmount * x.Qty),
                             DecidedSalePrice = x.DecidedSalePrice,
                             DiversionInSalePrice = p.BuyRatePerUnit,
                             ProductId = x.ProductTd,
                             ProductName = x.ProductName,
                             Qty = x.Qty,
                             VarientName = x.productVarientName,
                             PurchaseOrderId = p.PurchaseOrderReplyID,
                             WarehouseId = x.WarehouseId
                         }).
                         ToList()
                         .Join(db.PurchaseOrderReply, y => y.PurchaseOrderId, por => por.ID,  //Date Range Added by Rumana 02 / 04 / 2019 
                                                                                              //.Join(db.PurchaseOrderReply.Where(p => p.CreateDate >= fDate && p.CreateDate <= ToDt).AsEnumerable().ToList(), y => y.PurchaseOrderId, por => por.ID,  //Date Range Added by Rumana 02 / 04 / 2019 */                                                                                                                                                   //.Join(db.PurchaseOrderReply.Where(p => p.CreateDate >= fDate && p.CreateDate <= ToDt).AsEnumerable().ToList(), y => y.PurchaseOrderId, por => por.ID,  //Date Range Added by Rumana 02 / 04 / 2019 
                         (y, por) => new ClaimAmountTowardsDVViewModel
                         {
                             Batch = y.Batch,
                             ClaimAmount = (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5") ? -1 * y.ClaimAmount : y.ClaimAmount,
                             DecidedSalePrice = y.DecidedSalePrice,
                             DiversionInSalePrice = y.DiversionInSalePrice,
                             ProductId = y.ProductId,
                             ProductName = y.ProductName,
                             Qty = y.Qty,
                             VarientName = y.VarientName,
                             WarehouseId = y.WarehouseId,
                             WarehouseName = db.Warehouses.FirstOrDefault(w => w.ID == y.WarehouseId).Name,
                             POReplyDate = (DateTime)((por.ModifyDate == null) ? por.CreateDate : por.ModifyDate),
                             OrderCode = db.PurchaseOrders.FirstOrDefault(po => po.ID == por.PurchaseOrderID).PurchaseOrderCode
                         }).ToList();
                    list = list.Where(p => p.POReplyDate >= fDate && p.POReplyDate <= ToDt).AsEnumerable().ToList();
                }
                ViewBag.Flag = Flag;
                //if (Month == 0 || Month == null)
                //{
                //    Month = DateTime.Now.Date.Month;
                //}
                if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
                {
                    //display all warehouse Or Selected
                    if (WarehouseId_ != null && WarehouseId_ != 0)
                    {
                        list = list.Where(p => p.WarehouseId == WarehouseId_).ToList();
                        ViewBag.SelectedWarehouse = WarehouseId_;
                    }
                    ViewBag.DV = "0";
                }
                else
                {
                    ViewBag.DV = "1";
                    list = list.Where(p => p.WarehouseId == WarehouseID).ToList();
                }

                list = list.Where(p => p.ClaimAmount != 0).ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
            return View(list);
        }
        //public ActionResult ClaimAmountTowardsDV(long? Month, long? WarehouseId_, int? Flag)
        //{
        //    //if (Session["USER_NAME"] == null)
        //    //{
        //    //    return RedirectToAction("Index", "Login");
        //    //}
        //    //long WarehouseID = 0;
        //    //if (Session["WarehouseID"] != null)
        //    //{
        //    //    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
        //    //}
        //    long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
        //    List<ClaimAmountTowardsDVViewModel> list = new List<ClaimAmountTowardsDVViewModel>();
        //    //Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);

        //    //if ((obj_warehouse.IsFulfillmentCenter && obj_warehouse.ID != EzeeloWarehouseId) || Flag == 2)
        //    {
        //        //All FV
        //        ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == true && p.IsActive == true).ToList();
        //        Flag = 2;
        //        list = db.CustomerOrderDetails.Where(cod => cod.OrderStatus == 7)
        //          .Join(db.ShopStocks, cod => cod.ShopStockID, ss => ss.ID, (cod, ss) => new { cod, ss })
        //          .Join(db.WarehouseStocks, codss => codss.ss.WarehouseStockID, ws => ws.ID, (codss, ws) => new { codss, ws })
        //          .Select(p => new ClaimAmountTowardsDVViewModel
        //          {
        //              Batch = p.ws.BatchCode,
        //              Qty = p.codss.cod.Qty,
        //              ClaimAmount = (p.codss.cod.SaleRate - p.ws.SaleRatePerUnit) * p.codss.cod.Qty,
        //              DecidedSalePrice = p.ws.SaleRatePerUnit,
        //              DiversionInSalePrice = p.codss.cod.SaleRate,
        //              ProductId = p.ws.ProductID,
        //              ProductName = db.Products.FirstOrDefault(q => q.ID == p.ws.ProductID).Name,
        //              VarientName = db.Sizes.FirstOrDefault(s => s.ID == db.ProductVarients.FirstOrDefault(v => v.ID == p.ws.ProductVarientID).SizeID).Name,
        //              WarehouseId = p.ws.WarehouseID,
        //              WarehouseName = db.Warehouses.FirstOrDefault(w => w.ID == p.ws.WarehouseID).Name,
        //              POReplyDate = (DateTime)((p.codss.cod.ModifyDate == null) ? p.codss.cod.CreateDate : p.codss.cod.ModifyDate),
        //              OrderCode = db.CustomerOrders.FirstOrDefault(co => co.ID == p.codss.cod.CustomerOrderID).OrderCode
        //          }).ToList();

        //        if (Month!= null)
        //        {
        //            foreach (var item in list)
        //            {
        //                item.ClaimAmount = item.ClaimAmount * -1;
        //            }
        //        }

        //        else
        //        {
        //            //All DV

        //            ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true && p.ID != EzeeloWarehouseId).ToList();
        //            Flag = 1;
        //            list = db.WarehouseStockDeliveryDetails
        //                 .Join(db.WarehouseStocks, wsdd => wsdd.WarehouseStockID, ws => ws.ID,
        //                 (wsdd, ws) => new
        //                 {
        //                     BacthCode = ws.BatchCode,
        //                     ProductTd = ws.ProductID,
        //                     ProductName = db.Products.FirstOrDefault(p => p.ID == ws.ProductID).Name,
        //                     WarehouseId = ws.WarehouseID,
        //                     productVarientName = db.Sizes.FirstOrDefault(s => s.ID == db.ProductVarients.FirstOrDefault(pv => pv.ID == ws.ProductVarientID && pv.ProductID == ws.ProductID).SizeID).Name,
        //                     DecidedSalePrice = ws.SaleRatePerUnit,
        //                     PurchaseOrderReplyDetailId = wsdd.PurchaseOrderReplyDetailID,
        //                     Qty = wsdd.Quantity,
        //                     ClaimAmount = wsdd.ClaimAmountPerUnit,
        //                 })
        //                 .ToList()
        //                 .Join(db.PurchaseOrderReplyDetails, x => x.PurchaseOrderReplyDetailId, p => p.ID,
        //                 (x, p) => new
        //                 {
        //                     Batch = x.BacthCode,
        //                     ClaimAmount = (decimal)((x.ClaimAmount == null) ? 0 : x.ClaimAmount * x.Qty),
        //                     DecidedSalePrice = x.DecidedSalePrice,
        //                     DiversionInSalePrice = p.BuyRatePerUnit,
        //                     ProductId = x.ProductTd,
        //                     ProductName = x.ProductName,
        //                     Qty = x.Qty,
        //                     VarientName = x.productVarientName,
        //                     PurchaseOrderId = p.PurchaseOrderReplyID,
        //                     WarehouseId = x.WarehouseId
        //                 }).
        //                 ToList()
        //                 .Join(db.PurchaseOrderReply, y => y.PurchaseOrderId, por => por.ID,
        //                 (y, por) => new ClaimAmountTowardsDVViewModel
        //                 {
        //                     Batch = y.Batch,
        //                     ClaimAmount = (y.ClaimAmount!=null) ? -1 * y.ClaimAmount : y.ClaimAmount,
        //                     DecidedSalePrice = y.DecidedSalePrice,
        //                     DiversionInSalePrice = y.DiversionInSalePrice,
        //                     ProductId = y.ProductId,
        //                     ProductName = y.ProductName,
        //                     Qty = y.Qty,
        //                     VarientName = y.VarientName,
        //                     WarehouseId = y.WarehouseId,
        //                     WarehouseName = db.Warehouses.FirstOrDefault(w => w.ID == y.WarehouseId).Name,
        //                     POReplyDate = (DateTime)((por.ModifyDate == null) ? por.CreateDate : por.ModifyDate),
        //                     OrderCode = db.PurchaseOrders.FirstOrDefault(po => po.ID == por.PurchaseOrderID).PurchaseOrderCode
        //                 }).ToList();

        //        }
        //        ViewBag.Flag = Flag;
        //        if (Month == 0 || Month == null)
        //        {
        //            Month = DateTime.Now.Date.Month;
        //        }
        //        ViewBag.SelectedMonth = Month;
        //        ViewBag.SelectedWarehouse = 0;
        //        list = list.Where(p => p.POReplyDate.Month == Month).ToList();


        //        if (Month != null)
        //        { 
        //            //display all warehouse Or Selected
        //            if (WarehouseId_ != null && WarehouseId_ != 0)
        //            {
        //                list = list.Where(p => p.WarehouseId == WarehouseId_).ToList();
        //                ViewBag.SelectedWarehouse = WarehouseId_;
        //            }
        //        ViewBag.DV = "0";
        //    }
        //        else
        //        {
        //            ViewBag.DV = "1";
        //            //list = list.Where(p => p.WarehouseId == WarehouseID).ToList();
        //        }

        //        list = list.Where(p => p.ClaimAmount != 0).ToList();
        //        return View(list);
        //    }
        //}
    }
}