using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;

namespace BusinessLogicLayer
{
    public class ShoppingCartInitialization
    {
        int i = 0;
        //List<ShoppingCart> lShoppingCart = new List<ShoppingCart>();
        List<ShoppingCartViewModel> lShoppingCartViewModel = new List<ShoppingCartViewModel>();
        List<ShopProductVarientViewModel> lShopProductVarientViewModelCollection = new List<ShopProductVarientViewModel>();
        private EzeeloDBContext db = new EzeeloDBContext();
        // public string fConnectionString { get; set; }                  
        public ShopProductVarientViewModelCollection GetCookie(string fConnectionString, bool IsEarnUse)
        {
            ShopProductVarientViewModelCollection shopProductCollection = new ShopProductVarientViewModelCollection();
            try
            {
                if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] != null && HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value != null && HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value != string.Empty)
                {
                    int BoosterPlanProductCount = 0;
                    long BoosterPlanCategoryId = db.BoosterPlanMaster.FirstOrDefault(p => p.IsActive == true).BoosterCategoryId;
                    //Get tax list from master table
                    List<TaxList> lTaxList = this.GetTaxMasterList();
                    List<CalculatedTaxList> listCalculatedTaxList = new List<CalculatedTaxList>();
                    string cookieValue = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                    string[] ProDetails = cookieValue.Split(',');

                    foreach (string item in ProDetails)
                    {
                        if (item != string.Empty)
                        {
                            string[] indivItmDet = item.Split('$');
                            ProductDetails lProductDetails = new ProductDetails(System.Web.HttpContext.Current.Server);
                            ShopProductVarientViewModel lShopProductVarientViewModel;
                            //long.TryParse(HttpContext.Current.Session["UID"].ToString(),out custSessionID);
                            if (HttpContext.Current.Session["UID"] == null)
                            {
                                lShopProductVarientViewModel = lProductDetails.GetShopStockVarients(Convert.ToInt32(indivItmDet[0]), null);
                            }
                            else
                            {
                                lShopProductVarientViewModel = lProductDetails.GetShopStockVarients(Convert.ToInt32(indivItmDet[0]), Convert.ToInt64(HttpContext.Current.Session["UID"]));
                            }

                            if (lShopProductVarientViewModel != null)
                            {
                                string couponCode = string.Empty;
                                long couponValueRs = 0, couponValuePercent = 0;

                                this.SetCouponCodeInShoppingCart(lShopProductVarientViewModel.ShopStockID, out couponCode, out couponValueRs, out couponValuePercent);

                                //ShopProductVarientViewModel lShopProductVarientViewModel=lProductDetails.GetShopStockVarients(Convert.ToInt32(indivItmDet[0]),custSessionID);
                                ShoppingCartViewModel scvm = new ShoppingCartViewModel();
                                CustomerOrderDetail cOrderDetails = new CustomerOrderDetail();

                                //Yashaswi 29-8-2019 for Booster plan
                                int? ThirdLevelCat = db.Categories.FirstOrDefault(q => q.ID == (db.Categories.FirstOrDefault(p => p.ID == lShopProductVarientViewModel.CategoryID && p.Level == 3).ParentCategoryID) && q.Level == 2).ParentCategoryID;
                                if (BoosterPlanCategoryId == ThirdLevelCat)
                                {
                                    BoosterPlanProductCount = BoosterPlanProductCount + 1;
                                }
                                //end                                                                 
                                scvm.CategoryID = lShopProductVarientViewModel.CategoryID;
                                scvm.CategoryName = lShopProductVarientViewModel.CategoryName;
                                scvm.ColorCode = lShopProductVarientViewModel.ColorCode;
                                scvm.ColorID = lShopProductVarientViewModel.ColorID;
                                scvm.ColorName = lShopProductVarientViewModel.ColorName;
                                scvm.DimensionID = lShopProductVarientViewModel.DimensionID;
                                scvm.DimensionName = lShopProductVarientViewModel.DimensionName;
                                scvm.MaterialID = lShopProductVarientViewModel.MaterialID;
                                scvm.MaterialName = lShopProductVarientViewModel.MaterialName;
                                scvm.MRP = lShopProductVarientViewModel.MRP;
                                scvm.PackSize = lShopProductVarientViewModel.PackSize;
                                scvm.PackUnitName = lShopProductVarientViewModel.PackUnitName;
                                scvm.ProductID = lShopProductVarientViewModel.ProductID;
                                scvm.ProductName = lShopProductVarientViewModel.ProductName;
                                scvm.ProductVarientID = lShopProductVarientViewModel.ProductVarientID;
                                scvm.SaleRate = lShopProductVarientViewModel.SaleRate;
                                scvm.ShopID = lShopProductVarientViewModel.ShopID;
                                scvm.ShopName = lShopProductVarientViewModel.ShopName;
                                scvm.ShopStockID = lShopProductVarientViewModel.ShopStockID;
                                scvm.WarehouseStockID = lShopProductVarientViewModel.WarehouseStockID; //Added by Zubair for Inventory on 27-03-2018
                                scvm.SizeID = lShopProductVarientViewModel.SizeID;
                                scvm.SizeName = lShopProductVarientViewModel.SizeName;
                                scvm.StockQty = lShopProductVarientViewModel.StockQty;
                                scvm.StockThumbPath = lShopProductVarientViewModel.StockThumbPath;
                                scvm.CouponCode = couponCode;
                                scvm.CouponValueRs = couponValueRs;
                                scvm.CouponValuePercent = couponValuePercent;
                                scvm.IsInclusiveOfTax = lShopProductVarientViewModel.IsInclusiveOfTax;
                                scvm.BusinessPointPerUnit = lShopProductVarientViewModel.BusinessPointPerUnit; //Added by Zubair for MLm on 10-01-2018



                                try
                                {
                                    //Yashaswi Start 02-02-2019 for BUG 11 to Verify cart 
                                    string msg;
                                    int PurchaseQty = String.IsNullOrEmpty(indivItmDet[2]) ? 0 : Convert.ToInt32(indivItmDet[2]);
                                    int NewPurchaseQty;
                                    WarehouseStock objWarehouseStock = db.WarehouseStocks.FirstOrDefault(p => p.ID == (db.ShopStocks.FirstOrDefault(s => s.ID == lShopProductVarientViewModel.ShopStockID).WarehouseStockID));
                                    VerifyCartItem(lShopProductVarientViewModel.StockQty, PurchaseQty, objWarehouseStock.AvailableQuantity, out NewPurchaseQty, out msg);
                                    scvm.StockQty = lShopProductVarientViewModel.StockQty;
                                    scvm.PurchaseQuantity = NewPurchaseQty;
                                    scvm.CartVerificationMsg = msg;

                                    //if (lShopProductVarientViewModel.StockStatus == true)
                                    //{
                                    //    if (lShopProductVarientViewModel.StockQty == 0)
                                    //    {
                                    //        //scvm.StockQty = 100;        //set the stock quantity
                                    //        scvm.StockQty = 0;        //set the stock quantity
                                    //        if (indivItmDet[2] == string.Empty || Math.Round(Convert.ToDecimal(indivItmDet[2])) == 0)
                                    //        {
                                    //            scvm.PurchaseQuantity = 1;
                                    //        }
                                    //        else if (scvm.StockQty < Convert.ToInt32(Math.Round(Convert.ToDecimal(indivItmDet[2]))))
                                    //        {
                                    //            scvm.PurchaseQuantity = scvm.StockQty;
                                    //        }
                                    //        else
                                    //        {
                                    //            scvm.PurchaseQuantity = Convert.ToInt32(Math.Round(Convert.ToDecimal(indivItmDet[2])));
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        if (indivItmDet[2] == string.Empty || Math.Round(Convert.ToDecimal(indivItmDet[2])) == 0)
                                    //        {
                                    //            scvm.PurchaseQuantity = 1;
                                    //        }
                                    //        else if (lShopProductVarientViewModel.StockQty < Convert.ToInt32(Math.Round(Convert.ToDecimal(indivItmDet[2]))))
                                    //        {
                                    //            scvm.PurchaseQuantity = lShopProductVarientViewModel.StockQty;
                                    //            scvm.Message = "We're sorry! We are able to accommodate only " + lShopProductVarientViewModel.StockQty +
                                    //                " units of " + lShopProductVarientViewModel.ProductName + " for each customer.";
                                    //        }
                                    //        else
                                    //        {
                                    //            scvm.PurchaseQuantity = Convert.ToInt32(Math.Round(Convert.ToDecimal(indivItmDet[2])));
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    //display message as sold out 
                                    //}  
                                    //Yashaswi End 02-02-2019 for BUG 11 to Verify cart 
                                }
                                catch (Exception ex)
                                {
                                    scvm.PurchaseQuantity = 0; //Yashaswi 02-02-2019 for BUG 11 to Verify cart 
                                    throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][GetCookie]", "Can't get shopping cart detail!" + Environment.NewLine + ex.Message);
                                }
                                //Calculate tax for every product which tax is not included in sale rate 
                                if (!scvm.IsInclusiveOfTax)  //Check condition if tax applicable
                                {
                                    //string fConnectionString="";
                                    TaxationManagement objTaxationManagement = new TaxationManagement(fConnectionString);
                                    scvm.lCalulatedTaxesRecord = objTaxationManagement.CalculateTaxForProduct(scvm.ShopStockID);
                                    List<CalculatedTaxList> obj = this.GetTaxCalCulatedList(scvm.lCalulatedTaxesRecord, lTaxList, scvm.PurchaseQuantity);
                                    listCalculatedTaxList.AddRange(obj);
                                }
                                scvm.CashbackPoitPerUnit = lProductDetails.getCasbackPointsOnProduct(lShopProductVarientViewModel.WarehouseStockID.Value);
                                scvm.CashbackPoints = scvm.CashbackPoitPerUnit * scvm.PurchaseQuantity;
                                scvm.ActualWeight = lShopProductVarientViewModel.ActualWeight;
                                scvm.VolumetricWeight = lShopProductVarientViewModel.VolumetricWeight;
                                //lShoppingCart.Add(sc);
                                lShoppingCartViewModel.Add(scvm);
                            }
                            else
                            {
                                decimal? totalAmount = 0;
                                this.DeleteItemCookie(indivItmDet[0], out totalAmount);
                            }
                        }
                    }
                    // Filter Taxation list depending on name
                    //listCalculatedTaxList = listCalculatedTaxList.GroupBy(x => x.TaxName).ToList();


