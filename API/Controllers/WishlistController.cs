
//-----------------------------------------------------------------------
// <copyright file="WishlistController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using API.Models;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class WishlistController : ApiController
    {

        // GET api/wishlist/5
        /// <summary>
        /// Get list of products added in customer wishlist
        /// </summary>
        /// <param name="lCustLoginID">customer login ID</param>
        /// <returns>List of Products, added in authenticated customer's wishlist.</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        public object Get(long lCustLoginID, long lFranchiseId)
        {
            object obj = new object();
            try
            {
                if (lCustLoginID == null || lCustLoginID == 0 || lFranchiseId == null || lFranchiseId == 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid paramter.", data = string.Empty };
                }
                CustomerWishlist lcustWishlist = new CustomerWishlist(System.Web.HttpContext.Current.Server);
                var wishlist = lcustWishlist.GetWishlist(lCustLoginID, lFranchiseId);
                if (wishlist != null && wishlist.Count > 0)
                {
                    obj = new { Success = 1, Message = "Wishlist found.", data = wishlist };
                }
                else
                {
                    obj = new { Success = 1, Message = "Wishlist not found.", data = string.Empty };
                }
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }
        //// GET api/wishlist/5
        //public IEnumerable<ProductStockDetailViewModel> Get(long lCustLoginID, long lShopStockID)
        //{
        //    CustomerWishlist lcustWishlist = new CustomerWishlist(System.Web.HttpContext.Current.Server);
        //    return lcustWishlist.GetWishlist(lCustLoginID);
        //}
        //// POST api/wishlist
        // public void Post([FromBody]string value)
        // {
        // }


        // POST api/wishlist/5/5
        /// <summary>
        /// Add product in customer wishlist
        /// </summary>
        /// <param name="CustFavorites">CustomerFavorites object wich contains customer login ID and product stock id</param>
        /// <returns>Operation Status</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        public object Post(CustomerFavorites CustFavorites)
        {
            object obj = new object();
            try
            {
                if (CustFavorites == null || CustFavorites.CustLoginID == 0 || CustFavorites.CustLoginID <= 0 || CustFavorites.ShopStockID == null || CustFavorites.ShopStockID <= 0)
                {
                    return obj = new { Success = 0, Message = "Enter valid details", data = string.Empty };
                }
                CustomerWishlist lcustWishlist = new CustomerWishlist(System.Web.HttpContext.Current.Server);
                int oprStatus = lcustWishlist.SetWishlist(CustFavorites.CustLoginID, CustFavorites.ShopStockID);

                if (oprStatus == 101)
                    obj = new { Success = 1, Message = "Product Successfully Added in Wishlist.", data = new { Staus = 1 } };//Added by Sonali_23-11-2018
                //return new { HTTPStatusCode = "200", UserMessage = "Product Successfully Added in Wishlist." };

                else if (oprStatus == 107)
                    obj = new { Success = 1, Message = "Product is already added in wishlist.", data = new { Staus = 1 } };//Added by Sonali_23-11-2018
                //  return new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet : Product is already added in wishlist." };
                else
                    obj = new { Success = 0, Message = "Some error occur.", data = new { Staus = 0 } };//Added by Sonali_23-11-2018
                //return new { HTTPStatusCode = "400", UserMessage = "BadRequest", status = "True" };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            //BusinessLogicLayer.ErrorLog.ErrorLogFile("Enter " + CustFavorites.CustLoginID + CustFavorites.ShopStockID, ErrorLog.Module.API, System.Web.HttpContext.Current.Server);

            //CustomerWishlist lcustWishlist = new CustomerWishlist(System.Web.HttpContext.Current.Server);
            //int oprStatus = lcustWishlist.SetWishlist(CustFavorites.CustLoginID, CustFavorites.ShopStockID);

            //if (oprStatus == 101)
            //    return new { HTTPStatusCode = "200", UserMessage = "Product Successfully Added in Wishlist." };

            //else if (oprStatus == 107)
            //    return new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet : Product is already added in wishlist." };

            //return new { HTTPStatusCode = "400", UserMessage = "BadRequest", status = "True" };

            return obj;
        }

        // DELETE api/wishlist/5
        /// <summary>
        /// Remove product from customer's wishlist
        /// </summary>
        /// <param name="lCustLoginID">Customer login ID</param>
        /// <param name="lShopStockID">Product Stock ID</param>
        /// <returns>Operation Status</returns>
        [TokenVerification]
        [ApiException]
        [ValidateModel]
        public object Delete(long lCustLoginID, long lShopStockID)
        {
            object obj = new object();
            try
            {
                if (lCustLoginID <= 0 || lCustLoginID == null || lShopStockID <= 0 || lShopStockID == null)
                {
                    return obj = new { Success = 0, Message = "Enter valid details", data = string.Empty };
                }
                CustomerWishlist lcustWishlist = new CustomerWishlist(System.Web.HttpContext.Current.Server);
                int oprStatus = lcustWishlist.RemoveFromWishlist(lCustLoginID, lShopStockID);
                if (oprStatus == 500)
                    obj = new { Success = 0, Message = "Internal Server Error.", data = string.Empty };
                // obj = new { HTTPStatusCode = "500", UserMessage = "Internal Server Error." };
                else if (oprStatus == 106)
                    obj = new { Success = 1, Message = "The Product, which you want to remove, does not exist in your wishlist", data = new { Staus = 0 } };
                //obj = new { HTTPStatusCode = "304", UserMessage = "ConditionNotMet: The Product, which you want to remove, does not exist in your wishlist." };
                else
                    obj = new { Success = 1, Message = "Product Successfully Removed from wishlist", data = new { Staus = 0 } };
                //obj = new { HTTPStatusCode = "200", UserMessage = "Product Successfully Removed from wishlist" };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }

            return obj;
        }
    }
}
