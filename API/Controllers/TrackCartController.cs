using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models;
using System.Data.Entity;
using API.Models;
using ModelLayer.Models.ViewModel;
using System.Web.Configuration;

namespace API.Controllers
{
    public class TrackCartController : ApiController
    {
        private EzeeloDBContext db = new EzeeloDBContext();
        private static string fConnectionString = WebConfigurationManager.ConnectionStrings["EzeeloDBContext"].ToString();
        // GET api/trackcart
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/trackcart/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/trackcart
        //Sonali-15-09-2018//
        [ApiException]
        [ValidateModel]
        public object Post(List<TrackCartParameters> paramValues)
        {
            object obj = new object();
            try
            {
                if (paramValues == null || paramValues.Count <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                }
                //Yashaswi 30-8-2019 for booster plan
                int? franchiseId = paramValues.FirstOrDefault().FranchiseID;
                long? cartId = paramValues.FirstOrDefault().CartID;
                long UserloginId = paramValues.FirstOrDefault().UserLoginID;
                string Stage = paramValues.FirstOrDefault().Stage;
                List<ShopStockIDs> ShopStockIds_ = db.TrackCarts.Where(x => x.FranchiseID == franchiseId && x.CartID == cartId  && x.UserLoginID == UserloginId && x.Stage == Stage).
                    Select(x => new ShopStockIDs() { ssID = x.ShopStockID ?? 0 }).ToList();
                

                List<long> StockIDList = paramValues.Select(x => x.ShopStockID).ToList();
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
                    foreach (var item in paramValues)
                    {
                        long ProductId = db.ShopProducts.FirstOrDefault(sp => sp.ID == (db.ShopStocks.FirstOrDefault(ss => ss.ID == item.ShopStockID).ShopProductID)).ProductID;
                        string ProductName = db.Products.FirstOrDefault(p => p.ID == ProductId).Name; //Addedby yashaswi 12-6-19
                        var stcokQty = result.Where(x => x.ShopStockID == item.ShopStockID).Select(x => x.StockQty).FirstOrDefault();
                        var WarehouseStockId = result.Where(x => x.ShopStockID == item.ShopStockID).Select(x => x.WareHouseStockId).FirstOrDefault();
                        var warehouseQty = db.WarehouseStocks.Where(x => x.ID == WarehouseStockId).Select(x => x.AvailableQuantity).FirstOrDefault();
                        //Commented By yashaswi 12-6-2019
                        //if (stcokQty <= 0)
                        //{
                        //    return obj = new { Success = 0, Message = "Product is out of stock!", data = string.Empty };
                        //}
                        //if (warehouseQty <= 0)
                        //{
                        //    return obj = new { Success = 0, Message = "Product is out of stock!", data = string.Empty };
                        //}
                        //if (item.Qty > stcokQty)
                        //    return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + stcokQty + " units of this product.if more Qty required call to customer care.", data = string.Empty };
                        //if (item.Qty > warehouseQty)
                        //    return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + warehouseQty + " units of this product.if more Qty required call to customer care.", data = string.Empty };
                        //-- For Differentiate Old and New APP --//
                        if (item.Version == null)
                        { item.FranchiseID = null; }
                        //-- For update qty of item in cart --//
                        //Added by Yashaswi 30-8-2019 from boost plan
                        string msg = "";
                        bool result_ = (new ShoppingCartInitialization()).CheckForBoosterPlan(ProductId, ShopStockIds_, out msg);
                        if (result_ == false)
                        {
                            return obj = new { Success = 0, Message = msg, data = string.Empty };
                        }
                        //End
                        var existingItem = db.TrackCarts.Where(x => x.ShopStockID == item.ShopStockID && x.FranchiseID == item.FranchiseID && x.CartID == item.CartID && x.UserLoginID == item.UserLoginID && x.Stage == item.Stage).OrderByDescending(x => x.ID).FirstOrDefault();
                        if (existingItem != null)
                        {                            
                            //Added By yashaswi start 12-6-2019
                            if (existingItem.Qty <= item.Qty)
                            {
                                
                                if (stcokQty <= 0)
                                {
                                    return obj = new { Success = 0, Message = "Product " + ProductName + " is out of stock!", data = string.Empty };
                                }
                                if (warehouseQty <= 0)
                                {
                                    return obj = new { Success = 0, Message = "Product " + ProductName + " is out of stock!", data = string.Empty };
                                }
                                if (item.Qty > stcokQty)
                                    return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + stcokQty + " units of " + ProductName + ".if more Qty required call to customer care.", data = string.Empty };
                                if (item.Qty > warehouseQty)
                                    return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + warehouseQty + " units of " + ProductName + ".if more Qty required call to customer care.", data = string.Empty };

                            }
                            //End
                            existingItem.Qty = item.Qty;
                            db.Entry(existingItem).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                        else
                        {
                            //Added By yashaswi Start 12-6-2019
                            if (stcokQty <= 0)
                            {
                                return obj = new { Success = 0, Message = "Product " + ProductName + " is out of stock!", data = string.Empty };
                            }
                            if (warehouseQty <= 0)
                            {
                                return obj = new { Success = 0, Message = "Product " + ProductName + " is out of stock!", data = string.Empty };
                            }
                            if (item.Qty > stcokQty)
                                return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + stcokQty + " units of " + ProductName + ".if more Qty required call to customer care.", data = string.Empty };
                            if (item.Qty > warehouseQty)
                                return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + warehouseQty + " units of " + ProductName + ".if more Qty required call to customer care.", data = string.Empty };
                            //end
                            TrackCartBusiness.InsertCartDetails(item.CartID, item.Qty, item.UserLoginID, item.ShopStockID, item.Mobile, item.Stage, item.Lattitude, item.Longitude, item.DeviceType, item.DeviceID, item.City, item.IMEI_NO, item.FranchiseID);////added params item.FranchiseID for Multiple MCO
                        }
                    }


