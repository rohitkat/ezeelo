using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using System.Web.Configuration;
using System.Data.Entity;
using ModelLayer.Models.Enum;

namespace Inventory.Controllers
{
    public class ProductRateController : Controller
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        Margin_DivisionConstants objMagDiv = new Margin_DivisionConstants(); //Yashaswi 24/4/2018
        public ActionResult CurrentRateProductList(int? Month)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
            List<ProductRateListViewModel> lProduct = new List<ProductRateListViewModel>();
            DateTime CurrentDate = DateTime.Now.Date;
            long WarehouseID = 0;
            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }

            lProduct = GetCurrentProductRateList();

            if (Month == 0 || Month == null)
            {
                Month = DateTime.Now.Date.Month;
            }

            lProduct = lProduct.Where(p => p.QuotationDate.Month == Month).ToList();
            ViewBag.SelectedMonth = Month;
            foreach (var item in lProduct)
            {
                item.ProductImgPath = ImageDisplay.SetProductThumbPath(item.ProductId, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
            }

            return View(lProduct);

        }

        public List<ProductRateListViewModel> GetCurrentProductRateList()
        {
            DateTime CurrentDate = DateTime.Now.Date;

            List<ProductRateListViewModel> lProduct = new List<ProductRateListViewModel>();
            lProduct = db.Products.Join(db.RateCalculations.Where(r => r.IsActive == true && r.RateExpiry >= CurrentDate), p => p.ID, r => r.ProductId,
                              (p, r) => new ProductRateListViewModel
                              {
                                  ProductId = p.ID,
                                  ProductName = p.Name,
                                  HSNCode = p.HSNCode,
                                  IsActive = p.IsActive,
                                  QuotationID = 0,
                                  QuotationDate = CurrentDate, //Not in USed
                                  VarientCount = db.ProductVarients.Where(pv => pv.ProductID == p.ID && p.IsActive == true).Distinct().Count(),
                                  RateInsertDate = (DateTime)(r.ModifiedDate == null ? r.CreatedDate : r.ModifiedDate)
                              })
                              .Distinct().Where(p => p.IsActive == true)
                              .GroupBy(p => p.ProductId)
                              .Select(p => p.ToList().OrderByDescending(m => m.QuotationDate).FirstOrDefault())
                              .OrderByDescending(p => p.RateInsertDate).ThenByDescending(p => p.QuotationDate).ToList()
                              .ToList();
            return lProduct;
        }


        public ActionResult ProductList(int? Month, long? ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
            {
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
                                     }
                                     ).ToList()
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
                        Month = DateTime.Now.Date.Month;
                    }

                    lProduct = lProduct.Where(p => p.QuotationDate.Month == Month).ToList();
                   
                }
                ViewBag.SelectedMonth = Month;
                foreach (var item in lProduct)
                {
                    item.ProductImgPath = ImageDisplay.SetProductThumbPath(item.ProductId, "Default", string.Empty, ProductUpload.IMAGE_TYPE.Approved);
                }

                return View(lProduct);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ProductVarientList(long ProductId)
        {
            if (Session["USER_NAME"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
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
                            //Yashaswi 26/4/2018
                            GSTInPer = (db.RateCalculations.FirstOrDefault(rc => rc.ProductId == ProductId) == null) ? 0 : (db.RateCalculations.FirstOrDefault(rc => rc.ProductId == ProductId).GSTInPer),
                        }).Distinct().ToList();
                foreach (var item in lProductVarient)
                {
                    RateCalculation obj_ = db.RateCalculations.FirstOrDefault(Rc => Rc.ProductId == item.ProductId && Rc.ProductVarientId == item.ProductVarientId && Rc.IsActive == true);

                    if (obj_ != null)
                    {
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
                        item.IsEditable = !(db.PurchaseOrderDetails.Any(p => p.RateCalculationId == item.ID));
                        item.IsActive = obj_.IsActive;
                        item.MaxInwardMargin = obj_.MaxInwardMargin; //Yashaswi 6/6/2018
                        item.MarginPassedToCustomer = obj_.MarginPassedToCustomer; //Yashaswi 6/6/2018
                        item.ActualFlatMargin = obj_.ActualFlatMargin; //Yashaswi 21/6/2018
                        if (item.IsEditable == false)
                        {
                            if (item.RateExpiry < DateTime.Now.Date)
                            {
                                item.IsEditable = true;
                            }
                        }
                    }
                    else
                    {
                        item.MRP_ = 0;
                        item.PurchaseRate = 0;
                        item.IsActive = false;
                        item.IsEditable = true;
                    }


                }


                obj.VarientList = lProductVarient;
                return View(obj);
            }
            else
            {
                Session["Warning"] = "Sorry! You are not authorized user.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ProductVarientList(ProductRateListViewModel model)
        {
            try
            {
                if (Session["USER_NAME"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (Session["IsEzeeloLogin"].ToString() == "1" || Session["BusinessTypeID"].ToString() == "5")
                {
                    if (model != null)
                    {
                        if (model.VarientList.Count() != 0)
                        {
                            foreach (var item in model.VarientList)
                            {
                                if (item != null)
                                {
                                    if (item.checkbox)
                                    {
                                        bool IsInsert = true;
                                        //Save  Rate For Product
                                        //if (item.IsEditable) //Check allowed to save or update Rate
                                        //{
                                        long PersonalDetailID = 1;
                                        if (Session["USER_LOGIN_ID"] != null)
                                        {
                                            long ID = Convert.ToInt32(Session["USER_LOGIN_ID"]);
                                            PersonalDetailID = Convert.ToInt32(db.PersonalDetails.Where(x => x.UserLoginID == ID).Select(x => x.ID).First());
                                        }

                                        DateTime CurDate = DateTime.Now.Date;
                                        RateCalculation objRateCalculation = new RateCalculation();

                                        //Check rate is present in DB or not
                                        RateCalculation objRateCalculation_ = db.RateCalculations.FirstOrDefault(r => r.ID == item.ID);
                                        if (objRateCalculation_ != null)
                                        {
                                            if (objRateCalculation_.RateExpiry < CurDate)
                                            {
                                                IsInsert = true;
                                                DeactiveRate(item.ProductId, item.ProductVarientId);
                                                //Rate is Expired Insert new Rate
                                            }
                                            else
                                            {
                                                objRateCalculation = db.RateCalculations.FirstOrDefault(r => r.ID == item.ID);
                                                IsInsert = false;
                                                //Update Rate
                                            }
                                        }
                                        else
                                        {
                                            DeactiveRate(item.ProductId, item.ProductVarientId);
                                            IsInsert = true;
                                            //Insert New Rate                                               
                                        }
                                        if (item.IsEditable)
                                        {
                                            objRateCalculation.ProductId = item.ProductId;
                                            objRateCalculation.ProductVarientId = item.ProductVarientId;
                                            objRateCalculation.MRP = item.MRP;
                                            objRateCalculation.GSTInPer = item.GSTInPer;
                                            objRateCalculation.GrossMarginFlat = item.GrossMarginFlat;
                                            objRateCalculation.DecidedSalePrice = item.DecidedSalePrice;
                                            objRateCalculation.ValuePostGST = item.ValuePostGST;
                                            objRateCalculation.Dividend = item.Dividend;
                                            objRateCalculation.BaseInwardPriceEzeelo = item.BaseInwardPriceEzeelo;
                                            objRateCalculation.InwardMarginValue = item.InwardMarginValue;
                                            objRateCalculation.GSTOnPR = item.GSTOnPR;
                                            objRateCalculation.MaxInwardMargin = item.MaxInwardMargin;//Yashaswi 6/6/2018
                                            objRateCalculation.MarginPassedToCustomer = item.MarginPassedToCustomer;//Yashaswi 6/6/2018
                                            objRateCalculation.ActualFlatMargin = item.ActualFlatMargin; //Yashaswi 21/6/2018
                                            objRateCalculation.DeviceID = "X";
                                            objRateCalculation.DeviceType = "X";
                                            objRateCalculation.NetworkIP = CommonFunctions.GetClientIP();
                                        }
                                        objRateCalculation.RateExpiry = item.RateExpiry;
                                        objRateCalculation.IsActive = item.IsActive;

                                        if (IsInsert)
                                        {
                                            objRateCalculation.CreatedBy = PersonalDetailID;
                                            objRateCalculation.CreatedDate = DateTime.Now.Date;
                                            db.RateCalculations.Add(objRateCalculation);
                                        }
                                        else
                                        {
                                            objRateCalculation.ModifiedBy = PersonalDetailID;
                                            objRateCalculation.ModifiedDate = DateTime.Now.Date;
                                            db.Entry(objRateCalculation).State = EntityState.Modified;
                                        }
                                        db.SaveChanges();
                                        //}

                                    }
                                }
                            }
                            Session["Success"] = "Rate Saved Successfully.";
                            return RedirectToAction("DVFVRateMarginList", "Report", new { ProductId = model.ProductId });
                        }

                    }
                }
                else
                {
                    Session["Warning"] = "Sorry! You are not authorized user.";
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("ProductList");
            }
            catch (Exception Ex)
            {
                string msg = "Sorry! Problem Occured While Saving Product Rate.  Error : " + Ex.InnerException.InnerException.Message;
                Session["Error"] = msg.Replace("\r\n", "");
                return RedirectToAction("ProductList");
            }
        }

        public void DeactiveRate(long ProductId, long VarientId)
        {
            List<RateCalculation> List_rate = db.RateCalculations.Where(p => p.ProductId == ProductId && p.ProductVarientId == VarientId && p.IsActive == true).ToList();
            foreach (var item in List_rate)
            {
                RateCalculation obj = db.RateCalculations.FirstOrDefault(p => p.ID == item.ID);
                obj.IsActive = false;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Calculation For FV , DV and Bussiness Point
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <param name="ProductId"></param>
        /// <param name="ProductVarientId"></param>
        /// <returns></returns>
        /// //Yashaswi 24/4/2018
        public FVRateMarginListViewModel GetRateMarginForFV(long FVWarehouseId, long ProductId, long ProductVarientId)
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
                RateCalculation obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ProductId == ProductId && r.ProductVarientId == ProductVarientId && r.RateExpiry >= CurrentDate && r.IsActive == true);

                if (obj_Rate != null)
                {

                    FV.FVSalePrice = RoundUp(obj_Rate.DecidedSalePrice, 2);
                    FV.FVPurchasePrice = RoundUp((FV.FVSalePrice / (1 + FV.FVMargin / 100)), 2);
                    FV.FVMarginVAlueWithGST = RoundUp(FV.FVSalePrice - FV.FVPurchasePrice, 2);
                    FV.FVMarginVAlueWithGST = RoundUp(FV.FVMarginVAlueWithGST, 2);
                    FV.FVGST = RoundUp((double)(FV.FVMarginVAlueWithGST - (FV.FVMarginVAlueWithGST / (1 + (double)obj_Rate.GSTInPer / 100))), 2);

                    FV.DVSalePrice = RoundUp(FV.FVPurchasePrice, 2);
                    FV.DVPurchasePrice = RoundUp((FV.DVSalePrice / (1 + FV.DVMargin / 100)), 2);
                    FV.DVMarginVAlueWithGST = RoundUp(FV.DVSalePrice - FV.DVPurchasePrice, 2);
                    FV.DVGST = RoundUp((double)(FV.DVMarginVAlueWithGST - (FV.DVMarginVAlueWithGST / (1 + (double)obj_Rate.GSTInPer / 100))), 2);

                    FV.EzeeloMargin = RoundUpNegative(FV.DVPurchasePrice - obj_Rate.BaseInwardPriceEzeelo, 2); //Yashaswi 6/6/2018
                    //FV.EzeeloMargin = RoundUp(FV.DVPurchasePrice - obj_Rate.BaseInwardPriceEzeelo, 2);
                    FV.EzeeloGST = RoundUp((double)(FV.EzeeloMargin - (FV.EzeeloMargin / (1 + (double)obj_Rate.GSTInPer / 100))), 2);
                    FV.PostGSTMargin = RoundUp(FV.EzeeloMargin - FV.EzeeloGST, 2);
                    FV.ForLeadership = RoundUp(FV.PostGSTMargin * ((double)objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP) / 100), 2);
                    FV.ForEzeelo = RoundUp(FV.PostGSTMargin * ((double)objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.EZEELO) / 100), 2);
                    FV.ForLeadersRoyalty = RoundUp(FV.PostGSTMargin * ((double)objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERS_ROYALTY) / 100), 2);
                    FV.ForLifestyle = RoundUp(FV.PostGSTMargin * ((double)objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LIFESTYLE_FUND) / 100), 2);
                    FV.ForLeadershipDevelopment = RoundUp(FV.PostGSTMargin * ((double)objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP_DEVELOPMENT_FUND) / 100), 2);
                    FV.BussinessPoints = objMagDiv.RoundOfBP(FV.ForLeadership * objMagDiv.BPInPaise());
                    FV.TotalGST = RoundUp(FV.EzeeloGST + FV.DVGST + FV.FVGST + obj_Rate.GSTOnPR, 2);
                    FV.TotalMargin = RoundUp(FV.FVMarginVAlueWithGST + FV.DVMarginVAlueWithGST + FV.EzeeloMargin, 2); //Yashaswi 6/6/2018
                }

            }
            return FV;
        }

        public double GetRateForFV(long WarehouseId, long ProductId, long VarientId, bool IsSalePrice)
        {
            try
            {
                List<CurrentRate> objRate = new List<CurrentRate>();
                Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseId);
                if (objWarehouse != null)
                {
                    long? SupplierWarehouseID = objWarehouse.DistributorId;
                    if (SupplierWarehouseID != null && SupplierWarehouseID > 0)
                    {
                        Nullable<long> RateCalID = 0;
                        WarehouseStock objStock = db.WarehouseStocks.Where(x => x.WarehouseID == SupplierWarehouseID && x.ProductID == ProductId
                            && x.ProductVarientID == VarientId && x.AvailableQuantity > 0).OrderBy(x => x.ID).OrderBy(x => x.ExpiryDate).FirstOrDefault();

                        if (objStock != null && objStock.ID > 0)
                        {

                            long PurchaseOrderID = 0;
                            PurchaseOrderID = db.Invoices.Where(x => x.ID == objStock.InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                            RateCalID = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID && x.ProductID == ProductId
                                && x.ProductVarientID == VarientId).Select(x => x.RateCalculationId).FirstOrDefault();
                            if (RateCalID!=null && RateCalID > 0)
                            {
                                objRate = GetRateForFVPO(WarehouseId, ProductId, VarientId, IsSalePrice, RateCalID);
                            }
                            else
                            {
                                objRate = GetRateForFVPO(WarehouseId, ProductId, VarientId, IsSalePrice, null);
                            }

                        }
                        else
                        {
                            objRate = GetRateForFVPO(WarehouseId, ProductId, VarientId, IsSalePrice, null);
                        }
                    }

                    if (objRate.Count != 0)
                    {
                        return RoundUp(objRate[0].Rate, 2);
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public double GetRateForDV(long WarehouseId, long ProductId, long VarientId, bool IsSalePrice)
        {
            try
            {
                Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseId);
                if (objWarehouse != null)
                {
                    double Margin = 6;   //Default margin 6 to show rate in PO for DV
                    double DVMargin = 0;
                    double SalePrice = 0;
                    double PurchasePrice = 0;
                    double DVSalePrice = 0;
                    double DVPurchasePrice = 0;
                    DateTime CurrentDate = DateTime.Now.Date;
                    DVMargin = objWarehouse.Margin == null ? 0 : Convert.ToDouble(objWarehouse.Margin);
                    RateCalculation obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ProductId == ProductId && r.ProductVarientId == VarientId && r.RateExpiry >= CurrentDate && r.IsActive == true);
                    if (obj_Rate != null)
                    {
                        SalePrice = obj_Rate.DecidedSalePrice;
                        PurchasePrice = (SalePrice / (1 + Margin / 100));

                        DVSalePrice = PurchasePrice;
                        DVPurchasePrice = (DVSalePrice / (1 + DVMargin / 100));

                        if (IsSalePrice)
                        {
                            return RoundUp(DVSalePrice, 2);
                        }
                        else
                        {
                            return RoundUp(DVPurchasePrice, 2);
                        }
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }


        //Added by Zubair for PO on 20-04-2018
        public List<CurrentRate> GetRateForEzeeloPO(long WarehouseId, long ProductId, long VarientId, bool IsSalePrice,long? PORateCalculationID)
        {
            List<CurrentRate> objCR = new List<CurrentRate>();
            try
            {
                Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseId);
                if (objWarehouse != null)
                {
                    long RateCalculationID = 0;
                    double Margin = 6;   //Default margin 6 to show rate in PO for DV
                    double DVMargin = 2; //DV margin fixed to calculate ezeelo sale prize
                    double SalePrice = 0;
                    double PurchasePrice = 0;
                    double DVSalePrice = 0;
                    double EzeeloSalePrice = 0;
                    double EzeeloPurchasePrice = 0;
                    DateTime CurrentDate = DateTime.Now.Date;
                    //DVMargin = objWarehouse.Margin == null ? 0 : Convert.ToDouble(objWarehouse.Margin);

                    //Added by Zubair on 21-04-2018
                    //Get record on PO and Invoice/Delivery Note Entry
                    RateCalculation obj_Rate = new RateCalculation();
                    if (PORateCalculationID != null && Convert.ToInt64(PORateCalculationID) > 0)  //Get on Invoice/Delivery entry time
                    {                        
                        RateCalculationID = Convert.ToInt64(PORateCalculationID);
                        obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ID == RateCalculationID && r.ProductId == ProductId && r.ProductVarientId == VarientId);
                    }
                    else  //Get on Purchase Order creation time
                    {
                         obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ProductId == ProductId && r.ProductVarientId == VarientId && r.RateExpiry >= CurrentDate && r.IsActive == true);
                    }

                        if (obj_Rate != null)
                    {
                        RateCalculationID = obj_Rate.ID;

                        SalePrice = obj_Rate.DecidedSalePrice;
                        PurchasePrice = (SalePrice / (1 + Margin / 100));

                        DVSalePrice = PurchasePrice;
                        EzeeloSalePrice = (DVSalePrice / (1 + DVMargin / 100));
                        EzeeloPurchasePrice = obj_Rate.BaseInwardPriceEzeelo;                       

                        if (IsSalePrice)
                        {
                            CurrentRate obj = new CurrentRate();
                            obj.RateCalculationID = RateCalculationID;
                            obj.Rate = Math.Round(EzeeloSalePrice, 2);
                            obj.MRP = obj_Rate.MRP;
                            obj.DecidedSalePrice = obj_Rate.DecidedSalePrice;   // by Priti for Generating Po
                            obj.GrossMarginFlat = obj_Rate.GrossMarginFlat;              // by Priti for Generating Po
                            obj.TotalGSTInPer = obj_Rate.GSTInPer;
                            obj.TotalGSTAmount = obj_Rate.GSTOnPR;
                            objCR.Add(obj);
                            return objCR;
                        }
                        else
                        {
                            CurrentRate obj = new CurrentRate();
                            obj.RateCalculationID = RateCalculationID;
                            obj.Rate = Math.Round(EzeeloPurchasePrice, 2);
                            obj.MRP = obj_Rate.MRP;
                            obj.DecidedSalePrice = obj_Rate.DecidedSalePrice;            // by Priti for Generating Po
                            obj.GrossMarginFlat = obj_Rate.GrossMarginFlat;               // by Priti for Generating Po
                            obj.TotalGSTInPer = obj_Rate.GSTInPer;
                            obj.TotalGSTAmount = obj_Rate.GSTOnPR;
                            objCR.Add(obj);
                            return objCR;
                        }
                    }
                }
                return objCR;
            }
            catch
            {
                return objCR;
            }
        }

        //Added by Zubair for PO on 20-04-2018
        public List<CurrentRate> GetRateForFVPO(long WarehouseId, long ProductId, long VarientId, bool IsSalePrice, long? PORateCalculationID)
        {
            List<CurrentRate> objCR = new List<CurrentRate>();
            try
            {                
                Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseId);
                if (objWarehouse != null)
                {
                    long RateCalculationID = 0;
                    double Margin = 0;
                    double SalePrice = 0;
                    double PurchasePrice = 0;
                    double amtForGST = 0;
                    DateTime CurrentDate = DateTime.Now.Date;
                    Margin = objWarehouse.Margin == null ? 0 : Convert.ToDouble(objWarehouse.Margin);

                    //Added by Zubair on 21-04-2018
                    //Get record on PO and Invoice/Delivery Note Entry
                    RateCalculation obj_Rate = new RateCalculation();
                    if (PORateCalculationID != null && Convert.ToInt64(PORateCalculationID) > 0)  //Get on Invoice/Delivery entry time
                    {
                        RateCalculationID = Convert.ToInt64(PORateCalculationID);
                        obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ID == RateCalculationID && r.ProductId == ProductId && r.ProductVarientId == VarientId);
                    }
                    else  //Get on Purchase Order creation time
                    {
                        obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ProductId == ProductId && r.ProductVarientId == VarientId && r.RateExpiry >= CurrentDate && r.IsActive == true);
                    }

                    if (obj_Rate != null)
                    {
                        RateCalculationID = obj_Rate.ID;
                        SalePrice = obj_Rate.DecidedSalePrice;

                        PurchasePrice = (SalePrice / (1 + Margin / 100));

                        amtForGST = SalePrice - PurchasePrice;

                        if (IsSalePrice)
                        {
                            CurrentRate obj = new CurrentRate();
                            obj.RateCalculationID = RateCalculationID;
                            obj.Rate = Math.Round(SalePrice, 2);
                            obj.MRP = obj_Rate.MRP;
                            obj.TotalGSTInPer = obj_Rate.GSTInPer;
                            obj.TotalGSTAmount = Math.Round((double)(amtForGST - (amtForGST / (1 + (double)obj_Rate.GSTInPer / 100))),2);
                            objCR.Add(obj);
                            return objCR;
                        }
                        else
                        {
                            CurrentRate obj = new CurrentRate();
                            obj.RateCalculationID = RateCalculationID;
                            obj.Rate = Math.Round(PurchasePrice, 2);
                            obj.MRP = obj_Rate.MRP;
                            obj.TotalGSTInPer = obj_Rate.GSTInPer;
                            obj.TotalGSTAmount = (double)(amtForGST - (amtForGST / (1 + (double)obj_Rate.GSTInPer / 100)));
                            objCR.Add(obj);
                            return objCR;
                        }
                    }
                }
                return objCR;
            }
            catch
            {
                return objCR;
            }
        }


        //Added by Zubair for PO on 20-04-2018
        public List<CurrentRate> GetRateForDVPO(long WarehouseId, long ProductId, long VarientId, bool IsSalePrice, long? PORateCalculationID)
        {
            List<CurrentRate> objCR = new List<CurrentRate>();
            try
            {
                Warehouse objWarehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseId);
                if (objWarehouse != null)
                {
                    long RateCalculationID = 0;
                    double Margin = 6;   //Default margin 6 to show rate in PO for DV
                    double DVMargin = 0;
                    double SalePrice = 0;
                    double PurchasePrice = 0;
                    double DVSalePrice = 0;
                    double DVPurchasePrice = 0;
                    double amtForGST = 0;
                    DateTime CurrentDate = DateTime.Now.Date;
                    DVMargin = objWarehouse.Margin == null ? 0 : Convert.ToDouble(objWarehouse.Margin);

                    //Added by Zubair on 21-04-2018
                    //Get record on PO and Invoice/Delivery Note Entry
                    RateCalculation obj_Rate = new RateCalculation();
                    if (PORateCalculationID != null && Convert.ToInt64(PORateCalculationID) > 0)  //Get on Invoice/Delivery entry time
                    {
                        RateCalculationID = Convert.ToInt64(PORateCalculationID);
                        obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ID == RateCalculationID && r.ProductId == ProductId && r.ProductVarientId == VarientId);
                    }
                    else  //Get on Purchase Order creation time
                    {
                        obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ProductId == ProductId && r.ProductVarientId == VarientId && r.RateExpiry >= CurrentDate && r.IsActive == true);
                    }

                    if (obj_Rate != null)
                    {
                        RateCalculationID = obj_Rate.ID;
                        SalePrice = obj_Rate.DecidedSalePrice;
                        PurchasePrice = (SalePrice / (1 + Margin / 100));

                        DVSalePrice = PurchasePrice;
                        DVPurchasePrice = (DVSalePrice / (1 + DVMargin / 100));

                        amtForGST = DVSalePrice - DVPurchasePrice;

                        if (IsSalePrice)
                        {
                            CurrentRate obj = new CurrentRate();
                            obj.RateCalculationID = RateCalculationID;
                            obj.Rate = Math.Round(DVSalePrice, 2);
                            obj.MRP = obj_Rate.MRP;
                            obj.TotalGSTInPer = obj_Rate.GSTInPer;                           
                            obj.TotalGSTAmount = Math.Round((double)(amtForGST - (amtForGST / (1 + (double)obj_Rate.GSTInPer / 100))),2);
                            objCR.Add(obj);
                            return objCR;                           
                        }
                        else
                        {
                            CurrentRate obj = new CurrentRate();
                            obj.RateCalculationID = RateCalculationID;
                            obj.Rate = Math.Round(DVPurchasePrice, 2);
                            obj.MRP = obj_Rate.MRP;
                            obj.TotalGSTInPer = obj_Rate.GSTInPer;
                            obj.TotalGSTAmount = (double)(amtForGST - (amtForGST / (1 + (double)obj_Rate.GSTInPer / 100)));
                            objCR.Add(obj);
                            return objCR;                          
                        }
                    }
                }
                return objCR;
            }
            catch
            {
                return objCR;
            }
        }

        [HttpPost]
        public JsonResult GetRate(long ProductId, long VarientId, bool IsSalePrice,long SupplierID)
        {
            long WarehouseID = 0;
            //Added by Zubair for PO to get RateCalculationID with rate on 20-04-2018
            //double Rate = 0;
            List<CurrentRate> objRate = new List<CurrentRate>();
            Nullable<long> RateCalID = 0;

            long? SupplierWarehouseID = db.Suppliers.Where(x => x.ID == SupplierID).Select(x => x.WarehouseID).FirstOrDefault();
            if(SupplierWarehouseID!=null && SupplierWarehouseID>0)
            {
                WarehouseStock objStock = db.WarehouseStocks.Where(x => x.WarehouseID == SupplierWarehouseID && x.ProductID == ProductId
                    && x.ProductVarientID == VarientId && x.AvailableQuantity > 0).OrderBy(x=>x.ID).OrderBy(x=>x.ExpiryDate).FirstOrDefault();

                if(objStock!=null && objStock.ID>0)
                {
                    long PurchaseOrderID=0;
                    PurchaseOrderID = db.Invoices.Where(x => x.ID == objStock.InvoiceID).Select(x => x.PurchaseOrderID).FirstOrDefault();
                    RateCalID = db.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == PurchaseOrderID && x.ProductID == ProductId 
                        && x.ProductVarientID == VarientId).Select(x => x.RateCalculationId).DefaultIfEmpty(0).FirstOrDefault();
                    //if(rID!=null && rID>0)
                    //{
                    //    RateCalID = rID;
                    //}
                }
            }

            if (Session["WarehouseID"] != null)
            {
                WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
            }
            Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseID);
            if (obj_warehouse != null)
            {
                if (Session["IsEzeeloLogin"] != null && Session["IsEzeeloLogin"].ToString() == "1")
                {
                    objRate = GetRateForEzeeloPO(WarehouseID, ProductId, VarientId, IsSalePrice,null);
                }
                else
                {
                    if (obj_warehouse.IsFulfillmentCenter)
                    {
                        if (RateCalID!=null && RateCalID > 0)
                        {
                            objRate = GetRateForFVPO(WarehouseID, ProductId, VarientId, IsSalePrice, RateCalID);
                        }
                        else
                        {
                            objRate = GetRateForFVPO(WarehouseID, ProductId, VarientId, IsSalePrice, null);
                        }
                    }
                    else
                    {
                        //Yashaswi Inventory Return 20-12-2018
                        if (RateCalID != null && RateCalID > 0)
                        {
                            objRate = GetRateForDVPO(WarehouseID, ProductId, VarientId, IsSalePrice, RateCalID);
                        }
                        else
                        {
                            objRate = GetRateForDVPO(WarehouseID, ProductId, VarientId, IsSalePrice, null);
                        }
                    }
                }
            }
            return Json(objRate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsEzeeloSupplier(long SupplierId)
        {
            bool result = false;
            try
            {
                long WarehouseID = 0;
                if (Session["WarehouseID"] != null)
                {
                    WarehouseID = Convert.ToInt64(Session["WarehouseID"]);
                }

                Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(p => p.ID == WarehouseID);
                if (obj_warehouse != null)
                {
                    Supplier obj_Sup = db.Suppliers.FirstOrDefault(s => s.ID == SupplierId);
                    if (obj_Sup != null)
                    {
                        long Sup_warhouseId = obj_Sup.WarehouseID != null ? (long)obj_Sup.WarehouseID : 0;

                        if (obj_warehouse.IsFulfillmentCenter)
                        {
                            //check its DV
                            long DVId = obj_warehouse.DistributorId != null ? (long)obj_warehouse.DistributorId : 0;
                            if (DVId == Sup_warhouseId)
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            //check Ezeelo
                            long EzeeloWarehouseId = Convert.ToInt64(WebConfigurationManager.AppSettings["EZEELO_WAREHOUSE_ID"]);
                            if (Sup_warhouseId == EzeeloWarehouseId)
                            {
                                result = true;
                            }
                            else
                            {
                                result = true;  //OutSource Supplier for Ezeelo
                            }
                        }
                    }
                }



            }
            catch
            {
                result = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Table()
        {
            return View();
        }

        public double GetClaimAmountDV(long PurchaseOrderReplyDetailId, long WarehouseStockId)
        {
            /////Claim Amount
            /////For DV TO Ezeelo

            PurchaseOrderReplyDetail obj_PurchaseOrderReplyDetail = db.PurchaseOrderReplyDetails.FirstOrDefault(por => por.ID == PurchaseOrderReplyDetailId);
            if (obj_PurchaseOrderReplyDetail != null)
            {
                WarehouseStock obj_WarehouseStock = db.WarehouseStocks.FirstOrDefault(ws => ws.ID == WarehouseStockId);
                if (obj_WarehouseStock != null)
                {
                    decimal DecidedSalePrice = obj_WarehouseStock.SaleRatePerUnit;
                    decimal DiversionInSalePrice = obj_PurchaseOrderReplyDetail.BuyRatePerUnit;
                    double ClaimAmountPerUnit = (double)(DecidedSalePrice - DiversionInSalePrice);
                    return RoundUp(ClaimAmountPerUnit, 2);
                }
            }
            return 0;
        }

        //Yashaswi 24/4/2018
        public decimal GetBusinessPointsByRateCalulationId(decimal BuyRate, long RateCalculationId, long WarehouseId)
        {
            RateCalculation obj_Rate = db.RateCalculations.FirstOrDefault(r => r.ID == RateCalculationId);
            Warehouse obj_warehouse = db.Warehouses.FirstOrDefault(w => w.ID == WarehouseId);
            if (obj_Rate != null)
            {
                double EzeeloPurchasePrice = RoundUp(obj_Rate.BaseInwardPriceEzeelo, 2);
                long DVId = (long)obj_warehouse.DistributorId;
                double DVMargin = RoundUp((double)db.Warehouses.FirstOrDefault(w => w.ID == DVId).Margin, 2);
                decimal FVPurchasePrice = (decimal)RoundUp((double)BuyRate, 2);
                double DVSalePrice = RoundUp((double)FVPurchasePrice, 2);
                double DVPurchasePrice = RoundUp((DVSalePrice / (1 + DVMargin / 100)), 2);
                //at time of entry in warehouse stock and shop stock tabel 
                //decided sale price and diversion sale price is same
                //so claim amount is zero
                double claimAmount = 0;
                double EzeeloMargin = RoundUp((DVPurchasePrice - EzeeloPurchasePrice) + claimAmount, 2);
                double EzeeloGST = RoundUp((double)(EzeeloMargin - (EzeeloMargin / (1 + (double)obj_Rate.GSTInPer / 100))), 2);
                double PostGSTMargin = RoundUp(EzeeloMargin - EzeeloGST, 2);
                double ForLeadership = RoundUp(PostGSTMargin * ((double)objMagDiv.GetMarginDivision((int)Margin_DivisionConstants.Margin_Division.LEADERSHIP) / 100), 2);
                decimal BussinessPoints = objMagDiv.RoundOfBP(ForLeadership * objMagDiv.BPInPaise());
                BussinessPoints = (BussinessPoints < 0) ? 0 : BussinessPoints; //Yashaswi 26/4/2018
                return BussinessPoints;
            }

            return 0;
        }
        //Yashaswi 27/4/2018
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

        //Yashaswi 6/6/2018
        public static double RoundUpNegative(double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            double Result = Math.Ceiling(input * multiplier) / multiplier;
            return Result;
        }

        //Yashaswi 30/4/2018
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

    }
}