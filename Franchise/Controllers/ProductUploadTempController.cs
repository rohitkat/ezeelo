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
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

    public class ForLoopClass //----------------use this class for loop purpose in below functions----------------
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }

    public class ProductUploadTempController : Controller
    {

        #region Genral Code

        private EzeeloDBContext db = new EzeeloDBContext();
        private static long ShopID1 = 0;
        private long GetPersonalDetailID()
        {
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

        #region Code for Index

        //========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanRead")]
        public ActionResult MerchantList()
        {
            try
            {
                int franchiseID = Convert.ToInt32(Session["FRANCHISE_ID"]);
                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                List<ShopViewModel> lShop = (from s in db.Shops
                                             where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR" &&
                                             s.FranchiseID == franchiseID
                                             select new ShopViewModel
                                             {
                                                 ID = s.ID,
                                                 Name = s.Name,
                                                 FranchiseID = s.FranchiseID,
                                                 NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                                             }).ToList();

                return View(lShop);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTempController][GET:MerchantList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTempController][GET:MerchantList]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();
        }


        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanRead")]
        public ActionResult Index1(long shopId)
        {
            try
            {
                ViewBag.ShopId = shopId;
                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                var QueryResult = (from TP in db.TempProducts
                                   join TPV in db.TempProductVarients on new { ID = TP.ID } equals new { ID = TPV.ProductID }
                                   join TSP in db.TempShopProducts on new { ID = TP.ID } equals new { ID = TSP.ProductID }
                                   join TSS in db.TempShopStocks on new { ID = TPV.ID } equals new { ID = TSS.ProductVarientID }
                                   join CTG in db.Categories on new { CategoryID = TP.CategoryID } equals new { CategoryID = CTG.ID }
                                   join BRD in db.Brands on new { BrandID = TP.BrandID } equals new { BrandID = BRD.ID }
                                   where
                                     TSP.ShopID == shopId && TP.ApprovalStatus != approvalStatus
                                   group new { TSS, TP, CTG, BRD, TPV, TSP } by new
                                  {
                                      ShopID = shopId,
                                      TSS.ShopProductID,
                                      TP.Name,
                                      Column1 = CTG.Name,
                                      Column2 = BRD.Name,
                                      TP.ID,
                                      TP.ApprovalStatus,
                                      TP.ApprovalRemark
                                  } into g
                                   select new ProductUploadViewModel
                                   {
                                       Qty = g.Sum(p => p.TSS.Qty),
                                       ProductVarientID = g.Count(p => p.TPV.ID != null),
                                       ShopProductID = g.Key.ShopProductID,
                                       ProductName = g.Key.Name,
                                       CategoryName = g.Key.Column1,
                                       BrandName = g.Key.Column2,
                                       ProductID = g.Key.ID,
                                       ShopID = g.Key.ShopID,
                                       ApprovalStatus = g.Key.ApprovalStatus,
                                       ApprovalRemark = g.Key.ApprovalRemark,
                                       ColorID = g.Max(p => p.TPV.ColorID)
                                   }).OrderByDescending(x => x.ProductID).ToList();
                List<ProductUploadTempViewModel> listProductUploadTemp = new List<ProductUploadTempViewModel>();

                foreach (var ReadRecord in QueryResult)
                {
                    ProductUploadTempViewModel putvm = new ProductUploadTempViewModel();
                    putvm.ShopID = ReadRecord.ShopID;
                    putvm.ID = ReadRecord.ProductID;
                    putvm.CategoryName = ReadRecord.CategoryName;
                    putvm.BrandName = ReadRecord.BrandName;
                    putvm.Name = ReadRecord.ProductName;
                    putvm.Qty = ReadRecord.Qty;
                    putvm.PackSize = ReadRecord.PackSize;
                    putvm.ProductVarientID = ReadRecord.ProductVarientID;
                    putvm.ApprovalRemark = ReadRecord.ApprovalRemark;
                    putvm.ApprovalStatus = ReadRecord.ApprovalStatus;
                    putvm.ColorID = ReadRecord.ColorID;
                    if (ReadRecord.ColorID == 1)
                    {
                        putvm.ImageLocation = ImageDisplay.SetProductThumbPath(ReadRecord.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                    }
                    else
                    {
                        putvm.ImageLocation = ImageDisplay.SetProductThumbPath(ReadRecord.ProductID, db.Colors.Where(x => x.ID == ReadRecord.ColorID).FirstOrDefault().Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
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
                    + "[ProductUploadTemp][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product Upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return View();

        }

        //=========================================
        #endregion

        //Author:Harshada
        #region Code for Component


        [SessionExpire]
        //[CustomAuthorize(Roles = "PaymentMode/CanRead")]
        public ActionResult LoadVarient(long? productID, long? ShopID)
        {
            try
            {
                ViewBag.ShopID = ShopID;
                ShopID1 = Convert.ToInt64(ShopID);
                var VarientList = from TPV in db.TempProductVarients
                                  join C in db.Colors on new { ColorID = TPV.ColorID } equals new { ColorID = C.ID }
                                  join S in db.Sizes on new { SizeID = TPV.SizeID } equals new { SizeID = S.ID }
                                  join D in db.Dimensions on new { DimensionID = TPV.DimensionID } equals new { DimensionID = D.ID }
                                  join M in db.Materials on new { MaterialID = TPV.MaterialID } equals new { MaterialID = M.ID }
                                  join TSS in db.TempShopStocks on new { ProductVarientID = TPV.ID } equals new { ProductVarientID = TSS.ProductVarientID }
                                  join TSP in db.TempShopProducts on new { productID = TPV.ProductID } equals new { productID = TSP.ProductID }
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
        //[CustomAuthorize(Roles = "PaymentMode/CanRead")]
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
        //[CustomAuthorize(Roles = "PaymentMode/CanRead")]
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
        //[CustomAuthorize(Roles = "PaymentMode/CanRead")]
        //[CustomAuthorize(Roles = "PaymentMode/CanWrite")]
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
        //[CustomAuthorize(Roles = "PaymentMode/CanRead")]
        public JsonResult BindSavedComponent(long? ShopStockID, long? ProductVarientID)
        {
            try
            {
                //long ShopID = GetShopID();
                var IsIdPresent = db.TempStockComponents.Where(x => x.ShopStockID == ShopStockID).FirstOrDefault();
                if (IsIdPresent != null)
                {
                    var productComp = (from tsc in db.TempStockComponents

                                       join c in db.Components on tsc.ComponentID equals c.ID into c_join
                                       from c in c_join.DefaultIfEmpty()

                                       join tss in db.TempShopStocks on tsc.ShopStockID equals tss.ID into tss_join
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
        //[CustomAuthorize(Roles = "PaymentMode/CanRead")]
        //[CustomAuthorize(Roles = "PaymentMode/CanWrite")]
        public JsonResult SaveComponent(ProductComponentViewModel myData)
        {
            string Message = "";
            try
            {
                if (myData != null)
                {
                    TempStockComponent stockComponent = new TempStockComponent();
                    TempShopStock shopStock = new TempShopStock();
                    long stockCompnentID = (from tsp in db.TempStockComponents
                                            where tsp.ShopStockID == myData.ShopStockID && tsp.ComponentID == myData.ComponentID
                                            select tsp.ID).FirstOrDefault();

                    if (stockCompnentID > 0)
                    {
                        stockComponent = db.TempStockComponents.Find(stockCompnentID);
                        stockComponent.ShopStockID = stockComponent.ShopStockID;
                        stockComponent.ComponentID = stockComponent.ComponentID;
                        stockComponent.CreateDate = stockComponent.CreateDate;
                        stockComponent.CreateBy = stockComponent.CreateBy;
                        stockComponent.IsActive = stockComponent.IsActive;
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
                        stockComponent.IsActive = true;
                        stockComponent.CreateBy = 1;
                        stockComponent.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        stockComponent.DeviceID = "x";
                        stockComponent.DeviceType = "x";
                        db.TempStockComponents.Add(stockComponent);
                        db.SaveChanges();
                        Message = "Saved";

                    }
                    if (myData.ShopStockID > 0)
                    {
                        shopStock = db.TempShopStocks.Find(myData.ShopStockID);
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
                    + "[ProductUploadTemp][POST:SaveComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:SaveComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }

            return Json(Message, JsonRequestBehavior.AllowGet);

        }


        [SessionExpire]
        public JsonResult DeleteComponent(long? ComponentID, long? ShopStockID, decimal? Total)
        {
            string Message = "";
            try
            {
                TempShopStock shopStock = new TempShopStock();
                TempStockComponent tempComponent = db.TempStockComponents.Where(x => x.ComponentID == ComponentID && x.ShopStockID == ShopStockID).FirstOrDefault();
                db.TempStockComponents.Remove(tempComponent);
                Message = "Deleted Succesfully";
                if (ShopStockID > 0)
                {
                    shopStock = db.TempShopStocks.Find(ShopStockID);
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
                    + "[ProductUploadTemp][POST:DeleteComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Deleting Component!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:DeleteComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        //=========================End Of Component===========================================================================================================
        #endregion

        #region Code for DETAIL
        //=========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanRead")]
        public ActionResult Details1(long id, long shopId)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            long PersonalDetailID = GetPersonalDetailID();


            try
            {

                string[] src = ImageDisplay.DisplayProductImages(id, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                ViewBag.ImageURL = src;
                productuploadtempviewmodel.Path = src;

                ViewBag.textarea = CommonFunctions.LoadDescFile(id, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.NonApproved);

                TempProduct TP = db.TempProducts.Find(id);
                productuploadtempviewmodel.Name = TP.Name;
                //productuploadtempviewmodel.CategoryID = TP.CategoryID;
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

                TempShopProduct TSP = db.TempShopProducts.Where(x => x.ProductID == id).First();
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



                var Category = (from c in db.Categories
                                join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                                join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                join P in db.Plans on op.PlanID equals P.ID
                                where P.PlanCode.Contains("GBMR") && op.OwnerID == shopId
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                // ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                ViewBag.CategoryL_0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.CategoryL_0);
                ViewBag.CategoryL_1 = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.CategoryL_1);
                ViewBag.CategoryL_2 = new SelectList(db.Categories.Where(c => c.Level == 3), "ID", "Name", productuploadtempviewmodel.CategoryL_2);

                var query = (from TPV in db.TempProductVarients
                             join TSS in db.TempShopStocks on TPV.ID equals (TSS.ProductVarientID)
                             join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                             join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                             join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                             join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                             join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                             where TPV.ProductID == id
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
                    ColorID.Add(new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.ColorID));
                    SizeID.Add(new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.SizeID));
                    DimensionID.Add(new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.DimensionID));
                    MaterialID.Add(new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.MaterialID));
                    PackUnitID.Add(new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.PackUnitID));

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
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }
        //=========================================
        #endregion

        #region Code for CREATE
        //=========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanRead")]
        public ActionResult Create(long shopId)
        {
            try
            {
                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopId
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name");
                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name");
                ViewBag.DisplayProductFromDate1 = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == shopId).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                var FranchiseID = db.Shops.Where(x => x.ID == shopId).Select(x => x.FranchiseID).FirstOrDefault();
                var TaxIDD = (from TM in db.TaxationMasters
                              join FTD in db.FranchiseTaxDetails on TM.ID equals FTD.TaxationID
                              where FTD.IsActive == true && TM.IsActive == true && FTD.FranchiseID == FranchiseID
                              select new ForLoopClass { Name = TM.Name, ID = TM.ID }).OrderBy(x => x.Name).ToList();
                ViewBag.TaxationID = new SelectList(TaxIDD.OrderBy(x => x.Name).ToList(), "ID", "Name");

                List<NewProductVarient> ci = new List<NewProductVarient> { new NewProductVarient { ID = 0, ColorID = 0, SizeID = 0, DimensionID = 0, MaterialID = 0 } };
                ProductUploadTempViewModel productUploadTempViewModel = new ProductUploadTempViewModel();
                productUploadTempViewModel.DisplayProductFromDate = DateTime.Now;
                productUploadTempViewModel.NewProductVarientS = ci;
                productUploadTempViewModel.ShopID = shopId;
                return View(productUploadTempViewModel);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View();

        }

        [HttpPost]
        [ValidateInput(false)]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanWrite")]
        public ActionResult Create([Bind(Include = "Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate,CreateBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,DeliveryTime,DeliveryRate,TaxRate,TaxRatePer,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,ComponentID,ComponentWeight,ComponentUnitID,NewProductVarientS")] ProductUploadTempViewModel productuploadtempviewmodel, FormCollection collection,
       List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1, List<HttpPostedFileBase> files_2, List<HttpPostedFileBase> files_3, List<HttpPostedFileBase> files_4, List<HttpPostedFileBase> files_5, List<HttpPostedFileBase> files_6, List<HttpPostedFileBase> files_7, List<HttpPostedFileBase> files_8, List<HttpPostedFileBase> files_9, List<HttpPostedFileBase> files_10, List<HttpPostedFileBase> files_11, List<HttpPostedFileBase> files_12, List<HttpPostedFileBase> files_13, List<HttpPostedFileBase> files_14, List<HttpPostedFileBase> files_15, string submit, string DisplayProductFromDate1, string textarea, List<CategorySpecificationList> CategorySpecificationList)
        {
            string thumbnail = collection["ThumbIndex"].ToString();

            string StrInclusiveOfTax = collection["hdnIsInclusiveOfTax"].ToString();
            string[] NewinclusiveOfTax = StrInclusiveOfTax.Split('&');

            List<List<HttpPostedFileBase>> files = new List<List<HttpPostedFileBase>>();
            files.Add(files_0); files.Add(files_1); files.Add(files_2); files.Add(files_3); files.Add(files_4);
            files.Add(files_5); files.Add(files_6); files.Add(files_7); files.Add(files_8); files.Add(files_9);
            files.Add(files_10); files.Add(files_11); files.Add(files_12); files.Add(files_13); files.Add(files_14); files.Add(files_15);

            DateTime lDisplayProductFromDate = CommonFunctions.GetProperDateTime(DisplayProductFromDate1);
            productuploadtempviewmodel.DisplayProductFromDate = lDisplayProductFromDate;

            try
            {
                long ShopID = productuploadtempviewmodel.ShopID;
                long PersonalDetailID = GetPersonalDetailID();
                //--------------------------------------------
                int Proresult = (from f in db.TempProducts
                                 join sp in db.TempShopProducts on f.ID equals sp.ProductID
                                 where (f.Name == productuploadtempviewmodel.Name && sp.ShopID == ShopID)
                                 select new { f }).Count();
                //--------------------------------------------
                switch (submit)
                {
                    case "Save":
                        if (Proresult == 0)
                        {
                            using (TransactionScope ts = new TransactionScope())
                            {
                                try
                                {
                                    var Category = (from op in db.OwnerPlans
                                                    join p in db.Plans on op.PlanID equals p.ID
                                                    join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                                    join c in db.Categories on pcc.CategoryID equals c.ID
                                                    where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                                    && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                                    select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                                    ViewBag.CategoryID = new SelectList(Category, "ID", "Name");
                                    ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                                    ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name");

                                    ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                    ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == ShopID).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                                    TempProduct TP = new TempProduct();
                                    TP.Name = productuploadtempviewmodel.Name;
                                    TP.CategoryID = productuploadtempviewmodel.CategoryID;
                                    TP.WeightInGram = productuploadtempviewmodel.WeightInGram;
                                    TP.LengthInCm = productuploadtempviewmodel.LengthInCm;
                                    TP.BreadthInCm = productuploadtempviewmodel.BreadthInCm;
                                    TP.HeightInCm = productuploadtempviewmodel.HeightInCm;
                                    TP.Description = productuploadtempviewmodel.Description;
                                    TP.BrandID = productuploadtempviewmodel.BrandID;
                                    TP.SearchKeyword = productuploadtempviewmodel.SearchKeyword;
                                    TP.IsActive = true;
                                    TP.ApprovalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPLY_FOR_APPROVAL);
                                    TP.ApprovalRemark = "Apply for Approval";
                                    TP.CreateDate = DateTime.UtcNow;
                                    TP.CreateBy = PersonalDetailID;
                                    TP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    TP.DeviceID = "x";
                                    TP.DeviceType = "x";
                                    db.TempProducts.Add(TP);
                                    db.SaveChanges();


                                    // this code added by prashant for managing product specification
                                    ///////////////////////////
                                    if (CategorySpecificationList != null)
                                    {
                                        foreach (CategorySpecificationList clst in CategorySpecificationList)
                                        {
                                            TempProductSpecification TPS = new TempProductSpecification();

                                            TPS.ProductID = TP.ID;
                                            TPS.SpecificationID = clst.SpecificationID;
                                            TPS.Value = clst.SpecificationValue != null ? clst.SpecificationValue : "N/A";
                                            TPS.IsActive = true;
                                            TPS.CreateDate = DateTime.UtcNow;
                                            TPS.CreateBy = PersonalDetailID;
                                            TPS.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            TPS.DeviceType = "x";
                                            TPS.DeviceID = "x";

                                            db.TempProductSpecifications.Add(TPS);
                                            db.SaveChanges();
                                        }
                                    }

                                    ///////////////////////////


                                    TempShopProduct TSP = new TempShopProduct();
                                    TSP.ShopID = ShopID;
                                    TSP.ProductID = TP.ID;
                                    TSP.IsActive = true;
                                    TSP.DisplayProductFromDate = productuploadtempviewmodel.DisplayProductFromDate;
                                    TSP.DeliveryTime = productuploadtempviewmodel.DeliveryTime;
                                    TSP.DeliveryRate = productuploadtempviewmodel.DeliveryRate;
                                    TSP.TaxRate = productuploadtempviewmodel.TaxRate;
                                    TSP.TaxRatePer = productuploadtempviewmodel.TaxRatePer;
                                    TSP.CreateDate = DateTime.Now;
                                    TSP.CreateBy = PersonalDetailID;
                                    TSP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    TSP.DeviceID = "x";
                                    TSP.DeviceType = "x";
                                    db.TempShopProducts.Add(TSP);
                                    db.SaveChanges();

                                    int count = 0;
                                    using (EzeeloDBContext dc = new EzeeloDBContext())
                                    {
                                        foreach (var i in productuploadtempviewmodel.NewProductVarientS)
                                        {
                                            List<HttpPostedFileBase> MyFiles = new List<HttpPostedFileBase>();
                                            MyFiles = files[count]; count++;

                                            TempProductVarient tempProductVarient = new TempProductVarient();
                                            tempProductVarient.ProductID = TP.ID;
                                            tempProductVarient.ColorID = i.ColorID;
                                            tempProductVarient.DimensionID = i.DimensionID;
                                            tempProductVarient.SizeID = i.SizeID;
                                            tempProductVarient.MaterialID = i.MaterialID;
                                            tempProductVarient.IsActive = true;
                                            tempProductVarient.CreateDate = DateTime.Now;
                                            tempProductVarient.CreateBy = PersonalDetailID;
                                            tempProductVarient.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempProductVarient.DeviceID = "x";
                                            tempProductVarient.DeviceType = "x";
                                            dc.TempProductVarients.Add(tempProductVarient);
                                            dc.SaveChanges();


                                            TempShopStock tempShopStock = new TempShopStock();
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
                                            tempShopStock.IsActive = true;
                                            tempShopStock.CreateDate = DateTime.Now;
                                            tempShopStock.CreateBy = PersonalDetailID;
                                            tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempShopStock.DeviceID = "x";
                                            tempShopStock.DeviceType = "x";
                                            dc.TempShopStocks.Add(tempShopStock);
                                            dc.SaveChanges();


                                            try
                                            {

                                                int thumbIndex = IsThumbnail(thumbnail, count - 1);
                                                if (i.ColorID == 1)
                                                {
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, thumbIndex);
                                                }
                                                else
                                                {

                                                    var colorName = db.Colors.Where(x => x.ID == i.ColorID).FirstOrDefault();
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, thumbIndex);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            if (i.TaxationID != null)
                                            {
                                                foreach (var taxID in i.TaxationID)
                                                {
                                                    TempProductTax tempProductTax = new TempProductTax();
                                                    tempProductTax.ShopStockID = tempShopStock.ID;
                                                    tempProductTax.TaxID = Convert.ToInt32(taxID);
                                                    tempProductTax.IsActive = true;
                                                    tempProductTax.CreateDate = DateTime.Now;
                                                    tempProductTax.CreateBy = GetPersonalDetailID();
                                                    tempProductTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                    tempProductTax.DeviceID = "x";
                                                    tempProductTax.DeviceType = "x";
                                                    db.TempProductTaxes.Add(tempProductTax);
                                                    db.SaveChanges();

                                                }
                                            }
                                        }
                                    }
                                    try
                                    {
                                        CommonFunctions.UploadDescFile(TP.ID, textarea, ProductUpload.IMAGE_TYPE.NonApproved);
                                    }
                                    catch (BusinessLogicLayer.MyException myEx)
                                    {
                                        ModelState.AddModelError("Error", "There's Something wrong with the Description file!!");

                                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                            + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                            + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                    }
                                    catch (Exception ex)
                                    {

                                        ModelState.AddModelError("Error", "There's Something wrong with the Description file!!");

                                        BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                            + Environment.NewLine + ex.Message + Environment.NewLine
                                            + "[ProductUploadTemp][POST:Create]",
                                            BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                    }

                                    ts.Complete();
                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    ModelState.AddModelError("Error", "There's Something wrong with the Product!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                    ts.Dispose();

                                    ViewBag.message = "Some Error occured! Product is not Save! ";
                                    var Category = (from c in db.Categories
                                                    join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                                                    join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                                    join P in db.Plans on op.PlanID equals P.ID
                                                    where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                                                    select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                                    ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                                    ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                                    ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);

                                    ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");

                                    ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                                    ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                                    ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                                    ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                                    ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");
                                    ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name");
                                    ViewBag.ComponentUnitID = new SelectList(db.Units, "ID", "Name");

                                    return View(productuploadtempviewmodel);
                                }
                                catch (Exception ex)
                                {

                                    ModelState.AddModelError("Error", "There's Something wrong with the product!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[ProductUploadTemp][POST:Create]",
                                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                    ts.Dispose();

                                    ViewBag.message = "Some Error occured! Product is not Save! ";
                                    var Category = (from c in db.Categories
                                                    join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                                                    join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                                    join P in db.Plans on op.PlanID equals P.ID
                                                    where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                                                    select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                                    ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                                    ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                                    ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                                    ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ColorID);
                                    ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.SizeID);
                                    ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.DimensionID);
                                    ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.MaterialID);
                                    ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.PackUnitID);
                                    ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentID);
                                    ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentUnitID);

                                    return View(productuploadtempviewmodel);
                                }
                            }

                        }
                        else
                        {
                            ViewBag.message = "This product is already available in your shop(Non Approved Product List)";
                            var Category = (from c in db.Categories
                                            join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                                            join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                            join P in db.Plans on op.PlanID equals P.ID
                                            where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                                            select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                            ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                            ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                            ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ColorID);
                            ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.SizeID);
                            ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.DimensionID);
                            ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.MaterialID);
                            ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.PackUnitID);
                            ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentID);
                            ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentUnitID);

                            return View(productuploadtempviewmodel);
                        }
                        return RedirectToAction("Index1", new { shopId = productuploadtempviewmodel.ShopID });

                    case "Upload":
                        if (Request.Files.Count > 0)
                        {
                            var file = Request.Files[1];

                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                //var path = Path.Combine(Server.MapPath("~/FileUpload/"), fileName);
                                var path = Path.Combine(Server.MapPath("ftp://192.168.1.106/Content/FileUpload/"), fileName);
                                file.SaveAs(path);
                            }
                        }
                        ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                        ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ColorID);
                        ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.SizeID);
                        ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.DimensionID);
                        ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.MaterialID);
                        ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.PackUnitID);
                        ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentID);
                        ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentUnitID);

                        return View(productuploadtempviewmodel);


                    default:
                        {
                            ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                            ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ColorID);
                            ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.SizeID);
                            ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.DimensionID);
                            ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.MaterialID);
                            ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.PackUnitID);
                            ViewBag.ComponentID = new SelectList(db.Components.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentID);
                            ViewBag.ComponentUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.ComponentUnitID);

                            return RedirectToAction("Index1", new { shopId = productuploadtempviewmodel.ShopID });
                        }
                }

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return RedirectToAction("Index1", new { shopId = productuploadtempviewmodel.ShopID });
        }

        //========================================

        #endregion

        #region Code for EDIT
        //========================================
        [SessionExpire]
        [ValidateInput(false)]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanRead")]
        public ActionResult Edit(long id, long shopId)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            long PersonalDetailID = GetPersonalDetailID();

            try
            {
                // added by prashant for product ID
                ViewBag.ProductID = id;

                ViewBag.textarea = CommonFunctions.LoadDescFile(id, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.NonApproved);
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == shopId).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                TempProduct TP = db.TempProducts.Find(id);
                productuploadtempviewmodel.Name = TP.Name;
                productuploadtempviewmodel.CategoryID = TP.CategoryID;

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
                productuploadtempviewmodel.IsActive = TP.IsActive;
                productuploadtempviewmodel.CreateDate = DateTime.Now;
                productuploadtempviewmodel.CreateBy = PersonalDetailID;
                productuploadtempviewmodel.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                productuploadtempviewmodel.DeviceID = "x";
                productuploadtempviewmodel.DeviceType = "x";

                TempShopProduct TSP = db.TempShopProducts.Where(x => x.ProductID == id && x.ShopID == shopId).FirstOrDefault();
                productuploadtempviewmodel.ShopID = TSP.ShopID;
                productuploadtempviewmodel.ID = TSP.ID;
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
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopId
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();


                ViewBag.CategoryID = new SelectList(Category, "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", productuploadtempviewmodel.BrandID);
                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);

                var FranchiseID = db.Shops.Where(x => x.ID == shopId).Select(x => x.FranchiseID).FirstOrDefault();
                var TaxIDD = (from TM in db.TaxationMasters
                              join FTD in db.FranchiseTaxDetails on TM.ID equals FTD.TaxationID
                              where FTD.IsActive == true && TM.IsActive == true && FTD.FranchiseID == FranchiseID
                              select new ForLoopClass { Name = TM.Name, ID = TM.ID }).OrderBy(x => x.Name).ToList();

                var query = (from TPV in db.TempProductVarients
                             join TSS in db.TempShopStocks on TPV.ID equals (TSS.ProductVarientID)
                             join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                             join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                             join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                             join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                             join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                             where TPV.ProductID == id && TSS.TempShopProduct.ShopID == shopId
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
                    newProductVarient.PackSize = i.PackSize;
                    newProductVarient.PackUnitID = i.PackUnitID;
                    newProductVarient.MRP = i.MRP;
                    newProductVarient.RetailerRate = i.RetailerRate;
                    newProductVarient.WholeSaleRate = i.WholeSaleRate; 
                    newProductVarient.ShopStockID = i.ShopStockID;
                    newProductVarient.ColorName = i.ColorName;
                    newProductVarient.IsInclusiveOfTax = i.IsInclusiveOfTax;
                    newProductVarient.TaxationID = db.TempProductTaxes.Where(x => x.ShopStockID == i.ShopStockID && x.IsActive == true).Select(x => x.TaxID).ToList();
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
                        ii.ThumbPath = ImageDisplay.SetProductThumbPath(id, "default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        ii.Path = ImageDisplay.DisplayProductImages(id, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                    }
                    else
                    {
                        var colorName = db.Colors.Where(x => x.ID == ii.ColorID).FirstOrDefault();
                        ii.ThumbPath = ImageDisplay.SetProductThumbPath(id, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        ii.Path = ImageDisplay.DisplayProductImages(id, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                    }
                }
                ViewBag.ColorID = ColorID;
                ViewBag.SizeID = SizeID;
                ViewBag.DimensionID = DimensionID;
                ViewBag.MaterialID = MaterialID;
                ViewBag.PackUnitID = PackUnitID;
                ViewBag.TaxationID = TaxationID;
                return View(productuploadtempviewmodel);

            }


            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanWrite")]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate, CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,DeliveryTime,DeliveryRate,TaxRate,TaxRatePer,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,NewProductVarientS,CategoryL_2,Path,pathValue")] ProductUploadTempViewModel productuploadtempviewmodel,
        List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1, List<HttpPostedFileBase> files_2, List<HttpPostedFileBase> files_3, List<HttpPostedFileBase> files_4, List<HttpPostedFileBase> files_5, List<HttpPostedFileBase> files_6, List<HttpPostedFileBase> files_7, List<HttpPostedFileBase> files_8, List<HttpPostedFileBase> files_9, List<HttpPostedFileBase> files_10, List<HttpPostedFileBase> files_11, List<HttpPostedFileBase> files_12, List<HttpPostedFileBase> files_13, List<HttpPostedFileBase> files_14, List<HttpPostedFileBase> files_15,
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
                TempData.Remove("Description");
                TempData.Remove("Message");

                long PersonalDetailID = GetPersonalDetailID();

                DateTime lDisplayProductFromDate = CommonFunctions.GetProperDateTime(DisplayProductFromDate1);
                productuploadtempviewmodel.DisplayProductFromDate = lDisplayProductFromDate;

                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == productuploadtempviewmodel.ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);

                switch (submit)
                {
                    case "Save":
                        using (TransactionScope ts = new TransactionScope())
                        {
                            try
                            {
                                ViewBag.CategoryID = new SelectList(Category, "ID", "Name");

                                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");

                                EzeeloDBContext db1 = new EzeeloDBContext();

                                TempProduct TP = db1.TempProducts.Find(productuploadtempviewmodel.ID);
                                TP.Name = productuploadtempviewmodel.Name;
                                TP.CategoryID = productuploadtempviewmodel.CategoryID;
                                TP.WeightInGram = productuploadtempviewmodel.WeightInGram;
                                TP.LengthInCm = productuploadtempviewmodel.LengthInCm;
                                TP.BreadthInCm = productuploadtempviewmodel.BreadthInCm;
                                TP.HeightInCm = productuploadtempviewmodel.HeightInCm;
                                TP.Description = productuploadtempviewmodel.Description;
                                TP.BrandID = productuploadtempviewmodel.BrandID;
                                TP.SearchKeyword = ExtraSpaceRemoveFromString(productuploadtempviewmodel.SearchKeyword);
                                TP.IsActive = true;
                                TP.ApprovalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPLY_FOR_APPROVAL);
                                TP.ApprovalRemark = "Apply for Approval";
                                TP.CreateDate = TP.CreateDate;
                                TP.CreateBy = TP.CreateBy;
                                TP.ModifyDate = DateTime.UtcNow;
                                TP.ModifyBy = PersonalDetailID;
                                TP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                TP.DeviceID = "x";
                                TP.DeviceType = "x";
                                db1.SaveChanges();

                                // update product specifcation
                                // this code added by prashant for managing product specification
                                ///////////////////////////

                                List<TempProductSpecification> TPS = new List<TempProductSpecification>();

                                TPS = db1.TempProductSpecifications.Where(x => x.ProductID == TP.ID).ToList();
                                db1.TempProductSpecifications.RemoveRange(TPS);
                                db1.SaveChanges();


                                if (categorySpecificationList != null)
                                {

                                    foreach (CategorySpecificationList clst in categorySpecificationList)
                                    {
                                        TempProductSpecification TPS1 = new TempProductSpecification();

                                        TPS1.ProductID = TP.ID;
                                        TPS1.SpecificationID = clst.SpecificationID;
                                        TPS1.Value = clst.SpecificationValue != null ? clst.SpecificationValue : "N/A";
                                        TPS1.IsActive = true;
                                        TPS1.CreateDate = DateTime.UtcNow;
                                        TPS1.CreateBy = PersonalDetailID;
                                        TPS1.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                        TPS1.DeviceType = "x";
                                        TPS1.DeviceID = "x";

                                        db.TempProductSpecifications.Add(TPS1);
                                        db.SaveChanges();
                                    }
                                }

                                ///////////////////////////



                                //////////////


                                TempShopProduct TSP = db1.TempShopProducts.Where(x => x.ProductID == productuploadtempviewmodel.ID).First();
                                TSP.ShopID = productuploadtempviewmodel.ShopID;
                                TSP.ProductID = TP.ID;
                                TSP.IsActive = productuploadtempviewmodel.IsActive;
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
                                            TempProductVarient tempProductVarient = new TempProductVarient();
                                            tempProductVarient.ProductID = TP.ID;
                                            tempProductVarient.ColorID = i.ColorID;
                                            tempProductVarient.DimensionID = i.DimensionID;
                                            tempProductVarient.SizeID = i.SizeID;
                                            tempProductVarient.MaterialID = i.MaterialID;
                                            tempProductVarient.IsActive = true;
                                            tempProductVarient.CreateDate = DateTime.Now;
                                            tempProductVarient.CreateBy = PersonalDetailID;
                                            tempProductVarient.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempProductVarient.DeviceID = "x";
                                            tempProductVarient.DeviceType = "x";
                                            dc.TempProductVarients.Add(tempProductVarient);
                                            dc.SaveChanges();

                                            TempShopStock tempShopStock = new TempShopStock();
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
                                            tempShopStock.IsActive = true;
                                            tempShopStock.CreateDate = DateTime.Now;
                                            tempShopStock.CreateBy = PersonalDetailID;
                                            tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempShopStock.DeviceID = "x";
                                            tempShopStock.DeviceType = "x";
                                            dc.TempShopStocks.Add(tempShopStock);
                                            dc.SaveChanges();

                                            try
                                            {
                                                if (i.ColorID == 1)
                                                {
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, -1);
                                                }
                                                else
                                                {
                                                    var colorName = db.Colors.Where(x => x.ID == i.ColorID).FirstOrDefault();
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, -1);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]",
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
                                                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            if (i.TaxationID != null)
                                            {
                                                foreach (var taxID in i.TaxationID)
                                                {
                                                    TempProductTax tempProductTax = new TempProductTax();
                                                    tempProductTax.ShopStockID = tempShopStock.ID;
                                                    tempProductTax.TaxID = Convert.ToInt32(taxID);
                                                    tempProductTax.IsActive = true;
                                                    tempProductTax.CreateDate = DateTime.Now;
                                                    tempProductTax.CreateBy = GetPersonalDetailID();
                                                    tempProductTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                    tempProductTax.DeviceID = "x";
                                                    tempProductTax.DeviceType = "x";
                                                    db.TempProductTaxes.Add(tempProductTax);
                                                    db.SaveChanges();

                                                }
                                            }
                                        }
                                        else
                                        {

                                            TempProductVarient tempProductVarient = dc.TempProductVarients.Find(i.ProductVarientID);
                                            tempProductVarient.ProductID = TP.ID;
                                            tempProductVarient.ColorID = i.ColorID;
                                            tempProductVarient.DimensionID = i.DimensionID;
                                            tempProductVarient.SizeID = i.SizeID;
                                            tempProductVarient.MaterialID = i.MaterialID;
                                            tempProductVarient.IsActive = true;
                                            tempProductVarient.CreateDate = DateTime.Now;
                                            tempProductVarient.CreateBy = PersonalDetailID;
                                            tempProductVarient.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempProductVarient.DeviceID = "x";
                                            tempProductVarient.DeviceType = "x";
                                            //dc.TempProductVarients.Add(tempProductVarient);
                                            dc.SaveChanges();



                                            TempShopStock tempShopStock = dc.TempShopStocks.Find(i.ShopStockID);
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
                                            tempShopStock.IsActive = true;
                                            tempShopStock.ModifyDate = DateTime.Now;
                                            tempShopStock.ModifyBy = PersonalDetailID;
                                            tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                            tempShopStock.DeviceID = "x";
                                            tempShopStock.DeviceType = "x";
                                            // dc.TempShopStocks.Add(tempShopStock);
                                            dc.SaveChanges();

                                            try
                                            {
                                                if (i.ColorID == 1)
                                                {
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, -1);

                                                }
                                                else
                                                {
                                                    var colorName = db.Colors.Where(x => x.ID == i.ColorID).FirstOrDefault();
                                                    CommonFunctions.UploadProductImages(MyFiles, TP.Name, TP.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved, -1);
                                                }
                                            }
                                            catch (BusinessLogicLayer.MyException myEx)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]",
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
                                                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductUploadTemp][POST:Create]",
                                                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                            }

                                            if (i.TaxationID != null)
                                            {
                                                this.DeleteVariantTax(i.ShopStockID);
                                                foreach (var taxID in i.TaxationID)
                                                {
                                                    long PrevTaxID = db.TempProductTaxes.Where(x => x.ShopStockID == i.ShopStockID && x.TaxID == taxID).Select(x => x.TaxID).FirstOrDefault();
                                                    if (taxID == PrevTaxID)
                                                    {
                                                        TempProductTax tempProductTax = db.TempProductTaxes.Where(x => x.TaxID == taxID && x.ShopStockID == i.ShopStockID).FirstOrDefault();
                                                        tempProductTax.IsActive = true;
                                                        db.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        TempProductTax tempProductTax = new TempProductTax();
                                                        tempProductTax.ShopStockID = i.ShopStockID;
                                                        tempProductTax.TaxID = Convert.ToInt32(taxID);
                                                        tempProductTax.IsActive = true;
                                                        tempProductTax.CreateDate = DateTime.Now;
                                                        tempProductTax.CreateBy = GetPersonalDetailID();
                                                        tempProductTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                                        tempProductTax.DeviceID = "x";
                                                        tempProductTax.DeviceType = "x";
                                                        db.TempProductTaxes.Add(tempProductTax);
                                                        db.SaveChanges();
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                                try
                                {
                                    CommonFunctions.UploadDescFile(TP.ID, textarea, ProductUpload.IMAGE_TYPE.NonApproved);
                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    ModelState.AddModelError("Error", "There's Something wrong with the Description File!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {

                                    ModelState.AddModelError("Error", "There's Something wrong with the Description File!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[ProductUploadTemp][POST:Create]",
                                        BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);
                                }
                                TempData.Add("Message", "Changes Saved Successfully.");
                                ts.Complete();
                                return RedirectToAction("Index1", new { shopId = productuploadtempviewmodel.ShopID });
                            }
                            catch (BusinessLogicLayer.MyException myEx)
                            {
                                ModelState.AddModelError("Error", "There's Something wrong with the Product!!");

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                ts.Dispose();

                                TempData["MyErrorMsg"] = "Some Error occured! Product is not Updated! ";
                                TempData.Keep();
                                return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });

                            }
                            catch (Exception ex)
                            {

                                ModelState.AddModelError("Error", "There's Something wrong with the product!!");

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                    + "[ProductUploadTemp][POST:Edit]",
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                                ts.Dispose();

                                TempData["MyErrorMsg"] = "Some Error occured! Product is not Updated! ";
                                TempData.Keep();

                            }
                        }
                        return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });

                    case "Remove":
                        long Product = Convert.ToInt64(strTemp[1]);
                        long Color = Convert.ToInt64(strTemp[2]);
                        string Path = strTemp[3];
                        var ProductName = db.TempProducts.Where(x => x.ID == Product).FirstOrDefault();
                        if (Color == 1)
                        {
                            CommonFunctions.DeleteProductImages(Path, Product, ProductName.Name, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        }
                        else
                        {

                            var colorName = db.Colors.Where(x => x.ID == Color).FirstOrDefault();
                            CommonFunctions.DeleteProductImages(Path, Product, ProductName.Name, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        }
                        return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });

                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
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
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");

            }
            return RedirectToAction("Index1", productuploadtempviewmodel.ShopID);
        }
        //========================================
        #endregion

        #region Code for DELETE
        //-----------------------------------------
        public ActionResult Delete(long id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (TransactionScope ts = new TransactionScope())
            {

                TempProductVarient tempProductVarient = new TempProductVarient();
                TempShopStock tempShopStock = new TempShopStock();
                var listOfVariant = db.TempProductVarients.Where(x => x.ProductID == id).ToList();
                int count = listOfVariant.Count();
                if (count > 1)
                {
                    foreach (var pv in listOfVariant)
                    {
                        tempShopStock = db.TempShopStocks.Where(x => x.ProductVarientID == pv.ID).FirstOrDefault();
                        db.TempShopStocks.Remove(tempShopStock);
                        tempProductVarient = db.TempProductVarients.Where(x => x.ID == pv.ID).FirstOrDefault();
                        db.TempProductVarients.Remove(tempProductVarient);

                    }
                }

                else
                {
                    tempProductVarient = db.TempProductVarients.Where(x => x.ProductID == id).FirstOrDefault();
                    tempShopStock = db.TempShopStocks.Where(x => x.ProductVarientID == tempProductVarient.ID).FirstOrDefault();
                    db.TempShopStocks.Remove(tempShopStock);
                    // tempProductVarient = db.TempProductVarients.Where(x => x.ProductID == id).FirstOrDefault();
                    db.TempProductVarients.Remove(tempProductVarient);

                }
                TempShopProduct tempShopProduct = db.TempShopProducts.Where(x => x.ProductID == id).FirstOrDefault();
                db.TempShopProducts.Remove(tempShopProduct);

                //for delete product specification
                var Specification = db.TempProductSpecifications.Where(x => x.ProductID == id).ToList();
                if (Specification.Count() > 0)
                {
                    foreach (var spec in Specification)
                    {
                        TempProductSpecification tempProductSpecification = db.TempProductSpecifications.Where(x => x.ProductID == id).FirstOrDefault();
                        db.TempProductSpecifications.Remove(tempProductSpecification);
                    }
                }

                //Change by Harshada to Delete Bulk Products from ShopStockBulkLog and productBulkDetail (6/1/2016)
                var BulkStock = db.ShopStockBulkLogs.Where(x => x.TempProductID == id && x.TempShopStockID == tempShopStock.ID).ToList();
                if (BulkStock.Count() > 0)
                {
                    foreach (var bulk in BulkStock)
                    {
                        ShopStockBulkLog shopStockBulkLog = db.ShopStockBulkLogs.Where(x => x.BulkLogID == bulk.BulkLogID).FirstOrDefault();
                        db.ShopStockBulkLogs.Remove(shopStockBulkLog);
                    }
                }
                var bulkLogID = db.ProductBulkDetails.Where(x => x.TempProductID == id).ToList();
                if (bulkLogID.Count() > 0)
                {
                    foreach (var bulkId in bulkLogID)
                    {
                        ProductBulkDetail productBulkDetail = db.ProductBulkDetails.Where(x => x.TempProductID == bulkId.TempProductID).FirstOrDefault();
                        db.ProductBulkDetails.Remove(productBulkDetail);

                    }
                }
                //End of Delete Bulk Products from ShopStockBulkLog and productBulkDetail

                TempProduct tempProduct = db.TempProducts.Find(id);
                db.TempProducts.Remove(tempProduct);
                db.SaveChanges();

                CommonFunctions.DeleteAllProductImages(id, ProductUpload.IMAGE_TYPE.NonApproved);

                ts.Complete();

                return RedirectToAction("Index1", new { ShopId = tempShopProduct.ShopID });
            }

        }


        public ActionResult Remove(long? id)
        {
            TempProductVarient tempProductVarient = db.TempProductVarients.Find(id);
            TempShopStock tempShopStock = db.TempShopStocks.Where(x => x.ProductVarientID == id).FirstOrDefault();

            TempProduct tempProduct = db.TempProducts.Where(x => x.ID == tempProductVarient.ProductID).FirstOrDefault();

            //TempShopProduct tempShopProduct = db.TempShopProducts.Where(x => x.ProductID == tempProduct.ID).FirstOrDefault();
            TempShopProduct tempShopProduct = db.TempShopProducts.Where(x => x.ID == tempShopStock.ShopProductID).FirstOrDefault();

            var ColorID = db.TempProductVarients.Where(x => x.ID == id).Select(x => x.ColorID).FirstOrDefault();

            using (TransactionScope ts = new TransactionScope())
            {
                db.TempProductVarients.Remove(tempProductVarient);
                db.TempShopStocks.Remove(tempShopStock);
                db.SaveChanges();

                var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
                // var aaa = db.TempProductVarients.Where(x => x.ProductID == tempProductVarient.ProductID && x.ColorID == ColorID).Select(x => x.ColorID).Count();
                if (db.TempProductVarients.Where(x => x.ProductID == tempProductVarient.ProductID && x.ColorID == ColorID).Select(x => x.ColorID).Count() == 0)
                {
                    CommonFunctions.DeleteVariantImages(tempProductVarient.ProductID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                }
                ts.Complete();

            }

            return RedirectToAction("Edit", new { ID = tempProductVarient.ProductID, ShopID = tempShopProduct.ShopID });


        }
        //-------------------------------------------
        #endregion

        #region Code for Partial View Used in Edit Page
        //=========================================
        public ActionResult ProductVarientList(int id)
        {
            EzeeloDBContext db2 = new EzeeloDBContext();
            try
            {
                List<ProductUploadTempViewModel> PVML = new List<ProductUploadTempViewModel>();

                var query = (from TPV in db2.TempProductVarients
                             join TSS in db2.TempShopStocks on TPV.ID equals (TSS.ProductVarientID)
                             join CLR in db2.Colors on TPV.ColorID equals (CLR.ID)
                             join SIZ in db2.Sizes on TPV.SizeID equals (SIZ.ID)
                             join DMS in db2.Dimensions on TPV.DimensionID equals (DMS.ID)
                             join MTR in db2.Materials on TPV.MaterialID equals (MTR.ID)
                             join UNT in db2.Units on TSS.PackUnitID equals (UNT.ID)
                             where TPV.ProductID.Equals(id)
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
                                 ShopStockID = TSS.ID,
                                 ColorName = CLR.Name,
                                 SizeName = SIZ.Name,
                                 DiamentionName = DMS.Name,
                                 MaterialName = MTR.Name,
                                 UnitName = UNT.Name,
                                 IsInclusiveOfTax = TSS.IsInclusiveOfTax
                             }).ToList();


                foreach (var i in query)
                {
                    ProductUploadTempViewModel pUTVM = new ProductUploadTempViewModel();
                    pUTVM.ID = i.ID;
                    pUTVM.ProductVarientID = i.ProductVarientID;
                    pUTVM.ColorID = i.ColorID;
                    pUTVM.SizeID = i.SizeID;
                    pUTVM.DimensionID = i.DimensionID;
                    pUTVM.MaterialID = i.MaterialID;
                    pUTVM.Qty = i.Qty;
                    pUTVM.ReorderLevel = i.ReorderLevel;
                    pUTVM.StockStatus = i.StockStatus;
                    pUTVM.PackSize = i.PackSize;
                    pUTVM.PackUnitID = i.PackUnitID;
                    pUTVM.MRP = i.MRP;
                    pUTVM.ShopStockID = i.ShopStockID;
                    pUTVM.ColorName = i.ColorName;
                    pUTVM.SizeName = i.SizeName;
                    pUTVM.DiamentionName = i.DiamentionName;
                    pUTVM.MaterialName = i.MaterialName;
                    pUTVM.UnitName = i.UnitName;
                    pUTVM.IsInclusiveOfTax = i.IsInclusiveOfTax;
                    PVML.Add(pUTVM);
                }
                ProductUploadTempViewModelList productUploadTempViewModelList = new ProductUploadTempViewModelList();
                productUploadTempViewModelList.ProductUploadTempViewModelLIst = PVML;
                List<SelectListItem> IsInclusiveOfTaxList = new List<SelectListItem>(){
                                                                 new SelectListItem{ Value="true",Text="True"},
                                                                 new SelectListItem{ Value="false",Text="False"}
                                                             };

                // for (int ii = 0; ii < PVML.ProductUploadTempViewModelLIst.Count();ii++)
                foreach (var ii in productUploadTempViewModelList.ProductUploadTempViewModelLIst)
                {
                    ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name", ii.ColorID);
                    ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name", ii.SizeID);
                    ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name", ii.DimensionID);
                    ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name", ii.MaterialID);
                    ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name", ii.PackUnitID);
                    ViewBag.IsInclusiveOfTax = new SelectList(IsInclusiveOfTaxList, "Value", "Text", ii.IsInclusiveOfTax);


                }

                return PartialView(productUploadTempViewModelList);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);



            }
            return RedirectToAction("Index");

        }
        //===========================================
        #endregion

        #region Methods
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

        private int IsThumbnail(string val, int rowIndex)
        {
            try
            {

                string checkval = "MainTr_" + rowIndex;
                int IndexCount = 0;
                if (val.Contains(checkval))
                {
                    string[] a = val.Split('$');
                    foreach (string element in a)
                    {
                        if (element != string.Empty)
                        {
                            string[] b = element.Split('#');
                            if (b[0] == checkval)
                            {
                                IndexCount = Convert.ToInt32(b[1]);
                            }
                        }

                    }

                }
                return IndexCount;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult GetImagesWithThumb(long productID, long ColorID)
        {

            if (ColorID == 1)
            {
                string thumbPath = ImageDisplay.SetProductThumbPath(productID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                string[] Result = ImageDisplay.DisplayProductImages(productID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                Array.Resize(ref Result, Result.Length + 1);
                Result[Result.Length - 1] = thumbPath;
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
                string thumbPath = ImageDisplay.SetProductThumbPath(productID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                string[] Result = ImageDisplay.DisplayProductImages(productID, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                Array.Resize(ref Result, Result.Length + 1);
                Result[Result.Length - 1] = thumbPath;
                return Json(Result, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ImagesRemove(long ProductID, long ColorID, string ImagePath)
        {
            var ProductName = db.TempProducts.Where(x => x.ID == ProductID).FirstOrDefault();
            if (ColorID == 1)
            {
                CommonFunctions.DeleteProductImages(ImagePath, ProductID, ProductName.Name, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                var Result = ImageDisplay.DisplayProductImages(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
                CommonFunctions.DeleteProductImages(ImagePath, ProductID, ProductName.Name, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                var Result = ImageDisplay.DisplayProductImages(ProductID, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }

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
            dt = BusinessLogicLayer.CategorySpecificationListClass.Select_ProductSpecification(productID, categoryID, System.Web.HttpContext.Current.Server);

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

            Int64 categoryID = db.TempProducts.Where(x => x.ID == productID).FirstOrDefault().CategoryID;

            List<CategorySpecificationList> objActualspecification = new List<CategorySpecificationList>();

            DataTable dt = new DataTable();
            dt = BusinessLogicLayer.CategorySpecificationListClass.Select_ProductSpecification(productID, categoryID, System.Web.HttpContext.Current.Server);


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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult DownloadFile()
        {
            return Redirect("ftp://192.168.1.106/Content/FileDownload/Product.docx");
        }


        #endregion

        #region Web Methods

        public JsonResult GetCategoryLevel1ByParentCategory(int categoryID)
        {
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<Category> lCategory = new List<Category>();
            //List<City> lcity = new List<City>();
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
            //var district = (List<District>)db.Districts.Where(u => u.StateID == stateID).ToList();
            //var district = from cust in db.States 
            //                        select cust;
            List<Category> lCategory = new List<Category>();
            //List<City> lcity = new List<City>();
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
        public JsonResult AutoCompleteProName(string term, long ShopID)
        {
            //long ShopID = GetShopID();
            //var result = (from c in db.Categories
            //              join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
            //              join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
            //              join P in db.Plans on op.PlanID equals P.ID
            //              join r in db.Products on c.ID equals r.CategoryID
            //              //join pp in ProductId on r.ID equals pp.ProductID
            //              where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
            //              && r.IsActive == true
            //                  //&& r.ID != pp.ProductID
            //              && r.Name.ToLower().Contains(term.ToLower())
            //              && !
            //              (from pp in db.ProprietoryProducts
            //               select new
            //               {
            //                   pp.ProductID
            //               }).Contains(new { ProductID = r.ID })
            //              select new { Name = r.Name, r.ID, ShopID = ShopID }).Distinct().ToList();


            var result = (from op in db.OwnerPlans
                          join p in db.Plans on op.PlanID equals p.ID
                          join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                          join c in db.Categories on pcc.CategoryID equals c.ID
                          join r in db.Products on c.ID equals r.CategoryID
                          where op.Plan.PlanCode.StartsWith("GBMR") && op.IsActive == true && c.IsActive == true && op.OwnerID == ShopID
                                      && r.IsActive == true
                                      && r.Name.ToLower().Contains(term.ToLower())
                                      && !
                                      (from pp in db.ProprietoryProducts
                                       select new
                                       {
                                           pp.ProductID
                                       }).Contains(new { ProductID = r.ID })
                          select new { Name = r.Name, r.ID, ShopID = ShopID }).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AutoCompleteProName1(string term, long ShopID)
        {
            //var result = (from c in db.Categories
            //              join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
            //              join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
            //              join P in db.Plans on op.PlanID equals P.ID
            //              join r in db.Products on c.ID equals r.CategoryID
            //              //join pp in ProductId on r.ID equals pp.ProductID
            //              where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
            //              && r.IsActive == true
            //                  //&& r.ID != pp.ProductID
            //              && r.Name.ToLower() == term.ToLower()
            //              && !
            //              (from pp in db.ProprietoryProducts
            //               select new
            //               {
            //                   pp.ProductID
            //               }).Contains(new { ProductID = r.ID })
            //              select new { Name = r.Name, r.ID, ShopID = ShopID }).Distinct().ToList();

            var result = (from op in db.OwnerPlans
                          join p in db.Plans on op.PlanID equals p.ID
                          join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                          join c in db.Categories on pcc.CategoryID equals c.ID
                          join r in db.Products on c.ID equals r.CategoryID
                          where op.Plan.PlanCode.StartsWith("GBMR") && op.IsActive == true && c.IsActive == true
                                      && op.OwnerID == ShopID && r.IsActive == true
                                      && r.Name.ToLower() == term.ToLower()
                                      && !
                                      (from pp in db.ProprietoryProducts
                                       select new
                                       {
                                           pp.ProductID
                                       }).Contains(new { ProductID = r.ID })
                          select new { Name = r.Name, r.ID, ShopID = ShopID }).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        private void DeleteVariantTax(long ShopStockID)
        {
            var ID = db.TempProductTaxes.Where(x => x.ShopStockID == ShopStockID).Select(x => x.ID).ToList();
            foreach (var item in ID)
            {
                TempProductTax tempProductTax = db.TempProductTaxes.Find(item);
                tempProductTax.IsActive = false;
                db.SaveChanges();
            }

        }

        #endregion

        #region Code for GBCatalog
        //========================================
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanRead")]
        public ActionResult GBCatalog1(long id, long ShopID)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            long PersonalDetailID = GetPersonalDetailID();

            try
            {

                ViewBag.ProductID = id;
                ViewBag.textarea = CommonFunctions.LoadDescFile(id, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);

                int productLimit = (from s in db.Shops
                                    join o in db.OwnerPlans on s.ID equals o.OwnerID
                                    join p in db.Plans on o.PlanID equals p.ID
                                    where p.PlanCode.StartsWith("GBMR") && s.ID == ShopID
                                    select p.NoOfEntitiesAllowed).FirstOrDefault();
                int productUploadedinShop = db.ShopProducts.Where(x => x.ShopID == ShopID).Select(x => x.ID).Count();

                int ProductId = db.ShopProducts.Count(x => x.ShopID == ShopID && x.ProductID == id);
                if (ProductId == 0)//check Is available in your shop or not
                {
                    if (productLimit > productUploadedinShop)// check Product limit according to plan
                    {
                        Product TP = db.Products.Find(id);
                        productuploadtempviewmodel.Name = TP.Name;
                        productuploadtempviewmodel.CategoryID = TP.CategoryID;

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
                        productuploadtempviewmodel.IsActive = TP.IsActive;
                        productuploadtempviewmodel.CreateDate = DateTime.Now;
                        productuploadtempviewmodel.CreateBy = PersonalDetailID;
                        productuploadtempviewmodel.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        productuploadtempviewmodel.DeviceID = "x";
                        productuploadtempviewmodel.DeviceType = "x";


                        productuploadtempviewmodel.ShopID = ShopID;
                        ShopProduct TSP = db.ShopProducts.Where(x => x.ProductID == id).FirstOrDefault();

                        if (productuploadtempviewmodel == null)
                        {
                            return HttpNotFound();
                        }

                        var Category = (from op in db.OwnerPlans
                                        join p in db.Plans on op.PlanID equals p.ID
                                        join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                        join c in db.Categories on pcc.CategoryID equals c.ID
                                        where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                        && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                        select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                        ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                        ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                        ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);
                        ViewBag.DisplayProductFromDate1 = DateTime.Now.Date.ToString("dd/MM/yyyy");
                        ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);

                        var query = (from TPV in db.ProductVarients
                                     join TSS in db.ShopStocks on TPV.ID equals (TSS.ProductVarientID)
                                     join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                                     join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                                     join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                                     join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                                     join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                                     where TPV.ProductID == id
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
                                     }).ToList().GroupBy(x => x.ProductVarientID).Select(x => x.First());

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
                            newProductVarientList.Add(newProductVarient);
                        }

                        productuploadtempviewmodel.NewProductVarientS = newProductVarientList;
                        List<SelectList> ColorID = new List<SelectList>();
                        List<SelectList> SizeID = new List<SelectList>();
                        List<SelectList> DimensionID = new List<SelectList>();
                        List<SelectList> MaterialID = new List<SelectList>();
                        List<SelectList> PackUnitID = new List<SelectList>();
                        foreach (var ii in productuploadtempviewmodel.NewProductVarientS)
                        {
                            ColorID.Add(new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.ColorID));
                            SizeID.Add(new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.SizeID));
                            DimensionID.Add(new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.DimensionID));
                            MaterialID.Add(new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.MaterialID));
                            PackUnitID.Add(new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", ii.PackUnitID));
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

                        return View(productuploadtempviewmodel);
                    }
                    else
                    {
                        TempData["MyErrorMsg"] = "Your Product Limit is Exceed ! You can't Add more product ";
                        TempData.Keep();
                        return RedirectToAction("Create", "ProductUploadTemp", new { shopID = ShopID });
                    }
                }
                else
                {
                    TempData["MyErrorMsg"] = "This product is already available in your shop";
                    TempData.Keep();
                    return RedirectToAction("Create", "ProductUploadTemp", new { shopID = ShopID });
                }
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

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadTemp/CanWrite")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult GBCatalog1([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,NewProductVarientS,CategoryL_2,Path,pathValue,IsSelect")] ProductUploadTempViewModel productuploadtempviewmodel,
            List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1, List<HttpPostedFileBase> files_2, List<HttpPostedFileBase> files_3, List<HttpPostedFileBase> files_4, List<HttpPostedFileBase> files_5, List<HttpPostedFileBase> files_6, List<HttpPostedFileBase> files_7, List<HttpPostedFileBase> files_8, List<HttpPostedFileBase> files_9, List<HttpPostedFileBase> files_10, List<HttpPostedFileBase> files_11, List<HttpPostedFileBase> files_12, List<HttpPostedFileBase> files_13, List<HttpPostedFileBase> files_14, List<HttpPostedFileBase> files_15, FormCollection collection, string submit, string DisplayProductFromDate1, List<CategorySpecificationList> categorySpecificationList)
        {
            string thumbnail = collection["ThumbIndex"].ToString();

            List<List<HttpPostedFileBase>> files = new List<List<HttpPostedFileBase>>();
            files.Add(files_0); files.Add(files_1); files.Add(files_2); files.Add(files_3); files.Add(files_4);
            files.Add(files_5); files.Add(files_6); files.Add(files_7); files.Add(files_8); files.Add(files_9);
            files.Add(files_10); files.Add(files_11); files.Add(files_12); files.Add(files_13); files.Add(files_14); files.Add(files_15);

            try
            {
                long ShopID = productuploadtempviewmodel.ShopID;
                long PersonalDetailID = GetPersonalDetailID();

                DateTime lDisplayProductFromDate = CommonFunctions.GetProperDateTime(DisplayProductFromDate1);
                productuploadtempviewmodel.DisplayProductFromDate = lDisplayProductFromDate;

                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category, "ID", "Name");

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");

                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");

                EzeeloDBContext db1 = new EzeeloDBContext();

                List<ProductSpecification> TPS = new List<ProductSpecification>();

                TPS = db1.ProductSpecifications.Where(x => x.ProductID == productuploadtempviewmodel.ID).ToList();
                db1.ProductSpecifications.RemoveRange(TPS);
                db1.SaveChanges();


                if (categorySpecificationList != null)
                {

                    foreach (CategorySpecificationList clst in categorySpecificationList)
                    {
                        ProductSpecification TPS1 = new ProductSpecification();

                        TPS1.ProductID = productuploadtempviewmodel.ID;
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


                ShopProduct TSP = new ShopProduct();
                TSP.ShopID = ShopID;
                //TSP.ProductID = TP.ID;
                TSP.ProductID = productuploadtempviewmodel.ID;
                TSP.IsActive = true;
                TSP.DisplayProductFromDate = productuploadtempviewmodel.DisplayProductFromDate;
                TSP.CreateDate = DateTime.Now;
                TSP.CreateBy = PersonalDetailID;
                TSP.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                TSP.DeviceID = "x";
                TSP.DeviceType = "x";
                db.ShopProducts.Add(TSP);
                db.SaveChanges();

                int count = 0;
                using (EzeeloDBContext dc = new EzeeloDBContext())
                {
                    foreach (var i in productuploadtempviewmodel.NewProductVarientS)
                    {
                        List<HttpPostedFileBase> MyFiles = new List<HttpPostedFileBase>();
                        MyFiles = files[count]; count++;

                        if (i.ProductVarientID == 0)
                        {
                            ProductVarient tempProductVarient = new ProductVarient();
                            tempProductVarient.ProductID = productuploadtempviewmodel.ID;
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
                            tempShopStock.IsInclusiveOfTax = i.IsInclusiveOfTax;
                            tempShopStock.IsActive = true;
                            tempShopStock.CreateDate = DateTime.Now;
                            tempShopStock.CreateBy = PersonalDetailID;
                            tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                            tempShopStock.DeviceID = "x";
                            tempShopStock.DeviceType = "x";
                            dc.ShopStocks.Add(tempShopStock);
                            dc.SaveChanges();

                            try
                            {
                                int thumbIndex = IsThumbnail(thumbnail, count - 1);
                                if (i.ColorID == 1)
                                {
                                    CommonFunctions.UploadProductImages(MyFiles, productuploadtempviewmodel.Name, productuploadtempviewmodel.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved, thumbIndex);
                                }
                                else
                                {
                                    var colorName = db.Colors.Where(x => x.ID == i.ColorID).FirstOrDefault();
                                    CommonFunctions.UploadProductImages(MyFiles, productuploadtempviewmodel.Name, productuploadtempviewmodel.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved, thumbIndex);
                                }
                            }
                            catch (BusinessLogicLayer.MyException myEx)
                            {
                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                    + "[ProductUploadTemp][POST:Create]" + myEx.EXCEPTION_PATH,
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            }
                            catch (Exception ex)
                            {

                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                    + "[ProductUploadTemp][POST:Create]",
                                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
                            }
                        }
                        else if (i.IsSelect == true)
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
                            //tempShopStockSS.WholeSaleRate = i.WholeSaleRate;
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

                }

                return RedirectToAction("Index", "ProductUploadAproved", new { shopId = productuploadtempviewmodel.ShopID });

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadAproved][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Franchise, System.Web.HttpContext.Current.Server);

                ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");
                ViewBag.CategoryL_0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                ViewBag.CategoryL_1 = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name");
                ViewBag.CategoryL_2 = new SelectList(db.Categories.Where(c => c.Level == 3), "ID", "Name");
                // ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                //ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
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
                ViewBag.CategoryL_0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                ViewBag.CategoryL_1 = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name");
                ViewBag.CategoryL_2 = new SelectList(db.Categories.Where(c => c.Level == 3), "ID", "Name");
                //  ViewBag.CategoryLevel0 = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name");
                //ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");

            }
            return RedirectToAction("Index1");
        }
        //========================================
        #endregion


    }
}