                    obj = new { Success = 1, Message = "Product added in cart", data = string.Empty };
                }
                else
                {
                    obj = new { Success = 0, Message = "Enter valid ShopStockId.", data = string.Empty };
                }
                //List<string> res = new List<string> { "Opration Perform1", "Opration Perform2" };
                //return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;

        }

        // PUT api/trackcart/5
        [ApiException]
        [ValidateModel]
        [Route("api/TrackCart/Put")]
        public object Put(List<TrackCartParameters> trackcartList)
        {
            object obj = new object();
            try
            {
                if (trackcartList == null || trackcartList.Count <= 0)
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                bool isresult = false;
                List<long> StockIDList = trackcartList.Select(x => x.ShopStockID).ToList();
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
                    foreach (var item in trackcartList)
                    {
                        if (item.IsDelete == null && item.IsEdit == null)
                        {
                            return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                        }
                        if (item.IsDelete.Value)
                        {
                            var existingItem = db.TrackCarts.Where(x => x.ShopStockID == item.ShopStockID && x.FranchiseID == item.FranchiseID && x.CartID == item.CartID && x.UserLoginID == item.UserLoginID).OrderByDescending(x => x.ID).FirstOrDefault();
                            if (existingItem != null)
                            {
                                db.TrackCarts.Remove(existingItem);
                                db.SaveChanges();
                                isresult = true;
                                //obj = new { Success = 1, Message = "Successfully remove item.", data = string.Empty };
                            }
                            else
                                isresult = false;
                            //obj = new { Success = 1, Message = "Particular product is not present in cart.", data = string.Empty };
                        }
                        else
                        {
                            var stcokQty = result.Where(x => x.ShopStockID == item.ShopStockID).Select(x => x.StockQty).FirstOrDefault();
                            if (item.Qty > stcokQty)
                                return obj = new { Success = 0, Message = "We're sorry! We are able to accommodate only " + stcokQty + " units of this product.if more Qty required call to customer care.", data = string.Empty };

                            var existingItem = db.TrackCarts.Where(x => x.ShopStockID == item.ShopStockID && x.FranchiseID == item.FranchiseID && x.CartID == item.CartID && x.UserLoginID == item.UserLoginID).OrderByDescending(x => x.ID).FirstOrDefault();
                            if (existingItem != null)
                            {
                                existingItem.Qty = item.Qty;
                                db.Entry(existingItem).State = EntityState.Modified;
                                db.SaveChanges();
                                isresult = true;
                                //obj = new { Success = 1, Message = "Successfully update item.", data = existingItem };
                            }
                            else
                                isresult = false;
                            //obj = new { Success = 1, Message = "Particular product is not present in cart.", data = string.Empty };
                        }
                    }
                }
                if (isresult)
                    obj = new { Success = 1, Message = "Successfully done.", data = string.Empty };
                else
                    obj = new { Success = 1, Message = "Particular product is not present in cart.", data = string.Empty };

            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }


        //// GET api/GetVirtualCart
        //public object GetVirtualCart(long UserLoginID, long CityID, int? MCOID)
        //{
        //    string lCartID = "";
        //    try
        //    {
        //        TrackCartBusiness lTrackCartBusiness = new TrackCartBusiness();
        //        Cart lCart = lTrackCartBusiness.CreateVirtualAbandonedCart(UserLoginID, CityID, MCOID); 
        //        if(lCart != null)
        //        {
        //            lCartID = lCart.ID.ToString();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, lCartID);
        //        //throw;
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, lCartID);
        //}

        // DELETE api/trackcart/5
        //Sonali-15-09-2018//
        [ApiException]
        [ValidateModel]
        public object Delete(TrackCartParameters trackcart)
        {
            object obj = new object();
            try
            {
                if (trackcart == null)
                    return obj = new { Success = 0, Message = "Enter valid parameter.", data = string.Empty };
                var existingItem = db.TrackCarts.Where(x => x.ShopStockID == trackcart.ShopStockID && x.FranchiseID == trackcart.FranchiseID && x.CartID == trackcart.CartID && x.UserLoginID == trackcart.UserLoginID).OrderByDescending(x => x.ID).FirstOrDefault();
                if (existingItem != null)
                {
                    db.TrackCarts.Remove(existingItem);
                    db.SaveChanges();
                    obj = new { Success = 1, Message = "Successfully remove item.", data = string.Empty };
                }
                else
                    obj = new { Success = 1, Message = "Particular product is not present in cart.", data = string.Empty };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

    }

    public class TrackCartParameters
    {
        public Nullable<long> CartID { get; set; } //- Added by Avi Verma.  Date : 07-Sep-2016.
        public Nullable<int> Qty { get; set; } //- Added by Avi Verma.  Date : 07-Sep-2016.
        public long UserLoginID { get; set; }
        // public OfferStatus lOfferStatus { get; set; }
        public long ShopStockID { get; set; }
        public string Mobile { get; set; }
        public string Stage { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public string NetworkIP { get; set; }
        public string DeviceType { get; set; }
        public string DeviceID { get; set; }
        public string City { get; set; }
        public int? FranchiseID { get; set; }////Added by Ashish for multiple MCO
        public string IMEI_NO { get; set; }
        public int? Version { get; set; }////Added by Ashish For New App
        public bool? IsDelete { get; set; }
        public bool? IsEdit { get; set; }

        //public decimal BusinessPointPerUnit { get; set; } // Added by Sonali for MLM on 18/09/2018
        //public decimal BusinessPoints { get; set; }// Added by Sonali for MLM on 18/09/2018
    }

}
