//-----------------------------------------------------------------------
// <copyright file=" ProductApprovalController.cs" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Snehal Shende</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
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
using System.Transactions;
using Administrator.Models;
using System.Data.Entity.Validation;

namespace Administrator.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ProductApprovalController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        private static long ShopID1 = 0;

        #region Code for Index
        //========================================

        // GET: /ProductApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult MerchantList()
        {
            try
            {



                ViewBag.FranchiseList = new SelectList((from f in db.Franchises
                                                        join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                                        join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                        join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                        where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
                                                        select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + (pd.LastName == null ? " " : pd.LastName) + " (" + bd.Name + ")", }).ToList(), "ID", "Name");         // Name = bd.Name 

                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                List<ShopViewModel> lShop = (from s in db.Shops
                                             where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR"
                                             select new ShopViewModel
                                             {
                                                 ID = s.ID,
                                                 Name = s.Name.Trim(),
                                                 FranchiseID = s.FranchiseID,
                                                 NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                                             }).OrderBy(x => x.Name).ToList();

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
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        [HttpPost]
        public ActionResult MerchantList(int? FranchiseList)
        {
            try
            {
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises
                                                        join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                                        join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                        join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                        where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
                                                        select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")", }).ToList(), "ID", "Name", FranchiseList);         // Name = bd.Nam

                int fID = 0;
                int.TryParse(Convert.ToString(FranchiseList), out fID);
                List<ShopViewModel> lst = new List<ShopViewModel>();
                lst = GetMerchantList(fID);

                return View(lst);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApprovalController][POST:MerchantList]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovalController][POST:MerchantList]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // GET: /ProductApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult Index(long shopId)
        {
            try
            {
                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                var QueryResult = (from TP in db.TempProducts
                                   join TPV in db.TempProductVarients on new { ID = TP.ID } equals new { ID = TPV.ProductID }
                                   join TSP in db.TempShopProducts on new { ID = TP.ID } equals new { ID = TSP.ProductID }
                                   join TSS in db.TempShopStocks on new { ID = TPV.ID } equals new { ID = TSS.ProductVarientID }
                                   join CTG in db.Categories on new { CategoryID = TP.CategoryID } equals new { CategoryID = CTG.ID }
                                   join BRD in db.Brands on new { BrandID = TP.BrandID } equals new { BrandID = BRD.ID }
                                   where TSP.ShopID == shopId && TP.IsActive == true && TP.ApprovalStatus != approvalStatus
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
                                       ProductVarientID = g.Count(p => p.TPV.ID > 0),
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
                    putvm.HSNCode = ReadRecord.HSNCode;      ///Added by Priti on 2/11/2018 
                    putvm.EANCode = ReadRecord.EANCode;      ///Added by Priti on 2/11/2018                    
                    putvm.Name = ReadRecord.ProductName;
                    putvm.Qty = ReadRecord.Qty;
                    putvm.PackSize = ReadRecord.PackSize;
                    putvm.ProductVarientID = ReadRecord.ProductVarientID;
                    putvm.ColorID = ReadRecord.ColorID;
                    putvm.ApprovalRemark = ReadRecord.ApprovalRemark;
                    putvm.ApprovalStatus = ReadRecord.ApprovalStatus;
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
                    + "[ProductApproval][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product Upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                ViewBag.PShopID = ShopID;
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while loading varients!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:LoadVarient]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while loading Components partial view!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:AddComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Binding Components!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Binding  Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindComponentDetails]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                    //foreach (var i in productComp)
                    //{
                    //    if (i.ComponentUnitID != null && i.ComponentUnitID > 0)
                    //        i.ComponentUnitName = db.Units.Where(x => x.ID == i.ComponentUnitID).FirstOrDefault().Name;
                    //    else i.ComponentUnitName = string.Empty;
                    //    //ViewBag.unitID = new SelectList(db.Units, "ID", "Name", i.ComponentUnitID);
                    //}
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Binding Saved Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:BindSavedComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                        else if (myData.ComponentUnitID == null)
                        {
                            stockComponent.ComponentUnitID = 1;
                        }

                        stockComponent.CreateDate = DateTime.UtcNow;
                        //stockComponent.IsActive = true;
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
                    + "[ ProductApproval][POST:SaveComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Component Details!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ ProductApproval][POST:SaveComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Deleting Component!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:DeleteComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        //=========================End Of Component===========================================================================================================
        #endregion

        #region Code for EDIT
        //========================================
        [SessionExpire]
        [ValidateInput(false)]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult Edit(long id, long shopId)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            long PersonalDetailID = GetPersonalDetailID();

            try
            {
                // added by prashant for product ID
                ViewBag.ProductID = id;

                ViewBag.textarea = CommonFunctions.LoadDescFile(id, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.NonApproved);

                TempProduct TP = db.TempProducts.Find(id);
                productuploadtempviewmodel.Name = TP.Name;
                productuploadtempviewmodel.ID = TP.ID;
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

                productuploadtempviewmodel.HSNCode = TP.HSNCode;  //// Added by Priti on 2/11/2018  
                productuploadtempviewmodel.EANCode = TP.EANCode;  //// Added by Priti on 2/11/2018



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
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopId
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == shopId).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

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
                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanWrite")]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate, CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,DeliveryTime,DeliveryRate,TaxRate,TaxRatePer,IsSelect,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,NewProductVarientS,NewProductVarientPOP,CategoryL_2,Path,pathValue,HSNCode,EANCode")] ProductUploadTempViewModel productuploadtempviewmodel,
        List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1, List<HttpPostedFileBase> files_2, List<HttpPostedFileBase> files_3, List<HttpPostedFileBase> files_4, List<HttpPostedFileBase> files_5, List<HttpPostedFileBase> files_6, List<HttpPostedFileBase> files_7, List<HttpPostedFileBase> files_8, List<HttpPostedFileBase> files_9, List<HttpPostedFileBase> files_10, List<HttpPostedFileBase> files_11, List<HttpPostedFileBase> files_12, List<HttpPostedFileBase> files_13, List<HttpPostedFileBase> files_14, List<HttpPostedFileBase> files_15,
        FormCollection collection, string submit, string DisplayProductFromDate1, string textarea, List<CategorySpecificationList> categorySpecificationList, bool IsProprietory)
        {

            if (productuploadtempviewmodel != null && productuploadtempviewmodel.NewProductVarientS.Count != 0)
            {
                foreach (var item in productuploadtempviewmodel.NewProductVarientS)
                {
                    int count = 0;
                    var tax = item.TaxationID;
                    decimal GSTPart1 = 0;
                    decimal GSTPart2 = 0;
                    foreach (var i in tax)
                    {
                        int? FranchiseId = db.Shops.FirstOrDefault(p => p.ID == productuploadtempviewmodel.ShopID).FranchiseID;
                        FranchiseTaxDetail obj = db.FranchiseTaxDetails.FirstOrDefault(p => p.TaxationID == i && p.FranchiseID == FranchiseId && p.IsActive == true);
                        if (obj != null)
                        {
                            if (count == 0)
                            {
                                GSTPart1 = obj.InPercentage;
                                count++;
                            }
                            else
                            {
                                GSTPart2 = obj.InPercentage;
                            }
                        }
                    }
                    if (GSTPart1 != GSTPart2)
                    {
                        TempData["GSTSelectionMsg"] = "Select same CGST and SGCT % for varient " + db.Sizes.FirstOrDefault(p => p.ID == item.SizeID).Name;
                        return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, ShopId = productuploadtempviewmodel.ShopID });
                    }
                }

            }
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
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == productuploadtempviewmodel.ShopID).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                switch (submit)
                {
                    case "Save":
                        using (TransactionScope ts = new TransactionScope())
                        {
                            try
                            {
                                ViewBag.CategoryID = new SelectList(Category, "ID", "Name");
                                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                                ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

                                EzeeloDBContext db1 = new EzeeloDBContext();

                                TempProduct TP = db1.TempProducts.Find(productuploadtempviewmodel.ID);
                                TP.Name = productuploadtempviewmodel.Name;
                                TP.CategoryID = productuploadtempviewmodel.CategoryID;
                                TP.WeightInGram = productuploadtempviewmodel.WeightInGram;
                                TP.HSNCode = productuploadtempviewmodel.HSNCode;    ///Added by Priti on 2/11/2018  
                                TP.EANCode = productuploadtempviewmodel.EANCode;    ///Added by Priti on 2/11/2018
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
                                                    + "[ProductApproval][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductApproval][POST:Create]",
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                                                    + "[ProductApproval][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {
                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductApproval][POST:Create]",
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                                                    + "[ProductApproval][POST:Create]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductApproval][POST:Create]",
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                                                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                            }
                                            catch (Exception ex)
                                            {

                                                ModelState.AddModelError("Error", "There's Something wrong with the thumbnail Image!!");

                                                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                                    + Environment.NewLine + ex.Message + Environment.NewLine
                                                    + "[ProductApproval][POST:Edit]",
                                                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
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
                                    CommonFunctions.UploadDescFile(TP.ID, textarea, ProductUpload.IMAGE_TYPE.NonApproved);
                                }
                                catch (BusinessLogicLayer.MyException myEx)
                                {
                                    ModelState.AddModelError("Error", "There's Something wrong with description file!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                                        + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                }
                                catch (Exception ex)
                                {

                                    ModelState.AddModelError("Error", "There's Something wrong with description file!!");

                                    BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                                        + Environment.NewLine + ex.Message + Environment.NewLine
                                        + "[ProductApproval][POST:Edit]",
                                        BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
                                }
                                TempData.Add("Message", "Changes Saved Successfully.");
                                ts.Complete();
                                return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });
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
                        long lColorID = Convert.ToInt64(strTemp[2]);
                        string Path = strTemp[3];
                        string ProductName = db.TempProducts.Where(x => x.ID == productuploadtempviewmodel.ID).FirstOrDefault().Name;
                        if (lColorID == 1)
                        {
                            CommonFunctions.DeleteProductImages(Path, productuploadtempviewmodel.ID, ProductName, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        }
                        else
                        {
                            var colorName = db.Colors.Where(x => x.ID == lColorID).FirstOrDefault();
                            CommonFunctions.DeleteProductImages(Path, productuploadtempviewmodel.ID, ProductName, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                        }
                        return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });

                    case "Approve":
                        string msg = string.Empty;
                        if (!string.IsNullOrEmpty(textarea))
                        {
                            TempData.Add("Description", true);
                        }
                        else
                        {
                            TempData.Add("Description", false);
                        }
                        IsProductApprovable(productuploadtempviewmodel.ID, productuploadtempviewmodel.ShopID, IsProprietory, ref msg);
                        string[] msgarray = msg.Split('$');
                        if (!string.IsNullOrEmpty(msgarray[0]))
                        {
                            if (msgarray[0] == "Component")
                            {
                                int Status = Convert.ToInt32(msgarray[1]);
                                return RedirectToAction("DisplayComponent", "ProductApproval", new { ProductID = productuploadtempviewmodel.ID, ShopID = productuploadtempviewmodel.ShopID, IsProp = IsProprietory, Status = Status });
                            }

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

                                if (ii.ColorID == 1)
                                {
                                    ii.ThumbPath = ImageDisplay.SetProductThumbPath(productuploadtempviewmodel.ID, "default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                                    ii.Path = ImageDisplay.DisplayProductImages(productuploadtempviewmodel.ID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                                }
                                else
                                {
                                    var colorName = db.Colors.Where(x => x.ID == ii.ColorID).FirstOrDefault();
                                    ii.ThumbPath = ImageDisplay.SetProductThumbPath(productuploadtempviewmodel.ID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                                    ii.Path = ImageDisplay.DisplayProductImages(productuploadtempviewmodel.ID, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                                }
                            }
                            ViewBag.ColorID = ColorID;
                            ViewBag.SizeID = SizeID;
                            ViewBag.DimensionID = DimensionID;
                            ViewBag.MaterialID = MaterialID;
                            ViewBag.PackUnitID = PackUnitID;
                            ViewBag.CategoryID = new SelectList(Category, "ID", "Name");
                            ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");

                            return View(productuploadtempviewmodel);
                        }
                        return RedirectToAction("Index", new { shopId = productuploadtempviewmodel.ShopID });
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.ColorID = new SelectList(db.Colors.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.SizeID = new SelectList(db.Sizes.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.DimensionID = new SelectList(db.Dimensions.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");
                ViewBag.PackUnitID = new SelectList(db.Units.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name");

            }
            return RedirectToAction("Edit", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });
            //return RedirectToAction("Index", new { shopId = productuploadtempviewmodel.ShopID });
        }



        #endregion

        #region Variant Aproval Process
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult MerchantList1()
        {
            try
            {
                ViewBag.FranchiseList = new SelectList((from f in db.Franchises
                                                        join bd in db.BusinessDetails on f.BusinessDetailID equals bd.ID
                                                        join bt in db.BusinessTypes on bd.BusinessTypeID equals bt.ID
                                                        join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                                        join pd in db.PersonalDetails on ul.ID equals pd.UserLoginID
                                                        where ul.IsLocked == false && f.IsActive == true && bt.Prefix == "GBFR" && f.ID != 1
                                                        select new { ID = f.ID, Name = pd.Salutation.Name + " " + pd.FirstName + " " + pd.LastName + " (" + bd.Name + ")", }).ToList(), "ID", "Name");         // Name = bd.Name 

                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                List<ShopViewModel> lShop = (from s in db.Shops
                                             where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR"
                                             select new ShopViewModel
                                             {
                                                 ID = s.ID,
                                                 Name = s.Name.Trim(),
                                                 FranchiseID = s.FranchiseID,
                                                 // NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                                                 // ApproveProductCount = db.ShopProducts.Where(x => x.ShopID == s.ID && x.Product.IsActive == true).Count()
                                                 ApproveProductCount = (from pv in db.ProductVarients
                                                                        join st in db.ShopStocks on pv.ID equals st.ProductVarientID
                                                                        join sp in db.ShopProducts on st.ShopProductID equals sp.ID
                                                                        where sp.ShopID == s.ID && pv.IsActive == false
                                                                        select new { pv.ProductID }).Distinct().Count()
                                             }).OrderBy(x => x.Name).ToList();


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

        // GET: /ProductApproval/
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult Index1(long shopId)
        {
            try
            {
                //int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                //var QueryResult = (from TP in db.TempProducts
                //                   join TPV in db.TempProductVarients on new { ID = TP.ID } equals new { ID = TPV.ProductID }
                //                   join TSP in db.TempShopProducts on new { ID = TP.ID } equals new { ID = TSP.ProductID }
                //                   join TSS in db.TempShopStocks on new { ID = TPV.ID } equals new { ID = TSS.ProductVarientID }
                //                   join CTG in db.Categories on new { CategoryID = TP.CategoryID } equals new { CategoryID = CTG.ID }
                //                   join BRD in db.Brands on new { BrandID = TP.BrandID } equals new { BrandID = BRD.ID }
                //                   where TSP.ShopID == shopId && TP.IsActive == true && TP.ApprovalStatus != approvalStatus
                //                   group new { TSS, TP, CTG, BRD, TPV, TSP } by new
                //                   {
                //                       ShopID = shopId,
                //                       TSS.ShopProductID,
                //                       TP.Name,
                //                       Column1 = CTG.Name,
                //                       Column2 = BRD.Name,
                //                       TP.ID,
                //                       TP.ApprovalStatus,
                //                       TP.ApprovalRemark
                //                   } into g
                //                   select new ProductUploadViewModel
                //                   {
                //                       Qty = g.Sum(p => p.TSS.Qty),
                //                       ProductVarientID = g.Count(p => p.TPV.ID != null),
                //                       ShopProductID = g.Key.ShopProductID,
                //                       ProductName = g.Key.Name,
                //                       CategoryName = g.Key.Column1,
                //                       BrandName = g.Key.Column2,
                //                       ProductID = g.Key.ID,
                //                       ShopID = g.Key.ShopID,
                //                       ApprovalStatus = g.Key.ApprovalStatus,
                //                       ApprovalRemark = g.Key.ApprovalRemark,
                //                       ColorID = g.Max(p => p.TPV.ColorID)
                //                   }).OrderByDescending(x => x.ProductID).ToList();
                //List<ProductUploadTempViewModel> listProductUploadTemp = new List<ProductUploadTempViewModel>();

                //foreach (var ReadRecord in QueryResult)
                //{
                //    ProductUploadTempViewModel putvm = new ProductUploadTempViewModel();
                //    putvm.ShopID = ReadRecord.ShopID;
                //    putvm.ID = ReadRecord.ProductID;
                //    putvm.CategoryName = ReadRecord.CategoryName;
                //    putvm.BrandName = ReadRecord.BrandName;
                //    putvm.Name = ReadRecord.ProductName;
                //    putvm.Qty = ReadRecord.Qty;
                //    putvm.PackSize = ReadRecord.PackSize;
                //    putvm.ProductVarientID = ReadRecord.ProductVarientID;
                //    putvm.ColorID = ReadRecord.ColorID;
                //    putvm.ApprovalRemark = ReadRecord.ApprovalRemark;
                //    putvm.ApprovalStatus = ReadRecord.ApprovalStatus;
                //    if (ReadRecord.ColorID == 1)
                //    {
                //        putvm.ImageLocation = ImageDisplay.SetProductThumbPath(ReadRecord.ProductID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                //    }
                //    else
                //    {
                //        putvm.ImageLocation = ImageDisplay.SetProductThumbPath(ReadRecord.ProductID, db.Colors.Where(x => x.ID == ReadRecord.ColorID).FirstOrDefault().Name, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                //    }
                //    listProductUploadTemp.Add(putvm);
                //}

                //ProductUploadTempViewModelList PUTVML = new ProductUploadTempViewModelList();
                //PUTVML.ProductUploadTempViewModelLIst = listProductUploadTemp;
                //return View(PUTVML);

                var QueryResult = (from TPV in db.ProductVarients
                                   join TP in db.Products on TPV.ProductID equals TP.ID
                                   join TSP in db.ShopProducts on TP.ID equals TSP.ProductID
                                   join TSS in db.ShopStocks on TSP.ID equals TSS.ShopProductID
                                   join CTG in db.Categories on TP.CategoryID equals CTG.ID
                                   join BRD in db.Brands on TP.BrandID equals BRD.ID
                                   join SHP in db.Shops on TSP.ShopID equals SHP.ID
                                   where TSP.ShopID == shopId && TPV.ID == TSS.ProductVarientID && TPV.IsActive == false
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
                                       ColorID = g.Max(p => p.TPV.ColorID)
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
                    + "[ProductApproval][GET:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product Upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][GET:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return View();

        }

        [SessionExpire]
        [ValidateInput(false)]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult VariantAproval(long id, long shopId)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            long PersonalDetailID = GetPersonalDetailID();

            try
            {
                ViewBag.ProductID = id;

                ViewBag.textarea = CommonFunctions.LoadDescFile(id, BusinessLogicLayer.ProductUpload.IMAGE_TYPE.Approved);
                ViewBag.InstitutionalMerchantSale = db.Shops.Where(x => x.ID == shopId).Select(x => x.InstitutionalMerchantSale).FirstOrDefault();

                Product TP = db.Products.Find(id);
                productuploadtempviewmodel.Name = TP.Name;
                productuploadtempviewmodel.ID = TP.ID;
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

                ShopProduct TSP = db.ShopProducts.Where(x => x.ProductID == id && x.ShopID == shopId).FirstOrDefault();
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
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == shopId
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category.OrderBy(x => x.Name).ToList(), "ID", "Name", productuploadtempviewmodel.CategoryID);
                ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                ViewBag.BrandID = new SelectList(db.Brands.OrderBy(x => x.Name).Where(c => c.IsActive == true).ToList(), "ID", "Name", productuploadtempviewmodel.BrandID);
                ViewBag.ddlCategoryFirstID = new SelectList(db.Categories.Where(c => c.Level == 1), "ID", "Name", productuploadtempviewmodel.ddlCategoryFirstID);
                ViewBag.ddlCategorySecondID = new SelectList(db.Categories.Where(c => c.Level == 2), "ID", "Name", productuploadtempviewmodel.ddlCategorySecondID);

                var FranchiseID = db.Shops.Where(x => x.ID == shopId).Select(x => x.FranchiseID).FirstOrDefault();
                var TaxIDD = (from TM in db.TaxationMasters
                              join FTD in db.FranchiseTaxDetails on TM.ID equals FTD.TaxationID
                              where FTD.IsActive == true && TM.IsActive == true && FTD.FranchiseID == FranchiseID
                              select new ForLoopClass { Name = TM.Name, ID = TM.ID }).OrderBy(x => x.Name).ToList();

                var query = (from TPV in db.ProductVarients
                             join TSS in db.ShopStocks on TPV.ID equals (TSS.ProductVarientID)
                             join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                             join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                             join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                             join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                             join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                             where TPV.ProductID == id && TSS.ShopProduct.ShopID == shopId
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
                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }

        [HttpPost]
        [SessionExpire]
        [CustomAuthorize(Roles = "ProductUploadAproved/CanWrite")]
        [ValidateInput(false)]
        public ActionResult VariantAproval([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,DeliveryTime,DeliveryRate,TaxRate,TaxRatePer,IsSelect,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,NewProductVarientS,NewProductVarientPOP,CategoryL_2,Path,pathValue,HSNCode,EANCode")] ProductUploadTempViewModel productuploadtempviewmodel, List<HttpPostedFileBase> files_0, List<HttpPostedFileBase> files_1, List<HttpPostedFileBase> files_2, List<HttpPostedFileBase> files_3, List<HttpPostedFileBase> files_4, List<HttpPostedFileBase> files_5, List<HttpPostedFileBase> files_6, List<HttpPostedFileBase> files_7, List<HttpPostedFileBase> files_8, List<HttpPostedFileBase> files_9, List<HttpPostedFileBase> files_10, List<HttpPostedFileBase> files_11, List<HttpPostedFileBase> files_12, List<HttpPostedFileBase> files_13, List<HttpPostedFileBase> files_14, List<HttpPostedFileBase> files_15,
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
                                TP.HSNCode = productuploadtempviewmodel.HSNCode;   ///Added by Priti on 2/11/2018  
                                TP.EANCode = productuploadtempviewmodel.EANCode;   ///Added by Priti on 2/11/2018
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
                                                tempProductVarient.IsActive = true;
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
                                                tempShopStock.IsInclusiveOfTax = TaxationManagement.GetTaxStatus(NewinclusiveOfTax[count]);
                                                tempShopStock.WholeSaleRate = i.WholeSaleRate;
                                                tempShopStock.IsActive = i.SS_IsActive;
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
                                                tempShopStock.IsInclusiveOfTax = TaxationManagement.GetTaxStatus(NewinclusiveOfTax[count]);
                                                tempShopStock.WholeSaleRate = i.WholeSaleRate;
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
                                            // tempProductVarient.IsActive = i.IsActive;
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
                                            tempShopStock.RetailerRate = i.RetailerRate;
                                            tempShopStock.IsInclusiveOfTax = TaxationManagement.GetTaxStatus(NewinclusiveOfTax[count]);
                                            tempShopStock.WholeSaleRate = i.WholeSaleRate;
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
                                                this.DeleteVariantTaxProduct(i.ShopStockID);
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
                                //TempData.Add("Message", "Changes Saved Successfully.");
                                TempData["Message"] = "Changes Saved Successfully.";
                                ts.Complete();
                                return RedirectToAction("VariantAproval", new { id = productuploadtempviewmodel.ID, shopId = productuploadtempviewmodel.ShopID });
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

                        return RedirectToAction("VariantAproval", new { id = productuploadtempviewmodel.ID, shopID = productuploadtempviewmodel.ShopID });


                    case "Approve":

                        long NewVariantID = Convert.ToInt64(strTemp[1]);

                        foreach (var i in productuploadtempviewmodel.NewProductVarientS)
                        {
                            if (i.ProductVarientID == NewVariantID)
                            {
                                using (TransactionScope ts = new TransactionScope())
                                {
                                    ProductVarient tempProductVarient1 = db.ProductVarients.Find(i.ProductVarientID);
                                    tempProductVarient1.ProductID = productuploadtempviewmodel.ID;
                                    tempProductVarient1.ColorID = i.ColorID;
                                    tempProductVarient1.DimensionID = i.DimensionID;
                                    tempProductVarient1.SizeID = i.SizeID;
                                    tempProductVarient1.MaterialID = i.MaterialID;
                                    tempProductVarient1.IsActive = true;
                                    tempProductVarient1.CreateDate = DateTime.Now;
                                    tempProductVarient1.CreateBy = PersonalDetailID;
                                    tempProductVarient1.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    tempProductVarient1.DeviceID = "x";
                                    tempProductVarient1.DeviceType = "x";
                                    //dc.TempProductVarients.Add(tempProductVarient);
                                    db.SaveChanges();

                                    ShopStock tempShopStock = db.ShopStocks.Find(i.ShopStockID);
                                    // tempShopStock.ShopProductID = TSP.ID;
                                    tempShopStock.ProductVarientID = tempProductVarient1.ID;
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
                                    tempShopStock.ModifyDate = DateTime.Now;
                                    tempShopStock.ModifyBy = PersonalDetailID;
                                    tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    tempShopStock.DeviceID = "x";
                                    tempShopStock.DeviceType = "x";
                                    db.SaveChanges();

                                    ts.Complete();
                                }

                            }
                        }

                        //return RedirectToAction("Index", new { shopId = productuploadtempviewmodel.ShopID });
                        return RedirectToAction("VariantAproval", new { id = productuploadtempviewmodel.ID, shopID = productuploadtempviewmodel.ShopID });

                    case "Default":
                        return RedirectToAction("VariantAproval", new { id = productuploadtempviewmodel.ID, shopID = productuploadtempviewmodel.ShopID });
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

        public ActionResult Remove1(long? id)
        {
            ProductVarient tempProductVarient = db.ProductVarients.Find(id);
            ShopStock tempShopStock = db.ShopStocks.Where(x => x.ProductVarientID == id).FirstOrDefault();

            Product tempProduct = db.Products.Where(x => x.ID == tempProductVarient.ProductID).FirstOrDefault();

            // ShopProduct tempShopProduct = db.ShopProducts.Where(x => x.ProductID == tempProduct.ID).FirstOrDefault();
            ShopProduct tempShopProduct = db.ShopProducts.Where(x => x.ID == tempShopStock.ShopProductID).FirstOrDefault();

            var ColorID = db.ProductVarients.Where(x => x.ID == id).Select(x => x.ColorID).FirstOrDefault();

            using (TransactionScope ts = new TransactionScope())
            {
                db.ProductVarients.Remove(tempProductVarient);
                db.ShopStocks.Remove(tempShopStock);
                db.SaveChanges();

                var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
                // var aaa = db.TempProductVarients.Where(x => x.ProductID == tempProductVarient.ProductID && x.ColorID == ColorID).Select(x => x.ColorID).Count();
                if (db.ProductVarients.Where(x => x.ProductID == tempProductVarient.ProductID && x.ColorID == ColorID).Select(x => x.ColorID).Count() == 0)
                {
                    CommonFunctions.DeleteVariantImages(tempProductVarient.ProductID, colorName.Name, string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }
                ts.Complete();

            }

            return RedirectToAction("VariantAproval", new { ID = tempProductVarient.ProductID, ShopID = tempShopProduct.ShopID });


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
                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);



            }
            return RedirectToAction("Index");

        }
        //===========================================
        #endregion

        #region Code for GBCatalog
        //========================================
        [SessionExpire]
        public ActionResult GBCatalog1(long id, long? ShopID)
        {
            ProductUploadTempViewModel productuploadtempviewmodel = new ProductUploadTempViewModel();
            //long ShopID = GetShopID();
            long PersonalDetailID = GetPersonalDetailID();


            try
            {

                string[] src = ImageDisplay.DisplayProductImages(id, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                ViewBag.ImageURL = src;
                productuploadtempviewmodel.Path = src;

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

                        ShopProduct TSP = db.ShopProducts.Where(x => x.ProductID == id).First();
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
                                        where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                                        select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                        ViewBag.CategoryID = new SelectList(Category, "ID", "Name", productuploadtempviewmodel.CategoryID);
                        ViewBag.DisplayProductFromDate1 = productuploadtempviewmodel.DisplayProductFromDate.ToString("dd/MM/yyyy");
                        ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name", productuploadtempviewmodel.BrandID);

                        //var query = (from TPV in db.ProductVarients
                        //             join TSS in db.ShopStocks on TPV.ID equals (TSS.ProductVarientID)
                        //             join SP in db.ShopProducts on TSS.ShopProductID equals (SP.ID)
                        //             join CLR in db.Colors on TPV.ColorID equals (CLR.ID)
                        //             join SIZ in db.Sizes on TPV.SizeID equals (SIZ.ID)
                        //             join DMS in db.Dimensions on TPV.DimensionID equals (DMS.ID)
                        //             join MTR in db.Materials on TPV.MaterialID equals (MTR.ID)
                        //             join UNT in db.Units on TSS.PackUnitID equals (UNT.ID)
                        //             where TPV.ProductID == id && SP.ShopID == ShopID

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
                            //newProductVarient.Qty = i.Qty;
                            //newProductVarient.ReorderLevel = i.ReorderLevel;
                            //newProductVarient.StockStatus = i.StockStatus;
                            //newProductVarient.PackSize = i.PackSize;
                            //newProductVarient.PackUnitID = i.PackUnitID;
                            //newProductVarient.MRP = i.MRP;
                            //newProductVarient.RetailerRate = i.RetailerRate;
                            //newProductVarient.ShopStockID = i.ShopStockID;
                            //newProductVarient.ColorName = i.ColorName;
                            //newProductVarient.SizeName = i.SizeName;
                            //newProductVarient.DiamentionName = i.DiamentionName;
                            //newProductVarient.MaterialName = i.MaterialName;
                            //newProductVarient.UnitName = i.UnitName;
                            //newProductVarient.IsInclusiveOfTax = i.IsInclusiveOfTax;

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
                    else
                    {
                        TempData["MyErrorMsg"] = "Your Product Limit is Exceed ! You can't Add more product ";
                        TempData.Keep();
                        return RedirectToAction("Create", "ProductUploadTemp");
                    }
                }
                else
                {
                    TempData["MyErrorMsg"] = "This product is already available in your shop";
                    TempData.Keep();
                    return RedirectToAction("Create", "ProductUploadTemp");
                }
            }


            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            return View(productuploadtempviewmodel);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public ActionResult GBCatalog1([Bind(Include = "ID,Name,CategoryID,WeightInGram,LengthInCm,BreadthInCm,HeightInCm,Description,BrandID,SearchKeyword,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy,NetworkIP,DeviceType,DeviceID,ProductVarientID,ColorID,SizeID,DimensionID,MaterialID,TempShopProductID,ShopID,DisplayProductFromDate,Qty,ReorderLevel,ShopStockID,ShopProductID,StockStatus,PackSize,PackUnitID,MRP,WholeSaleRate,RetailerRate,IsInclusiveOfTax,NewProductVarientS,CategoryL_2,Path,pathValue,IsSelect,ShopID")] ProductUploadTempViewModel productuploadtempviewmodel, List<HttpPostedFileBase> Files, FormCollection collection, string submit, string DisplayProductFromDate1)
        {


            string strValue = submit;
            string[] strTemp = strValue.Split('$');

            var val1 = strTemp[0];
            var val2 = strTemp[1];
            if (val2 == "")
            {
                val2 = "1";
            }

            submit = val1.ToString();

            int StrID = Convert.ToInt32(val2);

            try
            {
                // long ShopID = GetShopID();
                long PersonalDetailID = GetPersonalDetailID();
                DateTime lDisplayProductFromDate = DateTime.Now;
                if (DisplayProductFromDate1 != "")
                {
                    if (DateTime.TryParse(DisplayProductFromDate1, out lDisplayProductFromDate)) { }
                    productuploadtempviewmodel.DisplayProductFromDate = lDisplayProductFromDate;
                }


                switch (submit)
                {
                    case "Save":
                        var Category = (from c in db.Categories
                                        join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                                        join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                                        join P in db.Plans on op.PlanID equals P.ID
                                        where P.PlanCode.Contains("GBMR") && op.OwnerID == productuploadtempviewmodel.ShopID
                                        select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                        ViewBag.CategoryID = new SelectList(Category, "ID", "Name");

                        ViewBag.BrandID = new SelectList(db.Brands, "ID", "Name");

                        ViewBag.ColorID = new SelectList(db.Colors, "ID", "Name");
                        ViewBag.SizeID = new SelectList(db.Sizes, "ID", "Name");
                        ViewBag.DimensionID = new SelectList(db.Dimensions, "ID", "Name");
                        ViewBag.MaterialID = new SelectList(db.Materials, "ID", "Name");
                        ViewBag.PackUnitID = new SelectList(db.Units, "ID", "Name");

                        EzeeloDBContext db1 = new EzeeloDBContext();



                        ShopProduct TSP = new ShopProduct();
                        TSP.ShopID = productuploadtempviewmodel.ShopID;
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

                        using (EzeeloDBContext dc = new EzeeloDBContext())
                        {
                            foreach (var i in productuploadtempviewmodel.NewProductVarientS)
                            {
                                if (i.ProductVarientID == 0)
                                {
                                    ProductVarient tempProductVarient = new ProductVarient();
                                    tempProductVarient.ProductID = productuploadtempviewmodel.ID;
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
                                    dc.ProductVarients.Add(tempProductVarient);
                                    dc.SaveChanges();

                                    ShopStock tempShopStock = new ShopStock();
                                    tempShopStock.ShopProductID = TSP.ID;
                                    tempShopStock.ProductVarientID = tempProductVarient.ID;
                                    tempShopStock.Qty = i.Qty;
                                    tempShopStock.ReorderLevel = i.ReorderLevel;
                                    tempShopStock.StockStatus = true;
                                    tempShopStock.PackSize = i.PackSize;
                                    tempShopStock.PackUnitID = i.PackUnitID;
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
                                else if (i.IsSelect == true)
                                {

                                    ProductVarient tempProductVarient = dc.ProductVarients.Find(i.ProductVarientID);
                                    //tempProductVarient.ProductID = productuploadtempviewmodel.ID;
                                    //tempProductVarient.ColorID = i.ColorID;
                                    //tempProductVarient.DimensionID = i.DimensionID;
                                    //tempProductVarient.SizeID = i.SizeID;
                                    //tempProductVarient.MaterialID = i.MaterialID;
                                    //tempProductVarient.IsActive = true;
                                    //tempProductVarient.CreateDate = DateTime.Now;
                                    //tempProductVarient.CreateBy = PersonalDetailID;
                                    //tempProductVarient.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    //tempProductVarient.DeviceID = "x";
                                    //tempProductVarient.DeviceType = "x";
                                    //dc.SaveChanges();



                                    //ShopStock tempShopStock = dc.ShopStocks.Find(i.ShopStockID);
                                    //tempShopStock.ShopProductID = TSP.ID;
                                    //tempShopStock.ProductVarientID = tempProductVarient.ID;
                                    //tempShopStock.Qty = i.Qty;
                                    //tempShopStock.ReorderLevel = i.ReorderLevel;
                                    //tempShopStock.StockStatus = true;
                                    //tempShopStock.PackSize = i.PackSize;
                                    //tempShopStock.PackUnitID = i.PackUnitID;
                                    //tempShopStock.MRP = i.MRP;
                                    ////tempShopStockSS.WholeSaleRate = i.WholeSaleRate;
                                    //tempShopStock.RetailerRate = i.RetailerRate;
                                    //tempShopStock.IsInclusiveOfTax = i.IsInclusiveOfTax;
                                    //tempShopStock.IsActive = true;
                                    //tempShopStock.CreateDate = DateTime.Now;
                                    //tempShopStock.CreateBy = PersonalDetailID;
                                    //tempShopStock.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                                    //tempShopStock.DeviceID = "x";
                                    //tempShopStock.DeviceType = "x";
                                    //// dc.TempShopStocks.Add(tempShopStock);
                                    //dc.SaveChanges();
                                    ShopStock tempShopStock = new ShopStock();
                                    tempShopStock.ShopProductID = TSP.ID;
                                    tempShopStock.ProductVarientID = tempProductVarient.ID;
                                    tempShopStock.Qty = i.Qty;
                                    tempShopStock.ReorderLevel = i.ReorderLevel;
                                    tempShopStock.StockStatus = true;
                                    tempShopStock.PackSize = i.PackSize;
                                    tempShopStock.PackUnitID = i.PackUnitID;
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

                        return RedirectToAction("Index");

                    case "Remove":
                        CommonFunctions.DeleteProductImages(productuploadtempviewmodel.Path[StrID], productuploadtempviewmodel.ID, productuploadtempviewmodel.Name, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        // return View(productuploadtempviewmodel);
                        return RedirectToAction("Edit", productuploadtempviewmodel.ID);

                    case "Default":

                        CommonFunctions.DeleteProductImages(productuploadtempviewmodel.Path[StrID], productuploadtempviewmodel.ID, productuploadtempviewmodel.Name, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        // return View(productuploadtempviewmodel);
                        return RedirectToAction("Edit", productuploadtempviewmodel.ID);
                }
                //}


            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong with the Product upload!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductApproval][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

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
                    + "[ProductApproval][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);

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
            return RedirectToAction("Index");
            //return View(productuploadtempviewmodel);
        }
        //========================================
        #endregion

        #region Specification
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
            //List<CategorySpecificationList> obj = new List<CategorySpecificationList>();

            //obj = (from cs in db.TempProductSpecifications
            //       where cs.ProductID == productID
            //       join s in db.Specifications on cs.SpecificationID equals s.ID
            //       where cs.ProductID == productID
            //       select new CategorySpecificationList
            //       {
            //           SpecificationID = cs.SpecificationID,
            //           ParentID = s.ParentSpecificationID,
            //           SpecificationName = s.Name,
            //           level = s.Level,
            //           SpecificationValue = cs.Value
            //       }
            //       ).ToList();

            //return Json(obj, JsonRequestBehavior.AllowGet);


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

        public JsonResult SpecificationList1(int categoryID, int? productID)
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

        public JsonResult ProductSpecification1(int productID)
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

        #endregion

        #region Methods

        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult DownloadFile()
        {
            return Redirect("ftp://192.168.1.106/Content/FileDownload/Product.docx");
        }

        //public ActionResult GetImages(long productID, long ColorID)
        //{

        //    if (ColorID == 1)
        //    {
        //        var Result = ImageDisplay.DisplayProductImages(productID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
        //        return Json(Result, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {

        //        var colorName = db.Colors.Where(x => x.ID == ColorID).FirstOrDefault();
        //        var Result = ImageDisplay.DisplayProductImages(productID, colorName.Name, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
        //        return Json(Result, JsonRequestBehavior.AllowGet);
        //    }

        //}

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
                // Result.Add((ImgPaths);
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

        private long GetPersonalDetailID()
        {
            //Session["USER_LOGIN_ID"] = 1;
            long UserLoginID = Convert.ToInt32(Session["ID"]);
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

        private void DeleteTempEntry(long productId, long shopId)
        {
            try
            {
                //  Session["ID"] = 1;
                List<TempProductSpecification> lTPS = db.TempProductSpecifications.Where(x => x.ProductID == productId).ToList();
                foreach (TempProductSpecification TpsRow in lTPS)
                {
                    db.TempProductSpecifications.Remove(TpsRow);
                    db.SaveChanges();
                }

                List<TempProductVarient> TPV = db.TempProductVarients.Where(x => x.ProductID == productId).ToList();
                foreach (TempProductVarient TpvRow in TPV)
                {
                    TempShopStock TSS = db.TempShopStocks.Where(x => x.ProductVarientID == TpvRow.ID).FirstOrDefault();

                    List<TempStockComponent> lTSC = db.TempStockComponents.Where(x => x.ShopStockID == TSS.ID).ToList();
                    foreach (TempStockComponent TscRow in lTSC)
                    {
                        db.TempStockComponents.Remove(TscRow);
                        db.SaveChanges();
                    }

                    //changes by Manoj for deleting TmpProductTax 
                    var ID = db.TempProductTaxes.Where(x => x.ShopStockID == TSS.ID).Select(x => x.ID).ToList();
                    foreach (var item in ID)
                    {
                        TempProductTax tempProductTax = db.TempProductTaxes.Find(item);
                        db.TempProductTaxes.Remove(tempProductTax);
                        db.SaveChanges();
                    }

                    EzeeloDBContext db2 = new EzeeloDBContext();
                    TempShopStock TSS1 = db2.TempShopStocks.Find(TSS.ID);

                    //Change by Harshada to Delete Bulk Products from ShopStockBulkLog and productBulkDetail (6/1/2016)
                    var BulkStock = db.ShopStockBulkLogs.Where(x => x.TempProductID == productId && x.TempShopStockID == TSS.ID).ToList();
                    if (BulkStock.Count() > 0)
                    {
                        foreach (var bulk in BulkStock)
                        {
                            ShopStockBulkLog shopStockBulkLog = db.ShopStockBulkLogs.Where(x => x.BulkLogID == bulk.BulkLogID).FirstOrDefault();
                            db.ShopStockBulkLogs.Remove(shopStockBulkLog);
                        }
                    }
                    var bulkLogID = db.ProductBulkDetails.Where(x => x.TempProductID == productId).ToList();
                    if (bulkLogID.Count() > 0)
                    {
                        foreach (var bulkId in bulkLogID)
                        {
                            ProductBulkDetail productBulkDetail = db.ProductBulkDetails.Where(x => x.TempProductID == bulkId.TempProductID).FirstOrDefault();
                            db.ProductBulkDetails.Remove(productBulkDetail);

                        }
                    }



                    //End of Delete Bulk Products from ShopStockBulkLog and productBulkDetail
                    db.TempShopStocks.Remove(TSS);
                    db.SaveChanges();

                    db.TempProductVarients.Remove(TpvRow);
                    db.SaveChanges();
                }

                TempShopProduct TSP = db.TempShopProducts.Where(x => x.ProductID == productId).Where(x => x.ShopID == shopId).FirstOrDefault();
                db.TempShopProducts.Remove(TSP);
                db.SaveChanges();

                TempProduct TP = db.TempProducts.Find(productId);
                db.TempProducts.Remove(TP);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => new { x.ErrorMessage, x.PropertyName });

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][DeleteTempEntry]", "Can't Delete Temp Table Values!" + Environment.NewLine + ex.Message);
            }
        }

        private void IsProductApprovable(long ProductID, long ShopID, bool IsProprietory, ref string msg)
        {
            //Comment :
            //1. Check Product Limit of merchant
            //2. Check Validation for the approval of product
            //3. Check component is present or not if component is not present open component in edit mode and do the changes if required and then go to step 5.
            //4. If component is not present go to step 5.
            //5. Approve the product
            //6. Set status of approval

            //try
            //{
            bool IsCompPresent = false;
            int status = 0;
            if (CheckProductLimitOfMerchant(ShopID))
            {
                if (CheckValidationForApproval(ProductID, ShopID, ref msg, ref status))
                {
                    IsCompPresent = IsComponentPresent(ProductID, ShopID);
                    if (IsCompPresent)
                    {
                        msg = "Component$" + status + "";

                        ModelState.AddModelError("CustomError", msg);
                        //return DisplayComponent(ProductID, ShopID);
                        //return View("DisplayComponent",lst);
                    }
                    else
                    {   //Change on 15/09/2015
                        ApproveProduct(ProductID, ShopID, IsProprietory, ref msg, ref status);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            SetApprovalStatus(ProductID, ref msg, ref status);
                            ViewBag.ProductID = ProductID;
                            ModelState.AddModelError("CustomError", msg);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(msg))
                {
                    SetApprovalStatus(ProductID, ref msg, ref status);
                    ViewBag.ProductID = ProductID;
                    ModelState.AddModelError("CustomError", msg);
                }
            }
            else
            {
                msg = "Sorry!! You can't proceed, Product approve limit has been exceeded...";
                status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.PRODUCT_LIMIT_EXCEEDED);
                SetApprovalStatus(ProductID, ref msg, ref status);
                ViewBag.ProductID = ProductID;
                ModelState.AddModelError("CustomError", msg);
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new BusinessLogicLayer.MyException("[ProductApproval][IsProductApprovable]", "Can't Approve Product!" + Environment.NewLine + ex.Message);
            //}
        }

        private bool IsComponentPresent(long ProductID, long ShopID)
        {
            bool IsCompPresent = false;
            try
            {
                long ShopProductId = db.TempShopProducts.Where(x => x.ProductID == ProductID && x.ShopID == ShopID).Select(x => x.ID).FirstOrDefault();
                if (ShopProductId > 0)
                {
                    var ShopStock = db.TempShopStocks.Where(x => x.ShopProductID == ShopProductId).ToList();
                    if (ShopStock != null)
                    {
                        var Component = (from ss in ShopStock
                                         join tsc in db.TempStockComponents on ss.ID equals tsc.ShopStockID
                                         where ss.ShopProductID == ShopProductId
                                         select new
                                         {
                                             ss.ID
                                         }).ToList();
                        //var ComponentID = db.TempStockComponents.Where(x => x.ShopStockID == ShopStockID).Select(x => x.ID).FirstOrDefault();
                        if (Component.Count() > 0)
                        {
                            IsCompPresent = true;
                        }
                        else
                        {
                            IsCompPresent = false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][IsComponentPresent]", "Can't Approve Product!" + Environment.NewLine + ex.Message);
            }
            return IsCompPresent;
        }

        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        [CustomAuthorize(Roles = "ProductApproval/CanWrite")]
        public ActionResult DisplayComponent(long ProductID, long ShopID, bool IsProp, int Status)
        {
            try
            {
                ShopID1 = ShopID;
                ViewBag.ProductID = ProductID;
                ViewBag.IsProprietory = IsProp;
                ViewBag.ShopID = ShopID;
                ViewBag.Status = Status;
                decimal TotalSaleRate = 0;
                var ProductDetails = (from SC in db.TempStockComponents
                                      join SS in db.TempShopStocks on SC.ShopStockID equals SS.ID
                                      join SP in db.TempShopProducts on SS.ShopProductID equals SP.ID
                                      join P in db.TempProducts on SP.ProductID equals P.ID
                                      where P.ID == ProductID && SP.ShopID == ShopID

                                      select new
                                      {
                                          ProductID = P.ID,
                                          ProductName = P.Name,
                                          ShopStockID = SS.ID

                                      }).ToList();

                var ComponentDetails = (from tsc in db.TempStockComponents

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
                                          scp.ShopID == ShopID
                                        select new ShopProductComponentViewModel
                                        {
                                            ComponentID = c.ID,
                                            ComponentName = c.Name,
                                            ComponentWeight = tsc.ComponentWeight,
                                            ComponentUnitID = (u.ID == null ? 0 : u.ID),
                                            ComponentUnitName = (u.Name == string.Empty ? string.Empty : u.Name),
                                            DependentOnComponentID = (scp.DependentOnComponentID == null ? 0 : scp.DependentOnComponentID),
                                            PerUnitRateInRs = scp.PerUnitRateInRs,
                                            PerUnitRateInPer = scp.PerUnitRateInPer,
                                            //Total = tss.MRP,
                                            DependentComponentName = (dc.Name == string.Empty ? "N/A" : dc.Name),
                                            ProductVarientID = tss.ProductVarientID,
                                        }).Distinct().ToList();


                List<ProductVarientViewModel> listProductComponent = new List<ProductVarientViewModel>();
                foreach (var pID in ProductDetails)
                {
                    //long ProduId=Convert.ToInt64(pID);
                    var ProductDetails1 = (from SC in db.TempStockComponents
                                           join SS in db.TempShopStocks on SC.ShopStockID equals SS.ID
                                           join SP in db.TempShopProducts on SS.ShopProductID equals SP.ID
                                           join P in db.TempProducts on SP.ProductID equals P.ID
                                           join C in db.Components on SC.ComponentID equals C.ID
                                           join PV in db.TempProductVarients on P.ID equals PV.ProductID into PV_join
                                           from PV in PV_join.DefaultIfEmpty()
                                               //join PV in db.TempProductVarients on SS.ProductVarientID equals PV.ID
                                           join Co in db.Colors on PV.ColorID equals Co.ID
                                           join S in db.Sizes on PV.SizeID equals S.ID
                                           join D in db.Dimensions on PV.DimensionID equals D.ID
                                           join M in db.Materials on PV.MaterialID equals M.ID
                                           where SP.ShopID == ShopID && P.ID == pID.ProductID


                                           select new
                                           {
                                               ProductID = P.ID,
                                               ProductName = P.Name,
                                               ProductVarientID = PV.ID,
                                               ColorID = Co.ID,
                                               ColorName = Co.Name,
                                               SizeID = S.ID,
                                               SizeName = S.Name,
                                               DimensionID = D.ID,
                                               DimensionName = D.Name,
                                               MaterialID = M.ID,
                                               MaterialName = M.Name,
                                               Total = 0,
                                               ShopStockID = SS.ID

                                           }).ToList();

                    foreach (var product in ProductDetails1)
                    {
                        ProductVarientViewModel ObjProductComponentViewModel = new ProductVarientViewModel();
                        ObjProductComponentViewModel.ProductID = product.ProductID;
                        ObjProductComponentViewModel.ProductVarientID = product.ProductVarientID;
                        ObjProductComponentViewModel.ProductName = product.ProductName;
                        ObjProductComponentViewModel.ColorName = product.ColorName;
                        ObjProductComponentViewModel.SizeName = product.SizeName;
                        ObjProductComponentViewModel.DimensionName = product.DimensionName;
                        ObjProductComponentViewModel.MaterialName = product.MaterialName;
                        //Total = Math.Round(Total, 2);
                        //ObjProductComponentViewModel.Total = Total;
                        ObjProductComponentViewModel.ShopStockID = product.ShopStockID;

                        long ProdId = ObjProductComponentViewModel.ProductID;
                        if (ObjProductComponentViewModel.ColorName == "N/A")
                        {
                            // ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, string.Empty, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                            ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                        }
                        else
                        {
                            //ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, ObjProductComponentViewModel.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                            ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                        }
                        long productVariantID = product.ProductVarientID;
                        var abc = ComponentDetails.Where(x => x.ProductVarientID == productVariantID).ToList();
                        foreach (var Component in abc)
                        {
                            ShopProductComponentViewModel ObjComponentList = new ShopProductComponentViewModel();
                            ObjComponentList.ComponentID = Component.ComponentID;
                            ObjComponentList.ComponentName = Component.ComponentName;
                            ObjComponentList.ComponentWeight = Component.ComponentWeight;
                            ObjComponentList.PerUnitRateInPer = Component.PerUnitRateInPer;
                            ObjComponentList.PerUnitRateInRs = Component.PerUnitRateInRs;
                            ObjComponentList.ComponentUnitID = Component.ComponentUnitID;
                            ObjComponentList.ComponentUnitName = Component.ComponentUnitName;
                            ObjComponentList.DependentOnComponentID = Component.DependentOnComponentID;
                            ObjComponentList.DependentComponentName = Component.DependentComponentName;
                            ObjProductComponentViewModel.shopProductComponentList.Add(ObjComponentList);
                        }
                        TotalSaleRate = CalculateComponentTotal(ObjProductComponentViewModel.shopProductComponentList);
                        TotalSaleRate = CalculateVAT(ObjProductComponentViewModel.shopProductComponentList, TotalSaleRate);

                        ObjProductComponentViewModel.TotalSaleRate = TotalSaleRate;
                        listProductComponent.Add(ObjProductComponentViewModel);


                    }
                }
                return View(listProductComponent);
                //return View("_ProductDetails", listProductComponent.ToList().ToPagedList(page ?? 1, 10));
            }


            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading index of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading index of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }

            return View();
        }

        private decimal CalculateVAT(List<ShopProductComponentViewModel> list, decimal TotalSaleRate)
        {

            decimal VatRate = 0;
            decimal VatPercent = 0;
            try
            {
                foreach (var comp in list)
                {
                    if (comp.ComponentName != "VAT")
                    {

                    }
                    else
                    {
                        VatPercent = comp.PerUnitRateInPer;
                        VatRate = TotalSaleRate * VatPercent / 100;
                        comp.ComponentRate = VatRate;
                        TotalSaleRate = Math.Round((TotalSaleRate + VatRate), 2);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopComponentPriceController][CalculateVAT]", "Unable to Calculate vat on components. !" + Environment.NewLine + ex.Message);
            }
            return TotalSaleRate;
        }

        private decimal CalculateComponentTotal(List<ShopProductComponentViewModel> list)
        {
            decimal Total = 0;
            decimal TotalSaleRate = 0;
            try
            {
                foreach (var comp in list)
                {
                    if (comp.DependentOnComponentID > 0)
                    {
                        var cmp = list.Where(x => x.ComponentID == comp.DependentOnComponentID).FirstOrDefault();
                        if (cmp != null)
                        {
                            if (comp.PerUnitRateInPer > 0)
                            {
                                decimal CompTotal = Convert.ToDecimal(cmp.PerUnitRateInRs * cmp.ComponentWeight);
                                Total = CompTotal * comp.PerUnitRateInPer / 100;
                            }
                            else if (comp.PerUnitRateInRs > 0)
                            {

                                Total = Convert.ToDecimal(cmp.ComponentWeight * comp.PerUnitRateInRs);
                            }
                        }
                    }
                    else
                    {
                        Total = Convert.ToDecimal(comp.PerUnitRateInRs * comp.ComponentWeight);
                    }
                    comp.ComponentRate = Total;
                    TotalSaleRate = Math.Round((TotalSaleRate + Total), 2);

                }
            }

            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopComponentPriceController][CalculateComponentTotal]", "Unable to calculate Component Total. !" + Environment.NewLine + ex.Message);
            }
            return TotalSaleRate;
        }
        private void ApproveProduct(long ProductID, long ShopID, bool IsProprietory, ref string msg, ref int status)
        {
            //Comment:
            //1. Start transaction lock
            //2. Insert product related entries in main tables from temp tables.
            //3. check product with same name already exist with different category if not then continue else show error 
            //4. Delete entries from temp table.
            //5. If IsProprietory is true then insert entry in Proprietory table
            //6. If description file exist for product then approve description file.
            //7. Approve product images from FTP

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long newProductId = 0, tempProductId = 0;

                    InsertProduct(ProductID, ShopID);
                    newProductId = Convert.ToInt64(ViewBag.ProductID);
                    tempProductId = ProductID;
                    int tempcategoryid = Convert.ToInt32(ViewBag.TempProductCatID);   //Change on 15/09/2015
                    int newcategoryid = db.Products.Where(y => y.ID == newProductId).Select(y => y.CategoryID).FirstOrDefault();
                    if (tempcategoryid != newcategoryid)
                    {

                        msg = "Product with Same Name already exists!! Either pick from eZeelo catalogue or change product name..";
                        TempProduct TP = db.TempProducts.Find(tempProductId);
                        status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.PRODUCT_WITH_SAME_NAME_ALREADY_EXIST);
                    }
                    else
                    {
                        //Commented on date : 3/10/2015
                        //changes due to bulk upload log record 
                        //for maintaining bulk log we have to keep the tempproduct record
                        //As product approved, approval status and approval remark is maintaining
                        DeleteTempEntry(ProductID, ShopID);

                        if (IsProprietory)
                        {
                            InsertProprietory(newProductId, ShopID);
                        }

                        //change on date 9/9/2015
                        //when same product is aprroves(2nd time) for other shop then images and description file should not be overwritten
                        //change by mohit req.

                        if (ViewBag.IsNewProduct)
                        {
                            if (Convert.ToBoolean(TempData.Peek("Description")))
                            {
                                CommonFunctions.ApproveProductDescription(tempProductId, newProductId);
                            }
                            if (((ArrayList)ViewBag.Colors).Count > 0)
                            {
                                foreach (string str in ViewBag.Colors)
                                {
                                    CommonFunctions.ApproveProductImages(tempProductId, newProductId, str, string.Empty);
                                }
                            }
                            //CommonFunctions.ApproveProductImages(tempProductId, newProductId, "Default", string.Empty);
                        }

                        //Update Approval status in tempproduct
                        //status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                        //msg = "APPROVED";
                        //SetApprovalStatus(tempProductId, ref msg, ref status);
                        //msg = string.Empty;
                        dbContextTransaction.Commit();
                        ViewBag.ProductID = newProductId;
                    }
                }
                catch (Exception ex)
                {
                    throw new BusinessLogicLayer.MyException("[ProductApproval][IsProductApprovable]", "Can't Approve Product!" + Environment.NewLine + ex.Message);
                }
            }
        }

        private void SetApprovalStatus(long tempProductId, ref string msg, ref int status)
        {
            try
            {
                TempProduct TP = db.TempProducts.Find(tempProductId);
                TP.ApprovalStatus = status;
                TP.ApprovalRemark = msg;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][SetApprovalStatus]", "Can't Set Approval Status!" + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost]

        [SessionExpire]
        [CustomAuthorize(Roles = "ProductApproval/CanRead")]
        public ActionResult ApproveProductComponent(long ProductID, long ShopID, bool IsProprietory, string msg, int status)
        {


            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long newProductId = 0, tempProductId = 0;

                    InsertProduct(ProductID, ShopID);
                    newProductId = Convert.ToInt64(ViewBag.ProductID);
                    tempProductId = ProductID;
                    int tempcategoryid = Convert.ToInt32(ViewBag.TempProductCatID);   //Change on 15/09/2015
                    int newcategoryid = db.Products.Where(y => y.ID == newProductId).Select(y => y.CategoryID).FirstOrDefault();
                    if (tempcategoryid != newcategoryid)
                    {
                        msg = "Product with Same Name already exists!! Either pick from eZeelo catalogue or change product name..";
                        TempProduct TP = db.TempProducts.Find(tempProductId);
                        status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.PRODUCT_WITH_SAME_NAME_ALREADY_EXIST);
                    }
                    else
                    {
                        //Commented on date : 3/10/2015
                        //changes due to bulk upload log record 
                        //for maintaining bulk log we have to keep the tempproduct record
                        //As product approved, approval status and approval remark is maintaining
                        //DeleteTempEntry(ProductID, ShopID);

                        if (IsProprietory)
                        {
                            InsertProprietory(newProductId, ShopID);
                        }

                        //change on date 9/9/2015
                        //when same product is aprroves(2nd time) for other shop then images and description file should not be overwritten
                        //change by mohit req.

                        if (ViewBag.IsNewProduct)
                        {
                            if (Convert.ToBoolean(TempData.Peek("Description")))
                            {
                                CommonFunctions.ApproveProductDescription(tempProductId, newProductId);
                            }
                            if (((ArrayList)ViewBag.Colors).Count > 0)
                            {
                                foreach (string str in ViewBag.Colors)
                                {
                                    CommonFunctions.ApproveProductImages(tempProductId, newProductId, str, string.Empty);
                                }
                            }
                            //CommonFunctions.ApproveProductImages(tempProductId, newProductId, "Default", string.Empty);
                        }


                        //Update Approval status in tempproduct
                        status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                        msg = "APPROVED";
                        SetApprovalStatus(tempProductId, ref msg, ref status);
                        msg = string.Empty;
                        dbContextTransaction.Commit();
                        ViewBag.ProductID = newProductId;
                    }
                }


                catch (Exception ex)
                {
                    return Json(new { ok = false, message = ex.Message });
                    throw new BusinessLogicLayer.MyException("[ProductApproval][IsProductApprovable]", "Can't Approve Product!" + Environment.NewLine + ex.Message);
                }
                if (msg == string.Empty)
                {
                    return Json(new { ok = true, newurl = Url.Action("Index", "ProductApproval", new { shopId = ShopID }) });
                }
                else
                {
                    return Json(new { ok = false, message = msg });
                }
            }
        }
        private void InsertProduct(long productId, long shopId)
        {
            //Comment:
            //1. check product with same name exist or not , if not then insert in Product table else go to step 2.
            //2. check product is present in same shop, if not insert in Shop product table else go to step 3.
            //3. check same productvarient is present for the product, if not insert in productVariant table else go to step 4.
            //4. insert entry for shopstock table
            //5. insert taxation entry against shopstock
            //6. insert entry for component if exist
            //7. insert entry for product specification


            //  Session["ID"] = 1;
            try
            {
                ViewBag.IsNewProduct = false;
                TempProduct TP = db.TempProducts.Find(productId);
                ViewBag.TempProductCatID = TP.CategoryID.ToString();     //Change on 15/09/2015
                Product lProduct = db.Products.Where(x => x.Name == TP.Name).FirstOrDefault();
                if (lProduct == null)
                {
                    lProduct = new Product();
                    lProduct.Name = TP.Name;
                    lProduct.CategoryID = TP.CategoryID;
                    lProduct.WeightInGram = TP.WeightInGram;
                    lProduct.LengthInCm = TP.LengthInCm;
                    lProduct.BreadthInCm = TP.BreadthInCm;
                    lProduct.HSNCode = TP.HSNCode;        ///Added by Priti on  2/11/2018
                    lProduct.EANCode = TP.EANCode;           ///Added by Priti on  2/11/2018
                    lProduct.Description = TP.Description;
                    lProduct.BrandID = TP.BrandID;
                    lProduct.SearchKeyword = TP.SearchKeyword;
                    lProduct.IsActive = true;
                    //lProduct.ModifyDate = TP.ModifyDate;
                    //lProduct.ModifyBy = TP.ModifyBy;
                    lProduct.CreateDate = DateTime.UtcNow;
                    lProduct.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    lProduct.NetworkIP = TP.NetworkIP;
                    lProduct.DeviceType = TP.DeviceType;
                    lProduct.DeviceID = TP.DeviceID;
                    if (ModelState.IsValid)
                    {
                        db.Products.Add(lProduct);
                        db.SaveChanges();

                    }
                    ViewBag.IsNewProduct = true;
                }
                ViewBag.ProductID = lProduct.ID.ToString();

                //CommonFunctions.UploadProductImages(Files, TP.Name, TP.ID, "Default", string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                ShopProduct lShopProduct = db.ShopProducts.Where(x => x.ProductID == lProduct.ID).Where(x => x.ShopID == shopId).FirstOrDefault();
                if (lShopProduct == null)
                {
                    lShopProduct = new ShopProduct();
                    TempShopProduct TSP = db.TempShopProducts.Where(x => x.ProductID == productId).Where(x => x.ShopID == shopId).FirstOrDefault();
                    lShopProduct.ShopID = TSP.ShopID;
                    lShopProduct.ProductID = lProduct.ID;
                    lShopProduct.IsActive = true;
                    lShopProduct.DisplayProductFromDate = TSP.DisplayProductFromDate;
                    lShopProduct.DeliveryTime = TSP.DeliveryTime;
                    lShopProduct.DeliveryRate = TSP.DeliveryRate;
                    lShopProduct.TaxRate = TSP.TaxRate;
                    lShopProduct.TaxRatePer = TSP.TaxRatePer;
                    lShopProduct.CreateDate = TSP.CreateDate;
                    lShopProduct.CreateBy = TSP.CreateBy;
                    lShopProduct.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    lShopProduct.ModifyDate = DateTime.UtcNow;
                    lShopProduct.NetworkIP = TSP.NetworkIP;
                    lShopProduct.DeviceID = TSP.DeviceID;
                    lShopProduct.DeviceType = TSP.DeviceType;
                    if (ModelState.IsValid)
                    {
                        db.ShopProducts.Add(lShopProduct);
                        db.SaveChanges();
                    }
                }

                //List<ProductVarient> lProductVarient = db.ProductVarients.Where(x => x.ProductID == lProduct.ID).ToList();
                //if (lProductVarient.Count == 0)
                //{
                List<ProductVarient> lProductVarient = new List<ProductVarient>();
                List<TempProductVarient> TPV = db.TempProductVarients.Where(x => x.ProductID == productId).ToList();
                ArrayList colorName = new ArrayList();
                foreach (TempProductVarient TpvRow in TPV)
                {
                    ProductVarient PV = db.ProductVarients.Where(x => x.ProductID == lProduct.ID && x.ColorID == TpvRow.ColorID && x.DimensionID == TpvRow.DimensionID && x.MaterialID == TpvRow.MaterialID && x.SizeID == TpvRow.SizeID).FirstOrDefault();
                    if (PV == null)
                    {
                        PV = new ProductVarient();
                        PV.ProductID = lProduct.ID;
                        PV.ColorID = TpvRow.ColorID;
                        PV.DimensionID = TpvRow.DimensionID;
                        PV.SizeID = TpvRow.SizeID;
                        PV.MaterialID = TpvRow.MaterialID;
                        PV.IsActive = true;
                        PV.CreateDate = TpvRow.CreateDate;
                        PV.CreateBy = TpvRow.CreateBy;
                        PV.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        PV.ModifyDate = DateTime.UtcNow;
                        PV.NetworkIP = TpvRow.NetworkIP;
                        PV.DeviceID = TpvRow.DeviceID;
                        PV.DeviceType = TpvRow.DeviceType;

                        string color = db.Colors.Where(x => x.ID == PV.ColorID).Select(x => x.Name).FirstOrDefault();
                        if (!string.IsNullOrEmpty(color))
                        {
                            if (color.ToUpper().Equals("N/A"))
                            {
                                if (!colorName.Contains("Default".ToLower()))
                                {
                                    colorName.Add("Default".ToLower());
                                }
                            }
                            else
                            {
                                if (!colorName.Contains(color.ToLower()))
                                {
                                    colorName.Add(color.ToLower());
                                }
                            }
                        }
                        ViewBag.Colors = colorName;

                        if (ModelState.IsValid)
                        {
                            db.ProductVarients.Add(PV);
                            db.SaveChanges();
                            lProductVarient.Add(PV);
                        }
                    }

                    TempShopStock TSS = db.TempShopStocks.Where(x => x.ProductVarientID == TpvRow.ID).FirstOrDefault();
                    ShopStock SS = new ShopStock();
                    SS.ShopProductID = lShopProduct.ID;
                    SS.ProductVarientID = PV.ID;
                    SS.Qty = TSS.Qty;
                    SS.ReorderLevel = TSS.ReorderLevel;
                    SS.StockStatus = TSS.StockStatus;
                    SS.PackSize = TSS.PackSize;
                    SS.PackUnitID = 1;// TSS.PackUnitID;
                    SS.MRP = TSS.MRP;
                    SS.WholeSaleRate = TSS.WholeSaleRate;
                    SS.RetailerRate = TSS.RetailerRate;
                    SS.IsInclusiveOfTax = TSS.IsInclusiveOfTax;
                    SS.IsActive = true;
                    SS.CreateDate = TSS.CreateDate;
                    SS.CreateBy = TSS.CreateBy;
                    SS.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    SS.ModifyDate = DateTime.UtcNow;
                    SS.NetworkIP = SS.NetworkIP;
                    SS.DeviceID = SS.DeviceID;
                    SS.DeviceType = SS.DeviceType;

                    if (ModelState.IsValid)
                    {
                        db.ShopStocks.Add(SS);
                        db.SaveChanges();
                    }
                    //Added by manoj for insert in productTax
                    var TaxationID = db.TempProductTaxes.Where(x => x.ShopStockID == TSS.ID).Select(x => x.TaxID).ToList();
                    foreach (var taxID in TaxationID)
                    {
                        ProductTax ProductTax = new ProductTax();
                        ProductTax.ShopStockID = SS.ID;
                        ProductTax.TaxID = Convert.ToInt32(taxID);
                        ProductTax.IsInclusive = true;
                        ProductTax.IsActive = true;
                        ProductTax.CreateDate = DateTime.Now;
                        ProductTax.CreateBy = GetPersonalDetailID();
                        ProductTax.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        ProductTax.DeviceID = "x";
                        ProductTax.DeviceType = "x";
                        db.ProductTaxes.Add(ProductTax);
                        db.SaveChanges();


                    }
                    List<TempStockComponent> lTSC = db.TempStockComponents.Where(x => x.ShopStockID == TSS.ID).ToList();
                    foreach (TempStockComponent TscRow in lTSC)
                    {
                        StockComponent SC = new StockComponent();
                        SC.ShopStockID = SS.ID;
                        SC.ComponentID = TscRow.ComponentID;
                        SC.ComponentWeight = TscRow.ComponentWeight;
                        SC.ComponentUnitID = TscRow.ComponentUnitID;
                        SC.CreateDate = TscRow.CreateDate;
                        SC.CreateBy = TscRow.CreateBy;
                        SC.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                        SC.ModifyDate = DateTime.UtcNow;
                        SC.NetworkIP = TscRow.NetworkIP;
                        SC.DeviceID = TscRow.DeviceID;
                        SC.DeviceType = TscRow.DeviceType;

                        if (ModelState.IsValid)
                        {
                            db.StockComponents.Add(SC);
                            db.SaveChanges();
                        }
                    }
                }
                List<TempProductSpecification> lTPS = db.TempProductSpecifications.Where(x => x.ProductID == productId).ToList();
                foreach (TempProductSpecification TpsRow in lTPS)
                {
                    ProductSpecification PS = new ProductSpecification();
                    PS.ProductID = lProduct.ID;
                    PS.SpecificationID = TpsRow.SpecificationID;
                    PS.Value = TpsRow.Value;
                    PS.IsActive = TpsRow.IsActive;
                    PS.CreateDate = TpsRow.CreateDate;
                    PS.CreateBy = TpsRow.CreateBy;
                    PS.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                    PS.ModifyDate = DateTime.UtcNow;
                    PS.NetworkIP = TpsRow.NetworkIP;
                    PS.DeviceID = TpsRow.DeviceID;
                    PS.DeviceType = TpsRow.DeviceType;

                    if (ModelState.IsValid)
                    {
                        db.ProductSpecifications.Add(PS);
                        db.SaveChanges();
                    }
                }
            }
            //}
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][InsertProduct]", "Can't Insert Values!" + Environment.NewLine + ex.Message);
            }
        }

        private void InsertProprietory(long productID, long shopID)
        {
            try
            {
                ProprietoryProduct PP = new ProprietoryProduct();
                PP.ProductID = productID;
                PP.ShopID = shopID;
                PP.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));
                PP.CreateDate = DateTime.UtcNow;
                PP.NetworkIP = CommonFunctions.GetClientIP();
                //if (ModelState.IsValid)
                {
                    db.ProprietoryProducts.Add(PP);
                    db.SaveChanges();
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[InsertRole]", "Can't assign Role..!" + Environment.NewLine + myEx.Message);
            }
        }

        private bool CheckValidationForApproval(long tempProductID, long shopID, ref string msg, ref int status)
        {
            //Comment :
            //1. check product is present in db or not (in tempproduct table)
            //2. check entry of product aginst temp shopproduct
            //3. Check at list one varient is present in db or not (in temp product variant table)
            //4. find color names in of all variants of product in temp table
            //5. Product images and thumb image is present for each variant on ftp server
            //6. Check Stock of the product in tempShopStock

            bool flag = false;
            //try
            //{
            TempProduct lTP = db.TempProducts.Find(tempProductID);
            if (lTP != null)
            {
                long shopProductId = (from s in db.Shops
                                      join tsp in db.TempShopProducts on s.ID equals tsp.ShopID
                                      join bd in db.BusinessDetails on s.BusinessDetailID equals bd.ID
                                      join ul in db.UserLogins on bd.UserLoginID equals ul.ID
                                      where ul.IsLocked == false && tsp.ShopID == shopID && tsp.ProductID == tempProductID
                                      select tsp.ID).FirstOrDefault();

                if (shopProductId > 0)
                {
                    List<TempProductVarient> productVarient = db.TempProductVarients.Where(x => x.ProductID == tempProductID).ToList();
                    if (productVarient.Count() > 0)
                    {
                        ArrayList colorName = new ArrayList();
                        foreach (TempProductVarient TPV in productVarient)
                        {
                            string color = db.Colors.Where(x => x.ID == TPV.ColorID).Select(x => x.Name).FirstOrDefault();
                            if (!string.IsNullOrEmpty(color))
                            {
                                if (color.ToLower().Equals("n/a"))
                                {
                                    if (!colorName.Contains("Default".ToLower()))
                                    {
                                        colorName.Add("Default".ToLower());
                                    }
                                }
                                else
                                {
                                    if (!colorName.Contains(color.ToLower()))
                                    {
                                        colorName.Add(color.ToLower());
                                    }
                                }
                            }
                        }
                        bool imgflag = false;
                        if (colorName.Count > 0)
                        {
                            foreach (string str in colorName)
                            {
                                //string[] imgfiles = ImageDisplay.LoadProductImages(tempProductID, str, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                                string imgThumb = ImageDisplay.SetProductThumbImage(tempProductID, str, string.Empty, ProductUpload.IMAGE_TYPE.NonApproved);
                                //if (imgfiles.Count() > 0 && (!imgfiles[0].ToLower().Contains("/no_thumbnail.png")) && !string.IsNullOrEmpty(imgThumb))
                                if (!string.IsNullOrEmpty(imgThumb))
                                {
                                    imgflag = true;
                                }
                                else
                                {
                                    imgflag = false;
                                    break;
                                }
                            }
                        }
                        if (imgflag)
                        {

                            var tempShopStock = db.TempShopStocks.Where(x => x.ShopProductID == shopProductId).ToList();
                            if (tempShopStock.Count() > 0)
                            {
                                flag = true;
                                status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.PENDING_BY_ADMIN);
                                return flag;
                            }
                            else
                            {
                                flag = false;
                                msg = "Stock not exist for product can't approve";
                                status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.STOCK_NOT_EXIST);
                            }
                        }
                        else
                        {
                            flag = false;
                            msg = "Images or thumbnail not exist for one of the variant of product, can't approve";
                            status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.IMAGE_NOT_EXIST);
                        }
                    }
                    else
                    {
                        flag = false;
                        msg = "Varient not exist for product can't approve";
                        status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.VARIENT_NOT_EXIST);
                    }
                }
                else
                {
                    flag = false;
                    msg = "Shop not approved for product can't approve";
                    status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.SHOP_NOT_APPROVED);
                }
                //}
                //else
                //{
                //    flag = false;
                //    msg = "Images not exist for product can't approve";
                //    status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.IMAGE_NOT_EXIST);
                //}
            }
            else
            {
                flag = false;
                msg = "Product not exists can't approve";
                status = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.PRODUCT_NOT_EXIST);
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new BusinessLogicLayer.MyException("[ProductApproval][CheckValidationForApproval]", "Can't Validate Values!" + Environment.NewLine + ex.Message);
            //}
            return flag;
        }

        private bool CheckProductLimitOfMerchant(long shopID)
        {
            try
            {
                int productLimit = (from s in db.Shops
                                    join o in db.OwnerPlans on s.ID equals o.OwnerID
                                    join p in db.Plans on o.PlanID equals p.ID
                                    where p.PlanCode.StartsWith("GBMR") && s.ID == shopID && o.IsActive == true
                                    select p.NoOfEntitiesAllowed).FirstOrDefault();
                int productUploadedinShop = db.ShopProducts.Where(x => x.ShopID == shopID).Select(x => x.ID).Count();

                if (productLimit > productUploadedinShop)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][CheckProductLimitOfMerchant]", "Can't Validate Values!" + Environment.NewLine + ex.Message);
            }
            return false;
        }

        public JsonResult ShopList(int franchiseID)
        {
            try
            {
                List<ShopViewModel> lst = new List<ShopViewModel>();
                lst = GetMerchantList(franchiseID);

                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][ShopList]", "Can't Get Shop List!" + Environment.NewLine + ex.Message);
            }

        }

        public JsonResult ShopList1(int franchiseID)
        {
            try
            {
                List<ShopViewModel> lst = new List<ShopViewModel>();
                lst = GetMerchantList1(franchiseID);

                return Json(lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][ShopList]", "Can't Get Shop List!" + Environment.NewLine + ex.Message);
            }

        }

        //public class MerchantDetail
        //{
        //    public Int64 ID { get; set; }
        //    public string Name { get; set; }
        //    public bool IsActive { get; set; }
        //}

        public List<ShopViewModel> GetMerchantList(int franchiseID)
        {
            List<ShopViewModel> mrctLst = new List<ShopViewModel>();
            try
            {
                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                if (franchiseID < 1)
                {
                    mrctLst = (from s in db.Shops
                               where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR"
                               select new ShopViewModel
                               {
                                   ID = s.ID,
                                   Name = s.Name.Trim(),
                                   FranchiseID = s.FranchiseID,
                                   NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                               }).OrderBy(x => x.Name).ToList();
                }
                else
                {
                    mrctLst = (from s in db.Shops
                               where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR" && s.FranchiseID == franchiseID
                               select new ShopViewModel
                               {
                                   ID = s.ID,
                                   Name = s.Name.Trim(),
                                   FranchiseID = s.FranchiseID,
                                   NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                               }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][GetMerchantList]", "Can't Get Merchant List!" + Environment.NewLine + ex.Message);
            }
            return mrctLst;
        }
        public List<ShopViewModel> GetMerchantList1(int franchiseID)
        {
            List<ShopViewModel> mrctLst = new List<ShopViewModel>();
            try
            {
                int approvalStatus = Convert.ToInt16(ProductUpload.DISAPPROVAL_REMARK.APPROVED);
                if (franchiseID < 1)
                {
                    mrctLst = (from s in db.Shops
                               where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR"
                               select new ShopViewModel
                               {
                                   ID = s.ID,
                                   Name = s.Name.Trim(),
                                   FranchiseID = s.FranchiseID,
                                   // NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                                   //ApproveProductCount = db.ShopProducts.Where(x => x.ShopID == s.ID && x.Product.IsActive == true).Count()
                                   ApproveProductCount = db.ShopProducts.Where(x => x.ShopID == s.ID && x.Product.IsActive == true).Count()
                               }).OrderBy(x => x.Name).ToList();
                }
                else
                {
                    mrctLst = (from s in db.Shops
                               where s.BusinessDetail.UserLogin.IsLocked == false && s.IsActive == true && s.BusinessDetail.BusinessType.Prefix == "GBMR" && s.FranchiseID == franchiseID
                               select new ShopViewModel
                               {
                                   ID = s.ID,
                                   Name = s.Name.Trim(),
                                   FranchiseID = s.FranchiseID,
                                   // NonApproveProductCount = db.TempShopProducts.Where(x => x.ShopID == s.ID && x.TempProduct.IsActive == true && x.TempProduct.ApprovalStatus != approvalStatus).Count()
                                   ApproveProductCount = db.ShopProducts.Where(x => x.ShopID == s.ID && x.Product.IsActive == true).Count()
                               }).OrderBy(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ProductApproval][GetMerchantList]", "Can't Get Merchant List!" + Environment.NewLine + ex.Message);
            }
            return mrctLst;
        }

        public void WriteToLogTable(Product obj, ModelLayer.Models.Enum.COMMAND mode)
        {
            try
            {
                //Log Table Insertion
                //LogTable logTable = new LogTable();
                //logTable.TableName = "Product";//table Name(Model Name)
                //logTable.RecordXML = ModelLayer.Models.ObjectToXml.GetXMLFromObject(obj);
                //logTable.TableRowID = obj.ID;
                //logTable.Command = mode.ToString();
                //long? rowOwnerID = (obj.ModifyBy >= 0 ? obj.ModifyBy : obj.CreateBy);
                //logTable.RowOwnerID = (long)rowOwnerID;
                //logTable.CreateDate = DateTime.UtcNow;
                //logTable.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["ID"]));//Session ID
                //db.LogTables.Add(logTable);
                /**************************************/
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                     + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                     + "[ProductApprovalController][WriteToLogTable]" + myEx.EXCEPTION_PATH,
                     BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductApprovalController][WriteToLogTable]",
                    BusinessLogicLayer.ErrorLog.Module.Administrator, System.Web.HttpContext.Current.Server);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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

        public JsonResult AutoCompleteProName(string term, string ddlMerchant)
        {

            long ShopID = Convert.ToInt64(ddlMerchant);
            var result = (from c in db.Categories
                          join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                          join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                          join P in db.Plans on op.PlanID equals P.ID
                          join r in db.Products on c.ID equals r.CategoryID
                          //join pp in ProductId on r.ID equals pp.ProductID
                          where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                          //&& r.ID != pp.ProductID
                          && r.Name.ToLower().Contains(term.ToLower())
                          && !
                          (from pp in db.ProprietoryProducts
                           select new
                           {
                               pp.ProductID
                           }).Contains(new { ProductID = r.ID })
                          select new { Name = r.Name, r.ID, ShopID }).Distinct().ToList();



            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AutoCompleteProName1(string term, string ddlMerchant)
        {
            //long ShopID = GetShopID();
            long ShopID = Convert.ToInt64(ddlMerchant);
            //var result = (from c in db.Categories
            //              join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
            //              join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
            //              join P in db.Plans on op.PlanID equals P.ID
            //              join r in db.Products on c.ID equals r.CategoryID
            //              where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
            //              && r.Name.ToLower() == term.ToLower()
            //              select new { r.Name, r.ID }).Distinct();
            var result = (from c in db.Categories
                          join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                          join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                          join P in db.Plans on op.PlanID equals P.ID
                          join r in db.Products on c.ID equals r.CategoryID
                          //join pp in ProductId on r.ID equals pp.ProductID
                          where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                          //&& r.ID != pp.ProductID
                          && r.Name.ToLower() == term.ToLower()
                          && !
                          (from pp in db.ProprietoryProducts
                           select new
                           {
                               pp.ProductID
                           }).Contains(new { ProductID = r.ID })
                          select new { Name = r.Name, r.ID, ShopID }).Distinct().ToList();


            return Json(result, JsonRequestBehavior.AllowGet);

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

        private void DeleteVariantTaxProduct(long ShopStockID)
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
