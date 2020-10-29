using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogicLayer;
using Merchant.Models;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;


namespace Merchant.Controllers
{
    public class OfferZoneController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public static long ShopID;
        //
        // GET: /OfferZone/

        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                ShopID = GetShopID();
                var ProductOfferViewModel = (from O in db.Offers
                                             join OD in db.OfferDurations on O.ID equals OD.OfferID
                                             where O.OwnerID == ShopID && O.IsActive == true
                                             select new ProductOfferViewModel
                                             {
                                                 OfferID = O.ID,
                                                 OfferName = O.ShortName,
                                                 DiscountInPercent = O.DiscountInPercent,
                                                 DiscountInRs = O.DiscountInRs,
                                                 StartDateTime = OD.StartDateTime,
                                                 EndDateTime = OD.EndDateTime,
                                                 MinPurchaseQty = O.MinPurchaseQty,
                                                 FreeOty = O.FreeOty
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
        [HttpPost]
        [SessionExpire]
        public ActionResult LoadProducts(long OfferID, string OfferName, int PageIndex, int PageSize, decimal DiscountInRs, decimal DiscountInPercent, int CategoryID)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            ViewBag.OfferID = OfferID;
            ViewBag.OfferName = OfferName;
            ViewBag.DiscountInRs = DiscountInRs;
            ViewBag.DiscountInPercent = DiscountInPercent;

            List<ProductOfferZoneViewModel> lstProductOfferZoneViewModel = new List<ProductOfferZoneViewModel>();
            try
            {

                //var Category = (from c in db.Categories
                //                join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                //                join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                //                join P in db.Plans on op.PlanID equals P.ID
                //                where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                //                select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category, "ID", "Name");

                var productDetails = (from op in db.OwnerPlans
                                      join p in db.Plans on op.PlanID equals p.ID
                                      join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                      join c in db.Categories on pcc.CategoryID equals c.ID
                                      join r in db.Products on c.ID equals r.CategoryID
                                      join SP in db.ShopProducts on r.ID equals SP.ProductID
                                      //join pp in ProductId on r.ID equals pp.ProductID
                                      where p.PlanCode.Contains("GBMR") && op.OwnerID == ShopID && SP.ShopID == ShopID
                                      && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                      select new
                                                 {
                                                     ProductID = r.ID,
                                                     ProductName = r.Name

                                                 }).ToList();

                TotalCount = productDetails.Count;
                ViewBag.TotalCount = TotalCount;
                var product = productDetails.Select(c => c.ProductID).Distinct().OrderBy(x => x).AsQueryable();
                var prodList = product.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = prodList.Count;
                TotalCount = product.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;

                foreach (var pID in prodList)
                {
                    var ProductOfferZoneViewModel = (from r in db.Products
                                                     join SP in db.ShopProducts on r.ID equals SP.ProductID
                                                     join SS in db.ShopStocks on SP.ID equals SS.ShopProductID
                                                     join PV in db.ProductVarients on SS.ProductVarientID equals PV.ID
                                                     join C in db.Colors on PV.ColorID equals C.ID
                                                     join S in db.Sizes on PV.SizeID equals S.ID
                                                     join M in db.Materials on PV.MaterialID equals M.ID
                                                     join D in db.Dimensions on PV.DimensionID equals D.ID
                                                     join OZP in db.OfferZoneProducts on SS.ID equals OZP.ShopStockID into OZP_join
                                                     from OZP in OZP_join.DefaultIfEmpty()
                                                     where r.ID == pID && SP.ShopID == ShopID && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                                     select new ProductOfferZoneViewModel
                                                     {
                                                         ProductID = r.ID,
                                                         ProductName = r.Name,
                                                         ShopStockID = SS.ID,
                                                         //ShopImage = ImageDisplay.LoadProductThumbnails(P.ID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved),
                                                         ProductVarientID = PV.ID,
                                                         ColorName = C.Name,
                                                         SizeName = S.Name,
                                                         DimensionName = D.Name,
                                                         MaterialName = M.Name,
                                                         PackSize = SS.PackSize,
                                                         Qty = SS.Qty,
                                                         MRP = SS.MRP,
                                                         RetailerRate = SS.RetailerRate


                                                     }).ToList();
                    foreach (var i in ProductOfferZoneViewModel)
                    {
                        ProductOfferZoneViewModel objProductOfferZoneViewModel = new ProductOfferZoneViewModel();
                        objProductOfferZoneViewModel.ProductID = i.ProductID;
                        objProductOfferZoneViewModel.ProductName = i.ProductName;
                        objProductOfferZoneViewModel.ShopStockID = i.ShopStockID;
                        objProductOfferZoneViewModel.ProductVarientID = i.ProductVarientID;
                        objProductOfferZoneViewModel.ColorName = i.ColorName;
                        objProductOfferZoneViewModel.SizeName = i.SizeName;
                        objProductOfferZoneViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(i.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                        objProductOfferZoneViewModel.DimensionName = i.DimensionName;
                        objProductOfferZoneViewModel.MaterialName = i.MaterialName;
                        objProductOfferZoneViewModel.PackSize = i.PackSize;
                        objProductOfferZoneViewModel.Qty = i.Qty;
                        objProductOfferZoneViewModel.MRP = i.MRP;
                        objProductOfferZoneViewModel.RetailerRate = i.RetailerRate;
                        objProductOfferZoneViewModel.IsActive = i.IsActive;
                        lstProductOfferZoneViewModel.Add(objProductOfferZoneViewModel);
                    }
                }
                return View("_LoadProducts", lstProductOfferZoneViewModel.ToList());

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }


        public ActionResult LoadProductsProdName(long OfferID, string OfferName, int PageIndex, int PageSize, decimal DiscountInRs, decimal DiscountInPercent, int CategoryID, string ProdName)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            ViewBag.OfferID = OfferID;
            ViewBag.OfferName = OfferName;
            ViewBag.DiscountInRs = DiscountInRs;
            ViewBag.DiscountInPercent = DiscountInPercent;

            List<ProductOfferZoneViewModel> lstProductOfferZoneViewModel = new List<ProductOfferZoneViewModel>();
            try
            {

                //var Category = (from c in db.Categories
                //                join opc in db.OwnerPlanCategoryCharges on c.ID equals opc.CategoryID
                //                join op in db.OwnerPlans on opc.OwnerPlanID equals op.ID
                //                join P in db.Plans on op.PlanID equals P.ID
                //                where P.PlanCode.Contains("GBMR") && op.OwnerID == ShopID
                //                select new ForLoopClass { Name = c.Name, ID = c.ID }).ToList();

                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.CategoryID = new SelectList(Category, "ID", "Name");

                var productDetails = (from op in db.OwnerPlans
                                      join p in db.Plans on op.PlanID equals p.ID
                                      join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                      join c in db.Categories on pcc.CategoryID equals c.ID
                                      join r in db.Products on c.ID equals r.CategoryID
                                      join SP in db.ShopProducts on r.ID equals SP.ProductID
                                      //join pp in ProductId on r.ID equals pp.ProductID
                                      where p.PlanCode.Contains("GBMR") && op.OwnerID == ShopID && SP.ShopID == ShopID
                                      && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                      && r.Name.Contains(ProdName)
                                      select new
                                      {
                                          ProductID = r.ID,
                                          ProductName = r.Name

                                      }).ToList();

                TotalCount = productDetails.Count;
                ViewBag.TotalCount = TotalCount;
                var product = productDetails.Select(c => c.ProductID).Distinct().OrderBy(x => x).AsQueryable();
                var prodList = product.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = prodList.Count;
                TotalCount = product.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;

                foreach (var pID in prodList)
                {
                    var ProductOfferZoneViewModel = (from r in db.Products
                                                     join SP in db.ShopProducts on r.ID equals SP.ProductID
                                                     join SS in db.ShopStocks on SP.ID equals SS.ShopProductID
                                                     join PV in db.ProductVarients on SS.ProductVarientID equals PV.ID
                                                     join C in db.Colors on PV.ColorID equals C.ID
                                                     join S in db.Sizes on PV.SizeID equals S.ID
                                                     join M in db.Materials on PV.MaterialID equals M.ID
                                                     join D in db.Dimensions on PV.DimensionID equals D.ID
                                                     join OZP in db.OfferZoneProducts on SS.ID equals OZP.ShopStockID into OZP_join
                                                     from OZP in OZP_join.DefaultIfEmpty()
                                                     where r.ID == pID && SP.ShopID == ShopID && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                                     select new ProductOfferZoneViewModel
                                                     {
                                                         ProductID = r.ID,
                                                         ProductName = r.Name,
                                                         ShopStockID = SS.ID,
                                                         //ShopImage = ImageDisplay.LoadProductThumbnails(P.ID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved),
                                                         ProductVarientID = PV.ID,
                                                         ColorName = C.Name,
                                                         SizeName = S.Name,
                                                         DimensionName = D.Name,
                                                         MaterialName = M.Name,
                                                         PackSize = SS.PackSize,
                                                         Qty = SS.Qty,
                                                         MRP = SS.MRP,
                                                         RetailerRate = SS.RetailerRate


                                                     }).ToList();
                    foreach (var i in ProductOfferZoneViewModel)
                    {
                        ProductOfferZoneViewModel objProductOfferZoneViewModel = new ProductOfferZoneViewModel();
                        objProductOfferZoneViewModel.ProductID = i.ProductID;
                        objProductOfferZoneViewModel.ProductName = i.ProductName;
                        objProductOfferZoneViewModel.ShopStockID = i.ShopStockID;
                        objProductOfferZoneViewModel.ProductVarientID = i.ProductVarientID;
                        objProductOfferZoneViewModel.ColorName = i.ColorName;
                        objProductOfferZoneViewModel.SizeName = i.SizeName;
                        objProductOfferZoneViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(i.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                        objProductOfferZoneViewModel.DimensionName = i.DimensionName;
                        objProductOfferZoneViewModel.MaterialName = i.MaterialName;
                        objProductOfferZoneViewModel.PackSize = i.PackSize;
                        objProductOfferZoneViewModel.Qty = i.Qty;
                        objProductOfferZoneViewModel.MRP = i.MRP;
                        objProductOfferZoneViewModel.RetailerRate = i.RetailerRate;
                        objProductOfferZoneViewModel.IsActive = i.IsActive;
                        lstProductOfferZoneViewModel.Add(objProductOfferZoneViewModel);
                    }
                }
                return View("_LoadProducts", lstProductOfferZoneViewModel.ToList());

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();

        }
        [HttpPost]
        [SessionExpire]
        public ActionResult LoadFreeProducts(long OfferID, long ShopStockID, int PageIndex, int PageSize, string OfferName, int CheckedCount, int CategoryID, string FreeProdName)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            ViewBag.OfferID = OfferID;
            ViewBag.OfferName = OfferName;
            ViewBag.ShopStockID = ShopStockID;
            ViewBag.CheckedCount = CheckedCount;
            List<ProductOfferZoneViewModel> lstProductOfferZoneViewModel = new List<ProductOfferZoneViewModel>();
            try
            {
                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.FreeCategoryID = new SelectList(Category, "ID", "Name");
                var productDetails = (from op in db.OwnerPlans
                                      join p in db.Plans on op.PlanID equals p.ID
                                      join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                      join c in db.Categories on pcc.CategoryID equals c.ID
                                      join r in db.Products on c.ID equals r.CategoryID
                                      join SP in db.ShopProducts on r.ID equals SP.ProductID
                                      //join pp in ProductId on r.ID equals pp.ProductID
                                      where p.PlanCode.Contains("GBMR") && op.OwnerID == ShopID && SP.ShopID == ShopID
                                      && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                      && r.Name.Contains(FreeProdName == "" || FreeProdName == null ? r.Name : FreeProdName)
                                      select new
                                      {
                                          ProductID = r.ID,
                                          ProductName = r.Name

                                      }).ToList();

                TotalCount = productDetails.Count;
                ViewBag.TotalCount = TotalCount;
                var product = productDetails.Select(c => c.ProductID).Distinct().OrderBy(x => x).AsQueryable();
                var prodList = product.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = prodList.Count;
                TotalCount = product.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;

                foreach (var pID in prodList)
                {
                    var ProductOfferZoneViewModel = (from r in db.Products
                                                     join SP in db.ShopProducts on r.ID equals SP.ProductID
                                                     join SS in db.ShopStocks on SP.ID equals SS.ShopProductID
                                                     join PV in db.ProductVarients on SS.ProductVarientID equals PV.ID
                                                     join C in db.Colors on PV.ColorID equals C.ID
                                                     join S in db.Sizes on PV.SizeID equals S.ID
                                                     join M in db.Materials on PV.MaterialID equals M.ID
                                                     join D in db.Dimensions on PV.DimensionID equals D.ID
                                                     join OZP in db.OfferZoneProducts on SS.ID equals OZP.ShopStockID into OZP_join
                                                     from OZP in OZP_join.DefaultIfEmpty()
                                                     where r.ID == pID && SP.ShopID == ShopID && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                                     select new ProductOfferZoneViewModel
                                                     {
                                                         ProductID = r.ID,
                                                         ProductName = r.Name,
                                                         ShopStockID = SS.ID,
                                                         //ShopImage = ImageDisplay.LoadProductThumbnails(P.ID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved),
                                                         ProductVarientID = PV.ID,
                                                         ColorName = C.Name,
                                                         SizeName = S.Name,
                                                         DimensionName = D.Name,
                                                         MaterialName = M.Name,
                                                         PackSize = SS.PackSize,
                                                         Qty = SS.Qty,
                                                         MRP = SS.MRP,
                                                         RetailerRate = SS.RetailerRate


                                                     }).ToList();
                    foreach (var i in ProductOfferZoneViewModel)
                    {
                        ProductOfferZoneViewModel objProductOfferZoneViewModel = new ProductOfferZoneViewModel();
                        objProductOfferZoneViewModel.ProductID = i.ProductID;
                        objProductOfferZoneViewModel.ProductName = i.ProductName;
                        objProductOfferZoneViewModel.ShopStockID = i.ShopStockID;
                        objProductOfferZoneViewModel.ProductVarientID = i.ProductVarientID;
                        objProductOfferZoneViewModel.ColorName = i.ColorName;
                        objProductOfferZoneViewModel.SizeName = i.SizeName;
                        objProductOfferZoneViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(i.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                        objProductOfferZoneViewModel.DimensionName = i.DimensionName;
                        objProductOfferZoneViewModel.MaterialName = i.MaterialName;
                        objProductOfferZoneViewModel.PackSize = i.PackSize;
                        objProductOfferZoneViewModel.Qty = i.Qty;
                        objProductOfferZoneViewModel.MRP = i.MRP;
                        objProductOfferZoneViewModel.RetailerRate = i.RetailerRate;
                        objProductOfferZoneViewModel.IsActive = i.IsActive;
                        lstProductOfferZoneViewModel.Add(objProductOfferZoneViewModel);
                    }
                }
                //return Json(lstProductOfferZoneViewModel);
                return PartialView("_LoadFreeProducts", lstProductOfferZoneViewModel.ToList());



            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //return View();
            return Json(lstProductOfferZoneViewModel);
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult LoadFreeProductsName(long OfferID, long ShopStockID, int PageIndex, int PageSize, string OfferName, int CheckedCount, int CategoryID, string FreeProdName)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            ViewBag.OfferID = OfferID;
            ViewBag.OfferName = OfferName;
            ViewBag.ShopStockID = ShopStockID;
            ViewBag.CheckedCount = CheckedCount;
            List<ProductOfferZoneViewModel> lstProductOfferZoneViewModel = new List<ProductOfferZoneViewModel>();
            try
            {
                var Category = (from op in db.OwnerPlans
                                join p in db.Plans on op.PlanID equals p.ID
                                join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                join c in db.Categories on pcc.CategoryID equals c.ID
                                where pcc.IsActive == true && op.IsActive == true && op.OwnerID == ShopID
                                && op.Plan.PlanCode.StartsWith("GBMR") && c.IsActive == true
                                select new ForLoopClass { Name = c.Name, ID = c.ID }).OrderBy(x => x.Name).ToList();

                ViewBag.FreeCategoryID = new SelectList(Category, "ID", "Name");
                var productDetails = (from op in db.OwnerPlans
                                      join p in db.Plans on op.PlanID equals p.ID
                                      join pcc in db.PlanCategoryCharges on op.PlanID equals pcc.PlanID
                                      join c in db.Categories on pcc.CategoryID equals c.ID
                                      join r in db.Products on c.ID equals r.CategoryID
                                      join SP in db.ShopProducts on r.ID equals SP.ProductID
                                      //join pp in ProductId on r.ID equals pp.ProductID
                                      where p.PlanCode.Contains("GBMR") && op.OwnerID == ShopID && SP.ShopID == ShopID
                                      && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                      && r.Name.Contains(FreeProdName)
                                      select new
                                      {
                                          ProductID = r.ID,
                                          ProductName = r.Name

                                      }).ToList();

                TotalCount = productDetails.Count;
                ViewBag.TotalCount = TotalCount;
                var product = productDetails.Select(c => c.ProductID).Distinct().OrderBy(x => x).AsQueryable();
                var prodList = product.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.PageSize = prodList.Count;
                TotalCount = product.Count();
                TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                ViewBag.TotalPages = TotalPages;

                foreach (var pID in prodList)
                {
                    var ProductOfferZoneViewModel = (from r in db.Products
                                                     join SP in db.ShopProducts on r.ID equals SP.ProductID
                                                     join SS in db.ShopStocks on SP.ID equals SS.ShopProductID
                                                     join PV in db.ProductVarients on SS.ProductVarientID equals PV.ID
                                                     join C in db.Colors on PV.ColorID equals C.ID
                                                     join S in db.Sizes on PV.SizeID equals S.ID
                                                     join M in db.Materials on PV.MaterialID equals M.ID
                                                     join D in db.Dimensions on PV.DimensionID equals D.ID
                                                     join OZP in db.OfferZoneProducts on SS.ID equals OZP.ShopStockID into OZP_join
                                                     from OZP in OZP_join.DefaultIfEmpty()
                                                     where r.ID == pID && SP.ShopID == ShopID && r.IsActive == true && r.CategoryID == (CategoryID == 0 ? r.CategoryID : CategoryID)
                                                     select new ProductOfferZoneViewModel
                                                     {
                                                         ProductID = r.ID,
                                                         ProductName = r.Name,
                                                         ShopStockID = SS.ID,
                                                         //ShopImage = ImageDisplay.LoadProductThumbnails(P.ID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved),
                                                         ProductVarientID = PV.ID,
                                                         ColorName = C.Name,
                                                         SizeName = S.Name,
                                                         DimensionName = D.Name,
                                                         MaterialName = M.Name,
                                                         PackSize = SS.PackSize,
                                                         Qty = SS.Qty,
                                                         MRP = SS.MRP,
                                                         RetailerRate = SS.RetailerRate


                                                     }).ToList();
                    foreach (var i in ProductOfferZoneViewModel)
                    {
                        ProductOfferZoneViewModel objProductOfferZoneViewModel = new ProductOfferZoneViewModel();
                        objProductOfferZoneViewModel.ProductID = i.ProductID;
                        objProductOfferZoneViewModel.ProductName = i.ProductName;
                        objProductOfferZoneViewModel.ShopStockID = i.ShopStockID;
                        objProductOfferZoneViewModel.ProductVarientID = i.ProductVarientID;
                        objProductOfferZoneViewModel.ColorName = i.ColorName;
                        objProductOfferZoneViewModel.SizeName = i.SizeName;
                        objProductOfferZoneViewModel.ShopImage = ImageDisplay.LoadProductThumbnails(i.ProductID, "Default", string.Empty, ProductUpload.THUMB_TYPE.SD, ProductUpload.IMAGE_TYPE.Approved);
                        objProductOfferZoneViewModel.DimensionName = i.DimensionName;
                        objProductOfferZoneViewModel.MaterialName = i.MaterialName;
                        objProductOfferZoneViewModel.PackSize = i.PackSize;
                        objProductOfferZoneViewModel.Qty = i.Qty;
                        objProductOfferZoneViewModel.MRP = i.MRP;
                        objProductOfferZoneViewModel.RetailerRate = i.RetailerRate;
                        objProductOfferZoneViewModel.IsActive = i.IsActive;
                        lstProductOfferZoneViewModel.Add(objProductOfferZoneViewModel);
                    }
                }
                //return Json(lstProductOfferZoneViewModel);
                return PartialView("_LoadFreeProducts", lstProductOfferZoneViewModel.ToList());



            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:LoadProducts]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //return View();
            return Json(lstProductOfferZoneViewModel);
        }


        [HttpPost]
        [SessionExpire]
        public JsonResult BindSavedOffersToProduct(long OfferID)
        {
            List<long> ProductOffer = new List<long>();
            try
            {
                ProductOffer = db.OfferZoneProducts.Where(x => x.OfferID == OfferID).Select(x => x.ShopStockID).ToList();
                //(from OZP in db.OfferZoneProducts
                //            where OZP.OfferID == OfferID
                //            select new ProductOfferZoneViewModel
                //            {
                //                ShopStockID = OZP.ShopStockID,
                //                IsActive = OZP.IsActive
                //            }).ToList();
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
        public JsonResult BindFreeSavedOffersToProduct(long OfferID, long ShopStockID)
        {
            List<long?> ProductFreeOffer = new List<long?>();
            try
            {
                ProductFreeOffer = db.OfferZoneProducts.Where(x => x.OfferID == OfferID && x.ShopStockID == ShopStockID).Select(x => x.FreeStockID).ToList();
                //(from OZP in db.OfferZoneProducts
                //            where OZP.OfferID == OfferID
                //            select new ProductOfferZoneViewModel
                //            {
                //                ShopStockID = OZP.ShopStockID,
                //                IsActive = OZP.IsActive
                //            }).ToList();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading Free Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:BindFreeSavedOffersToProduct]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading Free Products!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:BindFreeSavedOffersToProduct]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return Json(ProductFreeOffer.ToList());
        }

        [HttpPost]
        [SessionExpire]
        public JsonResult GetFreeQuantity(long OfferID)
        {
            int FreeQty = 0;
            try
            {
                FreeQty = db.Offers.Where(x => x.ID == OfferID && x.OwnerID == ShopID).Select(x => x.FreeOty).FirstOrDefault();

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in retriving free Quantity!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[OfferZone][POST:GetFreeQuantity]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in retriving free Quantity!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[OfferZone][POST:GetFreeQuantity]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return Json(FreeQty);
        }
        [HttpPost]
        [SessionExpire]
        public JsonResult SaveOffer(long ShopStockID, long FreeStockID, long OfferID)
        {
            string Message = "";
            try
            {
                OfferZoneProduct offerzone = new OfferZoneProduct();
                offerzone.OfferID = OfferID;
                offerzone.ShopStockID = ShopStockID;
                //offerzone.FreeStockID = FreeStockID;
                offerzone.IsActive = true;
                offerzone.CreateDate = DateTime.UtcNow;
                offerzone.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                offerzone.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                offerzone.DeviceID = "x";
                offerzone.DeviceType = "x";
                db.OfferZoneProducts.Add(offerzone);
                db.SaveChanges();
                Message = "Applied Succesfully";
                return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Saving Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:SaveComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:SaveComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }


        public JsonResult SaveFreeOffer(long ShopStockID, long FreeStockID, long OfferID)
        {
            string Message = "";
            try
            {
                OfferZoneProduct offerzone = new OfferZoneProduct();
                offerzone.OfferID = OfferID;
                offerzone.ShopStockID = ShopStockID;
                offerzone.FreeStockID = FreeStockID;
                offerzone.IsActive = true;
                offerzone.CreateDate = DateTime.UtcNow;
                offerzone.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                offerzone.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                offerzone.DeviceID = "x";
                offerzone.DeviceType = "x";
                db.OfferZoneProducts.Add(offerzone);
                db.SaveChanges();
                Message = "Applied Succesfully";
                return Json(Message);

            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong while Saving Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ProductUploadTemp][POST:SaveComponent]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Saving Offers!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:SaveComponent]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }

        public JsonResult DeleteOffer(long ShopStockID, long OfferID)
        {
            string Message = "";
            try
            {
                long ID = db.OfferZoneProducts.Where(x => x.OfferID == OfferID && x.ShopStockID == ShopStockID).Select(x => x.ID).FirstOrDefault();
                OfferZoneProduct offerzone = db.OfferZoneProducts.Find(ID);
                if (ID > 0)
                {
                    db.OfferZoneProducts.Remove(offerzone);
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
                    + "[ProductUploadTemp][POST:DeleteOffer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Removing offer on Product!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:DeleteOffer]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }

        public JsonResult DeleteFreeOffer(long ShopStockID, long FreeStockID, long OfferID)
        {
            string Message = "";
            try
            {
                long ID = db.OfferZoneProducts.Where(x => x.OfferID == OfferID && x.ShopStockID == ShopStockID && x.FreeStockID == FreeStockID).Select(x => x.ID).FirstOrDefault();
                OfferZoneProduct offerzone = db.OfferZoneProducts.Find(ID);
                if (ID > 0)
                {
                    db.OfferZoneProducts.Remove(offerzone);
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
                    + "[ProductUploadTemp][POST:DeleteOffer]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong while Removing offer on Product!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ProductUploadTemp][POST:DeleteOffer]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            //long Message = ShopStockID;
            return Json(Message);

        }

        //
        // GET: /OfferZone/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /OfferZone/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /OfferZone/Create
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
        // GET: /OfferZone/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /OfferZone/Edit/5
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
        // GET: /OfferZone/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /OfferZone/Delete/5
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
