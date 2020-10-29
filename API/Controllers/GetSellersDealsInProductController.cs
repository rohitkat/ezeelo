//-----------------------------------------------------------------------
// <copyright file="GetSellersDealsInProductController" company="Ezeelo Consumer Services Pvt. Ltd.">
//     Copyright (c) Ezeelo Consumer Services Pvt. Ltd. All rights reserved.
// </copyright>
// <author>Sujata Kullarkar</author>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLayer;
using ModelLayer.Models.ViewModel;
using API.Models;

namespace API.Controllers
{
    public class GetSellersDealsInProductController : ApiController
    {
        /// <summary>
        /// Get Sellers List Deals in product irrespective of city and location
        /// </summary>
        /// <param name="productID">Product ID</param>
        /// <returns></returns>
         [ApiException] 
        // GET api/productvarients/5
             [ValidateModel]
        public IEnumerable<ProductSellersViewModel> GetProductVarientList(long productID)
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            return prod.GetSellersDealsInProduct(productID);
        }
        /// <summary>
         ///Get Sellers List Deals in product in selected city and location
        /// </summary>
        /// <param name="productID">Product ID</param>
        /// <param name="cityID">City ID</param>
        /// <returns></returns>
         [ApiException] 
        [ValidateModel]
        // GET api/productvarients/5/6
         public IEnumerable<ProductSellersViewModel> GetProductVarientList(long productID, long cityID, int? franchiseID=null)////added int? franchiseID for Multiple MCO
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            return prod.GetSellersDealsInProduct(productID, cityID, franchiseID);////added franchiseID
        }
        /// <summary>
         /// Get Sellers List Deals in product in selected city and location
        /// </summary>
        /// <param name="productID">Product ID</param>
        /// <param name="cityID">City ID</param>
        /// <param name="locationID">Location ID</param>
        /// <returns></returns>
         [ApiException]
        [ValidateModel]
        // GET api/productvarients/5/6/5
         public IEnumerable<ProductSellersViewModel> GetProductVarientList(long productID, long cityID, long locationID, int? franchiseID=null)////added params int franchiseID for Multiple MCO
        {
            ProductDetails prod = new ProductDetails(System.Web.HttpContext.Current.Server);
            return prod.GetSellersDealsInProduct(productID, cityID, locationID, franchiseID);////added franchiseID
        }

    }
}
