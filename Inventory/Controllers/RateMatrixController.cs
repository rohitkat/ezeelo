using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.Enum;
using ModelLayer.Models.ViewModel;

namespace Inventory.Controllers
{
    public class RateMatrixController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        Margin_DivisionConstants objMagDiv = new Margin_DivisionConstants();
        public ActionResult ProductList(int? Month, long? ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
            Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
            //if (obj.Entity.Trim() == "EVW")
            //{
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            List<ProductRateListViewModel> lProduct = new List<ProductRateListViewModel>();
            if (ProductId != null || ProductId <= 0)
            {
                lProduct = db.Products.Where(p => p.ID == ProductId)
                    .Select(p => new ProductRateListViewModel
                    {
                        ProductId = p.ID,
                        ProductName = p.Name,
                        HSNCode = p.HSNCode,
                        IsActive = p.IsActive,
                        VarientCount = db.ProductVarients
                                       .Where(pv => pv.ProductID == p.ID && p.IsActive == true).Distinct().Count()
                    }).ToList();
            }
            else
            {
                lProduct = db.QuotationItemDetails
                                 .Join(db.Quotations.Where(q =>
                                 (db.QuotationSupplierLists.Where(qs => qs.SupplierID == db.Suppliers.FirstOrDefault(s => s.WarehouseID == EzeeloWarehouseId).ID).Select(qs => qs.QuotationID))
                                 .Contains(q.ID)
                                 || q.RequestFromWarehouseID == EzeeloWarehouseId), qi => qi.QuotationID, q => q.ID,
                                 (qi, q) => new
                                 {
                                     ProductID = qi.ProductID,
                                     QuotationRequestDate = q.QuotationRequestDate,
                                     QuotationModificationDate = q.ModifyDate,
                                     QuotationID = q.ID
                                 }).ToList()
                                 .Join(db.Products, qi => qi.ProductID, p => p.ID,
                                 (qi, p) => new ProductRateListViewModel
                                 {
                                     ProductId = p.ID,
                                     ProductName = p.Name,
                                     HSNCode = p.HSNCode,
                                     IsActive = p.IsActive,
                                     QuotationDate = qi.QuotationModificationDate == null ? (DateTime)qi.QuotationRequestDate : (DateTime)qi.QuotationModificationDate,
                                     QuotationID = qi.QuotationID,
                                     VarientCount = db.ProductVarients
                                                    .Where(pv => pv.ProductID == p.ID && p.IsActive == true).Distinct().Count()
                                 })
                                 .Distinct().Where(p => p.IsActive == true)
                                 .GroupBy(p => p.ProductId)
                                 .Select(p => p.ToList().OrderByDescending(m => m.QuotationDate).FirstOrDefault())
                                 .OrderByDescending(p => p.QuotationID).ThenByDescending(p => p.QuotationDate).ToList()
                                 .ToList();
                if (Month == 0 || Month == null)
                {
                    if (Session["ProductListMonth"] == null)
                    {
                        Month = DateTime.Now.Date.Month;
                    }
                    else
                    {
                        Month = Convert.ToInt16(Session["ProductListMonth"]);
                    }
                }
                else
                {
                    Session["ProductListMonth"] = Month;
                }

                lProduct = lProduct.Where(p => p.QuotationDate.Month == Month).ToList();

            }


            ViewBag.LEADERSHIP_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
            ViewBag.EZEELO_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.EZEELO);
            ViewBag.LEADERS_ROYALTY_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
            ViewBag.LIFESTYLE_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
            ViewBag.LEADERSHIP_DEVELOPMENT_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);
            ViewBag.DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == WarehouseId && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID))).ToList();

            ViewBag.LEADERSHIPMaster = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
            ViewBag.EZEELOMaster = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
            ViewBag.LEADERS_ROYALTYMaster = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
            ViewBag.LIFESTYLE_FUNDMaster = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
            ViewBag.LEADERSHIP_DEVELOPMENT_FUNDMaster = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);


            if (Session["txtMrgn1"] == null)
            {
                ViewBag.LEADERSHIP = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                ViewBag.EZEELO = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                ViewBag.LEADERS_ROYALTY = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                ViewBag.LIFESTYLE_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                ViewBag.LEADERSHIP_DEVELOPMENT_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);

                Session["txtMrgn1"] = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                Session["txtMrgn2"] = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                Session["txtMrgn3"] = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                Session["txtMrgn4"] = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                Session["txtMrgn5"] = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);
                string Ids = String.Join(",", db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == WarehouseId && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID))).Select(p => p.ID).ToList());
                Session["WarehouseIdCollection"] = ',' + Ids + ',';
                ViewBag.SelectedDVList = ',' + Ids + ',';
            }
            else
            {
                ViewBag.LEADERSHIP = Convert.ToInt16(Session["txtMrgn1"]);
                ViewBag.EZEELO = Convert.ToInt16(Session["txtMrgn2"]);
                ViewBag.LEADERS_ROYALTY = Convert.ToInt16(Session["txtMrgn3"]);
                ViewBag.LIFESTYLE_FUND = Convert.ToInt16(Session["txtMrgn4"]);
                ViewBag.LEADERSHIP_DEVELOPMENT_FUND = Convert.ToInt16(Session["txtMrgn5"]);
                ViewBag.SelectedDVList = Session["WarehouseIdCollection"].ToString();

            }


            ViewBag.SelectedMonth = Month;
            foreach (var item in lProduct)
            {
                item.ProductImgPath = ImageDisplay.SetProductThumbPath(item.ProductId, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            }

            return View(lProduct);
            //}
            //else
            //{
            //    Session["Warning"] = "Sorry! You are not authorized user.";
            //    return RedirectToAction("Index", "Home");
            //}
        }

        public ActionResult SaveDefaultValues(float txtMrgn1, float txtMrgn2, float txtMrgn3, float txtMrgn4, float txtMrgn5, string WarehouseIdCollection)
        {
            Session["txtMrgn1"] = txtMrgn1;
            Session["txtMrgn2"] = txtMrgn2;
            Session["txtMrgn3"] = txtMrgn3;
            Session["txtMrgn4"] = txtMrgn4;
            Session["txtMrgn5"] = txtMrgn5;
            Session["WarehouseIdCollection"] = WarehouseIdCollection;
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddRate(long ProductId)
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                Warehouse objW = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (objW.Entity.Trim() == "EVW")
                {
                    ProductRateListViewModel obj = new ProductRateListViewModel();
                    Product _product = db.Products.FirstOrDefault(p => p.ID == ProductId);
                    obj.ProductId = _product.ID;
                    obj.HSNCode = _product.HSNCode;
                    obj.ProductName = _product.Name;
                    obj.ProductImgPath = ImageDisplay.SetProductThumbPath(obj.ProductId, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                    List<ProductVarientRateListViewModel> lProductVarient = new List<ProductVarientRateListViewModel>();

                    lProductVarient = db.ProductVarients.Where(pv => pv.ProductID == ProductId && pv.IsActive == true)
                        .Join(db.Sizes.Where(s => s.IsActive == true && s.Name != ""), pv => pv.SizeID, s => s.ID, (pv, s) =>
                            new ProductVarientRateListViewModel
                            {
                                ProductId = ProductId,
                                ProductVarientId = pv.ID,
                                IsEditable = true,
                                IsFromQuotation = (db.QuotationItemDetails.FirstOrDefault(q => q.ProductID == ProductId && q.ProductVarientID == pv.ID) != null) ? true : false,
                                VarientName = s.Name,
                                GSTInPer = (db.RateMatrix.FirstOrDefault(rc => rc.ProductId == ProductId) == null) ? 0 : (db.RateMatrix.FirstOrDefault(rc => rc.ProductId == ProductId).GSTInPer),
                            }).Distinct().ToList();
                    foreach (var item in lProductVarient)
                    {
                        item.MRP_ = 0;
                        item.PurchaseRate = 0;
                        item.IsActive = true;
                        item.IsEditable = true;
                    }
                    obj.VarientList = lProductVarient;
                    string WarehouseIdCollection = Session["WarehouseIdCollection"].ToString();
                    string TruncateId = WarehouseIdCollection.Substring(1, WarehouseIdCollection.Length - 2);
                    long[] WarehouseIdArray = TruncateId.Split(',').Select(Int64.Parse).ToArray();
                    List<Warehouse> DVList = db.Warehouses.Where(p => WarehouseIdArray.Contains(p.ID)).ToList();
                    ViewBag.DVList = String.Join(", ", DVList.Select(p => p.Name).ToList());
                    return View(obj);
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult AddRate(ProductRateListViewModel model)
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                long RateMatrixId = 0;
                Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (obj.Entity.Trim() == "EVW")
                {
                    foreach (var item in model.VarientList.Where(p => p.checkbox == true).ToList())
                    {
                        using (TransactionScope transactionScope = new TransactionScope())
                        {
                            try
                            {
                                //To save Data in RateMatricExtension
                                string WarehouseIdCollection = Session["WarehouseIdCollection"].ToString();
                                string TruncateId = WarehouseIdCollection.Substring(1, WarehouseIdCollection.Length - 2);
                                long[] WarehouseIdArray = TruncateId.Split(',').Select(Int64.Parse).ToArray();
                                List<Warehouse> DVList = db.Warehouses.Where(p => WarehouseIdArray.Contains(p.ID)).ToList();
                                DeactiveRate(item.ProductId, item.ProductVarientId, DVList, obj.ID);

                                RateMatrix rateMatrix = new RateMatrix();
                                rateMatrix.ProductId = item.ProductId;
                                rateMatrix.ProductVarientId = item.ProductVarientId;
                                rateMatrix.MRP = item.MRP;
                                rateMatrix.GSTInPer = item.GSTInPer;
                                rateMatrix.GrossMarginFlat = item.GrossMarginFlat;
                                rateMatrix.DecidedSalePrice = item.DecidedSalePrice;
                                rateMatrix.ValuePostGST = item.ValuePostGST;
                                rateMatrix.Dividend = item.Dividend;
                                rateMatrix.BaseInwardPriceEzeelo = item.BaseInwardPriceEzeelo;
                                rateMatrix.InwardMarginValue = item.InwardMarginValue;
                                rateMatrix.GSTOnPR = item.GSTOnPR;
                                rateMatrix.MaxInwardMargin = item.MaxInwardMargin;
                                rateMatrix.MarginPassedToCustomer = item.MarginPassedToCustomer;
                                rateMatrix.ActualFlatMargin = item.ActualFlatMargin;
                                rateMatrix.DeviceID = "X";
                                rateMatrix.DeviceType = "X";
                                rateMatrix.NetworkIP = CommonFunctions.GetClientIP();
                                rateMatrix.RateExpiry = item.RateExpiry;
                                rateMatrix.IsActive = true;
                                rateMatrix.CreatedBy = WarehouseId;
                                rateMatrix.CreatedDate = DateTime.Now;
                                db.RateMatrix.Add(rateMatrix);
                                db.SaveChanges();
                                RateMatrixId = rateMatrix.ID;

                                //Loop through Each DV
                                foreach (var dv in DVList)
                                {
                                    List<Warehouse> FVList = db.Warehouses.Where(p => p.DistributorId == dv.ID).ToList();
                                    //Loop through Each FV
                                    foreach (var fv in FVList)
                                    {
                                        FVRateMarginListViewModel fVRateMarginListViewModel = CalculateForRateMatrixExtension(fv.ID, item.ProductId, item.ProductVarientId, Convert.ToDouble(Session["txtMrgn1"]), Convert.ToDouble(Session["txtMrgn2"]), Convert.ToDouble(Session["txtMrgn3"]), Convert.ToDouble(Session["txtMrgn4"]), Convert.ToDouble(Session["txtMrgn5"]), rateMatrix.GSTInPer, rateMatrix.BaseInwardPriceEzeelo, rateMatrix.DecidedSalePrice, rateMatrix.GSTOnPR, objMagDiv.BPInPaise());
                                        RateMatrixExtension rateCalculationExtension = new RateMatrixExtension();
                                        rateCalculationExtension.ProductVarientId = item.ProductVarientId;
                                        rateCalculationExtension.RateMatrixId = RateMatrixId;
                                        rateCalculationExtension.EVWID = obj.ID;

                                        rateCalculationExtension.FVID = fVRateMarginListViewModel.Id;
                                        rateCalculationExtension.FVMargin = Math.Round(fVRateMarginListViewModel.FVMargin, 2);
                                        rateCalculationExtension.FVPurchasePrice = Math.Round(fVRateMarginListViewModel.FVPurchasePrice, 4);
                                        rateCalculationExtension.FVSalePrice = Math.Round(fVRateMarginListViewModel.FVSalePrice, 4);
                                        rateCalculationExtension.FVMarginValueWithGST = Math.Round(fVRateMarginListViewModel.FVMarginVAlueWithGST, 2);
                                        rateCalculationExtension.FVGST = Math.Round(fVRateMarginListViewModel.FVGST, 2);

                                        rateCalculationExtension.DVID = fVRateMarginListViewModel.DVId;
                                        rateCalculationExtension.DVMargin = Math.Round(fVRateMarginListViewModel.DVMargin, 2);
                                        rateCalculationExtension.DVPurchasePrice = Math.Round(fVRateMarginListViewModel.DVPurchasePrice, 4);
                                        rateCalculationExtension.DVSalePrice = Math.Round(fVRateMarginListViewModel.DVSalePrice, 4);
                                        rateCalculationExtension.DVMarginValueWithGST = Math.Round(fVRateMarginListViewModel.DVMarginVAlueWithGST, 2);
                                        rateCalculationExtension.DVGST = Math.Round(fVRateMarginListViewModel.DVGST, 2);

                                        rateCalculationExtension.MarginLeftForEzeeloBeforeLeadershipPayout = Math.Round(fVRateMarginListViewModel.EzeeloMargin, 4);
                                        rateCalculationExtension.GSTForEzeeloMargin = Math.Round(fVRateMarginListViewModel.EzeeloGST, 2);
                                        rateCalculationExtension.PostGSTMargin = Math.Round(fVRateMarginListViewModel.PostGSTMargin, 2);
                                        rateCalculationExtension.ForLeadershipPercent = fVRateMarginListViewModel.ForLeadershipPer;
                                        rateCalculationExtension.ForLeadershipValue = Math.Round(fVRateMarginListViewModel.ForLeadership, 4);
                                        rateCalculationExtension.ForEzeeloPercent = fVRateMarginListViewModel.ForEzeeloPer;
                                        rateCalculationExtension.ForEzeeloValue = Math.Round(fVRateMarginListViewModel.ForEzeelo, 4);
                                        rateCalculationExtension.ForLeadersRoyaltyPercent = fVRateMarginListViewModel.ForLeadersRoyaltyPer;
                                        rateCalculationExtension.ForLeadersRoyaltyValue = Math.Round(fVRateMarginListViewModel.ForLeadersRoyalty, 4);
                                        rateCalculationExtension.ForLifestyleFundPercent = fVRateMarginListViewModel.ForLifestylePer;
                                        rateCalculationExtension.ForLifestyleFundValue = Math.Round(fVRateMarginListViewModel.ForLifestyle, 4);
                                        rateCalculationExtension.ForLeadershipDevelopmentFundPercent = fVRateMarginListViewModel.ForLeadershipDevelopmentPer;
                                        rateCalculationExtension.ForLeadershipDevelopmentFundValue = Math.Round(fVRateMarginListViewModel.ForLeadershipDevelopment, 4);
                                        rateCalculationExtension.TotalGSTInSupplyChain = Math.Round(fVRateMarginListViewModel.TotalGST, 2);
                                        rateCalculationExtension.TotalMargin = Math.Round(fVRateMarginListViewModel.TotalMargin, 2);
                                        rateCalculationExtension.OneBPInPaise = BPInPaise();
                                        rateCalculationExtension.RetailPoint = fVRateMarginListViewModel.BussinessPoints;
                                        rateCalculationExtension.CreateDate = DateTime.Now;
                                        rateCalculationExtension.CreateBy = obj.ID;
                                        rateCalculationExtension.NetworkIP = CommonFunctions.GetClientIP();
                                        rateCalculationExtension.IsActive = true;
                                        db.RateMatrixExtension.Add(rateCalculationExtension);
                                        db.SaveChanges();
                                    }
                                }

                                transactionScope.Complete();
                                transactionScope.Dispose();
                                Session["Success"] = "Rate save successfully!";
                            }
                            catch (TransactionException ex)
                            {
                                transactionScope.Dispose();
                                Session["Warning"] = ex.Message;
                                return RedirectToAction("Index", "Login");
                            }
                        }
                    }
                    return RedirectToAction("DVFVCurrentRateMarginList", new { ProductId = model.ProductId });
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Warning"] = ex.Message;
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult EdiRate(long RateMatrixId, long DVID, long ProductVarientId)
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                Warehouse objW = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (objW.Entity.Trim() == "EVW")
                {
                    if (IsRateEditable(RateMatrixId, objW.ID, DVID))
                    {
                        ProductVarient ProdVar = db.ProductVarients.FirstOrDefault(p => p.ID == ProductVarientId);

                        ProductRateListViewModel obj = new ProductRateListViewModel();
                        Product _product = db.Products.FirstOrDefault(p => p.ID == ProdVar.ProductID);
                        obj.ProductId = _product.ID;
                        obj.HSNCode = _product.HSNCode;
                        obj.ProductName = _product.Name;
                        obj.ProductImgPath = ImageDisplay.SetProductThumbPath(obj.ProductId, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                        List<ProductVarientRateListViewModel> lProductVarient = new List<ProductVarientRateListViewModel>();

                        lProductVarient = db.ProductVarients.Where(pv => pv.ProductID == ProdVar.ProductID && pv.IsActive == true && pv.ID == ProdVar.ID)
                            .Join(db.Sizes.Where(s => s.IsActive == true && s.Name != ""), pv => pv.SizeID, s => s.ID, (pv, s) =>
                                new ProductVarientRateListViewModel
                                {
                                    ProductId = ProdVar.ProductID,
                                    ProductVarientId = pv.ID,
                                    IsEditable = true,
                                    IsFromQuotation = (db.QuotationItemDetails.FirstOrDefault(q => q.ProductID == ProdVar.ProductID && q.ProductVarientID == pv.ID) != null) ? true : false,
                                    VarientName = s.Name,
                                    GSTInPer = (db.RateMatrix.FirstOrDefault(rc => rc.ProductId == ProdVar.ProductID) == null) ? 0 : (db.RateMatrix.FirstOrDefault(rc => rc.ProductId == ProdVar.ProductID).GSTInPer),
                                }).Distinct().ToList();
                        foreach (var item in lProductVarient)
                        {
                            RateMatrix obj_ = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixId);
                            RateMatrixExtension rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.RateMatrixId == RateMatrixId && p.ProductVarientId == ProductVarientId && p.DVID == DVID);
                            item.ID = obj_.ID;
                            item.MRP_ = obj_.MRP;
                            item.PurchaseRate = RoundUp(obj_.BaseInwardPriceEzeelo, 2);
                            item.MRP = obj_.MRP;
                            item.BaseInwardPriceEzeelo = RoundUp(obj_.BaseInwardPriceEzeelo, 2);
                            item.DecidedSalePrice = RoundUp(obj_.DecidedSalePrice, 2);
                            item.Dividend = RoundUp(obj_.Dividend, 2);
                            item.GrossMarginFlat = RoundUp(obj_.GrossMarginFlat, 2);
                            item.GSTInPer = obj_.GSTInPer;
                            item.InwardMarginValue = RoundUp(obj_.InwardMarginValue, 2);
                            item.RateExpiry = obj_.RateExpiry.Date;
                            item.ValuePostGST = RoundUp(obj_.ValuePostGST, 2);
                            item.GSTOnPR = RoundUp(obj_.GSTOnPR, 2);
                            item.BaseInwardPriceEzeelopreGSt = RoundUp(obj_.BaseInwardPriceEzeelo - obj_.GSTOnPR, 2);
                            item.IsEditable = IsRateEditable(RateMatrixId, objW.ID, DVID);
                            item.IsActive = obj_.IsActive;
                            item.MaxInwardMargin = obj_.MaxInwardMargin;
                            item.MarginPassedToCustomer = obj_.MarginPassedToCustomer;
                            item.ActualFlatMargin = obj_.ActualFlatMargin;
                            item.Margin1 = rateMatrixExtension.ForLeadershipPercent;
                            item.Margin2 = rateMatrixExtension.ForEzeeloPercent;
                            item.Margin3 = rateMatrixExtension.ForLeadersRoyaltyPercent;
                            item.Margin4 = rateMatrixExtension.ForLifestyleFundPercent;
                            item.Margin5 = rateMatrixExtension.ForLeadershipDevelopmentFundPercent;
                            item.DVId = DVID;
                            item.DVName = db.Warehouses.FirstOrDefault(p => p.ID == DVID).Name;
                            item.RateMatrixExtensionIsActive = rateMatrixExtension.IsActive;
                            item.RateMatrixExtensionIsActiveModel = false;
                        }
                        obj.VarientList = lProductVarient;

                        ViewBag.LEADERSHIP_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                        ViewBag.EZEELO_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                        ViewBag.LEADERS_ROYALTY_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                        ViewBag.LIFESTYLE_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                        ViewBag.LEADERSHIP_DEVELOPMENT_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);

                        return View(obj);
                    }
                    else
                    {
                        Session["Warning"] = "Rate is in used. You cant Edit.";
                        return RedirectToAction("ProductList");
                    }
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EdiRate(ProductRateListViewModel model)
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (obj.Entity.Trim() == "EVW")
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        try
                        {
                            foreach (var item in model.VarientList)
                            {
                                long RateMatrixId = item.ID;
                                List<RateMatrixExtension> list = db.RateMatrixExtension.Where(p => p.RateMatrixId == RateMatrixId && p.DVID != item.DVId).ToList();
                                if (list.Count() == 0)
                                {
                                    RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixId);
                                    rateMatrix.ProductId = item.ProductId;
                                    rateMatrix.ProductVarientId = item.ProductVarientId;
                                    rateMatrix.MRP = item.MRP;
                                    rateMatrix.GSTInPer = item.GSTInPer;
                                    rateMatrix.GrossMarginFlat = item.GrossMarginFlat;
                                    rateMatrix.DecidedSalePrice = item.DecidedSalePrice;
                                    rateMatrix.ValuePostGST = item.ValuePostGST;
                                    rateMatrix.Dividend = item.Dividend;
                                    rateMatrix.BaseInwardPriceEzeelo = item.BaseInwardPriceEzeelo;
                                    rateMatrix.InwardMarginValue = item.InwardMarginValue;
                                    rateMatrix.GSTOnPR = item.GSTOnPR;
                                    rateMatrix.MaxInwardMargin = item.MaxInwardMargin;
                                    rateMatrix.MarginPassedToCustomer = item.MarginPassedToCustomer;
                                    rateMatrix.ActualFlatMargin = item.ActualFlatMargin;
                                    rateMatrix.DeviceID = "X";
                                    rateMatrix.DeviceType = "X";
                                    rateMatrix.NetworkIP = CommonFunctions.GetClientIP();
                                    rateMatrix.RateExpiry = item.RateExpiry;
                                    rateMatrix.IsActive = true;
                                    rateMatrix.ModifiedBy = WarehouseId;
                                    rateMatrix.ModifiedDate = DateTime.Now;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    RateMatrix rateMatrix = new RateMatrix();
                                    rateMatrix.ProductId = item.ProductId;
                                    rateMatrix.ProductVarientId = item.ProductVarientId;
                                    rateMatrix.MRP = item.MRP;
                                    rateMatrix.GSTInPer = item.GSTInPer;
                                    rateMatrix.GrossMarginFlat = item.GrossMarginFlat;
                                    rateMatrix.DecidedSalePrice = item.DecidedSalePrice;
                                    rateMatrix.ValuePostGST = item.ValuePostGST;
                                    rateMatrix.Dividend = item.Dividend;
                                    rateMatrix.BaseInwardPriceEzeelo = item.BaseInwardPriceEzeelo;
                                    rateMatrix.InwardMarginValue = item.InwardMarginValue;
                                    rateMatrix.GSTOnPR = item.GSTOnPR;
                                    rateMatrix.MaxInwardMargin = item.MaxInwardMargin;
                                    rateMatrix.MarginPassedToCustomer = item.MarginPassedToCustomer;
                                    rateMatrix.ActualFlatMargin = item.ActualFlatMargin;
                                    rateMatrix.DeviceID = "X";
                                    rateMatrix.DeviceType = "X";
                                    rateMatrix.NetworkIP = CommonFunctions.GetClientIP();
                                    rateMatrix.RateExpiry = item.RateExpiry;
                                    rateMatrix.IsActive = true;
                                    rateMatrix.CreatedBy = WarehouseId;
                                    rateMatrix.CreatedDate = DateTime.Now;
                                    db.RateMatrix.Add(rateMatrix);
                                    db.SaveChanges();
                                    RateMatrixId = rateMatrix.ID;
                                }

                                if (item.RateMatrixExtensionIsActiveModel)
                                {
                                    List<RateMatrixExtension> rateCalculationExtension = db.RateMatrixExtension.Where(p => p.RateMatrixId != item.ID && p.DVID == item.DVId && p.ProductVarientId == item.ProductVarientId && p.EVWID == WarehouseId && p.IsActive == true).ToList();
                                    if (rateCalculationExtension.Count != 0)
                                    {
                                        foreach (var itemRE in rateCalculationExtension)
                                        {
                                            RateMatrixExtension objRe = db.RateMatrixExtension.FirstOrDefault(p => p.ID == itemRE.ID);
                                            if (objRe != null)
                                            {
                                                objRe.IsActive = false;
                                                objRe.ModifyBy = WarehouseId;
                                                objRe.ModifyDate = DateTime.Now;
                                                db.RateMatrixExtension.Attach(objRe);
                                                db.Entry(objRe).Property(x => x.IsActive).IsModified = true;
                                                db.Entry(objRe).Property(x => x.ModifyBy).IsModified = true;
                                                db.Entry(objRe).Property(x => x.ModifyDate).IsModified = true;
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                List<Warehouse> FVList = db.Warehouses.Where(p => p.DistributorId == item.DVId).ToList();
                                //Loop through Each FV
                                foreach (var fv in FVList)
                                {
                                    RateMatrixExtension rateCalculationExtension = db.RateMatrixExtension.FirstOrDefault(p => p.RateMatrixId == item.ID && p.FVID == fv.ID && p.DVID == item.DVId && p.ProductVarientId == item.ProductVarientId && p.EVWID == WarehouseId);
                                    FVRateMarginListViewModel fVRateMarginListViewModel = CalculateForRateMatrixExtension(fv.ID, item.ProductId, item.ProductVarientId, item.Margin1, item.Margin2, item.Margin3, item.Margin4, item.Margin5, item.GSTInPer, item.BaseInwardPriceEzeelo, item.DecidedSalePrice, item.GSTOnPR, (int)rateCalculationExtension.OneBPInPaise);

                                    rateCalculationExtension.ProductVarientId = item.ProductVarientId;
                                    rateCalculationExtension.RateMatrixId = RateMatrixId;
                                    rateCalculationExtension.EVWID = obj.ID;

                                    rateCalculationExtension.FVID = fVRateMarginListViewModel.Id;
                                    rateCalculationExtension.FVMargin = Math.Round(fVRateMarginListViewModel.FVMargin, 2);
                                    rateCalculationExtension.FVPurchasePrice = Math.Round(fVRateMarginListViewModel.FVPurchasePrice, 4);
                                    rateCalculationExtension.FVSalePrice = Math.Round(fVRateMarginListViewModel.FVSalePrice, 4);
                                    rateCalculationExtension.FVMarginValueWithGST = Math.Round(fVRateMarginListViewModel.FVMarginVAlueWithGST, 2);
                                    rateCalculationExtension.FVGST = Math.Round(fVRateMarginListViewModel.FVGST, 2);

                                    rateCalculationExtension.DVID = fVRateMarginListViewModel.DVId;
                                    rateCalculationExtension.DVMargin = Math.Round(fVRateMarginListViewModel.DVMargin, 2);
                                    rateCalculationExtension.DVPurchasePrice = Math.Round(fVRateMarginListViewModel.DVPurchasePrice, 4);
                                    rateCalculationExtension.DVSalePrice = Math.Round(fVRateMarginListViewModel.DVSalePrice, 4);
                                    rateCalculationExtension.DVMarginValueWithGST = Math.Round(fVRateMarginListViewModel.DVMarginVAlueWithGST, 2);
                                    rateCalculationExtension.DVGST = Math.Round(fVRateMarginListViewModel.DVGST, 2);

                                    rateCalculationExtension.MarginLeftForEzeeloBeforeLeadershipPayout = Math.Round(fVRateMarginListViewModel.EzeeloMargin, 4);
                                    rateCalculationExtension.GSTForEzeeloMargin = Math.Round(fVRateMarginListViewModel.EzeeloGST, 2);
                                    rateCalculationExtension.PostGSTMargin = Math.Round(fVRateMarginListViewModel.PostGSTMargin, 2);
                                    rateCalculationExtension.ForLeadershipPercent = fVRateMarginListViewModel.ForLeadershipPer;
                                    rateCalculationExtension.ForLeadershipValue = Math.Round(fVRateMarginListViewModel.ForLeadership, 4);
                                    rateCalculationExtension.ForEzeeloPercent = fVRateMarginListViewModel.ForEzeeloPer;
                                    rateCalculationExtension.ForEzeeloValue = Math.Round(fVRateMarginListViewModel.ForEzeelo, 4);
                                    rateCalculationExtension.ForLeadersRoyaltyPercent = fVRateMarginListViewModel.ForLeadersRoyaltyPer;
                                    rateCalculationExtension.ForLeadersRoyaltyValue = Math.Round(fVRateMarginListViewModel.ForLeadersRoyalty, 4);
                                    rateCalculationExtension.ForLifestyleFundPercent = fVRateMarginListViewModel.ForLifestylePer;
                                    rateCalculationExtension.ForLifestyleFundValue = Math.Round(fVRateMarginListViewModel.ForLifestyle, 4);
                                    rateCalculationExtension.ForLeadershipDevelopmentFundPercent = fVRateMarginListViewModel.ForLeadershipDevelopmentPer;
                                    rateCalculationExtension.ForLeadershipDevelopmentFundValue = Math.Round(fVRateMarginListViewModel.ForLeadershipDevelopment, 4);
                                    rateCalculationExtension.TotalGSTInSupplyChain = Math.Round(fVRateMarginListViewModel.TotalGST, 2);
                                    rateCalculationExtension.TotalMargin = Math.Round(fVRateMarginListViewModel.TotalMargin, 2);
                                    rateCalculationExtension.OneBPInPaise = rateCalculationExtension.OneBPInPaise;
                                    rateCalculationExtension.RetailPoint = fVRateMarginListViewModel.BussinessPoints;
                                    rateCalculationExtension.ModifyDate = DateTime.Now;
                                    rateCalculationExtension.ModifyBy = obj.ID;
                                    rateCalculationExtension.NetworkIP = CommonFunctions.GetClientIP();
                                    if (item.RateMatrixExtensionIsActiveModel)
                                    {
                                        rateCalculationExtension.IsActive = true;
                                    }
                                    db.SaveChanges();
                                }
                                transactionScope.Complete();
                                transactionScope.Dispose();
                                Session["Success"] = "Rate update successfully!";
                                return RedirectToAction("DVFVCurrentRateMarginList", new { ProductId = model.ProductId });
                            }

                        }
                        catch (TransactionException ex)
                        {
                            transactionScope.Dispose();
                            Session["Error"] = ex.Message;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    return View();
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Warning"] = ex.Message;
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult DVFVRateMarginList(long ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (obj.Entity.Trim() == "EVW")
                {
                    List<ProductRateMarginListViewModel> ProductList = GetListOfDVFVRateMargin(ProductId);
                    ViewBag.ProductId = ProductId;
                    ViewBag.LEADERSHIP = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                    ViewBag.EZEELO = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                    ViewBag.LEADERS_ROYALTY = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                    ViewBag.LIFESTYLE_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                    ViewBag.LEADERSHIP_DEVELOPMENT_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);

                    ViewBag.LEADERSHIP_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                    ViewBag.EZEELO_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                    ViewBag.LEADERS_ROYALTY_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                    ViewBag.LIFESTYLE_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                    ViewBag.LEADERSHIP_DEVELOPMENT_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);


                    ViewBag.Paise = objMagDiv.BPInPaise();

                    return View(ProductList);
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DVFVCurrentRateMarginList(long ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (obj.Entity.Trim() == "EVW")
                {
                    List<ProductRateMarginListViewModel> ProductList = GetCurrentListOfDVFVRateMargin(ProductId);
                    ViewBag.ProductId = ProductId;
                    ViewBag.LEADERSHIP = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                    ViewBag.EZEELO = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                    ViewBag.LEADERS_ROYALTY = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                    ViewBag.LIFESTYLE_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                    ViewBag.LEADERSHIP_DEVELOPMENT_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);

                    ViewBag.LEADERSHIP_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                    ViewBag.EZEELO_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                    ViewBag.LEADERS_ROYALTY_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                    ViewBag.LIFESTYLE_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                    ViewBag.LEADERSHIP_DEVELOPMENT_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);


                    ViewBag.Paise = objMagDiv.BPInPaise();

                    return View(ProductList);
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DVCurrentRateList(long ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
                Warehouse obj = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
                if (obj.Entity.Trim() == "DV")
                {
                    List<ProductRateMarginListViewModel> ProductList = GetCurrentListOfDVRate(ProductId, WarehouseId);
                    ViewBag.ProductId = ProductId;
                    ViewBag.LEADERSHIP = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                    ViewBag.EZEELO = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                    ViewBag.LEADERS_ROYALTY = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                    ViewBag.LIFESTYLE_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                    ViewBag.LEADERSHIP_DEVELOPMENT_FUND = objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);

                    ViewBag.LEADERSHIP_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP);
                    ViewBag.EZEELO_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.EZEELO);
                    ViewBag.LEADERS_ROYALTY_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY);
                    ViewBag.LIFESTYLE_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND);
                    ViewBag.LEADERSHIP_DEVELOPMENT_FUND_Name = objMagDiv.GetMarginDivisionName((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND);


                    ViewBag.Paise = objMagDiv.BPInPaise();

                    return View(ProductList);
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult FVCurrentRateList(long ProductId)
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
                    long dvid = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID).DistributorId.Value;
                    ProductList = GetCurrentListOfFVRate(ProductId, dvid, WarehouseID);
                }
            }
            return View(ProductList);
        }

        public List<ProductRateMarginListViewModel> GetListOfDVFVRateMargin(long ProductId)
        {
            string WarehouseIdCollection = Session["WarehouseIdCollection"].ToString();
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            long EVWId = Convert.ToInt64(Session["WarehouseId"]);
            DateTime CurDate = DateTime.Now.Date;
            List<ProductRateMarginListViewModel> ProductList = new List<ProductRateMarginListViewModel>();

            ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                       .Select(p => new ProductRateMarginListViewModel
                                       {
                                           HSNCode = p.HSNCode,
                                           ProductId = p.ID,
                                           ProductName = p.Name,
                                           PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                           .Join(db.RateMatrix.Where(r => r.RateExpiry >= CurDate && r.CreatedBy == EVWId).OrderByDescending(r => r.CreatedDate), v => v.ID, r => r.ProductVarientId,
                                           (v, r) => new ProductVarientRateMarginListViewModel
                                           {
                                               RateMatrixId = r.ID,
                                               ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                               ProductVarientId = v.ID,
                                               RateExpiry = r.RateExpiry,
                                               MRP = r.MRP,
                                               GST = r.GSTInPer,
                                               PurchasePrice = r.BaseInwardPriceEzeelo,
                                               GrossMarginFlat = r.GrossMarginFlat,
                                               ValuePostGST = r.ValuePostGST,
                                               MarginPassedToCustomer = r.MarginPassedToCustomer,
                                               MaxInwardMargin = r.MaxInwardMargin,
                                               ActualFlatMargin = r.ActualFlatMargin,
                                               DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID != EzeeloWarehouseId && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == EVWId && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID)))
                                                        .Select(w => new DVRateMarginListViewModel
                                                        {
                                                            Id = w.ID,
                                                            Name = w.Name,
                                                            IsFV = w.IsFulfillmentCenter,
                                                            FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true)
                                                                     .Select(f => new FVRateMarginListViewModel
                                                                     {
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
                        //loop through each FV in DV
                        foreach (var FV in DV.FVList)
                        {
                            RateMatrixExtension obj_rateCalculationExtension = db.RateMatrixExtension.FirstOrDefault(p => p.RateMatrixId == Varient.RateMatrixId && p.ProductVarientId == Varient.ProductVarientId && p.EVWID == EVWId && p.DVID == DV.Id && p.FVID == FV.Id);
                            //Display Rate from Database
                            if (obj_rateCalculationExtension != null)
                            {
                                DV.IsEditable = IsRateEditable(obj_rateCalculationExtension.RateMatrixId, EVWId, DV.Id);
                                FV.DVGST = Math.Round(obj_rateCalculationExtension.DVGST, 2);
                                FV.DVId = obj_rateCalculationExtension.DVID;
                                FV.DVMargin = Math.Round(obj_rateCalculationExtension.DVMargin, 2);
                                FV.DVMarginVAlueWithGST = Math.Round(obj_rateCalculationExtension.DVMarginValueWithGST, 2);
                                FV.DVPurchasePrice = Math.Round(obj_rateCalculationExtension.DVPurchasePrice, 4);
                                FV.DVSalePrice = Math.Round(obj_rateCalculationExtension.DVSalePrice, 4);

                                FV.FVGST = Math.Round(obj_rateCalculationExtension.FVGST, 2);
                                FV.FVMargin = Math.Round(obj_rateCalculationExtension.FVMargin, 2);//
                                FV.FVMarginVAlueWithGST = Math.Round(obj_rateCalculationExtension.FVMarginValueWithGST, 2);
                                FV.FVPurchasePrice = Math.Round(obj_rateCalculationExtension.FVPurchasePrice, 4);
                                FV.FVSalePrice = Math.Round(obj_rateCalculationExtension.FVSalePrice, 4);
                                FV.IsFV = true;

                                FV.ForLeadershipPer = obj_rateCalculationExtension.ForLeadershipPercent;
                                FV.ForEzeeloPer = obj_rateCalculationExtension.ForEzeeloPercent;
                                FV.ForLeadersRoyaltyPer = obj_rateCalculationExtension.ForLeadersRoyaltyPercent;
                                FV.ForLifestylePer = obj_rateCalculationExtension.ForLifestyleFundPercent;
                                FV.ForLeadershipDevelopmentPer = obj_rateCalculationExtension.ForLeadershipDevelopmentFundPercent;

                                FV.EzeeloMargin = Math.Round(obj_rateCalculationExtension.MarginLeftForEzeeloBeforeLeadershipPayout, 4);
                                FV.EzeeloGST = Math.Round(obj_rateCalculationExtension.GSTForEzeeloMargin, 2);
                                FV.PostGSTMargin = Math.Round(obj_rateCalculationExtension.PostGSTMargin, 2);
                                FV.ForLeadership = Math.Round(obj_rateCalculationExtension.ForLeadershipValue, 4);
                                FV.ForEzeelo = Math.Round(obj_rateCalculationExtension.ForEzeeloValue, 4);
                                FV.ForLeadersRoyalty = Math.Round(obj_rateCalculationExtension.ForLeadersRoyaltyValue, 4);
                                FV.ForLifestyle = Math.Round(obj_rateCalculationExtension.ForLifestyleFundValue, 4);
                                FV.ForLeadershipDevelopment = Math.Round(obj_rateCalculationExtension.ForLeadershipDevelopmentFundValue, 4);
                                FV.BussinessPoints = obj_rateCalculationExtension.RetailPoint;
                                FV.TotalGST = Math.Round(obj_rateCalculationExtension.TotalGSTInSupplyChain, 2);
                                FV.TotalMargin = Math.Round(obj_rateCalculationExtension.TotalMargin, 2);
                                FV.IsActive = obj_rateCalculationExtension.IsActive;
                            }
                        }
                    }
                }
            }
            return ProductList;
        }

        public List<ProductRateMarginListViewModel> GetCurrentListOfDVFVRateMargin(long ProductId)
        {
            string WarehouseIdCollection = Session["WarehouseIdCollection"].ToString();
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            long EVWId = Convert.ToInt64(Session["WarehouseId"]);
            DateTime CurDate = DateTime.Now.Date;
            List<ProductRateMarginListViewModel> ProductList = new List<ProductRateMarginListViewModel>();

            ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                       .Select(p => new ProductRateMarginListViewModel
                                       {
                                           HSNCode = p.HSNCode,
                                           ProductId = p.ID,
                                           ProductName = p.Name,
                                           PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                           .Select(v => new ProductVarientRateMarginListViewModel
                                           {
                                               ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                               ProductVarientId = v.ID,
                                               DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID != EzeeloWarehouseId && (db.EVWsDVs.Where(e => e.WarehouseId_EVW == EVWId && e.IsActive == true).Select(e => e.WarehouseId).Contains(w.ID)))
                                                        .Select(w => new DVRateMarginListViewModel
                                                        {
                                                            Id = w.ID,
                                                            Name = w.Name,
                                                            IsFV = w.IsFulfillmentCenter,
                                                            FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true)
                                                                     .Select(f => new FVRateMarginListViewModel
                                                                     {
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
                        //loop through each FV in DV
                        foreach (var FV in DV.FVList)
                        {
                            RateMatrixExtension obj_rateCalculationExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ProductVarientId == Varient.ProductVarientId && p.EVWID == EVWId && p.DVID == DV.Id && p.FVID == FV.Id && p.IsActive);
                            //Display Rate from Database
                            if (obj_rateCalculationExtension != null)
                            {
                                RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(r => r.ID == obj_rateCalculationExtension.RateMatrixId);
                                DV.IsEditable = IsRateEditable(obj_rateCalculationExtension.RateMatrixId, EVWId, DV.Id);
                                DV.RateMatrixId = rateMatrix.ID;
                                DV.RateExpiry = rateMatrix.RateExpiry;
                                DV.MRP = rateMatrix.MRP;
                                DV.GST = rateMatrix.GSTInPer;
                                DV.EzPurchasePrice = rateMatrix.BaseInwardPriceEzeelo;
                                DV.GrossMarginFlat = rateMatrix.GrossMarginFlat;
                                DV.ValuePostGST = rateMatrix.ValuePostGST;
                                DV.MarginPassedToCustomer = rateMatrix.MarginPassedToCustomer;
                                DV.MaxInwardMargin = rateMatrix.MaxInwardMargin;
                                DV.ActualFlatMargin = rateMatrix.ActualFlatMargin;

                                FV.DVGST = Math.Round(obj_rateCalculationExtension.DVGST, 2);
                                FV.DVId = obj_rateCalculationExtension.DVID;
                                FV.DVMargin = Math.Round(obj_rateCalculationExtension.DVMargin, 2);
                                FV.DVMarginVAlueWithGST = Math.Round(obj_rateCalculationExtension.DVMarginValueWithGST, 2);
                                FV.DVPurchasePrice = Math.Round(obj_rateCalculationExtension.DVPurchasePrice, 4);
                                FV.DVSalePrice = Math.Round(obj_rateCalculationExtension.DVSalePrice, 4);

                                FV.FVGST = Math.Round(obj_rateCalculationExtension.FVGST, 2);
                                FV.FVMargin = Math.Round(obj_rateCalculationExtension.FVMargin, 2);//
                                FV.FVMarginVAlueWithGST = Math.Round(obj_rateCalculationExtension.FVMarginValueWithGST, 2);
                                FV.FVPurchasePrice = Math.Round(obj_rateCalculationExtension.FVPurchasePrice, 4);
                                FV.FVSalePrice = Math.Round(obj_rateCalculationExtension.FVSalePrice, 4);
                                FV.IsFV = true;

                                FV.ForLeadershipPer = obj_rateCalculationExtension.ForLeadershipPercent;
                                FV.ForEzeeloPer = obj_rateCalculationExtension.ForEzeeloPercent;
                                FV.ForLeadersRoyaltyPer = obj_rateCalculationExtension.ForLeadersRoyaltyPercent;
                                FV.ForLifestylePer = obj_rateCalculationExtension.ForLifestyleFundPercent;
                                FV.ForLeadershipDevelopmentPer = obj_rateCalculationExtension.ForLeadershipDevelopmentFundPercent;

                                FV.EzeeloMargin = Math.Round(obj_rateCalculationExtension.MarginLeftForEzeeloBeforeLeadershipPayout, 4);
                                FV.EzeeloGST = Math.Round(obj_rateCalculationExtension.GSTForEzeeloMargin, 2);
                                FV.PostGSTMargin = Math.Round(obj_rateCalculationExtension.PostGSTMargin, 2);
                                FV.ForLeadership = Math.Round(obj_rateCalculationExtension.ForLeadershipValue, 4);
                                FV.ForEzeelo = Math.Round(obj_rateCalculationExtension.ForEzeeloValue, 4);
                                FV.ForLeadersRoyalty = Math.Round(obj_rateCalculationExtension.ForLeadersRoyaltyValue, 4);
                                FV.ForLifestyle = Math.Round(obj_rateCalculationExtension.ForLifestyleFundValue, 4);
                                FV.ForLeadershipDevelopment = Math.Round(obj_rateCalculationExtension.ForLeadershipDevelopmentFundValue, 4);
                                FV.BussinessPoints = obj_rateCalculationExtension.RetailPoint;
                                FV.TotalGST = Math.Round(obj_rateCalculationExtension.TotalGSTInSupplyChain, 2);
                                FV.TotalMargin = Math.Round(obj_rateCalculationExtension.TotalMargin, 2);
                                FV.IsActive = obj_rateCalculationExtension.IsActive;
                            }
                        }
                    }
                }
            }
            return ProductList;
        }

        public List<ProductRateMarginListViewModel> GetCurrentListOfDVRate(long ProductId, long DvId)
        {
            long EVWId = 0;
            EVWsDV evw = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == DvId && p.IsActive == true);
            if (evw != null)
            {
                EVWId = evw.WarehouseId_EVW;
            }
            DateTime CurDate = DateTime.Now.Date;
            List<ProductRateMarginListViewModel> ProductList = new List<ProductRateMarginListViewModel>();

            ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                       .Select(p => new ProductRateMarginListViewModel
                                       {
                                           HSNCode = p.HSNCode,
                                           ProductId = p.ID,
                                           ProductName = p.Name,
                                           PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                           .Select(v => new ProductVarientRateMarginListViewModel
                                           {
                                               ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                               ProductVarientId = v.ID,
                                               DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID == DvId)
                                                        .Select(w => new DVRateMarginListViewModel
                                                        {
                                                            Id = w.ID,
                                                            Name = w.Name,
                                                            IsFV = w.IsFulfillmentCenter,
                                                            FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true)
                                                                     .Select(f => new FVRateMarginListViewModel
                                                                     {
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
                        //loop through each FV in DV
                        foreach (var FV in DV.FVList)
                        {
                            RateMatrixExtension obj_rateCalculationExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ProductVarientId == Varient.ProductVarientId && p.EVWID == EVWId && p.DVID == DV.Id && p.FVID == FV.Id && p.IsActive);
                            //Display Rate from Database
                            if (obj_rateCalculationExtension != null)
                            {
                                RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(r => r.ID == obj_rateCalculationExtension.RateMatrixId);
                                DV.RateExpiry = rateMatrix.RateExpiry;
                                DV.MRP = rateMatrix.MRP;
                                DV.GST = rateMatrix.GSTInPer;

                                FV.DVGST = Math.Round(obj_rateCalculationExtension.DVGST, 2);
                                FV.DVId = obj_rateCalculationExtension.DVID;
                                FV.DVMargin = Math.Round(obj_rateCalculationExtension.DVMargin, 2);
                                FV.DVMarginVAlueWithGST = Math.Round(obj_rateCalculationExtension.DVMarginValueWithGST, 2);
                                FV.DVPurchasePrice = Math.Round(obj_rateCalculationExtension.DVPurchasePrice, 4);
                                FV.DVSalePrice = Math.Round(obj_rateCalculationExtension.DVSalePrice, 4);

                                FV.FVGST = Math.Round(obj_rateCalculationExtension.FVGST, 2);
                                FV.FVMargin = Math.Round(obj_rateCalculationExtension.FVMargin, 2);//
                                FV.FVMarginVAlueWithGST = Math.Round(obj_rateCalculationExtension.FVMarginValueWithGST, 2);
                                FV.FVPurchasePrice = Math.Round(obj_rateCalculationExtension.FVPurchasePrice, 4);//
                                FV.FVSalePrice = Math.Round(obj_rateCalculationExtension.FVSalePrice, 4);//
                                FV.IsFV = true;
                            }
                        }
                    }
                }
            }
            return ProductList;
        }

        public List<ProductRateMarginListViewModel> GetCurrentListOfFVRate(long ProductId, long DvId, long FvId)
        {
            long EVWId = 0;
            EVWsDV evw = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == DvId && p.IsActive == true);
            if (evw != null)
            {
                EVWId = evw.WarehouseId_EVW;
            }
            DateTime CurDate = DateTime.Now.Date;
            List<ProductRateMarginListViewModel> ProductList = new List<ProductRateMarginListViewModel>();

            ProductList = db.Products.Where(p => p.IsActive == true && p.ID == ProductId)
                                       .Select(p => new ProductRateMarginListViewModel
                                       {
                                           HSNCode = p.HSNCode,
                                           ProductId = p.ID,
                                           ProductName = p.Name,
                                           PVarientList = db.ProductVarients.Where(v => v.ProductID == p.ID && v.IsActive == true)
                                           .Select(v => new ProductVarientRateMarginListViewModel
                                           {
                                               ProductVarientName = db.Sizes.FirstOrDefault(s => s.ID == v.SizeID && s.IsActive == true).Name,
                                               ProductVarientId = v.ID,
                                               DVList = db.Warehouses.Where(w => w.IsActive == true && w.IsFulfillmentCenter == false && w.ID == DvId)
                                                        .Select(w => new DVRateMarginListViewModel
                                                        {
                                                            Id = w.ID,
                                                            Name = w.Name,
                                                            IsFV = w.IsFulfillmentCenter,
                                                            FVList = db.Warehouses.Where(f => f.DistributorId == w.ID && f.IsFulfillmentCenter == true && f.ID == FvId)
                                                                     .Select(f => new FVRateMarginListViewModel
                                                                     {
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
                        //loop through each FV in DV
                        foreach (var FV in DV.FVList)
                        {
                            RateMatrixExtension obj_rateCalculationExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ProductVarientId == Varient.ProductVarientId && p.EVWID == EVWId && p.DVID == DV.Id && p.FVID == FV.Id && p.IsActive);
                            //Display Rate from Database
                            if (obj_rateCalculationExtension != null)
                            {
                                RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(r => r.ID == obj_rateCalculationExtension.RateMatrixId);

                                Varient.RateExpiry = rateMatrix.RateExpiry;
                                Varient.MRP = rateMatrix.MRP;
                                Varient.GST = rateMatrix.GSTInPer;
                                Varient.SalePrice = obj_rateCalculationExtension.FVSalePrice;
                                Varient.PurchasePrice = obj_rateCalculationExtension.FVPurchasePrice;
                            }
                        }
                    }
                }
            }
            return ProductList;
        }

        public int DeactiveRate(long ProductId, long ProductVarientId, List<Warehouse> DVList, long EVWId)
        {
            try
            {
                foreach (var dv in DVList)
                {
                    List<RateMatrixExtension> list = db.RateMatrixExtension.Where(r => r.DVID == dv.ID && r.ProductVarientId == ProductVarientId && r.EVWID == EVWId && r.IsActive == true).ToList();
                    foreach (var item in list)
                    {
                        RateMatrixExtension objRE = db.RateMatrixExtension.FirstOrDefault(r => r.ID == item.ID && r.IsActive == true);
                        if (objRE != null)
                        {
                            objRE.IsActive = false;
                            objRE.ModifyBy = EVWId;
                            objRE.ModifyDate = DateTime.Now;
                            objRE.NetworkIP = CommonFunctions.GetClientIP();
                            db.RateMatrixExtension.Attach(objRE);
                            db.Entry(objRE).Property(x => x.IsActive).IsModified = true;
                            db.Entry(objRE).Property(x => x.ModifyBy).IsModified = true;
                            db.Entry(objRE).Property(x => x.ModifyDate).IsModified = true;
                            db.Entry(objRE).Property(x => x.NetworkIP).IsModified = true;
                            db.SaveChanges();
                        }
                    }

                }
                return 1;
            }
            catch
            {
                return 0;
            }

        }

        public int BPInPaise()
        {
            return 10;
        }

        public FVRateMarginListViewModel CalculateForRateMatrixExtension(long FVWarehouseId, long ProductId, long ProductVarientId, double txtMrgn1, double txtMrgn2, double txtMrgn3, double txtMrgn4, double txtMrgn5, double GSTInPer, double BaseInwardPriceEzeelo, double DecidedSalePrice, double GSTOnPR, int BPinPaise)
        {
            FVRateMarginListViewModel FV = new FVRateMarginListViewModel();
            Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == FVWarehouseId);
            if (objWarehouse != null)
            {
                FV.Id = objWarehouse.ID;
                FV.IsFV = objWarehouse.IsFulfillmentCenter;
                FV.FVMargin = objWarehouse.Margin == null ? 0 : Convert.ToDouble(objWarehouse.Margin);
                FV.DVId = objWarehouse.DistributorId == null ? 0 : Convert.ToInt64(objWarehouse.DistributorId);
                FV.DVMargin = Convert.ToDouble(db.Warehouses.FirstOrDefault(p => p.ID == FV.DVId).Margin);

                DateTime CurrentDate = DateTime.Now.Date;
                FV.FVSalePrice = RoundUp(DecidedSalePrice, 2);
                FV.FVPurchasePrice = RoundUp((FV.FVSalePrice / (1 + FV.FVMargin / 100)), 2);
                FV.FVMarginVAlueWithGST = RoundUp(FV.FVSalePrice - FV.FVPurchasePrice, 2);
                FV.FVMarginVAlueWithGST = RoundUp(FV.FVMarginVAlueWithGST, 2);
                FV.FVGST = RoundUp((FV.FVMarginVAlueWithGST - (FV.FVMarginVAlueWithGST / (1 + GSTInPer / 100))), 2);

                FV.DVSalePrice = RoundUp(FV.FVPurchasePrice, 2);
                FV.DVPurchasePrice = RoundUp((FV.DVSalePrice / (1 + FV.DVMargin / 100)), 2);
                FV.DVMarginVAlueWithGST = RoundUp(FV.DVSalePrice - FV.DVPurchasePrice, 2);
                FV.DVGST = RoundUp((FV.DVMarginVAlueWithGST - (FV.DVMarginVAlueWithGST / (1 + GSTInPer / 100))), 2);

                FV.EzeeloMargin = RoundUpNegative(FV.DVPurchasePrice - BaseInwardPriceEzeelo, 2);
                FV.EzeeloGST = RoundUp((FV.EzeeloMargin - (FV.EzeeloMargin / (1 + GSTInPer / 100))), 2);
                FV.PostGSTMargin = RoundUp(FV.EzeeloMargin - FV.EzeeloGST, 2);

                FV.ForLeadershipPer = txtMrgn1;
                FV.ForEzeeloPer = txtMrgn2;
                FV.ForLeadersRoyaltyPer = txtMrgn3;
                FV.ForLifestylePer = txtMrgn4;
                FV.ForLeadershipDevelopmentPer = txtMrgn5;

                FV.ForLeadership = RoundUp(FV.PostGSTMargin * (FV.ForLeadershipPer / 100), 2);
                FV.ForEzeelo = RoundUp(FV.PostGSTMargin * (FV.ForEzeeloPer / 100), 2);
                FV.ForLeadersRoyalty = RoundUp(FV.PostGSTMargin * (FV.ForLeadersRoyaltyPer / 100), 2);
                FV.ForLifestyle = RoundUp(FV.PostGSTMargin * (FV.ForLifestylePer / 100), 2);
                FV.ForLeadershipDevelopment = RoundUp(FV.PostGSTMargin * (FV.ForLeadershipDevelopmentPer / 100), 2);
                FV.BussinessPoints = objMagDiv.RoundOfBP(FV.ForLeadership * BPinPaise);
                FV.TotalGST = RoundUp(FV.EzeeloGST + FV.DVGST + FV.FVGST + GSTOnPR, 2);
                FV.TotalMargin = RoundUp(FV.FVMarginVAlueWithGST + FV.DVMarginVAlueWithGST + FV.EzeeloMargin, 2); //Yashaswi 6/6/2018

            }
            return FV;
        }

        public static double RoundUp(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            double Result = Math.Ceiling(input * multiplier) / multiplier;
            if (Result < 0)
            {
                Result = 0;
            }
            return Result;
        }

        public static double RoundUpNegative(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            double Result = Math.Ceiling(input * multiplier) / multiplier;
            return Result;
        }

        [HttpGet]
        public JsonResult AutoComplete(string term)
        {
            term = term.Replace("=", "+");
            List<AutoSuggestViewModel> lSearchList = new List<AutoSuggestViewModel>();
            var query = (from p in db.Products
                         join sp in db.ShopProducts on p.ID equals sp.ProductID
                         join s in db.Shops on sp.ShopID equals s.ID
                         join f in db.Franchises on s.FranchiseID equals f.ID
                         where p.IsActive == true
                         && sp.IsActive == true && s.IsActive == true && f.IsActive == true && p.Name.Contains(term)
                         select new { Name = p.Name, ID = p.ID }).Distinct().Take(15);

            lSearchList = query.ToList().Select(p => new AutoSuggestViewModel
            {
                ID = p.ID.ToString(),
                Name = p.Name
            }).ToList();
            return Json(lSearchList, JsonRequestBehavior.AllowGet);
        }

        public bool IsRateEditable(long RateMatrixId, long EVWId, long DVId)
        {
            try
            {
                bool result = true;
                List<Warehouse> FvList = db.Warehouses.Where(p => p.IsActive == true && p.IsFulfillmentCenter == true && p.DistributorId == DVId).ToList();
                var ids = FvList.Select(s => s.ID).ToList();
                var resultData = db.PurchaseOrders.Where(p => p.WarehouseID == EVWId || p.WarehouseID == DVId || ids.Contains(p.WarehouseID))
                         .Join(db.PurchaseOrderDetails.Where(p => p.RateMatrixId == RateMatrixId && !(db.PurchaseOrders.Any(q => q.ID == p.PurchaseOrderID && q.IsAcceptedBySupplier == 2))), po => po.ID, pod => pod.PurchaseOrderID, (po, pod) => new
                         {
                             PODId = pod.ID
                         }).ToList();
                if (resultData != null && resultData.Count != 0)
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult GetRateForPO(long ProductVarientId, long SupplierID, long SelectedDvId, long SelectedFvId)
        {
            Rate obj = new Rate();
            long WarehouseId = Convert.ToInt64(Session["WarehouseID"]);
            Warehouse objW = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseId);
            if (objW.Entity.Trim() == "EVW")
            {
                obj = GetRateForPO_EVW(ProductVarientId, SupplierID, SelectedDvId);
            }
            else if (objW.Entity.Trim() == "DV")
            {
                obj = GetRateForPO_DV(ProductVarientId, SupplierID, SelectedDvId, SelectedFvId);
            }
            else if (objW.Entity.Trim() == "FV")
            {
                obj = GetRateForPO_FV(ProductVarientId, SupplierID, SelectedDvId, SelectedFvId);
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public Rate GetRateForPO_EVW(long ProductVarientId, long SupplierID, long SelectedDvId)
        {
            Rate obj = new Rate();
            long EVWId = 0;
            EVWsDV evw = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == SelectedDvId && p.IsActive == true);
            if (evw != null)
            {
                EVWId = evw.WarehouseId_EVW;
            }
            RateMatrixExtension rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.DVID == SelectedDvId && p.EVWID == EVWId && p.ProductVarientId == ProductVarientId && p.IsActive == true);
            if (rateMatrixExtension != null)
            {
                long RateMatrixId = rateMatrixExtension.RateMatrixId;
                RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixId);
                if (rateMatrix != null)
                {
                    obj.PurchaseRate = rateMatrix.BaseInwardPriceEzeelo;
                    obj.MRP = rateMatrix.MRP;
                    obj.FlatMargin = rateMatrix.GrossMarginFlat;
                    obj.DecidedSalePrice = rateMatrix.DecidedSalePrice;
                    obj.RateMatrixId = rateMatrix.ID;
                    obj.RateMatrixExtensionId = 0;
                }
            }
            return obj;
        }

        public Rate GetRateForGRN_EVW(long RateMatrixID, long DVId, long FVId, long EVWId)
        {
            Rate obj = new Rate();
            RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixID);
            RateMatrixExtension rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.RateMatrixId == RateMatrixID && p.DVID == DVId && p.FVID == FVId && p.EVWID == EVWId);
            if (rateMatrixExtension != null)
            {
                obj.MRP = rateMatrix.MRP;
                obj.RateMatrixId = rateMatrix.ID;
                obj.RateMatrixExtensionId = rateMatrixExtension.ID;
                obj.SaleRate = rateMatrixExtension.DVPurchasePrice;
                obj.PurchaseRate = rateMatrix.BaseInwardPriceEzeelo;
                obj.GST = rateMatrix.GSTInPer;
                obj.TotalGST = rateMatrix.GSTOnPR;
            }
            return obj;
        }

        public Rate GetRateForGRN_DV(long RateMatrixID, long DVId, long FVId, long EVWId, long RateMatrixExtensionId)
        {
            Rate obj = new Rate();
            RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixID);
            RateMatrixExtension rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.RateMatrixId == RateMatrixID && p.ID == RateMatrixExtensionId);
            if (rateMatrixExtension != null)
            {
                obj.MRP = rateMatrix.MRP;
                obj.RateMatrixId = rateMatrix.ID;
                obj.RateMatrixExtensionId = rateMatrixExtension.ID;
                obj.SaleRate = rateMatrixExtension.DVSalePrice;
                obj.PurchaseRate = rateMatrixExtension.DVPurchasePrice;
                obj.GST = rateMatrix.GSTInPer;
                obj.TotalGST = Math.Round((double)(rateMatrixExtension.DVPurchasePrice - (rateMatrixExtension.DVPurchasePrice / (1 + (double)rateMatrix.GSTInPer / 100))), 2); ;
            }
            return obj;
        }

        public Rate GetRateForGRN_FV(long RateMatrixID, long DVId, long FVId, long EVWId, long RateMatrixExtensionId)
        {
            Rate obj = new Rate();
            RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixID);
            RateMatrixExtension rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.RateMatrixId == RateMatrixID && p.ID == RateMatrixExtensionId);
            if (rateMatrixExtension != null)
            {
                obj.MRP = rateMatrix.MRP;
                obj.RateMatrixId = rateMatrix.ID;
                obj.RateMatrixExtensionId = rateMatrixExtension.ID;
                obj.SaleRate = rateMatrixExtension.FVSalePrice;
                obj.PurchaseRate = rateMatrixExtension.FVPurchasePrice;
                obj.GST = rateMatrix.GSTInPer;
                obj.TotalGST = Math.Round((double)(rateMatrixExtension.FVPurchasePrice - (rateMatrixExtension.FVPurchasePrice / (1 + (double)rateMatrix.GSTInPer / 100))), 2); ;
            }
            return obj;
        }

        public Rate GetRateForPO_DV(long ProductVarientId, long SupplierID, long SelectedDvId, long SelectedFvId)
        {
            RateMatrixExtension rateMatrixExtension = new RateMatrixExtension();
            Rate obj = new Rate();
            long? RateMatrixId = 0;
            long? RateMatrixExtensionId = 0;
            long? SupplierWarehouseID = db.Suppliers.Where(x => x.ID == SupplierID).Select(x => x.WarehouseID).FirstOrDefault();
            if (SupplierWarehouseID != null && SupplierWarehouseID > 0)
            {
                WarehouseStock objStock = db.WarehouseStocks.Where(x => x.WarehouseID == SupplierWarehouseID && x.ProductVarientID == ProductVarientId && x.AvailableQuantity > 0).OrderBy(x => x.ID).OrderBy(x => x.ExpiryDate).FirstOrDefault();
                if (objStock != null && objStock.ID > 0)
                {
                    long PurchaseOrderID = 0;
                    PurchaseOrderID = db.Invoices.Where(x => x.ID == objStock.InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                    PurchaseOrder objPurchaseOrder = db.PurchaseOrders.FirstOrDefault(p => p.ID == PurchaseOrderID && p.DVId == SelectedDvId && p.FVId == SelectedFvId);
                    if (objPurchaseOrder != null)
                    {
                        PurchaseOrderDetail POD = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID
                        && x.ProductVarientID == ProductVarientId).FirstOrDefault();
                        RateMatrixExtensionId = POD.RateMatrixExtensionId;
                    }
                }
            }
            if (RateMatrixExtensionId != 0)
            {
                rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ID == RateMatrixExtensionId);
            }
            else
            {
                long EVWId = 0;
                EVWsDV evw = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == SelectedDvId && p.IsActive == true);
                if (evw != null)
                {
                    EVWId = evw.WarehouseId_EVW;
                }
                rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.DVID == SelectedDvId && p.EVWID == EVWId && p.ProductVarientId == ProductVarientId && p.IsActive == true && p.FVID == SelectedFvId);
            }
            if (rateMatrixExtension != null)
            {
                RateMatrixId = rateMatrixExtension.RateMatrixId;
                RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixId);
                if (rateMatrix != null)
                {
                    obj.PurchaseRate = rateMatrixExtension.DVPurchasePrice;
                    obj.MRP = rateMatrix.MRP;
                    obj.RateMatrixId = rateMatrix.ID;
                    obj.RateMatrixExtensionId = rateMatrixExtension.ID;
                    obj.DecidedSalePrice = 0;
                    obj.FlatMargin = 0;
                }
            }
            return obj;
        }

        public Rate GetRateForPO_FV(long ProductVarientId, long SupplierID, long SelectedDvId, long SelectedFvId)
        {
            RateMatrixExtension rateMatrixExtension = new RateMatrixExtension();
            Rate obj = new Rate();
            long? RateMatrixId = 0;
            long? RateMatrixExtensionId = 0;
            long? SupplierWarehouseID = db.Suppliers.Where(x => x.ID == SupplierID).Select(x => x.WarehouseID).FirstOrDefault();
            if (SupplierWarehouseID != null && SupplierWarehouseID > 0)
            {
                WarehouseStock objStock = db.WarehouseStocks.Where(x => x.WarehouseID == SupplierWarehouseID && x.ProductVarientID == ProductVarientId && x.AvailableQuantity > 0).OrderBy(x => x.ID).OrderBy(x => x.ExpiryDate).FirstOrDefault();
                if (objStock != null && objStock.ID > 0)
                {
                    long PurchaseOrderID = 0;
                    PurchaseOrderID = db.Invoices.Where(x => x.ID == objStock.InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                    PurchaseOrder objPurchaseOrder = db.PurchaseOrders.FirstOrDefault(p => p.ID == PurchaseOrderID && p.DVId == SelectedDvId && p.FVId == SelectedFvId);
                    if (objPurchaseOrder != null)
                    {
                        PurchaseOrderDetail POD = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID
                        && x.ProductVarientID == ProductVarientId).FirstOrDefault();
                        RateMatrixExtensionId = POD.RateMatrixExtensionId;
                    }
                }
            }
            if (RateMatrixExtensionId != 0)
            {
                rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.ID == RateMatrixExtensionId);
            }
            else
            {
                long EVWId = 0;
                EVWsDV evw = db.EVWsDVs.FirstOrDefault(p => p.WarehouseId == SelectedDvId && p.IsActive == true);
                if (evw != null)
                {
                    EVWId = evw.WarehouseId_EVW;
                }
                rateMatrixExtension = db.RateMatrixExtension.FirstOrDefault(p => p.DVID == SelectedDvId && p.EVWID == EVWId && p.ProductVarientId == ProductVarientId && p.IsActive == true && p.FVID == SelectedFvId);
            }
            if (rateMatrixExtension != null)
            {
                RateMatrixId = rateMatrixExtension.RateMatrixId;
                RateMatrix rateMatrix = db.RateMatrix.FirstOrDefault(p => p.ID == RateMatrixId);
                if (rateMatrix != null)
                {
                    obj.PurchaseRate = rateMatrixExtension.FVPurchasePrice;
                    obj.MRP = rateMatrix.MRP;
                    obj.RateMatrixId = rateMatrix.ID;
                    obj.RateMatrixExtensionId = rateMatrixExtension.ID;
                    obj.DecidedSalePrice = 0;
                    obj.FlatMargin = 0;
                }
            }
            return obj;
        }

        public List<RateMatrix> GetCurrentProductRateList(long WarehouseID)
        {
            DateTime CurrentDate = DateTime.Now.Date;

            List<ProductRateListViewModel> lProduct = new List<ProductRateListViewModel>();

            var Entity = db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.Entity).First();
            List<RateMatrixExtension> List_RateMatrixExtension = new List<RateMatrixExtension>();
            List<RateMatrix> List_RateMatrix = new List<RateMatrix>();
            if (Entity == "FV")
            {
                long DVID = Convert.ToInt32(db.Warehouses.Where(x => x.ID == WarehouseID).Select(x => x.DistributorId).First());
                long EVWID = Convert.ToInt32(db.EVWsDVs.Where(x => x.WarehouseId == DVID).Select(x => x.WarehouseId_EVW).First());

                List_RateMatrixExtension = db.RateMatrixExtension.Where(x => x.FVID == WarehouseID && x.DVID == DVID && x.EVWID == EVWID && x.IsActive == true).ToList();

            }
            if (Entity == "DV")
            {
                long EVWID = Convert.ToInt32(db.EVWsDVs.Where(x => x.WarehouseId == WarehouseID).Select(x => x.WarehouseId_EVW).First());

                List_RateMatrixExtension = db.RateMatrixExtension.Where(x => x.DVID == WarehouseID && x.EVWID == EVWID && x.IsActive == true).ToList();

            }
            if (Entity == "EVW")
            {

                List_RateMatrixExtension = db.RateMatrixExtension.Where(x => x.EVWID == WarehouseID && x.IsActive == true).ToList();

            }
            if (List_RateMatrixExtension != null && List_RateMatrixExtension.Count > 0)
            {
                List<long> Ids = List_RateMatrixExtension.Select(x => x.RateMatrixId).Distinct().ToList();
                List_RateMatrix = db.RateMatrix.Where(x => Ids.Contains(x.ID) && x.RateExpiry >= CurrentDate).ToList();
            }

            return List_RateMatrix;
        }
    }
}