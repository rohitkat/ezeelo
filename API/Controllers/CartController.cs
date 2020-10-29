using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ModelLayer.Models;
using BusinessLogicLayer;
using API.Models;
using ModelLayer.Models.ViewModel;

namespace API.Controllers
{
    public class CartController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();

        // GET api/Cart
        public IQueryable<Cart> GetCarts()
        {
            return db.Carts;
        }


        [ApiException]
        [ValidateModel]
        [HttpGet]
        //[Route("api/CartController/GetCartbyUserID")]
        public object GetCartbyUserID(long UserId, long CityId, string DeviceId, long? FranchiseId = null)
        {
            object obj = new object();
            db.Configuration.ProxyCreationEnabled = false;
            try
            {
                if (UserId == null || UserId <= 0 || CityId == null || CityId <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid data.", data = string.Empty };
                }

                var carts = db.Carts.Where(x => x.UserLoginID == UserId && x.IsActive && x.CityID == CityId && !string.IsNullOrEmpty(x.DeviceID)).OrderByDescending(x => x.ID).FirstOrDefault();//Changes by Sonali for fetched Cart by FracnshiseId and UserLoginId on 24-01-2019
                if (carts != null)
                {
                    var trackcartlist = db.TrackCarts.Where(x => x.CartID == carts.ID && x.FranchiseID == FranchiseId && x.UserLoginID == UserId && x.Qty > 0 && x.Stage == "SHOPING_CART").ToList();
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
                                    if(apiTrackCart.ShoppingCartProduct == null)
                                    {
                                        continue;
                                    }
                                    apiTrackCart.TotalAmt = apiTrackCart.Qty * apiTrackCart.ShoppingCartProduct.SaleRate ?? 0;
                                    apiTrackCart.TotalItemRetailPoint = apiTrackCart.Qty * apiTrackCart.ShoppingCartProduct.RetailPoint ?? 0;
                                    apiTrackCart.TotalItemCashbackPoint = apiTrackCart.Qty.Value * apiTrackCart.ShoppingCartProduct.CashbackPoint;
                                    // var warehouseStockId = db.ShopStocks.FirstOrDefault(p => p.ID == item.ShopStockID).WarehouseStockID;
                                    var warehosueStockQty = db.WarehouseStocks.Where(x => x.ID == apiTrackCart.ShoppingCartProduct.WareHouseStockId).Select(x => x.AvailableQuantity).FirstOrDefault();
                                    // int PurchaseQty = String.IsNullOrEmpty(indivItmDet[2]) ? 0 : Convert.ToInt32(indivItmDet[2]);
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

                            apiCart.PaybleAmount = apiCart.GrandOrderAmount + apiCart.DeliveryCharges;
                            if (apiCart.IsWalletApply)
                            {
                                if (apiCart.PaybleAmount < apiCart.WalletAmountUsed)
                                {
                                    apiCart.WalletAmountUsed = apiCart.PaybleAmount;
                                    Cart cart = db.Carts.FirstOrDefault(p => p.ID == carts.ID);
                                    cart.WalletUsed = apiCart.WalletAmountUsed;
                                    db.SaveChanges();
                                }
                                if (apiCart.WalletAmountUsed > 0)
                                {
                                    apiCart.PaybleAmount = apiCart.PaybleAmount - apiCart.WalletAmountUsed;
                                }
                            }

                            //apiCart.PaybleAmount = apiCart.GrandOrderAmount + apiCart.DeliveryCharges - (apiCart.IsWalletApply == true ? apiCart.WalletAmountUsed : 0);//Calculation changed by Sonali on 19-02-2019


                            apiCart.TotalRetailPoint = apiCart.TrackCarts.Sum(x => x.TotalItemRetailPoint);
                            apiCart.TotalCashbackPoint = apiCart.TrackCarts.Sum(x => x.TotalItemCashbackPoint);
                            obj = new { Success = 1, Message = "Carts found.", data = apiCart };
                        }
                        else  //Sonali_24/10/2018s
                            obj = new { Success = 1, Message = "Carts found but contains 0 items.", data = apiCart };
                    }
                    else
                    {
                        obj = new { Success = 1, Message = "Carts found but contains 0 items.", data = apiCart };
                    }
                }
                else
                {
                    obj = new { Success = 0, Message = "Carts not found.", data = string.Empty };//change status 1 to 0 for cart check by sonali on 16-04-2019
                }
            }
            catch (Exception ex)
            {
                BusinessLogicLayer.ErrorLog.ErrorLogFile("problem in Cart fetch:"  + ex.Message + ex.InnerException, ErrorLog.Module.BussinessLogicLayer, System.Web.HttpContext.Current.Server);
                obj = new { Success = 0, Message =  ex.Message, data = string.Empty };
            }
            return obj;
        }

        // GET api/Cart/5
        [ResponseType(typeof(Cart))]
        public IHttpActionResult GetCart(long id)
        {
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        // PUT api/Cart/5
        public IHttpActionResult PutCart(long id, Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cart.ID)
            {
                return BadRequest();
            }

            db.Entry(cart).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Cart
        public object Post(CartParameters paramValue)
        {
            object obj = new object();
            try
            {
                if (paramValue == null || paramValue.CityID == null || paramValue.CityID <= 0 || paramValue.UserLoginID == null || paramValue.UserLoginID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                }
                Cart OldCart = db.Carts.Where(x => (x.CityID == paramValue.CityID && x.UserLoginID == paramValue.UserLoginID && x.IsActive && !string.IsNullOrEmpty(x.DeviceID)) || x.ID == paramValue.OldCartID).OrderByDescending(x => x.ID).FirstOrDefault();// Remove  x.DeviceID == paramValue.DeviceId by Sonali on 24-01-2019
                if (OldCart != null)
                {
                    obj = new { Success = 1, Message = "Already Cart present.", data = new { CartId = OldCart.ID, Name = OldCart.Name } };
                }
                else
                {
                    UpdateOldCart(paramValue);
                    TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
                    Cart lCart = lTrackCartBusiness.CreateVirtualAbandonedCart(paramValue.UserLoginID, paramValue.CityID, paramValue.MCOID, paramValue.DeviceId, paramValue.DeviceType);
                    if (lCart == null)
                    {
                        obj = new { Success = 0, Message = "Error in creating Cart.", data = string.Empty };
                        // throw new Exception("Error in creating Cart.");
                    }
                    else
                    {
                        obj = new { Success = 1, Message = "Cart is created.", data = new { CartId = lCart.ID, Name = lCart.Name } };
                    }
                }

                //lCartID = lCart.ID;
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
                //return Request.CreateResponse(HttpStatusCode.InternalServerError, lCartID);
                //throw;
            }
            return obj;
            // return Request.CreateResponse(HttpStatusCode.OK, lCartID);
        }

        // DELETE api/Cart/5
        [ResponseType(typeof(Cart))]
        public object DeleteCart(long id)
        {
            object obj = new object();
            try
            {
                if (id == null || id <= 0)
                    return obj = new { Success = 0, Message = "Invalid parameter.", data = string.Empty };
                Cart cart = db.Carts.Find(id);
                if (cart == null)
                {
                    return obj = new { Success = 0, Message = "The record you want to delete, does not exists.", data = string.Empty };
                }
                db.Carts.Remove(cart);
                db.SaveChanges();
                obj = new { Success = 1, Message = "Cart deleted successfully.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private Boolean UpdateOldCart(CartParameters paramValue)
        {
            if (paramValue.OldCartID != null) //- Update old cart as it is placed by customer itself.
            {
                try
                {
                    Cart lOldCart = db.Carts.Find(paramValue.OldCartID);
                    if (lOldCart == null)
                    {
                        return false;
                    }
                    lOldCart.CustomerOrderID = paramValue.CurrentGBODID;
                    lOldCart.IsPlacedByCustomer = true;
                    lOldCart.Status = 1; //- Placed
                    lOldCart.IsActive = false;
                    db.Entry(lOldCart).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        private bool CartExists(long id)
        {
            return db.Carts.Count(e => e.ID == id) > 0;
        }
    }

    public class CartParameters
    {
        public long UserLoginID { get; set; }
        public long CityID { get; set; }
        public Nullable<int> MCOID { get; set; }
        public Nullable<long> OldCartID { get; set; }
        public Nullable<long> CurrentGBODID { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
    }

    public class ApiCartViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long UserLoginID { get; set; }
        public int Status { get; set; }
        public string CartPassword { get; set; }
        public Nullable<long> CustomerOrderID { get; set; }
        public Nullable<long> CityID { get; set; }
        public Nullable<long> MCOID { get; set; }
        public Nullable<Boolean> IsPlacedByCustomer { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<long> ModifyBy { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public List<ApiTrackcart> TrackCarts { get; set; }
        public decimal GrandOrderAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal PaybleAmount { get; set; }
        public decimal TotalRetailPoint { get; set; }
        public decimal TotalCashbackPoint { get; set; }
        public decimal WalletAmountUsed { get; set; } // Added by Sonali for EzeeyMoney Used on 04/02/2019
        public decimal EarnAmount { get; set; }// Added by Sonali for EzeeyMoney Used on 04/02/2019
        public decimal UsedEarnAmount { get; set; }// Added by Sonali for EzeeyMoney Used on 04/02/2019
        public decimal RemainEarnAmt { get; set; }// Added by Sonali for EzeeyMoney Used on 04/02/2019
        public bool IsCouponApply { get; set; }//Added by Sonali on 19-02-2019
        public string CouponCode { get; set; }//Added by Sonali on 19-02-2019
        public decimal CouponAmount { get; set; }//Added by Sonali on 19-02-201
        public bool IsWalletApply { get; set; }//Added by Sonali on 19-02-2019
    }

    public class ApiTrackcart
    {
        public long ID { get; set; }
        public Nullable<long> CartID { get; set; }
        public Nullable<long> UserLoginID { get; set; }
        public Nullable<long> ShopStockID { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Mobile { get; set; }
        public string Stage { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<long> CreateBy { get; set; }
        public string City { get; set; }
        public Nullable<int> FranchiseID { get; set; }//added
        public string IMEI_NO { get; set; }
        public MobileShoppingCartViewModel ShoppingCartProduct { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalItemRetailPoint { get; set; }
        public decimal TotalItemCashbackPoint { get; set; }
        public string Message { get; set; }
    }
}