using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using ModelLayer.Models;
using BusinessLogicLayer;
using System.IO;
using System.Text;
using Franchise.Models;
using System.Transactions;

namespace Franchise.Controllers
{
    public class ProductUploadAprovedController : Controller
    {
        public class ForLoopClass //----------------use this class for loop purpose in below functions----------------
        {
            public long ID { get; set; }
            public string Name { get; set; }
        }

        #region Genral Code

        private EzeeloDBContext db = new EzeeloDBContext();
        private static long ShopID1 = 0;        
        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
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

        #endregion

        #region Code for IndexAproveProduct

        //========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanRead")]
        public ActionResult MerchantList()
        {
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                List<ShopViewModel> lShop = (from s in db.Shops                                             
                                             where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR" &&
                                             s.FranchiseID == franchiseID
                                             select new ShopViewModel
                                             {
                                                 ID = s.ID,
                                                 Name = s.Name,
                                                 FranchiseID = s.FranchiseID,
                                                 ApproveProductCount = db.ShopProducts.Where(x => x.ShopID == s.ID && x.Product.IsActive == true).Count()
                                             }).ToList();

                return View(lShop);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovalController][GET:MerchantList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovalController][GET:MerchantList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanRead")]
        public ActionResult Index(long shopId)
        {
            //long ShopID = GetShopID();
            try
            {
                var QueryResult = (from TPV in db.ProductVarients
                                   join TP in db.Products on TPV.ProductID equals TP.ID
                                   join TSP in db.ShopProducts on TP.ID equals TSP.ProductID
                                   join TSS in db.ShopStocks on TSP.ID equals TSS.ShopProductID
                                   join CTG in db.Categories on TP.CategoryID equals CTG.ID
                                   join BRD in db.Brands on TP.BrandID equals BRD.ID
                                   join SHP in db.Shops on TSP.ShopID equals SHP.ID
                                   where TSP.ShopID == shopId && TPV.ID == TSS.ProductVarientID
                                   group new { TSS, TP, CTG, BRD, TPV } by new
                                   {
                                       TSS.ShopProductID,
                                       TP.Name,
                                       CategoryName = CTG.Name,
                                       BrandName = BRD.Name,
                                       TP.ID,
                                       ShopName = SHP.Name
                                   } into g
                                   select new ProductUploadViewModel
                                   {
                                       Qty = g.Sum(p => p.TSS.Qty),
                                       ProductVarientID = g.Count(p => p.TSS.ProductVarientID != null),
                                       ShopProductID = g.Key.ShopProductID,
                                       ProductName = g.Key.Name,
                                       CategoryName = g.Key.CategoryName,
                                       BrandName = g.Key.BrandName,
                                       ProductID = g.Key.ID,
                                       ShopID = shopId,
                                       ShopName = g.Key.ShopName,
                                       ColorID = g.Max(p=>p.TPV.ColorID)
                                   }).OrderByDescending(x => x.ProductID).ToList(); ;
                List<ProductUploadTempViewModel> listProductUploadTemp = new List<ProductUploadTempViewModel>();

                foreach (var ReadRecord in QueryResult)
                {
                    ProductUploadTempViewModel putvm = new ProductUploadTempViewModel();
                    putvm.ID = ReadRecord.ProductID;
                    putvm.CategoryName = ReadRecord.CategoryName;
                    putvm.BrandName = ReadRecord.BrandName;
                    putvm.Name = ReadRecord.ProductName;
                    putvm.Qty = ReadRecord.Qty;
                    putvm.PackSize = ReadRecord.PackSize;
                    putvm.ProductVarientID = ReadRecord.ProductVarientID;
                    putvm.ShopID = ReadRecord.ShopID;
                    putvm.ColorID = ReadRecord.ColorID;
                    if (ReadRecord.ColorID == 1)
                    {
                        putvm.ImageLocation = ImageDisplay.SetProductThumbPath(ReadRecord.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    }
                    else
                    {
                        putvm.ImageLocation = ImageDisplay.SetProductThumbPath(ReadRecord.ProductID, db.Colors.Where(x => x.ID == ReadRecord.ColorID).FirstOrDefault().Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    }                   
                    listProductUploadTemp.Add(putvm);
                }

                ProductUploadTempViewModelList PUTVML = new ProductUploadTempViewModelList();
                PUTVML.ProductUploadTempViewModelLIst = listProductUploadTemp;
                return View(PUTVML);
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product Upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadAproved][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product Upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadAproved][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View();


        }

        //=========================================
        #endregion

        //Author:Harshada
        #region Code for Component


        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanRead")]
        public ActionResult LoadVarient(long? productID, long? ShopID)
        {
            try
            {
                ViewBag.ShopID = ShopID;
                ShopID1 = Convert.ToInt64(ShopID);
                var VarientList = from TPV in db.ProductVarients
                                  join C in db.Colors on new { ColorID = TPV.ColorID } equals new { ColorID = C.ID }
                                  join S in db.Sizes on new { SizeID = TPV.SizeID } equals new { SizeID = S.ID }
                                  join D in db.Dimensions on new { DimensionID = TPV.DimensionID } equals new { DimensionID = D.ID }
                                  join M in db.Materials on new { MaterialID = TPV.MaterialID } equals new { MaterialID = M.ID }
                                  join TSS in db.ShopStocks on new { ProductVarientID = TPV.ID } equals new { ProductVarientID = TSS.ProductVarientID }
                                  join TSP in db.ShopProducts on new { productID = TPV.ProductID } equals new { productID = TSP.ProductID }
                                  where
                                    TPV.ProductID == productID && TPV.IsActive == true && TSP.ShopID == ShopID

                                  select new ProductComponentViewModel
                                  {
                                      ColorName = C.Name,
                                      SizeName = S.Name,
                                      DimensionName = D.Name,
                                      MaterialName = M.Name,
                                      Quantity = TSS.Qty,
                                      ShopStockID = TSS.ID,
                                      ProductVarientID = TPV.ID,
                                      MRP = TSS.MRP
                                  };

                return View(VarientList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while loading varients!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:LoadVarient]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while loading varients!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:LoadVarient]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        public ActionResult AddComponent(long? ShopStockID, long? ProductVarientID)
        {
            try
            {
                ViewBag.ShopStockID = ShopStockID;
                ViewBag.ProductVarientID = ProductVarientID;

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while loading Components partial view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:AddComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while loading Components partial view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:AddComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return PartialView("_AddComponent");
        }
        [HttpPost]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        public JsonResult BindComponent()
        {

            try
            {
                //long ShopID = GetShopID();
                var productComp = from scp in db.ShopComponentPrices
                                  join c in db.Components on new { ComponentID = scp.ComponentID } equals new { ComponentID = c.ID }
                                  where
                                    scp.ShopID == ShopID1 && c.IsActive == true
                                  select new ProductComponentViewModel
                                  {
                                      ComponentID = c.ID,
                                      ComponentName = c.Name
                                  };

                return Json(productComp);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Binding Components!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Binding Components!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json("");
        }
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        public JsonResult BindComponentDetails(long? componentID)
        {
            try
            {
                //long ShopID = GetShopID();
                var compDetail = (from c in db.Components
                                  join scp in db.ShopComponentPrices on c.ID equals scp.ComponentID into scp_join
                                  from scp in scp_join.DefaultIfEmpty()
                                  join dc in db.Components on scp.DependentOnComponentID equals dc.ID into dc_join
                                  from dc in dc_join.DefaultIfEmpty()
                                  join u in db.Units on scp.ComponentUnitID equals u.ID into u_join
                                  from u in u_join.DefaultIfEmpty()
                                  where
                                    scp.ShopID == ShopID1
                                    && scp.ComponentID == componentID
                                  select new ProductComponentViewModel
                                  {
                                      ComponentID = c.ID,
                                      ComponentName = c.Name,
                                      ComponentUnitID = u.ID,
                                      ComponentUnitName = (u.Name == string.Empty ? string.Empty : u.Name),
                                      DependentOnComponentID = (scp.DependentOnComponentID == null ? 0 : scp.DependentOnComponentID),
                                      // DependentOnComponentID = scp.DependentOnComponentID,
                                      DependentComponentName = (dc.Name == string.Empty ? "N/A" : dc.Name),
                                      PerUnitRateInPer = scp.PerUnitRateInPer,
                                      PerUnitRateInRs = scp.PerUnitRateInRs,

                                  }).ToList();

                return Json(compDetail.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Binding  Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindComponentDetails]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Binding  Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindComponentDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json("");
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        public JsonResult BindSavedComponent(long? ShopStockID, long? ProductVarientID)
        {
            try
            {
                //long ShopID = GetShopID();
                var IsIdPresent = db.TempStockComponents.Where(x => x.ShopStockID == ShopStockID).FirstOrDefault();
                if (IsIdPresent != null)
                {
                    var productComp = (from tsc in db.StockComponents

                                       join c in db.Components on tsc.ComponentID equals c.ID into c_join
                                       from c in c_join.DefaultIfEmpty()

                                       join tss in db.ShopStocks on tsc.ShopStockID equals tss.ID into tss_join
                                       from tss in tss_join.DefaultIfEmpty()

                                       join scp in db.ShopComponentPrices on c.ID equals scp.ComponentID into scp_join
                                       from scp in scp_join.DefaultIfEmpty()

                                       join dc in db.Components on scp.DependentOnComponentID equals dc.ID into dc_join
                                       from dc in dc_join.DefaultIfEmpty()

                                       join u in db.Units on scp.ComponentUnitID equals u.ID into u_join
                                       from u in u_join.DefaultIfEmpty()

                                       where
                                         scp.ShopID == ShopID1 && tss.ProductVarientID == ProductVarientID
                                       select new ProductComponentViewModel
                                       {
                                           ComponentID = c.ID,
                                           ComponentName = c.Name,
                                           ComponentUnitID = (u.ID == null ? 0 : u.ID),
                                           ComponentUnitName = (u.Name == string.Empty ? string.Empty : u.Name),
                                           ComponentWeight = tsc.ComponentWeight,
                                           // DependentOnComponentID = (scp.DependentOnComponentID == null ? 0 : scp.DependentOnComponentID),
                                           DependentOnComponentID = scp.DependentOnComponentID,
                                           PerUnitRateInRs = scp.PerUnitRateInRs,
                                           PerUnitRateInPer = scp.PerUnitRateInPer,
                                           Total = tss.MRP,
                                           DependentComponentName = (dc.Name == string.Empty ? "N/A" : dc.Name)
                                       }).OrderBy(x => x.ComponentID).ToList();

                    return Json(productComp);
                }
                else
                {
                    return Json("");
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Binding Saved Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindSavedComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Binding Saved Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindSavedComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json("");

        }
        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        public JsonResult SaveComponent(ProductComponentViewModel myData)
        {
            string Message = "";
            try
            {
                if (myData != null)
                {
                    StockComponent stockComponent = new StockComponent();
                    ShopStock shopStock = new ShopStock();
                    long stockCompnentID = (from tsp in db.TempStockComponents
                                            where tsp.ShopStockID == myData.ShopStockID && tsp.ComponentID == myData.ComponentID
                                            select tsp.ID).FirstOrDefault();

                    if (stockCompnentID > 0)
                    {
                        stockComponent = db.StockComponents.Find(stockCompnentID);
                        stockComponent.ShopStockID = stockComponent.ShopStockID;
                        stockComponent.ComponentID = stockComponent.ComponentID;
                        stockComponent.CreateDate = stockComponent.CreateDate;
                        stockComponent.CreateBy = stockComponent.CreateBy;
                        //stockComponent.IsActive = stockComponent.IsActive;
                        if (myData.ComponentWeight == 0)
                        {
                            stockComponent.ComponentWeight = null;
                        }
                        else
                        {
                            stockComponent.ComponentWeight = Convert.ToDecimal(myData.ComponentWeight);
                        }
                        if (myData.ComponentUnitID > 0)
                        {
                            stockComponent.ComponentUnitID = Convert.ToInt32(myData.ComponentUnitID);
                        }
                        else if (myData.ComponentUnitID == null)
                        {
                            stockComponent.ComponentUnitID = 1;
                        }

                        stockComponent.ModifyDate = DateTime.UtcNow;
                        stockComponent.ModifyBy = 1;
                        stockComponent.NetworkIP = stockComponent.NetworkIP;
                        stockComponent.DeviceID = stockComponent.DeviceID;
                        stockComponent.DeviceType = stockComponent.DeviceType;
                        //TryUpdateModel(stockComponent);
                        //if (ModelState.IsValid)
                        //{
                        db.Entry(stockComponent).State = EntityState.Modified;
                        db.SaveChanges();
                        Message = "Updated";
                        //}
                    }
                    else
                    {
                        stockComponent.ShopStockID = myData.ShopStockID;
                        stockComponent.ComponentID = Convert.ToInt32(myData.ComponentID);
                        if (myData.ComponentWeight == 0)
                        {
                            stockComponent.ComponentWeight = null;
                        }
                        else
                        {
                            stockComponent.ComponentWeight = Convert.ToDecimal(myData.ComponentWeight);
                        }
                        if (myData.ComponentUnitID > 0)
                        {
                            stockComponent.ComponentUnitID = Convert.ToInt32(myData.ComponentUnitID);
                        }
                        stockComponent.CreateDate = DateTime.UtcNow;
                        //stockComponent.IsActive = true;
                        stockComponent.CreateBy = 1;
                        stockComponent.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        stockComponent.DeviceID = "x";
                        stockComponent.DeviceType = "x";
                        db.StockComponents.Add(stockComponent);
                        db.SaveChanges();
                        Message = "Saved";

                    }
                    if (myData.ShopStockID > 0)
                    {
                        shopStock = db.ShopStocks.Find(myData.ShopStockID);
                        shopStock.MRP = Convert.ToDecimal(myData.Total);
                        shopStock.RetailerRate = Convert.ToDecimal(myData.Total);
                        shopStock.WholeSaleRate = Convert.ToDecimal(myData.Total);
                        TryUpdateModel(shopStock);
                        db.SaveChanges();
                    }

                    if (stockComponent == null && shopStock == null)
                    {
                        //return HttpNotFound();
                    }
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Saving Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ ProductUploadAproved][POST:SaveComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ProductUploadAproved][POST:SaveComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return Json(Message, JsonRequestBehavior.AllowGet);

        }


        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        public JsonResult DeleteComponent(long? ComponentID, long? ShopStockID, decimal? Total)
        {
            string Message = "";
            try
            {
                ShopStock shopStock = new ShopStock();
                StockComponent tempComponent = db.StockComponents.Where(x => x.ComponentID == ComponentID && x.ShopStockID == ShopStockID).FirstOrDefault();
                db.StockComponents.Remove(tempComponent);
                Message = "Deleted Succesfully";
                if (ShopStockID > 0)
                {
                    shopStock = db.ShopStocks.Find(ShopStockID);
                    shopStock.MRP = Convert.ToDecimal(Total);
                    TryUpdateModel(shopStock);
                    db.SaveChanges();
                    Message = "Deleted Succesfully";
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Deleting Component!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[  ProductUploadAproved][POST:DeleteComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Deleting Component!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[  ProductUploadAproved][POST:DeleteComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        //=========================End Of Component===========================================================================================================
        #endregion

        #region Code for DETAIL
        //=========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanRead")]
        public ActionResult Details1(long id, long? ShopID)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            //long ShopID = GetShopID();
            long PersonalDetailID = GetPersonalDetailID();


            try
            {

                string[] src = ImageDisplay.DisplayProductImages(id, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                ViewBag.ImageURL = src;

                Product TP = db.Products.Find(id);
                productuploadtempviewmodel.Name = TP.Name;
                productuploadtempviewmodel.CategoryL_2 = TP.CategoryID;
                var pID = (from dd in db.Categories
                           join dd1 in db.Categories on dd.ID equals dd1.ParentCategoryID
                           where dd1.ID == TP.CategoryID
                           select new { dd.ID }).First();
                productuploadtempviewmodel.CategoryL_1 = Convert.ToInt32(pID.ID);

                var pID1 = (from dd in db.Categories
                            join dd1 in db.Categories on dd.ID equals dd1.ParentCategoryID
                            where dd1.ID == pID.ID
                            select new { dd.ID }).First();
                productuploadtempviewmodel.CategoryL_0 = Convert.ToInt32(pID1.ID);



                productuploadtempviewmodel.WeightInGram = TP.WeightInGram;
                productuploadtempviewmodel.LengthInCm = TP.LengthInCm;
                productuploadtempviewmodel.BreadthInCm = TP.BreadthInCm;
                productuploadtempviewmodel.HeightInCm = TP.HeightInCm;
                productuploadtempviewmodel.Description = TP.Description;
                productuploadtempviewmodel.BrandID = TP.BrandID;
                productuploadtempviewmodel.SearchKeyword = TP.SearchKeyword.Trim();
                productuploadtempviewmodel.IsActive = TP.IsActive;
                productuploadtempviewmodel.CreateDate = DateTime.Now;
                productuploadtempviewmodel.CreateBy = PersonalDetailID;
                productuploadtempviewmodel.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                productuploadtempviewmodel.DeviceID = "x";
                productuploadtempviewmodel.DeviceType = "x";

                ShopProduct TSP = db.ShopProducts.Where(x => x.ProductID == id && x.ShopID == ShopID).First();
                productuploadtempviewmodel.ShopID = TSP.ShopID;
                productuploadtempviewmodel.ID = TSP.ID;
                productuploadtempviewmodel.IsActive = TSP.IsActive;
                productuploadtempviewmodel.DisplayProductFromDate = TSP.DisplayProductFromDate;
                productuploadtempviewmodel.CreateDate = DateTime.Now;
                productuploadtempviewmodel.CreateBy = PersonalDetailID;
                productuploadtempviewmodel.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                productuploadtempviewmodel.DeviceID = "x";
                productuploadtempviewmodel.DeviceType = "x";

                if (productuploadtempviewmodel == null)
                {
                    return HttpNotFound();
                }

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", productuploadtempviewmodel.BrandID);
                // ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", productuploadtempviewmodel.CategoryID);
                //ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                ViewBag.CategoryL_0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.CategoryL_0);
                ViewBag.CategoryL_1 = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.CategoryL_1);
                ViewBag.CategoryL_2 = new SelectList(db.Categories.Where(c => c.Level == 3), "ID", "Name", productuploadtempviewmodel.CategoryL_2);

                var query = (from TPV in db.ProductVarients
                             join TSS in db.ShopStocks on TPV.ID equals (TSS.ProductVarientID)
                             join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                             join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                             join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                             join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                             join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                             where TPV.ProductID == id && TSS.ShopProduct.ShopID == ShopID
                             select new
                             {
                                 ID = TPV.ProductID,
                                 ProductVarientID = TPV.ID,
                                 ColorID = TPV.ColorID,
                                 SizeID = TPV.SizeID,
                                 DimensionID = TPV.DimensionID,
                                 MaterialID = TPV.MaterialID,
                                 Qty = TSS.Qty,
                                 ReorderLevel = TSS.ReorderLevel,
                                 StockStatus = TSS.StockStatus,
                                 PackSize = TSS.PackSize,
                                 PackUnitID = TSS.PackUnitID,
                                 MRP = TSS.MRP,
                                 RetailerRate = TSS.RetailerRate,
                                 ShopStockID = TSS.ID,
                                 ColorName = CLR.Name,
                                 SizeName = SIZ.Name,
                                 DiamentionName = DMS.Name,
                                 MaterialName = MTR.Name,
                                 UnitName = UNT.Name,
                                 IsInclusiveOfTax = TSS.IsInclusiveOfTax
                             }).ToList();



                List<NewProductVarient> newProductVarientList = new List<NewProductVarient>();
                foreach (var i in query)
                {
                    NewProductVarient newProductVarient = new ModelLayer.Models.ViewModel.NewProductVarient();
                    newProductVarient.ID = i.ID;
                    newProductVarient.ProductVarientID = i.ProductVarientID;
                    newProductVarient.ColorID = i.ColorID;
                    newProductVarient.SizeID = i.SizeID;
                    newProductVarient.DimensionID = i.DimensionID;
                    newProductVarient.MaterialID = i.MaterialID;
                    newProductVarient.Qty = i.Qty;
                    newProductVarient.ReorderLevel = i.ReorderLevel;
                    //newProductVarient.StockStatus = i.StockStatus;
                    newProductVarient.PackSize = i.PackSize;
                    newProductVarient.PackUnitID = i.PackUnitID;
                    newProductVarient.MRP = i.MRP;
                    newProductVarient.RetailerRate = i.RetailerRate;
                    newProductVarient.ShopStockID = i.ShopStockID;
                    newProductVarient.ColorName = i.ColorName;
                    //newProductVarient.SizeName = i.SizeName;
                    //newProductVarient.DiamentionName = i.DiamentionName;
                    //newProductVarient.MaterialName = i.MaterialName;
                    //newProductVarient.UnitName = i.UnitName;
                    newProductVarient.IsInclusiveOfTax = i.IsInclusiveOfTax;

                    newProductVarientList.Add(newProductVarient);
                }

                //  ViewBag.list = newProductVarientList;

                productuploadtempviewmodel.NewProductVarientS = newProductVarientList;
                List<SelectList> ColorID = new List<SelectList>();
                List<SelectList> SizeID = new List<SelectList>();
                List<SelectList> DimensionID = new List<SelectList>();
                List<SelectList> MaterialID = new List<SelectList>();
                List<SelectList> PackUnitID = new List<SelectList>();
                //productuploadtempviewmodel.SizeDropDown.Add(new Size (list));
                foreach (var ii in productuploadtempviewmodel.NewProductVarientS)
                {
                    ColorID.Add(new SelectList(db.Colors, "ID", "Name", ii.ColorID));
                    SizeID.Add(new SelectList(db.Sizes, "ID", "Name", ii.SizeID));
                    DimensionID.Add(new SelectList(db.Dimensions, "ID", "Name", ii.DimensionID));
                    MaterialID.Add(new SelectList(db.Materials, "ID", "Name", ii.MaterialID));
                    PackUnitID.Add(new SelectList(db.Units, "ID", "Name", ii.PackUnitID));

                }
                ViewBag.ColorID = ColorID;
                ViewBag.SizeID = SizeID;
                ViewBag.DimensionID = DimensionID;
                ViewBag.MaterialID = MaterialID;
                ViewBag.PackUnitID = PackUnitID;

                return View(productuploadtempviewmodel);

            }


            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadAproved][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadAproved][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }
        //=========================================
        #endregion               

        #region Code for EDITAproveProduct
        //========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanRead")]
        public ActionResult Edit(long id, long? shopID)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            long PersonalDetailID = GetPersonalDetailID();

            try
            {

                ViewBag.ProductID = id;
                ViewBag.textarea = CommonFunctions.LoadDescFile(id, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == shopID).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                Product TP = db.Products.Find(id);
                productuploadtempviewmodel.Name = TP.Name;
                productuploadtempviewmodel.CategoryID = TP.CategoryID;
                productuploadtempviewmodel.ID = TP.ID;

                var pID = (from dd in db.Categories
                           join dd1 in db.Categories on dd.ID equals dd1.ParentCategoryID
                           where dd1.ID == TP.CategoryID
                           select new { dd.ID }).First();
                productuploadtempviewmodel.ddlCategorySecondID = Convert.ToInt32(pID.ID);


                var pID1 = (from dd in db.Categories
                            join dd1 in db.Categories on dd.ID equals dd1.ParentCategoryID
                            where dd1.ID == pID.ID
                            select new { dd.ID }).First();
                productuploadtempviewmodel.ddlCategoryFirstID = Convert.ToInt32(pID1.ID);

                productuploadtempviewmodel.WeightInGram = TP.WeightInGram;
                productuploadtempviewmodel.LengthInCm = TP.LengthInCm;
                productuploadtempviewmodel.BreadthInCm = TP.BreadthInCm;
                productuploadtempviewmodel.HeightInCm = TP.HeightInCm;
                productuploadtempviewmodel.Description = TP.Description;
                productuploadtempviewmodel.BrandID = TP.BrandID;
                productuploadtempviewmodel.SearchKeyword = TP.SearchKeyword.Trim();
                //productuploadtempviewmodel.IsActive = TP.IsActive;
                productuploadtempviewmodel.CreateDate = DateTime.Now;
                productuploadtempviewmodel.CreateBy = PersonalDetailID;
                productuploadtempviewmodel.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                productuploadtempviewmodel.DeviceID = "x";
                productuploadtempviewmodel.DeviceType = "x";

                ShopProduct TSP = db.ShopProducts.Where(x => x.ProductID == id && x.ShopID == shopID).First();
                productuploadtempviewmodel.ShopID = TSP.ShopID;
                productuploadtempviewmodel.ShopProductID = TSP.ID;
                productuploadtempviewmodel.IsActive = TSP.IsActive;
                productuploadtempviewmodel.DisplayProductFromDate = TSP.DisplayProductFromDate;
                productuploadtempviewmodel.DeliveryTime = TSP.DeliveryTime;
                productuploadtempviewmodel.DeliveryRate = TSP.DeliveryRate;
                productuploadtempviewmodel.TaxRate = TSP.TaxRate;
                productuploadtempviewmodel.TaxRatePer = TSP.TaxRatePer;
                productuploadtempviewmodel.CreateDate = DateTime.Now;
                productuploadtempviewmodel.CreateBy = PersonalDetailID;
                productuploadtempviewmodel.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                productuploadtempviewmodel.DeviceID = "x";
                productuploadtempviewmodel.DeviceType = "x";

                if (productuploadtempviewmodel == null)
                {
                    return HttpNotFound();
                }

                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopID
                                && op.Plan.PlanCode.StartsWith("GBMR")
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category, "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", productuploadtempviewmodel.BrandID);
                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);

                var FranchiseID = db.Shops.Where(x => x.ID == shopID).Select(x => x.FranchiseID).FirstOrDefault();
                var TaxIDD = (from TM in db.TaxationMasters
                              join FTD in db.FranchiseTaxDetails on TM.ID equals FTD.TaxationID
                              where FTD.IsActive == true && TM.IsActive == true && FTD.FranchiseID == FranchiseID
                              select new ForLoopClass { Name = TM.Name, ID = TM.ID }).OrderBy(x => x.Name).ToList();

                var query = (from TPV in db.ProductVarients
                             join TSS in db.ShopStocks on TPV.ID equals (TSS.ProductVarientID)
                             join SP in db.ShopProducts on TSS.ShopProductID equals (SP.ID)
                             join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                             join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                             join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                             join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                             join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                             where TPV.ProductID == id && SP.ShopID == shopID
                             select new
                             {
                                 ID = TPV.ProductID,
                                 ProductVarientID = TPV.ID,
                                 ColorID = TPV.ColorID,
                                 SizeID = TPV.SizeID,
                                 DimensionID = TPV.DimensionID,
                                 MaterialID = TPV.MaterialID,
                                 Qty = TSS.Qty,
                                 ReorderLevel = TSS.ReorderLevel,
                                 StockStatus = TSS.StockStatus,
                                 PackSize = TSS.PackSize,
                                 PackUnitID = TSS.PackUnitID,
                                 MRP = TSS.MRP,
                                 RetailerRate = TSS.RetailerRate,
                                 WholeSaleRate = TSS.WholeSaleRate,
                                 SS_IsActive = TSS.IsActive,
                                 IsPriority = TSS.IsPriority,
                                 ShopStockID = TSS.ID,
                                 ColorName = CLR.Name,
                                 SizeName = SIZ.Name,
                                 DiamentionName = DMS.Name,
                                 MaterialName = MTR.Name,
                                 UnitName = UNT.Name,
                                 IsInclusiveOfTax = TSS.IsInclusiveOfTax,
                                 IsActive = TPV.IsActive
                             }).ToList();



                List<NewProductVarient> newProductVarientList = new List<NewProductVarient>();
                foreach (var i in query)
                {
                    NewProductVarient newProductVarient = new ModelLayer.Models.ViewModel.NewProductVarient();
                    newProductVarient.ID = i.ID;
                    newProductVarient.ProductVarientID = i.ProductVarientID;
                    newProductVarient.ColorID = i.ColorID;
                    newProductVarient.SizeID = i.SizeID;
                    newProductVarient.DimensionID = i.DimensionID;
                    newProductVarient.MaterialID = i.MaterialID;
                    newProductVarient.Qty = i.Qty;
                    newProductVarient.ReorderLevel = i.ReorderLevel;
                    newProductVarient.PackSize = i.PackSize;
                    newProductVarient.PackUnitID = i.PackUnitID;
                    newProductVarient.MRP = i.MRP;
                    newProductVarient.RetailerRate = i.RetailerRate;
                    newProductVarient.WholeSaleRate = i.WholeSaleRate;
                    newProductVarient.SS_IsActive = i.SS_IsActive;
                    newProductVarient.IsPriority = i.IsPriority;
                    newProductVarient.ShopStockID = i.ShopStockID;
                    newProductVarient.ColorName = i.ColorName;
                    newProductVarient.IsInclusiveOfTax = i.IsInclusiveOfTax;
                    newProductVarient.IsActive = i.IsActive;
                    newProductVarient.TaxationID = db.ProductTaxes.Where(x => x.ShopStockID == i.ShopStockID && x.IsActive == true).Select(x => x.TaxID).ToList();
                    newProductVarientList.Add(newProductVarient);
                }

                productuploadtempviewmodel.NewProductVarientS = newProductVarientList;
                List<SelectList> ColorID = new List<SelectList>();
                List<SelectList> SizeID = new List<SelectList>();
                List<SelectList> DimensionID = new List<SelectList>();
                List<SelectList> MaterialID = new List<SelectList>();
                List<SelectList> PackUnitID = new List<SelectList>();
                List<MultiSelectList> TaxationID = new List<MultiSelectList>();
                foreach (var ii in productuploadtempviewmodel.NewProductVarientS)
                {
                    ColorID.Add(new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.ColorID));
                    SizeID.Add(new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.SizeID));
                    DimensionID.Add(new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.DimensionID));
                    MaterialID.Add(new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.MaterialID));
                    PackUnitID.Add(new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.PackUnitID));
                    TaxationID.Add(new MultiSelectList(TaxIDD.OrderBy(x => x.Name).ToList(), "ID", "Name", ii.TaxationID.ToArray()));
                    if (ii.ColorID == 1)
                    {
                        ii.ThumbPath = ImageDisplay.SetProductThumbPath(id, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        ii.Path = ImageDisplay.DisplayProductImages(id, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                    }
                    else
                    {
                        var colorName = db.Colors.Where(x => x.ID == ii.ColorID).FirstOrDefault();
                        ii.ThumbPath = ImageDisplay.SetProductThumbPath(id, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        ii.Path = ImageDisplay.DisplayProductImages(id, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                    }
                }
                ViewBag.ColorID = ColorID;
                ViewBag.SizeID = SizeID;
                ViewBag.DimensionID = DimensionID;
                ViewBag.MaterialID = MaterialID;
                ViewBag.PackUnitID = PackUnitID;
                ViewBag.TaxationID = TaxationID;
                //---------------------------------------------------------------------------------------------Existing Variant----------------------------------//


                var query1 = (from TPV in db.ProductVarients
                              //join TSS in db.ShopStocks on TPV.ID equals (TSS.ProductVarientID)
                              //join SP in db.ShopProducts on TSS.ShopProductID equals (SP.ID)
                              join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                              join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                              join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                              join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                              // join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                              where TPV.ProductID == id
                              select new
                              {
                                  ID = TPV.ProductID,
                                  ProductVarientID = TPV.ID,
                                  ColorID = TPV.ColorID,
                                  SizeID = TPV.SizeID,
                                  DimensionID = TPV.DimensionID,
                                  MaterialID = TPV.MaterialID,
                                  //Qty = TSS.Qty,
                                  //ReorderLevel = TSS.ReorderLevel,
                                  //StockStatus = TSS.StockStatus,
                                  //PackSize = TSS.PackSize,
                                  //PackUnitID = TSS.PackUnitID,
                                  //MRP = TSS.MRP,
                                  //RetailerRate = TSS.RetailerRate,
                                  //ShopStockID = TSS.ID,
                                  ColorName = CLR.Name,
                                  SizeName = SIZ.Name,
                                  DiamentionName = DMS.Name,
                                  MaterialName = MTR.Name,
                                  //UnitName = UNT.Name,
                                  // IsInclusiveOfTax = TSS.IsInclusiveOfTax
                              }).ToList();



                List<NewProductVarient> newProductVarientList1 = new List<NewProductVarient>();
                foreach (var i in query1)
                {
                    NewProductVarient newProductVarient1 = new ModelLayer.Models.ViewModel.NewProductVarient();
                    newProductVarient1.ID = i.ID;
                    newProductVarient1.ProductVarientID = i.ProductVarientID;
                    newProductVarient1.ColorID = i.ColorID;
                    newProductVarient1.SizeID = i.SizeID;
                    newProductVarient1.DimensionID = i.DimensionID;
                    newProductVarient1.MaterialID = i.MaterialID;
                    //newProductVarient.Qty = i.Qty;
                    //newProductVarient.ReorderLevel = i.ReorderLevel;
                    ////newProductVarient.StockStatus = i.StockStatus;
                    //newProductVarient.PackSize = i.PackSize;
                    //newProductVarient.PackUnitID = i.PackUnitID;
                    //newProductVarient.MRP = i.MRP;
                    //newProductVarient.RetailerRate = i.RetailerRate;
                    //newProductVarient.ShopStockID = i.ShopStockID;
                    //newProductVarient.ColorName = i.ColorName;
                    ////newProductVarient.SizeName = i.SizeName;
                    ////newProductVarient.DiamentionName = i.DiamentionName;
                    ////newProductVarient.MaterialName = i.MaterialName;
                    ////newProductVarient.UnitName = i.UnitName;
                    //newProductVarient.IsInclusiveOfTax = i.IsInclusiveOfTax;

                    newProductVarientList1.Add(newProductVarient1);
                }
                List<NewProductVarient> newProductVarientList3 = new List<NewProductVarient>();
                newProductVarientList3 = (from lst1 in newProductVarientList1
                                          where !newProductVarientList.Any(
                                          x => x.ProductVarientID == lst1.ProductVarientID
                                           )
                                          select lst1).ToList();

                //list1 = lst.ToList();
                //var result5 = newProductVarientList.Except(newProductVarientList1);
                //var result6 = newProductVarientList1.Except(newProductVarientList);

                productuploadtempviewmodel.NewProductVarientPOP = newProductVarientList3;
                List<SelectList> ColorID1 = new List<SelectList>();
                List<SelectList> SizeID1 = new List<SelectList>();
                List<SelectList> DimensionID1 = new List<SelectList>();
                List<SelectList> MaterialID1 = new List<SelectList>();
                List<SelectList> PackUnitID1 = new List<SelectList>();

                foreach (var ii in productuploadtempviewmodel.NewProductVarientPOP)
                {
                    ColorID1.Add(new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.ColorID));
                    SizeID1.Add(new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.SizeID));
                    DimensionID1.Add(new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.DimensionID));
                    MaterialID1.Add(new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.MaterialID));
                    PackUnitID1.Add(new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.PackUnitID));
                    if (ii.ColorID == 1)
                    {
                        ii.ThumbPath = ImageDisplay.SetProductThumbPath(id, "default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        ii.Path = ImageDisplay.DisplayProductImages(id, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);

                    }
                    else
                    {

                        var colorName = db.Colors.Where(x => x.ID == ii.ColorID).FirstOrDefault();
                        ii.ThumbPath = ImageDisplay.SetProductThumbPath(id, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        ii.Path = ImageDisplay.DisplayProductImages(id, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);

                    }
                }
                ViewBag.ColorID1 = ColorID1;
                ViewBag.SizeID1 = SizeID1;
                ViewBag.DimensionID1 = DimensionID1;
                ViewBag.MaterialID1 = MaterialID1;
                ViewBag.PackUnitID1 = PackUnitID1;
                //-----------------------------------------------------------------------------------------------------------------------------------------------//

                return View(productuploadtempviewmodel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadAproved][GET:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadAproved][GET:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,DeliveryTime,DeliveryRate,TaxRate,TaxRatePer,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,NewProductVarientS,NewProductVarientPOP,CategoryL_2,Path,pathValue")] ProductUploadTempViewModel productuploadtempviewmodel, List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1, List<HttpPostedFileBase> files_2, List<HttpPostedFileBase> files_3, List<HttpPostedFileBase> files_4, List<HttpPostedFileBase> files_5, List<HttpPostedFileBase> files_6, List<HttpPostedFileBase> files_7, List<HttpPostedFileBase> files_8, List<HttpPostedFileBase> files_9, List<HttpPostedFileBase> files_10, List<HttpPostedFileBase> files_11, List<HttpPostedFileBase> files_12, List<HttpPostedFileBase> files_13, List<HttpPostedFileBase> files_14, List<HttpPostedFileBase> files_15,
        FormCollection collection, string submit, string DisplayProductFromDate1, string textarea, List<CategorySpecificationList> categorySpecificationList)
        {

            List<List<HttpPostedFileBase>> files = new List<List<HttpPostedFileBase>>();
            files.Add(files_0); files.Add(files_1); files.Add(files_2); files.Add(files_3); files.Add(files_4);
            files.Add(files_5); files.Add(files_6); files.Add(files_7); files.Add(files_8); files.Add(files_9);
            files.Add(files_10); files.Add(files_11); files.Add(files_12); files.Add(files_13); files.Add(files_14); files.Add(files_15);

            string StrInclusiveOfTax = collection["hdnIsInclusiveOfTax"].ToString();
            string[] NewinclusiveOfTax = StrInclusiveOfTax.Split('&');

            string thumbnail = collection["ThumbIndex"].ToString();
            string strValue = submit;
            string[] strTemp = strValue.Split('$');
            var val1 = strTemp[0];
            submit = val1.ToString();

            try
            {
                ViewBag.ProductID = productuploadtempviewmodel.ID;
                long PersonalDetailID = GetPersonalDetailID();
                DateTime lDisplayProductFromDate = CommonFunctions.GetProperDateTime(DisplayProductFromDate1);
                productuploadtempviewmodel.DisplayProductFromDate = lDisplayProductFromDate;
                
                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == productuploadtempviewmodel.ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR")
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();
                ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == productuploadtempviewmodel.ShopID).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                switch (submit)
                {
                    case "Save":
                        using (TransactionScope ts = new TransactionScope())
                        {
                            try
                            {
                                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                                ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ColorID);
                                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.SizeID);
                                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.DimensionID);
                                ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.MaterialID);
                                ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.PackUnitID);

                                EzeeloDBContext db1 = new EzeeloDBContext();

                                Product TP = db1.Products.Find(productuploadtempviewmodel.ID);
                                TP.Name = productuploadtempviewmodel.Name;
                                TP.CategoryID = productuploadtempviewmodel.CategoryID;
                                TP.WeightInGram = productuploadtempviewmodel.WeightInGram;
                                TP.LengthInCm = productuploadtempviewmodel.LengthInCm;
                                TP.BreadthInCm = productuploadtempviewmodel.BreadthInCm;
                                TP.HeightInCm = productuploadtempviewmodel.HeightInCm;
                                TP.Description = productuploadtempviewmodel.Description;
                                TP.BrandID = productuploadtempviewmodel.BrandID;
                                TP.SearchKeyword = ExtraSpaceRemoveFromString(productuploadtempviewmodel.SearchKeyword);
                                TP.IsActive = true;                                 // productuploadtempviewmodel.IsActive;
                                TP.CreateDate = TP.CreateDate;
                                TP.CreateBy = TP.CreateBy;
                                TP.ModifyDate = DateTime.UtcNow;
                                TP.ModifyBy = PersonalDetailID;
                                TP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                TP.DeviceID = "x";
                                TP.DeviceType = "x";
                                db1.SaveChanges();

                                List<ProductSpecification> TPS = new List<ProductSpecification>();
                                TPS = db1.ProductSpecifications.Where(x => x.ProductID == TP.ID).ToList();
                                db1.ProductSpecifications.RemoveRange(TPS);
                                db1.SaveChanges();
                                if (categorySpecificationList != null)
                                {
                                    foreach (CategorySpecificationList clst in categorySpecificationList)
                                    {
                                        ProductSpecification TPS1 = new ProductSpecification();

                                        TPS1.ProductID = TP.ID;
                                        TPS1.SpecificationID = clst.SpecificationID;
                                        TPS1.Value = clst.SpecificationValue != null ? clst.SpecificationValue : "N/A";
                                        TPS1.IsActive = true;
                                        TPS1.CreateDate = DateTime.UtcNow;
                                        TPS1.CreateBy = PersonalDetailID;
                                        TPS1.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                        TPS1.DeviceType = "x";
                                        TPS1.DeviceID = "x";

                                        db.ProductSpecifications.Add(TPS1);
                                        db.SaveChanges();
                                    }
                                }


                                ShopProduct TSP = db1.ShopProducts.Where(x => x.ProductID == productuploadtempviewmodel.ID && x.ShopID == productuploadtempviewmodel.ShopID).First();
                                TSP.ShopID = productuploadtempviewmodel.ShopID;
                                TSP.ProductID = TP.ID;
                                TSP.IsActive = productuploadtempviewmodel.IsActive;//--------------------------------------- productuploadtempviewmodel.IsActive;
                                TSP.DisplayProductFromDate = productuploadtempviewmodel.DisplayProductFromDate;
                                TSP.DeliveryTime = productuploadtempviewmodel.DeliveryTime;
                                TSP.DeliveryRate = productuploadtempviewmodel.DeliveryRate;
                                TSP.TaxRate = productuploadtempviewmodel.TaxRate;
                                TSP.TaxRatePer = productuploadtempviewmodel.TaxRatePer;
                                TSP.CreateDate = TSP.CreateDate;
                                TSP.CreateBy = TSP.CreateBy;
                                TSP.ModifyDate = DateTime.UtcNow;
                                TSP.ModifyBy = PersonalDetailID;
                                TSP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                TSP.DeviceID = "x";
                                TSP.DeviceType = "x";
                                db1.SaveChanges();

                                int count = 0;
                                using (EzeeloDBContext dc = new EzeeloDBContext())
                                {
                                    foreach (var i in productuploadtempviewmodel.NewProductVarientS)
                                    {
                                        List<HttpPostedFileBase> MyFiles = new List<HttpPostedFileBase>();
                                        MyFiles = files[count]; count++;

                                        if (i.ProductVarientID == 0)
                                        {                                            
                                            int count1 = (from p in db.ProductVarients
                                                         join st in db.ShopStocks
                                                             on p.ID equals st.ProductVarientID
                                                         where p.ColorID == i.ColorID
                                                                && p.DimensionID == i.DimensionID
                                                                && p.SizeID == i.SizeID
                                                                && p.MaterialID == i.MaterialID
                                                                && p.ProductID == TP.ID
                                                         select p).Count();


                                            if (count1 == 0)//For update
                                            {
                                                ProductVarient tempProductVarient = new ProductVarient();
                                                tempProductVarient.ProductID = TP.ID;
                                                tempProductVarient.ColorID = i.ColorID;
                                                tempProductVarient.DimensionID = i.DimensionID;
                                                tempProductVarient.SizeID = i.SizeID;
                                                tempProductVarient.MaterialID = i.MaterialID;
                                                tempProductVarient.IsActive = false;
                                                tempProductVarient.CreateDate = DateTime.Now;
                                                tempProductVarient.CreateBy = PersonalDetailID;
                                                tempProductVarient.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                tempProductVarient.DeviceID = "x";
                                                tempProductVarient.DeviceType = "x";
                                                dc.ProductVarients.Add(tempProductVarient);
                                                dc.SaveChanges();

                                                ShopStock tempShopStock = new ShopStock();
                                                tempShopStock.ShopProductID = TSP.ID;
                                                tempShopStock.ProductVarientID = tempProductVarient.ID;
                                                tempShopStock.Qty = i.Qty;
                                                tempShopStock.ReorderLevel = i.ReorderLevel;
                                                tempShopStock.StockStatus = true;
                                                tempShopStock.PackSize = i.PackSize;
                                                tempShopStock.PackUnitID = 1;// i.PackUnitID;
                                                tempShopStock.MRP = i.MRP;
                                                tempShopStock.RetailerRate = i.RetailerRate;
                                                tempShopStock.WholeSaleRate = i.WholeSaleRate;
                                                tempShopStock.IsInclusiveOfTax = TaxationManagement.GetTaxStatus(NewinclusiveOfTax[count]);
                                                tempShopStock.IsActive = true;
                                                tempShopStock.IsPriority = i.IsPriority;
                                                tempShopStock.CreateDate = DateTime.Now;
                                                tempShopStock.CreateBy = PersonalDetailID;
                                                tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                tempShopStock.DeviceID = "x";
                                                tempShopStock.DeviceType = "x";
                                                dc.ShopStocks.Add(tempShopStock);
                                                dc.SaveChanges();

                                                if (i.TaxationID != null)
                                                {
                                                    foreach (var taxID in i.TaxationID)
                                                    {
                                                        ProductTax ProductTax = new ProductTax();
                                                        ProductTax.ShopStockID = tempShopStock.ID;
                                                        ProductTax.TaxID = Convert.ToInt32(taxID);
                                                        ProductTax.IsActive = true;
                                                        ProductTax.CreateDate = DateTime.Now;
                                                        ProductTax.CreateBy = GetPersonalDetailID();
                                                        ProductTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                        ProductTax.DeviceID = "x";
                                                        ProductTax.DeviceType = "x";
                                                        db.ProductTaxes.Add(ProductTax);
                                                        db.SaveChanges();

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ShopStock tempShopStock = new ShopStock();
                                                tempShopStock.ShopProductID = TSP.ID;
                                                tempShopStock.ProductVarientID = (from p in db.ProductVarients
                                                                                  join st in db.ShopStocks
                                                                                      on p.ID equals st.ProductVarientID
                                                                                  where p.ColorID == i.ColorID
                                                                                         && p.DimensionID == i.DimensionID
                                                                                         && p.SizeID == i.SizeID
                                                                                         && p.MaterialID == i.MaterialID
                                                                                         && p.ProductID == TP.ID
                                                                                  select p.ID).FirstOrDefault();
                                                tempShopStock.Qty = i.Qty;
                                                tempShopStock.ReorderLevel = i.ReorderLevel;
                                                tempShopStock.StockStatus = true;
                                                tempShopStock.PackSize = i.PackSize;
                                                tempShopStock.PackUnitID = 1;// i.PackUnitID;
                                                tempShopStock.MRP = i.MRP;
                                                tempShopStock.RetailerRate = i.RetailerRate;
                                                tempShopStock.WholeSaleRate = i.WholeSaleRate;
                                                tempShopStock.IsInclusiveOfTax = TaxationManagement.GetTaxStatus(NewinclusiveOfTax[count]);
                                                tempShopStock.IsActive = i.SS_IsActive;
                                                tempShopStock.IsPriority = i.IsPriority;
                                                tempShopStock.ModifyDate = DateTime.Now;
                                                tempShopStock.ModifyBy = PersonalDetailID;
                                                tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                tempShopStock.DeviceID = "x";
                                                tempShopStock.DeviceType = "x";
                                                dc.ShopStocks.Add(tempShopStock);
                                                dc.SaveChanges();

                                                if (i.TaxationID != null)
                                                {
                                                    foreach (var taxID in i.TaxationID)
                                                    {
                                                        ProductTax ProductTax = new ProductTax();
                                                        ProductTax.ShopStockID = tempShopStock.ID;
                                                        ProductTax.TaxID = Convert.ToInt32(taxID);
                                                        ProductTax.IsActive = true;
                                                        ProductTax.CreateDate = DateTime.Now;
                                                        ProductTax.CreateBy = GetPersonalDetailID();
                                                        ProductTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                        ProductTax.DeviceID = "x";
                                                        ProductTax.DeviceType = "x";
                                                        db.ProductTaxes.Add(ProductTax);
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }

                                            try
                                            {
                                                if (i.ColorID == 1)
                                                {
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved, -1);
                                                }
                                                else
                                                {
                                                    var colorName = db.Colors.Where(x => x.ID == i.ColorID).FirstOrDefault();
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved, -1);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }

                                            try
                                            {
                                                string thumbIndexDB = IsThumbnailDB(thumbnail, (i.ProductVarientID).ToString());
                                                if (!string.IsNullOrEmpty(thumbIndexDB))
                                                {
                                                    CommonFunctions.EditProductThumb(TP.ID, thumbIndexDB);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                        }
                                        else
                                        {
                                            //code to find is same varent is using by other mearchent
                                            
                                            ProductVarient tempProductVarient = dc.ProductVarients.Find(i.ProductVarientID);
                                            tempProductVarient.ProductID = TP.ID;
                                            tempProductVarient.ColorID = i.ColorID;
                                            tempProductVarient.DimensionID = i.DimensionID;
                                            tempProductVarient.SizeID = i.SizeID;
                                            tempProductVarient.MaterialID = i.MaterialID;
                                           // tempProductVarient.IsActive = true;
                                            tempProductVarient.CreateDate = DateTime.Now;
                                            tempProductVarient.CreateBy = PersonalDetailID;
                                            tempProductVarient.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempProductVarient.DeviceID = "x";
                                            tempProductVarient.DeviceType = "x";
                                            //dc.TempProductVarients.Add(tempProductVarient);
                                            dc.SaveChanges();

                                            ShopStock tempShopStock = dc.ShopStocks.Find(i.ShopStockID);
                                            tempShopStock.ShopProductID = TSP.ID;
                                            tempShopStock.ProductVarientID = tempProductVarient.ID;
                                            tempShopStock.Qty = i.Qty;
                                            tempShopStock.ReorderLevel = i.ReorderLevel;
                                            tempShopStock.StockStatus = true;
                                            tempShopStock.PackSize = i.PackSize;
                                            tempShopStock.PackUnitID = 1;// i.PackUnitID;
                                            tempShopStock.MRP = i.MRP;
                                            tempShopStock.WholeSaleRate = i.WholeSaleRate;
                                            tempShopStock.RetailerRate = i.RetailerRate;
                                            tempShopStock.IsInclusiveOfTax = TaxationManagement.GetTaxStatus(NewinclusiveOfTax[count]);
                                            tempShopStock.IsActive = i.SS_IsActive;
                                            tempShopStock.IsPriority = i.IsPriority;
                                            tempShopStock.ModifyDate = DateTime.Now;
                                            tempShopStock.ModifyBy = PersonalDetailID;
                                            tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempShopStock.DeviceID = "x";
                                            tempShopStock.DeviceType = "x";
                                            dc.SaveChanges();

                                            try
                                            {
                                                if (i.ColorID == 1)
                                                {
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved, -1);
                                                }
                                                else
                                                {
                                                    var colorName = db.Colors.Where(x => x.ID == i.ColorID).FirstOrDefault();
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved, -1);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }

                                            try
                                            {
                                                string thumbIndexDB = IsThumbnailDB(thumbnail, (i.ProductVarientID).ToString());
                                                if (!string.IsNullOrEmpty(thumbIndexDB))
                                                {
                                                    CommonFunctions.EditProductThumb(TP.ID, thumbIndexDB);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Edit]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }

                                            if (i.TaxationID != null)
                                            {
                                                this.DeleteVariantTax(i.ShopStockID);
                                                foreach (var taxID in i.TaxationID)
                                                {
                                                    long PrevTaxID = db.ProductTaxes.Where(x => x.ShopStockID == i.ShopStockID && x.TaxID == taxID).Select(x => x.TaxID).FirstOrDefault();
                                                    if (taxID == PrevTaxID)
                                                    {
                                                        ProductTax productTax = db.ProductTaxes.Where(x => x.TaxID == taxID && x.ShopStockID == i.ShopStockID).FirstOrDefault();
                                                        productTax.IsActive = true;
                                                        db.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        ProductTax productTax = new ProductTax();
                                                        productTax.ShopStockID = i.ShopStockID;
                                                        productTax.TaxID = Convert.ToInt32(taxID);
                                                        productTax.IsActive = true;
                                                        productTax.CreateDate = DateTime.Now;
                                                        productTax.CreateBy = GetPersonalDetailID();
                                                        productTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                        productTax.DeviceID = "x";
                                                        productTax.DeviceType = "x";
                                                        db.ProductTaxes.Add(productTax);
                                                        db.SaveChanges();
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    if (productuploadtempviewmodel.NewProductVarientPOP != null)
                                    {
                                        foreach (var i in productuploadtempviewmodel.NewProductVarientPOP)
                                        {
                                            try
                                            {
                                                if (i.IsSelect == true)
                                                {
                                                    ProductVarient tempProductVarient = dc.ProductVarients.Find(i.ProductVarientID);

                                                    ShopStock tempShopStock = new ShopStock();
                                                    tempShopStock.ShopProductID = TSP.ID;
                                                    tempShopStock.ProductVarientID = tempProductVarient.ID;
                                                    tempShopStock.Qty = i.Qty;
                                                    tempShopStock.ReorderLevel = i.ReorderLevel;
                                                    tempShopStock.StockStatus = true;
                                                    tempShopStock.PackSize = i.PackSize;
                                                    tempShopStock.PackUnitID = 1;// i.PackUnitID;
                                                    tempShopStock.MRP = i.MRP;
                                                    tempShopStock.WholeSaleRate = i.WholeSaleRate;
                                                    tempShopStock.RetailerRate = i.RetailerRate;
                                                    tempShopStock.IsInclusiveOfTax = i.IsInclusiveOfTax;
                                                    tempShopStock.IsActive = true;
                                                    tempShopStock.CreateDate = DateTime.Now;
                                                    tempShopStock.CreateBy = PersonalDetailID;
                                                    tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                    tempShopStock.DeviceID = "x";
                                                    tempShopStock.DeviceType = "x";
                                                    dc.ShopStocks.Add(tempShopStock);
                                                    dc.SaveChanges();
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the Existing Variant!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadAproved][POST:Edit]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the Existing Variant!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadAproved][POST:Edit]",
                                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                            }


                                        }
                                    }
                                }
                                try
                                {
                                    CommonFunctions.UploadDescFile(TP.ID, textarea, ProductUpload.IMAGE_TYPE.Approved);
                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    ModelState.AddModelError("Error", "There's Something wrong with the description file!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {

                                    ModelState.AddModelError("Error", "There's Something wrong with the description file!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[ProductUploadTemp][POST:Edit]",
                                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                }
                                ts.Complete();
                            }
                            catch (BusinessLogicLayer.MyException myEx)
                            {
                                ModelState.AddModelError("Error", "There's Something wrong with the Product!!");

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                    + "[ProductUploadAproved][POST:Edit]" + myEx.EXCEPTION_PATH,
                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                ts.Dispose();
                                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                //ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                                //ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
                                ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).ToList(), "ID", "Name");
                                TempData["MyErrorMsg"] = "Some Error occured! Product is not Updated! ";
                                TempData.Keep();
                                // return RedirectToAction("Edit", "ProductUploadTemp");
                                return View(productuploadtempviewmodel);
                            }
                            catch (Exception ex)
                            {

                                ModelState.AddModelError("Error", "There's Something wrong with the product!!");

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                    + "[ProductUploadAproved][POST:Edit]",
                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                ts.Dispose();
                                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                                //ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                                //ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
                                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");
                                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name");
                                ViewBag.ComponentUnitID = new SelectList(db.Units, "ID", "Name");
                                TempData["MyErrorMsg"] = "Some Error occured! Product is not Updated! ";
                                TempData.Keep();
                                //return RedirectToAction("Create", "ProductUploadTemp");
                                return View(productuploadtempviewmodel);
                            }

                        }

                        return RedirectToAction("Index", new { shopId = productuploadtempviewmodel.ShopID });

                    case "Remove":
                        long ColorID = Convert.ToInt64(strTemp[2]);
                        string Path = strTemp[3];
                        string ProductName = db.Products.Where(x => x.ID == productuploadtempviewmodel.ID).FirstOrDefault().Name;
                        if (ColorID == 1)
                        {
                            CommonFunctions.DeleteProductImages(Path, productuploadtempviewmodel.ID, ProductName, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        }
                        else
                        {
                            var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
                            CommonFunctions.DeleteProductImages(Path, productuploadtempviewmodel.ID, ProductName, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        }

                        return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopID = productuploadtempviewmodel.ShopID });

                    case "Default":
                        return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopID = productuploadtempviewmodel.ShopID });                                      
                } 
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadAproved][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadAproved][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");

            }
            return RedirectToAction("Index", new { ShopId = productuploadtempviewmodel.ShopID });
            //return View(productuploadtempviewmodel);
        }
        //========================================
        #endregion       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Method
        //===========================================
        private string ExtraSpaceRemoveFromString(string String) //Removing extra space---Used in Create page....Apply in "Search Keywords"
        {
            if (String != string.Empty)
            {
                String = System.Text.RegularExpressions.Regex.Replace(String, @"\s+", " ");
                String = String.Replace(" , ", ",");
                String = String.Replace(" ,", ",");
                String = String.Replace(", ", ",");
                String = String.TrimStart();
                String = String.TrimEnd();
            }
            return String;
        }

        public ActionResult DownloadFile()
        {
            return Redirect("ftp://192.168.1.106/Content/FileDownload/Product.docx");
        }

        private string IsThumbnailDB(string val, string ProductVariantId)
        {
            try
            {

                //string checkval = "MainTr_" + rowIndex;
                string ThumbSource = "";
                if (val.Contains(ProductVariantId))
                {
                    string[] a = val.Split('$');
                    foreach (string element in a)
                    {
                        if (element != string.Empty)
                        {
                            string[] b = element.Split('#');
                            if (b[0] == ProductVariantId)
                            {
                                ThumbSource = b[1];
                            }
                        }

                    }

                }
                return ThumbSource;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Fills and returns the jSon Result for category specification 
        /// </summary>
        /// <param name="categoryID">Selected Catgeory ID for which specification to be dipalyed</param>
        /// <returns>Returns the JSon result for category specification</returns>
        public JsonResult SpecificationList(int categoryID, int? productID)
        {
            List<CategorySpecificationList> objActualspecification = new List<CategorySpecificationList>();

            DataTable dt = new DataTable();
            dt = BusinessLogicLayer.CategorySpecificationListClass.Select_ProductSpecificationAfterApproved(productID, categoryID, System.Web.HttpContext.Current.Server);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CategorySpecificationList objCategorySpecifcation = new CategorySpecificationList();
                int SpecificationID = Convert.ToInt32(dt.Rows[i]["SpecificationID"].ToString());
                int ParentID = dt.Rows[i]["ParentID"].ToString() == string.Empty ? 0 : Convert.ToInt32(dt.Rows[i]["ParentID"].ToString());
                if (ParentID == 0 && Convert.ToInt32(dt.Rows[i]["sp_Level"].ToString()) == 1)
                {

                    objCategorySpecifcation.SpecificationID = Convert.ToInt32(dt.Rows[i]["SpecificationID"].ToString());
                    objCategorySpecifcation.ParentID = ParentID;
                    objCategorySpecifcation.level = Convert.ToInt32(dt.Rows[i]["sp_Level"].ToString());
                    objCategorySpecifcation.SpecificationName = dt.Rows[i]["SpecificationName"].ToString();
                    objCategorySpecifcation.SpecificationValue = dt.Rows[i]["SpecificationValue"].ToString();
                    objActualspecification.Add(objCategorySpecifcation);
                }
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    CategorySpecificationList objCategorySpecifcation1 = new CategorySpecificationList();
                    int ChildParentID = dt.Rows[j]["ParentID"].ToString() == string.Empty ? 0 : Convert.ToInt32(dt.Rows[j]["ParentID"].ToString());
                    if (ChildParentID > 0 && SpecificationID == ChildParentID && Convert.ToInt32(dt.Rows[j]["sp_Level"].ToString()) > 1)
                    {
                        objCategorySpecifcation1.SpecificationID = Convert.ToInt32(dt.Rows[j]["SpecificationID"].ToString());
                        objCategorySpecifcation1.ParentID = ChildParentID;
                        objCategorySpecifcation1.level = Convert.ToInt32(dt.Rows[j]["sp_Level"].ToString());
                        objCategorySpecifcation1.SpecificationName = dt.Rows[j]["SpecificationName"].ToString();
                        objCategorySpecifcation1.SpecificationValue = dt.Rows[j]["SpecificationValue"].ToString();
                        objActualspecification.Add(objCategorySpecifcation1);
                    }
                }
            }


            return Json(objActualspecification, JsonRequestBehavior.AllowGet);
        }

        // this code added by prashant to fill the category specificatioon

        /// <summary>
        /// Fills and returns the jSon Result for category specification 
        /// </summary>
        /// <param name="categoryID">Selected Catgeory ID for which specification to be dipalyed</param>
        /// <returns>Returns the JSon result for category specification</returns>
        public JsonResult ProductSpecification(int productID)
        {          

            Int64 categoryID = db.Products.Where(x => x.ID == productID).FirstOrDefault().CategoryID;

            List<CategorySpecificationList> objActualspecification = new List<CategorySpecificationList>();

            DataTable dt = new DataTable();
            dt = BusinessLogicLayer.CategorySpecificationListClass.Select_ProductSpecificationAfterApproved(productID, categoryID, System.Web.HttpContext.Current.Server);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CategorySpecificationList objCategorySpecifcation = new CategorySpecificationList();
                int SpecificationID = Convert.ToInt32(dt.Rows[i]["SpecificationID"].ToString());
                int ParentID = dt.Rows[i]["ParentID"].ToString() == string.Empty ? 0 : Convert.ToInt32(dt.Rows[i]["ParentID"].ToString());
                if (ParentID == 0 && Convert.ToInt32(dt.Rows[i]["sp_Level"].ToString()) == 1)
                {

                    objCategorySpecifcation.SpecificationID = Convert.ToInt32(dt.Rows[i]["SpecificationID"].ToString());
                    objCategorySpecifcation.ParentID = ParentID;
                    objCategorySpecifcation.level = Convert.ToInt32(dt.Rows[i]["sp_Level"].ToString());
                    objCategorySpecifcation.SpecificationName = dt.Rows[i]["SpecificationName"].ToString();
                    objCategorySpecifcation.SpecificationValue = dt.Rows[i]["SpecificationValue"].ToString();
                    objActualspecification.Add(objCategorySpecifcation);
                }
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    CategorySpecificationList objCategorySpecifcation1 = new CategorySpecificationList();
                    int ChildParentID = dt.Rows[j]["ParentID"].ToString() == string.Empty ? 0 : Convert.ToInt32(dt.Rows[j]["ParentID"].ToString());
                    if (ChildParentID > 0 && SpecificationID == ChildParentID && Convert.ToInt32(dt.Rows[j]["sp_Level"].ToString()) > 1)
                    {
                        objCategorySpecifcation1.SpecificationID = Convert.ToInt32(dt.Rows[j]["SpecificationID"].ToString());
                        objCategorySpecifcation1.ParentID = ChildParentID;
                        objCategorySpecifcation1.level = Convert.ToInt32(dt.Rows[j]["sp_Level"].ToString());
                        objCategorySpecifcation1.SpecificationName = dt.Rows[j]["SpecificationName"].ToString();
                        objCategorySpecifcation1.SpecificationValue = dt.Rows[j]["SpecificationValue"].ToString();
                        objActualspecification.Add(objCategorySpecifcation1);
                    }
                }
            }


            return Json(objActualspecification, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetImagesWithThumb(long productID, long ColorID)
        {

            if (ColorID == 1)
            {
                string thumbPath = ImageDisplay.SetProductThumbPath(productID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                string[] Result = ImageDisplay.DisplayProductImages(productID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                Array.Resize(ref Result, Result.Length + 1);
                Result[Result.Length - 1] = thumbPath;
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
                string thumbPath = ImageDisplay.SetProductThumbPath(productID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                string[] Result = ImageDisplay.DisplayProductImages(productID, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                Array.Resize(ref Result, Result.Length + 1);
                Result[Result.Length - 1] = thumbPath;
                // Result.Add((ImgPaths);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }

        }
        
        public JsonResult GetCategoryLevel1ByParentCategory(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == categoryID).ToList();
            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.Distinct().OrderBy(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryLevel2ByCategoryLevel1(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            lCategory = db.Categories.Where(x => x.ParentCategoryID == categoryID).ToList();
            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }

            return Json(forloopclasses.Distinct().OrderBy(x => x.Name).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSecondLevelCategory(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            var ParentCat = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault();
            lCategory = db.Categories.Where(x => x.ID == ParentCat.ParentCategoryID).ToList();
            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFirstLevelCategory(int categoryID)
        {
            List<Category> lCategory = new List<Category>();
            List<ForLoopClass> forloopclasses = new List<ForLoopClass>();
            var ParentCat = db.Categories.Where(x => x.ID == categoryID).FirstOrDefault();
            lCategory = db.Categories.Where(x => x.ID == ParentCat.ParentCategoryID).ToList();
            foreach (var c in lCategory)
            {
                ForLoopClass av = new ForLoopClass();
                av.ID = c.ID;
                av.Name = c.Name;
                forloopclasses.Add(av);
            }
            return Json(forloopclasses.ToList(), JsonRequestBehavior.AllowGet);
        }

        private void DeleteVariantTax(long ShopStockID)
        {
            var ID = db.ProductTaxes.Where(x => x.ShopStockID == ShopStockID).Select(x => x.ID).ToList();
            foreach (var item in ID)
            {
                ProductTax productTax = db.ProductTaxes.Find(item);
                productTax.IsActive = false;
                db.SaveChanges();
            }

        }

        #endregion

    }
}
