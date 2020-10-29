using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Merchant.Models;
using ModelLayer.Models;


namespace Merchant.Controllers
{
    public class ComponentOfferZoneController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public static long ShopID;
        //
        // GET: /ComponentOfferZone/
        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                ShopID = GetShopID();
                var ProductOfferViewModel = (from CO in db.ComponentOffers
                                             join SCOD in db.StockComponentOfferDurations on CO.ID equals SCOD.ComponentOfferID
                                             join C in db.Components on CO.ComponentID equals C.ID
                                             where CO.ShopID == ShopID && CO.IsActive==true
                                             select new ProductOfferViewModel
                                             {
                                                 OfferID = CO.ID,
                                                 OfferName = CO.ShortName,
                                                 DiscountInPercent = CO.OfferInRs,
                                                 DiscountInRs = CO.OfferInPercent,
                                                 StartDateTime = SCOD.StartDateTime,
                                                 EndDateTime = SCOD.EndDateTime,
                                                 ComponentId = C.ID,
                                                 ComponentName = C.Name

                                             }).ToList();

                return View(ProductOfferViewModel.ToList());
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading index of OfferZone!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading index of OfferZone!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }
        private long GetShopID()
        {

            long UserLoginID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
            long BusinessDetailID = 0;
            long ShopID = 0;
            try
            {
                if (UserLoginID > 0)
                {
                    BusinessDetailID = Convert.ToInt32(db.BusinessDetails.Where(x => x.UserLoginID == UserLoginID).Select(x => x.ID).First());
                    ShopID = Convert.ToInt32(db.Shops.Where(x => x.BusinessDetailID == BusinessDetailID).Select(x => x.ID).First());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopComponentPriceController][GetShopID]", "Can't find ShopID !" + Environment.NewLine + ex.Message);
            }
            return ShopID;
        }

        [SessionExpire]
        public ActionResult LoadProducts(int? ComponentID, int ComponentOfferID, string OfferName, int PageIndex, int PageSize,int CategoryID)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            decimal TotalSaleRate = 0;
            ViewBag.CompID = ComponentID;
            ViewBag.ComponentOfferID = ComponentOfferID;
            ViewBag.OfferName = OfferName;
            decimal Total = 0;
            try
            {
                long ShopID = GetShopID();

                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category, "ID", "Name");
              
                                      
                                     
                                     
                var ProductDetails = (from TSC in db.StockComponents
                                      join TSS in db.ShopStocks on TSC.ShopStockID equals TSS.ID
                                      join TSP in db.ShopProducts on TSS.ShopProductID equals TSP.ID
                                      join TP in db.Products on TSP.ProductID equals TP.ID
                                      join C in db.Categories on TP.CategoryID equals C.ID
                                      join pcc in db.PlanCategoryCharges on C.ID equals pcc.CategoryID
                                      join P in db.Plans on pcc.PlanID equals P.ID
                                      join op in db.OwnerPlans on P.ID equals op.PlanID
                                      where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID && TSC.ComponentID == ComponentID && TSP.ShopID == ShopID && 
                                      TP.IsActive == true && TP.CategoryID == (CategoryID == 0 ? TP.CategoryID : CategoryID)

                                      select new
                                      {
                                          ProductID = TP.ID,
                                          ProductName = TP.Name,
                                          ShopStockID = TSS.ID

                                      }).ToList();

                var ComponentDetails = (from tsc in db.StockComponents

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


                var product1 = ProductDetails.Select(c => c.ProductID).Distinct().OrderBy(x => x).AsQueryable();
                var products = product1.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = products.Count;
                TotalCount = product1.Count();
                ViewBag.TotalCount = TotalCount;
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;
                List<ProductVarientViewModel> listProductComponent = new List<ProductVarientViewModel>();
                foreach (var pID in products)
                {
                    var ProductDetails1 = (from TSC in db.StockComponents
                                           join TSS in db.ShopStocks on TSC.ShopStockID equals TSS.ID
                                           join TSP in db.ShopProducts on TSS.ShopProductID equals TSP.ID
                                           join TP in db.Products on TSP.ProductID equals TP.ID
                                           join C in db.Components on TSC.ComponentID equals C.ID
                                           join TPV in db.ProductVarients on TSS.ProductVarientID equals TPV.ID
                                           join Co in db.Colors on TPV.ColorID equals Co.ID
                                           join S in db.Sizes on TPV.SizeID equals S.ID
                                           join D in db.Dimensions on TPV.DimensionID equals D.ID
                                           join M in db.Materials on TPV.MaterialID equals M.ID
                                           //join scpl in db.StockComponentPriceLogs on TSS.ID equals scpl.ShopStockID
                                           where TSC.ComponentID == ComponentID && TSP.ShopID == ShopID && TP.ID==pID &&
                                           TP.IsActive == true && TP.CategoryID == (CategoryID == 0 ? TP.CategoryID : CategoryID)

                                           select new
                                           {
                                               ProductID = TP.ID,
                                               ProductName = TP.Name,
                                               ProductVarientID = TPV.ID,
                                               ColorID = Co.ID,
                                               ColorName = Co.Name,
                                               SizeID = S.ID,
                                               SizeName = S.Name,
                                               DimensionID = D.ID,
                                               DimensionName = D.Name,
                                               MaterialID = M.ID,
                                               MaterialName = M.Name,
                                               Total = 0,
                                               ShopStockID = TSS.ID

                                           }).ToList();

                    foreach (var product in ProductDetails1)
                    {
                        ProductVarientViewModel ObjProductComponentViewModel = new ProductVarientViewModel();
                        bool IsExist = db.StockComponentPriceLogs.Any(u => u.ShopStockID == product.ShopStockID);
                        if (IsExist)
                        {
                            //var ID = db.StockComponentPriceLogs.Select(x => x.ID).FirstOrDefault();
                            Total = db.StockComponentPriceLogs.OrderByDescending(x => x.CreatedDate).Where(x => x.ShopStockID == product.ShopStockID).Select(x => x.OldSaleRate).First();
                        }
                        else
                        {
                            Total = 0;
                        }
                        ObjProductComponentViewModel.ProductID = product.ProductID;
                        ObjProductComponentViewModel.ProductVarientID = product.ProductVarientID;
                        ObjProductComponentViewModel.ProductName = product.ProductName;
                        ObjProductComponentViewModel.ColorName = product.ColorName;
                        ObjProductComponentViewModel.SizeName = product.SizeName;
                        ObjProductComponentViewModel.DimensionName = product.DimensionName;
                        ObjProductComponentViewModel.MaterialName = product.MaterialName;
                        //Total = Math.Round(Total, 2);
                        ObjProductComponentViewModel.Total = Total;
                        ObjProductComponentViewModel.ShopStockID = product.ShopStockID;

                        long ProductID = ObjProductComponentViewModel.ProductID;
                        if (ObjProductComponentViewModel.ColorName == "N/A")
                        {
                            // ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, string.Empty, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                            ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                        }
                        else
                        {
                            //ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, ObjProductComponentViewModel.ColorName, string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.NonApproved);
                            ObjProductComponentViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
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
                return View("_LoadProducts", listProductComponent);
                //return View("_ProductDetails", listProductComponent.ToList().ToPagedList(page ?? 1, 10));
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading index of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Index]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading index of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Index]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
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

        [HttpPost]
        [SessionExpire]
        public JsonResult BindSavedOffersToProduct(long ComponentOfferID)
        {
            List<long> ProductOffer = new List<long>();
            try
            {
                ProductOffer = db.StockComponentOffers.Where(x => x.ComponentOfferID == ComponentOfferID).Select(x => x.ShopStockID).ToList();
               
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:BindSavedOffersToProduct]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:BindSavedOffersToProduct]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return Json(ProductOffer.ToList());
        }
         [HttpPost]
        [SessionExpire]
        public JsonResult SaveOffer(long ShopStockID, int ComponentOfferID)
        {
            string Message = "";
            try
            {
                StockComponentOffer stockcomponentoffer = new StockComponentOffer();
                stockcomponentoffer.ComponentOfferID = ComponentOfferID;
                stockcomponentoffer.ShopStockID = ShopStockID;
                //offerzone.FreeStockID = FreeStockID;
                stockcomponentoffer.IsActive = true;
                stockcomponentoffer.CreateDate = DateTime.UtcNow;
                stockcomponentoffer.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                stockcomponentoffer.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                stockcomponentoffer.DeviceID = "x";
                stockcomponentoffer.DeviceType = "x";
                db.StockComponentOffers.Add(stockcomponentoffer);
                db.SaveChanges();
                Message = "Applied Succesfully";
                return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Saving Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ComponentOfferZone][POST:SaveOffer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ComponentOfferZone][POST:SaveOffer]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }
         [HttpPost]
         [SessionExpire]
        public JsonResult DeleteOffer(long ShopStockID, int ComponentOfferID)
        {
            string Message = "";
            try
            {
                long ID = db.StockComponentOffers.Where(x => x.ComponentOfferID == ComponentOfferID && x.ShopStockID == ShopStockID).Select(x => x.ID).FirstOrDefault();
                StockComponentOffer stockcomponentoffer = db.StockComponentOffers.Find(ID);
                if (ID > 0)
                {
                    db.StockComponentOffers.Remove(stockcomponentoffer);
                    db.SaveChanges();
                    Message = "Removed Succesfully";
                }
                else
                {
                    Message = "Error";
                }

                return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Removing offer on Product!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ComponentOfferZone][POST:DeleteOffer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Removing offer on Product!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ComponentOfferZone][POST:DeleteOffer]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }
        //
        // GET: /ComponentOfferZone/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ComponentOfferZone/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ComponentOfferZone/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ComponentOfferZone/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ComponentOfferZone/Edit/5
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
        // GET: /ComponentOfferZone/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ComponentOfferZone/Delete/5
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
    }
}
