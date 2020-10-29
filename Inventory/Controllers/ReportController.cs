using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using System.Web.Configuration;
using Inventory.Common;
using ModelLayer.Models.Enum;
using System.IO;
using System.Web.UI;
using BusinessLogicLayer;
using System.Web.UI.WebControls;
using PagedList;
using PagedList.Mvc;
using System.Globalization;

namespace Inventory.Controllers
{
    public class ReportController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        ProductRateController PRate = new ProductRateController();
        Margin_DivisionConstants objMagDiv = new Margin_DivisionConstants(); //Yashaswi 24/4/2018
        public ActionResult DVFVRateMarginList(long ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
            {

                List<ProductRateMarginListViewModel> ProductList = GetListOfDVFVRateMargin(ProductId, null);
                ViewBag.ProductId = ProductId;
                ViewBag.LEADERSHIP = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                ViewBag.EZEELO = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                ViewBag.LEADERS_ROYALTY = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                ViewBag.LIFESTYLE_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                ViewBag.LEADERSHIP_DEVELOPMENT_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);

                //21-6-2018
                ViewBag.LEADERSHIP_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                ViewBag.EZEELO_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                ViewBag.LEADERS_ROYALTY_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                ViewBag.LIFESTYLE_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                ViewBag.LEADERSHIP_DEVELOPMENT_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);
                //End