                    listCalculatedTaxList =
                                            (
                                            from row in listCalculatedTaxList
                                            group row by new { row.TaxName, row.IsGSTInclusive } into g
                                            select new CalculatedTaxList()
                                            {
                                                TaxName = g.Key.TaxName,
                                                //ProjectDescription = g.Key.ProjectDescription,
                                                Amount = g.Sum(x => x.Amount),
                                                IsGSTInclusive = g.Key.IsGSTInclusive   //Added by Zubair for GST on 06-07-2017
                                            }
                                            ).ToList();

                    //scc.Collection = lShoppingCart;
                    shopProductCollection.lShopProductVarientViewModel = lShoppingCartViewModel;
                    shopProductCollection.lCalculatedTaxList = listCalculatedTaxList;
                    //Comment because not totally inmplemented
                    //shopProductCollection.lDeliverySchedule = ShowDeliverySchedule();

                    //========================== Refer and Earn ================================================
                    //if (IsEarnUse == true)
                    //{

                    ReferAndEarn laReferAndEarn = new ReferAndEarn();
                    ShoppingCartOrderDetails lShoppingCartOrderDetails = new ShoppingCartOrderDetails();
                    lShoppingCartOrderDetails.EarnAmount = laReferAndEarn.GetTotalEarnAmount(Convert.ToInt64(HttpContext.Current.Session["UID"]));
                    shopProductCollection.lShoppingCartOrderDetails = lShoppingCartOrderDetails;
                    lShoppingCartOrderDetails.IsUse = IsEarnUse;
                    //}
                    //========================== Refer and Earn ================================================


