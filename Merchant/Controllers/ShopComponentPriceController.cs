using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using BusinessLogicLayer;
using Merchant.Models;

namespace Merchant.Controllers
{
    public class ShopComponentPriceController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private EzeeloDBContext db1 = new EzeeloDBContext();
        // GET: /ShopComponentPrice/
        [SessionExpire]
        public ActionResult Index()
        {
            try
            {
                long ShopID = GetShopID();
                var shopcomponentprices = from c in db.Components
                                          join scp in db.ShopComponentPrices on c.ID equals scp.ComponentID into scp_join
                                          from scp in scp_join.DefaultIfEmpty()
                                          join dc in db.Components on scp.DependentOnComponentID equals dc.ID into dc_join
                                          from dc in dc_join.DefaultIfEmpty()
                                          join u in db.Units on scp.ComponentUnitID equals u.ID into u_join
                                          from u in u_join.DefaultIfEmpty()
                                          where
                                            scp.ShopID == ShopID

                                          select new ProductComponentViewModel
                                          {
                                              ComponentID = c.ID,
                                              ComponentName = c.Name,
                                              ComponentUnitID = (u.ID == null ? 0 : u.ID),
                                              ComponentUnitName = (u.Name == string.Empty ? string.Empty : u.Name),
                                              DependentOnComponentID = (scp.DependentOnComponentID == null ? 0 : scp.DependentOnComponentID),
                                              //DependentOnComponentID = scp.DependentOnComponentID,
                                              DependentComponentName = (dc.Name == string.Empty ? "N/A" : dc.Name),
                                              PerUnitRateInPer = scp.PerUnitRateInPer,
                                              PerUnitRateInRs = scp.PerUnitRateInRs,
                                              ShopID = scp.ShopID

                                          };
                return View(shopcomponentprices.ToList());
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

        [SessionExpire]
        public ActionResult LoadProducts(int? componentID, int PageIndex, int PageSize)
        {
            int TotalCount = 0;
            int TotalPages = 0;
            decimal TotalSaleRate = 0;
            ViewBag.CompID = componentID;
            decimal Total = 0;
            try
            {
                long ShopID = GetShopID();
                var ProductDetails = (from TSC in db.StockComponents
                                      join TSS in db.ShopStocks on TSC.ShopStockID equals TSS.ID
                                      join TSP in db.ShopProducts on TSS.ShopProductID equals TSP.ID
                                      join TP in db.Products on TSP.ProductID equals TP.ID
                                      where TSC.ComponentID == componentID && TSP.ShopID == ShopID

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
                                            DependentOnComponentID = (scp.DependentOnComponentID==null?0:scp.DependentOnComponentID),
                                            PerUnitRateInRs = scp.PerUnitRateInRs,
                                            PerUnitRateInPer = scp.PerUnitRateInPer,
                                            //Total = tss.MRP,
                                            DependentComponentName = (dc.Name == string.Empty ? "N/A" : dc.Name),
                                            ProductVarientID = tss.ProductVarientID,
                                        }).Distinct().ToList();
              
               
                var product1 = ProductDetails.Select(c => c.ProductID).Distinct().OrderBy(x=>x).AsQueryable();
                var products = product1.Skip((PageIndex-1) * PageSize).Take(PageSize);
                TotalCount = product1.Count();
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


                                           where TSC.ComponentID == componentID && TSP.ShopID == ShopID && TP.ID == pID

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
                return View("_ProductDetails", listProductComponent);
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
                        Total = Convert.ToDecimal( comp.PerUnitRateInRs * comp.ComponentWeight);
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

        private void UpdateSaleRateInDB(int? componentID)
        {
            try
            {
                ShopStock shopStock = new ShopStock();
                long ShopID = GetShopID();
                var ProductDetails = (from TSC in db.StockComponents
                                     join TSS in db.ShopStocks on TSC.ShopStockID equals TSS.ID
                                     join TSP in db.ShopProducts on TSS.ShopProductID equals TSP.ID
                                     join TP in db.Products on TSP.ProductID equals TP.ID
                                     join C in db.Components on TSC.ComponentID equals C.ID
                                     join TPV in db.ProductVarients on TSS.ProductVarientID equals TPV.ID
                                     join Co in db.Colors on TPV.ColorID equals Co.ID
                                     join S in db.Sizes on TPV.SizeID equals S.ID
                                     join D in db.Dimensions on TPV.DimensionID equals D.ID
                                     join M in db.Materials on TPV.MaterialID equals M.ID

                                     where TSC.ComponentID == componentID && TSP.ShopID == ShopID

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
                                         Total = TSS.MRP

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
                                            DependentOnComponentID = (scp.DependentOnComponentID==null?0:scp.DependentOnComponentID),
                                            PerUnitRateInRs = scp.PerUnitRateInRs,
                                            PerUnitRateInPer = scp.PerUnitRateInPer,
                                            //Total = tss.MRP,
                                            DependentComponentName = (dc.Name == string.Empty ? "N/A" : dc.Name),
                                            ProductVarientID = tss.ProductVarientID,
                                        }).OrderBy(x=>x.ComponentID).Distinct().ToList();
                List<ProductVarientViewModel> listProductComponent = new List<ProductVarientViewModel>();
                decimal TotalSaleRate = 0;
                foreach (var product in ProductDetails)
                {
                    ProductVarientViewModel ObjProductComponentViewModel = new ProductVarientViewModel();

                    ObjProductComponentViewModel.ProductID = product.ProductID;
                    ObjProductComponentViewModel.ProductVarientID = product.ProductVarientID;
                    foreach (var Component in ComponentDetails.Where(x => x.ProductVarientID == product.ProductVarientID))
                    {
                        ShopProductComponentViewModel ObjComponentList = new ShopProductComponentViewModel();
                        ObjComponentList.ComponentID = Component.ComponentID;
                        ObjComponentList.ComponentName = Component.ComponentName;
                        ObjComponentList.ComponentWeight = Convert.ToDecimal(Component.ComponentWeight);
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
                    long ShopProductID = db1.ShopProducts.Where(x => x.ShopID == ShopID && x.ProductID == ObjProductComponentViewModel.ProductID).Select(x => x.ID).First();
                    long ShopStockID = db1.ShopStocks.Where(x => x.ShopProductID == ShopProductID && x.ProductVarientID == ObjProductComponentViewModel.ProductVarientID).Select(x => x.ID).First();
                    if (ShopStockID > 0)
                    {
                        shopStock = db1.ShopStocks.Find(ShopStockID);
                        shopStock.MRP = Convert.ToDecimal(TotalSaleRate);
                        //shopStock.WholeSaleRate = Convert.ToDecimal(TotalSaleRate);
                        shopStock.RetailerRate = Convert.ToDecimal(TotalSaleRate);
                        //TryUpdateModel(shopStock);
                        db1.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopComponentPriceController][UpdateSaleRateInDB]", "Unable to Update Sale Rate in Database. !" + Environment.NewLine + ex.Message);
            }
        }

        private void UpdateOldSaleRateinLog(int? componentID)
        {
            try
            {
                StockComponentPriceLog stockComponentPriceLog = new StockComponentPriceLog();
                long ShopID = GetShopID();
                long PersonalDetailID = GetPersonalDetailID();
                var ProductDetails = (from TSC in db.StockComponents
                                     join TSS in db.ShopStocks on TSC.ShopStockID equals TSS.ID
                                     join TSP in db.ShopProducts on TSS.ShopProductID equals TSP.ID
                                     join TP in db.Products on TSP.ProductID equals TP.ID
                                     join C in db.Components on TSC.ComponentID equals C.ID
                                     join TPV in db.ProductVarients on TSS.ProductVarientID equals TPV.ID
                                     join Co in db.Colors on TPV.ColorID equals Co.ID
                                     join S in db.Sizes on TPV.SizeID equals S.ID
                                     join D in db.Dimensions on TPV.DimensionID equals D.ID
                                     join M in db.Materials on TPV.MaterialID equals M.ID

                                     where TSC.ComponentID == componentID && TSP.ShopID == ShopID

                                     select new
                                     {
                                         ProductID = TP.ID,
                                         ProductVarientID = TPV.ID,
                                         Total = TSS.MRP

                                     }).OrderBy(x => x.ProductID).ToList();

                foreach (var product in ProductDetails)
                {
                    ProductVarientViewModel ObjProductComponentViewModel = new ProductVarientViewModel();

                    ObjProductComponentViewModel.ProductID = product.ProductID;
                    ObjProductComponentViewModel.ProductVarientID = product.ProductVarientID;
                    ObjProductComponentViewModel.Total = product.Total;

                    long ShopProductID = db1.ShopProducts.Where(x => x.ShopID == ShopID && x.ProductID == ObjProductComponentViewModel.ProductID).Select(x => x.ID).First();
                    var ShopStock = db1.ShopStocks.Where(x => x.ShopProductID == ShopProductID && x.ProductVarientID == ObjProductComponentViewModel.ProductVarientID).FirstOrDefault();
                    if (ShopStock.ID > 0)
                    {
                        stockComponentPriceLog.ShopStockID = ShopStock.ID;
                        stockComponentPriceLog.OldSaleRate = ShopStock.MRP;
                        stockComponentPriceLog.CreatedDate = DateTime.UtcNow;
                        stockComponentPriceLog.CreatedBy = PersonalDetailID;
                        stockComponentPriceLog.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                        stockComponentPriceLog.DeviceID = "x";
                        stockComponentPriceLog.DeviceType = "x";
                        db1.StockComponentPriceLogs.Add(stockComponentPriceLog);
                        db1.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShopComponentPriceController][UpdateOldSaleRateinLog]", "Unable to Update  old Sale Rate in Database. !" + Environment.NewLine + ex.Message);
            }
        }

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
                throw new BusinessLogicLayer.MyException("[ShopComponentPriceController][GetPersonalDetailID]", "Can't find PersonalDetailID !" + Environment.NewLine + ex.Message);
            }
            return PersonalDetailID;
        }
        // GET: /ShopComponentPrice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShopComponentPrice shopcomponentprice = db.ShopComponentPrices.Find(id);
            if (shopcomponentprice == null)
            {
                return HttpNotFound();
            }
            return View(shopcomponentprice);
        }

        // GET: /ShopComponentPrice/Create
        [SessionExpire]
        public ActionResult Create()
        {
            try
            {
                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name", "Select Component");
                ViewBag.DependentOnComponentID = new SelectList(db.Components, "ID", "Name", "Select Dependent Component");
                ViewBag.ComponentUnitID = new SelectList(db.Units, "ID", "Name", "Select Unit");
                ViewBag.PerUnitRateInPer = 0;
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading create of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading create of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        // POST: /ShopComponentPrice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpire]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ComponentID,ShopID,ComponentUnitID,PerUnitRateInRs,PerUnitRateInPer,DependentOnComponentID,IsActive,CreateDate,CreateBy")] ShopComponentPrice shopcomponentprice)
        {
            long ID = 0;
            try
            {
                shopcomponentprice.ShopID = GetShopID();
                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name", shopcomponentprice.ComponentID);
                ViewBag.DependentOnComponentID = new SelectList(db.Components, "ID", "Name", shopcomponentprice.DependentOnComponentID);
                ViewBag.ComponentUnitID = new SelectList(db.Units, "ID", "Name", shopcomponentprice.ComponentUnitID);

                ID = db.ShopComponentPrices.Where(x => x.ShopID == shopcomponentprice.ShopID && x.ComponentID == shopcomponentprice.ComponentID).Select(x => x.ID).FirstOrDefault();
                if (ID > 0)
                {

                    ModelState.AddModelError("Error", "This component is already exist.Please select another component");
                    return View();
                }
                if(shopcomponentprice.ComponentUnitID==null)
                {
                    shopcomponentprice.ComponentUnitID = 1;
                }
                shopcomponentprice.CreateDate = DateTime.UtcNow;
                shopcomponentprice.CreateBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                shopcomponentprice.NetworkIP = BusinessLogicLayer.CommonFunctions.GetClientIP();
                shopcomponentprice.DeviceID = "x";
                shopcomponentprice.DeviceType = "x";
                //if (ModelState.IsValid)
                //{
                db.ShopComponentPrices.Add(shopcomponentprice);
                db.SaveChanges();
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in loading create of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Create]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in loading create of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Create]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }


            return RedirectToAction("Index");
            //}

        }

        // GET: /ShopComponentPrice/Edit/5
        [SessionExpire]
        public ActionResult Edit(int ComponentID, long ShopID)
        {
            var shopcomponentprice = db.ShopComponentPrices.Where(x => x.ComponentID == ComponentID && x.ShopID == ShopID).FirstOrDefault();
            try
            {

                if (shopcomponentprice == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name", shopcomponentprice.ComponentID);
                ViewBag.DependentOnComponentID = new SelectList(db.Components, "ID", "Name", shopcomponentprice.DependentOnComponentID);
                ViewBag.ComponentUnitID = new SelectList(db.Units, "ID", "Name", shopcomponentprice.ComponentUnitID);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Edit of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Edit of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(shopcomponentprice);
        }

        // POST: /ShopComponentPrice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ComponentID,ShopID,ComponentUnitID,PerUnitRateInRs,PerUnitRateInPer,DependentOnComponentID,IsActive,CreateDate,CreateBy,ModifyDate,ModifyBy")] ShopComponentPrice shopcomponentprice)
        {
            EzeeloDBContext db1 = new EzeeloDBContext();
            try
            {
                UpdateOldSaleRateinLog(shopcomponentprice.ComponentID);
                ShopComponentPrice CompPrice = db1.ShopComponentPrices.Find(shopcomponentprice.ID);
                if (shopcomponentprice.ComponentUnitID == null)
                {
                    shopcomponentprice.ComponentUnitID = CompPrice.ComponentUnitID;
                }
                shopcomponentprice.DependentOnComponentID = CompPrice.DependentOnComponentID;
                shopcomponentprice.CreateDate = CompPrice.CreateDate;
                shopcomponentprice.CreateBy = CompPrice.CreateBy;
                shopcomponentprice.ModifyDate = DateTime.Now;
                shopcomponentprice.ModifyBy = CommonFunctions.GetPersonalDetailsID(Convert.ToInt64(Session["USER_LOGIN_ID"]));
                if (ModelState.IsValid)
                {
                    db.Entry(shopcomponentprice).State = EntityState.Modified;
                    db.SaveChanges();
                    UpdateSaleRateInDB(shopcomponentprice.ComponentID);
                    ModelState.AddModelError("Error", "Component Rate Update Succesfully.");

                }
                ViewBag.ComponentID = new SelectList(db.Components, "ID", "Name", shopcomponentprice.ComponentID);
                ViewBag.DependentOnComponentID = new SelectList(db.Components, "ID", "Name", shopcomponentprice.DependentOnComponentID);
                ViewBag.ComponentUnitID = new SelectList(db.Units, "ID", "Name", shopcomponentprice.ComponentUnitID);
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Edit of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Edit]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Edit of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Edit]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(shopcomponentprice);
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

        // GET: /ShopComponentPrice/Delete/5
        public ActionResult Delete(int? ComponentID, long? ShopID)
        {
            long id = db.ShopComponentPrices.Where(x => x.ShopID == ShopID && x.ComponentID == ComponentID).Select(x => x.ID).FirstOrDefault();
            ShopComponentPrice shopcomponentprice = db.ShopComponentPrices.Find(id);
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (shopcomponentprice == null)
                {
                    return HttpNotFound();
                }
            }
            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Edit of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Edit of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View(shopcomponentprice);
        }

        // POST: /ShopComponentPrice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? ComponentID, long? ShopID)
        {
            long id = 0;
            try
            {
                var ProductDetails = (from TSC in db.StockComponents
                                     join TSS in db.ShopStocks on TSC.ShopStockID equals TSS.ID
                                     join TSP in db.ShopProducts on TSS.ShopProductID equals TSP.ID
                                     join TP in db.Products on TSP.ProductID equals TP.ID
                                     join C in db.Components on TSC.ComponentID equals C.ID
                                     join TPV in db.ProductVarients on TSS.ProductVarientID equals TPV.ID
                                     join Co in db.Colors on TPV.ColorID equals Co.ID
                                     join S in db.Sizes on TPV.SizeID equals S.ID
                                     join D in db.Dimensions on TPV.DimensionID equals D.ID
                                     join M in db.Materials on TPV.MaterialID equals M.ID

                                     where TSC.ComponentID == ComponentID && TSP.ShopID == ShopID

                                     select new ProductVarientViewModel
                                     {
                                         ProductID = TP.ID,
                                     }).ToList();

                id = db.ShopComponentPrices.Where(x => x.ShopID == ShopID && x.ComponentID == ComponentID).Select(x => x.ID).FirstOrDefault();
                ShopComponentPrice shopcomponentprice = db.ShopComponentPrices.Find(id);
                if (ProductDetails.Count>0)
                {
                    ModelState.AddModelError("Error", "You can not delete this component because it is present in various product.");
                    return View(shopcomponentprice);
                }
                else
                {
                    if (id > 0)
                    {
                        db.ShopComponentPrices.Remove(shopcomponentprice);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "There's Something wrong in Deleting of shop component Prices!!");
                    }
                }
            }

            catch (BusinessLogicLayer.MyException myEx)
            {
                ModelState.AddModelError("Error", "There's Something wrong in Deleting of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + myEx.EXCEPTION_MSG + Environment.NewLine
                    + "[ShopComponentPrice][POST:Delete]" + myEx.EXCEPTION_PATH,
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Error", "There's Something wrong in Deleting of shop component Prices!!");

                BusinessLogicLayer.ErrorLog.ErrorLogFile(DateTime.UtcNow.AddHours(5.5).ToShortTimeString()
                    + Environment.NewLine + ex.Message + Environment.NewLine
                    + "[ShopComponentPrice][POST:Delete]",
                    BusinessLogicLayer.ErrorLog.Module.Merchant, System.Web.HttpContext.Current.Server);
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