                ViewBag.Paise = objMagDiv.BPInPaise();
                return View(ProductList);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CurrentRateForWarehouse(long ProductId)
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
            Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseID);
            List<ProductRateMarginListViewModel> ProductList = new List<ProductRateMarginListViewModel>();
            if (objWarehouse != null)
            {
                DateTime CurDate = DateTime.Now.Date;
                ProductRateController Prate = new ProductRateController();
                if (objWarehouse.IsFulfillmentCenter)
                {
                    ViewBag.IsFV = "1";
                    //For FV
                    ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                              .Select(p => new ProductRateMarginListViewModel
                                              {
                                                  HSNCode = p.HSNCode,
                                                  ProductId = p.ID,
                                                  ProductName = p.Name,
                                                  PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                                  .Join(db.RateCalculations.Where(r => r.RateExpiry >= CurDate && r.IsActive == true), v => v.ID, r => r.ProductVarientId,
                                                  (v, r) => new ProductVarientRateMarginListViewModel
                                                  {
                                                      ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                                      ProductVarientId = v.ID,
                                                      RateExpiry = r.RateExpiry,
                                                      MRP = r.MRP,
                                                      GST = r.GSTInPer,
                                                      SalePrice = r.DecidedSalePrice,
                                                      //PurchasePrice = Prate.GetRateForFV(WarehouseID, v.ProductID, v.ID, false)
                                                  }).ToList()
                                              }).ToList();

                    foreach (var Product in ProductList)
                    {
                        foreach (var Varient in Product.PVarientList)
                        {
                            Varient.SalePrice = Math.Round(Varient.SalePrice, 2);
                            Varient.PurchasePrice = Math.Round(Prate.GetRateForFV(WarehouseID, Product.ProductId, Varient.ProductVarientId, false), 2);
                        }
                    }

                }
                else
                {
                    //For DV 
                    ViewBag.IsFV = "0";
                    ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                               .Select(p => new ProductRateMarginListViewModel
                                               {
                                                   HSNCode = p.HSNCode,
                                                   ProductId = p.ID,
                                                   ProductName = p.Name,
                                                   PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                                   .Join(db.RateCalculations.Where(r => r.RateExpiry >= CurDate && r.IsActive == true), v => v.ID, r => r.ProductVarientId,
                                                   (v, r) => new ProductVarientRateMarginListViewModel
                                                   {
                                                       ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                                       ProductVarientId = v.ID,
                                                       RateExpiry = r.RateExpiry,
                                                       MRP = r.MRP,
                                                       GST = r.GSTInPer,
                                                       //PurchasePrice = r.BaseInwardPriceEzeelo,
                                                       DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID == WarehouseID)
                                                                .Select(w => new DVRateMarginListViewModel
                                                                {
                                                                    //Select All DV
                                                                    Id = w.ID,
                                                                    Name = w.Name,
                                                                    IsFV = w.IsFulfillmentCenter,
                                                                    FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true)
                                                                             .Select(f => new FVRateMarginListViewModel
                                                                             {
                                                                                 //Select All FV under this Dv
                                                                                 Id = f.ID,
                                                                                 Name = f.Name,
                                                                                 IsFV = true
                                                                             }).ToList()
                                                                }).ToList()
                                                   }).ToList()
                                               }).ToList();


                    ProductList = ProductList.Distinct().ToList();

                    foreach (var Product in ProductList)
                    {
                        foreach (var Varient in Product.PVarientList)
                        {
                            foreach (var DV in Varient.DVList)
                            {
                                DV.Margin = (double)objWarehouse.Margin;
                                DV.PurchasePrice = Math.Round(Prate.GetRateForDV(DV.Id, Product.ProductId, Varient.ProductVarientId, false), 2);//DV PurchasePrice According to fixed 6%
                                DV.SalePrice = Math.Round(Prate.GetRateForDV(DV.Id, Product.ProductId, Varient.ProductVarientId, true), 2);//DV SalePrice According to fixed 6%
                                foreach (var FV in DV.FVList)
                                {
                                    Warehouse objFV = db.Warehouses.FirstOrDefault(f => f.ID == FV.Id);
                                    if (objFV != null)
                                    {
                                        double margin = (double)(objFV.Margin != null ? objFV.Margin : 0);
                                        FV.FVPurchasePrice = Math.Round(Prate.GetRateForFV(FV.Id, Product.ProductId, Varient.ProductVarientId, false), 2); //dv sale price according to fv margin%
                                        FV.DVPurchasePrice = Math.Round((FV.FVPurchasePrice / (1 + DV.Margin / 100)), 2);
                                        FV.FVMargin = Math.Round(margin, 2);
                                        FV.DVClaimAmt = Math.Round((FV.FVPurchasePrice - DV.SalePrice), 2) * -1;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return View(ProductList);
        }

        //public ActionResult ClaimAmountTowardsDV(long? Month, long? WarehouseId_, int? Flag)
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
        //    if (Session["USER_NAME"] == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    long WarehouseID = 0;
        //    if (Session["WarehouseID"] != null)
        //    {
        //        WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
        //    }
        //    long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
        //    List<ClaimAmountTowardsDVViewModel> list = new List<ClaimAmountTowardsDVViewModel>();
        //    Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);

        //    if ((obj_warehouse.IsFulfillmentCenter && obj_warehouse.ID != EzeeloWarehouseId) || Flag == 2)
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

        //        if (Session["IsEzeeloLogin"].ToString() != "1" && Session["BusinessTypeID"].ToString() != "5")
        //        {
        //            foreach (var item in list)
        //            {
        //                item.ClaimAmount = item.ClaimAmount * -1;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //All DV

        //        ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == false && p.IsActive == true && p.ID != EzeeloWarehouseId).ToList();
        //        Flag = 1;
        //        list = db.WarehouseStockDeliveryDetails
        //             .Join(db.WarehouseStocks, wsdd => wsdd.WarehouseStockID, ws => ws.ID,
        //             (wsdd, ws) => new
        //             {
        //                 BacthCode = ws.BatchCode,
        //                 ProductTd = ws.ProductID,
        //                 ProductName = db.Products.FirstOrDefault(p => p.ID == ws.ProductID).Name,
        //                 WarehouseId = ws.WarehouseID,
        //                 productVarientName = db.Sizes.FirstOrDefault(s => s.ID == db.ProductVarients.FirstOrDefault(pv => pv.ID == ws.ProductVarientID && pv.ProductID == ws.ProductID).SizeID).Name,
        //                 DecidedSalePrice = ws.SaleRatePerUnit,
        //                 PurchaseOrderReplyDetailId = wsdd.PurchaseOrderReplyDetailID,
        //                 Qty = wsdd.Quantity,
        //                 ClaimAmount = wsdd.ClaimAmountPerUnit,
        //             })
        //             .ToList()
        //             .Join(db.PurchaseOrderReplyDetails, x => x.PurchaseOrderReplyDetailId, p => p.ID,
        //             (x, p) => new
        //             {
        //                 Batch = x.BacthCode,
        //                 ClaimAmount = (decimal)((x.ClaimAmount == null) ? 0 : x.ClaimAmount * x.Qty),
        //                 DecidedSalePrice = x.DecidedSalePrice,
        //                 DiversionInSalePrice = p.BuyRatePerUnit,
        //                 ProductId = x.ProductTd,
        //                 ProductName = x.ProductName,
        //                 Qty = x.Qty,
        //                 VarientName = x.productVarientName,
        //                 PurchaseOrderId = p.PurchaseOrderReplyID,
        //                 WarehouseId = x.WarehouseId
        //             }).
        //             ToList()
        //             .Join(db.PurchaseOrderReply, y => y.PurchaseOrderId, por => por.ID,
        //             (y, por) => new ClaimAmountTowardsDVViewModel
        //             {
        //                 Batch = y.Batch,
        //                 ClaimAmount = (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5") ? -1 * y.ClaimAmount : y.ClaimAmount,
        //                 DecidedSalePrice = y.DecidedSalePrice,
        //                 DiversionInSalePrice = y.DiversionInSalePrice,
        //                 ProductId = y.ProductId,
        //                 ProductName = y.ProductName,
        //                 Qty = y.Qty,
        //                 VarientName = y.VarientName,
        //                 WarehouseId = y.WarehouseId,
        //                 WarehouseName = db.Warehouses.FirstOrDefault(w => w.ID == y.WarehouseId).Name,
        //                 POReplyDate = (DateTime)((por.ModifyDate == null) ? por.CreateDate : por.ModifyDate),
        //                 OrderCode = db.PurchaseOrders.FirstOrDefault(po => po.ID == por.PurchaseOrderID).PurchaseOrderCode
        //             }).ToList();

        //    }
        //    ViewBag.Flag = Flag;
        //    if (Month == 0 || Month == null)
        //    {
        //        Month = DateTime.Now.Date.Month;
        //    }
        //    ViewBag.SelectedMonth = Month;
        //    ViewBag.SelectedWarehouse = 0;
        //    list = list.Where(p => p.POReplyDate.Month == Month).ToList();

        //    if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
        //    {
        //        //display all warehouse Or Selected
        //        if (WarehouseId_ != null && WarehouseId_ != 0)
        //        {
        //            list = list.Where(p => p.WarehouseId == WarehouseId_).ToList();
        //            ViewBag.SelectedWarehouse = WarehouseId_;
        //        }
        //        ViewBag.DV = "0";
        //    }
        //    else
        //    {
        //        ViewBag.DV = "1";
        //        list = list.Where(p => p.WarehouseId == WarehouseID).ToList();
        //    }

        //    list = list.Where(p => p.ClaimAmount != 0).ToList();
        //    return View(list);
        //}

        public List<ProductRateMarginListViewModel> GetListOfDVFVRateMargin(long ProductId, long? WarehouseId)
        {
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);

            DateTime CurDate = DateTime.Now.Date;
            List<ProductRateMarginListViewModel> ProductList = new List<ProductRateMarginListViewModel>();
            if (WarehouseId == null)
            {
                ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                           .Select(p => new ProductRateMarginListViewModel
                                           {
                                               HSNCode = p.HSNCode,
                                               ProductId = p.ID,
                                               ProductName = p.Name,
                                               PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                               .Join(db.RateCalculations.Where(r => r.RateExpiry >= CurDate && r.IsActive == true), v => v.ID, r => r.ProductVarientId,
                                               (v, r) => new ProductVarientRateMarginListViewModel
                                               {
                                                   ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                                   ProductVarientId = v.ID,
                                                   RateExpiry = r.RateExpiry,
                                                   MRP = r.MRP,
                                                   GST = r.GSTInPer,
                                                   PurchasePrice = r.BaseInwardPriceEzeelo,
                                                   GrossMarginFlat = r.GrossMarginFlat,
                                                   ValuePostGST = r.ValuePostGST,
                                                   MarginPassedToCustomer = r.MarginPassedToCustomer, //Yashaswi 6/6/2018
                                                   MaxInwardMargin = r.MaxInwardMargin, //Yashaswi 6/6/2018
                                                   ActualFlatMargin = r.ActualFlatMargin, //Yashaswi 21/6/2018
                                                   DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID != EzeeloWarehouseId)
                                                            .Select(w => new DVRateMarginListViewModel
                                                            {
                                                                //Select All DV
                                                                Id = w.ID,
                                                                Name = w.Name,
                                                                IsFV = w.IsFulfillmentCenter,
                                                                FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true)
                                                                         .Select(f => new FVRateMarginListViewModel
                                                                         {
                                                                             //Select All FV under this Dv
                                                                             Id = f.ID,
                                                                             Name = f.Name,
                                                                             IsFV = true
                                                                         }).ToList()
                                                            }).ToList()
                                               }).ToList()
                                           }).ToList();
            }
            else
            {
                ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                          .Select(p => new ProductRateMarginListViewModel
                                          {
                                              HSNCode = p.HSNCode,
                                              ProductId = p.ID,
                                              ProductName = p.Name,
                                              PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                              .Join(db.RateCalculations.Where(r => r.RateExpiry >= CurDate), v => v.ID, r => r.ProductVarientId,
                                              (v, r) => new ProductVarientRateMarginListViewModel
                                              {
                                                  ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                                  ProductVarientId = v.ID,
                                                  RateExpiry = r.RateExpiry,
                                                  MRP = r.MRP,
                                                  GST = r.GSTInPer,
                                                  PurchasePrice = r.BaseInwardPriceEzeelo,
                                                  GrossMarginFlat = r.GrossMarginFlat,
                                                  ValuePostGST = r.ValuePostGST,
                                                  MarginPassedToCustomer = r.MarginPassedToCustomer, //Yashaswi 6/6/2018
                                                  MaxInwardMargin = r.MaxInwardMargin, //Yashaswi 6/6/2018
                                                  ActualFlatMargin = r.ActualFlatMargin, //Yashaswi 21/6/2018
                                                  DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID != EzeeloWarehouseId && w.ID == WarehouseId)
                                                           .Select(w => new DVRateMarginListViewModel
                                                           {
                                                               //Select All DV
                                                               Id = w.ID,
                                                               Name = w.Name,
                                                               IsFV = w.IsFulfillmentCenter,
                                                               FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true)
                                                                        .Select(f => new FVRateMarginListViewModel
                                                                        {
                                                                            //Select All FV under this Dv
                                                                            Id = f.ID,
                                                                            Name = f.Name,
                                                                            IsFV = true
                                                                        }).ToList()
                                                           }).ToList()
                                              }).ToList()
                                          }).ToList();
            }
            ProductList = ProductList.Distinct().ToList();

            ProductRateController Prate = new ProductRateController();
            foreach (var Product in ProductList)
            {
                foreach (var Varient in Product.PVarientList)
                {
                    foreach (var DV in Varient.DVList)
                    {
                        foreach (var FV in DV.FVList)
                        {
                            FVRateMarginListViewModel obj = Prate.GetRateMarginForFV(FV.Id, Product.ProductId, Varient.ProductVarientId);
                            FV.DVGST = Math.Round(obj.DVGST, 2);
                            FV.DVId = obj.DVId;
                            FV.DVMargin = Math.Round(obj.DVMargin, 2);
                            FV.DVMarginVAlueWithGST = Math.Round(obj.DVMarginVAlueWithGST, 2);
                            FV.DVPurchasePrice = Math.Round(obj.DVPurchasePrice, 4);
                            FV.DVSalePrice = Math.Round(obj.DVSalePrice, 4);

                            FV.FVGST = Math.Round(obj.FVGST, 2);
                            FV.FVMargin = Math.Round(obj.FVMargin, 2);
                            FV.FVMarginVAlueWithGST = Math.Round(obj.FVMarginVAlueWithGST, 2);
                            FV.FVPurchasePrice = Math.Round(obj.FVPurchasePrice, 4);
                            FV.FVSalePrice = Math.Round(obj.FVSalePrice, 4);
                            FV.IsFV = obj.IsFV;

                            FV.EzeeloMargin = Math.Round(obj.EzeeloMargin, 4);
                            FV.EzeeloGST = Math.Round(obj.EzeeloGST, 2);
                            FV.PostGSTMargin = Math.Round(obj.PostGSTMargin, 2);
                            FV.ForLeadership = Math.Round(obj.ForLeadership, 4);
                            FV.ForEzeelo = Math.Round(obj.ForEzeelo, 4);
                            FV.ForLeadersRoyalty = Math.Round(obj.ForLeadersRoyalty, 4);
                            FV.ForLifestyle = Math.Round(obj.ForLifestyle, 4);
                            FV.ForLeadershipDevelopment = Math.Round(obj.ForLeadershipDevelopment, 4);
                            FV.BussinessPoints = obj.BussinessPoints;
                            FV.TotalGST = Math.Round(obj.TotalGST, 2);
                            FV.TotalMargin = Math.Round(obj.TotalMargin, 2); //Yashaswi 6/6/2018
                        }
                    }
                }
            }
            return ProductList;
        }

        #region GST REPORT Yashaswi 21/5/2018

        //For FV And DV
        static bool? isFV_ = null;
        static int? year_ = null;
        static int? Month_ = null, SearchCat_ = null;
        static long? WarehouseId_ = null;
        static string searchTxt_ = null, frmDate_ = null, toDate_ = null;

        public ActionResult OutputGSTReport(bool? isFV, int? page, int? Month, int?year, long? WarehouseId, int? SearchCat, string searchTxt, string frmDate, string toDate)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (isFV == null)
            {
                WarehouseId = EzeeloWarehouseId;
            }
            searchTxt = searchTxt ?? "";
            frmDate = frmDate ?? "";
            toDate = toDate ?? "";
            if ((WarehouseId != null && page != null && Month != null ) && (WarehouseId == WarehouseId_ && Month == Month_ && year==year_&& isFV == isFV_ && SearchCat_ == SearchCat && searchTxt_ == searchTxt && frmDate_ == frmDate && toDate_ == toDate))
            {
                ViewBag.IsPaged = "1";
            }
            else
            {
                ViewBag.IsPaged = "0";
                Session["OutputGSTReportlist"] = null;
            }
            if (Month == null)
            {
                Month = DateTime.Now.Month;
            }
            //if (year == null)
            //{
            //    year = year; ;
            //}

            isFV_ = isFV;
            WarehouseId_ = WarehouseId;
            Month_ = Month;
            year_ = year;
            SearchCat_ = SearchCat;
            searchTxt_ = searchTxt;
            frmDate_ = frmDate;
            toDate_ = toDate;

            ViewBag.Month = Month;
            ViewBag.isFV = isFV;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.page = page;
            ViewBag.SearchCat = SearchCat;
            ViewBag.searchTxt = searchTxt;
            ViewBag.frmDate = frmDate;
            ViewBag.toDate = toDate;

            ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == isFV && p.IsActive == true && p.ID != EzeeloWarehouseId).ToList();
            return View();
        }

        public ActionResult InputGSTReport(bool? isFV, int? page, int? Month,int? year, long? WarehouseId, int? SearchCat, string searchTxt, string frmDate, string toDate)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            searchTxt = searchTxt ?? "";
            frmDate = frmDate ?? "";
            toDate = toDate ?? "";
            if (isFV == null)
            {
                WarehouseId = EzeeloWarehouseId;
            }
            if ((WarehouseId != null && page != null && Month != null) && (WarehouseId == WarehouseId_ && Month == Month_ &&  year==year_  && isFV == isFV_ && SearchCat_ == SearchCat && searchTxt_ == searchTxt && frmDate_ == frmDate && toDate_ == toDate))
            {
                ViewBag.IsPaged = "1";
            }
            else
            {
                ViewBag.IsPaged = "0";
                Session["InputGSTReportlist"] = null;
            }
            //if (Month == null)
            //{
            //    Month = DateTime.Now.Month;
            //}

            isFV_ = isFV;
            WarehouseId_ = WarehouseId;
            Month_ = Month;
            year_ = year;
            SearchCat_ = SearchCat;
            searchTxt_ = searchTxt;
            frmDate_ = frmDate;
            toDate_ = toDate;
            ViewBag.year_ = year;
            ViewBag.Month = Month;
            ViewBag.isFV = isFV;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.page = page;
            ViewBag.SearchCat = SearchCat;
            ViewBag.searchTxt = searchTxt;
            ViewBag.frmDate = frmDate;
            ViewBag.toDate = toDate;

            ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == isFV && p.IsActive == true && p.ID != EzeeloWarehouseId).ToList();
            return View();
        }

        public ActionResult GSTReport(bool? isFV, int? page, int?  Month, int?  year, long? WarehouseId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (isFV == null)
            {
                WarehouseId = EzeeloWarehouseId;
            }
            if ((WarehouseId != null && page != null && Month != null) && (WarehouseId == WarehouseId_ && Month == Month_ && isFV == isFV_))
            {
                ViewBag.IsPaged = "1";
            }
            else
            {
                ViewBag.IsPaged = "0";
                Session["GSTReportlist"] = null;
            }
            if (Month == null)
            {
                Month = DateTime.Now.Month;
            }

            if (year == null)
            {
               year = DateTime.Now.Year;
            }

            isFV_ = isFV;
            WarehouseId_ = WarehouseId;
            Month_ = Month;
            year_ = year;
            ViewBag.Month = Month;
            ViewBag.isFV = isFV;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.page = page;

            ViewBag.PossibleWarehouse = db.Warehouses.Where(p => p.IsFulfillmentCenter == isFV && p.IsActive == true && p.ID != EzeeloWarehouseId).ToList();
            return View();
        }

        private const int pageSize = 10;
        public PartialViewResult GetOutputGSTReportList(int? page, bool? isFV, int Month, int year, long? WarehouseId, int? SearchCat, string searchTxt, string frmDate, string toDate)
        {
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (isFV == null)
            {
                WarehouseId = EzeeloWarehouseId;
            }
            if (WarehouseId == WarehouseId_ && Month == Month_ &&  isFV == isFV_ && SearchCat_ == SearchCat && searchTxt_ == searchTxt && frmDate_ == frmDate && toDate_ == toDate)
            {

            }
            else
            {
                Session["OutputGSTReportlist"] = null;
                page = 1;
            }
            isFV_ = isFV;
            WarehouseId_ = WarehouseId;
            Month_ = Month;
            year_= year;
            SearchCat_ = SearchCat;
            searchTxt_ = searchTxt;
            frmDate_ = frmDate;
            toDate_ = toDate;


            ViewBag.isFV = isFV;
            ViewBag.Month = Month;
            ViewBag.year = year;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.SearchCat = SearchCat;
            ViewBag.searchTxt = searchTxt;
            ViewBag.frmDate = frmDate;
            ViewBag.toDate = toDate;

            List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
            int pageNumber = (page ?? 1);

            if (Session["OutputGSTReportlist"] == null)
            {
                //Show purchase Item's GST value
                //Ezeelo --> Vendor
                //DV --> Ezeelo
                //FV --> DV
                list = GetOutputGST((long)WarehouseId, (int)Month, (int)year,SearchCat, searchTxt, frmDate, toDate);
                Session["OutputGSTReportlist"] = list;
            }
            else
            {
                list = (List<InputOutputGSTReport>)Session["OutputGSTReportlist"];
            }

            var model = list.ToPagedList(pageNumber, pageSize);
            return PartialView("_outputGST", model);
        }
        public PartialViewResult GetInputGSTReportList(int? page, bool? isFV, int Month, int year,long? WarehouseId, int? SearchCat, string searchTxt, string frmDate, string toDate)
        {
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (isFV == null)
            {
                WarehouseId = EzeeloWarehouseId;
            }
            if (WarehouseId == WarehouseId_ && Month == Month_ && isFV == isFV_ && SearchCat_ == SearchCat && searchTxt_ == searchTxt && frmDate_ == frmDate && toDate_ == toDate)
            {

            }
            else
            {
                Session["InputGSTReportlist"] = null;
                page = 1;
            }
            //DateTime Bd = DateTime.Now.AddDays(-1);
            //DateTime Td = DateTime.Now.AddDays(1);
            isFV_ = isFV;
            WarehouseId_ = WarehouseId;
            Month_ = Month;
           year_ = year;
            SearchCat_ = SearchCat;
            searchTxt_ = searchTxt;
            frmDate_ = frmDate;
            toDate_ = toDate;

            ViewBag.isFV = isFV;
            ViewBag.Month = Month;
            ViewBag.year = year;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.SearchCat = SearchCat;
            ViewBag.searchTxt = searchTxt;
            ViewBag.frmDate = frmDate;
            ViewBag.toDate = toDate;

            List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
            int pageNumber = (page ?? 1);

            if (Session["InputGSTReportlist"] == null)
            {
                //Show Sale Item's GST value
                //Ezeelo --> DV
                //DV --> FV
                //FV --> Customer
                bool IsReportForFV = (isFV == null) ? false : (Convert.ToBoolean(isFV) == true ? true : false);
                list = GetInputGST((long)WarehouseId, (int)Month,(int)year, IsReportForFV, SearchCat, searchTxt, frmDate, toDate);
                Session["InputGSTReportlist"] = list;
            }
            else
            {
                list = (List<InputOutputGSTReport>)Session["InputGSTReportlist"];
            }

            var model = list.ToPagedList(pageNumber, pageSize);
            return PartialView("_InputGST", model);
        }
        public PartialViewResult GetGSTReportList(int? page, bool? isFV, int Month,int year, long? WarehouseId)
        {
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            if (isFV == null)
            {
                WarehouseId = EzeeloWarehouseId;
            }
            if (WarehouseId == WarehouseId_ && Month == Month_ && year ==year_ && isFV == isFV_)
            {

            }
            else
            {
                Session["GSTReportlist"] = null;
            }
            isFV_ = isFV;
            WarehouseId_ = WarehouseId;
            Month_ = Month;
            year_= year;
            ViewBag.isFV = isFV;
            ViewBag.Month = Month;
            ViewBag.year = year;
            ViewBag.WarehouseId = WarehouseId;

            List<GSTReport> list = new List<GSTReport>();
            int pageNumber = (page ?? 1);

            if (Session["GSTReportlist"] == null)
            {
                bool IsReportForFV = (isFV == null) ? false : (Convert.ToBoolean(isFV) == true ? true : false);
                list = GetGSTBalnce((long)WarehouseId, (int)Month,(int)year, IsReportForFV);
                Session["GSTReportlist"] = list;
            }
            else
            {
                list = (List<GSTReport>)Session["GSTReportlist"];
            }
            Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
            ViewBag.GSTNO = "GST Number : " + obj.GSTNumber;
            //ViewBag.Year = "Year : " + DateTime.Now.Year;
            ViewBag.Year = "Year : " + year;
            ViewBag.MonthName = "Month : " + new DateTime(2015, Month, 1).ToString("MMMM");
            ViewBag.Heading = "Monthly GST Report For " + obj.Name;
            ViewBag.Total = string.Format("{0:0.00}", list.Sum(p => p.Balance));
            ViewBag.InputTotal = string.Format("{0:0.00}", list.Sum(p => p.InputGSTAmount));
            ViewBag.OutputTotal = string.Format("{0:0.00}", list.Sum(p => p.OutputGSTAmount));
            var model = list.ToPagedList(pageNumber, pageSize);
            return PartialView("_GSTReport", model);
        }
        public List<InputOutputGSTReport> GetOutputGST(long WarehouseId, int Month,  int year, int? SearchCat, string searchTxt, string frmDate, string toDate)
        {
            //int Year = DateTime.Now.Year;
            int Year1 = year;
            List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
            IEnumerable<Invoice> InvoiceList;
            if (!string.IsNullOrEmpty(frmDate) && !string.IsNullOrEmpty(toDate) && SearchCat == 1)
            {
                DateTime FromDate_ = Convert.ToDateTime(frmDate);
                DateTime ToDate_ = Convert.ToDateTime(toDate).AddHours(23);
                InvoiceList = db.Invoices.Where(p => p.CreateDate > FromDate_ && p.CreateDate <= ToDate_);
            }
            else
            {
                if (SearchCat == 2)
                {
                    InvoiceList = db.Invoices.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year1 && p.InvoiceCode.Contains(searchTxt));
                }
                else
                {
                    InvoiceList = db.Invoices.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year1);
                }
            }


            list = db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseId)
                   .Join(db.InvoiceDetails
                   , ws => new { InvoiceID = ws.InvoiceID, ProductID = ws.ProductID, ProductVarientID = ws.ProductVarientID }
                   , id => new { InvoiceID = id.InvoiceID, ProductID = id.ProductID, ProductVarientID = id.ProductVarientID }
                   , (ws, id) => new { ws, id })
                   .Join((IEnumerable<Invoice>)InvoiceList, wsid => wsid.ws.InvoiceID, i => i.ID, (wsid, i) => new { wsid, i })
                   .Select(o => new InputOutputGSTReport
                   {
                       Date = o.i.CreateDate,
                       Invoice_No = o.i.InvoiceCode,
                       GST_No = db.Suppliers.FirstOrDefault(s => s.ID == (db.PurchaseOrders.FirstOrDefault(po => po.ID == o.i.PurchaseOrderID).SupplierID)).GSTNumber,
                       Items = db.Products.FirstOrDefault(p => p.ID == o.wsid.id.ProductID).Name + "(" + db.Sizes.FirstOrDefault(sz => sz.ID == (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wsid.id.ProductVarientID && pv.ProductID == o.wsid.id.ProductID).SizeID)).Name + ")",
                       GST = (o.wsid.id.GSTInPer == null) ? 0 : o.wsid.id.GSTInPer,
                       MRP = o.wsid.id.MRP,
                       Buy_rate_per_unit = o.wsid.id.BuyRatePerUnit,
                       Qty = o.wsid.id.ReceivedQuantity,
                       WarehouseId = o.wsid.ws.WarehouseID,
                       WarehouseStockId = o.wsid.ws.ID
                   })
                   .OrderBy(p => p.GST).ThenBy(p => p.Date)
                   .ToList();

            if (SearchCat == 3)
            {
                list = list.Where(p => p.Items.ToLower().Contains(searchTxt.ToLower())).ToList();
            }
            else if (SearchCat == 4)
            {
                int GST_ = Convert.ToInt16(searchTxt);
                list = list.Where(p => p.GST == GST_).ToList();
            }
            else if (SearchCat == 5)
            {
                decimal MRP_ = Convert.ToDecimal(searchTxt);
                list = list.Where(p => p.MRP == MRP_).ToList();
            }

            list.ForEach(p =>
            {
                p.Base_rate_per_unit = decimal.Round((p.Buy_rate_per_unit / (1 + ((decimal)p.GST / 100))), 2);
                p.GST_per_unit = decimal.Round(p.Buy_rate_per_unit - p.Base_rate_per_unit, 2);
                p.Total_GST_Amt = decimal.Round(p.GST_per_unit * p.Qty, 2);
            });
            return list;
        }

        public List<InputOutputGSTReport> GetInputGST(long WarehouseId, int Month, int year, bool IsReportForFV, int? SearchCat, string searchTxt, string frmDate, string toDate)
        {
            //int Year = DateTime.Now.Year;
            int Year = year;
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
            if (IsReportForFV == true)
            {
                //For Fv->Customer
                IEnumerable<CustomerOrderDetail> CustomerOrderDetailsList;
                if (!string.IsNullOrEmpty(frmDate) && !string.IsNullOrEmpty(toDate) && SearchCat == 1)
                {
                    DateTime FromDate_ = Convert.ToDateTime(frmDate);
                    DateTime ToDate_ = Convert.ToDateTime(toDate).AddHours(23);
                    CustomerOrderDetailsList = db.CustomerOrderDetails.Where(p => p.CreateDate > FromDate_ && p.CreateDate <= ToDate_ && p.OrderStatus == 7);
                }
                else
                {
                    CustomerOrderDetailsList = db.CustomerOrderDetails.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year && p.OrderStatus == 7);
                }
                long WarehouseFranchiseID = db.WarehouseFranchises.SingleOrDefault(wf => wf.WarehouseID == WarehouseId).FranchiseID;
                list = db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseId)
                       .Join(db.ShopStocks, ws => ws.ID, ss => ss.WarehouseStockID, (ws, ss) => new { ws, ss })
                       .Join(CustomerOrderDetailsList, wss => wss.ss.ID, cod => cod.ShopStockID, (wss, cod) => new { wss, cod })
                    // .Join(db.CustomerOrderDetails.Where(cod => cod.CreateDate <= Td && cod.CreateDate >= Bd && cod.OrderStatus == 7), wss => wss.ss.ID, cod => cod.ShopStockID, (wss, cod) => new { wss, cod })
                       .Select(o => new InputOutputGSTReport
                       {
                           Invoice_No = db.CustomerOrders.FirstOrDefault(co => co.ID == o.cod.CustomerOrderID).OrderCode,
                           Date = o.cod.CreateDate,
                           Items = db.Products.FirstOrDefault(p => p.ID == o.wss.ws.ProductID).Name + "(" + db.Sizes.FirstOrDefault(sz => sz.ID == (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wss.ws.ProductVarientID && pv.ProductID == o.wss.ws.ProductID).SizeID)).Name + ")",
                           MRP = o.cod.MRP,
                           Sale_rate_per_unit = o.cod.SaleRate,
                           Qty = o.cod.Qty,
                           WarehouseId = o.wss.ws.WarehouseID,
                           WarehouseStockId = o.wss.ws.ID,
                           CustomerOrderDetailID = o.cod.ID,
                           GST_No = db.Warehouses.FirstOrDefault(p => p.ID == EzeeloWarehouseId).GSTNumber,
                           GSTPart = db.FranchiseTaxDetails.Where(ftd => ftd.FranchiseID == WarehouseFranchiseID &&
                           (db.ProductTaxes.Where(pt =>
                           (db.TaxOnOrders.Where(to => to.CustomerOrderDetailID == o.cod.ID).Select(to => to.ProductTaxID))
                           .Contains(pt.ID)).Select(pt => pt.TaxID)).Contains(ftd.TaxationID))
                           .Sum(ftd => ftd.InPercentage)
                       })
                       .ToList();

                if (SearchCat == 2)
                {
                    list = list.Where(p => p.Invoice_No.Contains(searchTxt.ToLower())).ToList();
                }
                else if (SearchCat == 3)
                {
                    list = list.Where(p => p.Items.ToLower().Contains(searchTxt.ToLower())).ToList();
                }
                else if (SearchCat == 4)
                {
                    int GST_ = Convert.ToInt16(searchTxt);
                    list = list.Where(p => p.GST == GST_).ToList();
                }
                else if (SearchCat == 5)
                {
                    decimal MRP_ = Convert.ToDecimal(searchTxt);
                    list = list.Where(p => p.MRP == MRP_).ToList();
                }

                list.ForEach(p =>
                {
                    try
                    {
                        //Get GST value
                        //var gstQuery = db.TaxOnOrders.Where(to => to.CustomerOrderDetailID == p.CustomerOrderDetailID)
                        //               .Join(db.ProductTaxes, to => to.ProductTaxID, pt => pt.ID, (to, pt) => new { to, pt })
                        //               .Join(db.TaxationMasters, topt => topt.pt.TaxID, tm => tm.ID, (topt, tm) => new { topt, tm })
                        //               .Join(db.FranchiseTaxDetails.Where(ftd => ftd.FranchiseID ==
                        //                   (db.WarehouseFranchises.FirstOrDefault(wf => wf.WarehouseID == WarehouseId).FranchiseID))
                        //                   , toptm => toptm.tm.ID, ftd => ftd.TaxationID, (toptm, ftd) => new { toptm, ftd })
                        //               .Select(o => new
                        //               {
                        //                   o.ftd.InPercentage
                        //               });

                        if (p.GSTPart == null)
                        {
                            p.IsSelect = false;
                        }
                        else
                        {
                            p.IsSelect = true;
                            p.GST = Convert.ToInt16(p.GSTPart);
                            p.Base_rate_per_unit = decimal.Round((p.Sale_rate_per_unit / (1 + ((decimal)p.GST / 100))), 2);
                            p.GST_per_unit = decimal.Round(p.Sale_rate_per_unit - p.Base_rate_per_unit, 2);
                            p.Total_GST_Amt = decimal.Round(p.GST_per_unit * p.Qty, 2);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
                list = list.Where(p => p.IsSelect == true).OrderBy(p => p.GST).ThenBy(p => p.Date).ToList();
            }
            else
            {
                //For DV->FV And
                // For Ezeelo->DV

                IEnumerable<Invoice> InvoiceList;
                if (!string.IsNullOrEmpty(frmDate) && !string.IsNullOrEmpty(toDate) && SearchCat == 1)
                {
                    DateTime FromDate_ = Convert.ToDateTime(frmDate);
                    DateTime ToDate_ = Convert.ToDateTime(toDate);
                    InvoiceList = db.Invoices.Where(p => p.CreateDate > FromDate_ && p.CreateDate <= ToDate_);
                }
                else
                {
                    if (SearchCat == 2)
                    {
                        InvoiceList = db.Invoices.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year && p.InvoiceCode.Contains(searchTxt));
                    }
                    else
                    {
                        InvoiceList = db.Invoices.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year);
                    }
                }
                list = db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseId)
                       .Join(db.WarehouseStockDeliveryDetails, ws => ws.ID, wsdd => wsdd.WarehouseStockID, (ws, wsdd) => new { ws, wsdd })
                       .Join(db.PurchaseOrderReplyDetails, wsddws => wsddws.wsdd.PurchaseOrderReplyDetailID, pord => pord.ID, (wsddws, pord) => new { wsddws, pord })
                       .Join(db.PurchaseOrderReply, a => a.pord.PurchaseOrderReplyID, por => por.ID, (a, por) => new { a, por })
                       .Join(InvoiceList, b => b.por.InvoiceCode, i => i.InvoiceCode, (wsddws, i) => new { wsddws, i })
                       .Join(db.InvoiceDetails,
                         wsddwsi => new { InvoiceID = wsddwsi.i.ID, ProductID = wsddwsi.wsddws.a.wsddws.ws.ProductID, ProductVarientID = wsddwsi.wsddws.a.wsddws.ws.ProductVarientID },
                         id => new { InvoiceID = id.InvoiceID, ProductID = id.ProductID, ProductVarientID = id.ProductVarientID }, (wsddwsi, id) => new { wsddwsi, id }
                        )
                       .Select(o => new InputOutputGSTReport
                       {
                           Date = o.wsddwsi.i.CreateDate,
                           Invoice_No = o.wsddwsi.i.InvoiceCode,
                           GST_No = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId).GSTNumber,
                           Items = db.Products.FirstOrDefault(p => p.ID == o.id.ProductID).Name + "(" + db.Sizes.FirstOrDefault(sz => sz.ID == (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.id.ProductVarientID && pv.ProductID == o.id.ProductID).SizeID)).Name + ")",
                           GST = (o.id.GSTInPer == null) ? 0 : o.id.GSTInPer,
                           MRP = o.id.MRP,
                           Sale_rate_per_unit = o.id.BuyRatePerUnit,
                           Qty = o.id.ReceivedQuantity,
                           WarehouseId = o.wsddwsi.wsddws.a.wsddws.ws.WarehouseID,
                           WarehouseStockId = o.wsddwsi.wsddws.a.wsddws.ws.ID
                       })
                            .OrderBy(p => p.GST).ThenBy(p => p.Date)
                            .ToList();
                if (SearchCat == 3)
                {
                    list = list.Where(p => p.Items.ToLower().Contains(searchTxt.ToLower())).ToList();
                }
                else if (SearchCat == 4)
                {
                    int GST_ = Convert.ToInt16(searchTxt);
                    list = list.Where(p => p.GST == GST_).ToList();
                }
                else if (SearchCat == 5)
                {
                    decimal MRP_ = Convert.ToDecimal(searchTxt);
                    list = list.Where(p => p.MRP == MRP_).ToList();
                }
                list.ForEach(p =>
                {
                    p.Base_rate_per_unit = decimal.Round((p.Sale_rate_per_unit / (1 + ((decimal)p.GST / 100))), 2);
                    p.GST_per_unit = decimal.Round(p.Sale_rate_per_unit - p.Base_rate_per_unit, 2);
                    p.Total_GST_Amt = decimal.Round(p.GST_per_unit * p.Qty, 2);
                });
            }
            return list;
        }

        //public List<InputOutputGSTReport> GetInputGST(long WarehouseId, int Month, bool IsReportForFV, int? SearchCat, string searchTxt, string frmDate, string toDate)
        //{
        //    int Year = DateTime.Now.Year;
        //    long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
        //    List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
        //    if (IsReportForFV == true)
        //    {
        //        //For Fv->Customer
        //        IEnumerable<CustomerOrderDetail> CustomerOrderDetailsList;
        //        if (!string.IsNullOrEmpty(frmDate) && !string.IsNullOrEmpty(toDate) && SearchCat == 1)
        //        {
        //            DateTime FromDate_ = Convert.ToDateTime(frmDate);
        //            DateTime ToDate_ = Convert.ToDateTime(toDate);
        //            CustomerOrderDetailsList = db.CustomerOrderDetails.Where(p => p.CreateDate > FromDate_ && p.CreateDate <= ToDate_ &&  p.OrderStatus == 7);
        //        }
        //        else
        //        {
        //            CustomerOrderDetailsList = db.CustomerOrderDetails.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year && p.OrderStatus == 7);
        //        }

        //        list = db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseId)
        //               .Join(db.ShopStocks, ws => ws.ID, ss => ss.WarehouseStockID, (ws, ss) => new { ws, ss })
        //               .Join(CustomerOrderDetailsList, wss => wss.ss.ID, cod => cod.ShopStockID, (wss, cod) => new { wss, cod })
        //            // .Join(db.CustomerOrderDetails.Where(cod => cod.CreateDate <= Td && cod.CreateDate >= Bd && cod.OrderStatus == 7), wss => wss.ss.ID, cod => cod.ShopStockID, (wss, cod) => new { wss, cod })
        //               .Select(o => new InputOutputGSTReport
        //               {
        //                   Invoice_No = db.CustomerOrders.FirstOrDefault(co => co.ID == o.cod.CustomerOrderID).OrderCode,
        //                   Date = o.cod.CreateDate,
        //                   Items = db.Products.FirstOrDefault(p => p.ID == o.wss.ws.ProductID).Name + "(" + db.Sizes.FirstOrDefault(sz => sz.ID == (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.wss.ws.ProductVarientID && pv.ProductID == o.wss.ws.ProductID).SizeID)).Name + ")",
        //                   MRP = o.cod.MRP,
        //                   Sale_rate_per_unit = o.cod.SaleRate,
        //                   Qty = o.cod.Qty,
        //                   WarehouseId = o.wss.ws.WarehouseID,
        //                   WarehouseStockId = o.wss.ws.ID,
        //                   CustomerOrderDetailID = o.cod.ID,
        //                   GST_No = db.Warehouses.FirstOrDefault(p => p.ID == EzeeloWarehouseId).GSTNumber,
        //               })
        //               .ToList();

        //        if (SearchCat == 2)
        //        {
        //            list = list.Where(p => p.Invoice_No.Contains(searchTxt.ToLower())).ToList();
        //        }
        //        else if (SearchCat == 3)
        //        {
        //            list = list.Where(p => p.Items.ToLower().Contains(searchTxt.ToLower())).ToList();
        //        }
        //        else if (SearchCat == 4)
        //        {
        //            int GST_ = Convert.ToInt16(searchTxt);
        //            list = list.Where(p => p.GST == GST_).ToList();
        //        }
        //        else if (SearchCat == 5)
        //        {
        //            decimal MRP_ = Convert.ToDecimal(searchTxt);
        //            list = list.Where(p => p.MRP == MRP_).ToList();
        //        }

        //        list.ForEach(p =>
        //        {
        //            try
        //            {
        //                //Get GST value
        //                var gstQuery = db.TaxOnOrders.Where(to => to.CustomerOrderDetailID == p.CustomerOrderDetailID)
        //                               .Join(db.ProductTaxes, to => to.ProductTaxID, pt => pt.ID, (to, pt) => new { to, pt })
        //                               .Join(db.TaxationMasters, topt => topt.pt.TaxID, tm => tm.ID, (topt, tm) => new { topt, tm })
        //                               .Join(db.FranchiseTaxDetails.Where(ftd => ftd.FranchiseID ==
        //                                   (db.WarehouseFranchises.FirstOrDefault(wf => wf.WarehouseID == WarehouseId).FranchiseID))
        //                                   , toptm => toptm.tm.ID, ftd => ftd.TaxationID, (toptm, ftd) => new { toptm, ftd })
        //                               .Select(o => new
        //                               {
        //                                   o.ftd.InPercentage
        //                               });
        //                if (gstQuery == null || gstQuery.Count() == 0)
        //                {
        //                    p.IsSelect = false;
        //                }
        //                else
        //                {
        //                    p.IsSelect = true;
        //                    p.GST = Convert.ToInt16(gstQuery.Sum(g => g.InPercentage));
        //                    p.Base_rate_per_unit = decimal.Round((p.Sale_rate_per_unit / (1 + ((decimal)p.GST / 100))), 2);
        //                    p.GST_per_unit = decimal.Round(p.Sale_rate_per_unit - p.Base_rate_per_unit, 2);
        //                    p.Total_GST_Amt = decimal.Round(p.GST_per_unit * p.Qty, 2);
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        });
        //        list = list.Where(p => p.IsSelect == true).OrderBy(p => p.GST).ThenBy(p => p.Date).ToList();
        //    }
        //    else
        //    {
        //        //For DV->FV And
        //        // For Ezeelo->DV

        //        IEnumerable<Invoice> InvoiceList;
        //        if (!string.IsNullOrEmpty(frmDate) && !string.IsNullOrEmpty(toDate) && SearchCat == 1)
        //        {
        //            DateTime FromDate_ = Convert.ToDateTime(frmDate);
        //            DateTime ToDate_ = Convert.ToDateTime(toDate);
        //            InvoiceList = db.Invoices.Where(p => p.CreateDate > FromDate_ && p.CreateDate <= ToDate_);
        //        }
        //        else
        //        {
        //            if (SearchCat == 2)
        //            {
        //                InvoiceList = db.Invoices.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year && p.InvoiceCode.Contains(searchTxt));
        //            }
        //            else
        //            {
        //                InvoiceList = db.Invoices.Where(p => p.CreateDate.Month == Month && p.CreateDate.Year == Year);
        //            }
        //        }
        //        list = db.WarehouseStocks.Where(ws => ws.WarehouseID == WarehouseId)
        //               .Join(db.WarehouseStockDeliveryDetails, ws => ws.ID, wsdd => wsdd.WarehouseStockID, (ws, wsdd) => new { ws, wsdd })
        //               .Join(db.PurchaseOrderReplyDetails, wsddws => wsddws.wsdd.PurchaseOrderReplyDetailID, pord => pord.ID, (wsddws, pord) => new { wsddws, pord })
        //               .Join(db.PurchaseOrderReply, a => a.pord.PurchaseOrderReplyID, por => por.ID, (a, por) => new { a, por })
        //               .Join(InvoiceList, b => b.por.InvoiceCode, i => i.InvoiceCode, (wsddws, i) => new { wsddws, i })
        //               .Join(db.InvoiceDetails,
        //                 wsddwsi => new { InvoiceID = wsddwsi.i.ID, ProductID = wsddwsi.wsddws.a.wsddws.ws.ProductID, ProductVarientID = wsddwsi.wsddws.a.wsddws.ws.ProductVarientID },
        //                 id => new { InvoiceID = id.InvoiceID, ProductID = id.ProductID, ProductVarientID = id.ProductVarientID }, (wsddwsi, id) => new { wsddwsi, id }
        //                )
        //               .Select(o => new InputOutputGSTReport
        //               {
        //                   Date = o.wsddwsi.i.CreateDate,
        //                   Invoice_No = o.wsddwsi.i.InvoiceCode,
        //                   GST_No = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId).GSTNumber,
        //                   Items = db.Products.FirstOrDefault(p => p.ID == o.id.ProductID).Name + "(" + db.Sizes.FirstOrDefault(sz => sz.ID == (db.ProductVarients.FirstOrDefault(pv => pv.ID == o.id.ProductVarientID && pv.ProductID == o.id.ProductID).SizeID)).Name + ")",
        //                   GST = (o.id.GSTInPer == null) ? 0 : o.id.GSTInPer,
        //                   MRP = o.id.MRP,
        //                   Sale_rate_per_unit = o.id.BuyRatePerUnit,
        //                   Qty = o.id.ReceivedQuantity,
        //                   WarehouseId = o.wsddwsi.wsddws.a.wsddws.ws.WarehouseID,
        //                   WarehouseStockId = o.wsddwsi.wsddws.a.wsddws.ws.ID
        //               })
        //                    .OrderBy(p => p.GST).ThenBy(p => p.Date)
        //                    .ToList();
        //        if (SearchCat == 3)
        //        {
        //            list = list.Where(p => p.Items.ToLower().Contains(searchTxt.ToLower())).ToList();
        //        }
        //        else if (SearchCat == 4)
        //        {
        //            int GST_ = Convert.ToInt16(searchTxt);
        //            list = list.Where(p => p.GST == GST_).ToList();
        //        }
        //        else if (SearchCat == 5)
        //        {
        //            decimal MRP_ = Convert.ToDecimal(searchTxt);
        //            list = list.Where(p => p.MRP == MRP_).ToList();
        //        }
        //        list.ForEach(p =>
        //        {
        //            p.Base_rate_per_unit = decimal.Round((p.Sale_rate_per_unit / (1 + ((decimal)p.GST / 100))), 2);
        //            p.GST_per_unit = decimal.Round(p.Sale_rate_per_unit - p.Base_rate_per_unit, 2);
        //            p.Total_GST_Amt = decimal.Round(p.GST_per_unit * p.Qty, 2);
        //        });
        //    }
        //    return list;
        //}
        public List<GSTReport> GetGSTBalnce(long WarehouseId, int Month,int year, bool IsReportForFV)
        {
            List<GSTReport> list_GSTReport = new List<GSTReport>();
            List<InputOutputGSTReport> output_GSTReport = GetOutputGST((long)WarehouseId, (int)Month,(int)year,  0, "", "", "");
            List<InputOutputGSTReport> input_GSTReport = GetInputGST((long)WarehouseId, (int)Month, (int)year, IsReportForFV, 0, "", "", "");
            output_GSTReport = output_GSTReport.Join(input_GSTReport, o => o.WarehouseStockId, i => i.WarehouseStockId,
                (o, i) => new InputOutputGSTReport
                {
                    Date = o.Date,
                    Invoice_No = o.Invoice_No,
                    GST_No = o.GST_No,
                    Items = o.Items,
                    GST = o.GST,
                    MRP = o.MRP,
                    Buy_rate_per_unit = o.Buy_rate_per_unit,
                    Qty = i.Qty,
                    WarehouseId = o.WarehouseId,
                    WarehouseStockId = o.WarehouseStockId,
                    Base_rate_per_unit = o.Base_rate_per_unit,
                    GST_per_unit = o.GST_per_unit,
                    Total_GST_Amt = o.GST_per_unit
                }).ToList();


            var OutputGSTQuery = output_GSTReport.GroupBy(
                        p => p.GST,
                        p => p,
                        (key, g) => new { GST = key, OutputGSTReport = g.ToList() })
                        .ToList();
            var InputGSTQuery = input_GSTReport.GroupBy(
                        p => p.GST,
                        p => p,
                        (key, g) => new { GST = key, InputGSTReport = g.ToList() })
                        .ToList();

            list_GSTReport = InputGSTQuery.Join(OutputGSTQuery, i => i.GST, o => o.GST, (i, o) =>
                 new
                 {
                     i,
                     o
                 })
                 .Select(p => new GSTReport
                 {
                     InputGSTHeads = p.i.GST,
                     OutputGSTHeads = p.o.GST,
                     InputGSTAmount = p.i.InputGSTReport.Where(o => o.GST == p.i.GST).Sum(o => o.GST_per_unit * o.Qty),
                     OutputGSTAmount = p.o.OutputGSTReport.Where(o => o.GST == p.o.GST).Sum(o => o.GST_per_unit * o.Qty)
                 })
                 .Select(p => new GSTReport
                 {
                     InputGSTHeads = p.InputGSTHeads,
                     OutputGSTHeads = p.OutputGSTHeads,
                     InputGSTAmount = p.InputGSTAmount,
                     OutputGSTAmount = p.OutputGSTAmount,
                     Balance = p.InputGSTAmount - p.OutputGSTAmount
                 }).OrderBy(p => p.OutputGSTHeads).ToList();
            return list_GSTReport;
        }
        public ActionResult ExportToExcel(long ProductId, int flag)
        {
            var gv = new GridView();
            string ProductName = "";
            string FileName = "";
            string GST = "";
            if (flag == 1)
            {
                List<ProductRateMarginListViewModel> ProductList = GetListOfDVFVRateMargin(ProductId, null);
                List<DVFVRateExportListViewModel> list = new List<DVFVRateExportListViewModel>();

                foreach (var Product in ProductList)
                {
                    FileName = Product.ProductName;
                    ProductName = "Product Name: " + Product.ProductName + " HSN Code: " + Product.HSNCode;
                    foreach (var Varient in Product.PVarientList)
                    {
                        GST = Varient.GST.ToString();
                        foreach (var DV in Varient.DVList)
                        {
                            foreach (var FV in DV.FVList)
                            {
                                DVFVRateExportListViewModel obj = new DVFVRateExportListViewModel();
                                obj.BussinessPoints = FV.BussinessPoints;
                                obj.DVGST = FV.DVGST;
                                obj.DVId = FV.DVId;
                                obj.DVMargin = FV.DVMargin;
                                obj.DVMarginValueGST = FV.DVMarginVAlueWithGST;
                                obj.DVName = DV.Name;
                                obj.DVPurchasePrice = FV.DVPurchasePrice;
                                obj.DVSalePrice = FV.DVSalePrice;
                                obj.EzeeloGST = FV.EzeeloGST;
                                obj.EzeeloMargin = FV.EzeeloMargin;
                                obj.EzeeloPurchasePrice = Varient.PurchasePrice;
                                obj.ForEzeelo = FV.ForEzeelo;
                                obj.ForLeadership = FV.ForLeadership;
                                obj.ForLeadershipDevelopment = FV.ForLeadershipDevelopment;
                                obj.ForLeadersRoyalty = FV.ForLeadersRoyalty;
                                obj.ForLifestyle = FV.ForLifestyle;
                                obj.FVGST = FV.FVGST;
                                obj.FVId = FV.Id;
                                obj.FVMargin = FV.FVMargin;
                                obj.FVMarginValueGST = FV.FVMarginVAlueWithGST;
                                obj.FVName = FV.Name;
                                obj.FVPurchasePrice = FV.FVPurchasePrice;
                                obj.FVSalePrice = FV.FVSalePrice;
                                obj.GrossFlatMargin = Varient.GrossMarginFlat;
                                obj.GST = Varient.GST;
                                //obj.HSNCode = Product.HSNCode;
                                obj.MRP = Varient.MRP;
                                obj.PostGSTMargin = FV.PostGSTMargin;
                                //obj.ProductID = Product.ProductId;
                                //obj.ProductName = Product.ProductName;
                                obj.RateExpiryDate = Varient.RateExpiry;
                                obj.TotalGST = FV.TotalGST;
                                obj.ValuePostGST = Varient.ValuePostGST;
                                obj.VarientName = Varient.ProductVarientName;
                                list.Add(obj);
                            }
                        }
                    }
                }
                FileName = FileName + " Rate List " + DateTime.Now.ToString("dd-MM-yy");
                gv.DataSource = list;
                gv.DataBind();

                ProductName = ProductName + " GST: " + GST + "%";
                string headerTable = @"<Table style='font-weight:bolder; width:800px;'><tr><td>" + ProductName + "</td></tr></Table>";
                Response.Output.Write(headerTable);
            }
            else if (flag == 2)
            {
                //Output GST Report
                List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
                list = (List<InputOutputGSTReport>)Session["OutputGSTReportlist"];
                string warehouseName = "";
                if (list != null && list.Count() != 0)
                {
                    long WareId = list[0].WarehouseId;
                    warehouseName = db.Warehouses.FirstOrDefault(p => p.ID == WareId).Name;
                }
                gv.DataSource = list.Select(p => new { p.Date, p.Invoice_No, p.GST_No, p.Items, p.GST, p.MRP, p.Buy_rate_per_unit, p.Base_rate_per_unit, p.GST_per_unit, p.Qty, p.Total_GST_Amt });
                gv.DataBind();

                FileName = "Output GST Report For " + warehouseName + DateTime.Now.ToString("dd-MM-yy");
            }
            else if (flag == 3)
            {
                //Input GST Report
                List<InputOutputGSTReport> list = new List<InputOutputGSTReport>();
                list = (List<InputOutputGSTReport>)Session["InputGSTReportlist"];
                string warehouseName = "";
                if (list != null && list.Count() != 0)
                {
                    long WareId = list[0].WarehouseId;
                    warehouseName = db.Warehouses.FirstOrDefault(p => p.ID == WareId).Name;
                }
                gv.DataSource = list.Select(p => new { p.Date, p.Invoice_No, p.GST_No, p.Items, p.GST, p.MRP, p.Sale_rate_per_unit, p.Base_rate_per_unit, p.GST_per_unit, p.Qty, p.Total_GST_Amt }); ;
                gv.DataBind();

                FileName = "Input GST Report For " + warehouseName + DateTime.Now.ToString("dd-MM-yy");
            }
            else if (flag == 4)
            {
                //GST Report
                List<GSTReport> list = new List<GSTReport>();
                list = (List<GSTReport>)Session["GSTReportlist"];
                string warehouseName = "";
                long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                long WareId;
                if (ProductId == 0)
                {
                    WareId = EzeeloWarehouseId;
                }
                else
                {
                    WareId = ProductId;
                }
                warehouseName = db.Warehouses.FirstOrDefault(p => p.ID == WareId).Name;
                gv.DataSource = list;
                gv.DataBind();
                FileName = "GST Report For " + warehouseName + DateTime.Now.ToString("dd-MM-yy");
            }
            else
            {

            }
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
            return View("Index");
        }

        #endregion
    }
}