                    //Added by Zubair for MLm on 17-01-2018
                    long userLoginID = Convert.ToInt64(HttpContext.Current.Session["UID"]);
                    decimal walletAmount = 0;
                    decimal cashbackWalletAmount = 0;
                    MLMWallet objWallet = new MLMWallet();
                    var amount = Convert.ToDecimal(db.MLMWallets.Where(x => x.UserLoginID == userLoginID).Select(x => x.Amount).FirstOrDefault());
                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == userLoginID);
                    if (wallet != null)
                    {
                        cashbackWalletAmount = wallet.Amount;
                    }
                    if (amount > 0 || cashbackWalletAmount > 0)
                    {
                        //Get Usable Amount Yashaswi 7-9-2018
                        LeadersPayoutMaster objLeadersPayoutMaster = db.LeadersPayoutMasters.SingleOrDefault(p => p.ID == 1);
                        if (objLeadersPayoutMaster != null)
                        {
                            decimal Amt = 0;
                            if (amount > 0)
                            {
                                decimal minResAmt = Convert.ToDecimal(objLeadersPayoutMaster.Min_Resereved);
                                Amt = amount - (amount * minResAmt) / 100;
                                Amt = Math.Round(Amt, 2);
                                if (Amt < 0)
                                {
                                    Amt = 0;
                                }
                            }
                            amount = amount + cashbackWalletAmount;
                            Amt = Amt + cashbackWalletAmount;
                            //Start temp substraction of ezeeMoney while adjusting order by yashaswi on 29-01-2018
                            if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                            {
                                if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value != null && Convert.ToDecimal(HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value) > 0)
                                {
                                    decimal eWalletAmountUsed = Convert.ToDecimal(HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value);
                                    if (eWalletAmountUsed > Amt)
                                    {
                                        eWalletAmountUsed = 0;
                                        HttpCookie WalletAmtUsedCookie = new HttpCookie("EWalletAmountUsed");

                                        //Delete whole cookie
                                        if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                                        {
                                            WalletAmtUsedCookie.Expires = DateTime.Now.AddDays(-1);
                                            HttpContext.Current.Response.Cookies.Add(WalletAmtUsedCookie);
                                        }
                                        if (WalletAmtUsedCookie.Expires < DateTime.Now)
                                        {
                                            HttpContext.Current.Request.Cookies.Remove("EWalletAmountUsed");
                                        }
                                        HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Value = "0";

                                        HttpContext.Current.Response.Cookies.Add(WalletAmtUsedCookie);
                                        WalletAmtUsedCookie.Expires = System.DateTime.Now.AddDays(30);
                                    }
                                    walletAmount = walletAmount - eWalletAmountUsed;
                                    lShoppingCartOrderDetails.WalletAmountUsed = eWalletAmountUsed;
                                    amount = amount - eWalletAmountUsed;
                                    Amt = Amt - eWalletAmountUsed;
                                }
                            }
                            //end by yashaswi on 29-01-2018
                            amount = Math.Round(amount);
                            Amt = Math.Round(Amt);
                            objWallet.UsableWalletAmount = "Ezeelo Wallet: Rs. " + amount + " | Usable Amount: Rs. " + Amt + " Click Here to Use it";
                            walletAmount = Amt;
                        }
                        //walletAmount = amount;
                    }
                    objWallet.UserLoginID = userLoginID;
                    objWallet.Amount = walletAmount;
                    shopProductCollection.lMLMWallets = objWallet;

                    shopProductCollection.lShoppingCartOrderDetails = CalculateCartAmount(shopProductCollection);

                    //Yashaswi 29-8-2019 for booster plan
                    if (BoosterPlanProductCount == 0)
                    {
                        shopProductCollection.CartMessage = "";
                        shopProductCollection.IsBoosterPlan = false;
                    }
                    else
                    {
                        string CategoryName = db.Categories.FirstOrDefault(p => p.ID == BoosterPlanCategoryId).Name;
                        if (BoosterPlanProductCount == shopProductCollection.lShopProductVarientViewModel.Count)
                        {
                            shopProductCollection.IsBoosterPlan = true;
                            int MaxBoostRP = db.BoosterPlanMaster.FirstOrDefault(p => p.BoosterCategoryId == BoosterPlanCategoryId).RetailPoints;
                            if (MaxBoostRP > shopProductCollection.lShoppingCartOrderDetails.BusinessPointsTotal)
                            {
                                shopProductCollection.CartMessage = "To place " + CategoryName + " order, cart Retail Points total should be more than " + MaxBoostRP + " RP";
                            }
                            else
                            {
                                shopProductCollection.CartMessage = "";
                            }
                            //shopProductCollection.CartMessage = "To place " + CategoryName + " order, cart Retail Points total should be greater than " + MaxBoostRP + " RP";
                        }
                        else
                        {
                            shopProductCollection.IsBoosterPlan = false;
                            shopProductCollection.CartMessage = "Your cart contain " + CategoryName + " category product. " + CategoryName + " category product can not be purchase with other category product, to go further please remove other category products from your cart!!!";
                        }
                    }
                    //end
                }
                else // Condition Added by Zubair for MLM on 31-1-2018 to reset EWalletAmountUsed value
                {
                    HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value = "0";
                    HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Request.Cookies["EWalletAmountUsed"]);
                }
                //End
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][GetCookie]", "Can't get shopping cart detail!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][GetCookie]", "Can't get shopping cart detail!" + Environment.NewLine + ex.Message);
            }

            return shopProductCollection;
        }



        /// <summary>
        /// Function get product details for express buy
        /// </summary>
        /// <param name="cookieVal"></param>
        /// <returns></returns>
        public ShopProductVarientViewModelCollection GetCookie(string[] cookieVal, string fConnectionString)
        {
            ShopProductVarientViewModelCollection shopProductCollection = new ShopProductVarientViewModelCollection();
            try
            {
                if (cookieVal.Length > 0)
                {
                    //Get tax list from master table
                    List<TaxList> lTaxList = this.GetTaxMasterList();
                    List<CalculatedTaxList> listCalculatedTaxList = new List<CalculatedTaxList>();

                    long custSessionID;
                    foreach (string item in cookieVal)
                    {

                        string[] indivItmDet = item.Split('$');
                        ProductDetails lProductDetails = new ProductDetails(System.Web.HttpContext.Current.Server);
                        ShopProductVarientViewModel lShopProductVarientViewModel;
                        //long.TryParse(HttpContext.Current.Session["UID"].ToString(),out custSessionID);
                        if (HttpContext.Current.Session["UID"] == null)
                        {
                            lShopProductVarientViewModel = lProductDetails.GetShopStockVarients(Convert.ToInt32(indivItmDet[0]), null);
                        }
                        else
                        {
                            lShopProductVarientViewModel = lProductDetails.GetShopStockVarients(Convert.ToInt32(indivItmDet[0]), Convert.ToInt64(HttpContext.Current.Session["UID"]));
                        }

                        string couponCode = string.Empty;
                        long couponValueRs = 0, couponValuePercent = 0;

                        this.SetCouponCodeInShoppingCart(lShopProductVarientViewModel.ShopStockID, out couponCode, out couponValueRs, out couponValuePercent);

                        ShoppingCartViewModel scvm = new ShoppingCartViewModel();
                        CustomerOrderDetail cOrderDetails = new CustomerOrderDetail();

                        scvm.CategoryID = lShopProductVarientViewModel.CategoryID;
                        scvm.CategoryName = lShopProductVarientViewModel.CategoryName;
                        scvm.ColorCode = lShopProductVarientViewModel.ColorCode;
                        scvm.ColorID = lShopProductVarientViewModel.ColorID;
                        scvm.ColorName = lShopProductVarientViewModel.ColorName;
                        scvm.DimensionID = lShopProductVarientViewModel.DimensionID;
                        scvm.DimensionName = lShopProductVarientViewModel.DimensionName;
                        scvm.MaterialID = lShopProductVarientViewModel.MaterialID;
                        scvm.MaterialName = lShopProductVarientViewModel.MaterialName;
                        scvm.MRP = lShopProductVarientViewModel.MRP;
                        scvm.PackSize = lShopProductVarientViewModel.PackSize;
                        scvm.PackUnitName = lShopProductVarientViewModel.PackUnitName;
                        scvm.ProductID = lShopProductVarientViewModel.ProductID;
                        scvm.ProductName = lShopProductVarientViewModel.ProductName;
                        scvm.ProductVarientID = lShopProductVarientViewModel.ProductVarientID;
                        scvm.SaleRate = lShopProductVarientViewModel.SaleRate;
                        scvm.ShopID = lShopProductVarientViewModel.ShopID;
                        scvm.ShopName = lShopProductVarientViewModel.ShopName;
                        scvm.ShopStockID = lShopProductVarientViewModel.ShopStockID;
                        scvm.SizeID = lShopProductVarientViewModel.SizeID;
                        scvm.SizeName = lShopProductVarientViewModel.SizeName;
                        scvm.StockQty = lShopProductVarientViewModel.StockQty;
                        scvm.StockThumbPath = lShopProductVarientViewModel.StockThumbPath;
                        scvm.PurchaseQuantity = Convert.ToInt32(indivItmDet[2]);
                        scvm.ActualWeight = lShopProductVarientViewModel.ActualWeight;
                        scvm.VolumetricWeight = lShopProductVarientViewModel.VolumetricWeight;
                        scvm.CouponCode = couponCode;
                        scvm.CouponValueRs = couponValueRs;
                        scvm.CouponValuePercent = couponValuePercent;
                        scvm.BusinessPointPerUnit = lShopProductVarientViewModel.BusinessPointPerUnit; //Added by Zubair for MLm on 10-01-2018

                        if (!scvm.IsInclusiveOfTax)
                        {
                            //string fConnectionString="";
                            TaxationManagement objTaxationManagement = new TaxationManagement(fConnectionString);
                            scvm.lCalulatedTaxesRecord = objTaxationManagement.CalculateTaxForProduct(scvm.ShopStockID);
                            List<CalculatedTaxList> obj = this.GetTaxCalCulatedList(scvm.lCalulatedTaxesRecord, lTaxList, scvm.PurchaseQuantity);
                            listCalculatedTaxList.AddRange(obj);
                        }

                        lShoppingCartViewModel.Add(scvm);
                    }
                    listCalculatedTaxList = (from row in listCalculatedTaxList
                                             group row by new { row.TaxName } into g
                                             select new CalculatedTaxList()
                                             {
                                                 TaxName = g.Key.TaxName,
                                                 Amount = g.Sum(x => x.Amount)
                                             }).ToList();

                    shopProductCollection.lShopProductVarientViewModel = lShoppingCartViewModel;
                    shopProductCollection.lCalculatedTaxList = listCalculatedTaxList;
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][GetCookie]", "Can't get shopping cart detail!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][GetCookie]", "Can't get shopping cart detail!" + Environment.NewLine + ex.Message);
            }

            return shopProductCollection;
        }



        private List<CalculatedTaxList> GetTaxCalCulatedList(List<CalulatedTaxesRecord> lCalulatedTaxesRecord, List<TaxList> lTaxList, int qty)
        {
            try
            {
                List<CalculatedTaxList> listCalculatedTaxList = new List<CalculatedTaxList>();
                foreach (var item in lCalulatedTaxesRecord)
                {
                    foreach (var item1 in lTaxList)
                    {
                        if (item.TaxPrefix == item1.TaxPrefix)
                        {
                            CalculatedTaxList lCalculatedTaxList = new CalculatedTaxList();
                            lCalculatedTaxList.TaxName = item1.TaxPrefix;
                            lCalculatedTaxList.Amount = item.TaxableAmount * qty;
                            lCalculatedTaxList.IsGSTInclusive = item.IsGSTInclusive;
                            listCalculatedTaxList.Add(lCalculatedTaxList);
                        }
                    }

                }
                return listCalculatedTaxList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<TaxList> GetTaxMasterList()
        {
            try
            {
                List<TaxList> lTaxList = new List<TaxList>();
                lTaxList = (from tx in db.TaxationMasters

                            group tx by new { tx.Prefix } into g
                            select new TaxList
                            {
                                TaxPrefix = g.Key.Prefix
                            }).ToList();
                return lTaxList;
            }
            catch (Exception)
            {

                throw;
            }

        }




        /// <summary>
        /// Delete Item From Shopping Cart
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        public void DeleteItemCookie(string shopStockID, out decimal? TotalAmount)
        {
            TotalAmount = 0;//Added By Yashaswi
            try
            {
                string P = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                string[] individualItemCookie = P.Split(',');
                HttpCookie ShoppingCartCookie = new HttpCookie("ShoppingCartCookie");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] != null)
                {
                    ShoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                }
                if (ShoppingCartCookie.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("ShoppingCartCookie");
                }
                string DeletedProduct = ""; //Added By Yashaswi               
                //Reinitialize cookie
                //exclude item cookie which want to delete
                foreach (string item in individualItemCookie)
                {
                    if (item != string.Empty)
                    {
                        string[] individualItemDetailsCookie = item.Split('$');
                        if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] == null && individualItemDetailsCookie[0] != shopStockID)
                        {
                            HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = individualItemDetailsCookie[0].ToString().Trim() + "$"
                                + individualItemDetailsCookie[1].ToString().Trim() + "$" + individualItemDetailsCookie[2].ToString().Trim();
                            HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                            ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                        }
                        else if (individualItemDetailsCookie[0] != shopStockID)
                        {
                            HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value
                                + "," + individualItemDetailsCookie[0].ToString().Trim() + "$" + individualItemDetailsCookie[1].ToString().Trim() + "$"
                                + individualItemDetailsCookie[2].ToString().Trim();
                            HttpContext.Current.Response.AppendCookie(ShoppingCartCookie);
                            ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                        }
                        //Added By Yashaswi
                        if (individualItemDetailsCookie[0] == shopStockID)
                        {
                            DeletedProduct = item;
                        }
                    }
                }
                //Added By Yashaswi
                if (DeletedProduct != "" && DeletedProduct != null)
                {
                    string[] individualItemDetailsCookie = DeletedProduct.Split('$');
                    long _ShopStockId = Convert.ToInt64(shopStockID);
                    ShopStock ss = db.ShopStocks.FirstOrDefault(p => p.ID == _ShopStockId);
                    if (ss != null)
                    {
                        decimal SaleRate = ss.RetailerRate;
                        int PurchaseQty = Convert.ToInt16(individualItemDetailsCookie[2]);
                        TotalAmount = SaleRate * PurchaseQty;
                    }
                }


                HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteItemCookie]", "Can't delete shopping cart item cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteItemCookie]", "Can't delete shopping cart item cookie!" + Environment.NewLine + ex.Message);
            }
        }

        public void DeleteCouponCookie(string shopStockID)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] != null && HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value != string.Empty)
                {
                    string P = HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value;
                    if (!P.Contains(shopStockID))
                        return;

                    string[] individualCouponCookie = P.Split(',');
                    HttpCookie ShoppingCartCookie = new HttpCookie("CouponManagementCookie");

                    //Delete whole cookie
                    if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] != null)
                    {
                        ShoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
                        HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                    }
                    if (ShoppingCartCookie.Expires < DateTime.Now)
                    {
                        HttpContext.Current.Request.Cookies.Remove("CouponManagementCookie");
                    }

                    //Reinitialize cookie
                    //exclude item cookie which want to delete
                    foreach (string item in individualCouponCookie)
                    {
                        if (item != string.Empty)
                        {
                            string[] individualItemDetailsCookie = item.Split('$');
                            if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] == null && individualItemDetailsCookie[0] != shopStockID)
                            {
                                HttpContext.Current.Response.Cookies["CouponManagementCookie"].Value = individualItemDetailsCookie[0].ToString().Trim() + "$"
                                    + individualItemDetailsCookie[1].ToString().Trim() + "$" + individualItemDetailsCookie[2].ToString().Trim();
                                HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                                ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                            }
                            else if (individualItemDetailsCookie[0] != shopStockID)
                            {
                                HttpContext.Current.Response.Cookies["CouponManagementCookie"].Value = HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value
                                    + "," + individualItemDetailsCookie[0].ToString().Trim() + "$" + individualItemDetailsCookie[1].ToString().Trim() + "$"
                                    + individualItemDetailsCookie[2].ToString().Trim();
                                HttpContext.Current.Response.AppendCookie(ShoppingCartCookie);
                                ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                            }
                        }
                    }

                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                    ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteCouponCookie]", "Can't delete coupon cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteCouponCookie]", "Can't delete coupon cookie!" + Environment.NewLine + ex.Message);
            }
        }

        public void DeleteCouponCookie()
        {
            try
            {
                HttpCookie ShoppingCartCookie = new HttpCookie("CouponManagementCookie");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] != null)
                {
                    ShoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                }
                if (ShoppingCartCookie.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("CouponManagementCookie");
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteCouponCookie]", "Can't delete coupon cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteCouponCookie]", "Can't delete coupon cookie!" + Environment.NewLine + ex.Message);
            }
        }

        public void DeleteWalleteAmtUsed(decimal? hdnOrdAmt, decimal? RemovedProductAmount, long franchiseId, long CityId)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                {
                    if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value != null && Convert.ToDecimal(HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value) > 0)
                    {
                        decimal DeliveryCharge = 25;
                        decimal MaxOrdAmt = 750;
                        DeliveryCharge DC = db.DeliveryCharges.FirstOrDefault(p => p.FranchiseID == franchiseId && p.CityID == CityId && p.IsActive == true);
                        if (DC != null)
                        {
                            DeliveryCharge = DC.Charges;
                            MaxOrdAmt = DC.OrderAmount;
                        }

                        hdnOrdAmt = hdnOrdAmt ?? 0;
                        RemovedProductAmount = RemovedProductAmount ?? 0;

                        decimal EWalletAmountUsed = Convert.ToDecimal(HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value);
                        decimal PayableAmount = ((decimal)hdnOrdAmt - (decimal)RemovedProductAmount);
                        if (PayableAmount <= MaxOrdAmt)
                        {
                            PayableAmount = PayableAmount + DeliveryCharge;
                        }
                        if (EWalletAmountUsed > PayableAmount)
                        {
                            HttpCookie WalletAmtUsedCookie = new HttpCookie("EWalletAmountUsed");

                            //Delete whole cookie
                            if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                            {
                                WalletAmtUsedCookie.Expires = DateTime.Now.AddDays(-1);
                                HttpContext.Current.Response.Cookies.Add(WalletAmtUsedCookie);
                            }
                            if (WalletAmtUsedCookie.Expires < DateTime.Now)
                            {
                                HttpContext.Current.Request.Cookies.Remove("EWalletAmountUsed");
                            }
                            HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Value = PayableAmount.ToString();

                            HttpContext.Current.Response.Cookies.Add(WalletAmtUsedCookie);
                            WalletAmtUsedCookie.Expires = System.DateTime.Now.AddDays(30);
                        }
                    }
                }
            }
            catch (MyException myEx)
            {
                //throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteWalleteAmtUsed]", "Can't change wallet Amount Used!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                //throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteWalleteAmtUsed]", "Can't change wallet Amount Used!" + Environment.NewLine + ex.Message);
            }
        }
        //calculate merchant wise delivery charge
        //this function used before subscription module here shopwise delivery charges calculated
        //public ShopProductVarientViewModelCollection GetDeliveryCharge(ShopProductVarientViewModelCollection lShoppingCartCollection)
        //{
        //    try
        //    {
        //        DeliveryCharges dc = new DeliveryCharges();
        //        List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();


        //         List<long> merId = lShoppingCartCollection.lShopProductVarientViewModel.Select(p => p.ShopID).Distinct().ToList();
        //            for (int i = 0; i < merId.Count; i++)
        //            {
        //                decimal MerchantWiseSubTotal = 0;
        //                decimal orgProductWeight = 0;
        //                var orgProducts = lShoppingCartCollection.lShopProductVarientViewModel.Select(x => new
        //                {
        //                    x.ShopID,
        //                    x.ActualWeight,
        //                    x.PurchaseQuantity,
        //                    x.SaleRate,
        //                })
        //               .Where(x => x.ShopID == merId[i]).ToList();

        //                for (int j = 0; j < orgProducts.Count; j++)
        //                {
        //                    orgProductWeight = orgProductWeight + orgProducts[j].ActualWeight * orgProducts[j].PurchaseQuantity;
        //                    MerchantWiseSubTotal = MerchantWiseSubTotal + Convert.ToInt64(orgProducts[j].SaleRate * orgProducts[j].PurchaseQuantity);
        //                }
        //                ShopWiseDeliveryCharges lShopWiseDeliveryCharges = new ShopWiseDeliveryCharges();
        //                if (MerchantWiseSubTotal < 500)
        //                {

        //                    lShopWiseDeliveryCharges.ShopID = merId[i];
        //                    lShopWiseDeliveryCharges.OrderAmount = MerchantWiseSubTotal;
        //                    lShopWiseDeliveryCharges.Weight = orgProductWeight;
        //                    lShopWiseDeliveryCharges.DeliveryType = "Normal";


        //                    //while checking pincode if pincode is not entered by customer
        //                    //Two Delivery charge property initialize here because one property is used in shopping cart view to show delivery charge
        //                    //And other property is used in payment process page
        //                    if (HttpContext.Current.Request.Cookies["DeliverablePincode"] != null)
        //                    {
        //                        lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge = dc.GetDeliveryCharges(HttpContext.Current.Request.Cookies["DeliverablePincode"].Value, orgProductWeight, false);
        //                        lShopWiseDeliveryCharges.DeliveryCharge = lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge;
        //                    }
        //                    else
        //                    {
        //                        lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge = 0;
        //                        lShopWiseDeliveryCharges.DeliveryCharge = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge = 0;
        //                    lShopWiseDeliveryCharges.DeliveryCharge = 0;
        //                }
        //                listShopWiseDeliveryCharges.Add(lShopWiseDeliveryCharges);

        //            }
        //            lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return lShoppingCartCollection;
        //}


        /// <summary>
        /// changes for subscription module here delivery charges calculated irrespective of shops
        /// </summary>
        /// <param name="lShoppingCartCollection"></param>
        /// <returns></returns>
        public ShopProductVarientViewModelCollection GetDeliveryCharge(ShopProductVarientViewModelCollection lShoppingCartCollection)
        {
            try
            {
                DeliveryCharges dc = new DeliveryCharges();
                List<ShopWiseDeliveryCharges> listShopWiseDeliveryCharges = new List<ShopWiseDeliveryCharges>();
                ShopWiseDeliveryCharges lShopWiseDeliveryCharges = new ShopWiseDeliveryCharges();



                if (HttpContext.Current.Request.Cookies["DeliverablePincode"] != null)

                {
                    List<long> merId = lShoppingCartCollection.lShopProductVarientViewModel.Select(p => p.ShopID).Distinct().ToList();
                    for (int i = 0; i < merId.Count; i++)
                    {
                        decimal MerchantWiseSubTotal = 0;
                        decimal orgProductWeight = 0;
                        var orgProducts = lShoppingCartCollection.lShopProductVarientViewModel.Select(x => new
                        {
                            x.ShopID,
                            x.ActualWeight,
                            x.PurchaseQuantity,
                            x.SaleRate,
                            x.CategoryID
                        })
                       .Where(x => x.ShopID == merId[i]).ToList();

                        for (int j = 0; j < orgProducts.Count; j++)
                        {
                            orgProductWeight = orgProductWeight + orgProducts[j].ActualWeight * orgProducts[j].PurchaseQuantity;
                            MerchantWiseSubTotal = MerchantWiseSubTotal + Convert.ToInt64(orgProducts[j].SaleRate * orgProducts[j].PurchaseQuantity);
                        }


                        lShopWiseDeliveryCharges = new ShopWiseDeliveryCharges();
                        lShopWiseDeliveryCharges.ShopID = merId[i];
                        lShopWiseDeliveryCharges.OrderAmount = MerchantWiseSubTotal;
                        lShopWiseDeliveryCharges.Weight = orgProductWeight;
                        lShopWiseDeliveryCharges.DeliveryType = "Normal";
                        lShopWiseDeliveryCharges.CatID = orgProducts.FirstOrDefault().CategoryID;

                        listShopWiseDeliveryCharges.Add(lShopWiseDeliveryCharges);
                    }

                    GetShopWiseDeliveryChargesViewModel lGetShopWiseDeliveryChargesViewModel = new GetShopWiseDeliveryChargesViewModel();
                    lGetShopWiseDeliveryChargesViewModel.ShopWiseDelivery = listShopWiseDeliveryCharges;
                    lGetShopWiseDeliveryChargesViewModel.Pincode = HttpContext.Current.Request.Cookies["DeliverablePincode"].Value;
                    lGetShopWiseDeliveryChargesViewModel.IsExpress = false;

                    listShopWiseDeliveryCharges = dc.GetDeliveryCharges(lGetShopWiseDeliveryChargesViewModel);
                }
                else
                {
                    lShoppingCartCollection.lShopProductVarientViewModel[i].DeliveryCharge = 0;
                    lShopWiseDeliveryCharges.DeliveryCharge = 0;
                    listShopWiseDeliveryCharges.Add(lShopWiseDeliveryCharges);
                }

                lShoppingCartCollection.lShopWiseDeliveryCharges = listShopWiseDeliveryCharges;

            }
            catch (Exception)
            {

                throw;
            }
            return lShoppingCartCollection;
        }

        public void ChangeItemQuantity(string shopStockID, string quantity, out decimal? TotalAmount)
        {
            TotalAmount = 0;
            try
            {
                string DeletedProduct = ""; //Added By Yashaswi  
                string P = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                string[] individualItemCookie = P.Split(',');
                HttpCookie ShoppingCartCookie = new HttpCookie("ShoppingCartCookie");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] != null)
                {
                    ShoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                }
                if (ShoppingCartCookie.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("ShoppingCartCookie");
                }

                //Reinitialize cookie
                //exclude item cookie which want to delete
                foreach (string item in individualItemCookie)
                {
                    if (item != string.Empty)
                    {
                        string[] individualItemDetailsCookie = item.Split('$');
                        if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] == null)
                        {
                            if (individualItemDetailsCookie[0] == shopStockID)
                            {
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = individualItemDetailsCookie[0].ToString().Trim() + "$" +
                                    individualItemDetailsCookie[1].ToString().Trim() + "$" + quantity;
                            }
                            else
                            {
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = individualItemDetailsCookie[0].ToString().Trim() + "$" +
                                    individualItemDetailsCookie[1].ToString().Trim() + "$" + individualItemDetailsCookie[2].ToString().Trim();
                            }

                            HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                            ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                        }
                        else
                        {
                            if (individualItemDetailsCookie[0] == shopStockID)
                            {
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value
                                + "," + individualItemDetailsCookie[0].ToString().Trim() + "$" + individualItemDetailsCookie[1].ToString().Trim() + "$" + quantity;
                            }
                            else
                            {
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value
                                + "," + individualItemDetailsCookie[0].ToString().Trim() + "$" + individualItemDetailsCookie[1].ToString().Trim() + "$" +
                                individualItemDetailsCookie[2].ToString().Trim();
                            }

                            HttpContext.Current.Response.AppendCookie(ShoppingCartCookie);
                            ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                        }
                        //Added By Yashaswi
                        if (individualItemDetailsCookie[0] == shopStockID)
                        {
                            DeletedProduct = item;
                        }
                    }

                    //Added By Yashaswi
                    if (DeletedProduct != "" && DeletedProduct != null)
                    {
                        string[] individualItemDetailsCookie = DeletedProduct.Split('$');
                        long _ShopStockId = Convert.ToInt64(shopStockID);
                        ShopStock ss = db.ShopStocks.FirstOrDefault(p => p.ID == _ShopStockId);
                        if (ss != null)
                        {
                            decimal SaleRate = ss.RetailerRate;
                            int OldPurchaseQty = Convert.ToInt16(individualItemDetailsCookie[2]);
                            int PurchaseQty = Convert.ToInt16(quantity);
                            if (OldPurchaseQty > PurchaseQty)
                            {
                                PurchaseQty = OldPurchaseQty - PurchaseQty;
                            }
                            TotalAmount = SaleRate * PurchaseQty;
                        }
                    }

                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                    ShoppingCartCookie.Expires = System.DateTime.Now.AddDays(30);
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][ChangeItemQuantity]", "Can't change shopping cart item cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][ChangeItemQuantity]", "Can't change shopping cart item cookie!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Delete Shopping cart whole cookie
        /// </summary>
        public void DeleteShoppingCartCookie()
        {
            try
            {
                HttpCookie ShoppingCartCookie = new HttpCookie("ShoppingCartCookie");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] != null)
                {
                    ShoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                }
                else
                {
                    HttpContext.Current.Response.Cookies.Add(ShoppingCartCookie);
                    ShoppingCartCookie.Expires = DateTime.Now.AddDays(-1);
                }
                if (ShoppingCartCookie.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("ShoppingCartCookie");
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteShoppingCartCookie]", "Can't delete shopping cart cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteShoppingCartCookie]", "Can't delete shopping cart cookie!" + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// Set shopping cart cookie on buy event
        /// </summary>
        /// <param name="ShopStockID"></param>
        /// <param name="itemID"></param>
        /// <returns>1:indicates item is successfully added in shopping cart
        /// 2: indicates item is already added in cart</returns>
        public string SetCookie(long ShopStockID, long itemID, int quantity)
        {
            try
            {
                //Yashaswi 29-8-2019 for booster plan
                List<ShopStockIDs> ShopStockIds = new List<ShopStockIDs>();
                if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] != null && HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value != string.Empty)
                {
                    string cook = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                    string[] ItemID = cook.Split(',');
                    foreach (var item in ItemID)
                    {
                        string[] prd = item.Split('$');
                        long shpStId = Convert.ToInt64(prd[0]);
                        ShopStockIds.Add(new ShopStockIDs { ssID = shpStId });
                    }
                }
                if (ShopStockIds.Count != 0)
                {
                    string msg = "";
                    bool result = CheckForBoosterPlan(itemID, ShopStockIds, out msg);
                    if (result == false)
                    {
                        return "$" + msg;
                    }
                }
                //Start Yashaswi 28-01-2019
                //Check Out of Stock Condition
                ShopStock ShopStockcurrentAvlQty = db.ShopStocks.FirstOrDefault(p => p.ID == ShopStockID);
                if (ShopStockcurrentAvlQty != null)
                {
                    WarehouseStock WarehouseStockcurrentQty = db.WarehouseStocks.FirstOrDefault(p => p.ID == ShopStockcurrentAvlQty.WarehouseStockID);
                    int currentAvlQty = ShopStockcurrentAvlQty.Qty;
                    if (currentAvlQty <= 0 || WarehouseStockcurrentQty.AvailableQuantity <= 0)
                    {
                        //Product is out of stock
                        return "3";
                    }
                    else if (currentAvlQty < quantity && WarehouseStockcurrentQty.AvailableQuantity < quantity)
                    {
                        //current available quantity is less than required quantity
                        quantity = currentAvlQty;
                        return "4";
                    }
                    else if (quantity <= currentAvlQty && quantity <= WarehouseStockcurrentQty.AvailableQuantity)
                    {
                        //End Yashaswi 28-01-2019

                        // HttpCookie ShoppingCartCookie = new HttpCookie("ShoppingCartCookie");
                        if (HttpContext.Current.Request.Cookies["ShoppingCartCookie"] == null || HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value == string.Empty)
                        {

                            //Add Item Details cookie
                            if (quantity != null && quantity > 0)
                            {
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = ShopStockID + "$" + itemID + "$" + quantity;

                            }
                            else
                            {
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = ShopStockID + "$" + itemID + "$" + 1;
                            }
                            //ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
                            HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["ShoppingCartCookie"]);
                            HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
                            return "1";
                        }
                        else
                        {
                            string cook = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                            string[] ItemID = cook.Split(',');
                            // if (ItemID.Contains(ShopStockID + "$" + itemID + "$" + 1))
                            if (cook.Contains(ShopStockID + "$" + itemID + "$"))
                            {
                                ////prevent to add duplicate item in shopping cart
                                /*************************** If we not reInitialize cookie then Cookie gets cleared****************************/
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
                                //ControllerContext.HttpContext.Response.Cookies["ShoppingCartCookie"].Value = ControllerContext.HttpContext.Request.Cookies["ShoppingCartCookie"].Value;
                                HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["ShoppingCartCookie"]);
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
                                return "2";
                            }
                            else
                            {
                                if (quantity != null && quantity > 0)
                                {
                                    HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value + "," +
                                        ShopStockID.ToString() + "$" + itemID.ToString() + "$" + quantity;
                                }
                                else
                                {
                                    HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Value = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value + "," +
                                        ShopStockID.ToString() + "$" + itemID.ToString() + "$" + "1";
                                }
                                HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["ShoppingCartCookie"]);
                                HttpContext.Current.Response.Cookies["ShoppingCartCookie"].Expires = System.DateTime.Now.AddDays(30);
                                return "1";
                            }
                        }
                    }
                    else
                    {
                        //Product is out of stock at inventory side Yashaswi 04-02-2019
                        //current available quantity is less than required quantity
                        quantity = WarehouseStockcurrentQty.AvailableQuantity;
                        return "4";
                    }
                }
                else
                {
                    //Product is not found
                    return "3";
                }
                //return RedirectToAction("Index", "ShoppingCart");
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteShoppingCartCookie]", "Can't delete shopping cart cookie!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][DeleteShoppingCartCookie]", "Can't delete shopping cart cookie!" + Environment.NewLine + ex.Message);
            }
        }

        public void SetVerifiedPincode(string pincode)
        {
            try
            {
                HttpCookie DeliverablePincode = new HttpCookie("DeliverablePincode");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["DeliverablePincode"] != null)
                {
                    DeliverablePincode.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(DeliverablePincode);
                }
                if (DeliverablePincode.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("DeliverablePincode");
                }
                //set pincode in cookie
                HttpContext.Current.Response.Cookies["DeliverablePincode"].Value = pincode;
                HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Response.Cookies["DeliverablePincode"]);
                HttpContext.Current.Response.Cookies["DeliverablePincode"].Expires = System.DateTime.Now.AddDays(30);
                //HttpContext.Current.Response.Cookies.Add(DeliverablePincode);
                //DeliverablePincode.Expires = System.DateTime.Now.AddDays(30);
                //ControllerContext.HttpContext.Response.Cookies.Add(ControllerContext.HttpContext.Request.Cookies["DeliverablePincode"]);
                //ControllerContext.HttpContext.Response.Cookies["DeliverablePincode"].Expires = System.DateTime.Now.AddDays(30);
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][SetVerifiedPincode]", "Can't check pincode is verified or not!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][SetVerifiedPincode]", "Can't get shopping cart detail!" + Environment.NewLine + ex.Message);
            }
        }
        //Yashaswi 4/6/2018
        public void RemoveVerifiedPincode()
        {
            try
            {
                HttpCookie DeliverablePincode = new HttpCookie("DeliverablePincode");

                //Delete whole cookie
                if (HttpContext.Current.Request.Cookies["DeliverablePincode"] != null)
                {
                    DeliverablePincode.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(DeliverablePincode);
                }
                if (DeliverablePincode.Expires < DateTime.Now)
                {
                    HttpContext.Current.Request.Cookies.Remove("DeliverablePincode");
                }
            }
            catch (MyException myEx)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][RemoveVerifiedPincode]", "Can't check pincode is verified or not!" + Environment.NewLine + myEx.Message);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicLayer.MyException("[ShoppingCartInitialization][RemoveVerifiedPincode]", "Can't get shopping cart detail!" + Environment.NewLine + ex.Message);
            }
        }
        private void SetCouponCodeInShoppingCart(long shopStockId, out string couponCode, out long couponValueRs, out long couponValuePercent)
        {
            couponCode = string.Empty;
            couponValueRs = 0;
            couponValuePercent = 0;

            if (HttpContext.Current.Request.Cookies["CouponManagementCookie"] != null && HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value != string.Empty)
            {
                string cookieValue = HttpContext.Current.Request.Cookies["CouponManagementCookie"].Value;
                //Change by harshada 
                if (cookieValue != null && cookieValue != string.Empty)
                {
                    string[] cookieArray = cookieValue.Split(',');

                    foreach (var item in cookieArray)
                    {
                        string[] couponDetails = item.Split('$');
                        if (couponDetails[0] == shopStockId.ToString())
                        {
                            if (couponDetails.Length >= 4)
                            {
                                couponCode = couponDetails[1];
                                couponValueRs = Convert.ToInt64(couponDetails[2]);
                                couponValuePercent = Convert.ToInt64(couponDetails[3]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// return available delivery schedule
        /// </summary>
        /// <returns></returns>
        private List<DeliverySchedule> ShowDeliverySchedule()
        {
            List<DeliverySchedule> lDeliverySchedule = new List<DeliverySchedule>();
            try
            {
                DeliverySchedule obj = new DeliverySchedule();
                EzeeloDBContext db = new EzeeloDBContext();
                var qry = db.DeliverySchedules.ToList();
                for (int i = 0; i < qry.Count; i++)
                {
                    obj.ActualTimeFrom = qry[i].ActualTimeFrom;
                    obj.ActualTimeTo = qry[i].ActualTimeTo;
                    obj.DisplayName = qry[i].DisplayName;
                    obj.NoOfDelivery = qry[i].NoOfDelivery;
                    lDeliverySchedule.Add(obj);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return lDeliverySchedule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// //Yashaswi Start 02-02-2019 for BUG 11 to Verify cart 
        public int VerifyCartOnOrderPLace()
        {
            int result = 0;
            string P = HttpContext.Current.Request.Cookies["ShoppingCartCookie"].Value;
            string[] individualItemCookie = P.Split(',');
            foreach (string item in individualItemCookie)
            {
                if (item != string.Empty)
                {
                    string[] data = item.Split('$');
                    long shopstockId = Convert.ToInt64(data[0]);
                    ShopStock Sp = db.ShopStocks.FirstOrDefault(p => p.ID == shopstockId);
                    WarehouseStock Ws = db.WarehouseStocks.FirstOrDefault(p => p.ID == Sp.WarehouseStockID);
                    string msg;
                    int PurchaseQty = String.IsNullOrEmpty(data[2]) ? 0 : Convert.ToInt32(data[2]);
                    int NewPurchaseQty;
                    VerifyCartItem(Sp.Qty, PurchaseQty, Ws.AvailableQuantity, out NewPurchaseQty, out msg);
                    if (msg != "")
                    {
                        result = 1;
                        break;
                    }
                }
            }
            return result;
        }


        public void VerifyCartItem(int ShopStockQty, int PurchQty, int WarehouseStockQty, out int Qty, out string msg)
        {
            Qty = 0;
            msg = "";
            try
            {
                if (ShopStockQty <= 0 || WarehouseStockQty <= 0)
                {
                    msg = "Product is out of stock! Please remove product from cart and continue.";
                    Qty = 0;
                }
                else if (ShopStockQty < PurchQty && WarehouseStockQty < PurchQty)
                {
                    msg = "We're sorry! We are able to accommodate only " + ShopStockQty + " units of this product.";
                    Qty = ShopStockQty;
                }
                else if (ShopStockQty >= PurchQty && WarehouseStockQty >= PurchQty)
                {
                    Qty = PurchQty;
                    msg = "";
                }
                else
                {
                    msg = "Something went wrong!!!";
                    Qty = 0;
                }
            }
            catch (Exception ex)
            {

            }
        }
        //Yashaswi End 02-02-2019 for BUG 11 to Verify cart 
        ////


        public ShoppingCartOrderDetails CalculateCartAmount(ShopProductVarientViewModelCollection lShoppingCartCollection)
        {
            //Coupon amount and earn amount is not considered in this calculate method // disscused with Reena Mam
            double subTotal = 0.0; //Order amount
            double deliveryCharge = 25;
            double orderPayableAmount = 0.0;
            double taxAmount = 0;
            decimal businessPointsTotal = 0;
            decimal cashbackPointsTotal = 0;
            double eWalletAmountUsed = Convert.ToDouble(lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed);
            double totalWalletAmount = 0;
            decimal cashbackWalletAmount = 0;
            bool IsWalletAmountUsedChanged = false;
            ShoppingCartOrderDetails scartOrder = new ShoppingCartOrderDetails();
            try
            {
                if (lShoppingCartCollection.lShopProductVarientViewModel != null && lShoppingCartCollection.lShopProductVarientViewModel.Count() > 0)
                {
                    foreach (var item in lShoppingCartCollection.lShopProductVarientViewModel)
                    {
                        double OrdAmt = 0;
                        OrdAmt = Convert.ToDouble(item.SaleRate * item.PurchaseQuantity);
                        subTotal = subTotal + OrdAmt;
                        businessPointsTotal = businessPointsTotal + (item.BusinessPointPerUnit * item.PurchaseQuantity);
                        cashbackPointsTotal = cashbackPointsTotal + (item.CashbackPoitPerUnit * item.PurchaseQuantity);
                    }
                    if (lShoppingCartCollection.lCalculatedTaxList != null && lShoppingCartCollection.lCalculatedTaxList.Count() > 0)
                    {
                        foreach (var tax in lShoppingCartCollection.lCalculatedTaxList)
                        {
                            if (tax.IsGSTInclusive == false)
                            {
                                taxAmount = taxAmount + Convert.ToDouble(tax.Amount);
                            }
                        }
                    }
                    MLMWallet mLMWallet = db.MLMWallets.FirstOrDefault(p => p.UserLoginID == lShoppingCartCollection.lMLMWallets.UserLoginID);
                    if (mLMWallet != null && lShoppingCartCollection.lMLMWallets != null)
                    {
                        totalWalletAmount = Convert.ToDouble(mLMWallet.Amount);
                    }
                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == lShoppingCartCollection.lMLMWallets.UserLoginID);
                    if (wallet != null)
                    {
                        cashbackWalletAmount = wallet.Amount;
                    }
                    totalWalletAmount = totalWalletAmount + Convert.ToDouble(cashbackWalletAmount);
                    if (HttpContext.Current.Request.Cookies["CityCookie"] != null)
                    {
                        string cookieValue = HttpContext.Current.Request.Cookies["CityCookie"].Value;
                        string[] ArrycookieValue = cookieValue.Split('$');
                        if (ArrycookieValue.Count() == 4)
                        {
                            long CityId = Convert.ToInt64(ArrycookieValue[0]);
                            long FranchiseId = Convert.ToInt64(ArrycookieValue[2]);
                            deliveryCharge = 25;
                            double MaxOrdAmt = 750;
                            DeliveryCharge DC = db.DeliveryCharges.FirstOrDefault(p => p.FranchiseID == FranchiseId && p.CityID == CityId && p.IsActive == true);
                            if (DC != null)
                            {
                                MaxOrdAmt = Convert.ToDouble(DC.OrderAmount);
                                deliveryCharge = Convert.ToDouble(DC.Charges);
                            }
                            if (subTotal >= MaxOrdAmt)
                            {
                                deliveryCharge = 0;
                            }
                        }
                    }
                    if (eWalletAmountUsed > totalWalletAmount)
                    {
                        eWalletAmountUsed = totalWalletAmount;
                        IsWalletAmountUsedChanged = true;
                    }
                    orderPayableAmount = deliveryCharge + subTotal + taxAmount;
                    if (eWalletAmountUsed > orderPayableAmount)
                    {
                        eWalletAmountUsed = orderPayableAmount;
                        IsWalletAmountUsedChanged = true;
                    }
                    orderPayableAmount = orderPayableAmount - eWalletAmountUsed;
                    lShoppingCartCollection.lMLMWallets.Amount = Convert.ToDecimal(totalWalletAmount - eWalletAmountUsed);
                    lShoppingCartCollection.lMLMWallets.Amount = Math.Round(lShoppingCartCollection.lMLMWallets.Amount);
                    lShoppingCartCollection.lMLMWallets.UsableWalletAmount = "Ezeelo Wallet: Rs. " + lShoppingCartCollection.lMLMWallets.Amount + " | Usable Amount: Rs. " + lShoppingCartCollection.lMLMWallets.Amount + " Click Here to Use it"; ;

                    if (IsWalletAmountUsedChanged)
                    {
                        if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                        {
                            if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value != null && Convert.ToDecimal(HttpContext.Current.Request.Cookies["EWalletAmountUsed"].Value) > 0)
                            {
                                HttpCookie WalletAmtUsedCookie = new HttpCookie("EWalletAmountUsed");
                                //Delete whole cookie
                                if (HttpContext.Current.Request.Cookies["EWalletAmountUsed"] != null)
                                {
                                    WalletAmtUsedCookie.Expires = DateTime.Now.AddDays(-1);
                                    HttpContext.Current.Response.Cookies.Add(WalletAmtUsedCookie);
                                }
                                if (WalletAmtUsedCookie.Expires < DateTime.Now)
                                {
                                    HttpContext.Current.Request.Cookies.Remove("EWalletAmountUsed");
                                }
                                HttpContext.Current.Response.Cookies["EWalletAmountUsed"].Value = eWalletAmountUsed.ToString();

                                HttpContext.Current.Response.Cookies.Add(WalletAmtUsedCookie);
                                WalletAmtUsedCookie.Expires = System.DateTime.Now.AddDays(30);
                            }
                        }
                    }

                    scartOrder.BusinessPointsTotal = businessPointsTotal;
                    scartOrder.CashbackPointsTotal = cashbackPointsTotal;
                    scartOrder.DeliveryCharges = Convert.ToDecimal(deliveryCharge);
                    scartOrder.PayableAmount = Math.Round(Convert.ToDecimal(orderPayableAmount), 2);
                    scartOrder.TotalOrderAmount = Math.Round(Convert.ToDecimal(subTotal), 2);
                    scartOrder.WalletAmountUsed = Math.Round(Convert.ToDecimal(eWalletAmountUsed), 2);
                }
            }
            catch (Exception ex)
            {

            }
            return scartOrder;
        }

        //Start By yashaswi 29-08-2019 For Booster Plan
        public bool CheckForBoosterPlan(long ProductId, List<ShopStockIDs> ShopStockIds, out string Message)
        {
            Message = "";
            bool Result = true;
            try
            {
                //Get current product ThirdLevel Category
                Product product = db.Products.FirstOrDefault(p => p.ID == ProductId);
                if (product != null)
                {
                    int? ThirdLevelCategory = db.Categories.FirstOrDefault(q => q.ID == (db.Categories.FirstOrDefault(p => p.ID == product.CategoryID && p.Level == 3).ParentCategoryID) && q.Level == 2).ParentCategoryID;
                    string CategoryName = db.Categories.FirstOrDefault(p => p.ID == ThirdLevelCategory).Name;
                    //Check current product category is Booster plan category
                    if (db.BoosterPlanMaster.Any(p => p.BoosterCategoryId == ThirdLevelCategory && p.IsActive == true))
                    {
                        ////If true for Booster plan category
                        //then check cart 
                        if (ShopStockIds.Count == 0)
                        {
                            //Cart Empty then continue
                            Result = true;
                        }
                        else
                        {
                            //Cart Contain Products
                            foreach (var item in ShopStockIds)
                            {
                                Product prd = db.Products.FirstOrDefault(p => p.ID == (db.ShopProducts.FirstOrDefault(sp => sp.ID == (db.ShopStocks.FirstOrDefault(ss => ss.ID == item.ssID).ShopProductID)).ProductID));
                                if (prd != null)
                                {
                                    int? ThirdLevelCat = db.Categories.FirstOrDefault(q => q.ID == (db.Categories.FirstOrDefault(p => p.ID == prd.CategoryID && p.Level == 3).ParentCategoryID) && q.Level == 2).ParentCategoryID;
                                    //Check for other category
                                    if (ThirdLevelCat != ThirdLevelCategory)
                                    {
                                        //other category product found
                                        Message = "Your cart contain other category product. " + CategoryName + " category product can not be purchase with other category product, to go further please empty your cart!!!";
                                        Result = false;
                                        break;
                                    }
                                    else
                                    {
                                        //other category product not found
                                        Result = true;
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        ////If false for Booster plan 
                        //then check cart 
                        if (ShopStockIds.Count == 0)
                        {
                            //Cart Empty then continue
                            Result = true;
                        }
                        else
                        {
                            //Cart contais product
                            foreach (var item in ShopStockIds)
                            {
                                Product prd = db.Products.FirstOrDefault(p => p.ID == (db.ShopProducts.FirstOrDefault(sp => sp.ID == (db.ShopStocks.FirstOrDefault(ss => ss.ID == item.ssID).ShopProductID)).ProductID));
                                if (prd != null)
                                {
                                    int? ThirdLevelCat = db.Categories.FirstOrDefault(q => q.ID == (db.Categories.FirstOrDefault(p => p.ID == prd.CategoryID && p.Level == 3).ParentCategoryID) && q.Level == 2).ParentCategoryID;
                                    //Check for Booster category Product
                                    if (db.BoosterPlanMaster.Any(p => p.BoosterCategoryId == ThirdLevelCat && p.IsActive == true))
                                    {
                                        //Booster category product found
                                        CategoryName = db.Categories.FirstOrDefault(p => p.ID == (db.BoosterPlanMaster.FirstOrDefault(b => b.IsActive == true)).BoosterCategoryId).Name;
                                        Message = "Your cart contain " + CategoryName + " category product. " + CategoryName + " category product can not be purchase with other category product, to go further please empty your cart!!!";
                                        Result = false;
                                        break;
                                    }
                                    else
                                    {
                                        //Booster category product not found
                                        Result = true;
                                    }
                                }
                            }
                        }
                    }
                }
                //End
            }
            catch
            {

            }
            return Result;
        }
    }
}