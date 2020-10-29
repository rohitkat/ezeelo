using BusinessLogicLayer;
using ModelLayer.Models;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class EWalletAmountConfirmController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        public object Get(long UserId, long CityId, string DeviceId, decimal txtEWalletAmountUsed, decimal hdnOrderAmount, decimal hdndeliveryCharge, long? FranchiseId = null)
        {
            object obj = new object();
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (UserId == null || UserId <= 0 || CityId == null || CityId <= 0 || txtEWalletAmountUsed == null || hdnOrderAmount == null || hdndeliveryCharge == null)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }
                var carts = db.Carts.Where(x => x.UserLoginID == UserId && x.IsActive && x.CityID == CityId && !string.IsNullOrEmpty(x.DeviceID)).OrderByDescending(x => x.ID).FirstOrDefault();//Changes by Sonali for fetched Cart by FracnshiseId and UserLoginId on 24-01-2019
                if (carts != null)
                {
                    decimal walletAmount = 0;
                    MLMWallet objWallet = new MLMWallet();
                    var amount = Convert.ToDecimal(db.MLMWallets.Where(x => x.UserLoginID == UserId).Select(x => x.Amount).FirstOrDefault());
                    if (amount > 0)
                    {
                        walletAmount = amount;
                    }
                    //Get Usable Amount Yashaswi 7-9-2018
                    LeadersPayoutMaster objLeadersPayoutMaster = db.LeadersPayoutMasters.SingleOrDefault(p => p.ID == 1);
                    if (objLeadersPayoutMaster != null)
                    {
                        decimal minResAmt = Convert.ToDecimal(objLeadersPayoutMaster.Min_Resereved);
                        decimal Amt = amount - (amount * minResAmt) / 100;
                        Amt = Math.Round(Amt, 2);
                        
                        if (Amt < 0)
                        {
                            Amt = 0;
                        }
                        walletAmount = Amt;
                    }
                    //Get CashbackWallet Amount 03-10-2019

                    CashbackWallet wallet = db.cashbackWallets.FirstOrDefault(p => p.UserLoginID == UserId);
                    if (wallet != null)
                    {
                        walletAmount = walletAmount + wallet.Amount;
                    }

                    decimal payableAmount = hdnOrderAmount;
                    decimal deliveryCharge = hdndeliveryCharge;
                    if (deliveryCharge > 0)
                    {
                        payableAmount = payableAmount + deliveryCharge;
                    }
                    payableAmount = payableAmount - carts.CouponAmount;//Added CouponAmount for calcualtion payableAmount by Sonali on 19-02-2019
                    if (txtEWalletAmountUsed == null || txtEWalletAmountUsed <= 0)
                    {
                        return obj = new { Success = 0, Message = "Entered amount should be greater than 0.", data = string.Empty };
                    }
                    else if (walletAmount < txtEWalletAmountUsed)
                    {
                        return obj = new { Success = 0, Message = "Entered Amount should be less than EWallet Usable Amount.", data = string.Empty };
                    }
                    else if (payableAmount < txtEWalletAmountUsed)
                    {
                        return obj = new { Success = 0, Message = "Used E-wallet Amount can't be more than Payable Amount.", data = string.Empty };
                    }
                    else
                    {
                        carts.IsWalletApply = true;//Added by Sonali on 19-02-2019
                        carts.WalletUsed = txtEWalletAmountUsed;//Added by Sonali on 19-02-2019
                        db.Entry(carts).State = EntityState.Modified;
                        db.SaveChanges();

                        decimal UsedEarnAmount = txtEWalletAmountUsed;
                        decimal EarnAmount = walletAmount;
                        decimal RemainingAmount = walletAmount - txtEWalletAmountUsed;

                        var trackcartlist = db.TrackCarts.Where(x => x.CartID == carts.ID && x.FranchiseID == FranchiseId && x.UserLoginID == UserId && x.Stage == "SHOPING_CART").ToList();
                        ApiCartViewModel apiCart = new ApiCartViewModel();
                        apiCart.CartPassword = carts.CartPassword;
                        apiCart.CityID = carts.CityID;
                        apiCart.CreateBy = carts.CreateBy;
                        apiCart.CreateDate = carts.CreateDate;
                        apiCart.CustomerOrderID = carts.CustomerOrderID;
                        apiCart.DeviceID = carts.DeviceID;
                        apiCart.DeviceType = carts.DeviceType;
                        apiCart.ID = carts.ID;
                        apiCart.IsActive = carts.IsActive;
                        apiCart.IsPlacedByCustomer = carts.IsPlacedByCustomer;
                        apiCart.MCOID = carts.MCOID;
                        apiCart.ModifyBy = carts.ModifyBy;
                        apiCart.ModifyDate = carts.ModifyDate;
                        apiCart.Name = carts.Name;
                        apiCart.NetworkIP = carts.NetworkIP;
                        apiCart.UserLoginID = carts.UserLoginID;
                        apiCart.Status = carts.Status;
                        apiCart.WalletAmountUsed = UsedEarnAmount;
                        apiCart.UsedEarnAmount = UsedEarnAmount;
                        apiCart.EarnAmount = EarnAmount;
                        apiCart.RemainEarnAmt = RemainingAmount;
                        apiCart.IsWalletApply = carts.IsWalletApply;//Added by Sonali on 19-02-2019
                        apiCart.IsCouponApply = carts.IsCouponApply;//Added by Sonali on 19-02-2019
                        apiCart.CouponCode = carts.CouponCode;//Added by Sonali on 19-02-2019
                        apiCart.CouponAmount = carts.CouponAmount;//Added by Sonali on 19-02-2019
                        if (trackcartlist != null && trackcartlist.Count > 0)
                        {
                            apiCart.TrackCarts = new List<ApiTrackcart>();
                            List<long> StockIDList = trackcartlist.Select(x => x.ShopStockID ?? 0).Distinct().ToList();
                            foreach (var item in StockIDList)
                            {
                                var RemoveShopStockItem = trackcartlist.Where(x => x.ShopStockID == item && (x.Stage == "CHECK_OUT" || x.Stage == "PAYMENT_MODE")).Any();
                                if (RemoveShopStockItem)
                                {
                                    List<TrackCart> RemoveItemList = trackcartlist.Where(x => x.ShopStockID == item).ToList();
                                    foreach (var cart in RemoveItemList)
                                        trackcartlist.Remove(cart);
                                }
                            }
                            if (trackcartlist != null && trackcartlist.Count > 0)
                            {
                                List<ShopStockIDs> ShopStockIds = new List<ShopStockIDs>();
                                foreach (var item in StockIDList)
                                {
                                    ShopStockIDs shopsId = new ShopStockIDs();
                                    shopsId.ssID = item;
                                    ShopStockIds.Add(shopsId);
                                }
                                ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
                                var result = prod.GetShopStockVarients(ShopStockIds);
                                if (result != null)
                                {
                                    foreach (var item in trackcartlist)
                                    {
                                        ApiTrackcart apiTrackCart = new ApiTrackcart();
                                        apiTrackCart.CartID = item.CartID;
                                        apiTrackCart.City = item.City;
                                        apiTrackCart.CreateBy = item.CreateBy;
                                        apiTrackCart.CreateDate = item.CreateDate;
                                        apiTrackCart.DeviceID = item.DeviceID;
                                        apiTrackCart.DeviceType = item.DeviceType;
                                        apiTrackCart.FranchiseID = item.FranchiseID;
                                        apiTrackCart.ID = item.ID;
                                        apiTrackCart.IMEI_NO = item.IMEI_NO;
                                        apiTrackCart.Lattitude = item.Lattitude;
                                        apiTrackCart.Longitude = item.Longitude;
                                        apiTrackCart.Mobile = item.Mobile;
                                        apiTrackCart.NetworkIP = item.NetworkIP;
                                        apiTrackCart.Qty = item.Qty;
                                        apiTrackCart.ShopStockID = item.ShopStockID;
                                        apiTrackCart.Stage = item.Stage;
                                        apiTrackCart.UserLoginID = item.UserLoginID;
                                        apiTrackCart.ShoppingCartProduct = result.Where(x => x.ShopStockID == item.ShopStockID).FirstOrDefault();
                                        apiTrackCart.TotalAmt = apiTrackCart.Qty * apiTrackCart.ShoppingCartProduct.SaleRate ?? 0;
                                        apiTrackCart.TotalItemRetailPoint = apiTrackCart.Qty * apiTrackCart.ShoppingCartProduct.RetailPoint ?? 0;
                                        var warehosueStockQty = db.WarehouseStocks.Where(x => x.ID == apiTrackCart.ShoppingCartProduct.WareHouseStockId).Select(x => x.AvailableQuantity).FirstOrDefault();
                                        int NewPurchaseQty;
                                        string msg;
                                        ShoppingCartInitialization objCart = new ShoppingCartInitialization();
                                        objCart.VerifyCartItem(apiTrackCart.ShoppingCartProduct.StockQty, apiTrackCart.Qty.HasValue ? apiTrackCart.Qty.Value : 0, warehosueStockQty, out NewPurchaseQty, out msg);
                                        apiTrackCart.Message = msg;
                                        apiCart.TrackCarts.Add(apiTrackCart);
                                    }
                                }
                                apiCart.GrandOrderAmount = apiCart.TrackCarts.Sum(x => x.TotalAmt);
                                DeliveryCharges dc = new DeliveryCharges();
                                GetShopWiseDeliveryChargesViewModel ShopListAndPincode = new GetShopWiseDeliveryChargesViewModel();
                                ShopListAndPincode.CityID = CityId;
                                ShopListAndPincode.IsExpress = false;
                                ShopListAndPincode.Pincode = db.Franchises.Where(x => x.ID == FranchiseId).Select(x => x.Pincode.Name).FirstOrDefault();
                                ShopWiseDeliveryCharges shopwiseDeliverycharge = new ShopWiseDeliveryCharges();
                                shopwiseDeliverycharge.ShopID = result.Select(x => x.ShopID).FirstOrDefault();
                                shopwiseDeliverycharge.OrderAmount = apiCart.GrandOrderAmount;
                                shopwiseDeliverycharge.DeliveryType = "Normal";
                                ShopListAndPincode.ShopWiseDelivery = new List<ShopWiseDeliveryCharges>();
                                ShopListAndPincode.ShopWiseDelivery.Add(shopwiseDeliverycharge);
                                apiCart.DeliveryCharges = dc.GetDeliveryCharges(ShopListAndPincode).Sum(x => x.DeliveryCharge);
                                apiCart.PaybleAmount = apiCart.GrandOrderAmount + apiCart.DeliveryCharges - (apiCart.IsCouponApply == true ? apiCart.CouponAmount : 0);//Calculation changed by Sonali on 19-02-2019
                                apiCart.TotalRetailPoint = apiCart.TrackCarts.Sum(x => x.TotalItemRetailPoint);
                                apiCart.PaybleAmount = apiCart.PaybleAmount - UsedEarnAmount;
                                obj = new { Success = 1, Message = "Carts found.", data = apiCart };
                            }
                            else  //Sonali_24/10/2018s
                                obj = new { Success = 0, Message = "Carts found but contains 0 items.", data = apiCart };
                        }
                        else
                        {
                            obj = new { Success = 0, Message = "Carts found but contains 0 items.", data = apiCart };
                        }
                    }
                    //objWallet.UsableWalletAmount = txtEWalletAmountUsed;
                    //if (Session["IsEarnUse"] == null)
                    //{
                    //    lShoppingCartCollection = sci.GetCookie(fConnectionString, true);
                    //}
                    //else
                    //{
                    //    lShoppingCartCollection = sci.GetCookie(fConnectionString, (bool)Session["IsEarnUse"]);
                    //}

                    //lShoppingCartCollection.lShoppingCartOrderDetails.WalletAmountUsed = Convert.ToDecimal(txtEWalletAmountUsed);
                    //lShoppingCartCollection = this.CalculateMLMWalletAmount(lShoppingCartCollection);
                    //TempData["MLMEWalletAmountUsedMessage"] = null;
                }
                else
                {
                    obj = new { Success = 0, Message = "Carts not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
    }
}
