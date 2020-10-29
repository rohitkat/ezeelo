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
    public class BatchAllotmentController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        CommonController obj_comm = new CommonController();
        //
        // GET: /BatchAllotment/
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
                    //lWarehouseReorderLevel = db.WarehouseReorderLevels.Where(x => x.WarehouseID == WarehouseID).OrderBy(x => x.AvailableQuantity).ToList();

                    var stockQuanitity = (from wl in db.WarehouseReorderLevels
                                          //join ws in db.WarehouseStocks on wl.WarehouseID equals ws.WarehouseID
                                          join w in db.Warehouses on wl.WarehouseID equals w.ID
                                          join wf in db.WarehouseFranchises on w.ID equals wf.WarehouseID
                                          join s in db.Shops on wf.FranchiseID equals s.FranchiseID
                                          join sp in db.ShopProducts on s.ID equals sp.ShopID
                                          join ss in db.ShopStocks on sp.ID equals ss.ShopProductID
                                          where w.ID == WarehouseID && w.IsFulfillmentCenter == true && wl.ProductID == sp.ProductID
                                          && wl.ProductVarientID == ss.ProductVarientID && wl.AvailableQuantity>0
                                          select new WarehouseReorderLevelViewModel 
                                          {
                                              ID = wl.ID,
                                              WarehouseID = w.ID,
                                              ShopStockID = ss.ID,
                                              WarehouseStockID = ss.WarehouseStockID,
                                              ProductID = wl.ProductID,
                                              ProductVarientID = wl.ProductVarientID,
                                              InStockQty = wl.AvailableQuantity,
                                              InShopQty = db.ShopStocks.Where(x => x.ShopProductID == sp.ID && x.ProductVarientID == wl.ProductVarientID).Sum(x => x.Qty),
                                              ReorderLevel = wl.ReorderLevel,                                       
                                              isAllottedToShop = ss.WarehouseStockID==null?false:true
                                          }).OrderByDescending(x => x.InStockQty).OrderBy(x => x.isAllottedToShop).ToList();

                    objWS.WarehouseName = db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.Name).FirstOrDefault();

                    foreach (var item in stockQuanitity)
                    {                
                        WarehouseReorderLevelViewModel objPOD = new WarehouseReorderLevelViewModel();
                        objPOD.ID = item.ID;
                        objPOD.WarehouseID = item.WarehouseID;
                        objPOD.WarehouseStockID = item.WarehouseStockID;
                        objPOD.ShopStockID = item.ShopStockID;
                        objPOD.InStockQty = item.InStockQty;
                        objPOD.InShopQty = item.InShopQty;
                        objPOD.ReorderLevel = item.ReorderLevel;
                        objPOD.isAllottedToShop = item.isAllottedToShop;
                        objPOD.PlacedQty = item.WarehouseStockID == null ? 0 : db.CustomerOrderDetails.Where(x => x.WarehouseStockID == item.WarehouseStockID && x.OrderStatus != 0 && x.OrderStatus != 7 && x.OrderStatus != 8 && x.OrderStatus != 9 && x.OrderStatus != 10).Select(x => (int)x.Qty).DefaultIfEmpty(0).Sum();
                        objPOD.PendingQty = item.WarehouseStockID == null ? 0 : db.CustomerOrderDetails.Where(x => x.WarehouseStockID == item.WarehouseStockID && x.OrderStatus == 0).Select(x => (int)x.Qty).DefaultIfEmpty(0).Sum();
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

                objWS.lWarehouseReorderLevelViewModel = objWRLVM;
                Session["WarehouseReorderLevelViewModel"] = objWRLVM; //Added by rumana on 17-05-2019 for Export to Excel
            }
            catch (Exception)
            {
                //Transaction.Current.Rollback();
                //tscope.Dispose();
            }
            return View("Stock", objWS);
        }


        public ActionResult StockBatchwise(long ID, long ShopStockID)
        {
            BatchAllotmentViewModel objBatch = new BatchAllotmentViewModel();
            WarehouseStockViewModel objWS = new WarehouseStockViewModel();
            List<WarehouseStock> lWarehouseStock = new List<WarehouseStock>();
            List<WarehouseStockViewModel> objWRLVM = new List<WarehouseStockViewModel>();
            try
            {
                if (Session["USER_NAME"] != null)
                {}
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
                                      && ws.ProductVarientID == query.ProductVarientID && ws.AvailableQuantity>0
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
                        objWarehouseStockViewModel.InShopQty = db.ShopStocks.Where(x => x.ID == item.ShopStockID && x.WarehouseStockID==item.WarehouseStockID).Select(x => x.Qty).FirstOrDefault();
                        objWarehouseStockViewModel.PlacedQty = item.WarehouseStockID == null ? 0 : db.CustomerOrderDetails.Where(x => x.WarehouseStockID == item.ID && x.OrderStatus != 0 && x.OrderStatus != 7 && x.OrderStatus != 8 && x.OrderStatus != 9 && x.OrderStatus != 10).Select(x => (int)x.Qty).DefaultIfEmpty(0).Sum();
                        objWarehouseStockViewModel.PendingQty = item.WarehouseStockID == null ? 0 : db.CustomerOrderDetails.Where(x => x.WarehouseStockID == item.ID && x.OrderStatus == 0).Select(x => (int)x.Qty).DefaultIfEmpty(0).Sum();
                        objWarehouseStockViewModel.InStockQty = item.AvailableQuantity - objWarehouseStockViewModel.PlacedQty;
                        objWarehouseStockViewModel.StockThumbPath = ImageDisplay.SetProductThumbPath(item.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        objWarehouseStockViewModel.ShopStockID = item.ShopStockID;

                        var itemName = (from p in db.Products
                                        join v in db.ProductVarients on p.ID equals v.ProductID
                                        join s in db.Sizes on v.SizeID equals s.ID
                                        where v.ID == item.ProductVarientID
                                        select new InvoiceDetailViewModel { ItemName = p.Name+ " (" + s.Name + ")" , HSNCode = p.HSNCode }).ToList();// Remove + " (" + s.Name + ")" by Rumana on1/04/2019

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
            if (objWRLVM == null || objWRLVM.Count == 0)
            {
                return RedirectToAction("Stock");
            }
            //objBatch.lWarehouseStockViewModels = objWRLVM;
            return View("StockBatchwise", objWRLVM);
        }

        // POST: /BatchAllotment/StockBatchwise
        //[HttpPost]
        public ActionResult ApplyActiveBatch(long WarehouseStockID, long ShopStockID, long ReorderLevelID)
        {
            using (TransactionScope tscope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                try
                {
                    WarehouseStock lStock = db.WarehouseStocks.Where(x => x.ID == WarehouseStockID).FirstOrDefault();

                    int remainingQty = 0;

                    //var reserveQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == WarehouseStockID && (x.OrderStatus == 1
                    //        || x.OrderStatus == 2 || x.OrderStatus == 3 || x.OrderStatus == 4 || x.OrderStatus == 5 || x.OrderStatus == 6)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
                     var PlacedQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == WarehouseStockID && (x.OrderStatus == 1
                            || x.OrderStatus == 2 || x.OrderStatus == 3 || x.OrderStatus == 4 || x.OrderStatus == 5 || x.OrderStatus == 6)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
                        var PendingQty = db.CustomerOrderDetails.Where(x => x.WarehouseStockID == WarehouseStockID && (x.OrderStatus == 0)).Select(x => x.Qty).DefaultIfEmpty(0).Sum();
                        var reserveQty = PlacedQty + PendingQty;

                    if (reserveQty != null  && reserveQty <= lStock.AvailableQuantity)
                    {
                        remainingQty = lStock.AvailableQuantity - reserveQty;
                    }

                    if (lStock != null && lStock.AvailableQuantity > 0)
                    {
                        ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                        ShopStock objlog = db.ShopStocks.Where(x => x.ID == ShopStockID).FirstOrDefault();
                        if (reserveQty > lStock.AvailableQuantity)
                        {
                            objlog.Qty = 0;
                            
                        }
                        //if(remainingQty!=0)
                        //{
                        //    objlog.Qty = remainingQty;
                        //}
                        else 
                        {
                            objlog.Qty = remainingQty;
                        }
                        
                        objlog.MRP = Convert.ToDecimal(lStock.MRP);
                        objlog.RetailerRate = lStock.SaleRatePerUnit;
                        objlog.WarehouseStockID = lStock.ID;
                        objlog.StockStatus = true;
                        //yashaswi 23/04/2018
                        objlog.BusinessPoints = (decimal)lStock.BusinessPoints;
                        objlog.CashbackPoints = prod.getCasbackPointsOnProductFromWarehouse(WarehouseStockID);
                        objlog.ModifyDate = DateTime.Now;
                        objlog.ModifyBy = 1;
                        objlog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        objlog.DeviceID = "x";
                        objlog.DeviceType = "x";
                        db.SaveChanges();

                        tscope.Complete();
                        Session["Success"] = "Batch " + lStock.BatchCode+ " allotted Successfully.";
                        return RedirectToAction("StockBatchwise", new { ID = ReorderLevelID, ShopStockID = ShopStockID });
                    }
                    Session["Error"] = "Batch not allotted! Item quantity should be greater than 0."; ;
                    return RedirectToAction("StockBatchwise", new {ID= ReorderLevelID,ShopStockID= ShopStockID });
                }
                catch(Exception ex)
                {
                    Transaction.Current.Rollback();
                    tscope.Dispose();
                    Session["Error"] = ex;
                    return RedirectToAction("StockBatchwise", new { ID = ReorderLevelID, ShopStockID = ShopStockID });
                }
            }
        }

        //
        // GET: /BatchAllotment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /BatchAllotment/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        

        //
        // GET: /BatchAllotment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /BatchAllotment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /BatchAllotment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /BatchAllotment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
            FileName = FileName + " SHOP STOCK BATCH ALLOTMENT List";
            List<ProductReorderExport_ForBatchAllotmentReport> list = new List<ProductReorderExport_ForBatchAllotmentReport>();
            if (Session["WarehouseReorderLevelViewModel"] != null)
            {
                List<WarehouseReorderLevelViewModel> objWRLVM = (List<WarehouseReorderLevelViewModel>)Session["WarehouseReorderLevelViewModel"];

                if (objWRLVM.Count != 0)
                {
                    var i = 0;
                    foreach (var item in objWRLVM)
                    {
                        ProductReorderExport_ForBatchAllotmentReport o = new ProductReorderExport_ForBatchAllotmentReport();
                        o.SrNo = i + 1;
                        o.SKUID = item.ProductVarientID;
                        o.Item = item.ItemName;
                        o.HSNCode = item.HSNCode;
                        o.AvailableQty = item.InStockQty;
                        o.InShopQty = item.InShopQty;
                        o.PendingQty = item.PendingQty;
                        o.PlacedQty = item.PlacedQty;
                        o.ReOrderLevel = item.ReorderLevel;
                        if(item.isAllottedToShop==true)
                        {
                            o.IsLinkedWithShop = "Yes";
                        }
                        else
                        {
                            o.IsLinkedWithShop = "No";
                        }

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
    }
}
