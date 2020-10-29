using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using API.Models;
using ModelLayer.Models;
using System.Data.Entity;

namespace API.Controllers
{
    public class ApplyCoupenController : ApiController
    {

        private EzeeloDBContext db = new EzeeloDBContext();

        // GET api/applycoupen
        /// <summary>
        /// Apply coupencode against shop bill Amount
        /// </summary>
        /// <param name="couponCode">CoupenCode entered by Customer</param>
        /// <param name="shopId">Shop ID</param>
        /// <param name="billAmount">Shopwise Bill Amount Without Delivery Charges</param>
        /// <param name="customerLoginID">CustomerLoginID</param>
        /// <returns>Coupen details</returns>
        [ApiException]
        [ValidateModel]
        public object Get(string couponCode, long shopID, double GrandOrderAmount, long customerLoginID, long cityID, int? franchiseID = null)////added int? franchiseID for Multiple MCO
        {
            object obj = new object();
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (customerLoginID == null || customerLoginID <= 0 || cityID == null || cityID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                }
                if (couponCode.Equals(string.Empty) || couponCode.Trim().Length != 8)
                {
                    return obj = new { Success = 0, Message = "Invalid coupon code", data = string.Empty };
                }
                Cart carts = db.Carts.Where(x => x.UserLoginID == customerLoginID && x.IsActive && x.CityID == cityID && !string.IsNullOrEmpty(x.DeviceID)).OrderByDescending(x => x.ID).FirstOrDefault();//Changes by Sonali for fetched Cart by FracnshiseId and UserLoginId on 24-01-2019
                if (carts != null)
                {
                    CouponManagement couponManagement = new CouponManagement();
                    CouponDetailsViewModel CouponDetails = new CouponDetailsViewModel();
                    if (customerLoginID <= 0)
                        CouponDetails = couponManagement.CheckCouponCodeOnBillAmount(couponCode, shopID, GrandOrderAmount);
                    else
                        CouponDetails = couponManagement.CheckCouponCodeOnBillAmount(couponCode, shopID, GrandOrderAmount, customerLoginID, cityID, franchiseID);////added franchiseID
                    if (CouponDetails != null)
                    {
                        if (CouponDetails.Result == 1)
                        {
                            carts.IsCouponApply = true;
                            carts.CouponCode = couponCode;
                            carts.CouponAmount = Convert.ToDecimal(CouponDetails.VoucherAmount);
                            db.Entry(carts).State = EntityState.Modified;
                            db.SaveChanges();

                            var trackcartlist = db.TrackCarts.Where(x => x.CartID == carts.ID && x.FranchiseID == franchiseID && x.UserLoginID == customerLoginID && x.Stage == "SHOPING_CART").ToList();
                            if (trackcartlist != null && trackcartlist.Count > 0)
                            {
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
                                apiCart.CouponCode = carts.CouponCode;//Added by sonali on 19-02-2019
                                apiCart.CouponAmount = carts.CouponAmount;//Added by sonali on 19-02-2019
                                apiCart.IsCouponApply = carts.IsCouponApply;//Added by sonali on 19-02-2019
                                apiCart.WalletAmountUsed = carts.WalletUsed;//Added by sonali on 19-02-2019
                                apiCart.IsWalletApply = carts.IsWalletApply; //Added by sonali on 19-02-2019
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
                                    ShopListAndPincode.CityID = cityID;
                                    ShopListAndPincode.IsExpress = false;
                                    ShopListAndPincode.Pincode = db.Franchises.Where(x => x.ID == franchiseID).Select(x => x.Pincode.Name).FirstOrDefault();
                                    ShopWiseDeliveryCharges shopwiseDeliverycharge = new ShopWiseDeliveryCharges();
                                    shopwiseDeliverycharge.ShopID = result.Select(x => x.ShopID).FirstOrDefault();
                                    shopwiseDeliverycharge.OrderAmount = apiCart.GrandOrderAmount;
                                    shopwiseDeliverycharge.DeliveryType = "Normal";
                                    ShopListAndPincode.ShopWiseDelivery = new List<ShopWiseDeliveryCharges>();
                                    ShopListAndPincode.ShopWiseDelivery.Add(shopwiseDeliverycharge);
                                    apiCart.DeliveryCharges = dc.GetDeliveryCharges(ShopListAndPincode).Sum(x => x.DeliveryCharge);
                                    apiCart.PaybleAmount = apiCart.GrandOrderAmount + apiCart.DeliveryCharges - (apiCart.IsWalletApply == true ? apiCart.WalletAmountUsed : 0) - (apiCart.IsCouponApply == true ? apiCart.CouponAmount : 0);//Calculation changed by Sonali on 19-02-2019
                                    apiCart.TotalRetailPoint = apiCart.TrackCarts.Sum(x => x.TotalItemRetailPoint);
                                    CouponDetails.WalletAmount = apiCart.IsWalletApply ? apiCart.WalletAmountUsed : 0;
                                    CouponDetails.PayableAmount = apiCart.PaybleAmount;
                                    //apiCart.PaybleAmount = apiCart.PaybleAmount - UsedEarnAmount;
                                    obj = new { Success = 1, Message = CouponDetails.UserMessage, data = CouponDetails };
                                }
                                else  //Sonali_24/10/2018s
                                    obj = new { Success = 0, Message = "Carts found but contains 0 items.", data = CouponDetails };
                            }
                            else
                            {
                                obj = new { Success = 0, Message = "Carts found but contains 0 items.", data = CouponDetails };
                            }

                            // obj = new { Success = 1, Message = CouponDetails.UserMessage, data = CouponDetails };
                        }
                        else
                        {
                            obj = new { Success = 0, Message = CouponDetails.UserMessage, data = CouponDetails };
                        }
                    }
                    else
                    {
                        obj = new { Success = 0, Message = "Invalid coupan", data = string.Empty };
                    }
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
            //if (couponCode.Equals(string.Empty)|| couponCode.Trim().Length != 8 )
            //{
            //    object obj = new object();
            //    obj = new { HTTPStatusCode = "400", UserMessage = "Invalid paramters", ValidationError = "Invalid Coupen Code" };
            //    return Request.CreateResponse(HttpStatusCode.BadRequest, obj);
            //}

            //CouponManagement couponManagement = new CouponManagement();
            //CouponDetailsViewModel CouponDetails = new CouponDetailsViewModel();
            //if (customerLoginID <= 0)
            //    CouponDetails = couponManagement.CheckCouponCodeOnBillAmount(couponCode, shopID, billAmount);
            //else
            //    CouponDetails = couponManagement.CheckCouponCodeOnBillAmount(couponCode, shopID, billAmount, customerLoginID, cityID,franchiseID);////added franchiseID


            //return Request.CreateResponse(HttpStatusCode.OK, CouponDetails); 

        }

    }
}
